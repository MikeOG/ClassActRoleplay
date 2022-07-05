using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.EmergencyServices
{
    class DoorLockHandler : JobClass
    {
        #region DoorObjects
        private readonly List<LockedDoorModel> Doors = new List<LockedDoorModel>
        {
            new LockedDoorModel { Location = new Vector3(464.0f, -992.265f, 24.9149f), LockState = true, Model = "v_ilev_ph_cellgate", InitialHeading = 0.0f, RequiredJobType = JobType.Police | JobType.EMS} ,
            new LockedDoorModel { Location = new Vector3(462.381f, -993.651f, 24.9149f), LockState = true, Model = "v_ilev_ph_cellgate", InitialHeading = -90.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(462.331f, -998.152f, 24.9149f), LockState = true, Model = "v_ilev_ph_cellgate", InitialHeading = 90.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(462.704f, -1001.92f, 24.9149f), LockState = true, Model = "v_ilev_ph_cellgate", InitialHeading = 90.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(449.698f, -986.469f, 30.689f), LockState = true, Model = "v_ilev_ph_gendoor004", InitialHeading = 90.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(447.238f, -980.630f, 30.689f), LockState = true, Model = "v_ilev_ph_gendoor002", InitialHeading = -180.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(443.97f, -989.033f, 30.689f), LockState = true, Model = "v_ilev_ph_gendoor005", InitialHeading = -180.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(445.37f, -988.705f, 30.689f), LockState = true, Model = "v_ilev_ph_gendoor005", InitialHeading = 0.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(464.126f, -1002.78f, 24.9149f), LockState = true, Model = "v_ilev_gtdoor", InitialHeading = 0.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(1855.685f, 3683.93f, 34.59282f), LockState = true, Model = "v_ilev_shrfdoor", InitialHeading = 30.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(453.0938f, -983.2294f, 30.83927f), LockState = true, Model = "v_ilev_gtdoor", InitialHeading = 90.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(469.968f, -1014.452f, 26.53624f), LockState = true, Model = "v_ilev_rc_door2", InitialHeading = 179.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(467.3716f, -1014.452f, 26.53624f), LockState = true, Model = "v_ilev_rc_door2", InitialHeading = 0.0f, RequiredJobType = JobType.Police | JobType.EMS },


            //Pillbox main lobby doors
            new LockedDoorModel { Location = new Vector3(328.1364f, -592.7761f, 43.43391f), LockState = true, Model = "gabz_pillbox_doubledoor_l", InitialHeading = 249.0f, RequiredJobType = JobType.Police | JobType.EMS },
            new LockedDoorModel { Location = new Vector3(327.256f, -595.195f, 43.4391f), LockState = true, Model = "gabz_pillbox_doubledoor_r", InitialHeading = 249.0f, RequiredJobType = JobType.Police | JobType.EMS },

            //prison
            new LockedDoorModel { Location = new Vector3(1831.51600f, 2594.45400f, 44.95212f), LockState = true, Model = "bobo_prison_cellgate", InitialHeading = 90.0f, RequiredJobType = JobType.Police},

            //Realtor locks
            new LockedDoorModel { Location = new Vector3(-816.10680f, 177.51090f, 72.82738f), LockState = true, Model = "v_ilev_mm_doorm_r", InitialHeading = 291.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-816.716f, 179.09800f, 72.82738f), LockState = true, Model = "v_ilev_mm_doorm_l", InitialHeading = 291.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-794.50510f, 178.01240f, 73.04045f), LockState = true, Model = "prop_bh1_48_backdoor_r", InitialHeading = 21.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-796.56570f, 177.22140f, 73.04045f), LockState = true, Model = "prop_bh1_48_backdoor_l", InitialHeading = 21.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-793.39430f, 180.50750f, 73.04045f), LockState = true, Model = "prop_bh1_48_backdoor_l", InitialHeading = 111.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-794.18530f, 182.56800f, 73.04045f), LockState = true, Model = "prop_bh1_48_backdoor_r", InitialHeading = 111.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-14.86892f, -1441.18200f, 31.19323f), LockState = true, Model = "v_ilev_fa_frontdoor", InitialHeading = 180.0f, RequiredJobType = JobType.Horsemen},
            new LockedDoorModel { Location = new Vector3(-1149.709f, -1521.088f, 10.78267f), LockState = true, Model = "v_ilev_trev_doorfront", InitialHeading = 35.0f, RequiredJobType = JobType.Horsemen},

        };
        #endregion

        public DoorLockHandler()
        {
            for (var i = 0; i < Doors.Count; i++)
            {
                Doors[i].DoorId = i + 1;
            }
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var doorsToSend = Doors.ToList();

            //doorsToSend.ForEach(o => o.CanOpenDoor = CanOpenDoor(playerSession, o));
            for (int i = 0; i < doorsToSend.Count; i++)
            {
                doorsToSend[i].CanOpenDoor = CanOpenDoor(playerSession, doorsToSend[i]);
            }

            playerSession.TriggerEvent("Locks.ReceiveLockStates", JsonConvert.SerializeObject(doorsToSend));
        }

        public void OnPermissionRefresh(Session.Session playerSession) => OnCharacterLoaded(playerSession);

        public bool CanOpenDoor(Session.Session playerSession, LockedDoorModel door)
        {
            if (string.IsNullOrEmpty(door.RequiredJobPermissions))
            {
                return door.RequiredJobType.HasFlag(JobHandler.GetPlayerJob(playerSession));
                //return JobHandler.GetPlayerJob(playerSession).HasFlag(door.RequiredJobType);
            }
            else
            {
                return Permissions.Permissions.HasPermissions(playerSession, door.RequiredJobPermissions);
            }
        }

        [EventHandler("Locks.RequestDoorLock")]
        private void OnReqestLockDoor([FromSource] Player source, int doorId, bool state)
        {
            Log.Info($"{source.Name} is attempting to set the state of door #{doorId} to {state}");
            var playerSession = Sessions.GetPlayer(source);
            var door = Doors.FirstOrDefault(o => o.DoorId == doorId);

            if (playerSession == null || door == null || door.Location.DistanceToSquared(playerSession.Position) > 24.0f) return;

            if (CanOpenDoor(playerSession, door))
            {
                door.LockState = state;

                Log.ToClient("[Door]", $"Door  {(door.LockState ? "locked" : "unlocked")}", ConstantColours.Job, source);

                BaseScript.TriggerClientEvent("Locks.UpdateDoorState", doorId, state);
            }
        }

        [EventHandler("Locks.RequestLockStates")]
        private void OnRequestLocks([FromSource] Player source)
        {
            Log.Info($"{source.Name} is requesting locked door states");
            source.TriggerEvent("Locks.ReceiveLockStates", JsonConvert.SerializeObject(Doors));
        }
    }
}