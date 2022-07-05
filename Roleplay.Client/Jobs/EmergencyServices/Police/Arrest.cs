using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Helpers;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using VehicleSeat = CitizenFX.Core.VehicleSeat;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
    public class Arrest : JobClass
    {
        public CuffState CurrentCuffState => GetCuffState(Client.LocalSession);
        public DragState CurrentDragState => GetDragState(Client.LocalSession);
        private float maxPickupRadius = 6.0f;
        private readonly string arrestAnimDict = "mp_arresting";
        private readonly string arrestAnimName = "idle";
        private bool tickRegistered = false;
        private Random rand = new Random((int)DateTime.Now.Ticks);

        public Arrest()
        {
            Client.RegisterEventHandler("Cuff.FindCuffTarget", new Action<string>(FindCuffTarget));
            Client.RegisterEventHandler("Cuff.DoCuffNone", new Action(OnResetCuff));
            Client.RegisterEventHandler("Cuff.DoCuffSoftCuffed", new Action(OnDoSoftCuff));
            Client.RegisterEventHandler("Cuff.DoCuffHardCuffed", new Action(OnDoHardCuff));

            Client.RegisterEventHandler("Drag.FindDragTarget", new Action(FindDragTarget));
            Client.RegisterEventHandler("Drag.DoDrag", new Action<int>(OnDoDrag));
            Client.RegisterEventHandler("Drag.EndDrag", new Action(OnEndDrag));
            
            Client.RegisterEventHandler("PutInVeh.FindTarget", new Action<string>(FindVehTarget));
            Client.RegisterEventHandler("PutInVeh.UpdateState", new Action<int>(OnUpdateVehState));

            Client.RegisterEventHandler("Search.FindSearchTarget", new Action(FindSearchTarget));
        }

#region Cuffing
        public CuffState GetCuffState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.CuffState", "None");
            Enum.TryParse(stateString, out CuffState cuffState);

            return cuffState;
        }

        private void FindCuffTarget(string cuffType)
        {
            var onDutyAsPolice = JobHandler.OnDutyAsJob(JobType.Police);

            var cloestPlayer = GTAHelpers.GetClosestPlayer(maxPickupRadius);;
            if (cloestPlayer == null || !onDutyAsPolice && (!IsEntityPlayingAnim(cloestPlayer.Character.Handle, "busted", "idle_a", 3) && cuffType != "EndCuff")) return;

            Log.ToChat("", $"Found cuff target for {cuffType}", Color.FromArgb(255, 255, 255));
            Client.TriggerServerEvent($"Cuff.{cuffType}", cloestPlayer.ServerId);
        }

        // TODO probably move this or something
        public async Task DisableActions()
        {
            var cuffState = CurrentCuffState;
            var dragState = CurrentDragState;

            Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            Game.DisableControlThisFrame(1, Control.Jump);
            Game.DisableControlThisFrame(1, Control.SelectWeapon);
            Game.DisableControlThisFrame(1, Control.MeleeAttackLight);
            Game.DisableControlThisFrame(1, Control.Enter);
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.Aim);
            Game.DisableControlThisFrame(1, Control.Attack);
            Game.DisableControlThisFrame(1, Control.Attack2);
            Game.DisableControlThisFrame(1, Control.MeleeAttack1);
            Game.DisableControlThisFrame(1, Control.MeleeAttack2);

            if (cuffState == CuffState.HardCuffed || dragState == DragState.Dragged)
            {
                Game.DisableControlThisFrame(1, Control.MoveUpOnly);
                Game.DisableControlThisFrame(1, Control.MoveDownOnly);
                Game.DisableControlThisFrame(1, Control.MoveLeftOnly);
                Game.DisableControlThisFrame(1, Control.MoveRightOnly);
            }
        }

        private async Task CheckAnimTick()
        {
            var cuffState = CurrentCuffState;

            if (!IsEntityPlayingAnim(PlayerPedId(), arrestAnimDict, arrestAnimName, 3))
            {
                var animFlag = cuffState == CuffState.SoftCuffed ? (AnimationFlags)50 : (AnimationFlags)9;
                await Game.PlayerPed.Task.PlayAnimation(arrestAnimDict, arrestAnimName, 8.0f, 8.0f, -1, animFlag, 0.0f);
            }

            if (Game.PlayerPed.IsRunning)
            {
                var fallOver = rand.NextBool(40);

                if (fallOver)
                {
                    var ragdollTime = rand.Next(250, 750);
                    Game.PlayerPed.Task.ClearAll();
                    Game.PlayerPed.Ragdoll(ragdollTime);
                    await BaseScript.Delay(ragdollTime + 50);
                    OnDoSoftCuff();
                }
            }

            await BaseScript.Delay(500);
        }

        private void OnResetCuff()
        {
            //if(CurrentCuffState != CuffState.None) return;

            Client.DeregisterTickHandler(DisableActions);
            Client.DeregisterTickHandler(CheckAnimTick);

            tickRegistered = false;
            Game.PlayerPed.Task.ClearAll();
        }

        private void OnDoSoftCuff()
        {
            if (CurrentCuffState != CuffState.SoftCuffed) return;

            Game.PlayerPed.Task.ClearAll();

            Game.PlayerPed.Task.PlayAnimation(arrestAnimDict, arrestAnimName, 8.0f, 8.0f, -1, (AnimationFlags)50, 0.0f);

            if (tickRegistered) return;

            Client.RegisterTickHandler(DisableActions);
            Client.RegisterTickHandler(CheckAnimTick);
            tickRegistered = true;
        }

        private void OnDoHardCuff()
        {
            if (CurrentCuffState != CuffState.HardCuffed) return;

            Game.PlayerPed.Task.ClearAll();

            Game.PlayerPed.Task.PlayAnimation(arrestAnimDict, arrestAnimName, 8.0f, 8.0f, -1, (AnimationFlags)9, 0.0f);
        }
