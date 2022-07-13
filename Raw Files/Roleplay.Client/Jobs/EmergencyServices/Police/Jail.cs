using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
    public class Jail : ClientAccessor
    {
        private static float[] PreviousPosition;
        private static Vector3 PreviousPositionFull;
        private static float[][] PrisonPolygon = new float[12][]
        {
          new float[2]{ 1626.658f, 2583.903f },
          new float[2]{ 1717.181f, 2585.599f },
          new float[2]{ 1722.866f, 2587.398f },
          new float[2]{ 1723.232f, 2594.475f },
          new float[2]{ 1750.513f, 2594.101f },
          new float[2]{ 1750.596f, 2570.67f },
          new float[2]{ 1763.58f, 2570.063f },

          new float[2]{ 1759.76f, 2601.23f },
          new float[2]{ 1832.67f, 2593.75f },

          new float[2]{ 1826.5f, 2531.3f },

          new float[2]{ 1638.85f, 2467.225f },
          new float[2]{ 1579.635f, 2578.353f }
        };
        private Vector3 EnterPrisonLocation = new Vector3(1676.277f, 2536.605f, 45.565f);
        private Vector3 ExitPrisonLocation = new Vector3(1849.11f, 2586.02f, 46f);
        private static float ExitPrisonHeading = 265f;
        private static float EnterPrisonHeading = 120f;




        public Jail(Client client) : base(client)
        {
            client.RegisterEventHandler("Jail.SetJailState", new Action<bool>(OnSetJail)); 
        }

        private void OnSetJail(bool putInJail)
        {
            if (putInJail)
            {
                Client.RegisterTickHandler(JailTick);
                PreviousPosition = new float[] { Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y };
                PreviousPositionFull = Game.PlayerPed.Position;
                TriggerServerEvent("InteractSound_SV:PlayWithinDistance", 1, "jailed", 0.05);
                Client.TriggerServerEvent("InteractSound_SV:PlayWithinDistance", 1, "jailed", 0.05);
            }
            else
            {
                Client.DeregisterTickHandler(JailTick);
                Game.PlayerPed.Position = ExitPrisonLocation;
                Game.PlayerPed.Heading = ExitPrisonHeading;
            }
        }

        private async Task JailTick()
        {
            float[] currentPosition = { Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y };
            if (!PolygonCollision.Contains(PrisonPolygon, currentPosition))
            {
                if (PolygonCollision.Contains(PrisonPolygon, PreviousPosition))
                {
                    Game.PlayerPed.PositionNoOffset = PreviousPositionFull;
                    //await BaseScript.Delay(50);
                    return;
                }
                else
                {
                    Game.PlayerPed.IsPositionFrozen = true;
                    API.RequestCollisionAtCoord(EnterPrisonLocation.X, EnterPrisonLocation.Y, EnterPrisonLocation.Z);
                    Game.PlayerPed.PositionNoOffset = EnterPrisonLocation;
                    Game.PlayerPed.Heading = EnterPrisonHeading;
                    while (!API.HasCollisionLoadedAroundEntity(Game.PlayerPed.Handle))
                        await BaseScript.Delay(0);

                    Game.PlayerPed.IsPositionFrozen = false;
                }
            }
            Game.PlayerPed.Weapons.RemoveAll();
            PreviousPosition = currentPosition;
            PreviousPositionFull = Game.PlayerPed.Position;
            await BaseScript.Delay(1000);
        }
    }
}
