using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Enums;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Client.UI;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client.Vehicles
{
    public class FuelManager : ClientAccessor
    {
        /// <summary>
        /// Used to calculate how much fuel is used every second when driving a vehicle. The bigger the number the less fuel used
        /// </summary>
        private float fuelUsageMultiplier = 750.0f;
        private float vehicleFuel = -1;
        private float FuelPumpRange = 4f;
        private bool isNearFuelPump = false;
        private ObjectList ObjectList = new ObjectList();
        private VehicleList vehicleList = new VehicleList();
        private List<ObjectHash> FuelPumpModelHashes = new List<ObjectHash>()
        {
            ObjectHash.prop_gas_pump_1a,
            ObjectHash.prop_gas_pump_1b,
            ObjectHash.prop_gas_pump_1c,
            ObjectHash.prop_gas_pump_1d,
            ObjectHash.prop_gas_pump_old2,
            ObjectHash.prop_gas_pump_old3,
            ObjectHash.prop_vintage_pump
        };
        private Dictionary<VehicleClass, float> vehicleClassMultipliers = new Dictionary<VehicleClass, float>
        {
            [VehicleClass.Sports] = 1.5f,
            [VehicleClass.Super] = 1.5f
        };
        private readonly Animation jerryCanAnimation = new Animation("weapon@w_sp_jerrycan", "", "fire", "fire_outro", "Refuel", new AnimationOptions
        {
            LoopDoLoop = true
        });

        public FuelManager(Client client) : base(client)
        {
            EntityDecoration.RegisterProperty("Vehicle.Fuel", DecorationType.Float);
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>(async (veh, seat, name) =>
            {
                var vehicle = Cache.PlayerPed.CurrentVehicle;

                vehicleFuel = -1;
                if (vehicle.HasDecor("Vehicle.ID"))
                {
                    client.TriggerServerEvent("Vehicle.RequestFuelLevel", vehicle.GetDecor<int>("Vehicle.ID"));
                }

                await BaseScript.Delay(1000);
                client.RegisterTickHandler(FuelTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) =>
            {
                vehicleFuel = -1;
                client.DeregisterTickHandler(FuelTick);
            }));
            client.RegisterEventHandler("Vehicle.RecieveFuelLevel", new Action<float>(fuel =>
            {
                Log.Verbose($"Recieved a fuel level of {fuel}%");
                vehicleFuel = fuel;
            }));
            
            client.RegisterTickHandler(CheckForPumps);

            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemStandard
            {
                Title = "Refuel vehicle",
                OnActivate = item => PumpRefuel()
            }, () => isNearFuelPump && !Game.PlayerPed.IsInVehicle(), 500);

            CommandRegister.RegisterCommand("refuel", OnRefuelCommand);
        }

        private void PumpRefuel()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Log.ToChat("[Fuel]", $"You can't refuel while in a vehicle", ConstantColours.Fuel);
                return;
            }

            bool IsNearbyRefuelVehicle(Vehicle veh)
            {
                var hasBone = veh.Bones.HasBone("indicator_rr");

                if (hasBone)
                {
                    return veh.Bones["indicator_rr"].Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(4, 2);
                }

                return veh.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(4, 2);
            }

            var nearbyVehicles = vehicleList.Select(v => (CitizenFX.Core.Vehicle)Entity.FromHandle(v))
                .Where(IsNearbyRefuelVehicle)
                .OrderBy(v => v.Bones.HasBone("indicator_rr") ? v.Bones["indicator_rr"].Position.DistanceToSquared(Game.PlayerPed.Position) : v.Position.DistanceToSquared(Game.PlayerPed.Position)).ToList();

            if (!nearbyVehicles.Any())
            {
                Log.ToChat("[Fuel]", "You are not close enough to a vehicle.", ConstantColours.Fuel);
                return;
            }

            var vehicle = nearbyVehicles.First();

            var nearbyPumps = ObjectList.Select(o => new Prop(o))
                .Where(o => FuelPumpModelHashes.Contains((ObjectHash)(uint)o.Model.Hash))
                .Where(o => o.Position.DistanceToSquared(vehicle.Position) < Math.Pow(FuelPumpRange, 2));

            if (!nearbyPumps.Any())
            {
                Log.ToChat("[Fuel]", "You are not close enough to a pump.", ConstantColours.Fuel);
                return;
            }

            var vehId = vehicle.GetDecor<int>("Vehicle.ID");
            var netId = CitizenFX.Core.Native.API.VehToNet(vehicle.Handle);//vehicle.NetworkId;

            Log.Info($"Vehicle ID - {vehId}; Net ID - {netId}");
            Client.TriggerServerEvent("Vehicle.OnPumpRefuel", vehId, netId);
        }

        private async Task CheckForPumps()
        {
            try
            {
                if (!Game.PlayerPed.IsInVehicle())
                {
                    isNearFuelPump = ObjectList.Select(o => new Prop(o))
                        .Where(o => o.Exists() && FuelPumpModelHashes.Contains((ObjectHash)(uint)o.Model.Hash)).Any(o =>
                            o.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2 * FuelPumpRange, 2));
                    await BaseScript.Delay(isNearFuelPump ? 5000 : 1000);
                }
            }
            catch(Exception e) { Log.Error(e); }
        }

        private async Task FuelTick()
        {
            var playerVeh = Cache.PlayerPed.CurrentVehicle;

            if (playerVeh == null || playerVeh.Driver != Cache.PlayerPed || !playerVeh.HasDecor("Vehicle.ID")) return;

            if (vehicleFuel == -1)
            {
                Client.TriggerServerEvent("Vehicle.RequestFuelLevel", playerVeh.GetDecor<int>("Vehicle.ID"));
                playerVeh.SetDecor("Vehicle.Fuel", 50.0f);
                await BaseScript.Delay(2000);
                return;
            }

            if (playerVeh.IsEngineRunning)
            {
                var fuelUsed = playerVeh.Speed / fuelUsageMultiplier;
                if (fuelUsed <= 0.002f)
                    fuelUsed = 0.002f;

                var vehClass = playerVeh.ClassType;
                if (vehicleClassMultipliers.ContainsKey(vehClass))
                {
                    fuelUsed *= vehicleClassMultipliers[vehClass];
                }

                vehicleFuel -= fuelUsed;

                if (vehicleFuel <= 0.0f)
                {
                    vehicleFuel = 0.0f;
                    BaseScript.TriggerEvent("Vehicle.SetEngineState", false);
                }

                var vehId = playerVeh.GetDecor<int>("Vehicle.ID");
                Client.TriggerServerEvent("Vehicle.OnFuelUpdate", vehId, vehicleFuel);
                playerVeh.SetDecor("Vehicle.Fuel", vehicleFuel);
            }

            await BaseScript.Delay(1000);
        }

        private async void OnRefuelCommand(Command cmd)
        {
            var frontVeh = GTAHelpers.GetClosestVehicle();
            if (frontVeh != null)
            {
                if (!frontVeh.HasDecor("Vehicle.ID")) return;

                var currentWeapon = Game.PlayerPed.Weapons.Current;
                if (currentWeapon.Hash == WeaponHash.PetrolCan)
                {
                    var weaponAmmo = currentWeapon.Ammo / 45;
                    var vehId = frontVeh.GetDecor<int>("Vehicle.ID");
                    var netId = CitizenFX.Core.Native.API.VehToNet(frontVeh.Handle);//frontVeh.NetworkId;

                    jerryCanAnimation.PlayFullAnim();
                    Client.TriggerServerEvent("Vehicle.OnManualRefuel", vehId, netId, weaponAmmo);
                }
                else
                {
                    PumpRefuel();
                }
            }
            else
            {
                Log.ToChat("[Fuel]", "You are not close enough to a vehicle to refuel it", ConstantColours.Fuel);
            }
        }
    }
}
