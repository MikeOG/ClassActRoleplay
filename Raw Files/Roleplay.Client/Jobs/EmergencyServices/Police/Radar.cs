using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Loader;
using Font = CitizenFX.Core.UI.Font;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
    public class Radar : JobClass
    {
        private bool radarEnabled = false;
        private bool radarFrozen = false;
        private readonly ScreenText frontVehText = new ScreenText("", 828, 963, 0.4f, null, Color.FromArgb(255, 255, 255), Font.ChaletLondon, Alignment.Center);
        private readonly ScreenText backVehText = new ScreenText("", 828, 1006, 0.4f, null, Color.FromArgb(255, 255, 255), Font.ChaletLondon, Alignment.Center);
        public Radar()
        {
            Client.RegisterEventHandler("Police.ToggleRadar", new Action(ToggleRadar));
            CommandRegister.RegisterCommand("radar", cmd =>
            {
                ToggleRadar();
            });
        }

        public void ToggleRadar()
        {
            radarEnabled = !radarEnabled;
            Log.ToChat("[Police]", $"Radar {(radarEnabled ? "enabled" : "disabled")}", ConstantColours.Police);
        }
        private Vehicle GetRadarVehicle(float distance)
        {
            var playerVeh = Game.PlayerPed.CurrentVehicle;
            var baseOffset = GetOffsetFromEntityInWorldCoords(playerVeh.Handle, 0.0f, 1.0f, 1.0f);
            var captureOffset = GetOffsetFromEntityInWorldCoords(playerVeh.Handle, 0.0f, distance, 0.0f);
            var data = World.RaycastCapsule(baseOffset, captureOffset, 3.0f, (IntersectOptions)10, playerVeh);
            return data.DitHitEntity ? data.HitEntity as Vehicle : null;
        }
        [DynamicTick(TickUsage.InVehicle)]
        private async Task RadarTick()
        {
            if (!JobHandler.OnDutyAsJob(JobType.Police) || Cache.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Emergency) return;
            var frontVeh = GetRadarVehicle(1000f);
            var backVeh = GetRadarVehicle(-1000f);
            if (radarEnabled)
            {
                if (frontVeh != null && !radarFrozen)
                {
                    frontVehText.Caption = $"F: {frontVeh.LocalizedName} | {frontVeh.Mods.LicensePlate} | {Math.Round(frontVeh.Speed * 2.24f)} MPH";
                }
                if (backVeh != null && !radarFrozen)
                {
                    backVehText.Caption = $"B: {backVeh.LocalizedName} | {backVeh.Mods.LicensePlate} | {Math.Round(backVeh.Speed * 2.24f)} MPH";
                }
                if (Input.IsControlJustPressed(Control.FrontendRdown))
                {
                    radarFrozen = !radarFrozen;
                    frontVehText.Color = radarFrozen ? ConstantColours.Red : Color.FromArgb(255, 255, 255);
                    backVehText.Color = radarFrozen ? ConstantColours.Red : Color.FromArgb(255, 255, 255);
                }
                frontVehText.DrawTick();
                backVehText.DrawTick();
            }
            if (Input.IsControlJustPressed(Control.Detonate))
            {
                if (frontVeh != null)
                {
                    ExecuteCommand($"runplate {frontVeh.Mods.LicensePlate}");
                }
            }
        }
    }
}