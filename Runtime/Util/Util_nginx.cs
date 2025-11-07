using System;
using UnityEngine;

namespace _ARK_
{
    [Serializable]
    public class NginxEntry
    {
        public string name;
        public string type;
        public string mtime;
        public long size;
        public DateTimeOffset MTimeAsDate => mtime.ParseNginxMtimeToDate();
    }

    [Serializable]
    public class NginxWrapper
    {
        public NginxEntry[] entries;
        public static NginxWrapper FromJSon(in string json)
        {
            string modified = $"{{\"{nameof(entries)}\":{json}}}";
            return JsonUtility.FromJson<NginxWrapper>(modified);
        }
    }
}