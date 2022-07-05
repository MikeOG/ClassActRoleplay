using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client;
using Roleplay.Client.Jobs;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Weapons
{
   internal class WeaponHandler : ClientAccessor
    {
        private List<WeaponHash> pistolHashes = new List<WeaponHash>()
        {
            WeaponHash.VintagePistol,
            WeaponHash.Pistol,
            WeaponHash.APPistol,
            WeaponHash.CombatPistol,
            WeaponHash.Pistol50,
            WeaponHash.SNSPistol,
            WeaponHash.PistolMk2,
            WeaponHash.HeavyPistol,
            WeaponHash.MachinePistol,
            WeaponHash.MarksmanPistol,
        };

        private List<WeaponHash> SMGHashes = new List<WeaponHash>()
        {
            WeaponHash.MicroSMG,
            WeaponHash.SMG,
            WeaponHash.SMGMk2,
            WeaponHash.MiniSMG,
            WeaponHash.AssaultSMG,
        };

        private List<WeaponHash> rifleHashes = new List<WeaponHash>()
        {
            WeaponHash.SniperRifle,
            WeaponHash.AssaultRifleMk2,
            WeaponHash.CompactRifle,
            WeaponHash.BullpupRifle,
            WeaponHash.CarbineRifle,
            WeaponHash.AdvancedRifle,
            WeaponHash.AssaultRifle,
            WeaponHash.MarksmanRifle,
            WeaponHash.CarbineRifleMk2,
        };

        private List<WeaponHash> shotgunHashes = new List<WeaponHash>()
        {
            WeaponHash.SweeperShotgun,
            WeaponHash.PumpShotgun,
            WeaponHash.HeavyShotgun,
            WeaponHash.SawnOffShotgun,
            WeaponHash.BullpupShotgun,
            WeaponHash.AssaultShotgun,
            WeaponHash.DoubleBarrelShotgun,
        };

        public WeaponHandler(Client client) : base (client)
        {
            client.RegisterTickHandler(OnTick);
            client.RegisterEventHandler("Weapons.SetAmmoForWeaponType", new Action<string, int>(setAmmoForWeaponType));
            client.RegisterEventHandler("Weapons.LoadWeapons", new Action(LoadWeapons));
            client.RegisterEventHandler("Weapons.GiveWeapon", new Action<string, int>(GiveWeapon));
            client.RegisterEventHandler("Weapons.RemoveAllWeapons", new Action(() => Game.PlayerPed.Weapons.RemoveAll()));
            client.RegisterEventHandler("Player.OnSkinLoaded", new Action(LoadWeapons));
        }

        public bool IsAttackWeapon(WeaponHash weapon)
        {
            var allHashes = pistolHashes.Concat(SMGHashes).Concat(rifleHashes).Concat(rifleHashes).Concat(shotgunHashes);
            return allHashes.Any(i => i == weapon);
        }


        private async void LoadWeapons()
        {
            await LocalSession.UpdateData("Character.Inventory");

            if (Client.Get<JobHandler>().OnDutyAsJob(JobType.EMS | JobType.Police)) return;

            var playerInv = new PlayerInventory(LocalSession.GetGlobalData("Character.Inventory", ""), LocalSession);
            var playerWeapons = playerInv.InventoryItems.FindAll(o => o.itemCode.Contains("WEAPON_"));
            var playerPed = Game.PlayerPed;
            playerPed.Weapons.RemoveAll();
            playerWeapons.ForEach(o =>
            {
                var weaponHash = (WeaponHash) Game.GenerateHash(o.itemCode);
                if (!playerPed.Weapons.HasWeapon(weaponHash))
                {
                    if(weaponHash == WeaponHash.PetrolCan)
                        playerPed.Weapons.Give(weaponHash, 2250, false, false);
                    else
                        playerPed.Weapons.Give(weaponHash, 0, false, false);
                }
            });
            setAmmoForWeaponType("pistol", playerInv.GetItem("pistolammo")?.itemAmount ?? 0);
            setAmmoForWeaponType("smg", playerInv.GetItem("smgammo")?.itemAmount ?? 0);
            setAmmoForWeaponType("rifle", playerInv.GetItem("rifleammo")?.itemAmount ?? 0);
            setAmmoForWeaponType("shotgun", playerInv.GetItem("shotgunammo")?.itemAmount ?? 0);
        }

        private void setAmmoForWeaponType(string weaponType, int ammoCount)
        {
            WeaponCollection playerWeapons = Game.PlayerPed.Weapons;
            List<WeaponHash> weaponTypeHashes = getHashesForWeaponType(weaponType);
            weaponTypeHashes.ForEach(o =>
            {
                if (playerWeapons.HasWeapon(o))
                    playerWeapons[o].Ammo = ammoCount;
            });
        }

        private void removeAmmoFromWeapon(string weaponType, int newAmmoCount)
        {
            //await LocalSession.UpdateData("Character.Inventory");

            var playerInv = new PlayerInventory(LocalSession.GetGlobalData("Character.Inventory", ""), LocalSession);
            var ammoObject = playerInv.GetItem($"{weaponType}ammo") ?? InventoryItems.GetInvItemData($"{weaponType}ammo");

            if (ammoObject == null) return;

            var currentWeaponAmmo = ammoObject.itemAmount;
            var ammoToLose = currentWeaponAmmo - newAmmoCount;
            if (ammoToLose <= 0) return;

            Roleplay.Client.Client.Instance.TriggerServerEvent("Inventory.AddInvItem", JsonConvert.SerializeObject(ammoObject), -ammoToLose);
        }

        private List<WeaponHash> getHashesForWeaponType(string weaponType)
        {
            List<WeaponHash> weaponTypeHashes = new List<WeaponHash>();
            switch (weaponType)
            {
                case "pistol":
                    weaponTypeHashes = pistolHashes;
                    break;
                case "smg":
                    weaponTypeHashes = SMGHashes;
                    break;
                case "rifle":
                    weaponTypeHashes = rifleHashes;
                    break;
                case "shotgun":
                    weaponTypeHashes = shotgunHashes;
                    break;
                default:
                    weaponTypeHashes = null;
                    break;
            }

            return weaponTypeHashes;
        }

        private string getWeaponType(WeaponHash weapon)
        {
            if (pistolHashes.Contains(weapon))
                return "pistol";
            else if (SMGHashes.Contains(weapon))
                return "smg";
            else if (rifleHashes.Contains(weapon))
                return "rifle";
            else if (shotgunHashes.Contains(weapon))
                return "shotgun";
            else
                return "";
        }

        // TODO remove and reimplement into 1s when it can be
        private void GiveWeapon(string weapon, int ammo)
        {
            Enum.TryParse<WeaponHash>(weapon, out var weaponHash);
            Game.PlayerPed.Weapons.Give(weaponHash, ammo, false, true);

            Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
        }

        private WeaponHash prevWeapon = WeaponHash.Unarmed;
        private async Task OnTick()
        {
            if(Client.Get<JobHandler>() != null && LocalSession != null && Client.Get<JobHandler>().OnDutyAsJob(JobType.Police | JobType.EMS, LocalSession)) return;
            
            WeaponHash currentWeapon = Game.PlayerPed.Weapons.Current.Hash;
            if (prevWeapon != currentWeapon)
            {
                if (prevWeapon != WeaponHash.Unarmed && Game.PlayerPed.Weapons.HasWeapon(prevWeapon))
                {
                    var ammoAmount = Game.PlayerPed.Weapons[prevWeapon].Ammo;
                    removeAmmoFromWeapon(getWeaponType(prevWeapon), ammoAmount);
                }

                prevWeapon = currentWeapon;

                await BaseScript.Delay(250);
            }
        }
    }
}
