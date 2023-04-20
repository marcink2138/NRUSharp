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
            var station = new EnhancedFbe(Name, Env, Channel, fbeTimes, Offset, RngWrapper, Q,
                Bitr, SimulationTime);
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