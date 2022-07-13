using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Server.Vehicle;
using Roleplay.Server.HTTP;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class PlateChecker : JobClass
    {
        private Dictionary<string, string> vehiclePlateOwners = new Dictionary<string, string>();

        public PlateChecker()
        {
            CommandRegister.RegisterJobCommand("runplate", OnRunPlate, JobType.Police);
        }

        public async void CreateRandomName(string plate, Action<string> cb)
        {
            // TODO pick from different regions
            if(!vehiclePlateOwners.ContainsKey(plate))
            {
                try
                {
                    var webRequest = new HTTPRequest();
                    var response = await webRequest.Request("https://randomuser.me/api/?results=1", "GET", $"");
                    dynamic nameJSON = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(response.content);

                    vehiclePlateOwners[plate] = $"{nameJSON.results.name.first} {nameJSON.results.name.last}";
                    cb($"{nameJSON.name} {nameJSON.surname}");
                }
                catch{ }
            }
            else
            {
                cb(vehiclePlateOwners[plate]);
            }
        }

        private void OnRunPlate(Command cmd)
        {
            var plate = cmd.GetArgAs(0, "");

            var vehManager = Server.Get<VehicleManager>();
            var veh = vehManager.GetVehicleByPlate(plate);

            if (veh != null && (veh.VehID < 1000000 || veh.VehID >= 1000000 && veh.RentedVehicle))
            {
                var vehicleOwner = veh.VehicleOwner;

                Log.ToClient("[Radar]", $"The plate {plate.ToUpper()} is registered to {vehicleOwner.GetCharacterName()}; DOB - {vehicleOwner.GetGlobalData("Character.DOB", "")}", ConstantColours.Dispatch, cmd.Player);
            }
            else
            {
                CreateRandomName(plate.ToUpper(), name =>
                {
                    Log.ToClient("[Radar]", $"The plate {plate.ToUpper()} is registered to {name}", ConstantColours.Dispatch, cmd.Player);
                });
            }        
        }
    }
}
