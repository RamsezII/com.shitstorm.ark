using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class IMGUI_global : MonoBehaviour
    {
        public static IMGUI_global instance;

        public readonly DictListener<Func<Event, bool>, object> users_keydown = new();
        public readonly DictListener<Action, object> users_ongui = new();

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
            users_keydown.AddListener1(isNotEmpty => Refresh());
            users_ongui.AddListener1(isNotEmpty => Refresh());

            void Refresh()
            {
                enabled = users_keydown.IsNotEmpty || users_ongui.IsNotEmpty;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
                foreach (var pair in users_keydown._dict)
                {
                    if (pair.Value == null)
                        Debug.LogWarning($"[ERROR] found nul {users_keydown.value_type} in {GetType().FullName}.{nameof(users_keydown)} ({users_keydown.GetType().FullName})");

                    if (pair.Key(e))
                    {
                        e.Use();
                        return;
                    }
                }

            foreach (var pair in users_ongui._dict)
            {
                if (pair.Value == null)
                    Debug.LogWarning($"[ERROR] found nul key in {GetType().FullName}.{nameof(users_ongui)} ({users_ongui.GetType().FullName})");
                pair.Key();
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            users_keydown.Dispose();
            users_ongui.Dispose();
        }
    }
}