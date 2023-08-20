using NLog;
using NRUSharp.core.data;
using NRUSharp.core.node.fbeImpl;
using NRUSharp.core.rngWrapper.impl;
using NRUSharp.core.trafficGenerator.impl;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class GeneratedTraffic:BaseTest{
        private readonly ScenarioRunner _scenarioRunner;
        public GeneratedTraffic(ITestOutputHelper output) : base(output){
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void SingleStandardFBENode(){
            // TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            // TestLogManagerWrapper.InitializeTrafficGeneratorLogger(LogLevel.Trace, LogLevel.Fatal, "");
            RngWrapper rngWrapper = new RngWrapper();
            rngWrapper.Init();
            StandardFbeBuilder
                .WithCca(9)
                .WithFfp(10_000)
                .WithCot(4000)
                .WithName("Node 1")
                .WithRngWrapper(rngWrapper)
                .WithSimulationTime(20_000_000)
                .WithGeneratorUnitProvider(simulation => new Frame{
                    GenerationTime = simulation.NowD,
                    Size = 250
                });
            var lambdas = new[]{0.040, 0.080, 0.160, 0.320, 0.640, 1.280, 2.560, 5.12, 10.24, 20.48};
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            for (int i = 0; i < lambdas.Length; i++){
                var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[i], RngWrapper = rngWrapper};
                var node = StandardFbeBuilder.WithTrafficGenerator(tg).Build();
                scenarioMatrix[i].Add(node);
            }
            var scenarioDescription =
                new ScenarioDescription(10, 20_000_000, scenarioMatrix, "generatedTraffic\\standard-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }
        
        [Fact]
        public void FourStandardFBENode(){
            // TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            // TestLogManagerWrapper.InitializeTrafficGeneratorLogger(LogLevel.Trace, LogLevel.Fatal, "");
            RngWrapper rngWrapper = new RngWrapper();
            rngWrapper.Init();
            StandardFbeBuilder
                .WithCca(9)
                .WithFfp(10_000)
                .WithCot(4_000)
                .WithRngWrapper(rngWrapper)
                .WithSimulationTime(20_000_000)
                .WithGeneratorUnitProvider(simulation => new Frame{
                    GenerationTime = simulation.NowD,
                    Size = 250
                });
            var lambdas = new[]{0.040, 0.080, 0.160, 0.320, 0.640, 1.280, 2.560, 5.12, 10.24, 20.48};
            var offsets = new[]{0, 2500, 5000, 7500};
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            for (int i = 0; i < lambdas.Length; i++){
                for (int j = 0; j < offsets.Length; j++){
                    var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[i], RngWrapper = rngWrapper};
                    var node = StandardFbeBuilder
                        .WithTrafficGenerator(tg)
                        .WithName($"Node {j + 1}")
                        .WithOffsetBottom(offsets[j])
                        .WithOffsetTop(offsets[j])
                        .Build();
                    scenarioMatrix[i].Add(node);   
                }
            }
            var scenarioDescription =
                new ScenarioDescription(10, 20_000_000, scenarioMatrix, "generatedTraffic\\standard-fbe-generated-traffic-four-nodes");
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }
        
    }
}