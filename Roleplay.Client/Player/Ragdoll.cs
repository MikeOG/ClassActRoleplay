using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enums;
using Roleplay.Client.Player.Controls;

namespace Roleplay.Client.Player
{
    public class Ragdoll : ClientAccessor
    {
        private bool isInRagdoll = false;

        public Ragdoll(Client client) : base(client)
        {
            client.RegisterTickHandler(RagdollTick);

            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(RagdollTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.RegisterTickHandler(RagdollTick);
            }));
        }

        private async Task RagdollTick()
        {
            if (Input.IsControlJustPressed(Control.ReplayScreenshot))
            {
                isInRagdoll = !isInRagdoll;
                //Game.PlayerPed.Ragdoll(1000);
            }

            if (isInRagdoll)
            {
                Game.PlayerPed.Ragdoll(1000);
            }
        }
    }
}
