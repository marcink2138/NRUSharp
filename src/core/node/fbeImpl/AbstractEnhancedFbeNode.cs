using System.Collections.Generic;
using NRUSharp.core.node.fbeImpl.data;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl{
    public abstract class AbstractEnhancedFbeNode : AbstractFbeNode{
        protected AbstractEnhancedFbeNode(){
            AddCcaCallbacks();
        }

        protected BackoffState BackoffState = BackoffState.NotInitialized;
        public int Q{ get; init; }
        protected int Backoff{ get; set; }
        protected bool IsEnhancedCcaPhase{ get; private set; }

        public abstract override IEnumerable<Event> Start();

        private void AddCcaCallbacks(){
            FbeNodeCallbacks.AddCallback(FbeNodeCallbacks.Type.SuccessfulCca, () => {
                Logger.Debug("Executing Abstract enhanced FBE node callback after successful CCA");
                IsEnhancedCcaPhase = true;
            });
            FbeNodeCallbacks.AddCallback(FbeNodeCallbacks.Type.FailedCca, () => {
                Logger.Debug("Executing Abstract enhanced FBE node callback after failed CCA");
                IsEnhancedCcaPhase = false;
            });
        }

        protected override IEnumerable<Event> PerformCot(){
            foreach (var @event in base.PerformCot()){
                yield return @event;
            }

            IsEnhancedCcaPhase = false;
        }

        protected override void PrepareNodeParams(){
            Logger.Debug("{}|Preparing node params. BackoffState = {}", Env.NowD, BackoffState);
            if (BackoffState == BackoffState.InProcess){
                return;
            }

            SelectBackoff();
        }

        private void SelectBackoff(){
            Backoff = SelectRandomNumber(Q);
            Logger.Info("{}|Selected new backoff= {}", Env.NowD, Backoff);
            BackoffState = BackoffState.InProcess;
        }

        protected IEnumerable<Event> WaitForFrames(){
            yield return Env.TimeoutD(SimulationParams.SimulationTime);
        }

        public override void ResetNode(){
            base.ResetNode();
            Backoff = 0;
            IsEnhancedCcaPhase = false;
            BackoffState = BackoffState.NotInitialized;
        }
    }
}