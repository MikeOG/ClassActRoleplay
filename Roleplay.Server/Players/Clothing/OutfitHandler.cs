using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Server.Players.Clothing
{
    public class OutfitHandler : ServerAccessor
    {
        public OutfitHandler(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("saveoutfit", cmd =>
            {
                var outfitName = string.Join(" ", cmd.Args);
                var playerSession = cmd.Session;
                var playerSettings = playerSession.GetGlobalData("Character.Settings", "") == "" ? new Dictionary<string, dynamic>() : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(playerSession.GetGlobalData("Character.Settings", ""));
                var currentOutfits = GetPlayerOutfits(playerSession);

                if (!currentOutfits.ContainsKey(outfitName))
                {
                    currentOutfits[outfitName] = playerSession.GetGlobalData<string>("Character.SkinData");
                    playerSettings["Outfits"] = currentOutfits;

                    playerSession.SetGlobalData("Character.Settings", JsonConvert.SerializeObject(playerSettings));
                }
                else
                {
                    Log.ToClient("[Info]", $"An outfit with the name ^2{outfitName}^0 already exists", ConstantColours.Info, cmd.Player);
                }
            });

            CommandRegister.RegisterCommand("removeoutfit|deleteoutfit|deloutfit", cmd =>
            {
                var playerSession = cmd.Session;
                var outfitName = string.Join(" ", cmd.Args);
                var playerSettings = playerSession.GetGlobalData("Character.Settings", "") == "" ? new Dictionary<string, dynamic>() : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(playerSession.GetGlobalData("Character.Settings", ""));
                var currentOutfits = GetPlayerOutfits(cmd.Session);

                if (currentOutfits.ContainsKey(outfitName))
                {
                    currentOutfits.Remove(outfitName);
                    playerSettings["Outfits"] = currentOutfits;

                    playerSession.SetGlobalData("Character.Settings", JsonConvert.SerializeObject(playerSettings));
                    Log.ToClient("[Info]", $"Deleted outfit ^2{outfitName}^0", ConstantColours.Info, cmd.Player);
                }
            });

            Server.RegisterEventHandler("Outfit.AttemptChange", new Action<Player, string>(OnChangeOutfit));
        }

        public Dictionary<string, string> GetPlayerOutfits(Session.Session playerSession)
        {
            var settingsString = playerSession.GetGlobalData("Character.Settings", "");
            var playerSettings = settingsString == "" ? new Dictionary<string, dynamic>() : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(settingsString);
            var currentOutfits = playerSettings.ContainsKey("Outfits") ? ((JObject)playerSettings["Outfits"]).ToObject<Dictionary<string, string>>() : new Dictionary<string, string>();

            return currentOutfits;
        }

        private void OnChangeOutfit([FromSource] Player source, string outfitName)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            var currentOutfits = GetPlayerOutfits(playerSession);

            if (currentOutfits.ContainsKey(outfitName))
            {
                playerSession.SetGlobalData("Character.SkinData", currentOutfits[outfitName]);
                playerSession.TriggerEvent("Skin.RefreshSkin");
            }
        }
    }
}