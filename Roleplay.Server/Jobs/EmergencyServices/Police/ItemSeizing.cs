using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Helpers;
using Roleplay.Server.Vehicle;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class ItemSeizing : JobClass
    {
        public ItemSeizing()
        {
            CommandRegister.RegisterJobCommand("seizeitems", OnSeizeItems, JobType.Police);
            CommandRegister.RegisterJobCommand("seizecash|seizemoney", OnSeizeMoney, JobType.Police);
            //CommandRegister.RegisterJobCommand("seizeitem", OnSeizeSpecificItem, JobType.Police);
        }

        private void OnSeizeItems(Command cmd)
        {
            var closePlayer = cmd.Session.GetClosestPlayer(3.0f, o => o.CuffState != CuffState.None);

            if (closePlayer == null) return;

            var itemsString = string.Join(" ", cmd.Args);

            if (!string.IsNullOrEmpty(itemsString))
            {
                OnSeizeSpecificItems(cmd.Session, closePlayer, itemsString);
                return;
            }

            var playerInv = closePlayer.Inventory;

            playerInv.RemoveIllegalItems();

            Log.ToClient("[Police]", "You seized this persons items", ConstantColours.Police, cmd.Player);
            Log.ToClient("[Police]", "Your items were seized", ConstantColours.Police, closePlayer.Source);
        }

        private void OnSeizeSpecificItems(Session.Session sourceSession, Session.Session targetSession, string itemsString)
        {
            var itemsToRemove = itemsString.Split(';').ToList();

            Log.Verbose($"{sourceSession.PlayerName} is removing the following items from {targetSession.PlayerName}. {string.Join(",", itemsToRemove)}");

            RemoveInventoryItems(targetSession.Inventory, itemsToRemove);

            sourceSession.Message("[Police]", $"You seized the following items from this person: {string.Join(", ", itemsToRemove)}", ConstantColours.Police);
            targetSession.Message("[Police]", $"The following items were taken off you: {string.Join(", ", itemsToRemove)}", ConstantColours.Police);
        }

        [EventHandler("Items.SeizeVehicleItems")]
        private void OnSeizeVehicleItems([FromSource] Player source, int vehId, string itemsString)
        {
            var playerSession = Sessions.GetPlayer(source);
            var targetVeh = Server.Get<VehicleManager>().GetVehicle(vehId);

            if (playerSession == null || targetVeh == null || !JobHandler.OnDutyAs(playerSession, JobType.Police)) return;

            if (!string.IsNullOrEmpty(itemsString))
            {
                OnSeizeSpecificVehicleItems(playerSession, targetVeh, itemsString);
                return;
            }

            Log.Verbose($"{source.Name} is removing the illegal items from vehicle #{targetVeh.VehID}");

            targetVeh.Inventory.RemoveIllegalItems();

            playerSession.Message("[Police]", "You seized the illegal items in this vehicle", ConstantColours.Police);
        }

        private void OnSeizeSpecificVehicleItems(Session.Session playerSession, Vehicle.Models.Vehicle targetVeh, string itemsString)
        {
            var itemsToRemove = itemsString.Split(';').ToList();

            Log.Verbose($"{playerSession.PlayerName} is removing the following items from vehicle #{targetVeh.VehID}");

            RemoveInventoryItems(targetVeh.Inventory, itemsToRemove);

            playerSession.Message("[Police]", $"You seized the following items from this vehicle: {string.Join(", ", itemsToRemove)}", ConstantColours.Police);
        }

        private void OnSeizeMoney(Command cmd)
        {
            var closePlayer = cmd.Session.GetClosestPlayer(3.0f, o => o.CuffState != CuffState.None);

            if (closePlayer == null) return;

            closePlayer.SetGlobalData("Character.Cash", 0);

            Log.ToClient("[Police]", "You seized this persons cash", ConstantColours.Police, cmd.Player);
            Log.ToClient("[Police]", "Your cash were seized", ConstantColours.Police, closePlayer.Source);
        }

        private void RemoveInventoryItems(Inventory inv, List<string> items)
        {
            foreach (var item in items)
            {
                inv.RemoveItem(item);
            }
        }
    }
}
