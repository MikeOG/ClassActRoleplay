using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using MenuFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Client.Jobs;
using Roleplay.Client.Player;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.UI.Jobs
{
    public class EmergencyServicesVehicleMenu : ClientAccessor
    {
        public static MenuModel Menu;
        public static CitizenFX.Core.Vehicle serviceVehicle;

        // Vehicle models
        private static List<VehicleHash> VehicleHashValues;
        private static List<string> VehicleHashNames;

        // Vehicle colors
        private static List<VehicleColor> VehicleColorValues;
        private static List<string> VehicleColorNames;

        // Wheel types
        private static List<VehicleWheelType> VehicleWheelTypeValues;
        private static List<string> VehicleWheelTypeNames;

        private static int currentlySelectedVehicleOnFoot;

        private class EmergencyVehicleMenu : MenuModel
        {
            public static async Task ReplaceCurrentVehicleByIndex(int index)
            {
                await new Model(VehicleHashValues[index]).Request(10000);
                if (EmergencyServicesVehicleMenu.serviceVehicle != null)
                {
                    EmergencyServicesVehicleMenu.serviceVehicle.Delete();
                    serviceVehicle = null;
                }

                var model = new Model(VehicleHashValues[index]);
                while (!model.IsLoaded)
                    await model.Request(0);

                var v = await CitizenFX.Core.World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 3, 0)));
                serviceVehicle = v;
                Client.Instance.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(serviceVehicle), true);
            }
            public static void SetVehicleWheelTypeByIndex(int index)
            {
                EmergencyServicesVehicleMenu.serviceVehicle.Mods.WheelType = VehicleWheelTypeValues[index];
            }
            public static Task SetColorByIndex(int index, int layer)
            {
                if (layer == 0)
                {
                    EmergencyServicesVehicleMenu.serviceVehicle.Mods.PrimaryColor = VehicleColorValues[index];
                }
                else if (layer == 1)
                {
                    EmergencyServicesVehicleMenu.serviceVehicle.Mods.SecondaryColor = VehicleColorValues[index];
                }
                else if (layer == 2)
                {
                    EmergencyServicesVehicleMenu.serviceVehicle.Mods.PearlescentColor = VehicleColorValues[index];
                }
                else if (layer == 3)
                {
                    EmergencyServicesVehicleMenu.serviceVehicle.Mods.RimColor = VehicleColorValues[index];
                }

                return Task.FromResult(0);
            }

            private List<MenuItem> _menuItems = new List<MenuItem>();
            private int selectedVehicleIndex = 0;

            private int selectedLivery = 1;
            // TODO: Only check what's valid for a vehicle model once (i.e. save into a Dictionary with Model key)
            public override void Refresh()
            {
                _menuItems.Clear();
                if (serviceVehicle != null && serviceVehicle.Exists())
                {
                    try
                    {
                        //Log.Debug($"X {VehicleHashNames.Count()}");
                        VehicleColor primaryColor = serviceVehicle.Mods.PrimaryColor;
                        VehicleColor secondaryColor = serviceVehicle.Mods.SecondaryColor;
                        VehicleColor pearlescentColor = serviceVehicle.Mods.PearlescentColor;
                        VehicleColor RimColor = serviceVehicle.Mods.RimColor;
                        VehicleWheelType WheelType = (VehicleWheelType)Function.Call<int>(Hash.GET_VEHICLE_WHEEL_TYPE, serviceVehicle.Handle);
                        int LiveryCount = serviceVehicle.Mods.LiveryCount;
                        int Livery = serviceVehicle.Mods.Livery;

                        // R/G/B selection?
                        //System.Drawing.Color NeonLightsColor = vehicle.Mods.NeonLightsColor;

                        // Do we even want this one?
                        //LicensePlateStyle LicensePlateStyle = vehicle.Mods.LicensePlateStyle;

                        // I have gotten several requests not to implement tire smoke (realism level -150)
                        //Log.Debug($"{selectedVehicleIndex}");
                        VehicleMod[] AllMods = serviceVehicle.Mods.GetAllMods();
                        _menuItems.Add(new MenuItemHorNamedSelector
                        {
                            Title = "Vehicle",
                            Description = "Activate to replace your current vehicle.",
                            state = selectedVehicleIndex,
                            Type = MenuItemHorizontalSelectorType.NumberAndBar,
                            wrapAround = true,
                            optionList = VehicleHashNames,
                            OnChange = (selectedAlternative, selName, item) => { selectedVehicleIndex = (item as MenuItemHorNamedSelector).State; },
                            OnActivate = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await ReplaceCurrentVehicleByIndex(selectedAlternative); })
                        });
                        if (LiveryCount != -1)
                        {
                            if (Livery == -1) serviceVehicle.Mods.Livery = 1;
                            _menuItems.Add(new MenuItemHorSelector<int>
                            {
                                Title = $@"Livery",
                                state = selectedLivery,
                                Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                wrapAround = true,
                                minState = 1,
                                maxState = LiveryCount,
                                overrideDetailWith = serviceVehicle.Mods.LocalizedLiveryName != "" ? $"{serviceVehicle.Mods.LocalizedLiveryName}" : "",
                                OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) => { serviceVehicle.Mods.Livery = selectedAlternative; selectedLivery = selectedAlternative; })
                            });
                        }
                        _menuItems.Add(new MenuItemHorNamedSelector
                        {
                            Title = $"Primary Color",
                            state = VehicleColorValues.Contains(primaryColor) ? VehicleColorValues.IndexOf(primaryColor) : 0,
                            Type = MenuItemHorizontalSelectorType.NumberAndBar,
                            wrapAround = true,
                            optionList = VehicleColorNames,
                            OnChange = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await SetColorByIndex(selectedAlternative, 0); })
                        });
                        _menuItems.Add(new MenuItemHorNamedSelector
                        {
                            Title = $"Secondary Color",
                            state = VehicleColorValues.Contains(secondaryColor) ? VehicleColorValues.IndexOf(secondaryColor) : 0,
                            Type = MenuItemHorizontalSelectorType.NumberAndBar,
                            wrapAround = true,
                            optionList = VehicleColorNames,
                            OnChange = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await SetColorByIndex(selectedAlternative, 1); })
                        });
                        _menuItems.Add(new MenuItemHorNamedSelector
                        {
                            Title = $"Pearlescent Color",
                            state = VehicleColorValues.Contains(pearlescentColor) ? VehicleColorValues.IndexOf(pearlescentColor) : 0,
                            Type = MenuItemHorizontalSelectorType.NumberAndBar,
                            wrapAround = true,
                            optionList = VehicleColorNames,
                            OnChange = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await SetColorByIndex(selectedAlternative, 2); })
                        });
                        _menuItems.Add(new MenuItemHorNamedSelector
                        {
                            Title = $"Rim Color",
                            state = VehicleColorValues.Contains(RimColor) ? VehicleColorValues.IndexOf(RimColor) : 0,
                            Type = MenuItemHorizontalSelectorType.NumberAndBar,
                            wrapAround = true,
                            optionList = VehicleColorNames,
                            OnChange = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await SetColorByIndex(selectedAlternative, 3); })
                        });
                        var allowedWheelTypes = serviceVehicle.Mods.AllowedWheelTypes.ToList();
                        if (allowedWheelTypes.Contains(VehicleWheelType.HighEnd)) allowedWheelTypes.Remove(VehicleWheelType.HighEnd); // Removing for now, seems to not want to set this
                        // Wheel Types not a thing for bikes I believe; real and front wheel already a mod category
                        if (serviceVehicle.Model.IsCar && allowedWheelTypes.Count() > 0)
                        {
                            _menuItems.Add(new MenuItemHorNamedSelector
                            {
                                Title = $"Wheel Type",
                                state = allowedWheelTypes.ToList().Contains(WheelType) ? (int)WheelType : (int)allowedWheelTypes[0],
                                Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                wrapAround = true,
                                optionList = allowedWheelTypes.Select(i => $"{i}").ToList(),
                                OnChange = new Action<int, string, MenuItemHorNamedSelector>((selectedAlternative, selName, menuItem) => { serviceVehicle.Mods.WheelType = (VehicleWheelType)selectedAlternative; serviceVehicle.Mods[VehicleModType.FrontWheel].Index = 0; })
                            });
                        }
                        AllMods.Where(m => m.ModType != VehicleModType.Engine && m.ModType != VehicleModType.Transmission && m.ModType != VehicleModType.Brakes).ToList().ForEach(m =>
                        {
                            try
                            {
                                _menuItems.Add(new MenuItemHorSelector<int>
                                {
                                    Title = $@"{m.LocalizedModTypeName}",
                                    state = (m.Index >= -1 && m.Index <= m.ModCount - 1) ? m.Index : 0,
                                    Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                    wrapAround = true,
                                    minState = -1,
                                    maxState = m.ModCount - 1,
                                    overrideDetailWith = m.LocalizedModName,
                                    OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) => { m.Index = selectedAlternative; })
                                });
                                if (m.ModType == VehicleModType.FrontWheel)
                                {
                                    _menuItems.Add(new MenuItemCheckbox
                                    {
                                        Title = $"Special Tire Variation",
                                        Description = "This will only work for some wheels.",
                                        State = m.Variation,
                                        OnActivate = new Action<bool, MenuItemCheckbox>((selectedAlternative, menuItem) => { m.Variation = selectedAlternative; })
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                //Log.Error($"[COPVEHICLEMENU] Exception in vehicle mods code; {ex.Message}");
                            }
                        });

                        // Yes, there are this many extra indices
                        // TODO: Save these for each vehicle after iterating once; this is the first iteration
                        // No performance hit on my own PC though
                        Enumerable.Range(0, 50).ToList().ForEach(i =>
                        {
                            try
                            {
                                if (Function.Call<bool>(Hash.DOES_EXTRA_EXIST, serviceVehicle.Handle, i))
                                {
                                    _menuItems.Add(new MenuItemCheckbox
                                    {
                                        Title = $"Extra #{i + 1}",
                                        state = Function.Call<bool>(Hash.IS_VEHICLE_EXTRA_TURNED_ON, serviceVehicle.Handle, i),
                                        OnActivate = new Action<bool, MenuItemCheckbox>((selectedAlternative, menuItem) => { Function.Call(Hash.SET_VEHICLE_EXTRA, serviceVehicle.Handle, i, selectedAlternative ? 0 : -1); })
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                //Log.Error($"[COPVEHICLEMENU] Exception in extras code; {ex.Message}");
                            }
                        });
                        int firstVisibleItem = visibleItems.IndexOf(menuItems.First());
                        int lastVisibleItem = visibleItems.IndexOf(menuItems.Last());
                        visibleItems = _menuItems.Slice(firstVisibleItem, lastVisibleItem);
                        //if (lastVisibleItem - firstVisibleItem >= _menuItems.Count)
                        //    visibleItems = _menuItems;
                        //else
                        //{
                        //    visibleItems = _menuItems.Slice(firstVisibleItem, firstVisibleItem + Math.Min(Math.Max(lastVisibleItem - firstVisibleItem, _menuItems.Count - 1 - firstVisibleItem), numVisibleItems));
                        //}

                        menuItems = _menuItems;
                        SelectedIndex = SelectedIndex; // refreshes state
                    }
                    catch (Exception ex)
                    {
                        //Log.Error($"[COPVEHICLEMENU] Outer exception {ex.Message}");
                    }
                }
                else
                {
                    if (!currentlySelectedVehicleOnFoot.IsBetween(0, VehicleHashValues.Count - 1)) currentlySelectedVehicleOnFoot = 0;
                    _menuItems.Add(new MenuItemHorNamedSelector
                    {
                        Title = $"Spawn Vehicle",
                        Description = "Activate to spawn vehicle.",
                        state = currentlySelectedVehicleOnFoot,
                        Type = MenuItemHorizontalSelectorType.NumberAndBar,
                        wrapAround = true,
                        optionList = VehicleHashNames,
                        OnChange = new Action<int, string, MenuItemHorNamedSelector>((selectedAlternative, selName, menuItem) => { selectedVehicleIndex = currentlySelectedVehicleOnFoot = selectedAlternative; }),
                        OnActivate = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => { await ReplaceCurrentVehicleByIndex(selectedAlternative); })
                    });
                    menuItems = _menuItems;
                    SelectedIndex = SelectedIndex; // refreshes state
                }
            }
        }

        public EmergencyServicesVehicleMenu(Client client) : base(client)
        {
            serviceVehicle = null;

            VehicleHashValues = VehicleLoadoutPresets.serviceVehicles.Select(v => (VehicleHash)Game.GenerateHash(v)).ToList();
            VehicleHashNames = VehicleLoadoutPresets.serviceVehicles.Select(v => v.ToTitleCase().AddSpacesToCamelCase()).ToList();
            VehicleColorValues = Enum.GetValues(typeof(VehicleColor)).OfType<VehicleColor>().ToList();
            VehicleColorNames = Enum.GetNames(typeof(VehicleColor)).Select(c => c.AddSpacesToCamelCase()).ToList();
            VehicleWheelTypeValues = Enum.GetValues(typeof(VehicleWheelType)).OfType<VehicleWheelType>().ToList();
            VehicleWheelTypeNames = Enum.GetNames(typeof(VehicleWheelType)).Select(c => c.AddSpacesToCamelCase()).ToList();
            currentlySelectedVehicleOnFoot = 0;

            Menu = new EmergencyVehicleMenu { numVisibleItems = 7 };
            Menu.headerTitle = "Vehicle Customization";
            Menu.statusTitle = "";
            //Menu.Refresh();
            //Menu.menuItems = new List<MenuItem>() { new MenuItemStandard { Title = "Populating menu..." } }; // Currently we need at least one item in a menu; could make it work differently, but eh.
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu { Title = $"Spawn Service Vehicle", SubMenu = Menu }, () => Client.Instance.Get<JobHandler>().OnDutyAsJob(JobType.Police | JobType.EMS) && isInRangeOfDutyGarage(), 1000);

            client.RegisterTickHandler(OnTick);

            client.RegisterEventHandler("Job.SpawnServiceVehicle", new Action<int>(async index =>
            {
                if(isInRangeOfDutyGarage())
                    await EmergencyVehicleMenu.ReplaceCurrentVehicleByIndex(index);
            }));
        }

        public static void RemakeHashList()
        {
            VehicleHashValues = VehicleLoadoutPresets.serviceVehicles.Select(v => (VehicleHash)Game.GenerateHash(v)).ToList();
            VehicleHashNames = VehicleLoadoutPresets.serviceVehicles.Select(v => v.ToTitleCase().AddSpacesToCamelCase()).ToList();
        }

        private static Task OnTick()
        {
            try
            {
                if (InteractionUI.Observer.CurrentMenu == Menu)
                {
                    Menu.Refresh();
                }
            }
            catch (Exception ex)
            {
                //Log.Error($"[COPVEHICLEMENU] Exception in OnTick; {ex.Message}");
            }

            return Task.FromResult(0);
        }

        private readonly List<Vector3> garageLocations = new List<Vector3>()
        {
            new Vector3(438.157f, -1021.212f, 28.676f),
            new Vector3(642.056f, 0.739643f, 82.7867f),
            new Vector3(-445.692f, 6014.96f, 31.7164f),
            new Vector3(1853.78f, 3685.63f, 43.2671f),
            new Vector3(360.454f, -1584.85f, 29.2919f),
            new Vector3(827.447f, -1290.29f, 28.2407f),
            new Vector3(-1108.19f, -845.159f, 19.3169f),
            new Vector3(364.51f, -591.90f, 28.69f),
            new Vector3(290.26f, -588.64f, 43.18f),
            new Vector3(1150.981f, -1529.867f, 34.373f),
            new Vector3(297.06f, -1435.88f, 29.80f),
            new Vector3(-465.88f, -325.70f, 34.23f),
            new Vector3(1838.88f, 3673.535f, 34.277f),
            new Vector3(-248.473f, 6332.059f, 32.426f),
            new Vector3(-560.51f, -146.9f, 38.12f),

            //New Boat spawns
            new Vector3(-801.28f, -1505.82f, -0.47f),
            new Vector3(3853.96f, 4464.8f, 2.73f),
            new Vector3(-1604.68f, 5257.14f, 2.08f),

        };

        public bool isInRangeOfDutyGarage()
        {
            var inRange = false;
            garageLocations.ForEach(g =>
            {
                if (g.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(40.0f, 2))
                    inRange = true;
            });

            return inRange;
        }
    }
}