using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Models;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using static Roleplay.Shared.VehicleStoreItems;
using MenuFramework;
using Newtonsoft.Json;

namespace Roleplay.Client.UI.Store
{
    internal class VehicleStoreMenu : ClientAccessor
    {
        private MenuModel vehicleMenu;
        private bool isBuying = false;
        private string currentLoadingCategory = "";
        private CitizenFX.Core.Vehicle previewVehicle = null;
        private Dictionary<string, Model> loadedModels = new Dictionary<string, Model>();
        /// <summary>
        /// Vehicles will be previewed here (preview location closest to player at time will be used)
        /// </summary>
        private readonly Dictionary<string, List<Vector3>> vehiclePreviewLocation = new Dictionary<string, List<Vector3>>
        {
            ["VehicleStore"] = new List<Vector3>
            {
                new Vector3(-45.35f, -1098.42f, 26.42f),
                new Vector3(1222.89f, 2712.26f, 38.01f)
            },
            ["MotorcycleStore"] = new List<Vector3>
            {
                new Vector3(316.754f, -1165.5f, 29.2918f),
            },
            ["UtilityStore"] = new List<Vector3>
            {
                new Vector3(53.6084f, 6454.08f, 31.2541f),
            },
            ["TunerStore"] = new List<Vector3>
            {
                new Vector3(-1083.14f, -1254.64f, 5.41f),
            },
            ["BoatStore"] = new List<Vector3>
            {
                new Vector3(-833.38f, -1497f, 1.63f),
            }
        };
        /// <summary>
        /// Where the vehicle will spawn after being bought (spawn location closest to player will be used)
        /// </summary>
        private readonly Dictionary<string, List<Vector3>> vehicleSpawnLocations = new Dictionary<string, List<Vector3>>
        {
            ["VehicleStore"] = new List<Vector3>
            {
                new Vector3(-31.12f, -1090.63f, 26.42f),
                new Vector3(1239.26f, 2713.04f, 38.01f)
            },
            ["MotorcycleStore"] = new List<Vector3>
            {
                new Vector3(316.754f, -1157.17f, 29.2918f),
            },
            ["UtilityStore"] = new List<Vector3>
            {
                new Vector3(39.9251f, 6440.9f, 31.2405f),
            },
            ["TunerStore"] = new List<Vector3>
            {
                new Vector3(-1077.2f, -1250.41f, 5.41f),
            },
            ["BoatStore"] = new List<Vector3>
            {
                new Vector3(-833.38f, -1497f, 1.63f),
            }
        };

        private readonly List<Vector3> markerLocations = new List<Vector3>()
        {
            new Vector3(-34.42f, -1103.27f, 25.52f),
            new Vector3(1224.98f, 2724.05f, 37.10f),
            new Vector3(-1082.48f, -1261.57f, 5.61f), // Tuner Store
            new Vector3(307.3f, -1162.96f, 28.33f), // Motorcycle store
            new Vector3(57.19f, 6470.60f, 30.47f), // Utility store
            new Vector3(-846.26f, -1497.77f, 1.63f) // Boat Store
        };

        private Dictionary<string, MenuModel> storeMenus = new Dictionary<string, MenuModel>();

