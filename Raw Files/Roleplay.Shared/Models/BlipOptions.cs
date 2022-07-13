using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Shared.Models
{
    public class BlipOptions
    {
#if CLIENT
        public BlipSprite Sprite = BlipSprite.BigCircle;
        public BlipColor Colour = BlipColor.White;
#elif SERVER
        public int Sprite = 9;
        public int Colour = 0;
#endif
        public float Scale = 0.8f;
        public bool IsShortRange = true;
    }
}
