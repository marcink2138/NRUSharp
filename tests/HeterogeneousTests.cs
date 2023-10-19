using NRUSharp.core.rngWrapper.impl;
using NRUSharp.simulationFramework;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests;

public class HeterogeneousTests : BaseTest{
    private readonly ScenarioRunner _scenarioRunner;
    private const int Ffp = 2000;
    private const int Cot = 491;
    private const int Cca = 9;
    private const int SimulationTime = 60 * 1_000_000;

    public HeterogeneousTests(ITestOutputHelper output) : base(output){
        _scenarioRunner = new ScenarioRunner();
    }

    [Fact]
    public void HeterogeneousTest(){
        var scenarioMatrix = TestHelper.CreateScenarioMatrix(18);
        var rngWrapper = new RngWrapper();
        rngWrapper.Init();
        FloatingFbeBuilder
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);
        StandardFbeBuilder
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);
        FixedMutingFbeBuilder
            .WithMutedPeriods(1)
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);
        RandomMutingFbeBuilder
            .WithMutedPeriodNum(5)
            .WithTransmissionPeriodNum(5)
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);
        EnhancedFbeBuilder
            .WithQ(8)
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);
        GreedyEnhancedFbeBuilder
            .WithQ(8)
            .WithCca(Cca)
            .WithCot(Cot)
            .WithFfp(Ffp)
            .WithRngWrapper(rngWrapper)
            .WithSimulationTime(SimulationTime);

        //Standard vs Floating FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[0].Add(FloatingFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}").Build()
            );
            scenarioMatrix[0].Add(StandardFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"Standard FBE {i + 1}")
                .Build()
            );
        }

        //Standard vs E FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[1].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Enhanced FBE {i + 1}").Build()
            );
            scenarioMatrix[1].Add(StandardFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"Standard FBE {i + 1}")
                .Build()
            );
        }

        //Standard vs GE FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[2].Add(GreedyEnhancedFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[2].Add(StandardFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"Standard FBE {i + 1}")
                .Build()
            );
        }

        //Standard vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[3].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[3].Add(StandardFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"Standard FBE {i + 1}")
                .Build()
            );
        }

        //FM vs Floating
        for (var i = 0; i < 4; i++){
            scenarioMatrix[4].Add(FloatingFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[4].Add(FixedMutingFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"FM FBE {i + 1}")
                .Build()
            );
        }

        //FM vs E FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[5].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[5].Add(FixedMutingFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"FM FBE {i + 1}")
                .Build()
            );
        }

        //FM vs GE FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[6].Add(GreedyEnhancedFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[6].Add(FixedMutingFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"FM FBE {i + 1}")
                .Build()
            );
        }

        //FM vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[7].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[7].Add(FixedMutingFbeBuilder
                .WithOffsetBottom((9 + Cot) * i)
                .WithOffsetTop((9 + Cot) * i)
                .WithName($"FM FBE {i + 1}")
                .Build()
            );
        }

        //RM vs Floating
        for (var i = 0; i < 4; i++){
            scenarioMatrix[8].Add(RandomMutingFbeBuilder
                .WithOffsetTop((9 + Cot) * i)
                .WithOffsetBottom((9 + Cot) * i)
                .WithName($"RM FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[8].Add(FloatingFbeBuilder
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
        }

        //RM vs E FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[9].Add(RandomMutingFbeBuilder
                .WithOffsetTop((9 + Cot) * i)
                .WithOffsetBottom((9 + Cot) * i)
                .WithName($"RM FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[9].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"Enhanced FBE {i + 1}")
                .Build()
            );
        }

        //RM vs GE FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[10].Add(RandomMutingFbeBuilder
                .WithOffsetTop((9 + Cot) * i)
                .WithOffsetBottom((9 + Cot) * i)
                .WithName($"RM FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[10].Add(GreedyEnhancedFbeBuilder
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
        }

        //RM vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[11].Add(RandomMutingFbeBuilder
                .WithOffsetTop((9 + Cot) * i)
                .WithOffsetBottom((9 + Cot) * i)
                .WithName($"RM FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[11].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
        }

        // Floating vs E FBE

        for (var i = 0; i < 4; i++){
            scenarioMatrix[12].Add(FloatingFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[12].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"Enhanced FBE {i + 1}")
                .Build()
            );
        }

        // Floating vs GE FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[13].Add(FloatingFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[13].Add(GreedyEnhancedFbeBuilder
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
        }

        // Floating vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[14].Add(FloatingFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Floating FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[14].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
        }

        //E FBE vs GE FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[15].Add(GreedyEnhancedFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[15].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"Enhanced FBE {i + 1}")
                .Build()
            );
        }

        //E FBE vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[16].Add(EnhancedFbeBuilder
                .IsBitr(false)
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"Enhanced FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[16].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
        }

        //GE FBE vs BITR FBE
        for (var i = 0; i < 4; i++){
            scenarioMatrix[17].Add(GreedyEnhancedFbeBuilder
                .WithOffsetTop(0)
                .WithOffsetBottom(0)
                .WithName($"GE FBE {i + 1}")
                .Build()
            );
            scenarioMatrix[17].Add(EnhancedFbeBuilder
                .IsBitr(true)
                .WithOffsetBottom(0)
                .WithOffsetTop(0)
                .WithName($"BITR FBE {i + 1}")
                .Build()
            );
        }

        var scenarioDescription =
            new ScenarioDescription(10, SimulationTime, scenarioMatrix,
                "heterogeneous\\heterogeneous-all-combinations-in-one");
        _scenarioRunner.RunScenario(scenarioDescription);
    }
}