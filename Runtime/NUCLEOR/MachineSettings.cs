using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static class MachineSettings
    {
        [Serializable]
        class Infos : JSon
        {
            static readonly string FILE_NAME = typeof(MachineSettings).TypeToFileName() + txt;
            public static string GetFilePath() => Path.Combine(NUCLEOR.home_path.ForceDir().FullName, FILE_NAME);

            public string last_user;
        }

        public static DirectoryInfo GetUsersFolder() => Path.Combine(NUCLEOR.home_path, "Users").ForceDir();
        public static string GetPlayerFolder() => Path.Combine(GetUsersFolder().FullName, user_name.Value).ForceDir().FullName;

        public static readonly OnValue<string> user_name = new("default_user");

        public static bool user_ready;
        static Action on_user_ready;

        static Infos infos;

        public static DirectoryInfo[] users;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            infos = new();
            user_name.Reset();
            user_ready = false;
            on_user_ready = null;
            users = GetUsersFolder().GetDirectories();
            TryReadUserName();
        }

        //----------------------------------------------------------------------------------------------------------

        static void TryReadUserName()
        {
            string file_path = Infos.GetFilePath();

            if (JSon.Read(ref infos, file_path, false, true))
                if (!string.IsNullOrWhiteSpace(infos.last_user))
                {
                    Debug.Log($"[READ] {nameof(infos.last_user)}: \"{infos.last_user}\" ({file_path})".ToSubLog());
                    user_name.Update(infos.last_user);
                    OnUserReady();
                }
        }

        public static void SaveUserName(in string value)
        {
            string file_path = Infos.GetFilePath();

            infos.last_user = value;
            infos.Save(file_path, true);

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