using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
using Roleplay.Shared;
using Roleplay.Shared.Loader;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client
{
    public class Client : BaseScript
    {
        public static Client Instance { get; private set; }
        public static float ClientAspectRatio;
        public const float AspectRatio_16_9 = 1.777778f;
        public static Session.Session LocalSession => Instance.Instances.Session.GetPlayer(Game.Player) ?? new Session.Session(Game.Player.ServerId, new SessionData());
        public PlayerList PlayerList => Players;

        public static object UI { get; internal set; }

        public dynamic Instances = new System.Dynamic.ExpandoObject();

        /// <summary>
        /// Allows custom names to be given to certain classes to make things shorter and nicer
        /// </summary>
        private Dictionary<string, string> instanceNameAliases = new Dictionary<string, string>
        {
            ["SessionManager"] = "Session",
            ["MarkerHandler"] = "Markers",
            ["JobHandler"] = "Jobs",
            ["VehicleHandler"] = "Vehicles",
            ["WeaponHandler"] = "Weapons"
        };

        private List<string> prioityLoading = new List<string>
        {
            "SessionManager",
            "InteractionUI",
            "JobHandler",
            "MarkerHandler",
            "VehicleHandler"
        };

        private static Client _instance;

        public static Client GetInstance()
        {
            return _instance;
        }

        public Client()
        {
            Instance = this;

            // Loads all classes that are inherited by ClientAccessor
            Task.Factory.StartNew(async () =>
            {
                await BaseScript.Delay(0);
                foreach (var i in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var priorityClasses = i.GetTypes().Where(o => DataLoader.IsLoadableClass(o) && prioityLoading.Contains(o.Name));
                    priorityClasses.ToList().ForEach(o => createClassInstance(o));
                    var normalClasses = i.GetTypes().Where(o => DataLoader.IsLoadableClass(o) && !prioityLoading.Contains(o.Name));
                    normalClasses.ToList().ForEach(o => createClassInstance(o));
                }
            });
            RegisterTickHandler(InteractionTick);
        }

        public void RegisterEventHandler(string eventName, Delegate eventFunction) => EventHandlers.Add(eventName, eventFunction);
        public void RemoveEventHandler(string eventName) => EventHandlers.Remove(eventName);
        public void RegisterTickHandler(Func<Task> tickFunc) => Tick += tickFunc;
        public void DeregisterTickHandler(Func<Task> tickFunc) => Tick -= tickFunc;
        public new void TriggerServerEvent(string eventName, params object[] args) => /*Event.Security.Client.Events*/BaseScript.TriggerServerEvent(eventName, args);
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

        public void AddClassInstance(Type classInstance, bool includeThis = true, bool forceAdd = false)
        {
            createClassInstance(classInstance, includeThis, forceAdd);
        }

        private string getNameAlias(string currentName)
        {
            return instanceNameAliases.ContainsKey(currentName) ? instanceNameAliases[currentName] : currentName;
        }

        private int lastServerSend = Game.GameTime;
        private async Task InteractionTick()
        {
            if (Input.IsControlJustPressed(Control.Pickup))
            {
                TriggerEvent("Player.CheckForInteraction");

                if (Game.GameTime - lastServerSend > 1000)
                {
                    TriggerServerEvent("Player.OnInteraction", Cache.PlayerPed.IsInVehicle());
                    lastServerSend = Game.GameTime;
                }
            }
        }

        private void createClassInstance(Type instance, bool includeThis = true, bool forceAdd = false)
        {
            try
            {
                if (DataLoader.IsLoadableClass(instance) || forceAdd)
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
