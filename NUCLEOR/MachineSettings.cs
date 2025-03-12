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
            ReadInfos();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.onApplicationFocus += ReadInfos;
            NUCLEOR.delegates.onApplicationUnfocused += SaveInfos;
        }

        //----------------------------------------------------------------------------------------------------------

        static void SaveInfos()
        {
            File.WriteAllText(GetFilePath(), machine_name.Value);
            Debug.Log($"{typeof(MachineSettings).FullName}.WRITE {nameof(machine_name)}: \"{machine_name.Value}\" {GetFilePath()}".ToSubLog());
        }

        static void ReadInfos()
        {
            if (File.Exists(GetFilePath()))
            {
                string value = File.ReadAllText(GetFilePath());
                if (string.IsNullOrWhiteSpace(value))
                {
                    Debug.Log($"empty {nameof(machine_name)} found in \"{GetFilePath()}\", fallback to {nameof(default_name)} \"{default_name}\"");
                    machine_name.Update(default_name);
                }
                else
                {
                    Debug.Log($"{typeof(MachineSettings).FullName}.READ {nameof(machine_name)}: \"{value}\" ({GetFilePath()})".ToSubLog());
                    machine_name.Update(value);
                }
            }
            else
            {
                Debug.Log($"could not find \"{GetFilePath()}\", fallback to default {nameof(machine_name)}: \"{default_name}\"");
                machine_name.Update(default_name);
                SaveInfos();
            }
        }

        public static void ChangeMachineName(in string name)
        {
            machine_name.Update(name);
            SaveInfos();
        }
    }
}