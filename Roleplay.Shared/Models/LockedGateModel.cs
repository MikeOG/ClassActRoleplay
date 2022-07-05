using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Enums;

namespace Roleplay.Shared.Models
{
    public enum GateMovement
    {
        RightToLeft = 1,
        LeftToRight = -1
    }

    public class LockedGateModel
    {
        public int GateId;
        public Vector3 Location;
        public bool LockState;
        public string Model;
        public float InitialHeading;
        public bool CanOpenGate = false;
        public GateMovement OpeningMovement;

#if SERVER
        public JobType RequiredJobType = (JobType)(-1); // None
        public string RequiredJobPermissions = "";
#endif
    }
}
