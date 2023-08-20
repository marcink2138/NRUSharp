using NRUSharp.core.data;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class EnhancedFbeBuilder : AbstractFbeStationBuilder{
        private bool _bitr;
        private int _q;

        public EnhancedFbeBuilder WithQ(int q){
            _q = q;
            return this;
        }

        public EnhancedFbeBuilder IsBitr(bool isBitr){
            _bitr = isBitr;
            return this;
        }


        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new EnhancedFbe{
                Name = Name,
                Env = Env,
                Channel = Channel,
                FbeTimes = fbeTimes,
                RngWrapper = RngWrapper,
                Q = _q,
                IsBitrFbe = _bitr,
                SimulationParams = simulationParams
            };
            station.MountTrafficGenerator(TrafficGenerator);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            _bitr = false;
            _q = 0;
        }
    }
}