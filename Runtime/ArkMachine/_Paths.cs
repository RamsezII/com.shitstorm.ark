using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    partial class ArkMachine
    {
        public const string
#if UNITY_EDITOR
            dname_ignore = "_IGNORE_",
            dname_resources = "Resources",
#endif
            dname_tools = "tools",
            dname_temp = "temp",
            dname_home = "home",
            dname_users = "users",
            dname_bundles = "bundles",
            dname_texts = "texts",
            dname_builds = "builds",
            dname_windows = "windows",
            dname_linux = "linux",
            dname_universal = "universal";

        public static readonly string
            dpath_terminal = Directory.GetCurrentDirectory();

        public static string NameOS => Util.IsAppWindows ? dname_windows : dname_linux;
        public static DirectoryInfo ExecDir => Application.dataPath.GetDir(false).Parent;
        public static FileInfo ExecFile => new(Path.Combine(ExecDir.FullName, Application.productName + (Util.IsAppWindows ? ".exe" : ".x86_64")));
        public static DirectoryInfo DRoot => IsPortableBuild() ? ExecDir : ExecDir.Parent.Parent.Parent;
        public static DirectoryInfo DFTools => ExecDir.Combine(dname_tools).ForceDir();
        public static DirectoryInfo DTemp => DFHome.Combine(dname_temp);
        public static DirectoryInfo DFTemp => DTemp.ForceDir();
        public static DirectoryInfo DFHome => new DirectoryInfo(Path.Combine(DRoot.FullName, dname_home)).ForceDir();
        public static string GetHomeJSonPath<T>() => GetHomeJSonPath(typeof(T));
        public static string GetHomeJSonPath(in Type type) => Path.Combine(DFHome.FullName, type.GetJSonFileName());
        public static DirectoryInfo DFUsers => new DirectoryInfo(Path.Combine(DFHome.FullName, dname_users)).ForceDir();
        public static DirectoryInfo DFBundles => new DirectoryInfo(Path.Combine(DFHome.FullName, dname_bundles)).ForceDir();
        public static DirectoryInfo DFBundlesTexts => new DirectoryInfo(Path.Combine(DFBundles.FullName, dname_texts)).ForceDir();
        public static DirectoryInfo DFBundlesWindows => new DirectoryInfo(Path.Combine(DFBundles.FullName, dname_windows)).ForceDir();
        public static DirectoryInfo DFBundlesLinux => new DirectoryInfo(Path.Combine(DFBundles.FullName, dname_linux)).ForceDir();
        public static DirectoryInfo DFBundlesUniversal => new DirectoryInfo(Path.Combine(DFBundles.FullName, dname_universal)).ForceDir();
        public static DirectoryInfo DFBundlesOS => new DirectoryInfo(Path.Combine(DFBundles.FullName, NameOS)).ForceDir();
        public static DirectoryInfo DFBuilds => new DirectoryInfo(Application.isEditor ? Path.Combine(DFHome.FullName, Application.productName, dname_builds) : Path.Combine(DRoot.FullName, dname_builds)).ForceDir();
        public static DirectoryInfo DFBuildsOS => new DirectoryInfo(Path.Combine(DFBuilds.FullName, NameOS)).ForceDir();
        public static DirectoryInfo DFBuildsWindows => new DirectoryInfo(Path.Combine(DFBuilds.FullName, dname_windows)).ForceDir();
        public static DirectoryInfo DFBuildsLinux => new DirectoryInfo(Path.Combine(DFBuilds.FullName, dname_linux)).ForceDir();
        public static DirectoryInfo DFBuildsUniversal => new DirectoryInfo(Path.Combine(DFBuilds.FullName, dname_universal)).ForceDir();

#if UNITY_EDITOR
        public static DirectoryInfo DFIgnore => new DirectoryInfo(Path.Combine(Application.dataPath, dname_ignore)).ForceDir();
        public static DirectoryInfo DEditorTemp => new(Path.Combine(DFHome.FullName, "EditorTemp"));
        public static DirectoryInfo DFEditorTemp => DEditorTemp.ForceDir();
        public static DirectoryInfo DIgnoreTemp => DFIgnore.Combine("_TEMP_");
        public static DirectoryInfo DFIgnoreTemp => DIgnoreTemp.ForceDir();
        public static DirectoryInfo DFResources => new DirectoryInfo(Path.Combine(Application.dataPath, dname_resources)).ForceDir();
        public static DirectoryInfo DFIgnoreResources => new DirectoryInfo(Path.Combine(Application.dataPath, dname_ignore, dname_resources)).ForceDir();
#endif

        //----------------------------------------------------------------------------------------------------------

        static bool IsPortableBuild()
        {
            if (!Application.isEditor)
            {
                var build_dir = Application.dataPath.GetDir(false).Parent;

                if (build_dir.Parent == null || build_dir.Parent.Parent == null || build_dir.Parent.Parent.Parent == null)
                    return true;

                string dpath_root = build_dir.Parent.Parent.Parent.FullName;
                string dpath_app_expected = Path.Combine(dpath_root, dname_builds, NameOS, ExecDir.Name).NormalizePath();

                if (Util.IsSamePath_full(dpath_app_expected, ExecDir.FullName))
                    return false;
            }

            return true;
        }

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(OpenHomeFolder))]
        static void OpenHomeFolder() => Application.OpenURL(DFHome.FullName);
#endif
    }
}