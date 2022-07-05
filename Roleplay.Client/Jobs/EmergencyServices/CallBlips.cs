using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.EmergencyServices
{
    public class CallBlips : ClientAccessor
    {
        private Dictionary<int, Blip> playersBlips = new Dictionary<int, Blip>();

        public CallBlips(Client client) : base(client)
        {
            client.RegisterEventHandler("Blip.CreateEmergencyBlip", new Action<int>(CreateEmgerencyBlip));
            client.RegisterEventHandler("Blip.CreateJobBlip", new Action<string, int, int>(CreateJobBlip));
            client.RegisterTickHandler(RemoveBlipTick);
            CommandRegister.RegisterCommand("remblip|delblip", cmd =>
            {
                var targetBlip = cmd.GetArgAs(0, 0);
                if (playersBlips.ContainsKey(targetBlip))
                {
                    playersBlips[targetBlip].Delete();
                    playersBlips.Remove(targetBlip);
                }
            });
        }

        private void CreateEmgerencyBlip(int targetPlayer)
        {
            var player = Client.PlayerList.FirstOrDefault(o => o.ServerId == targetPlayer);

            if (player != null)
            {
                var callerBlip = BlipHandler.AddBlip($"911 #{targetPlayer}", player.Character.Position, new BlipOptions
                {
                    Sprite = (BlipSprite)459,
                    Colour = (BlipColor)44
                });

                if (playersBlips.ContainsKey(targetPlayer))
                    playersBlips[targetPlayer].Delete();

                playersBlips[targetPlayer] = callerBlip;
            }
        }

        private void CreateJobBlip(string blipMessage, int targetPlayer, int blipColour)
        {
            var player = Client.PlayerList.FirstOrDefault(o => o.ServerId == targetPlayer);

            if (player != null)
            {
                var callerBlip = BlipHandler.AddBlip(blipMessage, player.Character.Position, new BlipOptions
                {
                    Sprite = BlipSprite.GTAOMission,
                    Colour = (BlipColor)blipColour
                });

                if (playersBlips.ContainsKey(targetPlayer))
                    playersBlips[targetPlayer].Delete();

                playersBlips[targetPlayer] = callerBlip;
            }
        }

        private async Task RemoveBlipTick()
        {
            if (LocalSession == null) return;

            var playerPos = Cache.PlayerPed.Position;

            var blips = new Dictionary<int, Blip>(playersBlips);
            foreach (var kvp in blips)
            {
                if (playerPos.DistanceToSquared(kvp.Value.Position) < 64.0f)
                {
                    playersBlips[kvp.Key].Delete();
                    playersBlips.Remove(kvp.Key);
                }
            }
        }
    }
}
