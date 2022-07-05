using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Helpers;
using Roleplay.Shared;
using MenuFramework;

namespace Roleplay.Client.UI.Vehicle
{
    internal class DoorToggle : ClientAccessor
    {
        private MenuModel doorMenu;
        private List<MenuItem> _menuItems = new List<MenuItem>();

        public DoorToggle(Client client) : base(client)
        {
            doorMenu = new MenuModel { headerTitle = "Vehicle door Menu" };
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                SubMenu = doorMenu,
                Title = "Vehicle Door Menu"
            }, () => Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.Driver == Cache.PlayerPed, 500);
            Client.RegisterTickHandler(OnTick);
        }

        private bool AreAllDoorsOpen(VehicleDoor[] doors)
        {
            var allOpen = true;
            doors.ToList().ForEach(door =>
            {
                if (!door.IsOpen)
                    allOpen = false;
            });
            return allOpen;
        }

        private async Task OnTick()
        {
            try
            {
                if (InteractionUI.Observer.CurrentMenu == doorMenu)
                {
                    if (!Cache.PlayerPed.IsInVehicle())
                    {
                        InteractionUI.Observer.CloseMenu();
                    }
                    else
                    {
                        refreshMenuItems();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void refreshMenuItems()
        {
            try
            {
                _menuItems.Clear();
                var vehicle = Cache.PlayerPed.CurrentVehicle;
                var doors = vehicle.Doors.GetAll();
                _menuItems.Add(new MenuItemCheckbox
                {
                    Title = "Open all doors",
                    state = AreAllDoorsOpen(doors),
                    OnActivate = (state, item) =>
                    {
                        doors.ToList().ForEach(door =>
                        {
                            if (state)
                                door.Open();
                            else
                                door.Close();
                        });
                    }
                });
                doors.ToList().ForEach(door =>
                {
                    if (!door.IsBroken)
                        _menuItems.Add(new MenuItemCheckbox
                        {
                            Title = $"Open {door.Index.ToString().AddSpacesToCamelCase()}",
                            state = door.IsOpen,
                            OnActivate = (state, item) =>
                            {
                                if (state)
                                    door.Open();
                                else
                                    door.Close();
                            }
                        });
                });
                doorMenu.menuItems = _menuItems;
                doorMenu.SelectedIndex = doorMenu.SelectedIndex;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
