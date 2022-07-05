using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.MySQL;
using Roleplay.Server.Players;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Server.Session
{
    class LoginHandler : ServerAccessor
    {
        public LoginHandler(Server server) : base(server)
        {
            server.RegisterEventHandler("Login.RequestCharacters", new Action<Player>(OnCharactersRequest));
            server.RegisterEventHandler("Login.CreateCharacter", new Action<Player, string, string, string>(OnCreateRequest));
            server.RegisterEventHandler("Login.LoadCharacter", new Action<Player, int>(OnLoadRequest));
            server.RegisterEventHandler("Login.DeleteCharacter", new Action<Player, int>(OnCharacterDelete));
            CommandRegister.RegisterAdminCommand("logout", cmd => OnCharactersRequest(cmd.Player), AdminLevel.Developer);
            CommandRegister.RegisterCommand("showlogin", cmd =>
            {
                OnCharactersRequest(cmd.Player);
            });
        }

        /// <summary>
        /// Global method that can be put on any <see cref="ServerAccessor"/> inherited class to load data once a client has spawned into the world
        /// </summary>
        /// <param name="playerSession"></param>
        public void AddSessionData(Session playerSession)
        {

        }

        /// <summary>
        /// Loads characters for target player
        /// </summary>
        /// <param name="source"></param>
        private void OnCharactersRequest([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);

            if (playerSession == null)
            {
                Log.Debug($"{source.Name} Session object is currently null. Re-loading data for login");
                Sessions.OnPlayerLoaded(source); 
            }

            Log.Verbose($"Attempting to get characters for {source.Name}");
            MySQL.execute("SELECT * FROM character_data WHERE SteamID = @steam AND IsActive = true",
                new Dictionary<string, dynamic>
                {
                    ["@steam"] = source.Identifiers[Server.CurrentIdentifier]
                }, new Action<List<object>>(data =>
                {
                    source.TriggerEvent("Login.RecieveCharacters", data);
                    Log.Verbose($"Recieved characters for {source.Name}");
                }));
        }

        /// <summary>
        /// Attemtps to create a character
        /// </summary>
        /// <param name="source"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="dateOfBirth"></param>
        private async void OnCreateRequest([FromSource] Player source, string firstName, string lastName, string dateOfBirth)
        {
            //TODO add validation to this so people can edit via chrome and make extra characters
            Log.Verbose($"Attempting to create character {firstName} {lastName} for {source.Name}");
            var canUseName = await doesNameComboExist(firstName, lastName);
            if (canUseName)
            {
                MySQL.execute("INSERT INTO character_data (`SteamID`, `FirstName`, `LastName`, `DOB`, `Settings`, `Inventory`, `SkinData`) VALUES (@steam, @first, @last, @dob, @settings, @inv, @skin)", new Dictionary<string, dynamic>
                {
                    ["@steam"] = source.Identifiers[Server.CurrentIdentifier],
                    ["@first"] = firstName,
                    ["@last"] = lastName,
                    ["@dob"] = dateOfBirth,
                    ["@settings"] = JsonConvert.SerializeObject(new Dictionary<string, dynamic>()),
                    ["@inv"] = "",
                    ["@skin"] = "",
                }, new Action<dynamic>(rows =>
                {
                    OnCharactersRequest(source);
                    Log.Verbose($"Successfully created character {firstName} {lastName} for {source.Name}");
                }));
            }
            else
            {
                Log.ToClient("[Login]", $"This name is already taken", ConstantColours.Info, source);
                source.TriggerEvent("Login.RecieveCharacters", null);
                Log.Verbose($"Unable to create character {firstName} {lastName} for player {source.Name} due to it already existing");
            }
        }

        private async Task<bool> doesNameComboExist(string firstName, string lastName)
        {
            List<object> nameQuery = await MySQL.executeSync("SELECT SteamID FROM character_data WHERE FirstName = @first AND LastName = @last", new Dictionary<string, dynamic>
            {
                ["@first"] = firstName,
                ["@last"] = lastName
            });

            return nameQuery.ElementAtOrDefault(0) == null;
        }

        /// <summary>
        /// Loads character requested by player (if they actually can use that character)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="charId"></param>
        private async void OnLoadRequest([FromSource] Player source, int charId)
        {
            Session playerSession = Server.Instances.Session.GetPlayer(source);

            if (playerSession == null) return;

            Log.Verbose($"Attempting to load character ({charId}) for {source.Name}");
            /*MySQL.execute("SELECT * FROM character_data WHERE CharID = @id AND SteamID = @steam AND IsActive = true", new Dictionary<string, dynamic>
            {
                ["@steam"] = source.Identifiers[Server.CurrentIdentifier],
                ["@id"] = charId
            }, new Action<List<object>>(data =>
            {
                if (data.ElementAtOrDefault(0) != null)
                {
                    var globalDict = new Dictionary<string, dynamic>();
                    foreach (dynamic i in (dynamic)data[0])
                    {
                        //globalDict[$"Character.{i.Key}"] = i.Value;
                        playerSession.SetGlobalData($"Character.{i.Key}", i.Value);

                    }

                    //playerSession.SetGlobalData(globalDict);
                    playerSession.TriggerEvent("UI.Close");
                    playerSession.TriggerEvent("Player.OnLoginComplete");
                    Server.Instances.Session.TriggerSessionEvent("OnCharacterLoaded", playerSession);
                    Server.Instances.Admin.SendAdminMessage("[Info]", $"{source.Name} joined as {playerSession.GetGlobalData<string>("Character.FirstName")} {playerSession.GetGlobalData<string>("Character.LastName")}", ConstantColours.Info, AdminLevel.Moderator);
                    Server.Instances.BankHandler.LoadCharacterBankAccounts(playerSession.GetGlobalData("Character.CharID", 0));
                    Log.Verbose($"Successfully loaded character ({charId})");
                }
                else
                {
                    Log.ToClient("[Login]", $"Error retrieving data for character id {charId}. This is either a database error or you do not own this character. Please try again or contact a member of staff for assistance", ConstantColours.Info, source);
                }
            }));*/
            var query = new Query<List<dynamic>>("SELECT * FROM character_data", new Dictionary<string, dynamic>
            {
                {"CharID",  charId},
                {"SteamID", source.Identifiers[Server.CurrentIdentifier]}
            });

            var data = await query.Execute();

            if (data.ElementAtOrDefault(0) != null)
            {
                var globalDict = new Dictionary<string, dynamic>();
                foreach (dynamic i in data[0])
                {
                    globalDict[$"Character.{i.Key}"] = i.Value;
                    //playerSession.SetGlobalData($"Character.{i.Key}", i.Value);

                }

                playerSession.SetGlobalData(globalDict);
                await BaseScript.Delay(0);
                playerSession.TriggerEvent("UI.Close");
                playerSession.TriggerEvent("Player.OnLoginComplete");
                Server.Instances.Session.TriggerSessionEvent("OnCharacterLoaded", playerSession);
                Server.Instances.Admin.SendAdminMessage("[Info]", $"{source.Name} joined as {playerSession.GetGlobalData<string>("Character.FirstName")} {playerSession.GetGlobalData<string>("Character.LastName")}", ConstantColours.Info, AdminLevel.Moderator);
                Server.Instances.BankHandler.LoadCharacterBankAccounts(playerSession.GetGlobalData("Character.CharID", 0));
                //BaseScript.TriggerEvent("Phones.AddCharacterNumber", playerSession.ServerID, Server.Get<Phones>().IntToPhoneNumber(playerSession.CharId), /*$"steam:{playerSession.SteamIdentifier}"*/playerSession.CharId);
                Log.Verbose($"Successfully loaded character ({charId})");
            }
            else
            {
                Log.ToClient("[Login]", $"Error retrieving data for character id {charId}. This is either a database error or you do not own this character. Please try again or contact a member of staff for assistance", ConstantColours.Info, source);
            }
        }

        private void OnCharacterDelete([FromSource] Player source, int charId)
        {
            Log.Verbose($"Attempting to delete character ({charId}) for {source.Name}");
            MySQL.execute("SELECT * FROM character_data WHERE CharID = @charid and CreationDate < NOW() - INTERVAL 7 DAY",
                new Dictionary<string, dynamic>() { ["@charid"] = charId }, new Action<List<object>>(result =>
                {
                    if (result.ElementAtOrDefault(0) != null)
                    {
                        IDictionary<string, object> charData = (IDictionary<string, object>)result[0];
                        if ($"{source.Identifiers[Server.CurrentIdentifier]}" == Convert.ToString(charData["SteamID"]))
                        {
                            MySQL.execute("UPDATE character_data SET IsActive = false WHERE CharID = @charid", new Dictionary<string, dynamic>()
                            {
                                ["@charid"] = charId
                            }, new Action<dynamic>(target =>
                            {
                                OnCharactersRequest(source);
                                Log.Verbose($"Successfully deleted character {charId}");
                            }));
                        }
                        else
                        {
                            Log.ToClient("[Login]", "Looks like you're trying to delete a character not linked to your account. So yeh... Don't do that", ConstantColours.Red, source);
                            source.TriggerEvent("Login.RecieveCharacters", null);
                            Log.Warn($"{source.Name} does not own character ({charId}) but attempted to delete it!", "CharacterDeletionError");
                        }
                    }
                    else
                    {
                        Log.ToClient("[Login]", "Your character needs to be more than 7 days old to do this", ConstantColours.Red, source);
                        source.TriggerEvent("Login.RecieveCharacters", null);
                    }
                }));
        }
    }
}
