using System.Collections.Generic;
using NRUSharp.core.data;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class EnhancedFbe : BaseEnhancedFbeNode{
        public bool IsBitrFbe { get; init; }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            Backoff = SelectRandomNumber(Q);
            Logger.Info("{}|Selected init backoff -> {}", Env.NowD, Backoff);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (Backoff == 0){
                    Logger.Debug("{}|Backoff = 0. Starting transmission", Env.NowD);
                    yield return Env.Process(PerformTransmission());
                    var timeTillNextCca = FbeTimes.Ffp - FbeTimes.Cot - FbeTimes.Cca;
                    Logger.Debug("{}|Waiting for next Cca after transmission. Next Cca in {}", Env.NowD,
                        timeTillNextCca);
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
                        var timeTillNextCca = CalculateTimeTillNextCca();
                        yield return Env.TimeoutD(timeTillNextCca);
                    }
                }
            }
        }

        public override StationType GetStationType(){
            return IsBitrFbe ? StationType.BitrFbe : StationType.EnhancedFbe;
        }

        private int CalculateTimeTillNextCca(){
            int timeTillNextCca;
            if (IsBitrFbe){
                timeTillNextCca = FbeTimes.Cot;
                Logger.Debug("{}|[BitrFBE]Exiting enhanced cca phase with failure. Waiting till next CCA ({})",
                    Env.NowD, timeTillNextCca);
            }
            else{
                timeTillNextCca = FbeTimes.Ffp - FbeTimes.Cca;
                Logger.Debug("{}|Exiting enhanced cca phase with failure. Waiting till next CCA ({})",
                    Env.NowD, timeTillNextCca);
            }

            return timeTillNextCca;
        }
    }
}