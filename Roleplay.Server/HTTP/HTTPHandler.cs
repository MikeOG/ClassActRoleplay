using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Jobs.EmergencyServices;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Shared;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.HTTP
{
    public class HTTPHandler : ServerAccessor
    {
        private List<RequestEntry> serverRequests = new List<RequestEntry>();

        public HTTPHandler(Server server) : base(server)
        {
            //SetHttpHandler(new Action<dynamic, dynamic>(handleHttpRequest));
            Server.RegisterLocalEvent("HTTP.OnHTTPRequest", new Action<dynamic, dynamic>(handleHttpRequest));
            Server.RegisterLocalEvent("HTTP.OnPOSTRequest", new Action<dynamic, dynamic, dynamic>(handleRequest));
            RegisterHttpCallback("POST", "/cad-alerts", (request, response, body) =>
            {
                response.send(JsonConvert.SerializeObject(CADAlerts.CurrentAlerts));
            });
        }

        public void RegisterHttpCallback(string requestMethod, string requestPath, Action<dynamic, dynamic> requestFunc)
        {
            serverRequests.Add(new RequestEntry
            {
                RequestMethod = requestMethod,
                RequestPath = requestPath,
                RequestFunc = requestFunc
            });

            Log.Verbose($"Added a HTTP callback for path {GetLocalIPAddress()}:30120/{GetCurrentResourceName()}{requestPath} for a request method of {requestMethod}");
        }
        
        public void RegisterHttpCallback(string requestMethod, string requestPath, Action<dynamic, dynamic, dynamic> requestFunc)
        {
            serverRequests.Add(new RequestEntry
            {
                RequestMethod = requestMethod,
                RequestPath = requestPath,
                POSTRequestFunc = requestFunc
            });

            Log.Verbose($"Added a HTTP POST specific callback for path {GetLocalIPAddress()}:30120/{GetCurrentResourceName()}{requestPath} for a request method of {requestMethod}");
        }

        private void handleHttpRequest(dynamic request, dynamic response)
        {
            return;
            handleRequest(request, response);
        }

        private void handleRequest(dynamic request, dynamic response, dynamic body = null)
        {
            return;
            Log.Verbose($"Just recieved a {request.method} request for path {request.path}");

            var requestEntry = serverRequests.FirstOrDefault(o => o.RequestPath == request.path && o.RequestMethod == request.method);

            if (requestEntry != null)
            {
                requestEntry.HandleRequest(request, response, body);
            }
            else
            {
                response.writeHead(403);
                response.send("Unauthorized access");
            }
        }

        private string GetLocalIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            string ip = "";
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = Convert.ToString(IP);
                }
            }
            return ip;
        }
    }
}
