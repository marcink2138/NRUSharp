using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class EnhancedFbeBuilder : AbstractFbeStationBuilder{
        protected bool Bitr;
        protected int Q;

        public EnhancedFbeBuilder WithQ(int q){
            Q = q;
            return this;
        }

        public EnhancedFbeBuilder IsBitr(bool isBitr){
            Bitr = isBitr;
            return this;
        }


        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new EnhancedFbe(Name, Env, Channel, fbeTimes, RngWrapper, Q,
                Bitr, simulationParams);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            Bitr = false;
            Q = 0;
        }
    }
}