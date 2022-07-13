using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
 /*   public class SirenEvents : ServerAccessor
    {
        public SirenEvents(Server server) : base(server)
        {
            server.RegisterEventHandler("Sirens.SendStateUpdate", new Action<string, int>(handleSirenState));
            server.RegisterEventHandler("Sirens.SetSirenOutOfVehState", new Action<Player, bool>(handleSirenOutOfVehState));
        }

        private void handleSirenState(string sound, int source)
        {
            BaseScript.TriggerClientEvent("Sirens.UpdateSirenState", sound, source);
        }

        private void handleSirenOutOfVehState([FromSource] Player source, bool state)
        {
            BaseScript.TriggerClientEvent("Sirens.RecieveSirenOutOfVehState", Convert.ToInt32(source.Handle), state);
        }

    }*/
}
