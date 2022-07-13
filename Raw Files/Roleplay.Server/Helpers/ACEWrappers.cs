using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Server.Enums;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared;

namespace Roleplay.Server.Helpers
{
    public static class ACEWrappers
    {
        public static void CreateRestrictedCommand<T>(string commandName, Action<Command> commandFunc, T specifiedLevel, bool specifiedLevelOnly = false) where T : struct, Enum
        {
            var commandAliases = commandName.Split('_')[0].Split('|');
            var specifiedLevelValue = (int)(object)specifiedLevel;
            var levels = Enum.GetValues(typeof(T));
            var adminLevels = new List<int>();
            foreach(int i in levels)
                adminLevels.Add(i);

            commandAliases.ToList().ForEach(o =>
            {
                Log.Debug($"Adding restricted command {o} for group {Enum.GetName(typeof(T), specifiedLevelValue)}");
                if (!specifiedLevelOnly)
                {
                    adminLevels.ForEach(level =>
                    {
                        if (level >= specifiedLevelValue)
                            ExecuteCommand($"add_ace group.{Enum.GetName(typeof(T), level)} command.{o} allow");
                    });
                }
                else
                {
                    ExecuteCommand($"add_ace group.{Enum.GetName(typeof(T), specifiedLevelValue)} command.{o} allow");
                }
            });
            CommandRegister.RegisterCommand(commandName, commandFunc, true);
        }

        public static void AddPrivellagedUser<T>(string identifier, T specifiedLevel) where T : struct, Enum
        {
            var groupName = Enum.GetName(typeof(T), (int)(object)specifiedLevel);
            if(!IsPrincipalAceAllowed($"identifier.{identifier}", $"group.{groupName}"))
            {
                Log.Info($"Adding identifier {identifier} to group {groupName}");
                ExecuteCommand($"add_principal identifier.{identifier} group.{groupName}");
                ExecuteCommand($"add_ace identifier.{identifier} group.{groupName} allow");
            }
        }

        public static void RemovePrivellagedUser<T>(string identifier, T specifiedLevel) where T : struct, Enum
        {
            var groupName = Enum.GetName(typeof(T), (int)(object)specifiedLevel);
            if (IsPrincipalAceAllowed($"identifier.{identifier}", $"group.{groupName}"))
            {
                Log.Info($"Removing identifier {identifier} from group {groupName}");
                ExecuteCommand($"remove_principal identifier.{identifier} group.{groupName}");
                ExecuteCommand($"remove_ace identifier.{identifier} group.{groupName} allow");
            }
        }
    }
}
