using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using MenuFramework;

namespace Roleplay.Client.UI.Menus.CharacterEditor.CustomizeMenu
{
    internal class MPHairButton : MenuItemSubMenu
	{
		public MPHairButton( CharacterEditorMenu root ) {
			Title = "Hair";
			SubMenu = new MPHairMenu( root );
			OnSelect = new Action<MenuItem>( (menuItem) => { root.Editor.lookAtHead(); } );
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