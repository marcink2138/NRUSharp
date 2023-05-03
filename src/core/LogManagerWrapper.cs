using NLog;
using NLog.Config;
using NLog.Targets;

namespace NRUSharp.core{
    public static class LogManagerWrapper{
        private static bool _isConsoleLoggerInitialized;
        private static bool _isStationLoggerInitialized;
        public const string ConsoleLoggerPrefix = "NRU-Sharp-";
        public const string StationLoggerPrefix = "Station-";

        public static void InitializeConsoleLogger(){
            if (_isConsoleLoggerInitialized){
                return;
            }

            var consoleTarget = new ConsoleTarget();
            var loggingRule = new LoggingRule($"{ConsoleLoggerPrefix}*", LogLevel.Trace, LogLevel.Fatal, consoleTarget);
            AddLoggingRule(loggingRule);
            _isConsoleLoggerInitialized = true;
        }

        private static void AddLoggingRule(LoggingRule loggingRule){
            if (LogManager.Configuration == null){
                LogManager.Configuration = new LoggingConfiguration();
                LogManager.Configuration.AddRule(loggingRule);
                return;
            }
            LogManager.Configuration.AddRule(loggingRule);
        }

        public static void InitializeStationLogger(LogLevel min, LogLevel max, string path){
            if (_isStationLoggerInitialized){
                return;
            }

            var fileTarget = new FileTarget("logfile"){
                FileName = path + "logs.log",
            };
            var loggingRule = new LoggingRule($"{StationLoggerPrefix}*", min, max, fileTarget);
            AddLoggingRule(loggingRule);
            _isStationLoggerInitialized = true;
        }
    }
}