using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace _ARK_
{
    using CallbackContext = InputAction.CallbackContext;
    public static class ArkShortcuts
    {
        readonly struct ShortcutInfos
        {
            public readonly Type deviceType;
            public readonly Action action;
            public readonly bool control, shift, alt;

            //----------------------------------------------------------------------------------------------------------

            public ShortcutInfos(
                in Type deviceType,
                in Action action,
                in bool control = false,
                in bool shift = false,
                in bool alt = false
            )
            {
                this.deviceType = deviceType;
                this.action = action;
                this.control = control;
                this.shift = shift;
                this.alt = alt;
            }
        }

        static readonly Dictionary<InputAction, ShortcutInfos> shortcuts = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            foreach (var pair in shortcuts)
                pair.Key.Dispose();
            shortcuts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void RemoveShortcut(in Action action)
        {
            List<InputAction> removes = new();

            foreach (var pair in shortcuts)
                if (pair.Value.action == action)
                    removes.Add(pair.Key);

            for (int i = 0; i < removes.Count; i++)
            {
                shortcuts.Remove(removes[i]);
                removes[i].Dispose();
            }
        }

        public static void AddShortcut(
            in string shortcutName,
            in string nameof_button,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false
        ) => AddShortcut<Keyboard>(
            shortcutName: shortcutName,
            nameof_button: nameof_button,
            action: action,
            control: control,
            shift: shift,
            alt: alt
        );

        public static void AddShortcut<T>(
            in string shortcutName,
            in string nameof_button,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false
        ) where T : InputDevice
        {
            var input = new InputAction(
                name: shortcutName,
                type: InputActionType.Button
            );

            input.AddBinding($"<{typeof(T).Name}>/{nameof_button}");

            input.performed += OnShortcutPerformed;

            input.Enable();
            shortcuts.Add(input, new(typeof(T), action, control, shift, alt));
        }

        static void OnShortcutPerformed(CallbackContext context)
        {
            if (!shortcuts.TryGetValue(context.action, out ShortcutInfos shortcut))
            {
                Debug.LogWarning($"Could not recognize inputAction named: \"{context.action.name}\"");
                return;
            }

            if (context.control.device is Keyboard)
                if (!shortcut.control && !shortcut.alt)
                    if (NUCLEOR.instance.isTyping._value || !UsageManager.AllAreEmpty(UsageGroups.Typing))
                        return;

            bool
                control = false,
                shift = false,
                alt = false;

            if (Keyboard.current != null)
            {
                control = Keyboard.current.ctrlKey.isPressed || Keyboard.current.leftCommandKey.isPressed || Keyboard.current.rightCommandKey.isPressed;
                shift = Keyboard.current.shiftKey.isPressed;
                alt = Keyboard.current.altKey.isPressed;
            }

            if (shortcut.deviceType.IsAssignableFrom(context.control.device.GetType()))
                if (control == shortcut.control)
                    if (shift == shortcut.shift)
                        if (alt == shortcut.alt)
                            if (context.action.WasPressedThisFrame())
                                shortcut.action();
        }
    }
}