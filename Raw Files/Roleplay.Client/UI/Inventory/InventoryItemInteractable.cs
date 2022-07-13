using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;

namespace Roleplay.Client.UI.Inventory
{
    public class InventoryItemInteractable
    {
        public string Title;

        /// <summary>
        /// arg 1: item currently selected
        /// arg 2: amount wanting to be used
        /// </summary>
        public Action<inventoryItem, int> InteractFunc;

        public InventoryItemInteractable()
        {
            InteractFunc = (item, amount) =>
            {
                Client.Instance.TriggerServerEvent($"Inventory.{Title}InvItem", item.itemName, amount);
            };
        }
    }
}
