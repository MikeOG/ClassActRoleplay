using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;

namespace Roleplay.Client.Player
{
    public class GunShotResidue : ClientAccessor
    {
        public GunShotResidue(Client client) : base(client)
        {

        }

        [DynamicTick(TickUsage.Shooting)]
        private async Task AddResidueTick()
        {
            
        }
    }
}
