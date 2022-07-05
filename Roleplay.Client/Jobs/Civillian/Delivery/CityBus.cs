using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Jobs.Bases;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.Civillian.Delivery
{
    public class CityBus : DeliveryJob
    {
        public CityBus()
        {
            DeliveryVehicleHash = VehicleHash.Bus;
            VehicleSpawnLocation = new Vector3(442.2278f, -577.2351f, 26.9717f);
            MarkerLocation = new Vector3(454.4794f, -571.8281f, 27.4998f);
            VehicleSpawnHeading = 180.0f;
            StartString = "Drive to each stop to get paid";
            JobBlipName = "Bus stop";
            DeliveryLocations = new List<Vector3>
            {
                new Vector3(-231.0231f, -975.4913f, 29.1787f),
                new Vector3(-208.6053f, -1150.1742f, 22.9409f),
                new Vector3(11.9546f, -1145.9977f, 28.6828f),
                new Vector3(136.808f, -952.109f, 29.5709f),
                new Vector3(273.976f, -591.54f, 43.1383f),
                new Vector3(194.523f, -194.268f, 53.9052f),
                new Vector3(-154.592f, -68.6344f, 53.854f),
                new Vector3(-428.323f, -72.7076f, 43.1175f),
                new Vector3(-675.871f, -203.374f, 37.1932f),
                new Vector3(-595.464f, -669.25f, 31.9522f),
                new Vector3(-244.284f, -714.551f, 33.4407f),
            };
            DrawMarker();
            CreateBlip();
        }

        protected override List<Vector3> GetDeliveryLocations()
        {
            return DeliveryLocations;
        }

        public override void GetNextDeliveryLocation()
        {
            base.GetNextDeliveryLocation();
            if (CurrentDeliveryLocation == DeliveryLocations.Last())
            {
                CurrentDeliveryLocation = DeliveryLocations[0];
                loadDeliveryBlip();
            }
        }

        public override void GiveJobPayment()
        {
            Client.TriggerServerEvent("Job.RequestPayForJob", JobType.Delivery.ToString(), "CityBus");
            GetNextDeliveryLocation();
        }

        protected override void CreateBlip()
        {
            BlipHandler.AddBlip("Bus station", MarkerLocation, new BlipOptions
            {
                Sprite = BlipSprite.GTAOMission
            });
        }
    }
}
