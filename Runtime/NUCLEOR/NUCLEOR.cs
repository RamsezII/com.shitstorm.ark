using _UTIL_;
using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public sealed partial class NUCLEOR : MonoBehaviour
    {
        public struct Delegates
        {
            [Obsolete]
            public Action
                onFixedUpdate1, onFixedUpdate2, onFixedUpdate3,
                onUpdate1, onUpdate2, onUpdate3;

            public Action
                onFixedUpdateMuonRigidbodies,
                fixedUpdateVehiclePhysics,

                onStartOfFrame_once,
                shell_tick,
                onNetworkPull,
                getInputs,
                onPlayerInputs,
                onMuonInputs,
                updateVehicleVisuals,
                computeCameraCrons,

                onUpdatePlayers,
                onCronsApplied,

                onLateUpdate,
                onEndOfFrame_once,
                onNetworkPush,

                onApplicationFocus,
                onApplicationUnfocus,
                onApplicationQuit;
        }

        public static Delegates delegates;

        public static NUCLEOR instance;

        public readonly ParallelScheduler subScheduler = new();
        public readonly SequentialScheduler scheduler = new();
        public readonly CronGod crongod = new();

        public Camera camera_UI;
        public Canvas canvas3D, canvas2D;

        public static bool applicationQuit;

        public int fixedFrameCount;
        [Range(0, .1f)] public float averageDeltatime = 1;

        Action onMainThread;
        public readonly object mainThreadLock = new();

        public static bool game_path_is_working_path;
        public static string game_path, working_path, home_path, bundles_path, plugins_path, temp_path, terminal_path;

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
            plugins_path = Path.Combine(working_path, "Plugins");
            bundles_path = Path.Combine(working_path, "Bundles");
            temp_path = Path.Combine(home_path, "TEMP");

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
            applicationQuit = false;

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

            scheduler.list.Reset();
            subScheduler.list.Reset();

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
                delegates.onFixedUpdateMuonRigidbodies?.Invoke();
                delegates.onFixedUpdate1?.Invoke();
                delegates.onFixedUpdate2?.Invoke();
                delegates.onFixedUpdate3?.Invoke();
                delegates.fixedUpdateVehiclePhysics?.Invoke();
            }
        }

        //----------------------------------------------------------------------------------------------------------

        public void ToMainThread(in Action action)
        {
            lock (this)
                onMainThread += action;
        }

        private void Update()
        {
            lock (mainThreadLock)
            {
                averageDeltatime = Mathf.Lerp(averageDeltatime, Time.deltaTime, .5f);

                UsageManager.UpdateAltPress();

                delegates.onStartOfFrame_once?.Invoke();
                delegates.onStartOfFrame_once = null;

                delegates.shell_tick?.Invoke();
                delegates.onNetworkPull?.Invoke();
                delegates.getInputs?.Invoke();
                delegates.onPlayerInputs?.Invoke();
                delegates.onMuonInputs?.Invoke();
                delegates.updateVehicleVisuals?.Invoke();
                delegates.computeCameraCrons?.Invoke();

                delegates.onUpdate1?.Invoke();
                delegates.onUpdate2?.Invoke();
                delegates.onUpdate3?.Invoke();

                delegates.onUpdatePlayers?.Invoke();
                delegates.onCronsApplied?.Invoke();

                subScheduler.Tick();
                scheduler.Tick();
                crongod.Tick();

                lock (this)
                {
                    onMainThread?.Invoke();
                    onMainThread = null;
                }
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
                delegates.onApplicationFocus?.Invoke();
            else
                delegates.onApplicationUnfocus?.Invoke();
        }

        private void OnApplicationQuit()
        {
#if PLATFORM_STANDALONE_LINUX
            OnApplicationFocus(false);
#endif

            delegates.onApplicationQuit?.Invoke();
            applicationQuit = true;
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(LogSequentialScheduler))]
        void LogSequentialScheduler() => scheduler.LogStatus();

        [ContextMenu(nameof(LogParallelScheduler))]
        void LogParallelScheduler() => subScheduler.LogStatus();
#endif

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            lock (mainThreadLock)
            {
                delegates.onEndOfFrame_once?.Invoke();
                delegates.onEndOfFrame_once = null;

                delegates.onLateUpdate?.Invoke();
                delegates.onNetworkPush?.Invoke();
            }
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            subScheduler.Dispose();
            scheduler.Dispose();
            crongod.Dispose();

            if (this == instance)
            {
                instance = null;
                LogManager.ClearLogs();
            }

            if (Directory.Exists(temp_path))
                Directory.Delete(temp_path, true);
        }
    }
}