using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs.Civillian.Hunting.UI;
using Roleplay.Client.UI.Jobs;
using MenuFramework;
using Roleplay.Client.UI;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.Civillian.Hunting
{
    internal class Hunting : ClientAccessor
    {
        private const float lootableDistance = 3f;

        public static List<PedHash> HuntableModels = new List<PedHash>()
        {
            PedHash.Coyote,
           // PedHash.Chimp,
           // PedHash.ChickenHawk,
            //PedHash.Hen,
         //   PedHash.Boar,
           // PedHash.Chop,
            //PedHash.Cormorant,
           // PedHash.Cow,
           // PedHash.Crow,
            PedHash.Deer,
           // PedHash.Fish,
           // PedHash.Husky,
            PedHash.MountainLion,
           // PedHash.Pig,
           // PedHash.Pigeon,
           // PedHash.Rat,
          //  PedHash.Retriever,
          //  PedHash.Rhesus,
          //  PedHash.Rottweiler,
          //  PedHash.Seagull,
            //PedHash.TigerShark,
           // PedHash.Shepherd,
            //PedHash.HammerShark,
            PedHash.Rabbit,
           // PedHash.Cat,
            //PedHash.KillerWhale
        };

        private static PedList PedList = new PedList();
        private static List<Ped> NearbyAnimals = new List<Ped>();
        private static bool nextToDeadAnimal = false;
        private static HuntingLootMenu lootingMenu;
        private static HuntingSellerMenu sellingMenu;

        public Hunting(Client client) : base(client)
        {
            Function.Call(Hash.DECOR_REGISTER, "Animal.Looted");
            client.RegisterTickHandler(OnTick);
            client.RegisterTickHandler(RefreshAnimals);
            lootingMenu = new HuntingLootMenu();
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                Title = "Hunting looting",
                SubMenu = lootingMenu
            }, () => nextToDeadAnimal, 250);
            sellingMenu = new HuntingSellerMenu();
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                Title = "Hunting seller",
                SubMenu = sellingMenu
            }, () => sellingMenu.isInRangeOfSeller(), 500);
            client.RegisterEventHandler("Hunting.RecievePrices", new Action<dynamic>(prices =>
            {
                sellingMenu.sellingPrices = (IDictionary<string, object>)prices;
                sellingMenu.createSellingOptions();
            }));
            client.RegisterEventHandler("openHutingSellerMenu", new Action(() =>
            {
                InteractionUI.Observer.OpenMenu(sellingMenu);
            }));
        }

        static internal async Task OnTick()
        {
            NearbyAnimals.Where(a => a.IsDead && !Function.Call<bool>(Hash.DECOR_EXIST_ON, a.Handle, "Animal.Looted")).ToList().ForEach(a =>
            {
                CitizenFX.Core.World.DrawMarker(MarkerType.ChevronUpx3, a.Position + new Vector3(0, 0, 1f), Vector3.Zero, new Vector3(180f, 0, 0), 1.0f * Vector3.One, Color.FromArgb(255, 255, 0, 0), true);
            });

            var lootableAnimals = NearbyAnimals.Where(a => a.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(lootableDistance, 2) && a.IsDead && !Function.Call<bool>(Hash.DECOR_EXIST_ON, a.Handle, "Animal.Looted"));
            if (lootableAnimals.Any() && !Game.PlayerPed.IsInVehicle())
            {
                CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame($"Press ~INPUT_CONTEXT~ to loot {((PedHash)lootableAnimals.First().Model.Hash).ToString().AddSpacesToCamelCase()}");
                nextToDeadAnimal = true;
                lootingMenu.createLootOptions(((PedHash)lootableAnimals.First().Model.Hash).ToString(), lootableAnimals.First());
            }
            else
                nextToDeadAnimal = false;

            if (Game.IsControlJustPressed(1, Control.Context))
            {
                if (lootableAnimals.Any() && !Game.PlayerPed.IsInVehicle())
                {
                    /*Log.ToChat($"Looted a {(PedHash)LootableAnimals.First().Model.Hash}.");
                    Debug.playerDebug.sendDebugMessage($"Looted a {(PedHash)LootableAnimals.First().Model.Hash}.");*/
                    string animalName = ((PedHash)lootableAnimals.First().Model.Hash).ToString();
                    //TriggerEvent("sendNotifyEvent", $"Looting the {animalName.AddSpacesToCamelCase()}.");
                    Ped lootedAnimal = lootableAnimals.First();
                    //Function.Call(Hash.DECOR_SET_INT, lootedAnimal, "Animal.Looted", 1);
                    lootingMenu.createLootOptions(animalName, lootedAnimal);
                    InteractionUI.Observer.OpenMenu(lootingMenu);
                }
            }

            if (InteractionUI.Observer.CurrentMenu == lootingMenu && !nextToDeadAnimal)
                InteractionUI.Observer.CloseMenu();
            else if(InteractionUI.Observer.CurrentMenu == sellingMenu && !(sellingMenu.sellerLocation.DistanceToSquared(Game.PlayerPed.Position) < 6))
                InteractionUI.Observer.CloseMenu();
        }

        private async Task RefreshAnimals()
        {
            await BaseScript.Delay(10000);
            NearbyAnimals = PedList.Select(p => new Ped(p)).Where(p => HuntableModels.Contains(p.Model)).ToList();  
        }
    }
}

