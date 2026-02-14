using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent : MonoBehaviour
    {
        public Action onStart, onEnable, onDisable, onDestroy;
        public int arkID;
        public bool _destroyed;
        public readonly ValueHandler<bool> isEnabled = new();

        static int _arkID;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _arkID = 0;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            arkID = ++_arkID;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            onEnable?.Invoke();
            isEnabled.Value = true;
        }

        protected virtual void OnDisable()
        {
            onDisable?.Invoke();
            isEnabled.Value = false;
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
            onDestroy?.Invoke();
        }
    }
}