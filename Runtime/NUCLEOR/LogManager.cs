using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class LogManager
    {
        [Serializable]
        public readonly struct LogInfos
        {
            public readonly string message;
            public readonly string stackTrace;
            public readonly LogType type;

            //--------------------------------------------------------------------------------------------------------------

            public LogInfos(in string item1, in string item2, in LogType item3)
            {
                message = item1;
                stackTrace = item2;
                type = item3;
            }
        }

        static readonly Queue<LogInfos> last_logs = new(max_log);
        static Action<LogInfos> on_log;

        const byte max_log = 250;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            on_log = null;

            Application.logMessageReceivedThreaded -= OnLogMessage;
            Application.logMessageReceivedThreaded += OnLogMessage;

            Debug.Log(typeof(LogManager).FullName + " initialized.");
        }


        //--------------------------------------------------------------------------------------------------------------

        static void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning && message.StartsWith("The character with Unicode value "))
                return;

            if (type == LogType.Exception)
                message = message.TrimEnd('\n', '\r');

            LogInfos log = new(message, stackTrace, type);

            lock (last_logs)
            {
                if (last_logs.Count >= max_log)
                    last_logs.Dequeue();
                last_logs.Enqueue(log);
                on_log?.Invoke(log);
            }
        }

        public static void ToggleListener(in bool toggle, in Action<LogInfos> listener)
        {
            lock (last_logs)
            {
                on_log -= listener;
                if (toggle)
                    on_log += listener;
            }
        }

        public static void ReadLastLogs(in int count, in Action<LogInfos> on_log)
        {
            lock (last_logs)
            {
                int i = 0;
                foreach (LogInfos log in last_logs)
                {
                    if (i++ >= count)
                        break;
                    on_log?.Invoke(log);
                }
            }
        }

        internal static void ClearLogs()
        {
            lock (last_logs)
                last_logs.Clear();
        }
    }
}