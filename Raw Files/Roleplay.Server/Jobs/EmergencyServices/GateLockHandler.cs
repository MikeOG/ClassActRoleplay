using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Admin;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Server.Permissions;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Jobs.EmergencyServices
{
    class GateLockHandler : JobClass
    {
        #region GateObjects
        private readonly List<LockedGateModel> Gates = new List<LockedGateModel>
        {
            new LockedGateModel { Location = new Vector3(488.895f, -1017.210f, 27.147f), LockState = true, Model = "hei_prop_station_gate", InitialHeading = 90.0f, RequiredJobType = JobType.Police},
            new LockedGateModel { Location = new Vector3(1844.998f, 2604.813f, 44.63978f), LockState = true, Model = "prop_gate_prison_01", InitialHeading = 90.0f, RequiredJobType = JobType.Police},
            new LockedGateModel { Location = new Vector3(1818.543f, 2604.811f, 44.607f), LockState = true, Model = "prop_gate_prison_01", InitialHeading = 90.0f, RequiredJobType = JobType.Police},
            new LockedGateModel { Location = new Vector3(1799.608f, 2616.975f, 44.60325f), LockState = true, Model = "prop_gate_prison_01", InitialHeading = 180.0f, RequiredJobType = JobType.Police},


            //Realtor
            new LockedGateModel { Location = new Vector3(-815.28850f, 185.97240f, 73.03091f), LockState = true, Model = "prop_ld_garaged_01", InitialHeading = 290.0f, RequiredJobType = JobType.Realtor},
        };
        #endregion

        public GateLockHandler()
        {
            for (var i = 0; i < Gates.Count; i++)
            {
                Gates[i].GateId = i + 1;
            }
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var gatesToSend = Gates.ToList();
            for (int i = 0; i < gatesToSend.Count; i++)
            {
                gatesToSend[i].CanOpenGate = CanOpenGate(playerSession, gatesToSend[i]);
            }

            playerSession.TriggerEvent("Locks.ReceiveGateLockStates", JsonConvert.SerializeObject(gatesToSend));
        }

        public void OnPermissionRefresh(Session.Session playerSession) => OnCharacterLoaded(playerSession);

        public bool CanOpenGate(Session.Session playerSession, LockedGateModel gate)
        {
            if (string.IsNullOrEmpty(gate.RequiredJobPermissions))
            {
                return gate.RequiredJobType.HasFlag(JobHandler.GetPlayerJob(playerSession));
            }
            else
            {
                return Permissions.Permissions.HasPermissions(playerSession, gate.RequiredJobPermissions);
            }
        }

        [EventHandler("Locks.RequestGateLock")]
        private void OnReqestLockGate([FromSource] Player source, int gateId, bool state)
        {
            Log.Info($"{source.Name} is attempting to set the state of gate #{gateId} to {state}");
            var playerSession = Sessions.GetPlayer(source);
            var gate = Gates.FirstOrDefault(o => o.GateId == gateId);

            if (playerSession == null || gate == null || gate.Location.DistanceToSquared(playerSession.Position) > 260.0f) return;

            if (CanOpenGate(playerSession, gate))
            {
                gate.LockState = state;

                Log.ToClient("[Gate]", $"Gate  {(gate.LockState ? "locked" : "unlocked")}", ConstantColours.Job, source);

                BaseScript.TriggerClientEvent("Locks.UpdateGateState", gateId, state);
            }
        }

        [EventHandler("Locks.RequestGateLockStates")]
        private void OnRequestGateLocks([FromSource] Player source)
        {
            Log.Info($"{source.Name} is requesting locked Gate states");
            source.TriggerEvent("Locks.ReceiveGateLockStates", JsonConvert.SerializeObject(Gates));
        }
    }
}