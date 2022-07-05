using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using Roleplay.Client.UI;
using Roleplay.Shared;
using MenuFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.Player.Clothing
{
    public class Outfits : ClientAccessor
    {
        private List<Vector3> outfitChangeLocations = new List<Vector3>
        {
            new Vector3(-760.389f, 325.568f, 169.617f),
            new Vector3(-796.300f, 332.138f, 152.815f),
            new Vector3(-760.462f, 325.426f, 216.071f),
            new Vector3(-795.44f, 331.684f, 200.434f),
            new Vector3(350.932f, -994.405f, -98.157f),
            new Vector3(259.743f, -1003.448f, -97.010f),
            new Vector3(152.193f, -1001.224f, -97.01f),
            new Vector3(-167.466f, 488.013f, 132.854f),
            new Vector3(334.284f, 428.418f, 144.581f),
            new Vector3(374.44f, 411.637f, 141.11f),
            new Vector3(-671.656f, 587.511f, 140.58f),
            new Vector3(-855.374f, 680.105f, 148.063f),
            new Vector3(-571.238f, 649.873f, 141.042f),
            new Vector3(122.097f, 548.873f, 179.507f),
            new Vector3(-1286.108f, 438.176f, 93.105f),
            new Vector3(1397.875f, 1164.189f, 113.344f),
            new Vector3(429.769f, -811.333f, 28.4921f),
            new Vector3(71.235f, -1388.01f, 28.3771f),
            new Vector3(820.303f, -1067.66f, 10.338f),
            new Vector3(-704.855f, 151.566f, 36.4161f),
            new Vector3(118.626f, -225.241f, 53.5589f),
            new Vector3(-167.854f, -300.037f, 38.7343f),
            new Vector3(454.398f, -989.123f, 29.6906f),
            new Vector3(1201.47f, 2714.43f, 37.23f),
            new Vector3(268.789f, -1363.74f, 23.5388f),
            new Vector3(-3177.79f, 1043.11f, 19.8642f),
            new Vector3(4.11955f, 6506.05f, 30.8788f),
            new Vector3(-841.86f, 5403.24f, 33.6162f),
            new Vector3(1862.12f, 3666.94f, -119.86f),
            new Vector3(1704.03f, -1131.6f, 12.1623f),
            new Vector3(-1449.69f, -548.96f, 71.84f),
            new Vector3(-909.91f, -445.63f, 114.41f),
            new Vector3(-594.68f, 56.18f, 96.0f),
            new Vector3(-797.94f, 328.38f, 219.44f),
            new Vector3(-793.22f, 329.40f, 198.49f),
            new Vector3(-797.72f, 328.38f, 189.72f),
            new Vector3(-1150.63f, -1513.26f, 9.63f),
            new Vector3(-18.5f, -1438.74f, 30.1f),
            new Vector3(-1009.88f, -3166.6f, -37.89f),
            new Vector3(1122.14f, -3162.53f, -35.87f),
            new Vector3(1851.64f, 3690.7f, 33.27f),
            new Vector3(1698.63f, 4818.49f, 41.06f),
            new Vector3(620.4f, 2766.83f, 41.09f),
            new Vector3(-1100.47f, 2717.03f, 18.11f),
            new Vector3(-1520.64f, 849.14f, 180.69f),
            new Vector3(-568.82f, -114.54f, 33.88f),
            new Vector3(-449.41f, 6011.76f, 31.72f),

            //Pillbox
            new Vector3(301.51f, -599.19f, 42.29f),
            //Interior 21 Clothing
            new Vector3(-763.45f, 329.08f, 199.49f),
            //Interior 15 Clothing
            new Vector3(-767.55f, 610.91f, 140.33f),
            //Clothing store 12
            new Vector3(-1186.23f, -770.89f, 17.33f),
            //Clothing Store 2
            new Vector3(-1447.97f, -241.3f, 49.82f),
            //Clothing Store 3
            new Vector3(-820.15f, -1067.54f, 11.33f),
            //Clothing Store 9
            new Vector3(-705.1f, -152.25f, 37.42f),
            //Michaels House clothing point
            new Vector3(-811.46f, 175.3f, 76.75f),
            //VU Clothing
            new Vector3(104.64f, -1303.37f, 28.77f),
            //Clubs clothing
            new Vector3(-1619.45f, -3020.65f, -75.21f),
            //Office Clothing points
            new Vector3(-1566f, -570.85f, 108.52f),
            new Vector3(-78.4f, -812.76f, 243.39f),
            //New interior Clothing
            new Vector3(-1415.29f, -2638.64f, -91.99f),
        };

        private MenuModel outfitMenu;

        public Outfits(Client client) : base(client)
        {
            outfitMenu = new MenuModel
            {
                headerTitle = "Outfits",
                menuItems = new List<MenuItem> { new MenuItemStandard { Title = "You have no outfits" } }
            };

            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                Title = "Outfits",
                SubMenu = outfitMenu,
                OnActivate = updateOutfitList
            }, InRangeOfOutfitChange, 550);

            CommandRegister.RegisterCommand("useoutfit", cmd =>
            {
                var outfitName = string.Join(" ", cmd.Args);
                if(InRangeOfOutfitChange())
                    Client.TriggerServerEvent("Outfit.AttemptChange", outfitName);
            });

            MarkerHandler.AddMarkerAsync(outfitChangeLocations);
        }

        public Dictionary<string, string> GetPlayerOutfits()
        {
            var playerSession = LocalSession;
            var settingsString = playerSession.GetGlobalData("Character.Settings", "");
            var playerSettings = settingsString == "" ? new Dictionary<string, dynamic>() : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(settingsString);
            var currentOutfits = playerSettings.ContainsKey("Outfits") ? ((JObject)playerSettings["Outfits"]).ToObject<Dictionary<string, string>>() : new Dictionary<string, string>();

            return currentOutfits;
        }

        public bool InRangeOfOutfitChange()
        {
            var playerPos = Cache.PlayerPed.Position;
            return outfitChangeLocations.Any(o => o.DistanceToSquared(playerPos) < 20.0f);
        }

        private async void updateOutfitList(MenuItemSubMenu menu)
        {
            await LocalSession.UpdateData("Character.Settings");

            menu.SubMenu.menuItems = new List<MenuItem>{ new MenuItemStandard { Title = "You have no outfits" } };

            var playerOutifts = GetPlayerOutfits();

            if (playerOutifts.Count > 0)
            {
                menu.SubMenu.menuItems = new List<MenuItem>();
                foreach (var kvp in playerOutifts)
                {
                    menu.SubMenu.menuItems.Add(new MenuItemStandard
                    {
                        Title = kvp.Key,
                        OnActivate = item =>
                        {
                            CitizenFX.Core.Native.API.ExecuteCommand($"useoutfit {kvp.Key}");
                        }
                    });
                }
            }

            menu.SubMenu.SelectedIndex = menu.SubMenu.SelectedIndex;
        }
    }
}
