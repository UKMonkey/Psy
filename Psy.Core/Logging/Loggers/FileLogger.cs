using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Psy.Core.Configuration;

namespace Psy.Core.Logging.Loggers
{
    public class FileLogger : BaseLogger, IDisposable
    {
        public static string GlobalSource = "";

        public bool FlushAfterEachWrite { get; set; }
        private readonly string _timestamp;
        private readonly TextWriter _file;
        private readonly FileStream _stream;

        private static string ApplicationDataPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }

        public static string LogFolderPath
        {
            get { return Path.Combine(ApplicationDataPath, StaticConfigurationManager.ConfigurationManager.GetString("LogDir")); }
        }

        private string FullLogFilePath
        {
            get
            {
                return Path.Combine(
                    LogFolderPath, 
                    Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName) + "-" + _timestamp + ".log");
            }
        }

        public FileLogger()
        {
            _timestamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                .Replace(":", "-")
                .Replace("/", "-");

            CreateLogFolder();

            _stream = new FileStream(FullLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            _file = new StreamWriter(_stream);

            FlushAfterEachWrite = true;
        }

        public static void CreateLogFolder()
        {
            var di = new DirectoryInfo(LogFolderPath);
            if (!di.Exists)
            {
                di.Create();
            }
        }

        public override void Write(string logText, LoggerLevel loggerLevel, string source = "", string category = "", string metaData = "")
        {
            lock (this)
            {
                var useSource = string.Format(
                    "{0}{1}{2}", 
                    GlobalSource, 
                    (GlobalSource != "" & source != "" ? ":" : ""),
                    source);

                var dateString = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

                _file.WriteLine(
                    "{0}|{1}|{2}|{3}|{4}",
                    dateString,
                    useSource,
                    loggerLevel,
                    metaData,
                    logText);

                if (FlushAfterEachWrite)
                {
                    _file.Flush();
                }
            }
        }

        public void Dispose()
        {
            _file.Close();
        }
    }
}
