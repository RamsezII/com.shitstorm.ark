using _UTIL_;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

namespace _ARK_
{
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

#if UNITY_EDITOR
            public Action
                OnEditorQuit;
#endif
        }

        public static Delegates delegates;
        public bool is_nucleor_fixedUpdate, is_nucleor_update, is_nucleor_lateUpdate;

        public static NUCLEOR instance;

        public readonly SequentialSequencer sequencer = new();
        public readonly ParallelSequencer sequencer_parallel = new();
        public readonly HeartBeat heartbeat_fixed = new(), heartbeat_scaled = new(), heartbeat_unscaled = new();

        public Camera camera_UI;
        public Canvas canvas3D, canvas2D;

        public readonly ValueHandler<bool> isTyping = new();

        public readonly ValueHandler<byte> party_count = new();

        public static bool application_closed;

        public int fixedFrameCount;
        [Range(0, .1f)]
        public float
            averageDeltatime = 1,
            averageUnscaledDeltatime = 1;

        public readonly ValueHandler<float>
            timeScale_raw = new(1),
            timeScale_smooth = new(1);

        public readonly object mainThreadLock = new();

#if UNITY_EDITOR
        public bool _IsTyping => isTyping.Value;
        [ShowProperty(nameof(_IsTyping))] public bool _show_isTyping;
        public float _TimeScale_raw => timeScale_raw.Value;
        [ShowProperty(nameof(_TimeScale_raw)), Range(0, 2)] public float _show_timeScale_raw;
        public float _TimeScale_smooth => timeScale_smooth.Value;
        [ShowProperty(nameof(_TimeScale_smooth)), Range(0, 2)] public float _show_timeScale_smooth;
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

            UnityEditor.EditorApplication.playModeStateChanged -= LogPlayModeState;
            UnityEditor.EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        static void OnQuitEditor()
        {
            delegates.OnEditorQuit?.Invoke();
        }

        static void LogPlayModeState(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                instance.OnApplicationFocus(false);
#endif
        }

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

            sequencer.schedulables.Reset();
            sequencer_parallel.schedulables.Reset();

            camera_UI = transform.Find("Camera_UI").GetComponent<Camera>();
            canvas3D = camera_UI.transform.Find("Canvas3D").GetComponent<Canvas>();
            canvas2D = transform.Find("Canvas2D").GetComponent<Canvas>();

            timeScale_raw.AddListener(value => Time.timeScale = value);
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
                averageUnscaledDeltatime = Mathf.Lerp(averageUnscaledDeltatime, Time.unscaledDeltaTime, 2 * Time.unscaledDeltaTime);
                averageDeltatime = Mathf.Lerp(averageDeltatime, Time.deltaTime, 3 * Time.deltaTime);

                timeScale_smooth.Value = Mathf.MoveTowards(timeScale_smooth._value, timeScale_raw._value, 5f * Time.unscaledDeltaTime);

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

            if (Directory.Exists(ArkPaths.instance.Value.dpath_temp))
                Directory.Delete(ArkPaths.instance.Value.dpath_temp, true);
        }
    }
}