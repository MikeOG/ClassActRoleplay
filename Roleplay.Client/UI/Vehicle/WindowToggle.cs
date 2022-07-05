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
    internal class WindowToggle : ClientAccessor
    {
        private static MenuModel windowMenu;
        private static List<VehicleWindowIndex> VehicleWindowValues = Enum.GetValues(typeof(VehicleWindowIndex)).OfType<VehicleWindowIndex>().Where(w => (int)w < 4).ToList();
        private static List<string> VehicleWindowNames = VehicleWindowValues.Select(d => d.ToString().AddSpacesToCamelCase()).ToList();
        private static Dictionary<VehicleWindowIndex, bool> windowStates;

        private static List<MenuItem> _menuItems = new List<MenuItem>();
        private static CitizenFX.Core.Vehicle vehicle;

        public WindowToggle(Client client) : base(client)
        {
            windowMenu = new MenuModel { headerTitle = "Vehicle Window Menu" };
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                SubMenu = windowMenu,
                Title = "Vehicle Window Menu"
            }, () => Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed, 500);
            client.RegisterTickHandler(OnTick);
        }

        private static bool AreAllWindowsOpen(List<VehicleWindowIndex> windowValues, Dictionary<VehicleWindowIndex, bool> windowStates, CitizenFX.Core.Vehicle vehicle)
        {
            var allOpen = true;
            windowValues.Select((window, index) => new { window, index }).ToList().ForEach(o =>
            {
                var window = vehicle.Windows[o.window];
                if (!windowStates[window.Index])
                    allOpen = false;
            });
            return allOpen;
        }

        private async Task OnTick()
        {
            try
            {
                if (InteractionUI.Observer.CurrentMenu == windowMenu)
                {
                    if (!Game.PlayerPed.IsInVehicle())
                    {
                        vehicle = null;
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

        private static void refreshMenuItems()
        {
            try
            {
                if (vehicle != Game.PlayerPed.CurrentVehicle)
                {
                    windowStates = VehicleWindowValues.ToDictionary(v => v, v => false);
                    vehicle = Game.PlayerPed.CurrentVehicle;
                }
                _menuItems.Clear();
                _menuItems.Add(new MenuItemCheckbox
                {
                    Title = "Open all windows",
                    state = AreAllWindowsOpen(VehicleWindowValues, windowStates, vehicle),
                    OnActivate = (state, item) => 
                    {
                        VehicleWindowValues.Select((window, index) => new { window, index }).ToList().ForEach(o =>
                        {
                            var window = vehicle.Windows[o.window];
                            if (state)
                                window.RollDown();
                            else
                                window.RollUp();
                            windowStates[window.Index] = state;
                        });
                    }
                });
                VehicleWindowValues.Select((window, index) => new { window, index }).ToList().ForEach(o =>
                {
                    var window = vehicle.Windows[o.window];
                    _menuItems.Add(new MenuItemCheckbox
                    {
                        Title = $"Roll Down {window.Index.ToString().AddSpacesToCamelCase()}",
                        state = windowStates[window.Index],
                        OnActivate = (state, item) =>
                        {
                            if (state)
                                window.RollDown();
                            else
                                window.RollUp();
                            windowStates[window.Index] = state;
                        }
                    });
                });
                windowMenu.menuItems = _menuItems;
                windowMenu.SelectedIndex = windowMenu.SelectedIndex;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
