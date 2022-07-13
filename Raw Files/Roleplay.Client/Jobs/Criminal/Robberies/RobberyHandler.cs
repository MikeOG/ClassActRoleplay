using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enums;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Locations;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.Criminal.Robberies
{
    public class RobberyHandler : ClientAccessor
    {
        public SafeCracking safeInstance => Client.Get<SafeCracking>();
        private Random rand = new Random((int)DateTime.Now.Ticks);
        private string currentRobberyLocation = "";
        private bool? canRob = false;

        public RobberyHandler(Client client) : base(client)
        {
            client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraciton));
            Client.RegisterEventHandler("Robbery.StartRegisterRobbery", new Action<bool>(canRob => this.canRob = canRob));
            Client.RegisterEventHandler("Robbery.StartRobbery", new Action<bool>(canRob => this.canRob = canRob));
            loadVaults();
        }

        public async void AttemptRegisterRobbery()
        {
            var closeRegister = GTAHelpers.GetObjectInRange(ObjectHash.prop_till_01);
            var closeVault = GetStoreInRange(20.0f);
            if (closeVault.Key != null && closeRegister != 0)
            {
                var robbalbe = await awaitRobResult("Robbery.CheckCanRobRegister", closeVault.Key);   
                
                if (robbalbe)
                {
                    Log.ToChat("[Robbery]", "You begin lockpicking the cash register", ConstantColours.Blue);
                    await Game.PlayerPed.Task.PlayAnimation("missheistfbisetup1", "unlock_loop_janitor", 2.0f, 2.0f, -1, (AnimationFlags)49, 0);
                    await BaseScript.Delay(15000);
                    Log.ToChat("", "Hit the register to get the money from it");
                    Game.PlayerPed.Task.ClearAll();
                    while (!API.HasEntityBeenDamagedByAnyPed(closeRegister))
                        await BaseScript.Delay(0);
                    Client.TriggerServerEvent("Robbery.RequestRegisterPayment", closeVault.Key);
                }
            }
        }

        private void loadVaults()
        {
            foreach (var vault in StoreLocations.Positions)
            {
                var vaultObj = API.CreateObjectNoOffset((uint)ObjectHash.v_ilev_gangsafe, vault.Value.X, vault.Value.Y, vault.Value.Z, true, true, true);
                API.SetEntityAsMissionEntity(vaultObj, true, true);
                API.SetEntityHeading(vaultObj, vault.Value.W);
                var doorObj = API.CreateObjectNoOffset(unchecked((uint)ObjectHash.v_ilev_gangsafedoor), vault.Value.X, vault.Value.Y, vault.Value.Z, true, true, true);
                API.SetEntityAsMissionEntity(doorObj, true, true);
                API.SetEntityHeading(doorObj, vault.Value.W);
            }
        }

        private async void OnInteraciton()
        {
            var closeVault = GetStoreInRange();
            if (closeVault.Key != null)
            {
                if(!safeInstance.IsSafeDoorOpen() || currentRobberyLocation != closeVault.Key)
                {
                    var vaultObj = API.GetClosestObjectOfType(closeVault.Value.X, closeVault.Value.Y, closeVault.Value.Z, 3.0f, (uint)ObjectHash.v_ilev_gangsafe, false, false, false);
                    var doorObj = API.GetClosestObjectOfType(closeVault.Value.X, closeVault.Value.Y, closeVault.Value.Z, 3.0f, unchecked((uint)ObjectHash.v_ilev_gangsafedoor), false, false, false);
                    currentRobberyLocation = closeVault.Key;
                    var robbable = await awaitRobResult("Robbery.CheckCanRobStore", closeVault.Key);

                    if(robbable)
                    {
                        safeInstance.InitializeSafe(vaultObj, doorObj, new List<int>
                        {
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                            rand.Next(1, 90),
                        }, SafeCracking.RotationDirections.Clockwise);
                        startVaultOpen();
                    }
                }
                else if (safeInstance.IsSafeDoorOpen() && LocalSession.GetLocalData("Robbery.CanRequestPayout", false))
                {
                    await BaseScript.Delay(0);
                    Client.TriggerServerEvent("Robbery.RequestPayout", closeVault.Key);
                }
            }
            /*var closeRegister = GTAHelpers.GetObjectInRange(ObjectHash.prop_till_01);

            if (closeRegister != 0)
            {
                var closeStore = GetStoreInRange(20.0f);
                if (closeStore.Key == null) return;

                Client.TriggerServerEvent("Robbery.Store.AttemptRegisterRobbery", closeStore.Key, Entity.FromHandle(closeRegister).Position);
            }*/
        }

        private async void startVaultOpen()
        {
            var miniGameResult = await safeInstance.RunMiniGame();
            while (true)
            {
                miniGameResult = await safeInstance.RunMiniGame();
                if (miniGameResult == (int)SafeCracking.SafeCrackingResults.Success || miniGameResult == (int)SafeCracking.SafeCrackingResults.Failed)
                    break;

                await BaseScript.Delay(0);
            }

            if (miniGameResult == (int)SafeCracking.SafeCrackingResults.Success)
            {
                await safeInstance.OpenSafeDoor();
                Client.TriggerServerEvent("Robbery.OnRobberyCompleted", currentRobberyLocation);
            }
            else if (miniGameResult == (int)SafeCracking.SafeCrackingResults.Failed)
            {
                Client.TriggerServerEvent("Robbery.RobberyIncomplete", currentRobberyLocation);
            }
        }

        private async Task<bool> awaitRobResult(string eventName, string location)
        {
            canRob = null;
            Client.TriggerServerEvent(eventName, location);

            while (canRob == null)
                await BaseScript.Delay(0);

            return (bool)canRob;
        }

        private KeyValuePair<string, Vector4> GetStoreInRange(float distance = 2.0f)
        {
            return StoreLocations.Positions.FirstOrDefault(o => new Vector3(o.Value.X, o.Value.Y, o.Value.Z).DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(distance, 2));
        }
    }
}
