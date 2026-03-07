using _UTIL_;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _ARK_
{
    public static partial class ArkMachine
    {
        public static readonly DirectoryInfo dir_users = new(Path.Combine(ArkPaths.instance.Value.dpath_home, ArkPaths.dname_users));
        public static readonly IEnumerable<DirectoryInfo> EUsers = dir_users.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        public static DirectoryInfo ForceUsersFolder() => dir_users.FullName.GetDir(true);
        public static DirectoryInfo GetUserFolder(in string user_name, in bool force) => Path.Combine(ForceUsersFolder().FullName, user_name).GetDir(force);
        public static DirectoryInfo GetCurrentUserFolder(in bool force) => GetUserFolder(user_name, force);

        public static readonly ValueHandler<Languages> language = new();
        static string last_user_name, user_name;
        public static string CurrentUserName => user_name;

        static Action onReloadUserFiles;
        static Action<bool> onReloadUserFiles_log;

        public static string GetSettingsPath() => Path.Combine(ArkPaths.instance.Value.dpath_home, JSon.GetJSonName(typeof(ArkMachine)));

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            last_user_name = user_name = null;
            onReloadUserFiles = null;
            onReloadUserFiles_log = null;

            language.Reset();

            LoadMachineSettings(true);

            if (UserExists(last_user_name))
                SetUserName(last_user_name);
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
                LoadMachineSettings(log: false);

                GetCurrentUserFolder(force: true);

                onReloadUserFiles?.Invoke();
                onReloadUserFiles_log?.Invoke(false);
            };

            NUCLEOR.delegates.OnApplicationUnfocus += static () => SaveSettings(log: false);
        }

        //----------------------------------------------------------------------------------------------------------

        public static void SaveSettings(in bool log)
        {
            string fpath = GetSettingsPath();

            JObject jobj = new()
            {
                [nameof(last_user_name)] = user_name,
                [nameof(language)] = language._value.ToString(),
            };

            jobj.NJSave(fpath, log);
        }

        public static void LoadMachineSettings(in bool log)
        {
            string fpath = GetSettingsPath();

            if (fpath.NJRead(out JObject jobj, log))
            {
                last_user_name = jobj.Value<string>(nameof(last_user_name)) ?? user_name;

                var set_language = Application.systemLanguage switch
                {
                    SystemLanguage.French => Languages.French,
                    _ => Languages.English,
                };

                if (jobj.ContainsKey(nameof(language)))
                    if (Enum.TryParse(jobj.Value<string>(nameof(ArkMachine.language)), true, out Languages language))
                        set_language = language;

                language.Value = set_language;
            }
        }

        public static bool UserExists(in string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (dir_users.Exists)
                if (dir_users.EnumerateDirectories(name, SearchOption.TopDirectoryOnly).Any())
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

            last_user_name = user_name = value;

            SaveSettings(true);
            LoadMachineSettings(false);

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

            string path = Path.Combine(ForceUsersFolder().FullName, value);

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