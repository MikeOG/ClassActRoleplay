using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Client
{
    public class Cache : ClientAccessor
    {
        public static Ped PlayerPed = Game.PlayerPed;

        public Cache(Client client) : base(client)
        {
            client.RegisterTickHandler(UpdateTick);
        }

        private async Task UpdateTick()
        {
            PlayerPed = Game.PlayerPed;
        }
    }
}
