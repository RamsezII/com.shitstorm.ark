using _UTIL_;
using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;

namespace _ARK_
{
    [Serializable]
    public sealed partial class CodeInterpreter
    {
        [AutoStaticsCleanup] public static readonly Dictionary<string, CodeInterpreter> instances = new(StringComparer.OrdinalIgnoreCase);

        public delegate void Linter(in string text, in int index, in LintTheme lint_theme, out string lint_text, out string error);
        public Linter linter;

        //--------------------------------------------------------------------------------------------------------------

    }
}