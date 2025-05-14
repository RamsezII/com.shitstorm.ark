using _UTIL_;
using System;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        public class Settings : ArkJSon
        {
            public string last_user;
            public bool no_smooth;
        }

        public static Settings settings = new();

        //----------------------------------------------------------------------------------------------------------

        static void ReadInfos()
        {
            ArkJSon.Read(ref settings, true, true);
            Util_smooths.NO_SMOOTH = settings.no_smooth;
        }

        static void SaveInfos()
        {
            settings.last_user = user_name.Value;
            settings.SaveArkJSon(true);
        }
    }
}