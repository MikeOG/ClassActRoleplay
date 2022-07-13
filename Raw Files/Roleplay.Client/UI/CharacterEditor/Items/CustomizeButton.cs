using System.Collections.Generic;
using Roleplay.Client.UI;
using Roleplay.Client.UI.Menus.CharacterEditor.MainMenu;
using MenuFramework;

namespace Roleplay.Client.Menus.CharacterEditor.MainMenu
{
    internal class CustomizeButton : MenuItemSubMenu
	{
		public CustomizeButton( CharacterEditorMenu root, bool isClothingStore ) {
			Title = "Customize";
			SubMenu = new CustomizeMenu( root, isClothingStore );
		}

		public override void Refresh() {
			base.Refresh();
			SubMenu.Refresh();
		}

		public override void OnTick( long frameCount, int frameTime, long gameTimer ) {
			base.OnTick( frameCount, frameTime, gameTimer );
			SubMenu.OnTick( frameCount, frameTime, gameTimer );
		}
	}
}
