using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.EMS;
using Roleplay.Server.Session;
using Roleplay.Server.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        private CuffState cuffState = CuffState.None;
        public CuffState CuffState
        {
            get => cuffState;
            set => setCuffState(value);
        }

        private async void setCuffState(CuffState newState)
        {
            Log.Verbose($"Setting cuff state of {PlayerName} to {newState}");
            cuffState = newState;
            SetGlobalData("Character.CuffState", cuffState.ToString());
            await BaseScript.Delay(0);
            TriggerEvent($"Cuff.DoCuff{cuffState.ToString()}");
        }
    }
}

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class ArrestHandler : JobClass
    {
        //private SessionManager Session;

        public ArrestHandler()
        {
            //Session = Sessions;

            CommandRegister.RegisterCommand("cuff", OnCuffStart);
            CommandRegister.RegisterCommand("uncuff", OnCuffEnd);
            
            CommandRegister.RegisterCommand("putincar|putinveh", OnPutInCar);
            CommandRegister.RegisterCommand("takefromcar|takefromveh", OnTakeFromCar);
            CommandRegister.RegisterCommand("search", OnSearch);
            CommandRegister.RegisterJobCommand("frisk", OnFrisk, JobType.Police);
            CommandRegister.RegisterJobCommand("jail", OnDoJail, JobType.Police);
        }

#region Cuffing
        /*public CuffState GetCuffState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.CuffState", "None");
            Enum.TryParse(stateString, out CuffState cuffState);

            return cuffState;
        }

        public async void SetCuffState(Session.Session playerSession, CuffState cuffState)
        {
            Log.Verbose($"Setting cuff state of {playerSession.PlayerName} to {cuffState}");
            playerSession.SetGlobalData("Character.CuffState", cuffState.ToString());
            await BaseScript.Delay(0);
            playerSession.TriggerEvent($"Cuff.DoCuff{cuffState.ToString()}");
        }*/

        private bool CanCuffAsCiv(Session.Session playerSession)
        {
            var canCuff = JobHandler.OnDutyAs(playerSession, JobType.Police);

            if (!canCuff)
            {
                var playerInventory = new PlayerInventory(playerSession.GetGlobalData("Character.Inventory", ""), playerSession);
                if (playerInventory.HasItem("zipties"))
                    canCuff = true;
            }

            return canCuff;
        }

        private void OnCuffStart(Command cmd)
        {
            Func<Session.Session, bool> findfunc = null;

            if (!JobHandler.OnDutyAs(cmd.Session, JobType.Police))
            {
                findfunc = session => session.GetGlobalData("Character.HasHandsOverHead", false);
            }

            var closestPlayer = cmd.Session.GetClosestPlayer(4.0f, findfunc);

            if (closestPlayer == null) return;

            DoPlayerCuff(cmd.Session, closestPlayer, false);
        }

        private void OnCuffEnd(Command cmd)
        {
            var closestPlayer = cmd.Session.GetClosestPlayer(3.0f);

            if (closestPlayer == null || closestPlayer.CuffState == CuffState.None) return;

            DoPlayerCuff(cmd.Session, closestPlayer, true);
        }

        private void UnCuffPlayer(Session.Session playerSession)
        {
            playerSession.SetServerData("Cuff.CuffedBy", -1);
            //SetCuffState(playerSession, CuffState.None);
            playerSession.CuffState = CuffState.None;
        }

        private void DoPlayerCuff(Session.Session sourceSession, Session.Session targetSession, bool endCuff)
        {
            var onDutyAsPolice = JobHandler.OnDutyAs(sourceSession, JobType.Police);
            var canCuffAsCiv = CanCuffAsCiv(sourceSession);

            if (!onDutyAsPolice && !canCuffAsCiv && !endCuff) return;

            var currentCuffState = targetSession.CuffState;//GetCuffState(targetSession);
            var newCuffState = currentCuffState + 1;
            if ((int)newCuffState > Enum.GetValues(typeof(CuffState)).Cast<int>().Max()) // Reset back to soft cuff
            {
                newCuffState = CuffState.SoftCuffed;
            }
            
            if(endCuff && (onDutyAsPolice || targetSession.GetServerData("Cuff.CuffedBy", -1) == sourceSession.ServerID)) // If you were the original cuffer or police
            {
                newCuffState = CuffState.None;
                targetSession.SetServerData("Cuff.CuffedBy", -1);
            }
            else if (!endCuff) // set cuff owner (since its new cuff)
            {
                targetSession.SetServerData("Cuff.CuffedBy", sourceSession.ServerID);
            }

            if (!onDutyAsPolice && !endCuff)
            {
                var playerInv = sourceSession.Inventory;
                playerInv.AddItem("zipties", -1);

                newCuffState = CuffState.SoftCuffed; // zipties can only soft cuff
            }

            targetSession.CuffState = newCuffState;
        }
#endregion

#region Dragging
        
#endregion

#region Jail
        private void OnDoJail(Command cmd)
        {
            var targetPlayer = Server.Instance.Get<SessionManager>().GetPlayer(cmd.GetArgAs(0, 0));
            if(targetPlayer == null) return;

            var timeToJail = cmd.GetArgAs(1, 0);
            cmd.Args.RemoveAt(0);
            cmd.Args.RemoveAt(0);
            var arrestReason = string.Join(" ", cmd.Args);

            UnCuffPlayer(targetPlayer);
            targetPlayer.SetGlobalData("Character.JailTime", timeToJail);
            Server.Get<JailTimeHandler>().UpdateJailState(targetPlayer);

            if (timeToJail >= 9999)
            {
                Log.ToClient("[NEWS]", $"{targetPlayer.GetGlobalData("Character.FirstName", "")} {targetPlayer.GetGlobalData("Character.LastName", "")} has put in jail for {timeToJail} months for {arrestReason}", ConstantColours.Red);
            }
            else
            {
                JobHandler.SendJobAlert(JobType.EMS | JobType.Police, "[INFO]", $"{targetPlayer.GetGlobalData("Character.FirstName", "")} {targetPlayer.GetGlobalData("Character.LastName", "")} has put in jail for {timeToJail} months for {arrestReason}", ConstantColours.Red);
            }

            MySQL.execute("INSERT INTO fivem_server_data.player_arrests (`reason`, `time`, `arresting_officer`, `game_character_id`) VALUES (@reason, @time, @officer, @charid)", new Dictionary<string, dynamic>
            {
                {"@reason", arrestReason },
                {"@time", timeToJail },
                {"@officer",  cmd.Session.GetCharacterName()},
                {"@charid",  targetPlayer./*GetCharId()*/CharId}
            }, new Action<int>(rows =>
            {
                Log.Verbose($"Inserted a new arrest entry for character {targetPlayer.GetCharacterName()}");
            }));
        }
        #endregion

#region Put in car
        public VehState GetVehState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.VehState", "OutVeh");
            Enum.TryParse(stateString, out VehState vehState);

            return vehState;

        }

        private Session.Session FindClosestVehTarget(Session.Session playerSession)
        {
            var closestPlayer = playerSession.GetClosestPlayer(9.0f, session => (session.CuffState != CuffState.None || session.DeathState == DeathState.Dead));

            if (closestPlayer == null || playerSession.IsInVehicle() || (/*GetCuffState(closestPlayer)*/ closestPlayer.CuffState == CuffState.None && closestPlayer.DeathState == DeathState.Alive)) return null;

            Log.Verbose($"{playerSession.PlayerName} found a put in veh target of {closestPlayer.PlayerName}");

            return closestPlayer;
        }

        private async void OnPutInCar(Command cmd)
        {
            Session.Session targetSession;

            var currentDragTarget = cmd.Session.GetGlobalData("Drag.PlayerCurrentlyDragging", -1);
            if (currentDragTarget == -1)
            {
                targetSession = FindClosestVehTarget(cmd.Session);
            }
            else
            {
                targetSession = Server.Instance.Get<SessionManager>().GetPlayer(currentDragTarget);

                cmd.Session.SetGlobalData("Drag.PlayerCurrentlyDragging", -1);
                if (targetSession != null)
                {
                    targetSession.UpdateDragState(DragState.None);
                    await BaseScript.Delay(0);
                }
                else
                {
                    targetSession = FindClosestVehTarget(cmd.Session);
                }
            }

            HandleVehStateUpdate(targetSession, "InVeh");
        }

        private void OnTakeFromCar(Command cmd)
        {
            //cmd.Player.TriggerEvent("PutInVeh.FindTarget", "OutVeh");
            Log.Debug($"Taking from car!");
            var closePlayer = FindClosestVehTarget(cmd.Session);

            if (closePlayer == null)
            {
                Log.Debug($"Close player equals null!");
                return;
            }

            HandleVehStateUpdate(closePlayer, "OutVeh");
        }

        private void HandleVehStateUpdate(Session.Session targetSession, string vehState)
        {
            if (targetSession == null) return;

            Enum.TryParse(vehState, out VehState newVehState);
            var targetVehState = GetVehState(targetSession);

            //if (targetVehState != newVehState)
            {
                UpdateVehStatus(targetSession, newVehState);
            }
        }

        public async void UpdateVehStatus(Session.Session playerSession, VehState newState)
        {
            if (playerSession == null) return;

            Log.Debug($"Setting PutInVeh state for {playerSession.PlayerName} to {newState}");
            playerSession.SetGlobalData("Character.VehState", newState.ToString());
            await BaseScript.Delay(0);
            playerSession.TriggerEvent("PutInVeh.UpdateState", (int)newState);

            if (newState == VehState.InVeh && playerSession.DragState == DragState.Dragged)
            {
                playerSession.UpdateDragState(DragState.None);
            }
        }
