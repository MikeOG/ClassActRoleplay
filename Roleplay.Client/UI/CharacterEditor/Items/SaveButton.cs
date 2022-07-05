using System.Collections.Generic;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player;
using Roleplay.Client.UI;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using MenuFramework;
using Newtonsoft.Json;

namespace Roleplay.Client.Menus.CharacterEditor.MainMenu
{
    internal class SaveButton : MenuItemStandard
	{
		private CharacterEditorMenu Root;

		public SaveButton( CharacterEditorMenu root ) {
			Root = root;
			Title = "Save character";
			OnActivate = SaveAndExit;
		}

		private async void SaveAndExit( MenuItemStandard m )
		{
		    var playerSession = Client.LocalSession;
		    if (playerSession == null) return;

            //Log.ToChat( "CharacterEditorMenu SaveAndExit" );
            if (!Root.IsClothingStore)
            {
                CharacterEditorMenu.skinData = Root.AdditionalSaveData.getSaveableData(Root.IsClothingStore)/*.ToExpando()*/;
                Client.Instance.TriggerServerEvent("Skin.UpdatePlayerSkin", JsonConvert.SerializeObject(CharacterEditorMenu.skinData));
                Client.Instance.TriggerServerEvent("Skin.OnFinishInitialCreation");
                BaseScript.TriggerEvent("Player.OnSkinLoaded");
                Root.cleanCloseMenu();
            }
            else
            {
                var clothingStorePrice = CitizenFX.Core.Native.API.GetConvarInt("mg_clothingStorePrice", 25);
                if (await playerSession.CanPayForItem(clothingStorePrice))
                { 
                    CharacterEditorMenu.skinData = Root.AdditionalSaveData.getSaveableData(Root.IsClothingStore)/*.ToExpando()*/;
                    Log.ToChat("[Bank]", $"You paid ${clothingStorePrice} to change your clothes", ConstantColours.Bank);
                    Client.Instance.TriggerServerEvent("Skin.UpdatePlayerSkin", JsonConvert.SerializeObject(CharacterEditorMenu.skinData));
                    BaseScript.TriggerEvent("Player.OnSkinLoaded");
                    Root.cleanCloseMenu();
                }
                else
                {
                    Log.ToChat("[Bank]", "You cannot afford this", ConstantColours.Bank);
                }
            }
        }
	}
}
