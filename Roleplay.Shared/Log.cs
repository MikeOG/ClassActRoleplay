using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using CitizenFX.Core;
#if SERVER
using Roleplay.Server;
using Roleplay.Server.Dev;
using Roleplay.Server.Session;
using System.IO;
#elif CLIENT
using Roleplay.Client;
using Roleplay.Client.Dev;
#endif
using Roleplay.Shared.Helpers;

namespace Roleplay.Shared
{
    public static class Log
    {
#if SERVER
        static Log()
        {
            try
            {
                Server.Server.Instance.RegisterEventHandler("Log.PrintToServer", new Action<Player, string>(PrintClientMessage));
                //Server.Server.Instance.RegisterEventHandler("Log.RequestServerLogs", new Action<Player, long>(OnRequestLogs));
            }
            catch(Exception e)
            {
                Error(e);
            }
        }

        internal static void PrintClientMessage([FromSource] Player source, string message)
        {
            WriteLine($"MESSAGE FROM CLIENT ({source.Name} - {source.Handle})", $"{message}", ConsoleColor.Yellow);
        }

        public static void ToClient(string message, Player target = null, bool Info = true)
        {
            if (Info)
            {
                if (target == null)
                {
                    BaseScript.TriggerClientEvent("Log.PrintToClientConsole", message);
                }
                else
                {
                    BaseScript.TriggerClientEvent(target, "Log.PrintToClientConsole", message);
                }
            }
            else
            {
                ToClientChat("LOG FROM SERVER", Color.FromArgb(120, 255, 100), message, target);
            }
        }

        public static void Message(this Session playerSession, string message, Color prefixColour = default)
        {
            playerSession.Message("LOG", message, prefixColour);
        }

        public static void Message(this Session playerSession, string prefix, string message, Color prefixColour = default)
        {
            if (prefixColour == default) prefixColour = Color.BlueViolet;

            ToClient(prefix, message, prefixColour, playerSession.Source);
        }

        public static void ToClient(string prefix, string message, Color prefixColour = default, Player target = null)
        {
            var colour = prefixColour == default ? Color.FromArgb(255, 255, 255) : prefixColour;

            ToClientChat(prefix, prefixColour, message, target);
        }

        private static void ToClientChat(string prefix, Color prefixColour, string message, Player target = null)
        {
            var colorArray = new int[] { prefixColour.R, prefixColour.G, prefixColour.B };
            var messageArgs = prefix == "" ? new[] { message } : new[] { prefix, message };

            if (target == null)
            {
                BaseScript.TriggerClientEvent("chat:addMessage", new { color = colorArray, multiline = true, args = messageArgs });
            }
            else
            {
                BaseScript.TriggerClientEvent(target, "chat:addMessage", new { color = colorArray, multiline = true, args = messageArgs });
            }
        }

        public static void Warn(string message, string warningType)
        {
            WriteLine($"{warningType} warning", $"{message}", ConsoleColor.Magenta);
        }
#elif CLIENT
        static Log()
        {
            try
            {
                Client.Client.Instance.RegisterEventHandler("Log.PrintToClientConsole", new Action<string>(ServerToClient));
                Client.Client.Instance.RegisterEventHandler("Logs.ReceiveServerLogs", new Action<dynamic>(OnReceveLogs));
            }
            catch(Exception e)
            {
                Error(e);
            }
        }

        internal static void ServerToClient(string message)
        {
            CitizenFX.Core.Debug.WriteLine($@"[LOG FROM SERVER] {message}");
        }

        public static void ToServer(string message)
        {
            Roleplay.Client.Client.Instance.TriggerServerEvent("Log.PrintToServer", $@"{message}");
        }

        public static void ToChat(string message)
        {
            ToChat("LOG", message, ConstantColours.Log);
        }

        public static void ToChat(string prefix, string message, Color prefixColour = default(Color))
        {
            var colorArray = new int[] {prefixColour.R, prefixColour.G, prefixColour.B};
            var messageArgs = prefix == "" ? new[] { "^0" + message } : new[] { prefix, message };

            BaseScript.TriggerEvent("chat:addMessage", new { color = colorArray, multiline = true, args = messageArgs});
        }

        public static void Warn(string message, string warningType)
        {
            ToServer($"[{warningType} warning] {message}");
        }

        private static void OnReceveLogs(dynamic logData)
        {

        }
#endif

        public static void Debug(string message, bool toServer = false, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!DevEnviroment.IsDebugInstance) return;

            var callingLocation = fileName.Split('\\').ToList().Last();
            WriteLine($"Debug: {callingLocation} - {callingMember}", $"{message}"/*, DevEnviroment.IsDebugInstance*/, ConsoleColor.Yellow);
#if CLIENT
            if (toServer)
                ToServer($"[Debug: {callingLocation} - {callingMember}] - {message}");
#endif
        }

        public static void Verbose(string message, bool toServer = false, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!DevEnviroment.EnableVerboseLogging) return;

            var callingLocation = fileName.Split('\\').ToList().Last();
            WriteLine($"Verbose: {callingLocation} - {callingMember}", $"{message}"/*, DevEnviroment.EnableVerboseLogging*/, ConsoleColor.Cyan);

#if CLIENT
            if (toServer)
                ToServer($"[Verbose: {callingLocation} - {callingMember}] - {message}");
#endif
        }

        public static void Error(string errorString, bool toServer = false, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            WriteLine($"Error in {callingLocation}; Calling method: {callingMember}", $"{errorString}", ConsoleColor.Red);

#if CLIENT
            if (toServer)
                ToServer($"[Error in {callingLocation}; Calling method: {callingMember}] - {errorString}");
#endif
        }

        public static void Error(Exception e, bool toServer = false, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            WriteLine($"Error in {callingLocation}; Calling method: {callingMember}", $"The following error ({e.Message}) was caused from {callingMember} on line {e.StackTrace.Split(':').Last()}", ConsoleColor.Red);

#if CLIENT
            if (toServer)
                ToServer($"[Error in {callingLocation}; Calling method: {callingMember}] - The following error ({e.Message}) was caused from {callingMember} on line {e.StackTrace.Split(':').Last()}");
#endif
        }

        public static void Info(string message, bool toServer = false, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var callingLocation = fileName.Split('\\').ToList().Last();
            WriteLine($"Info: {callingLocation} - {callingMember}", $"{message}", ConsoleColor.Gray);
#if CLIENT
            if (toServer)
                ToServer($"[Info: {callingLocation} - {callingMember}] - {message}");
#endif
        }

        private static void WriteLine(string title, string message, ConsoleColor colour)
        {
            message = $"[{title}] {message}";

            if (!DevEnviroment.IsDebugInstance && !DevEnviroment.EnableVerboseLogging)
                message = message.RemoveSection(':', ']'); // Remove file locations from live servers

#if SERVER
            message = message.Replace("^0", "").Replace("^1", "").Replace("^2", "").Replace("^3", "").Replace("^4", "").Replace("^5", "").Replace("^6", "").Replace("^7", "").Replace("^8", "").Replace("^9", "");

            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ResetColor();
#else
            CitizenFX.Core.Debug.WriteLine(message);
#endif
        }
    }
}
