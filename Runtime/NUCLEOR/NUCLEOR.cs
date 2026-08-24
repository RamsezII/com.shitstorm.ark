using _UTIL_;
using System;
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
                FixedUpdate_OnStartOfFrame,
                FixedUpdate_OnMuonRigidbodies,
                FixedUpdate_ragdoll,
                FixedUpdate_OnVehiclePhysics,
                FixedUpdate,
                FixedUpdate_BeforeAnimator,

                LateFixedUpdate_AfterAnimator,

                Update_OnStartOfFrame_once,
                Update_OnStartOfFrame,

                Update_OnShellTick_before,
                Update_OnShellTick,
                Update_OnShellTick_after,
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
                Update_BeforeLateUpdate,

                LateUpdate_AfterAnimator,
                LateUpdate_Cameras_BeforeCharacterModifyPivot,
                LateUpdate_Players_BeforeCameraPosition,
                LateUpdate_CameraPosition,
                LateUpdate_CameraFinalApply,
                LateUpdate_Players_AfterCameraPosition,
                LateUpdate,
                LateUpdate_onEndOfFrame_once,
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

        public DateTimeOffset timestamp_app;

        public readonly SequencerMono
            sequencer_mono = new();

        public readonly SequencerMulti
            sequencer_multi = new();

        public readonly Scheduler
            scheduler_fixed = new(),
            scheduler_scaled = new(),
            scheduler_unscaled = new();

        public readonly ActionBuffer
            actionBuffer_update = new("actionbuffer:upd"),
            actionBuffer_fixedUpdate = new("actionbuffer:fupd");

        public readonly ValueNotifier<bool> isFocused = new();
        public readonly ValueNotifier<bool> isTyping = new();
        public readonly ValueNotifier<byte> party_count = new();

        public static bool application_closed;

        public int fixedFrameCount;
        [Range(0, .1f)]
        public float
            averageDeltatime = 1,
            averageUnscaledDeltatime = 1;

        public readonly ValueNotifier<float>
            timeScale_raw = new(1),
            timeScale_smooth = new(1);

        public readonly object mainThreadLock = new();

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
                var dtemp = ArkMachine.DEditorTemp;
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

            timestamp_app = DateTimeOffset.UtcNow;

            sequencer_mono.sequencables.Reset();
            sequencer_multi.sequencables.Reset();

            timeScale_raw.AddListener(value => Time.timeScale = value);

            Util.InstantiateOrCreateIfAbsent<ArkUI>();
        }

        //----------------------------------------------------------------------------------------------------------

        private void FixedUpdate()
        {
            lock (mainThreadLock)
            {
                ++fixedFrameCount;

                is_nucleor_fixedUpdate = true;

                delegates.FixedUpdate_OnStartOfFrame?.Invoke();
                delegates.FixedUpdate_OnMuonRigidbodies?.Invoke();
                delegates.FixedUpdate_ragdoll?.Invoke();

                delegates.onFixedUpdate1?.Invoke();
                delegates.onFixedUpdate2?.Invoke();
                delegates.onFixedUpdate3?.Invoke();

                delegates.FixedUpdate_OnVehiclePhysics?.Invoke();

                actionBuffer_fixedUpdate.Execute();
                scheduler_fixed.Tick(Time.fixedDeltaTime);

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
                averageUnscaledDeltatime = Mathf.Lerp(averageUnscaledDeltatime, Time.unscaledDeltaTime, 3.5f * Time.unscaledDeltaTime);
                averageDeltatime = Mathf.Lerp(averageDeltatime, Time.deltaTime, 3.5f * Time.deltaTime);

                timeScale_smooth.Value = Mathf.MoveTowards(timeScale_smooth._value, timeScale_raw._value, 5f * Time.unscaledDeltaTime);

                UsageManager.UpdateAltPress();

                is_nucleor_update = true;

                actionBuffer_update.Execute();
                scheduler_unscaled.Tick(Time.unscaledDeltaTime);
                scheduler_scaled.Tick(Time.deltaTime);

                delegates.Update_OnStartOfFrame_once?.Invoke();
                delegates.Update_OnStartOfFrame_once = null;

                delegates.Update_OnStartOfFrame?.Invoke();

                if (delegates.fixedupdate_flag.PullValue())
                    delegates.FixedUpdate_BeforeAnimator?.Invoke();

                delegates.Update_OnShellTick_before?.Invoke();
                delegates.Update_OnShellTick?.Invoke();
                delegates.Update_OnShellTick_after?.Invoke();
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
                delegates.Update_BeforeLateUpdate?.Invoke();

                sequencer_multi.Tick();
                sequencer_mono.Tick();

                is_nucleor_update = false;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            isFocused.Value = focus;
            lock (mainThreadLock)
                if (focus)
                    delegates.OnApplicationFocus?.Invoke();
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
        void LogSequentialScheduler() => sequencer_mono.LogStatus();

        [ContextMenu(nameof(LogParallelScheduler))]
        void LogParallelScheduler() => sequencer_multi.LogStatus();
#endif

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            lock (mainThreadLock)
            {
                is_nucleor_lateUpdate = true;

                delegates.LateUpdate_AfterAnimator?.Invoke();
                delegates.LateUpdate_Cameras_BeforeCharacterModifyPivot?.Invoke();
                delegates.LateUpdate_Players_BeforeCameraPosition?.Invoke();
                delegates.LateUpdate_CameraPosition?.Invoke();
                delegates.LateUpdate_CameraFinalApply?.Invoke();
                delegates.LateUpdate_Players_AfterCameraPosition?.Invoke();
                delegates.LateUpdate?.Invoke();

                delegates.LateUpdate_onEndOfFrame_once?.Invoke();
                delegates.LateUpdate_onEndOfFrame_once = null;

                delegates.LateUpdate_OnNetworkPush?.Invoke();

                is_nucleor_lateUpdate = false;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            lock (mainThreadLock)
            {
                isFocused.Value = false;

                sequencer_multi.Dispose();
                sequencer_mono.Dispose();
                scheduler_fixed.Dispose();
                scheduler_unscaled.Dispose();
                scheduler_scaled.Dispose();

                LogManager.ClearLogs();

                var dtemp = ArkMachine.DTemp;
                if (dtemp.Exists)
                    dtemp.Delete(true);
            }
        }
    }
}