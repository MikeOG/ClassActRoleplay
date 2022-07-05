using CitizenFX.Core;
using Roleplay.Client.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Client.Enums
{
    internal static class ControlHelper
    {
        public static Dictionary<ControlModifier, int> ModifierFlagToKeyCode = new Dictionary<ControlModifier, int>()
        {
            [ControlModifier.Ctrl] = 36,
            [ControlModifier.Alt] = 19,
            [ControlModifier.Shift] = 21
        };
        private const int defaultControlGroup = 0;
        private const int controllerControlGroup = 2;
        public static bool WasLastInputFromController()
        {
            return !NativeWrappers.IsInputDisabled(2);
        }

        public static bool IsControlModifierPressed(ControlModifier modifier)
        {
            if (modifier == ControlModifier.Any)
                return true;
            ControlModifier BitMask = ControlModifier.None;
            ControlHelper.ModifierFlagToKeyCode.ToList<KeyValuePair<ControlModifier, int>>().ForEach((Action<KeyValuePair<ControlModifier, int>>)(w =>
            {
                if (!Game.IsControlPressed(0, (Control)w.Value))
                    return;
                BitMask |= w.Key;
            }));
            return BitMask == modifier;
        }

        public static bool IsControlJustPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlJustPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsControlPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsControlJustReleased(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlJustReleased(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlJustPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlJustPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlJustReleased(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlJustReleased(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlJustPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlJustPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlJustReleased(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlJustPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlPressed(
          Control control,
          bool keyboardOnly = true,
          ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlPressed(0, control) && (!keyboardOnly || keyboardOnly && !ControlHelper.WasLastInputFromController()) && ControlHelper.IsControlModifierPressed(modifier);
        }

        public static bool IsControlJustPressed(ControlSetting setting)
        {
            return ControlHelper.IsControlJustPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsControlPressed(ControlSetting setting)
        {
            return ControlHelper.IsControlPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsControlJustReleased(ControlSetting setting)
        {
            return ControlHelper.IsControlJustReleased(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsDisabledControlJustPressed(ControlSetting setting)
        {
            return ControlHelper.IsDisabledControlJustPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsDisabledControlPressed(ControlSetting setting)
        {
            return ControlHelper.IsDisabledControlPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsDisabledControlJustReleased(ControlSetting setting)
        {
            return ControlHelper.IsDisabledControlJustReleased(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsEnabledControlJustPressed(ControlSetting setting)
        {
            return ControlHelper.IsEnabledControlJustPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsEnabledControlJustReleased(ControlSetting setting)
        {
            return ControlHelper.IsEnabledControlJustReleased(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }

        public static bool IsEnabledControlPressed(ControlSetting setting)
        {
            return ControlHelper.IsEnabledControlPressed(setting.Control, setting.KeyboardOnly, setting.Modifier);
        }
    }
}
