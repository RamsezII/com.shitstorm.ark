using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public sealed partial class NUCLEOR : MonoBehaviour
    {
        public static NUCLEOR instance;
        public readonly ParallelScheduler subScheduler = new();
        public readonly SequentialScheduler scheduler = new();
        public readonly CronGod crongod = new();

        public static Action
            onFixedUpdate1, onFixedUpdate2, onFixedUpdate3,
            fixedUpdateVehiclePhysics,
            updateVehicleAiming,
            updateVehicleVisuals,
            onNetworkPull,
            onInputs,
            onUpdate1, onUpdate2, onUpdate3,
            onLateUpdate,
            onStartOfFrame,
            onEndOfFrame,
            onNetworkPush;

        public static bool applicationQuit;

        public int fixedFrameCount;
        [Range(0, .1f)] public float averageDeltatime = 1;

        Action onMainThread;
        public readonly object mainThreadLock = new();

        public static readonly string temp_path = Path.Combine(Util.home_path, "TEMP");
        public static DirectoryInfo TEMP_DIR => temp_path.GetDir();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= LogPlayModeState;
            UnityEditor.EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
            onFixedUpdate1 = onFixedUpdate2 = onFixedUpdate3 = fixedUpdateVehiclePhysics = updateVehicleVisuals = updateVehicleAiming = onNetworkPull = onUpdate1 = onUpdate2 = onUpdate3 = onLateUpdate = onNetworkPush = onStartOfFrame = onEndOfFrame = null;
            applicationQuit = false;
            Util.InstantiateOrCreate<NUCLEOR>();
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
            AwakeUserGroups();
            scheduler.list.Clear();
            subScheduler.list.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        private void FixedUpdate()
        {
            lock (mainThreadLock)
            {
                ++fixedFrameCount;
                onFixedUpdate1?.Invoke();
                onFixedUpdate2?.Invoke();
                onFixedUpdate3?.Invoke();
                fixedUpdateVehiclePhysics?.Invoke();
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

                UpdateUserGroups();

                onStartOfFrame?.Invoke();
                onStartOfFrame = null;

                updateVehicleVisuals?.Invoke();
                onNetworkPull?.Invoke();
                onInputs?.Invoke();
                onUpdate1?.Invoke();
                onUpdate2?.Invoke();
                onUpdate3?.Invoke();
                updateVehicleAiming?.Invoke();

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

#if PLATFORM_STANDALONE_LINUX
        private void OnApplicationQuit() => OnApplicationFocus(false);
#endif

        private void OnApplicationFocus(bool focus)
        {
        }

        private void OnApplicationQuit()
        {
            applicationQuit = true;
            if (File.Exists(temp_path))
                Directory.Delete(temp_path, true);
            ClearUserGroups();
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
                onLateUpdate?.Invoke();
                onEndOfFrame?.Invoke();
                onEndOfFrame = null;
                onNetworkPush?.Invoke();
            }
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            subScheduler.Dispose();
            scheduler.Dispose();
            crongod.Dispose();

            if (this == instance)
                instance = null;

            if (Directory.Exists(temp_path))
                Directory.Delete(temp_path, true);
        }
    }
}