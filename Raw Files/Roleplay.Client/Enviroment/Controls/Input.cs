using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Enums;

namespace Roleplay.Client.Player.Controls
{
    /// <summary>
    /// Some control keycodes to get you started: https://wiki.fivem.net/wiki/Controls
    /// Alternatively, type /dev keycodetester in-game and press a key you wish to use
    /// Be way control codes can be bound to multiple keys as well
    /// (I.e. there are cross-binds in all directions)
    /// </summary>
    public static class Input
    {
        // To be moved to constants
        const int defaultControlGroup = 0;
        const int controllerControlGroup = 2;
        public static Dictionary<ControlModifier, int> ModifierFlagToKeyCode = new Dictionary<ControlModifier, int>()
        {
            [ControlModifier.Ctrl] = 36,
            [ControlModifier.Alt] = 19,
            [ControlModifier.Shift] = 21
        };

        public static bool WasLastInputFromController()
        {
            return !IsInputDisabled(controllerControlGroup);
        }

        /// <summary>
        /// Lets you know if the specified modifier is pressed
        /// </summary>
        /// <param name="modifier">You can either specify just one Modifier or combine multiple (with |)</param>
        /// <returns></returns>
        public static bool IsControlModifierPressed(ControlModifier modifier)
        {
            if (Phone.PhoneKeyboardOpen) return false;

            if (modifier == ControlModifier.Any)
            {
                return true;
            }
            else
            {
                ControlModifier BitMask = 0;
                ModifierFlagToKeyCode.ToList().ForEach(w =>
                {
                    if (Game.IsControlPressed(defaultControlGroup, (Control)w.Value))
                    {
                        BitMask = BitMask | w.Key;
                    }
                });
                if (BitMask == modifier)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsControlJustPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlJustPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsControlPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsControlJustReleased(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsControlJustReleased(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlJustPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlJustPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlJustReleased(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlJustReleased(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsDisabledControlPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsDisabledControlPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlJustPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlJustPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlJustReleased(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlJustPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        public static bool IsEnabledControlPressed(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            return Game.IsEnabledControlPressed(0, control) && (!keyboardOnly || (keyboardOnly && !WasLastInputFromController())) && IsControlModifierPressed(modifier);
        }

        /// <summary>
        /// Waits until a user releases a key or until the specified timeout time is reached
        /// </summary>
        /// <param name="control">The <see cref="Control"/> that is wanting to be waited for release</param>
        /// <param name="modifier">The <see cref="ControlModifier"/> of the current bind</param>
        /// <param name="timeout">How long the wait should be before until the loop is existed</param>
        /// <returns>Returns if the player held the control for longer than the specified time</returns>
        public static async Task<bool> WaitForKeyRelease(Control control, ControlModifier modifier = ControlModifier.None, int timeout = 1000)
        {
            var currentTicks = Game.GameTime + 1;

            await BaseScript.Delay(0);

            while (IsControlPressed(control, true, modifier) && Game.GameTime - currentTicks < timeout)
                await BaseScript.Delay(0);

            return Game.GameTime - currentTicks >= timeout;
        }
    }
}
