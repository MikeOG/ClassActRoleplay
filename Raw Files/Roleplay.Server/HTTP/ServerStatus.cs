using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Session;
using Roleplay.Shared;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;

namespace Roleplay.Server.HTTP
{
    public class ServerStatus : ServerAccessor
    {
        public ServerStatus(Server server) : base(server)
        {
            Task.Factory.StartNew(async () =>
            {
                while (server.Get<HTTPHandler>() == null)
                    await BaseScript.Delay(0);

                server.Get<HTTPHandler>().RegisterHttpCallback("GET", "/server-status", handleResponse);
            });
        }

        private void handleResponse(dynamic request, dynamic response)
        {
            var statusDict = new Dictionary<string, dynamic>
            {
                {"players", Server.Instances.Queue.TotalPlayerCount},
                {"queue", Server.Instances.Queue.QueueCount},
                {"uptime", getUptime()},
                //{"maxclients", GetConvarInt("sv_maxClients", 32)}
                {"average wait", Server.Instances.Queue.AverageWaitTime }
            };

            response.send(JsonConvert.SerializeObject(statusDict));
        }

        private string getUptime()
        {
            var timeString = "Error calculating uptime";
            var timeSinceJoin = (DateTime.Now - Server.StartTime);

            /*var seconds = Math.Round(timeSinceJoin.TotalSeconds);
            var minutes = Math.Round(timeSinceJoin.TotalMinutes);
            var hours = Math.Round(timeSinceJoin.TotalHours);

            if (seconds <= 60)
            {
                timeString = $"{seconds} second{(seconds > 1 ? "s" : "")}";
            }

            if (minutes >= 1)
            {
                timeString = $"{minutes} minute{(minutes > 1 ? "s" : "")}";
            }

            if (hours >= 1)
            {
                timeString = $"{hours} hour{(hours > 1 ? "s" : "")}";

                var curMin = hours * 60 - minutes;
                if (curMin > 0)
                {
                    timeString += $" {curMin} minute{(curMin > 1 ? "s" : "")}";
                }
            }*/

            try
            {
                var seconds = timeSinceJoin.Seconds;
                var minutes = timeSinceJoin.Minutes;
                var hours = timeSinceJoin.Hours;

                if (seconds <= 60)
                {
                    timeString = $"{seconds} second{(seconds > 1 ? "s" : "")}";
                }

                if (minutes >= 1)
                {
                    timeString = $"{minutes} minute{(minutes > 1 ? "s" : "")} {seconds} second{(seconds > 1 ? "s" : "")}";
                }

                if (hours >= 1)
                {
                    timeString = $"{hours} hour{(hours > 1 ? "s" : "")}";

                    if (minutes > 0)
                    {
                        timeString += $" {minutes} minute{(minutes > 1 ? "s" : "")}";
                    }
                }
            }
            catch { }

            return timeString;
        }
    }
}
