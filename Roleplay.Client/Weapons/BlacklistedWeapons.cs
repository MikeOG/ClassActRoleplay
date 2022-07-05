using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Client.Weapons
{
    public class BlacklistedWeapons : ClientAccessor
    {
        public List<WeaponHash> BlacklistedWeaponsList = new List<WeaponHash>
        {
            WeaponHash.RPG,
            WeaponHash.Railgun,
            WeaponHash.SniperRifle,
            WeaponHash.ProximityMine,
            WeaponHash.SpecialCarbine,
            WeaponHash.CombatMG,
            WeaponHash.HeavyShotgun,
            WeaponHash.CombatMGMk2,
            WeaponHash.HeavySniperMk2,
            WeaponHash.AssaultShotgun,
            WeaponHash.CompactGrenadeLauncher,
            WeaponHash.GrenadeLauncherSmoke,
            WeaponHash.GrenadeLauncher,
            WeaponHash.PipeBomb,
            WeaponHash.HomingLauncher,
        };

        public BlacklistedWeapons(Client client) : base(client)
        {
            client.RegisterTickHandler(RemoveBlacklistedWeaponsTick);
        }

        private async Task RemoveBlacklistedWeaponsTick()
        {
            await BaseScript.Delay(1000);

            var playerWeapons = Cache.PlayerPed.Weapons;
            BlacklistedWeaponsList.ForEach(o =>
            {
                if (playerWeapons.HasWeapon(o))
                {
                    playerWeapons.Remove(o);
                }
            });
        }
    }
}
