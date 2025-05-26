using _UTIL_;
using System;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        public class Settings : MachineJSon
        {
            public string last_user;
            public bool no_smooth;
            public Languages language = Traductable.GetSystemLanguage();
        }

        public static Settings settings = new();

        //----------------------------------------------------------------------------------------------------------

        static void LoadSettings(in bool init)
        {
            StaticJSon.ReadStaticJSon(ref settings, true, init);
            Util_smooths.NO_SMOOTH = settings.no_smooth;

            if (init)
                Traductable.Init(settings.language);
        }

        static void SaveSettings(in bool log)
        {
            settings.last_user = user_name.Value;
            settings.SaveStaticJSon(log);
        }
    }
}