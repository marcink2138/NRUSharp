using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class StandardFbe : AbstractFbeNode{
        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (IsFrameQueued && IsChannelIdle){
                    yield return Env.Process(PerformCot());
                    yield return Env.TimeoutD(FbeTimes.IdleTime - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                }
                else{
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                }
            }
        }

        private new IEnumerable<Event> PerformInitOffset(){
            yield return Env.Process(base.PerformInitOffset());
            yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
            yield return Env.Process(PerformCca());
        }

        public override NodeType GetNodeType(){
            return NodeType.StandardFbe;
        }
    }
}