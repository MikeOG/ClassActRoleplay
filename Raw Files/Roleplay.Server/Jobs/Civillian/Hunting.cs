using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Players;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Jobs.Civillian
{
    public class Hunting : ServerAccessor
    {
        private Dictionary<string, int> sellingPrices = new Dictionary<string, int>()
        {
            ["Coyote_skin"] = 75,
            ["Coyote_meat"] = 60,
            ["Coyote_pelts"] = 60,
            //["Deer_skin"] = 12,
            ["Deer_meat"] = 60,
            ["Deer_pelts"] = 75,
            ["Deer_antlers"] = 90,
            //["MountainLion_skin"] = 15,
            ["MountainLion_meat"] = 60,
            ["MountainLion_pelts"] = 60,
            ["MountainLion_teeth"] = 60,
            ["MountainLion_claws"] = 75,
            //["Rabbit_skin"] = 6,
            ["Rabbit_meat"] = 45,
            ["Rabbit_pelts"] = 30,
        };

        public Hunting(Server server) : base(server)
        {
            server.RegisterEventHandler("Hunting.AttemptLootItem", new Action<Player, string, int>(OnAttemptAdd));
            server.RegisterEventHandler("Hunting.RequestPrices", new Action<Player>(OnRequestPrices));
            server.RegisterEventHandler("Hunting.AttemptSellItems", new Action<Player, string, int>(OnRequestSell));
        }

        private void OnAttemptAdd([FromSource] Player source, string item, int itemAmount)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            var playerInv = new PlayerInventory(playerSession.GetGlobalData("Character.Inventory", ""), playerSession);

            if (playerInv.CanStoreItem(item, itemAmount))
            {
                playerInv.AddItem(item, itemAmount);

                var animalData = item.Split('_');

                Log.ToClient("[Hunting]", $"You looted {itemAmount} {animalData[0].AddSpacesToCamelCase()} {animalData[1]}", ConstantColours.Hunting, source);
            }
            else
            {
                Log.ToClient("[Hunting]", "You are not able to carry anymore items", ConstantColours.Hunting, source);
            }
        }

        private void OnRequestPrices([FromSource] Player source)
        {
            source.TriggerEvent("Hunting.RecievePrices", sellingPrices);
        }

        private void OnRequestSell([FromSource] Player source, string item, int itemAmount)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            var playerInv = new PlayerInventory(playerSession.GetGlobalData("Character.Inventory", ""), playerSession);

            if (playerInv.HasItemWithAmount(item, itemAmount))
            {
                playerInv.AddItem(item, -itemAmount);

                var animalData = item.Split('_');
                var sellingPrice = sellingPrices[item] * itemAmount;

                Server.Get<PaymentHandler>().UpdatePlayerCash(playerSession, sellingPrice, $"selling {itemAmount} {animalData[0].AddSpacesToCamelCase()} {animalData[1]}");
                Log.ToClient("[Hunting]", $"You just sold {itemAmount} {animalData[0].AddSpacesToCamelCase()} {animalData[1]} for ${sellingPrice}", ConstantColours.Hunting, source);
            }
            else
            {
                Log.ToClient("[Hunting]", $"You don't have enough of this item to do that", ConstantColours.Hunting, source);
            }
        }
    }
}
