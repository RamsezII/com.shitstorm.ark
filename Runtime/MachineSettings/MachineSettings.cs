using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public static partial class MachineSettings
    {
        public static DirectoryInfo ForceUsersFolder() => Path.Combine(NUCLEOR.home_path, "Users").ForceDir();
        public static DirectoryInfo ForceUserFolder(in string user_name) => Path.Combine(ForceUsersFolder().FullName, user_name).ForceDir();
        public static DirectoryInfo ForceUserFolder() => ForceUserFolder(user_name.Value);

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

        static void ScanUsers() => users.ModifyList(list =>
        {
            list.Clear();
            list.AddRange(ForceUsersFolder().EnumerateDirectories());
        });

        public static void SetUserName(in string value)
        {
            user_name.Update(value);
            ForceUserFolder();
            OnUserReady();
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

        public static bool TryRenameUser(in string value, out string error)
        {
            string new_path = Path.Combine(ForceUsersFolder().FullName, value);

            if (Directory.Exists(new_path))
            {
                error = $"User '{value}' already exists!";
                return false;
            }

            string old_path = ForceUserFolder().FullName;
            Directory.Move(old_path, new_path);

            user_name.Update(value);

            ForceUserFolder();
            ScanUsers();

            error = null;
            return true;
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