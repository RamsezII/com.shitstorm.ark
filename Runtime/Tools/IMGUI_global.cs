using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class IMGUI_global : MonoBehaviour
    {
        public static IMGUI_global instance;

        public readonly ListListener<Func<Event, bool>>
            gui_users = new(),
            inputs_users = new();

#if UNITY_EDITOR
        [SerializeField] int _frame, _gui_frame;
#endif
        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
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
            gui_users.AddListener1(this, isNotEmpty => Refresh());
            inputs_users.AddListener1(this, isNotEmpty => Refresh());

            void Refresh()
            {
                if (this == null)
                    return;
                useGUILayout = gui_users.IsNotEmpty || inputs_users.IsNotEmpty;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private void Update()
        {
            _frame = Time.frameCount;
        }
#endif

        //--------------------------------------------------------------------------------------------------------------

        private void OnGUI()
        {
#if UNITY_EDITOR
            ++_gui_frame;
#endif

            Event e = Event.current;

            if (e.type == EventType.KeyDown)
                switch (e.keyCode)
                {
                    case KeyCode.Escape:
                        Debug.LogWarning($"[{GetType()}] 'ESCAPE'", this);
                        e.Use();
                        return;

                    default:
                        if (e.control || e.command)
                            switch (e.keyCode)
                            {
                                case KeyCode.Z:
                                    if (e.shift)
                                        goto case KeyCode.Y;
                                    else
                                    {
                                        Debug.LogWarning($"[{GetType()}] 'UNDO'", this);
                                        ActionStack.Undo();
                                        e.Use();
                                        return;
                                    }

                                case KeyCode.Y:
                                    Debug.LogWarning($"[{GetType()}] 'REDO'", this);
                                    ActionStack.Redo();
                                    e.Use();
                                    return;

                                case KeyCode.C:
                                    Debug.LogWarning($"[{GetType()}] 'COPY'", this);
                                    e.Use();
                                    return;

                                case KeyCode.X:
                                    Debug.LogWarning($"[{GetType()}] 'CUT'", this);
                                    e.Use();
                                    return;

                                case KeyCode.V:
                                    Debug.LogWarning($"[{GetType()}] 'PASTE'", this);
                                    e.Use();
                                    return;
                            }
                        break;
                }

            switch (e.type)
            {
                case EventType.KeyDown:
                case EventType.MouseDown:
                case EventType.ScrollWheel:
                    for (int i = inputs_users._collection.Count - 1; i >= 0; i--)
                    {
                        var on_inputs = inputs_users._collection[i];
                        if (on_inputs(e))
                        {
                            e.Use();
                            return;
                        }
                    }
                    break;
            }

            for (int i = gui_users._collection.Count - 1; i >= 0; i--)
            {
                var on_gui = gui_users._collection[i];
                if (on_gui(e))
                {
                    e.Use();
                    return;
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            gui_users.Reset();
        }
    }
}