using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{

    public enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal,
        Nothing,
    }
    public interface ILogger
    {
        LogType LogLevel { get; set; }
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);

    }
}
