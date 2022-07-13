using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Locations;
using Roleplay.Shared.Models;

namespace Roleplay.Server.Jobs.Criminal
{
    public class Robberies : ServerAccessor
    {
        private float maxDistance = 3.0f;
        private List<RobberyModel> robbableLocations = new List<RobberyModel>();

        public Robberies(Server server) : base(server)
        {
            server.RegisterEventHandler("Robbery.AttemptStartRob", new Action<Player, string>(OnRobRequest));           
            //server.RegisterEventHandler("Robbery.OnRobberyCompleted", new Action<Player>(OnRobberyComplete));
            server.RegisterEventHandler("Robbery.CheckCanRobStore", new Action<Player, string>(OnRobberyRequest));
            server.RegisterEventHandler("Robbery.RequestPayout", new Action<Player, string>(OnPayoutRequest));
            server.RegisterEventHandler("Robbery.RobberyIncomplete", new Action<string>(OnRobberyIncomplete));
            server.RegisterEventHandler("Robbery.CheckCanRobRegister", new Action<Player, string>(CheckCanRobRegister));
            server.RegisterEventHandler("Robbery.RequestRegisterPayment", new Action<Player, string>(OnRegisterPayment));
            createRobberyModels();

            CommandRegister.RegisterCommand("getrobberystates", cmd =>
            {             
                foreach (var location in robbableLocations)
                {
                    Log.Info("-----------------------------------------------------------------------------------------------------------------------");
                    Log.Info($"Locaion: {location.LocationName}");
                    Log.Info($"Cooldown time: {location.CooldownTime}");
                    Log.Info($"Is bank: {location.IsBank}");
                    Log.Info($"Is being robbed: {location.IsBeingRobbed}");
                    Log.Info($"Is robbable: {location.IsRobbable}");
                    Log.Info($"Required police: {location.RequiredPolice}");
                    Log.Info($"Times robbed: {location.TimesRobbed}");
                    Log.Info("-----------------------------------------------------------------------------------------------------------------------");
                }
            });
        }

        public RobberyModel GetRobberyModel(string locationName) => robbableLocations.FirstOrDefault(o => o.LocationName == locationName);

        private void OnRobRequest([FromSource] Player source, string bank)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            var robberyModel = GetRobberyModel(bank);
            List<Session.Session> copsOnDuty = Server.Instances.Jobs.GetPlayersOnJob(JobType.Police);

            Log.Verbose($"{source.Name} is requesting to rob bank {bank}");
            if (!robberyModel.IsBeingRobbed && robberyModel.IsRobbable && copsOnDuty.Count >= robberyModel.RequiredPolice)
            {
                if(robberyModel.IsBank)
                {
                    var bankNameSplit = robberyModel.LocationName.AddSpacesToCamelCase().Split(' ').ToList();
                    var bankNewList = new List<string>(bankNameSplit);
                    bankNewList.Remove(bankNameSplit.Last());
                    var bankDisplayName = string.Join(" ", bankNewList);
                    if (/*playerSession.GetLocalData("Bank.CanRobVault", false) &&*/ robberyModel.IsVault || (!robberyModel.IsVault && !robberyModel.TellerBeenRobbed))
                    {
                        var robType = bankNameSplit.Last() == "Main" ? "teller" : "vault";
                        Server.Instances.Jobs.SendJobAlert(JobType.Police | JobType.EMS, "[Dispatch]", $"{bankDisplayName} {robType} is being robbed", ConstantColours.Dispatch);
                        Log.ToClient("[Robbery]", $"You started robbing the {robType}. Don't go to far away or the robbery will be cancelled", ConstantColours.Blue, source);
                        startRobbery(playerSession, robberyModel);
                    }
                    else
                    {
                        Log.ToClient("[Robbery]", "You are not able to rob this", ConstantColours.Blue, source);
                    }
                }
                else
                    Log.Debug($"Somehow a bank rob request was sent but the object obtained was not a bank? Sent robbery name was {bank}");
            }
            else
            {
                Log.ToClient("[Robbery]", "This has either been recently robbed or there are not enough police around", ConstantColours.Blue, source);
            }
        }

