using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roleplay.Server.Players
{
    /*public class Twitter : ServerAccessor
    {
        public Twitter(Server server) : base(server)
        {
            //CommandRegister.RegisterCommand("tweet", OnTweetCommand);
            //CommandRegister.RegisterCommand("sethandle", OnSetHandle);
            //ommandRegister.RegisterCommand("mutetwitter|mtwitter", OnMuteTwitter);
        }

        public string GetTwitterHandle(Session.Session playerSession)
        {
            var playerSettings = playerSession.CharacterSettings;
            var hasTwitterHandle = playerSettings.ContainsKey("TwitterHandle");
            var twitterHandle = $"{playerSession.GetGlobalData("Character.FirstName", "")}_{playerSession.GetGlobalData("Character.LastName", "")}";

            if (hasTwitterHandle)
            {
                twitterHandle = playerSettings["TwitterHandle"];
            }

            return "@" + twitterHandle;
        }

        public Dictionary<string, dynamic> UpdateTwitterHandle(Session.Session playerSession, string newHandle)
        {
            var playerSettings = playerSession.CharacterSettings;
            var currentTwitterHandle = GetTwitterHandle(playerSession);

            playerSettings["TwitterHandle"] = newHandle;
            playerSession.CharacterSettings = playerSettings;
            Log.Verbose($"Setting {playerSession.PlayerName} twitter handle from {currentTwitterHandle} to {newHandle}");

            return playerSettings;
        }

        public void CheckHandleExistance(string handle, Action<bool> cb)
        {
            MySQL.execute("SELECT Settings FROM character_data WHERE IsActive = true", new Dictionary<string, dynamic>(), 
                new Action<List<dynamic>>(data =>
            {
                foreach(var item in data)
                {
                    try
                    {
                        Dictionary<string, dynamic> settings = item.Settings == "" ? new Dictionary<string, dynamic>() : JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(item.Settings);
                        if (settings.ContainsKey("TwitterHandle"))
                        {
                            if (settings["TwitterHandle"] == handle)
                            {
                                cb(true);
                                return;
                            }
                        }
                    }
                    catch (JsonException e)
                    {
                        Log.Error(e);
                    }
                }

                cb(false);
            }));
        }

        private string parseTweetMessage(List<string> tweetArgs, Session.Session playerSession)
        {
            var playerHandle = GetTwitterHandle(playerSession);
            var tweetMessage = "";

            if (tweetArgs.Count == 0) return "";

            foreach (var arg in tweetArgs)
            {
                if (arg.Contains('#'))
                {
                    tweetMessage += $"^4{arg}^0"; // Hashtags are blue
                }
                else if (arg.Contains("@"))
                {
                    if (String.Equals(arg, playerHandle, StringComparison.CurrentCultureIgnoreCase))
                    {
                        tweetMessage += $"^2{arg}^0"; // Ours so make it green
                    }
                    else
                    {
                        tweetMessage += $"^5{arg}^0"; // Not ours so make it blue
                    }
                }
                else
                {
                    tweetMessage += arg; // normal
                }

                tweetMessage += " ";
            }

            return tweetMessage;
        }

        private void OnTweetCommand(Command cmd)
        {
            var twitterHandle = GetTwitterHandle(cmd.Session);
            var tweetArgs = cmd.Raw.Split(' ').ToList();
            tweetArgs.RemoveAt(0);

            if (cmd.Session.CuffState != CuffState.None || cmd.Session.DragState == DragState.Dragged) return;

            if (cmd.Session.GetGlobalData("Character.JailTime", 0) > 0) return;

            Sessions.ForAllClients(player =>
            {
                if (player.PlayerSettings.ContainsKey("MuteTwitter") && player.PlayerSettings["MuteTwitter"]) return;

                var tweetMessage = parseTweetMessage(tweetArgs, player);

                player.TriggerEvent("chat:addMessage", new
                {
                    templateId = "tweet",
                    multiline = true,
                    args = new[]
                    {
                        twitterHandle,
                        tweetMessage
                    }
                });
                cmd.Player.TriggerEvent("Player.PlayTextAnim");
            });
        }

        private void OnSetHandle(Command cmd)
        {
            var playerSession = cmd.Session;
            var newHandle = cmd.GetArgAs(0, "").Replace(" ", "").Replace("-", "").Replace("^", "").Replace("@", "");

            Log.ToClient("[Twitter]", $"Checking handle usage please wait", ConstantColours.Housing, cmd.Player);
            if(!playerSession.GetServerData("Twitter.CheckingHandle", false))
            {
                var playerSettings = playerSession.CharacterSettings;
                var lastTwitterUpdateTick = playerSettings.ContainsKey("LastTwitterHandleUpdate") ? playerSettings["LastTwitterHandleUpdate"] : 0;
                var lastTwitterUpdate = ((TimeSpan)(TimeSpan.FromTicks(DateTime.Now.Ticks) - TimeSpan.FromTicks(lastTwitterUpdateTick))).TotalHours;

                playerSession.SetServerData("Twitter.CheckingHandle", true);

                if(lastTwitterUpdate > 6)
                {
                    CheckHandleExistance(newHandle, handleInUse =>
                    {
                        if (!handleInUse)
                        {
                            var previousHandle = GetTwitterHandle(playerSession);
                            playerSettings = UpdateTwitterHandle(playerSession, newHandle);
                            Log.ToClient("[Twitter]", $"Set twitter handle to {newHandle}", ConstantColours.Housing, cmd.Player);

                            playerSettings["LastTwitterHandleUpdate"] = DateTime.Now.Ticks;
                            if (!playerSettings.ContainsKey("PreviousTwitterHandles"))
                            {
                                playerSettings["PreviousTwitterHandles"] = new List<Dictionary<string, dynamic>>();
                            }

                            if(playerSettings["PreviousTwitterHandles"].GetType() != typeof(List<Dictionary<string, dynamic>>)) // If this has been pulled from db it won't deserialize into what we want. work around time!
                            {
                                playerSettings["PreviousTwitterHandles"] = ((JArray)playerSettings["PreviousTwitterHandles"]).ToObject<List<Dictionary<string, dynamic>>>();
                            }

                            playerSettings["PreviousTwitterHandles"].Add(new Dictionary<string, dynamic>
                            {
                                {"Handle", previousHandle},
                                {"CreatedOn", lastTwitterUpdate}
                            });
                            playerSession.CharacterSettings = playerSettings;
                        }
                        else
                        {
                            Log.ToClient("[Twitter]", $"The handle {newHandle} is currently in use", ConstantColours.Housing, cmd.Player);
                        }

                        playerSession.SetServerData("Twitter.CheckingHandle", false);
                    });
                }
                else
                {
                    Log.ToClient("[Twitter]", $"You can only update your twitter handle once every 6 hours", ConstantColours.Housing, cmd.Player);
                    playerSession.SetServerData("Twitter.CheckingHandle", false);
                }
            }
        }

        private void OnMuteTwitter(Command cmd)
        {
            var playerSettings = cmd.Session.PlayerSettings;
            var newMuteStatus = !(playerSettings.ContainsKey("MuteTwitter") && playerSettings["MuteTwitter"]);

            playerSettings["MuteTwitter"] = newMuteStatus;
            cmd.Session.PlayerSettings = playerSettings;
            Log.ToClient("[Twitter]", $"{(newMuteStatus ? "Muted" : "Unmuted")} twitter", ConstantColours.Housing, cmd.Player);
        }
    }*/
}
