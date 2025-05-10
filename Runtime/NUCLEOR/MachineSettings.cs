using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static class MachineSettings
    {
        public static DirectoryInfo GetUsersFolder() => Path.Combine(NUCLEOR.home_path, "Users").ForceDir();
        public static string GetPlayerFolder() => Path.Combine(GetUsersFolder().FullName, user_name.Value).ForceDir().FullName;

        public static readonly OnValue<string> user_name = new("default_user");

        public static bool user_ready;
        static Action on_user_ready;

        public static DirectoryInfo[] users;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            user_name.Reset();
            user_ready = false;
            on_user_ready = null;
            users = GetUsersFolder().GetDirectories();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void SaveUserName(in string value)
        {
            user_name.Update(value);
            OnUserReady();
        }

        public static void AddListener(Action listener)
        {
            if (user_ready)
                listener();
            else
                Util.AddAction(ref on_user_ready, listener);
        }

        static void OnUserReady()
        {
            if (!user_ready)
            {
                user_ready = true;
                on_user_ready?.Invoke();
            }
            on_user_ready = null;
        }
    }
}