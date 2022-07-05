using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Roleplay.Client.Vehicles
{
    internal class NoAirControl : ClientAccessor
    {
        public NoAirControl(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.RegisterTickHandler(OnTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(OnTick);
            }));
        }

        //private async Task OnTick()
        private static async Task OnTick()
        {
            var inVeh = Cache.PlayerPed.IsInVehicle();
            if (inVeh)
            {
                var playerVeh = Cache.PlayerPed.CurrentVehicle;
                if (Game.PlayerPed.IsInVehicle() && (Game.PlayerPed.CurrentVehicle.Model.IsCar || Game.PlayerPed.CurrentVehicle.Model.IsBike) && (Game.PlayerPed.CurrentVehicle.IsInAir || Game.PlayerPed.CurrentVehicle.IsUpsideDown))
                {
                    Game.DisableControlThisFrame(1, Control.VehicleMoveLeftRight);
                    Game.DisableControlThisFrame(1, Control.VehicleMoveUpDown);
                }
            }
        }
    }
}
