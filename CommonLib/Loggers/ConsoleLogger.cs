using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Loggers
{
    public class ConsoleLogger : ILogger
    {
        object printlock = new object();
        public LogType LogLevel { get; set; }
        

        public ConsoleLogger(LogType LogLevel) 
        {
            this.LogLevel = LogLevel;
        }

        public static void PrintColord(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public void PrintLog(LogType level, string message, ConsoleColor color)
        {
            if (LogLevel <= level)
            {
                lock (printlock)
                {
                    Console.Write("[");
                    PrintColord(level.ToString(), color);
                    Console.WriteLine($"]: {message}");
                }
            }
        }

        public void Error(string message)
        {
            PrintLog(LogType.Error, message, ConsoleColor.Red);
        }

        public void Fatal(string message)
        {
            PrintLog(LogType.Fatal, message, ConsoleColor.DarkRed);
        }

        public void Info(string message)
        {
            PrintLog(LogType.Info, message, ConsoleColor.Blue);
        }

        public void Warn(string message)
        {
            PrintLog(LogType.Warning, message, ConsoleColor.Yellow);
        }

    }
}
