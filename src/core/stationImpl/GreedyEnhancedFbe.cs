using System.Collections.Generic;
using NRUSharp.common;
using NRUSharp.common.data;
using NRUSharp.common.interfaces;
using SimSharp;

namespace NRUSharp.impl{
    public class GreedyEnhancedFbe : BaseEnhancedFbeStation{
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
                        Logger.Debug("{}|ECCA failure. Next step -> ICCA", Env.NowD);
                    }
                }
                else{
                    Logger.Debug("{}| Performing ICCA", Env.NowD);
                    yield return Env.Process(PerformCca());
                }
            }
        }

        public GreedyEnhancedFbe(string name, Simulation env, IChannel channel, FBETimes fbeTimes, int offset, IRngWrapper rngWrapper,int q) :
            base(name, env, channel, fbeTimes, offset, rngWrapper,q){ }
    }
}