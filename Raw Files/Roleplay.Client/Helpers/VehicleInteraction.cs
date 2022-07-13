using CitizenFX.Core;
using Roleplay.Shared;
using System;

namespace Roleplay.Client.Helpers
{
    internal class VehicleInteraction
    {
        internal static int GetClosetVehicleAtPosition(Vector3 parkingSpot, float distanceThreshold = 16f)
        {
            try
            {
                foreach (int num in new VehicleList())
                {
                    Entity entity = Entity.FromHandle(num);
                    if (((PoolObject)entity).Exists())
                    {
                        Vector3 position = entity.Position;
                        if ((double)((Vector3)position).DistanceToSquared(parkingSpot) < (double)distanceThreshold)
                            return num;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return -1;
        }
    }
}
