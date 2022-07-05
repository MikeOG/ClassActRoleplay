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
    public class RemoteMessaging : ServerAccessor
    {
        public RemoteMessaging(Server server) : base(server)
        {
            Task.Factory.StartNew(async () =>
            {
                while (server.Get<HTTPHandler>() == null)
                    await BaseScript.Delay(0);

                server.Get<HTTPHandler>().RegisterHttpCallback("POST", "/message", handleResponse);
            });
        }

        private void handleResponse(dynamic request, dynamic response, dynamic data)
        {
            IDictionary<string, dynamic> body = data;

            if (body.ContainsKey("message"))
            {
                var msg = body["message"];
                CitizenFX.Core.Native.API.ExecuteCommand($"say {msg}");

                response.send(JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "status", "ok" }
                }));
                return;
            }

            response.send(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "status", "fail" },
                { "error", "Failed to provide a message within your body" }
            }));
        }
    }
}
