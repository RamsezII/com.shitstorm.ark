using _UTIL_;
using UnityEngine;

namespace _ARK_
{
    public interface IMouseUser : IUserGroup
    {
    }

    public interface IKeyboardUser : IUserGroup
    {
    }

    public interface IInputsUser : IUserGroup
    {
    }

    partial class NUCLEOR : IMouseUser, IInputsUser
    {
        public static readonly UserGroup<IMouseUser> mouseUsers = new();
        public static readonly UserGroup<IKeyboardUser> keyboardUsers = new();
        public static readonly UserGroup<IInputsUser> inputsUsers = new();

        float lastMouseBlockKey;

        //----------------------------------------------------------------------------------------------------------

        void AwakeUserGroups()
        {
            ClearUserGroups();

            mouseUsers.isUsed.AddOnChange(value =>
            {
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = value;
            });
        }

        //----------------------------------------------------------------------------------------------------------

        void UpdateUserGroups()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                float time = Time.unscaledTime;
                float delta = time - lastMouseBlockKey;
                if (delta < 0.3f)
                    mouseUsers.ToggleUser(this);
                lastMouseBlockKey = time;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        public static void ClearUserGroups()
        {
            mouseUsers.Clear();
            keyboardUsers.Clear();
            inputsUsers.Clear();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}