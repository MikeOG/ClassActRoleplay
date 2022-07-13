using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Shared.Models
{
    public class Drug
    {
        // What gets collected from the harvest location
        public string HarvestDrugName = "";
        public string HarvestInteractionText = "collect";
        // What is created from the process location
        public string ProcessDrugName = "";
        public string ProcessInteractionText = "process";

        public int HarvestTime;
        public int ProcessTime;

        public List<Vector3> HarvestLocations;
        public List<Vector3> ProcessLocations;
        
#if SERVER
        private Random rand = new Random();

        public int SellPrice => rand.Next(SellPriceMin, SellPriceMax);

        public int SellPriceMin = 100;
        public int SellPriceMax = 400;
#endif

        public override string ToString()
        {
            return $"Harvest Drug: {HarvestDrugName}\nProcess Drug: {ProcessDrugName}\n";
        }
    }
}

