using System.Collections.Generic;
using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using SimSharp;

namespace NRUSharp.core.stationImpl{
    public class DeterministicBackoffFbe : BaseStation{
        private int _backoffCounter;
        private int _interruptCounter;
        private int _retransmissionCounter;
        private double _monitorEnd;
        private readonly int _maxRetransmissionNum;
        private readonly int _threshold;
        private readonly int _initialBackoff;
        private bool _monitorFailureFlag;

        public DeterministicBackoffFbe(string name, Simulation env, IChannel channel, FbeTimes fbeTimes,
            IRngWrapper rngWrapper, SimulationParams simulationParams, int initialBackoff, int threshold,
            int maxRetransmissionNum) : base(name, env, channel, fbeTimes, rngWrapper,
            simulationParams){
            _initialBackoff = initialBackoff;
            _threshold = threshold;
            _maxRetransmissionNum = maxRetransmissionNum;
        }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (_backoffCounter == 0){
                    yield return Env.Process(PerformTransmission());
                    yield return Env.TimeoutD(FbeTimes.IdleTime - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                }
                else{
                    yield return Env.Process(PerformChannelMonitoring());
                    yield return Env.TimeoutD(FbeTimes.IdleTime - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                }
            }
        }

        public override StationType GetStationType(){
            return StationType.DeterministicBackoffFbe;
        }

        public override void FailedTransmission(){
            base.FailedTransmission();
            if (_retransmissionCounter < _maxRetransmissionNum){
                Logger.Debug("{}|Incrementing retransmission counter: {} -> {}", Env.NowD, _retransmissionCounter,
                    _retransmissionCounter + 1);
                _retransmissionCounter++;
            }
            else{
                Logger.Debug("{}|Retransmission counter is higher than max retransmision number: {} > {}, setting to 0",
                    Env.NowD,
                    _retransmissionCounter,
                    _maxRetransmissionNum);
                _retransmissionCounter++;
                _retransmissionCounter = 0;
            }
        }

        public override void SuccessfulTransmission(){
            base.SuccessfulTransmission();
            _retransmissionCounter = 0;
        }

        public override IEnumerable<Event> PerformInitOffset(){
            _backoffCounter = _initialBackoff;
            return base.PerformInitOffset();
        }

        public override IEnumerable<Event> FinishTransmission(bool isSuccessful, double timeLeft){
            foreach (var @event in base.FinishTransmission(isSuccessful, timeLeft)){
                yield return @event;
            }

            SelectBackoff();
        }

        public override IEnumerable<Event> FinishCca(bool isSuccessful, double timeLeft){
            foreach (var @event in base.FinishCca(isSuccessful, timeLeft)){
                yield return @event;
            }

            if (!isSuccessful) yield break;
            Logger.Debug("{}|Decrementing backoff: {} -> {}", Env.NowD, _backoffCounter, _backoffCounter - 1);
            _backoffCounter--;
        }

        private IEnumerable<Event> PerformChannelMonitoring(){
            CcaProcess = Env.Process(StartChannelMonitor());
            yield return CcaProcess;
            var timeLeft = DeterminateMonitorStatus();
            yield return Env.Process(FinishChannelMonitor(timeLeft));
        }

        private IEnumerable<Event> StartChannelMonitor(){
            Logger.Debug("{}|Channel monitoring - START", Env.NowD);
            _monitorEnd = Env.NowD + FbeTimes.Cot;
            Channel.AddToCcaList(this);
            if (Channel.GetTransmissionListSize() > 0){
                _monitorFailureFlag = true;
            }

            yield return Env.TimeoutD(FbeTimes.Cot);
        }

        private double DeterminateMonitorStatus(){
            if (Env.ActiveProcess.HandleFault() || _monitorFailureFlag){
                var timeLeft = _monitorEnd - Env.NowD;
                Logger.Debug(
                    "{}|Monitor failed. Transmission time left: {}. Incrementing interruption counter {} -> {}",
                    Env.NowD, timeLeft, _interruptCounter, _interruptCounter + 1);
                _monitorFailureFlag = false;
                _interruptCounter++;
                return timeLeft;
            }

            Logger.Debug("{}|Successful channel monitoring", Env.NowD);
            _monitorFailureFlag = false;
            return 0;
        }

        private IEnumerable<Event> FinishChannelMonitor(double timeLeft){
            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            Channel.RemoveFromCcaList(this);
        }

        private void SelectBackoff(){
            if (_retransmissionCounter % _maxRetransmissionNum < _threshold){
                _backoffCounter = _initialBackoff + _interruptCounter;
                Logger.Debug("{}|r % m < t -> TRUE, selected new backoff {}", Env.NowD, _backoffCounter);
                _interruptCounter = 0;
            }
            else{
                _backoffCounter = RngWrapper.GetInt(1, _maxRetransmissionNum);
                Logger.Debug("{}|r % m < t -> FALSE, selected new backoff {}", Env.NowD, _backoffCounter);
            }
        }

        public override void ResetStation(){
            base.ResetStation();
            _retransmissionCounter = 0;
            _interruptCounter = 0;
            _backoffCounter = 0;
            _monitorEnd = 0;
            _monitorFailureFlag = false;
        }
    }
}