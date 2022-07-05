using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.UI.Inventory.Menus;
using Roleplay.Shared;
using MenuFramework;

namespace Roleplay.Client.UI.Inventory
{
    public class InventoryMenuHandler : ClientAccessor
    {
        private Dictionary<string, InventoryMenu> inventoryMenus = new Dictionary<string, InventoryMenu>();

        public InventoryMenuHandler(Client client) : base(client)
        {
            RegisterInventoryMenu("PlayerInventory", new PlayerInventoryMenu(""));
            RegisterInventoryMenu("VehicleInventory", new VehicleInventoryMenu(""));
            RegisterInventoryMenu("PropertyInventory", new PropertyInventoryMenu(""));

            CommandRegister.RegisterCommand("inv|inventory", cmd =>
            {
                OpenInventoryMenu("PlayerInventory");
            });
            CommandRegister.RegisterCommand("vehinv|vehicleinventory", new Action<Command>(cmd =>
            {
                OpenInventoryMenu("VehicleInventory");
            }));
        }

        public void OpenInventoryMenu(string menuName)
        {
            var menu = GetInventoryMenu(menuName);

            menu.GetOnActivateFunction()(menu.GetSubMenu());
            InteractionUI.Observer.OpenMenu(menu);
        }

        public InventoryMenu GetInventoryMenu(string menuName)
        {
            return GetInventoryMenu<InventoryMenu>(menuName);
        }

        public T GetInventoryMenu<T>(string menuName) where T : InventoryMenu
        {
            return inventoryMenus.TryGetValue(menuName, out var menu) ? menu as T : default;
        }

        public void RegisterInventoryMenu(string menuName, InventoryMenu menu)
        {
            Log.Info($"Registering inventory menu {menuName}");

            if (inventoryMenus.ContainsKey(menuName)) // Remove old traces of menu if any exist
            {
                Log.Verbose($"Found old traces of the inventory menu {menu} removing");
                Client.Get<InteractionUI>().RemoveInteractionMenuItem(inventoryMenus[menuName].GetSubMenu());
            }

            inventoryMenus[menuName] = menu;

            Client.Get<InteractionUI>().RegisterInteractionMenuItem(menu.GetSubMenu(), menu.CanViewMenu, menu.MenuPriority);
        }

        [EventHandler("Inventory.UI.UpdateMenu")]
        private void OnUpdateMenu(string menu, string invString)
        {
            if (inventoryMenus.ContainsKey(menu))
            {
                inventoryMenus[menu].RefreshMenu(invString);
            }
        }
    }
}
