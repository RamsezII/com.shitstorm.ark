using _UTIL_;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public class RuntimeInfo : Disposable
    {
        static readonly HashSet<RuntimeInfo> instances = new();
        public readonly static IEnumerable<RuntimeInfo> EInstances = instances;

        public Traductions infos;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            instances.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

        public RuntimeInfo(in Traductions infos = default) : base(infos.GetAutomatic())
        {
            this.infos = infos;
            instances.Add(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            instances.Remove(this);
        }
    }
}