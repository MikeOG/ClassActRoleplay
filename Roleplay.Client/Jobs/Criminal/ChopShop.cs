using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using Roleplay.Shared;
using Roleplay.Client.UI;
using Roleplay.Client.Helpers;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Loader;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client.Jobs.Criminal
{
    internal class ChopShop : JobClass
    {
        #region Variables

        private static List<VehicleHash> VehicleHashValues = new List<VehicleHash>()
        {
            VehicleHash.Alpha,
            VehicleHash.Bati,
            VehicleHash.BfInjection,
            VehicleHash.Biff,
            VehicleHash.BJXL,
            VehicleHash.Blazer,
            VehicleHash.Blista,
            VehicleHash.BobcatXL,
            VehicleHash.Buffalo,
            VehicleHash.Caddy2,
            VehicleHash.Comet2,
            VehicleHash.Coquette,
            VehicleHash.Daemon,
            VehicleHash.Dilettante,
            VehicleHash.Dominator,
            VehicleHash.Dubsta,
            VehicleHash.Dukes,
            VehicleHash.Fusilade,
            VehicleHash.Glendale,
            VehicleHash.Granger,
            VehicleHash.Habanero,
            VehicleHash.Hakuchou,
            VehicleHash.Hexer,
            VehicleHash.Huntley,
            VehicleHash.Ingot,
            VehicleHash.Intruder,
            VehicleHash.Jackal,
            VehicleHash.Mesa,
            VehicleHash.Minivan,
            VehicleHash.Ninef,
            VehicleHash.Penumbra,
            VehicleHash.Picador,
            VehicleHash.Pigalle,
            VehicleHash.RancherXL,
            VehicleHash.Regina,
            VehicleHash.Rhapsody,
            VehicleHash.Rocoto,
            VehicleHash.Ruiner,
            VehicleHash.Rumpo,
            VehicleHash.Seminole,
            VehicleHash.Serrano,
            VehicleHash.Sultan,
            VehicleHash.Surano,
            VehicleHash.Warrener,
            VehicleHash.Youga,
        };

        private Random rand = new Random((int)DateTime.Now.Ticks);

        private List<string> VehicleHashNames = VehicleHashValues.Select(o => o.ToString().AddSpacesToCamelCase()).ToList();

        private Vector3 chopShopEnter = Vector3.Zero;
        private Vector3 chopShopExit = new Vector3(970.58f, -2990.94f, -39.65f);

        private List<Vector3> dropoffPoints = new List<Vector3>()
        {
            new Vector3(979.255f, -3002.136f, -39.8f)
        };

        private List<Vector3> payPhoneLocations = new List<Vector3>()
        {
            new Vector3(213.54f, -853.21f, 29.60f),
            new Vector3(187.56f, -1043.76f, 28.53f),
            new Vector3(298.15f, -795.61f, 28.68f),
            new Vector3(55.25f, -1081.58f, 28.65f),
            new Vector3(56.12f, -1079.85f, 28.66f),
            new Vector3(190.27f, -1044.68f, 28.53f),
            new Vector3(298.82f, -793.85f, 28.68f)
        };

        private List<VehicleHash> currentVehicleList = null;
        /// <summary>
        /// Overall time of current vehicle run
        /// </summary>
        private DateTime totalVehicleRunStartTime = DateTime.Now;
        /// <summary>
        /// Time per seperate run
        /// </summary>
        private DateTime seperateVehicleStartTime = DateTime.Now;

        private bool isChoppingVehicle = false;
        #endregion

        #region Init
        public ChopShop()
        {
            Client.RegisterTickHandler(OnTick);
            Client.RegisterEventHandler("Player.CheckForInteraction", new Action(async () =>
            {
                if (isNearStartLocation() && currentVehicleList == null)
                {
                    currentVehicleList = await getNewVehicleList();
                    await Game.PlayerPed.Task.PlayAnimation("amb@code_human_wander_mobile@male@base", "static", 2.0f, 2.0f, 2000, (AnimationFlags)50, 0);
                    listCurrentVehicles();
                    totalVehicleRunStartTime = DateTime.Now;
                }
                else if (isNearEnterLocation())
                {
                    Entity teleportEntity = Game.PlayerPed;

                    if (((Ped)teleportEntity).IsInVehicle())
                        teleportEntity = Game.PlayerPed.CurrentVehicle;

                    await teleportEntity.TeleportToLocation(chopShopExit, true);

                    teleportEntity.Heading = 210.0f;
                }
                else if (isNearExitLocation())
                {
                    Entity teleportEntity = Game.PlayerPed;

                    if (((Ped)teleportEntity).IsInVehicle())
                        teleportEntity = Game.PlayerPed.CurrentVehicle;

                    await teleportEntity.TeleportToLocation(chopShopEnter, true);
                }
            }));
            Client.RegisterEventHandler("ChopShop.SetCurrentGarage", new Action<List<object>>(enterLocation =>
            {
                chopShopEnter = new Vector3(enterLocation.Select(Convert.ToSingle).ToArray());
                MarkerHandler.AddMarker(chopShopEnter, new MarkerOptions
                {
                    Color = ConstantColours.Red,
                    ScaleFloat = 2.0f
                });

                MarkerHandler.AddMarker(dropoffPoints, new MarkerOptions
                {
                    Color = ConstantColours.Red,
                    ScaleFloat = 2.0f
                });

                MarkerHandler.AddMarker(payPhoneLocations, new MarkerOptions
                {
                    Color = ConstantColours.Red,
                    ScaleFloat = 0.5f
                });
            }));
            Client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemStandard
            {
                Title = "Chop vehicle",
                OnActivate = state =>
                {
                    checkChopInteraction();
                }
            }, () => isNearDropoff() && currentVehicleList != null, 250);
            Client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemStandard
            {
                Title = "Show vehicle list",
                OnActivate = state => { listCurrentVehicles(); }
            }, () => currentVehicleList != null, 250);

            Client.TriggerServerEvent("ChopShop.GetCurrentGarage");
        }
        #endregion

        #region Methods

        private async Task OnTick()
        {
            if (currentVehicleList != null)
            {
                if (getRemainingTime() == "00:00:00")
                {
                    currentVehicleList = null;
                }
            }
        }

        private async void checkChopInteraction()
        {
            if (isChoppingVehicle) return;

            isChoppingVehicle = true;
            try
            {
                var playerVeh = Game.PlayerPed.IsInVehicle() ? Game.PlayerPed.CurrentVehicle : Game.PlayerPed.LastVehicle;

                VehicleHash vehHash = (VehicleHash)playerVeh.Model.Hash;
                bool hasVehId = playerVeh.HasDecor("Vehicle.ID");
                bool isOwnedVeh = hasVehId && playerVeh.GetDecor<int>("Vehicle.ID") < 1000000;
                if (currentVehicleList.FindAll(o => o.ToString().Contains(vehHash.ToString())).Any() && !isOwnedVeh)
                {
                    float engineHealth = playerVeh.EngineHealth < 100 ? 100 : playerVeh.EngineHealth;
                    float bodyHealth = playerVeh.BodyHealth < 100 ? 100 : playerVeh.BodyHealth;
                    playerVeh.LockStatus = VehicleLockStatus.Locked;
                    if (playerVeh.Occupants.Length != 0)
                    {
                        Log.ToChat("[Chopshop]", "Everyone out of the vehicle so chopping can begin", ConstantColours.Criminal);
                        while (playerVeh.Occupants.Length != 0)
                            await BaseScript.Delay(0);
                    }

                    await playerVeh.Doors.GetAll().ToList().ForEachAsync(async o =>
                    {
                        await BaseScript.Delay(800);
                        o.Open();
                    });

                    for (int i = 1; i < 12; i++)
                    {
                        if (playerVeh.Wheels[i] != null)
                            playerVeh.Wheels[i].Burst();
                    }

                    playerVeh.IsDriveable = false;
                    Log.Debug($"Engine health is {engineHealth}");
                    Log.Debug($"Body health is {bodyHealth}");
                    int vehicleReward = (int)Math.Round((getBasePriceForClass(playerVeh) * (engineHealth / rand.Next(500, 700)) * (bodyHealth / rand.Next(500, 700))) / 2, 0);
                    await BaseScript.Delay(2500);
                    playerVeh.Delete();
                    currentVehicleList.Remove(vehHash);
                    giveVehCompletionAward(vehicleReward);
                }
                else
                {
                    Log.ToChat("[Chopshop]", "This vehicle isn't wanted right now", ConstantColours.Criminal);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                isChoppingVehicle = false;
            }
        }

        private int getBasePriceForClass(CitizenFX.Core.Vehicle veh)
        {
            int basePrice = 300;
            switch (veh.ClassType)
            {
                case VehicleClass.Commercial:
                case VehicleClass.Sedans:
                case VehicleClass.Motorcycles:
                    basePrice = 500;
                    break;
                case VehicleClass.Sports:
                case VehicleClass.Super:
                    basePrice = 650;
                    break;
                case VehicleClass.SUVs:
                case VehicleClass.OffRoad:
                case VehicleClass.Coupes:
                case VehicleClass.Compacts:
                case VehicleClass.Vans:
                case VehicleClass.Utility:
                case VehicleClass.SportsClassics:
                    basePrice = 550;
                    break;
            }

            return basePrice;
        }

        private async Task<List<VehicleHash>> getNewVehicleList()
        {
            List<VehicleHash> vehicleList = new List<VehicleHash>();
            int vehHashIndexes = VehicleHashValues.Count - 1;
            List<int> chosenNumbers = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                int index = new Random().Next(0, vehHashIndexes);
                do
                {
                    index = new Random().Next(0, vehHashIndexes);
                }
                while (chosenNumbers.Contains(index) || VehicleHashNames[index].Any(c => char.IsDigit(c)) || VehicleHashNames[index].Contains("police") || VehicleHashNames[index].Contains("sheriff"));

                vehicleList.Add(VehicleHashValues[index]);
                chosenNumbers.Add(index);
                await BaseScript.Delay(0);
            }

            return vehicleList;
        }

        private void listCurrentVehicles()
        {
            if (currentVehicleList != null)
            {
                var thing = "Get the following vehicles:";
                currentVehicleList.ForEach(o =>
                {
                    thing += $"<br>{o.ToString().AddSpacesToCamelCase().FirstLetterToUpper()}";
                });
                //BaseScript.TriggerEvent("chat:addMessage", new { templateId = "chop", multiline = true, args = new[] { thing, getRemainingTime() } });
                BaseScript.TriggerEvent("UI.Toast", $"{thing} <b style='font-size:1rem' class='right'>Remaining time: {getRemainingTime()}</b>", 7500);
            }
        }

        private string getRemainingTime()
        {
            var remTime = (TimeSpan.FromHours(1) - (DateTime.Now - totalVehicleRunStartTime)).ToString(@"hh\:mm\:ss");
            return remTime;
        }

        private void giveVehCompletionAward(int rewardPrice)
        {
            TimeSpan startSpan = TimeSpan.FromTicks(seperateVehicleStartTime.Ticks);
            TimeSpan finishSpan = TimeSpan.FromTicks(DateTime.Now.Ticks);
            double timeBetween = finishSpan.TotalSeconds - startSpan.TotalSeconds;
            Log.Debug($"Time difference = {timeBetween}");
            float timeMult = getTimeMultipler(timeBetween);
            int rewardTotal = (int)(rewardPrice * timeMult) * 3;
            Log.ToChat("[Chopshop]", $"You got ${rewardTotal} for that vehicle", ConstantColours.Criminal);
            Client.TriggerServerEvent("Job.RequestRemoteJobPayment", rewardTotal, rewardTotal * 4,  rewardTotal * (rewardTotal * 4), "chop shop");
            if (currentVehicleList.Count == 0)
            {
                currentVehicleList = null;
                Log.ToChat("[Chopshop]", "Looks like you got all the vehicles. Here's some extra money", ConstantColours.Criminal);
                var extraPay = rand.Next(150, 300);
                Client.TriggerServerEvent("Job.RequestRemoteJobPayment", extraPay, extraPay * 4, extraPay * (extraPay * 4), "chop shop extra pay");
            }
        }

        private float getTimeMultipler(double totalTime)
        {
            float timeMult = 1.1f;
            if (totalTime < 240)
                timeMult = 1.5f;
            else if (totalTime >= 240 && totalTime < 360)
                timeMult = 1.4f;
            else if (totalTime >= 360 && totalTime < 480)
                timeMult = 1.3f;
            return timeMult;
        }

        private bool isNearDropoff()
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            var closeLocation = dropoffPoints.Select(o => new Vector3(o.X, o.Y, o.Z))
                .Where(o => o.DistanceToSquared(playerPos) < Math.Pow(2, 2))
                .OrderBy(o => o.DistanceToSquared(playerPos));

            return closeLocation.Any() && !isChoppingVehicle;
        }

        private bool isNearStartLocation()
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            var closeLocation = payPhoneLocations.Select(o => new Vector3(o.X, o.Y, o.Z))
                .Where(o => o.DistanceToSquared(playerPos) < Math.Pow(2, 1.5))
                .OrderBy(o => o.DistanceToSquared(playerPos));

            return closeLocation.Any();
        }

        private bool isNearEnterLocation()
        {
            return Game.PlayerPed.Position.DistanceToSquared(chopShopEnter) < 20.0f;
        }

        private bool isNearExitLocation()
        {
            return Game.PlayerPed.Position.DistanceToSquared(chopShopExit) < 20.0f;
        }
        #endregion
    }
}
