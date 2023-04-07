using System.Collections.Generic;

namespace NRUSharp.simulationFramework{
    public record SimulationObjectDescription{
        public string Name;
        public Dictionary<string, List<int>> ParamsDescription;
    }
}