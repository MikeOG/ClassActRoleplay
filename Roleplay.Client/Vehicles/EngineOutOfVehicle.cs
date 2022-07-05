using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.UI.Vehicle;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    public class EngineOutOfVehicle : ClientAccessor
    {
        private VehicleList vehicleList = new VehicleList();
        private List<Vehicle> cachedVehicles = new List<Vehicle>();
        private List<int> cachedKeys = new List<int>();
        private VehicleHandler vehHandler;

        public EngineOutOfVehicle(Client client) : base(client)
        {
            vehHandler = Client.Get<VehicleHandler>();
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(RunEngineTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.RegisterTickHandler(RunEngineTick);
            }));
            client.RegisterTickHandler(RunEngineTick);
        }

        private void UpdateVehicles()
        {
            cachedVehicles.Clear();

            if (cachedKeys.Count == 0) return;

            cachedVehicles = vehicleList.Select(o => new Vehicle(o)).Where(o => o.HasDecor("Vehicle.ID") && cachedKeys.Contains(o.GetDecor<int>("Vehicle.ID"))).ToList();
        }

        private async Task RunEngineTick()
        {
            if(LocalSession == null) return;

            var currentKeys = vehHandler.GetVehiclesWithKeys();

            if (currentKeys != null && !currentKeys.All(cachedKeys.Contains))
            {
                cachedKeys = currentKeys;
                UpdateVehicles();
            }

            var disableEngine = EngineToggle.DisableEngine;
            foreach (var veh in cachedVehicles)
            {
                if (veh.EngineHealth > 0 && !disableEngine) // keep engine on
                {
                    CitizenFX.Core.Native.API.SetVehicleEngineOn(veh.Handle, true, true, true);
                }
            }
        }
    }
}
