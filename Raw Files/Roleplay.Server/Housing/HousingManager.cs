using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Housing;
using Roleplay.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Roleplay.Server.Bank;
using Roleplay.Server.UI;
using Newtonsoft.Json;

namespace Roleplay.Server.Housing
{
    public class HousingManager : ServerAccessor
    {
        private List<Vector3> houseInteriorLocations = new List<Vector3>
        {
            new Vector3(-781.8839f, 326.3657f, 175.8037f),
            new Vector3(-774.6219f, 331.1073f, 159.0015f),
            new Vector3(-782.1074f, 326.7740f, 222.2576f),
            new Vector3(-774.0350f, 330.4203f, 208.6210f),
            new Vector3(346.45f, -1013.03f, -100.20f),
            new Vector3(266.08f, -1007.52f, -102.0086f),
            new Vector3(151.53f, -1007.58f, -99.9999f),
            new Vector3(-174.31f, 497.53f, 136.67f),
            new Vector3(341.64f, 437.69f, 148.39f),
            new Vector3(373.61f, 423.56f, 144.91f),
            new Vector3(-682.33f, 592.47f, 144.39f),
            new Vector3(-741.18f, 594.18f, 145.26f),
            new Vector3(-758.67f, 618.98f, 143.15f),
            new Vector3(-859.78f, 690.95f, 151.86f),
            new Vector3(-572.00f, 661.68f, 144.84f),
            new Vector3(117.27f, 559.77f, 183.30f),
            new Vector3(-1289.78f, 449.40f, 96.90f),
            new Vector3(1397.06f, 1141.78f, 113.332f),
        };
        private Dictionary<string, Action<Command>> housingCommands = new Dictionary<string, Action<Command>>();

        public HousingManager(Server server) : base(server)
        {
            server.RegisterEventHandler("Player.OnInteraction", new Action<Player>(OnInteraction));
            //server.RegisterEventHandler("Housing.CheckCanStoreVeh", new Action<Player>(OnAttemptStoreVehicle));
            CommandRegister.RegisterCommand("showhouse|showhouses", OnShowHouse);
            RegisterHousingCommand("enter", OnHouseEnter);
            RegisterHousingCommand("exit", OnHouseExit);
            RegisterHousingCommand("allow", OnAllowEntry);
            RegisterHousingCommand("disallow", OnDisallowEntry);
            RegisterHousingCommand("buy", OnBuyCommand);
            RegisterHousingCommand("sell", OnSellCommand);

            CommandRegister.RegisterCommand("house", cmd =>
            {
                var cmdName = cmd.GetArgAs(0, "");

                if (housingCommands.ContainsKey(cmdName))
                {
                    cmd.Args.RemoveAt(0);
                    housingCommands[cmdName](cmd);
                }
            });
            loadHouseOwners();
        }

        public async void OnCharacterLoaded(Session.Session playerSession)
        {
            if (Dev.DevEnviroment.IsDebugInstance) return;

            await playerSession.Transition(600, 900, 600);
   
            if (playerSession.GetGlobalData("Character.Home", 0) != 0 && playerSession.GetGlobalData("Character.SkinData", "") != "")
            {
                var playerHome = HousingLocations.Locations.First(o => o.HouseId == playerSession.GetGlobalData("Character.Home", 0));

                if (!playerHome.HouseEnterAccess.Contains(playerSession.GetGlobalData<int>("Character.CharID")))
                    playerHome.HouseEnterAccess.Add(playerSession.GetGlobalData<int>("Character.CharID"));

                
                playerSession.SetServerData("Character.HomeObject", playerHome);
                //playerSession.TriggerEvent("Housing.SetGarageLocation", playerHome.GarageLocation);
                playerSession.AddGarage(new GarageModel
                {
                    Name = "Home",
                    Location = playerHome.GarageLocation,
                    MaxVehicles = playerHome.MaxGarageVehicles,
                    AlternateDisplayName = "Home garage",
                    BlipOptions = new BlipOptions
                    {
                        Sprite = 357
                    },
                    MarkerOptions = new MarkerOptions
                    {
                        ColorArray = Color.FromArgb(255, 69, 0).ToArray(),
                        ScaleFloat = 1.5f
                    }
                });

                if (playerSession.CharacterSettings.ContainsKey("SpawnLocation")) return;
                playerSession.SetPlayerPosition(playerHome.EntranceLocation);
            }
        }

