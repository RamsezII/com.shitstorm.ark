using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public readonly struct ArkBuildID
    {
        public const string
            rname_buildID = nameof(ArkBuildID);

#if UNITY_EDITOR
        public const string
            fname_buildID = rname_buildID + ".txt";

        public readonly string
            fpath_buildID;

        public static readonly Lazy<ArkBuildID> instance = new(() => new(null));

        const string button_prefixe = "Assets/" + nameof(_ARK_) + "/";

        //----------------------------------------------------------------------------------------------------------

        ArkBuildID(object o)
        {
            fpath_buildID = Path.Combine(ArkPaths.instance.Value.dpath_resources, fname_buildID);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(LoadBuildID))]
#endif
        public static ulong LoadBuildID() => TryLoadBuildID(out ulong buildID) ? buildID : 0;
        public static bool TryLoadBuildID(out ulong buildID)
        {
            TextAsset text = Resources.Load<TextAsset>(rname_buildID);
            if (text != null)
            {
                buildID = ulong.Parse(text.text);
                Debug.Log($"{typeof(ArkBuildID)} LOAD: {buildID}");
                return true;
            }

#if UNITY_EDITOR
            if (Application.isEditor)
                buildID = IncrementBuildID();
            else
#endif
                buildID = 0;

            return false;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(ResetBuildID))]
        public static void ResetBuildID()
        {
            if (File.Exists(instance.Value.fpath_buildID))
                File.Delete(instance.Value.fpath_buildID);
            IncrementBuildID();
        }

        [UnityEditor.MenuItem(button_prefixe + nameof(IncrementBuildID))]
        public static ulong IncrementBuildID()
        {
            ulong buildID = 1 + LoadBuildID();
            if (!Directory.Exists(ArkPaths.instance.Value.dpath_resources))
                Directory.CreateDirectory(ArkPaths.instance.Value.dpath_resources);
            File.WriteAllText(instance.Value.fpath_buildID, buildID.ToString());
            Debug.Log($"{typeof(ArkBuildID)} NEW VALUE: {buildID}");
            UnityEditor.AssetDatabase.Refresh();
            return buildID;
        }

        [UnityEditor.MenuItem(button_prefixe + nameof(OpenBuildID))]
        public static void OpenBuildID() => Application.OpenURL(instance.Value.fpath_buildID);
#endif
    }
}