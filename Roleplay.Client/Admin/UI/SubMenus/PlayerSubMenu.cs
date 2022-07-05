using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Admin.Interfaces;
using Roleplay.Client.Helpers;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared;
using MenuFramework;
using SessionManager = Roleplay.Client.Session.SessionManager;

namespace Roleplay.Client.Admin.UI.SubMenus
{
    internal class PlayerSubMenu : MenuModel, IAdminMenu
    {
        private CitizenFX.Core.Player _player;
        private bool isSpectating = false;

        public PlayerSubMenu(CitizenFX.Core.Player player)
        {
            _player = player;
            headerTitle = $"{_player.Name} settings";
            menuItems.Add(getPlayerData());
            menuItems.Add(new MenuItemStandard
            {
                Title = "Spectate player",
                OnActivate = state =>
                {
                    spectatePlayer();
                }
            });
            menuItems.Add(new MenuItemStandard
            {
                Title = "Goto player",
                OnActivate = state =>
                {
                    gotoPlayer();
                }
            });
            menuItems.Add(new MenuItemStandard
            {
                Title = "Bring player",
                OnActivate = state =>
                {
                    ExecuteCommand($"bring {_player.ServerId}");
                }
            });
            menuItems.Add(new MenuItemStandard
            {
                Title = "Revive player",
                OnActivate = state =>
                {
                    ExecuteCommand($"arevive {_player.ServerId}");
                }
            });
            menuItems.Add(new MenuItemStandard
            {
                Title = "Kick player",
                OnActivate = async state =>
                {
                    InteractionUI.Observer.CloseMenu(true);
                    ExecuteCommand($"kick {_player.ServerId} {await Game.GetUserInput(9999)}");
                    InteractionUI.Observer.OpenMenu(this);
                }
            });
        }

        private async void spectatePlayer()
        {
            bool spectatePlayer = !isSpectating;
            CitizenFX.Core.UI.Screen.Fading.FadeOut(1500);
            while (CitizenFX.Core.UI.Screen.Fading.IsFadingOut)
                await BaseScript.Delay(0);

            if (spectatePlayer)
            {
                Vector3 targetCoords = _player.Character.Position;
                RequestCollisionAtCoord(targetCoords.X, targetCoords.Y, targetCoords.Z);
                NetworkSetInSpectatorMode(true, _player.Character.Handle);
                isSpectating = true;
                Log.ToChat($"Spectating {_player.Name}");
            }
            else
            {
                Vector3 targetCoords = Game.PlayerPed.Position;
                RequestCollisionAtCoord(targetCoords.X, targetCoords.Y, targetCoords.Z);
                NetworkSetInSpectatorMode(false, Game.PlayerPed.Handle);
                isSpectating = false;
                Log.ToChat($"Stopped spectating");
            }

            CitizenFX.Core.UI.Screen.Fading.FadeIn(1500);
        }

        private async void gotoPlayer()
        {
            Vector3 targetCoords = _player.Character.Position;

#pragma warning disable 4014
            Game.PlayerPed.TeleportToLocation(targetCoords);
#pragma warning restore 4014
        }

        private MenuItem getPlayerData()
        {
            var userData = new List<MenuItem>
            {
                new MenuItemStandard{Title = "No data"}
            };

            var playerSession = Client.Instance.Get<SessionManager>().GetPlayer(_player);

            if (playerSession != null)
            {
                userData.Clear();

                foreach (var kvp in playerSession.GlobalData)
                {
                    userData.Add(new MenuItemStandard
                    {
                        Title = kvp.Key.Replace("Character.", ""),
                        Detail = kvp.Key == "Character.Inventory" ? "Soon(tm)" : kvp.Value.ToString()
                    });
                }
            }

            return new MenuItemSubMenu
            {
                Title = "Player data",
                SubMenu = new MenuModel
                {
                    headerTitle = $"{_player.Name} data",
                    menuItems = userData
                }
            };
        }

        public MenuItem GetSubMenu()
        {
            return new MenuItemSubMenu
            {
                Title = _player.Name,
                SubMenu = this
            };
        }
    }
}
