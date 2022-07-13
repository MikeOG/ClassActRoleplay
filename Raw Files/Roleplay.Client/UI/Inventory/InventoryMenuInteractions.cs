using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs;
using Roleplay.Client.Property;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Newtonsoft.Json;

namespace Roleplay.Client.UI.Inventory
{
    public static class InventoryMenuInteractions
    {
        public static InventoryItemInteractable UseItem = new InventoryItemInteractable
        {
            Title = "Use"
        };

        public static InventoryItemInteractable DropItem = new InventoryItemInteractable
        {
            Title = "Drop",
            InteractFunc = (item, amount) =>
            {
                if (!Game.PlayerPed.IsInVehicle())
                {
                    Client.Instance.TriggerServerEvent($"Inventory.DropInvItem", JsonConvert.SerializeObject(item), amount, Game.PlayerPed.Position.ToArray().ToList());
                }
            }
        };

        public static InventoryItemInteractable TakeFromVehItem = new InventoryItemInteractable
        {
            Title = "Take from vehicle",
            InteractFunc = (item, amount) =>
            {
                CitizenFX.Core.Vehicle closeOwnedVeh;
                if (Client.Instance.Get<JobHandler>().OnDutyAsJob(JobType.Police))
                {
                    closeOwnedVeh = GTAHelpers.GetClosestVehicle(3.0f, o => o.HasDecor("Vehicle.ID"));
                }
                else
                {
                    closeOwnedVeh = Client.Instance.Get<VehicleHandler>().GetClosestVehicleWithKeys();
                }

                if (closeOwnedVeh != null)
                {
                    Client.Instance.TriggerServerEvent($"Inventory.TakeVehInvItem", JsonConvert.SerializeObject(item), amount, closeOwnedVeh.GetDecor<int>("Vehicle.ID"));
                }
                else
                {
                    Log.ToChat("", "You don't have keys for this vehicle");
                }
            }
        };

        public static InventoryItemInteractable PutInVehItem = new InventoryItemInteractable
        {
            Title = "Put in vehicle",
            InteractFunc = (item, amount) =>
            {
                CitizenFX.Core.Vehicle closeOwnedVeh = null;
                if (Client.Instance.Get<JobHandler>().OnDutyAsJob(JobType.Police))
                {
                    closeOwnedVeh = GTAHelpers.GetClosestVehicle(3.0f, o => o.HasDecor("Vehicle.ID"));
                }
                else
                {
                    closeOwnedVeh = Client.Instance.Get<VehicleHandler>().GetClosestVehicleWithKeys();
                }

                if (closeOwnedVeh != null)
                {
                    Client.Instance.TriggerServerEvent($"Inventory.AddInvItem", JsonConvert.SerializeObject(item), amount, closeOwnedVeh.GetDecor<int>("Vehicle.ID"));
                }
                else
                {
                    Log.ToChat("", "You don't have keys for this vehicle");
                }
            }
        };

        public static InventoryItemInteractable PutInStorageItem = new InventoryItemInteractable
        {
            Title = "Put in storage",
            InteractFunc = (item, amount) =>
            {
                if (Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage())
                {
                    Client.Instance.TriggerServerEvent($"Inventory.AddInvItem", JsonConvert.SerializeObject(item), amount, "property");
                }
            }
        };

        public static InventoryItemInteractable TakeFromStorageItem = new InventoryItemInteractable
        {
            Title = "Take from storage",
            InteractFunc = (item, amount) =>
            {
                if (Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage())
                {
                    Client.Instance.TriggerServerEvent($"Inventory.TakeVehInvItem", JsonConvert.SerializeObject(item), amount, "property");
                }
            }
        };

        public static InventoryItemInteractable DummyItem = new InventoryItemInteractable
        {
            Title = "Dummy item"
        };
    }
}
