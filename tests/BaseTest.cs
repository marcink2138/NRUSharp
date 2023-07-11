using NRUSharp.core.node.fbeImpl.builder;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class BaseTest{
        protected readonly StandardFbeBuilder StandardFbeBuilder = new();
        protected readonly EnhancedFbeBuilder EnhancedFbeBuilder = new();
        protected readonly FixedMutingFbeBuilder FixedMutingFbeBuilder = new();
        protected readonly FloatingFbeBuilder FloatingFbeBuilder = new();
        protected readonly GreedyEnhancedFbeBuilder GreedyEnhancedFbeBuilder = new();
        protected readonly RandomMutingFbeBuilder RandomMutingFbeBuilder = new();
        protected readonly DeterministicBackoffFbeBuilder DeterministicBackoffFbeBuilder = new();
        protected readonly ITestOutputHelper OutputHelper;

        protected BaseTest(ITestOutputHelper output){
            OutputHelper = output;
            SetUpLogger();
        }

        private void SetUpLogger(){
            TestLogManagerWrapper.InitializeConsoleLogger(OutputHelper);
        }
    }
}