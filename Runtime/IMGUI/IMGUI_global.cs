using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class IMGUI_global : MonoBehaviour
    {
        public static IMGUI_global instance;

        public readonly DictListener<Func<Event, bool>, object>
            users_ongui = new(),
            users_inputs = new();

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            instance = FindAnyObjectByType<IMGUI_global>();
            Util.InstantiateOrCreateIfAbsent<IMGUI_global>();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Start()
        {
            users_ongui.AddListener1(this, isNotEmpty => Refresh());
            users_inputs.AddListener1(this, isNotEmpty => Refresh());

            void Refresh()
            {
                if (this == null)
                    return;
                useGUILayout = users_ongui.IsNotEmpty || users_inputs.IsNotEmpty;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [SerializeField] int _frame, _gui_frame;

        private void Update()
        {
            _frame = Time.frameCount;
        }

        [ContextMenu(nameof(ShowUsers))]
        void ShowUsers()
        {
            System.Text.StringBuilder sb = new();
            Populate(nameof(users_inputs), users_inputs);
            Populate(nameof(users_ongui), users_ongui);
            Debug.Log(sb.ToString(), this);

            void Populate(in string name, in DictListener<Func<Event, bool>, object> dict)
            {
                sb.AppendLine($"{name}.{nameof(dict._collection.Count)}: {dict._collection.Count}");
                foreach (var pair in dict._collection)
                    sb.AppendLine($" - {name}: {{ {pair.Key} : {pair.Value} }}");
            }
        }
#endif

        private void OnGUI()
        {
#if UNITY_EDITOR
            ++_gui_frame;
#endif

            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                Debug.LogWarning($"TRIED ESCAPE KEY IN IMGUI_GLOBAL", this);
                e.Use();
            }

            switch (e.type)
            {
                case EventType.KeyDown:
                case EventType.MouseDown:
                case EventType.ScrollWheel:
                    foreach (var pair in users_inputs._collection)
                    {
                        if (pair.Value == null)
                            Debug.LogWarning($"[ERROR] found nul {users_inputs.value_type} in {GetType().FullName}.{nameof(users_inputs)} ({users_inputs.GetType().FullName})");
                        if (pair.Key(e))
                        {
                            e.Use();
                            return;
                        }
                    }
                    break;
            }

            foreach (var pair in users_ongui._collection)
            {
                if (pair.Value == null)
                    Debug.LogWarning($"[ERROR] found nul {users_ongui.value_type} in {GetType().FullName}.{nameof(users_ongui)} ({users_ongui.GetType().FullName})");

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
            users_ongui.Reset();
        }
    }
}