using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Shared.Models
{
    public class MarkerOptions
    {
        public Vector3 Rotation { get; set; }
        public Vector3 Direction { get; set; }
#if CLIENT
        public MarkerType Type { get; set; } = MarkerType.HorizontalCircleFat;
#elif SERVER
        public int Type { get; set; } = 23;
#endif
        public float ScaleFloat = 1.0f;
        public Vector3 ScaleVector { get; set; } = new Vector3(1f, 1f, 1f);
        public Vector3 Scale => ScaleFloat * ScaleVector;
        [JsonIgnore]
        public System.Drawing.Color Color { get; set; } = System.Drawing.Color.FromArgb(120, ConstantColours.Yellow);
        public int[] ColorArray { get; set; } = null;
        public System.Drawing.Color ColorInternal => this.ColorArray == null ? this.Color : System.Drawing.Color.FromArgb(120, this.ColorArray[0], this.ColorArray[1], this.ColorArray[2]);
        public float zOffset = 0.0f;
        public string MarkerId;
    }
}
