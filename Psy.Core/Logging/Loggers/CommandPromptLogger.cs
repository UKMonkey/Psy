using System;

namespace Psy.Core.Logging.Loggers
{
    public class CommandPromptLogger : BaseLogger
    {
        public override void Write(string logText, LoggerLevel loggerLevel, string source = "", string category = "", string metaData = "")
        {
            var foregroundColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = GetColourForLevel(loggerLevel);
            System.Console.WriteLine(logText);
            System.Console.ForegroundColor = foregroundColour;
        }

        private static ConsoleColor GetColourForLevel(LoggerLevel loggerLevel)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Critical:
                    return ConsoleColor.Red;
                case LoggerLevel.Error:
                    return ConsoleColor.Yellow;
                case LoggerLevel.Warning:
                    return ConsoleColor.DarkYellow;
                case LoggerLevel.Info:
                    return ConsoleColor.Cyan;
                case LoggerLevel.Note:
                    return ConsoleColor.White;
                case LoggerLevel.Debug:
                    return ConsoleColor.White;
                case LoggerLevel.Trace:
                    return ConsoleColor.Gray;
                default:
                    throw new ArgumentOutOfRangeException("loggerLevel");
            }
        }
    }
}