        private void createRobberyModels()
        {
            foreach (var bank in BankLocations.Positions)
            {
                robbableLocations.Add(new RobberyModel
                {
                    LocationName = bank.Key,
                    IsBank = true
                });
            }
            foreach (var store in StoreLocations.Positions)
            {
                robbableLocations.Add(new RobberyModel
                {
                    LocationName = store.Key,
                    IsBank = false
                });
            }
        }

        private void startRobbery(Session.Session player, RobberyModel location)
        {
            location.IsRobbable = false;
            location.IsBeingRobbed = true;
            var robberySeconds = location.TimeToRob / 1000;
            var currentSecond = 0;
            var isCheckingLocation = false;
            Task.Factory.StartNew(async () =>
            {
                while(location.IsBeingRobbed)
                {
                    await BaseScript.Delay(1000);
                    currentSecond++;
                    Log.Debug($"{location.LocationName} remaining time = {robberySeconds - currentSecond} seconds");
                    if (currentSecond >= robberySeconds && !isCheckingLocation && location.IsBeingRobbed)
                    {
                        var robberyPayout = location.Payout;
                        if (!location.IsVault)
                        {
                            Log.ToClient("[Robbery]", $"You got ${robberyPayout} from the teller window", ConstantColours.Blue, player.Source);
                            Server.Get<PaymentHandler>().UpdatePlayerCash(player, robberyPayout, $"teller robbery ({location.LocationName})");
                            player.SetLocalData("Bank.CanRobVault", true);
                            player.TriggerEvent("Bank.ShowVaultMarker", location.LocationName.AddSpacesToCamelCase().Split(' ')[0] + "Vault");
                            location.TellerBeenRobbed = true;
                            location.IsBeingRobbed = false;
                            location.IsRobbable = true;
                        }
                        else
                        {
                            var playerInv = new PlayerInventory(player.GetGlobalData("Character.Inventory", ""), player);
                            playerInv.AddItem("DirtyMoney", robberyPayout);
                            Log.ToClient("[Robbery]", $"You got ${robberyPayout} in dirty money from the vault", ConstantColours.Blue, player.Source);
                            StartGlobalCooldown(location, false);
                            endRobbery(location);
                        }
                        return;
                    }
                    else
                    {
#pragma warning disable 4014
                        //Task.Factory.StartNew(() =>
#pragma warning restore 4014
                             {
                                 if (!isCheckingLocation)
                                 {
                                     isCheckingLocation = true;
                                     /*var currentTick = 0;
                                     playerLocations[player] = Vector3.Zero;
                                     player.TriggerEvent("Robbery.UpdateRobberyPosition");
                                     while (playerLocations[player] == Vector3.Zero)
                                     {
                                         await BaseScript.Delay(0);
                                         currentTick++;
                                         if (currentTick >= 400)
                                         {
                                             break;
                                         }
                                     }
     
                                     if (currentTick >= 400)
                                     {
                                         Log.Verbose($"Robbery on location {location.LocationName} cancelled due to coord request timeout");
                                         endRobbery(location);
                                         return;
                                     }*/

                                     var playerPos = /*playerLocations[player]await player.GetPosition();*/player.GetPlayerPosition();
                                     var bankPos = BankLocations.Positions[location.LocationName];
                                     if (bankPos.DistanceToSquared(playerPos) > Math.Pow(maxDistance, 2))
                                     {
                                         Log.ToClient("[Robbery]", "You moved to far away from the location", ConstantColours.Blue, player.Source);
                                         StartGlobalCooldown(location, false);
                                         endRobbery(location);
                                         return;
                                     }

                                     isCheckingLocation = false;
                                 }
                             }
                    }
                }
            });
        }

        private void endRobbery(RobberyModel bank)
        {
            bank.IsBeingRobbed = false;
            startRobberyCooldown(bank);
            Log.Debug($"End robbery for bank {bank.LocationName}");
        }

