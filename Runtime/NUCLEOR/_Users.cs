using _UTIL_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _ARK_
{
    public partial class NUCLEOR
    {
        public static DirectoryInfo GetUsersDir => DFUsers.ForceDir();
        public static IEnumerable<DirectoryInfo> EUsers => GetUsersDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        public static DirectoryInfo GetUserFolder(in string user_name, in bool force) => Path.Combine(GetUsersDir.FullName, user_name).GetDir(force);
        public static DirectoryInfo GetCurrentUserFolder(in bool force) => GetUserFolder(user_name._value, force);

        public static readonly ValueNotifier<string> user_name = new();
        public static readonly ValueNotifier<Languages> static_language = new(Application.systemLanguage switch
        {
            SystemLanguage.French => Languages.French,
            _ => Languages.English,
        });

        static Action onReloadUserFiles;
        static Action<bool> onReloadUserFiles_log;

        //----------------------------------------------------------------------------------------------------------

        public static bool UserExists(in string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (GetUsersDir.Exists)
                if (GetUsersDir.EnumerateDirectories(name, SearchOption.TopDirectoryOnly).Any())
                    return true;

            return false;
        }

        public static void SetUserName(in string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Debug.LogWarning($"can not set empty user name ({nameof(value)}: \"{value}\")");
                return;
            }

            user_name.Value = instance.last_user_name = value;

            instance.SaveHomeText(log: true);
            instance.LoadHomeText(log: false);
            delegates.OnApplicationFocus?.Invoke();

            onReloadUserFiles?.Invoke();
            onReloadUserFiles_log?.Invoke(false);
        }

        public static bool TryDeleteUser(in string value, out string error)
        {
            if (string.Equals(user_name._value, value, StringComparison.OrdinalIgnoreCase))
            {
                error = "Can not delete current user";
                return false;
            }

            string path = Path.Combine(GetUsersDir.FullName, value);

            if (!Directory.Exists(path))
            {
                error = $"User '{value}' does not exist";
                return false;
            }

            Directory.Delete(path, true);
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

            if (old_name.Equals(user_name._value, StringComparison.Ordinal))
                user_name.Value = new_name;

            GetCurrentUserFolder(force: true);

            onReloadUserFiles?.Invoke();
            onReloadUserFiles_log?.Invoke(false);

            error = null;
            return true;
        }

        public static void AddOnReloadUserFiles(in Action action, in bool doNotCallThisTime = false)
        {
            onReloadUserFiles -= action;
            if (!doNotCallThisTime)
                action();
            onReloadUserFiles += action;
        }

        public static void RemoveOnReloadUserFiles(in Action action)
        {
            onReloadUserFiles -= action;
        }

        public static void ShutdownApplication()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                UnityEditor.EditorApplication.isPlaying = false;
            else
#endif
                Application.Quit();
        }
    }
}