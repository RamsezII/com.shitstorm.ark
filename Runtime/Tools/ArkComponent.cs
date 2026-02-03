using System;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent : MonoBehaviour
    {
        public Action onStart, onEnable, onDisable, onDestroy;
        public int arkID;
        public bool _destroyed;

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
            onDestroy?.Invoke();
        }
    }
}