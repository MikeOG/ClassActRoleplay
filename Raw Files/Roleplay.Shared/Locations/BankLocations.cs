using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Shared.Locations
{
    public static class BankLocations
    {
        public static Dictionary<string, Vector3> Positions = new Dictionary<string, Vector3>
        {
            ["PillboxMain"] = new Vector3(149.8642f, -1040.2297f, 28.4f),
            ["PillboxVault"] = new Vector3(146.7505f, -1044.8401f, 28.3778f),

            ["PaletoMain"] = new Vector3(-113.5642f, 6469.3960f, 30.7f),
            ["PaletoVault"] = new Vector3(-103.7352f, 6477.9512f, 30.6267f),

            ["GreatOceanMain"] = new Vector3(-2963.234f, 482.989f, 14.8f),
            ["GreatOceanVault"] = new Vector3(-2957.642f, 481.828f, 14.697f),

            ["Route68Main"] = new Vector3(1175.14f, 2706.39f, 37.394f),
            ["Route68Vault"] = new Vector3(1176.6f, 2711.61f, 37.3978f),

            ["HawickAltaMain"] = new Vector3(314.16f, -279.11f, 54.17f),
            ["HawickAltaVault"] = new Vector3(311.77f, -283.32f, 54.16f),

            ["HawickBurtonMain"] = new Vector3(-351.16f, -49.83f, 49.04f),
            ["HawickBurtonVault"] = new Vector3(-353.49f, -54.26f, 49.04f),

            ["PacificStandardnMain"] = new Vector3(248.37f, 222.88f, 106.29f),
            ["PacificStandardVault"] = new Vector3(253.72f, 225.27f, 101.88f),

            ["JewelryStoreMain"] = new Vector3(-1212.86f, -330.82f, 39.76f),
            ["JewelryStoreVault"] = new Vector3(-623.53f, -230.15f, 38.06f),
        };
    }
}
