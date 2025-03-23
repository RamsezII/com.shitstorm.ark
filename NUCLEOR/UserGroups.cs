using _UTIL_;
using System;
using UnityEngine;

namespace _ARK_
{
    partial class NUCLEOR
    {
        [Flags]
        public enum Usages : byte
        {
            IngameMouse = 1,
            TrueMouse = 2,
            Keyboard = 4,
            Typing = 8,
            BlockPlayers = 16,
            _all_ = byte.MaxValue
        }

        public static readonly ListListener users_ingameMouse = new();
        public static readonly ListListener users_trueMouse = new();
        public static readonly ListListener users_keyboards = new();
        public static readonly ListListener users_typing = new();
        public static readonly ListListener users_blockPlayers = new();

        float last_ALT;

        //----------------------------------------------------------------------------------------------------------

        void AwakeUserGroups()
        {
            users_ingameMouse.Reset();
            users_trueMouse.Reset();
            users_keyboards.Reset();
            users_typing.Reset();
            users_blockPlayers.Reset();

            users_trueMouse.AddListener1(_ => UpdateCursorState());
            users_ingameMouse.AddListener1(_ => UpdateCursorState());

            static void UpdateCursorState()
            {
                if (users_ingameMouse.IsEmpty && users_trueMouse.IsEmpty)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if (users_trueMouse.IsEmpty)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------

        void UpdateAltPress()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                float time = Time.unscaledTime;
                if (time - last_ALT < 0.3f)
                    users_trueMouse.ToggleElement(this);
                last_ALT = time;
            }
        }

        public static void ToggleUser(object user, bool toggle, Usages usages)
        {
            if (usages.HasFlag(Usages.IngameMouse))
                users_ingameMouse.ToggleElement(user, toggle);
            if (usages.HasFlag(Usages.TrueMouse))
                users_trueMouse.ToggleElement(user, toggle);
            if (usages.HasFlag(Usages.Keyboard))
                users_keyboards.ToggleElement(user, toggle);
            if (usages.HasFlag(Usages.Typing))
                users_typing.ToggleElement(user, toggle);
            if (usages.HasFlag(Usages.BlockPlayers))
                users_blockPlayers.ToggleElement(user, toggle);
        }

        public static void RemoveUser(object user)
        {
            users_ingameMouse.RemoveElement(user);
            users_trueMouse.RemoveElement(user);
            users_keyboards.RemoveElement(user);
            users_typing.RemoveElement(user);
            users_blockPlayers.RemoveElement(user);
        }
    }
}