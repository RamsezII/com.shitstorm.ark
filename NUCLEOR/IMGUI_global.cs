using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class IMGUI_global : MonoBehaviour
    {
        public static IMGUI_global instance;

        public readonly DictListener<Func<Event, bool>, object> users = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<IMGUI_global>();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            users.AddListener1(isNotEmpty => useGUILayout = isNotEmpty);
        }

        //--------------------------------------------------------------------------------------------------------------

        [SerializeField] int _frame, _gui_frame;

        private void Update()
        {
            _frame = Time.frameCount;
        }

        private void OnGUI()
        {
            ++_gui_frame;
            Event e = Event.current;
            foreach (var pair in users._dict)
            {
                if (pair.Value == null)
                    Debug.LogWarning($"[ERROR] found nul {users.value_type} in {GetType().FullName}.{nameof(users)} ({users.GetType().FullName})");

                if (pair.Key(e))
                {
                    e.Use();
                    return;
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            users.Dispose();
        }
    }
}