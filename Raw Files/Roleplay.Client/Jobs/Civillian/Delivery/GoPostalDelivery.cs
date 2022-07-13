using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using Roleplay.Client.Jobs.Bases;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Models;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.Civillian.Delivery
{
    public sealed class GoPostalDelivery : DeliveryJob
    {
        public GoPostalDelivery()
        {
            VehicleSpawnLocation = new Vector3(66.02f, 116.51f, 79.1f);
            MarkerLocation = new Vector3(79.26f, 111.97f, 81.17f);
            VehicleSpawnHeading = 79.1f;
            DeliveryLocations = new List<Vector3>
            {
                new Vector3(822.008f, -2143.18f, 28.8315f),
                new Vector3(851.084f, -1010.56f, 28.6684f),
                new Vector3(-669.031f, -952.288f, 21.1993f),
                new Vector3(-1300.12f, -384.517f, 36.5631f),
                new Vector3(241.541f, -43.0412f, 69.7362f),
                new Vector3(2554.91f, 288.592f, 108.461f),
                new Vector3(-1131.3f, 2698.17f, 18.8004f),
                new Vector3(1697.34f, 3744.89f, 34.0315f),
                new Vector3(-318.431f, 6082.66f, 31.4622f),
                new Vector3(-3186.73f, 1085.29f, 20.8402f),
                new Vector3(123.85f, 6623.38f, 31.82f),
                new Vector3(-448.24f, 6032.63f, 31.34f),
                new Vector3(424.23f, -1014.63f, 29.02f),

            };
            DrawMarker();
            CreateBlip();
        }

        protected override void CreateBlip()
        {
            BlipHandler.AddBlip("Postal depot", MarkerLocation, new BlipOptions
            {
                Sprite = BlipSprite.GTAOMission
            });
        }
    }
}