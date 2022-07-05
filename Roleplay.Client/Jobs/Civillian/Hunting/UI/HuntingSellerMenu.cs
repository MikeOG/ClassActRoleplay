using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using MenuFramework;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs;
using Roleplay.Client.Models;
using Roleplay.Client.UI;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.Civillian.Hunting.UI
{
    internal class HuntingSellerMenu : MenuModel
    {
        private List<MenuItem> _menuItems = new List<MenuItem>();
        public Vector3 sellerLocation = new Vector3(-841.222f, 5401.401f, 33.615f);
        public IDictionary<string, object> sellingPrices;

        public HuntingSellerMenu()
        {
            headerTitle = "Hunting seller";
            statusTitle = "";
            Client.Instance.TriggerServerEvent("Hunting.RequestPrices");
            BlipHandler.AddBlip("Hunting seller", sellerLocation, new BlipOptions
            {
                Sprite = BlipSprite.DollarSignSquared
            });
            //createSellingOptions();
        }

        public void createSellingOptions()
        {
            _menuItems.Clear();
            Hunting.HuntableModels.ForEach(o =>
            {
                string animal = o.ToString();
                List<string> extraOptions = HuntingLootMenu.getExtraAnimalRewards(animal);
                HuntingLootMenu.lootableRewards.ForEach(item =>
                {
                    _menuItems.Add(new MenuItemStandard
                    {
                        Title = $"Sell {animal.AddSpacesToCamelCase()} {item} (${sellingPrices[animal + "_" + item]})",
                        OnActivate = async state =>
                        {
                            InteractionUI.Observer.CloseMenu(true);
                            int amountToUse;
                            if (Int32.TryParse(await Game.GetUserInput(4), out amountToUse))
                            {
                                InteractionUI.Observer.OpenMenu(InteractionUI.InteractionMenu);
                                InteractionUI.Observer.OpenMenu(this);
                                if (amountToUse < 0)
                                    amountToUse *= -1;
                                //BaseScript.TriggerEvent("sendNotifyEvent", $"You would be selling {amountToUse} {animal.AddSpacesToCamelCase()} {item}");
                                Client.Instance.TriggerServerEvent("Hunting.AttemptSellItems", animal + "_" + item, amountToUse);
                                //BaseScript.TriggerServerEvent("addToHuntingItems", animal + "_" + item, amountToUse);
                            }
                        }
                    });
                });
                extraOptions.ForEach(item =>
                {
                    _menuItems.Add(new MenuItemStandard
                    {
                        Title = $"Sell {animal.AddSpacesToCamelCase()} {item} (${sellingPrices[animal + "_" + item]})",
                        OnActivate = async state =>
                        {
                            InteractionUI.Observer.CloseMenu(true);
                            int amountToUse;
                            if (Int32.TryParse(await Game.GetUserInput(4), out amountToUse))
                            {
                                InteractionUI.Observer.OpenMenu(InteractionUI.InteractionMenu);
                                InteractionUI.Observer.OpenMenu(this);
                                if (amountToUse < 0)
                                    amountToUse *= -1;
                                //BaseScript.TriggerEvent("sendNotifyEvent", $"You would be selling {amountToUse} {animal.AddSpacesToCamelCase()} {item}");
                                Client.Instance.TriggerServerEvent("Hunting.AttemptSellItems", animal + "_" + item, amountToUse);
                                //BaseScript.TriggerServerEvent("addToHuntingItems", animal + "_" + item, amountToUse);
                            }
                        }
                    });
                });
            });
            menuItems = _menuItems;
            SelectedIndex = SelectedIndex;
        }

        public bool isInRangeOfSeller()
        {
            if (sellerLocation.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2))
                return true;
            else
                return false;
        }
    }
}
