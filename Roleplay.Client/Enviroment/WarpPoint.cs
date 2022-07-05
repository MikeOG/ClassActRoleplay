using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Enviroment
{
    public class WarpPoint
    {
        public Vector3 EnterLocation { get; }
        public Vector3 ExitLocation { get; }

        public WarpPoint(Vector3 enterLocation, Vector3 exitLoccation, Color markerColour = default(Color), float markerScale = 1.0f)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            MarkerHandler.AddMarkerAsync(new List<Vector3>
            {
                enterLocation,
                exitLoccation
            }, new MarkerOptions
            {
                Color = markerColour == default(Color) ? ConstantColours.Yellow : markerColour,
                ScaleFloat = markerScale
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            EnterLocation = enterLocation;
            ExitLocation = exitLoccation;

            Client.Instance.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
        }

        private async void OnInteraction()
        {
            var playerPed = Cache.PlayerPed;
            var closeToEnter = EnterLocation.DistanceToSquared(playerPed.Position) < 1.0f;

            if (closeToEnter)
            {
                await playerPed.TeleportToLocation(ExitLocation, true);
            }
            else
            {
                if (ExitLocation.DistanceToSquared(playerPed.Position) < 1.0f)
                {
                    await playerPed.TeleportToLocation(EnterLocation, true);
                }
            }
        }
    }
}
