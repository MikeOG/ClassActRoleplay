using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using MenuFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Client.Emotes
{
    /*internal static class Expressions
    {
        internal static Dictionary<string, Expressions.ExpressionModel> Expression = new Dictionary<string, Expressions.ExpressionModel>()
        {
            ["happy"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_happy_1"),
            ["unhappy"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_drivefast_1"),
            ["angry"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_angry_1"),
            ["kiss"] = new Expressions.ExpressionModel("facials@gen_female@base", "blowkiss_1"),
            ["effort"] = new Expressions.ExpressionModel("facials@gen_male@base", "effort_1"),
            ["effort2"] = new Expressions.ExpressionModel("facials@gen_male@base", "effort_2"),
            ["effort3"] = new Expressions.ExpressionModel("facials@gen_male@base", "effort_3"),
            ["drunk"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_drunk_1"),
            ["injured"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_injured_1"),
            ["sleep"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_sleeping_1"),
            ["smug"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_smug_1"),
            ["upset"] = new Expressions.ExpressionModel("facials@gen_male@base", "mood_stressed_1"),
            ["pain"] = new Expressions.ExpressionModel("facials@gen_male@base", "pain_1"),
            ["pain2"] = new Expressions.ExpressionModel("facials@gen_male@base", "pain_2"),
            ["pain3"] = new Expressions.ExpressionModel("facials@gen_male@base", "pain_3"),
            ["pain4"] = new Expressions.ExpressionModel("facials@gen_male@base", "pain_4"),
            ["shocked"] = new Expressions.ExpressionModel("facials@gen_male@base", "shocked_1"),
            ["shocked2"] = new Expressions.ExpressionModel("facials@gen_male@base", "shocked_2")
        };
        private static string CurrentExpression;

        public static void Init()
        {
            CommandRegister.RegisterCommand("/mood", new Action<Command>(Expressions.StartExpression));
            CommandRegister.RegisterCommand("/moods", new Action<Command>(Expressions.HandleMoodSearch));
            List<MenuItem> emotesMenuItems = new List<MenuItem>();
            Expressions.Expression.Keys.OrderBy<string, string>((Func<string, string>)(x => x)).ToList<string>().ForEach((Action<string>)(expression =>
            {
                List<MenuItem> menuItemList = emotesMenuItems;
                menuItemList.Add((MenuItem)new MenuItemStandard()
                {
                    Title = string.Format("{0}", (object)expression.ToTitleCase()),
                    OnActivate = (Action<MenuItemStandard>)(item => Expressions.PlayExpression(item.Title.ToLower()))
                });
            }));
            MenuItemSubMenu menuItemSubMenu = new MenuItemSubMenu();
            menuItemSubMenu.Title = "Moods";
            menuItemSubMenu.SubMenu = new MenuModel((MenuOptions)null)
            {
                headerTitle = "Moods",
                menuItems = emotesMenuItems
            };
            menu.SubMenu.SelectedIndex = menu.SubMenu.SelectedIndex;
            //InteractionListMenu.RegisterInteractionMenuItem((MenuItem)menuItemSubMenu, (Func<bool>)(() => true), 1147);
        }

        internal static void StartExpression(Command cmd)
        {
            if (cmd.Args.Count >= 1)
            {
                if (!Expressions.Expression.ContainsKey(cmd.Args.Get(0).ToLower()))
                    return;
                Expressions.CurrentExpression = cmd.Args.Get(0).ToLower();
                Expressions.PlayExpression(Expressions.CurrentExpression);
            }
            else
                Expressions.HandleMoodSearch(cmd);
        }

        private static void HandleMoodSearch(Command cmd)
        {
            try
            {
                if (cmd.Args.Count == 0)
                {
                    List<string> list = Expressions.Expression.Keys.Select<string, string>((Func<string, string>)(e => e.ToTitleCase())).ToList<string>();
                    list.Sort();
                    BaseScript.TriggerEvent("Chat.Message", new object[3]
                    {
                        (object) "",
                        (object) ConstantColours.Help,
                        (object) string.Format("Valid moods: {0}", (object) string.Join(", ", (IEnumerable<string>) list))
                    });
                }
                else
                {
                    if (cmd.Args.Count != 1)
                        return;
                    string search = cmd.Args.Get(0);
                    string str = string.Join(", ", Expressions.Expression.Keys.Where<string>((Func<string, bool>)(n => n.ToUpper().Contains(search.ToUpper()))).Select<string, string>((Func<string, string>)(emote => string.Format("{0}", (object)emote))));
                    BaseScript.TriggerEvent("Chat.Message", new object[3]
                    {
                        (object) "",
                        (object) ConstantColours.Help,
                        (object) string.Format("Mood '{0}' matches: {1}", (object) search, (object) str)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("HandleMoodSearch error: {0}", (object)ex));
            }
        }

        internal static async void PlayExpression(string express)
        {
            API.PlayFacialAnim(((PoolObject)Game.PlayerPed).Handle, Expressions.Expression[express].AnimationName, Expressions.Expression[express].DictionaryName);
        }

        internal class ExpressionModel
        {
            public ExpressionModel(string dict, string anim)
            {
                this.DictionaryName = dict;
                this.AnimationName = anim;
            }
            public string DictionaryName { get; set; }
            public string AnimationName { get; set; }
        }
    }*/
}
