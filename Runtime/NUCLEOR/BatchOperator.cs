using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public sealed class BatchOperator
    {
        public readonly List<Action> actions = new();
        [SerializeField] int index;
        [SerializeField, Range(0, 1)] float accumulator;

        //----------------------------------------------------------------------------------------------------------

        public void Tick(in float delta)
        {
            if (actions.Count == 0)
                return;

            accumulator += delta;
            int count = (int)accumulator;

            if (count == 0)
                return;

            accumulator -= count;

            count = Mathf.Min(count, actions.Count);
            int start = index;
            index += count;

            for (int i = 0; i < count; i++)
            {
                int index = (start + i) % actions.Count;
                actions[index]();
            }
        }
    }
}
