using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using Rectangle = CitizenFX.Core.UI.Rectangle;

namespace Roleplay.Client.UI.Classes
{
    class Rect : Rectangle
    {
        public bool IsBeingDrawn { get; private set; }
        private Func<Task> drawTick;

        public Rect(int x, int y, int width, int height)
            : this(x, y, width, height, null, Color.FromArgb(255, 0, 0, 0), false) { }

        public Rect(int x, int y, int width, int height, Func<Task> drawTick)
            : this(x, y, width, height, drawTick, Color.FromArgb(255, 0, 0, 0), false) { }

        public Rect(int x, int y, int width, int height, Func<Task> drawTick, Color boxColour)
            : this(x, y, width, height, drawTick, boxColour, false) { }

        public Rect(int x, int y, int width, int height, Func<Task> drawTick, Color boxColour, bool centered)
            : base(new PointF(x * 0.6666666667f, y * 0.6666666667f), new SizeF(width * 0.6666666667f, height * 0.6666666667f), boxColour, centered)
        {
            this.drawTick = drawTick;
        }

        ~Rect()
        {
            if(IsBeingDrawn) Client.Instance.DeregisterTickHandler(drawTick ?? defaultTick);
        }

        public void Draw()
        {
            if (!IsBeingDrawn)
            {
                if(drawTick != null)
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
                if(drawTick != null)
                    Client.Instance.DeregisterTickHandler(drawTick);

                Client.Instance.DeregisterTickHandler(defaultTick);
            }    
        }

        private async Task defaultTick() => base.Draw();
    }
}
