using System;
using UnityEngine;

namespace _ARK_
{
    partial class ArkMachine
    {
        [Serializable]
        public class HSettings : HomeJSon
        {
            internal static string GetSettingsPath() => GetHomeJSonPath(typeof(HSettings));

            public string last_user_name;
            public Languages langage = Application.systemLanguage switch
            {
                SystemLanguage.French => Languages.French,
                _ => Languages.English,
            };
        }

        static HSettings settings;

        //----------------------------------------------------------------------------------------------------------

        public static void SaveHSettings(in bool log)
        {
            string spath = HSettings.GetSettingsPath();

            settings.langage = language._value;

            settings.Save(spath, log);
        }

        public static void LoadHSettings(in bool log)
        {
            string fpath = HSettings.GetSettingsPath();

            JSon.Read(ref settings, fpath, true, log);

            language.Value = settings.langage;
        }
    }
}