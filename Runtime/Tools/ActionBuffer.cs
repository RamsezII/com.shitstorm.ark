using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ARK_
{
    public sealed class ActionBuffer : Disposable
    {
        readonly List<Action> stack = new();

        //----------------------------------------------------------------------------------------------------------

        public ActionBuffer(in string name) : base(name)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public void Add(int wait, in Action action)
        {
            for (int i = 0; i < wait - stack.Count; i++)
                stack.Insert(i, null);

            if (stack[^wait] == null)
                stack[^wait] = action;
            else
                stack[^wait] += action;
        }

        public void Execute()
        {
            if (stack.Count > 0)
            {
                stack[^1]?.Invoke();
                stack.RemoveAt(stack.Count - 1);
            }
        }
    }
}