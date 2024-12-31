using UnityEngine;

namespace _ARK_
{
    public abstract class OS : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnLoadTexts(true);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        protected virtual void Start()
        {
            NUCLEOR.onInputs -= UpdateInputs;
            NUCLEOR.onInputs += UpdateInputs;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
                OnLoadTexts(false);
            else
                OnSaveTexts(false);
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(LoadTexts))]
        void LoadTexts() => OnLoadTexts(true);
#endif
        protected virtual void OnLoadTexts(in bool log)
        {
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveTexts))]
        void SaveTexts() => OnSaveTexts(true);
#endif
        protected virtual void OnSaveTexts(in bool log)
        {
        }

        protected virtual void UpdateInputs()
        {
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            NUCLEOR.onInputs -= UpdateInputs;
        }
    }
}