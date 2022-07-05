using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    public class VehicleDamageHandler : ClientAccessor
    {
        private float previousEngineHealth = 1000;
        private float previousBodyHealth = 1000;

        public VehicleDamageHandler(Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) =>
            {
                previousEngineHealth = Cache.PlayerPed.CurrentVehicle.EngineHealth;
                previousBodyHealth = Cache.PlayerPed.CurrentVehicle.BodyHealth;

                client.RegisterTickHandler(DamageCheckTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                client.DeregisterTickHandler(DamageCheckTick);
            }));
        }

        private async Task DamageCheckTick()
        {
            if (Cache.PlayerPed.CurrentVehicle.Driver != Cache.PlayerPed) return;

            var veh = Cache.PlayerPed.CurrentVehicle;

            var newEngineHealth = veh.EngineHealth;
            var engineHealthChange = previousEngineHealth - newEngineHealth;
            var engineHealthPercentChange = (previousEngineHealth - newEngineHealth) / previousEngineHealth * 100; // Percent change since last check

            var newBodyHealth = veh.BodyHealth;
            var bodyHealthChange = previousBodyHealth - newBodyHealth;
            var bodyHealthPercentChange = (previousBodyHealth - newBodyHealth) / previousBodyHealth * 100; // Percent change since last check

            previousEngineHealth = newEngineHealth;
            previousBodyHealth = newBodyHealth;

            if (engineHealthChange > 56.575f || bodyHealthChange > 75.35f)
            {
                var healthDamageMult = (engineHealthChange > bodyHealthChange ? engineHealthChange : bodyHealthChange) * 0.02;

                var pedMaxHeatlh = Cache.PlayerPed.MaxHealth;
                var newHealth = Cache.PlayerPed.Health - healthDamageMult;

                //Log.ToChat($"New ped health is {newHealth}");

                if (newHealth > 10)
                {
                    Cache.PlayerPed.Health = (int)Math.Floor(newHealth > pedMaxHeatlh ? pedMaxHeatlh : newHealth);
                }
            }

            if (engineHealthChange > 289.5f || newBodyHealth == 0)
            {
                veh.IsEngineRunning = false;
                veh.EngineHealth = 0;

                if (veh.BodyHealth < 150.0f)
                {
                    veh.BodyHealth = 150.0f;
                }
            }

            //Log.ToChat($"Engine health: {newEngineHealth}");
            //Log.ToChat($"Engine health change: {engineHealthChange}");
            //Log.ToChat($"Body health: {newBodyHealth}");
            //Log.ToChat($"Body health change: {bodyHealthChange}");

            await BaseScript.Delay(1000);
        }
    }
}
