using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        public class Settings : MachineJSon
        {
            public string last_user;
            public bool no_smooth;

            public Languages language = Application.systemLanguage switch
            {
                SystemLanguage.French => Languages.French,
                _ => Languages.English,
            };
        }

        public static Settings settings = new();

        //----------------------------------------------------------------------------------------------------------

        public static void LoadSettings(in bool init)
        {
            StaticJSon.ReadStaticJSon(ref settings, true, init);
            ApplySettings();
        }

        public static void SaveSettings(in bool log)
        {
            settings.SaveStaticJSon(log);
        }

        public static void ApplySettings()
        {
            Util_smooths.NO_SMOOTH = settings.no_smooth;
            Traductable.language.Update(settings.language);
        }
    }
}