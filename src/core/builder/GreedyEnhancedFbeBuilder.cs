using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class GreedyEnhancedFbeBuilder : AbstractFbeStationBuilder{
        protected int Q;

        public GreedyEnhancedFbeBuilder WithQ(int q){
            Q = q;
            return this;
        }

        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var station = new GreedyEnhancedFbe(Name, Env, Channel, fbeTimes, Offset, RngWrapper, Q,
                SimulationTime);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            Q = 0;
        }
    }
}