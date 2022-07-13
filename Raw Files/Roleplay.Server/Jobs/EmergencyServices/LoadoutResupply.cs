using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Server.Jobs.EmergencyServices
{
    public class LoadoutResupply : JobClass
    {
        private List<Vector3> resupplyLocations = new List<Vector3>
        {
            new Vector3(458.507f, -992.993f, 29.78f),
            new Vector3(1857.135f, 3689.292f, 33.4f),
            new Vector3(-449.60f, 6016.65f, 30.72f),
            new Vector3(-571.25f, -110.86f, 33.88f),
        };

        public LoadoutResupply()
        {
            Server.RegisterEventHandler("Player.OnInteraction", new Action<Player>(OnInteraction));
        }

        public void OnDutyChangeState(Session.Session playerSession)
        {
            // draw marker if on duty remove if not
            if (JobHandler.OnDutyAs(playerSession, JobType.EMS | JobType.Police))
            {
                resupplyLocations.ForEach(o =>
                {
                    playerSession.TriggerEvent("Markers.AddMarker", o.ToArray());
                });
            }
        }

        private void OnInteraction([FromSource] Player source)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            if (JobHandler.OnDutyAs(playerSession, JobType.EMS | JobType.Police))
            {
                var playerPos = playerSession.GetPlayerPosition();
                resupplyLocations.ForEach(o =>
                {
                    if (playerPos.DistanceToSquared(o) < 6.0f)
                    {
                        Server.Get<SharedEmergencyItems>().RefreshLoadout(playerSession);
                        Log.ToClient("[Job]", "Restocked loadout", ConstantColours.Job, playerSession.Source);
                    }
                });
            }
        }
    }
}
