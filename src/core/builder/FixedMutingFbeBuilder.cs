using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class FixedMutingFbeBuilder : AbstractFbeStationBuilder{
        protected int MutedPeriods;

        public FixedMutingFbeBuilder WithMutedPeriods(int mutedPeriods){
            MutedPeriods = mutedPeriods;
            return this;
        }

        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var station = new FixedMutingFbe(Name, Env, Channel, fbeTimes, Offset, RngWrapper, MutedPeriods,
                SimulationTime);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            MutedPeriods = 0;
        }
    }
}