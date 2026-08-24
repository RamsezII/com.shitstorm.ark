using UnityEngine;

namespace _ARK_
{
    public abstract class ArkComponent2 : ArkComponent1
    {
        static int _arkID;
        public int arkID;

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
    }
}