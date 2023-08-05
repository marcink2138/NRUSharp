using System.Collections.Generic;
using NLog;
using SimSharp;
using Exponential = MathNet.Numerics.Distributions.Exponential;

namespace NRUSharp.core.trafficGenerator.impl{
    public class DistributionTrafficGenerator<T> : AbstractTrafficGenerator<T>{
        private readonly Logger _logger = LogManager.GetLogger("DistributionGenerator");

        public override IEnumerable<Event> Start(NodeQueue<T> receiver){
            ItsQueue = receiver;
            while (true){
                var sleepTime = Exponential.Sample(RngWrapper.Rng, 10) * 1_000;
                _logger.Debug("Waiting {} till generating new frame", sleepTime);
                yield return Env.TimeoutD((int) sleepTime);
                ItsQueue.Enqueue(GenerateUnit());
            }
        }

        public override void Dequeue(){ }
    }
}