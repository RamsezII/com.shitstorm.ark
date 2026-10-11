using System;

namespace _ARK_
{
    public abstract partial class ArkShell : IDisposable
    {

        //----------------------------------------------------------------------------------------------------------

        void IDisposable.Dispose() => Clear();
        public virtual void Clear()
        {

        }
    }
}