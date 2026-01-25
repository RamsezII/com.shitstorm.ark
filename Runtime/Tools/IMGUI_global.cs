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

            if (false)
                if (e.type == EventType.KeyDown || e.type == EventType.ValidateCommand || e.type == EventType.ExecuteCommand)
                    Debug.Log($"[TERM GUI] type={e.type} key={e.keyCode} char={(int)e.character} commandName={e.commandName} keyboardControl={GUIUtility.keyboardControl} hotControl={GUIUtility.hotControl}");

            if (e.type == EventType.ExecuteCommand)
                switch (e.commandName)
                {
                    case "Copy":
                        if (clipboard_users.IsNotEmpty)
                            foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                if (element(e, ClipboardOperations.Copy))
                                {
                                    e.Use();
                                    return;
                                }
                        break;

                    case "Paste":
                        if (clipboard_users.IsNotEmpty)
                            foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                if (element(e, ClipboardOperations.Paste))
                                {
                                    e.Use();
                                    return;
                                }
                        break;

                    case "Cut":
                        if (clipboard_users.IsNotEmpty)
                            foreach (var (index, element) in clipboard_users.ReversedOrderIteration())
                                if (element(e, ClipboardOperations.Cut))
                                {
                                    e.Use();
                                    return;
                                }
                        break;
                }

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
                                        ActionStack.Undo_focused();
                                        e.Use();
                                        return;
                                    }

                                case KeyCode.Y:
                                    ActionStack.Redo_focused();
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