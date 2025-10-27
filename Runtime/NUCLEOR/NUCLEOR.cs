using _UTIL_;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _ARK_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public sealed partial class NUCLEOR : MonoBehaviour
    {
        public struct Delegates
        {
            public Action
                onFixedUpdate1, onFixedUpdate2, onFixedUpdate3,
                onUpdate1, onUpdate2, onUpdate3;

            internal bool fixedupdate_flag;

            public Action
                FixedUpdate_ragdoll,
                FixedUpdate_OnMuonRigidbodies,
                FixedUpdate_OnVehiclePhysics,
                FixedUpdate,
                FixedUpdate_BeforeAnimator,

                LateFixedUpdate_AfterAnimator,

                Update_OnStartOfFrame_once,
                Update_OnStartOfFrame,

                Update_OnShellTick,
                Update_OnNetworkPull,
                Update_GettInputs,
                Update_OnPlayerInputs,
                Update_OnMuonInputs,
                Update_ControlSeatInputs,
                Update_OnVehicleVisuals,

                Update_Players1,
                Update_UpdateAndRotateCameras,
                Update_Crons,
                Update_Players2,
                Update,
                Update_BeforeAnimator,

                LateUpdate_onEndOfFrame_once,
                LateUpdate_AfterAnimator,
                LateUpdate_Players1,
                LateUpdate_CameraPosition,
                LateUpdate_CameraFinalApply,
                LateUpdate_Players2,
                LateUpdate,
                LateUpdate_OnNetworkPush,

                OnApplicationFocus,
                OnApplicationUnfocus,
                OnApplicationQuit;
        }

        public static Delegates delegates;
        public bool is_nucleor_fixedUpdate, is_nucleor_update, is_nucleor_lateUpdate;

        public static NUCLEOR instance;

        public readonly SequentialSequencer sequencer = new();
        public readonly ParallelSequencer sequencer_parallel = new();
        public readonly HeartBeat heartbeat_fixed = new(), heartbeat_scaled = new(), heartbeat_unscaled = new();

        public Camera camera_UI;
        public Canvas canvas3D, canvas2D;

        public readonly OnValue_bool isTyping = new();

        public static bool application_closed;

        public int fixedFrameCount;
        [Range(0, .1f)] public float averageDeltatime = 1;

        public readonly object mainThreadLock = new();

        public static bool game_path_is_working_path;

        public static string
            game_path,
            working_path,
            home_path,
            bundles_texts_path,
            bundles_archives_path_auto,
            bundles_archives_path_universal,
            bundles_archives_path_windows,
            bundles_archives_path_linux,
            temp_path,
            terminal_path;

#if UNITY_EDITOR
        public static string assets_path;

        public bool _IsTyping => isTyping.Value;
        [ShowProperty(nameof(_IsTyping))] public bool _show_isTyping;
#endif

        //----------------------------------------------------------------------------------------------------------

        static NUCLEOR()
        {
            Debug.Log($"{typeof(NUCLEOR)} static constructor");
            InitPaths();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(InitPaths))]
#endif
        static void InitPaths()
        {
            game_path = Directory.GetParent(Application.dataPath).FullName;
            working_path = Directory.GetCurrentDirectory();
            game_path_is_working_path = Util.Equals_path(working_path, game_path);
            working_path = game_path_is_working_path ? game_path : Directory.GetParent(game_path).FullName;
            home_path = Path.Combine(working_path, "Home");
            bundles_texts_path = Path.Combine(working_path, "Bundles_texts");

            bundles_archives_path_universal = Path.Combine(working_path, "Bundles_universal");
            bundles_archives_path_windows = Path.Combine(working_path, "Bundles_windows");
            bundles_archives_path_linux = Path.Combine(working_path, "Bundles_linux").DOS2UNIX_full();

            bundles_archives_path_auto = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => bundles_archives_path_windows,
                PlatformID.Unix => bundles_archives_path_linux,
                _ => bundles_archives_path_universal,
            };

            temp_path = Path.Combine(home_path, "TEMP");

#if UNITY_EDITOR
            assets_path = Directory.GetParent(Application.streamingAssetsPath).FullName.DOS2UNIX_full();
