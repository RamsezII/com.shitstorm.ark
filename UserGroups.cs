using _UTIL_;
using UnityEngine;

namespace _ARK_
{
    public interface IMouseUser : IUserGroup
    {
    }

    public interface IInputsUser : IUserGroup
    {
    }

    partial class NUCLEOR : IMouseUser, IInputsUser
    {
        public static readonly UserGroup<IMouseUser> mouseUsers = new();
        public static readonly UserGroup<IInputsUser> inputsUsers = new();
        
        float lastMouseBlockKey;

        //----------------------------------------------------------------------------------------------------------

        void AwakeUserGroups()
        {
            mouseUsers.Clear();
            inputsUsers.Clear();

            mouseUsers.isUsed.onChange += value =>
            {
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = value;
            };
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
    }
}