using NRUSharp.core.data;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class RandomMutingFbeBuilder : AbstractFbeStationBuilder{
        private int _transmissionPeriodNum;
        private int _mutedPeriodNum;

        public RandomMutingFbeBuilder WithTransmissionPeriodNum(int transmissionPeriodNum){
            _transmissionPeriodNum = transmissionPeriodNum;
            return this;
        }

        public RandomMutingFbeBuilder WithMutedPeriodNum(int mutedPeriodNum){
            _mutedPeriodNum = mutedPeriodNum;
            return this;
        }

        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new RandomMutingFbe{
                Name = Name,
                Env = Env,
                Channel = Channel,
                FbeTimes = fbeTimes,
                RngWrapper = RngWrapper,
                SimulationParams = simulationParams,
                MutedPeriodNum = _mutedPeriodNum,
                TransmissionPeriodNum = _transmissionPeriodNum
            };
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            _transmissionPeriodNum = 0;
            _mutedPeriodNum = 0;
        }
    }
}