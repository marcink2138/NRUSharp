using System.Collections.Generic;
using NRUSharp.core;

namespace NRUSharp.simulationFramework{
    public record SimulationObjectDescription{
        public string Name;
        public Dictionary<string, List<int>> ParamsDescription;
        public StationType StationType;
    }
}