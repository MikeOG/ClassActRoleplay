using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Jobs;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Vehicle
{
    public class VehicleGarageHandler : ServerAccessor
    {
        public List<GarageModel> GlobalGarages = new List<GarageModel>
        {
            new GarageModel
            {
                Name = "Public1",
                Location = new Vector3(215.246f, -791.41f, 29.8699f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public2",
                Location = new Vector3(1881.36f, 3757.43f, 31.9999f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public3",
                Location = new Vector3(139.74f, 6631.92f, 31.0497f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public4",
                Location = new Vector3(274.11f, -330.86f, 43.92f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public5",
                Location = new Vector3(-335.24f, -777.28f, 32.97f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public6",
                Location = new Vector3(25.79f, -1714.82f, 28.3f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public7",
                Location = new Vector3(1226.36f, -434.72f, 67.62f),
                AlternateDisplayName = "The Blackout Nightclub"
            },
            new GarageModel
            {
                Name = "Public8",
                Location = new Vector3(321.85f, -546.64f, 28.74f),
                AlternateDisplayName = "EMS Parking Garage"
            },
            new GarageModel
            {
                Name = "Public9",
                Location = new Vector3(469.5f, -1091.71f, 29.2f),
                AlternateDisplayName = "Police Parking Garage"
            },
            new GarageModel
            {
                Name = "Pink Cage",
                Location = new Vector3(325.42f, -208.36f, 54.09f),
                AlternateDisplayName = "Pink Cage Garage"
            },
            new GarageModel
            {
                Name = "Eastern Motel",
                Location = new Vector3(339.4f, 2629.11f, 44.5f),
                AlternateDisplayName = "Eastern Motel Garage"
            },
            new GarageModel
            {
                Name = "Bahama Mamas",
                Location = new Vector3(-1432.78f, -582.35f, 30.59f),
                AlternateDisplayName = "Bahama Mamas Garage"
            },
            new GarageModel
            {
                Name = "Public10",
                Location = new Vector3(1346.88f, 4369.96f, 44.34f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public11",
                Location = new Vector3(-1578.37f, 5159.8f, 19.82f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public12",
                Location = new Vector3(3801.62f, 4460.92f, 4.81f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Public13",
                Location = new Vector3(-691.04f, -1418.78f, 5f),
                AlternateDisplayName = "Public garage"
            },
            new GarageModel
            {
                Name = "Boat Parking",
                Location = new Vector3(-805.32f, -1506.28f, -0.47f),
                AlternateDisplayName = "Boat Parking Garage"
            },
            new GarageModel
            {
                Name = "Boat Parking2",
                Location = new Vector3(3864.07f, 4467.51f, -0.47f),
                AlternateDisplayName = "Boat Parking Garage 2"
            },
            new GarageModel
            {
                Name = "Boat Parking3",
                Location = new Vector3(-1602.74f, 5258.77f, -0.47f),
                AlternateDisplayName = "Boat Parking Garage 3"
            },
            new GarageModel
            {
                Name = "Boat Parking4",
                Location = new Vector3(1335.7f, 4265.46f, 29.94f),
                AlternateDisplayName = "Boat Parking Garage 4"
            },
            new GarageModel
            {
                Name = "Sandy Air Parking",
                Location = new Vector3(1738.7f, 3284.76f, 41.10f),
                AlternateDisplayName = "Sandy Air Garage"
            },
            new GarageModel
            {
                Name = "Impound",
                Location = new Vector3(393.27f, -1617.66f, 29.26f),
                BlipOptions = new BlipOptions
                {
                    Sprite = 50,
                    Colour = 1
                }
            }
        };

        private VehicleManager vehManager;
        public VehicleManager VehManager => vehManager ?? (vehManager = Server.Get<VehicleManager>());

        public VehicleGarageHandler(Server server) : base(server)
        {

        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            playerSession.AddGarages(GlobalGarages);
        }

        [EventHandler("Vehicle.Garage.AttemptStoreVehicle")]
        private async void OnAttemptStoreVehicle([FromSource] Player source, int vehicleId, string vehicleMods/*, string garageName*/)
        {
            Log.Verbose($"Attempting to store vehicle #{vehicleId} for {source.Name}");
            try
            {
                var playerSession = Sessions.GetPlayer(source);

                if (playerSession == null) return;
                Log.Debug($"playerSession is not null");

                var garage = playerSession.GetClosestGarage();

                if (garage == null) return;
                Log.Debug($"garage is not null");

                Log.Verbose($"{source.Name} is next to garage {garage.Name} running storage checks");

                if (garage.MaxVehicles != -1) // do garage size check for this garage
                {
                    bool? canStoreVehicle = null;

                    MySQL.execute("SELECT Count(*) AS NumVehs FROM vehicle_data WHERE Garage = @garage AND CharID = @char AND VehID != @curveh", new Dictionary<string, dynamic> { { "@char", playerSession.CharId }, { "@curveh", vehicleId }, { "@garage", garage.Name } },
                        new Action<List<dynamic>>(count =>
                        {
                            Log.Debug($"Count of garage vehicles for location {garage.Name} is {count[0].NumVehs}");
                            canStoreVehicle = Convert.ToInt32(count[0].NumVehs) <= garage.MaxVehicles;
                        }));

                    var ticks = 0;
                    while (canStoreVehicle == null && ticks < 150)
                    {
                        await BaseScript.Delay(0);
                        ticks++;
                    }

                    if (canStoreVehicle != null && !(bool)canStoreVehicle)
                    {
                        playerSession.Message("[Garage]", $"You currently cannot store this vehicle here because this garage is at max capacity ({garage.MaxVehicles} vehicles)", ConstantColours.Green);
                        return;
                    }
                }

                playerSession.TriggerEvent("Vehicle.DeleteCurrentVehicle");
                playerSession.Message("[Garage]", "Storing vehicle", ConstantColours.Green);
                storeVehicle(vehicleId, vehicleMods, garage.Name);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        //[EventHandler("Vehicles.StoreOwnedVehicle")]
        private void storeVehicle(int vehicleId, string vehicleMods, string garage)
        {
            Log.Debug($"About to start storing vehicle ({vehicleId})");
            var veh = VehManager.GetVehicle(vehicleId);

            Log.Debug($"Vehicle plate -> {veh?.Plate}");
            if (veh == null) return;

            var oldMods = veh.Mods;

            veh.Mods = JsonConvert.DeserializeObject<VehicleDataModel>(vehicleMods);
            veh.Mods.VehicleFuel = oldMods.VehicleFuel;

            veh.Garage = garage;

            VehManager.SaveVehicle(veh, true);

            Log.Verbose($"Storing vehicle ({vehicleId}) at garage ({garage})");
        }
    }
}
