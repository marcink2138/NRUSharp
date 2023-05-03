using System.Collections.Generic;
using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using SimSharp;

namespace NRUSharp.core{
    public abstract class BaseEnhancedFbeStation : BaseStation{
        protected readonly int Q;
        protected int Backoff;
        protected bool IsEnhancedCcaPhase;

        protected BaseEnhancedFbeStation(string name, Simulation env, IChannel channel, FbeTimes fbeTimes,
            IRngWrapper rngWrapper, int q, SimulationParams simulationParams) : base(name, env, channel, fbeTimes,
            rngWrapper, simulationParams){
            Q = q;
        }

        public abstract override IEnumerable<Event> Start();

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
            Channel.RemoveFromTransmissionList(this);
        }

        public override IEnumerable<Event> FinishCca(bool isSuccessful, double timeLeft){
            if (isSuccessful){
                Logger.Debug("{}|Channel sensed as idle!", Env.NowD);
                Channel.RemoveFromCcaList(this);
                IsEnhancedCcaPhase = true;
                yield break;
            }

            Logger.Debug("{}|Channel sensed as taken!", Env.NowD);
            if (timeLeft > 0){
                yield return Env.TimeoutD(timeLeft);
            }

            IsEnhancedCcaPhase = false;
            Channel.RemoveFromCcaList(this);
        }

        private new void FailedTransmission(){
            base.FailedTransmission();
            Backoff = SelectRandomNumber(Q);
            Logger.Info("{}|Selected new backoff= {}", Env.NowD, Backoff);
            IsEnhancedCcaPhase = false;
        }

        private new void SuccessfulTransmission(){
            base.SuccessfulTransmission();
            Backoff = SelectRandomNumber(Q);
            Logger.Info("{}|Selected new backoff= {}", Env.NowD, Backoff);
            IsEnhancedCcaPhase = false;
        }
    }
}