using System;
using System.Collections.Generic;
using Psy.Core.Logging.Loggers;

namespace Psy.Core.Logging
{
    public static class Logger
    {
        private static readonly List<BaseLogger> Loggers = new List<BaseLogger>(4); 

        /// <summary>
        /// Initialize with a specific logger.
        /// </summary>
        /// <param name="logger"></param>
        public static void Add(BaseLogger logger)
        {
            Loggers.Add(logger);
            Loggers.Sort();
        }

        /// <summary>
        /// Write to the log.
        /// </summary>
        /// <param name="logText">Text to log</param>
        /// <param name="loggerLevel">Optional.</param>
        public static void Write(string logText, LoggerLevel loggerLevel = LoggerLevel.Debug, string source = "")
        {
            foreach (var logger in Loggers)
            {
                if (!logger.WillHandle(loggerLevel))
                    break;

                logger.Write(logText, loggerLevel, source);
            }
        }

        public static void WriteException(Exception exception, int indent = 0)
        {
            var indentChars = "".PadLeft(indent);

            Write(indentChars + "======== Exception raised ========", LoggerLevel.Critical);
            Write(indentChars + "Message: " + exception.Message, LoggerLevel.Critical);
            Write(indentChars + "Source: " + exception.Source, LoggerLevel.Critical);
            Write(indentChars + "Stack Trace: " + exception.StackTrace, LoggerLevel.Critical);

            if (exception.InnerException == null)
                return;

            Write(indentChars + "Inner exception: ", LoggerLevel.Critical);
            WriteException(exception.InnerException, indent + 2);
        }
    }
}
