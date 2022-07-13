using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.Vehicles
{
    public class VehicleHandler : ClientAccessor
    {
        private List<Vehicle> ownedVehicles = new List<Vehicle>();
        private int saveInterval = CitizenFX.Core.Native.API.GetConvarInt("mg_clientVehicleSaveInterval", 60000);

        public VehicleHandler(Client client) : base(client)
        {
            client.RegisterEventHandler("Vehicle.SetBoughtVehID", new Action<int>(SetBoughtVehId));
            client.RegisterEventHandler("Vehicle.SpawnGarageVehicle", new Action<string, int, string>(OnSpawnRequest));
            client.RegisterEventHandler("Vehicle.ReceiveExternalVehID", new Action<int>(SetVehicleVehId));
            client.RegisterTickHandler(SaveVehicleTick);
            EntityDecoration.RegisterProperty("Vehicle.ID", DecorationType.Int);
            /*CommandRegister.RegisterCommand("testadd", cmd =>
            {
                var closeVeh = GTAHelpers.GetClosestVehicle();
                Roleplay.Client.Client.Instance.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(closeVeh));
            });
            CommandRegister.RegisterCommand("getvehid", cmd =>
            {
                Log.ToChat(GTAHelpers.GetClosestVehicle().GetDecor<int>("Vehicle.ID").ToString());
            });*/
            CommandRegister.RegisterCommand("givekeys", OnGiveKeysCommand);
            CommandRegister.RegisterCommand("giveveh|givevehicle", OnGiveVehCommand);
        }

        public Vehicle GetClosestVehicleWithKeys(float distance = 3.0f)
        {
            var closeVeh = GTAHelpers.GetClosestVehicle(distance);

            if (closeVeh != null && closeVeh.HasDecor("Vehicle.ID"))
            {
                var currentVehKeys = GetVehiclesWithKeys();
                if (currentVehKeys.Contains(closeVeh.GetDecor<int>("Vehicle.ID")))
                {
                    return closeVeh;
                }
            }

            return null;
        }

        public Vehicle GetClosestOwnedVehicle(float distance = 3.0f)
        {
            var closeVeh = Cache.PlayerPed.CurrentVehicle ?? GTAHelpers.GetClosestVehicle(distance);

            if (closeVeh != null)
            {
                if (!closeVeh.HasDecor("Vehicle.ID")) return null;

                var currentVehKeys = GetOwnedVehicles();
                if (currentVehKeys.Contains(closeVeh.GetDecor<int>("Vehicle.ID")))
                {
                    return closeVeh;
                }
            }

            return null;
        }

        public List<int> GetVehiclesWithKeys()
        {
            return LocalSession.GetLocalData("Vehicles.AccessibleVehicles", new JArray()).ToObject<List<int>>();
        }

        public List<int> GetOwnedVehicles()
        {
            return LocalSession.GetLocalData("Vehicles.OwnedVehicles", new JArray()).ToObject<List<int>>();
        }

        public async void SetVehicleVehId(int vehId)
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();

            while (closeVeh == null)
            {
                closeVeh = GTAHelpers.GetClosestVehicle();
                await BaseScript.Delay(0);
            }
            closeVeh.SetDecor("Vehicle.ID", vehId);
        }

        private async void SetBoughtVehId(int vehId)
        {
            var closeVeh = GTAHelpers.GetClosestVehicle();
            while (closeVeh == null || closeVeh.HasDecor("Vehicle.IsPreviewVehicle"))
            {
                closeVeh = GTAHelpers.GetClosestVehicle();
                await BaseScript.Delay(0);
            }
            closeVeh.SetDecor("Vehicle.ID", vehId);
        }

        private async void OnSpawnRequest(string plate, int vehId, string vehData)
        {
            Log.Info($"Just recieved a vehicle spawn request for a vehicle with Plate: {plate}; VehID: {vehId}");
            VehicleDataModel vehDataModel; 
            CitizenFX.Core.Vehicle spawnedVeh; 

            if (vehData.Contains("{"))
            {
                Log.Info("vehData contains a {");
                vehDataModel = JsonConvert.DeserializeObject<VehicleDataModel>(vehData);

                Log.Info($"vehData contained the following data:\nModel: {vehDataModel.Model}\nPlate: {vehDataModel.LicensePlate}");

                Log.Info($"About to start spawning vehicle");
                spawnedVeh = await VehicleDataPacker.UnpackVehicleData(vehDataModel, Game.PlayerPed.Position, 0.0f);
                Log.Info($"Vehicle should have spawned now (World.CreateVehicle finished execution)");

                Log.Info($"Spawned vehicle spawn handle is {spawnedVeh.Handle}");
            }
            else
            {
                var model = new Model(Game.GenerateHash(vehData));
                while (!model.IsLoaded)
                    await model.Request(0);

                spawnedVeh = await World.CreateVehicle(model, Game.PlayerPed.Position, 0.0f);
            }

            spawnedVeh.Mods.LicensePlate = plate;
            spawnedVeh.SetDecor("Vehicle.ID", vehId);
            Game.PlayerPed.Task.WarpIntoVehicle(spawnedVeh, VehicleSeat.Driver);
            ownedVehicles.Add(spawnedVeh);
        }

        private void OnGiveKeysCommand(Command cmd)
        {
            var closeOwnedVeh = GetClosestVehicleWithKeys();
            if (closeOwnedVeh != null)
            {
                var targetId = cmd.GetArgAs(0, 0);
                if (targetId == 0)
                {
                    var closestPlayer = GTAHelpers.GetClosestPlayer();
                    if(closestPlayer != null)
                        targetId = closestPlayer.ServerId;
                }
                else
                {
                    var closestPlayer = GTAHelpers.GetClosestPlayer();
                    if (closestPlayer.ServerId != targetId)
                    {
                        Log.ToChat("", "This person is not close enough to receive the vehicle keys");
                        return;
                    }
                }

                if(targetId != 0 && targetId != Game.Player.ServerId)
                    Client.TriggerServerEvent("Vehicle.GiveKeyAccess", closeOwnedVeh.GetDecor<int>("Vehicle.ID"), targetId);
            }
        }

        private void OnGiveVehCommand(Command cmd)
        {
            var closeOwnedVeh = GetClosestOwnedVehicle();
            if (closeOwnedVeh != null)
            {
                var targetId = cmd.GetArgAs(0, 0);
                if (targetId == 0)
                {
                    var closestPlayer = GTAHelpers.GetClosestPlayer();
                    if (closestPlayer != null)
                        targetId = closestPlayer.ServerId;
                }
                else
                {
                    var closestPlayer = GTAHelpers.GetClosestPlayer();
                    if (closestPlayer.ServerId != targetId)
                    {
                        Log.ToChat("", "This person is not close enough to recieve the vehicle");
                        return;
                    }
                }

                if (targetId != 0 && targetId != Game.Player.ServerId)
                    Client.TriggerServerEvent("Vehicle.GiveVehicleOwnership", closeOwnedVeh.GetDecor<int>("Vehicle.ID"), targetId);
            }
        }

        [EventHandler("Vehicle.DeleteCurrentVehicle")]
        private async void OnDeleteCurrentVehicle()
        {
            var playerVeh = Cache.PlayerPed.IsInVehicle() ? Cache.PlayerPed.CurrentVehicle : Cache.PlayerPed.LastVehicle;

            if (playerVeh != null)
            {
                Cache.PlayerPed.Task.LeaveVehicle(LeaveVehicleFlags.BailOut);
                playerVeh.LockStatus = VehicleLockStatus.Locked;
                playerVeh.IsDriveable = false;
                await BaseScript.Delay(100);

                playerVeh.Delete();
                await BaseScript.Delay(250);

                playerVeh.Position = Vector3.Zero;
            }
        }

        private async Task SaveVehicleTick()
        {
            await BaseScript.Delay(saveInterval);
            await ownedVehicles.ForEachAsync(async o =>
            {
                if (!o.IsDead)
                {
                    Client.TriggerServerEvent("Vehicles.UpdateVehicleData", o.GetDecor<int>("Vehicle.ID"), VehicleDataPacker.PackVehicleData(o));
                }

                await BaseScript.Delay(0);
            });
        }
    }
}
