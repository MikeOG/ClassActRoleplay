using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Newtonsoft.Json;

namespace Roleplay.Server.HTTP
{
    public class ServerRestart : ServerAccessor
    {
        // Amount of time left until a server restart
        private int restartTimer = -1;

        public ServerRestart(Server server) : base(server)
        {
            Task.Factory.StartNew(async () =>
            {
                while (server.Get<HTTPHandler>() == null)
                    await BaseScript.Delay(0);

                server.Get<HTTPHandler>().RegisterHttpCallback("POST", "/restart", handleResponse);
            });
        }

        private void handleResponse(dynamic request, dynamic response, dynamic body)
        {
            IDictionary<string, dynamic> data = body;
            if (data.ContainsKey("restart-time"))
            {
                if (restartTimer != -1)
                {
                    Server.DeregisterTickHandler(CheckRestartTick);
                }

                if(int.TryParse(data["restart-time"].ToString(), out restartTimer) && restartTimer > 5)
                {
                    Log.Info($"Recieved a restart countdown time of {restartTimer} minutes beginning timer");
                    Server.RegisterTickHandler(CheckRestartTick);
                    return;
                }
            }

            response.send(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"status", "fail" },
                {"error", "Unable to location a restart-time entry in the request body or restart-time entry was invalid" }
            }));
        }

        private async Task CheckRestartTick()
        {
            if(restartTimer <= 10)
            {
                CitizenFX.Core.Native.API.ExecuteCommand($"say Server will be restarting in {restartTimer} minutes");

                if (restartTimer <= 5)
                {
                    Log.Info($"Less than 5 minutes until server restart. Disabling server joining");
                    CitizenFX.Core.Native.API.SetConvar("mg_queueJoiningEnabled", "false");
                }

                if (restartTimer <= 2)
                {
                    Log.Info($"Less than 2 minutes until server restart. Kicking all players from server and saving data");
                    foreach (var player in /*Server.PlayerList*/ Players)
                    {
                        player.Drop($"Server is restarting in {restartTimer} minutes");
                    }

                    CitizenFX.Core.Native.API.ExecuteCommand("savealldata");
                }
            }

            restartTimer--;
            await BaseScript.Delay(60000);
        }
    }
}
