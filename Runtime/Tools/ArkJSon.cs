using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    [Serializable]
    public abstract class StaticJSon : JSon
    {
        public abstract string GetFilePath();
        public void SaveStaticJSon(in bool log) => Save(GetFilePath(), log);
        public static bool ReadStaticJSon<T>(out T json, in bool force, in bool log) where T : StaticJSon, new()
        {
            json = new T();
            return Read(ref json, json.GetFilePath(), force, log);
        }
    }

    [Serializable]
    public abstract class HomeJSon : StaticJSon
    {
        [Obsolete]
        public static string GetFilePath(in Type type) => NUCLEOR.GetHomeJSonPath(type);
        public override string GetFilePath() => NUCLEOR.GetHomeJSonPath(GetType());
    }

    [Serializable]
    public abstract class UserJSon : StaticJSon
    {
        public static string GetFilePath(in Type type) => Path.Combine(NUCLEOR.instance.GetCurrentUserFolder(force: true).FullName, type.FullName + json_txt);
        public override string GetFilePath() => GetFilePath(GetType());
    }

    [Serializable]
    public abstract class ResourcesJSon : JSon
    {
#if UNITY_EDITOR
        public static string GetFilePath<T>() => GetFilePath(typeof(T));
        public static string GetFilePath(in Type type) => Path.Combine(NUCLEOR.DFResources.ForceDir().FullName, type.FullName + json_txt);
        public string GetFilePath() => GetFilePath(GetType());
#endif

        public static bool TryReadResourcesJSon<T>(in bool log, out T json) where T : ResourcesJSon, new()
        {
            string fname = typeof(T).FullName + ".json";
            TextAsset asset = Resources.Load<TextAsset>(fname);

            if (asset != null)
            {
                json = JsonUtility.FromJson<T>(asset.text);
                return true;
            }
            else
            {
                json = new();
#if UNITY_EDITOR
                json.SaveResourcesJSon();
#endif
                return false;
            }
        }

#if UNITY_EDITOR
        public void SaveResourcesJSon()
        {
            string fpath = GetFilePath();
            Save(fpath, true);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}