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
    public class Garbage : DeliveryJob
    {
        public Garbage()
        {
            DeliveryVehicleHash = VehicleHash.Trash;
            VehicleSpawnLocation = new Vector3(-315.95f, -1524.39f, 27.56f);
            MarkerLocation = new Vector3(-355.04f, -1513.41f, 27.72f);
            VehicleSpawnHeading = 269.76f;
            StartString = "Drive to each stop to get paid";
            JobBlipName = "Garbage Hauler";
            DeliveryLocations = new List<Vector3>
            {
                new Vector3(467.69f, -950.4f, 27.77f),
                new Vector3(87.84f, -1282.88f, 29.26f),
                new Vector3(-0.97f, -1357.84f, 29.25f),
                new Vector3(-1195.74f, -1346.02f, 4.82f),
                new Vector3(123.05f, 6653.1f, 31.65f),
                new Vector3(-683.39f, 5782.65f, 17.33f),
                new Vector3(-2177.44f, 4263.24f, 48.96f),
                new Vector3(-2955.59f, 58.6f, 11.61f),
                new Vector3(492.79f, -1746.43f, 28.52f),
                new Vector3(312.95f, -2017.84f, 20.42f),
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
            Client.TriggerServerEvent("Job.RequestPayForJob", JobType.Delivery.ToString(), "Garbage");
            GetNextDeliveryLocation();
        }

        protected override void CreateBlip()
        {
            BlipHandler.AddBlip("Garbage Job", MarkerLocation, new BlipOptions
            {
                Sprite = BlipSprite.GarbageTruck
            });
        }
    }
}
