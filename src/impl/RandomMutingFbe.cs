using System.Collections.Generic;
using NRUSharp.common;
using SimSharp;

namespace NRUSharp.impl{
    public class RandomMutingFbe : BaseStation{
        private readonly int _transmissionPeriodNum;
        private readonly int _mutedPeriodNum;
        private int _transmissionPeriodCounter;
        private int _mutedPeriodCounter;

        public RandomMutingFbe(string name, Simulation env, Channel channel, FBETimes fbeTimes, int offset,
            int transmissionPeriodNum, int mutedPeriodNum) : base(name, env, channel, fbeTimes, offset){
            _transmissionPeriodNum = transmissionPeriodNum;
            _mutedPeriodNum = mutedPeriodNum;
            _transmissionPeriodCounter = -1;
            _mutedPeriodCounter = -1;
        }

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
                        _transmissionPeriodCounter = SelectRandomNumber(_transmissionPeriodNum);
                        Logger.Debug("Channel was idle after muted period phase. Selected transmission counter -> {}",
                            _transmissionPeriodCounter);
                    }
                }
                else{
                    Logger.Debug("{}|Performing FFP without transmission", Env.NowD);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                    if (IsChannelIdle){
                        _transmissionPeriodCounter = SelectRandomNumber(_transmissionPeriodNum);
                        Logger.Debug("Channel was idle. Selected transmission counter -> {}",
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
            _mutedPeriodCounter = SelectRandomNumber(_mutedPeriodNum);
            Logger.Debug("Transmission period finished successfully. Selected muted counter -> {}",
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
            yield return Env.TimeoutD(Offset);
            yield return Env.Process(PerformCca());
            if (IsChannelIdle){
                _transmissionPeriodCounter = SelectRandomNumber(_transmissionPeriodNum);
                Logger.Debug("Channel was idle after init CCA. Selected transmission counter -> {}", _transmissionPeriodCounter);
            }
        }
    }
}