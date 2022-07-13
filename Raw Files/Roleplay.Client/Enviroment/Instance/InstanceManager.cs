using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Session;
using Roleplay.Shared;

namespace Roleplay.Client.Enviroment.Instance
{
    public class InstanceManager : ClientAccessor
    {
        public static int DefaultInstance = GetConvarInt("mg_defaultInstanceId", 0);
        private static int previousInstance = DefaultInstance;

        public InstanceManager(Client client) : base(client)
        {
            client.RegisterTickHandler(InstanceCheckTick); 
        }

        private async Task InstanceCheckTick()
        {
            if (Client.LocalSession == null) return;

            var localSession = Client.LocalSession;

            var currentInstance = localSession.GetGlobalData("Character.Instance", 0);
            var currentPlayers = Client.Get<SessionManager>().PlayerList;

            foreach (var player in currentPlayers)
            {
                if (player == localSession || player.Player.Character.Handle == localSession.Player.Character.Handle) continue;

                var playerInstance = player.GetGlobalData("Character.Instance", 0);
                var isVisible = currentInstance == playerInstance; 
                var playerPed = player.Player.Character;
                playerPed.IsCollisionEnabled = isVisible;
                playerPed.IsVisible = isVisible;
            }

            if (currentInstance != previousInstance)
            {
                Log.Debug($"Detected a new instance. {previousInstance} -> {currentInstance}");
                NetworkSetVoiceChannel(currentInstance);

                if (currentInstance == 0)
                {
                    NetworkClearVoiceChannel();
                }

                previousInstance = currentInstance;
            }
        }
    }
}
