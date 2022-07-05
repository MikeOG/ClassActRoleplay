using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Loader;
using System.Threading.Tasks;

namespace Roleplay.Client.Jobs.Criminal.Robberies
{
    public class AIRobbing : ClientAccessor
    {
        private Ped victimPed;

        public AIRobbing(Client client) : base(client)
        {

        }

        [DynamicTick(TickUsage.Aiming)]
        private async Task OnAimingTick()
        {
            if (victimPed == null)
            {
                int entity = 0;
                if (GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref entity))
                {
                    if (Entity.FromHandle(entity) is Ped ped)
                    {
                        victimPed = ped;
                    }
                }

                return; // Incase victim ped is null return so the logic is done again
            }

            if (victimPed.IsPlayer || victimPed.IsDead || !victimPed.IsHuman || !victimPed.Exists())
            {
                victimPed = null;
                return;
            }
        }
    }
}
