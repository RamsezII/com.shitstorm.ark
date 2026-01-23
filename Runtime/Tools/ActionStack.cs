using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public sealed class ActionStack : ArkComponent
    {
        readonly struct Command
        {
            internal readonly Action execute, undo;

            //----------------------------------------------------------------------------------------------------------

            internal Command(in Action execute, in Action undo)
            {
                this.execute = execute;
                this.undo = undo;
            }

            internal Command(in Command other) : this(other.execute, other.undo)
            {
            }
        }

        readonly List<Command> history = new();
        int pointer;

        public static ActionStack focused;
        public Func<bool> hasFocus;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            focused = null;
        }

        //----------------------------------------------------------------------------------------------------------

        public void Do(in Action execute, in Action undo)
        {
            if (pointer < history.Count)
                history.RemoveRange(pointer, history.Count - pointer);
            history.Add(new Command(execute, undo));
            pointer = history.Count;
        }

        public void TakeFocus() => focused = this;
        public void UntakeFocus()
        {
            if (this == focused)
                focused = null;
        }

        public static void Undo() => focused?._Undo();
        void _Undo()
        {
            if (_destroyed || !hasFocus() || pointer == 0)
                return;
            history[--pointer].undo();
        }

        public static void Redo() => focused?._Redo();
        void _Redo()
        {
            if (_destroyed || !hasFocus() || pointer >= history.Count)
                return;
            history[pointer++].execute();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            history.Clear();
            pointer = 0;
        }
    }
}