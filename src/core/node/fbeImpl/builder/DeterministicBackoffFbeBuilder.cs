using NRUSharp.core.data;
using NRUSharp.core.interfaces;

namespace NRUSharp.core.node.fbeImpl.builder{
    public class DeterministicBackoffFbeBuilder : AbstractFbeStationBuilder{
        private int _maxRetransmissionNum = 5;
        private int _threshold = 3;
        private int _initialBackoff = 3;

        public DeterministicBackoffFbeBuilder WithMaxRetransmissionNum(int maxRetransmissionNum){
            _maxRetransmissionNum = maxRetransmissionNum;
            return this;
        }

        public DeterministicBackoffFbeBuilder WithThreshold(int threshold){
            _threshold = threshold;
            return this;
        }

        public DeterministicBackoffFbeBuilder WithInitialBackoff(int initBackoff){
            _initialBackoff = initBackoff;
            return this;
        }

        public override INode Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new DeterministicBackoffFbe{
                FbeTimes = fbeTimes,
                Env = Env,
                Channel = Channel,
                Name = Name,
                RngWrapper = RngWrapper,
                SimulationParams = simulationParams,
                InitialBackoff = _initialBackoff,
                Threshold = _threshold,
                MaxRetransmissionNum = _maxRetransmissionNum
            };
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            _maxRetransmissionNum = 3;
            _threshold = 5;
            _initialBackoff = 3;
        }
    }
}