        public VehicleStoreMenu(Client client) : base(client)
        {
            try
            { 
                client.RegisterTickHandler(OnTick);
                client.RegisterEventHandler("Vehicle.Store.SpawnBoughtVehicle", new Action<string, string, string, string>(SpawnBoughtVehicle));
                client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
                markerLocations.ForEach(o => MarkerHandler.AddMarker(new Marker(o, MarkerType.HorizontalCircleFat, System.Drawing.Color.FromArgb(100, 204, 204, 0), 3)));
                EntityDecoration.RegisterProperty("Vehicle.IsPreviewVehicle", DecorationType.Bool);

                StoreMenus.ToList().ForEach(async o =>
                {
                    var menuCategories = new Dictionary<string, List<VehicleModel>>();
                    var storeMenuItems = new List<MenuItem>();
                    o.Value.ForEach(vehData =>
                    {
                        if (!menuCategories.ContainsKey(vehData.vehicleCategory))
                            menuCategories.Add(vehData.vehicleCategory, new List<VehicleModel>());

                        menuCategories[vehData.vehicleCategory].Add(vehData);
                    });
                    foreach (var i in menuCategories)
                    {
                        var vehicleList = i.Value.OrderBy(a => a.displayName).ToList();
                        var categoryItems = new List<MenuItem>();
                        var vehicleCategoryMenu = new MenuModel { headerTitle = i.Key, menuItems = categoryItems };
                        await vehicleList.ForEachAsync(async veh =>
                        {
                            categoryItems.Add(new MenuItemStandard
                            {
                                Title = $"{veh.displayName}",
                                OnActivate = async state =>
                                {
                                    isBuying = true;
                                    InteractionUI.Observer.CloseMenu(true);
                                    if (!loadedModels.ContainsKey(veh.modelName))
                                    {
                                        loadedModels[veh.modelName] = new Model(Game.GenerateHash(veh.modelName));
                                    }

                                    var vehModel = loadedModels[veh.modelName];
                                    //if (!Cache.PlayerPed.IsInVehicle() || previewVehicle == null)
                                    {
                                        Log.ToChat("[Store]", "Purchasing vehicle...", ConstantColours.Green);
                                        while (!vehModel.IsLoaded)
                                            await vehModel.Request(0);

                                        deletePreviewVehicle();

                                        var spawnLocation = vehiclePreviewLocation[o.Key].First(b => b.DistanceToSquared(Game.PlayerPed.Position) < 250.0f);
                                        previewVehicle = new CitizenFX.Core.Vehicle(API.CreateVehicle((uint)vehModel.Hash, spawnLocation.X, spawnLocation.Y, spawnLocation.Z, 0.0f, false, false))
                                        {
                                            IsPositionFrozen = true,
                                            LockStatus = VehicleLockStatus.StickPlayerInside,
                                            IsDriveable = false,
                                            IsInvincible = true
                                        }; // No network
                                        previewVehicle.SetDecor("Vehicle.IsPreviewVehicle", true);
                                        Game.PlayerPed.Task.WarpIntoVehicle(previewVehicle, VehicleSeat.Driver);
                                        await BaseScript.Delay(1000);
                                    }

                                    var vehData = VehicleDataPacker.PackVehicleData(previewVehicle);
                                    if (vehData == null)
                                    {
                                        Log.ToChat("[Store]", "There was an error purchasing this vehicle. Please try again", ConstantColours.Store);
                                        isBuying = false;
                                        return;
                                    }

                                    var vehDataModel = JsonConvert.DeserializeObject<VehicleDataModel>(vehData);
                                    if(vehDataModel.Model != 0 && vehModel.Hash == Cache.PlayerPed.CurrentVehicle.Model.Hash)
                                    {
                                        Roleplay.Client.Client.Instance.TriggerServerEvent("Vehicle.Store.BuyVehicle", veh.modelName, veh.price, o.Key, vehData);
                                    }
                                    else
                                    {
                                        Log.ToChat("[Store]", "There was an error purchasing this vehicle. Please try again", ConstantColours.Store);
                                    }

                                    isBuying = false;
                                },
                                OnSelect = state =>
                                {
                                    var modelLoaded = loadedModels.ContainsKey(veh.modelName);
                                    if (currentLoadingCategory != veh.vehicleCategory || !modelLoaded)
                                        requestModelsForVehicleType(o.Key, vehicleCategoryMenu, veh.vehicleCategory);

                                    if (!modelLoaded) return;

                                    var vehModel = loadedModels[veh.modelName];
                                    deletePreviewVehicle();

                                    var spawnLocation = vehiclePreviewLocation[o.Key].First(b => b.DistanceToSquared(Game.PlayerPed.Position) < 250.0f);
                                    previewVehicle = new CitizenFX.Core.Vehicle(API.CreateVehicle((uint)vehModel.Hash, spawnLocation.X, spawnLocation.Y, spawnLocation.Z, 0.0f, false, false))
                                    {
                                        IsPositionFrozen = true,
                                        LockStatus = VehicleLockStatus.StickPlayerInside,
                                        IsDriveable = false,
                                        IsInvincible = true
                                    }; // No network
                                    previewVehicle.SetDecor("Vehicle.IsPreviewVehicle", true);
                                    Game.PlayerPed.Task.WarpIntoVehicle(previewVehicle, VehicleSeat.Driver);
                                },
                                Detail = veh.price == 0 ? $"Free" : $"(${veh.price})",
                            });
                            await BaseScript.Delay(0);
                        });
                        storeMenuItems.Add(new MenuItemSubMenu
                        {
                            Title = i.Key,
                            SubMenu = vehicleCategoryMenu,
                            OnActivate = item =>
                            {
                                deletePreviewVehicle();

                                requestModelsForVehicleType(o.Key, vehicleCategoryMenu, i.Key);

                                vehicleCategoryMenu.menuItems[vehicleCategoryMenu.SelectedIndex].OnSelect(new MenuItemStandard());
                            }
                        });
                        await BaseScript.Delay(0);
                    }
                    var menuThing = new MenuModel {headerTitle = o.Key.AddSpacesToCamelCase(), menuItems = storeMenuItems};
                    storeMenus[o.Key] = menuThing;
                    client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
                    {
                        Title = o.Key.AddSpacesToCamelCase(),
                        SubMenu = menuThing
                    }, () => vehiclePreviewLocation.ContainsKey(o.Key) && vehiclePreviewLocation[o.Key].Any(loc => loc.DistanceToSquared(Game.PlayerPed.Position) < 250.0f), 510);
                });
                vehiclePreviewLocation.ToList().ForEach(loc =>
                {
                    BlipHandler.AddBlip(loc.Key.AddSpacesToCamelCase(), loc.Value, new BlipOptions
                    {
                        Sprite = (BlipSprite)326
                    });
                });
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        private async Task OnTick()
        {
            if (previewVehicle != null)
            {
                previewVehicle.IsEngineRunning = false;
                Game.PlayerPed.IsVisible = false;
            }

            if (InteractionUI.Observer.CurrentMenu == null)
            {
                if (previewVehicle != null && !isBuying)
                {
                    Game.PlayerPed.IsVisible = true;
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        Game.PlayerPed.Task.WarpOutOfVehicle(Game.PlayerPed.CurrentVehicle);
                        Game.PlayerPed.CurrentVehicle.Delete();
                    }
                    previewVehicle = null;
                }

                flushLoadedModels();
                currentLoadingCategory = "";
            }

            if(InteractionUI.Observer.CurrentMenu != null && storeMenus.ContainsValue(InteractionUI.Observer.CurrentMenu))
            {
                if (!vehiclePreviewLocation.Any(o => o.Value.Any(b => b.DistanceToSquared(Game.PlayerPed.Position) < 250.0f)))
                    InteractionUI.Observer.CloseMenu();
            }

            //if(Game.IsControlJustPressed(1, Control.Pickup) && vehiclePreviewLocation.Any(o => o.Value.Any(b => b.DistanceToSquared(Game.PlayerPed.Position) < 250.0f)))
               // InteractionUI.Observer.OpenMenu(storeMenus["VehicleStore"]);
        }

