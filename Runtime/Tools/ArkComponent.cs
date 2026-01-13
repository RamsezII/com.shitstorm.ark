using System;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent : MonoBehaviour
    {
        public Action onDestroy;
        public bool _destroyed;

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            onDestroy?.Invoke();
        }
    }
}