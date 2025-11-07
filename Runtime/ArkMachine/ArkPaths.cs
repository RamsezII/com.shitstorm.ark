using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public readonly struct ArkPaths
    {
        public const string
            dname_home = "home",
            dname_users = "users",
            dname_temp = "temp",
            dname_bundles = "bundles",
            dname_texts = "texts",
            dname_builds = "builds",
            name_windows = "windows",
            name_linux = "linux",
            dname_universal = "universal";

        public static readonly string
            name_app = Application.productName,
            name_exe = Util.is_app_windows ? name_app + ".exe" : name_app + ".x86_64",
            name_os = Util.is_app_windows ? name_windows : name_linux;

        public readonly string
            dname_build,
            dpath_root,
            dpath_home,
            dpath_terminal,
            dpath_temp,

            dpath_bundles,
            dpath_bundles_texts,
            dpath_bundles_os,
            dpath_bundles_windows,
            dpath_bundles_linux,
            dpath_bundles_universal,

            dpath_builds,
            dpath_builds_os,
            dpath_builds_windows,
            dpath_builds_linux,
            dpath_builds_universal,

            dpath_app_expected,
            dpath_app_actual;

        [Serializable]
        public readonly struct LocalBuildInfos
        {
            public readonly DirectoryInfo dir;
            public readonly DateTimeOffset date;
            public readonly bool ok, is_windows;
            public LocalBuildInfos(in DirectoryInfo dir, in bool is_windows)
            {
                this.dir = dir;
                ok = dir.Name.TryParseIntoDate(out date);
                this.is_windows = is_windows;
            }
        }

        public readonly int current_build_index;
        public readonly LocalBuildInfos current_build;
        public readonly LocalBuildInfos[] local_builds;

#if UNITY_EDITOR
        public readonly string
            dpath_assets,
            dpath_resources;

        const string button_prefixe = "Assets/" + nameof(_ARK_) + "/";

        [UnityEditor.MenuItem(button_prefixe + nameof(OpenResourcesFolder))]
        public static void OpenResourcesFolder() => Application.OpenURL(instance.Value.dpath_resources);
#endif

        public readonly string error;

        public static readonly Lazy<ArkPaths> instance = new(() => new(null));

        //----------------------------------------------------------------------------------------------------------

        ArkPaths(object o)
        {
            Debug.Log($"INIT {typeof(ArkPaths)}");

            error = null;

#if UNITY_EDITOR
            dpath_assets = Application.dataPath.NormalizePath();
            dpath_resources = Path.Combine(dpath_assets, "Resources").NormalizePath();
#endif

            DirectoryInfo pdir = Directory.GetParent(Application.dataPath);
            dname_build = pdir.Name;
            dpath_app_actual = pdir.FullName.NormalizePath();

            if (Application.isEditor)
            {
                dpath_app_expected = dpath_app_actual;
                dpath_terminal = dname_build + "/" + dname_home;

                dpath_home = Util.CombinePaths(dpath_app_actual, dname_home);
                dpath_temp = Util.CombinePaths(dpath_home, dname_temp);
                dpath_root = Path.Combine(dpath_home, name_app).NormalizePath();

                dpath_builds = Path.Combine(dpath_home, name_app, dname_builds).NormalizePath();
                dpath_builds_windows = Path.Combine(dpath_builds, name_windows).NormalizePath();
                dpath_builds_linux = Path.Combine(dpath_builds, name_linux).NormalizePath();
                dpath_builds_universal = Path.Combine(dpath_builds, dname_universal).NormalizePath();
                dpath_builds_os = Util.is_windows ? dpath_builds_windows : dpath_builds_linux;

                dpath_bundles = Util.CombinePaths(dpath_home, dname_bundles);
                dpath_bundles_texts = Util.CombinePaths(dpath_bundles, dname_texts);
                dpath_bundles_windows = Util.CombinePaths(dpath_bundles, name_windows);
                dpath_bundles_linux = Util.CombinePaths(dpath_bundles, name_linux);
                dpath_bundles_universal = Util.CombinePaths(dpath_bundles, dname_universal);
                dpath_bundles_os = Util.is_windows ? dpath_bundles_windows : dpath_bundles_linux;
            }
            else
            {
                if (pdir.Parent == null || pdir.Parent.Parent == null || pdir.Parent.Parent.Parent == null)
                {
                    string dpath_rel_app_expected = Path.Combine(name_app, dname_builds, name_os, dname_build).NormalizePath();
                    error = $"mismatch in expected installation path: \"{dpath_app_actual}\" (expected something like: \"{dpath_rel_app_expected}\").";
                    dpath_root = dpath_app_actual;
                    dpath_app_expected = null;
                }
                else
                {
                    dpath_root = pdir.Parent.Parent.Parent.FullName;
                    dpath_app_expected = Path.Combine(dpath_root, dname_builds, name_os, dname_build).NormalizePath();

                    if (!Util.IsSamePath_full(dpath_app_expected, dpath_app_actual))
                        error = $"wrong installation path (expected \"{dpath_app_expected}\", got \"{dpath_app_actual}\")";
                }

                dpath_home = Util.CombinePaths(dpath_root, dname_home);
                dpath_temp = Util.CombinePaths(dpath_home, dname_temp);

                dpath_builds = Path.Combine(dpath_root, dname_builds).NormalizePath();
                dpath_builds_windows = Path.Combine(dpath_builds, name_windows).NormalizePath();
                dpath_builds_linux = Path.Combine(dpath_builds, name_linux).NormalizePath();
                dpath_builds_universal = Path.Combine(dpath_builds, dname_universal).NormalizePath();
                dpath_builds_os = Util.is_windows ? dpath_builds_windows : dpath_builds_linux;

                dpath_bundles = Util.CombinePaths(dpath_home, dname_bundles);
                dpath_bundles_texts = Util.CombinePaths(dpath_bundles, dname_texts);
                dpath_bundles_windows = Util.CombinePaths(dpath_bundles, name_windows);
                dpath_bundles_linux = Util.CombinePaths(dpath_bundles, name_linux);
                dpath_bundles_universal = Util.CombinePaths(dpath_bundles, dname_universal);
                dpath_bundles_os = Util.is_windows ? dpath_bundles_windows : dpath_bundles_linux;

                dpath_terminal = $"{name_app}/{dname_home}";

                if (error != null)
                    Debug.LogError(error);
            }

            if (Application.isEditor || error == null)
            {
                dpath_home.GetDir(true);
                dpath_builds_linux.GetDir(true);
                dpath_builds_windows.GetDir(true);
                dpath_bundles_texts.GetDir(true);
                dpath_bundles_linux.GetDir(true);
                dpath_bundles_windows.GetDir(true);
            }

            string[] dbuilds_lin = Directory.GetDirectories(dpath_builds_linux, "*", SearchOption.TopDirectoryOnly);
            string[] dbuilds_win = Directory.GetDirectories(dpath_builds_windows, "*", SearchOption.TopDirectoryOnly);

            int dbuilds_count = dbuilds_lin.Length + dbuilds_win.Length;
            local_builds = new LocalBuildInfos[dbuilds_count];

            for (int i = 0; i < dbuilds_lin.Length; ++i)
                local_builds[i] = new(dbuilds_lin[i].GetDir(false), false);

            for (int i = 0; i < dbuilds_win.Length; ++i)
                local_builds[i + dbuilds_lin.Length] = new(dbuilds_win[i].GetDir(false), true);

            Array.Sort(local_builds, (a, b) => b.date.CompareTo(a.date));

            current_build_index = 0;
            current_build = default;

            if (!Application.isEditor)
                for (int i = 0; i < dbuilds_count; ++i)
                    if (local_builds[i].dir.Name == dname_build)
                    {
                        current_build_index = i;
                        current_build = local_builds[i];
                        break;
                    }
        }
    }
}