using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambot_3
{
    public static class Logger
    {

        private enum LogLevel
        {
            Debug,
            Low,
            Medium,
            Fatal,
            Error,
            Info
        }

        private static void Log(LogLevel logLevel = LogLevel.Debug, string message = null)
        {
            

            Console.ForegroundColor = logLevel == LogLevel.Debug 
                ? ConsoleColor.Gray : logLevel == LogLevel.Low 
                ? ConsoleColor.Green : logLevel == LogLevel.Medium 
                ? ConsoleColor.Yellow : logLevel == LogLevel.Fatal 
                ? ConsoleColor.DarkRed : logLevel == LogLevel.Error 
                ? ConsoleColor.Red : logLevel == LogLevel.Info 
                ? ConsoleColor.White : ConsoleColor.White;

            Console.WriteLine(message ?? "Something went wrong...");
            Console.ResetColor();
        }

        public static void Debug(string message = null) => Log(LogLevel.Debug, message);

        public static void Debug(Exception exception) => Log(LogLevel.Debug, exception.ToString());
        public static void Low(string message = null) => Log(LogLevel.Low, message);
        public static void Low(Exception exception) => Log(LogLevel.Low, exception.ToString());
        public static void Medium(string message = null) => Log(LogLevel.Medium, message);
        public static void Medium(Exception exception) => Log(LogLevel.Medium, exception.ToString());
        public static void Fatal(string message = null) => Log(LogLevel.Fatal, message);
        public static void Fatal(Exception exception)=> Log(LogLevel.Fatal, exception.ToString());
        public static void Error(string message = null) => Log(LogLevel.Error, message);
        public static void Error(Exception exception) => Log(LogLevel.Error, exception.ToString());
        public static void Info(string message = null) => Log(LogLevel.Info, message);
        public static void Info(Exception exception) => Log(LogLevel.Info, exception.ToString());
    }
}
