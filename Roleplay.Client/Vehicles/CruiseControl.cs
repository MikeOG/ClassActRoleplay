using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.UI.Vehicle;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Vehicles
{
    public class CruiseControl : ClientAccessor
    {
        private bool inCruiseControl;
        private float vehicleSpeed;

        public CruiseControl(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                SetCruiseStatus(false);
                client.RegisterTickHandler(CruiseStartTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                SetCruiseStatus(false);
                client.DeregisterTickHandler(CruiseStartTick);
            }));
        }

        public void SetCruiseStatus(bool status, float vehSpeed = 0.0f)
        {
            inCruiseControl = status;

            var indicator = Client.Get<VehicleDisplay>().SpeedIndicator;
            var mph = Client.Get<VehicleDisplay>().MPHText;

            if (inCruiseControl)
            {
                indicator.Color = ConstantColours.Green;
                mph.Color = ConstantColours.Green;
                Client.RegisterTickHandler(CruiseTick);
            }
            else
            {
                indicator.Color = Color.FromArgb(255, 255, 255);
                mph.Color = Color.FromArgb(255, 255, 255);
                Client.DeregisterTickHandler(CruiseTick);
            }

            vehicleSpeed = vehSpeed;
        }

        private bool canActivateCruiseControl(Vehicle playerVeh)
        {
            return playerVeh.Speed > 0 && playerVeh.Speed < 50 && playerVeh.IsOnAllWheels && playerVeh.IsEngineRunning && CitizenFX.Core.Native.API.GetEntitySpeedVector(playerVeh.Handle, true).Y > 0;
        }

        private async Task CruiseStartTick()
        {
            if (Input.IsControlJustPressed(Control.MpTextChatTeam))
            {
                var playerVeh = Cache.PlayerPed.CurrentVehicle;

                if (playerVeh.Driver == Cache.PlayerPed && (playerVeh.Model.IsCar || playerVeh.Model.IsBike))
                {
                    var currentVehSpeed = playerVeh.Speed;
                    if (canActivateCruiseControl(playerVeh))
                    {
                        SetCruiseStatus(!inCruiseControl, currentVehSpeed);
                    }
                }
            }
        }

        private async Task CruiseTick()
        {
            if (inCruiseControl)
            {
                var playerVeh = Cache.PlayerPed.CurrentVehicle;

                if (Input.IsControlPressed(Control.MoveUpOnly))
                {
                    var canUseCruise = canActivateCruiseControl(playerVeh);

                    while (Input.IsControlPressed(Control.MoveUpOnly) && canUseCruise)
                    {
                        await BaseScript.Delay(0);
                        canUseCruise = canActivateCruiseControl(playerVeh);
                    }

                    SetCruiseStatus(canUseCruise, playerVeh.Speed);
                    return;
                }

                if (Input.IsControlPressed(Control.MoveDownOnly) || Input.IsControlPressed(Control.VehicleHandbrake))
                {
                    SetCruiseStatus(false);
                    return;
                }

                if (canActivateCruiseControl(playerVeh))
                {
                    playerVeh.Speed = vehicleSpeed;
                }
                else
                {
                    SetCruiseStatus(false);
                }
            }
            await BaseScript.Delay(250);
        }
    }
}
