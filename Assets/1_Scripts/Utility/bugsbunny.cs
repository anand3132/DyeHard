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

        public static LogLevel CurrentLogLevel = LogLevel.FullLog;

        public static void Log(string message)
        {
            if (CurrentLogLevel == LogLevel.FullLog)
            {
                Debug.Log(message);
            }
        }

        public static void Log(LogLevel level, string message)
        {
            if (CurrentLogLevel == LogLevel.FullLog)
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
    }
}