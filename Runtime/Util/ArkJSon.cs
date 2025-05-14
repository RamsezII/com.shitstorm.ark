using _UTIL_;
using System;
using System.IO;

namespace _ARK_
{
    public abstract class ArkJSon : JSon
    {
        public const string arkjson = ".ark" + json;
        public string GetFileName() => GetType().FullName + arkjson;
        public string GetExtension() => "." + GetFileName();
        public string GetFilePath() => Path.Combine(IsUserFile ? MachineSettings.GetUserFolder(true).FullName : NUCLEOR.home_path.GetDir(true).FullName, GetFileName());

        public void SaveArkJSon(in bool log) => Save(GetFilePath(), log);
        public static bool Read<T>(ref T json, in bool force, in bool log) where T : ArkJSon, new() => Read(ref json, json.GetFilePath(), force, log);
        protected virtual bool IsUserFile => false;
    }


    [Serializable]
    public class ArkJSonArray<T> : ArkJSon where T : IBytes
    {
        public T[] array;

        //----------------------------------------------------------------------------------------------------------

        public override void WriteBytes(in BinaryWriter writer)
        {
            base.WriteBytes(writer);
            if (array == null)
                writer.Write((ushort)0);
            else
            {
                writer.Write((ushort)array.Length);
                for (int i = 0; i < array.Length; ++i)
                    array[i].WriteBytes(writer);
            }
        }

        public override void ReadBytes(in BinaryReader reader)
        {
            base.ReadBytes(reader);

            ushort count = reader.ReadUInt16();
            array = new T[count];

            for (int i = 0; i < count; ++i)
                array[i].ReadBytes(reader);
        }
    }
}