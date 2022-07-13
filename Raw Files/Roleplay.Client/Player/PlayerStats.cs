using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared;

namespace Roleplay.Client.Player
{
    internal class PlayerStats
    {
        private static readonly Dictionary<PlayerStats.MpPlayerStats, PlayerStats.PlayerStatModel> Stats = new Dictionary<PlayerStats.MpPlayerStats, PlayerStats.PlayerStatModel>()
        {
            {
                PlayerStats.MpPlayerStats.Flying,
                new PlayerStats.PlayerStatModel("MP0_FLYING_ABILITY")
            },
            {
                PlayerStats.MpPlayerStats.LungCapacity,
                new PlayerStats.PlayerStatModel("MP0_LUNG_CAPACITY")
            },
            {
                PlayerStats.MpPlayerStats.Shooting,
                new PlayerStats.PlayerStatModel("MP0_SHOOTING_ABILITY")
            },
            {
                PlayerStats.MpPlayerStats.Stamina,
                new PlayerStats.PlayerStatModel("MP0_STAMINA")
            },
            {
                PlayerStats.MpPlayerStats.Stealth,
                new PlayerStats.PlayerStatModel("MP0_STEALTH_ABILITY")
            },
            {
                PlayerStats.MpPlayerStats.Strength,
                new PlayerStats.PlayerStatModel("MP0_STRENGTH")
            },
            {
                PlayerStats.MpPlayerStats.Wheelie,
                new PlayerStats.PlayerStatModel("MP0_WHEELIE_ABILITY")
            }
        };

        public static void IncrementStat(PlayerStats.MpPlayerStats stat)
        {
            try
            {
                PlayerStats.PlayerStatModel playerStatModel;
                if (!PlayerStats.Stats.TryGetValue(stat, out playerStatModel))
                    return;
                int currentValue = playerStatModel.CurrentValue;
                playerStatModel.CurrentValue = MathUtil.Clamp(currentValue + 1, 0, 100);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void DecrementStat(PlayerStats.MpPlayerStats stat)
        {
            try
            {
                PlayerStats.PlayerStatModel playerStatModel;
                if (!PlayerStats.Stats.TryGetValue(stat, out playerStatModel))
                    return;
                playerStatModel.CurrentValue--;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static int GetStat(PlayerStats.MpPlayerStats stat)
        {
            try
            {
                PlayerStats.PlayerStatModel playerStatModel;
                return !PlayerStats.Stats.TryGetValue(stat, out playerStatModel) ? -1 : playerStatModel.CurrentValue;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1;
            }
        }

        public static void SetStat(PlayerStats.MpPlayerStats stat, int value)
        {
            try
            {
                PlayerStats.PlayerStatModel playerStatModel;
                if (!PlayerStats.Stats.TryGetValue(stat, out playerStatModel))
                    return;
                playerStatModel.CurrentValue = value;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public enum MpPlayerStats
        {
            Flying,
            LungCapacity,
            Shooting,
            Stamina,
            Stealth,
            Strength,
            Wheelie,
        }

        public class PlayerStatModel
        {
            private readonly uint _hash;
            public PlayerStatModel(string statName)
            {
                this._hash = (uint)Game.GenerateHash(statName);
            }

            public int CurrentValue
            {
                get
                {
                    try
                    {
                        int num = -1;
                        API.StatGetInt(this._hash, ref num, -1);
                        return num;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        return -1;
                    }
                }
                set
                {
                    try
                    {
                        API.StatSetInt(this._hash, MathUtil.Clamp(value, 0, 100), true);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }
        }
    }
}