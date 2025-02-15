using Common.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Common.Loggers
{
    public class FileLogger : ILogger
    {
        private readonly string logFilePath;
        private static readonly object _lock = new object();
        LogType logLevel;
        public LogType LogLevel { get; set; }

        public FileLogger(LogType logLevel, string logFilePath = "log.txt")
        {
            this.logFilePath = logFilePath;
            this.logLevel = logLevel;
        }

        public void Info(string message) => Log(LogType.Info, message);
        public void Warn(string message) => Log(LogType.Warning, message);
        public void Error(string message) => Log(LogType.Error, message);
        public void Fatal(string message) => Log(LogType.Fatal, message);

        private void Log(LogType logType, string message)
        {
            if (logType < LogLevel) return; // Skip logging if below current level

            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {message}";

            lock (_lock) // Ensure thread safety
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}
