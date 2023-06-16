using NLog;
using NRUSharp.core;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class ValidationWithOffset : BaseTest{
        private readonly ScenarioRunner _scenarioRunner;
        private const int Ffp = 10_000;
        private const int Cca = 9;
        private readonly int[] Cots ={1_000, 2_000, 3_000, 4_000, 5_000, 6_000, 7_000, 8_000, 9_000};
        private readonly int[] Offsets ={0, 2_500, 5_000, 7_500};
        private const int SimulationTime = 20_000_000;
        private const int Repetitions = 10;

        public ValidationWithOffset(ITestOutputHelper output) : base(output){
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void StandardFbe(){
            var resultsFileName = "StandardFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            StandardFbeBuilder
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Standard FBE {i + 1}";
                    var station = StandardFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }
        [Fact]
        public void FixedMutingFbe(){
            var resultsFileName = "FixedMutingFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            FixedMutingFbeBuilder
                .WithMutedPeriods(1)   
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Fixed-muting FBE {i + 1}";
                    var station = FixedMutingFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            FixedMutingFbeBuilder.Reset();
        }
        
        [Fact]
        public void RandomMutingFbe(){
            var resultsFileName = "RandomMutingFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            RandomMutingFbeBuilder
                .WithMutedPeriodNum(5)
                .WithTransmissionPeriodNum(5)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Random-muting FBE {i + 1}";
                    var station = RandomMutingFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }
        
        [Fact]
        public void FloatingFbe(){
            var resultsFileName = "FloatingFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            FloatingFbeBuilder
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Floating FBE {i + 1}";
                    var station = FloatingFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            FloatingFbeBuilder.Reset();
        }
        
        [Fact]
        public void DbFbe(){
            var resultsFileName = "DeterministicBackoffFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            DeterministicBackoffFbeBuilder
                .WithThreshold(3)
                .WithMaxRetransmissionNum(5)
                .WithInitialBackoff(2)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Deterministic-backoff FBE {i + 1}";
                    var station = DeterministicBackoffFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            DeterministicBackoffFbeBuilder.Reset();
        }
        
        [Fact]
        public void EnhancedFbe(){
            var resultsFileName = "EnhancedFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            EnhancedFbeBuilder
                .WithQ(8)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Enhanced FBE {i + 1}";
                    var station = EnhancedFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }
        
        [Fact]
        public void BitrFbe(){
            var resultsFileName = "BitrFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            EnhancedFbeBuilder
                .WithQ(8)
                .IsBitr(true)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"BITR FBE {i + 1}";
                    var station = EnhancedFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }
        
        [Fact]
        public void BitrFbeV2(){
            var resultsFileName = "BitrFBEWithOffset_v2";
            TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(1);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            EnhancedFbeBuilder
                .WithQ(8)
                .IsBitr(true)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            // for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"BITR FBE {i + 1}";
                    var station = EnhancedFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(9000)
                        .WithName(name)
                        .Build();
                    scenarioMatrix[0].Add(station);
                }
            // }

            var scenarioDescription = new ScenarioDescription(1, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }
        
        [Fact]
        public void GeFbe(){
            var resultsFileName = "GeFBEWithOffset";
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(Cots.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            GreedyEnhancedFbeBuilder
                .WithQ(8)
                .WithFfp(Ffp)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);

            for (var scenarioNum = 0; scenarioNum < scenarioMatrix.Count; scenarioNum++){
                for (var i = 0; i < Offsets.Length; i++){
                    var name = $"Greedy-enhanced FBE {i + 1}";
                    var station = GreedyEnhancedFbeBuilder
                        .WithOffsetBottom(Offsets[i])
                        .WithOffsetTop(Offsets[i])
                        .WithCot(Cots[scenarioNum])
                        .WithName(name)
                        .Build();
                    scenarioMatrix[scenarioNum].Add(station);
                }
            }

            var scenarioDescription = new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, resultsFileName);
            _scenarioRunner.RunScenario(scenarioDescription);
            GreedyEnhancedFbeBuilder.Reset();
        }
        
    }
}