        private void OnInteraction()
        {
            var closestVehicleStore = vehiclePreviewLocation.FirstOrDefault(o => o.Value.Any(b => b.DistanceToSquared(Game.PlayerPed.Position) < 250.0f));
            if (closestVehicleStore.Value != null && InteractionUI.Observer.CurrentMenu != storeMenus[closestVehicleStore.Key])
                InteractionUI.Observer.OpenMenu(storeMenus[closestVehicleStore.Key]);
        }

        private async void requestModelsForVehicleType(string vehicleType, MenuModel vehicleCategory, string vehCategoryName)
        {
            if(loadedModels.Count >= 100)
                flushLoadedModels();

            var vehicles = StoreMenus[vehicleType].Where(o => o.vehicleCategory == vehCategoryName).ToList();
            currentLoadingCategory = vehCategoryName;
            foreach (var o in vehicles)
            {
                if (loadedModels.ContainsKey(o.modelName)) continue;
                var vehicleDisplayItem = vehicleCategory.menuItems.Find(b => b.Title.Contains(o.displayName));
                if(vehicleDisplayItem != null)
                    vehicleDisplayItem.Title = $"{o.displayName} (loading)";
                vehicleCategory.SelectedIndex = vehicleCategory.SelectedIndex;
            }
            // Two loops of vehicles are needed because the first one does initial checks quicker than the second one (even the the second one still does the same thing)
            foreach (var o in vehicles)
            {
                var vehicleDisplayItem = vehicleCategory.menuItems.Find(b => b.Title.Contains(o.displayName));
                if (vehicleDisplayItem != null)
                    vehicleDisplayItem.Title = $"{o.displayName} (loading)";

                vehicleCategory.SelectedIndex = vehicleCategory.SelectedIndex;
                if(IsModelInCdimage((uint)GetHashKey(o.modelName)))
                {
                    var vehModel = new Model(o.modelName);
                    while (!vehModel.IsLoaded)
                        await vehModel.Request(0);

                    try
                    {
                        if (!loadedModels.ContainsKey(o.modelName))
                        {
                            loadedModels.Add(o.modelName, vehModel);
                        }
                    }
                    catch (Exception e) { Log.Error(e); }

                    vehicleDisplayItem = vehicleCategory.menuItems.Find(b => b.Title.Contains(o.displayName));

                    if (vehicleDisplayItem != null)
                        vehicleDisplayItem.Title = o.displayName;
                }
                else
                {
                    if (vehicleDisplayItem != null)
                        vehicleDisplayItem.Title = $"{o.displayName} (invalid model)";
                }
                vehicleCategory.SelectedIndex = vehicleCategory.SelectedIndex;
            }

            vehicleCategory.SelectedIndex = vehicleCategory.SelectedIndex;
        }

