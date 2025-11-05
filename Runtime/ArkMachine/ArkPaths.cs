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
            dname_windows = "windows",
            dname_linux = "linux",
            dname_universal = "universal";

        public readonly string
            dname_root,
            dname_os,
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
            dpath_rel_app_expected,
            dpath_app_expected,
            dpath_app_actual,
            dpath_workdir;

#if UNITY_EDITOR
        public readonly string
            dpath_assets,
            dpath_resources;
#endif

        public readonly string error;

        public static readonly Lazy<ArkPaths> instance = new(() => new(null));

        //----------------------------------------------------------------------------------------------------------

        ArkPaths(object o)
        {
            error = null;

            dname_root = Application.productName;
            dname_os = Util.is_windows ? dname_windows : dname_linux;
            dpath_workdir = Directory.GetCurrentDirectory();
            dpath_assets = Application.dataPath.NormalizePath();
            dpath_resources = Path.Combine(dpath_assets, "Resources").NormalizePath();

            DirectoryInfo pdir = Directory.GetParent(Application.dataPath);
            dname_build = pdir.Name;
            dpath_app_actual = pdir.FullName.NormalizePath();
            dpath_terminal = dname_build + Path.DirectorySeparatorChar + dname_home;

            if (Application.isEditor)
            {
                error = null;

                dpath_app_expected = dpath_app_actual;
                dpath_rel_app_expected = null;

                dpath_home = Util.CombinePaths(dpath_app_actual, dname_home);
                dpath_temp = Util.CombinePaths(dpath_home, dname_temp);
                dpath_root = Path.Combine(dpath_home, dname_root).NormalizePath();

                dpath_builds = Path.Combine(dpath_home, dname_builds).NormalizePath();
                dpath_builds_windows = Path.Combine(dpath_builds, dname_windows).NormalizePath();
                dpath_builds_linux = Path.Combine(dpath_builds, dname_linux).NormalizePath();
                dpath_builds_universal = Path.Combine(dpath_builds, dname_universal).NormalizePath();
                dpath_builds_os = Util.is_windows ? dpath_builds_windows : dpath_builds_linux;

                dpath_bundles = Util.CombinePaths(dpath_home, dname_bundles);
                dpath_bundles_texts = Util.CombinePaths(dpath_bundles, dname_texts);
                dpath_bundles_windows = Util.CombinePaths(dpath_bundles, dname_windows);
                dpath_bundles_linux = Util.CombinePaths(dpath_bundles, dname_linux);
                dpath_bundles_universal = Util.CombinePaths(dpath_bundles, dname_universal);
                dpath_bundles_os = Util.is_windows ? dpath_bundles_windows : dpath_bundles_linux;

                dpath_home.GetDir(true);
                dpath_builds_os.GetDir(true);
                dpath_bundles_texts.GetDir(true);
                dpath_bundles_os.GetDir(true);
            }
            else
            {
                dpath_rel_app_expected = Path.Combine(dname_root, dname_builds, dname_os, dname_build).NormalizePath();

                if (pdir.Parent == null || pdir.Parent.Parent == null || pdir.Parent.Parent.Parent == null)
                {
                    error = $"missing parent in installation: \"{dpath_app_actual}\" (expected this hierarchy: \"{dpath_rel_app_expected}\").";
                    dpath_root = dpath_app_actual;
                    dpath_app_expected = null;
                }
                else
                {
                    dpath_root = pdir.Parent.Parent.Parent.Parent.FullName;
                    dpath_app_expected = Path.Combine(dpath_root, dname_builds, dname_os, dname_build).NormalizePath();

                    if (!Util.IsSamePath_full(dpath_app_expected, dpath_app_actual))
                        error = $"wrong installation path (expected \"{dpath_app_expected}\", got \"{dpath_app_actual}\")";
                }

                dpath_home = Util.CombinePaths(dpath_root, dname_home);
                dpath_temp = Util.CombinePaths(dpath_home, dname_temp);

                dpath_builds = Path.Combine(dpath_home, dname_builds).NormalizePath();
                dpath_builds_windows = Path.Combine(dpath_builds, dname_windows).NormalizePath();
                dpath_builds_linux = Path.Combine(dpath_builds, dname_linux).NormalizePath();
                dpath_builds_universal = Path.Combine(dpath_builds, dname_universal).NormalizePath();
                dpath_builds_os = Util.is_windows ? dpath_builds_windows : dpath_builds_linux;

                dpath_bundles = Util.CombinePaths(dpath_home, dname_bundles);
                dpath_bundles_texts = Util.CombinePaths(dpath_bundles, dname_texts);
                dpath_bundles_windows = Util.CombinePaths(dpath_bundles, dname_windows);
                dpath_bundles_linux = Util.CombinePaths(dpath_bundles, dname_linux);
                dpath_bundles_universal = Util.CombinePaths(dpath_bundles, dname_universal);
                dpath_bundles_os = Util.is_windows ? dpath_bundles_windows : dpath_bundles_linux;

                dpath_home.GetDir(true);
                dpath_builds_os.GetDir(true);
                dpath_bundles_texts.GetDir(true);
                dpath_bundles_os.GetDir(true);

                if (error != null)
                    Debug.LogError(error);
            }
        }
    }
}