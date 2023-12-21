using NLog;
using NRUSharp.core.data;
using NRUSharp.core.rngWrapper.impl;
using NRUSharp.core.trafficGenerator.impl;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class HomogeneousNotOptimizedTrafficGeneratedTests : BaseTest{
        private readonly ScenarioRunner _scenarioRunner;
        private readonly int Ffp = 5000;
        private readonly int Cot = 500;
        private readonly int Cca = 9;
        private readonly int SimulationTime = 60 * 1_000_000;
        private readonly double[] lambdas = {0.0040, 0.0080, 0.0160, 0.0320, 0.0640, 0.1280, 0.2560};


        public HomogeneousNotOptimizedTrafficGeneratedTests(ITestOutputHelper output) : base(output){
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void FloatingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            // TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Error, "");
            FloatingFbeBuilder
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            //2, 4, 8, 16
            for (var i = 0; i < lambdas.Length; i++){
                for (var j = 0; j < 16; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[i], RngWrapper = rngWrapper};
                    FloatingFbeBuilder
                        .WithCot(1_500)
                        .WithGeneratorUnitProvider(simulation => new Frame{
                            GenerationTime = simulation.NowD,
                            Size = 1_500
                        })
                        .WithTrafficGenerator(tg)
                        .WithFfp(2_000);
                    scenarioMatrix[i].Add(FloatingFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "notOptimized\\floating-fbe-homogeneous-tg");
            _scenarioRunner.RunScenario(scenarioDescription);
            FloatingFbeBuilder.Reset();
        }

        [Fact]
        public void StandardFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var cot = 491;
            for (var i = 0; i < lambdas.Length; i++){
                for (var j = 0; j < 16; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[i], RngWrapper = rngWrapper};
                    var node = StandardFbeBuilder
                        .WithFfp(4_000)
                        .WithCot(cot)
                        .WithCca(Cca)
                        .WithSimulationTime(SimulationTime)
                        .WithRngWrapper(rngWrapper)
                        .WithOffsetTop((9 + cot) * j)
                        .WithOffsetBottom((9 + cot) * j)
                        .WithGeneratorUnitProvider(simulation => new Frame{
                            GenerationTime = simulation.NowD,
                            Size = cot
                        })
                        .WithTrafficGenerator(tg)
                        .WithName(name)
                        .Build();
                    scenarioMatrix[i].Add(node);
                }
            }
        
            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "notOptimized\\standard-fbe-homogeneous-tg");
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }
        
        [Fact]
        public void FixedMutingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            var ffp = 1000;
            var cot = 491;
            FixedMutingFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper)
                .WithGeneratorUnitProvider(simulation => new Frame{
                    GenerationTime = simulation.NowD,
                    Size = 491
                });
            ;
            for (var i = 0; i < lambdas.Length; i++){
                for (var j = 0; j < 16; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[i], RngWrapper = rngWrapper};
                    FixedMutingFbeBuilder
                        .WithMutedPeriods(3)
                        .WithOffsetTop((9 + cot) * j)
                        .WithOffsetBottom((9 + cot) * j)
                        .WithTrafficGenerator(tg);
                    scenarioMatrix[i].Add(FixedMutingFbeBuilder.WithName(name).Build());
                }
            }
        
            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "notOptimized\\fm-fbe-homogeneous-tg");
            _scenarioRunner.RunScenario(scenarioDescription);
            FixedMutingFbeBuilder.Reset();
        }
        
        
        [Fact]
        public void RandomMutingFbev2(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(lambdas.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            RandomMutingFbeBuilder
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper)
                .WithGeneratorUnitProvider(simulation => new Frame{
                    GenerationTime = simulation.NowD,
                    Size = 491
                });
            for (int j = 0; j < lambdas.Length; j++){
                for (int i = 0; i < 16; i++){
                    var name = $"Test_{3}_{i + 1}";
                    var tg = new DistributionTrafficGenerator<Frame>{Lambda = lambdas[j], RngWrapper = rngWrapper};
                    RandomMutingFbeBuilder
                        .WithMutedPeriodNum(2)
                        .WithTransmissionPeriodNum(2)
                        .WithFfp(2500)
                        .WithCot(491)
                        .WithOffsetBottom(250 * i)
                        .WithOffsetTop(250 * i)
                        .WithTrafficGenerator(tg)
                        .WithName(name);
                    scenarioMatrix[j].Add(RandomMutingFbeBuilder.Build());
                }   
            }
        
            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "notOptimized\\rm-fbe-homogeneous-tg");
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }
    }
}