using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using Roleplay.Shared;
using Font = CitizenFX.Core.UI.Font;

namespace Roleplay.Client.UI.Classes
{
    public class ScreenText : Text
    {
        public bool IsBeingDrawn { get; private set; }
        private Func<Task> drawTick;

        public ScreenText(string text, int x, int y, float scale)
            : this(text, x, y, scale, null, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, Alignment.Center, true, true)
        {
            
        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick)
            : this(text, x, y, scale, drawTick, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, Alignment.Center, true, true)
        {

        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick, Color textColour)
            : this(text, x, y, scale, drawTick, textColour, Font.ChaletLondon, Alignment.Center, true, true)
        {

        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick, Color textColour, Font textFont)
            : this(text, x, y, scale, drawTick, textColour, textFont, Alignment.Center, true, true)
        {

        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick, Color textColour, Font textFont, Alignment textAlignment)
            : this(text, x, y, scale, drawTick, textColour, textFont, textAlignment, true, true)
        {

        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick, Color textColour, Font textFont, Alignment textAlignment, bool useShadow)
            : this(text, x, y, scale, drawTick, textColour, textFont, textAlignment, useShadow, true)
        {

        }

        public ScreenText(string text, int x, int y, float scale, Func<Task> drawTick, Color textColour, Font textFont, Alignment textAlignment, bool useShadow, bool haveOutline)
            : base(text, new PointF(x * 0.6666666667f, y * 0.6666666667f), scale, textColour, textFont, textAlignment, useShadow, haveOutline)
        {
            this.drawTick = drawTick;
        }

        ~ScreenText()
        {
            if (IsBeingDrawn) Client.Instance.DeregisterTickHandler(drawTick ?? defaultTick);
        }

        public override void Draw()
        {
            if (!IsBeingDrawn)
            {
                if (drawTick != null)
                    Client.Instance.RegisterTickHandler(drawTick);

                Client.Instance.RegisterTickHandler(defaultTick);
                IsBeingDrawn = true;
            }
        }

        public void DrawTick()
        {
            if (!IsBeingDrawn)
            {
                base.Draw();
                if (drawTick != null) drawTick();
            }
        }

        public void StopDraw()
        {
            if (IsBeingDrawn)
            {
                if (drawTick != null)
                    Client.Instance.DeregisterTickHandler(drawTick);

                Client.Instance.DeregisterTickHandler(defaultTick);

                IsBeingDrawn = false;
            }
        }

        private async Task defaultTick() => base.Draw();
    }
}
