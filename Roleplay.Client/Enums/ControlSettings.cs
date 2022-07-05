using CitizenFX.Core;

namespace Roleplay.Client.Enums
{
    public class ControlSetting
    {
        public Control Control { get; internal set; }

        public ControlModifier Modifier { get; internal set; }

        public bool KeyboardOnly { get; internal set; }

        public ControlSetting(Control control, bool keyboardOnly = true, ControlModifier modifier = ControlModifier.None)
        {
            this.Control = control;
            this.Modifier = modifier;
            this.KeyboardOnly = keyboardOnly;
        }
    }
}
