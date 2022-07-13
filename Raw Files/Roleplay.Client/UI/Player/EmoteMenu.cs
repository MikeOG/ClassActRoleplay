using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Emotes;
using MenuFramework;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Client.Player;
using Roleplay.Shared.Enums;
using Roleplay.Shared;


namespace Roleplay.Client.UI
{
    internal class EmoteMenu : ClientAccessor
    {
        private static MenuModel emoteUI;
        private static List<MenuItem> _menuItems = new List<MenuItem>();
        private static MenuItem CancelMenuItem = new MenuItemStandard
        {
            Title = "Cancel Emote",
            OnActivate = item =>
            {
                Game.PlayerPed.Task.ClearAll();
                EmoteManager.IsPlayingAnim = false;
                InteractionUI.Observer.CloseMenu();
            }
        };

        private static MenuItem CancelImmediatelyMenuItem = new MenuItemStandard
        {
            Title = "Cancel Emote (Immediately)",
            OnActivate = item =>
            {
                Game.PlayerPed.Task.ClearAllImmediately();
                EmoteManager.IsPlayingAnim = false;
                InteractionUI.Observer.CloseMenu();
            }
        };

        public EmoteMenu(Client client) : base(client)
        {
            try
            {
                var cancelEmoteMenu = new MenuModel { headerTitle = "Emotes", menuItems = new List<MenuItem>() { CancelMenuItem, CancelImmediatelyMenuItem } };
                emoteUI = new MenuModel
                {
                    headerTitle = "Emotes list",
                    statusTitle = ""
                };
                foreach (var i in EmoteManager.playerAnimations.OrderBy(o => o.Value.DisplayName))
                {
                    _menuItems.Add(new MenuItemStandard
                    {
                        Title = i.Value.DisplayName,
                        OnActivate = async state =>
                        { 
                            var playerPed = Game.PlayerPed;
                            if (!playerPed.IsDoingAction())
                            {
                                playerPed.Task.ClearAll();
                                await BaseScript.Delay(0);
                                await i.Value.PlayFullAnim();
                                InteractionUI.Observer.OpenMenu(cancelEmoteMenu);
                            }
                        }
                    });
                }
                emoteUI.menuItems = _menuItems;
                client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
                {
                    Title = "Emotes",
                    SubMenu = emoteUI
                }, () => true, 101);
                client.Get<InteractionUI>().RegisterInteractionMenuItem(CancelMenuItem, () => EmoteManager.IsPlayingAnim, 100);
                client.Get<InteractionUI>().RegisterInteractionMenuItem(CancelImmediatelyMenuItem, () => EmoteManager.IsPlayingAnim, 100);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
