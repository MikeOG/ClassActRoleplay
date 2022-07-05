using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using Roleplay.Client.Enums;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Locations;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using MenuFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.UI.Store
{
    internal class StoreMenu : ClientAccessor
    {
        #region Variables
        private static Dictionary<string, dynamic> generalStoreItems = new Dictionary<string, dynamic>()
        {
            ["Food items"] = new Dictionary<string, int>()
            {
                ["Doughnut"] = 2,
                ["Beef jerky"] = 4,
                ["Chocolate"] = 2
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Beer"] = 4,
                ["Vodka"] = 10
            },
            ["Misc items"] = new Dictionary<string, int>()
            {
                ["Cigarette"] = 3,
                ["Bobby Pin"] = 25,
            }
        };

        private static Dictionary<string, dynamic> ammuNationItems = new Dictionary<string, dynamic>()
        {
            ["Weapons"] = new Dictionary<string, int>()
            {
                ["Knife"] = 150,
                ["Pistol"] = 1000,
                ["Vintage Pistol"] = 1500,
                ["SNS Pistol"] = 1255,
                ["Colt python"] = 3000,
                [".50 Pistol"] = 2222,
                ["SPAS-12"] = 4995,
                ["Musket"] = 5000
            },
            ["Ammunition"] = new Dictionary<string, int>()
            {
                ["Pistol ammo"] = 1200,
                ["Shotgun ammo"] = 1600,
            }
        };

        private static Dictionary<string, dynamic> hardwareStoreItems = new Dictionary<string, dynamic>()
        {
            ["Items"] = new Dictionary<string, int>()
            {
                ["Hammer"] = 30,
                ["Bat"] = 45,
                ["Flashlight"] = 25,
                ["Petrol can"] = 50,
                ["Radio"] = 75,
                ["Repair kit"] = 250,
                ["Zipties"] = 150,
            }
        };

        private static Dictionary<string, dynamic> blackMarketStoreItems = new Dictionary<string, dynamic>()
        {
            ["Melee"] = new Dictionary<string, int>()
            {
                ["Switchblade"] = 500,
                ["Machete"] = 750,
                ["Brass knuckles"] = 1000,
            },
            ["Pistols"] = new Dictionary<string, int>()
            {
                ["AP Pistol"] = 25000,
            },
            ["Shotguns"] = new Dictionary<string, int>()
            {
                ["SPAS-12"] = 50000,
                ["Sawn-off shotgun"] = 50000,
            },
            ["SMGs"] = new Dictionary<string, int>()
            {
                ["TEC-9"] = 40000,
                ["Mini uzi"] = 40000,
                ["Mini smg"] = 40000,
            },
            ["Assault Rifles"] = new Dictionary<string, int>()
            {
                ["Type 56"] = 95000,
                ["AK-47u"] = 100000,
            },
            ["Throwables"] = new Dictionary<string, int>()
            {
                ["Molotov(s)"] = 55000,
            },
            ["Misc items"] = new Dictionary<string, int>()
            {
                ["Lockpick"] = 100,
                //["Light armour"] = 2500,
                ["Cuff lockpick"] = 1500,
            },
            ["Ammunition"] = new Dictionary<string, int>()
            {
                ["Rifle ammo"] = 1200,
                ["Pistol ammo"] = 1200,
                ["Shotgun ammo"] = 1600,
                ["SMG ammo"] = 600,
            }
        };

        private static Dictionary<string, dynamic> fishingStoreItems = new Dictionary<string, dynamic>()
        {
            ["Rods"] = new Dictionary<string, int>()
            {
                ["Fishing rod"] = 500,
                ["Professional rod"] = 1500,
                ["Expert rod"] = 3000
            }
        };

        private static Dictionary<string, dynamic> ClubStoreItems = new Dictionary<string, dynamic>()
        {
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Beer"] = 3,
                ["Red Wine"] = 6,
                ["White Wine"] = 7,
                ["Brandy"] = 5,
                ["Rum"] = 5,
                ["Gin"] = 8,
                ["Vodka"] = 8
            }
        };

        private static Dictionary<string, dynamic> burgerStoreItems = new Dictionary<string, dynamic>()
        {
            ["Burgers / Fries"] = new Dictionary<string, int>()
            {
                ["Heart Stopper Burger"] = 6,
                ["Bleeder Burger"] = 4,
                ["Money Shot Burger"] = 7,
                ["Torpedo Burger"] = 7,
                ["Meat Free Burger"] = 5,
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Sprunk Can"] = 4,
                ["eCola Can"] = 4,
                ["Vanilla Milkshake"] = 4,
                ["Strawberry Milkshake"] = 4,
                ["Chocolate Milkshake"] = 4,
            },
        };

        private static Dictionary<string, dynamic> tacoStoreItems = new Dictionary<string, dynamic>()
        {
            ["Taco / Fries"] = new Dictionary<string, int>()
            {
                ["Taco"] = 6,
                ["Burrito"] = 4,
                ["Quesadilla"] = 7,
                ["Fries"] = 7,
                ["SteakFries"] = 4,
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Sprunk Can"] = 4,
                ["eCola Can"] = 4,
            },
        };

        private static Dictionary<string, dynamic> chuteStoreItems = new Dictionary<string, dynamic>()
        {
            /*["Items"] = new Dictionary<string, int>()
            {
                ["Parachute"] = 1000,
            }*/
        };

        private static Dictionary<string, dynamic> firstaidStoreItems = new Dictionary<string, dynamic>()
        {
            ["Healing items"] = new Dictionary<string, int>()
            {
                ["Bandage"] = 50,
                ["First aid kit"] = 150
            },
        };

        private static Dictionary<string, dynamic> popsStoreItems = new Dictionary<string, dynamic>()
        {
            ["Food"] = new Dictionary<string, int>()
            {
                ["HamBurger"] = 16,
                ["Cheese Burger"] = 14,
                ["Double Cheese Burger"] = 17,
                ["Bacon Cheese Burger"] = 17,
                ["Black Bean Burger"] = 17,
                ["Black and Blue Burger"] = 14,
                ["Fries"] = 13,
                ["Tots"] = 12,
                ["Jello"] = 12,
                ["Mozzarella Sticks"] = 13,
                ["Onion Rings"] = 14,
                ["Chicken Wings"] = 15,
                ["Chicken Tendies"] = 12,
                ["Chicken Club Sandwich"] = 13,
                ["Grilled Chicken Sandwich"] = 12,
                ["BLT"] = 12,
                ["Chili Cheese Dog"] = 13,
                ["Hot Dog"] = 14,
                ["Tuna Melt"] = 15,
                ["Grilled Cheese"] = 15,
                ["Philly Cheese Steak"] = 20,
                ["Cobbs Salad"] = 13,
                ["Chef Salad"] = 12,
                ["Garden Salad"] = 12,
                ["Chicken Salad"] = 13,
                ["Steak Salad"] = 14,
                ["Hot Fudge Sundae"] = 12,
                ["Banana Split"] = 12,
                ["Single Scoop Ice Cream"] = 12,
                ["Apple Pie"] = 12,

            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Coffee"] = 10,
                ["Chocolate Shake"] = 12,
                ["Vanilla Shake"] = 12,
                ["Strawberry Shake"] = 12,
                ["Banana Shake"] = 12,
                ["Peanut Butter Shake"] = 12,
                ["Cookie Shake"] = 12,
            },
        };

        private Dictionary<string, Tuple<string, dynamic, MenuModel>> storeMenuData = new Dictionary<string, Tuple<string, dynamic, MenuModel>>()
        {
            ["general"] = new Tuple<string, dynamic, MenuModel>("General store", generalStoreItems, null),
            ["ammunation"] = new Tuple<string, dynamic, MenuModel>("Ammu Nation", ammuNationItems, null),
            ["hardware"] = new Tuple<string, dynamic, MenuModel>("Hardware store", hardwareStoreItems, null),
            ["blackmarket"] = new Tuple<string, dynamic, MenuModel>("Black market", blackMarketStoreItems, null),
            ["fishing"] = new Tuple<string, dynamic, MenuModel>("Fishing store", fishingStoreItems, null),
            ["club"] = new Tuple<string, dynamic, MenuModel>("Bar", ClubStoreItems, null),
            ["burger"] = new Tuple<string, dynamic, MenuModel>("Burger Shot", burgerStoreItems, null),
            ["taco"] = new Tuple<string, dynamic, MenuModel>("Taco Shack", tacoStoreItems, null),
            ["pops"] = new Tuple<string, dynamic, MenuModel>("Pops Diner", popsStoreItems, null),
            ["Chute"] = new Tuple<string, dynamic, MenuModel>("Parachute Shop", chuteStoreItems, null),
            ["First Aid"] = new Tuple<string, dynamic, MenuModel>("First Aid Shop", firstaidStoreItems, null),
        };
        private Dictionary<string, MenuModel> storeUIMenus = new Dictionary<string, MenuModel>();
        #endregion

        #region Init
        public StoreMenu(Client client) : base(client)
        {
            Client.RegisterEventHandler("Player.OnLoginComplete", new Action(OnLogin));
            Client.RegisterEventHandler("Player.CheckForInteraction", new Action(() =>
            {
                foreach (var i in storeUIMenus.Keys)
                {
                    if (IsNearStoreType(i))
                    {
                        InteractionUI.Observer.OpenMenu(storeUIMenus[i]);
                    }
                }
            }));

            Client.RegisterEventHandler("Player.OnLoginComplete", new Action(() => Client.TriggerServerEvent("Store.RequestStoreData")));
            Client.RegisterEventHandler("Store.BuildStoreMenus", new Action<string>(OnReceiveMenus));
            Client.RegisterTickHandler(OnTick);
        }
        #endregion

        #region Methods

        private void OnReceiveMenus(string data)
        {
            var menus = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            foreach (var store in menus)
            {
                var menuObjects = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(store.Value);
                var menuData = menuObjects.ToDictionary(o => o.Key, o => (dynamic)o.Value.ToObject<Dictionary<string, int>>());

                if (store.Key == "generalStoreItems")
                {
                    generalStoreItems = menuData;
                }
                else if (store.Key == "ammuNationItems")
                {
                    ammuNationItems = menuData;
                }
                else if (store.Key == "hardwareStoreItems")
                {
                    hardwareStoreItems = menuData;
                }
                else if (store.Key == "blackMarketStoreItems")
                {
                    blackMarketStoreItems = menuData;
                }
                else if (store.Key == "fishingStoreItems")
                {
                    fishingStoreItems = menuData;
                }
                else if (store.Key == "clubStoreItems")
                {
                    ClubStoreItems = menuData;
                }
                else if (store.Key == "burgerStoreItems")
                {
                    burgerStoreItems = menuData;
                }
                else if (store.Key == "tacoStoreItems")
                {
                    tacoStoreItems = menuData;
                }
                else if (store.Key == "popsStoreItems")
                {
                    popsStoreItems = menuData;
                }
                else if (store.Key == "chuteStoreItems")
                {
                    chuteStoreItems = menuData;
                }
                else if (store.Key == "firstaidStoreItems")
                {
                    firstaidStoreItems = menuData;
                }
            }

            storeMenuData = new Dictionary<string, Tuple<string, dynamic, MenuModel>>()
            {
                ["generalStoreItems"] = new Tuple<string, dynamic, MenuModel>("General store", generalStoreItems, null),
                ["ammuNationItems"] = new Tuple<string, dynamic, MenuModel>("Ammu Nation", ammuNationItems, null),
                ["hardwareStoreItems"] = new Tuple<string, dynamic, MenuModel>("Hardware store", hardwareStoreItems, null),
                ["blackMarketStoreItems"] = new Tuple<string, dynamic, MenuModel>("Black market", blackMarketStoreItems, null),
                ["fishingStoreItems"] = new Tuple<string, dynamic, MenuModel>("Fishing store", fishingStoreItems, null),
                ["clubStoreItems"] = new Tuple<string, dynamic, MenuModel>("Club", ClubStoreItems, null),
                ["burgerStoreItems"] = new Tuple<string, dynamic, MenuModel>("Burger Shot", burgerStoreItems, null),
                ["tacoStoreItems"] = new Tuple<string, dynamic, MenuModel>("Taco Shack", tacoStoreItems, null),
                ["popsStoreItems"] = new Tuple<string, dynamic, MenuModel>("Pops Diner", popsStoreItems, null),
                ["chuteStoreItems"] = new Tuple<string, dynamic, MenuModel>("Parachute Store", chuteStoreItems, null),
                ["firstaidStoreItems"] = new Tuple<string, dynamic, MenuModel>("First Aid Vending", firstaidStoreItems, null)
            };

            LoadStoreMenus();
        }

        private void LoadStoreMenus()
        {
            foreach (var i in storeMenuData)
            {
                var storeMenu = new MenuModel() { headerTitle = i.Value.Item1 };
                CreateStoreMenu(i.Value.Item2, storeMenu, i.Key);
                Client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
                {
                    Title = i.Value.Item1,
                    SubMenu = storeMenu
                }, () => IsNearStoreType(i.Key), 500);
                storeUIMenus[i.Key] = storeMenu;
            }
        }

        private void CreateStoreMenu(Dictionary<string, dynamic> storeData, MenuModel parentMenu, string baseMenuName)
        {
            List<MenuItem> storeSubMenus = new List<MenuItem>();
            foreach (var i in storeData.Keys)
            {
                Dictionary<string, int> storeCategory = storeData[i];
                List<MenuItem> subMenuData = new List<MenuItem>();
                foreach (var b in storeCategory.Keys)
                    subMenuData.Add(new MenuItemStandard
                    {
                        Title = $"{b}",
                        Detail = $"(${storeCategory[b]})",
                        OnActivate = async state =>
                        {
                            //InteractionUI.Observer.CloseMenu(true);
                            var amountToUse = 1;
                            //if (Int32.TryParse(await Game.GetUserInput(4), out amountToUse))
                            //{
                                //InteractionUI.Observer.OpenMenu(InteractionUI.InteractionMenu);
                                //InteractionUI.Observer.OpenMenu(parentMenu);
                                if (amountToUse < 0)
                                    amountToUse *= -1;

                                //TriggerEvent("buyStoreItem", b, amountToUse, storeCategory[b]);
                                Client.TriggerServerEvent("Store.AttemptBuyItem", baseMenuName, i, b, amountToUse); // store, category, item, amount
                            //}
                        },
                    });
                storeSubMenus.Add(new MenuItemSubMenu
                {
                    Title = i,
                    SubMenu = new MenuModel
                    {
                        headerTitle = i,
                        menuItems = subMenuData
                    }
                });
            }
            parentMenu.menuItems = storeSubMenus;
        }

        private bool IsNearStoreType(string storeType)
        {
            bool nearStore = false;
            if (storeType == "generalStoreItems")
                GeneralStores.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(1.75, 2))
                        nearStore = true;
                });
            else if(storeType == "ammuNationItems")
                AmmuNation.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(1.75, 2))
                        nearStore = true;
                });
            else if(storeType == "hardwareStoreItems")
                HardwareStores.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(1.75, 2))
                        nearStore = true;
                });
            else if(storeType == "blackMarketStoreItems")
                BlackMarket.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(1.75, 2))
                        nearStore = true;
                });
            else if (storeType == "fishingStoreItems")
                Fishing.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(1.75, 2))
                        nearStore = true;
                });
            else if(storeType == "clubStoreItems")
                Clubs.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            else if (storeType == "tacoStoreItems")
                Taco.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            else if (storeType == "popsStoreItems")
                Pops.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            else if (storeType == "chuteStoreItems")
                Chute.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            else if(storeType == "burgerStoreItems")
                Burger.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            else if (storeType == "firstaidStoreItems")
                FirstAid.Positions.ForEach(o =>
                {
                    if (o.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                        nearStore = true;
                });
            return nearStore;
        }

        private async Task OnTick()
        {
            if (storeUIMenus.ContainsValue(InteractionUI.Observer.CurrentMenu))
            {
                bool isNearAnyStore = false;
                foreach (var i in storeUIMenus.Keys)
                {
                    if (IsNearStoreType(i))
                        isNearAnyStore = true;

                    if (InteractionUI.Observer.CurrentMenu == storeUIMenus[i] && !isNearAnyStore)
                    {
                        InteractionUI.Observer.CloseMenu();
                    } 
                }
            }
        }

        private async void OnLogin()
        {
            // General store
            await BlipHandler.AddBlipAsync("General store", GeneralStores.Positions, new BlipOptions
            {
                Sprite = BlipSprite.Store
            });
            await MarkerHandler.AddMarkerAsync(GeneralStores.Positions);
            // Hardware store
            await BlipHandler.AddBlipAsync("Hardware store", HardwareStores.Positions, new BlipOptions
            {
                Sprite = BlipSprite.Repair
            });
            await MarkerHandler.AddMarkerAsync(HardwareStores.Positions);

            // Ammu nation
            await BlipHandler.AddBlipAsync("Ammu Nation", AmmuNation.Positions, new BlipOptions
            {
                Sprite = BlipSprite.AmmuNation,
                Colour = (BlipColor)6
            });
            await MarkerHandler.AddMarkerAsync(AmmuNation.Positions);

            // Fishing store
            await BlipHandler.AddBlipAsync("Fishing store", Fishing.Positions, new BlipOptions
            {
                Sprite = BlipSprite.Marina,
            });
            await MarkerHandler.AddMarkerAsync(Fishing.Positions);

            // Hospitals
            await BlipHandler.AddBlipAsync("Hospital", Hospital.Positions, new BlipOptions
            {
                Sprite = BlipSprite.Hospital,
            });

            // Police Stations 
            await BlipHandler.AddBlipAsync("Police Station", PoliceStations.Positions, new BlipOptions
            {
                Sprite = BlipSprite.PoliceStation,
            });

            // Repair Stations 
            await BlipHandler.AddBlipAsync("Mechanic Shops", Mechanic.Positions, new BlipOptions
            {
                Sprite = BlipSprite.LosSantosCustoms,
            });

            // Clubs
            await MarkerHandler.AddMarkerAsync(Clubs.Positions, new MarkerOptions
            {
                ScaleFloat = 3.0f
            });

            // Taco Shack
            await MarkerHandler.AddMarkerAsync(Taco.Positions, new MarkerOptions
            {
                Color = ConstantColours.Red
            });


            // Pops Diner
            await MarkerHandler.AddMarkerAsync(Pops.Positions, new MarkerOptions
            {
                Color = ConstantColours.Red
            });

            // Pops Diner
            await MarkerHandler.AddMarkerAsync(Chute.Positions, new MarkerOptions
            {
                Color = ConstantColours.Red
            });

            // Black market
            await MarkerHandler.AddMarkerAsync(BlackMarket.Positions, new MarkerOptions
            {
                Color = ConstantColours.Red
            });
            // Black market
            await MarkerHandler.AddMarkerAsync(FirstAid.Positions, new MarkerOptions
            {
                Color = ConstantColours.Red
            });
        }
        #endregion
    }
}
