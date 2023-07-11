using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.trafficGenerator{
    public interface ITrafficGenerator{
        public IEnumerable<Event> Start();
        public void Notify();
        public int GetGeneratedUnitsNum();
        
        public void SetSimulationEnvironment(Simulation simulation);
    }
}