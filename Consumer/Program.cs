using Common.Interfaces;
using System.CommandLine;
using Newtonsoft.Json;
using System;
using Consumer;
using CommonLib.PluginLoader;

ConsumerConfigManager configManager = new();
var logger = configManager.GetLogger();

var rootCommand = new RootCommand("This is a Tool for producing generic C# Types. put your custom classes in dll Directory to get them loaded automaticaly.");

var configOption = new Option<string>(["--config", "-c"], "Path to the config file (by default is ./config/default.conf)");
var dllDirOption = new Option<string>(["--dlldir", "-d"], "Path to the Plugin Directory (by default is ./plugins/)");
var verboseOption = new Option<bool>(["--verbose", "-v"], "Prints more Output. (sets log level to 0)");
var quietOption = new Option<bool>(["--quiet", "-q"], "Prints more Output. (sets log level to 4)");
var logLevelOption = new Option<int>(["--loglevel", "-l"], "Prints the given and higher level logs. (0: info 1: warning 2: error 3: fatal 4: Nothing)");
var logOutputOption = new Option<string>(["--logpath", "-o"], "Prints logs in the specified file. (the Program generates the file each time it runs so be carefull to not overwrite existing logs. if you dont provide this option the logs are printed in the console)");

rootCommand.Add(configOption);
rootCommand.Add(dllDirOption);
rootCommand.Add(verboseOption);
rootCommand.Add(quietOption);
rootCommand.Add(logLevelOption);
rootCommand.Add(logOutputOption);

rootCommand.SetHandler((string configPath, string dllDirPath, bool verbose, bool quiet, int level, string outputPath) =>
{
    logger.Info($"setting The config Path to ({configPath})");
    if (configPath != null)
        configManager.ConfigPath = configPath;
    if (dllDirPath != null)
        configManager.DLLDirPath = dllDirPath;
    if (verbose)
        configManager.LogLevel = LogType.Info;
    if (quiet)
        configManager.LogLevel = LogType.Nothing;
    if (!((int)LogType.Info <= level && level <= (int)LogType.Nothing))
        throw new ArgumentException("log level should be in the range 0 to 4");
    configManager.LogLevel = (LogType)level;
    if (outputPath != null)
        configManager.OutputPath = outputPath;
}, configOption, dllDirOption, verboseOption, quietOption, logLevelOption, logOutputOption);

//try
//{
logger.Info("Loading Config");
rootCommand.InvokeAsync(args).Wait();
logger.Info("Configuration Sucessfully Loaded");
logger.Info("Reloading Logger");
logger = configManager.GetLogger();
logger.Info("New Logger Sucessfully Loaded");


ConsumerPluginLoader loader = new(logger);
var ProducerTypes = loader.Load(configManager.DLLDirPath);
ConsumptionUnit consumptionUnit = new ConsumptionUnit(ProducerTypes, "http://127.0.0.1:5155/", logger);
await consumptionUnit.StartConsumption();

//}
//catch (Exception ex)
//{
//    logger.Error(ex.ToString());
//}
