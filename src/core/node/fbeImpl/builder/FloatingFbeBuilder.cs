using NRUSharp.core.data;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class FloatingFbeBuilder : AbstractFbeStationBuilder{
        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new FloatingFbe{
                Name = Name,
                Env = Env,
                Channel = Channel,
                FbeTimes = fbeTimes,
                RngWrapper = RngWrapper,
                SimulationParams = simulationParams
            };
            station.MountTrafficGenerator(TrafficGenerator);
            if (reset){
                Reset();
            }

            return station;
        }
    }
}