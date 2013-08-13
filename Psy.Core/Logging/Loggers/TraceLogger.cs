using System.Diagnostics;

namespace Psy.Core.Logging.Loggers
{
    public class TraceLogger : BaseLogger
    {
        public override void Write(string logText, LoggerLevel loggerLevel, string source = "", string category = "", string metaData = "")
        {
            Trace.WriteLine(string.Format("[{0}] {1}", loggerLevel, logText));
        }
    }
}