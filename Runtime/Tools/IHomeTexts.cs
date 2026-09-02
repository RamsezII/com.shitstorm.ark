using _UTIL_;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class Util_HomeTexts
    {
        public static void SaveHomeText(this IHomeTexts target, in bool log)
        {
            JObject jobj = new();
            jobj.WriteFields<NJTextAttribute>(target);
            target.OnSaveHTexts(jobj, log: log);
            jobj.NJSave(NUCLEOR.GetHomeJSonPath(target.GetType()), log: log);
        }

        public static void LoadHomeText(this IHomeTexts target, in bool log)
        {
            string lpath = NUCLEOR.GetHomeJSonPath(target.GetType());
            lpath.TryNJRead(out JObject jobj, force: true, log_success: log);
            jobj.ReadFields<NJTextAttribute>(target);
            target.OnLoadHTexts(jobj, log: log);
        }
    }

    public interface IHomeTexts
    {
        public static readonly HashSet<IHomeTexts> _users = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            _users.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += () =>
            {
                foreach (var instance in _users)
                    instance.LoadHomeText(log: false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += () =>
            {
                foreach (var instance in _users)
                    instance.SaveHomeText(log: false);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        public static void AddUser(IHomeTexts user)
        {
            user.LoadHomeText(log: false);
            _users.Add(user);
        }

        public static void RemoveUser(IHomeTexts user)
        {
            _users.Remove(user);
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveHomeText))]
        void SaveHomeText() => this.SaveHomeText(log: true);

        [ContextMenu(nameof(LoadHomeText))]
        void LoadHomeText() => this.LoadHomeText(log: true);
#endif

        void OnSaveHTexts(in JObject jobj, in bool log)
        {
        }

        void OnLoadHTexts(in JObject jobj, in bool log)
        {
        }
    }
}