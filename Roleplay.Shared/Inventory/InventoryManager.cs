using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CitizenFX.Core;
using static Roleplay.Shared.InventoryItems;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
#if SERVER
using Roleplay.Server;
using Roleplay.Server.Vehicle;
using Roleplay.Server.Players;
using Roleplay.Server.Session;
#elif CLIENT
using Roleplay.Client;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
#endif


namespace Roleplay.Shared
{
    internal class InventoryManager : 
        #if SERVER
            ServerAccessor
        #elif CLIENT
            ClientAccessor
        #endif
    {
#if SERVER
        private VehicleManager vehManager => Server.Instances.Vehicles;

        public InventoryManager(Server.Server server) : base(server)
        {
            server.RegisterEventHandler("Inventory.AddInvItem", new Action<Player, string, int, string>(AddInvItem));
            server.RegisterEventHandler("Inventory.RequestInventory", new Action<Player, string>(RequestInventory));
            server.RegisterEventHandler("Inventory.UseInvItem", new Action<Player, string, int>(UseInvItem));
            server.RegisterEventHandler("Inventory.TakeVehInvItem", new Action<Player, string, int, string>(TakeVehInvItem));
            server.RegisterEventHandler("Inventory.GetInvStats", new Action<Player, string>(GetInvStats));
            CommandRegister.RegisterCommand("use", cmd =>
            {
                var item = string.Join(" ", cmd.Args).ToLower();
                var playerSession = Server.Instances.Session.GetPlayer(cmd.Player);
                var invItem = inventoryItems.FirstOrDefault(o => o.Key.ToLower().Equals(item) || o.Value.itemName.ToLower().Equals(item) || o.Value.itemCode.ToLower().Equals(item));
                if(invItem.Key == null) 
                    invItem = inventoryItems.FirstOrDefault(o => o.Key.ToLower().Contains(item) || o.Value.itemName.ToLower().Contains(item) || o.Value.itemCode.ToLower().Contains(item));

                if(invItem.Key != null)
                {
                    PlayerInventory playerInv = GetPlayerInventory(playerSession);
                    if (playerInv.HasItem(invItem.Value.itemCode))
                    {
                        playerInv.UseItem(invItem.Value.itemCode, 1);
                    }
                    else
                    {
                        Log.ToClient("[Inventory]", $"You don't have any {invItem.Value.itemName}(s) to use", ConstantColours.Inventory, cmd.Player);
                    }
                }
            });
        }

        private void AddInvItem([FromSource] Player source, string itemData, int itemAmount, string altInventory = null)
        {
            // TODO make this less bad
            var playerSession = Sessions.GetPlayer(source);
            var charInv = playerSession.Inventory as PlayerInventory;
            var item = JsonConvert.DeserializeObject<inventoryItem>(itemData);
            item.itemAmount = 0;

            Log.Debug($"Doing an AddInvItem for {item.itemName} of amount {itemAmount} (is for alternate inventory {altInventory != null})");
            if (altInventory == null)
            {
                charInv.AddItem(item, itemAmount); // We assume a random player can't gain control of someone elses inventory (as it is next to impossible to do this) so no validation
            }
            else // Is other inventory
            {
                if (!altInventory.Contains("property"))
                {
                    var targetVeh = vehManager.GetVehicle(int.Parse(altInventory));
                    var isStoringInVeh = itemAmount > 0;
                    Log.Debug($"isStoringInVeh: {isStoringInVeh}");
                    if (targetVeh != null && (isStoringInVeh && targetVeh.Inventory.CanStoreItem(item.itemCode, itemAmount) || !isStoringInVeh && targetVeh.Inventory.HasItemWithAmount(item, itemAmount * -1) && charInv.CanStoreItem(item.itemCode, itemAmount * -1)))
                    {
                        targetVeh.Inventory.AddItem(item, itemAmount, source);
                        charInv.AddItem(item, itemAmount * -1);
                    }
                    else
                    {
                        Log.ToClient("[Inventory]", $"{(isStoringInVeh ? "This vehicle" : "You")} cannot hold {(isStoringInVeh ? itemAmount : itemAmount * -1)} {item.itemName}(s)", ConstantColours.Inventory, source);
                        source.TriggerEvent("Inventory.UI.UpdateMenu", isStoringInVeh ? "PlayerInventory" : "VehicleInventory", isStoringInVeh ? charInv.ToString() : targetVeh?.Inventory.ToString());
                    }
                }
                else
                {
                    Log.Verbose($"{source.Name} is attempting to do something to a storage property");

                    var targetProperty = playerSession.PropertyCurrentlyInside; // if they are editing a storage property they must be inside one... right?

                    if (targetProperty == null) return;

                    var storingInProperty = itemAmount > 0;
                    Log.Debug($"storingInProperty: {storingInProperty}");

                    if (storingInProperty && charInv.HasItemWithAmount(item, itemAmount * -1))
                    {
                        targetProperty.StorageInventory.AddItem(item, itemAmount, source);
                        charInv.AddItem(item, itemAmount * -1);

                        source.TriggerEvent("Inventory.UI.UpdateMenu", "PlayerInventory", charInv.ToString());
                    }

                    if (!storingInProperty && targetProperty.StorageInventory.HasItemWithAmount(item, itemAmount * -1) && charInv.CanStoreItem(item.itemCode, itemAmount * -1))
                    {
                        targetProperty.StorageInventory.AddItem(item, itemAmount, source);
                        charInv.AddItem(item, itemAmount * -1);

                        source.TriggerEvent("Inventory.UI.UpdateMenu", "PropertyInventory", targetProperty.StorageInventory.ToString());
                    }
                }
            }
        }

        private void TakeVehInvItem([FromSource] Player source, string itemName, int itemAmount, string vehiclePlate)
        {
            AddInvItem(source, itemName, itemAmount * -1, vehiclePlate);
        }

        private void RequestInventory([FromSource] Player source, string altInventory = null)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            Log.Verbose($"{source.Name} is requesting an inventory ({altInventory})");

            if(altInventory == null)
            {
                source.TriggerEvent("Inventory.UI.UpdateMenu", "PlayerInventory", playerSession.Inventory.ToString());
            }
            else
            {
                if (!altInventory.Contains("property"))
                {
                    var veh = vehManager.GetVehicle(int.Parse(altInventory));
                    if (veh != null)
                    {
                        source.TriggerEvent("Inventory.UI.UpdateMenu", "VehicleInventory", veh.Inventory.ToString());
                    }
                    else
                    {
                        source.TriggerEvent("Inventory.UI.UpdateMenu", "VehicleInventory", "");
                    }
                }
                else
                {
                    var property = playerSession.PropertyCurrentlyInside;
                    if (property == null) return;

                    source.TriggerEvent("Inventory.UI.UpdateMenu", "PropertyInventory", property.StorageInventory.ToString());
                }
            }
        }

