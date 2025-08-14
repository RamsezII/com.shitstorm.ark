using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class Scheduler
    {
        public class Task
        {
            public readonly UnityEngine.Object memlink;
            public readonly float time;
            public readonly Action<float> action;
            public Task next;

            //----------------------------------------------------------------------------------------------------------

            public Task(in float time, in Action<float> action, in UnityEngine.Object memlink = null)
            {
                this.memlink = memlink;
                this.time = time;
                this.action = action;
                next = null;
            }

            //----------------------------------------------------------------------------------------------------------

            public void Enqueue(in Task task)
            {
                if (next == null)
                    next = task;
                else if (next.time >= task.time)
                {
                    task.next = next;
                    next = task;
                }
                else
                    next.Enqueue(task);
            }
        }

        [SerializeField] Task next;

        //----------------------------------------------------------------------------------------------------------

        public Task Add(in float time, in Action<float> action)
        {
            Task task = new(time, action);
            if (next == null)
                next = task;
            else
                next.Enqueue(task);
            return task;
        }

        public void Tick(in float time)
        {
            while (next != null)
                if (time < next.time)
                    break;
                else
                {
                    next.action(next.time - time);
                    next = next.next;
                }
        }
    }
}