using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class TestLogManagerWrapper{
        private static bool _isConsoleLoggerInitialized;
        private static bool _isStationLoggerInitialized;
        private const string ConsoleLoggerPrefix = "NRU-Sharp-";
        private const string StationLoggerPrefix = "Station-";
        private const string TrafficGeneratorLoggerPrefix = "TG";

        public static void InitializeConsoleLogger(ITestOutputHelper outputHelper){
            if (_isConsoleLoggerInitialized){
                return;
            }

            var consoleTarget = new XunitLoggerTarget(outputHelper);
            var loggingRule = new LoggingRule($"{ConsoleLoggerPrefix}*", LogLevel.Trace, consoleTarget);
            AddLoggingRule(loggingRule);
            _isConsoleLoggerInitialized = true;
        }

        private static void AddLoggingRule(LoggingRule loggingRule){
            var configuration = LogManager.Configuration;
            if (configuration == null){
                var loggingConfiguration = new LoggingConfiguration();
                loggingConfiguration.AddRule(loggingRule);
                LogManager.Configuration = loggingConfiguration;
                return;
            }

            configuration.AddRule(loggingRule);
            LogManager.Configuration = configuration;
        }

        public static void InitializeStationLogger(LogLevel min, LogLevel max, string path){
            if (_isStationLoggerInitialized){
                return;
            }

            var fileTarget = new FileTarget("logfile"){
                FileName =  "${basedir}/logs.log",
            };
            var loggingRule = new LoggingRule($"{StationLoggerPrefix}*", min, max, fileTarget);
            var loggingRule2 = new LoggingRule($"{TrafficGeneratorLoggerPrefix}*", min, max, fileTarget);
            AddLoggingRule(loggingRule);
            AddLoggingRule(loggingRule2);
            _isStationLoggerInitialized = true;
        }

        public static void InitializeTrafficGeneratorLogger(LogLevel min, LogLevel max, string path){
            var fileTarget = new FileTarget("logfile"){
                FileName =  "${basedir}/logs.log",
            };
            var loggingRule = new LoggingRule($"{TrafficGeneratorLoggerPrefix}*", min, max, fileTarget);
            AddLoggingRule(loggingRule);
        }
    }
}