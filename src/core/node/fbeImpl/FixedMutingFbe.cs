﻿using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public class FixedMutingFbe : AbstractFbeNode{
        private int _mutedPeriodCounter;
        public int MutedPeriods{ get; init; }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                if (IsChannelIdle && _mutedPeriodCounter == 0 && IsFrameQueued){
                    yield return Env.Process(PerformCot());
                    yield return Env.TimeoutD(FbeTimes.IdleTime);
                    _mutedPeriodCounter = MutedPeriods;
                }
                else{
                    yield return Env.Process(HandleMutedPeriod());
                }
            }
        }

        private IEnumerable<Event> HandleMutedPeriod(){
            switch (_mutedPeriodCounter){
                case 1:
                    Logger.Debug("{}|Performing last muted period. CCA will be performed", Env.NowD);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                    _mutedPeriodCounter--;
                    break;
                case 0:
                    Logger.Debug(
                        "{}|Muted periods counter equals 0 but channel was not idle. CCA will be performed at the end of FFP",
                        Env.NowD);
                    yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
                    yield return Env.Process(PerformCca());
                    break;
                default:
                    yield return Env.TimeoutD(FbeTimes.Ffp);
                    Logger.Debug("{}|Decrementing mutedPeriodCounter {} -> {}", Env.NowD, _mutedPeriodCounter,
                        _mutedPeriodCounter - 1);
                    _mutedPeriodCounter--;
                    break;
            }
        }

        public override void ResetNode(){
            base.ResetNode();
            _mutedPeriodCounter = 0;
        }

        protected override IEnumerable<Event> PerformInitOffset(){
            yield return Env.Process(base.PerformInitOffset());
            yield return Env.TimeoutD(FbeTimes.Ffp - FbeTimes.Cca);
            yield return Env.Process(PerformCca());
        }

        public override NodeType GetNodeType(){
            return NodeType.FixedMutingFbe;
        }
    }
}