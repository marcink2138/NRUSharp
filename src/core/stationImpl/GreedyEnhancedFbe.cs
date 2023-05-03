using System.Collections.Generic;
using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using SimSharp;

namespace NRUSharp.core.stationImpl{
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

        public override StationType GetStationType(){
            return StationType.GreedyEnhancedFbe;
        }

        public GreedyEnhancedFbe(string name, Simulation env, IChannel channel, FbeTimes fbeTimes,
            IRngWrapper rngWrapper, int q, SimulationParams simulationParams) :
            base(name, env, channel, fbeTimes, rngWrapper, q, simulationParams){ }
    }
}