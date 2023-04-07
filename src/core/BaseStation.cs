using System.Collections.Generic;
using NLog;
using NRUSharp.common.data;
using NRUSharp.common.interfaces;
using SimSharp;

namespace NRUSharp.common{
    public abstract class BaseStation : IStation{
        public readonly StationResults Results;
        public readonly string Name;
        protected readonly Simulation Env;
        protected readonly IChannel Channel;
        protected readonly Logger Logger;
        protected FBETimes FbeTimes;
        protected bool IsChannelIdle;
        public Process TransmissionProcess;
        public Process CcaProcess;
        private readonly StationEventTimes _stationEventTimes;
        private bool _transmissionFailureFlag;
        private bool _ccaFailureFlag;
        protected readonly int Offset;
        private readonly IRngWrapper _rngWrapper;

        protected BaseStation(string name, Simulation env, IChannel channel, FBETimes fbeTimes, int offset,
            IRngWrapper rngWrapper){
            Name = name;
            Env = env;
            Channel = channel;
            Logger = LogManager.GetLogger(Name);
            FbeTimes = fbeTimes;
            Offset = offset;
            _rngWrapper = rngWrapper;
            _stationEventTimes = new StationEventTimes();
            Results = new StationResults();
        }

        public IEnumerable<Event> StartTransmission(){
            Logger.Debug("{}|Starting transmission", Env.NowD);
            _stationEventTimes.TransmissionStart = Env.NowD;
            double transmissionTime;
            if (Env.NowD + FbeTimes.Cot > _stationEventTimes.SimulationTime){
                transmissionTime = _stationEventTimes.SimulationTime - Env.NowD;
                _stationEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
            }
            else{
                transmissionTime = FbeTimes.Cot;
                _stationEventTimes.TransmissionEnd = Env.NowD + transmissionTime;
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
            Results.IncrementAirTime((int)(_stationEventTimes.TransmissionEnd - _stationEventTimes.TransmissionStart));
            Logger.Info("{}|Current successful transmission counter -> {}, current air time -> {}", Env.NowD,
                Results.SuccessfulTransmissions, Results.AirTime);
        }

        public IEnumerable<Event> StartCca(){
            Logger.Debug("{}|CCA - START", Env.NowD);
            _stationEventTimes.CcaStart = Env.NowD;
            _stationEventTimes.CcaEnd = Env.NowD + FbeTimes.Cca;
            Channel.AddToCcaList(this);
            if (Channel.GetTransmissionListSize() > 0){
                _ccaFailureFlag = true;
            }

            yield return Env.TimeoutD(FbeTimes.Cca);
        }

        public (bool isSuccessful, double timeLeft) DeterminateTransmissionStatus(){
            if (Env.ActiveProcess.HandleFault() || _transmissionFailureFlag){
                var transmissionTimeLeft = _stationEventTimes.TransmissionEnd - Env.NowD;
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
                var ccaTimeLeft = _stationEventTimes.CcaEnd - Env.NowD;
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
            return _rngWrapper.GetInt(start, end + 1);
        }
    }
}