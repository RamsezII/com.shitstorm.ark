using _UTIL_;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
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
        public string callerName, description;
        public IEnumerator routine;
        public Func<bool> moveNext;
        public Action action, _task;
        public Task task;
        public bool Scheduled { get; set; }

        static int _id;
        [SerializeField] int id = ++_id;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public Schedulable([CallerMemberName] string callerName = null)
        {
            this.callerName = callerName;

            if (ArkOS.instance.settings.keepSchedulableStackTraces)
            {
                StringBuilder log = new();
                StackTrace stackTrace = new();

                for (int i = stackTrace.FrameCount - 1; i > 0; i--)
                {
                    StackFrame stackFrame = stackTrace.GetFrame(i);
                    var method = stackFrame.GetMethod();
                    log.AppendLine($"{new string(' ', 2 + 2 * (stackTrace.FrameCount - i))}{method.DeclaringType?.FullName ?? "¤"}.{method.Name}");
                }

                description = log.ToString()[..^1];
                if (ArkOS.instance.settings.logNewSchedulables)
                    UnityEngine.Debug.Log(description.ToSubLog());
            }
            else if (ArkOS.instance.settings.logNewSchedulables)
                UnityEngine.Debug.Log($"{GetType().FullName}[{id}](\"{callerName}\")".ToSubLog());
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnSchedule()
        {
            try
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
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{this}.{nameof(OnSchedule)}() -> {nameof(description)}:\n{description}");
                UnityEngine.Debug.LogException(e);
                Dispose();
            }
        }

        public virtual void OnTick()
        {
            try
            {
                if (moveNext != null && !moveNext())
                    Dispose();
                if (routine != null && !routine.MoveNext())
                    Dispose();
                if (task != null && task.IsCompleted)
                    Dispose();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{this}.{nameof(OnTick)}() -> {nameof(description)}:\n{description}");
                UnityEngine.Debug.LogException(e);
                Dispose();
            }
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
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}