using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Loader;
using Font = CitizenFX.Core.UI.Font;
using CitizenFX.Core.Native;

namespace Roleplay.Client.UI.Vehicle
{
    public class VehicleDisplay : ClientAccessor
    {
        private float LeftBarOnePercent;
        private float RightBarOnePercent;

        private float CurrentFuelLevel = 80.0f;

        public ScreenText SpeedIndicator;
        public ScreenText MPHText;

        private ScreenText FuelIndicator;
        private ScreenText FuelText;

        private ScreenText CompassDirection;
        private ScreenText StreetDisplay;
        public VehicleDisplay(Client client) : base(client)
        {
            SpeedIndicator = new ScreenText("0", 400, 956, 0.64f, async () =>
            {
                SpeedIndicator.Caption = $"{Math.Floor(Cache.PlayerPed.CurrentVehicle.Speed * 2.24f)}";
            }, Color.FromArgb(255, 255, 255), Font.ChaletComprimeCologne, Alignment.Left);
            MPHText = new ScreenText("mph", 450, 970, 0.5f, async () => { }, Color.FromArgb(255, 255, 255), Font.ChaletComprimeCologne, Alignment.Left);

            FuelIndicator = new ScreenText("0", 300, 956, 0.64f, null, Color.FromArgb(255, 255, 255), Font.ChaletComprimeCologne, Alignment.Left);
            FuelText = new ScreenText("fuel", 350, 970, 0.5f, async () => { }, Color.FromArgb(255, 255, 255), Font.ChaletComprimeCologne, Alignment.Left);

            client.RegisterTickHandler(RadarTick);

            CompassDirection = new ScreenText("S", 300, 1024, 0.29f, async () =>
            {
                CompassDirection.Caption = GetCardinalDirection();
            }, Color.FromArgb(255, 255, 255), Font.ChaletLondon, Alignment.Left, true, true);

            StreetDisplay = new ScreenText($"~b~{GetCrossingName(Cache.PlayerPed.Position)[0]}~w~ in ~y~{World.GetZoneLocalizedName(Cache.PlayerPed.Position)}", 320, 1024, 0.29f, async () =>
            {
                StreetDisplay.Caption = $"~b~{GetCrossingName(Cache.PlayerPed.Position)[0]}~w~ in ~y~{World.GetZoneLocalizedName(Cache.PlayerPed.Position)}";
            }, Color.FromArgb(255, 255, 255), Font.ChaletLondon, Alignment.Left, true, true);
        }

        public List<string> GetCrossingName(Vector3 position)
        {
            uint streetName = 0;
            uint crossingRoad = 0;
            GetStreetNameAtCoord(position.X, position.Y, position.Z, ref streetName, ref crossingRoad);

            return new List<string>
            {
                GetStreetNameFromHashKey(streetName),
                GetStreetNameFromHashKey(crossingRoad)
            };
        }

        public string GetCardinalDirection()
        {
            float h = Game.PlayerPed.Heading;
            if (h >= 315f || h < 45f) return "N";
            else if (h >= 45f && h < 135f) return "W";
            else if (h >= 135f && h < 225f) return "S";
            else if (h >= 225f && h < 315f) return "E";
            else return "N";
        }

        [DynamicTick(TickUsage.InVehicle)]
        private async Task VehicleTick()
        {
            if (CinematicMode.InCinematicMode) return;

            var playerVeh = Cache.PlayerPed.CurrentVehicle;

            if (playerVeh == null) return;

            if (playerVeh.Driver == Cache.PlayerPed)
            {
                if (playerVeh.ClassType == VehicleClass.Cycles) return;

                var fuelLevel = playerVeh.HasDecor("Vehicle.Fuel") ? Math.Round(playerVeh.GetDecor<float>("Vehicle.Fuel")).ToString() : "100";

                FuelIndicator.Caption = fuelLevel;
            }

            SpeedIndicator.DrawTick();
            MPHText.DrawTick();

            FuelIndicator.DrawTick();
            FuelText.DrawTick();

            CompassDirection.DrawTick();
            StreetDisplay.DrawTick();
        }

        private async Task RadarTick()
        {
            if (Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Cycles && !CinematicMode.InCinematicMode)
            {
                DisplayRadar(true);
                SetRadarBigmapEnabled(false, true);
            }
            else
            {
                DisplayRadar(false);
                SetRadarBigmapEnabled(true, true);

                DisplayRadar(false);
                SetRadarBigmapEnabled(false, true);
            }
        }
    }
}