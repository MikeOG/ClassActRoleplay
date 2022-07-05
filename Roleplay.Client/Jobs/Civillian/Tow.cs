using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player;
using Roleplay.Client.UI;
using Roleplay.Client.Vehicles;
using Roleplay.Client.Interfaces;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client.Jobs.Civillian
{
    internal class Tow : JobClass, IJob
    {
        private CitizenFX.Core.Vehicle currentTowTruck;
        private CitizenFX.Core.Vehicle towedVehicle;
        private CitizenFX.Core.Vehicle currentJobVehicle;
        private Vector3 towDropoffPoint = new Vector3(409.28f, -1644.19f, 29.29f);
        private Vector3 towMarkerLocation = new Vector3(409.68f, -1625.55f, 28.31f);
        private int markerZRotation = 0;

        private MenuItem returnVehicleItem;

        public Tow()
        {
            returnVehicleItem = new MenuItemStandard
            {
                Title = "Return Job Vehicle",
                OnActivate = async item =>
                {
                    if (Game.PlayerPed.Position.DistanceToSquared(towDropoffPoint) < 200.0f && currentTowTruck != null && (currentTowTruck.Position.DistanceToSquared(towDropoffPoint) < 200.0f || currentTowTruck.IsDead))
                    {
                        if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle == currentTowTruck)
                            Game.PlayerPed.Task.LeaveVehicle(currentTowTruck, true);

                        Log.ToChat("[Job]", "Returning job vehicle", ConstantColours.Job);

                        await BaseScript.Delay(3000);
                        EndJob();
                        Client.Get<InteractionUI>().RemoveInteractionMenuItem(returnVehicleItem);
                    }
                }
            };
            Client.RegisterTickHandler(VehicleGetTick);
            Client.RegisterTickHandler(JobTick);
            Client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
            Client.RegisterEventHandler("Job.GetTowRequest", new Action(OnTowRequest));
            CommandRegister.RegisterCommand("tow", handleTowCommand);
            CommandRegister.RegisterCommand("canceltowjob", handleTowJobCancel);

            BlipHandler.AddBlip("Tow yard", towMarkerLocation, new BlipOptions
            {
                Sprite = BlipSprite.TowTruck
            });

            MarkerHandler.AddMarker(towMarkerLocation, new MarkerOptions
            {
                ScaleFloat = 4.0f
            });
        }

        private void handleTowCommand(Command cmd)
        {
            if (!JobHandler.OnDutyAsJob(JobType.Tow) || Cache.PlayerPed.IsInVehicle()) return;

            var frontVeh = GTAHelpers.GetClosestVehicle(12.0f, veh => veh != currentTowTruck);

            if (frontVeh == null || currentTowTruck == null) return;

            if (frontVeh.ClassType == VehicleClass.Helicopters || frontVeh.ClassType == VehicleClass.Military || frontVeh.ClassType == VehicleClass.Trains || frontVeh.Occupants.Length > 0) return;

            if (towedVehicle != null && !Game.PlayerPed.IsInVehicle() && Game.PlayerPed.Position.DistanceToSquared(towedVehicle.Position) < 16.0f)
            {
                towedVehicle.AttachTo(currentTowTruck, new Vector3(-0.5f, -12, 0));
                towedVehicle.Detach();
                towedVehicle = null;
                return;
            }

            if (frontVeh.Model.Hash == (int)VehicleHash.Flatbed) return;

            if (frontVeh != currentTowTruck)
            {
                towedVehicle = frontVeh;
                towedVehicle.AttachTo(currentTowTruck, new Vector3(0, -2.5f, 0.85f));
                towedVehicle.AttachedBlips.ToList().ForEach(o => o.Delete());
            }
        }

        private void handleTowJobCancel(Command cmd)
        {
            if (!JobHandler.OnDutyAsJob(JobType.Tow)) return;
            if (currentJobVehicle != null)
            {
                currentJobVehicle.AttachedBlips.ToList().ForEach(o => o.Delete());
                currentJobVehicle = null;
                Log.ToChat("[Tow]", "You cancelled your current tow job", ConstantColours.TalkMarker);
            }
        }

        private async Task VehicleGetTick()
        {
            if (Client.LocalSession == null) return;

            if (JobHandler.OnDutyAsJob(JobType.Tow))
            {
                await BaseScript.Delay(30000);
                getRandomVehicle();
            }
        }

        public async Task JobTick()
        {
            if (currentJobVehicle != null)
            {
                if (!currentJobVehicle.IsAttached())
                    World.DrawMarker(MarkerType.ChevronUpx1,
                        new Vector3(currentJobVehicle.Position.X, currentJobVehicle.Position.Y,
                            currentJobVehicle.Position.Z + 2), new Vector3(0, 0, 0),
                        new Vector3(180f, 0, markerZRotation), new Vector3(1f, 1f, 1f),
                        System.Drawing.Color.FromArgb(255, 255, 255, 0), true);

                if (towDropoffPoint.DistanceToSquared(Game.PlayerPed.Position) < 120 && Game.PlayerPed.LastVehicle == currentTowTruck && !currentJobVehicle.IsAttached() && currentJobVehicle.Position.DistanceToSquared(Game.PlayerPed.Position) < 120)
                {
                    GiveJobPayment();
                    Log.Info("You would get payed for someting");
                    await BaseScript.Delay(5000);
                }

                World.DrawMarker(MarkerType.CheckeredFlagCircle, towDropoffPoint, new Vector3(0, 0, 0), new Vector3(0, 0, markerZRotation), new Vector3(1f, 1f, 1f), System.Drawing.Color.FromArgb(255, 255, 255, 0), true);

                if (JobVehicle.IsDead)
                {
                    Log.ToChat("[Job]", "Your job vehicle was destroyed", ConstantColours.Job);
                    EndJob();
                    JobVehicle = null;
                    return;
                }
            }

            markerZRotation++;
            if (markerZRotation >= 360)
                markerZRotation = 0;
        }

        private void getRandomVehicle()
        {
            if (currentJobVehicle != null || currentTowTruck == null) return;
            currentJobVehicle = new VehicleList().Select(o =>
                new CitizenFX.Core.Vehicle(o)).FirstOrDefault(o => o.Position.DistanceToSquared(Game.PlayerPed.Position) < 50000.0f
                && o.Occupants.Length == 0
                && (!o.HasDecor("Vehicle.ID") || o.HasDecor("Vehicle.ID") && o.GetDecor<int>("Vehicle.ID") >= 1000000)
                && o.Position.DistanceToSquared(towDropoffPoint) > 12000.0f
                && !(o.ClassType == VehicleClass.Helicopters || o.ClassType == VehicleClass.Utility || o.ClassType == VehicleClass.Commercial ||  o.ClassType == VehicleClass.Emergency || o.ClassType == VehicleClass.Military || o.ClassType == VehicleClass.Trains));

            if (currentJobVehicle != null)
            {
                currentJobVehicle.AttachBlip();
                currentJobVehicle.AttachedBlip.IsShortRange = true;
                Log.ToChat("[Tow]", "You have a new tow job", ConstantColours.TalkMarker);
            }
        }

        public void GiveJobPayment()
        {
            currentJobVehicle = null;
            Client.Instance.TriggerServerEvent("Job.RequestPayForJob", "Tow", "Tow");
        }

        public async void StartJob()
        {
            currentTowTruck = await CreateJobVehicle(VehicleHash.Flatbed, towDropoffPoint, 0.0f);
            JobHandler.SetPlayerJob(JobType.Tow);
            JobHandler.SetDutyState(true);
            Client.Get<InteractionUI>().RegisterInteractionMenuItem(returnVehicleItem, inRangeOfTowPickup, 500);
        }

        public void EndJob()
        {
            currentTowTruck?.Delete();
            currentTowTruck = null;
            currentJobVehicle?.AttachedBlips.ToList().ForEach(o => o.Delete());
            currentJobVehicle = null;
            JobHandler.SetDutyState(false);
            JobHandler.SetPlayerJob(JobType.Civillian);
            RemoveJobVehicle();
        }

        private async void OnInteraction()
        {
            if (Game.PlayerPed.Position.DistanceToSquared(towMarkerLocation) < Math.Pow(3, 2))
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

        private bool inRangeOfTowPickup()
        {
            return Game.PlayerPed.Position.DistanceToSquared(towDropoffPoint) < 200.0f;
        }

        private void OnTowRequest()
        {
            var closeVeh = GTAHelpers.GetClosestVehicle(6.0f);

            if (closeVeh != null)
            {
                Client.TriggerServerEvent("Job.SendTowVehicle", $"{closeVeh.Mods.PrimaryColor.ToString().AddSpacesToCamelCase()} {closeVeh.LocalizedName} with plate {closeVeh.Mods.LicensePlate}");
            }
        }
    }
}



