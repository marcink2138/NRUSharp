using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class StandardFbeBuilder : AbstractFbeStationBuilder{
        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var station = new StandardFbe(Name, Env, Channel, fbeTimes, Offset, RngWrapper, SimulationTime);
            if (reset){
                Reset();
            }

            return station;
        }
    }
}