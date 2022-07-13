using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Vehicle
{
    public class FuelManager : ServerAccessor
    {
        private int pricePerLitre = 1;
        private VehicleManager _cachedVehicleManager;
        private PaymentHandler _cachedPaymentManager;

        private VehicleManager vehicleManager => _cachedVehicleManager ?? (_cachedVehicleManager = Server.Get<VehicleManager>());
        private PaymentHandler paymentHandler => _cachedPaymentManager ?? (_cachedPaymentManager = Server.Get<PaymentHandler>());

        public FuelManager(Server server) : base(server)
        {
            //server.RegisterEventHandler("Vehicle.OnPumpRefuel", new Action<Player, int, int>(OnPumpRefuel));
        }

        public int GetMaxRefuelAmount(Session.Session playerSession, Models.Vehicle veh)
        {
            var currentBalance = paymentHandler.GetPaymentTypeBalance(playerSession);
            var amountToRefuel = Math.Ceiling(100 - veh.Mods.VehicleFuel);
            if (amountToRefuel >= 99)
                amountToRefuel = 100;
            var maxFuelCost = amountToRefuel * pricePerLitre;

            //Log.SendNotify("Refuelling");
            if (currentBalance >= maxFuelCost)
                return (int)amountToRefuel;

            while (currentBalance < maxFuelCost)
            {
                amountToRefuel--;
                maxFuelCost = amountToRefuel * pricePerLitre;
                if (currentBalance <= 0)
                    break;

                //await BaseScript.Delay(0);
            }

            return (int)amountToRefuel;
        }

        [EventHandler("Vehicle.OnFuelUpdate")]
        private void OnFuelUpdate([FromSource] Player source, int vehId, float fuelAmount)
        {
            //Log.Debug($"{source.Name} is attempting to set the fuel level of vehicle #{vehId} to {fuelAmount}%");

            var veh = vehicleManager.GetVehicle(vehId);

            if(veh == null) return;

            veh.Mods.VehicleFuel = fuelAmount;
        }

        [EventHandler("Vehicle.RequestFuelLevel")]
        private void OnRequestFuel([FromSource] Player source, int vehId)
        {
            Log.Verbose($"{source.Name} is requesting the fuel level of vehicle #{vehId}");
            var veh = vehicleManager.GetVehicle(vehId);

            if (veh == null)
            {
                source.TriggerEvent("Vehicle.RecieveFuelLevel", new Random().Next(35, 80));
                return;
            }

            if (veh.Mods.VehicleFuel == -1f)
            {
                veh.Mods.VehicleFuel = new Random().Next(35, 80);
            }

            source.TriggerEvent("Vehicle.RecieveFuelLevel", veh.Mods.VehicleFuel);
        }

        [EventHandler("Vehicle.OnManualRefuel")]
        private async void OnManualRefuel([FromSource] Player source, int vehId, int vehNetId, float refuelAmount)
        {
            Log.Verbose($"{source.Name} is doing a manual refuel on vehicle #{vehId}");
            var playerSession = Sessions.GetPlayer(source);
            var veh = vehicleManager.GetVehicle(vehId);
            var playerVehEntity = Entity.FromNetworkId(vehNetId);

            if (playerSession == null || veh == null || playerVehEntity == null || !(playerVehEntity is CitizenFX.Core.Vehicle playerVeh)) return;

            var playerInv = playerSession./*GetInventory()*/Inventory;
            var petrolCan = playerInv.GetItem("WEAPON_PETROLCAN");

            if(petrolCan == null && !Server.Get<JobHandler>().OnDutyAs(playerSession, JobType.Police | JobType.EMS)) return;

            await DoRefuel(playerSession, veh, playerVeh, refuelAmount);

            if(petrolCan != null)
                playerInv.AddItem(petrolCan, -1);

            Log.ToClient("[Fuel]", "You finished refuelling", ConstantColours.Fuel, source);

            source.TriggerEvent("Weapons.LoadWeapons");
        }

        [EventHandler("Vehicle.OnPumpRefuel")]
        private async void OnPumpRefuel([FromSource] Player source, int vehId, int vehNetId)
        {
            var playerSession = Sessions.GetPlayer(source);
            var veh = vehicleManager.GetVehicle(vehId);

            if (playerSession == null || veh == null) return;

            var refuelAmount = GetMaxRefuelAmount(playerSession, veh);

            if (refuelAmount > 0)
            {
                var fuelPrice = refuelAmount * pricePerLitre;

                if (paymentHandler.CanPayForItem(playerSession, fuelPrice))
                {
                    var playerVeh = Entity.FromNetworkId(vehNetId);

                    if (playerVeh == null)
                    {
                        playerVeh = playerSession.Ped;
                    }

                    if(!await DoRefuel(playerSession, veh, playerVeh, refuelAmount)) return;
                    paymentHandler.PayForItem(playerSession, fuelPrice, "fuel");
                    Log.ToClient("[Fuel]", $"You paid ${fuelPrice} to refuel your vehicle", ConstantColours.Fuel, source);
                }
                else
                {
                    Log.ToClient("[Fuel]", "You cannot afford to refuel this car", ConstantColours.Fuel, source);
                }
            }
            else
            {
                Log.ToClient("[Fuel]", "You cannot refuel this vehicle right now", ConstantColours.Fuel, source);
            }
        }

        private async Task<bool> DoRefuel(Session.Session playerSession, Models.Vehicle vehObj, Entity vehEntity, float refuelAmount)
        {
            var source = playerSession.Source;
            var startingPlayerPosition = playerSession.GetPlayerPosition();
            var startingVehPosition = vehEntity.Position;

            var previousFuelAmount = vehObj.Mods.VehicleFuel;
            var refueled = 0f;
            int refuelTick = 1;
            float refuelTickAmount = 0.5f;

            Log.ToClient("[Fuel]", $"Refuelling...", ConstantColours.Fuel, source);

            Log.Info($"{playerSession.PlayerName} is refueling vehicle #{vehObj.VehID}");
            while (refueled < refuelAmount)
            {
                if (startingVehPosition != vehEntity.Position)
                {
                    Log.ToClient("[Fuel]", $"Your vehicle moved while refuelling.", ConstantColours.Fuel, source);
                    vehObj.Mods.VehicleFuel = previousFuelAmount;
                    return false;
                }

                if (startingPlayerPosition.DistanceToSquared(playerSession.GetPlayerPosition()) > 3f)
                {
                    Log.ToClient("[Fuel]", "You moved while refuelling", ConstantColours.Fuel, source);
                    vehObj.Mods.VehicleFuel = previousFuelAmount;
                    return false;
                }

                if (vehObj.Mods.VehicleFuel >= 100)
                {
                    return true;
                }

                refueled += refuelTickAmount;
                vehObj.Mods.VehicleFuel += refuelTickAmount;
                await BaseScript.Delay(refuelTick);
                vehObj.Mods.VehicleFuel = Math.Min(100f, vehObj.Mods.VehicleFuel);
            }

            return true;
        }
    }
}
