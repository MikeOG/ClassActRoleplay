using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Bank
{
    public class TempCustoms : ServerAccessor
    {
        public TempCustoms(Server server) : base(server)
        {
            server.RegisterLocalEvent("Customs.AttemptPaymentRepair", new Action<int, int, dynamic, dynamic>(OnAttemptPay));
        }

        private void OnAttemptPay(int source, int price, dynamic name, dynamic button)
        {
            var payHandler = Server.Get<PaymentHandler>();
            var playerSession = Sessions.GetPlayer(source);
            Log.Verbose($"{playerSession.PlayerName} is attempting to repair/mod a vehicle for price ${price}");

            if (payHandler.CanPayForItem(playerSession, price))
            {
                payHandler.PayForItem(playerSession, price, "vehicle repair");
                playerSession.TriggerEvent("LSC:buttonSelected", name, button, true);
                Log.ToClient("[Bank]", $"You paid ${price} for vehicle mods/repairs", ConstantColours.Bank, playerSession.Source);
            }
            else
            {
                Log.ToClient("[Bank]", $"You cannot afford these mods/repairs", ConstantColours.Bank, playerSession.Source);
            }
        }   
    }
}
