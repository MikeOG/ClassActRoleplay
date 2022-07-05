using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Jobs.EmergencyServices
{
    public class SharedEmergencyItems : JobClass
    {
        public SharedEmergencyItems()
        {
            Client.RegisterEventHandler("Job.FixVehicle", new Action(OnFixVehicle));
            Client.RegisterEventHandler("Job.RepairVehicle", new Action(OnReairVehicle));
            Client.RegisterEventHandler("Job.SetVehicleExtra", new Action<string, string>(OnSetVehicleExtra));
            Client.RegisterEventHandler("Player.OnDutyStatusChange", new Action<bool>(OnDutyChange));
        }

        public void OnDutyChange(bool state)
        {
            if (state && JobHandler.GetPlayerJob() == JobType.Police)
            {
                Game.PlayerPed.Armor = 100;
            }
        }

        [EventHandler("Job.DeleteVehicle")]
        private void OnDeleteVehicle(bool impound)
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();

            if (closeVeh != null)
            {
                if (closeVeh.HasDecor("Vehicle.ID") /*&& JobHandler.OnDutyAsJob(JobType.Police)*/)
                {
                    var vehId = closeVeh.GetDecor<int>("Vehicle.ID");
                    if (vehId < 1000000 && JobHandler.OnDutyAsJob(JobType.Police))
                    {
                        if(impound)
                        {
                            Client.TriggerServerEvent("Vehicle.ImpoundVehicle", vehId);
                        }
                        closeVeh.Delete();
                    }
                    else if (vehId > 1000000)
                    {
                        closeVeh.Delete();
                    }
                }
                else
                {
                    closeVeh.Delete();
                }
            }
        }

        private void OnFixVehicle()
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();

            if (closeVeh != null)
            {
                closeVeh.Repair();

                Log.ToChat("[Job]", "Fixed vehicle", ConstantColours.Job);
            }
        }

        private async void OnReairVehicle()
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();

            if (closeVeh != null)
            {
                Log.ToChat("[Job]", "Repairing vehicle", ConstantColours.Inventory);
                await EmoteManager.playerAnimations["mechanic"].PlayFullAnim();

                await BaseScript.Delay(new Random((int)DateTime.Now.Ticks).Next(3500, 7500));

                if (closeVeh.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(3, 2))
                {
                    if (closeVeh.EngineHealth < 150)
                        closeVeh.EngineHealth = 150;

                    if (closeVeh.BodyHealth < 150)
                        closeVeh.BodyHealth = 150;

                    SetVehicleTyreFixed(closeVeh.Handle, 0);
                    SetVehicleTyreFixed(closeVeh.Handle, 1);
                    SetVehicleTyreFixed(closeVeh.Handle, 2);
                    SetVehicleTyreFixed(closeVeh.Handle, 3);
                    SetVehicleTyreFixed(closeVeh.Handle, 4);
                    SetVehicleTyreFixed(closeVeh.Handle, 5);

                    Log.ToChat("[Job]", "Vehicle repaired", ConstantColours.Inventory);
                }
                else
                {
                    Log.ToChat("[Job]", "You moved to far away from the vehicle", ConstantColours.Inventory);
                }

                EmoteManager.playerAnimations["mechanic"].End(Game.PlayerPed);
            }
        }

        private void OnSetVehicleExtra(string extra, string state)
        {
            var playerVeh = Game.PlayerPed.CurrentVehicle;

            if (playerVeh != null && playerVeh.ClassType == VehicleClass.Emergency)
            {
                var enableExtra = state == "true";

                if (extra == "all")
                {
                    for (var i = 0; i < 50; i++)
                    {
                        playerVeh.ToggleExtra(i, enableExtra);
                    }
                }
                else
                {
                    playerVeh.ToggleExtra(Convert.ToInt32(extra), enableExtra);
                }
            }
        }
    }
}
