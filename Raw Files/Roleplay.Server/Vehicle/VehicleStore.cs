using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Players;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using static Roleplay.Shared.VehicleStoreItems;

namespace Roleplay.Server.Vehicle
{
    internal class VehicleStore : ServerAccessor
    {
        public VehicleStore(Server server) : base(server)
        {
            server.RegisterEventHandler("Vehicle.Store.BuyVehicle", new Action<Player, string, int, string, string>(handleVehiclePriceCheck));
        }

        private async void handleVehiclePriceCheck([FromSource] Player source, string vehName, int vehPrice, string vehStoreMenu, string vehModData)
        {
            Session.Session playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            Log.Verbose($"{playerSession.PlayerName} is attmempting to buy a {vehName}");

            var targetVehicles = StoreMenus[vehStoreMenu];
            foreach(var vehicle in targetVehicles)
            {
                if (vehicle.modelName == vehName)
                {
                    if (vehicle.price != vehPrice)
                    {
                        //TriggerEvent("flagPlayer", Convert.ToInt32(source.Handle), "hex editing vehicle store", $"Vehicle: {b.displayName}\nActual price: ${b.price}\nHex edited cost: ${vehPrice}", 8, "Hex editing vehicle store");
                    }

                    var payHandler = Server.Get<PaymentHandler>();
                    if (payHandler.CanPayForItem(playerSession, vehicle.price, 1))
                    {
                        var vehiclePlate = await RandomPlateGenerator.GenerateRandomPlate();
                        payHandler.PayForItem(playerSession, vehicle.price, $"buying vehicle {vehicle.displayName}");
                        Log.ToClient("[Bank]", $"You just bought this {vehicle.displayName} for ${vehicle.price}", ConstantColours.Bank, source);
                        source.TriggerEvent("Vehicle.Store.SpawnBoughtVehicle", vehicle.modelName, vehiclePlate, vehStoreMenu, vehModData);
                        RegisterVehicle(playerSession, vehicle.displayName, vehiclePlate, vehModData, vehicle.price);
                    }
                    else
                    {
                        Log.ToClient("[Bank]", "You do not have enough money to buy this item", ConstantColours.Bank, source);
                    }

                    return;
                }
            }
        }

        private void RegisterVehicle(Session.Session playerSession, string vehName, string plate, string vehMods, int vehPrice)
        {
            MySQL.execute("INSERT INTO vehicle_data (`CharID`, `SteamID`, `VehicleName`, `Plate`, `Mods`, `VehiclePrice`, `Inventory`) VALUES (@char, @steam, @name, @plate, @mods, @price, @inv)", new Dictionary<string, dynamic>
            {
                ["@char"] = playerSession.GetGlobalData<int>("Character.CharID"),
                ["@steam"] = playerSession.SteamIdentifier,
                ["@name"] = vehName,
                ["@plate"] = plate,
                ["@mods"] = vehMods ?? "",
                ["@price"] = vehPrice,
                ["@inv"] = ""
            }, new Action<dynamic>(dat =>
            {
                MySQL.execute("SELECT * FROM vehicle_data WHERE Plate = @plate", new Dictionary<string, dynamic>
                {
                    ["@plate"] = plate
                }, new Action<List<dynamic>>(data =>
                {
                    playerSession.Source.TriggerEvent("Vehicle.SetBoughtVehID", data[0].VehID);
                    Server.Get<VehicleManager>().AddVehicle(new Models.Vehicle(data[0], playerSession));
                    Server.Get<VehicleManager>().AddPlayerOwnedVehicle(playerSession, data[0].VehID);
                    MySQL.execute("UPDATE vehicle_data SET OutGarage = true WHERE VehID = @id", new Dictionary<string, dynamic>
                    {
                        ["@id"] = data[0].VehID
                    });
                }));
            }));
            playerSession.SetLocalData("Vehicle.Store.CanAddVehicle", false);        
        }
    }
}
