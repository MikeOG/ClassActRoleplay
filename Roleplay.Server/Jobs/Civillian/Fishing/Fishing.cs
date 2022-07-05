using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using static CitizenFX.Core.Native.API;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.Civillian.Fishing
{
    public class Fishing : JobClass
    {
        private static Random rand => new Random((int)(GetGameTimer() * DateTime.Now.Ticks));
        private int timePerFish => rand.Next(GetConvarInt("mg_fishingTimeLower", 6), GetConvarInt("mg_fishingTimeUpper", 10));
        private Dictionary<string, Func<bool>> fishChances = new Dictionary<string, Func<bool>>
        {
            {"bcatfish", () => rand.NextBool(GetConvarInt("mg_bcatfishChance", 45)) },
            {"bullhead", () => rand.NextBool(GetConvarInt("mg_bullheadChance", 25)) },
            {"gsturgeon", () => rand.NextBool(GetConvarInt("mg_gsturgeonChance", 15)) },
        };
        private Dictionary<string, int> fishSellPrices = new Dictionary<string, int>
        {
            {"bcatfish", GetConvarInt("mg_bcatfishSellPrice", 40) },
            {"bullhead", GetConvarInt("mg_bullheadSellPrice", 75) },
            {"gsturgeon", GetConvarInt("mg_gsturgeonSellPrice", 110) },
        };

        //private Vector3 fishingLocation = new Vector3(-1858.74f, -1243.31f, 8.62f);
        //private Vector3 fishSellLocation = new Vector3(-3275.62f, 965.20f, 8.35f);

        private List<Vector3> fishingLocations = new List<Vector3>();
        private List<Vector3> fishSellLocations = new List<Vector3>
        {
            new Vector3(-3275.62f, 965.20f, 7.45f),
            new Vector3(1309.38f, 4362.07f, 40.6f),
            new Vector3(-55f, 6523f, 30.50f),
        };

        public Fishing()
        {
            Server.RegisterEventHandler("Player.OnInteraction", new Action<Player>(OnInteraction));
            Server.RegisterEventHandler("Fishing.CancelFishing", new Action<Player>(OnCancelFishing));
            generateFishingLocations();
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            playerSession.AddBlip("Fish seller", fishSellLocations, new BlipOptions
            {
                Sprite = 356
            });
            playerSession.AddMarker(fishSellLocations, new MarkerOptions
            {
                ScaleFloat = 2.0f,
                ColorArray = ConstantColours.Yellow.ToArray()
            });

            playerSession.AddBlip("Fishing pier", fishingLocations, new BlipOptions
            {
                Sprite = 316
            });
        }

        public bool InFishingZone(Session.Session playerSession)
        {
            return playerSession.GetPlayerPosition().IsInPolygon(FishingPolygons.Polygons);
        }

        private void OnInteraction([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null || playerSession.IsInVehicle()) return;
            
            var inFishingZone = InFishingZone(playerSession);
            var playerInv = playerSession./*GetInventory()*/Inventory;
            var canCarryFish = fishChances.Any(o => playerInv.CanStoreItem(o.Key, 1));
            var hasRod = playerInv.HasItem("FishingRod");
            Log.Debug($"{source.Name} in a fishing polygon - {inFishingZone}");

            if(inFishingZone)
            {
                var isFishing = playerSession.GetLocalData("Character.IsFishing", false);

                if (isFishing)
                {
                    playerSession.SetLocalData("Character.IsFishing", false);
                    Log.ToClient("[Fishing]", $"Looks like your fingers slipped", ConstantColours.Job, playerSession.Source);
                }

                if (!isFishing && canCarryFish && hasRod)
                {
                    playerSession.SetLocalData("Character.IsFishing", true);
                    StartFishing(playerSession);
                }
                else if (!canCarryFish)
                {
                    Log.ToClient("[Fishing]", $"You cannot carry anymore fish", ConstantColours.Job, playerSession.Source);
                }
                else if (!hasRod)
                {
                    Log.ToClient("[Fishing]", $"You need a fishing rod to fish", ConstantColours.Job, playerSession.Source);
                }

                return;
            }

            var playerPos = playerSession.GetPlayerPosition();

            if (/*playerPos.DistanceToSquared(fishSellLocation) <= 6.0f*/ fishSellLocations.Any(o => o.DistanceToSquared(playerPos) <= 6.0f))
            {   
                StartFishSelling(playerSession);
                Log.Verbose($"{playerSession.PlayerName} is about to sell fish");
            }   
        }

        private void StartFishing(Session.Session playerSession)
        {
            Log.Verbose($"Starting fishing for {playerSession.PlayerName}");
            playerSession.TriggerEvent("Fishing.StartFishing");

            Task.Factory.StartNew(async () =>
            {
                var currentSecond = timePerFish;
                var currentTick = 0;
                while (playerSession.GetLocalData("Character.IsFishing", false))
                {
                    if (!InFishingZone(playerSession) || playerSession.IsInVehicle())
                    {
                        playerSession.SetLocalData("Character.IsFishing", false);
                        Log.ToClient("[Fishing]", "You moved to far away from the fishing area", ConstantColours.Job, playerSession.Source);
                        Log.Debug($"{playerSession.PlayerName} no longer in a fishing zone. Terminating thread");
                        return;
                    }

                    if (currentTick >= 1000)
                    {
                        currentTick = 0;
                        currentSecond -= 1;

                        if (playerSession.GetLocalData("Character.IsFishing", false) && currentSecond <= 0) // Make sure no change
                        {
                            AttemptAddFish(playerSession);
                            playerSession.SetLocalData("Character.IsFishing", false);
                            /*await BaseScript.Delay(2000);
                            currentSecond = timePerFish;*/
                        }
                    }
                        
                    await BaseScript.Delay(100);
                    currentTick += 100;
                }
            });
        }

        private void AttemptAddFish(Session.Session playerSession)
        {
            var playerInv = playerSession./*GetInventory()*/Inventory;
            var caughtFish = getRandomFish();

            if (caughtFish != "none")
            {
                if (playerInv.CanStoreItem(caughtFish, 1))
                {
                    Log.ToClient("[Fishing]", $"You caught a {InventoryItems.GetInvItemData(caughtFish).itemName}", ConstantColours.Job, playerSession.Source);
                    playerInv.AddItem(caughtFish, 1);
                }
                else
                {
                    Log.ToClient("[Fishing]", $"You cannot carry anymore fish", ConstantColours.Job, playerSession.Source);
                    playerSession.SetLocalData("Character.IsFishing", false);
                }
            }
            else
            {
                Log.ToClient("[Fishing]", "You caught nothing", ConstantColours.Job, playerSession.Source);
            }
        }

        private void OnCancelFishing([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            playerSession.SetLocalData("Character.IsFishing", false);
        }

        private string getRandomFish()
        {
            var fishes = fishChances.Where(o => o.Value()).ToList().Shuffle();
            //Log.Info(fishes.Count.ToString());
            return fishes.Count > 0 ? fishes[rand.Next(0, fishes.Count - 1)].Key : "none";
        }

        private async void StartFishSelling(Session.Session playerSession)
        {
            var playerInv = playerSession./*GetInventory()*/Inventory;
            var playerFishes = fishChances.Where(o => playerInv.HasItem(o.Key)).Select(o => o.Key);

            if (playerFishes.Any())
            {
                Log.ToClient("[Fishing]", "Selling in progress", ConstantColours.Job, playerSession.Source);
                await BaseScript.Delay(2500);

                if (/*playerSession.Position.DistanceToSquared(fishSellLocation) <= 6.0f*/ fishSellLocations.Any(o => o.DistanceToSquared(playerSession.Position) <= 6.0f))
                {
                    playerInv = playerSession./*GetInventory()*/Inventory; // Refresh inv

                    var sellAmount = 0;
                    foreach (var fish in playerFishes)
                    {
                        if (playerInv.HasItem(fish))
                        {
                            var invItem = playerInv.GetItem(fish);
                            var itemAmount = invItem.itemAmount;
                            playerInv.AddItem(invItem, -invItem.itemAmount);
                            sellAmount += itemAmount * fishSellPrices[fish];
                            Log.Debug($"{playerSession.PlayerName} is selling {itemAmount} {invItem.itemName}");
                        }
                    }

                    if (sellAmount != 0 && sellAmount > 0)
                    {
                        Server.Get<PaymentHandler>().UpdatePlayerCash(playerSession, sellAmount, "fish selling");
                        Log.ToClient("[Fishing]", $"You just sold all your fish for ${sellAmount}", ConstantColours.Job, playerSession.Source);
                    }
                }
                else
                {
                    Log.ToClient("[Fishing]", "You moved to far away from the seller", ConstantColours.Job, playerSession.Source);
                }
            }
            else
            {
                Log.ToClient("[Fishing]", "You don't have any fish to sell", ConstantColours.Job, playerSession.Source);
            }
        }

        private void generateFishingLocations()
        {
            foreach (var polygon in FishingPolygons.Polygons)
            {
                var xTotal = 0.0f;
                var yTotal = 0.0f;

                foreach (var point in polygon)
                {
                    xTotal += point[0];
                    yTotal += point[1];
                }

                xTotal /= polygon.Length;
                yTotal /= polygon.Length;

                var vec = new Vector3(xTotal, yTotal, 0.0f);
                
                Log.Debug($"Adding location {vec} as a fishing locaiton blip");
                fishingLocations.Add(vec);
            }
        }
    }
}
