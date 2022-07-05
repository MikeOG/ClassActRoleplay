using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Vehicle;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Loader;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Dev
{
    public class DevEnviroment : ServerAccessor
    {
        public static bool IsDebugInstance = true;
        public static bool EnableVerboseLogging = true;

        private Dictionary<string, Action<Command>> devCommands = new Dictionary<string, Action<Command>>();
        private List<string> clientDevCommands = new List<string>();

        public DevEnviroment(Server server) : base(server)
        {
            IsDebugInstance = GetConvar("mg_debugInstance", "false") == "true";
            EnableVerboseLogging = GetConvar("mg_enableVerboseLogging", "false") == "true";

            CommandRegister.RegisterAdminCommand("dev", OnDevCommand, AdminLevel.Developer);
            CommandRegister.RegisterAdminCommand("debug", OnSetDebug, AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("verbose", OnSetVerbose, AdminLevel.SuperAdmin);

            RegisterDevCommand("eui", OnSetEntityUI);
            RegisterDevCommand("removevehaccess", OnRemoveVehicleAccess);
            
            Log.Info($"Debug instance is {(IsDebugInstance ? "enabled" : "disabled")}");
            Log.Info($"Verbose logging is {(EnableVerboseLogging ? "enabled" : "disabled")}");
        }

        public void RegisterDevCommand(string commandName, Action<Command> commandFunc)
        {
            devCommands[commandName] = commandFunc;

            Log.Debug($"Registered dev command /dev {commandName}");
        }

        /// <summary>
        /// Requests all client created dev commands to the server (one time only)
        /// </summary>
        /// <param name="playerSession"></param>
        public void OnCharacterLoaded(Session.Session playerSession)
        {
            if (clientDevCommands.Count == 0)
            {
                clientDevCommands.Add("yes");
                playerSession.TriggerEvent("Dev.RequestClientCommands");
            }
        }

        [EventHandler("Dev.RecieveClientCommands")]
        private void OnRecieveClientEvents([FromSource] Player source, List<object> clientCommands)
        {
            var playerSession = Sessions.GetPlayer(source);

            if (playerSession == null || playerSession.AdminLevel < AdminLevel.Moderator && !IsDebugInstance) return;

            Log.Verbose($"Recieved client dev commands from {source.Name}");
            clientDevCommands = clientCommands.Select(o => o.ToString()).ToList();
        }

        private void OnDevCommand(Command cmd)
        {
            var command = cmd.GetArgAs(0, "");
            cmd.Args.RemoveAt(0);

            if (devCommands.ContainsKey(command))
                devCommands[command](cmd);

            if(clientDevCommands.Contains(command))
                cmd.Session.TriggerEvent("Dev.ExecuteClientCommand", command, cmd.Args);
        }

        private void OnSetDebug(Command cmd)
        {
            var overrideState = cmd.GetArgAs(0, "");

            if (overrideState == "")
                IsDebugInstance = !IsDebugInstance;
            else if (overrideState == "on")
                IsDebugInstance = true;
            else
                IsDebugInstance = false;

            SetConvarReplicated("mg_debugInstance", IsDebugInstance.ToString().ToLower());

            Log.Info($"Debug instance {(IsDebugInstance ? "enabled" : "disabled")} by {(cmd.Source == 0 ? "RCON" : cmd.Player.Name)}");
        }

        private void OnSetVerbose(Command cmd)
        {
            var overrideState = cmd.GetArgAs(0, "");

            if (overrideState == "")
                EnableVerboseLogging = !EnableVerboseLogging;
            else if (overrideState == "on")
                EnableVerboseLogging = true;
            else
                EnableVerboseLogging = false;

            SetConvarReplicated("mg_enableVerboseLogging", EnableVerboseLogging.ToString().ToLower());

            Log.Info($"Verbose logging {(EnableVerboseLogging ? "enabled" : "disabled")} by {(cmd.Source == 0 ? "RCON" : cmd.Player.Name)}");
        }

        private void OnSetEntityUI(Command cmd)
        {
            var overrideState = cmd.GetArgAs(0, "");
            var currentState = cmd.Session.GetServerData("Dev.UI.EntityUIState", false);

            if (overrideState == "")
                currentState = !currentState;
            else if (overrideState == "on")
                currentState = true;
            else
                currentState = false;

            cmd.Session.SetServerData("Dev.UI.EntityUIState", currentState);
            cmd.Session.TriggerEvent("Dev.UI.SetEntityUIState", currentState);
        }

        private void OnRemoveVehicleAccess(Command cmd)
        {
            var vehId = cmd.GetArgAs(0, 0);

            Server.Get<VehicleManager>().RemovePlayerVehicleAccess(cmd.Session, vehId);

            cmd.Session.Message("[Dev]", $"Removed vehicle access for vehicle #{vehId}");
        }
    }
}
