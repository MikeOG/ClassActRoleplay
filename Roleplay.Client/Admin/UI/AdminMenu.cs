using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Roleplay.Client;
using Roleplay.Client.Admin.UI.SubMenus;
using Roleplay.Client.UI;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using MenuFramework;

namespace Roleplay.Client.Admin.UI
{
    internal class AdminMenu : ClientAccessor
    {
        //TODO create sub menu for individual players with different options

        private AdminOptions adminOptions;
        private bool canUseMenu = false;

        public AdminMenu(Client client) : base(client)
        {
            adminOptions = new AdminOptions();
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu
            {
                Title = "[ADMIN] Admin menu",
                SubMenu = adminOptions
            }, () => canUseMenu, 1010);

            client.RegisterEventHandler("Player.OnLoginComplete", new Action(() =>
            {
                canUseMenu = LocalSession.GetLocalData("User.PermissionLevel", 0) > 0;
            }));
            client.RegisterEventHandler("Admin.GoToTarget", new Action<int>(async target =>
            {
                CitizenFX.Core.Player targetPlayer = new CitizenFX.Core.Player(GetPlayerFromServerId(target));
                Vector3 targetCoords = targetPlayer.Character.Position;

                await Game.PlayerPed.TeleportToLocation(targetCoords);
            }));

            client.RegisterTickHandler(CheckForMenu);
        }

        private async Task CheckForMenu()
        {
            await BaseScript.Delay(10000);

            canUseMenu = LocalSession.GetLocalData("User.PermissionLevel", 0) > 0;
        }
    }

    internal class AdminOptions : MenuModel
    {
        private PlayerDisplay playersMenu = new PlayerDisplay();
        private WeaponMenu gameSettingsMenu = new WeaponMenu();
        //private DevEditMenu.gameEditMenu gameEditor;

        public AdminOptions()
        {
            Client.Instance.RegisterTickHandler(OnTick);

            headerTitle = $"Admin settings";

            menuItems.Add(playersMenu.GetSubMenu()); 
            menuItems.Add(gameSettingsMenu.GetSubMenu()); 
        }

        private async Task OnTick()
        {
            if (InteractionUI.Observer.CurrentMenu == playersMenu)
                playersMenu.Refresh();
        }
    }
}