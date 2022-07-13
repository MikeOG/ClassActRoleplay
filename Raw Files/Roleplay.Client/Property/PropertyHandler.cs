using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.Property
{
    public class PropertyHandler : ClientAccessor
    {
        public PropertyHandler(Client client) : base(client)
        {

        }

        public bool IsNearPropertyStorage()
        {
            var storageLocations = LocalSession.GetLocalData("Property.AccessableStorages", new JArray()).ToObject<List<Vector3>>();
            var playerPos = Cache.PlayerPed.Position;

            return storageLocations.Any(o => o.DistanceToSquared(playerPos) < Math.Pow(3, 2));
        }
    }
}
