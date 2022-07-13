using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.MySQL;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Vehicle
{
    public class VehicleSelling : ServerAccessor
    {
        public VehicleSelling(Server server) : base(server)
        {
            
        }

        [EventHandler("Vehicle.ShowVehicleSellPrice")]
        private void OnRequestSellPrice([FromSource] Player source, int vehId)
        {
            var playerSession = Sessions.GetPlayer(source);
            var veh = Server.Get<VehicleManager>().GetVehicle(vehId);

            if (veh == null || playerSession == null) return;

            Log.ToClient("[Store]", $"Selling this vehicle will give you ${Math.Round((double)(veh.VehiclePrice / 2))} are you such you want to do this?", ConstantColours.Store, source);
            //Log.ToClient("[DEBUG]", $"Selling this vehicle will give you ${veh.VehiclePrice} are you such you want to do this?", ConstantColours.Store, source);
        }

        [EventHandler("Vehicle.SellVehicle")]
        private async void OnSellVehicle([FromSource] Player source, int vehId)
        {
            var vehManager = Server.Get<VehicleManager>();
            var playerSession = Sessions.GetPlayer(source);
            var veh = vehManager.GetVehicle(vehId);

            if (veh == null || playerSession == null) return;

            Log.ToClient("[Store]", $"Selling vehicle...", ConstantColours.Store, source);

            await BaseScript.Delay(1000);

            var vehPrice = (int)Math.Round((double)(veh.VehiclePrice / 2));

            Log.Verbose($"{source.Name} is attempting to sell vehicle #{vehId}");

            vehManager.RemoveVehicle(veh);

            Server.Get<PaymentHandler>().UpdateBankBalance(playerSession.GetBankAccount(), vehPrice, playerSession, $"selling vehicle {veh.VehicleName}");
            Log.ToClient("[Bank]", $"You just sold this vehicle for ${vehPrice}", ConstantColours.Bank, source);

            MySQL.execute("DELETE FROM vehicle_data WHERE VehID = @id", new Dictionary<string, dynamic>
            {
                ["@id"] = vehId
            }, new Action<dynamic>(rows =>
            {
                Log.Verbose($"{source.Name} just sold vehicle #{vehId} for ${vehPrice}");
            }));
        }
    }
}
