using _UTIL_;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _ARK_
{
    public static partial class ArkMachine
    {
        public static DirectoryInfo GetUsersDir => DFUsers.ForceDir();
        public static IEnumerable<DirectoryInfo> EUsers => GetUsersDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        public static DirectoryInfo GetUserFolder(in string user_name, in bool force) => Path.Combine(GetUsersDir.FullName, user_name).GetDir(force);
        public static DirectoryInfo GetCurrentUserFolder(in bool force) => GetUserFolder(user_name, force);

        public static readonly ValueNotifier<Languages> language = new();
        static string user_name;
        public static string CurrentUserName => user_name;

        static Action onReloadUserFiles;
        static Action<bool> onReloadUserFiles_log;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            settings = null;

            user_name = null;
            onReloadUserFiles = null;
            onReloadUserFiles_log = null;

            language.Reset();

            LoadHSettings(true);

            if (UserExists(settings.last_user_name))
                SetUserName(settings.last_user_name);
            else
                SetUserName("default_user");

            onReloadUserFiles?.Invoke();
            onReloadUserFiles_log?.Invoke(false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += static () =>
            {
                LoadHSettings(log: false);

                GetCurrentUserFolder(force: true);

                onReloadUserFiles?.Invoke();
                onReloadUserFiles_log?.Invoke(false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += static () => SaveHSettings(log: false);
        }

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

            settings.last_user_name = user_name = value;

            SaveHSettings(true);
            LoadHSettings(false);

            onReloadUserFiles?.Invoke();
            onReloadUserFiles_log?.Invoke(false);
        }

        public static bool TryDeleteUser(in string value, out string error)
        {
            if (string.Equals(user_name, value, StringComparison.OrdinalIgnoreCase))
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

            if (old_name.Equals(user_name, StringComparison.Ordinal))
                user_name = new_name;

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