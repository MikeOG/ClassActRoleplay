using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Jobs.EmergencyServices.EMS;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Server.Shared.Enums;
using Roleplay.Server.Shared.Models;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Enviroment
{
    public class DroppedItemHandler : ServerAccessor
    {
        private List<DroppedServerItem> droppedItems = new List<DroppedServerItem>();
        private int currentItemId;
        private InventoryManager InvManager => Server.Get<InventoryManager>();

        public DroppedItemHandler(Server server) : base(server)
        {
            server.RegisterEventHandler("Inventory.DropInvItem", new Action<Player, string, int, List<object>>(OnRequestDrop));
            server.RegisterEventHandler("Items.RequestItemPickup", new Action<Player, int>(OnRequestPickup));
            server.RegisterEventHandler("Items.RequestDroppedItems", new Action<Player>(BroadcastDroppedItems));
        }

        public void BroadcastDroppedItems([FromSource] Player targetPlayer = null)
        {
            if(targetPlayer == null)
                BaseScript.TriggerClientEvent("Items.UpdateDroppedItems", JsonConvert.SerializeObject(droppedItems.Select(o => new DroppedItemModel(o))));
            else
                targetPlayer.TriggerEvent("Items.UpdateDroppedItems", JsonConvert.SerializeObject(droppedItems.Select(o => new DroppedItemModel(o))));
        }

        public void AddDroppedItem(inventoryItem invItem, int dropAmount, Vector3 itemPos)
        {
            currentItemId++;
            invItem.itemAmount = dropAmount;
            Log.Debug($"Creating a dropped item for {invItem.itemAmount} {invItem.itemName} with meta data {invItem.metaData}");
            droppedItems.Add(new DroppedServerItem
            {
                ItemId = currentItemId,
                ItemData = invItem,
                ItemPos = itemPos
            });
            BroadcastDroppedItems();
        }

        private void OnRequestDrop([FromSource] Player source, string itemData, int dropAmount, List<object> pos)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null || dropAmount == 0) return;

            if (/*Server.Get<ArrestHandler>().GetCuffState(playerSession)*/playerSession.CuffState != CuffState.None ||
                /*Server.Get<ArrestHandler>().GetDragState(playerSession)*/playerSession.DragState != DragState.None ||
                playerSession.DeathState != DeathState.Alive) return;

            //PlayerInventory playerInv = InvManager.GetPlayerInventory(playerSession);
            var playerInv = playerSession.Inventory;
            var invItem = JsonConvert.DeserializeObject<inventoryItem>(itemData);
            Log.Verbose($"{source.Name} is attempting to drop {dropAmount} {invItem.itemName}");

            if (playerInv.HasItemWithAmount(invItem, dropAmount))
            {
                playerInv.AddItem(invItem, -dropAmount);
                AddDroppedItem(invItem, dropAmount, pos.ToVector3());
                source.TriggerEvent("Items.OnItemDropped");
            }
            else
            {
                Log.ToClient("[Inventory]", "You don't have enough of this item to do that", ConstantColours.Inventory, source);
            }
        }

        private void OnRequestPickup([FromSource] Player source, int itemId)
        {
            var droppedItem = droppedItems.FirstOrDefault(o => o.CanBePickedUp && o.ItemId == itemId);

            Log.Verbose($"{source.Name} is attempting to pickup ItemId {itemId}");
            if (droppedItem != null)
            {
                droppedItem.CanBePickedUp = false;
                var playerSession = Server.Instances.Session.GetPlayer(source);
                if (playerSession == null)
                {
                    droppedItem.CanBePickedUp = true;
                    return;
                }

                if (/*Server.Get<ArrestHandler>().GetCuffState(playerSession)*/playerSession.CuffState != CuffState.None ||
                   /*Server.Get<ArrestHandler>().GetDragState(playerSession)*/playerSession.DragState != DragState.None ||
                   playerSession.DeathState != DeathState.Alive)
                {
                    droppedItem.CanBePickedUp = true;
                    return;
                }

                PlayerInventory playerInv = InvManager.GetPlayerInventory(playerSession);

                if (playerInv.CanStoreItem(droppedItem.ItemData.itemCode, droppedItem.ItemData.itemAmount))
                {
                    var amountToAdd = droppedItem.ItemData.itemAmount;
                    droppedItem.ItemData.itemAmount = 0;
                    playerInv.AddItem(droppedItem.ItemData, amountToAdd);
                    droppedItems.Remove(droppedItem);
                    BroadcastDroppedItems();
                    Log.Debug($"Deleted ItemId {droppedItem.ItemId} due to it being picked up");
                }
                else
                {
                    Log.ToClient("[Inventory]", "You don't have enough space to carry this item", ConstantColours.Inventory, source);
                    droppedItem.CanBePickedUp = true;
                }
            }
            else
            {
                Log.Verbose($"ItemId {itemId} is null (could be being picked up)");
            }
        }
    }

    public class DroppedServerItem
    {
        public int ItemId;
        public inventoryItem ItemData;
        public Vector3 ItemPos;
        public string MetaData 
        {
            get => ItemData.metaData;
            set => ItemData.metaData = value;
        }

        public bool CanBePickedUp = true;
    }
}
