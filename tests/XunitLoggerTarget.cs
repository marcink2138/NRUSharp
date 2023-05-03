using NLog;
using NLog.Targets;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class XunitLoggerTarget : TargetWithLayout{
        private readonly ITestOutputHelper _outputHelper;

        public XunitLoggerTarget(ITestOutputHelper outputHelper){
            _outputHelper = outputHelper;
        }

        protected override void Write(LogEventInfo logEvent){
            var mess = Layout.Render(logEvent);
            _outputHelper.WriteLine(mess);
        }
    }
}