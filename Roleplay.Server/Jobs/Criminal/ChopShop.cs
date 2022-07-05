using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Server.Jobs.Criminal
{
    internal class ChopShop : JobClass
    {
        private int currentLocationIndex = 0;
        // Add more for varying entrance locations for chop shop
        private List<Vector3> garageLocation = new List<Vector3>()
        {
            new Vector3(501.87f, -1338.86f, 28.51f),
            new Vector3(-19.97f, -1676.20f, 28.49f)
        };

        public ChopShop()
        {
            currentLocationIndex = new Random((int)DateTime.Now.Ticks).Next(0, garageLocation.Count - 1);
            Server.Instance.RegisterEventHandler("ChopShop.GetCurrentGarage", new Action<Player>(GetGarageLocation));
        }

        private void GetGarageLocation([FromSource] Player source)
        {
            var currentLocation = garageLocation[currentLocationIndex];
            source.TriggerEvent("ChopShop.SetCurrentGarage", currentLocation.ToArray().ToList());
        }
    }
}
