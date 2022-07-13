using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Player.Controls;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Vehicles
{
    public class StopSeatShuffle : ClientAccessor
    {
        public StopSeatShuffle(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>(async (veh, seat, name) =>
            {
                await BaseScript.Delay(5000);
                client.RegisterTickHandler(StopShuffleTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(StopShuffleTick);
            }));
        }

        private async Task StopShuffleTick()
        {
            if (Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.GetPedOnSeat(VehicleSeat.RightFront) == Cache.PlayerPed)
            {
                if (Input.IsControlJustPressed(Control.ThrowGrenade))
                {
                    await BaseScript.Delay(5000);
                }

                if (GetIsTaskActive(Cache.PlayerPed.Handle, 165))
                {
                    Cache.PlayerPed.SetIntoVehicle(Cache.PlayerPed.CurrentVehicle, VehicleSeat.RightFront);
                }
            }
        }
    }
}
