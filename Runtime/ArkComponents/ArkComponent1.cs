using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public abstract partial class ArkComponent1 : MonoBehaviour
    {
        public static readonly HashSet<ArkComponent1> instances1 = new();

        public Action onStart, onEnable, onDisable, onDestroy;
        public bool _destroyed;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            instances1.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            instances1.Add(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            onEnable?.Invoke();
        }

        protected virtual void OnDisable()
        {
            onDisable?.Invoke();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
            onStart?.Invoke();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            instances1.Remove(this);
            onDestroy?.Invoke();
        }
    }
}