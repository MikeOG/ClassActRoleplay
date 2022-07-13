using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Vehicles
{
    public class RandomLockedCars : ClientAccessor
    {
        private int carJackChance = 30;
        private int parkedCarChance = 20;

        private Random rand => new Random((int)(DateTime.Now.Ticks * GetGameTimer()));
        private PedList peds = new PedList();

        public RandomLockedCars(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteringVehicle", new Action<int, int, string>((veh, seat, name) => 
            {
                AttemptEnterVehicle(new Vehicle(veh));
            }));
            EntityDecoration.RegisterProperty("Vehicle.RobAttempts", DecorationType.Int);
            EntityDecoration.RegisterProperty("Vehicle.HasBeenAlerted", DecorationType.Bool);
        }

        private Ped getPedInRange()
        {
            return peds.Select(o => new Ped(o)).FirstOrDefault(o => HasEntityClearLosToEntityInFront(o.Handle, Game.PlayerPed.Handle) && o != Game.PlayerPed && !o.IsPlayer && !o.IsDead);
        }

        private void AttemptEnterVehicle(Vehicle veh)
        {
            var robAttempts = veh.HasDecor("Vehicle.RobAttempts") ? veh.GetDecor<int>("Vehicle.RobAttempts") : 0;

            if(veh.HasDecor("Vehicle.ID")) return;

            veh.LockStatus = VehicleLockStatus.Locked;

            if(robAttempts > 1) return;

            veh.SetDecor("Vehicle.RobAttempts", robAttempts + 1);

            var isCarjacking = veh.Occupants.ToList().Count > 0;

            if (isCarjacking)
            {
                var deadDriver = veh.Driver.IsDead;
                var shouldUnlock = rand.NextBool(carJackChance);
                if (shouldUnlock)
                {
                    veh.LockStatus = VehicleLockStatus.Unlocked;
                    Client.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(veh));
                }

                var vehColour = veh.Mods.PrimaryColor.ToString().AddSpacesToCamelCase();
                var vehName = veh.LocalizedName;
                var vehPlate = veh.Mods.LicensePlate;
                var location = GTAHelpers.GetLocationString();

                if (veh.HasDecor("Vehicle.HasBeenAlerted")) return;

                veh.SetDecor("Vehicle.HasBeenAlerted", true);

                if (deadDriver)
                {
                    attemptPedReport(veh, shouldUnlock);
                }
                else
                {
                    if (rand.NextBool(40)) return;

                    Client.TriggerServerEvent("Alerts.SendCADAlert", $"10-61{(shouldUnlock ? "" : "A")}", $"{vehColour} {vehName} | {vehPlate}", location);
                }
            }
            else
            {
                var shouldUnlock = rand.NextBool(parkedCarChance);
                if (shouldUnlock)
                {
                    veh.LockStatus = VehicleLockStatus.Unlocked;
                    Client.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(veh));
                }

                attemptPedReport(veh, shouldUnlock);
            }
        }

        private async void attemptPedReport(Vehicle veh, bool shouldUnlock)
        {
            var vehColour = veh.Mods.PrimaryColor.ToString().AddSpacesToCamelCase();
            var vehName = veh.LocalizedName;
            var vehPlate = veh.Mods.LicensePlate;
            var location = GTAHelpers.GetLocationString();

            var visiblePed = getPedInRange();

            if (visiblePed == null) return;

            var sendReport = await GTAHelpers.PlayReportAnim(visiblePed);

            if (sendReport)
            {
                Client.TriggerServerEvent("Alerts.SendCADAlert", $"10-61{(shouldUnlock ? "" : "A")}", $"{vehColour} {vehName} | {vehPlate}", location);
            }
        }
    }
}