        private async void SpawnBoughtVehicle(string vehModel, string vehPlate, string vehStoreMenu, string vehMods)
        {
            var targetVehModel = loadedModels.ContainsKey(vehModel) ? loadedModels[vehModel] : new Model(vehModel);

            while (!targetVehModel.IsLoaded)
                await targetVehModel.Request(0);

            if(Game.PlayerPed.IsInVehicle())
                Game.PlayerPed.CurrentVehicle.Delete();

            previewVehicle = null;
            var newVehicle = await VehicleDataPacker.UnpackVehicleData(JsonConvert.DeserializeObject<VehicleDataModel>(vehMods), vehicleSpawnLocations[vehStoreMenu].First(o => o.DistanceToSquared(Game.PlayerPed.Position) < 500.0f), 339);
            Game.PlayerPed.IsVisible = true;
            newVehicle.PlaceOnGround();
            newVehicle.Mods.LicensePlate = vehPlate;
            newVehicle.IsPersistent = true;
            Game.PlayerPed.Task.WarpIntoVehicle(newVehicle, VehicleSeat.Driver);
            InteractionUI.Observer.CloseMenu(true);
        }

        private void flushLoadedModels()
        {
            loadedModels.ToList().ForEach(o =>
            {
                o.Value.MarkAsNoLongerNeeded();
            });

            loadedModels = new Dictionary<string, Model>();
        }

        private void deletePreviewVehicle()
        {
            if (previewVehicle != null)
            {
                previewVehicle.MarkAsNoLongerNeeded();
                previewVehicle.Position = new Vector3(5000, 5000, 5000);
                Game.PlayerPed.CurrentVehicle.Delete();
                previewVehicle = null;
            }
        }
    }
}