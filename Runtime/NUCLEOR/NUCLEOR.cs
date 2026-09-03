using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed partial class NUCLEOR : MonoBehaviour
    {
        public static NUCLEOR instance;

        public DateTimeOffset timestamp_app;

        public readonly ValueNotifier<bool> isFocused = new();
        public readonly ValueNotifier<bool> isTyping = new();
        public readonly HashSetListener<object> players = new();

        public static bool application_closed;

#if UNITY_EDITOR
        public static DateTimeOffset timestamp_editorStart;

        //----------------------------------------------------------------------------------------------------------

        static NUCLEOR()
        {
            timestamp_editorStart = DateTimeOffset.UtcNow;

            Debug.Log($"{typeof(NUCLEOR).FullName}.CONSTRUCTOR {nameof(timestamp_editorStart)}: {timestamp_editorStart.LocalDateTime}");

            LoadEText();

            UnityEditor.EditorApplication.quitting += () =>
            {
                var dtemp = DEditorTemp;
                if (dtemp.Exists)
                    dtemp.Delete(true);
            };
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnResetStatics()
        {
            delegates = default;
            application_closed = false;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting -= OnQuitEditor;
            UnityEditor.EditorApplication.quitting += OnQuitEditor;

            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChange;

            LoadEText();
#endif
        }

#if UNITY_EDITOR
        static void OnQuitEditor()
        {
            delegates.OnEditorQuit?.Invoke();
        }

        static void OnPlayModeStateChange(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                instance.OnApplicationFocus(false);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<NUCLEOR>();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

            players.AddElement(this);

            timestamp_app = DateTimeOffset.UtcNow;

            monolith.sequencables.Reset();
            routinizer.sequencables.Reset();

            timeScale_raw.AddListener(value => Time.timeScale = value);

            AwakeUser();

            IHomeTexts.AddUser(this);

            Util.InstantiateOrCreateIfAbsent<ArkUI>();
        }

        private void OnApplicationFocus(bool focus)
        {
            isFocused.Value = focus;
            lock (mainThreadLock)
                if (focus)
                {
                    GetCurrentUserFolder(force: true);
                    delegates.OnApplicationFocus?.Invoke();
                }
                else
                    delegates.OnApplicationUnfocus?.Invoke();
        }

        private void OnApplicationQuit()
        {
            isFocused.Value = false;
            lock (mainThreadLock)
            {
#if PLATFORM_STANDALONE_LINUX
                OnApplicationFocus(false);
#endif

                delegates.OnApplicationQuit?.Invoke();

                application_closed = true;
            }
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(LogSequentialScheduler))]
        void LogSequentialScheduler() => monolith.LogStatus();

        [ContextMenu(nameof(LogParallelScheduler))]
        void LogParallelScheduler() => routinizer.LogStatus();
#endif

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            IHomeTexts.RemoveUser(this);

            lock (mainThreadLock)
            {
                isFocused.Value = false;

                routinizer.Dispose();
                monolith.Dispose();
                scheduler_fixed.Dispose();
                scheduler_unscaled.Dispose();
                scheduler_scaled.Dispose();

                LogManager.ClearLogs();

                var dtemp = DTemp;
                if (dtemp.Exists)
                    dtemp.Delete(true);
            }
        }
    }
}