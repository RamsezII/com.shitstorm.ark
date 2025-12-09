using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static partial class ArkMachine
    {
        public static DirectoryInfo ForceUsersFolder() => Path.Combine(ArkPaths.instance.Value.dpath_home, ArkPaths.dname_users).GetDir(true);
        public static DirectoryInfo GetUserFolder(in bool force) => GetUserFolder(user_name.Value, force);
        public static DirectoryInfo GetUserFolder(in string user_name, in bool force) => Path.Combine(ForceUsersFolder().FullName, user_name).GetDir(force);


        public static readonly ValueHandler<string> user_name = new();

        public static bool user_ready;
        static Action on_user_ready;

        public static readonly ListListener<DirectoryInfo> users = new();

        public static bool flag_shutdown;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            flag_shutdown = false;

            user_ready = false;
            on_user_ready = null;

            user_name.Reset("default_user");
            users.Reset();

            ScanUsers();
            LoadSettings(true);

            user_name.AddListener(value =>
            {
                h_settings.last_user = value;
            });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += () => LoadSettings(false);
            NUCLEOR.delegates.OnApplicationUnfocus += () => SaveSettings(false);
        }

        //----------------------------------------------------------------------------------------------------------

        public static void ShutdownApplication(in bool force)
        {
            if (force)
                flag_shutdown = true;

            if (flag_shutdown)
#if UNITY_EDITOR
                if (Application.isEditor)
                    UnityEditor.EditorApplication.isPlaying = false;
                else
#endif
                    Application.Quit();
        }

        static void ScanUsers() => users.Modify(list =>
        {
            list.Clear();
            list.AddRange(ForceUsersFolder().EnumerateDirectories());
        });

        public static void SetUserName(in string value)
        {
            user_name.Value = value;
            GetUserFolder(true);
            ScanUsers();
            OnUserReady();
            SaveSettings(true);
        }

        public static bool TryRemoveUser(in string value, out string error)
        {
            string path = Path.Combine(ForceUsersFolder().FullName, value);

            if (!Directory.Exists(path))
            {
                error = $"User '{value}' does not exist!";
                return false;
            }

            Directory.Delete(path, true);
            ScanUsers();
            error = null;

            return true;
        }

        public static bool TryRenameUser(in string old_name, in string new_name, out string error)
        {
            DirectoryInfo old_user = GetUserFolder(old_name, false);
            if (!old_user.Exists)
            {
                error = $"User '{old_name}' does not exist!";
                return false;
            }

            DirectoryInfo new_user = GetUserFolder(new_name, false);
            if (new_user.Exists)
            {
                error = $"User '{new_name}' already exists!";
                return false;
            }

            old_user.MoveTo(new_user.FullName);

            if (old_name.Equals(user_name.Value, StringComparison.Ordinal))
                user_name.Value = new_name;

            GetUserFolder(true);
            ScanUsers();

            error = null;
            return true;
        }

        public static void AddListener(in Action listener)
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