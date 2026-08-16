using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public interface IHomeTexts
    {
        static readonly HashSet<IHomeTexts> instances = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            instances.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += () =>
            {
                foreach (var instance in instances)
                    instance.OnLoadHTexts(log: false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += () =>
            {
                foreach (var instance in instances)
                    instance.OnSaveHTexts(log: false);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        public static void AddUser(IHomeTexts user)
        {
            user.OnLoadHTexts(true);
            instances.Add(user);
        }

        public static void RemoveUser(IHomeTexts user)
        {
            instances.Remove(user);
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnSaveHTexts(in bool log);
        void OnLoadHTexts(in bool log);
    }
}