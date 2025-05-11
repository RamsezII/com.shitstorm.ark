using System;
using System.IO;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        public class Infos : JSon
        {
            static readonly string FILE_NAME = typeof(MachineSettings).TypeToFileName() + json;
            public static string GetFilePath() => Path.Combine(NUCLEOR.home_path.GetDir(true).FullName, FILE_NAME);

            public string last_user;
        }

        public static Infos infos = new();

        //----------------------------------------------------------------------------------------------------------

        static void ReadInfos()
        {
            JSon.Read(ref infos, Infos.GetFilePath(), false, true);
        }

        static void SaveInfos()
        {
            infos.last_user = user_name.Value;
            infos.Save(Infos.GetFilePath(), true);
        }
    }
}