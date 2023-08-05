using System;
using NRUSharp.core.data;
using NRUSharp.core.node;
using NRUSharp.core.rngWrapper.impl;
using NRUSharp.core.trafficGenerator;
using NRUSharp.core.trafficGenerator.impl;
using SimSharp;
using Xunit;

namespace NRUSharp.tests{
    class TestNode : IQueueListener<Frame>{
        public void HandleNewItem(Frame item){ }
    }

    public class NodeQueueTest{
        [Fact]
        public void QueueLimitTest(){
            Frame UnitProvider(Simulation simulation) => new Frame{GenerationTime = simulation.NowD, Retries = 0};
            IQueueListener<Frame> simpleNode = new TestNode();
            var queue = new NodeQueue<Frame>(200, UnitProvider, simpleNode);
            var queue2 = new NodeQueue<Frame>(200, UnitProvider, simpleNode);
            var queue3 = new NodeQueue<Frame>(200, UnitProvider, simpleNode);
            var queue4 = new NodeQueue<Frame>(200, UnitProvider, simpleNode);
            var queue5 = new NodeQueue<Frame>(200, UnitProvider, simpleNode);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init(500);
            var tg = new DistributionTrafficGenerator<Frame>{
                GeneratorUnitProvider = UnitProvider,
                RngWrapper = rngWrapper
            };
            var tg2 = new DistributionTrafficGenerator<Frame>{
                GeneratorUnitProvider = UnitProvider,
                RngWrapper = rngWrapper
            };
            var tg3 = new DistributionTrafficGenerator<Frame>{
                GeneratorUnitProvider = UnitProvider,
                RngWrapper = rngWrapper
            };
            var tg4 = new DistributionTrafficGenerator<Frame>{
                GeneratorUnitProvider = UnitProvider,
                RngWrapper = rngWrapper
            };
            var tg5 = new DistributionTrafficGenerator<Frame>{
                GeneratorUnitProvider = UnitProvider,
                RngWrapper = rngWrapper
            };
            queue.TrafficGenerator = tg;
            queue2.TrafficGenerator = tg2;
            queue3.TrafficGenerator = tg3;
            queue4.TrafficGenerator = tg4;
            queue5.TrafficGenerator = tg5;
            var env = new Simulation(TimeSpan.FromSeconds(1));
            queue.Start(env);
            queue2.Start(env);
            queue3.Start(env);
            queue4.Start(env);
            queue5.Start(env);
            env.Run(TimeSpan.FromSeconds(100_000_000));
            Assert.Equal(200, queue.Count);
        }
    }
}