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
            if (this is IHomeTexts iuser)
                IHomeTexts.AddUser(iuser);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            _destroyed = true;
            onDestroy?.Invoke();
            if (this is IHomeTexts iuser)
                IHomeTexts.RemoveUser(iuser);
        }
    }
}