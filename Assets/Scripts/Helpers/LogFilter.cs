using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry
{
    public class LogFilter
    {
        // Mimics Unity Networking Log Filter.

        public enum FilterLevel
        {
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5
        }

        public const int Debug = 1;
        public const int Info = 2;
        public const int Warn = 3;
        public const int Error = 4;
        public const int Fatal = 5;

        private static int s_CurrentLogLevel = Info;
        public static int currentLogLevel { get { return s_CurrentLogLevel; } set { s_CurrentLogLevel = value; } }

        public static bool logDebug { get { return s_CurrentLogLevel <= Debug; } }
        public static bool logInfo { get { return s_CurrentLogLevel <= Info; } }
        public static bool logWarn { get { return s_CurrentLogLevel <= Warn; } }
        public static bool logError { get { return s_CurrentLogLevel <= Error; } }
        public static bool logFatal { get { return s_CurrentLogLevel <= Fatal; } }
    }
}