        private void UseInvItem([FromSource] Player source, string itemName, int itemAmount)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            GetPlayerInventory(playerSession).UseItem(itemName, itemAmount);
        }

        private void DropInvItem([FromSource] Player source, string itemData, int itemAmount)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            PlayerInventory playerInv = GetPlayerInventory(playerSession);
            var item = JsonConvert.DeserializeObject<inventoryItem>(itemData);

            if (playerInv.HasItemWithAmount(item, itemAmount))
            {
                playerInv.AddItem(item, -itemAmount);
            }
            else
            {
                Log.ToClient("[Inventory]", "You don't have enough of this item to do that", ConstantColours.Inventory, source);
            }
        }

        private void SetPlayerInv([FromSource] Player source, string invString)
        {
            //var playerObj = SessionManager.SessionInstance.GetClient(Convert.ToInt32(source.Handle));
            //playerObj.Character.Inventory = new PlayerInventory(invString, playerObj.Character);
        }

        private void GetInvStats([FromSource] Player source, string altInventory = null)
        {
            if (altInventory == null)
            {
                var playerSession = Server.Instances.Session.GetPlayer(source);
                var playerInv = GetPlayerInventory(playerSession);
                Log.ToClient("[Inventory]", $"You are currently carrying {playerInv.GetCurrentInventoryWeight()}/{playerInv.GetMaxInventoryWeight()} of your current weight capacity", ConstantColours.Inventory, source);
            }
            else
            {
                var targetVeh = vehManager.GetVehicle(int.Parse(altInventory));

                var maxWeight = targetVeh != null ? targetVeh.Inventory.GetMaxInventoryWeight().ToString() : "0";
                var currentWeight = targetVeh != null ? targetVeh.Inventory.GetCurrentInventoryWeight().ToString() : "0";

                Log.ToClient("[Inventory]", $"This vehicle is currently using {currentWeight}/{maxWeight} of its current weight capacity", ConstantColours.Inventory, source);
            }
        }
#elif CLIENT
        public InventoryManager(Client.Client client) : base(client)
        {
            client.RegisterEventHandler("Inventory.RecieveInventory", new Action<string, bool>(HandleReceiveInventory));
        }

        private void HandleReceiveInventory(string invString, bool isVehicle = false)
        {
            Log.Debug($"handleRecieveInventory {invString}\n{isVehicle}");
            if(isVehicle)
                Inventory.vehicleInventory = new VehicleInventory(invString, new Session(0, new SessionData()));
            else
                Inventory.playerInventory = new PlayerInventory(invString, new Session(0, new SessionData()));
        }
