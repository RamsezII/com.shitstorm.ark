using System;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        public class Infos : ArkJSon
        {
            public string last_user;
        }

        public static Infos infos = new();

        //----------------------------------------------------------------------------------------------------------

        static void ReadInfos()
        {
            ArkJSon.Read(ref infos, true, true);
        }

        static void SaveInfos()
        {
            infos.last_user = user_name.Value;
            infos.SaveArkJSon(true);
        }
    }
}