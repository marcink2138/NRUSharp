using System;
using System.Collections.Generic;
using NRUSharp.core.rngWrapper;
using SimSharp;

namespace NRUSharp.core.trafficGenerator{
    public interface ITrafficGenerator<T>{
        public Simulation Env{ get; set; }
        public IRngWrapper RngWrapper{ get; init; }
        public Func<Simulation, T> GeneratorUnitProvider{ get; set; }
        public int GeneratedUnits{ set; get; }
        public IEnumerable<Event> Start(NodeQueue<T> receiver);
        public void Notify();
    }
}