using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Player;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared;

namespace Roleplay.Client.UI.Player
{
    public class HealthDisplay : ClientAccessor
    {
        private Rect backRect;

        private Rect healthFrontBar;
        private Rect healthBackBar;

        private Rect armourFrontBar;
        private Rect armourBackBar;

        public HealthDisplay(Client client) : base(client)
        {
            backRect = new Rect(29, 1045, 270, 18, null, Color.FromArgb(160, 0, 0, 0));

            healthBackBar = new Rect(29, 1049, 134, 9, null, Color.FromArgb(180, 57, 100, 54));
            
            healthFrontBar = new Rect(29, 1049, 134, 9, () =>
            {
                var healthOnePercent = healthBackBar.Size.Width / Game.PlayerPed.MaxHealth;
                var health = healthOnePercent * Game.PlayerPed.Health;
                if (health < 0)
                    health = 0;

                healthFrontBar.Size = new SizeF(health, healthFrontBar.Size.Height);

                return Task.FromResult(0);
            }, Color.FromArgb(180, 88, 154, 83));
            
            armourBackBar = new Rect(166, 1049, 133, 9, null, Color.FromArgb(180, 52, 93, 113));
            
            armourFrontBar = new Rect(166, 1049, 133, 9, () =>
            {
                var armourOnePercent = armourBackBar.Size.Width / 100;

                armourFrontBar.Size = new SizeF(armourOnePercent * Game.PlayerPed.Armor, healthFrontBar.Size.Height);

                return Task.FromResult(0);
            }, Color.FromArgb(180, 64, 133, 171));

            client.RegisterTickHandler(DrawTick);
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                var vehicle = new CitizenFX.Core.Vehicle(veh);

                if (vehicle.ClassType == VehicleClass.Cycles) return;
                client.DeregisterTickHandler(DrawTick);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                var vehicle = new CitizenFX.Core.Vehicle(veh);

                if (vehicle.ClassType == VehicleClass.Cycles) return;
                client.RegisterTickHandler(DrawTick);
            }));
        }

        private async Task DrawTick()
        {
            if (CinematicMode.InCinematicMode) return;

            backRect.DrawTick();
            healthBackBar.DrawTick();
            healthFrontBar.DrawTick();
            armourBackBar.DrawTick();
            armourFrontBar.DrawTick();
        }
    }
}

