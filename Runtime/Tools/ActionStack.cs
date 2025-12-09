using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class ActionStack
    {
        readonly struct Command
        {
            internal readonly Func<object> execute;
            internal readonly Action<object> undo;
            internal readonly object _reference;

            //----------------------------------------------------------------------------------------------------------

            internal Command(in Command other)
            {
                execute = other.execute;
                undo = other.undo;
                _reference = other.execute();
            }

            internal Command(in Func<object> execute, in Action<object> undo)
            {
                this.execute = execute;
                this.undo = undo;
                _reference = execute();
            }
        }

        static readonly List<Command> history = new();
        public static readonly Dictionary<object, object> references = new();
        static int pointer;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            history.Clear();
            references.Clear();
            pointer = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public static void Do(in Func<object> execute, in Action<object> undo)
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
            Command command = history[pointer];
            command.undo(command._reference);
        }

        public static void Redo()
        {
            if (pointer >= history.Count)
                return;
            Command command = history[pointer];
            history[pointer] = new Command(command);
            ++pointer;
        }
    }
}