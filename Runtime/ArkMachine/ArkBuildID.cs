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
#endif

        public readonly ulong buildID;

        public readonly string error;

        public static readonly Lazy<ArkBuildID> instance = new(() => new(null));

        //----------------------------------------------------------------------------------------------------------

        ArkBuildID(object o)
        {
            error = null;
            buildID = 0;

#if UNITY_EDITOR
            fpath_buildID = Path.Combine(ArkPaths.instance.Value.dpath_resources, fname_buildID);
#endif

            TextAsset text = Resources.Load<TextAsset>(rname_buildID);

            if (text != null)
            {
                buildID = ulong.Parse(text.text);
                Debug.Log($"loaded {nameof(buildID)}: {buildID}");
            }
#if UNITY_EDITOR
            else if (Application.isEditor)
            {
                File.WriteAllText(fpath_buildID, buildID.ToString());
                Debug.Log($"saved {nameof(buildID)}: {buildID}");
            }
#endif
            else
                error = $"could not load {nameof(buildID)}";

            if (error != null)
                Debug.LogError(error);
        }

        //----------------------------------------------------------------------------------------------------------

        public static string GetBuildName()
        {
            return Application.productName + "_" + instance.Value.buildID;
        }
    }
}