        public void RegisterHousingCommand(string cmdName, Action<Command> cmdFunc)
        {
            housingCommands.Add(cmdName, cmdFunc);
        }

        public bool DoesHouseHaveOwner(HousingDataModel house)
        {
            var currentPlayers = Sessions.PlayerList;

            return house.HouseEnterAccess.Count > 0;
        }

        public bool HasOwnership(HousingDataModel house, Session.Session playerSession, bool absolouteOwner = false)
        {
            return absolouteOwner ? house.HouseId == playerSession.GetGlobalData("Character.Home", 0) : house.HouseEnterAccess.Contains(playerSession./*GetCharId()*/CharId) || house.HouseId == playerSession.GetGlobalData("Character.Home", 0);
        }

        public HousingDataModel GetPlayerHome(Session.Session playerSession)
        {
            return playerSession.GetServerData<HousingDataModel>("Character.HomeObject", null);
        }

        public HousingDataModel GetClosestHouseFromLocation(Vector3 pos)
        {
            return HousingLocations.Locations.FirstOrDefault(o => o.EntranceLocation.DistanceToSquared(pos) < 6.0f);
        }

        private void OnInteraction([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var closestHouse = GetClosestHouseFromLocation(playerSession.GetPlayerPosition());

            if (closestHouse != null)
            {
                Log.Debug($"{playerSession.PlayerName} just interacted with house {closestHouse.HouseAddress}");
                var currentHouse = playerSession.GetGlobalData("Character.Home", 0);
                var hasHasOwner = DoesHouseHaveOwner(closestHouse);

                Log.ToClient("[Housing]", $"{closestHouse.HouseAddress}", ConstantColours.Housing, source);
                if (!hasHasOwner)
                {
                    if (currentHouse == 0)
                    {
                        Log.ToClient("[Housing]", $"This house is currently on sale for ${closestHouse.HousePrice}. Garage size: {closestHouse.MaxGarageVehicles} vehicles. To buy this property do /house buy", ConstantColours.Housing, source);
                    }
                    else
                    {
                        Log.ToClient("[Housing]", $"This house is currently for sale but you already own one! You must sell your current house first before trying to buy another one", ConstantColours.Housing, source);
                    }
                }
                else
                {
                    if (HasOwnership(closestHouse, playerSession, true))
                    {
                        Log.ToClient("[Housing]", "Current commands (all start with /house):\nenter, exit, allow [id], disallow [id], sell", ConstantColours.Housing, source);
                    }
                    else
                    {
                        Log.ToClient("[Housing]", "This house is currently owned by somebody", ConstantColours.Housing, source);
                    }
                }
            }
        }

        private void OnHouseEnter(Command cmd)
        {
            var targetHome = GetClosestHouseFromLocation(cmd.Session.GetPlayerPosition());
            var playerPos = cmd.Session.GetPlayerPosition();

            if (targetHome == null) return;

            Log.Debug($"{cmd.Session.PlayerName} is close to house #{targetHome.HouseId}");
            if (!HasOwnership(targetHome, cmd.Session))
            {
                Log.Debug($"{cmd.Session.PlayerName} has no house perms at all for house #{targetHome.HouseId}");
                targetHome = null;
            }

            if (targetHome != null && playerPos.DistanceToSquared(targetHome.EntranceLocation) < 6.0f)
            {
                var interiorPos = getHouseEntryPos(targetHome.HouseType, targetHome.HouseId);

                cmd.Session.SetServerData("Character.CurrentHouseObj", targetHome);
                cmd.Session.SetPlayerPosition(interiorPos);
                cmd.Session.SetGlobalData("Character.Instance", targetHome.HouseId);
            }
        }

        private void OnHouseExit(Command cmd)
        {
            var currentHome = cmd.Session.GetServerData<HousingDataModel>("Character.CurrentHouseObj", null);
            var playerPos = cmd.Session.GetPlayerPosition();

            if (currentHome != null && houseInteriorLocations.Any(o => playerPos.DistanceToSquared(o) < 3.0f)) // If they are close to an interior entrance they are basically at a door to exit
            {
                cmd.Session.SetServerData("Character.CurrentHouseObj", null);
                cmd.Session.SetPlayerPosition(currentHome.EntranceLocation);
                cmd.Session.SetGlobalData("Character.Instance", 0);
            }
        }

        private Vector3 getHouseEntryPos(string houseType, int houseId)
        {
            if (houseType == "ULTRALOW")
            {
                return houseInteriorLocations[5];
            }
            else if (houseType == "LOW")
            {
                return houseInteriorLocations[6];
            }
            else if (houseType == "MED")
            {
                return houseInteriorLocations[4];
            }
            else if (houseType == "HOUSE")
            {
                return houseInteriorLocations[16];
            }
            else if (houseType == "UNIQUE")
            {
                return houseInteriorLocations[17];
            }
            else if (houseType == "HIGHEND")
            {
                if (houseId == 193)
                {
                    return houseInteriorLocations[14];
                }
                else if (houseId == 196)
                {
                    return houseInteriorLocations[10];
                }
                else if (houseId == 197)
                {
                    return houseInteriorLocations[12];
                }
                else if (houseId == 194)
                {
                    return houseInteriorLocations[13];
                }
            }
            else if (houseType == "ULTRA")
            {
                return houseInteriorLocations[1];
            }

            return Vector3.Zero;
        }

        private void OnAllowEntry(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var playerHome = cmd.Session.GetServerData<HousingDataModel>("Character.HomeObject", null);

            if (playerHome != null)
            {
                var targetSession = Sessions.GetPlayer(targetId);
                if (targetSession == null)
                {
                    Log.ToClient("[Housing]", $"ID {targetId} not found", ConstantColours.Housing, cmd.Session.Source);
                    return;
                }   

                playerHome.HouseEnterAccess.Add(targetSession./*GetCharId()*/CharId);
                Log.ToClient("[Housing]", $"You just got given access to {cmd.Session.GetCharacterName()}' house", ConstantColours.Housing, targetSession.Source);
                Log.ToClient("[Housing]", $"You just gave {targetSession.GetGlobalData("Character.FirstName", "")} access to your house", ConstantColours.Housing, cmd.Session.Source);
            }
        }

        private void OnDisallowEntry(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var playerHome = cmd.Session.GetServerData<HousingDataModel>("Character.HomeObject", null);

            if (playerHome != null)
            {
                var targetSession = Sessions.GetPlayer(targetId);
                playerHome.HouseEnterAccess.Remove(targetSession./*GetCharId()*/CharId);
                Log.ToClient("[Housing]", $"You just revoked {targetSession.GetGlobalData("Character.FirstName", "")} access to your house", ConstantColours.Housing, cmd.Session.Source);
            }
        }

        private void OnBuyCommand(Command cmd)
        {
            var closeHouse = GetClosestHouseFromLocation(cmd.Session.GetPlayerPosition());

            if (closeHouse != null)
            {
                var houseHasOwner = DoesHouseHaveOwner(closeHouse);
                if (!houseHasOwner)
                {
                    if (Server.Get<PaymentHandler>().CanPayForItem(cmd.Session, closeHouse.HousePrice))
                    {
                        Server.Get<PaymentHandler>().PayForItem(cmd.Session, closeHouse.HousePrice, $"buying house {closeHouse.HouseAddress}");
                        cmd.Session.SetGlobalData("Character.Home", closeHouse.HouseId);
                        cmd.Session.SetServerData("Character.HomeObject", closeHouse);
                        cmd.Session.TriggerEvent("Housing.SetGarageLocation", closeHouse.GarageLocation.ToArray());
                        closeHouse.HouseEnterAccess.Add(cmd.Session./*GetCharId()*/CharId);
                        Log.ToClient("[Housing]", $"You just bought {closeHouse.HouseAddress} for ${closeHouse.HousePrice}. If you are not around for 3 weeks this property will lose ownership of this property", ConstantColours.Housing, cmd.Player);
                    }
                    else
                    {
                        Log.ToClient("[Bank]", "You cannot afford to buy this property", ConstantColours.Bank, cmd.Player);
                    }
                }
            }
        }

        private void OnSellCommand(Command cmd)
        {
            var closeHouse = GetClosestHouseFromLocation(cmd.Session.GetPlayerPosition());

            if (closeHouse != null)
            {
                var isHouseOwner = HasOwnership(closeHouse, cmd.Session, true);
                if (isHouseOwner)
                {
                    closeHouse.HouseEnterAccess.Clear();
                    cmd.Session.SetServerData("Character.HomeObject", null);
                    cmd.Session.SetGlobalData("Character.Home", 0);

                    var houseSellPrice = closeHouse.HousePrice;
                    Server.Get<PaymentHandler>().UpdateBankBalance(cmd.Session.GetBankAccount(), houseSellPrice, cmd.Session, $"selling house {closeHouse.HouseAddress}");
                    Log.ToClient("[Bank]", $"You just sold your property for ${houseSellPrice}", ConstantColours.Bank, cmd.Player);
                }
            }
        }

        [EventHandler("Housing.CheckCanStoreVeh")]
        private void OnAttemptStoreVehicle([FromSource] Player source, int curVehId)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var playerHome = GetPlayerHome(playerSession);

            if (playerHome != null)
            {
                var playerPos = playerSession.GetPlayerPosition();
                if (playerPos.DistanceToSquared(playerHome.GarageLocation) < 9.0f)
                {
                    MySQL.execute("SELECT Count(*) AS NumVehs FROM vehicle_data WHERE Garage = 'Home' AND CharID = @char AND VehID != @curveh", new Dictionary<string, dynamic>{{"@char", playerSession./*GetCharId()*/CharId }, {"@curveh", curVehId } },
                        new Action<List<dynamic>>(count =>
                        {
                            Log.Debug($"Count of garage vehicles for house {playerHome.HouseAddress} is {count[0].NumVehs}");
                            playerSession.TriggerEvent("Vehicle.UpdateHouseGarageStore", Convert.ToInt32(count[0].NumVehs) <= playerHome.MaxGarageVehicles, playerHome.MaxGarageVehicles);
                        }));
                }
            }
        }

        private void OnShowHouse(Command cmd)
        {
            cmd.Session.TriggerEvent("Housing.ToggleHouseBlips", JsonConvert.SerializeObject(HousingLocations.Locations.Where(DoesHouseHaveOwner)));
        }

        private async void loadHouseOwners()
        {
            await BaseScript.Delay(5000);
            MySQL.execute("SELECT CharID, Home FROM character_data WHERE Home != 0", new Dictionary<string, dynamic>(),
                new Action<List<dynamic>>(result =>
                {   
                    Log.Debug($"Loading home owners");
                    result.ForEach(character =>
                    {
                        var house = HousingLocations.Locations.FirstOrDefault(o => o.HouseId == character.Home);

                        if (house != null)
                        {
                            house.HouseEnterAccess.Add(character.CharID);
                            Log.Debug($"Added character ID {character.CharID} to the access list for house ID {house.HouseId}");
                        }
                    });
                }));
        }
    }
}
