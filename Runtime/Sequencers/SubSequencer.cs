using _UTIL_;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _ARK_
{
    public sealed class SubSequencer : Disposable
    {
        public readonly SequencerMulti base_sequencer;
        readonly Queue<SubSequencerOperation> operations = new();

        //----------------------------------------------------------------------------------------------------------

        internal SubSequencer(in SequencerMulti base_sequencer, in string name) : base(name)
        {
            this.base_sequencer = base_sequencer;
        }

        //----------------------------------------------------------------------------------------------------------

        static IEnumerator<float> EBlock(SubSequencer sequ)
        {
            try
            {
                Debug.Log($"{sequ}.{nameof(EBlock)} START".ToSubLog());
                while (true)
                    yield return 0;
            }
            finally
            {
                Debug.Log($"{sequ}.{nameof(EBlock)} STOP".ToSubLog());
            }
        }

        public void GetStatus(in StringBuilder sb)
        {
            if (operations.Count > 0)
            {
                sb.AppendLine($"{GetType()}'{name}'({operations.Count} operations):");
                foreach (var op in operations)
                    op.GetStatus(sb);
            }
        }

        public SubSequencerRoutine AddBlock() => AddRoutine($"{this}.{nameof(EBlock)}", EBlock(this));
        public SubSequencerRoutine AddRoutine(in string name, in IEnumerator<float> routine, in Action onDone = null, in Action<float> onProgress = null)
        {
            var op = new SubSequencerRoutine(this, name, routine, onProgress, onDone);
            operations.Enqueue(op);
            return op;
        }

        public SubSequencerAction AddAction(in string name, in Action action)
        {
            var op = new SubSequencerAction(this, name, action);
            operations.Enqueue(op);
            return op;
        }

        internal void Tick()
        {
            if (operations.TryPeek(out var op))
                lock (op)
                    if (op._disposed)
                        operations.Dequeue();
                    else if (!op.MoveNext())
                        operations.Dequeue();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var op in operations)
                op.Dispose();
            operations.Clear();
        }
    }

}