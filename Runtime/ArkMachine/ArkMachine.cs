using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static partial class ArkMachine
    {
        public static DirectoryInfo ForceUsersFolder() => Path.Combine(NUCLEOR.home_path, "Users").GetDir(true);
        public static DirectoryInfo GetUserFolder(in bool force) => GetUserFolder(user_name.Value, force);
        public static DirectoryInfo GetUserFolder(in string user_name, in bool force) => Path.Combine(ForceUsersFolder().FullName, user_name).GetDir(force);


        public static readonly OnValue<string> user_name = new("default_user");

        public static bool user_ready;
        static Action on_user_ready;

        public static readonly ListListener<DirectoryInfo> users = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            user_name.Reset();
            user_ready = false;
            on_user_ready = null;
            users.Reset();
            ScanUsers();
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            LoadSettings(true);

            user_name.AddListener(value =>
            {
                settings.last_user = value;
            });

            NUCLEOR.delegates.onApplicationFocus += () => LoadSettings(false);
            NUCLEOR.delegates.onApplicationUnfocus += () => SaveSettings(false);
        }

        //----------------------------------------------------------------------------------------------------------

        static void ScanUsers() => users.Modify(list =>
        {
            list.Clear();
            list.AddRange(ForceUsersFolder().EnumerateDirectories());
        });

        public static void SetUserName(in string value)
        {
            user_name.Update(value);
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
                user_name.Update(new_name);

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