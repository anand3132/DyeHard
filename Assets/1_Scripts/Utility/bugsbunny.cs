using UnityEngine;
namespace RedGaint
{

    public static class BugsBunny
    {
        public enum LogLevel
        {
            one,
            Two,
            Three,
            FullLog,      // Log everything
            PartialLog,   // Log warnings and errors only
            ErrorOnly     // Log errors only
        }

        public static LogLevel CurrentLogLevel = LogLevel.Three;

        public static void Log(string message)
        {
            if (CurrentLogLevel == LogLevel.FullLog)
            {
                Debug.Log(message);
            }
        }

        public static void Log1(string message)
        {
            if (CurrentLogLevel == LogLevel.one)
            {
                Debug.Log(message);
            }
        }
        public static void Log2(string message)
        {
            if (CurrentLogLevel == LogLevel.Two)
            {
                Debug.Log(message);
            }
        }
        public static void Log3(string message)
        {
            if (CurrentLogLevel == LogLevel.Three)
            {
                Debug.Log(message);
            }
        }
        public static void FullLog(LogLevel level, string message)
        {
            if (CurrentLogLevel == LogLevel.FullLog)
            {
                Debug.Log(message);
            }
        }
        public static void PartialLog(LogLevel level, string message)
        {
            if (CurrentLogLevel == LogLevel.PartialLog)
            {
                Debug.Log(message);
            }
        }
        public static void Warning(string message)
        {
            if (CurrentLogLevel == LogLevel.FullLog || CurrentLogLevel == LogLevel.PartialLog)
            {
                Debug.LogWarning(message);
            }
        }

        public static void LogError(string message)
        {
            Debug.LogError(message);
        }
        public static void LogRed(string message)
        {
            LogWithColor(message, "red");
        }

        public static void LogGreen(string message)
        {
            LogWithColor(message, "green");
        }

        public static void LogBlue(string message)
        {
            LogWithColor(message, "blue");
        }

        public static void LogYellow(string message)
        {
            LogWithColor(message, "yellow");
        }
        public static void LogWithColor(string message, string color)
        {
            Debug.Log($"<color={color}> {message} </color>");
        }
    }
}