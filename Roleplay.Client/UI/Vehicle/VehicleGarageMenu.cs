using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuFramework;
using CitizenFX.Core;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Client.Models;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.UI.Vehicle
{
    internal class VehicleGarageMenu : ClientAccessor
    {
        private GarageMenuModel garageMenu;
        private GarageModel closestGarage = new GarageModel(); // stops errors
        private List<GarageModel> garageLocations = new List<GarageModel>();
        private List<Blip> garageBlips = new List<Blip>();
        private List<int> markerIds = new List<int>();

        public VehicleGarageMenu(Client client) : base(client)
        {
            garageMenu = new GarageMenuModel
            {
                headerTitle = "Garage",
                statusTitle = "",
                menuItems = new List<MenuItem> {new MenuItemStandard {Title = "Populating"}}
            };

            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemStandard
            {
                Title = "Garage",
                OnActivate = item =>
                {
                    populateGarageMenu();
                }
            }, () => isInRangeOfVehicleGarage(), 500);
            client.Get<InteractionUI>().RegisterInteractionMenuItem(new MenuItemStandard
            {
                Title = "Store vehicle",
                OnActivate = async item =>
                {
                    OnInteraction();
                }
            }, () => isInRangeOfVehicleGarage(false) && closestGarage.DisplayName != "Impound", 500);
            client.RegisterEventHandler("Vehicle.RecieveGarageVehicles", new Action<List<object>, bool>((garageData, isImpound) =>
            {
                garageMenu.buildGarage(garageData, isImpound);
                if (InteractionUI.Observer.CurrentMenu == null)
                    InteractionUI.Observer.OpenMenu(garageMenu);
            }));
            client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraction));
            client.RegisterTickHandler(OnTick);
            LoadBlips();
        }

        public void AddGarage(string garageName, Vector3 garagePosition)
        {
            //garageLocations.Add(garageName, garagePosition);
        }

        private bool isInRangeOfVehicleGarage(bool vehCheck = true)
        {
            bool inRange = false;
            var playerPos = Cache.PlayerPed.Position;
            foreach (var garage in garageLocations)
            {
                if (garage.Location.DistanceToSquared(playerPos) < Math.Pow(3, 2))
                {
                    inRange = true;
                    closestGarage = garage;
                    break;
                }
            }

            if(vehCheck) return inRange && !Cache.PlayerPed.IsInVehicle();
            
            return inRange;
        }

        private void populateGarageMenu()
        {
            garageMenu.menuItems = new List<MenuItem> { new MenuItemStandard { Title = "Populating..." } };
            garageMenu.SelectedIndex = garageMenu.SelectedIndex;
            if (!Game.PlayerPed.IsInVehicle())
            {
                InteractionUI.Observer.OpenMenu(garageMenu);
                Client.Instance.TriggerServerEvent("Vehicle.RequestGarageVehicles", closestGarage.Name);
            }
        }

        [EventHandler("Vehicle.Garage.AddGarage")]
        private void OnAddGarage(string garageData)
        {
            var garages = new List<GarageModel>();

            if (!garageData.TryParseJson(out garages))
            {
                garages = new List<GarageModel>
                {
                    JsonConvert.DeserializeObject<GarageModel>(garageData)
                };
            }

            foreach(var garage in garages)
            {
                if (!garageLocations.Contains(garage))
                {
                    Log.Verbose($"Adding a garage with name {garage.DisplayName} at location {garage.Location}");

                    garageLocations.Add(garage);
                }
            }
            LoadBlips();
        }

        [EventHandler("Vehicle.Garage.RemoveGarage")]
        private void OnRemoveGarage(string garageName)
        {
            var garage = garageLocations.FirstOrDefault(o => o.Name == garageName);

            if (garage == null) return;

            Log.Verbose($"Removing garage {garage.Name} from the garage list");

            garageLocations.Remove(garage);

            LoadBlips();
        }

        private async Task OnTick()
        {
            if (InteractionUI.Observer.CurrentMenu == garageMenu)
            {
                if(!isInRangeOfVehicleGarage())
                    InteractionUI.Observer.CloseMenu();
            }   
        }

        private async void OnInteraction()
        {
            var closeOwnedVeh = Client.Get<VehicleHandler>().GetClosestOwnedVehicle();
            if (Cache.PlayerPed.IsInVehicle() && closeOwnedVeh != null && closeOwnedVeh.HasDecor("Vehicle.ID") && closeOwnedVeh.GetDecor<int>("Vehicle.ID") < 1000000 && Cache.PlayerPed.CurrentVehicle == closeOwnedVeh && isInRangeOfVehicleGarage(false) && closestGarage.DisplayName != "Impound")
            {
                InteractionUI.Observer.CloseMenu(true);
                Log.Verbose($"Close to garage {closestGarage.Name} attempting to store vehicle {closeOwnedVeh.GetDecor<int>("Vehicle.ID")}");
                Client.TriggerServerEvent("Vehicle.Garage.AttemptStoreVehicle", closeOwnedVeh.GetDecor<int>("Vehicle.ID"), VehicleDataPacker.PackVehicleData(closeOwnedVeh)/*, closestGarage.Name*/);
            }
            else if (isInRangeOfVehicleGarage())
            {
                populateGarageMenu();
            }
        }

        private void LoadBlips()
        {
            garageBlips.ForEach(o => o.Delete()); garageBlips.Clear();
            markerIds.ForEach(MarkerHandler.RemoveMarker); markerIds.Clear();

            foreach (var garage in garageLocations)
            {
                var blip = BlipHandler.AddBlip(garage.DisplayName, garage.Location, garage.BlipOptions);

                garageBlips.Add(blip);
                markerIds.Add(MarkerHandler.AddMarker(garage.Location, garage.MarkerOptions));
            }
        }
    }

    class GarageMenuModel : MenuModel
    {
        public void buildGarage(List<object> garageData, bool isImpound)
        {
            headerTitle = isImpound ? "Impound lot" : "Garage";
            var _menuItems = new List<MenuItem>();
            try
            {
                foreach (var i in garageData)
                {
                    IDictionary<string, object> vehData = (IDictionary<string, object>)i;
                    _menuItems.Add(new MenuItemStandard
                    {
                        Title = isImpound ? $"(${vehData["ImpoundPrice"]}) {vehData["VehicleName"]}" : $"({vehData["VehID"]}) {vehData["VehicleName"]}",
                        OnActivate = state =>
                        {
                            try
                            {
                                InteractionUI.Observer.CloseMenu(true);
                                Client.Instance.TriggerServerEvent("Vehicle.RequestVehicleData", vehData["VehID"]);
                                _menuItems.Clear();
                                menuItems.Clear();
                                SelectedIndex = SelectedIndex;
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    });
                }
            }
            catch (Exception e) { Log.Error(e); }
            menuItems = _menuItems.Count > 0 ? _menuItems : new List<MenuItem>() { new MenuItemStandard { Title = "Empty" } };
            SelectedIndex = SelectedIndex;
        }
    }
}
