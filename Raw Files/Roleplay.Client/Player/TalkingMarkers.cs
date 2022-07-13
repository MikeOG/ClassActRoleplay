using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Helpers;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Player
{
    public class TalkingMarkers : ClientAccessor
    {
        public TalkingMarkers(Client client) : base(client)
        {
            client.RegisterTickHandler(MarkerTick);
        }

        private async Task MarkerTick()
        {
            if(CinematicMode.InCinematicMode) return;

            var playerPed = Game.PlayerPed;
            Client.PlayerList.ToList().ForEach(o =>
            {
                if (NetworkIsPlayerTalking(o.Handle))
                {
                    var otherPed = o.Character;
                    if (otherPed.IsInVehicle()) return;

                    var playerPos = otherPed.Position;
                    playerPos.Z -= 0.95f;

                    if(otherPed == playerPed) return;

                    World.DrawMarker(MarkerType.HorizontalCircleFat, playerPos, Vector3.Zero, Vector3.Zero, 0.8f * new Vector3(1, 1, 1), ConstantColours.TalkMarker);
                }
            });
        }
    }
}
