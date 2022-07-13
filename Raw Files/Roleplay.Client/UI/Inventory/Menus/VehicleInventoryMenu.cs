using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Client.Helpers;
using Roleplay.Client.Jobs;
using Roleplay.Client.Property;
using Roleplay.Client.Vehicles;
using Roleplay.Shared.Enums;
using MenuFramework;

namespace Roleplay.Client.UI.Inventory.Menus
{
    public sealed class VehicleInventoryMenu : InventoryMenu
    {
        public override string MenuHeaderTitle { get; set; } = "Vehicle Inventory";
        public override string ItemDescription { get; set; } = "This vehicle has {0} of this item with a weight of {1}";

        public VehicleInventoryMenu(Shared.Inventory targetInv)
            : base(targetInv)
        {

        }

        public VehicleInventoryMenu(string invString)
            : base(invString)
        {

        }

        public override bool CanViewMenu() => !Client.Instance.Get<PropertyHandler>().IsNearPropertyStorage();

        public override Action<MenuItemSubMenu> GetOnActivateFunction()
        {
            return item =>
            {
                RefreshMenu("");
                var closeOwnedVeh = Client.Instance.Get<JobHandler>().OnDutyAsJob(JobType.Police) ? GTAHelpers.GetClosestVehicle(3.0f, o => o.HasDecor("Vehicle.ID")) : Client.Instance.Get<VehicleHandler>().GetClosestVehicleWithKeys();
                if (closeOwnedVeh != null)
                {
                    Client.Instance.TriggerServerEvent("Inventory.RequestInventory", closeOwnedVeh.GetDecor<int>("Vehicle.ID"));
                }
            };
        }

        protected override List<InventoryItemInteractable> getMenuInteractables()
        {
            return new List<InventoryItemInteractable>
            {
                InventoryMenuInteractions.TakeFromVehItem
            };
        }
    }
}
