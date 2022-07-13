using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Roleplay.Client.Enums;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Client.Player
{
    /*internal class WorkingOut
    {
        private static readonly Dictionary<string, string> Scenarios = new Dictionary<string, string>()
        {
            ["chinup"] = "PROP_HUMAN_MUSCLE_CHIN_UPS",
            ["chinupp"] = "PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON",
            ["chinupa"] = "PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY",
            ["benchpress"] = "PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS",
            ["benchpressp"] = "PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS_PRISON",
            ["sitchair"] = "PROP_HUMAN_SEAT_CHAIR_MP_PLAYER"
        };
        private static readonly Vector3 MuscleSandsBlipCoords = new Vector3(-1208.698f, -1574.074f, 4.608f);
        private static readonly Tuple<Vector3, Vector3, float, Vector3, Vector3>[] BenchPressCoords = new Tuple<Vector3, Vector3, float, Vector3, Vector3>[4]
        {
          new Tuple<Vector3, Vector3, float, Vector3, Vector3>(new Vector3(-1197.051f, -1567.65f, 4.619f), new Vector3(-1197.935f, -1568.203f, 4.356f), 305f, new Vector3(-1198.469f, -1568.586f, 4.568f), new Vector3(35.258f, 179.828f, 124.969f)),
          new Tuple<Vector3, Vector3, float, Vector3, Vector3>(new Vector3(-1201.508f, -1562.686f, 4.617f), new Vector3(-1200.698f, -1562.19f, 4.357f), 125f, new Vector3(-1200.164f, -1561.807f, 4.567f), new Vector3(35.262f, 179.931f, -55.033f)),
          new Tuple<Vector3, Vector3, float, Vector3, Vector3>(new Vector3(-1206.504f, -1561.664f, 4.61f), new Vector3(-1207.048f, -1560.891f, 4.366f), 215f, new Vector3(-1207.432f, -1560.357f, 4.559f), new Vector3(35.241f, 179.924f, 34.976f)),
          new Tuple<Vector3, Vector3, float, Vector3, Vector3>(new Vector3(-1200.626f, -1575.885f, 4.609f), new Vector3(-1201.209f, -1575.092f, 4.354f), 215f, new Vector3(-1201.592f, -1574.558f, 4.558f), new Vector3(35.256f, 179.922f, 34.97f))
        };
        private static readonly Tuple<Vector3, float, float, float>[] ChinupCoords = new Tuple<Vector3, float, float, float>[8]
        {
          new Tuple<Vector3, float, float, float>(new Vector3(-1199.93f, -1571.27f, 4.58f), 215f, -0.11f, 0.1f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1204.84f, -1564.22f, 4.58f), 215f, -0.11f, 0.1f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1225.07f, -1600.25f, 4.13f), 270f, -0.14f, -0.03f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1240.25f, -1599.537f, 4.08f), 215f, -0.09f, 0.1125f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1244.75f, -1614.118f, 4.08f), 215f, -0.09f, 0.1125f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1251.191f, -1604.918f, 4.115f), 215f, -0.09f, 0.1125f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1252.341f, -1603.276f, 4.101f), 215f, -0.09f, 0.1125f),
          new Tuple<Vector3, float, float, float>(new Vector3(-1253.378f, -1601.796f, 4.125f), 215f, -0.09f, 0.1125f)
        };
        private static readonly Random Rnd = new Random();
        private const int MsPerS = 1000;
        private const float PlayerDistanceFromMuscleSandsThreshold = 100f;
        private const float PlayerDistanceFromChinupThreshold = 0.7f;
        private const float PlayerDistanceFromBenchPressThreshold = 1f;
        private const string LayDownAnimDictName = "amb@prop_human_seat_muscle_bench_press@enter";
        private const string BarbellPropName = "prop_barbell_02";
        private const int FullWorkoutS = 120;
        private const int SufficientWorkoutS = 30;
        private const int DaysWithoutWorkoutUntilPenalty = 2;
        private static int _dailyWorkoutTimeS;
        private static int _currentWorkoutSessionTime;
        private static bool _hasCompletedWorkout;
        private static bool _hasWorkedOutSufficientS;
        private static TimeSpan _workoutRestExpireDeadline;
        private static TimeSpan _gainsExpireDeadline;
        private static bool _atMuscleSands;
        private static bool _isPlayingEmote;
        private static int _initPlayerHealth;
        private static bool _isDoingChinup;
        private static float _chinupRotx;
        private static float _chinupRoty;
        private static bool _isDoingBenchpress;
        private static Vector3 _barbellPos;
        private static Vector3 _barbellRot;
        private static Vector3 _benchPressPos;
        private static WorkingOut.BenchpressStates _benchPressState;
        private static WorkingOut.ChinupStates _chinupState;
        private static Vector3 _currentClosestEquipPos;
        private static float _equipHeading;

        public static void Init()
        {
            try
            {
                WorkingOut._dailyWorkoutTimeS = 0;
                WorkingOut._hasCompletedWorkout = false;
                Client.Instance.RegisterTickHandler(new Func<Task>(WorkingOut.OnTick));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal static async Task OnTick()
        {
            try
            {
                if (!API.HasAnimDictLoaded("amb@prop_human_seat_muscle_bench_press@enter"))
                {
                    API.RequestAnimDict("amb@prop_human_seat_muscle_bench_press@enter");
                    DateTime endtime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, 1000);
                    while (!API.HasAnimDictLoaded("amb@prop_human_seat_muscle_bench_press@enter"))
                    {
                        await BaseScript.Delay(0);
                        if (DateTime.UtcNow >= endtime)
                            return;
                    }
                }
                if (WorkingOut._hasWorkedOutSufficientS)
                {
                    if (WorkingOut.HasDeadlineExpired(WorkingOut._workoutRestExpireDeadline))
                    {
                        WorkingOut.ApplyWorkoutGainsToPlayer(WorkingOut._dailyWorkoutTimeS);
                        WorkingOut._dailyWorkoutTimeS = 0;
                        WorkingOut._hasWorkedOutSufficientS = false;
                        WorkingOut._hasCompletedWorkout = true;
                        WorkingOut.EnableGainsExpireDeadlineTimer(WorkingOut._workoutRestExpireDeadline);
                    }
                    else if (WorkingOut._hasCompletedWorkout)
                        WorkingOut.DisableGainsExpireDeadlineTimer();
                }
                else if (WorkingOut._hasCompletedWorkout && WorkingOut._gainsExpireDeadline.Days != -1 && WorkingOut.HasDeadlineExpired(WorkingOut._gainsExpireDeadline))
                {
                    WorkingOut.ApplyPenaltyForLazyness();
                    WorkingOut.EnableGainsExpireDeadlineTimer(WorkingOut._gainsExpireDeadline);
                }
                if (WorkingOut.IsPlayerWithinRangeOfMuscleSands((ISpatial)Game.PlayerPed))
                {
                    if (!WorkingOut._atMuscleSands)
                    {
                        WorkingOut._atMuscleSands = true;
                        Client.Instance.RegisterTickHandler(new Func<Task>(WorkingOut.ChinupRoutineTask));
                        Client.Instance.RegisterTickHandler(new Func<Task>(WorkingOut.BenchpressRoutineTask));
                        WorkingOut._chinupState = WorkingOut.ChinupStates.Idle;
                        WorkingOut._benchPressState = WorkingOut.BenchpressStates.Idle;
                    }
                    if (WorkingOut._isPlayingEmote)
                    {
                        int health = ((Entity)Game.PlayerPed).Health;
                        if (health > WorkingOut._initPlayerHealth)
                            WorkingOut._initPlayerHealth = health;
                        bool flag = health < WorkingOut._initPlayerHealth;
                        if (flag || ControlHelper.IsControlPressed((Control)33, true, ControlModifier.None) || (ControlHelper.IsControlPressed((Control)32, true, ControlModifier.None) || ControlHelper.IsControlPressed((Control)34, true, ControlModifier.None)) || ControlHelper.IsControlPressed((Control)35, true, ControlModifier.None))
                        {
                            if (WorkingOut._isDoingChinup || WorkingOut._isDoingBenchpress)
                            {
                                if (WorkingOut._isDoingBenchpress)
                                    API.FreezeEntityPosition(((PoolObject)Game.PlayerPed).Handle, false);
                                WorkingOut._isDoingChinup = false;
                                WorkingOut._isDoingBenchpress = false;
                            }
                            WorkingOut._isPlayingEmote = false;
                            WorkingOut.ControlWeapons(Game.PlayerPed, true, false);
                            if (flag)
                                Game.PlayerPed.Task.ClearAllImmediately();
                            Game.PlayerPed.Task.ClearAll();
                        }
                        await BaseScript.Delay(225);
                        return;
                    }
                    await BaseScript.Delay(1000);
                    return;
                }
                if (WorkingOut._atMuscleSands)
                {
                    WorkingOut._atMuscleSands = false;
                    WorkingOut.DeleteProps("prop_barbell_02", WorkingOut.MuscleSandsBlipCoords, 100f, false);
                    Client.Instance.DeregisterTickHandler(new Func<Task>(WorkingOut.ChinupRoutineTask));
                    Client.Instance.DeregisterTickHandler(new Func<Task>(WorkingOut.BenchpressRoutineTask));
                }
                await BaseScript.Delay(5000);
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            int num = await Task.FromResult<int>(0);
        }

        internal static async Task ChinupRoutineTask()
        {
            try
            {
                switch (WorkingOut._chinupState)
                {
                    case WorkingOut.ChinupStates.Idle:
                        if (WorkingOut._atMuscleSands)
                        {
                            WorkingOut._chinupState = WorkingOut.ChinupStates.EquipmentSelection;
                            return;
                        }
                        await BaseScript.Delay(6000);
                        return;
                        case WorkingOut.ChinupStates.EquipmentSelection:
                        foreach (Tuple<Vector3, float, float, float> chinupCoord in WorkingOut.ChinupCoords)
                        {
                            if ((double)WorkingOut.GetDistanceToLocation(((Entity)Game.PlayerPed).Position, chinupCoord.Item1) < 0.699999988079071)
                            {
                                if (((Entity)PedInteraction.GetClosestBodyFromPosition(chinupCoord.Item1, 0.5f), (Entity)null))
                                {
                                    Screen.ShowNotification("Find equipment that is open.", false);
                                    return;
                                }
                                WorkingOut._currentClosestEquipPos = chinupCoord.Item1;
                                WorkingOut._equipHeading = chinupCoord.Item2;
                                WorkingOut._chinupRotx = chinupCoord.Item3;
                                WorkingOut._chinupRoty = chinupCoord.Item4;
                                WorkingOut._chinupState = WorkingOut.ChinupStates.ReadyToStartRoutine;
                                return;
                            }
                        }
                        await BaseScript.Delay(200);
                        return;
                        case WorkingOut.ChinupStates.ReadyToStartRoutine:
                        Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to start working out.");
                        if (!ControlHelper.IsControlJustPressed((Control)38, true, ControlModifier.None))
                        {
                            if ((double)WorkingOut.GetDistanceToLocation(((Entity)Game.PlayerPed).Position, WorkingOut._currentClosestEquipPos) < 0.699999988079071)
                                return;
                            WorkingOut._chinupState = WorkingOut.ChinupStates.EquipmentSelection;
                            return;
                        }
                        WorkingOut._chinupState = WorkingOut.ChinupStates.RoutineStart;
                        return;
                        case WorkingOut.ChinupStates.RoutineStart:
                        WorkingOut._initPlayerHealth = ((Entity)Game.PlayerPed).Health;
                        WorkingOut.ControlWeapons(Game.PlayerPed, false, true);
                        Tuple<Vector3, float> chinupStartPosition = WorkingOut.GetChinupStartPosition(WorkingOut._currentClosestEquipPos, WorkingOut._equipHeading);
                        WorkingOut.DoChinup(chinupStartPosition.Item1, chinupStartPosition.Item2);
                        WorkingOut._currentWorkoutSessionTime = 0;
                        WorkingOut._chinupState = WorkingOut.ChinupStates.WorkingOut;
                        return;
                        case WorkingOut.ChinupStates.WorkingOut:
                        if (!WorkingOut._isDoingChinup)
                        {
                            WorkingOut._chinupState = WorkingOut.ChinupStates.Idle;
                            return;
                        }
                        await BaseScript.Delay(250);
                        WorkingOut.HandleWorkoutTime();
                        return;
                    default:
                        await BaseScript.Delay(1000);
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            int num = await Task.FromResult<int>(0);
        }

        internal static async Task BenchpressRoutineTask()
        {
            try
            {
                switch (WorkingOut._benchPressState)
                {
                    case WorkingOut.BenchpressStates.Idle:
                        if (WorkingOut._atMuscleSands)
                        {
                            WorkingOut._benchPressState = WorkingOut.BenchpressStates.EquipmentSelection;
                            return;
                        }
                        await BaseScript.Delay(5000);
                        return;
                    case WorkingOut.BenchpressStates.EquipmentSelection:
                        foreach (Tuple<Vector3, Vector3, float, Vector3, Vector3> benchPressCoord in WorkingOut.BenchPressCoords)
                        {
                            if ((double)WorkingOut.GetDistanceToLocation(((Entity)Game.PlayerPed).Position, benchPressCoord.Item1) < 1.0)
                            {
                                if (((Entity)PedInteraction.GetClosestBodyFromPosition(benchPressCoord.Item2, 1.4f), (Entity)null))
                                {
                                    Screen.ShowNotification("Find equipment that is open.", false);
                                    return;
                                }
                                WorkingOut._currentClosestEquipPos = benchPressCoord.Item1;
                                WorkingOut._benchPressPos = benchPressCoord.Item2;
                                WorkingOut._equipHeading = benchPressCoord.Item3;
                                WorkingOut._barbellPos = benchPressCoord.Item4;
                                WorkingOut._barbellRot = benchPressCoord.Item5;
                                WorkingOut._benchPressState = WorkingOut.BenchpressStates.ReadyToStartRoutine;
                                return;
                            }
                        }
                        await BaseScript.Delay(200);
                        return;
                        case WorkingOut.BenchpressStates.ReadyToStartRoutine:
                        Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to start working out.");
                        if (!ControlHelper.IsControlJustPressed((Control)38, true, ControlModifier.None))
                        {
                            if ((double)WorkingOut.GetDistanceToLocation(((Entity)Game.PlayerPed).Position, WorkingOut._currentClosestEquipPos) < 1.0)
                                return;
                            WorkingOut._benchPressState = WorkingOut.BenchpressStates.EquipmentSelection;
                            return;
                        }
                        WorkingOut._benchPressState = WorkingOut.BenchpressStates.MoveToRoutineStartPos;
                        return;
                        case WorkingOut.BenchpressStates.MoveToRoutineStartPos:
                        Game.PlayerPed.Heading(WorkingOut._equipHeading);
                        int num = await Game.SafeTeleport(Game.PlayerPed, WorkingOut._currentClosestEquipPos) ? 1 : 0;
                        WorkingOut.DeleteProps("prop_barbell_02", ((Entity)Game.PlayerPed).Position, 3f, true);
                        WorkingOut.CreateProp("prop_barbell_02", WorkingOut._barbellPos, WorkingOut._barbellRot);
                        await BaseScript.Delay(500);
                        WorkingOut._benchPressState = WorkingOut.BenchpressStates.RoutineStart;
                        return;
                        case WorkingOut.BenchpressStates.RoutineStart:
                        WorkingOut._initPlayerHealth = ((Entity)Game.PlayerPed).Health;
                        WorkingOut.ControlWeapons(Game.PlayerPed, false, true);
                        WorkingOut.SitOnBench();
                        for (int timeToSit = WorkingOut.Rnd.Next(3000, 5001); timeToSit > 0; timeToSit -= 20)
                        {
                            if (!WorkingOut._isPlayingEmote)
                            {
                                WorkingOut._benchPressState = WorkingOut.BenchpressStates.Idle;
                                return;
                            }
                            await BaseScript.Delay(20);
                        }
                        WorkingOut._isPlayingEmote = false;
                        API.FreezeEntityPosition(((PoolObject)Game.PlayerPed).Handle, true);
                        WorkingOut.LayOnBench(((PoolObject)Game.PlayerPed).Handle, (float)WorkingOut._benchPressPos.X, (float)WorkingOut._benchPressPos.Y, (float)WorkingOut._currentClosestEquipPos.Z, WorkingOut._equipHeading);
                        WorkingOut._benchPressState = WorkingOut.BenchpressStates.Benchpress;
                        return;
                        case WorkingOut.BenchpressStates.Benchpress:
                        if (API.IsEntityPlayingAnim(((PoolObject)Game.PlayerPed).Handle, "amb@prop_human_seat_muscle_bench_press@enter", "enter", 3))
                        {
                            await BaseScript.Delay(25);
                            if (WorkingOut._isPlayingEmote)
                                return;
                            WorkingOut._benchPressState = WorkingOut.BenchpressStates.Idle;
                            return;
                        }
                        WorkingOut.DeleteProps("prop_barbell_02", ((Entity)Game.PlayerPed).Position, 3f, true);
                        WorkingOut.DoBenchpress(WorkingOut._benchPressPos, WorkingOut._equipHeading);
                        await BaseScript.Delay(10);
                        WorkingOut._benchPressState = WorkingOut.BenchpressStates.WorkingOut;
                        return;
                        case WorkingOut.BenchpressStates.WorkingOut:
                        if (!WorkingOut._isDoingBenchpress)
                        {
                            WorkingOut._benchPressState = WorkingOut.BenchpressStates.Idle;
                            return;
                        }
                        await BaseScript.Delay(250);
                        WorkingOut.HandleWorkoutTime();
                        return;
                    default:
                        await BaseScript.Delay(1000);
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            int num1 = await Task.FromResult<int>(0);
        }

        private static bool IsPlayerWithinRangeOfMuscleSands(ISpatial player)
        {
            try
            {
                return (double)WorkingOut.GetDistanceToLocation(player.Position, WorkingOut.MuscleSandsBlipCoords) < 100.0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        private static void HandleWorkoutTime()
        {
            try
            {
                ++WorkingOut._currentWorkoutSessionTime;
                if (WorkingOut._currentWorkoutSessionTime < 4)
                    return;
                WorkingOut._currentWorkoutSessionTime = 0;
                ++WorkingOut._dailyWorkoutTimeS;
                if (WorkingOut._hasWorkedOutSufficientS || WorkingOut._dailyWorkoutTimeS < 30)
                    return;
                WorkingOut._hasWorkedOutSufficientS = true;
                TimeSpan currentDayTime = World.CurrentDayTime;
                int clockDayOfWeek = API.GetClockDayOfWeek();
                WorkingOut._workoutRestExpireDeadline = new TimeSpan(clockDayOfWeek + 1 > 6 ? 0 : clockDayOfWeek + 1, currentDayTime.Hours, currentDayTime.Minutes, currentDayTime.Seconds);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static bool HasDeadlineExpired(TimeSpan deadline)
        {
            try
            {
                return Time.HasDeadlineExpired(deadline.Days, deadline.Hours, deadline.Minutes, deadline.Seconds);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        private static void EnableGainsExpireDeadlineTimer(TimeSpan startTime)
        {
            try
            {
                int days = startTime.Days + 2;
                if (days > 6)
                    days -= 7;
                WorkingOut._gainsExpireDeadline = new TimeSpan(days, startTime.Hours, startTime.Minutes, startTime.Seconds);
            }
            catch (Exception ex)
            {
                WorkingOut.DisableGainsExpireDeadlineTimer();
                Log.Error(ex);
            }
        }

        private static void DisableGainsExpireDeadlineTimer()
        {
            try
            {
                WorkingOut._gainsExpireDeadline = new TimeSpan(-1, -1, -1, -1);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void ApplyWorkoutGainsToPlayer(int workoutTime)
        {
            try
            {
                float num1 = (float)workoutTime / 120f;
                float num2 = 0.0f;
                //CurrentPlayer.SprintSpeed += (double)num1 < 0.97 || (double)num1 >= 1.03 ? ((double)num1 >= 0.85 && (double)num1 < 0.97 || (double)num2 >= 1.03 && (double)num2 < 1.15 ? (float)WorkingOut.Rnd.Next(4, 9) / 100f : ((double)num1 >= 0.5 && (double)num1 < 0.85 || (double)num2 >= 1.15 && (double)num2 < 1.5 ? (float)WorkingOut.Rnd.Next(2, 5) / 100f : ((double)num1 >= 0.25 && (double)num1 < 0.5 || (double)num2 >= 1.5 && (double)num2 < 1.75 ? (float)WorkingOut.Rnd.Next(1, 3) / 100f : ((double)num2 < 1.75 || (double)num2 >= 2.0 ? (float)(-1.0 * ((double)WorkingOut.Rnd.Next(7, 15) / 100.0)) : (float)(-1.0 * ((double)WorkingOut.Rnd.Next(1, 8) / 100.0)))))) : (float)WorkingOut.Rnd.Next(8, 13) / 100f;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void ApplyPenaltyForLazyness()
        {
            try
            {
                //CurrentPlayer.SprintSpeed += (float)(-1.0 * ((double)WorkingOut.Rnd.Next(1, 16) / 100.0));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void SitOnBench()
        {
            try
            {
                WorkingOut.DoEmoteAtPosition("sitchair", ((Entity)Game.PlayerPed).GetOffsetPosition(new Vector3(0.0f, -0.8f, -0.5f)), ((Entity)Game.PlayerPed).Heading, true, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void LayOnBench(int pedHandle, float x, float y, float z, float zRotation)
        {
            try
            {
                API.SetEntityCoordsNoOffset(pedHandle, x, y, z - 0.5f, false, false, true);
                API.TaskPlayAnimAdvanced(pedHandle, "amb@prop_human_seat_muscle_bench_press@enter", "enter", x, y, z - 0.5f, 0.0f, 0.0f, zRotation, 8f, -8f, -1, 262152, 0.435f, 2, 0);
                WorkingOut._isPlayingEmote = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void DoBenchpress(Vector3 position, float heading)
        {
            try
            {
                WorkingOut.DoEmoteAtPosition("benchpress", new Vector3((float)position.X, (float)position.Y, (float)(position.Z - 0.280000001192093)), heading, true, false);
                WorkingOut._isDoingBenchpress = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static Tuple<Vector3, float> GetChinupStartPosition(
        Vector3 equipmentPos,
        float equipmentHeading)
        {
            try
            {
                float num1 = equipmentHeading + 90f;
                float num2 = equipmentHeading - 90f;
                float num3 = (double)num1 > 360.0 ? num1 - 360f : num1;
                float num4 = (double)num2 < 0.0 ? 360f + num2 : num2;
                float num5 = equipmentHeading;
                Vector3 vector3;
                if ((double)Game.PlayerPed.Heading > (double)num4 && (double)Game.PlayerPed.Heading < (double)num3)
                {
                    ((Vector3)vector3).ctor((float)equipmentPos.X + WorkingOut._chinupRotx, (float)equipmentPos.Y + WorkingOut._chinupRoty, (float)equipmentPos.Z);
                }
                else
                {
                    ((Vector3)vector3).ctor((float)equipmentPos.X - WorkingOut._chinupRotx, (float)equipmentPos.Y - WorkingOut._chinupRoty, (float)equipmentPos.Z);
                    num5 = equipmentHeading - 180f;
                }
                return new Tuple<Vector3, float>(vector3, num5);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Tuple<Vector3, float>((Vector3)Vector3.Zero, -1f);
            }
        }

        private static void DoChinup(Vector3 position, float heading)
        {
            try
            {
                WorkingOut.DoEmoteAtPosition("chinup", position, heading, false, false);
                WorkingOut._isDoingChinup = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void DoEmoteAtPosition(
        string emoteName,
        Vector3 position,
        float heading,
        bool teleportToPosition,
        bool isSittingEmote = false)
        {
            try
            {
                API.SetScenarioTypeEnabled(WorkingOut.Scenarios[emoteName], true);
                API.ResetScenarioTypesEnabled();
                API.TaskStartScenarioAtPosition(((PoolObject)Game.PlayerPed).Handle, WorkingOut.Scenarios[emoteName], (float)position.X, (float)position.Y, (float)position.Z, heading, -1, isSittingEmote, teleportToPosition);
                WorkingOut._isPlayingEmote = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void ControlWeapons(Ped ped, bool pedCanSelectWeap, bool unarmPlayer)
        {
            try
            {
                if (unarmPlayer)
                    ped.Weapons.Select((WeaponHash)1569615261, true);
                //ped.CanSwitchWeapons(pedCanSelectWeap);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void CreateProp(string propName, Vector3 position, Vector3 rotation)
        {
            try
            {
                IEnumerable<string> source1 = ((IEnumerable<string>)Enum.GetNames(typeof(ObjectHash))).Where<string>((Func<string, bool>)(n => n.Contains(propName)));
                if (!(source1 is IList<string> stringList))
                    stringList = (IList<string>)source1.ToList<string>();
                IList<string> source2 = stringList;
                if (!source2.Any<string>())
                    return;
                //ManipulateObject.CreateObjectAtPosition((ObjectHash)Enum.Parse(typeof(ObjectHash), source2.Skip<string>(0).Take<string>(1).First<string>(), true), position, rotation);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void DeleteProps(
        string propName,
        Vector3 position,
        float distanceThreshold,
        bool deleteImmediately)
        {
            try
            {
                int hash = Game.GenerateHash(propName);
                int num1 = 0;
                int firstObject = API.FindFirstObject(ref num1);
                while (API.FindNextObject(firstObject, ref num1))
                {
                    int num2 = num1;
                    int entityModel = API.GetEntityModel(num2);
                    if (entityModel == hash || entityModel == 2121050683 || entityModel == -63539571)
                    {
                        Entity entity = Entity.FromHandle(num2);
                        if ((double)WorkingOut.GetDistanceToLocation(position, entity.Position) <= (double)distanceThreshold)
                        {
                            if (deleteImmediately)
                                ((PoolObject)entity).Delete();
                            entity.MarkAsNoLongerNeeded();
                        }
                    }
                }
                API.EndFindObject(firstObject);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static float GetDistanceToLocation(Vector3 startPos, Vector3 coords)
        {
            try
            {
                return World.GetDistance(startPos, coords);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1f;
            }
        }

        public enum BenchpressStates
        {
            Idle,
            EquipmentSelection,
            ReadyToStartRoutine,
            MoveToRoutineStartPos,
            RoutineStart,
            Benchpress,
            WorkingOut,
        }

        public enum ChinupStates
        {
            Idle,
            EquipmentSelection,
            ReadyToStartRoutine,
            RoutineStart,
            WorkingOut,
        }
    }*/
}
