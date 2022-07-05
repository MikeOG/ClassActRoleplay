using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Jobs.Bases;
using Roleplay.Client.Models;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Jobs.Civillian.Delivery
{
    class Trucking : DeliveryJob
    {
        private Vehicle truckTrailer;
        private VehicleHash trailerHash = VehicleHash.Trailers2;

        public Trucking()
        {
            DeliveryVehicleHash = VehicleHash.Phantom;
            VehicleSpawnLocation = new Vector3(-244.213f, -2463.574f, 6.001f);
            MarkerLocation = new Vector3(-270.689f, -2474.58f, 6.295f);
            VehicleSpawnHeading = 80.0f;
            DeliveryLocations = new List<Vector3>
            {
                new Vector3(154.127f, -1542.086f, 28.143f),
                new Vector3(-333.556f, -1495.938f, 29.647f),
                new Vector3(-736.127f, -1413.966f, 4.001f),
                new Vector3(-2065.964f, -304.012f, 12.147f),
                new Vector3(-2534.964f, 2343.139f, 32.06f),
                new Vector3(134.707f, 6579.54f, 31.0599f),
                //Grapeseed store
                new Vector3(1701.39f, 4804.46f, 41.82f),
                //Paleto Motel
                new Vector3(-675.79f, 5786.88f, 17.33f),
                //Chieck factory
                new Vector3(182.24f, 6402.57f, 31.3f),
                //Paleto Garage
                new Vector3(105.06f, 6637.66f, 31.41f),
                //Pops Diner
                new Vector3(1569.08f, 6460.5f, 24.43f),
                //Airport
                new Vector3(-764.43f, -2604.25f, 13.83f),
                //Docks
                new Vector3(-456.84f, -2796.26f, 6f),
            };
            DrawMarker();
            CreateBlip();
            StartString = "Deliver the trailer to the specifed location";
            JobBlipName = "Trailer dropoff";
        }

        public override void GiveJobPayment()
        {
            Client.TriggerServerEvent("Job.RequestDeliveryPayment", VehicleSpawnLocation.ToArray(), CurrentDeliveryLocation.ToArray(), "trucking job");
            CurrentDestinationBlip?.Delete();
            CurrentDestinationBlip = null;
            truckTrailer?.Delete();
            truckTrailer = null;
        }

        protected override async void OnInteraction()
        {
            if (Game.PlayerPed.Position.DistanceToSquared(MarkerLocation) < Math.Pow(3, 2))
            {
                if (!JobHandler.IsOnDuty)
                {
                    StartJob();
                }
                else if (JobHandler.OnDutyAsJob(JobType.Delivery) && truckTrailer == null)
                {
                    truckTrailer = await spawnTruckTrailer();
                    Client.DeregisterTickHandler(JobTick);
                    StartJob();        
                }
                else
                {
                    Log.ToChat("[Job]", "You must quit your current job before starting another one", ConstantColours.Job);
                }
            }
        }

        protected override async Task<Vehicle> spawnJobVehicle()
        {
            var jobVeh = await base.spawnJobVehicle();
            
            truckTrailer = await spawnTruckTrailer();
            AttachVehicleToTrailer(jobVeh.Handle, truckTrailer.Handle, 10.0f);

            return jobVeh;
        }

        protected override List<Vector3> GetDeliveryLocations()
        {
            return new List<Vector3>
            {
                DeliveryLocations[new Random().Next(0, DeliveryLocations.Count - 1)]
            };
        }

        protected override async Task AttemptJobPayment()
        {
            if (!Cache.PlayerPed.IsInVehicle())
            {
                CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame("You must be in your vehicle to do this");
                return;
            }

            var playerVeh = Cache.PlayerPed.CurrentVehicle;
            int attachedTrailerHandle = -1;
            GetVehicleTrailerVehicle(playerVeh.Handle, ref attachedTrailerHandle);
            if (playerVeh.Speed < 0.2f && playerVeh == JobVehicle && truckTrailer != null && attachedTrailerHandle == truckTrailer.Handle)
            {
                CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame("Press ~INPUT_VEH_HEADLIGHT~ to drop off your trailer");
                if (Input.IsControlJustPressed(Control.VehicleHeadlight))
                {
                    DetachVehicleFromTrailer(playerVeh.Handle);
                    GiveJobPayment();
                    await BaseScript.Delay(1000);
                }
            }
        }

        protected override void CreateBlip()
        {
            BlipHandler.AddBlip("Trucking depot", MarkerLocation, new BlipOptions
            {
                Sprite = BlipSprite.GTAOMission
            });
        }

        private async Task<Vehicle> spawnTruckTrailer()
        {
            var vehModel = new Model(trailerHash);
            while (!vehModel.IsLoaded)
                await vehModel.Request(0);

            var veh = await World.CreateVehicle(vehModel, VehicleSpawnLocation, VehicleSpawnHeading);

            vehModel.MarkAsNoLongerNeeded();

            return veh;
        }
    }
}

