using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Player.Controls;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Player
{
    public class HeadIDS : ClientAccessor
    {
        public HeadIDS(Client client) : base(client)
        {
            client.RegisterTickHandler(HeadIDTick);
        }

        private async Task HeadIDTick()
        {
            if (Input.IsControlPressed(Control.FrontendSocialClub))
            {
                var playerPos = Cache.PlayerPed.Position;
                foreach (var player in Client.PlayerList)
                {
                    var pos = player.Character.Position;
                    if(playerPos.DistanceToSquared(pos) <= 250.0f)
                        DrawText3D(pos.X, pos.Y, pos.Z + 0.935f, player.ServerId.ToString());
                }
            }
        }

        private void DrawText3D(float x, float y, float z, string text)
        {
            var textColour = new[]{ 255, 255, 255 };
            var mult = 1.3f;
            var gameplayCamCoords = GetGameplayCamCoords();
            var dist = GetDistanceBetweenCoords(gameplayCamCoords.X, gameplayCamCoords.Y, gameplayCamCoords.Z, x, y, z, true);


            var scale = (1 / dist) * 2;
            var fov = (1 / GetGameplayCamFov()) * 100;
            scale = scale * fov;

            SetDrawOrigin(x, y, z, 0);
            SetTextScale(0.0f, mult * scale);
            SetTextFont(0);
            SetTextProportional(true);
            SetTextColour(textColour[0], textColour[1], textColour[2], 255);
            SetTextDropshadow(0, 0, 0, 0, 255);
            SetTextEdge(2, 0, 0, 0, 150);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            SetTextCentre(true);
            AddTextComponentString(text);
            DrawText(0.0f, 0.0f);
            ClearDrawOrigin();
        }
    }
}
