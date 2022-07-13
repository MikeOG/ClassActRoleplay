using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.HTTP;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Server.Admin
{
    public class PlayerReporting : ServerAccessor
    {
        private const string reportHook = "https://discordapp.com/api/webhooks/673668117259354124/H6pj7ZSh9Pr_Xzbo7bep8-UeGaupmi46y_Puu-6VOmbzZcxma07v1ugog77EEF-pB8J9";
        public PlayerReporting(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("report", OnReportCommand);
        }

        private void OnReportCommand(Command cmd)
        {
            var playerToReport = cmd.GetArgAs(0, 0);
            var reportedSession = Sessions.GetPlayer(playerToReport);

            if (reportedSession == null)
            {
                Log.ToClient("[Report]", "Invalid server ID", ConstantColours.Red, cmd.Player);
                return;
            }

            cmd.Args.RemoveAt(0);
            var reportReason = string.Join(" ", cmd.Args);

            Server.Get<Admin>().SendAdminMessage("[Report]", $"{cmd.Session.PlayerName} reported {reportedSession.PlayerName} for reason {reportReason}", ConstantColours.Red);
            SendWebhookMessage(reportHook, "Report", $"**Report on {reportedSession.PlayerName}**\n```\nReported by: {cmd.Session.PlayerName}\nReporter Server ID: {cmd.Player.Handle}\nReport reason: {reportReason}\nReported Server ID: {reportedSession.ServerID}```");
            Log.ToClient("[Report]", $"You reported {reportedSession.PlayerName} for reason {reportReason}", ConstantColours.Red, cmd.Player);
        }

        private void SendWebhookMessage(string webhook, string username, string content)
        {
            var request = new HTTPRequest();
#pragma warning disable 4014
            request.Request(webhook, "POST", JsonConvert.SerializeObject(new Dictionary<string, string>
#pragma warning restore 4014
            {
                {"username", username },
                {"content", content }
            }), new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json"
            });
        }
    }
}
