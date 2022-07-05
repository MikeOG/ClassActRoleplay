using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
#if CLIENT
using Roleplay.Client;
using Roleplay.Client.Jobs;
using Roleplay.Client.Dev;
#elif SERVER
using Roleplay.Server;
using Roleplay.Server.Jobs;
using Roleplay.Server.Dev;
using Roleplay.Server.Enums;

#endif
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Models;

namespace Roleplay.Shared.Loader
{
    /// <summary>
    /// Create a enum called TickUsage that will execute a tick under certain times e.g. when a player enters a vehicle TickUsage.EnteredVehicle, TickUsage.Aiming
    /// </summary>
    public class DataLoader
    {
        /// <summary>
        /// This will show all current active tick states
        /// </summary>
        public static TickUsage CurrentTickState = TickUsage.All;

        public static string CurrentPlatform =
                    #if CLIENT
                        "Client";
                    #elif SERVER
                        "Server";
                    #endif


        private static List<string> classesToLoad = CitizenFX.Core.Native.API.GetConvar($"mg_loadable{CurrentPlatform}Classes", "*").Split(',').ToList();
        private static List<string> classesToExclude = CitizenFX.Core.Native.API.GetConvar($"mg_excluded{CurrentPlatform}Classes", "").Split(',').ToList();

        private static List<DynamicTick> ticks = new List<DynamicTick>();

        private static 
            #if CLIENT
                Roleplay.Client.Client
            #elif SERVER
                Roleplay.Server.Server
            #endif
                curInstance = 
                    #if CLIENT
                        Roleplay.Client.Client.Instance;
                    #elif SERVER
                        Roleplay.Server.Server.Instance;
                    #endif
                        

        static DataLoader()
        {
            curInstance.RegisterTickHandler(dynamicTickHandler);
            curInstance.RegisterTickHandler(checkDebugTick);
        }

        public static void AddTickState(TickUsage usage)
        {
            if (!CurrentTickState.HasFlag(usage))
            {
                Log.Verbose($"Adding usage {usage} to current tick state");

                Log.Debug($"Current tick state is: {CurrentTickState}");
                CurrentTickState |= usage;
                Log.Debug($"New tick state is: {CurrentTickState}");
            }
        }

        public static void RemoveTickState(TickUsage usage)
        {
            if (CurrentTickState.HasFlag(usage))
            {
                Log.Verbose($"Removing usage {usage} from current tick state");

                Log.Debug($"Current tick state is: {CurrentTickState}");
                CurrentTickState &= ~usage;
                Log.Debug($"New tick state is: {CurrentTickState}");
            }
        }

        public static bool IsLoadableClass(Type t)
        {
            return !t.IsAbstract && t.IsSubclassOf(typeof(
                       #if CLIENT
                           ClientAccessor
                       #elif SERVER
                           ServerAccessor
                       #endif
                       )) && (classesToLoad.Contains("*") || classesToLoad.Contains(t.Name)) && !classesToExclude.Contains(t.Name);

        }

        public static dynamic LoadClass(Type instance, params dynamic[] ctorParams)
        {
            var classObj = Activator.CreateInstance(instance, ctorParams);

            DataLoader.LoadEvents(classObj);
            DataLoader.LoadTicks(classObj);
            DataLoader.LoadCommands(classObj);

            return classObj;
        }

        public static async Task LoadEvents(object classObj)
        {
            var allMethods = classObj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                
            var methods = getMethodsWithAttribute(
                #if CLIENT
                    typeof(EventHandlerAttribute)
                #elif SERVER
                    typeof(EventHandlerAttribute)
                #endif
                , allMethods);

            foreach (var method in methods)
            {
                //if (method != null)
                {
                    var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
                    var actionType = Expression.GetDelegateType(parameters.Concat(new[]
                    {
                        typeof(void)
                    }).ToArray());
                    var attribute = method.GetCustomAttribute<
                                #if CLIENT
                                    EventHandlerAttribute
                                #elif SERVER
                                    EventHandlerAttribute
                                #endif
                                    >();

                    Log.Debug($"Registering EventHandler {attribute.Name} for attributed method {method.Name}");
                              ///$", with parameters {string.Join(", ", parameters.Select(p => p.GetType().ToString()))}");
                    curInstance.
                        #if CLIENT
                        RegisterEventHandler
                        #elif SERVER
                        RegisterSecuredEvent
                        #endif
                            (attribute.Name, Delegate.CreateDelegate(actionType, classObj, method.Name));
                }
            }  
        }

        public static async Task LoadTicks(object classObj)
        {
            var allMethods = classObj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            var methods = getMethodsWithAttribute(typeof(DynamicTickAttribute), allMethods);

            foreach (var method in methods)
            {
                //if (method != null)
                {
                    var tickUsage = method.GetCustomAttribute<DynamicTickAttribute>();

                    var tickData = new DynamicTick
                    {
                        Usage = tickUsage.Usage,
                        Func = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), classObj, method.Name),
                        FuncInfo = method,
                        ParentClass = classObj.GetType().Name
                    };

                    Log.Debug($"Registered Tick for attributed method {method.Name}");
                    ticks.Add(tickData);
                }
            }

