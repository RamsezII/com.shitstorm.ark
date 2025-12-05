using System;
using UnityEngine;

namespace _ARK_
{
    partial class ArkMachine
    {
        [Serializable]
        public class HSettings : HomeJSon
        {
            public string last_user;

            public Languages language = Application.systemLanguage switch
            {
                SystemLanguage.French => Languages.French,
                _ => Languages.English,
            };
        }

        public static HSettings h_settings;

        //----------------------------------------------------------------------------------------------------------

        public static void LoadSettings(in bool log)
        {
            StaticJSon.ReadStaticJSon(out h_settings, true, log);
            ApplySettings();
        }

        public static void SaveSettings(in bool log)
        {
            h_settings.SaveStaticJSon(log);
        }

        public static void ApplySettings()
        {
            Traductable.language.Value = h_settings.language;
        }
    }
}