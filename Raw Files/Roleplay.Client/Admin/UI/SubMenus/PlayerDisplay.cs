using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Admin.Interfaces;
using MenuFramework;

namespace Roleplay.Client.Admin.UI.SubMenus
{
    internal class PlayerDisplay : MenuModel, IAdminMenu
    {
        public PlayerDisplay()
        {
            headerTitle = "Online players";
        }

        public override void Refresh()
        {
            List<MenuItem> _menuItems = new List<MenuItem>();
            var players = Client.Instance.PlayerList;

            foreach (var player in players)
            {
                //if (player.Name == Game.Player.Name) continue;

                _menuItems.Add(new PlayerSubMenu(player).GetSubMenu());
            }

            if (_menuItems.Count == 0)
            {
                _menuItems.Add(new MenuItemStandard
                {
                    Title = "No online players"
                });
            }

            menuItems = _menuItems;
            SelectedIndex = SelectedIndex;
        }

        public MenuItem GetSubMenu()
        {
            return new MenuItemSubMenu
            {
                Title = "Online players",
                SubMenu = this
            };
        }
    }
}
