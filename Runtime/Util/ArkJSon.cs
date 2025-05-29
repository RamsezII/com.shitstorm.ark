using System.IO;

namespace _ARK_
{
    public abstract class StaticJSon : JSon
    {
        public abstract string GetFilePath();
        public void SaveStaticJSon(in bool log) => Save(GetFilePath(), log);
        public static bool ReadStaticJSon<T>(ref T json, in bool force, in bool log) where T : StaticJSon, new()
        {
            json ??= new T();
            return Read(ref json, json.GetFilePath(), force, log);
        }
    }

    public abstract class MachineJSon : StaticJSon
    {
        public override string GetFilePath() => Path.Combine(NUCLEOR.home_path.GetDir(true).FullName, GetFileName());
    }

    public abstract class UserJSon : StaticJSon
    {
        public override string GetFilePath() => Path.Combine(ArkMachine.GetUserFolder(true).FullName, GetFileName());
    }
}