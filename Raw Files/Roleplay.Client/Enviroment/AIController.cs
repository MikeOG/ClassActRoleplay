using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Enviroment
{
    class AIController : ClientAccessor
    {
        public AIController(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
            SetMaxWantedLevel(0);
        }

        private async Task OnTick()
        {
            API.SetGarbageTrucks(true);
            API.SetRandomBoats(true);
            API.SetCreateRandomCops(false);
            API.SetCreateRandomCopsNotOnScenarios(false);
            API.SetCreateRandomCopsOnScenarios(false);
            API.EnableDispatchService(1, false); // Police Automobile
            API.EnableDispatchService(2, false); // Police Helicopter
            API.EnableDispatchService(3, false); // Ambulance
            API.EnableDispatchService(4, false); //Swat Automobile
            API.EnableDispatchService(5, false); // Ambulance Department
            API.EnableDispatchService(6, false); //Police Riders
            API.EnableDispatchService(7, false); // Police Vehicle Request
            API.EnableDispatchService(8, false); // Police Road Block
            API.EnableDispatchService(9, false); // PoliceAutomobileWaitPulledOver 
            API.EnableDispatchService(10, false); // PoliceAutomobilewaitcrusing
            API.EnableDispatchService(11, false); // Gangs
            API.EnableDispatchService(12, false); // Swat Helicopter
            API.EnableDispatchService(13, false); //Police Boat
            API.EnableDispatchService(14, false); //Army Vehicles
            API.EnableDispatchService(15, false); // Biker Backup
        }
    }
}