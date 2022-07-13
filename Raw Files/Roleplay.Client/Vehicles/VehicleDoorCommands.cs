using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    public class VehicleDoorCommands : ClientAccessor
    {
        public VehicleDoorCommands(Client client) : base(client)
        {
            Enum.GetValues(typeof(VehicleDoorIndex)).Cast<VehicleDoorIndex>().ToList().ForEach(o =>
            {
                CommandRegister.RegisterCommand(o.ToString().Replace("Door", "").Replace("Back", "Rear").ToLower(), cmd => ToggleVehicleDoor(o));
            });
        }

        private void ToggleVehicleDoor(VehicleDoorIndex vehDoorIndex)
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();

            if (closeVeh?.LockStatus != VehicleLockStatus.Unlocked)
                closeVeh = Client.Get<VehicleHandler>().GetClosestVehicleWithKeys(6.0f);

            if (closeVeh != null && closeVeh.HasDecor("Vehicle.ID"))
            {
                var vehDoor = closeVeh.Doors[vehDoorIndex];

                if(vehDoor.IsOpen)
                    vehDoor.Close();
                else
                    vehDoor.Open();
            }
        }
    }
}
