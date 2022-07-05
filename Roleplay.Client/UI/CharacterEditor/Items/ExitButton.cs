using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.UI;
using Roleplay.Shared;
using MenuFramework;
using Newtonsoft.Json;

namespace Roleplay.Client.Menus.CharacterEditor.MainMenu
{
    internal class ExitButton : MenuItemStandard
	{
		private CharacterEditorMenu Root;
		private MenuModel Prompt;

		public ExitButton( CharacterEditorMenu root ) {
			Root = root;

			Title = "Leave without saving";
			OnActivate = OpenExitPrompt;

			Prompt = new MenuModel { headerTitle = "Leave without saving?", statusTitle = "All changes will be lost." };
			Prompt.menuItems.Add( new MenuItemStandard { Title = "Yes", OnActivate = ExitWithoutSaving } );
			Prompt.menuItems.Add( new MenuItemStandard { Title = "No", OnActivate = AbortExit } );
		}

		public override void OnTick( long frameCount, int frameTime, long gameTimer ) {
			base.OnTick( frameCount, frameTime, gameTimer );

			if( Root.Observer.CurrentMenu == Prompt ) {
				if( Game.IsDisabledControlJustReleased( 0, Control.FrontendCancel ) ) {
					AbortExit( this );
				}
			}
		}

		private void OpenExitPrompt( MenuItemStandard m ) {
			Root.Observer.OpenMenu( Prompt );
		}

		private void ExitWithoutSaving( MenuItemStandard m ) {
			//Log.ToChat( "CharacterEditorMenu ExitWithoutSaving" );
            Root.cleanCloseMenu();
		    var pedData = JsonConvert.DeserializeObject<PedData>(Client.Instance.Instances.Session.GetPlayer(Game.Player).GetGlobalData("Character.SkinData", ""));
		    CharacterEditorMenu.handleSkinCreate(pedData);
        }

		private void AbortExit( MenuItemStandard m ) {
			Root.Observer.CloseMenu();
		}
	}
}
