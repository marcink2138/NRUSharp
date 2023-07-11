using System;
using System.Collections.Generic;
using NLog;
using SimSharp;

namespace NRUSharp.core.trafficGenerator.impl{
    public class SimpleGenerator<T> : ITrafficGenerator{
        private readonly Logger _logger = LogManager.GetLogger("SimpleGenerator");
        private Simulation Env{ get; set; }
        private readonly NodeQueue<T> _itsQueue;
        private readonly Func<Simulation, T> _generatorUnitProvider;
        private int _generatedUnits;

        public SimpleGenerator(NodeQueue<T> itsQueue, Func<Simulation, T> generatorUnitProvider){
            _itsQueue = itsQueue;
            _generatorUnitProvider = generatorUnitProvider;
        }

        public IEnumerable<Event> Start(){
            _logger.Debug(
                "Put single unit right now, exit generation process and finally put traffic only after notyfication");
            _itsQueue.Enqueue(GenerateFrame());
            yield break;
        }

        public void Notify(){
            _logger.Debug("Notification recieved");
            _itsQueue.Enqueue(GenerateFrame());
        }

        public int GetGeneratedUnitsNum(){
            return _generatedUnits;
        }

        public void SetSimulationEnvironment(Simulation simulation){
            Env = simulation;
        }

        private T GenerateFrame(){
            _generatedUnits++;
            return _generatorUnitProvider(Env);
        }
    }
}