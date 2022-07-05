using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Helpers
{
    // NOTE this is all experimental and will need to be tested to see if it actaully works properly
    public static class NetworkExtensions
    {
        public static async Task<int> NetworkEntity(this Entity entity)
        {
            int netId = NetworkGetNetworkIdFromEntity(entity.Handle);
            int attempts = 0;
            while (!NetworkDoesNetworkIdExist(netId) && attempts < 10)
            {
                await BaseScript.Delay(50);
                netId = NetworkGetNetworkIdFromEntity(entity.Handle);
                NetworkRegisterEntityAsNetworked(entity.Handle);
                entity.IsPersistent = true;
                SetNetworkIdCanMigrate(netId, true);
                SetNetworkIdExistsOnAllMachines(netId, true);
                NetworkRequestControlOfEntity(entity.Handle);
                attempts += 1;
            }

            return netId;
        }

        public static async Task<int> GetNetworkId(this Entity entity)
        {
            int netId = NetworkGetNetworkIdFromEntity(entity.Handle);
            if (!NetworkDoesNetworkIdExist(netId))
                netId = await NetworkEntity(entity);

            return netId;
        }

        public static async Task<bool> GetControlOfEntity(this Entity entity)
        {
            int netId = await entity.GetNetworkId();
            if (!NetworkDoesNetworkIdExist(netId)) return false;
            if (NetworkHasControlOfNetworkId(netId))
            {
                return true;
            }
            else
            {
                int attempts = 0;
                while(!NetworkHasControlOfNetworkId(netId) && attempts < 10)
                {
                    await BaseScript.Delay(50);
                    NetworkRequestControlOfNetworkId(netId);
                    attempts += 1;
                }

                if(attempts >= 10 || !NetworkHasControlOfNetworkId(netId))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool HasControlOfEntity(this Entity entity)
        {
            return NetworkHasControlOfEntity(entity.Handle);
        }
    }
}
