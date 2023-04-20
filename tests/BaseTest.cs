using NLog;
using NLog.Config;
using NLog.Targets;
using NRUSharp.core.builder;

namespace NRUSharp.tests{
    public class BaseTest{
        protected StandardFbeBuilder StandardFbeBuilder = new();
        protected EnhancedFbeBuilder EnhancedFbeBuilder = new();
        protected FixedMutingFbeBuilder FixedMutingFbeBuilder = new();
        protected FloatingFbeBuilder FloatingFbeBuilder = new();
        protected GreedyEnhancedFbeBuilder GreedyEnhancedFbeBuilder = new();
        protected RandomMutingFbeBuilder RandomMutingFbeBuilder = new();

        protected BaseTest(){
            SetUpLogger();
        }

        private void SetUpLogger(){
            var config = new LoggingConfiguration();
            // var logfile = new FileTarget("logfile"){FileName = "logs.log"};
            // config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;
        }
    }
}