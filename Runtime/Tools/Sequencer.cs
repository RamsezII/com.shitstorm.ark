using _UTIL_;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace _ARK_
{
    public abstract class Sequencer : Disposable
    {
        public readonly ListListener<Sequencable> sequencables = new();

        //----------------------------------------------------------------------------------------------------------

        protected Sequencer(in string name) : base(name)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public void LogStatus() => Debug.Log(GetStatus());
        public string GetStatus()
        {
            StringBuilder log = new();
            log.AppendLine($"{this} -> {sequencables._collection.Count} {nameof(sequencables)}");
            lock (sequencables)
                for (int i = 0; i < sequencables._collection.Count; i++)
                    if (sequencables._collection[i] is Sequencable sequencable)
                        log.AppendLine($"{i}. {sequencable.GetType().FullName}.{nameof(sequencable.description)}:\n{sequencable.description}");
                    else
                        log.AppendLine($"{i}. {sequencables._collection[i].GetType().FullName}");
            return log.TroncatedForLog();
        }

        public Sequencable AddAction(in Action action, [CallerMemberName] string callerName = null) => AddSequencable(new Sequencable(callerName) { action = action });
        public Sequencable AddRoutine(in IEnumerator<float> routine, [CallerMemberName] string callerName = null) => AddSequencable(new Sequencable(callerName) { routine = routine });
        public Sequencable AddRoutine(in IEnumerator routine, [CallerMemberName] string callerName = null) => AddSequencable(new Sequencable(callerName) { routine = routine.ESchedulize() });

        public T AddSequencable<T>(in T sequencable) where T : Sequencable
        {
            lock (sequencables)
                if (sequencables._collection.Contains(sequencable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(sequencable)}) -> {sequencable} is already scheduled");
                else
                    sequencables.AddElement(sequencable);
            return sequencable;
        }

        public abstract void Tick();

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            lock (sequencables)
            {
                for (int i = 0; i < sequencables._collection.Count; i++)
                    sequencables._collection[i].Dispose();
                sequencables.Clear();
            }
        }
    }

    public sealed class SequencerMono : Sequencer
    {
        public readonly ThreadSafe_struct<bool> isTick = new();

        //----------------------------------------------------------------------------------------------------------

        public SequencerMono() : base(typeof(SequencerMono).FullName)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public Sequencable InsertAction(in Action action, [CallerMemberName] string callerName = null) => InsertSchedulable(new Sequencable(callerName) { action = action });
        public Sequencable InsertRoutine(in IEnumerator<float> routine, [CallerMemberName] string callerName = null) => InsertSchedulable(new Sequencable(callerName) { routine = routine });

        public T InsertSchedulable<T>(in T schedulable) where T : Sequencable
        {
            lock (sequencables)
                if (sequencables._collection.Contains(schedulable))
                    throw new Exception($"{this}.{nameof(AddRoutine)}({nameof(schedulable)}) -> {schedulable} is already scheduled");
                else
                    sequencables.InsertElementAt(0, schedulable);
            return schedulable;
        }

        public override void Tick()
        {
            lock (sequencables)
                if (sequencables._collection.Count > 0)
                {
                    Sequencable schedulable = sequencables._collection[0];
                    if (schedulable == null)
                    {
                        sequencables.RemoveElementAt(0);
                        Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(sequencables)}[0] was null");
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
                                        schedulable.OnFirstThick();
                                    }

                                schedulable.OnTick();

                                if (schedulable._disposed)
                                    sequencables.RemoveElement(schedulable);
                            }
                        }
                        catch (Exception e)
                        {
                            sequencables.RemoveElement(schedulable);
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{schedulable.description}");
                            Debug.LogException(e);
                        }

                        isTick.Value = false;
                    }
                }
        }
    }

    public sealed class SequencerMulti : Sequencer
    {
        public readonly HashSet<Queue<IEnumerator<float>>> queues = new();

        //----------------------------------------------------------------------------------------------------------

        public SequencerMulti() : base(typeof(SequencerMulti).FullName)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public override void Tick()
        {
            lock (sequencables)
                if (sequencables._collection.Count > 0)
                    for (int i = 0; i < sequencables._collection.Count; i++)
                    {
                        Sequencable schedulable = sequencables._collection[i];

                        try
                        {
                            lock (schedulable)
                            {
                                lock (schedulable.scheduled)
                                    if (!schedulable.scheduled._value)
                                    {
                                        schedulable.scheduled._value = true;
                                        schedulable.OnFirstThick();
                                    }

                                schedulable.OnTick();

                                if (schedulable._disposed)
                                    sequencables.RemoveElementAt(i--);
                            }
                        }
                        catch (Exception e)
                        {
                            sequencables.RemoveElementAt(i--);
                            Debug.LogError($"{this}.{nameof(Tick)}() -> {nameof(schedulable)}:\n{schedulable.description}");
                            Debug.LogException(e);
                        }
                    }

            lock (queues)
                foreach (var queue in queues)
                    if (queue.TryPeek(out var routine))
                        if (!routine.MoveNext())
                        {
                            routine.Dispose();
                            queue.Dequeue();
                        }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            lock (queues)
            {
                foreach (var queue in queues)
                    while (queue.TryDequeue(out var routine))
                        routine.Dispose();
                queues.Clear();
            }
        }
    }
}