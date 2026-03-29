using System;
using System.Collections.Generic;

namespace _ARK_.tools
{
    public sealed class LoopSequencer
    {
        public class Task
        {
            internal readonly float time;
            public Action action1;
            public Action<float> action2;

            //----------------------------------------------------------------------------------------------------------

            public Task(in float time)
            {
                this.time = time;
            }
        }

        readonly List<Task> _tasks = new();
        float _time;

        //----------------------------------------------------------------------------------------------------------

        public Task AddTask(in float time)
        {
            Task task = new Task(time);
            _tasks.Add(task);
            return task;
        }

        public void Update(in float dtime)
        {
            float ntime = _time + dtime;
            float ntime01 = ntime % 1;

            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                if (ntime >= task.time && _time < task.time)
                {
                    task.action1?.Invoke();
                    task.action2?.Invoke(ntime - task.time);
                }
                else if (ntime01 >= task.time && _time > task.time)
                {
                    task.action1?.Invoke();
                    task.action2?.Invoke(ntime01 - task.time);
                }
            }

            _time = ntime01;
        }
    }
}