using _UTIL_;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _ARK_
{
    public abstract class Scheduler : Disposable
    {
        public readonly ListListener<Schedulable> list = new();

        //----------------------------------------------------------------------------------------------------------

        public void LogStatus() => Debug.Log(GetStatus());
        public string GetStatus()
        {
            StringBuilder log = new();
            log.AppendLine($"{this} -> {list._list.Count} schedulables");
            lock (list)
                for (int i = 0; i < list._list.Count; i++)
                    if (list._list[i] is Schedulable schedulable)
                        log.AppendLine($"{i}. {schedulable.GetType().FullName}.{nameof(schedulable.description)}:\n{schedulable.description}");
                    else
                        log.AppendLine($"{i}. {list._list[i].GetType().FullName}");
            return log.TroncatedForLog();
        }

        public Schedulable AddAction(in Action action, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { action = action });
        public Schedulable AddFunc(in Func<bool> moveNext, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { moveNext = moveNext });
        public Schedulable AddRoutine(in IEnumerator<float> routine, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { routine = routine });
        public Schedulable AddTask(in Action task, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { _task = task });
        public Schedulable AddTask(in Task task, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { task = task });

        public T AddSchedulable<T>(in T schedulable) where T : Schedulable
        {
            lock (list)
                if (list._list.Contains(schedulable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(schedulable)}) -> {schedulable} is already scheduled");
                else
                    list.AddElement(schedulable);
            return schedulable;
        }

        public abstract void Tick();

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            lock (list)
            {
                foreach (Schedulable schedulable in list._list)
                    schedulable.Dispose();
                list.ClearList();
            }
        }
    }

    public class SequentialScheduler : Scheduler
    {
        public readonly ThreadSafe_struct<bool> isTick = new();

        //----------------------------------------------------------------------------------------------------------

        public Schedulable InsertAction(in Action action, [CallerMemberName] string callerName = null) => InsertSchedulable(new Schedulable(callerName) { action = action });
        public Schedulable InsertFunc(in Func<bool> moveNext, [CallerMemberName] string callerName = null) => InsertSchedulable(new Schedulable(callerName) { moveNext = moveNext });
        public Schedulable InsertRoutine(in IEnumerator<float> routine, [CallerMemberName] string callerName = null) => InsertSchedulable(new Schedulable(callerName) { routine = routine });
        public Schedulable InsertTask(in Action task, [CallerMemberName] string callerName = null) => InsertSchedulable(new Schedulable(callerName) { _task = task });
        public Schedulable InsertTask(in Task task, [CallerMemberName] string callerName = null) => InsertSchedulable(new Schedulable(callerName) { task = task });

        public T InsertSchedulable<T>(in T schedulable) where T : Schedulable
        {
            lock (list)
                if (list._list.Contains(schedulable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(schedulable)}) -> {schedulable} is already scheduled");
                else
                    list.InsertElementAt(0, schedulable);
            return schedulable;
        }

        public override void Tick()
        {
            lock (list)
                if (list._list.Count > 0)
                {
                    Schedulable schedulable = list._list[0];
                    if (schedulable == null)
                    {
                        list.RemoveElementAt(0);
                        Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(list)}[0] was null");
                    }
                    else
                    {
                        isTick.Value = true;

                        try
                        {
                            lock (schedulable)
                            {
                                lock (schedulable.scheduled)
                                    if (!schedulable.scheduled._value)
                                    {
                                        schedulable.scheduled._value = true;
                                        schedulable.OnSchedule();
                                    }

                                schedulable.OnTick();

                                if (schedulable.Disposed)
                                    list.RemoveElement(schedulable);
                            }
                        }
                        catch (Exception e)
                        {
                            list.RemoveElement(schedulable);
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{schedulable.description}");
                            Debug.LogException(e);
                        }

                        isTick.Value = false;
                    }
                }
        }
    }

    public class ParallelScheduler : Scheduler
    {
        public override void Tick()
        {
            lock (list)
                if (list._list.Count > 0)
                    for (int i = 0; i < list._list.Count; i++)
                    {
                        Schedulable schedulable = list._list[i];

                        try
                        {
                            lock (schedulable)
                            {
                                lock (schedulable.scheduled)
                                    if (!schedulable.scheduled._value)
                                    {
                                        schedulable.scheduled._value = true;
                                        schedulable.OnSchedule();
                                    }

                                schedulable.OnTick();

                                if (schedulable.Disposed)
                                    list.RemoveElementAt(i--);
                            }
                        }
                        catch (Exception e)
                        {
                            list.RemoveElementAt(i--);
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{schedulable.description}");
                            Debug.LogException(e);
                        }
                    }
        }
    }
}