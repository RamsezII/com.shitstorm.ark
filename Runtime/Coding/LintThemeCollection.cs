using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace _ARK_
{
    [Serializable, CreateAssetMenu(fileName = "LintThemeCollection", menuName = "ARK/Lint Theme Collection")]
    public sealed class LintThemeCollection : ScriptableObject
    {
        [SerializeField] LintTheme[] _themes = Array.Empty<LintTheme>();
        public IReadOnlyList<LintTheme> themes => _themes;

        public LintTheme this[string name] => Array.Find(_themes, theme => theme != null && string.Equals(((UnityEngine.Object)theme).name, name, StringComparison.OrdinalIgnoreCase));
        public LintTheme theme_dark => this["Dark"];
        public LintTheme theme_light => this["Light"];

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void InitializeAutoFill()
        {
            EditorApplication.projectChanged += RefreshLintThemeCollections;
            EditorApplication.delayCall += RefreshLintThemeCollections;
        }

        [MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(RefreshLintThemeCollections))]
        static void RefreshLintThemeCollections()
        {
            // Persist references in the editor so prefabs include their themes in player builds.
            var themes = AssetDatabase.FindAssets("t:LintTheme", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath).OrderBy(path => path, StringComparer.Ordinal).Select(AssetDatabase.LoadAssetAtPath<LintTheme>).ToArray();
            foreach (string guid in AssetDatabase.FindAssets("t:LintThemeCollection", new[] { "Assets" }))
            {
                var collection = AssetDatabase.LoadAssetAtPath<LintThemeCollection>(AssetDatabase.GUIDToAssetPath(guid));
                if (collection._themes.SequenceEqual(themes))
                    continue;

                collection._themes = themes;
                EditorUtility.SetDirty(collection);
                AssetDatabase.SaveAssetIfDirty(collection);
            }
        }
#endif
    }
}
