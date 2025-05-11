using System;
using System.IO;

namespace _ARK_
{
    partial class MachineSettings
    {
        [Serializable]
        class Infos : JSon
        {
            static readonly string FILE_NAME = typeof(MachineSettings).TypeToFileName() + json;
            public static string GetFilePath() => Path.Combine(ForceUsersFolder().FullName, FILE_NAME);
        }
    }
}