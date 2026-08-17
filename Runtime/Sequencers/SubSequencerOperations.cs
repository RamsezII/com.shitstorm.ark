using _UTIL_;
using System;
using System.Collections.Generic;
using System.Text;

namespace _ARK_
{

    public abstract class SubSequencerOperation : Disposable
    {
        public readonly SubSequencer sequencer;

        //----------------------------------------------------------------------------------------------------------

        internal SubSequencerOperation(in SubSequencer sequencer, in string name) : base(name)
        {
            this.sequencer = sequencer;
        }

        //----------------------------------------------------------------------------------------------------------

        public abstract void GetStatus(in StringBuilder sb);

        public abstract bool MoveNext();
    }

    public sealed class SubSequencerRoutine : SubSequencerOperation
    {
        public readonly IEnumerator<float> routine;
        internal readonly Action<float> onProgress;
        internal readonly Action onDone;

        //----------------------------------------------------------------------------------------------------------

        internal SubSequencerRoutine(in SubSequencer sequencer, in string name, in IEnumerator<float> routine, in Action<float> onProgress, in Action onDone) : base(sequencer, name)
        {
            this.routine = routine;
            this.onProgress = onProgress;
            this.onDone = onDone;
        }

        //----------------------------------------------------------------------------------------------------------

        public override void GetStatus(in StringBuilder sb)
        {
            sb.Append($"{GetType()}'{name}'");
            if (_disposed)
                sb.Append("[disposed]");
            else
                sb.Append(routine.Current.PercentLog());
            sb.AppendLine();
        }

        public override bool MoveNext()
        {
            if (!routine.MoveNext())
            {
                onDone?.Invoke();
                Dispose();
                return false;
            }
            return true;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            routine.Dispose();
        }
    }

    public sealed class SubSequencerAction : SubSequencerOperation
    {
        internal readonly Action action;

        //----------------------------------------------------------------------------------------------------------

        internal SubSequencerAction(in SubSequencer sequencer, in string name, in Action action) : base(sequencer, name)
        {
            this.action = action;
        }

        //----------------------------------------------------------------------------------------------------------

        public override void GetStatus(in StringBuilder sb)
        {
            sb.Append($"{GetType()}'{name}'");
            if (_disposed)
                sb.Append("[disposed]");
            sb.AppendLine();
        }

        public override bool MoveNext()
        {
            action?.Invoke();
            return false;
        }
    }
}