using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;

using Roleplay.Server.Jobs.Criminal.Robbery.Models;
using Roleplay.Shared;
using Roleplay.Shared.Locations;

namespace Roleplay.Server.Jobs.Criminal.Robbery
{
    public class RobberyHandler : JobClass
    {
        public IReadOnlyList<RobbableLocation> Locations => new List<RobbableLocation>(robbableLocations);

        private List<RobbableLocation> robbableLocations = new List<RobbableLocation>();

        public RobberyHandler()
        {
            foreach (var store in StoreLocations.Positions)
            {
                Log.Verbose($"Adding robbable store location {store.Key} with a vault position of {store.Value}");
                robbableLocations.Add(new RobbableStore(store.Key, store.Value));
            }
        }

        public RobbableLocation GetLocationByName(string locationName) => Locations.FirstOrDefault(o => o.LocationName == locationName);

        [EventHandler("Player.OnInteraction")]
        private void OnInteraction([FromSource] Player source)
        {

        }

        [EventHandler("Robbery.Store.AttemptRegisterRobbery")]
        private void OnAttemptRegisterRobbery([FromSource] Player source, string location, Vector3 registerPosition)
        {
            Log.Debug($"location: {location}; registerPosition: {registerPosition}");

            var session = Sessions.GetPlayer(source);

            if (session == null) return;


        }
    }
}
