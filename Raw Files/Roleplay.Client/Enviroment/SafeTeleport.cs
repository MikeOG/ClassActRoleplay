using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace Roleplay.Client.Enviroment
{
    public class SafeTeleport
    {
        public static async Task Teleport(int entity, Position position, int interval = 10)
        {
            API.RequestCollisionAtCoord(position.X, position.Y, position.Z);
            API.RequestAdditionalCollisionAtCoord(position.X, position.Y, position.Z);
            API.SetEntityCoordsNoOffset(entity, position.X, position.Y, position.Z, false, false, false);
            API.SetEntityHeading(entity, position.Heading);

            while (!API.HasCollisionLoadedAroundEntity(entity))
            {
                API.RequestCollisionAtCoord(position.X, position.Y, position.Z);
                API.RequestAdditionalCollisionAtCoord(position.X, position.Y, position.Z);

                await BaseScript.Delay(interval);
            }

            API.SetEntityCoordsNoOffset(entity, position.X, position.Y, position.Z, false, false, false);
            API.SetEntityHeading(entity, position.Heading);
        }
    }
}