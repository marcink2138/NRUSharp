using NLog;
using NRUSharp.core.rngWrapper.impl;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class HomogeneousOptimizedTests : BaseTest{
        private readonly ScenarioRunner _scenarioRunner;
        private readonly int Ffp = 5000;
        private readonly int Cot = 500;
        private readonly int Cca = 9;
        private readonly int SimulationTime = 60 * 1_000_000;
        private readonly int StationIncrementation = 2;
        private readonly int[] _numberOfStations ={2, 4, 8, 16, 32};

        public HomogeneousOptimizedTests(ITestOutputHelper output) : base(output){
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void FloatingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var cot = 491;
            var offsetTop = 500;
            int[] ffps ={1_000, 2_000, 4_000, 8_000};
            //Populating builder with basic props
            FloatingFbeBuilder
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            //2, 4, 8, 16
            for (var i = 0; i < _numberOfStations.Length - 1; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    FloatingFbeBuilder
                        .WithFfp(ffps[i])
                        .WithOffsetTop(offsetTop)
                        .WithOffsetBottom(0);
                    scenarioMatrix[i].Add(FloatingFbeBuilder.WithName(name).Build());
                }
            }

            //32
            cot = 303;
            for (var j = 0; j < _numberOfStations[4]; j++){
                var name = $"Test_{4 + 1}_{j + 1}";
                FloatingFbeBuilder
                    .WithFfp(10_000)
                    .WithCot(cot)
                    .WithOffsetTop(offsetTop)
                    .WithOffsetBottom(0);
                scenarioMatrix[4].Add(FloatingFbeBuilder.WithName(name).Build());
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "floating-fbe-homogeneous-optimized.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            FloatingFbeBuilder.Reset();
        }

        [Fact]
        public void StandardFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            // TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            int[] ffps ={1_000, 2_000, 4_000, 8_000};
            // 2,4,8,16 nodes
            var cot = 491;
            for (var i = 0; i < _numberOfStations.Length - 1; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    var node = StandardFbeBuilder
                        .WithFfp(ffps[i])
                        .WithCot(cot)
                        .WithCca(Cca)
                        .WithSimulationTime(SimulationTime)
                        .WithRngWrapper(rngWrapper)
                        .WithOffsetTop((9 + cot) * j)
                        .WithOffsetBottom((9 + cot) * j)
                        .WithName(name)
                        .Build();
                    scenarioMatrix[i].Add(node);
                }
            }

            // 32 nodes
            cot = 303;
            for (var i = 0; i < 32; i++){
                var name = $"Test_{5}_{i + 1}";
                var node = StandardFbeBuilder
                    .WithFfp(10_000)
                    .WithCot(cot)
                    .WithCca(Cca)
                    .WithSimulationTime(SimulationTime)
                    .WithRngWrapper(rngWrapper)
                    .WithOffsetTop((9 + cot) * i)
                    .WithOffsetBottom((9 + cot) * i)
                    .WithName(name)
                    .Build();
                scenarioMatrix[4].Add(node);
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "standard-fbe-homogeneous-optimized.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }


        [Fact]
        public void EnhancedFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var ffp = 2000;
            var cot = 1900;
            //Populating builder with basic props
            EnhancedFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    EnhancedFbeBuilder
                        .WithQ(_numberOfStations[i] * 2);
                    scenarioMatrix[i].Add(EnhancedFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "enhanced-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void BITRFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var ffp = 2000;
            var cot = 1900;
            //Populating builder with basic props
            EnhancedFbeBuilder
                .IsBitr(true)
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    EnhancedFbeBuilder
                        .WithQ(_numberOfStations[i] * 2);
                    scenarioMatrix[i].Add(EnhancedFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "BITR-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void GEFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var ffp = 2000;
            var cot = 1900;
            //Populating builder with basic props
            GreedyEnhancedFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    GreedyEnhancedFbeBuilder
                        .WithQ(_numberOfStations[i] * 2);
                    scenarioMatrix[i].Add(GreedyEnhancedFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "ge-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            GreedyEnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void FixedMutingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
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
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    FixedMutingFbeBuilder
                        .WithMutedPeriods(_numberOfStations[i]/2 - 1)
                        .WithOffsetTop((9 + cot) * j)
                        .WithOffsetBottom((9 + cot) * j);
                    scenarioMatrix[i].Add(FixedMutingFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "fm-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            FixedMutingFbeBuilder.Reset();
        }
        
        [Fact]
        public void RandomMutingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            var ffp = 2000;
            var cot = 1900;
            RandomMutingFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            int[] periods ={2, 4, 8, 16, 32};
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    RandomMutingFbeBuilder
                        .WithMutedPeriodNum(periods[i]*2)
                        .WithTransmissionPeriodNum(periods[i]*2)
                        .WithOffsetTop((Cca) * j)
                        .WithOffsetBottom((Cca) * j);
                    scenarioMatrix[i].Add(RandomMutingFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "rm-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }

        [Fact]
        public void RandomMutingFbev2(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var ffp = 2000;
            var cot = 1900;
            //Populating builder with basic props
            RandomMutingFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < _numberOfStations[i]; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    RandomMutingFbeBuilder
                        .WithMutedPeriodNum(_numberOfStations[i]*2)
                        .WithTransmissionPeriodNum(_numberOfStations[i]*2)
                        .WithOffsetTop(1000)
                        .WithOffsetBottom(0);
                    scenarioMatrix[i].Add(RandomMutingFbeBuilder.WithName(name).Build());
                }
            }
        
            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "rm-fbe-homogeneous-optimized");
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }
        [Fact]
        public void RandomMutingFbeTest(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            var ffp = 2000;
            var cot = 1900;
            //Populating builder with basic props
            TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            RandomMutingFbeBuilder
                .WithFfp(ffp)
                .WithCot(cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < 32; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    RandomMutingFbeBuilder
                        .WithMutedPeriodNum(32)
                        .WithTransmissionPeriodNum(32)
                        .WithOffsetTop(cot)
                        .WithOffsetBottom(0);
                    scenarioMatrix[i].Add(RandomMutingFbeBuilder.WithName(name).Build());
                }
            }

            var scenarioDescription =
                new ScenarioDescription(1, SimulationTime, scenarioMatrix, "rm-fbe-homogeneous-optimized.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }

        [Fact]
        public void DBFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(_numberOfStations.Length);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            DeterministicBackoffFbeBuilder
                .WithMaxRetransmissionNum(22)
                .WithThreshold(3)
                .WithInitialBackoff(3)
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < _numberOfStations.Length; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    scenarioMatrix[i].Add(DeterministicBackoffFbeBuilder.WithName(name).Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(10, SimulationTime, scenarioMatrix, "DB-fbe-homogeneous.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            DeterministicBackoffFbeBuilder.Reset();
        }
    }
}