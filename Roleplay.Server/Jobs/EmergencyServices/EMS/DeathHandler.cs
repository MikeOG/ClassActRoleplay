using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.Players;
using Roleplay.Server.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.EMS;
using Roleplay.Server.Session;
using Roleplay.Shared.Attributes;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        private DeathState deathState = DeathState.Alive;
        public DeathState DeathState
        {
            get => deathState;
            set
            {
                deathState = value;
                TriggerEvent(deathState == DeathState.Dead ? "Death.StartDeathThread" : "Death.EndDeathThread");
                if (deathState == DeathState.Dead)
                    Server.Instance.Get<DeathHandler>().StartDeathTimer(this);
                else
                    TriggerEvent("Skin.RefreshSkin");
            }
        }
    }
}

namespace Roleplay.Server.Jobs.EmergencyServices.EMS
{
    public class DeathHandler : ServerAccessor
    {
        public DeathHandler(Server server) : base(server)
        {
            server.RegisterEventHandler("baseevents:onPlayerDied", new Action<Player, int, dynamic>(OnPlayerDied));
            server.RegisterEventHandler("baseevents:onPlayerKilled", new Action<Player, int, dynamic>(OnPlayerKilled));
            server.RegisterEventHandler("Death.SendReviveTarget", new Action<Player, int>(OnReceiveRevive));
            CommandRegister.RegisterCommand("respawn", OnRespawnCommand);
            CommandRegister.RegisterAdminCommand("arevive", OnAdminRevive, AdminLevel.Moderator);
            CommandRegister.RegisterJobCommand("revive", OnReviveComamnd, JobType.EMS | JobType.Police);
        }

        public void StartDeathTimer(Session.Session playerSession)
        {
            var deathTimer = getRespawnTime();
            Task.Factory.StartNew(async () =>
            {
                playerSession.SetLocalData("Death.DeathTimer", deathTimer);
                while (deathTimer > 0)
                {
                    await BaseScript.Delay(1000);
                    if (playerSession.DeathState == DeathState.Alive || string.IsNullOrEmpty(playerSession.Source.Name)) return;

                    deathTimer--;
                    playerSession.SetLocalData("Death.DeathTimer", deathTimer);
                }

                playerSession.SetLocalData("Death.DeathTimer", 0);
                playerSession.SetLocalData("Death.RespawnCost", getRespawnCost());
            });
        }

        private void OnPlayerDied([FromSource] Player source, int killerType, dynamic deathData)
        {
            Server.Instances.Admin.SendAdminMessage("[Info]", $"{source.Name} died", ConstantColours.Info, AdminLevel.Moderator);
            var playerSession = Sessions.GetPlayer(source);//Server.Instances.Session.GetPlayer(source);
            if(playerSession.DeathState == DeathState.Alive)
            {
                //SetPlayerDeathState(playerSession, DeathState.Dead);
                playerSession.DeathState = DeathState.Dead;
                Server.TriggerLocalEvent("Log.ToDatabase", playerSession.PlayerName, playerSession.SteamIdentifier, "death", "died");
            }
        }

        private void OnPlayerKilled([FromSource] Player source, int killerId, dynamic deathData)
        {
            Server.Instances.Admin.SendAdminMessage("[Info]", $"{source.Name} was killed by {/*Server.PlayerList*/Players[killerId].Name}", ConstantColours.Info, AdminLevel.Moderator);
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession.DeathState == DeathState.Alive)
            {
                playerSession.DeathState = DeathState.Dead;
                Server.TriggerLocalEvent("Log.ToDatabase", playerSession.PlayerName, playerSession.SteamIdentifier, "death", $"was killed by {/*Server.PlayerList*/Players[killerId].Name}");
            }
        }

        private int getRespawnTime()
        {
            var emergencyServicesOnDuty = Server.Instances.Jobs.GetPlayersOnJob(JobType.EMS | JobType.Police);
            var baseTime = GetConvarInt("mg_deathRespawnTimer", 600);

            if (emergencyServicesOnDuty.Count == 0)
            {
                baseTime = GetConvarInt("mg_deathNoServiceRespawnTimer", 120);
            }

            return baseTime;
        }

