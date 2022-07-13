using System;

namespace Roleplay.Shared.Enums
{
    [Flags]
    public enum JobType
    {
        Civillian = 1,
        Police = 2,
        EMS = 4,
        Mechanic = 8,
        Tow = 16,
        Taxi = 32,
        Delivery = 64,
        Realtor = 128,
        Horsemen = 999
    }
}