#endregion

#region Dragging
        public DragState GetDragState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.DragState", "None");
            Enum.TryParse(stateString, out DragState dragState);

            return dragState;
        }

        private void FindDragTarget()
        {
            //if (!JobHandler.OnDutyAsJob(JobType.Police) && !JobHandler.OnDutyAsJob(JobType.EMS)) return;

            var cloestPlayer = GTAHelpers.GetClosestPlayer(maxPickupRadius);
            Log.Info(cloestPlayer?.Name);
            if (cloestPlayer == null) return;

            Log.ToChat("", $"Found drag target", Color.FromArgb(255, 255, 255));
            Roleplay.Client.Client.Instance.TriggerServerEvent($"Drag.StartDrag", cloestPlayer.ServerId);
        }

        private async void OnDoDrag(int target)
        {
            try
            {
                //var targetSession = Client.Instance.Instances.Session.GetPlayer(target);

                var targetPlayer = Client.PlayerList.FirstOrDefault(o => o.ServerId == target);

                Log.ToServer($"Our drag owner is {targetPlayer} target player object == null {targetPlayer == null}");

                if (targetPlayer == null) return;

                var ticks = 0;
                while(!Cache.PlayerPed.IsAttachedTo(targetPlayer.Character) && ticks < 10)
                {
                    //Log.ToServer("Trying to attach");
                    if (Cache.PlayerPed.IsAttached()) Cache.PlayerPed.Detach();
                    AttachEntityToEntity(Game.PlayerPed.Handle, targetPlayer.Character.Handle, 1, 0.0f, 1.0f, 0.0f, 0.0f, -90.0f, 0.0f, false, false, false, true, 1, true);
                    ticks++;
                    await BaseScript.Delay(250);
                }

                Client.RegisterTickHandler(DisableActions);
                Client.RegisterTickHandler(CheckPlayerExists);
                Client.RegisterTickHandler(CheckAttachTick);

                Cache.PlayerPed.IsCollisionEnabled = false;
            }
            catch (Exception e)
            {
                Log.Error(e, true);
            }
        }

        private void OnEndDrag()
        {
            Cache.PlayerPed.IsCollisionEnabled = true;

            if (Game.PlayerPed.IsAttached()) Game.PlayerPed.Detach();

            Client.DeregisterTickHandler(DisableActions);
            Client.DeregisterTickHandler(CheckPlayerExists);
            Client.DeregisterTickHandler(CheckAttachTick);
        }

        private async Task CheckPlayerExists()
        {
            Session.Session playerSession = Client.Instance.Instances.Session.GetPlayer(Game.Player);
            var dragOwnerId = playerSession.GetGlobalData("Drag.CurrentDragOwner", -1);
            if(dragOwnerId != -1)
            {
                Session.Session dragOwnerSession = Client.Instance.Instances.Session.GetPlayer(dragOwnerId);

                if (dragOwnerSession == null)
                {
                    Client.Instance.TriggerServerEvent("Drag.RequestUnDrag");
                }
            }

            await BaseScript.Delay(1000);
        }

        private async Task CheckAttachTick()
        {
            var dragOwner = Client.LocalSession.GetGlobalData("Drag.CurrentDragOwner", -1);

            if (dragOwner != -1)
            {
                var ownerObj = Client.PlayerList.FirstOrDefault(o => o.ServerId == dragOwner);

                if (ownerObj == null) return;

                if (!Cache.PlayerPed.IsAttachedTo(ownerObj.Character))
                {
                    AttachEntityToEntity(Game.PlayerPed.Handle, ownerObj.Character.Handle, 1, 0.0f, 1.0f, 0.0f, 0.0f, -90.0f, 0.0f, false, false, false, true, 1, true);
                }
            }
        }
