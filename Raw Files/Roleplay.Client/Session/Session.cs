using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Session
{
    public class Session
    {
        public int ServerID { get; }
        public static bool HasJoinedRP { get; set; }

        public CitizenFX.Core.Player Player => new CitizenFX.Core.Player(CitizenFX.Core.Native.API.GetPlayerFromServerId(ServerID));

        private readonly Dictionary<string, dynamic> globalData = new Dictionary<string, dynamic>();
        protected internal IReadOnlyDictionary<string, dynamic> GlobalData => new Dictionary<string, dynamic>(globalData);

        private readonly Dictionary<string, dynamic> localData = new Dictionary<string, dynamic>();
        protected internal IReadOnlyDictionary<string, dynamic> LocalData => new Dictionary<string, dynamic>(localData);

        private bool dataUpdated = false;
        private bool? canPayForItem = null;

        public Session(int serverID, SessionData data)
        {
            ServerID = data.ServerID;

            foreach (var dataEntry in data.GlobalData)
            {
                globalData[dataEntry.Key] = dataEntry.Value;
                Log.Debug($"Session.ctor: Added globalData of {dataEntry.Key}: {dataEntry.Value}");
            }

            foreach (var dataEntry in data.LocalData)
            {
                localData[dataEntry.Key] = dataEntry.Value;
                Log.Debug($"Session.ctor: Added localData of {dataEntry.Key}: {dataEntry.Value}");
            }

            if (data.LocalData != new Dictionary<string, dynamic>())
            {
                Client.Instance.RegisterEventHandler("Payment.SendPaymentStatus", new Action<bool>(canPay => canPayForItem = canPay));
            }
        }

        public T GetGlobalData<T>(string key, T defaulValue = default(T)) => globalData.ContainsKey(key) ? (T)globalData[key] : defaulValue;

        protected internal void UpdateGlobalData(string data)
        {
            dataUpdated = false;
            Log.Debug($"Recieved a GlobalData update for {Player.Name} with the following data: {data}");
            var dataDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);
            foreach (var dataEntry in dataDict)
            {
                globalData[dataEntry.Key] = dataEntry.Value;
            }
            dataUpdated = true;
        }

        public T GetLocalData<T>(string key, T defaulValue = default(T)) => localData.ContainsKey(key) ? (T)localData[key] : defaulValue;

        protected internal void UpdateLocalData(string data)
        {
            dataUpdated = false;
            Log.Debug($"Recieved a LocalData update for {Player.Name} with the following data: {data}");
            var dataDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);
            foreach (var dataEntry in dataDict)
            {
                localData[dataEntry.Key] = dataEntry.Value;
            }
            dataUpdated = true;
        }

        protected internal async Task UpdateData(string updateData = "*", [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            dataUpdated = false;
            Roleplay.Client.Client.Instance.TriggerServerEvent("Session.UpdateClientData", updateData, $"{callingMember} in {fileName.Split('\\').ToList().Last()} on line {lineNumber}");
            while (!dataUpdated)
            {
                await BaseScript.Delay(0);
            }
        }

        protected internal async Task<bool> CanPayForItem(int itemCost)
        {
            canPayForItem = null;
            Roleplay.Client.Client.Instance.TriggerServerEvent("Payment.CanPayForItem");
            while (canPayForItem == null)
            {
                await BaseScript.Delay(0);
            }

            return (bool)canPayForItem;
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
