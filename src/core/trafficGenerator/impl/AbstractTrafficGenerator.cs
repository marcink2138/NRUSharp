using System;
using System.Collections.Generic;
using NRUSharp.core.rngWrapper;
using SimSharp;

namespace NRUSharp.core.trafficGenerator.impl{
    public abstract class AbstractTrafficGenerator<T> : ITrafficGenerator<T>{
        public Simulation Env{ get; set; }
        public Func<Simulation, T> GeneratorUnitProvider{ get; set; }
        public int GeneratedUnits{ get; set; }
        public IRngWrapper RngWrapper{ get; init; }
        protected NodeQueue<T> ItsQueue{ get; set; }
        public abstract IEnumerable<Event> Start(NodeQueue<T> receiver);

        public abstract void Dequeue();

        protected T GenerateUnit(){
            GeneratedUnits++;
            return GeneratorUnitProvider(Env);
        }
    }
}