using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Shared.Models
{
    public class VehicleDataModel
    {
#if CLIENT
        public string LicensePlate { get; set; }
        public int Class { get; set; }
        //public float VehicleFuel { get; set; } = -1f;
        public VehicleHash Model { get; set; }
        public float EngineHealth { get; set; }
        public float BodyHealth { get; set; }
        public int Livery { get; set; }
        public VehicleColor PrimaryColor { get; set; }
        public VehicleColor SecondaryColor { get; set; }
        public VehicleColor PearlescentColor { get; set; }
        public VehicleColor RimColor { get; set; }
        public VehicleWheelType WheelType { get; set; }
        public VehicleWindowTint WindowTint { get; set; }
        public LicensePlateStyle LicensePlateStyle { get; set; }
        public Dictionary<VehicleModType, int> Mods { get; set; } = new Dictionary<VehicleModType, int>();
        public Dictionary<VehicleToggleModType, bool> ToggleMods { get; set; } = new Dictionary<VehicleToggleModType, bool>();
        public Dictionary<int, bool> Extras { get; set; } = new Dictionary<int, bool>();
        public bool CustomWheelVariation { get; set; }

        public Dictionary<VehicleNeonLight, bool> NeonLights { get; set; } = new Dictionary<VehicleNeonLight, bool>();
        public int[] NeonLightsColour { get; set; } = {255, 255, 0};
#elif SERVER
        public string LicensePlate { get; set; }
        public int Class { get; set; }
        public float VehicleFuel { get; set; } = -1f;
        public long Model { get; set; }
        public float EngineHealth { get; set; }
        public float BodyHealth { get; set; }
        public int Livery { get; set; }
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public int PearlescentColor { get; set; }
        public int RimColor { get; set; }
        public int WheelType { get; set; }
        public int WindowTint { get; set; }
        public int LicensePlateStyle { get; set; }
        public Dictionary<string, int> Mods { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, bool> ToggleMods { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, bool> Extras { get; set; } = new Dictionary<string, bool>();
        public bool CustomWheelVariation { get; set; }

        public Dictionary<string, bool> NeonLights { get; set; } = new Dictionary<string, bool>();
        public int[] NeonLightsColour { get; set; } = { 255, 255, 0 };
#endif
    }
}
