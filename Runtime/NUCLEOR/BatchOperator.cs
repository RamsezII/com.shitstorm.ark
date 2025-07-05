using System;
using System.Collections.Generic;

namespace _ARK_
{
    public sealed class BatchOperator
    {
        readonly List<(object user, Action action)> actions = new();
        int current;

        //----------------------------------------------------------------------------------------------------------

        public void AddTask(in object user, in Action action)
        {
            actions.Add((user, action));
        }

        internal void Tick()
        {
            if (actions.Count == 0)
                return;

            current = (current + 1) % actions.Count;

            (object user, Action action) = actions[current];

            if (user == null)
                actions.RemoveAt(current);
            else
                actions[current].action();
        }
    }
}
