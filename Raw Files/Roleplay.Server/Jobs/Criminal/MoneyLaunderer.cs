using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.Criminal
{
    public class MoneyLaunderer : JobClass
    {
        private Vector3 laundererLocation = new Vector3(1123.14f, -3194.91f, -41.37f);

        public MoneyLaunderer()
        {
            Server.RegisterEventHandler("Player.OnInteraction", new Action<Player>(OnInteraction));
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            playerSession.TriggerEvent("Markers.AddMarker", laundererLocation.ToArray(), JsonConvert.SerializeObject(new MarkerOptions
            {
                ColorArray = ConstantColours.Red.ToArray()
            }));
        }

        private void OnInteraction([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var playerPos = playerSession.GetPlayerPosition();

            if (playerPos.DistanceToSquared(laundererLocation) < 3.0f)
            {
                startMoneyClean(playerSession);
            }
        }

        private async void startMoneyClean(Session.Session playerSession)
        {
            var playerInv = playerSession./*GetInventory()*/Inventory;
            var dirtyMoneyItem = playerInv.GetItem("DirtyMoney");

            if (dirtyMoneyItem != null)
            {
                var dirtyMoneyAmount = dirtyMoneyItem.itemAmount;

                if (dirtyMoneyAmount > 0)
                {
                    var currentSecond = 0;
                    Log.ToClient("[Laundry]", $"You begin cleaning your money", ConstantColours.Criminal, playerSession.Source);
                    while(currentSecond != 12)
                    {
                        await BaseScript.Delay(1000);
                        var playerPos = playerSession.GetPlayerPosition();
                        if (playerPos.DistanceToSquared(laundererLocation) > 3.0f)
                        {
                            Log.ToClient("[Laundry]", $"You moved too far away to launder the money", ConstantColours.Criminal, playerSession.Source);
                            return;
                        }
                        currentSecond++;
                    }

                    playerInv = playerSession./*GetInventory()*/Inventory;
                    dirtyMoneyItem = playerInv.GetItem("DirtyMoney");
                    if(dirtyMoneyItem != null && dirtyMoneyItem.itemAmount == dirtyMoneyAmount)
                    {
                        playerInv.AddItem(dirtyMoneyItem, -dirtyMoneyAmount);
                        Server.Get<PaymentHandler>().UpdatePlayerCash(playerSession, (int)(dirtyMoneyAmount * 0.85), "laundering dirty money");
                        Log.ToClient("[Laundry]", $"You just got ${dirtyMoneyAmount} in clean money", ConstantColours.Criminal, playerSession.Source);
                    }
                    else
                    {
                        Log.ToClient("[Laundry]", $"What you trying to pull here, huh?", ConstantColours.Criminal, playerSession.Source);
                    }

                    return;
                }
            }

            Log.ToClient("[Laundry]", $"You don't have any money to clean", ConstantColours.Criminal, playerSession.Source);
        }
    }
}
