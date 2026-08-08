using System;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent1 : MonoBehaviour
    {
        public Action onStart, onEnable, onDisable, onDestroy;
        public bool _destroyed;

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
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