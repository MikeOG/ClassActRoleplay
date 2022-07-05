using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Enviroment
{
    public class Alerts : ClientAccessor
    {
        private float MaxMessageDistance = 20.0f;

        public Alerts(Client client) : base(client)
        {
            client.RegisterEventHandler("Alerts.SendMeMessage", new Action<int, string>(OnMeMessage));
            client.RegisterEventHandler("Alerts.SendDoMessage", new Action<int, string>(OnDoMessage));
            client.RegisterEventHandler("Alerts.SendStatusMessage", new Action<int>(OnStatusMessage));
        }

        private void OnMeMessage(int targetPlayer, string message)
        {
            var sourcePlayerChar = Client.PlayerList.FirstOrDefault(o => o.ServerId == targetPlayer)?.Character;

            if(sourcePlayerChar == null) return;

            if(Game.PlayerPed.Position.DistanceToSquared(sourcePlayerChar.Position) <= MaxMessageDistance)    
                Log.ToChat("", $"^6{message}");
        }

        private async void OnStatusMessage(int targetPlayer)
        {

            var sourcePlayerChar = Client.PlayerList.FirstOrDefault(o => o.ServerId == targetPlayer)?.Character;

            if (sourcePlayerChar == null) return;

            bool successParse = Guid.TryParse((LocalSession.GetLocalData("User.Status.Guid", "")), out Guid myGuid);

            if (successParse)
            {

                while (Guid.TryParse((LocalSession.GetLocalData("User.Status.Guid", "")), out Guid guidTwo) && guidTwo == myGuid)
                {
                    TriggerServerEvent("Chat.AutomaticMe", LocalSession.GetLocalData("User.StatusText", ""));
                    await BaseScript.Delay(5000);
                }
            }
        }

        private void OnDoMessage(int targetPlayer, string message)
        {
            var sourcePlayerChar = Client.PlayerList.FirstOrDefault(o => o.ServerId == targetPlayer)?.Character;

            if (sourcePlayerChar == null) return;

            if (Game.PlayerPed.Position.DistanceToSquared(sourcePlayerChar.Position) <= MaxMessageDistance)
                Log.ToChat("Action:", $"{message}", ConstantColours.Do);
        }
    }
}
