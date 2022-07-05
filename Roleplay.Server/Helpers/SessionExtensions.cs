using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Server.Helpers
{
    public static class SessionExtensions
    {
        static SessionExtensions()
        {
            Server.Instance.RegisterEventHandler("Session.UpdateLocation", new Action<Player, string>(OnRecieveLocation));
            //Server.Instance.RegisterEventHandler("Session.UpdatePositon", new Action<Player, List<object>>(OnRecievePosition));
        }

        public static async Task<string> GetLocation(this Session.Session playerSession)
        {
            var ticks = 0;
            playerSession.SetServerData("Player.Location", default(string));
            playerSession.TriggerEvent("Player.UpdateLocation");

            while (playerSession.GetServerData("Player.Location", default(string)) == default && ticks < 600)
            {
                await BaseScript.Delay(0);
                ticks++;
            }

            if (ticks >= 600) return "";

            return playerSession.GetServerData("Player.Location", "");
        }

        private static void OnRecieveLocation([FromSource] Player source, string location)
        {
            var playerSession = Server.Instance.Instances.Session.GetPlayer(source);

            playerSession.SetServerData("Player.Location", location);
        }

        public static string GetCharacterName(this Session.Session playerSession)
        {
            return playerSession.GetGlobalData("Character.FirstName", "") + " " + playerSession.GetGlobalData("Character.LastName", "");
        }

        public static void UpdateChatTheme(this Session.Session playerSession, string theme)
        {
            if (theme == "default" || theme == "gtao")
            {
                var playerSettings = playerSession.PlayerSettings;
                playerSettings["ChatTheme"] = theme;
                playerSession.PlayerSettings = playerSettings;
                playerSession.RefreshChatTheme();

                Log.ToClient("[Info]", $"Updated chat theme to {theme}", ConstantColours.Info, playerSession.Source);
            }
        }

        public static void RefreshChatTheme(this Session.Session playerSession)
        {
            var playerSettings = playerSession.PlayerSettings;
            var theme = playerSettings.ContainsKey("ChatTheme") ? playerSettings["ChatTheme"] : "default";

            playerSession.TriggerEvent($"chat:{(theme == "default" ? "clearThemes" : "reloadThemes")}");         
        }

        public static BankAccountModel GetBankAccount(this Session.Session playerSession, string bankId = "")
        {
            if (bankId != "")
            {

                return null;
            }
            else
            {
                return Server.Instance.Get<BankHandler>().GetBank(playerSession.CharId);
            }
        }

        public static bool IsInVehicle(this Session.Session playerSession)
        {
            return playerSession.GetServerData("Character.IsInVehicle", false);
        }

        public static Dictionary<string, string> GetEmoteBinds(this Session.Session playerSession)
        {
            var characterSettings = playerSession.CharacterSettings;

            if (characterSettings.ContainsKey("EmoteBinds"))
            {
                if (characterSettings["EmoteBinds"].GetType() == typeof(JObject))
                    characterSettings["EmoteBinds"] = ((JObject)characterSettings["EmoteBinds"]).ToObject<Dictionary<string, string>>();
            }
            else
            {
                characterSettings["EmoteBinds"] = new Dictionary<string, string>();
            }

            return characterSettings["EmoteBinds"];
        }

        public static void AddBlip(this Session.Session playerSession, string blipName, Vector3 blipPosition, BlipOptions blipOptions = null)
        {
            playerSession.TriggerEvent("Blips.AddBlip", blipName, blipPosition.ToArray(), JsonConvert.SerializeObject(blipOptions));
        }

        public static void AddBlip(this Session.Session playerSession, string blipName, List<Vector3> blipPositions, BlipOptions blipOptions = null)
        {
            playerSession.TriggerEvent("Blips.AddBlips", blipName, blipPositions.Select(o => o.ToArray()).ToArray(), JsonConvert.SerializeObject(blipOptions));
        }

        public static void AddMarker(this Session.Session playerSession, Vector3 markerPosition, MarkerOptions markerOptions = null)
        {
            playerSession.TriggerEvent("Markers.AddMarker", markerPosition.ToArray(), JsonConvert.SerializeObject(markerOptions));
        }

        public static void AddMarker(this Session.Session playerSession, List<Vector3> markerPosition, MarkerOptions markerOptions = null)
        {
            playerSession.TriggerEvent("Markers.AddMarkers", markerPosition.Select(o => o.ToArray()).ToArray(), JsonConvert.SerializeObject(markerOptions));
        }

        public static void RemoveMarker(this Session.Session playerSession, string markerId)
        {
            playerSession.TriggerEvent("Markers.RemoveMarker", markerId);
        }

        public static void RemoveMarkers(this Session.Session playerSession, List<string> markerIds) => markerIds.ForEach(playerSession.RemoveMarker);
    }
}
