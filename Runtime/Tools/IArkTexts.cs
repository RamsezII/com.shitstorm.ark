using _UTIL_;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _ARK_
{
    public static class Util_ArkTexts
    {
        public static void SaveArkTexts(this IArkTexts target, in bool log)
        {
            var jobj = new JObject();
            Dictionary<Type, JObject> jobjs = new()
            {
                [target.GetType()] = jobj,
            };

            target.OnBeforeSaveArkTexts(jobjs, jobj, log);
            jobjs.WriteFields<NJFieldAttribute>(target);

            foreach (var pair in jobjs)
            {
                string spath = target switch
                {
                    IHomeTexts => NUCLEOR.GetHomeJSonPath(pair.Key),
                    IUserTexts => NUCLEOR.instance.GetCurrentUserTextPath(pair.Key),
                    _ => throw new NotImplementedException(),
                };
                pair.Value.NJSave(spath, log);
            }
        }

        public static void LoadArkTexts(this IArkTexts target, in bool log)
        {
            Dictionary<Type, JObject> jobjs = new();

            for (var t = target.GetType(); t != null; t = t.BaseType)
                if (t == target.GetType() || target.EFields<NJFieldAttribute>(t).Any(field => field.DeclaringType == t))
                {
                    string lpath = target switch
                    {
                        IHomeTexts => NUCLEOR.GetHomeJSonPath(t),
                        IUserTexts => NUCLEOR.instance.GetCurrentUserTextPath(t),
                        _ => throw new NotImplementedException(),
                    };

                    lpath.TryNJRead(out JObject jobj, force: true, log_success: log);
                    jobjs.Add(t, jobj);
                }

            jobjs.ReadFields<NJFieldAttribute>(target);

            target.OnAfterLoadArkTexts(jobjs, jobjs[target.GetType()], log);
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
                    user.LoadArkTexts(log: false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += () =>
            {
                foreach (var user in _users)
                    user.SaveArkTexts(log: false);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        public static void AddUser(IArkTexts user)
        {
            user.LoadArkTexts(log: false);
            _users.Add(user);
        }

        public static void RemoveUser(IArkTexts user)
        {
            _users.Remove(user);
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveArkText))]
        void SaveArkText() => this.SaveArkTexts(log: true);

        [ContextMenu(nameof(LoadArkText))]
        void LoadArkText() => this.LoadArkTexts(log: true);
#endif

        void OnBeforeSaveArkTexts(in Dictionary<Type, JObject> jobjs, in JObject jobj, in bool log)
        {
        }

        void OnAfterLoadArkTexts(in Dictionary<Type, JObject> jobjs, in JObject jobj, in bool log)
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
