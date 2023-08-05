using System;
using System.Collections.Generic;
using NLog;
using SimSharp;

namespace NRUSharp.core.trafficGenerator.impl{
    public class SimpleTrafficGenerator<T> : AbstractTrafficGenerator<T>{
        private readonly Logger _logger = LogManager.GetLogger("SimpleGenerator");


        public SimpleTrafficGenerator(Func<Simulation, T> generatorGeneratorUnitProvider){
            GeneratorUnitProvider = generatorGeneratorUnitProvider;
        }

        public override IEnumerable<Event> Start(NodeQueue<T> receiver){
            ItsQueue = receiver;
            _logger.Debug(
                "Put single unit right now, exit generation process and finally put traffic only after notyfication");
            ItsQueue.Enqueue(GenerateUnit());
            yield break;
        }

        public override void Dequeue(){
            _logger.Debug("Notification recieved");
            ItsQueue.Enqueue(GenerateUnit());
        }
    }
}