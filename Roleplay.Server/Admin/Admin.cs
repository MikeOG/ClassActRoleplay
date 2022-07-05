using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Admin
{
    class Admin : ServerAccessor
    {
        public Admin(Server server) : base(server)
        {
            CommandRegister.RegisterAdminCommand("setadminlevel", SetPermissionLevel, AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("kick", KickPlayer, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("bring", OnBringCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("return", OnReturnCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("mute", OnMuteCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("unmute", OnUnMuteCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("freeze", OnFreezeCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("unfreeze", OnUnFreezeCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("announce", OnAnnounceCommand, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("say", OnSayCommand, AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("playerlist", OnPlayerlistCommand, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("sc", OnStaffChatCommand, AdminLevel.Moderator);

            CommandRegister.RegisterRCONCommand("kickall", cmd =>
            {
                foreach (var player in /*Server.PlayerList.ToList()*/Players.ToList())
                {
                    player.Drop(string.Join(" ", cmd.Args));
                }
            });
        }

        public void OnPlayerDisconnect(Session.Session playerSession, string reason)
        {
            //ACEWrappers.RemovePrivellagedUser($"steam:{playerSession.SteamIdentifier}", playerSession.GetLocalData("User.PermissionLevel", AdminLevel.User));
            SendAdminMessage("", $"^2{playerSession.PlayerName} left ({reason})", ConstantColours.White);
        }

        public void SendAdminMessage(string prefix, string message, Color prefixColour, AdminLevel requiredLevel = AdminLevel.Moderator)
        {
            var session = Sessions;
            foreach (var player in session.PlayerList)
            {
                var adminLevel = player.GetLocalData("User.PermissionLevel", AdminLevel.User);
                if (adminLevel >= requiredLevel)
                {
                    Log.ToClient(prefix, message, prefixColour, player.Source);
                }
            }
        }

        private void SetPermissionLevel(Command cmd)
        {
            var player = cmd.GetArgAs<int>(0);
            var targetLevel = cmd.GetArgAs(1, AdminLevel.User);
            if (player == cmd.Source) return;

            var playerSession = Sessions.GetPlayer(player);

            if (playerSession == null) return;

            var currentLevel = playerSession.GetLocalData("User.PermissionLevel", AdminLevel.User);
            playerSession.SetLocalData("User.PermissionLevel", targetLevel);
            ACEWrappers.RemovePrivellagedUser($"steam:{playerSession.SteamIdentifier}", currentLevel);
            ACEWrappers.AddPrivellagedUser($"steam:{playerSession.SteamIdentifier}", targetLevel);
            Log.Info($"Set the permission level of {playerSession.Source.Name} from {currentLevel.ToString().AddSpacesToCamelCase()} to {targetLevel.ToString().AddSpacesToCamelCase()}");
        }

        private void KickPlayer(Command cmd)
        {
            var player = cmd.GetArgAs(0, 0);

            if (player == cmd.Source || player == 0) return;

            cmd.Args.RemoveAt(0);

            var playerObj = Sessions.GetPlayer(player);
            if (playerObj != null)
            {
                var playerName = playerObj.PlayerName;
                var playerIdentifier = playerObj.Source.Identifiers[Server.CurrentIdentifier];
                var kickReason = string.Join(" ", cmd.Args);

                playerObj.Source.Drop($"Kicked: {kickReason}");

                Server.TriggerLocalEvent("Log.ToDatabase", playerName, playerIdentifier, "kick", kickReason);
            }
        }

        private void OnBringCommand(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);

#if ONESYNC
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession != null)
            {
                targetSession.SetServerData("Admin.PreviousLocation", targetSession.GetPlayerPosition());
                targetSession.SetPlayerPosition(cmd.Session.GetPlayerPosition());
                Log.ToClient("[Admin]", $"Brought {targetSession.PlayerName} to your location", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You were brought to {cmd.Player.Name}", ConstantColours.Admin, targetSession.Source);
            }
#else
            new PlayerList()[targetId].TriggerEvent("Admin.GoToTarget", cmd.Source);
#endif
        }

        private void OnReturnCommand(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);

#if ONESYNC
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession != null && targetSession.GetServerData("Admin.PreviousLocation", Vector3.Zero) != Vector3.Zero)
            {
                targetSession.SetPlayerPosition(targetSession.GetServerData("Admin.PreviousLocation", Vector3.Zero));
                targetSession.SetServerData("Admin.PreviousLocation", Vector3.Zero);
                Log.ToClient("[Admin]", $"Returned {targetSession.PlayerName} to their previous location", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You were returned to your previous location", ConstantColours.Admin, targetSession.Source);
            }
            else
            {
                Log.ToClient("[Admin]", $"No location to return {targetSession?.PlayerName} to", ConstantColours.Admin, cmd.Player);
            }
#endif
        }

        private void OnMuteCommand(Command cmd)
        {
            var player = cmd.GetArgAs(0, 0);
            var muteTime = cmd.GetArgAs(1, 0) * 60;

            var targetSession = Sessions.GetPlayer(player);
            if (targetSession != null)
            {
                Log.ToClient("[Admin]", $"You muted {targetSession.PlayerName} from typing in help chat for {muteTime / 60} minutes", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You are muted from typing in help chat for {muteTime / 60} minutes", ConstantColours.Admin, targetSession.Source);
                targetSession.SetLocalData("User.IsMuted", true);
                Task.Factory.StartNew(async () =>
                {
                    await BaseScript.Delay(muteTime * 1000);
                    targetSession.SetLocalData("User.IsMuted", false);
                });
            }
        }

        private void OnUnMuteCommand(Command cmd)
        {
            var player = cmd.GetArgAs(0, 0);

            var targetSession = Sessions.GetPlayer(player);
            if (targetSession != null)
            {
                Log.ToClient("[Admin]", $"You unmuted {targetSession.PlayerName} from typing in help chat", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You were unmuted from typing in help chat", ConstantColours.Admin, targetSession.Source);
                targetSession.SetLocalData("User.IsMuted", false);
            }
        }

        private void OnFreezeCommand(Command cmd)
        {
            var player = cmd.GetArgAs(0, 0);

            var targetSession = Sessions.GetPlayer(player);
            if (targetSession != null)
            {
                Log.ToClient("[Admin]", $"You froze {targetSession.PlayerName}", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"You were shot with a freeze ray", ConstantColours.Admin, targetSession.Source);
                targetSession.SetFreezeStatus(true);
            }
        }

        private void OnUnFreezeCommand(Command cmd)
        {
            var player = cmd.GetArgAs(0, 0);

            var targetSession = Sessions.GetPlayer(player);
            if (targetSession != null)
            {
                Log.ToClient("[Admin]", $"You unfroze {targetSession.PlayerName}", ConstantColours.Admin, cmd.Player);
                Log.ToClient("[Admin]", $"Looks like you thawed out the ice", ConstantColours.Admin, targetSession.Source);
                targetSession.SetFreezeStatus(false);
            }
        }

        private void OnSayCommand(Command cmd)
        {
            Log.ToClient("[Server]", string.Join(" ", cmd.Args), ConstantColours.Info);
        }

        private void OnAnnounceCommand(Command cmd)
        {
            Log.ToClient("[Admin announcement]", string.Join(" ", cmd.Args), ConstantColours.Info);
        }

        private void OnPlayerlistCommand(Command cmd)
        {
            var currentPlayers = Sessions.PlayerList.OrderBy(o => o.ServerID);
            var playerList = "";

            foreach (var player in currentPlayers)
            {
                if(player.ServerID < 60000)
                {
                    playerList += $"{player.PlayerName} ({player.ServerID}) - {player.GetCharacterName()}\n";
                }
            }

            Log.ToClient("", playerList, ConstantColours.White, cmd.Player);
        }

        private void OnStaffChatCommand(Command cmd)
        {
            var message = string.Join(" ", cmd.Args);

            SendAdminMessage($"[Staff][{cmd.Session.PlayerName}]", message, ConstantColours.Admin);
        }

        [EventHandler("AFK.RequestKick")]
        private void OnAFKRequest([FromSource] Player source)
        {
            Log.Info($"Kicking {source.Name} for AFK");

            source.Drop("Kicked: AFK");
        }
    }
}
