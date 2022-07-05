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
    public class GateLocks : JobClass
    {
        private List<LockedGateModel> gates = new List<LockedGateModel>();

        public GateLocks()
        {
            Client.RegisterEventHandler("Locks.ReceiveGateLockStates", new Action<string>(locks => gates = JsonConvert.DeserializeObject<List<LockedGateModel>>(locks)));
            Client.RegisterEventHandler("Locks.UpdateGateState", new Action<int, bool>(UpdateGateState));
            Client.RegisterTickHandler(GateLockTick);
        }

        private void UpdateGateState(int gateId, bool state)
        {
            Log.Info($"Setting state of gate #{gateId} to {state}");

            var gate = gates.FirstOrDefault(o => o.GateId == gateId);

            if (gate == null) return;

            gate.LockState = state;
        }

        private async Task GateLockTick()
        {
            if (Client.LocalSession == null) return;

            var playerPos = Cache.PlayerPed.Position;

            foreach (var gate in gates)
            {
                var distance = gate.Location.DistanceToSquared(playerPos);

                if (distance <= 260.0f)
                {
                    if (!uint.TryParse(gate.Model, out var gateHash))
                    {
                        gateHash = (uint)Game.GenerateHash(gate.Model);
                    }

                    if (!DoesObjectOfTypeExistAtCoords(gate.Location.X, gate.Location.Y, gate.Location.Z, 1.0f, gateHash, true)) return;

                    var obj = Entity.FromHandle(GetClosestObjectOfType(gate.Location.X, gate.Location.Y, gate.Location.Z, 1.0f, gateHash, false, false, false));

                    obj.IsPositionFrozen = gate.LockState;

                    if (gate.LockState)
                    {
                        obj.Heading = gate.InitialHeading;
                    }

                    if (distance <= 260.0f)
                    {
                        if (!gate.CanOpenGate) return;

                        CitizenFX.Core.UI.Screen.DisplayHelpTextThisFrame($"Press ~INPUT_CONTEXT~ to {(gate.LockState ? "unlock" : "lock")} the gate");

                        if (Input.IsControlJustPressed(Control.Pickup))
                        {
                            Log.Debug($"About to set debug state of gate #{gate.GateId} to {!gate.LockState}");
                            Client.TriggerServerEvent("Locks.RequestGateLock", gate.GateId, !gate.LockState);
                            return;
                        }
                    }
                }
            }
        }
    }
}
