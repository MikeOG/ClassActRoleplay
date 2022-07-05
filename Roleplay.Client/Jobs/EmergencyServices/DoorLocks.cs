using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Jobs.EmergencyServices
{
    public class DoorLocks : JobClass
    {
        private List<LockedDoorModel> doors = new List<LockedDoorModel>(); private Animation lockAnim = new Animation("anim@mp_player_intmenu@key_fob@", "", "fob_click", "", "lol", new AnimationOptions { LoopEnableMovement = true, LoopDuration = 500 });
       

        public DoorLocks()
        {
            Client.RegisterEventHandler("Locks.ReceiveLockStates", new Action<string>(locks => doors = JsonConvert.DeserializeObject<List<LockedDoorModel>>(locks)));
            Client.RegisterEventHandler("Locks.UpdateDoorState", new Action<int, bool>(UpdateDoorState));
            //Client.RegisterEventHandler("Player.OnLoginComplete", new Action(() => Client.TriggerServerEvent("Locks.RequestLockStates")));

            Client.RegisterTickHandler(DoorLockTick);
        }

        private void UpdateDoorState(int doorId, bool state)
        {
            Log.Info($"Setting state of door #{doorId} to {state}");

            var door = doors.FirstOrDefault(o => o.DoorId == doorId);

            if (door == null) return;

            door.LockState = state;
        }

        private async Task DoorLockTick()
        {
            if (Client.LocalSession == null) return;

            var playerPos = Cache.PlayerPed.Position;

            foreach (var door in doors)
            {
                var distance = door.Location.DistanceToSquared(playerPos);

                if (distance <= 160.0f)
                {
                    if (!uint.TryParse(door.Model, out var doorHash))
                    {
                        doorHash = (uint)Game.GenerateHash(door.Model);
                    }

                    if (!DoesObjectOfTypeExistAtCoords(door.Location.X, door.Location.Y, door.Location.Z, 1.0f, doorHash, true)) return;

                    var obj = Entity.FromHandle(GetClosestObjectOfType(door.Location.X, door.Location.Y, door.Location.Z, 1.0f, doorHash, false, false, false));

                    obj.IsPositionFrozen = door.LockState;

                    if (door.LockState)
                    {
                        //obj.Position = door.Location;
                        obj.Heading = door.InitialHeading;
                    }

                    if (distance <= 2.0f)
                    {
                        if (!/*JobHandler.OnDutyAsJob(JobType.EMS | JobType.Police)*/door.CanOpenDoor) return;

                        CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame($"Press ~INPUT_CONTEXT~ to {(door.LockState ? "unlock" : "lock")} the door");

                        if (Input.IsControlJustPressed(Control.Pickup))
                        {
                            await lockAnim.PlayFullAnim();

                            Log.Debug($"About to set debug state of door #{door.DoorId} to {!door.LockState}");
                            Client.TriggerServerEvent("Locks.RequestDoorLock", door.DoorId, !door.LockState);
                            //await Cache.PlayerPed.Task.PlayAnimation("anim@mp_player_intmenu@key_fob@", "fob_click_fp", 1.0f, 1.0f, -1, AnimationFlags.UpperBodyOnly, 1.0f);
                            
                            return;
                            //await BaseScript.Delay(500);
                        }
                    }
                }
            }
        }
    }
}
