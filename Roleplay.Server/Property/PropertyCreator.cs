using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Bank;
using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Models;
using Roleplay.Server.Realtor.Models;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Realtor
{
    public class PropertyCreator : ServerAccessor
    {
        public PropertyCreator(Server server) : base(server)
        {

        }

        public void OnPlayerDisconnect(Session.Session session, string reason)
        {
            if (session.GetServerData("Property.Creator.IsCreating", false))
            {
                Log.Verbose($"{session.PlayerName} was in property creation and disconnected. Removing property");

                var property = GetCurrentCreatorProperty(session);

                property.DesyncProperty();
                Server.Get<PropertyManager>().RemoveProperty(property);
            }
        }

        [ServerCommand("realtor_help", JobType.Realtor, true)]
        private void OnShowRealtorHelp(Command cmd)
        {
            cmd.Session.Message("[Realtor]", $"Available realtor commands", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor create|start [characterID] [price]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setentrance|placeentrance", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setgarage|placegarage", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor garagesize|setgaragesize [garagesize]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setstorage|placestorage", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setstoragesize|storagesize [storagesize]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setaddress|address [address]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setprice|setpropertyprice [price]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor setinterior|setinteriorid [interiorID]", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor save|complete", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor cancel|cancelcreation|stop|stopcreation", ConstantColours.Green);
            cmd.Session.Message("[Realtor]", $"/realtor delete|deleteproperty", ConstantColours.Green);
        }

        [ServerCommand("realtor_create|start", JobType.Realtor, true)]
        private async void OnStartPropertyCreation(Command cmd)
        {
            if (!Settings.PropetyCreationEnabled)
            {
                cmd.Session.Message("[Realtor]", $"Property creation is currently disabled", ConstantColours.Green);
                return;
            }

            var session = cmd.Session;
            var targetCharacter = cmd.GetArgAs(0, -1);
            //var garageSize = cmd.GetArgAs(1, 0); 
            var propertyPrice = cmd.GetArgAs(1, 0);

            /*if (garageSize == 0)
            {
                session.Message("[Realtor]", $"Please set the garage size of this property", ConstantColours.Green);
                return;
            }*/

            if (propertyPrice <= 0)
            {
                session.Message("[Realtor]", $"Please set the price of the property", ConstantColours.Green);
                return;
            }

            if (Sessions.GetPlayerByCharID(targetCharacter) == null && targetCharacter != -1)
            {
                session.Message("[Realtor]", $"Invalid character ID entered (character ID is just someones phone number!)", ConstantColours.Green);
                return;
            }

            if (GetCurrentCreatorProperty(session) == null)
            {
                var propertyId = await Server.Get<PropertyManager>().GeneratePropertyId();

                if (targetCharacter == -1)
                {
                    Log.Debug($"Starting creation of a purchasable property creation (ID: {propertyId})");
                }
                else
                {
                    Log.Debug($"Starting property creation (ID: {propertyId}) for {session.PlayerName} for target character ID ({targetCharacter})");
                }

                var property = new PropertyModel
                {
                    OwnerCharacterId = targetCharacter,
                    CurrentPropertyEditor = session.ServerID,
                    PropertyId = propertyId,
                    IsLocked = false
                };
                property.FinanceData.PropertyPrice = propertyPrice;

                session.SetServerData("Property.Creator.CurrentProperty", property);
                session.SetServerData("Property.Creator.IsCreating", true);

                Server.Get<PropertyManager>().AddProperty(property);

                session.Message("[Realtor]", $"Started property creation", ConstantColours.Green);
            }
            else
            {
                session.Message("[Realtor]", $"You are already creating a property. Please do /realtor save or /realtor cancel to make a new property", ConstantColours.Green);
            }
        }

        [ServerCommand("realtor_cancel|cancelcreation|stop|stopcreation", JobType.Realtor, true)]
        private void OnCancelCreation(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            Server.Get<PropertyManager>().RemoveProperty(property);
            cmd.Session.SetServerData("Property.Creator.CurrentProperty", null);

            cmd.Session.Message("[Realtor]", $"Cancelled creation of this property", ConstantColours.Green);
        }

        [ServerCommand("realtor_edit", JobType.Realtor, true)]
        private void OnEditProperty(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property != null)
            {
                cmd.Session.Message("[Realtor]", $"You are already editing a property", ConstantColours.Green);
                return;
            }

            var closeProperty = Server.Get<PropertyManager>().GetClosestProperty(cmd.Session);

            if (closeProperty == null)
            {
                cmd.Session.Message("[Realtor]", $"There is no property nearby to edit", ConstantColours.Green);
                return;
            }

            Log.Verbose($"{cmd.Session.PlayerName} is about to edit property {closeProperty.Address} ({closeProperty.PropertyId})");

            cmd.Session.SetServerData("Property.Creator.CurrentProperty", closeProperty);
            cmd.Session.SetServerData("Property.Creator.IsEditing", true);
            cmd.Session.Message("[Realtor]", $"Started editing {closeProperty.Address} ({closeProperty.PropertyId})", ConstantColours.Green);
        }

        [ServerCommand("realtor_setentrance|placeentrance", JobType.Realtor, true)]
        private void OnPlaceEntrance(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var playerPos = cmd.Session.Position;

            Log.Debug($"Setting entrance location of property being created by {cmd.Session.PlayerName}");

            playerPos.Z -= 0.85f;
            property.EntranceLocation = playerPos;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Placed entrance for this property at {playerPos}", ConstantColours.Green);
        }

        [ServerCommand("realtor_setgarage|placegarage", JobType.Realtor, true)]
        private void OnPlaceGarage(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var playerPos = cmd.Session.Position;

            Log.Debug($"Setting garage location of property being created by {cmd.Session.PlayerName}");

            playerPos.Z -= 0.85f;
            property.GarageLocaiton = playerPos;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Placed garage for this property at {playerPos}", ConstantColours.Green);
        }

        [ServerCommand("realtor_garagesize|setgaragesize", JobType.Realtor, true)]
        private void OnSetGarageSize(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var garageSize = cmd.GetArgAs(0, 1);

            Log.Debug($"Setting size of garage to {garageSize} for the property being created by {cmd.Session.PlayerName}");

            property.GarageMaxVehicles = garageSize;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Set the size of this properties garage to {garageSize}", ConstantColours.Green);
        }

        [ServerCommand("realtor_setstorage|placestorage", JobType.Realtor, true)]
        private void OnSetStorageLocation(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var playerPos = cmd.Session.Position;

            Log.Debug($"Setting storage location of property being created by {cmd.Session.PlayerName}");

            playerPos.Z -= 0.85f;
            property.StorageLocation = playerPos;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Placed storage for this property at {playerPos}", ConstantColours.Green);
        }

        [ServerCommand("realtor_setstoragesize|storagesize", JobType.Realtor, true)]
        private void OnSetStorageSize(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var storageSize = cmd.GetArgAs(0, 200);

            Log.Debug($"Setting storage size of property being created by {cmd.Session.PlayerName} to {storageSize}");

            property.StorageSize = storageSize;

            cmd.Session.Message("[Realtor]", $"Set storage size for this property to {storageSize}", ConstantColours.Green);
        }

        [ServerCommand("realtor_setaddress|address", JobType.Realtor, true)]
        private void OnSetPropertyAddress(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var addressName = string.Join(" ", cmd.Args);

            Log.Debug($"Setting address name to {addressName} for the property being created by {cmd.Session.PlayerName}");

            property.Address = addressName;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Set address of the property to {addressName}");
        }

        [ServerCommand("realtor_setinterior|setinteriorid", JobType.Realtor, true)]
        private void OnSetInteriorId(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            var targetInteriorId = cmd.GetArgAs(0, 1) - 1;

            if (!(targetInteriorId > -1 && targetInteriorId <= PropertyManager.HouseInteriorLocations.Count)) // -1 since 0 is an index of the list
            {
                cmd.Session.Message("[Realtor]", $"The interior ID must be between 1 and {PropertyManager.HouseInteriorLocations.Count}", ConstantColours.Green);
                return;
            }

            if (property.AnyPlayerInside())
            {
                cmd.Session.Message("[Realtor]", $"There must be nobody inside the property to change interior", ConstantColours.Green);
                return;
            }

            Log.Debug($"Setting property id to {targetInteriorId} for the property being created by {cmd.Session.PlayerName}");

            property.InteriorId = targetInteriorId;
            property.ResyncProperty();

            cmd.Session.Message("[Realtor]", $"Set interior id of this property to {targetInteriorId + 1}", ConstantColours.Green);
        }

        [ServerCommand("realtor_save|complete", JobType.Realtor, true)]
        private void OnSaveProperty(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            if(cmd.Session.GetServerData("Property.Creator.IsEditing", false))
            {
                cmd.Session.SetServerData("Property.Creator.IsEditing", false);
                cmd.Session.SetServerData("Property.Creator.CurrentProperty", null);

                cmd.Session.Message("[Realtor]", $"Saved edited property", ConstantColours.Green);

                return;
            }

            Log.Verbose($"{cmd.Session.PlayerName} is attempting to save property {property.PropertyId}");
            if(property.IsPropertyCreated())
            {
                if (property.OwnerCharacterId != -1) // attempt payment
                {
                    var payHandler = Server.Get<PaymentHandler>();
                    var ownerSession = Sessions.GetPlayerByCharID(property.OwnerCharacterId);
                    if (ownerSession == null) return;
                    Log.Verbose($"Attempting to process down payment for property {property.PropertyId} for player {ownerSession.PlayerName}");

                    var downPayment = (int)Math.Ceiling(property.FinanceData.PropertyPrice * Settings.PropertyRequiredDownPaymentPercent);
                    var realtorCut = (int)Math.Ceiling(property.FinanceData.PropertyPrice * Settings.PropertyDownPaymentPercentForRealtor);
                    Log.Debug($"Property {property.PropertyId} down payment price is ${downPayment}");

                    if (!payHandler.CanPayForItem(ownerSession, downPayment, paymentTypeOverride: (int)PaymentType.Debit))
                    {
                        Log.Debug($"{ownerSession.PlayerName} is not able to pay for property {property.PropertyId} not continuing with saving");
                        cmd.Session.Message("[Realtor]", $"Payment unsuccessful", ConstantColours.Green);

                        return;
                    }

                    property.FinanceData.TotalAmountPaid = downPayment;

                    ownerSession.Message("[Realtor]", $"You just paid a ${downPayment} down payment for {property.Address}", ConstantColours.Green);
                    cmd.Session.Message("[Realtor]", $"Payment successful. You recieved ${realtorCut} for this sale", ConstantColours.Green);
                    payHandler.PayForItem(ownerSession, downPayment, $"buying property {property.Address} ({property.PropertyId})", (int)PaymentType.Debit);
                    payHandler.UpdateBankBalance(cmd.Session.GetBankAccount(), realtorCut, cmd.Session, $"realtor cut for selling property {property.Address} ({property.PropertyId})");
                }

                cmd.Session.SetServerData("Property.Creator.CurrentProperty", null);
                cmd.Session.SetServerData("Property.Creator.IsCreating", false);
                property.DesyncPropertyForPlayer(cmd.Session);
                property.CurrentPropertyEditor = -1;

                property.ResyncProperty();
                Server.Instance.Get<PropertyManager>().RegisterPropertyToDatabase(property);

                cmd.Session.Message("[Realtor]", $"Property successfully registered", ConstantColours.Green);
            }
        }

        [ServerCommand("realtor_setprice|setpropertyprice", JobType.Realtor, true)]
        private void OnSetPropertyPrice(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            var price = cmd.GetArgAs(0, 0);

            if (property == null || price == 0) return;

            cmd.Session.Message("[Realtor]", $"Set price of this property to ${price}", ConstantColours.Green);

            property.FinanceData.PropertyPrice = price;
        }

        [ServerCommand("realtor_display", JobType.Realtor, true)]
        private void OnDisplayProperty(Command cmd)
        {
            var property = GetCurrentCreatorProperty(cmd.Session);
            if (property == null) return;

            cmd.Session.Message("[Realtor]", $"Data for current property:\n{property}", ConstantColours.Green);
        }

        [ServerCommand("realtor_delete|deleteproperty", JobType.Realtor, true)]
        private void OnDeleteProperty(Command cmd)
        {
            var propertyToDelete = cmd.Session.GetServerData<PropertyModel>("Property.Creator.PropertyToDelete");

            if (propertyToDelete != null)
            {
                var confirmText = cmd.GetArgAs(0, "cancel").ToLower();

                if (confirmText == "confirm")
                {
                    cmd.Session.Message("[Realtor]", $"Deleting property {propertyToDelete.Address} (ID: {propertyToDelete.PropertyId})", ConstantColours.Green);
                    Server.Get<PropertyManager>().DeleteProperty(propertyToDelete);
                }
                else
                {
                    cmd.Session.Message("[Realtor]", $"Deletion cancelled", ConstantColours.Green);
                    cmd.Session.SetServerData("Property.Creator.PropertyToDelete", null);
                }

                return;
            }

            var closeProperty = Server.Get<PropertyManager>().Properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(cmd.Session.Position) < 12.0f);

            if (closeProperty == null)
            {
                cmd.Session.Message("[Realtor]", $"You are not close enough to a property to do this", ConstantColours.Green);
                return;
            }

            if (closeProperty.IsBeingEdited())
            {
                cmd.Session.Message("[Realtor]", $"This property is currently being edited and can't be deleted", ConstantColours.Green);
                return;
            }

            if (closeProperty.AnyPlayerInside())
            {
                cmd.Session.Message("[Realtor]", $"There is currently people inside this property so deletion can't continue", ConstantColours.Green);
                return;
            }

            cmd.Session.SetServerData("Property.Creator.PropertyToDelete", closeProperty);
            cmd.Session.Message("[Realtor]", $"You are wanting to delete {closeProperty.Address}. This will remove all traces of this property and is an irreversible action. If still want to continue do /realtor delete confirm otherwise do /realtor delete cancel", ConstantColours.Green);
        }

        [ServerCommand("realtor_show|showproperties", JobType.Realtor, true)]
        private void OnShowPropetiesCommand(Command cmd)
        {
            var charId = cmd.Session.CharId;
            if (cmd.Session.GetServerData("Realtor.IsShowingProperties", false))
            {
                foreach (var property in Server.Get<PropertyManager>().Properties)
                {
                    if(property.IsPropertyTenant(charId) || property.IsPropertyOwner(charId)) continue;

                    property.DesyncPropertyForPlayer(cmd.Session);
                }

                cmd.Session.SetServerData("Realtor.IsShowingProperties", false);
            }
            else
            {
                foreach (var property in Server.Get<PropertyManager>().Properties)
                {
                    if (property.IsPropertyTenant(charId) || property.IsPropertyOwner(charId)) continue;

                    property.ResyncPropertyForPlayer(cmd.Session);
                }

                cmd.Session.SetServerData("Realtor.IsShowingProperties", true);
            }
        }

        private PropertyModel GetCurrentCreatorProperty(Session.Session session) => session.GetServerData<PropertyModel>("Property.Creator.CurrentProperty");
    }
}
