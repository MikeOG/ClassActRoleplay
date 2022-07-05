using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Newtonsoft.Json;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        public int ServerID { get; }
        public Player Source { get; }
        public string SteamIdentifier { get; }
        public ulong SteamID64 { get; }
        public string PlayerName { get; }

        public Dictionary<string, dynamic> GlobalData { get; protected internal set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> LocalData { get; protected internal set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> ServerData { get; protected internal set; } = new Dictionary<string, dynamic>();

        private Dictionary<string, dynamic> AllData => GlobalData.Concat(LocalData).Concat(ServerData).ToDictionary(o => o.ToString(), o => (dynamic)o);

        public Session(Player source, Dictionary<string, dynamic> globalData = null, Dictionary<string, dynamic> localData = null, Dictionary<string, dynamic> serverData = null)
        {
            ServerID = int.Parse(source.Handle);
            SteamIdentifier = source.Identifiers[Server.CurrentIdentifier];
            SteamID64 = Convert.ToUInt64(SteamIdentifier, 16);
            Source = source;
            PlayerName = source.Name;

            if (globalData != null)
            {
                foreach(var data in globalData)
                {
                    GlobalData[data.Key] = data.Value;
                }
            }

            if (localData != null)
            {
                foreach (var data in localData)
                {
                    LocalData[data.Key] = data.Value;
                }
            }
            
            if (serverData != null)
            {
                foreach (var data in serverData)
                {
                    ServerData[data.Key] = data.Value;
                }
            }
        }

        /// <summary>
        /// Sets the the value of the specified key to the local data dictionary. This data will only be shared to the client this session is linked to
        /// </summary>
        /// <param name="key">Key to store the value in</param>
        /// <param name="data">Data wanting to be put into the entry</param>
        public void SetLocalData(string key, dynamic data)
        {
            SetLocalData(new Dictionary<string, dynamic> { { key, data } });
        }

        /// <summary>
        /// Adds all data in the dictionary to the local data dictionary. This data will only be shared to the client this session is linked to
        /// </summary>
        /// <param name="data">The dictionary containing wanted data to be added</param>
        public void SetLocalData(Dictionary<string, dynamic> data)
        {
            foreach (var dataEntry in data)
            {
                LocalData[dataEntry.Key] = dataEntry.Value;
                Log.Debug($"Setting LocalData for {PlayerName}: [Key: {dataEntry.Key}, Value: {dataEntry.Value}]");
            }

            TriggerEvent("Session.UpdateLocalData", ServerID, JsonConvert.SerializeObject(data));
        }

        //public void ResyncLocalData() => TriggerEvent("Session.UpdateLocalData", ServerID, JsonConvert.SerializeObject(LocalData));

        /// <summary>
        /// Gets the local data at the specified key if it exists
        /// </summary>
        /// <param name="key">Key of the entry you are wanting to get</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetLocalData<T>(string key, T defaultValue = default(T)) => LocalData.ContainsKey(key) ? (T)LocalData[key] : defaultValue;

        /// <summary>
        /// Sets the the value of the specified key to the global data dictionary. This data will be shared between all clients client-side
        /// </summary>
        /// <param name="key">Key to store the value in</param>
        /// <param name="data">Data wanting to be put into the entry</param>
        public void SetGlobalData(string key, dynamic data)
        {
            SetGlobalData(new Dictionary<string, dynamic>{ { key, data } });
        }

        /// <summary>
        /// Adds all data in the dictionary to the global data dictionary. This data will be shared betwreen all clients client-side
        /// </summary>
        /// <param name="data">The dictionary containing wanted data to be added</param>
        public void SetGlobalData(Dictionary<string, dynamic> data)
        {
            foreach (var dataEntry in data)
            {
                GlobalData[dataEntry.Key] = dataEntry.Value;
                Log.Debug($"Setting GlobalData for {PlayerName}: [Key: {dataEntry.Key}, Value: {dataEntry.Value}]");
            }

            BaseScript.TriggerClientEvent("Session.UpdateGlobalData", ServerID, JsonConvert.SerializeObject(data));
        }

        //public void ResyncGlobalData() => BaseScript.TriggerClientEvent("Session.UpdateGlobalData", ServerID, JsonConvert.SerializeObject(GlobalData));

        /// <summary>
        /// Gets the global data at the specified key if it exists
        /// </summary>
        /// <param name="key">Key of the entry you are wanting to get</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetGlobalData<T>(string key, T defaultValue = default(T)) => GlobalData.ContainsKey(key) ? (T)GlobalData[key] : defaultValue;

        /// <summary>
        /// Sets the the value of the specified key to the server data dictionary. This data will only be available on the server
        /// </summary>
        /// <param name="key">Key to store the value in</param>
        /// <param name="data">Data wanting to be put into the entry</param>
        public void SetServerData(string key, dynamic data)
        {
            SetServerData(new Dictionary<string, dynamic> { { key, data } });
        }

        /// <summary>
        /// Adds all data in the dictionary to the server data dictionary. This data will only be available on the server
        /// </summary>
        /// <param name="data">The dictionary containing wanted data to be added</param>
        public void SetServerData(Dictionary<string, dynamic> data)
        {
            foreach (var dataEntry in data)
            {
                ServerData[dataEntry.Key] = dataEntry.Value;
                Log.Debug($"Setting ServerData for {PlayerName}: [Key: {dataEntry.Key}, Value: {dataEntry.Value}]");
            }
        }

        /// <summary>
        /// Gets the server data at the specified key if it exists
        /// </summary>
        /// <param name="key">Key of the entry you are wanting to get</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetServerData<T>(string key, T defaultValue = default(T)) => ServerData.ContainsKey(key) ? (T)ServerData[key] : defaultValue;

        public void TriggerEvent(string eventName, params object[] args)
        {
            Source.TriggerEvent(eventName, args);
        }

        public void ResyncData(string dataToUpdate)
        {
            if (dataToUpdate == "*") // All data
            {
                TriggerEvent("Session.UpdateGlobalData", ServerID, JsonConvert.SerializeObject(GlobalData));
                TriggerEvent("Session.UpdateLocalData", ServerID, JsonConvert.SerializeObject(LocalData));
            }
            else
            {
                var dataEntries = dataToUpdate.Split(',');
                var globalUpdateData = new Dictionary<string, dynamic>();
                var locallUpdateData = new Dictionary<string, dynamic>();
                foreach (var kvp in GlobalData)
                {
                    if (dataEntries.Contains(kvp.Key))
                    {
                        globalUpdateData.Add(kvp.Key, kvp.Value);
                    }
                }

                foreach (var kvp in LocalData)
                {
                    if (dataEntries.Contains(kvp.Key))
                    {
                        locallUpdateData.Add(kvp.Key, kvp.Value);
                    }
                }

                if(globalUpdateData.Count > 0)
                    TriggerEvent("Session.UpdateGlobalData", ServerID, JsonConvert.SerializeObject(globalUpdateData));
                if (locallUpdateData.Count > 0)
                    TriggerEvent("Session.UpdateLocalData", ServerID, JsonConvert.SerializeObject(locallUpdateData));
            }
        }

        public bool Equals(Session session)
        {
            return !ReferenceEquals(session, null) && ServerID == session.ServerID;
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((Session)obj);
        }

        public static bool operator ==(Session left, Session right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(Session left, Session right)
        {
            return !(left == right);
        }
    }
}
