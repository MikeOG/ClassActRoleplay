using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Client.Enviroment
{
    public class PedWeaponsManipulation : ClientAccessor
    {

    public PedWeaponsManipulation(Client client) : base(client)
    {
        client.RegisterTickHandler(OnTick);
    }

    private async Task OnTick()
        {
            World.GetAllPeds().ToList().ForEach(self => API.SetPedDropsWeaponsWhenDead(self.Handle, false));
            await BaseScript.Delay(500);
        }
    }
}
