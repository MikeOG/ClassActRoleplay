using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
    public class VehicleWeaponSelect : JobClass
    {
        private string currentSelectedWeapon = "None";
        private int currentSelectedIndex = 0;
        private List<Tuple<string, bool>> selectableWeapons = new List<Tuple<string, bool>>
        {
            new Tuple<string, bool>("None", false),
            new Tuple<string, bool>("StunGun", false),
            new Tuple<string, bool>("CombatPistol", false),
            new Tuple<string, bool>("PumpShotgun", true),
            new Tuple<string, bool>("CarbineRifle", true),
            new Tuple<string, bool>("CarbineRifleMk2", true),
        };

        public VehicleWeaponSelect()
        {
            Client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) =>
            {
                if (JobHandler.OnDutyAsJob(JobType.Police))
                    Client.RegisterTickHandler(WeaponSelectTick);
            }));
            Client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) =>
            {
                if (JobHandler.OnDutyAsJob(JobType.Police))
                    Client.DeregisterTickHandler(WeaponSelectTick);
            }));
        }

        private async Task WeaponSelectTick()
        {
            if (Input.IsControlJustPressed(Control.VehicleDuck) && Cache.PlayerPed.CurrentVehicle.ClassType == VehicleClass.Emergency)
            {
                var previousItem = selectableWeapons[currentSelectedIndex];
                currentSelectedIndex += 1;

                if (currentSelectedIndex > selectableWeapons.Count - 1)
                    currentSelectedIndex = 0;

                var newItem = selectableWeapons[currentSelectedIndex];
                var validWeapon = Enum.TryParse<WeaponHash>(newItem.Item1, out var weaponHash);
                var pedWeapons = Game.PlayerPed.Weapons;

                if (validWeapon)
                {
                    if (!pedWeapons.HasWeapon(weaponHash))
                        pedWeapons.Give(weaponHash, 150, true, true);

                    pedWeapons.Select(weaponHash, true);
                    EmoteManager.currentWeapon = weaponHash;
                }

                if (previousItem.Item2)
                {
                    Enum.TryParse<WeaponHash>(previousItem.Item1, out var previousHash);
                    pedWeapons.Remove(previousHash);
                }

                Log.ToChat("", $"^5* Equipped: {(validWeapon ? weaponHash.ToString().AddSpacesToCamelCase() : "None")}");
            }
        }
    }
}
