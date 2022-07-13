using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Jobs;
using Roleplay.Client.Player.Clothing;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using MenuFramework;

namespace Roleplay.Client.UI.Jobs
{
    public class EmergencyCharacterMenu : ClientAccessor
    {
        public static MenuModel Menu;

        static float PoliceMarkerAoe = (float)Math.Pow(5f, 2);

        static Dictionary<string, Tuple<int, int>> componentSettings = new Dictionary<string, Tuple<int, int>>();
        static Dictionary<string, Tuple<int, int>> propSettings = new Dictionary<string, Tuple<int, int>>();
        static Dictionary<string, string> componentAndPropRenamings = new Dictionary<string, string>()
        {
            ["Torso"] = "Arms",
            ["Legs"] = "Pants",
            ["Hands"] = "Parachutes, Vests and Bags",
            ["Special1"] = "Neck",
            ["Special2"] = "Overshirt",
            ["Special3"] = "Tactical Vests",
            ["Textures"] = "Logos",
            ["Torso"] = "Arms",
            ["Torso2"] = "Jacket",
            ["EarPieces"] = "Ear Pieces"
        };

        static List<string> PedHashFilter = new List<string>() { "s_m_y_cop_01", "s_f_y_sheriff_01", "s_m_y_hwaycop_01", "s_m_y_ranger_01", "s_m_y_sheriff_01", "s_m_y_swat_01", "s_f_y_cop_01" };
        static List<PedHash> PedHashFilterConverted = PedHashFilter.Select(p => (PedHash)Game.GenerateHash(p)).ToList();
        static List<PedHash> PedHashValues = Enum.GetValues(typeof(PedHash)).OfType<PedHash>().Where(p => PedHashFilterConverted.Contains(p)).ToList();
        static List<string> PedHashNames = PedHashValues.Select(c => c.ToString().AddSpacesToCamelCase()).ToList();

        static List<PedProps> PedPropValues = Enum.GetValues(typeof(PedProps)).OfType<PedProps>().ToList();
        static List<string> PedPropNames = Enum.GetNames(typeof(PedProps)).Select(c => c.AddSpacesToCamelCase()).ToList();

        class PoliceCharacterMenuModel : MenuModel
        {
            public static async Task ReplaceCurrentPedModelByIndex(int index)
            {
                await new Model(PedHashValues[index]).Request(10000);
                await Game.Player.ChangeModel(new Model(PedHashValues[index]));
                await BaseScript.Delay(100);
                if (PedHashValues[index] == PedHash.FreemodeMale01 || PedHashValues[index] == PedHash.FreemodeFemale01)
                {
                    Function.Call(Hash.SET_PED_HEAD_BLEND_DATA, Game.PlayerPed.Handle, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
                }
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.PlayerPed.Handle, 1, 0, 0, 0);
            }

            public override void Refresh()
            {
                var _menuItems = new List<MenuItem>();
                var ped = Game.PlayerPed;
                int selectedPedHashIndex = PedHashValues.Contains((PedHash)ped.Model.Hash) ? PedHashValues.IndexOf((PedHash)ped.Model.Hash) : 0;
                if (Game.PlayerPed.Exists())
                {
                    /*_menuItems.Add(new MenuItemHorNamedSelector
                    {
                        Title = $"Skin",
                        Description = "Activate to replace your current skin.",
                        state = menuItems[0] is MenuItemHorNamedSelector ? (menuItems[0] as MenuItemHorNamedSelector).state : selectedPedHashIndex,
                        Type = MenuItemHorizontalSelectorType.NumberAndBar,
                        wrapAround = true,
                        optionList = PedHashNames,
                        OnActivate = new Action<int, string, MenuItemHorNamedSelector>(async (selectedAlternative, selName, menuItem) => {
                            await ReplaceCurrentPedModelByIndex(selectedAlternative); componentSettings.Clear(); propSettings.Clear();
                        })
                    });*/

                    PedComponent[] components = Game.PlayerPed.Style.GetAllComponents();
                    PedProp[] props = Game.PlayerPed.Style.GetAllProps();

                    components.ToList().ForEach(c =>
                    {
                        try
                        {
                            if (!(c.ToString() == "Face") || (!((PedHash)ped.Model.Hash == PedHash.FreemodeMale01) && !((PedHash)ped.Model.Hash == PedHash.FreemodeFemale01))) // Since Face doesn't work on freemode characters (if you use the blending/morphing option anyway, which everybody should be)
                            {
                                if (!componentSettings.ContainsKey(c.ToString())) componentSettings.Add(c.ToString(), new Tuple<int, int>(0, 0));
                                if (componentSettings[c.ToString()].Item1 < 0 || componentSettings[c.ToString()].Item1 > c.Count - 1) componentSettings[c.ToString()] = new Tuple<int, int>(0, componentSettings[c.ToString()].Item2);
                                if (componentSettings[c.ToString()].Item2 < 0 || componentSettings[c.ToString()].Item2 > c.TextureCount - 1) componentSettings[c.ToString()] = new Tuple<int, int>(componentSettings[c.ToString()].Item1, 0);
                                if (c.Count > 1)
                                {
                                    _menuItems.Add(new MenuItemHorSelector<int>
                                    {
                                        Title = $@"{(componentAndPropRenamings.ContainsKey(c.ToString()) ? componentAndPropRenamings[c.ToString()] : c.ToString())}",
                                        state = componentSettings[c.ToString()].Item1,
                                        Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                        wrapAround = true,
                                        minState = 0,
                                        maxState = c.Count - 1,
                                        overrideDetailWith = $"{componentSettings[c.ToString()].Item1 + 1}/{c.Count}",
                                        OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) =>
                                        {
                                            componentSettings[c.ToString()] = new Tuple<int, int>(selectedAlternative, 0);
                                            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped.Handle, Enum.GetNames(typeof(PedComponents)).ToList().IndexOf(c.ToString()), selectedAlternative, 0, 0);
                                        })
                                    });
                                }
                                if (c.TextureCount > 1)
                                {
                                    _menuItems.Add(new MenuItemHorSelector<int>
                                    {
                                        Title = $@"{(componentAndPropRenamings.ContainsKey(c.ToString()) ? componentAndPropRenamings[c.ToString()] : c.ToString())}: Variants",
                                        state = componentSettings[c.ToString()].Item2,
                                        Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                        wrapAround = true,
                                        minState = 0,
                                        maxState = c.TextureCount - 1,
                                        overrideDetailWith = $"{componentSettings[c.ToString()].Item2 + 1}/{c.TextureCount}",
                                        OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) => {
                                            componentSettings[c.ToString()] = new Tuple<int, int>(componentSettings[c.ToString()].Item1, selectedAlternative);
                                            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped.Handle, Enum.GetNames(typeof(PedComponents)).ToList().IndexOf(c.ToString()), componentSettings[c.ToString()].Item1, selectedAlternative, 0);
                                        })
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"[POLICEPEDMENU] Exception in ped components code; {ex.Message}");
                        }
                    });

