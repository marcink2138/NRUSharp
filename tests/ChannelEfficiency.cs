using NLog;
using NRUSharp.core;
using NRUSharp.core.rngWrapper.impl;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class ChannelEfficiency : BaseTest{
        private readonly ScenarioRunner _scenarioRunner;
        private const int SimulationNumber = 10;
        private const int SimulationTime = 20_000_000;
        private const int Cca = 9;
        private const int Ffp = 5000;
        private const int Cot = 4500;
        private const int StationIncrementation = 1;
        private const int Repetitions = 10;

        public ChannelEfficiency(ITestOutputHelper output) : base(output){
            _scenarioRunner = new ScenarioRunner();
        }

        [Fact]
        public void FloatingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            FloatingFbeBuilder
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    scenarioMatrix[i].Add(FloatingFbeBuilder.WithName(name).Build());
                }

                stationNum += StationIncrementation;
            }

            OutputHelper.WriteLine("siema");
            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "floating-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            FloatingFbeBuilder.Reset();
        }

        [Fact]
        public void RandomMutingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            const int mutedPeriods = 5;
            const int transmissionPeriods = 5;
            //Populating builder with basic props
            RandomMutingFbeBuilder
                .WithTransmissionPeriodNum(transmissionPeriods)
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithOffsetBottom(0)
                .WithOffsetTop(Cot)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                RandomMutingFbeBuilder
                    .WithMutedPeriodNum(mutedPeriods * (i + 1));
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    RandomMutingFbeBuilder
                        .WithName(name);
                    scenarioMatrix[i].Add(RandomMutingFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "random-muting-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            RandomMutingFbeBuilder.Reset();
        }

        [Fact]
        public void FixedMutingFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            const int halfOfCot = Cot / 2;
            //Populating builder with basic props
            FixedMutingFbeBuilder
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    FixedMutingFbeBuilder
                        .WithMutedPeriods(j + 1)
                        .WithOffsetBottom(j * halfOfCot)
                        .WithOffsetTop(j * halfOfCot)
                        .WithName(name);
                    scenarioMatrix[i].Add(FixedMutingFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "fixed-muting-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            FixedMutingFbeBuilder.Reset();
        }

        [Fact]
        public void StandardFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            const int halfOfCot = Cot / 2;
            //Populating builder with basic props
            StandardFbeBuilder
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    StandardFbeBuilder
                        .WithOffsetBottom(j * halfOfCot)
                        .WithOffsetTop(j * halfOfCot)
                        .WithName(name);
                    scenarioMatrix[i].Add(StandardFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "standard-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            StandardFbeBuilder.Reset();
        }

        [Fact]
        public void EnhancedFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            EnhancedFbeBuilder
                .WithQ(10)
                .IsBitr(false)
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    EnhancedFbeBuilder
                        .WithName(name);
                    scenarioMatrix[i].Add(EnhancedFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "enhanced-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void GreedyEnhancedFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            GreedyEnhancedFbeBuilder
                .WithQ(10)
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    GreedyEnhancedFbeBuilder
                        .WithName(name);
                    scenarioMatrix[i].Add(GreedyEnhancedFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "greedy-enhanced-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            GreedyEnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void BitrFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            TestLogManagerWrapper.InitializeStationLogger(LogLevel.Warn, LogLevel.Fatal, "");
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            //Populating builder with basic props
            EnhancedFbeBuilder
                .WithQ(10)
                .IsBitr(true)
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    EnhancedFbeBuilder
                        .WithName(name);
                    scenarioMatrix[i].Add(EnhancedFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "bitr-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            EnhancedFbeBuilder.Reset();
        }

        [Fact]
        public void DeterministicBackoffFbe(){
            var scenarioMatrix = TestHelper.CreateScenarioMatrix(SimulationNumber);
            var rngWrapper = new RngWrapper();
            rngWrapper.Init();
            TestLogManagerWrapper.InitializeStationLogger(LogLevel.Trace, LogLevel.Fatal, "");
            //Populating builder with basic props
            DeterministicBackoffFbeBuilder
                .WithFfp(Ffp)
                .WithCot(Cot)
                .WithCca(Cca)
                .WithSimulationTime(SimulationTime)
                .WithRngWrapper(rngWrapper);
            var stationNum = 1;
            for (var i = 0; i < SimulationNumber; i++){
                for (var j = 0; j < stationNum; j++){
                    var name = $"Test_{i + 1}_{j + 1}";
                    DeterministicBackoffFbeBuilder
                        .WithName(name);
                    scenarioMatrix[i].Add(DeterministicBackoffFbeBuilder.Build());
                }

                stationNum += StationIncrementation;
            }

            var scenarioDescription =
                new ScenarioDescription(Repetitions, SimulationTime, scenarioMatrix, "db-fbe.csv");
            _scenarioRunner.RunScenario(scenarioDescription);
            DeterministicBackoffFbeBuilder.Reset();
        }
    }
}