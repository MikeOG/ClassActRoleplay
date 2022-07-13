using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.Property;
using Roleplay.Client.Session;
using Roleplay.Client.Vehicles;
using MenuFramework;

namespace Roleplay.Client.UI.Inventory.Menus
{
    public sealed class PlayerInventoryMenu : InventoryMenu
    {
        public override string MenuHeaderTitle { get; set; } = "Player Inventory";

        public PlayerInventoryMenu(Shared.Inventory targetInv)
            : base(targetInv)
        {

        }

        public PlayerInventoryMenu(string invString)
            : base(invString)
        {

        }

        public override Action<MenuItemSubMenu> GetOnActivateFunction()
        {
            return item =>
            {
                refreshPlayerInventory();
            };
        }

        protected override List<InventoryItemInteractable> getMenuInteractables()
        {
            var actions = new List<InventoryItemInteractable>
            {
                InventoryMenuInteractions.UseItem,
                InventoryMenuInteractions.DropItem,
            };

            if (Client.Instance.Get<VehicleHandler>().GetClosestVehicleWithKeys() != null)
            {
                actions.Add(InventoryMenuInteractions.PutInVehItem);
            }

            if (Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage())
            {
                actions.Add(InventoryMenuInteractions.PutInStorageItem);
            }

            return actions;
        }

        private void refreshPlayerInventory()
        {
            var playerSession = Client.Instance.Get<SessionManager>().GetPlayer(Game.Player);
            RefreshMenu(playerSession.GetGlobalData("Character.Inventory", ""));
        }
    }
}
