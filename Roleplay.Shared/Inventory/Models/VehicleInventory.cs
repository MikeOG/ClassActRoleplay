using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using static Roleplay.Shared.InventoryItems;
#if SERVER
using Roleplay.Server.Players;
using Vehicle = Roleplay.Server.Vehicle.Models.Vehicle;
#else
using Roleplay.Client.Session;
#endif



namespace Roleplay.Shared.Models
{
    public sealed class VehicleInventory : Inventory
    {
#if SERVER
        private Vehicle inventoryOwner;

        public VehicleInventory(string invString, Vehicle inventoryOwner)
            : base(invString, inventoryOwner)
        {
            Log.Verbose($"Loaded an inv string of {invString} for vehicle {inventoryOwner.Plate}");
            this.inventoryOwner = inventoryOwner;
        }

        public override void AddItem(inventoryItem item, int amount, Player userSource = null, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            var sourcePlayer = userSource == null ? inventoryOwner.VehicleOwner.Source : userSource;
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
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"put {amount} {invItem.itemName} in vehicle {inventoryOwner.Plate}");
            }
            else
            {
                //BaseScript.TriggerEvent("Log.ToDbType", "inventory", $"took {amount * -1} {invItem.itemName} in vehicle {inventoryOwner.Plate}", Convert.ToInt32(sourcePlayer.Handle));
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"took {amount * -1} {invItem.itemName} from vehicle {inventoryOwner.Plate}");
            }

            sourcePlayer.TriggerEvent(/*"Inventory.RefreshVehicleInventory"*/"Inventory.UI.UpdateMenu", "VehicleInventory", this.ToString());
        }

        public override bool CanStoreItem(string itemKey, int itemAmount)
        {
            try
            {
                var currentInvWeight = GetCurrentInventoryWeight();
                var maxCarryCapacity = -1;
                var item = GetInvItemData(itemKey);
                if (item == null)
                    return false; // Don't allow unregistered items to be stored via weight check (only through direct addition)

                if (item.itemCode.Contains("WEAPON_"))
                {
                    item = new inventoryItem(item)
                    {
                        itemWeight = 50.0f
                    };
                }

                var vehClass = inventoryOwner.Mods.Class;
                maxCarryCapacity = GetVehCarryCapacity(vehClass);

                return (currentInvWeight + (item.itemWeight * itemAmount)) <= maxCarryCapacity;

            }
            catch (Exception e) { Log.Error(e); }

            return false;
        }

        public override int GetCurrentInventoryWeight()
        {
            var weightValue = 0.0f;
            _inventoryItems.ForEach(o =>
            {
                weightValue += o.itemWeight * o.itemAmount;
                if (o.itemCode.Contains("WEAPON_"))
                    weightValue += 50.0f;
            });
            return (int)weightValue;
        }

        public override int GetMaxInventoryWeight()
        {
            var vehClass = inventoryOwner.Mods.Class;
            return GetVehCarryCapacity(vehClass);
        }

        public override void Save()
        {
            // Nothing
        }

        private int GetVehCarryCapacity(int vehClass)
        {
            var maxCapacity = 200;
            switch (vehClass)
            {
                case 0:
                case 1:
                case 3:
                case 9:
                    maxCapacity = 250;
                    break;
                case 2:
                case 11:
                    maxCapacity = 300;
                    break;
                case 4:
                case 5:
                    maxCapacity = 200;
                    break;
                case 6:
                case 7:
                    maxCapacity = 160;
                    break;
                case 8:
                    maxCapacity = 50;
                    break;
                case 13:
                    maxCapacity = 0;
                    break;
                case 12:
                    maxCapacity = 450;
                    break;
                case 20:
                    maxCapacity = 550;
                    break;
            }

            return maxCapacity;
        }
#else
        public VehicleInventory(string invString, Session inventoryOwner)
            : base(invString, inventoryOwner) { }
#endif
    }
}
