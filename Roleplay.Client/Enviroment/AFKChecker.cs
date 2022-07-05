using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Client.Enviroment
{
    public class AFKChecker : ClientAccessor
    {
        public static int AFKTimer = 30;
        private Vector3 previousLocation = Vector3.Zero;

        public AFKChecker(Client client) : base(client)
        {
            client.RegisterTickHandler(AFKCheckTick);
        }

        private async Task AFKCheckTick()
        {
            await BaseScript.Delay(60000);

            var currentLocation = Cache.PlayerPed.Position;

            if (currentLocation == previousLocation)
            {
                AFKTimer--;

                if (AFKTimer <= 5)
                {
                    Log.ToChat($"You will be kicked for AFK in {AFKTimer} minutes");
                }

                if (AFKTimer <= 0)
                {
                    Client.TriggerServerEvent("AFK.RequestKick");
                }
            }
            else
            {
                AFKTimer = 30;
            }

            previousLocation = currentLocation;
        }
    }
}
