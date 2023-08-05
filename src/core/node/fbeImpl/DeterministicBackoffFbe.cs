using System.Collections.Generic;
using NRUSharp.core.node.fbeImpl.data;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class DeterministicBackoffFbe : AbstractFbeNode{

        private BackoffState _backoffState = BackoffState.NotInitialized;
        private int _backoffCounter;
        private int _interruptCounter;
        private int _retransmissionCounter;
        private double _monitorEnd;
        public int MaxRetransmissionNum{ get; init; }
        public int Threshold{ get; init; }
        public int InitialBackoff{ get; init; }
        private bool _monitorFailureFlag;

        public DeterministicBackoffFbe(){
            FbeNodeCallbacks.AddCallback(FbeNodeCallbacks.Type.SuccessfulCca, () => {
                Logger.Debug("{}|Decrementing backoff: {} -> {}", Env.NowD, _backoffCounter, _backoffCounter - 1);
                _backoffCounter--;
            });
        }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (_backoffCounter == 0){
                    _backoffState = BackoffState.Finished;
                    yield return Env.Process(PerformCot());
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

        public override NodeType GetNodeType(){
            return NodeType.DeterministicBackoffFbe;
        }

        protected override void FailedTransmission(){
            base.FailedTransmission();
            if (_retransmissionCounter < MaxRetransmissionNum){
                Logger.Debug("{}|Incrementing retransmission counter: {} -> {}", Env.NowD, _retransmissionCounter,
                    _retransmissionCounter + 1);
                _retransmissionCounter++;
            }
            else{
                Logger.Debug("{}|Retransmission counter is higher than max retransmision number: {} > {}, setting to 0",
                    Env.NowD,
                    _retransmissionCounter,
                    MaxRetransmissionNum);
                _retransmissionCounter++;
                _retransmissionCounter = 0;
            }
        }

        protected override void SuccessfulTransmission(){
            base.SuccessfulTransmission();
            _retransmissionCounter = 0;
        }

        protected override IEnumerable<Event> PerformInitOffset(){
            yield return Env.Process(base.PerformInitOffset());
            yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
            yield return Env.Process(PerformCca());
        }

        private IEnumerable<Event> PerformChannelMonitoring(){
            Cca = Env.Process(StartChannelMonitor());
            yield return Cca;
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
            switch (_backoffState){
                case BackoffState.NotInitialized:
                    _backoffCounter = InitialBackoff;
                    _backoffState = BackoffState.InProcess;
                    return;
                case BackoffState.InProcess:
                    return;
            }

            _backoffState = BackoffState.InProcess;
            if (_retransmissionCounter % MaxRetransmissionNum < Threshold){
                _backoffCounter = InitialBackoff + _interruptCounter;
                Logger.Debug("{}|r % m < t -> TRUE, selected new backoff {}", Env.NowD, _backoffCounter);
                _interruptCounter = 0;
            }
            else{
                _backoffCounter = RngWrapper.GetInt(1, MaxRetransmissionNum);
                Logger.Debug("{}|r % m < t -> FALSE, selected new backoff {}", Env.NowD, _backoffCounter);
            }
        }

        protected override void PrepareNodeParams(){
            SelectBackoff();
        }

        public override void ResetNode(){
            base.ResetNode();
            _retransmissionCounter = 0;
            _interruptCounter = 0;
            _backoffCounter = 0;
            _monitorEnd = 0;
            _monitorFailureFlag = false;
            _backoffState = BackoffState.NotInitialized;
        }
    }
}