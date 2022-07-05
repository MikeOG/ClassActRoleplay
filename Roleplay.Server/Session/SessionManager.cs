using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Session
{
    public class SessionManager : ServerAccessor
    {
        private readonly List<Session> currentSessions = new List<Session>();

        public IReadOnlyList<Session> PlayerList => new List<Session>(currentSessions);

        public SessionManager(Server server) : base(server)
        {
            server.RegisterEventHandler("playerConnecting", new Action<Player, string, dynamic, dynamic>(OnInitialConnect));
            server.RegisterEventHandler("playerDropped", new Action<Player, string>(OnPlayerDropped));
            server.RegisterEventHandler("Session.Loaded", new Action<Player>(OnPlayerLoaded));
            server.RegisterEventHandler("Session.UpdateClientData", new Action<Player, string, string>(OnDataUpdate));
            server.RegisterEventHandler("Session.SetPlayerStatus", new Action<Player, string, dynamic>(OnSetStatus)); // TEMP
            server.RegisterEventHandler("chatMessage", new Action<int, string, string>(OnChatMessageEntered));
            server.RegisterLocalEvent("Log.ToDatabase", new Action<string, string, string, string>(OnLogData));
            server.RegisterTickHandler(SaveTick);
            CommandRegister.RegisterAdminCommand("listuser", cmd =>
            {
                var player = GetPlayer(cmd.GetArgAs(0, 0));
                if (player == null) return;

                Log.Info($"Server ID: {player.ServerID}");
                Log.Info($"Player name: {player.PlayerName}");
                Log.Info($"Player ped handle: {player.Ped?.Handle}");
                Log.Info($"Position: {player.Position}");
                foreach (var kvp in player.GlobalData)
                {
                    Log.Info($"{kvp.Key} - {kvp.Value}");
                }

                foreach (var kvp in player.LocalData)
                {
                    Log.Info($"{kvp.Key} - {kvp.Value}");
                }

                foreach (var kvp in player.ServerData)
                {
                    Log.Info($"{kvp.Key} - {kvp.Value}");
                }
            }, AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("saveusers", cmd => SaveAllUsers(), AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("savealldata|savedata", cmd =>
            {
                CitizenFX.Core.Native.API.ExecuteCommand("saveusers");
                CitizenFX.Core.Native.API.ExecuteCommand("savevehicles");
                CitizenFX.Core.Native.API.ExecuteCommand("saveproperties");
            }, AdminLevel.SuperAdmin);
        }

        public Session GetPlayer(Player source) => currentSessions.FirstOrDefault(o => int.TryParse(source.Handle, out var serverID) && o.ServerID == serverID);
        public Session GetPlayer(int serverID) => currentSessions.FirstOrDefault(o => o.ServerID == serverID);
        public Session GetPlayerBySteamID(string steamId) => currentSessions.FirstOrDefault(o => o.SteamIdentifier == steamId);
        public Session GetPlayerByCharID(int charId) => currentSessions.FirstOrDefault(o => o.CharId == charId);

        public void ForAllClients(Action<Session> actionFunc)
        {
            var plyList = PlayerList;
            for (var i = 0; i < plyList.Count; i ++)
            {
                try
                {
                    var session = plyList[i];
                    actionFunc(session);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public async Task ForAllClientsAsync(Func<Session, Task> actionTask)
        {
            var plyList = PlayerList;
            for (var i = 0; i < plyList.Count; i++)
            {
                try
                {
                    var session = plyList[i];
                    await actionTask(session);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private void OnPlayerDropped([FromSource] Player source, string reason)
        {
            var session = GetPlayer(source);
            if (session == null) return;

            currentSessions.Remove(session);
            SavePlayerData(session);
            TriggerSessionEvent("OnPlayerDisconnect", session, reason);
            BaseScript.TriggerClientEvent("Session.RemovePlayer", session.ServerID);
            Log.Verbose($"{session.PlayerName} has disconnected ({reason})");
            Server.TriggerLocalEvent("Log.ToDatabase", session.PlayerName, session.SteamIdentifier, "disconnect", reason);
        }
        
        private async void OnInitialConnect([FromSource] Player source, string playerName, dynamic setKickReason, dynamic d)
        {
            d.defer();
            await BaseScript.Delay(0);
            d.update("Checking ban status please wait...");

            if (string.IsNullOrEmpty(source.Identifiers["steam"]))
            {
               d.done("Steam must be running to join this server. Please restart Steam and FiveM and try and join again");
                return;
            }

            var dummySession = new Session(source);
            var isBanned = await CheckIfBanned(dummySession);

            if (isBanned)
            {
                d.done(dummySession.GetServerData("Queue.RemovalMessage", ""));
                return;
            }

            var oldSession = GetPlayer(source);//GetPlayerBySteamID(playerSession.SteamIdentifier);
            if (oldSession != null) // Sometimes playerDropped doesn't call so we'll do it here just in case
            {
                Log.Debug($"{oldSession.PlayerName} had old session data in the server. Saving and removing");
                SavePlayerData(oldSession);
                currentSessions.Remove(oldSession);
                
            }

            oldSession = new Session(source);
            LoadUserData(oldSession);
            currentSessions.Add(oldSession);

            d.done();
        }

        private async Task<bool> CheckIfBanned(Session session)
        {
            bool? isBanned = null;

            var idenString = $"{string.Join("\' OR BannedIdentifier = \'", session.Source.Identifiers.ToList())}";
            Server.Instance.MySQL.execute($"SELECT * FROM user_bans WHERE CURRENT_TIMESTAMP < BannedExpires AND (BannedIdentifier = \'{idenString}\') AND BanActive = true", new Dictionary<string, dynamic>(),
                new Action<List<dynamic>>(result =>
                {
                    var banned = result.ElementAtOrDefault(0) != null;

                    if (banned)
                    {
                        var data = result[0];
                        session.SetServerData("Queue.RemovalMessage", $"You are currently banned for: {data.BanReason} until {UnixToDatetime(data.BannedExpires).ToString("HH:mm dd-MM-yyyy")}. By the staff member {data.Banner}. BanID: {data.BanID}. To dispute a ban go to dedifire.net and fill out a ban appeal.");
                    }

                    isBanned = banned; // Has to return false to fail verification
                }));

            var ticks = 0;
            while (isBanned == null && ticks < 300)
            {
                ticks++;
                await BaseScript.Delay(0);
            }

            if (ticks >= 300) isBanned = true;

            return (bool)isBanned;
        }

        private DateTime UnixToDatetime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp / 1000);
            return dtDateTime;
        }

        public async void OnPlayerLoaded([FromSource] Player source)
        {
            var playerSession = GetPlayerBySteamID(source.Identifiers[Server.CurrentIdentifier]);

            if (playerSession == null)
            {
                playerSession = new Session(source);
                LoadUserData(playerSession);
            }
            else
            {
                currentSessions.Remove(playerSession);
                playerSession = new Session(source, playerSession.GlobalData, playerSession.LocalData, playerSession.ServerData);
            }
            currentSessions.Add(playerSession);
            TriggerSessionEvent("AddSessionData", playerSession);

            Log.Verbose($"{source.Name} successfully loaded into the server and joined. Re-syncing clients");
            foreach (var i in PlayerList)
            {
                var sessionModel = new SessionData
                {
                    ServerID = playerSession.ServerID,
                    GlobalData = playerSession.GlobalData,
                };

                if (i.Source.Handle == source.Handle)
                {
                    sessionModel.LocalData = playerSession.LocalData;
                }
                else
                {
                    var otherSessionModel = new SessionData
                    {
                        ServerID = i.ServerID,
                        GlobalData = i.GlobalData
                    };

                    playerSession.TriggerEvent("Session.AddPlayer", i.ServerID, JsonConvert.SerializeObject(otherSessionModel));
                }

                i.TriggerEvent("Session.AddPlayer", playerSession.ServerID, JsonConvert.SerializeObject(sessionModel));
                await BaseScript.Delay(0);
            }
            Log.Verbose($"Finished sending {source.Name}s session data to other clients");
        }

        public async void TriggerSessionEvent(string eventName, /*Session sessionToPass*/ params dynamic[] args)
        {
            foreach (var instance in (IDictionary<string, dynamic>)Server.Instances)
            {
                try
                {
                    MethodInfo sessionEvent = instance.Value.GetType().GetMethod(eventName);
                    if (sessionEvent != null)
                    {
                        Log.Debug($"Invoking {sessionEvent.Name} in class {instance.Value}");
                        sessionEvent.Invoke(instance.Value, /*new []{ sessionToPass }*/ args);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public async Task SaveAllUsers() => await ForAllClientsAsync(async o =>
        {
            SavePlayerData(o);
            await BaseScript.Delay(0);
        });

        private void OnSetStatus([FromSource] Player source, string status, dynamic data)
        {
            var session = GetPlayer(source);
            if (session == null) return;

            Log.Info($"{source.Name} is remotely setting {status} to {data}");
            session.SetGlobalData(status, data);
        }

        private void LoadUserData(Session target)
        {
            if (string.IsNullOrEmpty(target.Source.Identifiers["steam"])) return;

            Log.Verbose($"Loading user data for {target.PlayerName}");

            MySQL.execute("SELECT * FROM user_data WHERE SteamID = @steam", new Dictionary<string, dynamic>
            {
                ["@steam"] = target.SteamIdentifier
            }, new Action<List<object>>(result =>
            {
                if (result.ElementAtOrDefault(0) == null)
                {
                    RegisterUserData(target);
                }
                else
                {
                    var localDict = new Dictionary<string, dynamic>();
                    foreach (var i in (dynamic)result[0])
                    {
                        localDict[$"User.{i.Key}"] = i.Value;
                    }
                    target.SetLocalData(localDict);
                }
            }));
        }

        private void RegisterUserData(Session target)
        {
            Log.Verbose($"Registering user data for {target.PlayerName}");
            MySQL.execute("INSERT INTO user_data (`SteamID`, `PlayerName`, `Identifiers`, `Settings`) VALUES (@steam, @playername, @idens, @settings)", new Dictionary<string, dynamic>
            {
                ["@steam"] = target.SteamIdentifier,
                ["@playername"] = target.Source.Name,
                ["@idens"] = JsonConvert.SerializeObject(target.Source.Identifiers.ToList()),
                ["@settings"] = JsonConvert.SerializeObject(new Dictionary<string, dynamic>())
            }, new Action<int>(rows => LoadUserData(target)));
        }

        private async Task SaveTick()
        {
            await BaseScript.Delay(CitizenFX.Core.Native.API.GetConvarInt("mg_saveInterval", 300000));

            Log.Info("Saving user data");
            await SaveAllUsers();
            Log.Info("Saving user data complete");
        }

        private void SavePlayerData(Session playerSession)
        {
            if (playerSession.GetGlobalData("Character.FirstName", "undefined") != "undefined")
            {
                var playerJob = JobHandler.GetPlayerJob(playerSession);

                if (playerSession.GetGlobalData("Character.Cash", 0) < 0)
                {
                    playerSession.SetGlobalData("Character.Cash", 0);
                }

                MySQL.execute("UPDATE character_data SET FirstName = @first, LastName = @last, Job = @job, Cash = @cash, JailTime = @jail, Inventory = @inv, SkinData = @skin, bill = @bill, Home = @home, Settings = @settings WHERE CharID = @char",
                    new Dictionary<string, dynamic>
                    {
                        ["@first"] = playerSession.GetGlobalData<string>("Character.FirstName"),
                        ["@last"] = playerSession.GetGlobalData<string>("Character.LastName"),
                        ["@job"] = JobHandler.SaveableJobTypes.Contains(playerJob) ? playerJob.ToString() : "Civillian",
                        ["@cash"] = playerSession.GetGlobalData<int>("Character.Cash"),
                        ["@jail"] = playerSession.GetGlobalData<int>("Character.JailTime"),
                        ["@inv"] = playerSession.GetGlobalData<string>("Character.Inventory"),
                        ["@skin"] = playerSession.GetGlobalData<string>("Character.SkinData"),
                        ["@bill"] = playerSession.GetGlobalData<int>("Character.Bill"),
                        ["@char"] = playerSession.GetGlobalData<int>("Character.CharID"),
                        ["@home"] = playerSession.GetGlobalData<int>("Character.Home"),
                        ["@settings"] = playerSession.GetGlobalData<string>("Character.Settings"),
                    });
            }

            if (playerSession.GetLocalData("User.PermissionLevel", -1) != -1)
                MySQL.execute("UPDATE user_data SET /*PermissionLevel = @perm,*/ Strikes = @strike, Reports = @report, PlayerName = @name, Identifiers = @idens, Settings = @settings WHERE SteamID = @id",
                    new Dictionary<string, dynamic>
                    {
                        ["@strike"] = playerSession.GetLocalData<int>("User.Strikes"),
                        ["@report"] = playerSession.GetLocalData<int>("User.Reports"),
                        ["@name"] = playerSession.PlayerName,
                        ["@idens"] = playerSession.GetLocalData<string>("User.Identifiers"),
                        ["@settings"] = playerSession.GetLocalData<string>("User.Settings"),
                        ["@id"] = playerSession.SteamIdentifier
                    });
        }

        private void OnChatMessageEntered(int source, string name, string message)
        {
            var sourceObj = Players[source];
            if (message.Substring(0, 1) == "/")
            {
                Log.ToClient("LOG", $"The command {message.Split(' ')[0]} doesn't exist in the server", ConstantColours.Log, sourceObj);
            }
            else
            {
                sourceObj.TriggerEvent("Player.ExecuteCommand", $"pooc {message}");
            }

            CitizenFX.Core.Native.API.CancelEvent();
        }

        private void OnDataUpdate([FromSource] Player source, string updateData, string fileLocation)
        {
            var session = GetPlayer(source);
            if (session == null) return;

            Log.Verbose($"Re-syncing session data for {session.PlayerName} (called from {fileLocation})");
            session.ResyncData(updateData);
        }

        private void OnLogData(string username, string identifier, string type, string text)
        {
            Log.Debug($"About to insert a log of type {type} for identifier {identifier}");
            MySQL.execute("INSERT INTO server_logs (`logger_username`, `logger_identifier`, `log_type`, `log_text`) VALUES (@username, @iden, @type, @text)", new Dictionary<string, dynamic>
            {
                ["@username"] = username,
                ["@iden"] = identifier,
                ["@type"] = type,
                ["@text"] = text
            });
        }
    }
}
