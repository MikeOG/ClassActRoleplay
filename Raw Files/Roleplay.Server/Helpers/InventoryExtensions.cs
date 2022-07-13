using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;

namespace Roleplay.Server.Helpers
{
    public static class InventoryExtensions
    {
        public static void RemoveIllegalItems(this Inventory playerInv)
        {
            foreach (var item in playerInv.InventoryItems)
            {
                if (item.isIllegal)
                {
                    playerInv.AddItem(item, -item.itemAmount);
                }
            }
        }

        public static void RemoveAllWeapons(this Inventory playerInv)
        {
            foreach (var item in playerInv.InventoryItems)
            {
                if (item.itemCode.ToUpper().Contains("WEAPON_"))
                {
                    playerInv.AddItem(item, -item.itemAmount);
                }
            }
        }
    }
}
