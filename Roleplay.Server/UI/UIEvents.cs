using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.UI
{
    public class UIEvents : ServerAccessor
    {
        private Dictionary<string, Tuple<int, int>> interactionMenuLocations = new Dictionary<string, Tuple<int, int>>
        {
            {"topleft", new Tuple<int, int>(13, 13) },
            {"topright", new Tuple<int, int>(1010, 13) },
            {"bottomright", new Tuple<int, int>(1010, 350) }
        }; 

        public UIEvents(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("setmenulocation", async cmd =>
            {
                var menuLocationName = cmd.GetArgAs(0, "topright");

                if (interactionMenuLocations.ContainsKey(menuLocationName))
                {
                    var playerSettings = cmd.Session./*GetPlayerSettings()*/PlayerSettings;
                    var menuLocation = interactionMenuLocations[menuLocationName];

                    playerSettings["Interaction.Menu.X"] = menuLocation.Item1;
                    playerSettings["Interaction.Menu.Y"] = menuLocation.Item2;
                    cmd.Session./*SetPlayerSettings()*/PlayerSettings = playerSettings;

                    cmd.Session.TriggerEvent("UI.UpdateInteractionMenuLocation", playerSettings["Interaction.Menu.X"], playerSettings["Interaction.Menu.Y"]);
                    Log.ToClient("[Info]", $"Set interacion menu location to {menuLocationName}", ConstantColours.Info, cmd.Player);
                }
            });
        }

        public void NotOnCharacterLoaded(Session.Session playerSession)
        {
            var playerSettings = playerSession./*GetPlayerSettings()*/PlayerSettings;

            if (!playerSettings.ContainsKey("Interaction.Menu.X") || !playerSettings.ContainsKey("Interaction.Menu.Y"))
            {
                playerSettings["Interaction.Menu.X"] = 1010;
                playerSettings["Interaction.Menu.Y"] = 13;
                playerSession./*SetPlayerSettings()*/PlayerSettings = playerSettings;
            }
            
            playerSession.TriggerEvent("UI.UpdateInteractionMenuLocation", playerSettings["Interaction.Menu.X"], playerSettings["Interaction.Menu.Y"]);
        }
    }
}
