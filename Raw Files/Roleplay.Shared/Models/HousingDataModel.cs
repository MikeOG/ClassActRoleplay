using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Shared.Models
{
    public class HousingDataModel
    {
        public int HouseId;
        public string HouseType;
        public Vector3 EntranceLocation;
        public string HouseAddress;
        public int HousePrice;
        public Vector3 GarageLocation;
        public int MaxGarageVehicles;
        public List<int> HouseEnterAccess = new List<int>();
    }
}
