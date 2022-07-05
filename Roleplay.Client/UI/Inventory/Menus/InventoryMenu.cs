using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using MenuFramework;
using Roleplay.Shared;
using Roleplay.Shared.Models;

namespace Roleplay.Client.UI.Inventory.Menus
{
    public abstract class InventoryMenu : MenuModel
    {
        public virtual string MenuHeaderTitle { get; set; } = "Inventory";
        public virtual string ItemDescription { get; set; } = "You have {0} of this item with a weight of {1}";
        public virtual string ItemSubMenuHeader { get; set; } = "{0} interactions";
        public virtual int MenuPriority { get; set; } = 100;

        private Shared.Inventory targetInventory;
        private MenuItemSubMenu cachedSubMenu;

        protected InventoryMenu(Shared.Inventory targetInv)
        {
            this.headerTitle = MenuHeaderTitle;

            RefreshMenu(targetInv);
        }

        protected InventoryMenu(string invString)
            : this(new PlayerInventory(invString, Client.LocalSession))
        {

        }

        /// <summary>
        /// Refreshes contents of this <see cref="InventoryMenu"/> with what is in the specified <see cref="Shared.Inventory"/>
        /// </summary>
        /// <param name="targetInv"><see cref="Shared.Inventory"/> wanting to be displayed in the menu</param>
        public void RefreshMenu(Shared.Inventory targetInv)
        {
            this.targetInventory = targetInv;

            menuItems.Clear();
            menuItems = CreateMenuItems();
            menuItems = menuItems.OrderBy(o => o.Title).ToList();

            SelectedIndex = SelectedIndex;
            SelectedItem = SelectedItem;
        }

        /// <summary>
        /// Refreshes contents of this <see cref="InventoryMenu"/> based on the specified inventory data string
        /// </summary>
        /// <param name="invString">Inventory data string wanting to be display in the menu</param>
        public void RefreshMenu(string invString)
        {
            RefreshMenu(new PlayerInventory(invString, Client.LocalSession));
        }

        public virtual List<MenuItem> CreateMenuItems()
        {
            var items = new List<MenuItem>();

            foreach (var item in targetInventory.InventoryItems)
            {
                items.Add(new MenuItemSubMenu
                {
                    Title = item.itemName, SubMenu = CreateItemSubMenu(item), Description = string.Format(ItemDescription, item.itemAmount, (int)(item.itemWeight * item.itemAmount))
                });
            }

            if (items.Count == 0)
            {
                items.Add(new MenuItemStandard
                {
                    Title = "Empty"
                });
            }

            return items;
        }

        public virtual MenuModel CreateItemSubMenu(inventoryItem invItem)
        {
            var menu = new MenuModel
            {
                headerTitle = string.Format(ItemSubMenuHeader, invItem.itemName)
            };
            var menuSubItems = new List<MenuItem>();
            var interactables = getMenuInteractables();

            foreach (var interaction in interactables)
            {
                menuSubItems.Add(new MenuItemStandard
                {
                    Title = interaction.Title,
                    OnActivate = async item =>
                    {
                        Log.Debug($"Activated an inventory menu interactable of {interaction.Title} for inventory item {invItem.itemName}");

                        InteractionUI.Observer.CloseMenu(true);
                        if (int.TryParse(await Game.GetUserInput(4), out var amountToUse))
                        {
                            Log.Debug($"Wanting to use {amountToUse} of this item");
                            InteractionUI.Observer.OpenMenu(InteractionUI.InteractionMenu);
                            InteractionUI.Observer.OpenMenu(this);
                            await BaseScript.Delay(0);
                            if (amountToUse > invItem.itemAmount)
                                amountToUse = invItem.itemAmount;
                            else if (amountToUse < 0)
                                amountToUse = 0;

                            Log.Debug($"Will actually use {amountToUse} of this item because validation says this is what we can use");

                            //invItem.itemAmount = 0;
                            interaction.InteractFunc(invItem, amountToUse);
                        }
                    }
                });
            }

            menu.menuItems = menuSubItems;

            return menu;
        }

        public virtual bool CanViewMenu() => true;

        public virtual MenuItemSubMenu GetSubMenu()
        {
            return cachedSubMenu ?? (cachedSubMenu = new MenuItemSubMenu
            {
                Title = MenuHeaderTitle, SubMenu = this, OnActivate = GetOnActivateFunction()
            });
        }

        public abstract Action<MenuItemSubMenu> GetOnActivateFunction();

        /// <summary>
        /// Gets a list of <see cref="InventoryItemInteractable"/> that can be done within this inventory menu
        /// </summary>
        /// <returns>A list of <see cref="InventoryItemInteractable"/> that can be done within this inventory menu</returns>
        protected abstract List<InventoryItemInteractable> getMenuInteractables();
    }
}