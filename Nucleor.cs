using System;
using System.IO;
using UnityEngine;

namespace _ARK_
{
    public sealed class NUCLEOR : MonoBehaviour
    {
        public static NUCLEOR instance;
        public readonly ParallelScheduler subScheduler = new();
        public readonly SequentialScheduler scheduler = new();
        public readonly CronGod crongod = new();

        public static Action
            onFixedUpdate1, onFixedUpdate2, onFixedUpdate3,
            onNetworkPull, 
            onUpdate1, onUpdate2, onUpdate3,
            onLateUpdate, 
            onEndOfFrame,
            onNetworkPush;

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
            onFixedUpdate1 = onFixedUpdate2 = onFixedUpdate3 = onNetworkPull = onUpdate1 = onUpdate2 = onUpdate3 = onLateUpdate = onNetworkPush = onEndOfFrame = null;
        }

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreate<NUCLEOR>();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= LogPlayModeState;
            UnityEditor.EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
        }

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
        }

#if PLATFORM_STANDALONE_LINUX
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 75;
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        private void FixedUpdate()
        {
            lock (mainThreadLock)
            {
                ++fixedFrameCount;
                onFixedUpdate1?.Invoke();
                onFixedUpdate2?.Invoke();
                onFixedUpdate3?.Invoke();
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
            mainThreadLock.Lock();

            averageDeltatime = Mathf.Lerp(averageDeltatime, Time.deltaTime, .5f);

            onNetworkPull?.Invoke();
            onUpdate1?.Invoke();
            onUpdate2?.Invoke();
            onUpdate3?.Invoke();

            subScheduler.Tick();
            scheduler.Tick();
            crongod.Tick();

            lock (this)
            {
                onMainThread?.Invoke();
                onMainThread = null;
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
            if (File.Exists(temp_path))
                Directory.Delete(temp_path, true);
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            onLateUpdate?.Invoke();
            mainThreadLock.Unlock();
            onEndOfFrame?.Invoke();
            onEndOfFrame = null;
            onNetworkPush?.Invoke();
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            subScheduler.Dispose();
            scheduler.Dispose();
            crongod.Dispose();

            if (instance == this)
                instance = null;

            if (Directory.Exists(temp_path))
                Directory.Delete(temp_path, true);
        }
    }
}