        private void startRobberyCooldown(RobberyModel location, int cooldownTime = -1)
        {
            location.IsRobbable = false;
            location.TimesRobbed = 111;
            Task.Factory.StartNew(async () =>
            {
                await BaseScript.Delay(cooldownTime == -1 ? location.CooldownTime : cooldownTime);
                location.IsRobbable = true;
                location.IsBeingRobbed = false;
                location.TellerBeenRobbed = false;
                if(!location.InRegisterCooldown)
                {
                    location.TimesRobbed = 0;
                    location.InRegisterCooldown = false;
                }
            });
        }

        private void OnRobberyRequest([FromSource] Player source, string location)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null || playerSession.GetServerData("Character.CanDoRobbery", false)) return;

            var robberyModel = GetRobberyModel(location);
            var copsOnDuty = Server.Get<JobHandler>().GetPlayersOnJob(JobType.Police);

            Log.Verbose($"{source.Name} is requesting to rob {location} vault");
            if (!robberyModel.IsBeingRobbed && robberyModel.IsRobbable && copsOnDuty.Count >= robberyModel.RequiredPolice)
            {
                playerSession.SetServerData("Character.CanDoRobbery", true);
                robberyModel.IsBeingRobbed = true;
                source.TriggerEvent("Robbery.StartRobbery", true);
                StartGlobalCooldown(robberyModel);
                var nameSplit = robberyModel.LocationName.AddSpacesToCamelCase().Split(' ').ToList();
                nameSplit.Remove(nameSplit.Last());
                Server.Instances.Jobs.SendJobAlert(JobType.Police | JobType.EMS, "[Dispatch]", $"Silent Alarm | {string.Join(" ", nameSplit)} general store vault", ConstantColours.Dispatch);
                Task.Factory.StartNew(async () =>
                {
                    playerSession.SetServerData("Character.CanDoRobbery", true);
                    await BaseScript.Delay(300000);
                    playerSession.SetServerData("Character.CanDoRobbery", false);
                });
                Task.Factory.StartNew(async () =>
                {
                    var vaultLocation = source.Character.Position;
                    while (robberyModel.IsBeingRobbed)
                    {
                        await BaseScript.Delay(60000);

                        var playerNearLocaiton = false;
                        var currentPlayers = Sessions.PlayerList;

                        foreach (var player in currentPlayers)
                        {
                            if(playerNearLocaiton) continue;

                            if (player.GetPlayerPosition().DistanceToSquared(vaultLocation) <= 20.0f)
                                playerNearLocaiton = true;
                        }

                        if (!playerNearLocaiton)
                        {
                            Log.Verbose($"No player was found near the vault {robberyModel.LocationName} setting IsBeingRobbed to false");
                            robberyModel.IsBeingRobbed = false;

                            return;
                        }
                    }
                });
            }
            else
            {
                source.TriggerEvent("Robbery.StartRobbery", false);
                Log.ToClient("[Robbery]", "This vault is unable to be robbed at this time", ConstantColours.Blue, source);
            }
        }

        [EventHandler("Robbery.OnRobberyCompleted")]
        private void OnRobberyComplete([FromSource] Player source, string location)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            var robberyModel = GetRobberyModel(location);
            if (playerSession == null || robberyModel == null) return;

            Log.Verbose($"{source.Name} completed a robbery and can now request payout");
            playerSession.SetLocalData("Robbery.CanRequestPayout", true);
            robberyModel.IsRobbable = false;

            startRobberyCooldown(robberyModel);
        }

        private void OnPayoutRequest([FromSource] Player source, string location)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            if(playerSession.GetLocalData("Robbery.CanRequestPayout", false))
            {
                playerSession.SetLocalData("Robbery.CanRequestPayout", false);
                var robberyModel = GetRobberyModel(location);
                var payout = robberyModel.Payout;
                Log.ToClient("[Robbery]", $"You got ${payout} from the vault", ConstantColours.Blue, source);
                Server.Get<PaymentHandler>().UpdatePlayerCash(playerSession, payout, $"store vault robbery ({robberyModel.LocationName})");

                Log.Debug($"Giving {source.Name} ${payout} from {location} vault");
            }
        }

        private void OnRobberyIncomplete(string location)
        {
            var robberyModel = GetRobberyModel(location);
            
            Log.Verbose($"Robbery for location {location} was incomplete allowing it to be robbable again");
            Task.Factory.StartNew(async () =>
            {
                await BaseScript.Delay(60000);
                robberyModel.IsBeingRobbed = false;
            });
        }

        private void CheckCanRobRegister([FromSource] Player source, string location)
        {
            var robberyModel = GetRobberyModel(location);
            List<Session.Session> copsOnDuty = Server.Instances.Jobs.GetPlayersOnJob(JobType.Police);

            Log.Verbose($"Checking if location {location} registers are robbable");
            if(robberyModel.TimesRobbed < 1 && copsOnDuty.Count >= robberyModel.RequiredPolice)
            {
                source.TriggerEvent("Robbery.StartRegisterRobbery", true);
                var nameSplit = robberyModel.LocationName.AddSpacesToCamelCase().Split(' ').ToList();
                nameSplit.Remove(nameSplit.Last());
                Server.Instances.Jobs.SendJobAlert(JobType.Police | JobType.EMS, "[Dispatch]", $"Silent Alarm | {string.Join(" ", nameSplit)} general store registers", ConstantColours.Dispatch);
                robberyModel.TimesRobbed += 1;
                Log.Verbose($"Location {location} registers are robbable new TimesRobbed count: {robberyModel.TimesRobbed}");
            }
            else
            {
                source.TriggerEvent("Robbery.StartRegisterRobbery", false);
                Log.ToClient("[Robbery]", "This register is unable to be robbed at this time", ConstantColours.Blue, source);
            }
            
            if(robberyModel.TimesRobbed >= 2)
            {
                //startRobberyCooldown(robberyModel);
                robberyModel.InRegisterCooldown = true;
                Task.Factory.StartNew(async () =>
                {
                    await BaseScript.Delay(robberyModel.CooldownTime);
                    robberyModel.TimesRobbed = 0;
                    robberyModel.InRegisterCooldown = false;
                });
            }
        }

        private void OnRegisterPayment([FromSource] Player source, string location)
        {
            var playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            var robberyModel = GetRobberyModel(location);
            var payout = robberyModel.Payout / 15;
            Log.ToClient("[Robbery]", $"You got ${payout} from the register", ConstantColours.Blue, source);
            Server.Get<PaymentHandler>().UpdatePlayerCash(playerSession, payout, $"store register robbery ({robberyModel.LocationName})");
        }

        private void StartGlobalCooldown(RobberyModel model, bool stores = true)
        {
            foreach (var location in robbableLocations)
            {
                if(location == model) continue;
                
                if (stores && !location.IsBank)
                {
                    if (location.IsRobbable)
                    {
                        startRobberyCooldown(location, location.CooldownTime / 4);
                    }
                }
                else if (!stores && location.IsBank)
                {
                    if (location.IsRobbable)
                    {
                        startRobberyCooldown(location, location.CooldownTime / 2);
                    }
                }
            }
        }
    }

    public class RobberyModel
    {
        public string LocationName;
        public bool IsRobbable = true;
        public bool IsBeingRobbed = false;
        public bool TellerBeenRobbed = false;
        public bool IsVault => !LocationName.Contains("Main") || LocationName.Contains("Vault");
        public bool IsBank = false;
        public int TimeToRob => CitizenFX.Core.Native.API.GetConvarInt($"mg_{LocationName}RobberyTime", 240) * 1000 / (!IsVault ? 4 : 1);

        public int RequiredPolice => CitizenFX.Core.Native.API.GetConvarInt($"mg_{LocationName}RequiredPolice", IsBank ? 3 : 2);
        private int MaxPayout => CitizenFX.Core.Native.API.GetConvarInt($"mg_{LocationName}RobberyPayout", IsBank ? (!IsVault ? 60000 : 150000) : 10000);
        private Random rand => new Random((int)DateTime.Now.Ticks);
        public int Payout => rand.Next(MaxPayout / 4, MaxPayout);
        public int CooldownTime => CitizenFX.Core.Native.API.GetConvarInt($"mg_{LocationName}CooldownTime", IsBank ? 900 : 300) * 1000 / (IsBank ? (!IsVault ? 2 : 1) : 1);

        // Store stuff only
        public int TimesRobbed = 0;
        public bool InRegisterCooldown = false;
    }
}
