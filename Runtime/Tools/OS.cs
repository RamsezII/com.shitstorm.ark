using UnityEngine;

namespace _ARK_
{
    public abstract class OS : ArkComponent1
    {

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
            OnLoadTexts(true);
            NUCLEOR.delegates.OnApplicationFocus += () => OnLoadTexts(log: false);
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveTexts))]
        void SaveTexts() => OnSaveTexts();
        protected virtual void OnSaveTexts()
        {
        }
#endif

        [ContextMenu(nameof(LoadTexts))]
        void LoadTexts() => OnLoadTexts(true);
        protected virtual void OnLoadTexts(in bool log)
        {
        }
    }
}