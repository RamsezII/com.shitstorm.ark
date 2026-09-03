using _UTIL_;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public static class Util_ArkTexts
    {
        public static void SaveArkText(this IArkTexts target, in bool log)
        {
            string spath = target switch
            {
                IHomeTexts => NUCLEOR.GetHomeJSonPath(target.GetType()),
                IUserTexts => NUCLEOR.instance.GetCurrentUserTextPath(target.GetType()),
                _ => throw new System.NotImplementedException(),
            };

            JObject jobj = new();
            target.OnBeforeSaveArkText(jobj, log: log);
            jobj.WriteFields<NJFieldAttribute>(target);
            jobj.NJSave(spath, log: log);
        }

        public static void LoadArkText(this IArkTexts target, in bool log)
        {
            string lpath = target switch
            {
                IHomeTexts => NUCLEOR.GetHomeJSonPath(target.GetType()),
                IUserTexts => NUCLEOR.instance.GetCurrentUserTextPath(target.GetType()),
                _ => throw new System.NotImplementedException(),
            };

            lpath.TryNJRead(out JObject jobj, force: true, log_success: log);
            jobj.ReadFields<NJFieldAttribute>(target);
            target.OnAfterLoadArkText(jobj, log: log);
        }
    }

    public interface IArkTexts
    {
        public static readonly HashSet<IArkTexts> _users = new();

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
                foreach (var user in _users)
                    user.LoadArkText(log: false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += () =>
            {
                foreach (var user in _users)
                    user.SaveArkText(log: false);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        public static void AddUser(IArkTexts user)
        {
            user.LoadArkText(log: false);
            _users.Add(user);
        }

        public static void RemoveUser(IArkTexts user)
        {
            _users.Remove(user);
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveArkText))]
        void SaveArkText() => this.SaveArkText(log: true);

        [ContextMenu(nameof(LoadArkText))]
        void LoadArkText() => this.LoadArkText(log: true);
#endif

        void OnBeforeSaveArkText(in JObject jobj, in bool log)
        {
        }

        void OnAfterLoadArkText(in JObject jobj, in bool log)
        {
        }
    }

    public interface IUserTexts : IArkTexts
    {
    }

    public interface IHomeTexts : IArkTexts
    {
    }
}