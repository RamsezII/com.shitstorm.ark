using _UTIL_;
using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent2 : ArkComponent1
    {
        public int arkID;
        public readonly ValueNotifier<bool> isEnabled = new();

        static int _arkID;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _arkID = 0;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            arkID = ++_arkID;
            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();
            isEnabled.Value = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isEnabled.Value = false;
        }
    }
}