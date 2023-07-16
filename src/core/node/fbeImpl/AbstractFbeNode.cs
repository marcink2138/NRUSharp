using System.Collections.Generic;
using NLog;
using NRUSharp.core.channel;
using NRUSharp.core.data;
using NRUSharp.core.rngWrapper;
using NRUSharp.core.trafficGenerator;
using NRUSharp.simulationFramework.constants;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public abstract class AbstractFbeNode : INode{
        private readonly string _name;
        private NodeResults Results{ get; set; }

        public string Name{
            get => _name;
            init{
                _name = value;
                Logger = LogManager.GetLogger(LogManagerWrapper.StationLoggerPrefix + _name) ??
                         LogManager.CreateNullLogger();
            }
        }

        public Simulation Env{ get; set; }
        public IChannel Channel{ get; set; }
        private NodeEventTimes NodeEventTimes{ get; }
        public FbeTimes FbeTimes{ get; init; }
        public IRngWrapper RngWrapper{ get; init; }

        public int QueueCapacity{
            get => NodeQueue.MaxSize;
            init => NodeQueue.MaxSize = value;
        }

        protected readonly Logger Logger;
        protected bool IsChannelIdle;

        private int Offset{
            get{
                if (RngWrapper == null || SimulationParams == null){
                    return 0;
                }

                if (SimulationParams.OffsetRangeBottom >= SimulationParams.OffsetRangeTop){
                    return SimulationParams.OffsetRangeBottom;
                }

                return RngWrapper.GetInt(SimulationParams.OffsetRangeBottom, SimulationParams.OffsetRangeTop);
            }
        }

        public SimulationParams SimulationParams{ get; init; }
        private bool _transmissionFailureFlag;
        private bool _ccaFailureFlag;
        protected readonly NodeQueue<Frame> NodeQueue;
        public Process Transmission{ get; set; }
        public Process Cca{ get; set; }

        protected AbstractFbeNode(){
            NodeEventTimes = new NodeEventTimes();
            Results = new NodeResults();
            NodeQueue = new NodeQueue<Frame>(10, simulation => new Frame{
                GenerationTime = simulation.NowD,
                Retries = 0
            });
        }

        public IEnumerable<Event> StartTransmission(){
            Logger.Debug("{}|Starting transmission", Env.NowD);
            NodeEventTimes.TransmissionStart = Env.NowD;
            Results.ChannelAccessDelay.TransmissionStarted(NodeEventTimes.TransmissionStart);
            double transmissionTime;
            if (Env.NowD + FbeTimes.Cot > SimulationParams.SimulationTime){
                transmissionTime = SimulationParams.SimulationTime - Env.NowD;
                NodeEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
            }
            else{
                transmissionTime = FbeTimes.Cot;
                NodeEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
            }

            Channel.InterruptCca();
            Channel.InterruptOnGoingTransmissions();
            if (Channel.GetTransmissionListSize() > 0){
                Logger.Debug("{}|There is ongoing transmission in the Channel!", Env.NowD);
                _transmissionFailureFlag = true;
            }

            Channel.AddToTransmissionList(this);
            yield return Env.TimeoutD(transmissionTime);
        }

        public abstract IEnumerable<Event> Start();

        public virtual void FailedTransmission(){
            Results.IncrementFailedTransmissions();
            Logger.Info("{}|Current failed transmission counter -> {}", Env.NowD, Results.FailedTransmissions);
            _transmissionFailureFlag = false;
        }

        public virtual void SuccessfulTransmission(){
            Results.IncrementAirTime((int) (NodeEventTimes.TransmissionEnd - NodeEventTimes.TransmissionStart));
            Results.IncrementSuccessfulTransmissions();
            Results.ChannelAccessDelay.Success();
            Logger.Info("{}|Current successful transmission counter -> {}, current air time -> {}", Env.NowD,
                Results.SuccessfulTransmissions, Results.AirTime);
        }

        public IEnumerable<Event> StartCca(){
            Logger.Debug("{}|CCA - START", Env.NowD);
            NodeEventTimes.CcaStart = Env.NowD;
            NodeEventTimes.CcaEnd = Env.NowD + FbeTimes.Cca;
            Channel.AddToCcaList(this);
            if (Channel.GetTransmissionListSize() > 0){
                _ccaFailureFlag = true;
            }

            yield return Env.TimeoutD(FbeTimes.Cca);
        }

        public (bool isSuccessful, double timeLeft) DeterminateTransmissionStatus(){
            if (Env.ActiveProcess.HandleFault() || _transmissionFailureFlag){
                var transmissionTimeLeft = NodeEventTimes.TransmissionEnd - Env.NowD;
                Logger.Debug("{}|Collision detected. Transmission time left: {}", Env.NowD, transmissionTimeLeft);
                return (false, transmissionTimeLeft);
            }

            Logger.Debug("{}|Transmission successful!", Env.NowD);
            return (true, 0);
        }

        public (bool isSuccessful, double timeLeft) DeterminateCcaStatus(){
            if (Env.ActiveProcess.HandleFault() || _ccaFailureFlag){
                Logger.Debug("{}|CCA Failed -> setting isChannelIdle flag to false", Env.NowD);
                IsChannelIdle = false;
                _ccaFailureFlag = false;
                var ccaTimeLeft = NodeEventTimes.CcaEnd - Env.NowD;
                return (IsChannelIdle, ccaTimeLeft);
            }

            Logger.Debug("{}|CCA Successful -> channel is idle", Env.NowD);
            IsChannelIdle = true;
            _ccaFailureFlag = false;
            return (IsChannelIdle, 0);
        }

        public virtual IEnumerable<Event> FinishTransmission(bool isSuccessful, double timeLeft){
            if (isSuccessful){
                SuccessfulTransmission();
                Channel.RemoveFromTransmissionList(this);
                yield break;
            }

            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            FailedTransmission();
            Channel.RemoveFromTransmissionList(this);
        }

        public virtual IEnumerable<Event> FinishCca(bool isSuccessful, double timeLeft){
            if (isSuccessful){
                Logger.Debug("{}|Channel sensed as idle!", Env.NowD);
                Channel.RemoveFromCcaList(this);
                yield break;
            }

            Logger.Debug("{}|Channel sensed as taken!", Env.NowD);
            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            Channel.RemoveFromCcaList(this);
        }

        public IEnumerable<Event> PerformCca(){
            Cca = Env.Process(StartCca());
            yield return Cca;
            var (isSuccessful, timeLeft) = DeterminateCcaStatus();
            yield return Env.Process(FinishCca(isSuccessful, timeLeft));
        }

        public IEnumerable<Event> PerformTransmission(){
            Transmission = Env.Process(StartTransmission());
            yield return Transmission;
            var (isSuccessful, timeLeft) = DeterminateTransmissionStatus();
            yield return Env.Process(FinishTransmission(isSuccessful, timeLeft));
        }

        public virtual IEnumerable<Event> PerformInitOffset(){
            Logger.Info("{}|Performing Initial offset {}", Env.NowD, Offset);
            yield return Env.TimeoutD(Offset);
            NodeQueue.Start(Env);
        }

        protected int SelectRandomNumber(int end, int start = 1){
            return RngWrapper.GetInt(start, end + 1);
        }

        public virtual void ResetStation(){
            Channel.ResetChannel();
            Env = null;
            IsChannelIdle = false;
            Transmission = null;
            Cca = null;
            _ccaFailureFlag = false;
            _transmissionFailureFlag = false;
            Results = new NodeResults();
        }

        public abstract StationType GetStationType();

        public List<KeyValuePair<string, object>> FetchResults(){
            return new List<KeyValuePair<string, object>>(){
                new(DfColumns.Name, Name),
                new(DfColumns.Airtime, Results.AirTime),
                new(DfColumns.SuccessfulTransmissions, Results.SuccessfulTransmissions),
                new(DfColumns.FailedTransmissions, Results.FailedTransmissions),
                new(DfColumns.Cot, FbeTimes.Cot),
                new(DfColumns.Ffp, FbeTimes.Ffp),
                new(DfColumns.Offset, Offset),
                new(DfColumns.StationVersion, GetStationType().ToString()),
                new(DfColumns.MeanChannelAccessDelay, Results.MeanChannelAccessDelay)
            };
        }

        public void MountTrafficGenerator(ITrafficGenerator<Frame> trafficGenerator){
            NodeQueue.TrafficGenerator = trafficGenerator;
        }
    }
}