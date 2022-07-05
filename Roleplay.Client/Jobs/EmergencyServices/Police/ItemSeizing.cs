using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Enums;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
    public class ItemSeizing : JobClass
    {
        public ItemSeizing()
        {
            CommandRegister.RegisterCommand("seizevehitems|seizevehicleitems", OnSeizeVehItems);
        }

        private void OnSeizeVehItems(Command cmd)
        {
            if (JobHandler.OnDutyAsJob(JobType.Police))
            {
                var closeVeh = GTAHelpers.GetClosestVehicle(3.0f, o => o.HasDecor("Vehicle.ID"));

                if (closeVeh != null)
                {
                    Client.TriggerServerEvent("Items.SeizeVehicleItems", closeVeh.GetDecor<int>("Vehicle.ID"), string.Join(" ", cmd.Args));
                }
            }
        }
    }
}
