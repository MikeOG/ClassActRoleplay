using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Helpers;
using static Roleplay.Shared.InventoryItems;
#if SERVER
using System.Runtime.CompilerServices;
using Roleplay.Server.Session;
#else
using Roleplay.Client.Session;
#endif

namespace Roleplay.Shared.Models
{
    public sealed class PlayerInventory : Inventory
    {
#if SERVER
        private Session inventoryOwner;

        public PlayerInventory(string invString, Session inventoryOwner) :
            base(invString, inventoryOwner)
        {
            this.inventoryOwner = inventoryOwner;
        }

        public override void AddItem(inventoryItem item, int amount, Player userSource = null, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            var sourcePlayer = userSource == null ? inventoryOwner.Source : userSource;
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

            if (invItem.itemCode.Contains("WEAPON_") && invItem.itemAmount >= 0)
                invItem.itemAmount = 1;

            if (amount >= 0)
            {
                //BaseScript.TriggerEvent("Log.ToDbType", "inventory", $"obtained {amount} {invItem.itemName}", Convert.ToInt32(sourcePlayer.Handle));
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"obtained {amount} {invItem.itemName}");
                //Log.Debug($"[{Log.getDateString(DateTime.Now)}] " +
                    //$"{sourcePlayer.Name} (steam:{sourcePlayer.Identifiers[Roleplay.Server.Server.CurrentIdentifier]}) obtained {amount} {invItem.itemName} // Called via: {callingMember} in {callingLocation} on line {lineNumber}");
            }
            else
            {
                //BaseScript.TriggerEvent("Log.ToDbType", "inventory", $"disposed of {amount * -1} {invItem.itemName}", Convert.ToInt32(sourcePlayer.Handle));
                Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", sourcePlayer.Name, sourcePlayer.Identifiers[Server.Server.CurrentIdentifier], "inventory", $"disposed of {amount * -1} {invItem.itemName}");
                //Log.Debug($"[{Log.getDateString(DateTime.Now)}] " +
                 //           $"{sourcePlayer.Name} (steam:{sourcePlayer.Identifiers[Roleplay.Server.Server.CurrentIdentifier]}) disposed of {amount * -1} {invItem.itemName} // Called via: {callingMember} in {callingLocation} on line {lineNumber}");
            }

            //inventoryOwner.SetGlobalData("Character.Inventory", this.ToString());
            //inventoryOwner.TriggerEvent("Inventory.RefreshPlayerInventory");
            this.Save();
            inventoryOwner.TriggerEvent("Inventory.UI.UpdateMenu", "PlayerInventory", this.ToString());
            if (invItem.itemCode.Contains("WEAPON_") || invItem.itemCode.Contains("ammo"))
                inventoryOwner.TriggerEvent("Weapons.LoadWeapons");
        }

        /// <summary>
        /// Uses the specified <see cref="inventoryItem"/> if available
        /// </summary>
        /// <param name="itemKey">Item name or item code of the <see cref="inventoryItem"/> wanting to be used</param>
        /// <param name="itemAmount">Amount of the item wanting to be used</param>
        public void UseItem(string itemKey, int itemAmount)
        {
            var inventorySource = inventoryOwner.Source;
            if (HasItem(itemKey))
            {
                var item = GetItem(itemKey);
                if (item.itemAmount >= itemAmount)
                {
                    if (item.isUseable)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await BaseScript.Delay(0);
                            for (var i = 0; i < itemAmount; i++)
                            {
                                Log.ToClient("[Inventory]", $"You {item.interactionAction} {item.itemName.ToLower()}", ConstantColours.Inventory, inventorySource);
                                inventorySource.TriggerEvent(item.interactionEvent, item.interactionData);
                                if(item.removeItemOnUse)
                                {
                                    AddItem(item, -1);
                                }
                                await BaseScript.Delay(2500);
                            }
                        });
                    }
                    else
                    {
                        Log.ToClient("[Inventory]", $"You are not able to use this item", ConstantColours.Inventory, inventorySource);
                    }
                }
                else
                {
                    Log.ToClient("[Inventory]", $"You do not have {itemAmount} {item.itemName} to use", ConstantColours.Inventory, inventorySource);
                }
            }
            else
            {
                Log.ToClient("[Inventory]", $"You do not any {GetInvItemData(itemKey).itemName} to use", ConstantColours.Inventory, inventorySource);
            }

            inventorySource.TriggerEvent("Inventory.RefreshPlayerInventory");
        }

        public override bool CanStoreItem(string itemKey, int itemAmount)
        {
            try
            {
                var currentInvWeight = GetCurrentInventoryWeight();
                var maxCarryCapacity = GetPlayerCarryCapacity();
                var item = GetInvItemData(itemKey);
                if (item == null)
                    return false; // Don't allow unregistered items to be stored via weight check (only through direct addition)

                if (itemKey.Contains("WEAPON_"))
                    itemAmount = 0;

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
                if(!o.itemName.Contains("WEAPON_"))
                    weightValue += o.itemWeight * o.itemAmount;
            });
            return (int)weightValue;
        }

        public override int GetMaxInventoryWeight()
        {
            return GetPlayerCarryCapacity();
        }

        public override void Save()
        {
            inventoryOwner.SetGlobalData("Character.Inventory", GetInvString());
        }

        private int GetPlayerCarryCapacity()
        {
            return 70;
        }
#else
        public PlayerInventory(string invString, Session inventoryOwner)
            : base(invString, inventoryOwner) { }

        /// <summary>
        /// Uses the specified <see cref="inventoryItem"/> if available
        /// </summary>
        /// <param name="itemKey">Item name or item code of the <see cref="inventoryItem"/> wanting to be used</param>
        /// <param name="itemAmount">Amount of the item wanting to be used</param>
        public void UseItem(string itemKey, int itemAmount) => Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.UseInvItem", itemKey, itemAmount);
#endif
    }
}