#endregion

#region Put in car
        private void FindVehTarget(string vehState)
        {
            var closestPlayer = GTAHelpers.GetClosestPlayer(6.0f);
            Log.ToChat($"You are going to do something with {closestPlayer?.Name}");

            if(closestPlayer == null || Game.PlayerPed.IsInVehicle() || CurrentCuffState != CuffState.None || CurrentDragState != DragState.None) return;

            Client.TriggerServerEvent("PutInVeh.SendVehPlayer", closestPlayer.ServerId, vehState);
        }

        public VehState GetVehState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.VehState", "OutVeh");
            Enum.TryParse(stateString, out VehState vehState);

            return vehState;
        }

        /*private VehicleSeat getEmtptySeat(CitizenFX.Core.Vehicle playerVeh)
        {
            for (var i = GetVehicleMaxNumberOfPassengers(playerVeh.Handle); i > -1; i--)
            {
                if(IsVehicleSeatFree(player))
            }
        }*/

        private async void OnUpdateVehState(int newState)
        {
            var newVehState = (VehState)newState;//GetVehState(Client.LocalSession);
            if (newVehState == VehState.InVeh)
            {
                var playerPed = Cache.PlayerPed;
                var closeVeh = GTAHelpers.GetClosestVehicle();
                while (closeVeh == null)
                {
                    await BaseScript.Delay(0);
                    closeVeh = GTAHelpers.GetClosestVehicle();
                }
                
                if(playerPed.IsAttached()) playerPed.Detach();

                playerPed.Task.ClearAllImmediately();

                foreach (var seat in Enum.GetValues(typeof(VehicleSeat)).Cast<VehicleSeat>().Reverse().ToList())
                //var maxPassengers = GetVehicleMaxNumberOfPassengers(closeVeh.Handle);
                //for (var i = maxPassengers; i > maxPassengers; i--)
                {
                    //var seat = (VehicleSeat)i;
                    //Log.ToServer($"Checking seat {seat}");
                    if (seat != VehicleSeat.Driver && seat != VehicleSeat.Any && closeVeh.IsSeatFree(seat))
                    {
                        //Log.ToServer($"Seat {seat} is free!");
                        var ticks = 0;
                        while(!playerPed.IsInVehicle(closeVeh) && ticks < 10)
                        {
                            //Log.ToServer($"Attempting to enter seat {seat}");
                            playerPed.Task.WarpIntoVehicle(closeVeh, seat);
                            ticks++;
                            await BaseScript.Delay(200);
                        }
                    }
                }
            }
            else
            {
                Game.PlayerPed.Task.LeaveVehicle();
            }
        }
#endregion

 #region Search

        private void FindSearchTarget()
        {
            var onDutyAsPolice = JobHandler.OnDutyAsJob(JobType.Police);

            var cloestPlayer = GTAHelpers.GetClosestPlayer(maxPickupRadius);
            var playerSession = Client.Instances.Session.GetPlayer(cloestPlayer);

            if (cloestPlayer == null || !onDutyAsPolice && (!IsEntityPlayingAnim(cloestPlayer.Character.Handle, "busted", "idle_a", 3)  || GetCuffState(playerSession) < CuffState.SoftCuffed)) return;

            Log.ToChat("", $"Found search target", Color.FromArgb(255, 255, 255));
            Client.TriggerServerEvent($"Search.SearchPlayer", cloestPlayer.ServerId);
        }
#endregion
    }
}
