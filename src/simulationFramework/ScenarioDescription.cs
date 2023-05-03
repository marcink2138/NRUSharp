using System.Collections.Generic;
using NRUSharp.core.interfaces;

namespace NRUSharp.simulationFramework{
    public record ScenarioDescription(int Repetitions, int SimulationTime, List<List<IStation>> ScenarioMatrix, string ResultsFileName);
}