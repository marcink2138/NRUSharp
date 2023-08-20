using System.Collections.Generic;
using SimSharp;
using Exponential = MathNet.Numerics.Distributions.Exponential;

namespace NRUSharp.core.trafficGenerator.impl{
    public class DistributionTrafficGenerator<T> : AbstractTrafficGenerator<T>{

        public double Lambda{ set; get; } = 1.0;

        public override IEnumerable<Event> Start(NodeQueue<T> receiver){
            ItsQueue = receiver;
            while (true){
                var sleepTime = Exponential.Sample(RngWrapper.Rng, Lambda) * 1_000;
                Logger.Debug("Waiting {} till generating new frame", (int) sleepTime);
                yield return Env.TimeoutD((int) sleepTime);
                ItsQueue.Enqueue(GenerateUnit());
            }
        }

        public override void Dequeue(){ }
    }
}