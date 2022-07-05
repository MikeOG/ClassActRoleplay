using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared;
using System;
using System.Collections.Generic;

namespace Roleplay.Client.Helpers
{
    internal static class Time
    {
        public static readonly string[] DaysOfWeekAbbrev = new string[7]
        {
          "Sun",
          "Mon",
          "Tue",
          "Wed",
          "Thu",
          "Fri",
          "Sat",
        };
        public static readonly Dictionary<string, TimeSpan> EnvironmentTimes = new Dictionary<string, TimeSpan>()
        {
          {
            "sunrise",
            new TimeSpan(4, 30, 0)
          },
          {
            "sunUp",
            new TimeSpan(5, 30, 0)
          },
          {
            "streetLightsOff",
            new TimeSpan(18, 0, 0)
          },
          {
            "completeDarkness",
            new TimeSpan(21, 0, 0)
          }
        };

        public static bool IsDarkOut()
        {
            try
            {
                TimeSpan currentDayTime = World.CurrentDayTime;
                if (currentDayTime.CompareTo(Time.EnvironmentTimes["streetLightsOff"]) < 0)
                {
                    if (currentDayTime.CompareTo(Time.EnvironmentTimes["sunUp"]) >= 0)
                        goto label_4;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        label_4:
            return false;
        }

        public static Time.TimeRemaining GetTimeRemainingUntilDeadline(
        int day,
        int hour,
        int min,
        int sec)
        {
            try
            {
                int clockDayOfWeek = API.GetClockDayOfWeek();
                TimeSpan currentDayTime = World.CurrentDayTime;
                int days = day - clockDayOfWeek;
                if (days < 0)
                    days += 7;
                int hours = hour - currentDayTime.Hours;
                if (hours < 0)
                {
                    --days;
                    hours += 24;
                }
                int minutes = min - currentDayTime.Minutes;
                if (minutes < 0)
                {
                    --hours;
                    if (hours < 0)
                    {
                        --days;
                        hours += 24;
                    }
                    minutes += 60;
                }
                int seconds = sec - currentDayTime.Seconds;
                if (seconds < 0)
                {
                    --minutes;
                    if (minutes < 0)
                    {
                        --hours;
                        if (hours < 0)
                        {
                            --days;
                            hours += 24;
                        }
                        minutes += 60;
                    }
                    seconds += 60;
                }
                return new Time.TimeRemaining(days, hours, minutes, seconds);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Time.TimeRemaining(-1, -1, -1, -1);
            }
        }

        public static bool HasDeadlineExpired(int day, int hour, int min, int sec)
        {
            try
            {
                return Time.GetTimeRemainingUntilDeadline(day, hour, min, sec).Days < 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public static string GetCurrentTimeString(bool is24HourClock)
        {
            try
            {
                int num = API.GetClockHours();
                int clockMinutes = API.GetClockMinutes();
                string str1 = clockMinutes < 10 ? "0" + clockMinutes.ToString() : clockMinutes.ToString();
                if (!is24HourClock)
                    num = num > 12 ? num - 12 : num;
                string str2 = num < 10 ? "0" + num.ToString() : num.ToString();
                string str3 = Time.DaysOfWeekAbbrev[API.GetClockDayOfWeek()] + " " + str2 + ":" + str1;
                if (is24HourClock)
                    return str3;
                char ch = (num <= 11 ? 0 : (num < 24 ? 1 : 0)) != 0 ? 'p' : 'a';
                return str3 + (object)ch;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return "";
            }
        }

        public class TimeRemaining
        {
            public int Days { get; }

            public int Hours { get; }

            public int Minutes { get; }

            public int Seconds { get; }

            public TimeRemaining(int days, int hours, int minutes, int seconds)
            {
                this.Days = days;
                this.Hours = hours;
                this.Minutes = minutes;
                this.Seconds = seconds;
            }
        }

        public enum DaysOfWeek
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
        }
    }
}