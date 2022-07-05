using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Enviroment
{
    public class Stores : ServerAccessor
    {
#region Fields
        private static Dictionary<string, dynamic> generalStoreItems = new Dictionary<string, dynamic>()
        {
            ["Food items"] = new Dictionary<string, int>()
            {
                ["Beef Jerky"] = 4,
                ["Soup"] = 4,
                ["Frozen Pizza"] = 4,
                ["Frozen Peas"] = 4,
                ["Pasta"] = 4,
                ["Chocolate"] = 2,
                ["Doughnut"] = 2,
                ["EgoChaser Bar"] = 2,
                ["Meteorite Bar"] = 2,
                ["Zebra Bar"] = 3
            },
            ["Fruits / Vegetables"] = new Dictionary<string, int>()
            {
                ["Orange"] = 1,
                ["Pineapple"] = 2,
                ["Pear"] = 2,
                ["Banana"] = 1,
                ["Apple"] = 1
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Beer"] = 7,
                ["Coffee"] = 7,
                ["Sprunk Can"] = 4,
                ["eCola Can"] = 4,
                ["Apple Juice"] = 2,
                ["Silver Claw"] = 2,
                ["Orange Juice"] = 2
            },
            ["Misc items"] = new Dictionary<string, int>()
            {
                ["Cigarette"] = 3,
                ["Bobby Pin"] = 25,
                ["Ball"] = 8
            }
        };

        private static Dictionary<string, dynamic> chuteStoreItems = new Dictionary<string, dynamic>()
        {
            /*["Items"] = new Dictionary<string, int>()
            {
                ["Parachute"] = 1000,
            }*/
        };

        private static Dictionary<string, dynamic> ammuNationItems = new Dictionary<string, dynamic>()
        {
            ["Melee"] = new Dictionary<string, int>()
            {
                ["Knife"] = 30,
                ["Hammer"] = 25,
                ["Baseball Bat"] = 40,
                ["Golf Club"] = 50,
                ["Crowbar"] = 50,
                ["Nightstick"] = 100,
                ["Dagger"] = 30,
                ["Hatchet"] = 120,
                ["Pool Cue"] = 100,
                ["Wrench"] = 60
            },
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

        private static Dictionary<string, dynamic> BlackMarketStoreItems = new Dictionary<string, dynamic>()
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
                ["Shotgun ammo"] = 600,
                ["SMG ammo"] = 600,
            }
        };

        private static Dictionary<string, dynamic> fishingStoreItems = new Dictionary<string, dynamic>()
        {
            ["Rods"] = new Dictionary<string, int>()
            {
                ["Fishing rod"] = 500,
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
                ["Steak Fries"] = 3,
                ["Curly Fries"] = 3,
                ["Fries"] = 2
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Sprunk Can"] = 4,
                ["eCola Can"] = 4,
                ["Vanilla Milkshake"] = 4,
                ["Strawberry Milkshake"] = 4,
                ["Chocolate Milkshake"] = 4
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
                ["SteakFries"] = 4
            },
            ["Drinks"] = new Dictionary<string, int>()
            {
                ["Sprunk Can"] = 4,
                ["eCola Can"] = 4
            },
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

        private Dictionary<string, dynamic> itemDicts = new Dictionary<string, dynamic>
        {
            {"generalStoreItems", generalStoreItems },
            {"ammuNationItems", ammuNationItems },
            {"hardwareStoreItems", hardwareStoreItems },
            {"blackMarketStoreItems", BlackMarketStoreItems },
            {"fishingStoreItems",  fishingStoreItems},
            {"clubStoreItems", ClubStoreItems },
            {"burgerStoreItems", burgerStoreItems },
            {"tacoStoreItems", tacoStoreItems },
            {"popsStoreItems", popsStoreItems },
            {"chuteStoreItems", chuteStoreItems },
            {"firstaidStoreItems", firstaidStoreItems },
        };

        private string _cachedItemsForClient;
        private string itemsForClient
        {
            get
            {
                if (string.IsNullOrEmpty(_cachedItemsForClient))
                {
                    _cachedItemsForClient = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"generalStoreItems", JsonConvert.SerializeObject(generalStoreItems) },
                        {"ammuNationItems", JsonConvert.SerializeObject(ammuNationItems) },
                        {"hardwareStoreItems", JsonConvert.SerializeObject(hardwareStoreItems) },
                        {"blackMarketStoreItems", JsonConvert.SerializeObject(BlackMarketStoreItems) },
                        {"fishingStoreItems",  JsonConvert.SerializeObject(fishingStoreItems)},
                        {"clubStoreItems", JsonConvert.SerializeObject(ClubStoreItems) },
                        {"burgerStoreItems", JsonConvert.SerializeObject(burgerStoreItems) },
                        {"tacoStoreItems", JsonConvert.SerializeObject(tacoStoreItems) },
                        {"popsStoreitems", JsonConvert.SerializeObject(popsStoreItems) },
                        {"chuteStoreitems", JsonConvert.SerializeObject(chuteStoreItems) },
                        {"firstaidStoreItems", JsonConvert.SerializeObject(firstaidStoreItems) },
                    });
                }

                return _cachedItemsForClient;
            }
        }
        
        #endregion

        public Stores(Server server) : base(server)
        {
            server.RegisterEventHandler("Store.RequestStoreData", new Action<Player>(OnStoreRequest));
            server.RegisterEventHandler("Store.AttemptBuyItem", new Action<Player, string, string, string, int>(OnBuyItem));
        }

        private void OnBuyItem([FromSource] Player source, string store, string category, string item, int itemAmount)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if(playerSession == null) return;

            int itemPrice = itemDicts[store][category][item];

            Log.Info($"{store}->{category}->{item}");
            var payHandler = Server.Get<PaymentHandler>();
            if(payHandler.CanPayForItem(playerSession, itemPrice, itemAmount))
            {
                var playerInv = new PlayerInventory(playerSession.GetGlobalData("Character.Inventory", ""), playerSession);
                if(playerInv.CanStoreItem(item, itemAmount))
                {
                    payHandler.PayForItem(playerSession, itemPrice * itemAmount, $"buying {itemAmount}x {item}");

                    if (item.Contains("ammo"))
                    {
                        itemAmount = 50;
                    }
                    playerInv.AddItem(item, itemAmount);
                    Log.ToClient("[Store]", $"You just bought {itemAmount}x {item}", ConstantColours.Store, source);
                }
                else
                {
                    Log.ToClient("[Inventory]", "You do not have enough space to carry these item(s)", ConstantColours.Inventory, source);
                }
            }
            else
            {
                Log.ToClient("[Bank]", "You don't have enough money to but this item", ConstantColours.Bank, source);
            }
        }

        private void OnStoreRequest([FromSource] Player source)
        {
            /*var storeData = new Dictionary<string, dynamic>();
            var storeFields = GetType().GetProperties().Where(o => o.Name.Contains("Items"));
            storeFields.ToList().ForEach(o =>
            {
                Log.Info(o.Name);
                storeData.Add(o.Name, o.GetValue(this));
            });
            var storeData = new Dictionary<string, string>
            {
                {"generalStoreItems", JsonConvert.SerializeObject(generalStoreItems) },
                {"ammuNationItems", JsonConvert.SerializeObject(ammuNationItems) },
                {"hardwareStoreItems", JsonConvert.SerializeObject(hardwareStoreItems) },
                {"blackMarketStoreItems", JsonConvert.SerializeObject(blackMarketStoreItems) },
                {"fishingStoreItems",  JsonConvert.SerializeObject(fishingStoreItems)},
                {"clubStoreItems", JsonConvert.SerializeObject(clubStoreItems) }
            };*/

            source.TriggerEvent("Store.BuildStoreMenus", itemsForClient);
        }
    }
}
