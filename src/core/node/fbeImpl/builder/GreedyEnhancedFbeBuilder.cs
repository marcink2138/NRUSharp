using NRUSharp.core.data;
using NRUSharp.core.interfaces;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class GreedyEnhancedFbeBuilder : AbstractFbeStationBuilder{
        private int _q;

        public GreedyEnhancedFbeBuilder WithQ(int q){
            _q = q;
            return this;
        }

        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new GreedyEnhancedFbe{
                Name = Name,
                Env = Env,
                Channel = Channel,
                FbeTimes = fbeTimes,
                RngWrapper = RngWrapper,
                SimulationParams = simulationParams,
                Q = _q
            };
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            _q = 0;
        }
    }
}