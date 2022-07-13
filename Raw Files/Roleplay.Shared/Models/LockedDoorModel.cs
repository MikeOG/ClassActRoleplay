using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Enums;

namespace Roleplay.Shared.Models
{
    public class LockedDoorModel
    {
        public int DoorId;
        public Vector3 Location;
        public bool LockState;
        public string Model;
        public float InitialHeading;
        public bool CanOpenDoor = false;

#if SERVER
        public JobType RequiredJobType = (JobType)(-1); // None
        /// <summary>
        /// The required job groups / principals needed. If this is set then <see cref="RequiredJobType"/> will be ignored. Split by ;
        /// </summary>
        public string RequiredJobPermissions = "";
#endif
    }
}
