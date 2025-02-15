using Common.Interfaces;
using Common.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class ConsumerConfigManager
    {
        ILogger logger;
        string dllDirPath;
        string configPath;
        string? outputPath;
        LogType logType;
        public string DLLDirPath
        {
            get { return dllDirPath; }
            set
            {
                logger.Info($"setting The dll Path to ({value})");
                if (!Directory.Exists(value))
                    throw new ArgumentException($"The Dll Path provided ({value}) is not a valid Directory.");
                dllDirPath = value;
                logger.Info($"setting The dll Path Successfull");
            }
        }
        public string ConfigPath
        {
            get { return configPath; }
            set
            {
                logger.Info($"setting The config Path to ({configPath})");
                if (!File.Exists(value))
                    throw new ArgumentException($"The Config File provided ({value}) is not a valid File.");
                configPath = value;
                logger.Info($"setting The config Path Successfull");
            }
        }
        public string? OutputPath
        {
            get { return outputPath; }
            set
            {
                if (value == null)
                {
                    logger.Warn("Clearing the output path. the logs will be shown in the console.");
                    outputPath = value;
                    logger.Info("Output Path cleared");
                }
                else
                {
                    logger.Info($"setting The output Path to ({outputPath})");
                    outputPath = value;
                    logger.Info($"setting The output Path Successfull");
                }
            }
        }
        public LogType LogLevel
        {
            get { return logType; }
            set
            {
                logger?.Info($"setting The log level to {value}");
                logType = value;
                logger?.Info($"setting The log level successfull");
            }
        }

        public ConsumerConfigManager()
        {
            LogLevel = LogType.Warning;
            logger = GetLogger();
            DLLDirPath = @"..\..\..\plugins";
            ConfigPath = @"..\..\..\config\default.conf";
        }

        public ILogger GetLogger()
        {
            if (OutputPath == null)
                return new ConsoleLogger(LogLevel);
            else
                return new FileLogger(LogLevel, OutputPath);
        }
    }
}
