using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace _ARK_
{
    public sealed class CursorManager : MonoBehaviour
    {
        public static CursorManager instance;

        public Mouse hwmouse, vmouse;
        public Vector2 position;

        public bool stopMoving;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            return;
            Util.InstantiateOrCreateIfAbsent<CursorManager>();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;

            hwmouse = Mouse.current;
            if (hwmouse != null && false)
                InputSystem.DisableDevice(hwmouse);

            vmouse = InputSystem.GetDevice<Mouse>("VirtualMouse") ?? InputSystem.AddDevice<Mouse>("VirtualMouse");

            InputSystem.EnableDevice(vmouse);

            position = .5f * new Vector2(Screen.width, Screen.height);
        }

        //----------------------------------------------------------------------------------------------------------

        internal void MoveMouse()
        {
            // --- 1) Lire les sources ---
            Mouse mouse = Mouse.current;
            Vector2 delta = stopMoving ? Vector2.zero : mouse.delta.ReadValue();
            Vector2 scroll = mouse.scroll.ReadValue();

            // --- 2) Construire delta “virtuel” ---
            position += delta;

            // Clamp à l’écran
            position = Vector2.Max(Vector2.zero, position);
            position = Vector2.Min(position, new Vector2(Screen.width - 1, Screen.height - 1));

            // --- 3) Boutons virtuels ---
            bool leftPressed = false;
            bool rightPressed = false;

            if (mouse != null)
            {
                leftPressed |= mouse.leftButton.isPressed;
                rightPressed |= mouse.rightButton.isPressed;
            }

            // --- 4) Emettre l’état de la souris virtuelle ---
            InputSystem.QueueDeltaStateEvent(vmouse.position, position);
            InputSystem.QueueDeltaStateEvent(vmouse.delta, delta);

            // Boutons (bitfield: 1=left, 2=right, 4=middle)
            ushort buttons = 0;
            if (leftPressed)
                buttons |= 1;
            if (rightPressed)
                buttons |= 2;

            // Scroll (positive up)
            if (scroll.sqrMagnitude > Mathf.Epsilon)
                InputSystem.QueueDeltaStateEvent(vmouse.scroll, scroll);

            // État complet (utile pour être sûr que les flags/buttons sont cohérents)
            MouseState state = new()
            {
                position = position,
                delta = delta,
                buttons = buttons,
                scroll = scroll
            };
            InputSystem.QueueStateEvent(vmouse, state);

            if (false)
                InputSystem.Update(); // pousse l’état tout de suite (facultatif)
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            InputSystem.RemoveDevice(vmouse);
        }
    }
}