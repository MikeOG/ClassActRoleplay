using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Client.Enviroment
{
    public class TimeWeatherSync : ClientAccessor
    {
        public static int CurrentHour { get; private set; } = 12;
        public static int CurrentMinute { get; private set; } = 0;

        public TimeWeatherSync(Client client) : base(client)
        {
            client.RegisterEventHandler("Sync.ReceiveTime", new Action<int, int>(OnReceiveTime));
            client.RegisterEventHandler("Sync.ReceiveWeather", new Action<string>(OnReceiveWeather));

            client.TriggerServerEvent("Sync.RequestTime");
            client.TriggerServerEvent("Sync.RequestWeather");
        }

        private void OnReceiveTime(int hour, int minute)
        {
            CurrentHour = hour;
            CurrentMinute = minute;
            CitizenFX.Core.Native.API.NetworkOverrideClockTime(hour, minute, 0);
            
        }

        private void OnReceiveWeather(string weatherType)
        {
            Enum.TryParse<Weather>(weatherType, out var weather);

            World.TransitionToWeather(weather, 45.0f);
        }
    }
}
