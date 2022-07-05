using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Client.Property;
using MenuFramework;

namespace Roleplay.Client.UI.Inventory.Menus
{
    public class PropertyInventoryMenu : InventoryMenu
    {
        public override string MenuHeaderTitle { get; set; } = "Property storage";
        public override string ItemDescription { get; set; } = "This storage has {0} of this item";
        public PropertyInventoryMenu(Shared.Inventory targetInv)
            : base(targetInv)
        {

        }

        public PropertyInventoryMenu(string invString)
            : base(invString)
        {

        }

        public override Action<MenuItemSubMenu> GetOnActivateFunction()
        {
            return item =>
            {
                RefreshMenu("");
                if (Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage())
                {
                    Client.Instance.TriggerServerEvent("Inventory.RequestInventory", "property");
                }
            };
        }

        public override bool CanViewMenu() => Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage();

        protected override List<InventoryItemInteractable> getMenuInteractables()
        {
            return new List<InventoryItemInteractable>
            {
                InventoryMenuInteractions.TakeFromStorageItem
            };
        }
    }
}
