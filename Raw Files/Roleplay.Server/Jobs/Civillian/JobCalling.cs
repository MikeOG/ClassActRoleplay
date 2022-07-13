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

namespace Roleplay.Server.Jobs.Civillian
{
    public class JobCalling : JobClass
    {
        public JobCalling()
        {
            CommandRegister.RegisterCommand("taxi", OnTaxiMessage);
            CommandRegister.RegisterJobCommand("taxir", OnTaxiRespond, JobType.Taxi);

            CommandRegister.RegisterJobCommand("towr", OnTowRespond, JobType.Tow);
            CommandRegister.RegisterCommand("calltow", OnCallTow);

            CommandRegister.RegisterCommand("taxis", CheckAvailableTaxis);
            CommandRegister.RegisterCommand("tows", CheckAvailableTows);

            Server.RegisterEventHandler("Job.SendTowVehicle", new Action<Player, string>(OnRecieveTow));
        }

        private void OnTaxiMessage(Command cmd)
        {
            var message = string.Join(" ", cmd.Args);

            Log.ToClient("[To taxi]", message, ConstantColours.Yellow, cmd.Player);
            JobHandler.SendJobAlert(JobType.Taxi, $"[Taxi request #{cmd.Source}]", message, ConstantColours.Yellow);
            JobHandler.GetPlayersOnJob(JobType.Taxi).ForEach(o =>
            {
                o.TriggerEvent("Blip.CreateJobBlip", $"Taxi call #{cmd.Source}", cmd.Source, 66);
            });
        }

        private void OnTaxiRespond(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession == null) return;

            cmd.Args.RemoveAt(0);
            var message = string.Join(" ", cmd.Args);

            Log.ToClient($"[To fare #{targetId}]", message, ConstantColours.Yellow, cmd.Player);
            Log.ToClient($"[From taxi #{targetId}]", message, ConstantColours.Yellow, targetSession.Source);
        }

        private void OnTowRespond(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession == null) return;

            cmd.Args.RemoveAt(0);
            var message = string.Join(" ", cmd.Args);

            Log.ToClient($"[To request #{targetId}]", message, ConstantColours.TalkMarker, cmd.Player);
            Log.ToClient($"[From tow #{targetId}]", message, ConstantColours.TalkMarker, targetSession.Source);
        }

        private void OnCallTow(Command cmd)
        {
            if (JobHandler.GetPlayersOnJob(JobType.Tow).Count > 0)
            {
                cmd.Session.TriggerEvent("Job.GetTowRequest");

                /*JobHandler.GetPlayersOnJob(JobType.Tow).ForEach(o =>
                {
                    o.TriggerEvent("Blip.CreateJobBlip", $"Tow request #{cmd.Source}", cmd.Source, 66);
                });*/
            }
            else
            {
                Log.ToClient("[Tow]", $"There are no tow drivers on duty", ConstantColours.Job, cmd.Player);
            }
        }

        private void CheckAvailableTaxis(Command cmd)
        {
            Log.ToClient("[Taxis]", $"There are currently {JobHandler.GetPlayersOnJob(JobType.Taxi).Count} taxis on duty", ConstantColours.Yellow, cmd.Player);
        }

        private void CheckAvailableTows(Command cmd)
        {
            Log.ToClient("[Tows]", $"There are currently {JobHandler.GetPlayersOnJob(JobType.Tow).Count} tow truck drivers on duty", ConstantColours.Job, cmd.Player);
        }

        private async void OnRecieveTow([FromSource] Player source, string message)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var isPolice = JobHandler.OnDutyAs(playerSession, JobType.Police);
            var playerLocation = await playerSession.GetLocation();

            Log.ToClient($"[To #{source.Handle}]", $"You requested tow for a {message} in {playerLocation}", ConstantColours.TalkMarker, source);
            JobHandler.GetPlayersOnJob(JobType.Tow).ForEach(o =>
            {
                Log.ToClient($"[Tow request #{source.Handle}]", $"{(isPolice ? "Police" : "Somebody")} has requested tow for a {message} in {playerLocation}", ConstantColours.TalkMarker, o.Source);
                o.TriggerEvent("Blip.CreateJobBlip", $"Tow request #{source.Handle}", int.Parse(source.Handle), 66);
            });
        }
    }
}
