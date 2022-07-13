using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Roleplay.Shared
{
    public class inventoryItem
    {
        public string itemName = "";
        public string itemCode = "";
        public string metaData = "";
        public string interactionData = "";
        public string interactionAction = "";
        public string interactionEvent = "";
        public bool isIllegal = false;
        public float itemWeight = 0.0f;
        public int itemAmount = 0;
        public bool isUseable = false;
        public bool removeItemOnUse = true;

        public inventoryItem()
        {

        }

        public inventoryItem(inventoryItem item)
        {
            itemName = item.itemName;
            itemCode = item.itemCode;
            metaData = item.metaData;
            interactionData = item.interactionData;
            interactionAction = item.interactionAction;
            interactionEvent = item.interactionEvent;
            isIllegal = item.isIllegal;
            itemWeight = item.itemWeight;
            itemAmount = item.itemAmount;
            isUseable = item.isUseable;
            removeItemOnUse = item.removeItemOnUse;
        }  
    }

    public static class InventoryItems
    {
        public static readonly Dictionary<string, inventoryItem> inventoryItems = new Dictionary<string, inventoryItem>()
        {
            ["Amphetamine"] = new inventoryItem { itemName = "Methamphetamine", itemCode = "Amphetamine", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 2.0f },
            ["cocaine"] = new inventoryItem { itemName = "Cocaine baggie", itemCode = "cocaine", interactionEvent = "startDrugEffect", interactionAction = "smoked", interactionData = "20", isIllegal = true, isUseable = false, itemWeight = 2.0f },
            ["MethRock"] = new inventoryItem { itemName = "Meth Rock", itemCode = "MethRock", interactionEvent = "startDrugEffect", interactionAction = "", interactionData = "40", isIllegal = true, isUseable = true, itemWeight = 2.0f },
            ["unprocessedcocaine"] = new inventoryItem { itemName = "Unprocessed Cocaine", itemCode = "unprocessedcocaine", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 2.0f },
            ["Weed"] = new inventoryItem { itemName = "Weed baggie", itemCode = "Weed", interactionEvent = "Player.DoSmoke", interactionAction = "smoked a", interactionData = "Weed", isIllegal = true, isUseable = true, itemWeight = 2.0f },
            ["Bud"] = new inventoryItem { itemName = "Bud", itemCode = "Bud", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 2.0f },
            ["Oxycontin"] = new inventoryItem { itemName = "Oxycontin", itemCode = "oxycontin", interactionEvent = "startDrugEffect", interactionAction = "smoked", interactionData = "", isIllegal = true, isUseable = true, itemWeight = 2.0f },
            ["unprocessedoxy"] = new inventoryItem { itemName = "unprocessed Oxy", itemCode = "unprocessedoxy", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 2.0f },
            // Burger shot
            ["Fries"] = new inventoryItem { itemName = "Fries", itemCode = "Fries", interactionEvent = "Player.DoHeal", interactionAction = "ate some", interactionData = "Fries", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["SteakFries"] = new inventoryItem { itemName = "Steak Fries", itemCode = "SteakFries", interactionEvent = "Player.DoHeal", interactionAction = "ate some", interactionData = "SteakFries", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["CurlyFries"] = new inventoryItem { itemName = "Curly Fries", itemCode = "CurlyFries", interactionEvent = "Player.DoHeal", interactionAction = "ate some", interactionData = "CurlyFries", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["HeartStopperBurger"] = new inventoryItem { itemName = "Heart Stopper Burger", itemCode = "HeartStopperBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "HeartStopperBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["BleederBurger"] = new inventoryItem { itemName = "Bleeder Burger", itemCode = "BleederBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "BleederBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["MoneyShotBurger"] = new inventoryItem { itemName = "Money Shot Burger", itemCode = "MoneyShotBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "MoneyShotBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["TorpedoBurger"] = new inventoryItem { itemName = "Torpedo Burger", itemCode = "TorpedoBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "TorpedoBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["MeatFreeBurger"] = new inventoryItem { itemName = "Meat Free Burger", itemCode = "MeatFreeBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "MeatFreeBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Vanilla Milkshake"] = new inventoryItem { itemName = "Vanilla Milkshake Can", itemCode = "Vanilla Milkshake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Vanilla Milkshake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Strawberry Milkshake"] = new inventoryItem { itemName = "Strawberry Milkshake", itemCode = "Strawberry Milkshake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Strawberry Milkshake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chocolate Milkshake"] = new inventoryItem { itemName = "Chocolate Milkshake", itemCode = "Chocolate Milkshake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Chocolate Milkshake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            // Taco Stand
            ["Taco"] = new inventoryItem { itemName = "Taco", itemCode = "Taco", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Taco", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Burrito"] = new inventoryItem { itemName = "Burrito", itemCode = "Burrito", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Burrito", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Quesadilla"] = new inventoryItem { itemName = "Quesadilla", itemCode = "Quesadilla", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Quesadilla", isIllegal = false, isUseable = true, itemWeight = 1.0f },

            // Pops Diner
            ["HamBurger"] = new inventoryItem { itemName = "HamBurger", itemCode = "HamBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "HamBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Cheese Burger"] = new inventoryItem { itemName = "Cheese Burger", itemCode = "Cheese Burger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Cheese Burger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Double Cheese Burger"] = new inventoryItem { itemName = "Double Cheese Burger", itemCode = "Double Cheese Burger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Double Cheese Burger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Bacon Cheese Burger"] = new inventoryItem { itemName = "Bacon Cheese Burger", itemCode = "Bacon Cheese Burger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Bacon Cheese Burger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Black Bean Burger"] = new inventoryItem { itemName = "Black Bean Burger", itemCode = "Black Bean Burger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Black Bean Burger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Black and Blue Burger"] = new inventoryItem { itemName = "Black and Blue Burger", itemCode = "Black and Blue Burger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Black and Blue Burger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Fries"] = new inventoryItem { itemName = "Fries", itemCode = "Fries", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Fries", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Tots"] = new inventoryItem { itemName = "Tots", itemCode = "Tots", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Tots", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Jello"] = new inventoryItem { itemName = "Jello", itemCode = "Jello", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Jello", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Mozzarella Sticks"] = new inventoryItem { itemName = "Mozzarella Sticks", itemCode = "Mozzarella Sticks", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Mozzarella Sticks", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Onion Rings"] = new inventoryItem { itemName = "Onion Rings", itemCode = "Onion Rings", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Onion Rings", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chicken Wings"] = new inventoryItem { itemName = "Chicken Wings", itemCode = "Chicken Wings", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chicken Wings", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chicken Tendies"] = new inventoryItem { itemName = "Chicken Tendies", itemCode = "Chicken Tendies", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chicken Tendies", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chicken Club Sandwich"] = new inventoryItem { itemName = "Chicken Club Sandwich", itemCode = "Chicken Club Sandwich", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chicken Club Sandwich", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Grilled Chicken Sandwich"] = new inventoryItem { itemName = "Grilled Chicken Sandwich", itemCode = "Grilled Chicken Sandwich", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Grilled Chicken Sandwich", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["BLT"] = new inventoryItem { itemName = "BLT", itemCode = "BLT", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "BLT", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chili Cheese Dog"] = new inventoryItem { itemName = "Chili Cheese Dog", itemCode = "Chili Cheese Dog", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chili Cheese Dog", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Hot Dog"] = new inventoryItem { itemName = "Hot Dog", itemCode = "Hot Dog", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Hot Dog", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Tuna Melt"] = new inventoryItem { itemName = "Tuna Melt", itemCode = "Tuna Melt", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Tuna Melt", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Grilled Cheese"] = new inventoryItem { itemName = "Grilled Cheese", itemCode = "Grilled Cheese", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Grilled Cheese", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Philly Cheese Steak"] = new inventoryItem { itemName = "Philly Cheese Steak", itemCode = "Philly Cheese Steak", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Philly Cheese Steak", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Cobbs Salad"] = new inventoryItem { itemName = "Cobbs Salad", itemCode = "Cobbs Salad", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Cobbs Salad", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chef Salad"] = new inventoryItem { itemName = "Chef Salad", itemCode = "Chef Salad", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chef Salad", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Garden Salad"] = new inventoryItem { itemName = "Garden Salad", itemCode = "Garden Salad", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Garden Salad", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chicken Salad"] = new inventoryItem { itemName = "Chicken Salad", itemCode = "Chicken Salad", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Chicken Salad", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Steak Salad"] = new inventoryItem { itemName = "Steak Salad", itemCode = "Steak Salad", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Steak Salad", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Hot Fudge Sundae"] = new inventoryItem { itemName = "Hot Fudge Sundae", itemCode = "Hot Fudge Sundae", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Hot Fudge Sundae", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Banana Split"] = new inventoryItem { itemName = "Banana Split", itemCode = "Banana Split", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Banana Split", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Single Scoop Ice Cream"] = new inventoryItem { itemName = "Single Scoop Ice Cream", itemCode = "Single Scoop Ice Cream", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Single Scoop Ice Cream", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Apple Pie"] = new inventoryItem { itemName = "Apple Pie", itemCode = "Apple Pie", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Apple Pie", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["HamBurger"] = new inventoryItem { itemName = "HamBurger", itemCode = "HamBurger", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "HamBurger", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Coffee"] = new inventoryItem { itemName = "Coffee", itemCode = "Coffee", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Coffee", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chocolate Shake"] = new inventoryItem { itemName = "Chocolate Shake", itemCode = "Chocolate Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Chocolate Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Vanilla Shake"] = new inventoryItem { itemName = "Vanilla Shake", itemCode = "Vanilla Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Vanilla Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Strawberry Shake"] = new inventoryItem { itemName = "Strawberry Shake", itemCode = "Strawberry Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Strawberry Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Banana Shake"] = new inventoryItem { itemName = "Banana Shake", itemCode = "Banana Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Banana Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Peanut Butter Shake"] = new inventoryItem { itemName = "Peanut Butter Shake", itemCode = "Peanut Butter Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Peanut Butter Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Cookie Shake"] = new inventoryItem { itemName = "Cookie Shake", itemCode = "Cookie Shake", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Cookie Shake", isIllegal = false, isUseable = true, itemWeight = 1.0f },

            // General Store // Bars
            ["Apple"] = new inventoryItem { itemName = "Apple", itemCode = "Apple", interactionEvent = "Player.DoHeal", interactionAction = "ate an", interactionData = "Apple", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["AppleJuice"] = new inventoryItem { itemName = "Apple Juice", itemCode = "AppleJuice", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "AppleJuice", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Banana"] = new inventoryItem { itemName = "Banana", itemCode = "Banana", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Banana", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Bandage"] = new inventoryItem { itemName = "Bandage", itemCode = "Bandage", interactionEvent = "Player.DoHeal", interactionAction = "used a", interactionData = "Bandage", isIllegal = false, isUseable = true, itemWeight = 2.0f },
            ["BeefJerky"] = new inventoryItem { itemName = "Beef Jerky", itemCode = "BeefJerky", interactionEvent = "Player.DoHeal", interactionAction = "ate", interactionData = "BeefJerky", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Beer"] = new inventoryItem { itemName = "Beer", itemCode = "Beer", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "low", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["bobbypin"] = new inventoryItem { itemName = "Bobby Pin", itemCode = "bobbypin", interactionEvent = "Lockpick.StartVehicleLockpick", interactionAction = "are using a", interactionData = "0", isIllegal = true, isUseable = true, itemWeight = 3.0f, removeItemOnUse = false },
            ["Brandy"] = new inventoryItem { itemName = "Brandy", itemCode = "Brandy", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "high", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Chocolate"] = new inventoryItem { itemName = "Chocolate", itemCode = "Chocolate", interactionEvent = "Player.DoHeal", interactionAction = "ate a piece of", interactionData = "Chocolate", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Cigarette"] = new inventoryItem { itemName = "Cigarette", itemCode = "Cigarette", interactionEvent = "Player.DoSmoke", interactionAction = "smoked a", interactionData = "Cigarette", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Doughnut"] = new inventoryItem { itemName = "Doughnut", itemCode = "Doughnut", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Doughnut", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["eColaCan"] = new inventoryItem { itemName = "eCola Can", itemCode = "eColaCan", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "eColaCan", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["EgochaserBar"] = new inventoryItem { itemName = "EgoChaser Bar", itemCode = "EgochaserBar", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "EgochaserBar", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["FirstAid"] = new inventoryItem { itemName = "First aid kit", itemCode = "FirstAid", interactionEvent = "Player.DoHeal", interactionAction = "used a", interactionData = "FirstAid", isIllegal = false, isUseable = true, itemWeight = 5.0f },
            ["FrozenPeas"] = new inventoryItem { itemName = "Frozen Peas", itemCode = "FrozenPeas", interactionEvent = "Player.DoHeal", interactionAction = "ate", interactionData = "FrozenPeas", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["FrozenPizza"] = new inventoryItem { itemName = "Frozen Pizza", itemCode = "FrozenPizza", interactionEvent = "Player.DoHeal", interactionAction = "ate", interactionData = "FrozenPizza", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Gin"] = new inventoryItem { itemName = "Gin", itemCode = "Gin", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "high", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Vodka"] = new inventoryItem { itemName = "Vodka", itemCode = "Vodka", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "high", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["MeteoriteBar"] = new inventoryItem { itemName = "Meteorite Bar", itemCode = "MeteoriteBar", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "MeteoriteBar", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Orange"] = new inventoryItem { itemName = "Orange", itemCode = "Orange", interactionEvent = "Player.DoHeal", interactionAction = "ate an", interactionData = "Orange", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["OrangeJuice"] = new inventoryItem { itemName = "Orange Juice", itemCode = "OrangeJuice", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "OrangeJuice", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Pasta"] = new inventoryItem { itemName = "Pasta", itemCode = "Pasta", interactionEvent = "Player.DoHeal", interactionAction = "ate", interactionData = "Pasta", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Pear"] = new inventoryItem { itemName = "Pear", itemCode = "Pear", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "Pear", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Pineapple"] = new inventoryItem { itemName = "Pineapple", itemCode = "Pineapple", interactionEvent = "Player.DoHeal", interactionAction = "ate a whole", interactionData = "Pineapple", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["RedWine"] = new inventoryItem { itemName = "Red Wine", itemCode = "RedWine", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "low", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Rum"] = new inventoryItem { itemName = "Rum", itemCode = "Rum", interactionEvent = "Player.DoDrunkEffect", interactionAction = "drank", interactionData = "high", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["SilverClaw"] = new inventoryItem { itemName = "Silver Claw", itemCode = "SilverClaw", interactionEvent = "Player.DoHeal", interactionAction = "drank", interactionData = "SilverClaw", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["Soup"] = new inventoryItem { itemName = "Soup", itemCode = "Soup", interactionEvent = "Player.DoHeal", interactionAction = "ate", interactionData = "Soup", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["SprunkCan"] = new inventoryItem { itemName = "Sprunk Can", itemCode = "SprunkCan", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "SprunkCan", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["WhiteWine"] = new inventoryItem { itemName = "White Wine", itemCode = "WhiteWine", interactionEvent = "Player.DoDrunkEffect", interactionAction = "low", interactionData = "low", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            ["ZebraBar"] = new inventoryItem { itemName = "Zebra Bar", itemCode = "ZebraBar", interactionEvent = "Player.DoHeal", interactionAction = "ate a", interactionData = "ZebraBar", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            // Fishing / Fishing Store
            ["bcatfish"] = new inventoryItem { itemName = "Blue Catfish", itemCode = "bcatfish", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["bullhead"] = new inventoryItem { itemName = "Black Bullhead", itemCode = "bullhead", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 4.0f },
            ["fishingrod"] = new inventoryItem { itemName = "Fishing rod", itemCode = "fishingrod", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 12.0f },
            ["gsturgeon"] = new inventoryItem { itemName = "Green Sturgeon", itemCode = "gsturgeon", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 6.0f },
            // Black Market Store
            ["Lockpick"] = new inventoryItem { itemName = "Lockpick", itemCode = "Lockpick", interactionEvent = "Lockpick.StartVehicleLockpick", interactionAction = "are using a", interactionData = "1", isIllegal = true, isUseable = true, itemWeight = 5.0f, removeItemOnUse = false},
            ["rifleammo"] = new inventoryItem { itemName = "Rifle ammo", itemCode = "rifleammo", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["shotgunammo"] = new inventoryItem { itemName = "Shotgun ammo", itemCode = "shotgunammo", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["smgammo"] = new inventoryItem { itemName = "SMG ammo", itemCode = "smgammo", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            // Hardware Store
            ["Binoculars"] = new inventoryItem { itemName = "Binoculars", itemCode = "Binoculars", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 6.0f },
            ["Radio"] = new inventoryItem { itemName = "Radio", itemCode = "Radio", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 3.0f },
            ["RepKit"] = new inventoryItem { itemName = "Repair Kit", itemCode = "RepKit", interactionEvent = "Vehicle.StartRepair", interactionAction = "are using a", interactionData = "0", isIllegal = false, isUseable = true, itemWeight = 15.0f, removeItemOnUse = false},
            ["zipties"] = new inventoryItem { itemName = "Zipties", itemCode = "zipties", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = true, isUseable = false, itemWeight = 3.0f },
            ["camera"] = new inventoryItem { itemName = "News Camera", itemCode = "camera", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["Coffee"] = new inventoryItem { itemName = "Coffee", itemCode = "Coffee", interactionEvent = "Player.DoDrink", interactionAction = "drank", interactionData = "Coffee", isIllegal = false, isUseable = true, itemWeight = 1.0f },
            // Hunting Items
            ["Coyote_meat"] = new inventoryItem { itemName = "Coyote meat", itemCode = "Coyote_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["Coyote_pelts"] = new inventoryItem { itemName = "Coyote pelts", itemCode = "Coyote_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["Coyote_skin"] = new inventoryItem { itemName = "Coyote teeth", itemCode = "Coyote_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 4.0f },
            ["Deer_antlers"] = new inventoryItem { itemName = "Deer antler", itemCode = "Deer_antlers", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 20.0f },
            ["Deer_meat"] = new inventoryItem { itemName = "Deer meat", itemCode = "Deer_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 4.0f },
            ["Deer_pelts"] = new inventoryItem { itemName = "Deer pelts", itemCode = "Deer_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["Deer_skin"] = new inventoryItem { itemName = "Deer skin", itemCode = "Deer_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 8.0f },
            ["HammerShark_meat"] = new inventoryItem { itemName = "Hammer Shark meat", itemCode = "HammerShark_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 22.0f },
            ["HammerShark_pelts"] = new inventoryItem { itemName = "Hammer Shark pelts", itemCode = "HammerShark_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["HammerShark_skin"] = new inventoryItem { itemName = "Hammer Shark skin", itemCode = "HammerShark_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 24.0f },
            ["KillerWhale_meat"] = new inventoryItem { itemName = "Killer Whale meat", itemCode = "KillerWhale_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 20.0f },
            ["KillerWhale_pelts"] = new inventoryItem { itemName = "Killer Whale pelts", itemCode = "KillerWhale_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 5.0f },
            ["KillerWhale_skin"] = new inventoryItem { itemName = "Killer Whale skin", itemCode = "KillerWhale_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 25.0f },
            ["MountainLion_claws"] = new inventoryItem { itemName = "Mountain Lion claw", itemCode = "MountainLion_claws", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 1.0f },
            ["MountainLion_meat"] = new inventoryItem { itemName = "Mountain Lion meat", itemCode = "MountainLion_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 6.0f },
            ["MountainLion_pelts"] = new inventoryItem { itemName = "Mountain Lion pelts", itemCode = "MountainLion_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["MountainLion_skin"] = new inventoryItem { itemName = "Mountain Lion skin", itemCode = "MountainLion_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 8.0f },
            ["MountainLion_teeth"] = new inventoryItem { itemName = "Mountain Lion teeth", itemCode = "MountainLion_teeth", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 1.0f },
            ["Rabbit_meat"] = new inventoryItem { itemName = "Rabbit meat", itemCode = "Rabbit_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 3.0f },
            ["Rabbit_pelts"] = new inventoryItem { itemName = "Rabbit pelts", itemCode = "Rabbit_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["Rabbit_skin"] = new inventoryItem { itemName = "Rabbit skin", itemCode = "Rabbit_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 5.0f },
            ["TigerShark_meat"] = new inventoryItem { itemName = "Tiger Shark meat", itemCode = "TigerShark_meat", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 6.0f },
            ["TigerShark_pelts"] = new inventoryItem { itemName = "Tiger Shark pelts", itemCode = "TigerShark_pelts", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 2.0f },
            ["TigerShark_skin"] = new inventoryItem { itemName = "Tiger Shark skin", itemCode = "TigerShark_skin", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 10.0f },
            ["CuffLockpick"] = new inventoryItem { itemName = "Cuff Lockpick", itemCode = "CuffLockpick", interactionEvent = "", interactionAction = "are using a", interactionData = "0", isIllegal = true, isUseable = true, itemWeight = 7.0f },
            ["DebitCard"] = new inventoryItem { itemName = "Bank Card", itemCode = "DebitCard", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["DirtyMoney"] = new inventoryItem { itemName = "Dirty Money", itemCode = "DirtyMoney", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["mic"] = new inventoryItem { itemName = "Microphone", itemCode = "mic", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["MobilePhone"] = new inventoryItem { itemName = "Phone", itemCode = "MobilePhone", interactionEvent = "", interactionAction = "", interactionData = "5", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            // Weapons / Gun Store
            ["pistolammo"] = new inventoryItem { itemName = "Pistol ammo", itemCode = "pistolammo", interactionEvent = "", interactionAction = "", interactionData = "", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_ADVANCEDRIFLE"] = new inventoryItem { itemName = "MTAR-21", itemCode = "WEAPON_ADVANCEDRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_APPISTOL"] = new inventoryItem { itemName = "AP Pistol", itemCode = "WEAPON_APPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_ASSAULTRIFLE"] = new inventoryItem { itemName = "AK-47", itemCode = "WEAPON_ASSAULTRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_ASSAULTSHOTGUN"] = new inventoryItem { itemName = "AA-12", itemCode = "WEAPON_ASSAULTSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_ASSAULTSMG"] = new inventoryItem { itemName = "P90", itemCode = "WEAPON_ASSAULTSMG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BAT"] = new inventoryItem { itemName = "Baseball bat", itemCode = "WEAPON_BAT", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BALL"] = new inventoryItem { itemName = "Ball", itemCode = "WEAPON_BALL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BATTLEAXE"] = new inventoryItem { itemName = "Battle Axe", itemCode = "WEAPON_BATTLEAXE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BOTTLE"] = new inventoryItem { itemName = "Broken Bottle", itemCode = "WEAPON_BOTTLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BULLPUPRIFLE"] = new inventoryItem { itemName = "Type 56", itemCode = "WEAPON_BULLPUPRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BULLPUPSHOTGUN"] = new inventoryItem { itemName = "KSG", itemCode = "WEAPON_BULLPUPSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_BZGAS"] = new inventoryItem { itemName = "BZ Gas", itemCode = "WEAPON_BZGAS", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_CARBINERIFLE"] = new inventoryItem { itemName = "M416", itemCode = "WEAPON_CARBINERIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_COMBATMG"] = new inventoryItem { itemName = "M60E4", itemCode = "WEAPON_COMBATMG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_COMBATPDW"] = new inventoryItem { itemName = "MPX-SD", itemCode = "WEAPON_COMBATPDW", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_COMBATPISTOL"] = new inventoryItem { itemName = "Combat pistol", itemCode = "WEAPON_COMBATPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_COMPACTRIFLE"] = new inventoryItem { itemName = "AK-47u", itemCode = "WEAPON_COMPACTRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_CROWBAR"] = new inventoryItem { itemName = "Crowbar", itemCode = "WEAPON_CROWBAR", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_DAGGER"] = new inventoryItem { itemName = "Dagger", itemCode = "WEAPON_DAGGER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_DBSHOTGUN"] = new inventoryItem { itemName = "Double barrel shotgun", itemCode = "WEAPON_DBSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_FIREEXTINGUISHER"] = new inventoryItem { itemName = "Fire extinguisher", itemCode = "WEAPON_FIREEXTINGUISHER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_FIREWORK"] = new inventoryItem { itemName = "Firework launcher", itemCode = "WEAPON_FIREWORK", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_FLARE"] = new inventoryItem { itemName = "Flare", itemCode = "WEAPON_FLARE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_FLAREGUN"] = new inventoryItem { itemName = "Flare gun", itemCode = "WEAPON_FLAREGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_FLASHLIGHT"] = new inventoryItem { itemName = "Flashlight", itemCode = "WEAPON_FLASHLIGHT", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_GOLFCLUB"] = new inventoryItem { itemName = "Golf club", itemCode = "WEAPON_GOLFCLUB", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_GRENADE"] = new inventoryItem { itemName = "Grenade", itemCode = "WEAPON_GRENADE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_GRENADELAUNCHER"] = new inventoryItem { itemName = "Grenade launcher", itemCode = "WEAPON_GRENADELAUNCHER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_GRENADELAUNCHER_SMOKE"] = new inventoryItem { itemName = "Smoke grenade launcher", itemCode = "WEAPON_GRENADELAUNCHER_SMOKE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_GUSENBERG"] = new inventoryItem { itemName = "Tommy gun", itemCode = "WEAPON_GUSENBERG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_HAMMER"] = new inventoryItem { itemName = "Hammer", itemCode = "WEAPON_HAMMER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_HATCHET"] = new inventoryItem { itemName = "Hatchet", itemCode = "WEAPON_HATCHET", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_HEAVYPISTOL"] = new inventoryItem { itemName = "M1911", itemCode = "WEAPON_HEAVYPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_HEAVYSHOTGUN"] = new inventoryItem { itemName = "S12", itemCode = "WEAPON_HEAVYSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_HOMINGLAUNCHER"] = new inventoryItem { itemName = "Homing launcher", itemCode = "WEAPON_HOMINGLAUNCHER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_KNIFE"] = new inventoryItem { itemName = "Knife", itemCode = "WEAPON_KNIFE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_KNUCKLE"] = new inventoryItem { itemName = "Brass knuckles", itemCode = "WEAPON_KNUCKLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MACHETE"] = new inventoryItem { itemName = "Machete", itemCode = "WEAPON_MACHETE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MACHINEPISTOL"] = new inventoryItem { itemName = "TEC-9", itemCode = "WEAPON_MACHINEPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MARKSMANPISTOL"] = new inventoryItem { itemName = "Marksman pistol", itemCode = "WEAPON_MARKSMANPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MARKSMANRIFLE"] = new inventoryItem { itemName = "Marksman rifle", itemCode = "WEAPON_MARKSMANRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MG"] = new inventoryItem { itemName = "MK48", itemCode = "WEAPON_MG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MICROSMG"] = new inventoryItem { itemName = "Mini uzi", itemCode = "WEAPON_MICROSMG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MINISMG"] = new inventoryItem { itemName = "Mini smg", itemCode = "WEAPON_MINISMG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MINIGUN"] = new inventoryItem { itemName = "Minigun", itemCode = "WEAPON_MINIGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MOLOTOV"] = new inventoryItem { itemName = "Molotov(s)", itemCode = "WEAPON_MOLOTOV", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_MUSKET"] = new inventoryItem { itemName = "Musket", itemCode = "WEAPON_MUSKET", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_NIGHTSTICK"] = new inventoryItem { itemName = "Nightstick", itemCode = "WEAPON_NIGHTSTICK", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },

            ["WEAPON_PETROLCAN"] = new inventoryItem { itemName = "Petrol can", itemCode = "WEAPON_PETROLCAN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["GADGET_PARACHUTE"] = new inventoryItem { itemName = "Parachute", itemCode = "GADGET_PARACHUTE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },

            ["WEAPON_PIPEBOMB"] = new inventoryItem { itemName = "Pipe bomb", itemCode = "WEAPON_PIPEBOMB", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_PISTOL"] = new inventoryItem { itemName = "Pistol", itemCode = "WEAPON_PISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_PISTOL50"] = new inventoryItem { itemName = ".50 Pistol", itemCode = "WEAPON_PISTOL50", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_POOLCUE"] = new inventoryItem { itemName = "Pool Cue", itemCode = "WEAPON_POOLCUE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_PROXMINE"] = new inventoryItem { itemName = "Proximity mine", itemCode = "WEAPON_PROXMINE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_PUMPSHOTGUN"] = new inventoryItem { itemName = "SPAS-12", itemCode = "WEAPON_PUMPSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_REVOLVER"] = new inventoryItem { itemName = "Colt python", itemCode = "WEAPON_REVOLVER", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_RPG"] = new inventoryItem { itemName = "RPG", itemCode = "WEAPON_RPG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SAWNOFFSHOTGUN"] = new inventoryItem { itemName = "Sawn-off shotgun", itemCode = "WEAPON_SAWNOFFSHOTGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SMG"] = new inventoryItem { itemName = "MP5", itemCode = "WEAPON_SMG", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SMOKEGRENADE"] = new inventoryItem { itemName = "Smoke grenade", itemCode = "WEAPON_SMOKEGRENADE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SNIPERRIFLE"] = new inventoryItem { itemName = "Sniper rifle", itemCode = "WEAPON_SNIPERRIFLE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SNOWBALL"] = new inventoryItem { itemName = "Snowball", itemCode = "WEAPON_SNOWBALL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = false, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SNSPISTOL"] = new inventoryItem { itemName = "SNS Pistol", itemCode = "WEAPON_SNSPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SPECIALCARBINE"] = new inventoryItem { itemName = "G36C", itemCode = "WEAPON_SPECIALCARBINE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_STICKYBOMB"] = new inventoryItem { itemName = "Sticky bomb", itemCode = "WEAPON_STICKYBOMB", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_STUNGUN"] = new inventoryItem { itemName = "Tazer", itemCode = "WEAPON_STUNGUN", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_SWITCHBLADE"] = new inventoryItem { itemName = "Switchblade", itemCode = "WEAPON_SWITCHBLADE", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_VINTAGEPISTOL"] = new inventoryItem { itemName = "Vintage pistol", itemCode = "WEAPON_VINTAGEPISTOL", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
            ["WEAPON_WRENCH"] = new inventoryItem { itemName = "Wrench", itemCode = "WEAPON_WRENCH", interactionEvent = "", interactionAction = "", interactionData = "0", isIllegal = true, isUseable = false, itemWeight = 0.0f },
        };

        public static bool DoesInvItemExist(string itemKey)
        {
            return inventoryItems.FirstOrDefault(o => String.Equals(o.Value.itemCode, itemKey, StringComparison.CurrentCultureIgnoreCase)).Value != null || inventoryItems.FirstOrDefault(o => String.Equals(o.Value.itemName, itemKey, StringComparison.CurrentCultureIgnoreCase)).Value != null;
        }

        public static inventoryItem GetInvItemData(string itemKey)
        {
            inventoryItem invItem = inventoryItems.FirstOrDefault(o => String.Equals(o.Value.itemCode, itemKey, StringComparison.CurrentCultureIgnoreCase)).Value ??
                                    inventoryItems.FirstOrDefault(o => String.Equals(o.Value.itemName, itemKey, StringComparison.CurrentCultureIgnoreCase)).Value;
            return invItem;
        }
    }

}
