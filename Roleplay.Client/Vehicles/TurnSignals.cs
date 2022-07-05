using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    /*public class TurnSignals : ClientAccessor
    {
        public TurnSignals(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.RegisterTickHandler(OnTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(OnTick);
            }));
            client.RegisterEventHandler("Vehicle.SetIndicatorStatus", new Action<int, string, bool>(OnRemoteIndicatorSet));
        }

        private void startSignalDelay(Vehicle veh)
        {
            Task.Factory.StartNew(async () =>
            {
                while (veh.Speed <= 0.3)
                    await BaseScript.Delay(0);

                await BaseScript.Delay(3000);

                if (veh.Speed >= 0.3 || veh.IsDead || veh.Driver != Game.PlayerPed)
                {
                    /*veh.IsLeftIndicatorLightOn = false;
                    veh.IsRightIndicatorLightOn = false;

                    toggleIndicatorState(veh, "all", false);
                }
                else
                    startSignalDelay(veh);
            });
        }

        private void toggleIndicatorState(Vehicle veh, string side, bool state, CitizenFX.Core.Player sourcePlayer = null)
        {
            var indicatorState = state;
            if (side == "left")
            {
                //indicatorState = !veh.IsLeftIndicatorLightOn;
                veh.IsLeftIndicatorLightOn = indicatorState;

                if (veh.IsRightIndicatorLightOn)
                {
                    veh.IsRightIndicatorLightOn = false;
                    if(sourcePlayer == null)
                        Client.TriggerServerEvent("Vehicles.SetIndicatorStatus", "right", false);
                }
            }
            else if (side == "right")
            {
                //indicatorState = !veh.IsRightIndicatorLightOn;
                veh.IsRightIndicatorLightOn = indicatorState;

                if(veh.IsLeftIndicatorLightOn)
                {
                    veh.IsLeftIndicatorLightOn = false;
                    if(sourcePlayer == null)
                        Client.TriggerServerEvent("Vehicles.SetIndicatorStatus", "left", false);
                }
            }
            else if (side == "all")
            {
                var currentState = veh.IsRightIndicatorLightOn || veh.IsLeftIndicatorLightOn;

                //indicatorState = !currentState;
                veh.IsRightIndicatorLightOn = indicatorState;
                veh.IsLeftIndicatorLightOn = indicatorState;
            }

            if(indicatorState && side != "all")
                startSignalDelay(veh);
            
            if (sourcePlayer == null)
                Client.TriggerServerEvent("Vehicles.SetIndicatorStatus", side, indicatorState);
        }

        private void OnRemoteIndicatorSet(int player, string side, bool state)
        {
            var targetPlayer = Client.PlayerList.FirstOrDefault(o => o.ServerId == player);

            Log.Debug($"Recieved a remote indicator request from {targetPlayer?.Name} for side {side} with a state of {state}");
            if (targetPlayer != null && targetPlayer != Game.Player)
            {
                if(targetPlayer.Character.IsInVehicle())
                    toggleIndicatorState(targetPlayer.Character.CurrentVehicle, side, state, targetPlayer);
            }
        }

        private async Task OnTick()
        {
            var playerVeh = Game.PlayerPed.CurrentVehicle;

            if (Input.IsControlJustPressed(Control.PhoneLeft)) // left
            {
                toggleIndicatorState(playerVeh, "left", !playerVeh.IsLeftIndicatorLightOn);
            }

            if (Input.IsControlJustPressed(Control.PhoneRight)) // right
            {
                toggleIndicatorState(playerVeh, "right", !playerVeh.IsRightIndicatorLightOn);
            }
        }
    }*/
}
