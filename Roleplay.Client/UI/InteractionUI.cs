using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared.Helpers;
using MenuFramework;

namespace Roleplay.Client.UI
{
    class InteractionUI : ClientAccessor
    {
        public static MenuObserver Observer;
        public static MenuModel InteractionMenu;
        public List<Tuple<int, MenuItem, Func<bool>>> ItemsAll = new List<Tuple<int, MenuItem, Func<bool>>>();
        internal List<MenuItem> ItemsFiltered = new List<MenuItem>();
        public bool IsDirty = false;

        private static List<string> walkingStyles = new List<string>() { "Reset to default", "move_m@injured", "move_m@gangster@generic", "move_m@gangster@ng", "move_m@buzzed", "move_m@drunk@a", "move_f@hurry@a", "move_f@femme@", "move_f@arrogant@a", "move_m@sassy", "move_m@quick", "move_m@tough_guy@", "move_f@tough_guy@", "move_m@alien", "move_m@hobo@a", "move_m@money", "move_m@swagger", "move_m@posh@", "move_m@sad@a", "move_m@shadyped@a", "move_m@drunk@slightlydrunk", "move_m@drunk@verydrunk", "move_f@arrogant@a", "move_f@arrogant@b", "move_f@arrogant@c", "move_f@heels@c", "move_f@heels@d", "move_f@sassy", "move_f@posh@", "move_f@maneater", "move_f@chichi", "move_m@swagger", "move_m@brave", "move_m@casual@a", "move_m@casual@b", "move_m@casual@c", "move_m@casual@d", "move_m@casual@e", "move_m@casual@f", "move_m@confident", "move_m@business@a", "move_m@business@b", "move_m@business@c", "move_f@multiplayer", "move_m@multiplayer", "move_characters@franklin@fire", "move_characters@michael@fire", "move_m@fire", "move_f@flee@a", "anim@move_m@grooving@", "move_m@prison_gaurd", "move_m@prisoner_cuffed", "move_m@hiking", "move_m@hipster@a", "move_m@jog@", "move_m@muscle@a", "move_f@scared", "move_f@sexy@a" };
        private static int walkingStyleIndex = 0;

        private MenuItem WalkingStyleItem = new MenuItemHorNamedSelector
        {
            Title = $"Walking Style",
            State = walkingStyleIndex,
            Type = MenuItemHorizontalSelectorType.NumberAndBar,
            wrapAround = true,
            optionList = walkingStyles,
            overrideDetailWith = walkingStyles[walkingStyleIndex].Replace("move_m@", "").Replace("move_f@", "").Replace("move_characters", "").Replace("move_", "").Replace("@", " ").Replace("_", " ").ToTitleCase(),
            OnChange = async (selectedAlternative, selName, menuItem) => // TODO make this work properly
            {
                walkingStyleIndex = selectedAlternative;
                if (walkingStyleIndex == 0)
                {
                    Function.Call(Hash.RESET_PED_MOVEMENT_CLIPSET, Game.PlayerPed.Handle);
                }
                else
                {
                    Function.Call(Hash.REQUEST_CLIP_SET, walkingStyles[selectedAlternative]);
                    Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, Game.PlayerPed.Handle, walkingStyles[selectedAlternative], 1.0f);
                    await BaseScript.Delay(1000);
                    if (walkingStyleIndex == selectedAlternative) Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, Game.PlayerPed.Handle, walkingStyles[selectedAlternative], 0.25f);
                }
            }
        };

        public InteractionUI(Client client) : base(client)
        {
            Observer = new MenuObserver();
            InteractionMenu = new MenuModel { numVisibleItems = 10 };
            InteractionMenu.headerTitle = "Interaction Menu";
            InteractionMenu.statusTitle = "";
            InteractionMenu.menuItems = new List<MenuItem> { new MenuItemStandard { Title = "Populating menu..." } }; // Currently we need at least one item in a menu; could make it work differently, but eh.
            Client.Instance.RegisterTickHandler(OnTick);
            RegisterInteractionMenuItem(WalkingStyleItem, () => true, 100);

            client.RegisterEventHandler("UI.UpdateInteractionMenuLocation", new Action<int, int>((x, y) =>
            {
                MenuGlobals.MenuLocation = new PointF(x, y);
            }));
        }

        /// <summary>
        /// Priority Assumptions: 
        /// 1 is Admin | Dev commands (Kick, ban, spawn, etc)
        /// 100 is default commands (Emotes for example)
        /// 250 is job specific commands (Tow, process, package, whatever)
        /// 500 is location specific commands (Shopping)
        /// 1000 is Duty commands (Police commands, ems, etc)
        /// </summary>
        public void RegisterInteractionMenuItem(MenuItem item, Func<bool> check, int priority = 100)
        {
            ItemsAll.Add(new Tuple<int, MenuItem, Func<bool>>(priority, item, check));
        }

        public void RemoveInteractionMenuItem(MenuItem item)
        {
            ItemsAll.Remove(ItemsAll.Find(o => o.Item2 == item));
        }

        public bool ContainsInteractionMenuItem(MenuItem item)
        {
            return ItemsAll.FirstOrDefault(o => o.Item2 == item) != null;
        }

        public void toggleMenuState()
        {
            if (Input.IsControlJustPressed(Control.InteractionMenu))
            {
                if (Observer.CurrentMenu == null)
                {
                    Observer.OpenMenu(InteractionMenu);
                }
                else
                {
                    Observer.CloseMenu(true);
                }
                MenuController.PlaySound("NAV_UP_DOWN");
            }
        }

        private async Task OnTick()
        {
            try
            {
                if (Observer.CurrentMenu == InteractionMenu)
                {
                    (WalkingStyleItem as MenuItemHorNamedSelector).State = walkingStyleIndex;
                    (WalkingStyleItem as MenuItemHorNamedSelector).overrideDetailWith = walkingStyles[walkingStyleIndex].Replace("move_m@", "").Replace("move_f@", "").Replace("@", " ").Replace("_", " ").ToTitleCase();

                    ItemsFiltered = ItemsAll.Where(m => m.Item3.Invoke()).OrderBy(m => m.Item2.Title).OrderByDescending(m => m.Item1).Select(m => m.Item2).ToList();
                    if (!ItemsFiltered.SequenceEqual(InteractionMenu.menuItems) || IsDirty)
                    {
                        if (ItemsFiltered.Contains(InteractionMenu.SelectedItem))
                        {
                            int newSelectedIndex = ItemsFiltered.IndexOf(InteractionMenu.SelectedItem);
                            InteractionMenu.menuItems = ItemsFiltered;
                            InteractionMenu.SelectedIndex = newSelectedIndex;
                        }
                        else if (InteractionMenu.SelectedIndex.IsBetween(0, ItemsFiltered.Count - 1))
                        {
                            InteractionMenu.menuItems = ItemsFiltered;
                            InteractionMenu.SelectedIndex = InteractionMenu.SelectedIndex; // Simply refreshes selection
                        }
                        else
                        {
                            InteractionMenu.menuItems = ItemsFiltered;
                            InteractionMenu.SelectedIndex = 0;
                        }

                        IsDirty = false;
                    }
                }

                Observer.Tick();
                toggleMenuState();
            }
            catch (Exception ex)
            {
                Roleplay.Shared.Log.Error(ex);
            }

            //await Task.FromResult(0);
        }
    }
}