#endregion
 
#region Search
        private void OnSearch(Command cmd)
        {
#if ONESYNC
            var closePlayer = cmd.Session.GetClosestPlayer(3.0f);
            if (closePlayer != null)
            {
                OnSearchPlayerSent(cmd.Session, closePlayer.ServerID);
            }
            //OnSearchPlayerSent(cmd.Player, cmd.Source);
#else
            cmd.Player.TriggerEvent("Search.FindSearchTarget");
            
#endif
        }

        private void OnSearchPlayerSent(Session.Session sourceSession, int targetPlayer)
        {
            var onDutyAsPolice = JobHandler.OnDutyAs(sourceSession, JobType.Police);
            var targetSession = Server.Instance.Get<SessionManager>().GetPlayer(targetPlayer);
            var playerInv = targetSession./*GetInventory()*/Inventory;

            var itemString = "";
            var weaponString = "";
            var invItems = playerInv.InventoryItems.OrderBy(o => o.isIllegal).ToList();
            invItems.ForEach(o =>
            {
                if(!o.itemCode.Contains("WEAPON_"))
                {
                    if(o.isIllegal && onDutyAsPolice)
                        itemString += $"^1{o.itemName} ({o.itemAmount}x)^0, ";
                    else
                        itemString += $"{o.itemName} ({o.itemAmount}x), ";
                }
                else
                    weaponString += $"{o.itemName} ({o.itemAmount}x), ";
            });

            if(onDutyAsPolice || targetSession.GetGlobalData("Character.HasHandsUp", false) || targetSession.GetGlobalData("Character.HasHandsOverHead", false))
            {
                Log.ToClient("Identification: ", $"Name - {targetSession.GetCharacterName()}; DOB - {targetSession.GetGlobalData("Character.DOB", "")}", ConstantColours.Do, sourceSession.Source);
                Log.ToClient("Cash: ", $"{targetSession.GetGlobalData("Character.Cash", 0)}", ConstantColours.Do, sourceSession.Source);
                Log.ToClient("Items:", $"{itemString}", ConstantColours.Do, sourceSession.Source);
                Log.ToClient("Weapons:", $"{weaponString}", ConstantColours.Do, sourceSession.Source);

                Log.ToClient("Identification: ", $"Name - {targetSession.GetCharacterName()}; DOB - {targetSession.GetGlobalData("Character.DOB", "")}", ConstantColours.Do, targetSession.Source);
                Log.ToClient("Cash: ", $"{targetSession.GetGlobalData("Character.Cash", 0)}", ConstantColours.Do, targetSession.Source);
                Log.ToClient("Items:", $"{itemString}", ConstantColours.Do, targetSession.Source);
                Log.ToClient("Weapons:", $"{weaponString}", ConstantColours.Do, targetSession.Source);
            }
            //Log.ToClient("", $"Items: {itemString}\nWeapons: {weaponString}", default(Color), targetSession.Source);
        }

        private void OnFrisk(Command cmd)
        {
#if ONESYNC
            var closePlayer = cmd.Session.GetClosestPlayer(3.0f);
            if (closePlayer != null)
            {
                DoFriskOnPlayer(cmd.Session, closePlayer.ServerID);
            }   
#else
            //cmd.Player.TriggerEvent("Search.FindSearchTarget");     
#endif
        }

        private void DoFriskOnPlayer(Session.Session sourceSession, int targetPlayer)
        {
            var onDutyAsPolice = JobHandler.OnDutyAs(sourceSession, JobType.Police);
            var targetSession = Server.Instance.Get<SessionManager>().GetPlayer(targetPlayer);
            var playerInv = targetSession./*GetInventory()*/Inventory;

            var weaponString = "";
            playerInv.InventoryItems.ForEach(o =>
            {
                if (o.itemCode.Contains("WEAPON_"))
                    weaponString += $"{o.itemName} ({o.itemAmount}x), ";
            });

            if (onDutyAsPolice || targetSession.GetGlobalData("Character.HasHandsUp", false) || targetSession.GetGlobalData("Character.HasHandsOverHead", false))
            {
                Log.ToClient("Identification: ", $"Name - {targetSession.GetCharacterName()}; DOB - {targetSession.GetGlobalData("Character.DOB", "")}", ConstantColours.Do, sourceSession.Source);
                Log.ToClient("Weapons:", $"{weaponString}", ConstantColours.Do, sourceSession.Source);

                Log.ToClient("Identification: ", $"Name - {targetSession.GetCharacterName()}; DOB - {targetSession.GetGlobalData("Character.DOB", "")}", ConstantColours.Do, targetSession.Source);
                Log.ToClient("Weapons:", $"{weaponString}", ConstantColours.Do, targetSession.Source);
            }
            //Log.ToClient("Weapons:", $"{weaponString}", Color.FromArgb(200, 140, 220), targetSession.Source);
        }
        #endregion
    }
}
