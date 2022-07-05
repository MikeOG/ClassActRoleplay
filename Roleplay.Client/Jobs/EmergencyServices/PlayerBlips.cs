using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Enums;

namespace Roleplay.Client.Jobs.EmergencyServices
{
    public class PlayerBlips : JobClass
    {
        private Dictionary<JobType, BlipColor> jobColours = new Dictionary<JobType, BlipColor>
        {
            {JobType.EMS, BlipColor.Red},
            {JobType.Police, BlipColor.Blue}
        };
        private Dictionary<int, Blip> CurrentDutyBlips = new Dictionary<int, Blip>();

        public PlayerBlips()
        {

        }

        [EventHandler("Player.OnDutyStatusChange")]
        private void OnDutyStateChange(bool state)
        {
            if (jobColours.ContainsKey(JobHandler.GetPlayerJob()))
            {
                if (state)
                {
                    Log.Verbose($"Gone on duty adding duty blips tick");
                    Client.RegisterTickHandler(BlipHandlerTick);
                }
                else
                {
                    Log.Verbose($"Gone off duty removing duty blips tick");
                    Client.DeregisterTickHandler(BlipHandlerTick);
                    foreach (var kvp in CurrentDutyBlips)
                    {
                        kvp.Value.Delete();
                    }
                    CurrentDutyBlips.Clear();
                }
            }
        }

        private async Task BlipHandlerTick()
        {
            foreach (var player in Sessions.PlayerList)
            {
                if(player == Client.LocalSession) continue;
                
                var playerJob = JobHandler.GetPlayerJob(player);

                if(!jobColours.TryGetValue(playerJob, out var blipColour)) continue;

                var playerPed = player.Player.Character;
                var hasBlip = CurrentDutyBlips.TryGetValue(player.ServerID, out var blip);
                var settings = player.GetCharacterSettings();
                var callsign = "Unknown callsign";

                if (settings.ContainsKey("Callsign"))
                {
                    callsign = settings["Callsign"];
                }

                var inVehicle = playerPed.IsInVehicle();
                if (hasBlip && !inVehicle)
                {
                    Log.Verbose($"Removing duty blip for {player.Player.Name}");
                    blip.Delete();
                    CurrentDutyBlips.Remove(player.ServerID);
                    continue;
                }

                if (!hasBlip && inVehicle && playerPed.CurrentVehicle.ClassType == VehicleClass.Emergency && playerPed.CurrentVehicle.ClassType == VehicleClass.Helicopters)
                {
                    Log.Verbose($"Creating duty blip for {player.Player.Name}");
                    var newBlip = playerPed.CurrentVehicle.AttachBlip();
                    newBlip.Name = $"{callsign}";
                    newBlip.Color = blipColour;
                    CurrentDutyBlips[player.ServerID] = newBlip;
                }
            }
        }
    }
}
