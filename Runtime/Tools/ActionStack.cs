using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class ActionStack
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

        static readonly List<Command> history = new();
        static int pointer;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            history.Clear();
            pointer = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public static void Do(in Action execute, in Action undo)
        {
            if (pointer < history.Count)
                history.RemoveRange(pointer, history.Count - pointer);
            history.Add(new Command(execute, undo));
            pointer = history.Count;
        }

        public static void Undo()
        {
            if (pointer == 0)
                return;
            history[--pointer].undo();
        }

        public static void Redo()
        {
            if (pointer >= history.Count)
                return;
            history[pointer++].execute();
        }
    }
}