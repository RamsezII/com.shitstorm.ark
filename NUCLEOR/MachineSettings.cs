using _UTIL_;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static class MachineSettings
    {
        public static readonly string FILE_NAME = typeof(MachineSettings).FullName + JSon.txt;
        public static string GetFilePath() => Path.Combine(NUCLEOR.home_path.ForceDir().FullName, FILE_NAME);

        public static readonly OnValue<string> machine_name = new();
        const string default_name = "default_user";

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            ReadInfos(true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.onApplicationFocus += ReadInfosNoLog;
            NUCLEOR.delegates.onApplicationUnfocused += SaveInfosNoLog;
        }

        //----------------------------------------------------------------------------------------------------------

        static void SaveInfosNoLog() => SaveInfos(false);
        static void SaveInfos(in bool log)
        {
            File.WriteAllText(GetFilePath(), machine_name.Value);
            if (log)
                Debug.Log($"{typeof(MachineSettings).FullName}.WRITE {nameof(machine_name)}: \"{machine_name.Value}\" {GetFilePath()}".ToSubLog());
        }

        static void ReadInfosNoLog() => ReadInfos(false);
        static void ReadInfos(in bool log)
        {
            if (File.Exists(GetFilePath()))
            {
                string value = File.ReadAllText(GetFilePath());
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (log)
                        Debug.Log($"empty {nameof(machine_name)} found in \"{GetFilePath()}\", fallback to {nameof(default_name)} \"{default_name}\"");
                    machine_name.Update(default_name);
                }
                else
                {
                    if (log)
                        Debug.Log($"{typeof(MachineSettings).FullName}.READ {nameof(machine_name)}: \"{value}\" ({GetFilePath()})".ToSubLog());
                    machine_name.Update(value);
                }
            }
            else
            {
                if (log)
                    Debug.Log($"could not find \"{GetFilePath()}\", fallback to default {nameof(machine_name)}: \"{default_name}\"");
                machine_name.Update(default_name);
                SaveInfos(log);
            }
        }
    }
}