using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.UI.Menus.CharacterEditor.MainMenu;
using Roleplay.Client.Menus.CharacterEditor.MainMenu;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player.Clothing;
using MenuFramework;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.UI
{
    internal class CharacterEditorMenu : MenuModel
	{
        public static dynamic skinData { get; set; }
        public CharacterEditorCamera Editor { get; private set; }
		public MenuObserver Observer { get; private set; }
		public PedData AdditionalSaveData { get; private set; } = new PedData();
        public bool IsClothingStore = false;

		public CharacterEditorMenu(bool isClothingStore = false) {
			headerTitle = "Character designer";
			statusTitle = "";
            IsClothingStore = isClothingStore;
		    Editor = new CharacterEditorCamera(!isClothingStore);

            //menuItems.Add( new GenderSelector( this ) );
            if (!IsClothingStore) menuItems.Add( new ModelSelector( this ) );
			menuItems.Add( new CustomizeButton( this, isClothingStore ) );
			menuItems.Add( new SaveButton( this ) );
			if(IsClothingStore) menuItems.Add( new ExitButton( this ) );

			Observer = new MenuObserver();
			Observer.OpenMenu( this );

			// Remove default ESC, Backspace & RMB behaviour
			Observer.Controller.controlBinds.Remove( Control.PhoneCancel );

		    Client.Instance.RegisterTickHandler( Update );
            Client.Instance.RegisterEventHandler("Skin.TestCreate", new Action<dynamic>(handleSkinCreate)); 

            Game.PlayerPed.IsPositionFrozen = true;

            if (skinData != null)
		    {
		        AdditionalSaveData.PrimaryHairColor = (byte)skinData.PrimaryHairColor;
		        AdditionalSaveData.SecondaryHairColor = (byte)skinData.SecondaryHairColor;
		    }
            InteractionUI.Observer.CloseMenu(true);
        }

		~CharacterEditorMenu() {
			Observer.CloseMenu( true );
		    Client.Instance.DeregisterTickHandler( Update );
		    Game.PlayerPed.IsPositionFrozen = false;
        }

		public override void Refresh() {
			base.Refresh();

			foreach( MenuItem item in menuItems ) {
				item.Refresh();
			}
		}

        public void cleanCloseMenu()
        {
            World.RenderingCamera = null;
            Observer.CloseMenu(true);
            Client.Instance.DeregisterTickHandler(Update);
            Game.PlayerPed.IsPositionFrozen = false;
        }

		private async Task Update() {
			long frameCount = Function.Call<long>( Hash.GET_FRAME_COUNT );
			int frameTime = Function.Call<int>( Hash.GET_FRAME_TIME );
			long gameTimer = Function.Call<long>( Hash.GET_GAME_TIMER );

			foreach( MenuItem item in menuItems ) {
				item.OnTick( frameCount, frameTime, gameTimer );
			}

		    if (!IsClothingStore)
		    {
		        //Cache.PlayerPed.Health = 100;
		        Cache.PlayerPed.Task.ClearAll();
            }

		    Cache.PlayerPed.IsPositionFrozen = true;
            
            Observer.Tick();
		}

        public static async void handleSkinCreate(dynamic skin)
        {
            try
            {
                dynamic skinData = ((PedData)skin).getSaveableData(keepModel: true).ToExpando();
                CharacterEditorMenu.skinData = skinData;
                PedHash playerModel = (PedHash)Convert.ToInt32(skinData.SkinModel);
                Repeat:
                if (await Game.Player.ChangeModel(new Model(playerModel)))
                {
                    var playerPed = Game.PlayerPed;
                    API.SetPedDefaultComponentVariation(playerPed.Handle);
                    if(skinData.Components.GetType() != typeof(List<object>))
                    {
                        IDictionary<string, object> skinComponents = skinData.Components;
                        await Enum.GetValues(typeof(PedComponents)).Cast<PedComponents>().ToList().ForEachAsync(async o =>
                        {
                            if (skinComponents.ContainsKey(o.ToString()))
                            {
                                dynamic skinComponentData = skinComponents[o.ToString()];
                                playerPed.Style[o].Index = Convert.ToInt32(skinComponentData.Index);
                                playerPed.Style[o].TextureIndex = Convert.ToInt32(skinComponentData.TextureIndex);
                            }

                            await BaseScript.Delay(0);
                        });
                    }
                    else
                    {
                        List<dynamic> skinCompList = skinData.Components;
                        IDictionary<string, object> skinComponents = new Dictionary<string, object>();
                        await skinCompList.ForEachAsync(async o =>
                        {
                            dynamic compData = (IDictionary<string, object>)o.Value;
                            if (!skinComponents.ContainsKey(o.Key)) 
                                skinComponents.Add(o.Key, compData);

                            await BaseScript.Delay(0);
                        });
                        await Enum.GetValues(typeof(PedComponents)).Cast<PedComponents>().ToList().ForEachAsync(async o =>
                        {
                            if (skinComponents.ContainsKey(o.ToString()))
                            {
                                dynamic skinComponentData = skinComponents[o.ToString()];
                                playerPed.Style[o].Index = Convert.ToInt32(skinComponentData.Index);
                                playerPed.Style[o].TextureIndex = Convert.ToInt32(skinComponentData.TextureIndex);
                            }

                            await BaseScript.Delay(0);
                        });
                    }

                    if (skinData.Props.GetType() != typeof(List<object>))
                    {
                        IDictionary<string, object> skinProps = skinData.Props;
                        await Enum.GetValues(typeof(PedProps)).Cast<PedProps>().ToList().ForEachAsync(async o =>
                        {
                            if (skinProps.ContainsKey(o.ToString()))
                            {
                                dynamic skinPropData = skinProps[o.ToString()];
                                playerPed.Style[o].Index = Convert.ToInt32(skinPropData.Index);
                                playerPed.Style[o].TextureIndex = Convert.ToInt32(skinPropData.TextureIndex);
                            }

                            await BaseScript.Delay(0);
                        });
                    }
                    else

                    {
                        List<dynamic> skinCompList = skinData.Props;
                        IDictionary<string, object> skinProps = new Dictionary<string, object>();
                        await skinCompList.ForEachAsync(async o =>
                        {
                            dynamic compData = (IDictionary<string, object>)o.Value;
                            if (!skinProps.ContainsKey(o.Key))
                                skinProps.Add(o.Key, compData);

                            await BaseScript.Delay(0);
                        });
                        await Enum.GetValues(typeof(PedProps)).Cast<PedProps>().ToList().ForEachAsync(async o =>
                        {
                            if (skinProps.ContainsKey(o.ToString()))
                            {
                                dynamic skinPropData = skinProps[o.ToString()];
                                playerPed.Style[o].Index = Convert.ToInt32(skinPropData.Index);
                                playerPed.Style[o].TextureIndex = Convert.ToInt32(skinPropData.TextureIndex);
                            }

                            await BaseScript.Delay(0);
                        });
                    }

                    if (playerModel == PedHash.FreemodeFemale01 || playerModel == PedHash.FreemodeMale01)
                    {
                        try
                        {
                            API.SetPedHeadBlendData(playerPed.Handle, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
                            API.SetPedHeadBlendData(playerPed.Handle, Convert.ToInt32(skinData.FatherHead),
                                Convert.ToInt32(skinData.MotherHead), 0, Convert.ToInt32(skinData.FatherSkin),
                                Convert.ToInt32(skinData.MotherSkin), 0, (float) Convert.ToDouble(skinData.HeadWeight),
                                (float) Convert.ToDouble(skinData.SkinWeight), 0.0f, false);
                            await ((IDictionary<string, object>) skinData.FacialFeatures).ToList().ForEachAsync(async o =>
                            {
                                API.SetPedFaceFeature(playerPed.Handle, Convert.ToInt32(o.Key), (float) Convert.ToDouble(o.Value));

                                await BaseScript.Delay(0);
                            });
                        }
                        catch (Exception e)
                        {
                            List<dynamic> skinCompList = skinData.FacialFeatures;
                            IDictionary<string, object> facialFeatures = new Dictionary<string, object>();
                            await skinCompList.ForEachAsync(async o =>
                            {
                                //dynamic compData = (IDictionary<string, object>) o.Value;
                                if (!facialFeatures.ContainsKey(o.Key.ToString()))
                                {
                                    facialFeatures.Add(o.Key.ToString(), o.Value);
                                }

                                await BaseScript.Delay(0);
                            });
                            await facialFeatures.ToList().ForEachAsync(async o =>
                            {
                                API.SetPedFaceFeature(playerPed.Handle, Convert.ToInt32(o.Key), (float) Convert.ToDouble(o.Value));

                                await BaseScript.Delay(0);
                            });
                        }

                        if (skinData.HeadOverlays.GetType() != typeof(List<object>))
                        {
                            IDictionary<string, object> headOverlays = (IDictionary<string, object>) skinData.HeadOverlays;
                            await headOverlays.ToList().ForEachAsync(async o =>
                            {
                                dynamic overlayData = o.Value;
                                var index = Convert.ToInt32(overlayData.ID);
                                var colourType = 0;
                                if (index == 1 || index == 2 || index == 10) colourType = 1;
                                else if (index == 5 || index == 8) colourType = 2;
                                API.SetPedHeadOverlay(playerPed.Handle, Convert.ToInt32(o.Key), Convert.ToInt32(overlayData.Variant), (float) Convert.ToDouble(overlayData.Opacity));
                                API.SetPedHeadOverlayColor(playerPed.Handle, index, colourType, Convert.ToInt32(overlayData.PrimaryColor), Convert.ToInt32(overlayData.SecondaryColor));

                                await BaseScript.Delay(0);
                            });
                        }
                        else
                        {
                            try
                            {
                                List<object> overlays = skinData.HeadOverlays;
                                await overlays.ForEachAsync(async b =>
                                {
                                    IDictionary<string, object> headOverlays = (IDictionary<string, object>) b;
                                    dynamic overlayData = headOverlays.ToList().ToDictionary(o => o.Key, o => o.Value)
                                        .ToExpando();
                                    var index = Convert.ToInt32(overlayData.Value.ID);
                                    var colourType = 0;
                                    if (index == 1 || index == 2 || index == 10)
                                        colourType = 1;
                                    else if (index == 5 || index == 8)
                                        colourType = 2;
                                    API.SetPedHeadOverlay(playerPed.Handle, index, Convert.ToInt32(overlayData.Value.Variant), (float) Convert.ToDouble(overlayData.Value.Opacity));
                                    API.SetPedHeadOverlayColor(playerPed.Handle, index, colourType, Convert.ToInt32(overlayData.Value.PrimaryColor), Convert.ToInt32(overlayData.Value.SecondaryColor));

                                    await BaseScript.Delay(0);
                                });
                            }
                            catch (Exception e)
                            {
                                //Log.Error(e, true);
                                List<dynamic> overlays = skinData.HeadOverlays;
                                await overlays.ForEachAsync(async b =>
                                {
                                    dynamic overlayData = ((IDictionary<string, object>)b.Value).ToList().ToDictionary(o => o.Key, o => o.Value)
                                        .ToExpando();
                                    var index = Convert.ToInt32(b.Key); // b is a kvp
                                    var colourType = 0;
                                    if (index == 1 || index == 2 || index == 10)
                                        colourType = 1;
                                    else if (index == 5 || index == 8)
                                        colourType = 2;
                                    API.SetPedHeadOverlay(playerPed.Handle, index, Convert.ToInt32(overlayData.Variant), (float)Convert.ToDouble(overlayData.Opacity));
                                    API.SetPedHeadOverlayColor(playerPed.Handle, index, colourType,Convert.ToInt32(overlayData.PrimaryColor), Convert.ToInt32(overlayData.SecondaryColor));

                                    await BaseScript.Delay(0);
                                });
                            }

                        }

                        var primaryHair = Convert.ToInt32(skinData.PrimaryHairColor);
                        var secondaryHair = Convert.ToInt32(skinData.SecondaryHairColor);
                        API.SetPedHairColor(playerPed.Handle, primaryHair, secondaryHair);
                        API.SetPedEyeColor(playerPed.Handle, Convert.ToInt32(skinData.EyeColor));

                        try
                        {
                            Dictionary<string, string> equippedTattoos = new Dictionary<string, string>();
                            List<dynamic> tattoos = skinData.Tattoos;
                            API.ClearPedDecorations(Game.PlayerPed.Handle);
                            await tattoos.ForEachAsync(async tattooData =>
                            {
                                //var tatData = (IDictionary<string, object>) o;
                                //dynamic tattooData =
                                    //tatData.ToList().ToDictionary(b => b.Key, b => b.Value).ToExpando();
                                API.ApplyPedOverlay(playerPed.Handle, (uint) Game.GenerateHash(tattooData.Value), (uint) Game.GenerateHash(tattooData.Key));

                                equippedTattoos.Add(tattooData.Key.ToString(), tattooData.Value.ToString());

                                await BaseScript.Delay(0);
                            });
                            Tattoos.CurrentTattoos = equippedTattoos;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            IDictionary<string, object> equippedTattoos = (IDictionary<string, object>) skinData.Tattoos;
                            API.ClearPedDecorations(playerPed.Handle);
                            await equippedTattoos.ToList().ForEachAsync(async o =>
                            {
                                dynamic tattooData = o;
                                API.ApplyPedOverlay(playerPed.Handle, (uint) Game.GenerateHash(tattooData.Value), (uint) Game.GenerateHash(tattooData.Key));

                                equippedTattoos.Add(tattooData.Key.ToString(), tattooData.Value.ToString());

                                await BaseScript.Delay(0);
                            });
                            Tattoos.CurrentTattoos = equippedTattoos.ToDictionary(o => o.Key, o => o.Value.ToString());
                        }
                    }
                    playerPed.MaxHealth = 100;
                    playerPed.Health = 100;
                    API.SetPlayerHealthRechargeMultiplier(API.PlayerId(), 0.0f);
                    BaseScript.TriggerEvent("Player.OnSkinLoaded");
                } else { goto Repeat; }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    public class PedData
    {
        public byte FatherHead = 0;
        public byte MotherHead = 0;
        public byte FatherSkin = 0;
        public byte MotherSkin = 0;
        public float HeadWeight = 0.0f;
        public float SkinWeight = 0.0f;
        public long SkinModel = 0;
        public Dictionary<int, float> FacialFeatures = new Dictionary<int, float>();
        public Dictionary<string, System.Dynamic.ExpandoObject> Components = new Dictionary<string, ExpandoObject>();
        public Dictionary<string, System.Dynamic.ExpandoObject> Props = new Dictionary<string, ExpandoObject>();
        public Dictionary<string, string> Tattoos = new Dictionary<string, string>();

        public byte PrimaryHairColor = 0;
        public byte SecondaryHairColor = 0;
        public byte EyeColor = 0;

        public Dictionary<byte, HeadOverlay> HeadOverlays = new Dictionary<byte, HeadOverlay>();
        public class HeadOverlay
        {
            public byte Variant { get; set; } = 0;
            public float Opacity { get; set; } = 0.0f;
            public byte PrimaryColor { get; set; } = 0;
            public byte SecondaryColor { get; set; } = 0;

            public HeadOverlay()
            {

            }
        }

        public Dictionary<string, dynamic> getSaveableData(bool isClothingStore = false, bool keepModel = false)
        {
            Dictionary<string, dynamic> skinItems = new Dictionary<string, dynamic>();

            dynamic skinData = new System.Dynamic.ExpandoObject();
            if (CharacterEditorMenu.skinData != null)
            {
                skinData = CharacterEditorMenu.skinData.GetType() == typeof(ExpandoObject) ? CharacterEditorMenu.skinData : ((IDictionary<string, object>)CharacterEditorMenu.skinData).ToExpando();
            }

            skinItems["FatherHead"] = isClothingStore ? skinData.FatherHead : FatherHead;
            skinItems["MotherHead"] = isClothingStore ? skinData.MotherHead : MotherHead;
            skinItems["FatherSkin"] = isClothingStore ? skinData.FatherSkin : FatherSkin;
            skinItems["MotherSkin"] = isClothingStore ? skinData.MotherSkin : MotherSkin;
            skinItems["HeadWeight"] = isClothingStore ? skinData.HeadWeight : HeadWeight;
            skinItems["SkinWeight"] = isClothingStore ? skinData.SkinWeight : SkinWeight;
            skinItems["FacialFeatures"] = /*isClothingStore ? skinData.FacialFeatures :*/ FacialFeatures;
            /*skinItems["PrimaryHairColor"] = isClothingStore ? CharacterEditorMenu.skinData.PrimaryHairColor : PrimaryHairColor;
            skinItems["SecondaryHairColor"] = isClothingStore ? CharacterEditorMenu.skinData.SecondaryHairColor : SecondaryHairColor;*/
            skinItems["EyeColor"] = isClothingStore ? skinData.EyeColor : EyeColor;

            skinItems["PrimaryHairColor"] = PrimaryHairColor;
            skinItems["SecondaryHairColor"] = SecondaryHairColor;

            var headOverlayForServer = new Dictionary<byte, dynamic>();
            HeadOverlays.ToList().ForEach(o =>
            {
                headOverlayForServer.Add(o.Key, new Dictionary<string, dynamic>()
                {
                    ["ID"] = o.Key,
                    ["Variant"] = o.Value.Variant,
                    ["Opacity"] = o.Value.Opacity,
                    ["PrimaryColor"] = o.Value.PrimaryColor,
                    ["SecondaryColor"] = o.Value.SecondaryColor
                });
            });

            if(isClothingStore)
            {
                try
                {
                    if (skinData.HeadOverlays.GetType() != typeof(List<object>))
                    {
                        ((IDictionary<string, object>) skinData.HeadOverlays).ToList().ForEach(b =>
                        {
                            var hasItem = headOverlayForServer.ContainsKey((byte) Convert.ToInt32(b.Key));
                            if (!hasItem)
                            {
                                dynamic o = (dynamic) b;
                                headOverlayForServer.Add((byte) Convert.ToInt32(o.Key),
                                    new Dictionary<string, dynamic>()
                                    {
                                        ["ID"] = o.Key,
                                        ["Variant"] = o.Value.Variant,
                                        ["Opacity"] = o.Value.Opacity,
                                        ["PrimaryColor"] = o.Value.PrimaryColor,
                                        ["SecondaryColor"] = o.Value.SecondaryColor
                                    });
                            }
                        });
                    }
                    else
                    {
                        List<dynamic> overlays = skinData.HeadOverlays;
                        overlays.ForEach(b =>
                        {
                            try
                            {
                                IDictionary<string, object> headOverlays;
                                if (b.GetType() == typeof(Dictionary<byte, object>))
                                {
                                    Dictionary<byte, object> overlayDict = (Dictionary<byte, object>)b;

                                    headOverlays = overlayDict.ToDictionary(o => o.Key.ToString(), o => o.Value);
                                }
                                else if (b.GetType() == typeof(KeyValuePair<byte, object>))
                                {
                                    headOverlays = new Dictionary<string, object>
                                    {
                                        [b.Key.ToString()] = b.Value
                                    };
                                }
                                else
                                {
                                    headOverlays = (IDictionary<string, object>)b;
                                }

                                dynamic overlayData = headOverlays.ToList().ToDictionary(o => o.Key, o => o.Value).ToExpando();
                                foreach (dynamic kvp in (IDictionary<string, object>)overlayData)
                                {
                                    /*Log.ToServer($"{kvp.Key}: {kvp.Value}");
                                    foreach (var pvk in ((IDictionary<string, object>)kvp.Value))
                                    {
                                        Log.ToServer($"{pvk.Key}: {pvk.Value}");
                                    }*/
                                    var hasItem = headOverlayForServer.ContainsKey((byte)Convert.ToInt32(kvp.Value.ID));
                                    if (!hasItem)
                                    {
                                        var o = kvp;
                                        headOverlayForServer.Add((byte)Convert.ToInt32(o.Value.ID),
                                            new Dictionary<string, dynamic>()
                                            {
                                                ["ID"] = o.Value.ID,
                                                ["Variant"] = o.Value.Variant,
                                                ["Opacity"] = o.Value.Opacity,
                                                ["PrimaryColor"] = o.Value.PrimaryColor,
                                                ["SecondaryColor"] = o.Value.SecondaryColor
                                            });
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, true);
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, true);
                }
            }
            
            skinItems["HeadOverlays"] = headOverlayForServer;
            skinItems["SkinModel"] = keepModel ? SkinModel : Game.PlayerPed.Model.Hash;
            if (!keepModel)
            {
                skinItems["Components"] = new Dictionary<string, System.Dynamic.ExpandoObject>();
                Enum.GetValues(typeof(PedComponents)).Cast<PedComponents>().ToList().ForEach(o =>
                {
                    skinItems["Components"][o.ToString()] = new System.Dynamic.ExpandoObject();
                    skinItems["Components"][o.ToString()].Index = Game.PlayerPed.Style[o].Index;
                    skinItems["Components"][o.ToString()].TextureIndex = Game.PlayerPed.Style[o].TextureIndex;
                });

                skinItems["Props"] = new Dictionary<string, System.Dynamic.ExpandoObject>();
                Enum.GetValues(typeof(PedProps)).Cast<PedProps>().ToList().ForEach(o =>
                {
                    skinItems["Props"][o.ToString()] = new System.Dynamic.ExpandoObject();
                    skinItems["Props"][o.ToString()].Index = Game.PlayerPed.Style[o].Index;
                    skinItems["Props"][o.ToString()].TextureIndex = Game.PlayerPed.Style[o].TextureIndex;
                });
            }
            else
            {
                skinItems["Components"] = Components;
                skinItems["Props"] = Props;
            }

            skinItems["Tattoos"] = keepModel ? Tattoos : Roleplay.Client.Player.Clothing.Tattoos.CurrentTattoos;

            return skinItems;
        }
    }
}