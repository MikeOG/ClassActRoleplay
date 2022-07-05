using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Loader;

namespace Roleplay.Server
{
    public class Server : BaseScript
    {
        public static string CurrentIdentifier = CitizenFX.Core.Native.API.GetConvar("mg_currentIdentifier", "steam");
        public static DateTime StartTime = DateTime.Now;
        public static Server Instance { get; private set; }
        public PlayerList PlayerList => Players;
        public dynamic Instances = new System.Dynamic.ExpandoObject();
        public dynamic MySQL => GetExports("GHMattiMySQL");

        /// <summary>
        /// Allows custom names to be given to certain classes to make things shorter and nicer
        /// </summary>
        private Dictionary<string, string> instanceNameAliases = new Dictionary<string, string>
        {
            ["SessionManager"] = "Session",
            ["JobHandler"] = "Jobs",
            ["VehicleManager"] = "Vehicles",
            ["PaymentHandler"] = "Payments",
            ["JailTimeHandler"] = "Jail"
        };

        private List<string> prioityLoading = new List<string>
        {
            "DevEnviroment",
            "SessionManager",
            "JobHandler",
            "HTTPHandler"
        };

        public Server()
        {
            Instance = this;

            LoadClassInstances();
        }

        public void RegisterEventHandler(string eventName, Delegate eventFunction) => EventHandlers.Add(eventName, eventFunction);//Event.Security.Server.Events.RegisterRemoteEvent(eventName, eventFunction);
        public void RegisterSecuredEvent(string eventName, Delegate eventFunction) => EventHandlers.Add(eventName, eventFunction);
        public void RegisterLocalEvent(string eventName, Delegate eventFunction) => EventHandlers.Add($"{eventName}-local", eventFunction);
        public void TriggerLocalEvent(string eventName, params object[] args) => BaseScript.TriggerEvent($"{eventName}-local", args);
        //public void RemoveEventHandler(string eventName) => Event.Security.Server.Events.RemoveRemoteEvent(eventName);
        public void RegisterTickHandler(Func<Task> tickFunc) => Tick += tickFunc;
        public void DeregisterTickHandler(Func<Task> tickFunc) => Tick -= tickFunc;
        public dynamic GetExports(string resource) => Exports[resource];

        /// <summary>
        /// Gets the instance of a specified class if it exists
        /// </summary>
        /// <typeparam name="T">Type of the class you want to instance from</typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            IDictionary<string, dynamic> instanceDict = Instances;
            var nameAlias = getNameAlias(typeof(T).Name);
            return instanceDict.ContainsKey(nameAlias) ? (T)instanceDict[nameAlias] : default(T);
        }

        public void LoadClassInstances()
        {
            foreach (var i in AppDomain.CurrentDomain.GetAssemblies())
            {
                var priorityClasses = i.GetTypes().Where(o => DataLoader.IsLoadableClass(o) && prioityLoading.Contains(o.Name) && !((IDictionary<string, dynamic>)Instances).ContainsKey(getNameAlias(o.Name)));
                priorityClasses.OrderBy(o => o.Name).ToList().ForEach(o => createClassInstance(o));

                var normalClasses = i.GetTypes().Where(o => DataLoader.IsLoadableClass(o) && !prioityLoading.Contains(o.Name) && !((IDictionary<string, dynamic>)Instances).ContainsKey(getNameAlias(o.Name)));
                normalClasses.ToList().ForEach(o => createClassInstance(o));
            }
        }
            
        public void AddClassInstance(Type classInstance, bool includeThis = true, bool forceAdd = false)
        {
            createClassInstance(classInstance, includeThis, forceAdd);
        }

        private string getNameAlias(string currentName)
        {
            return instanceNameAliases.ContainsKey(currentName) ? instanceNameAliases[currentName] : currentName;
        }

        private void createClassInstance(Type instance, bool includeThis = true, bool forceAdd = false)
        {
            try
            {
                if(DataLoader.IsLoadableClass(instance) || forceAdd)
                { 
                    var obj = includeThis ? DataLoader.LoadClass(instance, this) : DataLoader.LoadClass(instance);
                    ((IDictionary<string, dynamic>)Instances)[getNameAlias(instance.Name)] = Convert.ChangeType(obj, instance);
                    Log.Debug($"Created instance of class {instance.Name}");
                }
                else
                {
                    Log.Verbose($"Not loading class {instance.Name} due to it being flagged as not loadable");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to create instance of class {instance.Name} - {e.Message}");
            }
        }
    }
}