        private int getRespawnCost()
        {
            var emergencyServicesOnDuty = Server.Instances.Jobs.GetPlayersOnJob(JobType.EMS | JobType.Police);
            var baseCost = GetConvarInt("mg_respawnCost", 3000);

            if (emergencyServicesOnDuty.Count == 0)
            {
                baseCost = GetConvarInt("mg_respawnNoServiceCost", 250);
            }

            return baseCost;
        }

        private void OnRespawnCommand(Command cmd)
        {
            var playerSession = cmd.Session;

            if (playerSession.GetLocalData("Death.DeathTimer", -1) == 0)
            {
                playerSession./*GetInventory()*/Inventory.RemoveAllWeapons();
                playerSession.SetLocalData("Death.DeathTimer", -1);
                playerSession.DeathState = DeathState.Alive;
                playerSession.TriggerEvent("Death.RespawnPlayer");
                //playerSession.TriggerEvent("Weapons.LoadWeapons");

                var payHandler = Server.Get<PaymentHandler>();
                var deathCost = playerSession.GetLocalData("Death.RespawnCost", 0);
                if (payHandler.CanPayForItem(playerSession, deathCost, paymentTypeOverride: (int)PaymentType.Debit))
                {
                    payHandler.PayForItem(playerSession, deathCost, "respawning", (int)PaymentType.Debit);
                }
                else
                {
                    var currentBill = playerSession.GetGlobalData("Character.Bill", 0);
                    playerSession.SetGlobalData("Character.Bill", currentBill + deathCost);
                }

                var jobHandler = Server.Get<JobHandler>();

                if (jobHandler.OnDutyAs(playerSession, JobType.EMS | JobType.Police))
                {
                    playerSession.TriggerEvent("Player.OnDutyStatusChange", true);
                    Sessions.TriggerSessionEvent("OnDutyChangeState", playerSession);
                }

                Log.ToClient("", $"You were transported to the hospital and had to pay ${deathCost} in medical bills", ConstantColours.White, cmd.Player);
            }   
        }

        private void OnAdminRevive(Command cmd)
        {
            var targetPlayer = Sessions.GetPlayer(cmd.GetArgAs(0, 0));
            if(targetPlayer == null) return;

            if(targetPlayer.DeathState == DeathState.Dead)
            {
                targetPlayer.DeathState = DeathState.Alive;
                Log.ToClient("[Admin]", $"You admin revived {targetPlayer.PlayerName}", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You were revived by an admin", ConstantColours.Admin, targetPlayer.Source);
            }
            else
            {
                Log.ToClient("[Admin]", $"This player isn't dead", ConstantColours.Admin, cmd.Player);
            }
        }

        private void OnReceiveRevive([FromSource] Player source, int targetId)
        {
            var playerSession = Sessions.GetPlayer(source);
            var targetSession = Sessions.GetPlayer(targetId);

            if(playerSession == null || targetSession == null) return;

            if (JobHandler.OnDutyAs(playerSession, JobType.EMS | JobType.Police) && targetSession.DeathState == DeathState.Dead)
            {
                targetSession.DeathState = DeathState.Alive;
                targetSession.Message("[Medic]", "You were brought back to your feet by a medic", ConstantColours.EMS);
                playerSession.Message("[Medic]", $"You brought {targetSession.GetCharacterName()} to their feet", ConstantColours.EMS);
            }
        }

        private void OnReviveComamnd(Command cmd)
        {
            var closePlayer = cmd.Session.GetClosestPlayer(customFindFunc: session => session.DeathState == DeathState.Dead);

            if (closePlayer != null)
            {
                Log.Debug($"{cmd.Player.Name} found a target of {closePlayer.PlayerName} to revive");
                OnReceiveRevive(cmd.Player, closePlayer.ServerID);
            }
        }

        [ServerCommand("heal", JobType.EMS)]
        private void OnHealCommand(Command cmd)
        {
            var closePlayer = cmd.Session.GetClosestPlayer();

            if (closePlayer != null)
            {
                Log.Debug($"{cmd.Player.Name} found a target of {closePlayer.PlayerName} to heal");

                cmd.Session.Message("[Medic]", $"You just healed {closePlayer.GetCharacterName()}", ConstantColours.EMS);
                closePlayer.Message("[Medic]", $"You were just healed by a medic", ConstantColours.EMS);

                closePlayer.TriggerEvent("Player.Heal");
            }
        }
    }
}
