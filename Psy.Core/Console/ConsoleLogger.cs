using System;
using Psy.Core.Logging;
using Psy.Core.Logging.Loggers;
using SlimMath;

namespace Psy.Core.Console
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger()
        {
            LoggerLevel = LoggerLevel.Note;
        }

        public override void Write(string logText, LoggerLevel loggerLevel, string source = "", string category = "", string metaData = "")
        {
            StaticConsole.Console.AddLine(logText, GetColour(loggerLevel));
        }

        private static Color4 GetColour(LoggerLevel loggerLevel)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Critical:
                    return new Color4(1.0f, 0.0f, 0.0f);
                case LoggerLevel.Error:
                    return new Color4(255, 127, 0);
                case LoggerLevel.Warning:
                    return new Color4(255, 255, 0);
                case LoggerLevel.Info:
                    return new Color4(0, 255, 255);
                case LoggerLevel.Note:
                    return new Color4(0, 128, 255);
                case LoggerLevel.Debug:
                    return new Color4(0, 255, 0);
                case LoggerLevel.Trace:
                    return new Color4(128, 255, 128);
                default:
                    throw new ArgumentOutOfRangeException("loggerLevel");
            }
        }
    }
}
