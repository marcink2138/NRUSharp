using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class RandomMutingFbe : AbstractFbeNode{
        public int TransmissionPeriodNum{ get; init; }
        public int MutedPeriodNum{ get; init; }
        private int _transmissionPeriodCounter = -1;
        private int _mutedPeriodCounter = -1;

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (_transmissionPeriodCounter != -1){
                    Logger.Debug("{}|Enterning transmission phase", Env.NowD);
                    yield return Env.Process(TransmissionPeriodsPhase());
                }
                else if (_mutedPeriodCounter != -1){
                    Logger.Debug("{}|Enterning muted phase", Env.NowD);
                    yield return Env.Process(MutedPeriodsPhase());
                    if (IsChannelIdle){
                        _transmissionPeriodCounter = SelectRandomNumber(TransmissionPeriodNum);
                        Logger.Debug(
                            "{}|Channel was idle after muted period phase. Selected transmission counter -> {}",
                            Env.NowD,
                            _transmissionPeriodCounter);
                    }
                }
                else{
                    Logger.Debug("{}|Performing FFP without transmission", Env.NowD);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                    if (IsChannelIdle){
                        _transmissionPeriodCounter = SelectRandomNumber(TransmissionPeriodNum);
                        Logger.Debug("{}|Channel was idle. Selected transmission counter -> {}",
                            Env.NowD,
                            _transmissionPeriodCounter);
                    }
                }
            }
        }

        private IEnumerable<Event> TransmissionPeriodsPhase(){
            while (_transmissionPeriodCounter > 0){
                Logger.Debug("{}|Current transmission counter -> {}", Env.NowD, _transmissionPeriodCounter);
                yield return Env.Process(PerformTransmission());
                yield return Env.TimeoutD(FbeTimes.IdleTime - FbeTimes.Cca);
                yield return Env.Process(PerformCca());
                if (_transmissionPeriodCounter == -1 || !IsChannelIdle){
                    Logger.Debug("{}|Exiting transmission phase. transmission counter -> {}, is channel idle -> {}",
                        Env.NowD, _transmissionPeriodCounter, IsChannelIdle);
                    _transmissionPeriodCounter = -1;
                    yield break;
                }

                _transmissionPeriodCounter--;
            }

            _transmissionPeriodCounter = -1;
            _mutedPeriodCounter = SelectRandomNumber(MutedPeriodNum);
            Logger.Debug("{}|Transmission period finished successfully. Selected muted counter -> {}",
                Env.NowD,
                _mutedPeriodCounter);
        }

        private IEnumerable<Event> MutedPeriodsPhase(){
            while (_mutedPeriodCounter > 0){
                Logger.Debug("{}|Current muted counter -> {}", Env.NowD, _mutedPeriodCounter);
                if (_mutedPeriodCounter == 1){
                    Logger.Debug("{}|Performing last muted period. CCA will be performed at the end of FFP", Env.NowD);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                    _mutedPeriodCounter = -1;
                    yield break;
                }

                yield return Env.TimeoutD(FbeTimes.Ffp);
                _mutedPeriodCounter--;
            }
        }

        public override IEnumerable<Event> FinishTransmission(bool isSuccessful, double timeLeft){
            if (isSuccessful){
                SuccessfulTransmission();
                Channel.RemoveFromTransmissionList(this);
                yield break;
            }

            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            FailedTransmission();
            _transmissionPeriodCounter = -1;
            Channel.RemoveFromTransmissionList(this);
        }

        private new IEnumerable<Event> PerformInitOffset(){
            yield return Env.Process(base.PerformInitOffset());
            yield return Env.Process(PerformCca());
            if (IsChannelIdle){
                _transmissionPeriodCounter = SelectRandomNumber(TransmissionPeriodNum);
                Logger.Debug("{}|Channel was idle after init CCA. Selected transmission counter -> {}",
                    Env.NowD,
                    _transmissionPeriodCounter);
            }
        }

        public override void ResetStation(){
            base.ResetStation();
            _mutedPeriodCounter = -1;
            _transmissionPeriodCounter = -1;
        }

        public override StationType GetStationType(){
            return StationType.RandomMutingFbe;
        }
    }
}