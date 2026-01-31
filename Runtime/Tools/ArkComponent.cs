using System;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent : MonoBehaviour
    {
        public Action onDestroy;
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