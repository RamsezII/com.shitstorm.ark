using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    public sealed class IMGUI_global : MonoBehaviour
    {
        public enum ClipboardOperations : byte
        {
            Copy,
            Cut,
            Paste
        }

        public static IMGUI_global instance;

        public readonly ListListener<Action>
            escape_users = new();

        public readonly ListListener<Func<Event, ClipboardOperations, bool>>
            clipboard_users = new();

        public readonly ListListener<Func<Event, bool>>
            gui_users = new(),
            inputs_users = new();

#if UNITY_EDITOR
        [SerializeField] int _frame, _gui_frame;
#endif

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Util.InstantiateOrCreateIfAbsent<IMGUI_global>();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            if (instance != null)
                Debug.LogError($"the fuck");

            instance = this;
            DontDestroyOnLoad(gameObject);
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
                        if (escape_users._collection.Count > 0)
                        {
                            escape_users._collection[^1]();
                            e.Use();
                            return;
                        }
                        break;

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
                                    if (clipboard_users.IsNotEmpty)
                                        foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                            if (element(e, ClipboardOperations.Copy))
                                            {
                                                e.Use();
                                                return;
                                            }
                                    Debug.LogWarning($"[{GetType()}] 'Copy'", this);
                                    e.Use();
                                    return;

                                case KeyCode.X:
                                    if (clipboard_users.IsNotEmpty)
                                        foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                            if (element(e, ClipboardOperations.Cut))
                                            {
                                                e.Use();
                                                return;
                                            }
                                    Debug.LogWarning($"[{GetType()}] 'CUT'", this);
                                    e.Use();
                                    return;

                                case KeyCode.V:
                                    if (clipboard_users.IsNotEmpty)
                                        foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                            if (element(e, ClipboardOperations.Paste))
                                            {
                                                e.Use();
                                                return;
                                            }
                                    Debug.LogWarning($"[{GetType()}] 'PASTE'", this);
                                    e.Use();
                                    return;
                            }
                        break;
                }

            if (inputs_users.IsNotEmpty)
                switch (e.type)
                {
                    case EventType.KeyDown:
                    case EventType.MouseDown:
                    case EventType.ScrollWheel:
                        foreach (var (index, element) in inputs_users.ReversedOrderIteration())
                            if (element(e))
                            {
                                e.Use();
                                return;
                            }
                        break;
                }

            if (gui_users.IsNotEmpty)
                foreach (var (index, element) in gui_users.ReversedOrderIteration())
                    if (element(e))
                    {
                        e.Use();
                        return;
                    }
        }
    }
}