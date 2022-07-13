using System.Collections.Generic;
using CitizenFX.Core;
using Roleplay.Client.UI.Menus.CharacterEditor.CustomizeMenu;
using MenuFramework;

namespace Roleplay.Client.UI.Menus.CharacterEditor.MainMenu
{
    internal class CustomizeMenu : MenuModel
	{
		private CharacterEditorMenu Root;
        private bool isClothingStore = false;

		public CustomizeMenu( CharacterEditorMenu root, bool isClothingStore ) {
			Root = root;
            this.isClothingStore = isClothingStore;

			headerTitle = "Customizable";
			statusTitle = "Customizable your model";

			PopulateMenu();
		}

		public override void Refresh() {
			base.Refresh();
			PopulateMenu();
		}

		public override void OnTick( long frameCount, int frameTime, long gameTimer ) {
			base.OnTick( frameCount, frameTime, gameTimer );

			foreach( MenuItem item in menuItems ) {
				item.OnTick( frameCount, frameTime, gameTimer );
			}

			if( Root.Observer.CurrentMenu == this ) {
				if( Game.IsDisabledControlJustReleased( 0, Control.FrontendCancel ) ) {
					CloseCustomizeMenu( null );
				}

				if( SelectedIndex > menuItems.Count - 1 )
					SelectedIndex = menuItems.Count - 1;
			}
		}

		private void PopulateMenu() {
			List<MenuItem> menuItemsBuffer = new List<MenuItem>();

			if( (PedHash)Game.PlayerPed.Model.Hash == PedHash.FreemodeMale01 || (PedHash)Game.PlayerPed.Model.Hash == PedHash.FreemodeFemale01 ) {
				if(!isClothingStore) menuItemsBuffer.Add( new MPBodyButton( Root ) );
				menuItemsBuffer.Add( new MPHairButton( Root ) );
                if (!isClothingStore) menuItemsBuffer.Add( new MPEyeSelector( Root ) );
				menuItemsBuffer.Add( new MPOverlayButton( Root ) );

				IPedVariation[] variations = Game.PlayerPed.Style.GetAllVariations();

				foreach( IPedVariation variation in variations ) {
					if( variation.ToString() == "Face" || variation.ToString() == "Hair" )
						continue;

					if( variation.HasAnyVariations ) {
						menuItemsBuffer.Add( new ModelTextureSelector( Root, variation ) );
					}
				}
			}
			else {
				IPedVariation[] variations = Game.PlayerPed.Style.GetAllVariations();

				foreach( IPedVariation variation in variations ) {
					if( variation.HasAnyVariations ) {
						menuItemsBuffer.Add( new ModelTextureSelector( Root, variation ) );
					}
				}
			}

			menuItemsBuffer.Add( new MenuItemStandard { Title = "Back", OnActivate = CloseCustomizeMenu } );

			menuItems = menuItemsBuffer;
			SelectedIndex = 0;
		}

		private void CloseCustomizeMenu( MenuItemStandard m ) {
			if( Root.Observer.CurrentMenu == this ) {
				Root.Observer.CloseMenu();
			}
		}
	}
}
