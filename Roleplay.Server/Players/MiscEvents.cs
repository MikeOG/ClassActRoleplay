using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enviroment;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Roleplay.Shared.Enums;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Players
{
    class MiscEvents : ServerAccessor
    {
        public MiscEvents(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("me", OnMeCommand);
            CommandRegister.RegisterCommand("status", OnStatusCommand);
            CommandRegister.RegisterCommand("do", OnDoCommand);
            CommandRegister.RegisterCommand("ghelp|help", OnHelpCommand);
            CommandRegister.RegisterCommand("mutehelp|mhelp", OnMuteHelp);
            CommandRegister.RegisterCommand("pooc", OnPoocCommand);
            CommandRegister.RegisterCommand("showid", OnShowId);
            CommandRegister.RegisterCommand("setchattheme", OnSetChatTheme);
            CommandRegister.RegisterCommand("pm", OnPrivateMessage);
            CommandRegister.RegisterCommand("ad|advert", OnAdvertCommand);
            CommandRegister.RegisterCommand("anonad", OnAnonAdvertCommand);
            CommandRegister.RegisterCommand("roll", OnRollCommand);
            CommandRegister.RegisterCommand("resetspawn", OnResetSpawn);

            server.RegisterEventHandler("baseevents:enteredVehicle", new Action<Player>(OnVehicleEntered));
            server.RegisterEventHandler("baseevents:leftVehicle", new Action<Player>(OnVehicleExit));
            server.RegisterEventHandler("Emotes.BindEmote", new Action<Player, string, string>(OnBindEmote));
            server.RegisterEventHandler("Chat.AutomaticMe", new Action<Player, string>(OnMeAutomaticCommand));
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            playerSession.RefreshChatTheme();
        }

        private void OnMeCommand(Command cmd)
        {
            var characterName = $"{cmd.Session.GetGlobalData("Character.FirstName", "")}";
            
            BaseScript.TriggerClientEvent("Alerts.SendMeMessage", cmd.Source, $"{characterName} {string.Join(" ", cmd.Args)}");
        }

        private void OnMeAutomaticCommand([FromSource] Player source, string message)
        {
            Log.ToClient($"OnMeAutomaticCommand pre if");
            if (message.Equals(string.Empty)) return;
            Log.ToClient($"OnMeAutomaticCommand post if");
            Log.ToClient(source.Handle);
            BaseScript.TriggerClientEvent("Alerts.SendMeMessage", source, $"{message}");
            Log.ToClient($"OnMeAutomaticCommand post alert");

        }

        private void OnStatusCommand(Command cmd)
        {
            bool doesContainArgs = cmd.Args.Count != 0;
            var guid = cmd.Session.GetLocalData("User.Status.Guid", Guid.Empty);
            if (guid != Guid.Empty)
            {
                cmd.Session.SetLocalData("User.StatusText", "");
                cmd.Session.SetLocalData("User.Status.Guid", Guid.Empty);
            }
            else if (doesContainArgs)
            {
                Guid g = Guid.NewGuid();
                var characterName = $"{cmd.Session.GetGlobalData("Character.FirstName", "")}";

                cmd.Session.SetLocalData("User.StatusText", $"{characterName} {string.Join(" ", cmd.Args)}");
                cmd.Session.SetLocalData("User.Status.Guid", g);

                BaseScript.TriggerClientEvent(cmd.Player, "Alerts.SendStatusMessage", cmd.Source);
            }

        }

        private void OnDoCommand(Command cmd)
        {
            var characterName = $"{cmd.Session.GetGlobalData("Character.FirstName", "")}";

            BaseScript.TriggerClientEvent("Alerts.SendDoMessage", cmd.Source, $"{string.Join(" ", cmd.Args)} ({characterName})");
        }

        private void OnHelpCommand(Command cmd)
        {
            var isMuted = cmd.Session.GetLocalData("User.IsMuted", false);

            if (!isMuted)
            {
                Sessions.ForAllClients(o =>
                {
                    var playerSettings = o./*GetPlayerSettings()*/PlayerSettings;
                    var helpMuted = playerSettings.ContainsKey("MuteHelp") ? playerSettings["MuteHelp"] : false;
                    if (!helpMuted || o.Source == cmd.Player)
                    {
                        var helpMessage = "";
                        if(cmd.Args.Count > 0)
                            helpMessage = string.Join(" ", cmd.Args);
                        
                        if (helpMuted)
                        {
                            Log.ToClient($"[System]", "You currently have help chat muted so you are unable to send messages", ConstantColours.Red, o.Source);
                            return;
                        }

                        Log.ToClient($"[{cmd.Session.PlayerName}][Help]", helpMessage, ConstantColours.Help, o.Source);
                    }
                });
            }
        }

        private void OnMuteHelp(Command cmd)
        {
            var playerSettings = cmd.Session./*GetPlayerSettings()*/PlayerSettings;
            var newMuteStatus = !(playerSettings.ContainsKey("MuteHelp") && playerSettings["MuteHelp"]);

            playerSettings["MuteHelp"] = newMuteStatus;
            cmd.Session./*SetPlayerSettings()*/PlayerSettings = playerSettings;
            Log.ToClient("[Info]", $"{(newMuteStatus ? "Muted" : "Unmuted")} help chat", ConstantColours.Info, cmd.Player);
        }

        private void OnPoocCommand(Command cmd)
        {
            var playerCoords = cmd.Session.Position;

            Sessions.ForAllClients(o =>
            {
                try
                {
                    var clientCoords = o.Position;

                    if (playerCoords.DistanceToSquared(clientCoords) < 18.0f)
                    {
                        Log.ToClient($"[{cmd.Session.PlayerName}][Pooc]", string.Join(" ", cmd.Args), ConstantColours.Pooc, o.Source);
                    }
                }
                catch (Exception e)
                {

                }
            });
        }

        private void OnShowId(Command cmd)
        {
            var playerCoords = cmd.Session.Position;

            Sessions.ForAllClients(o =>
            {
                var clientCoords = o.Position;

                if (playerCoords.DistanceToSquared(clientCoords) < 12.0f)
                {
                    Log.ToClient($"Identification: ", $"Name - {cmd.Session.GetCharacterName()}; DOB - {cmd.Session.GetGlobalData("Character.DOB", "")}; Phone - {Server.Get<Phones>().IntToPhoneNumber(cmd.Session./*GetCharId()*/CharId)} (#{cmd.Session.ServerID})", ConstantColours.Do, o.Source);
                }
            });
        }

        private void OnSetChatTheme(Command cmd)
        {
            var theme = cmd.GetArgAs(0, "");

            cmd.Session.UpdateChatTheme(theme);
        }

        private void OnVehicleEntered([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if(playerSession == null) return;

            playerSession.SetServerData("Character.IsInVehicle", true);
        }
        
        private void OnVehicleExit([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if(playerSession == null) return;

            playerSession.SetServerData("Character.IsInVehicle", false);
        }

        private void OnBindEmote([FromSource] Player source, string key, string emote)
        {
            var GetCharacterSettings = Sessions.GetPlayer(source);
            if (GetCharacterSettings == null) return;

            var playerSettings = GetCharacterSettings.PlayerSettings;
            var emoteBinds = GetCharacterSettings.GetEmoteBinds();

            emoteBinds[key] = emote;

            playerSettings["EmoteBinds"] = emoteBinds;
            Log.Verbose($"Setting the key {key} to emote {emote} for {source.Name}");
            GetCharacterSettings.PlayerSettings = playerSettings;
        }

        private void OnPrivateMessage(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession == null)
            {
                Log.ToClient($"", "Invalid server ID", ConstantColours.Pooc, cmd.Player);
                return;
            }

            cmd.Args.RemoveAt(0);
            var message = string.Join(" ", cmd.Args);

            Log.ToClient($"PRIVATE OOC MESSAGE FROM {cmd.Session.PlayerName}", message, ConstantColours.Yellow, targetSession.Source);
            Log.ToClient($"PRIVATE OOC MESSAGE TO {targetSession.PlayerName}", message, ConstantColours.Yellow, cmd.Session.Source);
        }

        private void OnAdvertCommand(Command cmd)
        {
            Log.ToClient($"[Advert][{cmd.Session.GetCharacterName()}]", string.Join(" ", cmd.Args), ConstantColours.Advert);
        }

        private void OnAnonAdvertCommand(Command cmd)
        {
            Log.ToClient($"[Advert]", string.Join(" ", cmd.Args), ConstantColours.Advert);
        }

        private void OnResetSpawn(Command cmd)
        {
            var characterSettings = cmd.Session./*GetCharacterSettings()*/CharacterSettings;
            if (characterSettings.ContainsKey("SpawnLocation"))
                characterSettings.Remove("SpawnLocation");

            cmd.Session./*SetCharacterSettings()*/CharacterSettings = characterSettings;

            Log.ToClient("[Spawn]", $"Reset spawn location", ConstantColours.Job, cmd.Player);
        }

        private void OnRollCommand(Command cmd)
        {
            var amountToRoll = cmd.GetArgAs(0, 2);
            var dieSides = cmd.GetArgAs(1, 6);

            var rand = new Random();
            var rollValues = new List<int>();

            for (var i = 0; i < amountToRoll; i++)
            {
                rollValues.Add(rand.Next(1, dieSides));
            }

            Messages.SendProximityMessage(cmd.Session, "[Roll]", $"{cmd.Session.FirstName} rolled ^2{amountToRoll}^0 die, giving the following. ^2{string.Join("^0,^2 ", rollValues)}^0 having a total value of ^2{rollValues.Sum()}", ConstantColours.Green, 25.0f);
        }
    }
}
