using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Server.Session;
using Roleplay.Shared;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Helpers
{
#if ONESYNC
    public static class OneSyncExtensions
    {
        public static int GetPlayerPed(this Session.Session playerSession)
        {
            return API.GetPlayerPed(playerSession.Source.Handle);
        }

        public static int GetPlayerModel(this Session.Session playerSession)
        {
            return GetEntityModel(playerSession.GetPlayerPed());
        }

        public static float GetPlayerHeading(this Session.Session playerSession)
        {
            return GetEntityHeading(playerSession.GetPlayerPed());
        }

        public static Vector3 GetPlayerPosition(this Session.Session playerSession)
        {
#if ONESYNC
            return playerSession.Position; //return GetEntityCoords(playerSession.GetPlayerPed());
#else
            return playerSession.GetPosition().Result;
#endif
        }

        public static void SetPlayerPosition(this Session.Session playerSession, Vector3 newPosition)
        {
            Log.Debug($"Setting position of {playerSession.PlayerName} to {newPosition}");

            //SetEntityCoords(playerSession.GetPlayerPed(), newPosition.X, newPosition.Y, newPosition.Z, false, false, false, false);
            playerSession.TriggerEvent("Player.SetPosition", newPosition);
        }

        public static void SetPlayerHeading(this Session.Session playerSession, float heading)
        {
            Log.Debug($"Setting heading of {playerSession.PlayerName} to {heading}");

            SetEntityHeading(playerSession.GetPlayerPed(), heading);
        }

        public static void SetFreezeStatus(this Session.Session playerSession, bool status)
        {
            FreezeEntityPosition(playerSession.GetPlayerPed(), status);
        }

        public static Session.Session GetClosestPlayer(this Session.Session playerSession, float distance = 6.0f, Func<Session.Session, bool> customFindFunc = null)
        {
            Session.Session closestPlayer = null;
            var currentPlayers = Server.Instance.Get<SessionManager>().PlayerList;
            var playerPos = playerSession.GetPlayerPosition();

            foreach (var player in currentPlayers)
            {
                try
                {
                    var otherPos = player.GetPlayerPosition();
                    if (playerPos.DistanceToSquared(otherPos) <= distance && playerSession != player && (customFindFunc != null && customFindFunc(player) || customFindFunc == null))
                    {
                        closestPlayer = player;
                    }
                }
                catch (Exception e)
                {
                    // probably something dumb causes the pos code to break
                }
            }

            return closestPlayer;
        }
    }
#endif
}
