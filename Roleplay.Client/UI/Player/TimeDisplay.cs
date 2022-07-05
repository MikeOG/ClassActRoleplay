using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Player;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared.Helpers;
using Font = CitizenFX.Core.UI.Font;

namespace Roleplay.Client.UI.Player
{
    public class TimeDisplay : ClientAccessor
    {
        private ScreenText timeText;

        public TimeDisplay(Client client) : base(client)
        {
            timeText = new ScreenText("12:00", 166, 1026, 0.25f, async () =>
            {
                var hour = TimeWeatherSync.CurrentHour;
                var minute = TimeWeatherSync.CurrentMinute < 10 ? $"0{TimeWeatherSync.CurrentMinute}" : TimeWeatherSync.CurrentMinute.ToString();
                var timeEnding = hour < 12 ? "AM" : "PM";

                timeText.Caption = $"{hour}:{minute} {timeEnding}";
            }, System.Drawing.Color.FromArgb(255, 200, 200, 200), Font.ChaletLondon, Alignment.Left);

            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                timeText.Position = new PointF(166 * 0.6666666667f, 1026 * 0.6666666667f);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                timeText.Position = new PointF(166 * 0.6666666667f, 1026 * 0.6666666667f);
            }));
            client.RegisterTickHandler(DrawTimeTick);
        }

        private async Task DrawTimeTick()
        {
            if(CinematicMode.InCinematicMode) return;

            timeText.DrawTick();
        }
    }
}
