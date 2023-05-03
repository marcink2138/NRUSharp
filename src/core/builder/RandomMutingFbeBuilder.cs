using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class RandomMutingFbeBuilder : AbstractFbeStationBuilder{
        protected int TransmissionPeriodNum;
        protected int MutedPeriodNum;

        public RandomMutingFbeBuilder WithTransmissionPeriodNum(int transmissionPeriodNum){
            TransmissionPeriodNum = transmissionPeriodNum;
            return this;
        }

        public RandomMutingFbeBuilder WithMutedPeriodNum(int mutedPeriodNum){
            MutedPeriodNum = mutedPeriodNum;
            return this;
        }

        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new RandomMutingFbe(Name, Env, Channel, fbeTimes, RngWrapper, TransmissionPeriodNum,
                MutedPeriodNum, simulationParams);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            TransmissionPeriodNum = 0;
            MutedPeriodNum = 0;
        }
    }
}