using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public class ArkOS : OS
    {
        [Serializable]
        public class Settings : SettingsFile
        {
            public bool
                logFileActivity = false;
        }

        public static ArkOS instance;

        public Settings settings;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<ArkOS>();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            instance = this;
            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnSaveTexts()
        {
            settings ??= new();
            settings.Save();
        }

        protected override void OnLoadTexts(in bool log)
        {
            base.OnLoadTexts(log);
            SettingsFile.Load(ref settings, log);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this == instance)
                instance = null;
        }
    }
}