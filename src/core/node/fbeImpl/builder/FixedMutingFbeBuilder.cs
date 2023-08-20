using NRUSharp.core.data;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class FixedMutingFbeBuilder : AbstractFbeStationBuilder{
        private int _mutedPeriods;

        public FixedMutingFbeBuilder WithMutedPeriods(int mutedPeriods){
            _mutedPeriods = mutedPeriods;
            return this;
        }

        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new FixedMutingFbe{
                Name = Name,
                Env = Env,
                Channel = Channel,
                FbeTimes = fbeTimes,
                RngWrapper = RngWrapper,
                SimulationParams = simulationParams,
                MutedPeriods = _mutedPeriods
            };
            station.MountTrafficGenerator(TrafficGenerator);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            _mutedPeriods = 0;
        }
    }
}