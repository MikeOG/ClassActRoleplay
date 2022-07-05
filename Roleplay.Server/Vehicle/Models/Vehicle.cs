using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Vehicle.Models
{
    public class Vehicle
    {
        public Session.Session VehicleOwner { get; set; }
        public int VehID { get; set; }
        public int CharID { get; set; } = -1;
        public string SteamID { get; set; }
        public string VehicleName { get; set; }
        public string Plate { get; set; }
        public VehicleDataModel Mods { get; set; }
        public VehicleInventory Inventory { get; set; }
        public int VehiclePrice { get; set; }
        public bool Impounded { get; set; }
        public bool OutGarage { get; set; }
        public string Garage { get; set; }
        public List<int> KeyAccessCharacters = new List<int>();
        public bool RentedVehicle;

        public Vehicle(IDictionary<string, dynamic> data, Session.Session vehOwner)
        {
            var vehType = GetType();

            foreach (var kvp in data)
            {
                try
                {
                    var field = vehType.GetProperty(kvp.Key);
                    //Log.Debug($"Currently doing stuff for {kvp.Key}");
                    if (field != null)
                    {
                        //Log.Debug($"{kvp.Key} exists in this object!");
                        if (kvp.Key == "Mods")
                        {
                            if(kvp.Value.Contains("{"))
                                field.SetValue(this, JsonConvert.DeserializeObject<VehicleDataModel>(kvp.Value));
                            else
                                field.SetValue(this, new VehicleDataModel());
                            Log.Debug($"Settings mods");
                        }
                        else if (kvp.Key == "Inventory")
                        {
                            field.SetValue(this, new VehicleInventory(kvp.Value, this));
                            Log.Debug($"Setting inventory with an inv string of {kvp.Value}");
                        }
                        else
                        {
                            field.SetValue(this, kvp.Value);
                            Log.Debug($"Setting generic value of {kvp.Key}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            VehicleOwner = vehOwner;
            KeyAccessCharacters.Add(vehOwner.GetGlobalData<int>("Character.CharID"));
        }

        public Vehicle()
        {

        }

        public bool Equals(Vehicle veh)
        {
            return !ReferenceEquals(veh, null) && VehID == veh.VehID;
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((Vehicle)obj);
        }

        public static bool operator ==(Vehicle left, Vehicle right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(Vehicle left, Vehicle right)
        {
            return !(left == right);
        }
    }
}
