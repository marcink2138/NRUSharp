using System.Collections.Generic;
using NLog;
using NRUSharp.common.data;
using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.constants;
using SimSharp;

namespace NRUSharp.core{
    public abstract class BaseStation : IStation{
        public StationResults Results{ get; set; }
        public string Name{ get; set; }
        public Simulation Env{ get; set; }
        public IChannel Channel{ get; set; }
        public StationEventTimes StationEventTimes{ get; set; }
        public FbeTimes FbeTimes{ get; set; }
        public IRngWrapper RngWrapper{ get; set; }
        protected readonly Logger Logger;
        protected bool IsChannelIdle;
        protected readonly int Offset;
        private bool _transmissionFailureFlag;
        private bool _ccaFailureFlag;
        public Process TransmissionProcess;
        public Process CcaProcess;

        protected BaseStation(string name, Simulation env, IChannel channel, FbeTimes fbeTimes, int offset,
            IRngWrapper rngWrapper, int simulationTime) : this(){
            Name = name;
            Env = env;
            Channel = channel;
            Logger = LogManager.GetLogger(Name) ?? LogManager.CreateNullLogger();
            StationEventTimes = new StationEventTimes{
                SimulationTime = simulationTime
            };
            FbeTimes = fbeTimes;
            Offset = offset;
            RngWrapper = rngWrapper;
        }

        protected BaseStation(){
            StationEventTimes = new StationEventTimes();
            Results = new StationResults();
        }

        public IEnumerable<Event> StartTransmission(){
            Logger.Debug("{}|Starting transmission", Env.NowD);
            StationEventTimes.TransmissionStart = Env.NowD;
            double transmissionTime;
            if (Env.NowD + FbeTimes.Cot > StationEventTimes.SimulationTime){
                transmissionTime = StationEventTimes.SimulationTime - Env.NowD;
                StationEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
            }
            else{
                transmissionTime = FbeTimes.Cot;
                StationEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
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

        public void FailedTransmission(){
            Results.IncrementFailedTransmissions();
            Logger.Info("{}|Current failed transmission counter -> {}", Env.NowD, Results.FailedTransmissions);
            _transmissionFailureFlag = false;
        }

        public void SuccessfulTransmission(){
            Results.IncrementSuccessfulTransmissions();
            Results.IncrementAirTime((int) (StationEventTimes.TransmissionEnd - StationEventTimes.TransmissionStart));
            Logger.Info("{}|Current successful transmission counter -> {}, current air time -> {}", Env.NowD,
                Results.SuccessfulTransmissions, Results.AirTime);
        }

        public IEnumerable<Event> StartCca(){
            Logger.Debug("{}|CCA - START", Env.NowD);
            StationEventTimes.CcaStart = Env.NowD;
            StationEventTimes.CcaEnd = Env.NowD + FbeTimes.Cca;
            Channel.AddToCcaList(this);
            if (Channel.GetTransmissionListSize() > 0){
                _ccaFailureFlag = true;
            }

            yield return Env.TimeoutD(FbeTimes.Cca);
        }

        public (bool isSuccessful, double timeLeft) DeterminateTransmissionStatus(){
            if (Env.ActiveProcess.HandleFault() || _transmissionFailureFlag){
                var transmissionTimeLeft = StationEventTimes.TransmissionEnd - Env.NowD;
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
                var ccaTimeLeft = StationEventTimes.CcaEnd - Env.NowD;
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
            CcaProcess = Env.Process(StartCca());
            yield return CcaProcess;
            var (isSuccessful, timeLeft) = DeterminateCcaStatus();
            yield return Env.Process(FinishCca(isSuccessful, timeLeft));
        }

        public IEnumerable<Event> PerformTransmission(){
            TransmissionProcess = Env.Process(StartTransmission());
            yield return TransmissionProcess;
            var (isSuccessful, timeLeft) = DeterminateTransmissionStatus();
            yield return Env.Process(FinishTransmission(isSuccessful, timeLeft));
        }

        public IEnumerable<Event> PerformInitOffset(){
            yield return Env.TimeoutD(Offset);
        }

        protected int SelectRandomNumber(int end, int start = 1){
            return RngWrapper.GetInt(start, end + 1);
        }

        public virtual void ResetStation(){
            Channel.ResetChannel();
            Env = null;
            IsChannelIdle = false;
            TransmissionProcess = null;
            CcaProcess = null;
            _ccaFailureFlag = false;
            _transmissionFailureFlag = false;
            Results = new StationResults();
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
                new(DfColumns.StationVersion, GetStationType().ToString())
            };
        }

        public void SetSimulationEnvironment(Simulation simulation){
            Env = simulation;
        }

        public void SetChannel(IChannel channel){
            Channel = channel;
        }
    }
}