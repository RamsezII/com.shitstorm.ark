using _UTIL_;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _ARK_
{
    public abstract class Scheduler : Disposable
    {
        public bool IsBusy => list.Count > 0;
        public readonly List<ISchedulable> list = new();

        //----------------------------------------------------------------------------------------------------------

        public void LogStatus() => Debug.Log(GetStatus());
        public string GetStatus()
        {
            StringBuilder log = new();
            log.AppendLine($"{this} -> {list.Count} schedulables");
            lock (list)
                for (int i = 0; i < list.Count; i++)
                    if (list[i] is Schedulable schedulable)
                        log.AppendLine($"{i}. {schedulable.GetType().FullName}.{nameof(schedulable.description)}:\n{schedulable.description}");
                    else
                        log.AppendLine($"{i}. {list[i].GetType().FullName}");
            return log.ToString()[..^1];
        }

        public T AddSchedulable<T>(in T schedulable) where T : ISchedulable
        {
            AddISchedulable(schedulable);
            return schedulable;
        }

        public Schedulable AddAction(in Action action, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { action = action });
        public Schedulable AddFunc(in Func<bool> moveNext, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { moveNext = moveNext });
        public Schedulable AddRoutine(in IEnumerator routine, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { routine = routine });
        public Schedulable AddTask(in Action task, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { _task = task });
        public Schedulable AddTask(in Task task, [CallerMemberName] string callerName = null) => AddSchedulable(new Schedulable(callerName) { task = task });

        public T AddISchedulable<T>(in T schedulable) where T : ISchedulable
        {
            lock (list)
                if (list.Contains(schedulable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(schedulable)}) -> {schedulable} is already scheduled");
                else
                    list.Add(schedulable);
            return schedulable;
        }

        public abstract void Tick();

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            lock (list)
            {
                foreach (ISchedulable schedulable in list)
                    schedulable.Dispose();
                list.Clear();
            }
        }
    }

    public class SequentialScheduler : Scheduler
    {
        public readonly ThreadSafe<bool> isTick = new();

        //----------------------------------------------------------------------------------------------------------

        public Schedulable InsertAction(in Action action, [CallerMemberName] string callerName = null) => InsertISchedulable(new Schedulable(callerName) { action = action });
        public Schedulable InsertFunc(in Func<bool> moveNext, [CallerMemberName] string callerName = null) => InsertISchedulable(new Schedulable(callerName) { moveNext = moveNext });
        public Schedulable InsertRoutine(in IEnumerator routine, [CallerMemberName] string callerName = null) => InsertISchedulable(new Schedulable(callerName) { routine = routine });
        public Schedulable InsertTask(in Action task, [CallerMemberName] string callerName = null) => InsertISchedulable(new Schedulable(callerName) { _task = task });
        public Schedulable InsertTask(in Task task, [CallerMemberName] string callerName = null) => InsertISchedulable(new Schedulable(callerName) { task = task });

        public T InsertISchedulable<T>(in T schedulable) where T : ISchedulable
        {
            lock (list)
                if (list.Contains(schedulable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(schedulable)}) -> {schedulable} is already scheduled");
                else
                    list.Insert(0, schedulable);
            return schedulable;
        }

        public override void Tick()
        {
            lock (list)
                if (list.Count > 0)
                {
                    ISchedulable schedulable = list[0];
                    isTick.Value = true;

                    try
                    {
                        lock (schedulable)
                        {
                            if (!schedulable.Scheduled)
                            {
                                schedulable.Scheduled = true;
                                schedulable.OnSchedule();
                            }

                            schedulable.OnTick();

                            if (schedulable.Disposed)
                                list.RemoveAt(0);
                        }
                    }
                    catch (Exception e)
                    {
                        list.RemoveAt(0);

                        if (schedulable is Schedulable s)
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{s.description}");
                        else
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}: {schedulable}");

                        Debug.LogException(e);
                    }

                    isTick.Value = false;
                }
        }
    }

    public class ParallelScheduler : Scheduler
    {
        public override void Tick()
        {
            lock (list)
                if (list.Count > 0)
                    for (int i = 0; i < list.Count; i++)
                    {
                        ISchedulable schedulable = list[i];

                        try
                        {
                            lock (schedulable)
                            {
                                if (!schedulable.Scheduled)
                                {
                                    schedulable.Scheduled = true;
                                    schedulable.OnSchedule();
                                }

                                schedulable.OnTick();

                                if (schedulable.Disposed)
                                    list.RemoveAt(i--);
                            }
                        }
                        catch (Exception e)
                        {
                            list.RemoveAt(i--);
                            if (schedulable is Schedulable s)
                                Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{s.description}");
                            else
                                Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}: {schedulable}");
                            Debug.LogException(e);
                        }
                    }
        }
    }
}