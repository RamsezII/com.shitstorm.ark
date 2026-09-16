using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public abstract partial class ArkComponent2 : ArkComponent1
    {
        public static readonly HashSet<ArkComponent2> instances2 = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            instances2.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationUnfocus += () =>
            {
                foreach (var instance in instances2)
                    if (instance.CandidateForTextSave)
                        instance.SaveArkTexts(log: false, reloadAllTextsAfterSave: false);
            };

            NUCLEOR.delegates.OnApplicationFocus += () =>
            {
                foreach (var instance in instances2)
                    instance.LoadArkTexts(log: false);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            instances2.Add(this);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            instances2.Remove(this);
        }
    }
}
