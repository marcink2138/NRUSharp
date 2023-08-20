using System.Collections.Generic;
using NRUSharp.core.node.fbeImpl.data;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class GreedyEnhancedFbe : AbstractEnhancedFbeNode{
        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (NodeQueue.Count == 0){
                    FrameWaitingProcess = Env.Process(WaitForFrames());
                    if (Env.ActiveProcess.HandleFault()){
                        Logger.Debug("{}|Node was notified about new queue item. Starting transmission phase", Env.NowD);
                    }
                    break;
                }
                if (Backoff == 0 && BackoffState == BackoffState.InProcess){
                    BackoffState = BackoffState.Finished;
                    Logger.Debug("{}|Backoff = 0. Starting transmission");
                    yield return Env.Process(PerformCot());
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

        public override NodeType GetNodeType(){
            return NodeType.GreedyEnhancedFbe;
        }
    }
}