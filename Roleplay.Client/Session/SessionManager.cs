using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Session
{
    public class SessionManager : ClientAccessor
    {
        private readonly List<Session> currentSessions = new List<Session>();
        private bool firstSpawn = true;

        public IReadOnlyList<Session> PlayerList => new List<Session>(currentSessions);

        public SessionManager(Client client) : base(client)
        {
            client.RegisterEventHandler("Session.UpdateGlobalData", new Action<int, string>(UpdateGlobalData));
            client.RegisterEventHandler("Session.UpdateLocalData", new Action<int, string>(UpdateLocalData));
            client.RegisterEventHandler("Session.AddPlayer", new Action<int, string>(AddPlayer));
            client.RegisterEventHandler("Session.RemovePlayer", new Action<int>(RemovePlayer));
            client.RegisterEventHandler("playerSpawned", new Action(() =>
            {
                if (firstSpawn)
                {
                    BaseScript.TriggerEvent("Session.Loaded");
                    BaseScript.TriggerServerEvent("Session.Loaded");
                    client.GetExports("spawnmanager").setAutoSpawn(false);
                    firstSpawn = false;
                }
            }));
        }

        public Session GetPlayer(CitizenFX.Core.Player source) => GetPlayer(source.ServerId);
        public Session GetPlayer(int playerHandle) => currentSessions.FirstOrDefault(o => o.ServerID == playerHandle);

        private void UpdateGlobalData(int serverID, string data)
        {
            var playerSession = GetPlayer(serverID);
            if (playerSession == null) return;

            playerSession.UpdateGlobalData(data);
        }

        private void UpdateLocalData(int serverID, string data)
        {
            var playerSession = GetPlayer(serverID);
            if (playerSession == null) return;

            playerSession.UpdateLocalData(data);
        }

        private void AddPlayer(int serverID, string data)
        {
            var model = JsonConvert.DeserializeObject<SessionData>(data);
            var oldSession = GetPlayer(serverID);

            if (oldSession != null)
            {
                Log.Verbose($"Removing old session data for ServerID {serverID}");
                currentSessions.Remove(oldSession);
            }

            var session = new Session(serverID, model);
            currentSessions.Add(session);
            Log.Verbose($"Added session data for {session.Player.Name}. ServerID is {serverID}");
        }

        private void RemovePlayer(int serverID)
        {
            var playerSession = GetPlayer(serverID);

            if (playerSession == null) return;
            Log.Verbose($"Removing session data for {playerSession.Player.Name}");

            currentSessions.Remove(playerSession);
        }
    }
}
