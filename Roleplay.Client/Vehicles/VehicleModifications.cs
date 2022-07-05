using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Client.Vehicles
{
    public class VehicleModifications
    {
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public int PearlescentColor { get; set; }
        public int WheelType { get; set; }
        public int WheelColor { get; set; }
        public int WindowTint { get; set; }
        public bool[] Neon { get; set; }
        public int[] NeonColors { get; set; }
        public int[] TyreSmokeColor { get; set; }
        public int Spoiler { get; set; }
        public int FrontBumper { get; set; }
        public int RearBumper { get; set; }
        public int SideSkirt { get; set; }
        public int Exhaust { get; set; }
        public int Frame { get; set; }
        public int Grille { get; set; }
        public int Hood { get; set; }
        public int Fender { get; set; }
        public int SecondaryFender { get; set; }
        public int Roof { get; set; }
        public int Engine { get; set; }
        public int Brakes { get; set; }
        public int Transmission { get; set; }
        public int Horns { get; set; }
        public int Suspension { get; set; }
        public int Armor { get; set; }
        public bool Turbo { get; set; }
        public bool SmokeEnabled { get; set; }
        public bool Xenon { get; set; }
        public int FrontWheels { get; set; }
        public int BackWheels { get; set; }
        public int PlateHolder { get; set; }
        public int VanityPlate { get; set; }
        public int TrimPrimary { get; set; }
        public int TrimSecondary { get; set; }
        public int Ornaments { get; set; }
        public int Dashboard { get; set; }
        public int Dial { get; set; }
        public int DoorSpeaker { get; set; }
        public int Seats { get; set; }
        public int SteeringWheel { get; set; }
        public int ShifterLeavers { get; set; }
        public int PlateDecorations { get; set; }
        public int Speakers { get; set; }
        public int Trunk { get; set; }
        public int Hydrolic { get; set; }
        public int EngineBlock { get; set; }
        public int AirFilter { get; set; }
        public int Struts { get; set; }
        public int ArchCover { get; set; }
        public int Aerials { get; set; }
        public int Tank { get; set; }
        public int Windows { get; set; }
        public int Livery { get; set; }

        public void Install(int vehicle)
        {
            API.SetVehicleModKit(vehicle, 0);
            API.SetVehicleColours(vehicle, PrimaryColor, SecondaryColor);
            API.SetVehicleExtraColours(vehicle, PearlescentColor, WheelColor);
            API.SetVehicleWheelType(vehicle, WheelType);
            API.SetVehicleWindowTint(vehicle, WindowTint);

            if (Neon == null || Neon.Length < 4)
            {
                Neon = new[] { false, false, false, false };
            }

            API.SetVehicleNeonLightEnabled(vehicle, 0, Neon[0]);
            API.SetVehicleNeonLightEnabled(vehicle, 1, Neon[1]);
            API.SetVehicleNeonLightEnabled(vehicle, 2, Neon[2]);
            API.SetVehicleNeonLightEnabled(vehicle, 3, Neon[3]);

            if (NeonColors == null || NeonColors.Length < 3)
            {
                NeonColors = new[] { 255, 255, 255 };
            }

            API.SetVehicleNeonLightsColour(vehicle, NeonColors[0], NeonColors[1], NeonColors[2]);
            API.ToggleVehicleMod(vehicle, 20, SmokeEnabled);
            API.ToggleVehicleMod(vehicle, 18, Turbo);
            API.ToggleVehicleMod(vehicle, 22, Xenon);

            if (TyreSmokeColor == null || TyreSmokeColor.Length < 3)
            {
                TyreSmokeColor = new[] { 255, 255, 255 };
            }

            API.SetVehicleTyreSmokeColor(vehicle, TyreSmokeColor[0], TyreSmokeColor[1], TyreSmokeColor[2]);

            ApplyMod(vehicle, 0, Spoiler);
            ApplyMod(vehicle, 1, FrontBumper);
            ApplyMod(vehicle, 2, RearBumper);
            ApplyMod(vehicle, 3, SideSkirt);
            ApplyMod(vehicle, 4, Exhaust);
            ApplyMod(vehicle, 5, Frame);
            ApplyMod(vehicle, 6, Grille);
            ApplyMod(vehicle, 7, Hood);
            ApplyMod(vehicle, 8, Fender);
            ApplyMod(vehicle, 9, SecondaryFender);
            ApplyMod(vehicle, 10, Roof);
            ApplyMod(vehicle, 11, Engine);
            ApplyMod(vehicle, 12, Brakes);
            ApplyMod(vehicle, 13, Transmission);
            ApplyMod(vehicle, 14, Horns);
            ApplyMod(vehicle, 15, Suspension);
            ApplyMod(vehicle, 16, Armor);
            ApplyMod(vehicle, 23, FrontWheels);
            ApplyMod(vehicle, 24, BackWheels);
            ApplyMod(vehicle, 25, PlateHolder);
            ApplyMod(vehicle, 26, VanityPlate);
            ApplyMod(vehicle, 27, TrimPrimary);
            ApplyMod(vehicle, 28, Ornaments);
            ApplyMod(vehicle, 29, Dashboard);
            ApplyMod(vehicle, 30, Dial);
            ApplyMod(vehicle, 31, DoorSpeaker);
            ApplyMod(vehicle, 32, Seats);
            ApplyMod(vehicle, 33, SteeringWheel);
            ApplyMod(vehicle, 34, ShifterLeavers);
            ApplyMod(vehicle, 35, PlateDecorations);
            ApplyMod(vehicle, 36, Speakers);
            ApplyMod(vehicle, 37, Trunk);
            ApplyMod(vehicle, 38, Hydrolic);
            ApplyMod(vehicle, 39, EngineBlock);
            ApplyMod(vehicle, 40, AirFilter);
            ApplyMod(vehicle, 41, Struts);
            ApplyMod(vehicle, 42, ArchCover);
            ApplyMod(vehicle, 43, Aerials);
            ApplyMod(vehicle, 44, TrimSecondary);
            ApplyMod(vehicle, 45, Tank);
            ApplyMod(vehicle, 46, Windows);
            ApplyMod(vehicle, 48, Livery);

            API.SetVehicleLivery(vehicle, Livery - 1);
        }

        private void ApplyMod(int vehicle, int mod, int index)
        {
            API.SetVehicleMod(vehicle, mod, index - 1, false);
        }
    }
}