            previousTickUsage = (TickUsage)(-1);
        }

        public static async Task LoadCommands(object classObj)
        {
            var allMethods = classObj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            var methods = getMethodsWithAttribute(typeof(ServerCommandAttribute), allMethods);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ServerCommandAttribute>();

#if SERVER
                if (attribute.AdminLevel != (AdminLevel)(-1))
                {
                    CommandRegister.RegisterAdminCommand(attribute.Command, cmd =>
                    {
                        if (method.IsStatic)
                        {
                            method.Invoke(null, new object[] { cmd });
                        }
                        else
                        {
                            method.Invoke(classObj, new object[] { cmd });
                        }
                    }, attribute.AdminLevel);

                    Log.Debug($"Registered admin command {attribute.Command}");

                    continue;
                }

                if (attribute.JobType != (JobType)(-1))
                {
                    CommandRegister.RegisterJobCommand(attribute.Command, cmd =>
                    {
                        if (method.IsStatic)
                        {
                            method.Invoke(null, new object[] { cmd });
                        }
                        else
                        {
                            method.Invoke(classObj, new object[] { cmd });
                        }
                    }, attribute.JobType, attribute.SkipDutyCheck);

                    Log.Debug($"Registered job command {attribute.Command}");

                    continue;
                }
#endif

                CommandRegister.RegisterCommand(attribute.Command, cmd =>
                {
                    if (method.IsStatic)
                    {
                        method.Invoke(null, new object[] { cmd });
                    }
                    else
                    {
                        method.Invoke(classObj, new object[] { cmd });
                    }
                }, attribute.IsRestriced);

                Log.Debug($"Registered command {attribute.Command}");
            }
        }

        private static IEnumerable<MethodInfo> getMethodsWithAttribute(Type attr, MethodInfo[] allMethods)
        {
            return allMethods.Where(m => m.GetCustomAttributes(attr, false).Length > 0);
        }

        private static TickUsage previousTickUsage = (TickUsage)(-1);
        private static List<DynamicTick> ticksInUse = new List<DynamicTick>();

        private static async Task dynamicTickHandler()
        {
            if (previousTickUsage != CurrentTickState)
            {
                Log.Debug($"Tick state has changed ({previousTickUsage} -> {CurrentTickState}) updating tick list");
                foreach (var tick in ticksInUse)
                {
                    curInstance.DeregisterTickHandler(tick.Func);
                }
                ticksInUse = ticks.Where(o => CurrentTickState.HasFlag(o.Usage)).ToList();

                previousTickUsage = CurrentTickState;

                foreach (var tick in ticksInUse)
                {
                    curInstance.RegisterTickHandler(tick.Func);
                }
            }
        }

        private static async Task checkDebugTick()
        {
            if (DevEnviroment.IsDebugInstance)
            {
                DataLoader.AddTickState(TickUsage.Debug);
            }
            else
            {
                DataLoader.RemoveTickState(TickUsage.Debug);
            }
        }
    }

#if CLIENT
    public class TickEvents : ClientAccessor
    {
        public TickEvents(Client.Client client) : base(client)
        {
            client.RegisterEventHandler("baseevents:enteredVehicle", new Action<int, int, string>((veh, seat, name) => {
                DataLoader.RemoveTickState(TickUsage.OutVehicle);
                DataLoader.AddTickState(TickUsage.InVehicle);
            }));
            client.RegisterEventHandler("baseevents:leftVehicle", new Action<int, int, string>((veh, seat, name) => {
                DataLoader.RemoveTickState(TickUsage.InVehicle);
                DataLoader.AddTickState(TickUsage.OutVehicle);
            }));
            DataLoader.AddTickState(TickUsage.OutVehicle);
            DataLoader.AddTickState(TickUsage.NotAiming);
        }

        [DynamicTick]
        private async Task CheckWeaponTick()
        {
            if (Cache.PlayerPed.IsAiming)
            {
                DataLoader.RemoveTickState(TickUsage.NotAiming);
                DataLoader.AddTickState(TickUsage.Aiming);
            }
            else
            {
                DataLoader.RemoveTickState(TickUsage.Aiming);
                DataLoader.AddTickState(TickUsage.NotAiming);
            }

            if (Cache.PlayerPed.IsShooting)
            {
                DataLoader.AddTickState(TickUsage.Shooting);
            }
            else
            {
                DataLoader.RemoveTickState(TickUsage.Shooting);
            }
        }
    }
#endif

    class DynamicTick
    {
        public TickUsage Usage = TickUsage.All;
        public Func<Task> Func;
        public MethodInfo FuncInfo;
        public string ParentClass;
    }
}
