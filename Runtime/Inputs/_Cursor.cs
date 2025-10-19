using System;
using UnityEngine;

namespace _ARK_
{
    partial class UsageManager
    {
        enum Cursors : byte
        {
            TrueMouse,
            GameMouse,
            BlockCamera,
        }

        [Flags]
        enum CursorsM : byte
        {
            TrueMouse = 1 << Cursors.TrueMouse,
            GameMouse = 1 << Cursors.GameMouse,
            RotateCamera = 1 << Cursors.BlockCamera,
        }

        static void UpdateCursorState()
        {
            CursorsM mask = 0;

            if (usages[(int)UsageGroups.TrueMouse].IsNotEmpty)
                mask |= CursorsM.TrueMouse;

            if (usages[(int)UsageGroups.GameMouse].IsNotEmpty)
                mask |= CursorsM.GameMouse;

            if (usages[(int)UsageGroups.BlockCamera].IsNotEmpty)
                mask |= CursorsM.RotateCamera;

            switch (mask)
            {
                case CursorsM.TrueMouse:
                case CursorsM.GameMouse | CursorsM.TrueMouse:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;

                case CursorsM.GameMouse:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    break;

                case 0:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
            }
        }
    }
}