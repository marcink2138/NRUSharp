using System;
using System.Collections.Generic;
using NLog;
using NRUSharp.core.channel;
using NRUSharp.core.data;
using NRUSharp.core.node.fbeImpl.data;
using NRUSharp.core.rngWrapper;
using NRUSharp.core.trafficGenerator;
using NRUSharp.simulationFramework.constants;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public abstract class AbstractFbeNode : INode, IQueueListener<Frame>{
        private readonly string _name;
        private NodeResults Results{ get; set; }

        public string Name{
            get => _name;
            init{
                _name = value;
                Logger = LogManager.GetLogger(LogManagerWrapper.NodeLoggerPrefix + _name) ??
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
        protected bool IsFrameQueued;
        protected FbeNodeCallbacks FbeNodeCallbacks = new();

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
        protected Process FrameWaitingProcess{ get; set; }

        protected AbstractFbeNode(){
            NodeEventTimes = new NodeEventTimes();
            Results = new NodeResults();
            NodeQueue = new NodeQueue<Frame>(10, simulation => new Frame{
                GenerationTime = simulation.NowD,
                Size = FbeTimes.Cot
            }, this);
        }

        private IEnumerable<Event> StartTransmission(){
            Logger.Debug("{}|Starting transmission", Env.NowD);
            NodeEventTimes.TransmissionStart = Env.NowD;
            double transmissionTime = GetFramesFromQueue();
            if (Env.NowD + transmissionTime > SimulationParams.SimulationTime){
                transmissionTime = SimulationParams.SimulationTime - Env.NowD;
                NodeEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
            }
            else{
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

        protected virtual void FailedTransmission(){
            Results.IncrementFailedTransmissions();
            Logger.Info("{}|Current failed transmission counter -> {}", Env.NowD, Results.FailedTransmissions);
            _transmissionFailureFlag = false;
        }

        protected virtual void SuccessfulTransmission(){
            Results.IncrementAirTime((int) (NodeEventTimes.TransmissionEnd - NodeEventTimes.TransmissionStart));
            Results.IncrementSuccessfulTransmissions();
            Logger.Info("{}|Current successful transmission counter -> {}, current air time -> {}", Env.NowD,
                Results.SuccessfulTransmissions, Results.AirTime);
        }

        private IEnumerable<Event> StartCca(){
            Logger.Debug("{}|CCA - START", Env.NowD);
            NodeEventTimes.CcaStart = Env.NowD;
            NodeEventTimes.CcaEnd = Env.NowD + FbeTimes.Cca;
            Channel.AddToCcaList(this);
            if (Channel.GetTransmissionListSize() > 0){
                _ccaFailureFlag = true;
            }

            yield return Env.TimeoutD(FbeTimes.Cca);
        }

        private (bool isSuccessful, double timeLeft) DeterminateTransmissionStatus(){
            if (Env.ActiveProcess.HandleFault() || _transmissionFailureFlag){
                var transmissionTimeLeft = NodeEventTimes.TransmissionEnd - Env.NowD;
                Logger.Debug("{}|Collision detected. Transmission time left: {}", Env.NowD, transmissionTimeLeft);
                return (false, transmissionTimeLeft);
            }

            Logger.Debug("{}|Transmission successful!", Env.NowD);
            SuccessfulTransmission();
            Channel.RemoveFromTransmissionList(this);
            return (true, 0);
        }

        private (bool isSuccessful, double timeLeft) DeterminateCcaStatus(){
            if (Env.ActiveProcess.HandleFault() || _ccaFailureFlag){
                Logger.Debug("{}|CCA Failed -> setting isChannelIdle flag to false", Env.NowD);
                IsChannelIdle = false;
                _ccaFailureFlag = false;
                var ccaTimeLeft = NodeEventTimes.CcaEnd - Env.NowD;
                return (IsChannelIdle, ccaTimeLeft);
            }

            Logger.Debug("{}|CCA Successful -> channel is idle", Env.NowD);
            Channel.RemoveFromCcaList(this);
            IsChannelIdle = true;
            _ccaFailureFlag = false;
            return (IsChannelIdle, 0);
        }

        private IEnumerable<Event> FinishTransmission(bool isSuccessful, double timeLeft){
            if (isSuccessful){
                yield break;
            }

            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            FailedTransmission();
            Channel.RemoveFromTransmissionList(this);
        }

        private IEnumerable<Event> FinishCca(bool isSuccessful, double timeLeft){
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

        protected virtual IEnumerable<Event> PerformCot(){
            Results.ChannelAccessDelay.ChannelAccessed(NodeEventTimes.TransmissionStart);
            NodeEventTimes.CotStart = Env.NowD;
            NodeEventTimes.CotEnd = Env.NowD + FbeTimes.Cot;
            while (true){
                yield return Env.Process(PerformTransmissions());
                if (NodeEventTimes.CotEnd < Env.NowD){
                    break;
                }

                //We have guarantee that queue is empty
                FrameWaitingProcess = Env.Process(StartCotWaitingProcess());
                if (!Env.ActiveProcess.HandleFault()){
                    Logger.Debug("{}|Node was not notified about new queue item. Ending COT", Env.NowD);
                    break;
                }

                Logger.Debug("{}|Node was notified about new item in queue.", Env.NowD);
                //Check if diff is less than 16 us gap
                if (Env.NowD - NodeEventTimes.TransmissionEnd < 16){
                    Logger.Debug("{}|Gap between transmissions is less than 16 us. Performin transmission immiedietly",
                        Env.NowD);
                    yield return Env.Process(PerformTransmissions());
                    break;
                }

                if (Env.NowD + FbeTimes.Cca >= NodeEventTimes.CotEnd){
                    Logger.Debug(
                        "{}|Gap between transmissions is larger than 16 us and cannot start CCA during current COT. Skipping",
                        Env.NowD);
                    yield return Env.TimeoutD(NodeEventTimes.CotEnd - Env.NowD);
                    break;
                }

                Logger.Debug("{}|Performing CCA after 16 us gap", Env.NowD);
                yield return Env.Process(PerformCca(true));
                if (IsChannelIdle){
                    yield return Env.Process(PerformTransmissions());
                }
            }
        }

        private IEnumerable<Event> StartCotWaitingProcess(){
            var waitTime = NodeEventTimes.CotEnd - Env.NowD;
            yield return Env.TimeoutD(waitTime);
        }

        protected IEnumerable<Event> PerformCca(bool innerCotCca = false){
            if (NodeQueue.Count == 0){
                Logger.Debug("{}|Node queue is empty. Skipping CCA and next COT");
                IsFrameQueued = false;
                yield return Env.TimeoutD(FbeTimes.Cca);
                yield break;
            }
            
            if (!innerCotCca){
                IsFrameQueued = true;
                PrepareNodeParams();
            }

            Cca = Env.Process(StartCca());
            yield return Cca;
            var (isSuccessful, timeLeft) = DeterminateCcaStatus();
            yield return Env.Process(FinishCca(isSuccessful, timeLeft));
            if (innerCotCca){
                yield break;
            }

            if (isSuccessful){
                FbeNodeCallbacks.ExecuteCallbacks(FbeNodeCallbacks.Type.SuccessfulCca);
                yield break; 
            }
            FbeNodeCallbacks.ExecuteCallbacks(FbeNodeCallbacks.Type.FailedCca);
        }

        protected virtual void PrepareNodeParams(){
            Logger.Debug("{}|Basic implementation of PrepareNodeParamsMethod. Do nothing...", Env.NowD);
        }

        protected IEnumerable<Event> PerformTransmissions(){
            while (NodeQueue.Count > 0 && Env.NowD < NodeEventTimes.CotEnd){
                Transmission = Env.Process(StartTransmission());
                yield return Transmission;
                var (isSuccessful, timeLeft) = DeterminateTransmissionStatus();
                yield return Env.Process(FinishTransmission(isSuccessful, timeLeft));
            }
        }

        protected virtual IEnumerable<Event> PerformInitOffset(){
            Logger.Info("{}|Performing Initial offset {}", Env.NowD, Offset);
            yield return Env.TimeoutD(Offset);
            NodeQueue.Start(Env);
        }

        protected int SelectRandomNumber(int end, int start = 1){
            return RngWrapper.GetInt(start, end + 1);
        }

        public virtual void ResetNode(){
            Channel.ResetChannel();
            Env = null;
            IsChannelIdle = false;
            Transmission = null;
            Cca = null;
            _ccaFailureFlag = false;
            _transmissionFailureFlag = false;
            Results = new NodeResults();
        }

        public abstract NodeType GetNodeType();

        public List<KeyValuePair<string, object>> FetchResults(){
            return new List<KeyValuePair<string, object>>{
                new(DfColumns.Name, Name),
                new(DfColumns.Airtime, Results.AirTime),
                new(DfColumns.SuccessfulTransmissions, Results.SuccessfulTransmissions),
                new(DfColumns.FailedTransmissions, Results.FailedTransmissions),
                new(DfColumns.Cot, FbeTimes.Cot),
                new(DfColumns.Ffp, FbeTimes.Ffp),
                new(DfColumns.Offset, Offset),
                new(DfColumns.StationVersion, GetNodeType().ToString()),
                new(DfColumns.MeanChannelAccessDelay,
                    Results.GetMeanChannelAccessDelay((int) SimulationParams.SimulationTime))
            };
        }

        public void MountTrafficGenerator(ITrafficGenerator<Frame> trafficGenerator){
            NodeQueue.TrafficGenerator = trafficGenerator;
        }

        public void HandleNewItem(Frame item){
            if (FrameWaitingProcess is not{IsAlive: true, IsOk: true}){
                return;
            }

            FrameWaitingProcess.Interrupt();
        }

        private int GetFramesFromQueue(){
            if (!NodeQueue.TryPeek(out var frame)){
                throw new InvalidOperationException("Trying to perform transmission while queue is empty!");
            }

            int availableTransmissionTime;
            if (Env.NowD + frame.Size > NodeEventTimes.CotEnd){
                availableTransmissionTime = (int) (NodeEventTimes.CotEnd - Env.NowD);
                frame.Size -= availableTransmissionTime;
                return availableTransmissionTime;
            }

            availableTransmissionTime = frame.Size;
            NodeQueue.Dequeue();
            return availableTransmissionTime;
        }
    }
}