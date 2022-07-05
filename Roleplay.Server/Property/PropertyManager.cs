using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs;
using Roleplay.Server.Property;
using Roleplay.Server.Realtor.Models;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Realtor
{
    // TODO properties owned by a player are loaded as the player logs but ones they are tenants are loaded after the fact and are just placed in a backlog queue to be loaded
    public class PropertyManager : ServerAccessor
    {
        public static List<Vector3> HouseInteriorLocations = new List<Vector3>
        {
            new Vector3(-781.8839f, 326.3657f, 175.8037f),
            new Vector3(-774.6219f, 331.1073f, 159.0015f),
            new Vector3(-782.1074f, 326.7740f, 222.2576f),
            new Vector3(-774.0350f, 330.4203f, 208.6210f),
            new Vector3(346.45f, -1013.03f, -100.20f),
            new Vector3(266.08f, -1007.52f, -102.0086f), 
            new Vector3(151.53f, -1007.58f, -99.9999f),
            new Vector3(-1452.24f, -540.57f, 74.04f), 
            new Vector3(-892.06f, -434.21f, 121.61f), 
            new Vector3(-603.61f, 59.06f, 98.2f),
            new Vector3(-174.31f, 497.53f, 136.67f),
            new Vector3(341.64f, 437.69f, 148.39f),
            new Vector3(373.61f, 423.56f, 144.91f),
            new Vector3(-682.33f, 592.47f, 144.39f),
            new Vector3(-758.67f, 618.98f, 143.15f),
            new Vector3(-859.78f, 690.95f, 151.86f),
            new Vector3(-572.00f, 661.68f, 144.84f),
            new Vector3(117.27f, 559.77f, 183.30f),
            new Vector3(-1289.78f, 449.40f, 96.90f),
            new Vector3(-786.27f, 315.75f, 217.02f), 
            new Vector3(-774.89f, 341.94f, 196.69f), 
            new Vector3(-786.4f, 315.78f, 187.91f), 
            new Vector3(1397.06f, 1141.78f, 113.332f),
            new Vector3(-1150.61f, -1520.79f, 10.64f),
            new Vector3(-14.33f, -1440.01f, 31.1f),
            new Vector3(237.82f, -1004.78f, -99.97f),
            new Vector3(207.33f, -999.14f, -99.0f),
            new Vector3(997.33f, -3158.07f, -38.91f),
            new Vector3(1121.01f, -3152.49f, -37.06f),

            //New Mid Interior
            new Vector3(-1407.05f, -2629.86f, -91.99f),
            //Michaels house Interior
            new Vector3(-815.8f, 178.57f, 72.15f),

            //Office shit
            new Vector3(-1398.97f, -481.93f, 72.04f),
            new Vector3(-77.39f, -833.55f, 243.39f),
        };
        public IReadOnlyList<PropertyModel> Properties => new List<PropertyModel>(properties);

        private List<PropertyModel> properties = new List<PropertyModel>();
        private List<string> usedPropertyIds = new List<string>();

        public PropertyManager(Server server) : base(server)
        {
            CommandRegister.RegisterRCONCommand("listproperties", cmd =>
            {
                foreach (var property in properties)
                {
                    Log.Info("-------------------------------------------------------");
                    Log.Info(property.ToString());
                    Log.Info("-------------------------------------------------------");
                }
            });
            CommandRegister.RegisterRCONCommand("saveproperties", cmd =>
            {
                saveAllProperties();
            });
            loadPropertyIds();
        }

        public PropertyModel GetProperty(string propertyId) => Properties.FirstOrDefault(o => o.PropertyId == propertyId);

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            //LoadPropertiesForUser(playerSession, loadedProperties => 
            {
                var propertiesToSync = properties.Where(o => o.HasPropertyKeys(playerSession.CharId));

                foreach (var property in propertiesToSync)
                {
                    property.ResyncPropertyForPlayer(playerSession);
                }
            }//);
        }

        public PropertyModel GetClosestProperty(Session.Session session, float dist = 12.0f)
        {
            var playerPos = session.Position;
            var insideProperty = session.PropertyCurrentlyInside;

            return insideProperty ?? Properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(playerPos) < dist);
        }

        public void SpawnPlayerAtProperty(Session.Session session, string propertyId)
        {
            PropertyModel property;

            if ((property = GetProperty(propertyId)) == null) return;

            Log.Verbose($"Spawning {session.PlayerName} at property {property.Address} ({propertyId})");

            session.SetPlayerPosition(property.EntranceLocation);
        }

        public async Task<string> GeneratePropertyId()
        {
            var propertyId = MiscHelpers.CreateRandomString(6);
            while (usedPropertyIds.Contains(propertyId))
            {
                propertyId = MiscHelpers.CreateRandomString(6);
                await BaseScript.Delay(0);
            }
            usedPropertyIds.Add(propertyId);

            return propertyId;
        }

        public void LoadProperty(string propertyId, Action<PropertyModel> cb = null)
        {
            var queryString = "SELECT * FROM property_data WHERE PropertyID = @id";

            MySQL.execute(queryString, new Dictionary<string, dynamic>{ {"@id", propertyId} }, new Action<List<dynamic>>(propertyData =>
            {
                PropertyModel property = null;

                if (propertyData.ElementAtOrDefault(0) != null)
                {
                    property = databaseDataToProperty(propertyData[0]);

                    Log.Verbose($"Loaded property {property.PropertyId} from the database");

                    AddProperty(property);
                }

                cb?.Invoke(property);
            }));
        }

        // TODO optimize lazy loading algorithms to make this better than just loading all properties at once
        public void LoadPropertiesForUser(Session.Session playerSession, Action<List<PropertyModel>> cb = null)
        {
            // TODO load properties also have persistent access to
            Log.Debug($"Loading properties for {playerSession.PlayerName}");
            MySQL.execute("SELECT property_data.*, property_tenants.TenantCharacterID FROM property_data LEFT JOIN property_tenants ON property_tenants.PropertyID = property_data.PropertyID WHERE property_data.OwnerID = @id OR property_tenants.TenantCharacterID = @id", new Dictionary<string, dynamic> { { "@id", playerSession.CharId } }, new Action<List<dynamic>>(propertyData =>
            {
                var loadedProperties = new List<PropertyModel>();

                foreach (var propData in propertyData)
                {
                    PropertyModel property = databaseDataToProperty(propData);

                    Log.Verbose($"Loaded property {property.PropertyId} from the database for {playerSession.PlayerName}");

                    AddProperty(property);
                    loadedProperties.Add(property);

                    foreach (var item in propData)
                    {
                        Log.Info(item.ToString());
                    }

                    if (((IDictionary<string, dynamic>)propData).TryGetValue("TenantCharacterID", out var id)) // TODO find better way to do this
                    {
                        if (id == null) return;

                        Log.Debug($"propData.TenantCharacterID exists");
                        var prop = properties.First(o => o == property);
                        if(!prop.PropertyCharacterAccess.Contains(id))
                            prop.PropertyCharacterAccess.Add(id); // the property obj might not get added so cant edit that here without _issues_

                        property.ResyncPropertyForPlayer(playerSession);
                    }
                }

                cb?.Invoke(loadedProperties);
            }));
        }

        // TODO merge this query into one single query (joins?)
        public void LoadAllProperties()
        {
            Log.Debug($"Loading all properties from the database");
            MySQL.execute("SELECT property_data.*, COALESCE( GROUP_CONCAT( CONCAT(property_tenants.TenantCharacterID, ';',  property_tenants.AccessType) ), '' ) AS Tenants, property_finance.PropertyPrice, property_finance.TotalAmountPaid, property_finance.TotalMissedPayments, property_finance.ConsecutiveMissedPayments, property_finance.LastPayment, property_finance.CurrentInstallmentPayed FROM property_data LEFT JOIN property_tenants ON property_tenants.PropertyID = property_data.PropertyID LEFT JOIN property_finance ON property_data.PropertyID = property_finance.PropertyID GROUP BY property_data.PropertyID", new Dictionary<string, dynamic>(), new Action<List<dynamic>>(propertiesData =>
            {
                foreach (var propertyData in propertiesData)
                {
                    PropertyModel property = databaseDataToProperty(propertyData);
                    var tenants = ((string[])propertyData.Tenants.ToString().Split(',')).ToList(); // if the tenants list is empty it throws and "input string not in correct format" error so don't parse to int here 

                    Log.Debug($"Property {property.PropertyId} loading stage - property_data");
                    Log.Verbose($"Loaded property {property.PropertyId} data from the database");

                    {
                        Log.Debug($"Property {property.PropertyId} loading stage - property_tenants");
                        Log.Verbose($"Loaded tenants for property {property.PropertyId} adding them to the property access list");

                        foreach (var tenantData in tenants)
                        {
                            if(string.IsNullOrEmpty(tenantData)) continue;

                            var dataSplit = tenantData.Split(';');
                            var tenant = dataSplit[0];
                            var accessType = dataSplit[1];

                            Log.Verbose($"Adding character id {tenant} to property {property.PropertyId} with access type {accessType}");

                            if(accessType == "guest")
                            {
                                property.TemporaryCharacterAccess.Add(int.Parse(tenant));
                            }

                            if (accessType == "co-owner")
                            {
                                property.PropertyCharacterAccess.Add(int.Parse(tenant));
                            }
                        }

                        {
                            Log.Debug($"Property {property.PropertyId} loading stage - property_finance");
                            Log.Verbose($"Loaded property {property.PropertyId} finance data from the database");

                            property.FinanceData.LoadDataFromDatabase(propertyData);

                            AddProperty(property);

                            Sessions.TriggerSessionEvent("OnPropertyLoaded", property);
                        }
                    }
                }
            }));
        }

        public void AddProperty(PropertyModel property)
        {
            if(!properties.Contains(property))
            {
                Log.Debug($"Adding property {property.Address} with an id of {property.PropertyId}");
                properties.Add(property);
            }
        }

        public void RemoveProperty(PropertyModel property)
        {
            if (properties.Contains(property))
            {
                Log.Debug($"Removing proeprty {property.Address} with an id of {property.PropertyId}");
                properties.Remove(property);
            }
        }

        public void DeleteProperty(PropertyModel property)
        {
            Log.Verbose($"Deleting property {property.PropertyId}");
            property.DesyncProperty();
            property.OwnerCharacterId = -1;
            property.PropertyCharacterAccess.Clear();
            property.TemporaryCharacterAccess.Clear();

            MySQL.execute("DELETE FROM property_data WHERE PropertyID = ?", new List<string>{ property.PropertyId }, new Action<dynamic>(data =>
            {
                //Log.Verbose($"Removed property {property.PropertyId} from property_data");
            }));

            MySQL.execute("DELETE FROM property_finance WHERE PropertyID = ?", new List<string> { property.PropertyId }, new Action<dynamic>(data =>
            {
                //Log.Verbose($"Removed property {property.PropertyId} from property_finance");
            }));

            MySQL.execute("DELETE FROM property_tenants WHERE PropertyID = ?", new List<string> { property.PropertyId }, new Action<dynamic>(data =>
            {
                //Log.Verbose($"Removed property {property.PropertyId} from property_tenants");
            }));

            MySQL.execute("UPDATE vehicle_data SET Garage = 'Public1' WHERE Garage = ?", new List<string> { $"home-{property.PropertyId}" }, new Action<dynamic>(data =>
            {
                //Log.Verbose($"Reset garages for vehicles in property {property.PropertyId} to Public1 due to the property being deleted");
            }));

            RemoveProperty(property);
        }

        public void RegisterPropertyToDatabase(PropertyModel property)
        {
            if(!properties.Contains(property)) AddProperty(property);

            Log.Verbose($"Attempting to register property {property.PropertyId} to the database");

            MySQL.execute("INSERT INTO property_data (`OwnerID`, `PropertyID`, `Address`, `StorageLocation`, `StorageSize`, `GarageLocation`, `GarageMaxVehicles`, `EntranceLocation`, `InteriorID`, `StorageInventory`) VALUES (@owner, @propid, @address, @storeloc, @storesize, @garageloc, @garagevehs, @entranceloc, @interiorid, @inv)", new Dictionary<string, dynamic>
            {
                {"@owner", property.OwnerCharacterId},
                {"@propid", property.PropertyId},
                {"@address", property.Address},
                {"@storeloc", JsonConvert.SerializeObject(property.StorageLocation)},
                {"@storesize", property.StorageSize},
                {"@garageloc", JsonConvert.SerializeObject(property.GarageLocaiton)},
                {"@garagevehs", property.GarageMaxVehicles},
                {"@entranceloc", JsonConvert.SerializeObject(property.EntranceLocation)},
                {"@interiorid", property.InteriorId},
                {"@inv", ""}
            }, new Action<dynamic>(rows => {
                Log.Info($"Successfully registered property {property.PropertyId} to the database");
                Log.Verbose($"Registering property {property.PropertyId} finance data to the database due to successful registration");

                property.FinanceData.LastPaymentDate = DateTime.Now;

                RegisterPropertyFinanceData(property);
            }));
        }

        public void RegisterPropertyFinanceData(PropertyModel property)
        {
            Log.Verbose($"Attempting to register property {property.PropertyId} finance data to the database");

            MySQL.execute("INSERT INTO property_finance (`PropertyID`, `PropertyPrice`, `TotalAmountPaid`) VALUES (@id, @price, @curpaid)", new Dictionary<string, dynamic>
            {
                {"@id", property.PropertyId}, 
                {"@price", property.FinanceData.PropertyPrice},
                {"@curpaid", property.FinanceData.TotalAmountPaid},
            }, new Action<dynamic>(rows =>
            {
                Log.Info($"Successfully registered property {property.PropertyId} finance data to the database");
            }));
        }

        public void AddPropertyTenant(PropertyModel property, int tenantId, string accessType = "guest")
        {
            var tenantSession = Sessions.GetPlayerByCharID(tenantId);
            if (tenantSession == null) return;

            MySQL.execute("INSERT INTO property_tenants (`PropertyID`, `TenantCharacterID`, `AccessType`) VALUES (@id, @charid, @access)", new Dictionary<string, dynamic>
            {
                {"@id", property.PropertyId},
                {"@charid", tenantId},
                {"@access", accessType},
            }, new Action<dynamic>(rows =>
            {
                Log.Info($"Successfully registered character id {tenantId} as a tenant of property {property.PropertyId} with access type {accessType}");

                var ownerSession = Sessions.GetPlayerByCharID(property.OwnerCharacterId);

                if(accessType == "guest")
                {
                    ownerSession.Message("[Property]", $"You just gave {tenantSession.FirstName} {tenantSession.LastName} keys to this property");
                    tenantSession.Message("[Property]", $"You just got given keys to {property.Address} by {ownerSession.FirstName} {ownerSession.LastName}");

                    property.TemporaryCharacterAccess.Add(tenantId);
                }

                if (accessType == "co-owner")
                {
                    ownerSession.Message("[Property]", $"You just gave {tenantSession.FirstName} {tenantSession.LastName} partial ownership to this property");
                    tenantSession.Message("[Property]", $"You just got given partial ownership to {property.Address} by {ownerSession.FirstName} {ownerSession.LastName}");

                    property.PropertyCharacterAccess.Add(tenantId);
                }

                
                property.ResyncProperty();
            }));
        }

        public void RemovePropertyTenant(PropertyModel property, int tenantId, bool silent = false)
        {
            MySQL.execute("DELETE FROM property_tenants WHERE PropertyID = @id AND TenantCharacterID = @charid", new Dictionary<string, dynamic>
            {
                {"@id", property.PropertyId},
                {"@charid", tenantId}
            }, new Action<dynamic>(rows =>
            {
                Log.Info($"Successfully removed character id {tenantId} as a tenant of property {property.PropertyId}");

                var ownerSession = Sessions.GetPlayerByCharID(property.OwnerCharacterId);
                var tenantSession = Sessions.GetPlayerByCharID(tenantId);

                if(!silent)
                {
                    ownerSession.Message("[Property]", $"You just revoked {(tenantSession == null ? tenantId.ToString() : tenantSession.FirstName + " " + tenantSession.LastName)} persistent access to this property");
                    tenantSession?.Message("[Property]", $"Your keys for {property.Address} were just taken off you");
                }

                if(property.PropertyCharacterAccess.Contains(tenantId)) property.PropertyCharacterAccess.Remove(tenantId);
                if(property.TemporaryCharacterAccess.Contains(tenantId)) property.TemporaryCharacterAccess.Remove(tenantId);

                MySQL.execute("UPDATE vehicle_data SET Garage = 'Public1' WHERE Garage = ? AND CharID = ?", new List<dynamic> { $"home-{property.PropertyId}", tenantId }, new Action<dynamic>(data =>
                {
                    Log.Verbose($"Reset garages for vehicles in property {property.PropertyId} to Public1 due to the property being deleted");
                }));

                property.DesyncPropertyForPlayer(tenantId);
                property.ResyncProperty();
            }));
        }

        public void SyncPropertyData(Session.Session session, PropertyModel property)
        {
            Log.Verbose($"Preforming individual property sync for property {property.PropertyId} on {session.PlayerName}");
            property.ResyncPropertyForPlayer(session);
        }

        public void SaveProperty(PropertyModel property)
        {
            MySQL.execute("UPDATE property_data SET OwnerID = @owner, Address = @address, StorageLocation = @storeloc, StorageInventory = @storeinv, StorageSize = @storesize, GarageLocation = @garageloc, GarageMaxVehicles = @garagevehs, EntranceLocation = @entranceloc, InteriorID = @interiorid WHERE PropertyID = @propid", new Dictionary<string, dynamic>
            {
                {"@owner", property.OwnerCharacterId},
                {"@propid", property.PropertyId},
                {"@address", property.Address},
                {"@storeloc", JsonConvert.SerializeObject(property.StorageLocation)},
                {"@storeinv", property.StorageInventory.ToString()},
                {"@storesize", property.StorageSize},
                {"@garageloc", JsonConvert.SerializeObject(property.GarageLocaiton)},
                {"@garagevehs", property.GarageMaxVehicles},
                {"@entranceloc", JsonConvert.SerializeObject(property.EntranceLocation)},
                {"@interiorid", property.InteriorId}
            });
            Server.Get<PropertyFinanceManager>().SaveFinanceInformation(property);
        }

        public void AttemptLeaveProperty(Session.Session session)
        {
            if (!session.IsInProperty) return;

            var property = session.PropertyCurrentlyInside;
            var playerPos = session.Position;

            Log.Debug($"{session.PlayerName} is in a property ({property.PropertyId})");
            if (HouseInteriorLocations.Any(o => o.DistanceToSquared(playerPos) < 6.0f))
            {
                property.OnExitProperty(session);
                session.SetPlayerPosition(session.PropertyEntranceLocation);
                session.PropertyEntranceLocation = Vector3.Zero;
                session.PropertyCurrentlyInside = null;
                session.Instance = 0;
            }
        }

        [EventHandler("Player.OnInteraction")]
        private void OnInteraction([FromSource] Player source)
        {
            var session = Sessions.GetPlayer(source);
            if (session == null) return;

            var playerPos = session.Position;

            if (session.IsInProperty)
            {
                AttemptLeaveProperty(session);
                return;
            }

            var closeProperty = properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(playerPos) < 6.0f);
            if (closeProperty == null) return;

            Log.Debug($"{session.PlayerName} is near a property ({closeProperty.PropertyId}) entrance");

            //if (closeProperty.IsPropertyTenant(session.CharId)) // TODO make function to do this
            {
                if (closeProperty.IsLocked && !Server.Get<JobHandler>().OnDutyAs(session, JobType.Police))
                {
                    session.Message("[Property]", "The property is currently locked", ConstantColours.Housing);
                    return;
                }

                closeProperty.OnEnterProperty(session);
                session.PropertyEntranceLocation = playerPos; // store last known position outside of property so they can be teleported out after
                session.SetPlayerPosition(HouseInteriorLocations[closeProperty.InteriorId]);
                session.PropertyCurrentlyInside = closeProperty;
                session.Instance = closeProperty.OwnerCharacterId; // TODO change this to something better
            }
        }

        [EventHandler("Property.AttemptToggleLocks")]
        private void OnAttemptToggleLocks([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            Log.Verbose($"{source.Name} is attempting to toggle the lock status of a property");

            var playerPos = playerSession.Position;
            var closeProperty = properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(playerPos) < 6.0f && o.HasPropertyKeys(playerSession.CharId));

            try
            {
                if (closeProperty == null)
                {
                    Log.Debug($"closeProperty was null checking if {source.Name} is in a property");
                    if (playerSession.IsInProperty) // If someone has keys to this property allow them to lock in while inside
                    {
                        Log.Debug($"{source.Name} is in a property");
                        var insideProperty = playerSession.PropertyCurrentlyInside;

                        if (!insideProperty.HasPropertyKeys(playerSession.CharId))
                        {
                            Log.Debug($"{source.Name} doesn't have keys for the property they are inside");
                            return;
                        }

                        closeProperty = insideProperty;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                return;
            }

            playerSession.TriggerEvent("Animation.PlayLockAnim");

            Log.Verbose($"{source.Name} is close to a property toggling lock status");
            Log.Debug($"Property current lock status: {closeProperty.IsLocked}");
            Log.Debug($"Property new lock status: {!closeProperty.IsLocked}");

            closeProperty.IsLocked = !closeProperty.IsLocked;
            playerSession.Message("[Property]", $"Property {(closeProperty.IsLocked ? "locked" : "unlocked")}", ConstantColours.Housing);
        }

        private PropertyModel databaseDataToProperty(dynamic data)
        {
            try
            {
                var property = new PropertyModel
                {
                    OwnerCharacterId = data.OwnerID,
                    PropertyId = data.PropertyID,
                    Address = data.Address,
                    StorageLocation = JsonConvert.DeserializeObject<Vector3>(data.StorageLocation),
                    GarageLocaiton = JsonConvert.DeserializeObject<Vector3>(data.GarageLocation),
                    GarageMaxVehicles = data.GarageMaxVehicles,
                    EntranceLocation = JsonConvert.DeserializeObject<Vector3>(data.EntranceLocation),
                    InteriorId = data.InteriorID
                };
                property.StorageInventory = new PropertyInventory(data.StorageInventory, property);

                return property;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return null;
        }

        private async void loadPropertyIds()
        {
            await BaseScript.Delay(5000);
            usedPropertyIds.Clear();
            MySQL.execute("SELECT PropertyID FROM property_data", new Dictionary<string, dynamic>(), new Action<List<dynamic>>(data =>
            {
                Log.Debug("Loading property ids");
                foreach (var property in data)
                {
                    usedPropertyIds.Add(property.PropertyID);
                    Log.Debug($"Adding property id: {property.PropertyID}");
                }
            }));
            LoadAllProperties();
        }

        private void saveAllProperties()
        {
            Log.Info("Saving property data");
            foreach (var property in properties)
            {
                SaveProperty(property);
            }
            Log.Info("Saving property data complete");
        }

        [DynamicTick]
        private async Task savePropertyTick()
        {
            await BaseScript.Delay(CitizenFX.Core.Native.API.GetConvarInt("mg_saveInterval", 300000));

            saveAllProperties();
        }
    }
}
