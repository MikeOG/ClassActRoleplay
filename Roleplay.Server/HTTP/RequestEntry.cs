using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Newtonsoft.Json;

namespace Roleplay.Server.HTTP
{
    public class RequestEntry
    {
        public string RequestMethod = "POST";
        public string RequestPath;
        public Action<dynamic, dynamic> RequestFunc;
        public Action<dynamic, dynamic, dynamic> POSTRequestFunc;

        public void HandleRequest(dynamic request, dynamic response, dynamic body = null)
        {
            var isAuthenticated = AuthenticateRequest(request);

            if (isAuthenticated)
            {
                if (body == null)
                    RequestFunc(request, response);
                else
                    POSTRequestFunc(request, response, body);
            }
            else
            {
                response.writeHead(403);
                response.send("Unauthorized request");
            }
        }

        protected internal bool AuthenticateRequest(dynamic request)
        {
            if (RequestMethod == "GET")
            {
                return true;
            }
            else
            {
                IDictionary<string, dynamic> headers = request.headers;

                return headers.ContainsKey("auth-token") && headers["auth-token"] == "g5TadZz34Rd";
            }
        }
    }
}
