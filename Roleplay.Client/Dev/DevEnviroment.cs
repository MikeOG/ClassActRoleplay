using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.Session;
using Roleplay.Client.UI;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Loader;
using Roleplay.Shared.Models;
using MenuFramework;

namespace Roleplay.Client.Dev
{
    public class DevEnviroment : ClientAccessor
    {
        private static CachedConvar<bool> IsDebugConvar = new CachedConvar<bool>("mg_debugInstance", false, 60000);
        private static CachedConvar<bool> VerboseLoggingConvar = new CachedConvar<bool>("mg_enableVerboseLogging", false, 60000);

        public static bool IsDebugInstance => IsDebugConvar.Value;
        public static bool EnableVerboseLogging => VerboseLoggingConvar.Value;

        private Dictionary<string, Action<Command>> devCommands = new Dictionary<string, Action<Command>>();

        public DevEnviroment(Client client) : base(client)
        {
            RegisterDevCommand("getmarker", cmd =>
            {
                var playerPos = Game.PlayerPed.Position;
                var itemHeight = playerPos.Z;
                if (CitizenFX.Core.Native.API.IsValidInterior(CitizenFX.Core.Native.API.GetInteriorAtCoords(playerPos.X, playerPos.Y, playerPos.Z)))
                {
                    itemHeight -= 1.0f;
                }
                else
                {
                    itemHeight = World.GetGroundHeight(playerPos);
                }
                playerPos.Z = itemHeight + 0.04f;
                Log.ToChat($"The coordinates for this marker are {playerPos.ToObjectString()}");
                Log.Info(playerPos.ToObjectString());
                Log.ToServer(playerPos.ToObjectString());

                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await BaseScript.Delay(0);
                        World.DrawMarker(MarkerType.HorizontalCircleFat, playerPos, Vector3.Down, Vector3.Down, new Vector3(3, 3, 3), Color.FromArgb(140, ConstantColours.Yellow));
                    }
                });
            });
            RegisterDevCommand("goto", cmd =>
            {
                Game.PlayerPed.Position = new Vector3(Convert.ToSingle(cmd.GetArgAs(0, "0").Replace("f", "")), Convert.ToSingle(cmd.GetArgAs(1, "0").Replace("f", "")), Convert.ToSingle(cmd.GetArgAs(2, "0").Replace("f", "")));
            });
            RegisterDevCommand("getvehnetworkid", cmd =>
            {
                Log.ToChat(Cache.PlayerPed.CurrentVehicle.NetworkId.ToString());
            });
            RegisterDevCommand("printcoords", new Action<Command>(cmd =>
            {
                Log.Info($"new_Vector3({Game.PlayerPed.Position.ToString("0.00f")})"
                    .Replace("X", "")
                    .Replace("Y", "")
                    .Replace("Z", "")
                    .Replace(":", "")
                    .Replace(" ", ", ")
                    .Replace("_", " "));
                Log.ToServer($"new_Vector3({Game.PlayerPed.Position.ToString("0.00f")})"
                    .Replace("X", "")
                    .Replace("Y", "")
                    .Replace("Z", "")
                    .Replace(":", "")
                    .Replace(" ", ", ")
                    .Replace("_", " "));
            }));
            RegisterDevCommand("registerveh", OnRegisterVeh);
            RegisterDevCommand("listdata", OnListData);
            RegisterDevCommand("fix", OnFixVehicle);
            RegisterDevCommand("printvehid", OnPrintVehID);
            client.RegisterEventHandler("Dev.UI.SetEntityUIState", new Action<bool>(OnSetEntityUI));
        }

        public void RegisterDevCommand(string commandName, Action<Command> commandFunc)
        {
            devCommands[commandName] = commandFunc;

            Log.Debug($"Registered dev command /dev {commandName}");
        }

        [EventHandler("Dev.ExecuteClientCommand")]
        public void OnDevCommand(string command, List<object> args)
        {
            if(devCommands.ContainsKey(command))
                devCommands[command](new Command(Game.Player.Handle, args, string.Join(" ", args)));
        }

        [EventHandler("Dev.RequestClientCommands")]
        private void OnRequestCommands()
        {
            Client.TriggerServerEvent("Dev.RecieveClientCommands", devCommands.Keys.ToList());
        }

        private void OnSetEntityUI(bool enable)
        {
            if (enable)
            {
                Client.Instance.RegisterTickHandler(DrawEntityDrawTick);
            }
            else
            {
                Client.Instance.DeregisterTickHandler(DrawEntityDrawTick);
            }
        }

        private async Task DrawEntityDrawTick()
        {
            UI.DevEntityUI.renderUI();
        }

        private void OnRegisterVeh(Command cmd)
        {
            var closeVeh = Cache.PlayerPed.CurrentVehicle;

            if (closeVeh == null)
            {
                closeVeh = GTAHelpers.GetClosestVehicle(6.0f);
            }

            if (closeVeh == null)
            {
                Log.ToChat("No vehicle found nearby");
                return;
            }

            closeVeh.LockStatus = VehicleLockStatus.Unlocked;
            Client.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(closeVeh));

            Log.ToChat("Registered and unlocked the closest vehicle to you");
        }

        private void OnPrintVehID(Command cmd)
        {
            var closeVeh = Cache.PlayerPed.CurrentVehicle;

            if (closeVeh == null)
            {
                closeVeh = GTAHelpers.GetClosestVehicle(6.0f);
            }

            if (closeVeh == null || !closeVeh.HasDecor("Vehicle.ID"))
            {
                Log.ToChat("No vehicle with a ID found nearby");
                return;
            }

            Log.ToChat(closeVeh.GetDecor<int>("Vehicle.ID").ToString());
        }

        private void OnListData(Command cmd)
        {
            try
            {
                var session = Client.Get<SessionManager>();
                var player = session.GetPlayer(cmd.GetArgAs<int>(0));
                if (player == null)
                {
                    Log.ToChat($"Session data is null for server id {cmd.GetArgAs<int>(0)}");
                    return;
                }

                Log.ToChat($"{player.GetGlobalData<string>("Character.FirstName")} {player.GetGlobalData<string>("Character.LastName")}");

                var ui = Client.Get<InteractionUI>();
                var globalDataMenu = new MenuModel
                {
                    headerTitle = "Global data"
                };
                var globalDataItems = new List<MenuItem>();
                foreach (var i in player.GlobalData)
                {
                    globalDataItems.Add(new MenuItemStandard
                    {
                        Title = i.Key,
                        Detail = i.Value.ToString()
                    });
                }

                globalDataMenu.menuItems = globalDataItems;

                var localDataMenu = new MenuModel
                {
                    headerTitle = "Global data"
                };
                var localDataItems = new List<MenuItem>();
                foreach (var i in player.LocalData)
                {
                    localDataItems.Add(new MenuItemStandard
                    {
                        Title = i.Key,
                        Detail = i.Value.ToString()
                    });
                }

                localDataMenu.menuItems = localDataItems;

                ui.RegisterInteractionMenuItem(new MenuItemSubMenu
                {
                    Title = "Global data",
                    SubMenu = globalDataMenu
                }, () => true);
                ui.RegisterInteractionMenuItem(new MenuItemSubMenu
                {
                    Title = "Local data",
                    SubMenu = localDataMenu
                }, () => true);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void OnFixVehicle(Command cmd)
        {
            if (Cache.PlayerPed.IsInVehicle())
            {
                Cache.PlayerPed.CurrentVehicle.Repair();
                Log.ToChat("Vehicle repaired");
            }
        }
    }
}
