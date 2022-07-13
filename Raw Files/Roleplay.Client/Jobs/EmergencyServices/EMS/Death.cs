using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Client.Player;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Jobs.EmergencyServices.EMS
{
    public class Death : ClientAccessor
    {
        private Vector3 hospitalLocation = new Vector3(333.02f, -572.23f, 43.32f);
        private WarpPoint pillboxElevatorPoint = new WarpPoint(new Vector3(340.59f, -595.38f, 28.38f), new Vector3(332.84f, -569.21f, 42.29f));
        private WarpPoint pillboxElevatorRoofPoint = new WarpPoint(new Vector3(327.34f, -603.17f, 42.29f), new Vector3(339.13f, -583.92f, 74.16f));
        private WarpPoint morgueElevatorPoint = new WarpPoint(new Vector3(241.15f, -1378.86f, 33.74f), new Vector3(275.85f, -1361.56f, 24.54f));
        static float HealRange = 3f;

        //static PlayerList PlayerList = new PlayerList();
        static PedList PedList = new PedList();

        private static ScreenText deathString = new ScreenText("You have 600 seconds until respawn (/911 [message] for help)", 960, 540, 0.5f, async () =>
        {
            var playerSession = Client.LocalSession;
            var remainingTime = playerSession.GetLocalData("Death.DeathTimer", 600);
            if (remainingTime > 0)
                deathString.Caption = $"You have {remainingTime} seconds until respawn (/911 [message] for help)";
            else
                deathString.Caption = $"You are now able to respawn with /respawn but it will cost you ${playerSession.GetLocalData("Death.RespawnCost", 10000)}";

            await BaseScript.Delay(0);
        });

        public Death(Client client) : base(client)
        {
            client.RegisterEventHandler("Death.StartDeathThread", new Action(OnStartDeath));
            client.RegisterEventHandler("Death.EndDeathThread", new Action(OnEndDeath));
            client.RegisterEventHandler("Death.RespawnPlayer", new Action(OnPlayerRespawn));
            client.RegisterEventHandler("Death.FindReviveTarget", new Action(OnFindRevive));
            Client.RegisterEventHandler("NPC.Revive", new Action<string>(ReceiveNPCRevive));
            client.RegisterTickHandler(EnablePVPTick);
        }

        [EventHandler("Player.Heal")]
        private void OnHealPlayer()
        {
            Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
        }

        private void OnStartDeath()
        {
            var playerPed = Game.PlayerPed;
            ResurrectPed(playerPed.Handle);
            NetworkResurrectLocalPlayer(playerPed.Position.X, playerPed.Position.Y, playerPed.Position.Z, playerPed.Heading, true, false);
            Client.RegisterTickHandler(DeathTick);
        }

        private async void OnEndDeath()
        {
            var playerPed = Game.PlayerPed;
            Client.DeregisterTickHandler(DeathTick);
            playerPed.Task.ClearAllImmediately();
            ResurrectPed(playerPed.Handle);
            NetworkResurrectLocalPlayer(playerPed.Position.X, playerPed.Position.Y, playerPed.Position.Z, playerPed.Heading, true, false);
            await BaseScript.Delay(0);
            playerPed.Ragdoll(1000);
            playerPed.IsInvincible = false;
            playerPed.MaxHealth = 100;
            playerPed.Health = 100;
        }

        static public void ReceiveNPCRevive(string serializedLocation)
        {
            try
            {
                var eventData = Helpers.MsgPack.Deserialize<float[]>(serializedLocation);
                var reviveListAI = PedList.Select(p => new Ped(p)).Where(p => p.Position.DistanceToSquared(eventData.ToVector3()) < Math.Pow(HealRange, 2) && Function.Call<bool>(Hash.DECOR_GET_BOOL, p.Handle, "Ped.BeingRevived"));

                if (reviveListAI.Count() > 0)
                {
                    var reviveTarget = reviveListAI.First();

                    Function.Call(Hash.RESURRECT_PED, reviveTarget.Handle);
                    Function.Call(Hash.REVIVE_INJURED_PED, reviveTarget.Handle);
                    Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, reviveTarget.Handle);
                    Function.Call(Hash.SET_ENTITY_COLLISION, reviveTarget.Handle, true, true);
                    Function.Call(Hash._PLAY_AMBIENT_SPEECH1, reviveTarget.Handle, "GENERIC_THANKS", "SPEECH_PARAMS_FORCE");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"EMS/Cop NPC Revive error: {ex.Message}");
            }
        }

        private void OnPlayerRespawn()
        {
            OnEndDeath();
            Game.PlayerPed.Position = hospitalLocation;
            Game.PlayerPed.Heading = 47.0f;
        }

        private async Task DeathTick()
        {
            var playerPed = Cache.PlayerPed;
            playerPed.IsInvincible = true;
            if(Client.Get<Arrest>().GetVehState(LocalSession) == VehState.OutVeh)
            {
                playerPed.CanRagdoll = true;
                playerPed.Ragdoll(30000);
            }
            Client.Get<Arrest>().DisableActions();
            Game.DisableControlThisFrame(1, Control.MoveUpOnly);
            Game.DisableControlThisFrame(1, Control.MoveDownOnly);
            Game.DisableControlThisFrame(1, Control.MoveLeftOnly);
            Game.DisableControlThisFrame(1, Control.MoveRightOnly);
            /*if(playerPed.IsInVehicle() && playerPed.CurrentVehicle.Driver == playerPed)
            {
                playerPed.Task.ClearAllImmediately();
                playerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            }*/

            if(CinematicMode.InCinematicMode) return;
            deathString.DrawTick();
        }

        private void OnFindRevive()
        {
            var closestPlayer = GTAHelpers.GetClosestPlayer(4.0f);
            
            if(closestPlayer != null)
                Client.TriggerServerEvent("Death.SendReviveTarget", closestPlayer.ServerId);
        }

        private async Task EnablePVPTick()
        {
            await BaseScript.Delay(5000);
            Client.PlayerList.ToList().ForEach(o =>
            {
                SetCanAttackFriendly(o.Character.Handle, true, true);
                NetworkSetFriendlyFireOption(true);
            });
        }
    }
}
