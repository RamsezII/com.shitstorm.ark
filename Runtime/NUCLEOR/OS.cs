using UnityEngine;

namespace _ARK_
{
    public abstract class OS : MonoBehaviour
    {

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnLoadTexts(true);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
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

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
                OnLoadTexts(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
        }
    }
}