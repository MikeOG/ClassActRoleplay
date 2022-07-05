using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.UI;
using Roleplay.Client.Helpers;
using Roleplay.Client.Interfaces;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client.Jobs.Civillian
{
    internal class Taxi : JobClass, IJob
    {
        private Ped targetPed = null;
        private bool inRangeOfPed = false;
        // For making spinning markers
        private int markerZRotation = 0;
        private readonly List<Vector3> taxiDropoffLocations = new List<Vector3>()
        {
            new Vector3(-1466.73f, -923.92f, 10.03f),
            new Vector3(-1450.51f, -951.58f, 6.99f),
            new Vector3(-1403.05f, -939.94f, 10.38f),
            new Vector3(-1329.91f, -1061.49f, 6.67f),
            new Vector3(-1332.36f, -1091.38f, 6.52f),
            new Vector3(-1281.85f, -1154.41f, 5.35f),
            new Vector3(-1237.33f, -1171.07f, 7.10f),
            new Vector3(-1177.06f, -1275.50f, 5.27f),
            new Vector3(-1088.96f, -1362.13f, 4.73f),
            new Vector3(-1088.79f, -1476.32f, 4.58f),
            new Vector3(-949.60f, -1536.29f, 4.60f),
            new Vector3(-1055.66f, -1401.49f, 4.96f),
            new Vector3(-957.89f, -1191.46f, 4.01f),
            new Vector3(-987.77f, -1112.17f, 1.65f),
            new Vector3(-1032.54f, -1083.49f, 2.20f),
            new Vector3(-1073.75f, -907.82f, 3.47f),
            new Vector3(-1024.67f, -788.04f, 16.90f),
            new Vector3(-1117.71f, -807.43f, 16.58f),
            new Vector3(-1178.68f, -878.52f, 13.48f),
            new Vector3(-1181.84f, -834.14f, 13.86f),
            new Vector3(-1306.05f, -854.22f, 14.99f),
            new Vector3(-1404.51f, -731.93f, 23.18f),
            new Vector3(-1354.57f, -680.00f, 24.95f),
            new Vector3(-1414.18f, -593.46f, 29.93f),
            new Vector3(-1408.90f, -501.04f, 32.13f),
            new Vector3(-1450.63f, -487.27f, 33.83f),
            new Vector3(-1520.34f, -441.57f, 34.97f),
            new Vector3(-1604.30f, -466.63f, 36.72f),
            new Vector3(-1664.41f, -459.69f, 38.40f),
            new Vector3(-1738.29f, -461.07f, 40.88f),
            new Vector3(-1874.97f, -382.48f, 47.50f),
            new Vector3(-1945.93f, -335.65f, 45.78f),
            new Vector3(-1925.02f, -222.18f, 35.58f),
            new Vector3(-1792.14f, -343.65f, 44.00f),
            new Vector3(-1748.03f, -415.62f, 43.23f),
            new Vector3(-1640.19f, -348.89f, 49.62f),
            new Vector3(-1659.13f, -320.86f, 49.59f),
            new Vector3(-1600.08f, -263.57f, 52.82f),
            new Vector3(-1622.81f, -235.55f, 53.59f),
            new Vector3(-1545.99f, -214.98f, 54.12f),
            new Vector3(-1518.78f, -205.23f, 53.06f),
            new Vector3(-1602.85f, -102.44f, 57.41f),
            new Vector3(-1440.13f, -100.67f, 50.50f),
            new Vector3(-34.87f, -732.34f, 43.72f),
            new Vector3(-49.08f, -781.79f, 43.79f),
            new Vector3(2.92f, -970.75f, 29.01f),
            new Vector3(40.25f, -993.83f, 28.90f),
            new Vector3(65.56f, -1054.87f, 28.91f),
            new Vector3(119.84f, -1060.93f, 28.72f),
            new Vector3(33.17f, -1108.85f, 28.82f),
            new Vector3(-66.29f, -1106.99f, 25.63f),
            new Vector3(-144.68f, -823.49f, 30.79f),
            new Vector3(-168.77f, -810.53f, 30.90f),
            new Vector3(-262.92f, -805.34f, 31.56f),
            new Vector3(-354.46f, -829.33f, 31.06f),
            new Vector3(-361.15f, -862.77f, 31.02f),
            new Vector3(-276.99f, -1064.86f, 25.32f),
            new Vector3(-222.73f, -1154.91f, 22.58f),
            new Vector3(46.99f, -1304.11f, 28.76f),
            new Vector3(111.62f, -1323.36f, 28.91f),
            new Vector3(27.43f, -1356.37f, 28.82f),
            new Vector3(-21.18f, -1355.31f, 28.69f),
            new Vector3(89.64f, -1399.63f, 28.70f),
            new Vector3(302.63f, -1384.15f, 31.14f),
            new Vector3(267.41f, -1284.97f, 28.74f),
            new Vector3(241.02f, -1162.29f, 28.81f),
            new Vector3(428.23f, -960.70f, 28.74f),
            new Vector3(397.95f, -905.74f, 28.95f),
            new Vector3(205.07f, -846.94f, 30.03f),
            new Vector3(209.37f, -304.69f, 44.74f),
            new Vector3(191.33f, -191.59f, 53.51f),
            new Vector3(87.58f, -206.23f, 54.02f),
            new Vector3(45.29f, -213.29f, 51.90f),
            new Vector3(-56.22f, -221.92f, 44.98f),
            new Vector3(49.33f, -1566.27f, 28.89f),
            new Vector3(3.80f, -1546.45f, 28.82f),
            new Vector3(-63.33f, -1556.75f, 30.62f),
            new Vector3(-83.80f, -1496.08f, 32.47f),
            new Vector3(-24.16f, -1456.45f, 30.21f),
            new Vector3(-9.46f, -1642.60f, 28.70f),
            new Vector3(-64.94f, -1750.71f, 28.91f),
            new Vector3(105.28f, -1823.58f, 25.87f),
            new Vector3(174.75f, -1737.12f, 28.84f),
            new Vector3(126.96f, -1714.39f, 28.61f),
            new Vector3(150.53f, -1635.91f, 28.82f),
            new Vector3(270.13f, -1557.68f, 28.56f),
            new Vector3(412.80f, -1612.12f, 28.83f),
            new Vector3(462.82f, -1683.42f, 28.82f),
            new Vector3(555.55f, -1727.42f, 28.78f),
            new Vector3(454.32f, -284.79f, 48.38f),
            new Vector3(1712.37f, 3595.95f, 34.93f),
            new Vector3(1983.41f, 3762.79f, 31.71f),
            new Vector3(1967.65f, 3845.87f, 31.53f),
            new Vector3(1713.54f, 3770.17f, 33.88f),
            new Vector3(-82.11f, 6431.43f, 31.02f),
            new Vector3(-119.98f, 6455.52f, 30.95f),
            new Vector3(-179.52f, 6447.05f, 30.65f),
            new Vector3(-275.02f, 6371.76f, 30.28f),
            new Vector3(-214.13f, 6196.36f, 31.02f),
            new Vector3(141.10f, 6635.23f, 31.16f),
        };
        private bool inTaxiJob;
        private static bool lookingForTaxiJob;
        private MenuItemCheckbox taxiDutyItem = new MenuItemCheckbox
        {
            Title = "Looking For Taxi Job",
            state = true,
            OnActivate = (state, item) =>
            {
                lookingForTaxiJob = state;
            }
        };
        private Blip taxiDestination;
        private Vector3 markerLocation = new Vector3(905.152f, -174.219f, 73.2781f);
        private Vector3 vehicleSpawnLocation = new Vector3(902.631f, -185.892f, 73.0379f);
        private MenuItem returnVehicleItem;

        public Taxi()
        {
            returnVehicleItem = new MenuItemStandard
            {
                Title = "Return Job Vehicle",
                OnActivate = async item =>
                {
                    if (Game.PlayerPed.Position.DistanceToSquared(vehicleSpawnLocation) < 200.0f && JobVehicle != null && (JobVehicle.Position.DistanceToSquared(vehicleSpawnLocation) < 200.0f || JobVehicle.IsDead))
                    {
                        if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle == JobVehicle)
                            Game.PlayerPed.Task.LeaveVehicle(JobVehicle, true);

                        Log.ToChat("[Job]", "Returning job vehicle", ConstantColours.Job);

                        await BaseScript.Delay(3000);
                        EndJob();
                        Client.Get<InteractionUI>().RemoveInteractionMenuItem(returnVehicleItem);
                    }
                }
            };
            CommandRegister.RegisterCommand("canceltaxijob", new Action<Command>(cmd =>
            {
                if (inTaxiJob)
                {
                    Log.ToChat("[Job]", "You cancelled your current taxi job", ConstantColours.Job);
                    endTaxiJob();
                }
                else
                {
                    Log.ToChat("[Job]", "You must have a fare to cancel your job", ConstantColours.Job);
                }
            }));
            Client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
            LoadBlips();
            //Client.Instance.RegisterTickHandler(JobTick);
            //Client.Instance.RegisterTickHandler(taxiSearchTick);
        }

        public async void StartJob()
        {
            await CreateJobVehicle(VehicleHash.Taxi, vehicleSpawnLocation, 0.0f);
            JobHandler.SetPlayerJob(JobType.Taxi);
            JobHandler.SetDutyState(true);
            Client.RegisterTickHandler(JobTick);
            Client.RegisterTickHandler(taxiSearchTick);
            lookingForTaxiJob = true;
            inTaxiJob = false;
            Client.Get<InteractionUI>().RegisterInteractionMenuItem(taxiDutyItem, () => JobHandler.OnDutyAsJob(JobType.Taxi) && !inTaxiJob, 250);
            Client.Get<InteractionUI>().RegisterInteractionMenuItem(returnVehicleItem, () => JobHandler.OnDutyAsJob(JobType.Taxi) && Game.PlayerPed.Position.DistanceToSquared(vehicleSpawnLocation) < 200.0f, 250);
        }

        public void EndJob()
        {
            JobHandler.SetPlayerJob(JobType.Civillian);
            JobHandler.SetDutyState(false);
            lookingForTaxiJob = false;
            inTaxiJob = false;
            targetPed = null;
            Client.DeregisterTickHandler(JobTick);
            Client.DeregisterTickHandler(taxiSearchTick);
            Client.Get<InteractionUI>().RemoveInteractionMenuItem(taxiDutyItem);
            RemoveJobVehicle();
        }

        public void GiveJobPayment(Vector3 start, Vector3 end)
        {
            Client.TriggerServerEvent("Job.RequestDeliveryPayment", start.ToArray().ToList(), end.ToArray().ToList(), "taxi job");
        }

        public void GiveJobPayment(){ }

        public async Task JobTick()
        {
            if (targetPed != null && inTaxiJob)
            {
                bool drawMarker = true;
                Vector3 targetCoords = targetPed.Position;
                targetCoords.Z += 1.5f;
                if (targetPed.Position.DistanceToSquared(Game.PlayerPed.Position) <= 75.0f)
                {
                    if (Game.PlayerPed.CurrentVehicle.Speed < 0.5f)
                    {
                        drawMarker = false;
                        inRangeOfPed = true;
                    }
                    else if(targetPed.IsInVehicle())
                    {
                        drawMarker = false;
                    }
                }
                else
                {
                    inRangeOfPed = false;
                }

                if (drawMarker)
                    World.DrawMarker(MarkerType.ChevronUpx1, targetCoords, new Vector3(0, 0, 0), new Vector3(180f, 0, markerZRotation), new Vector3(1f, 1f, 1f), System.Drawing.Color.FromArgb(255, 255, 255, 0), true);
            }

            if (JobVehicle.IsDead)
            {
                Log.ToChat("[Job]", "Your job vehicle was destroyed", ConstantColours.Job);
                EndJob();
                JobVehicle = null;
                return;
            }

            markerZRotation++;
            if (markerZRotation >= 360)
                markerZRotation = 0;
        }

        private async Task taxiSearchTick()
        {
            if(lookingForTaxiJob && !inTaxiJob)
            {
                await BaseScript.Delay(30000);
                if(lookingForTaxiJob && !inTaxiJob)
                    lookForTaxiJob();
            }
        }

        private async void lookForTaxiJob()
        {
            var targetPeds = new PedList().Select(o => new Ped(o))
                    .Where(o => o.Position.DistanceToSquared(Game.PlayerPed.Position) < 2500.0f && o != Game.PlayerPed && !o.IsInVehicle() && !o.IsFleeing && !o.IsDead && !isServerPlayer(o) && o.IsHuman)
                    .OrderBy(o => o.Position.DistanceToSquared(Game.PlayerPed.Position));

            if (targetPeds.Any() && Game.PlayerPed.IsInVehicle())
            {
                try
                {
                    inTaxiJob = true;
                    Log.ToChat("[Job]", "You have a new taxi call", ConstantColours.Job);
                    targetPed = targetPeds.ToList()[new Random().Next(0, targetPeds.ToList().Count)];
                    targetPed.IsPersistent = true;
                    targetPed.BlockPermanentEvents = true;
                    Blip pedBlip = targetPed.AttachBlip();
                    pedBlip.ShowRoute = true;
                    pedBlip.Color = BlipColor.Green;
                    targetPed.Task.LookAt(Game.PlayerPed);
                    targetPed.Task.TurnTo(Game.PlayerPed);
                    inRangeOfPed = false;
                    while (!inRangeOfPed)
                        await BaseScript.Delay(0);
                    targetPed.Task.EnterVehicle(Game.PlayerPed.CurrentVehicle, VehicleSeat.LeftRear);
                    while (!targetPed.IsInVehicle())
                    {
                        targetPed.Task.EnterVehicle(Game.PlayerPed.CurrentVehicle, VehicleSeat.LeftRear);
                        await BaseScript.Delay(2500);
                    }
                    targetPed.AttachedBlip.Delete();
                    startTaxiRoute();
                } catch(Exception e) { }
            }
        }

        private async void startTaxiRoute()
        {
            Vector3 dropOffPoint = taxiDropoffLocations[new Random().Next(0, taxiDropoffLocations.Count - 1)];
            Vector3 startingCoords = Game.PlayerPed.Position;
            taxiDestination = World.CreateBlip(dropOffPoint);
            taxiDestination.Color = BlipColor.Green;
            taxiDestination.IsShortRange = true;
            taxiDestination.ShowRoute = true;
            bool runTaxiThread = true;
            DateTime? leaveTime = null;
            while(runTaxiThread)
            {
                try
                {
                    if (Game.PlayerPed.Position.DistanceToSquared(dropOffPoint) >= 20.0f)
                    {
                        World.DrawMarker(MarkerType.CheckeredFlagCircle, dropOffPoint, new Vector3(0, 0, 0), new Vector3(0, 0, markerZRotation), new Vector3(1f, 1f, 1f), System.Drawing.Color.FromArgb(255, 255, 255, 0), true);
                    }
                    else
                    {
                        if (Game.PlayerPed.CurrentVehicle.Speed < 0.5f)
                        {
                            /*int dropoffReward = (int)Math.Round(Math.Sqrt(dropOffPoint.DistanceToSquared(startingCoords)) / 20, 0);
                            Log.SendNotify($"You got ${dropoffReward} from this job");
                            TriggerServerEvent("updateMoneyByCharId", 1, dropoffReward, false, "taxi job");*/
                            GiveJobPayment(dropOffPoint, startingCoords);
                            endTaxiJob();
                            return;
                        }
                    }

                    if (targetPed.IsDead)
                    {
                        Log.ToChat("[Job]", "Your fare died", ConstantColours.Job);
                        targetPed.Delete();
                        endTaxiJob();
                        return;
                    }

                    if (!Game.PlayerPed.IsInVehicle())
                    {
                        if (leaveTime == null)
                        {
                            leaveTime = DateTime.Now;
                            Log.ToChat("[Job]", "Please return to your vehicle promptly otherwise your fare will leave without paying", ConstantColours.Job);
                            await BaseScript.Delay(0);
                        }
                        else
                        {
                            if ((DateTime.Now - leaveTime).Value.TotalMinutes >= 2)
                            {
                                Log.ToChat("[Job]", "Your client left without paying because you were away from the vehicle for too long", ConstantColours.Job);
                                endTaxiJob();
                                return;
                            }
                        }
                    }
                    else
                    {
                        leaveTime = null;
                    }

                    await BaseScript.Delay(0);
                } catch(Exception e) { return; }
            }
        }

        private async void endTaxiJob()
        {
            inRangeOfPed = true; // This is to error out the waiting thread before a npc is picked up (just incase)
            if (taxiDestination != null)
                taxiDestination.Delete();
            taxiDestination = null;
            targetPed.BlockPermanentEvents = false;
            targetPed.AttachedBlips.ToList().ForEach(o => { o.Delete(); });
            if (targetPed.IsInVehicle())
                targetPed.Task.LeaveVehicle(targetPed.CurrentVehicle, true);
            targetPed.Task.GoTo(World.GetNextPositionOnSidewalk(targetPed.Position));
            targetPed = null;
            
            
            inTaxiJob = false;
        }

        private bool isServerPlayer(Ped target)
        {
            bool isServerPlayer = false;
            Client.PlayerList.ToList().ForEach(o =>
            {
                if (o.Character == target)
                    isServerPlayer = true;
            });
            return isServerPlayer;
        }

        private async void OnInteraction()
        {
            if (Game.PlayerPed.Position.DistanceToSquared(markerLocation) < Math.Pow(3, 2))
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

        private void LoadBlips()
        {
            BlipHandler.AddBlip("Taxi Depot", markerLocation, new BlipOptions
            {
                Sprite = BlipSprite.PoliceCar,
                Colour = BlipColor.Yellow
            });

            var taxiMarker = new Marker(markerLocation, MarkerType.HorizontalCircleFat, Color.FromArgb(150, ConstantColours.Yellow), 4.0f);

            MarkerHandler.AddMarker(taxiMarker);
        }
    }
}