#endif

        public Inventory GetPlayerInventory(Session playerSession)
        {
            return new PlayerInventory(playerSession.GetGlobalData("Character.Inventory", ""), playerSession);
        }
    }

    public abstract class Inventory
    {
        /// <summary>
        /// List containing all items in an inventory
        /// </summary>
        protected List<inventoryItem> _inventoryItems = new List<inventoryItem>();
        public List<inventoryItem> InventoryItems => new List<inventoryItem>(_inventoryItems);
        private dynamic _inventoryOwner;

        public Inventory(string invString, dynamic inventoryOwner)
        {
            _inventoryOwner = inventoryOwner;
            var invItems = invString.Split('|').ToList();
            invItems.ForEach(o =>
            {
                var itemData = o.Split(',');
                if (itemData.Length == 3)
                {
                    var invItem = GetInvItemData(itemData[0]);
                    if (invItem != null)
                    {
                        var item = new inventoryItem(invItem)
                        {
                            itemAmount = Convert.ToInt32(itemData[1]),
                            metaData = itemData[2]
                        };
                        _inventoryItems.Add(item);
                    }
                    else
                    {
                        _inventoryItems.Add(new inventoryItem
                        {
                            itemName = itemData[0],
                            itemAmount = Convert.ToInt32(itemData[1]),
                            metaData = itemData[2]
                        });
                        Log.Warn($"The item {itemData[0]} was not found in the inventoryItems list. The item will still be added but consider actually adding it as an official item", "UnregisteredInventoryItem");
                    }
                }
            });
        }

        /// <summary>
        /// Gets a saveable and loadable string for this <see cref="Inventory"/>
        /// </summary>
        /// <returns></returns>
        public string GetInvString()
        {
            var invString = "";
            _inventoryItems.ForEach(o =>
            {
                var itemName = o.itemName;
                if (o.itemCode != "") itemName = o.itemCode;
                invString += $"{itemName},{o.itemAmount},{o.metaData}|";
            });

            return invString;

            //return JsonConvert.SerializeObject(_inventoryItems);
        }

        /// <summary>
        /// Checks if this <see cref="Inventory"/> contains the specified <see cref="inventoryItem"/>
        /// </summary>
        /// <param name="itemName">Item code or item name of the <see cref="inventoryItem"/></param>
        /// <returns><see cref="Boolean"/> value indicating if a player has the specified item or not</returns>
        public bool HasItem(string itemName) => _inventoryItems.Find(o => o.itemName.ToLower() == itemName.ToLower()) != null || _inventoryItems.Find(o => o.itemCode.ToLower() == itemName.ToLower()) != null;

        /// <summary>
        /// Checks if this <see cref="Inventory"/> contains the specified <see cref="inventoryItem"/>
        /// </summary>
        /// <param name="invItem"><see cref="inventoryItem"/> object</param>
        /// <returns><see cref="Boolean"/> value indicating if a player has the specified item or not</returns>
        public bool HasItem(inventoryItem invItem) => GetItem(invItem) != null;

        /// <summary>
        /// Checks if this <see cref="Inventory"/> contains an <see cref="inventoryItem"/> with the specified meta data
        /// </summary>
        /// <param name="metaData">Meta data that is wanting to be searched for</param>
        /// <returns><see cref="Boolean"/> value indicating if the <see cref="Inventory"/> has any <see cref="inventoryItem"/> with containing the meta data</returns>
        public bool HasItemWithData(string metaData) => _inventoryItems.Find(o => o.metaData.Contains(metaData)) != null;

        /// <summary>
        /// Gets a specified <see cref="inventoryItem"/> via its name
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public inventoryItem GetItem(inventoryItem item) => _inventoryItems.Find(o => o.metaData == item.metaData && o.itemName == item.itemName) 
                                                            ?? _inventoryItems.Find(o => o.metaData == item.metaData && o.itemCode == item.itemCode)
                                                            ?? _inventoryItems.Find(o => o.itemName == item.itemName) 
                                                            ?? _inventoryItems.FirstOrDefault(o => o.itemCode == item.itemCode);

        public inventoryItem GetItem(string itemName) => _inventoryItems.Find(o => o.itemName.ToLower() == itemName.ToLower()) ?? _inventoryItems.Find(o => o.itemCode.ToLower() == itemName.ToLower());

        public override string ToString() => GetInvString();
#if SERVER
        /// <summary>
        /// Checks if this <see cref="Inventory"/> has enough of the specified <see cref="inventoryItem"/>
        /// </summary>
        /// <param name="item">Specified <see cref="inventoryItem"/></param>
        /// <param name="itemAmount">Amount of the item</param>
        /// <returns></returns>
        public bool HasItemWithAmount(inventoryItem item, int itemAmount)
        {
            return _inventoryItems.Any(o => (o.itemCode == item.itemCode || o.itemName == item.itemName) && o.itemAmount >= itemAmount);
        }

        /// <summary>
        /// Checks if this <see cref="Inventory"/> has enough of the specified item key
        /// </summary>
        /// <param name="itemName">Name of the specified</param>
        /// <param name="itemAmount">Amount of the item</param>
        /// <returns></returns>
        public bool HasItemWithAmount(string itemName, int itemAmount)
        {
            var invItem = GetItem(itemName);

            if (invItem != null)
            {
                return invItem.itemAmount >= itemAmount;
            }

            return false;
        }
        public void AddItem(string item, int amount, Player userSource = null, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) => AddItem(GetItem(item) ?? AddItem(item), amount);

        /// <summary>
        /// Adds or removes a specified amount of an <see cref="inventoryItem"/> to this <see cref="Inventory"/>
        /// </summary>
        /// <param name="item">Item code or item name of the <see cref="inventoryItem"/> wanting to be added</param>
        /// <param name="amount">Amount of the <see cref="inventoryItem"/> wanting to be added</param>
        /// <param name="userSource">Optional <see cref="Player"/> parameter to be used if this <see cref="Inventory"/> is being used by the non-owning player</param>
        /// <param name="callingMember"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public abstract void AddItem(inventoryItem item, int amount, Player userSource = null, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0);

        public void RemoveItem(inventoryItem item) // TODO finish this
        {
            var invItem = GetItem(item);

            if (invItem != null)
            {
                _inventoryItems.Remove(item);

                Save();
            }
        }

        public void RemoveItem(string itemName)
        {
            var invItem = GetItem(itemName);

            if (invItem != null)
            {
                RemoveItem(invItem);
            }
        }

        internal inventoryItem AddItem(inventoryItem item)
        {
            _inventoryItems.Add(item);

            return item;
        }
        
        internal inventoryItem AddItem(string itemKey)
        {
            var invItem = GetInvItemData(itemKey);
            if (invItem != null)
            {
                _inventoryItems.Add(new inventoryItem(invItem));
            }
            else
            {
                _inventoryItems.Add(new inventoryItem
                {
                    itemName = itemKey,
                });
                Log.Warn($"The item {itemKey} was not found in the inventoryItems list. The item will still be added but consider actually adding it as an official item", "UnregisteredInventoryItem");
            }
            return _inventoryItems.Last();
        }
        
        /// <summary>
        /// Checks if this <see cref="Inventory"/> can store the specified amount of a certain <see cref="inventoryItem"/>
        /// </summary>
        /// <param name="itemKey">Item name or item code for the wanted <see cref="inventoryItem"/></param>
        /// <param name="itemAmount">Amount of the item wanting to be added</param>
        /// <returns><see cref="Boolean"/> value stating wether the item can be stored or not</returns>
        public abstract bool CanStoreItem(string itemKey, int itemAmount);

        public abstract int GetCurrentInventoryWeight();

        public abstract int GetMaxInventoryWeight();

        /// <summary>
        /// Save the current <see cref="Inventory"/>
        /// </summary>
        public abstract void Save();
#elif CLIENT
/// <summary>
/// Adds or removes a specified amount of an <see cref="inventoryItem"/> from this <see cref="Inventory"/>
/// </summary>
/// <param name="item">Item code or item name of the <see cref="inventoryItem"/> wanting to be added</param>
/// <param name="itemAmount">Amount of the <see cref="inventoryItem"/> wanting to be added</param>
        public void AddItem(string item, int itemAmount) => Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.AddInvItem", item, itemAmount);

        /// <summary>
        /// Adds or removes a specified amount of an <see cref="inventoryItem"/> from this vehicle <see cref="Inventory"/>
        /// </summary>
        /// <param name="item">Item code or item name of the <see cref="inventoryItem"/> wanting to be added</param>
        /// <param name="itemAmount">Amount of the <see cref="inventoryItem"/> wanting to be added</param>
        /// <param name="vehiclePlate">Plate of vehicle that owns the <see cref="Inventory"/></param>
        public void AddItem(string item, int itemAmount, string vehiclePlate) => Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.AddInvItem", item, itemAmount, vehiclePlate);

        public static PlayerInventory playerInventory;
        public static VehicleInventory vehicleInventory;

        public async Task<Inventory> Update(bool isVeh, string vehPlate = "")
        {
            Log.Debug("Requesting inv");
            if (isVeh)
            {
                vehicleInventory = null;
                Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.RequestInventory", vehPlate);
                while (vehicleInventory == null)
                    await BaseScript.Delay(0);
            }
            else
            {
                playerInventory = null;
                Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.RequestInventory");
                while (playerInventory == null)
                    await BaseScript.Delay(0);
            }

            return isVeh ? vehicleInventory as Inventory : playerInventory as Inventory;
        }
#endif
    }
}
