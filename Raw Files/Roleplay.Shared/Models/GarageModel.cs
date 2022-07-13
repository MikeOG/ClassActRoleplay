using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;

namespace Roleplay.Shared.Models
{
    public class GarageModel
    {
        public string Name;
        public string AlternateDisplayName;

        [JsonIgnore]
        public string DisplayName => string.IsNullOrEmpty(AlternateDisplayName) ? Name : AlternateDisplayName;
        public Vector3 Location;
        public int MaxVehicles = -1;
        public MarkerOptions MarkerOptions;
        public BlipOptions BlipOptions = new BlipOptions
        {
#if CLIENT
            Sprite = BlipSprite.Garage2
#elif SERVER
            Sprite = 50
#endif
        };

        public bool Equals(GarageModel garage)
        {
            return !ReferenceEquals(garage, null) && Name == garage.Name;
        }
        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((GarageModel)obj);
        }

        public static bool operator ==(GarageModel left, GarageModel right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(GarageModel left, GarageModel right)
        {
            return !(left == right);
        }
    }
}
