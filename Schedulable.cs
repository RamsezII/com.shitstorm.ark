using _UTIL_;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace _ARK_
{
    public interface ISchedulable : IDisposable
    {
        bool Scheduled { set; get; }
        bool Disposed { get; }
        void OnSchedule();
        void OnTick();
    }

    public class Schedulock : Disposable, ISchedulable
    {
        bool ISchedulable.Scheduled { get; set; }
        void ISchedulable.OnTick()
        {
        }
        void ISchedulable.OnSchedule()
        {
        }
    }

    public class Schedulable : Disposable, ISchedulable
    {
        public IEnumerator routine;
        public Func<bool> moveNext;
        public Action action, _task;
        public Task task;
        public bool Scheduled { get; set; }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnSchedule()
        {
            if (action != null)
            {
                action();
                Dispose();
            }

            if (_task != null)
                task = Task.Run(_task);

            moveNext?.Invoke();
            routine?.MoveNext();
        }

        public virtual void OnTick()
        {
            if (moveNext != null && !moveNext())
                Dispose();
            if (routine != null && !routine.MoveNext())
                Dispose();
            if (task != null && task.IsCompleted)
                Dispose();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            try
            {
                task?.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}