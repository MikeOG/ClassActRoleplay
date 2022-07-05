using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Client.Models;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Player.Clothing
{
    internal class SkinHandler : ClientAccessor
    {
        private List<Vector3> clothingStoreLocaitons = new List<Vector3>()
        {
            new Vector3(75.6231f, -1392.5754f, 28.3761f),
            new Vector3(-1450.8428955078f, -236.42501831055f, 48.8f),
            new Vector3(-821.623046875f, -1073.1802978516f, 10.3f),
            new Vector3(1197.3519287109f, 2710.1650390625f, 37.23f),
            new Vector3(4.1294679641724f, 6511.8754882813f, 30.8f),
            new Vector3(-3170.5529785156f, 1044.8f, 19.8f),
            new Vector3(125.239f, -223.924f, 53.5578f),
            new Vector3(425.01f, -806.161f, 28.49f),
            new Vector3(-710.404f, -153.275f, 36.415f),
            new Vector3(-162.714f, -303.064f, 38.733f),
            new Vector3(615.03f, 2762.77f, 41.09f),
            new Vector3(-1193.52f, -768.67f, 16.31f),
            new Vector3(1693.1f, 4822.62f, 41.06f),
            new Vector3(-1101.18f, 2710.23f, 18.11f),
        };

        private bool canRefreshSkin = true;

        public SkinHandler(Client client) : base(client)
        {
            client.RegisterEventHandler("Skin.LoadPlayerSkin", new Action<dynamic>(data =>
            {
                CharacterEditorMenu.handleSkinCreate(data == null ? CharacterEditorMenu.skinData : data);
            }));
            client.RegisterEventHandler("Skin.StartCharacterCreation", new Action<int>(async playerHome =>
            {
                CharacterEditorMenu charMenu = new CharacterEditorMenu();
                while (charMenu.Observer.CurrentMenu != null)
                {
                    Client.Get<Arrest>().DisableActions();
                    Game.PlayerPed.Task.ClearAll();
                    await BaseScript.Delay(0);
                }
                //if (playerHome != 0) TriggerEvent("spawnPlayerHome", playerHome);
            }));
            client.RegisterEventHandler("Skin.UpdatePlayerSkinData", new Action<dynamic>(skinData => CharacterEditorMenu.skinData = skinData));
            client.RegisterEventHandler("Skin.SaveCurrentOutfit", new Action<string>(handleOutfitCreate));
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuFramework.MenuItemStandard
            {
                Title = "Clothing Store",
                OnActivate = state =>
                {
                    new CharacterEditorMenu(true);
                    InteractionUI.Observer.CloseMenu(true);
                }
            }, () => clothingStoreLocaitons.Any(o => o.DistanceToSquared(Game.PlayerPed.Position) < 50.0f), 500);
            BlipHandler.AddBlip("Clothing store", clothingStoreLocaitons, new BlipOptions
            {   
                Sprite = BlipSprite.Clothes
            });
            MarkerHandler.AddMarker(clothingStoreLocaitons);
            client.RegisterEventHandler("Skin.RefreshSkin", new Action(RefreshPlayerSkin));
            client.RegisterEventHandler("Player.OnLoginComplete", new Action(RefreshPlayerSkin));
            client.RegisterEventHandler("Player.CheckForInteraction", new Action(() =>
            {
                if (clothingStoreLocaitons.Any(o => o.DistanceToSquared(Game.PlayerPed.Position) < 5.0f))
                    new CharacterEditorMenu(true);
            }));
            client.RegisterEventHandler("Player.OnSkinLoaded", new Action(() => canRefreshSkin = false));
            CommandRegister.RegisterCommand("refreshskin", async cmd =>
            {
                if(!canRefreshSkin) return;
                await cmd.Session.UpdateData("Character.SkinData");
                RefreshPlayerSkin();
            });
        }

        public async void RefreshPlayerSkin()
        {
            while (LocalSession == null) await BaseScript.Delay(0);

            if (!string.IsNullOrEmpty(LocalSession.GetGlobalData("Character.SkinData", "")))
            {
                var pedData = JsonConvert.DeserializeObject<PedData>(LocalSession.GetGlobalData("Character.SkinData", ""));
                CharacterEditorMenu.handleSkinCreate(pedData);
            }
            else
            {
                //BaseScript.TriggerEvent("Skin.StartCharacterCreation");
            }
        }

        private void handleOutfitCreate(string outfitName)
        {
            Roleplay.Client.Client.Instance.TriggerServerEvent("Skin.AddOutfitToPlayer", outfitName, JsonConvert.SerializeObject(new PedData().getSaveableData(true)));
        }
    }
}
