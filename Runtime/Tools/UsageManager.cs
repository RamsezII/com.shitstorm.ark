using _UTIL_;
using System;
using System.Text;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace _ARK_
{
    public enum UsageGroups : byte
    {
        GameMouse,
        TrueMouse,
        Keyboard,
        Typing,
        BlockPlayer,
        IMGUI,
        _last_
    }

    public enum MouseStatus : byte
    {
        None,
        GameMouse,
        TrueMouse,
    }

    public static class UsageManager
    {
        [AutoStaticsCleanup] public static readonly ListListener[] usages = new ListListener[(int)UsageGroups._last_];

        [AutoStaticsCleanup] static float last_ALT;

        [AutoStaticsCleanup] static readonly object mouse_user = new();
        [AutoStaticsCleanup] public static Action on_double_alt, on_usages_change;

        [AutoStaticsCleanup] public static readonly ValueNotifier<MouseStatus> mouse_status = new();
        public static bool BlockPlayerInputs => !AllAreEmpty(UsageGroups.BlockPlayer, UsageGroups.GameMouse, UsageGroups.TrueMouse);

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_ARK_) + "/" + nameof(UsageManager) + "." + nameof(_LogUsages))]
        static void _LogUsages()
        {
            StringBuilder sb = new();

            sb.AppendLine("USAGES :");
            for (int i = 0; i < (int)UsageGroups._last_; i++)
            {
                sb.AppendLine($"[{i}] {(UsageGroups)i} : {usages[i]._collection.Count}");
                for (int j = 0; j < usages[i]._collection.Count; j++)
                    sb.AppendLine($"    [{j}] {usages[i]._collection[j]}");
            }

            Debug.Log(sb);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            for (int i = 0; i < (int)UsageGroups._last_; i++)
            {
                usages[i] = new ListListener();
                usages[i].AddListener2(list => on_usages_change?.Invoke(), doNotCallThisTime: false);
            }

            usages[(int)UsageGroups.GameMouse].AddListener1(_ => UpdateCursorState());
            usages[(int)UsageGroups.TrueMouse].AddListener1(_ => UpdateCursorState());

            mouse_status.AddListener(value =>
            {
                switch (value)
                {
                    case MouseStatus.None:
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        break;

                    case MouseStatus.GameMouse:
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = false;
                        break;

                    case MouseStatus.TrueMouse:
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        break;
                }
            });
        }

        //----------------------------------------------------------------------------------------------------------

        static void UpdateCursorState()
        {
            if (usages[(int)UsageGroups.GameMouse].IsNotEmpty)
                mouse_status.Value = MouseStatus.GameMouse;
            else if (usages[(int)UsageGroups.TrueMouse].IsNotEmpty)
                mouse_status.Value = MouseStatus.TrueMouse;
            else
                mouse_status.Value = MouseStatus.None;
        }

        public static void UpdateAltPress()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                float time = Time.unscaledTime;
                if (time - last_ALT < 0.3f)
                    if (on_double_alt != null)
                        on_double_alt();
                    else
                        usages[(int)UsageGroups.TrueMouse].ToggleElement(mouse_user);
                last_ALT = time;
            }
#endif
        }

        public static void ToggleUser(object user, bool toggle, params UsageGroups[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
                usages[(int)groups[i]].ToggleElement(user, toggle);
        }

        public static void AddUser(object user, params UsageGroups[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
                usages[(int)groups[i]].AddElement(user);
        }

        public static void RemoveUser(object user)
        {
            for (int i = 0; i < (int)UsageGroups._last_; i++)
                usages[i].RemoveElement(user);
        }

        public static bool AllAreEmpty(params UsageGroups[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
                if (!usages[(int)groups[i]].IsEmpty)
                    return false;
            return true;
        }

        public static bool AreEmptyOrLast(in object user, params UsageGroups[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
                if (!usages[(int)groups[i]].IsEmptyOrLast(user))
                    return false;
            return true;
        }
    }
}