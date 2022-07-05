using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Property.Models
{
    public class PropertyFinanceInformation
    {
        public int PropertyPrice;
        public int TotalAmountPaid;
        public int TotalMissedPayments;
        public int ConsecutiveMissedPayments;
        public double LastPayment;
        public int CurrentInstallmentPayed;
        public DateTime LastPaymentDate;

        public void LoadDataFromDatabase(dynamic data)
        {
            var type = GetType();

            foreach (var kvp in data)
            {
                try
                {
                    var field = type.GetField(kvp.Key);

                    if (field != null)
                    {
                        field.SetValue(this, kvp.Value is UInt32 ? (int)kvp.Value : kvp.Value);
                        Log.Debug($"Setting value of {kvp.Key} to {kvp.Value.ToString()}");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            LastPaymentDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(LastPayment).ToLocalTime();
        }

        public override string ToString()
        {
            if (Dev.DevEnviroment.IsDebugInstance)
            {
                var propertyData = "";

                FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    propertyData += $"{field.Name}: {field.GetValue(this)}\n";
                }

                return propertyData;
            }

            return GetType().FullName;
        }
    }
}
