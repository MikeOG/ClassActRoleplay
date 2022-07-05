using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.UI.Menus.CharacterEditor.MPHairMenu;
using MenuFramework;

namespace Roleplay.Client.UI.Menus.CharacterEditor.CustomizeMenu
{
    internal class MPHairMenu : MenuModel
	{
		private CharacterEditorMenu Root;

		private ItemSelector HairModel;
		private ItemSelector PrimaryHairColor;
		private ItemSelector SecondaryHaircolor;
        private List<int> hairCount = new List<int>();

		public MPHairMenu( CharacterEditorMenu root ) {
			Root = root;

			headerTitle = "Hair";
			statusTitle = "";
            for (int i = 0; i < Game.PlayerPed.Style[PedComponents.Hair].Count; i++) hairCount.Add(i);
			HairModel = new ItemSelector( this, "Hair", /*GameData.MPHairModelList.GetLength( 0 )*/Game.PlayerPed.Style[PedComponents.Hair].Count - 1 );
			PrimaryHairColor = new ItemSelector( this, "Primary hair color", 63 );
			SecondaryHaircolor = new ItemSelector( this, "Secondary hair color", 63 );

			menuItems.Add( HairModel );
			menuItems.Add( PrimaryHairColor );
			menuItems.Add( SecondaryHaircolor );
			menuItems.Add( new MenuItemStandard { Title = "Back", OnActivate = CloseMenu } );
		}

		public void SetNewAppearance() {
            int newLook = Game.PlayerPed.Style[PedComponents.Hair].IsVariationValid(Game.PlayerPed.Style[PedComponents.Hair].Index + 1, 0) ? Game.PlayerPed.Style[PedComponents.Hair].Index + 1 : 0 ;
			API.SetPedComponentVariation( Game.PlayerPed.Handle, (int)PedComponents.Hair, /*GameData.MPHairModelList[HairModel.Value]*/hairCount[HairModel.Value], 0, 0 );
			API.SetPedHairColor( Game.PlayerPed.Handle, PrimaryHairColor.Value, SecondaryHaircolor.Value );

			Root.AdditionalSaveData.PrimaryHairColor = (byte)PrimaryHairColor.Value;
			Root.AdditionalSaveData.SecondaryHairColor = (byte)SecondaryHaircolor.Value;
		}

		public override void OnTick( long frameCount, int frameTime, long gameTimer ) {
			base.OnTick( frameCount, frameTime, gameTimer );

			foreach( MenuItem item in menuItems ) {
				item.OnTick( frameCount, frameTime, gameTimer );
			}

			if( Root.Observer.CurrentMenu == this ) {
				if( Game.IsDisabledControlJustReleased( 0, Control.FrontendCancel ) ) {
					CloseMenu( null );
				}

				SelectedIndex = MathUtil.Clamp( SelectedIndex, 0, menuItems.Count - 1 );
			}
		}

		private void CloseMenu( MenuItemStandard m ) {
			if( Root.Observer.CurrentMenu == this ) {
				Root.Observer.CloseMenu();
			}
		}
	}
}
