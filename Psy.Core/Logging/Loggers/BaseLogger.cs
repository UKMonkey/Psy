using System;

namespace Psy.Core.Logging.Loggers
{
    public abstract class BaseLogger : IComparable<BaseLogger>
    {
        public LoggerLevel LoggerLevel { get; set; }

        public abstract void Write(string logText, LoggerLevel loggerLevel, string source = "", string category = "",
                                   string metaData = "");

        protected BaseLogger()
        {
            LoggerLevel = LoggerLevel.Warning;
        }

        public bool WillHandle(LoggerLevel loggerLevel)
        {
            return ((int)LoggerLevel >= (int)loggerLevel);
        }

        public int CompareTo(BaseLogger other)
        {
            return (int)other.LoggerLevel - (int)LoggerLevel;
        }
    }
}