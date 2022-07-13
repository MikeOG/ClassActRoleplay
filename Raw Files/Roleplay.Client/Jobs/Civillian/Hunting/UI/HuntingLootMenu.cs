using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuFramework;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.Civillian.Hunting.UI
{
    public class HuntingLootMenu : MenuModel
    {
        private static List<MenuItem> _menuItems = new List<MenuItem>();
        private static List<string> deerExtraRewards = new List<string>() { "antlers" };
        private static List<string> mountainLionExtraRewards = new List<string>() { "teeth", "claws" };
        private static List<string> coyoteExtraRewards = new List<string>() { "skin" };
        public static List<string> lootableRewards = new List<string>()
        {
            /*"skin",*/ "meat", "pelts"
        };

        public HuntingLootMenu()
        {
            headerTitle = "Animal loot";
            statusTitle = "";
            menuItems = new List<MenuItem>() { new MenuItemStandard { Title = "Populating" } };
        }

        public void createLootOptions(string animalName, Ped animalPed)
        {
            headerTitle = $"{animalName.AddSpacesToCamelCase()} looting";
            _menuItems.Clear();
            lootableRewards.ForEach(o =>
            {
                _menuItems.Add(new MenuItemStandard
                {
                    Title = $"Loot {animalName.AddSpacesToCamelCase()} {o}",
                    OnActivate = async state =>
                    {
                        int lootedAmount = new Random().Next(1, 4);
                        Log.ToChat("[Hunting]", $"Looting {animalName}", ConstantColours.Hunting);
                        await BaseScript.Delay(2500);
                        if (Function.Call<bool>(Hash.DECOR_EXIST_ON, animalPed.Handle, "Animal.Looted")) return;
                        Client.Instance.TriggerServerEvent("Hunting.AttemptLootItem", $"{animalName}_{o}", lootedAmount);
                        Function.Call(Hash.DECOR_SET_INT, animalPed, "Animal.Looted", 1);
                    }
                });
            });
            addExtraAnimalRewards(animalName, animalPed);
            menuItems = _menuItems;
            SelectedIndex = SelectedIndex;
        }

        private void addExtraAnimalRewards(string animalName, Ped animalPed)
        {
            List<string> extraList = getExtraAnimalRewards(animalName);

            extraList.ForEach(o =>
            {
                _menuItems.Add(new MenuItemStandard
                {
                    Title = $"Loot {animalName.AddSpacesToCamelCase()} {o}",
                    OnActivate = async state =>
                    {
                        int lootedAmount = new Random().Next(1, 4);
                        Log.ToChat("[Hunting]", $"Looting {animalName}", ConstantColours.Hunting);
                        await BaseScript.Delay(2500);
                        if (Function.Call<bool>(Hash.DECOR_EXIST_ON, animalPed.Handle, "Animal.Looted")) return;
                        Client.Instance.TriggerServerEvent("Hunting.AttemptLootItem", $"{animalName}_{o}", lootedAmount);
                        Function.Call(Hash.DECOR_SET_INT, animalPed, "Animal.Looted", 1);
                    }
                });
            });
        }

        public static List<string> getExtraAnimalRewards(string animalName)
        {
            List<string> extraOptions = new List<string>();
            if (animalName == "Deer")
                extraOptions = deerExtraRewards;
            else if (animalName == "MountainLion")
                extraOptions = mountainLionExtraRewards;
            else if (animalName == "Coyote")
                extraOptions = coyoteExtraRewards;

            return extraOptions;
        }
    }
}
