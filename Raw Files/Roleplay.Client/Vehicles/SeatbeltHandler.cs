using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    internal class SeatbeltHandler : ClientAccessor
    {
        private bool hasSeatbeltOn = false;

        public SeatbeltHandler(Client client) : base(client)
        {
            EntityDecoration.RegisterProperty("seatbeltEject", DecorationType.Bool);
            EntityDecoration.RegisterProperty("ejectSpeed", DecorationType.Float);
            client.RegisterEventHandler("baseevents:enteringVehicle", new Action<int, int, string>((veh, seat, name) => {
                new CitizenFX.Core.Vehicle(veh).SetDecor("seatbeltEject", false);
            }));
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.RegisterTickHandler(OnTick);
                client.RegisterTickHandler(CrashCheckTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                hasSeatbeltOn = false;
                client.DeregisterTickHandler(OnTick);
                client.DeregisterTickHandler(CrashCheckTick);
            }));
        }

        private async Task OnTick()
        {
            var inVehicle = Cache.PlayerPed.IsInVehicle();

            if (inVehicle)
            {
                var playerVeh = Cache.PlayerPed.CurrentVehicle;

                if (playerVeh.ClassType == VehicleClass.Motorcycles) return;

                if (Game.IsControlPressed(1, Control.SpecialAbilitySecondary) /*&& (playerVeh.Speed < 8 || hasSeatbeltOn)*/)
                {
                    hasSeatbeltOn = !hasSeatbeltOn;
                    if (hasSeatbeltOn)
                    {
                        hasSeatbeltOn = false;
                        Log.ToChat("", "Putting on seatbelt");
                        await BaseScript.Delay(Convert.ToInt32(125 * (playerVeh.Speed + 2)));
                        hasSeatbeltOn = true;
                        TriggerServerEvent("InteractSound_SV:PlayOnSource", "seatbelt", 0.03);
                    }
                    Log.ToChat("", hasSeatbeltOn ? "Seatbelt on" : "Seatbelt off");
                    await BaseScript.Delay(150);
                }

                if (hasSeatbeltOn)
                {
                    Game.DisableControlThisFrame(1, Control.VehicleExit);
                    if (Game.IsDisabledControlJustPressed(1, Control.VehicleExit))
                        //TriggerEvent("sendNotifyEvent", "Your seatbelt is preventing you from leaving this vehicle.");
                        Log.ToChat("", "Your seatbelt is preventing you from leaving this vehicle.");

                    return;
                }

                if (playerVeh.HasDecor("seatbeltEject") && playerVeh.GetDecor<bool>("seatbeltEject"))
                {
                    doVehEject(playerVeh.GetDecor<float>("ejectSpeed"));
                }
            }
        }

        private async Task CrashCheckTick()
        {   
            bool inVeh = Cache.PlayerPed.IsInVehicle();
            if (inVeh)
            {
                var playerVeh = Cache.PlayerPed.CurrentVehicle;
                if (playerVeh.ClassType != VehicleClass.Motorcycles && playerVeh.Driver == Game.PlayerPed && !hasSeatbeltOn)
                {
                    if (!playerVeh.HasDecor("seatbeltEject"))
                        playerVeh.SetDecor("seatbeltEject", false);
                    float currentVehSpeed = playerVeh.Speed;
                    await BaseScript.Delay(10);
                    float newVehSpeed = playerVeh.Speed;
                    float ejectPercent = 0.78f;
                    if (currentVehSpeed > 20 && newVehSpeed <= (currentVehSpeed * ejectPercent))
                    {
                        playerVeh.SetDecor("seatbeltEject", true);
                        playerVeh.SetDecor("ejectSpeed", currentVehSpeed);
                        await BaseScript.Delay(1500);
                        playerVeh.SetDecor("seatbeltEject", false);
                        playerVeh.SetDecor("ejectSpeed", 0.0f);
                    }
                }
            }
            else
            {
                hasSeatbeltOn = false;
            }
        }

        private async void doVehEject(float currentVehSpeed)
        {
            CitizenFX.Core.Vehicle playerVeh = Cache.PlayerPed.CurrentVehicle;
            playerVeh.IsPositionFrozen = true;
            playerVeh.Speed = 0.0f;
            playerVeh.IsPositionFrozen = false;
            Cache.PlayerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            while(Cache.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                await BaseScript.Delay(0);
            }
            Vector3 direction = Vector3.Multiply(Cache.PlayerPed.ForwardVector, currentVehSpeed);
            Cache.PlayerPed.ApplyForce(direction);
            await BaseScript.Delay(25);
            Cache.PlayerPed.ApplyDamage((int)(currentVehSpeed / 1.5f));
            Cache.PlayerPed.Ragdoll((int)(currentVehSpeed * 125) + 500);
        }
    }
}
