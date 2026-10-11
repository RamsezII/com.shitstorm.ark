using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;

namespace _ARK_
{
    [Serializable]
    public sealed partial class CodeInterpreter
    {
        [AutoStaticsCleanup] public static readonly HashSet<CodeInterpreter> instances = new();

        public delegate void Linter(in string text, in int index, in LintTheme lint_theme, out string lint_text, out string error);
        public Linter linter;
        public Func<string, object> execution;
        public readonly string name, extension;

        //--------------------------------------------------------------------------------------------------------------

        public CodeInterpreter(in string name, in string extension)
        {
            this.name = name;
            this.extension = extension;
        }
    }
}