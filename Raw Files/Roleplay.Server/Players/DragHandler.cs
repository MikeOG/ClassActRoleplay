using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.EMS;
using Roleplay.Server.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Enums;

namespace Roleplay.Server.Session
{
    public partial class Session
    {
        private DragState dragState;
        public DragState DragState => dragState;

        public void UpdateDragState(DragState newState, int ownerId = -1)
        {
            dragState = newState;

            SetGlobalData(new Dictionary<string, dynamic>
            {
                ["Character.DragState"] = dragState.ToString(),
                ["Drag.CurrentDragOwner"] = ownerId
            });

            if (dragState == DragState.None || ownerId == -1)
            {
                TriggerEvent("Drag.EndDrag");
            }
            else if (dragState == DragState.Dragged)
            {
                TriggerEvent("Drag.DoDrag", ownerId);
            }
        }
    }
}

namespace Roleplay.Server.Players
{
    public class DragHandler : ServerAccessor
    {
        public DragHandler(Server server) : base(server)
        {
            //server.RegisterEventHandler("Drag.StartDrag", new Action<Player, int>(HandleDragRequest));
            server.RegisterEventHandler("Drag.RequestUnDrag", new Action<Player>(OnUnDragRequest));

            CommandRegister.RegisterCommand("drag", OnDragCommand);
        }

        public DragState GetDragState(Session.Session targetSession)
        {
            var stateString = targetSession.GetGlobalData("Character.DragState", "None");
            Enum.TryParse(stateString, out DragState dragState);

            return dragState;
        }

        private void OnDragCommand(Command cmd)
        {
            var endDrag = cmd.GetArgAs(0, "") == "end";
            var currentDraggedPlayer = cmd.Session.GetGlobalData("Drag.PlayerCurrentlyDragging", -1);

            if (endDrag || currentDraggedPlayer != -1)
            { 
                HandleDragRequest(cmd.Session, currentDraggedPlayer);
            }
            else
            {
                var closestPlayer = cmd.Session.GetClosestPlayer(6, o => (o.CuffState != CuffState.None || o.DeathState == DeathState.Dead) && o.DragState == DragState.None);
                if (closestPlayer != null)
                {
                    Log.Debug($"Found a drag target of {closestPlayer.PlayerName} for {cmd.Session.PlayerName}");
                    HandleDragRequest(cmd.Session, closestPlayer.ServerID);
                }
            }
        }

        private void HandleDragRequest(Session.Session playerSession, int targetPlayer)
        {
            if (playerSession == null) return;
            Log.Debug("playerSession is not null");

            var targetSession = Sessions.GetPlayer(targetPlayer);

            if (targetSession == null) return;
            Log.Debug("targetSession is not null");

            var targetDragState = GetDragState(targetSession);
            Log.Debug($"Targets current drag state is {targetDragState}");

            Log.Debug($"Targets current stats are\nCuff: {targetSession.CuffState}\nDeath: {targetSession.DeathState}");
            if (targetSession.CuffState == CuffState.None && targetSession.DeathState == DeathState.Alive) return;

            if (targetDragState == DragState.None)
            {
                //UpdateDragState(targetSession, DragState.Dragged, playerSession.ServerID);
                targetSession.UpdateDragState(DragState.Dragged, playerSession.ServerID);

                playerSession.SetGlobalData("Drag.PlayerCurrentlyDragging", targetSession.ServerID);
            }
            else if (targetDragState == DragState.Dragged && playerSession.GetGlobalData("Drag.PlayerCurrentlyDragging", -1) == targetPlayer)
            {
                //UpdateDragState(targetSession, DragState.None, -1);
                targetSession.UpdateDragState(DragState.None);

                playerSession.GetGlobalData("Drag.PlayerCurrentlyDragging", -1);
            }
        }

        private void UpdateDragState(Session.Session playerSession, DragState dragState, int ownerId)
        {
            playerSession.SetGlobalData(new Dictionary<string, dynamic>
            {
                ["Character.DragState"] = dragState.ToString(),
                ["Drag.CurrentDragOwner"] = ownerId
            });

            if (dragState == DragState.None || ownerId == -1)
            {
                playerSession.TriggerEvent("Drag.EndDrag");
            }
            else if (dragState == DragState.Dragged)
            {
                playerSession.TriggerEvent("Drag.DoDrag", ownerId);
            }
        }

        private void OnUnDragRequest([FromSource] Player source)
        {
            Session.Session playerSession = Sessions.GetPlayer(source);
            if (playerSession == null || GetDragState(playerSession) == DragState.None) return;

            var currentDragOwner = playerSession.GetGlobalData("Drag.CurrentDragOwner", -1);

            if (currentDragOwner != -1)
            {
                Session.Session ownerSession = Sessions.GetPlayer(currentDragOwner);

                if (ownerSession == null)
                {
                    UpdateDragState(playerSession, DragState.None, -1);

                    source.TriggerEvent("Drag.EndDrag");
                }
            }
        }
    }
}
