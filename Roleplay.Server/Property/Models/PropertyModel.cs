using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Models;
using Roleplay.Server.Property.Models;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Realtor.Models
{
    public class PropertyModel
    {
        public string PropertyId;
        public string Address;
        public int OwnerCharacterId;
        /// <summary>
        /// Stores all characters apart from the owner who have access to this property (over restarts)
        /// </summary>
        public List<int> PropertyCharacterAccess = new List<int>();

        public List<int> TemporaryCharacterAccess = new List<int>();

        public Vector3 StorageLocation;
        public PropertyInventory StorageInventory;
        public int StorageSize;

        public Vector3 GarageLocaiton;
        public int GarageMaxVehicles = -1;

        public bool IsLocked = true;

        public Vector3 EntranceLocation;

        public int InteriorId;

        public int CurrentPropertyEditor = -1; // server id of who is currently editing this property

        public PropertyFinanceInformation FinanceData = new PropertyFinanceInformation();

        private GarageModel cachedGarageModel;

        public PropertyModel()
        {
            StorageInventory = new PropertyInventory("", this);
        }

        // TODO finish proper validation
        public bool IsPropertyCreated()
        {
            if (!Settings.PropertyCreationValidationEnabled) return true;

            var editorSession = Server.Instance.Get<SessionManager>().GetPlayer(CurrentPropertyEditor);
            if (editorSession == null) return false;

            var isProperlyCreated = true;

            if (string.IsNullOrEmpty(Address))
            {
                editorSession.Message("[Realtor]", $"Please set an address for this property", ConstantColours.Green);
                isProperlyCreated = false;
            }

            if (StorageLocation == Vector3.Zero)
            {
                editorSession.Message("[Realtor]", $"Please set a storage location for this property", ConstantColours.Green);
                isProperlyCreated = false;
            }

            if (GarageLocaiton == Vector3.Zero)
            {
                editorSession.Message("[Realtor]", $"Please set a garage location for this property", ConstantColours.Green);
                isProperlyCreated = false;
            }

            if (EntranceLocation == Vector3.Zero)
            {
                editorSession.Message("[Realtor]", $"Please set a entrance location for this property", ConstantColours.Green);
                isProperlyCreated = false;
            }

            if (GarageMaxVehicles == -1)
            {
                editorSession.Message("[Realtor]", $"Please set the garage size for this property", ConstantColours.Green);
                isProperlyCreated = false;
            }

            return isProperlyCreated;
        }

        public bool IsPropertyTenant(int charId) => IsPropertyOwner(charId) || PropertyCharacterAccess.Any(o => o == charId);

        public bool HasPropertyKeys(int charId) => TemporaryCharacterAccess.Contains(charId) || IsPropertyTenant(charId);

        public bool IsPropertyOwner(int charId) => charId == OwnerCharacterId;

        public bool IsBeingEdited() => CurrentPropertyEditor != -1;

        public bool AnyPlayerInside() => Server.Instance.Get<SessionManager>().PlayerList.Count(o => o.PropertyCurrentlyInside == this) > 0;

        public void ResyncProperty()
        {
            var editorSession = Server.Instance.Get<SessionManager>().GetPlayer(CurrentPropertyEditor);
            var ownerSession = Server.Instance.Get<SessionManager>().GetPlayerByCharID(OwnerCharacterId);

            ResyncPropertyForPlayer(editorSession);
            
            ResyncPropertyForPlayer(ownerSession);

            foreach (var tenant in PropertyCharacterAccess)
            {
                ResyncPropertyForPlayer(tenant);
            }

            foreach (var guest in TemporaryCharacterAccess)
            {
                ResyncPropertyForPlayer(guest);
            }
        }

        public void DesyncProperty()
        {
            var ownerSession = Server.Instance.Get<SessionManager>().GetPlayerByCharID(OwnerCharacterId);

            DesyncPropertyForPlayer(ownerSession);

            foreach (var tenant in PropertyCharacterAccess)
            {
                DesyncPropertyForPlayer(tenant);
            }

            foreach (var guest in TemporaryCharacterAccess)
            {
                DesyncPropertyForPlayer(guest);
            }
        }

        public void ResyncPropertyForPlayer(Session.Session playerSession, bool skipChecks = true)
        {
            if (playerSession == null || !skipChecks && !IsPropertyTenant(playerSession.CharId)) return;

            Log.Debug($"Syncing property {PropertyId} for {playerSession.PlayerName}");

            syncMarkers(playerSession);

            if (playerSession.PropertyCurrentlyInside != null && playerSession.PropertyCurrentlyInside == this)
            {
                syncStorage(playerSession);
            }
        }

        public void ResyncPropertyForPlayer(int charId, bool skipChecks = true) => ResyncPropertyForPlayer(Server.Instance.Get<SessionManager>().GetPlayerByCharID(charId), skipChecks);


        public void DesyncPropertyForPlayer(Session.Session playerSession, bool skipChecks = true)
        {
            if(playerSession == null ||!skipChecks && !IsPropertyTenant(playerSession.CharId)) return;

            Log.Debug($"Desync property {PropertyId} for {playerSession.PlayerName}");

            desyncProperty(playerSession);
        }

        public void DesyncPropertyForPlayer(int charId, bool skipChecks = true) => DesyncPropertyForPlayer(Server.Instance.Get<SessionManager>().GetPlayerByCharID(charId), skipChecks);

        public void OnEnterProperty(Session.Session session)
        {
            syncStorage(session);
        }

        public void OnExitProperty(Session.Session session)
        {
            desyncStorage(session);
        }

        private void syncMarkers(Session.Session session)
        {
            if (this.EntranceLocation != default)
            {
                session.AddMarker(this.EntranceLocation, new MarkerOptions
                {
                    ColorArray = ConstantColours.Yellow.ToArray(),
                    MarkerId = $"property:entrance:{PropertyId}"
                });
            }

            if(IsPropertyTenant(session.CharId)) session.AddGarage(getPropertyGarage()); // only sync garage if they are an actual tenant
        }

        private void desyncMarkers(Session.Session session)
        {
            if (this.EntranceLocation != default)
            {
                session.RemoveMarker($"property:entrance:{PropertyId}");
            }
        }

        private void desyncProperty(Session.Session session)
        {
            desyncMarkers(session);
            desyncStorage(session);

            session.RemoveGarage(getPropertyGarage());
        }

        private GarageModel getPropertyGarage()
        {
            if (cachedGarageModel != null && !IsBeingEdited()) return cachedGarageModel; // Only return cached data after creation

            if (GarageLocaiton != default && !string.IsNullOrEmpty(Address))
            {
                cachedGarageModel = new GarageModel
                {
                    Name = $"home-{PropertyId}",
                    AlternateDisplayName = Address,
                    Location = GarageLocaiton,
                    MaxVehicles = GarageMaxVehicles,
                    BlipOptions = new BlipOptions
                    {
                        Sprite = 357
                    },
                    MarkerOptions = new MarkerOptions
                    {
                        ColorArray = Color.FromArgb(255, 69, 0).ToArray(),
                        ScaleFloat = 1.5f
                    }
                };

                return cachedGarageModel;
            }

            return null;
        }

        private void syncStorage(Session.Session session)
        {
            if (StorageLocation != default)
            {
                Log.Verbose($"Syncing storage for {session.PlayerName} for property {PropertyId}");

                var locs = session.AccessablePropertyStorages;

                if (!locs.Contains(StorageLocation))
                {
                    locs.Add(StorageLocation);
                    session.AccessablePropertyStorages = locs;
                }

                if(IsPropertyTenant(session.CharId))
                {
                    session.AddMarker(this.StorageLocation, new MarkerOptions
                    {
                        ColorArray = ConstantColours.Red.ToArray(), MarkerId = $"property:storage:{PropertyId}"
                    });
                }
            }
        }

        private void desyncStorage(Session.Session session)
        {
            if (this.StorageLocation != default)
            {
                Log.Verbose($"Desyncing storage for {session.PlayerName} for property {PropertyId}");

                var storages = session.AccessablePropertyStorages;
                storages.Remove(StorageLocation);
                session.AccessablePropertyStorages = storages;

                session.RemoveMarker($"property:storage:{PropertyId}");
            }
        }

        public bool Equals(PropertyModel property)
        {
            return !ReferenceEquals(property, null) && PropertyId == property.PropertyId;
        }
        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((PropertyModel)obj);
        }

        public static bool operator ==(PropertyModel left, PropertyModel right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(PropertyModel left, PropertyModel right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            if(Dev.DevEnviroment.IsDebugInstance)
            {
                var propertyData = "";

                FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    propertyData += $"{field.Name}: {field.GetValue(this)}\n";
                }

                return propertyData;
            }
            return $"Property: {Address} ({PropertyId})";
        }
    }
}
