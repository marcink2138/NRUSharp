using System.Collections.Generic;
using NRUSharp.core.node.fbeImpl.data;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class EnhancedFbe : AbstractEnhancedFbeNode{
        public bool IsBitrFbe{ get; init; }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (NodeQueue.Count == 0){
                    FrameWaitingProcess = Env.Process(WaitForFrames());
                    if (Env.ActiveProcess.HandleFault()){
                        Logger.Debug("{}|Node was not notified about new queue item. Starting transmission phase",
                            Env.NowD);
                    }

                    break;
                }

                if (Backoff == 0 && BackoffState == BackoffState.InProcess){
                    BackoffState = BackoffState.Finished;
                    Logger.Debug("{}|Backoff = 0. Starting transmission", Env.NowD);
                    yield return Env.Process(PerformCot());
                    var timeTillNextCca = FbeTimes.Ffp - FbeTimes.Cot - FbeTimes.Cca;
                    Logger.Debug("{}|Waiting for next Cca after transmission. Next Cca in {}", Env.NowD,
                        timeTillNextCca);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cot - FbeTimes.Cca);
                }
                else if (IsEnhancedCcaPhase){
                    Logger.Debug("{}| Performing ECCA", Env.NowD);
                    yield return Env.Process(PerformCca(true));
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

        public override NodeType GetNodeType(){
            return IsBitrFbe ? NodeType.BitrFbe : NodeType.EnhancedFbe;
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