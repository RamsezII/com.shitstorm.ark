using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class ActionStack
    {
        readonly struct Command
        {
            public readonly Action execute;
            public readonly Action undo;

            //----------------------------------------------------------------------------------------------------------

            public Command(in Action execute, in Action undo)
            {
                this.execute = execute;
                this.undo = undo;
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

        public static void Push(in Action execute, in Action undo)
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
            --pointer;
            history[pointer].undo();
        }

        public static void Redo()
        {
            if (pointer >= history.Count)
                return;
            history[pointer].execute();
            ++pointer;
        }
    }
}