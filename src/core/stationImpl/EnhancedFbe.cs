using System.Collections.Generic;
using NRUSharp.common;
using NRUSharp.common.data;
using NRUSharp.common.interfaces;
using SimSharp;

namespace NRUSharp.impl{
    public class EnhancedFbe : BaseEnhancedFbeStation{
        private readonly bool _isBitrFbe;

        public EnhancedFbe(string name, Simulation env, IChannel channel, FBETimes fbeTimes, int offset, IRngWrapper rngWrapper,int q,
            bool isBitrFbe) : base(
            name, env, channel, fbeTimes, offset, rngWrapper, q){
            _isBitrFbe = isBitrFbe;
        }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            Backoff = SelectRandomNumber(Q);
            Logger.Info("{}|Selected init backoff -> {}", Env.NowD, Backoff);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (Backoff == 0){
                    Logger.Debug("{}|Backoff = 0. Starting transmission");
                    yield return Env.Process(PerformTransmission());
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cot - FbeTimes.Cca);
                }
                else if (IsEnhancedCcaPhase){
                    Logger.Debug("{}| Performing ECCA", Env.NowD);
                    yield return Env.Process(PerformCca());
                    if (IsChannelIdle){
                        Logger.Debug("{}|Decrementing ECCA backoff {} -> {}", Env.NowD, Backoff, Backoff - 1);
                        Backoff--;
                    }
                    else{
                        var timeTillNextCca = CalculateTimeTillNextCca();
                        yield return Env.TimeoutD(timeTillNextCca);
                    }
                }
                else{
                    Logger.Debug("{}| Performing ICCA", Env.NowD);
                    yield return Env.Process(PerformCca());
                    if (!IsChannelIdle){
                        var timeTillNextCca = FbeTimes.Ffp - FbeTimes.Cca;
                        Logger.Debug("{}|Waiting till next CCA ({})", Env.NowD, timeTillNextCca);
                        yield return Env.TimeoutD(timeTillNextCca);
                    }
                }
            }
        }

        private int CalculateTimeTillNextCca(){
            int timeTillNextCca;
            if (_isBitrFbe){
                timeTillNextCca = FbeTimes.Cot;
                Logger.Debug("{}|[BitrFBE]Exiting enhanced cca phase with failure. Waiting till next CCA ({})",
                    Env.NowD, timeTillNextCca);
            }
            else{
                timeTillNextCca = FbeTimes.Ffp - FbeTimes.Cca;
                Logger.Debug("{}|[BitrFBE]Exiting enhanced cca phase with failure. Waiting till next CCA ({})",
                    Env.NowD, timeTillNextCca);
            }

            return timeTillNextCca;
        }
    }
}