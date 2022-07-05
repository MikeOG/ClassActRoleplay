using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Enums;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.UI.Classes;

namespace Roleplay.Client.Player
{
    public class CinematicMode : ClientAccessor
    {
        public static bool InCinematicMode = false;

        private int blackBarHeight = 0;
        private Dictionary<int, List<Rect>> blackBars = new Dictionary<int, List<Rect>>
        {
            {1, new List<Rect>
                {
                    new Rect(0, 0, 1920, 94),
                    new Rect(0, 987, 1920, 94)
                }
            },
            {2, new List<Rect>
                {
                    new Rect(0, 0, 1920, 141),
                    new Rect(0, 940, 1920, 141)
                }
            },
            {3, new List<Rect>
                {
                    new Rect(0, 0, 1920, 188),
                    new Rect(0, 893, 1920, 188)
                }
            },
        };
        private List<HudComponent> disabledHudComponents = new List<HudComponent>
        {
            HudComponent.Reticle,
            HudComponent.MpVehicle,
            HudComponent.VehicleName,
            HudComponent.StreetName,
            HudComponent.AreaName
        };

        public CinematicMode(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
            client.RegisterTickHandler(DisableHUDTick);
            client.RegisterTickHandler(DrawBlackBars);
        }

        private async Task OnTick()
        {
            if (ControlHelper.IsControlJustPressed((Control)183, true, ControlModifier.None))
            //if (Input.IsControlJustPressed(Control.183))
            {
                if (!InCinematicMode)
                {
                    InCinematicMode = true;
                    MarkerHandler.ShowMarkers = false;
                }
                else
                {
                    MarkerHandler.ShowMarkers = true;
                    InCinematicMode = false;
                }
            }

            if (Input.IsControlJustPressed(Control.MultiplayerInfo, true, ControlModifier.Shift))
            {
                blackBarHeight++;

                if (blackBarHeight > 3)
                    blackBarHeight = 0;
            }

            disabledHudComponents.ForEach(o =>
            {
                if (o == HudComponent.Reticle)
                {
                    if (!IsAimCamActive() || !IsFirstPersonAimCamActive())
                    {
                        Screen.Hud.HideComponentThisFrame(o);
                    }

                    return;
                }

                Screen.Hud.HideComponentThisFrame(o);
            });
        }

        private async Task DisableHUDTick()
        {
            if(!InCinematicMode) return;

            Enum.GetValues(typeof(HudComponent)).Cast<HudComponent>().ToList().ForEach(Screen.Hud.HideComponentThisFrame);
        }

        private async Task DrawBlackBars()
        {
            if(blackBarHeight == 0) return;

            if (blackBars.ContainsKey(blackBarHeight))
            {
                blackBars[blackBarHeight].ForEach(o => o.DrawTick());
            }
        }
    }
}
