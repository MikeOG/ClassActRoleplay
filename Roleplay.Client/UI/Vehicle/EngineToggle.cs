using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared;
using MenuFramework;

namespace Roleplay.Client.UI.Vehicle
{
    internal class EngineToggle : ClientAccessor
    {
        public static bool DisableEngine = false;

        private MenuItemCheckbox engineItem = new MenuItemCheckbox { Title = "Turn On Engine", state = true, OnActivate = (state, item) => { BaseScript.TriggerEvent("Vehicle.SetEngineState", state); } };

        public EngineToggle(Client client) : base(client)
        {
            client.Get<InteractionUI>().RegisterInteractionMenuItem(engineItem, () => Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.Driver == Cache.PlayerPed, 500);
            client.RegisterEventHandler("Vehicle.SetEngineState", new Action<bool>(setEngineState));
            client.RegisterTickHandler(OnTick);
            CommandRegister.RegisterCommand("engine", handleEngineCommand);
        }

        private async Task OnTick()
        {
            if(InteractionUI.Observer.CurrentMenu != null)
            {
                if(Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.Driver == Cache.PlayerPed)
                {
                    engineItem.state = Cache.PlayerPed.CurrentVehicle.IsEngineRunning;
                }
            }
        }

        private async Task DisableEngineTick()
        {
            if (Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver) == Cache.PlayerPed && DisableEngine)
            {
                Cache.PlayerPed.CurrentVehicle.IsEngineRunning = false;
            }
            else
            {
                Client.DeregisterTickHandler(DisableEngineTick);
            }
        }

        private void setEngineState(bool engineState)
        {
            var playerVeh = Cache.PlayerPed.CurrentVehicle;
            DisableEngine = !engineState;
            SetVehicleEngineOn(playerVeh.Handle, engineState, false, true);
            if (DisableEngine)
            {
                Client.RegisterTickHandler(DisableEngineTick);
            }
        }

        private void handleEngineCommand(Command cmd)
        {
            if (Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver) == Cache.PlayerPed)
            {
                var toggleState = cmd.GetArgAs(0, "off");
                if (cmd.Args.Count == 0)
                {
                    BaseScript.TriggerEvent("Vehicle.SetEngineState", !Cache.PlayerPed.CurrentVehicle.IsEngineRunning);
                }
                else if (toggleState == "on")
                {
                    BaseScript.TriggerEvent("Vehicle.SetEngineState", true);
                }
                else if (toggleState == "off")
                {
                    BaseScript.TriggerEvent("Vehicle.SetEngineState", false);
                }
            }
            else
            {
                Log.ToChat("You need to be driving a vehicle first.");
            }
        }
    }
}
