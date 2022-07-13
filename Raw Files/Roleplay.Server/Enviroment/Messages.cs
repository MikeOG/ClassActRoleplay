using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Enviroment
{
    public class Messages : ServerAccessor
    {
        public Messages(Server server) : base(server)
        {

        }

        [EventHandler("Messages.SendProximityMessage")]
        private void OnProximityMessage([FromSource] Player source, string prefix, string message, float distance, List<object> colourArray = null)
        {
            var session = Sessions.GetPlayer(source);
            if (session == null) return;

            var colour = colourArray != null && colourArray.Count == 4 ? Color.FromArgb(int.Parse(colourArray[0].ToString()), int.Parse(colourArray[1].ToString()), int.Parse(colourArray[2].ToString()), int.Parse(colourArray[3].ToString())) : ConstantColours.Green; 

            SendProximityMessage(session, prefix, message, colour, distance);
        }

        public static void SendProximityMessage(Session.Session sourceSession, string prefix, string message, Color prefixColour, float distance = 6.0f, bool showForSource = true)
        {
            var sourcePos = sourceSession.GetPlayerPosition();
            Server.Instance.Get<SessionManager>().ForAllClients(o =>
            {
                var playerPos = o.Position;

                if(sourcePos.DistanceToSquared(playerPos) <= distance && (showForSource || sourceSession != o))
                    Log.ToClient(prefix, message, prefixColour, o.Source);
            });
        }
    }
}
