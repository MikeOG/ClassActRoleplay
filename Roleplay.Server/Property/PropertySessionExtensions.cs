using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Realtor;
using Roleplay.Server.Realtor.Models;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        public bool IsInProperty => PropertyEntranceLocation != Vector3.Zero;

        /// <summary>
        /// Location where a player was before they entered a property
        /// </summary>
        public Vector3 PropertyEntranceLocation
        {
            get => GetServerData("Property.Entry.LastOutsidePosition", Vector3.Zero);
            set => SetServerData("Property.Entry.LastOutsidePosition", value);
        }

        public PropertyModel PropertyCurrentlyInside
        {
            get => GetServerData<PropertyModel>("Property.CurrentlyIn", null);
            set => SetServerData("Property.CurrentlyIn", value);
        }

        public List<Vector3> AccessablePropertyStorages
        {
            get => GetLocalData("Property.AccessableStorages", new List<Vector3>());
            set => SetLocalData("Property.AccessableStorages", value);
        }

        public PropertyModel GetClosestOwnedProperty(float distance = 6.0f)
        {
            if (IsInProperty)
            {
                if (PropertyCurrentlyInside.IsPropertyOwner(CharId))
                {
                    return PropertyCurrentlyInside;
                }
            }

            var playerPos = Position;
            return Server.Instance.Get<PropertyManager>().Properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(playerPos) < distance && o.IsPropertyOwner(CharId));
        }

        public PropertyModel GetClosestPropertyWithTenancy(float distance = 12.0f)
        {
            if (IsInProperty)
            {
                if (PropertyCurrentlyInside.IsPropertyTenant(CharId))
                {
                    return PropertyCurrentlyInside;
                }
            }

            var playerPos = Position;
            return Server.Instance.Get<PropertyManager>().Properties.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(playerPos) < distance && o.IsPropertyTenant(CharId));
        }
    }
}
