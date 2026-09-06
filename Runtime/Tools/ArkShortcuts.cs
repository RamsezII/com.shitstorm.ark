using System;
using System.Collections.Generic;
using _UTIL_;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace _ARK_
{
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

        public static readonly IA_ArkShortcuts IA_main = new();

        public static bool Ctrl => IA_main.ArkShortcuts.control.IsPressed();
        public static bool Alt => IA_main.ArkShortcuts.alt.IsPressed();
        public static bool Shift => IA_main.ArkShortcuts.shift.IsPressed();
        public static bool Ctrl_Alt_Shift_or => Ctrl || Alt || Shift;
        public static bool Ctrl_Alt_Shift_and => Ctrl && Alt && Shift;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            foreach (var pair in shortcuts)
                pair.Key.Dispose();
            shortcuts.Clear();
            IA_main.Enable();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void RemoveShortcut(Action action) => NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
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
        };

        public static void AddShortcut_keyboard(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params Key[] bindings
        ) => AddShortcut_internal<Keyboard, Key>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_keyboard(binding)
        );

        public static void AddShortcut_keyboard_special(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params KeyboardSpecial[] bindings
        ) => AddShortcut_internal<Keyboard, KeyboardSpecial>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_keyboard_special(binding)
        );

        public static void AddShortcut_mouse(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params MouseButton[] bindings
        ) => AddShortcut_internal<Mouse, MouseButton>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_mouse(binding)
        );

        public static void AddShortcut_mouse_special(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params MouseSpecial[] bindings
        ) => AddShortcut_internal<Mouse, MouseSpecial>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_mouse_special(binding)
        );

        public static void AddShortcut_gamepad(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params GamepadButton[] bindings
        ) => AddShortcut_internal<Gamepad, GamepadButton>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_gamepad(binding)
        );

        public static void AddShortcut_gamepad_special(
            in string shortcutName,
            in Action action,
            in bool control = false,
            in bool shift = false,
            in bool alt = false,
            params GamepadSpecial[] bindings
        ) => AddShortcut_internal<Gamepad, GamepadSpecial>(
            shortcutName: shortcutName,
            action: action,
            control: control,
            shift: shift,
            alt: alt,
            bindings: bindings,
            addBinding: static (input, binding) => input.AddBinding_gamepad_special(binding)
        );

        static void AddShortcut_internal<TDevice, TBinding>(
            in string shortcutName,
            in Action action,
            in bool control,
            in bool shift,
            in bool alt,
            TBinding[] bindings,
            Action<InputAction, TBinding> addBinding
        ) where TDevice : InputDevice
        {
            var input = new InputAction(
                name: shortcutName,
                type: InputActionType.Button
            );

            for (int i = 0; i < bindings.Length; i++)
                addBinding(input, bindings[i]);

            input.performed += OnShortcutPerformed;

            input.Enable();
            shortcuts.Add(input, new(typeof(TDevice), action, control, shift, alt));
        }

        static void OnShortcutPerformed(InputAction.CallbackContext context)
        {
            if (!shortcuts.TryGetValue(context.action, out ShortcutInfos shortcut))
            {
                Debug.LogWarning($"Could not recognize inputAction named: \"{context.action.name}\"");
                return;
            }

            if (context.control.device is Keyboard)
                if (!shortcut.control && !shortcut.alt && !shortcut.shift)
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
