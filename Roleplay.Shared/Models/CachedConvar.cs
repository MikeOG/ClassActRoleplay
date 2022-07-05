using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;

namespace Roleplay.Shared.Models
{
    public sealed class CachedConvar<T> : CachedValue<T>
    {
        public string ConVar { get; }
        public T DefaultValue { get; }

        public CachedConvar(string convar, T defaultValue, long timeoutMs = 30000) : base(timeoutMs)
        {
            ConVar = convar;
            DefaultValue = defaultValue;

#if SERVER
            if(Update().Equals(defaultValue))
            {
                API.SetConvar(convar, defaultValue.ToString());
            }
#endif
        }

        protected override T Update()
        {
            var type = typeof(T);

            if (type == typeof(int))
            {
                return (T)Convert.ChangeType(API.GetConvarInt(ConVar, Convert.ToInt32(DefaultValue)), typeof(T));
            }

            if (type == typeof(float))
            {
                return (T)Convert.ChangeType(API.GetConvar(ConVar, DefaultValue.ToString()), typeof(T));
            }
            
            if (type == typeof(bool))
            {
                var value = API.GetConvar(ConVar, DefaultValue.ToString());
                return (T)Convert.ChangeType(value == "True" || value == "true" || value == "1", typeof(T));
            }

            return (T)Convert.ChangeType(API.GetConvar(ConVar, DefaultValue.ToString()), typeof(T));
        }
    }
}
