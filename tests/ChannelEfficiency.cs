using NRUSharp.core;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class ChannelEfficiency : BaseTest{
        private readonly ITestOutputHelper _output;
        private readonly ScenarioRunner _scenarioRunner;

        public ChannelEfficiency(ITestOutputHelper output){
            _output = output;
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void FloatingFbe(){
            var simulatioNumber = 5;
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(simulatioNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            const int cca = 9;
            const int cot = 4500;
            const int ffp = 5000;
            const int simulationTime = 20_000_000;
            //Populating builder with basic props
            FloatingFbeBuilder.WithCca(9)
                .WithOffset(0)
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(cca)
                .WithSimulationTime(simulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 10;
            for (int i = 0; i < simulatioNumber; i++){
                for (int j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    scenarioMatrix[i].Add(FloatingFbeBuilder.WithName(name).Build());
                }

                stationNum += 10;
            }

            _output.WriteLine("siema");
            var scenarioDescription = new ScenarioDescription(3, simulationTime, scenarioMatrix);
            _scenarioRunner.RunScenario(scenarioDescription);
        }
    }
}