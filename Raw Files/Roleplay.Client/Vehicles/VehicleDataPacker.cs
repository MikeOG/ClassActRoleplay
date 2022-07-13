using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using Roleplay.Shared.Models;
using Roleplay.Shared;
using Newtonsoft.Json;

namespace Roleplay.Client.Vehicles
{
    public static class VehicleDataPacker
    {
        public static async Task<Vehicle> UnpackVehicleData(VehicleDataModel vehicleModel, Vector3 position, float heading)
        {
            try
            {
                var vehModel = new Model(vehicleModel.Model);
                if (!vehModel.IsInCdImage)
                {
                    Log.ToChat($"Unable to spawn vehicle with model {vehicleModel.Model} as it doesn't exist?");
                    return null;
                }

                while (!vehModel.IsLoaded)
                    await vehModel.Request(0);

                Vehicle vehicle = await World.CreateVehicle(vehModel, position, heading);
                vehicle.Mods.InstallModKit();
                if (vehicleModel.BodyHealth < 150.0f) vehicleModel.BodyHealth = 150.0f;
                vehicle.BodyHealth = vehicleModel.BodyHealth;
                if (vehicleModel.EngineHealth < 150.0f) vehicleModel.EngineHealth = 150.0f;
                vehicle.EngineHealth = vehicleModel.EngineHealth;
                vehicle.Mods.LicensePlate = vehicleModel.LicensePlate;
                //vehicle.SetDecor("Vehicle.Fuel", vehicleModel.VehicleFuel);
                vehicle.Mods.Livery = vehicleModel.Livery;
                vehicle.Mods.PrimaryColor = vehicleModel.PrimaryColor;
                vehicle.Mods.SecondaryColor = vehicleModel.SecondaryColor;
                vehicle.Mods.PearlescentColor = vehicleModel.PearlescentColor;
                vehicle.Mods.RimColor = vehicleModel.RimColor;
                vehicle.Mods.WheelType = vehicleModel.WheelType;
                vehicle.Mods.WindowTint = vehicleModel.WindowTint;
                vehicle.Mods.LicensePlateStyle = vehicleModel.LicensePlateStyle;
                vehicleModel.Mods?.ToList().ForEach(m =>
                {
                    vehicle.Mods[m.Key].Index = m.Value;
                });
                Function.Call(Hash.SET_VEHICLE_EXTRA, vehicle.Handle, VehicleModType.FrontWheel, vehicleModel.CustomWheelVariation ? 0 : -1);
                vehicleModel.Extras?.ToList().ForEach(e =>
                {
                    Function.Call(Hash.SET_VEHICLE_EXTRA, vehicle.Handle, e.Key, e.Value ? 0 : -1);
                });

                vehicleModel.ToggleMods?.ToList().ForEach(o =>
                {
                    vehicle.Mods[o.Key].IsInstalled = o.Value;
                });

                vehicleModel.NeonLights?.ToList().ForEach(o =>
                {
                    vehicle.Mods.SetNeonLightsOn(o.Key, o.Value);
                    if (o.Value) vehicle.Mods.NeonLightsColor = Color.FromArgb(vehicleModel.NeonLightsColour[0], vehicleModel.NeonLightsColour[1], vehicleModel.NeonLightsColour[2]);
                });

                return vehicle;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }

        public static string PackVehicleData(Vehicle vehicle)
        {
            try
            {
                var data = new VehicleDataModel
                {
                    LicensePlate = vehicle.Mods.LicensePlate,
                    Class = (int)vehicle.ClassType,
                    //VehicleFuel = vehicle.HasDecor("Vehicle.Fuel") ? vehicle.GetDecor<float>("Vehicle.Fuel") : 100.0f,
                    Model = vehicle.Model,
                    EngineHealth = vehicle.EngineHealth,
                    BodyHealth = vehicle.BodyHealth,
                    Livery = vehicle.Mods.Livery,
                    PrimaryColor = vehicle.Mods.PrimaryColor,
                    SecondaryColor = vehicle.Mods.SecondaryColor,
                    PearlescentColor = vehicle.Mods.PearlescentColor,
                    RimColor = vehicle.Mods.RimColor,
                    WheelType = vehicle.Mods.WheelType,
                    WindowTint = vehicle.Mods.WindowTint,
                    LicensePlateStyle = vehicle.Mods.LicensePlateStyle
                };
                foreach (var mod in vehicle.Mods.GetAllMods())
                {
                    if (!data.Mods.ContainsKey(mod.ModType))
                    {
                        data.Mods.Add(mod.ModType, mod.Index);
                    }
                }
                data.CustomWheelVariation = API.IsVehicleExtraTurnedOn(vehicle.Handle, (int)VehicleModType.FrontWheel);
                Enumerable.Range(0, 50).ToList().ForEach(e =>
                {
                    data.Extras[e] = API.IsVehicleExtraTurnedOn(vehicle.Handle, e);
                });
                Enum.GetValues(typeof(VehicleToggleModType)).Cast<VehicleToggleModType>().ToList().ForEach(o =>
                {
                    var toggleMod = vehicle.Mods[o];
                    data.ToggleMods[o] = toggleMod.IsInstalled;
                });
                Enum.GetValues(typeof(VehicleNeonLight)).Cast<VehicleNeonLight>().ToList().ForEach(o =>
                {
                    data.NeonLights[o] = vehicle.Mods.IsNeonLightsOn(o);
                });
                var neonColour = vehicle.Mods.NeonLightsColor;
                data.NeonLightsColour = new int[] {neonColour.R, neonColour.G, neonColour.B};

                return JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
    }
}