                    props.ToList().ForEach(p =>
                    {
                        try
                        {
                            if (!propSettings.ContainsKey(p.ToString())) propSettings.Add(p.ToString(), new Tuple<int, int>(0, 0));
                            if (propSettings[p.ToString()].Item1 < -1 || propSettings[p.ToString()].Item1 > p.Count - 1) componentSettings[p.ToString()] = new Tuple<int, int>(0, propSettings[p.ToString()].Item2);
                            if (propSettings[p.ToString()].Item2 < 0 || propSettings[p.ToString()].Item2 > p.TextureCount - 1) propSettings[p.ToString()] = new Tuple<int, int>(propSettings[p.ToString()].Item1, 0);
                            if (p.Count > 1)
                            {
                                _menuItems.Add(new MenuItemHorSelector<int>
                                {
                                    Title = $@"{(componentAndPropRenamings.ContainsKey(p.ToString()) ? componentAndPropRenamings[p.ToString()] : p.ToString())}",
                                    state = propSettings[p.ToString()].Item1,
                                    Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                    wrapAround = true,
                                    minState = -1,
                                    maxState = p.Count - 1,
                                    overrideDetailWith = $"{propSettings[p.ToString()].Item1 + 2}/{p.Count + 1}",
                                    OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) => {
                                        propSettings[p.ToString()] = new Tuple<int, int>(selectedAlternative, 0);
                                        if (selectedAlternative == -1)
                                            Function.Call(Hash.CLEAR_PED_PROP, ped.Handle, Enum.GetNames(typeof(PedProps)).ToList().IndexOf(p.ToString()));
                                        else
                                            Function.Call(Hash.SET_PED_PROP_INDEX, ped.Handle, Enum.GetNames(typeof(PedProps)).ToList().IndexOf(p.ToString()), selectedAlternative, 0, false);
                                    })
                                });
                            }
                            if (p.TextureCount > 1)
                            {
                                _menuItems.Add(new MenuItemHorSelector<int>
                                {
                                    Title = $"{(componentAndPropRenamings.ContainsKey(p.ToString()) ? componentAndPropRenamings[p.ToString()] : p.ToString())}: Variants",
                                    state = propSettings[p.ToString()].Item2,
                                    Type = MenuItemHorizontalSelectorType.NumberAndBar,
                                    wrapAround = true,
                                    minState = 0,
                                    maxState = p.TextureCount - 1,
                                    overrideDetailWith = $"{propSettings[p.ToString()].Item2 + 1}/{p.TextureCount}",
                                    OnChange = new Action<int, MenuItemHorSelector<int>>((selectedAlternative, menuItem) => {
                                        propSettings[p.ToString()] = new Tuple<int, int>(propSettings[p.ToString()].Item1, selectedAlternative);
                                        Function.Call(Hash.SET_PED_PROP_INDEX, ped.Handle, Enum.GetNames(typeof(PedProps)).ToList().IndexOf(p.ToString()), propSettings[p.ToString()].Item1, selectedAlternative, false);
                                    })
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"[POLICEPEDMENU] Exception in ped props code; {ex.Message}");
                        }
                    });
                }

                visibleItems = _menuItems.Slice(visibleItems.IndexOf(menuItems.First()), visibleItems.IndexOf(menuItems.Last()));
                menuItems = _menuItems;
                SelectedIndex = SelectedIndex; // refreshes state
            }
        }

        public EmergencyCharacterMenu(Client client) : base(client)
        {
            Menu = new PoliceCharacterMenuModel { numVisibleItems = 7 };
            Menu.headerTitle = "Character Customization";
            Menu.statusTitle = "";
            Menu.menuItems = new List<MenuItem>() { new MenuItemStandard { Title = "Populating menu..." } }; // Currently we need at least one item in a menu; could make it work differently, but eh
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemSubMenu { Title = $"Customise Loadout", SubMenu = Menu }, () => Client.Get<JobHandler>().OnDutyAsJob(JobType.EMS | JobType.Police) && Client.Get<Outfits>().InRangeOfOutfitChange(), 1000);
            client.RegisterTickHandler(OnTick);
        }

        private async Task OnTick()
        {
            try
            {
                if (InteractionUI.Observer.CurrentMenu == Menu)
                {
                    Menu.Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[COPCHARACTERMENU] Exception in OnTick; {ex.Message}");
            }
        }
    }
}
