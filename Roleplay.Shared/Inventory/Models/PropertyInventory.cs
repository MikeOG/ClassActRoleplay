#if CLIENT
using Roleplay.Client.Session;
#elif SERVER
using Roleplay.Server.Session;
using Roleplay.Server.Realtor.Models;
#endif
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CitizenFX.Core;

namespace Roleplay.Shared.Models
{
    public sealed class PropertyInventory : Inventory
    {

#if SERVER
        private PropertyModel inventoryOwner;

        public PropertyInventory(string invString, PropertyModel inventoryOwner)
            : base(invString, inventoryOwner)
        {
            this.inventoryOwner = inventoryOwner;
        }

        public override void AddItem(inventoryItem item, int amount, Player userSource = null, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            var sourcePlayer = userSource == null ? Server.Server.Instance.Get<SessionManager>().GetPlayer(inventoryOwner.CurrentPropertyEditor).Source : userSource;
            var invItem = GetItem(item) ?? AddItem(item);

            Log.Debug($"Item name: {item.itemName}");
            Log.Debug($"Current item amount: {invItem.itemAmount}");

            invItem.itemAmount += amount;
            if (invItem.itemAmount <= 0)
            {
                _inventoryItems.Remove(invItem);
                Log.Verbose($"Removing {invItem.itemName} from this inventory (source player is {sourcePlayer.Name}) as it currently has less than 0 in it");
            }

            Log.Debug($"New item amount: {invItem.itemAmount}");

            if (amount >= 0)
            {
                //Log.Debug($"[{Log.getDateString(DateTime.Now)}] " +
                //$"{sourcePlayer.Name} (steam:{sourcePlayer.Identifiers[Server.CurrentIdentifier]}) put {amount} {invItem.itemName} in vehicle {inventoryOwner.vehiclePlate} // Called via: {callingMember} in {callingLocation} on line {lineNumber}");
                //BaseScript.TriggerEvent("Log.ToDbType", "inventory", $"put {amount} {invItem.itemName} in vehicle {inventoryOwner.Plate}", Convert.ToInt32(sourcePlayer.Handle));
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"put {amount} {invItem.itemName} in property {inventoryOwner.PropertyId}");
            }
            else
            {
                //BaseScript.TriggerEvent("Log.ToDbType", "inventory", $"took {amount * -1} {invItem.itemName} in vehicle {inventoryOwner.Plate}", Convert.ToInt32(sourcePlayer.Handle));
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"took {amount * -1} {invItem.itemName} from property {inventoryOwner.PropertyId}");
            }

            sourcePlayer.TriggerEvent("Inventory.RefreshPropertyInventory", this.ToString());
        }

        public override bool CanStoreItem(string itemKey, int itemAmount)
        {
            try
            {
                var currentInvWeight = GetCurrentInventoryWeight();
                var maxCarryCapacity = GetMaxInventoryWeight();
                var item = Shared.InventoryItems.GetInvItemData(itemKey);
                if (item == null)
                    return false;

                if (item.itemCode.Contains("WEAPON_"))
                {
                    item = new inventoryItem(item)
                    {
                        itemWeight = 50.0f
                    };
                }

                return currentInvWeight + item.itemWeight * itemAmount <= maxCarryCapacity;

            }
            catch (Exception e) { Log.Error(e); }

            return false;
        }

        public override int GetCurrentInventoryWeight()
        {
            return (int)InventoryItems.Select(o => o.itemCode.Contains("WEAPON_") ? 50.0f : o.itemWeight * o.itemAmount).Sum();
        }

        public override int GetMaxInventoryWeight()
        {
            return inventoryOwner.StorageSize;
        }

        public override void Save()
        {
            // not needed
        }
#elif CLIENT
        public PropertyInventory(string invString, Session inventoryOwner)
            : base(invString, inventoryOwner)
        {

        }
#endif
    }
}