#endif

            if (Directory.Exists(temp_path))
                Directory.Delete(temp_path, true);

            if (game_path_is_working_path)
                terminal_path = Path.GetFileNameWithoutExtension(game_path);
            else
            {
                string root_dir = Directory.GetCurrentDirectory();
                terminal_path = Path.GetRelativePath(root_dir, game_path);
            }
            Debug.Log($"{nameof(game_path_is_working_path)}: {game_path_is_working_path}, {nameof(terminal_path)}: {terminal_path}");
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= LogPlayModeState;
            UnityEditor.EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
            delegates = default;
            application_closed = false;

            Util.InstantiateOrCreateIfAbsent<NUCLEOR>();
        }

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private static void LogPlayModeState(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                instance.OnApplicationFocus(false);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);

            sequencer.list.Reset();
            sequencer_parallel.list.Reset();

            camera_UI = transform.Find("Camera_UI").GetComponent<Camera>();
            canvas3D = camera_UI.transform.Find("Canvas3D").GetComponent<Canvas>();
            canvas2D = transform.Find("Canvas2D").GetComponent<Canvas>();
        }

        //----------------------------------------------------------------------------------------------------------

        private void FixedUpdate()
        {
            lock (mainThreadLock)
            {
                ++fixedFrameCount;

                is_nucleor_fixedUpdate = true;

                delegates.FixedUpdate_OnMuonRigidbodies?.Invoke();
                delegates.FixedUpdate_ragdoll?.Invoke();

                delegates.onFixedUpdate1?.Invoke();
                delegates.onFixedUpdate2?.Invoke();
                delegates.onFixedUpdate3?.Invoke();

                delegates.FixedUpdate_OnVehiclePhysics?.Invoke();

                heartbeat_fixed.Tick(Time.fixedDeltaTime);

                delegates.FixedUpdate?.Invoke();

                is_nucleor_fixedUpdate = false;

                delegates.fixedupdate_flag = true;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        private void Update()
        {
            lock (mainThreadLock)
            {
                averageDeltatime = Mathf.Lerp(averageDeltatime, Time.deltaTime, .5f);

                if (CursorManager.instance != null)
                    CursorManager.instance.MoveMouse();

                UsageManager.UpdateAltPress();

                is_nucleor_update = true;

                delegates.Update_OnStartOfFrame_once?.Invoke();
                delegates.Update_OnStartOfFrame_once = null;

                delegates.Update_OnStartOfFrame?.Invoke();

                if (delegates.fixedupdate_flag.PullValue())
                    delegates.FixedUpdate_BeforeAnimator?.Invoke();

                delegates.Update_OnShellTick?.Invoke();
                delegates.Update_OnNetworkPull?.Invoke();

                isTyping.Value = EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null;

                delegates.Update_GettInputs?.Invoke();
                delegates.Update_OnPlayerInputs?.Invoke();
                delegates.Update_OnMuonInputs?.Invoke();
                delegates.Update_ControlSeatInputs?.Invoke();
                delegates.Update_OnVehicleVisuals?.Invoke();

                delegates.onUpdate1?.Invoke();
                delegates.onUpdate2?.Invoke();
                delegates.onUpdate3?.Invoke();

                delegates.Update_Players1?.Invoke();
                delegates.Update_UpdateAndRotateCameras?.Invoke();
                delegates.Update_Crons?.Invoke();
                delegates.Update_Players2?.Invoke();
                delegates.Update?.Invoke();
                delegates.Update_BeforeAnimator?.Invoke();

                sequencer_parallel.Tick();
                sequencer.Tick();

                is_nucleor_update = false;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            lock (mainThreadLock)
                if (focus)
                    delegates.OnApplicationFocus?.Invoke();
                else
                    delegates.OnApplicationUnfocus?.Invoke();
        }

        private void OnApplicationQuit()
        {
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
        void LogSequentialScheduler() => sequencer.LogStatus();

        [ContextMenu(nameof(LogParallelScheduler))]
        void LogParallelScheduler() => sequencer_parallel.LogStatus();
#endif

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            lock (mainThreadLock)
            {
                is_nucleor_lateUpdate = true;

                delegates.LateUpdate_onEndOfFrame_once?.Invoke();
                delegates.LateUpdate_onEndOfFrame_once = null;

                heartbeat_unscaled.Tick(Time.unscaledDeltaTime);
                heartbeat_scaled.Tick(Time.deltaTime);

                delegates.LateUpdate_AfterAnimator?.Invoke();
                delegates.LateUpdate_Players1?.Invoke();
                delegates.LateUpdate_CameraPosition?.Invoke();
                delegates.LateUpdate_CameraFinalApply?.Invoke();
                delegates.LateUpdate_Players2?.Invoke();
                delegates.LateUpdate?.Invoke();
                delegates.LateUpdate_OnNetworkPush?.Invoke();

                is_nucleor_lateUpdate = false;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            sequencer_parallel.Dispose();
            sequencer.Dispose();
            heartbeat_fixed.Dispose();
            heartbeat_unscaled.Dispose();
            heartbeat_scaled.Dispose();

            LogManager.ClearLogs();

            if (Directory.Exists(temp_path))
                Directory.Delete(temp_path, true);
        }
    }
}