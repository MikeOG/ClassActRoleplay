using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
#if SERVER
using Roleplay.Server.Helpers;
using Roleplay.Server.Enums;
using Roleplay.Server.Session;
using Roleplay.Server.Jobs;
#elif CLIENT
using Roleplay.Client.Session;
#endif

namespace Roleplay.Shared
{
    public class Command
    {
        public Command(int src, List<object> arg, string rawCom)
        {
            Source = src; 
            Args = arg; 
            Raw = rawCom;
            Player =
#if SERVER
            Server.Server.Instance.PlayerList[src];
#elif CLIENT
            Client.Client.Instance.PlayerList[src];
#endif
            Session =
#if SERVER
            Server.Server.Instance.Get<SessionManager>().GetPlayer(Player);
#elif CLIENT
            Client.Client.Instance.Get<SessionManager>().GetPlayer(Game.Player);
#endif
        }
        public int Source;
        public List<object> Args;
        public string Raw;
        public Player Player;
        public Session Session;

        public T GetArgAs<T>(int index, T defaultValue = default(T))
        {
            var argValue = Args.ElementAtOrDefault(index);
            if (argValue != null)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(argValue.ToString());
                    }
                }
                catch (Exception e)
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }
    }

    public static class CommandRegister
    {
#if SERVER
        public static void RegisterAdminCommand(string commandName, Action<Command> commandFunc, AdminLevel requiredLevel)
        {
            ACEWrappers.CreateRestrictedCommand(commandName, commandFunc, requiredLevel);
        }

        public static void RegisterJobCommand(string commandName, Action<Command> commandFunc, JobType requiredLevel, bool skipDutyCheck = false)
        {
            var jobTypes = Enum.GetValues(typeof(JobType)).Cast<JobType>().ToList();
            jobTypes.ForEach(o =>
            {
                if(requiredLevel.ToString().Contains(o.ToString()))
                {
                    var dutyFunc = new Action<Command>(cmd =>
                    {
                        if (Server.Server.Instance.Get<JobHandler>().OnDutyAs(cmd.Session, requiredLevel) || skipDutyCheck)
                            commandFunc(cmd);
                    });
                    ACEWrappers.CreateRestrictedCommand(commandName, dutyFunc, o, true);
                }
            });
            
        }

        public static void RegisterRCONCommand(string commandName, Action<Command> commandFunc)
        {
            RegisterAdminCommand(commandName, cmd =>
            {
                if (cmd.Source == 0)
                    commandFunc(cmd);
            }, AdminLevel.SuperAdmin);
        }

        private static void OnCommandExecuted(string commandName, int source, List<object> args, string raw, Action<Command> commandFunc)
        {
            var cmd = new Command(source, args, raw);

            commandFunc(cmd);

            if (cmd.Source != 0) Server.Server.Instance.TriggerLocalEvent("Log.ToDatabase", cmd.Session.PlayerName, cmd.Session.SteamIdentifier, "command", $"used command \'{commandName}\' with the parameters ({raw})");
        }
#elif CLIENT
        private static void OnCommandExecuted(string commandName, int source, List<object> args, string raw, Action<Command> commandFunc)
        {
            commandFunc(new Command(source, args, raw));
            
            Roleplay.Client.Client.Instance.TriggerServerEvent("Log.ToDbType", "command", $"used command \'{commandName}\' with the parameters ({raw})\n");
        }
#endif
        private static Dictionary<string, Action<Command>> commandsWithSpaces = new Dictionary<string, Action<Command>>();

        public static void RegisterCommand(string commandName, Action<Command> commandFunc, bool isRestricted = false)
        {
            var extendedCommand = commandName.Contains('_');
            var commandAliases = commandName.Split('|');

            if (extendedCommand)
            {
                var extendedCommands = CreateExtendedCommands(commandName);
                var baseCommands = GetExtendedCommandSection(commandName, 0);

                var initCmdFunc = commandFunc;

                commandAliases = commandName.Split('_')[0].Split('|');

                foreach (var cmd in extendedCommands)
                {
                    Log.Debug($"Registered extended command ^2{cmd}^0");

                    commandsWithSpaces[cmd] = initCmdFunc;
                }

                commandFunc = cmd =>
                {
                    var baseCommand = cmd.Raw.Split(' ')[0];

                    var enteredCommand = cmd.Raw.Replace(' ', '_').Split('_').ToList();
                    var enteredSpacedCommand = "";
                    var commandExists = commandsWithSpaces.TryGetValue(enteredSpacedCommand, out var actionFunc);

                    do
                    {
                        enteredSpacedCommand = string.Join("_", enteredCommand);

                        enteredCommand.RemoveAt(enteredCommand.Count - 1);
                    } while (!(commandExists = commandsWithSpaces.TryGetValue(enteredSpacedCommand, out actionFunc)) && enteredCommand.Count > 0);

                    if (baseCommands.Any(o => o == baseCommand) && commandExists)
                    {
                        var numberOfSpaces = enteredSpacedCommand.Count(o => o == '_');

                        for (var i = 0; i < numberOfSpaces; i++) cmd.Args.RemoveAt(0); // remove the spaced command sections

                        actionFunc(cmd);
                    }
                };
            }

            commandAliases.ToList().ForEach(o =>
            {
                Log.Verbose($"Registered command ^2{o}^0");
                API.RegisterCommand(o, new Action<int, List<object>, string>((source, args, raw) =>
                {
                    OnCommandExecuted(o, source, args, raw, commandFunc);
                }), isRestricted);
            });
        }

        /// <summary>
        /// Creates a list of extended commands from a inputted command string
        /// </summary>
        /// <param name="commandString">Command string wanting to be parsed</param>
        /// <returns></returns>
        private static List<string> CreateExtendedCommands(string commandString)
        {
            var commands = GetExtendedCommandSection(commandString, 0);
            var currentSectionIndex = 1;

            while (!IsExtendedCommandCreationComplete(commandString, currentSectionIndex))
            {
                var sectionAliases = GetExtendedCommandSection(commandString, currentSectionIndex); // get current section we need

                commands = AddCommandSection(commands, sectionAliases); // append section to current created commands

                currentSectionIndex++;
            }

            return commands;
        }

        /// <summary>
        /// Gets a specific section of an extended command
        /// </summary>
        /// <param name="commandString">Command string wanting to obtain data from</param>
        /// <param name="index">Which section is wanted to be obtain</param>
        /// <returns></returns>
        private static List<string> GetExtendedCommandSection(string commandString, int index)
        {
            var splitCommand = commandString.Split('_');

            if (index <= splitCommand.Length - 1)
            {
                return splitCommand[index].Split('|').ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Adds a section of a extended command to the already existing extended command strings
        /// </summary>
        /// <param name="currentCommandStrings"></param>
        /// <param name="sectionAliases"></param>
        /// <returns></returns>
        private static List<string> AddCommandSection(List<string> currentCommandStrings, List<string> sectionAliases)
        {
            var commandStrings = new List<string>();

            foreach (var curString in currentCommandStrings) // loop through all current entries so next section + aliases will be added to every entry
            {
                foreach (var secAlias in sectionAliases) // loop through all section aliases so all are added
                {
                    commandStrings.Add($"{curString}_{secAlias}");
                }
            }

            return commandStrings;
        }

        private static bool IsExtendedCommandCreationComplete(string commandString, int currentIndex)
        {
            return currentIndex > commandString.Split('_').Length - 1;
        }
    }
}
