using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using MenuFramework;
using Roleplay.Client.Interfaces;
using Roleplay.Client.UI;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.Bases
{
    public abstract class DeliveryJob : JobClass, IJob
    {
        protected List<Vector3> DeliveryLocations = new List<Vector3>();
        protected List<Vector3> CurrentDeliveryLocations = new List<Vector3>();
        protected Vector3 VehicleSpawnLocation = Vector3.Zero;
        protected Vector3 MarkerLocation = Vector3.Zero;
        protected Vector3 CurrentDeliveryLocation;
        protected Vector3 PreviousDeliveryLocation;
        protected float VehicleSpawnHeading = 0.0f;
        protected VehicleHash DeliveryVehicleHash = VehicleHash.Boxville2;
        protected MenuItemStandard ReturnVehicleItem;
        protected Blip CurrentDestinationBlip;
        protected string JobBlipName = "Delivery point";
        protected string StartString = "Deliver to each location to get paid";
        public Marker JobMarker;

        protected DeliveryJob()
        {
            ReturnVehicleItem = new MenuItemStandard
            {
                Title = "Return Job Vehicle",
                OnActivate = async item =>
                {
                    if (Game.PlayerPed.Position.DistanceToSquared(VehicleSpawnLocation) < 400.0f && JobVehicle != null && (JobVehicle.Position.DistanceToSquared(VehicleSpawnLocation) < 400.0f || JobVehicle.IsDead))
                    {
                        if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle == JobVehicle)
                            Game.PlayerPed.Task.LeaveVehicle(JobVehicle, true);

                        Log.ToChat("[Job]", "Returning job vehicle", ConstantColours.Job);
                        await BaseScript.Delay(3000);
                        RemoveJobVehicle();
                        EndJob();
                        Client.Get<InteractionUI>().RemoveInteractionMenuItem(ReturnVehicleItem);
                    }
                }
            };

            Client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
        }

        public override async void StartJob()
        {
            base.StartJob();
            if (JobVehicle == null)
                JobVehicle = await spawnJobVehicle();

            if (!JobHandler.OnDutyAsJob(JobType.Delivery))
            {
                JobHandler.SetPlayerJob(JobType.Delivery);
                JobHandler.SetDutyState(true);
            }

            if(!Client.Get<InteractionUI>().ContainsInteractionMenuItem(ReturnVehicleItem))
                Client.Get<InteractionUI>().RegisterInteractionMenuItem(ReturnVehicleItem, () => JobHandler.OnDutyAsJob(JobType.Delivery) && Game.PlayerPed.Position.DistanceToSquared(VehicleSpawnLocation) < 400.0f, 500);

            CurrentDeliveryLocations = GetDeliveryLocations();
            CurrentDeliveryLocation = CurrentDeliveryLocations[0];
            loadDeliveryBlip();
            Log.ToChat("[Job]", StartString, ConstantColours.Job);
            Client.RegisterTickHandler(JobTick);
        }

        protected virtual void OnInteraction()
        {
            if (Game.PlayerPed.Position.DistanceToSquared(MarkerLocation) < Math.Pow(3, 2))
            {
                if (!JobHandler.IsOnDuty)
                {
                    StartJob();
                }
                else
                {
                    Log.ToChat("[Job]", "You must quit your current job before starting another one", ConstantColours.Job);
                }
            }
        }

        protected virtual async Task<Vehicle> spawnJobVehicle()
        {
            /*var veh = await World.CreateVehicle(DeliveryVehicleHash, VehicleSpawnLocation, VehicleSpawnHeading);
            veh.PlaceOnGround();
            veh.LockStatus = VehicleLockStatus.Unlocked;
            veh.IsPersistent = true;
            veh.Mods.Livery = 1;
            Game.PlayerPed.Task.WarpIntoVehicle(veh, VehicleSeat.Driver);
            Roleplay.Client.Client.Instance.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(veh));

            return veh;*/
            return await CreateJobVehicle(DeliveryVehicleHash, VehicleSpawnLocation, VehicleSpawnHeading);
        }

        protected virtual List<Vector3> GetDeliveryLocations()
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            var currentLocations = new List<Vector3>();
            var nextLocation = DeliveryLocations[rand.Next(0, DeliveryLocations.Count - 1)];
            while (currentLocations.Count != 5)
            {
                if (!currentLocations.Contains(nextLocation))
                    currentLocations.Add(nextLocation);

                nextLocation = DeliveryLocations[rand.Next(0, DeliveryLocations.Count - 1)];
            }
            currentLocations.Add(VehicleSpawnLocation);
            PreviousDeliveryLocation = VehicleSpawnLocation;

            return currentLocations;
        }

        public override void GiveJobPayment()
        {
            if (CurrentDeliveryLocation != VehicleSpawnLocation)
            {
                Client.TriggerServerEvent("Job.RequestDeliveryPayment", PreviousDeliveryLocation.ToArray(), CurrentDeliveryLocation.ToArray(), "delivery job");
                GetNextDeliveryLocation();
            }
            else
            {
                // do final stuff
                if (JobVehicle != null && !JobVehicle.IsDead)
                {
                    StartJob();
                }
                else
                {
                    EndJob();
                }
            }
        }

        public virtual void GetNextDeliveryLocation()
        {
            var currentIndex = CurrentDeliveryLocations.FindIndex(o => o == CurrentDeliveryLocation);
            if (currentIndex == CurrentDeliveryLocations.Count - 1)
            {
                return;
            }

            PreviousDeliveryLocation = CurrentDeliveryLocation;
            CurrentDeliveryLocation = CurrentDeliveryLocations[currentIndex + 1];
            loadDeliveryBlip();
        }

        public override void EndJob()
        {
            base.EndJob();
            CurrentDestinationBlip?.Delete();
            CurrentDestinationBlip = null;
            JobHandler.SetDutyState(false);
            JobHandler.SetPlayerJob(JobType.Civillian);
            Client.DeregisterTickHandler(JobTick);
        }

        protected virtual void loadDeliveryBlip()
        {
            CurrentDestinationBlip?.Delete();
            CurrentDestinationBlip = World.CreateBlip(CurrentDeliveryLocation);
            CurrentDestinationBlip.IsShortRange = true;
            CurrentDestinationBlip.Name = JobBlipName;
            CurrentDestinationBlip.ShowRoute = true;
        }

        protected void DrawMarker()
        {
            JobMarker = new Marker(MarkerLocation, MarkerType.HorizontalCircleFat, Color.FromArgb(140, ConstantColours.Yellow), 3.0f);
            JobMarker.Position = new Vector3(JobMarker.Position.X, JobMarker.Position.Y, JobMarker.Position.Z + 0.01f);
            MarkerHandler.AddMarker(JobMarker);
        }

        protected virtual async Task AttemptJobPayment()
        {
            if (!Cache.PlayerPed.IsInVehicle())
            {
                CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame("You must be in your vehicle to do this");
                return;
            }

            if (Cache.PlayerPed.CurrentVehicle.Speed < 0.2f && Cache.PlayerPed.CurrentVehicle == JobVehicle)
            {
                GiveJobPayment();
                await BaseScript.Delay(1000);
            }
        }

        protected int MarkerDirection = 0;
        public override async Task JobTick()
        {
            MarkerDirection++;
            if (JobVehicle != null)
            {
                /*if (JobVehicle.IsDead)
                {
                    Log.ToChat("[Job]", "Your job vehicle was destroyed", ConstantColours.Job);
                    EndJob();
                    JobVehicle = null;
                    return;
                }*/

                if (JobVehicle.Position.DistanceToSquared(CurrentDeliveryLocation) >= 80.0f)
                {
                    World.DrawMarker(MarkerType.ChevronUpx1, CurrentDeliveryLocation, Vector3.Zero, new Vector3(0, 180, MarkerDirection), new Vector3(1, 1, 1), ConstantColours.Yellow, true);
                }
                else
                {
                    await AttemptJobPayment();
                }
            }
        }

        protected abstract void CreateBlip();
    }
}
