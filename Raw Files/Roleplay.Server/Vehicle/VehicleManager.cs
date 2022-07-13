using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Roleplay.Server.Bank;
using Roleplay.Server.Enums;
using Roleplay.Server.Players;
using Roleplay.Server.Session;
using Roleplay.Server.Vehicle.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Vehicle
{
    public class VehicleManager : ServerAccessor
    {
        //private SessionManager Session;
        private List<Models.Vehicle> currentVehicles = new List<Models.Vehicle>();
        private int saveInterval = CitizenFX.Core.Native.API.GetConvarInt("mg_serverVehicleSaveInterval", 300000);
        private int externalVehs = 1000000;

        public IReadOnlyList<Models.Vehicle> Vehicles => new List<Models.Vehicle>(currentVehicles);

        public VehicleManager(Server server) : base(server)
        {
            //Session = Sessions;
            //server.RegisterEventHandler("Vehicle.RequestGarageVehicles", new Action<Player, string>(OnVehiclesRequest));
            //server.RegisterEventHandler("Vehicle.RequestVehicleData", new Action<Player, int>(OnDataRequest));
            //server.RegisterEventHandler("Vehicle.CreateExternalVehicle", new Action<Player, string>(OnCreateExternalVehicle));
            //server.RegisterEventHandler("Vehicle.GiveKeyAccess", new Action<Player, int, int>(OnGiveKeyAccess));
            //server.RegisterEventHandler("Vehicle.GiveVehicleOwnership", new Action<Player, int, int>(OnGiveVehOwnership));
            //server.RegisterEventHandler("Vehicle.ImpoundVehicle", new Action<Player, int>(OnImpoundVehicle));
            //server.RegisterEventHandler("Vehicles.UpdateVehicleData", new Action<Player, int, string>(OnDataUpdate));
            //server.RegisterEventHandler("Vehicles.StoreOwnedVehicle", new Action<Player, int, string, string>(OnStoreVehicle));
            //server.RegisterEventHandler("Vehicles.SetIndicatorStatus", new Action<Player, string, bool>(OnSetIndicator));
            server.RegisterTickHandler(SaveVehicleTick);
            CommandRegister.RegisterAdminCommand("listvehdata", cmd =>
            {
                var veh = GetVehicle(cmd.GetArgAs<int>(0));
                foreach (var property in veh.GetType().GetRuntimeProperties())
                {
                    dynamic propertyValue = property.GetValue(veh, null);
                    var propertyString = propertyValue.ToString();
                    if (propertyValue.GetType() == typeof(List<int>))
                    {
                        propertyString = "";
                        foreach (var val in propertyValue)
                        {
                            propertyString += $"{val},";
                        }
                    }
                    Log.ToClient($"{property.Name}: {propertyString}");
                }
            }, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("savevehicles", cmd => SaveAllVehicles(), AdminLevel.SuperAdmin);
            Task.Factory.StartNew(async () =>
            {
                await BaseScript.Delay(5000);
                MySQL.execute("UPDATE vehicle_data SET OutGarage = false");

                MySQL.execute("SELECT * FROM vehicle_data", new Dictionary<string, dynamic>
                {
                    
                }, new Action<List<dynamic>>(data =>
                {
                    foreach(var veh in data)
                    {
                        try
                        {
                            VehicleDataModel mods = JsonConvert.DeserializeObject<VehicleDataModel>(veh.mods);
                            if(mods.Model == 0)
                            {
                                MySQL.execute("UPDATE bank_accounts SET Balance = Balance + @price WHERE OwnerID = @id", new Dictionary<string, dynamic>
                                {
                                    {
                                        "@price", veh.VehiclePrice
                                    },
                                    {
                                        "@id", veh.CharID
                                    }
                                });
                                MySQL.execute("DELETE FROM vehicle_data WHERE VehID = @id", new Dictionary<string, dynamic>
                                {
                                    {
                                        "@id", veh.VehID
                                    }
                                });
                            }
                        }
                        catch { }
                    }
                }));
            });
        }

        public Models.Vehicle GetVehicle(int vehId) => currentVehicles.FirstOrDefault(o => o.VehID == vehId);
        public Models.Vehicle GetVehicleByPlate(string vehPlate) => currentVehicles.FirstOrDefault(o => String.Equals(o.Plate, vehPlate, StringComparison.CurrentCultureIgnoreCase));

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var characterId = playerSession.GetGlobalData<int>("Character.CharID");
            var vehicleKeyAccess = currentVehicles.Where(o => o.KeyAccessCharacters.Contains(characterId)).Select(o => o.VehID).ToList();
            var ownedVehicles = currentVehicles.Where(o => o.CharID == characterId).Select(o => o.VehID).ToList();

            //playerSession.SetLocalData("Vehicles.AccessibleVehicles", vehicleKeyAccess);
            //playerSession.SetLocalData("Vehicles.OwnedVehicles", ownedVehicles);
            AddPlayerOwnedVehicle(playerSession, ownedVehicles);
            GivePlayerVehicleAccess(playerSession, vehicleKeyAccess);
        }

        public void GivePlayerVehicleAccess(Session.Session playerSession, List<int> vehList) => vehList.ForEach(o => GivePlayerVehicleAccess(playerSession, o));

        public void GivePlayerVehicleAccess(Session.Session playerSession, int vehId)
        {
            var currentVehOwners = playerSession.GetLocalData("Vehicles.AccessibleVehicles", new List<int>());
            
            if(!currentVehOwners.Contains(vehId))
                currentVehOwners.Add(vehId);

            playerSession.SetLocalData("Vehicles.AccessibleVehicles", currentVehOwners);

            var veh = GetVehicle(vehId);

            if (!veh.KeyAccessCharacters.Contains(playerSession.GetGlobalData<int>("Character.CharID")))
                veh.KeyAccessCharacters.Add(playerSession.GetGlobalData<int>("Character.CharID"));
        }

        public void AddPlayerOwnedVehicle(Session.Session playerSession, List<int> vehList) => vehList.ForEach(o => AddPlayerOwnedVehicle(playerSession, o));

        public void AddPlayerOwnedVehicle(Session.Session playerSession, int vehId)
        {
            var currentOwnedVehs = playerSession.GetLocalData("Vehicles.OwnedVehicles", new List<int>());

            if (!currentOwnedVehs.Contains(vehId))
                currentOwnedVehs.Add(vehId);

            GivePlayerVehicleAccess(playerSession, vehId);
            playerSession.SetLocalData("Vehicles.OwnedVehicles", currentOwnedVehs);

            var veh = GetVehicle(vehId);

            if(!veh.KeyAccessCharacters.Contains(playerSession.GetGlobalData<int>("Character.CharID")))
                veh.KeyAccessCharacters.Add(playerSession.GetGlobalData<int>("Character.CharID"));
        }

        public void RemovePlayerOwnedVehicle(Session.Session playerSession, int vehId)
        {
            var currentOwnedVehs = playerSession.GetLocalData("Vehicles.OwnedVehicles", new List<int>());

            if (currentOwnedVehs.Contains(vehId))
                currentOwnedVehs.Remove(vehId);

            playerSession.SetLocalData("Vehicles.OwnedVehicles", currentOwnedVehs);
        }

        public void RemovePlayerVehicleAccess(Session.Session playerSession, int vehId)
        {
            var currentOwnedVehs = playerSession.GetLocalData("Vehicles.AccessibleVehicles", new List<int>());

            if (currentOwnedVehs.Contains(vehId))
                currentOwnedVehs.Remove(vehId);

            playerSession.SetLocalData("Vehicles.AccessibleVehicles", currentOwnedVehs);
        }

        public void AddVehicle(Models.Vehicle veh)
        {
            currentVehicles.Add(veh);
        }

        public void RemoveVehicle(Models.Vehicle veh)
        {
            try
            {
                currentVehicles.Remove(veh);
            }
            catch (Exception e)
            {
                // none
            }
        }

        public void SaveAllVehicles()
        {
            currentVehicles.ForEach(o =>
            {
                if (o.CharID != -1)
                {
                    SaveVehicle(o);
                }
            });
        }

        [EventHandler("Vehicle.CreateExternalVehicle")]
        public void OnCreateExternalVehicle([FromSource] Player source, string vehData, bool isRentedVeh = false)
        {
            try
            {
                Log.Verbose($"{source.Name} is attempting to create an external vehicle. Is rented = {isRentedVeh}");
                Session.Session playerSession = Sessions.GetPlayer(source);
                if (playerSession == null) return;

                var externalVeh = new Models.Vehicle
                {
                    VehicleOwner = playerSession,
                    VehID = externalVehs,
                    Mods = JsonConvert.DeserializeObject<VehicleDataModel>(vehData),
                    KeyAccessCharacters = new List<int>
                    {
                        playerSession.GetGlobalData<int>("Character.CharID")
                    },
                    RentedVehicle = isRentedVeh
                };

                externalVeh.Plate = externalVeh.Mods.LicensePlate;
                externalVeh.Inventory = new VehicleInventory("", externalVeh);
                currentVehicles.Add(externalVeh);

                GivePlayerVehicleAccess(playerSession, externalVehs);
                playerSession.TriggerEvent("Vehicle.ReceiveExternalVehID", externalVehs);

                Log.Verbose($"Created an extneral vehicle with a Vehicle.ID of {externalVehs} and a plate of {externalVeh.Plate}");
                externalVehs += 1;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        [EventHandler("Vehicle.RequestGarageVehicles")]
        private void OnVehiclesRequest([FromSource] Player source, string garage)
        {
            Session.Session playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            if(garage == "Impound")
            {
                GetImpoundVehicles(playerSession);
                return;
            }

            Log.Verbose($"Requesting vehicle data for {source.Name} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            MySQL.execute("SELECT * FROM vehicle_data WHERE CharID = @char AND Garage = @garage AND OutGarage = false AND Impounded = false", new Dictionary<string, dynamic>
            {
                ["@char"] = playerSession.GetGlobalData<int>("Character.CharID"),
                ["@garage"] = garage
            }, new Action<List<object>>(data =>
            {
                source.TriggerEvent("Vehicle.RecieveGarageVehicles", data);
                Log.Verbose($"Got vehicle data for player {source.Name} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            }));
        }

        // Gets all impounded vehicles of a players current character
        private void GetImpoundVehicles(Session.Session playerSession)
        {
            Log.Verbose($"Requesting impounded vehicle data for {playerSession.PlayerName} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            MySQL.execute("SELECT * FROM vehicle_data WHERE CharID = @char AND Impounded = true", new Dictionary<string, dynamic>
            {
                ["@char"] = playerSession.GetGlobalData<int>("Character.CharID"),
            }, new Action<List<dynamic>>(data =>
            {
                data.ForEach(o =>
                {
                    ((IDictionary<string, dynamic>)o)["ImpoundPrice"] = o.VehiclePrice * 0.1f;
                });
                playerSession.TriggerEvent("Vehicle.RecieveGarageVehicles", data, true);
                Log.Verbose($"Got impounded vehicle data for player {playerSession.PlayerName} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            }));
        }

        [EventHandler("Vehicle.RequestVehicleData")]
        private void OnDataRequest([FromSource] Player source, int vehId)
        {
            Session.Session playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            Log.Verbose($"Attempting to load vehicle ({vehId}) for {source.Name} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            MySQL.execute("SELECT * FROM vehicle_data WHERE VehID = @id", new Dictionary<string, dynamic>
            {
                ["@id"] = vehId
            }, new Action<List<dynamic>>(data =>
            {
                if (data[0].Impounded)
                {
                    RetrieveImpoundVehicle(playerSession, data[0]);
                    return;
                }

                //VehicleDataModel modData = JsonConvert.DeserializeObject<VehicleDataModel>(data[0].Mods);
                var didParse = ((string) data[0].Mods).TryParseJson(out VehicleDataModel modData);

                if (!didParse)
                {
                    modData = new VehicleDataModel();
                }

                try
                {
                    modData.Model = CitizenFX.Core.Native.API.GetHashKey(VehicleStoreItems.StoreMenus.FirstOrDefault(o => o.Value.FirstOrDefault(b => b.displayName == data[0].VehicleName) != null).Value.FirstOrDefault(o => o.displayName == data[0].VehicleName)?.modelName);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                source.TriggerEvent("Vehicle.SpawnGarageVehicle", data[0].Plate, data[0].VehID, /*data[0].Mods*/JsonConvert.SerializeObject(modData));
                currentVehicles.Add(new Models.Vehicle(data[0], playerSession));
                AddPlayerOwnedVehicle(playerSession, data[0].VehID);
                MySQL.execute("UPDATE vehicle_data SET OutGarage = true WHERE VehID = @id", new Dictionary<string, dynamic>
                {
                    ["@id"] = vehId
                });

                Log.Verbose($"Loaded vehicle ({vehId}) for {source.Name} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            }));
        }

        // Attempts to pay for an impounded vehicle
        private void RetrieveImpoundVehicle(Session.Session playerSession, dynamic vehData)
        {
            var impoundPrice = (int)(vehData.VehiclePrice * 0.1f);

            if (Server.Get<PaymentHandler>().CanPayForItem(playerSession, impoundPrice))
            {
                Server.Get<PaymentHandler>().PayForItem(playerSession, impoundPrice, $"getting {vehData.VehicleName} #({vehData.VehID}) from impound");

                playerSession.TriggerEvent("Vehicle.SpawnGarageVehicle", vehData.Plate, vehData.VehID, vehData.Mods);
                var vehicle = new Models.Vehicle(vehData, playerSession)
                {
                    Impounded = false
                };
                currentVehicles.Add(vehicle);
                AddPlayerOwnedVehicle(playerSession, vehData.VehID);
                MySQL.execute("UPDATE vehicle_data SET OutGarage = true, Impounded = false WHERE VehID = @id", new Dictionary<string, dynamic>
                {
                    ["@id"] = vehData.VehID
                });

                Log.Verbose($"Loaded impounded vehicle ({vehData.VehID}) for {playerSession.PlayerName} (CharID: {playerSession.GetGlobalData<int>("Character.CharID")})");
            }
            else
            {
                Log.ToClient("[Bank]", $"You cannot afford to do this", ConstantColours.Bank, playerSession.Source);
                Log.Verbose($"{playerSession.PlayerName} was unable to pay for impounded vehicle ({vehData.VehID})");
            }
        }

        [EventHandler("Vehicle.GiveKeyAccess")]
        private void OnGiveKeyAccess([FromSource] Player source, int vehId, int targetPlayerId)
        {
            var targetSession = Sessions.GetPlayer(targetPlayerId);
            if (targetSession == null) return;

            GivePlayerVehicleAccess(targetSession, vehId);

            Log.ToClient("", "You just got given keys to this vehicle", Color.Empty, targetSession.Source);
            Log.ToClient("", $"You just gave {targetSession.GetGlobalData<string>("Character.FirstName")} access to this vehicle", Color.Empty, source);
        }

        [EventHandler("Vehicle.GiveVehicleOwnership")]
        private void OnGiveVehOwnership([FromSource] Player source, int vehId, int targetPlayerId)
        {
            var targetSession = Sessions.GetPlayer(targetPlayerId);
            var veh = GetVehicle(vehId);
            if (targetSession == null || veh == null) return;

            Log.Verbose($"{source.Name} just gave {targetSession.PlayerName} ownership of vehicle {vehId}");
            //GivePlayerVehicleAccess(targetSession, vehId);
            AddPlayerOwnedVehicle(targetSession, vehId);
            RemovePlayerOwnedVehicle(Sessions.GetPlayer(source), vehId);
            veh.CharID = targetSession.GetGlobalData<int>("Character.CharID");
            veh.Garage = "Public1";
            SaveVehicle(veh);

            Log.ToClient("", "You just got given ownership of this vehicle", Color.Empty, targetSession.Source);
            Log.ToClient("", $"You just gave {targetSession.GetGlobalData<string>("Character.FirstName")} ownership of this vehicle", Color.Empty, source);
        }

        [EventHandler("Vehicle.ImpoundVehicle")]
        private void OnImpoundVehicle([FromSource] Player source, int vehId)
        {
            var veh = GetVehicle(vehId);
            Session.Session playerSession = Sessions.GetPlayer(source);

            if (playerSession == null || veh == null) return;

            veh.Impounded = true;
            veh.OutGarage = false;
            SaveVehicle(veh, true);

            Log.Verbose($"Impounded vehicle {vehId}");
        }

        [EventHandler("Vehicles.UpdateVehicleData")]
        private void OnDataUpdate([FromSource] Player source, int vehicleId, string vehicleMods)
        {
            var playerSession = Sessions.GetPlayer(source);
            var veh = GetVehicle(vehicleId);
            if (veh == null || playerSession == null) return;

            Log.Debug($"Updating vehicle data for vehicle ({veh.VehID})");
            if (veh.CharID == playerSession.GetGlobalData<int>("Character.CharID"))
            {
                var oldMods = veh.Mods;

                veh.Mods = JsonConvert.DeserializeObject<VehicleDataModel>(vehicleMods);
                veh.Mods.VehicleFuel = oldMods.VehicleFuel;
            }
        }

        public void SaveVehicle(Models.Vehicle vehicle, bool removeVehicle = false)
        {
            MySQL.execute("UPDATE vehicle_data SET CharID = @charid, VehicleName = @veh, Plate = @plate, Mods = @mods, Inventory = @inv, VehiclePrice = @price, Garage = @garage, Impounded = @impound, OutGarage = @outstate WHERE VehID = @id", new Dictionary<string, dynamic>
            {
                ["@charid"] = vehicle.CharID,
                ["@veh"] = vehicle.VehicleName,
                ["@plate"] = vehicle.Plate,
                ["@mods"] = JsonConvert.SerializeObject(vehicle.Mods),
                ["@inv"] = vehicle.Inventory.ToString(),
                ["@price"] = vehicle.VehiclePrice,
                ["@garage"] = vehicle.Garage,
                ["@impound"] = vehicle.Impounded,
                ["@outstate"] = !removeVehicle,
                ["@id"] = vehicle.VehID
            });

            if (removeVehicle)
                currentVehicles.Remove(vehicle);
        }

        [EventHandler("Vehicles.SetIndicatorStatus")]
        private void OnSetIndicator([FromSource] Player source, string indicator, bool state)
        {
            BaseScript.TriggerClientEvent("Vehicle.SetIndicatorStatus", int.Parse(source.Handle), indicator, state);
        }

        private async Task SaveVehicleTick()
        {
            await BaseScript.Delay(saveInterval);

            SaveAllVehicles();
        }
    }
}