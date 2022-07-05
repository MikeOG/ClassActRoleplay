using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Shared.Enums
{
    [Flags]
    public enum TickUsage
    {
        All = 0,
        InVehicle = 1,
        OutVehicle = 2,
        Aiming = 4,
        NotAiming = 8,
        Shooting = 16,
        Debug = 32,
    }
}
