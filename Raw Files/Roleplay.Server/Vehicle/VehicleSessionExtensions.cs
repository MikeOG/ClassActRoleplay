using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        /// <summary>
        /// Stores all garages this session has access to
        /// </summary>
        public List<GarageModel> Garages = new List<GarageModel>();

        public void AddGarage(GarageModel garage, bool registerOnClient = true)
        {
            if (garage == null) return;

            if (!Garages.Contains(garage))
            {
                Log.Verbose($"Adding garage {garage.Name} to {PlayerName}");

                Garages.Add(garage);

                if(registerOnClient) TriggerEvent("Vehicle.Garage.AddGarage", JsonConvert.SerializeObject(garage));
            }
        }

        public void AddGarages(List<GarageModel> garages)
        {
            foreach (var garage in garages)
            {
                AddGarage(garage, false);
            }

            TriggerEvent("Vehicle.Garage.AddGarage", JsonConvert.SerializeObject(garages));
        }

        public void RemoveGarage(GarageModel garageToRemove)
        {
            var garage = Garages.FirstOrDefault(o => o == garageToRemove);

            if (garage == null) return;

            Log.Verbose($"Removing garage {garage.Name} from {PlayerName}' garage list");

            Garages.Remove(garage);

            TriggerEvent("Vehicle.Garage.RemoveGarage", garage.Name);
        }

        public GarageModel GetClosestGarage(float distance = 9.0f)
        {
            var playerPos = Position;
            return Garages.FirstOrDefault(o => o.Location.DistanceToSquared(playerPos) < distance);
        }
    }
}
