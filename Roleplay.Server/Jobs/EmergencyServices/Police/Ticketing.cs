using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class Ticketing : JobClass
    {
        public Ticketing()
        {
            CommandRegister.RegisterJobCommand("ticket", OnTicketCommand, JobType.Police);
            CommandRegister.RegisterCommand("payticket", OnPayTicket);
            CommandRegister.RegisterCommand("refuseticket", OnPayTicket);
        }

        private void OnTicketCommand(Command cmd)
        {
            var targetUser = Sessions.GetPlayer(cmd.GetArgAs(0, 0));
            var ticketAmount = cmd.GetArgAs(1, 0);
            cmd.Args.RemoveAt(0); cmd.Args.RemoveAt(0);
            var ticketReason = string.Join(" ", cmd.Args);

            if (targetUser == null || ticketAmount < 0) return;

            targetUser.SetServerData("Character.Ticket.Amount", ticketAmount);
            targetUser.SetServerData("Character.Ticket.StartTime", DateTime.Now);
            targetUser.SetServerData("Character.Ticket.IssuingOfficer", cmd.Session.GetCharacterName());
            targetUser.SetServerData("Character.Ticket.Reason", ticketReason);
            startTicketTimer(targetUser);
            Log.ToClient("[Info]", $"You have been ticketed ${ticketAmount} for {ticketReason} To pay the ticket do /payticket or to refuse the ticket do /refuseticket", ConstantColours.Info, targetUser.Source);
            JobHandler.SendJobAlert(JobType.Police, "[Info]", $"{targetUser.GetCharacterName()} has been ticketed ${ticketAmount} for {ticketReason}", ConstantColours.Info);
        }

        private void OnPayTicket(Command cmd)
        {
            var ticketAmount = cmd.Session.GetServerData("Character.Ticket.Amount", 0);
            cmd.Session.SetServerData("Character.Ticket.HasPaid", true);
            cmd.Session.SetServerData("Character.Ticket.Complete", true);

            if (Server.Get<PaymentHandler>().CanPayForItem(cmd.Session, ticketAmount, 1, (int)PaymentType.Debit))
            {
                Log.ToClient("[Info]", $"You paid the ticket", ConstantColours.Info, cmd.Player);
                JobHandler.SendJobAlert(JobType.Police, "[Info]", $"{cmd.Session.GetCharacterName()} paid the ticket", ConstantColours.Info);
                Server.Get<PaymentHandler>().PayForItem(cmd.Session, ticketAmount, "paying ticket", (int)PaymentType.Debit);

                MySQL.execute("INSERT INTO fivem_server_data.player_tickets (`reason`, `amount`, `issuing_officer`, `game_character_id`) VALUES (@reason, @amount, @officer, @charid)", new Dictionary<string, dynamic>
                {
                    {"@reason", cmd.Session.GetServerData("Character.Ticket.Reason", "") },
                    {"@amount", ticketAmount },
                    {"@officer",  cmd.Session.GetServerData("Character.Ticket.IssuingOfficer", "")},
                    {"@charid",  cmd.Session./*GetCharId()*/CharId}
                }, new Action<int>(rows =>
                {
                    Log.Verbose($"Inserted a new ticket entry for character {cmd.Session.GetCharacterName()}");
                }));
            }
            else
            {
                Log.ToClient("[Info]", $"You cannot pay for this ticket", ConstantColours.Info, cmd.Player);
                JobHandler.SendJobAlert(JobType.Police, "[Info]", $"{cmd.Session.GetCharacterName()} cannot afford to pay the ticket", ConstantColours.Info);
            }
        }

        private void OnRefuseTicket(Command cmd)
        {
            cmd.Session.SetServerData("Character.Ticket.HasPaid", true);
            cmd.Session.SetServerData("Character.Ticket.Complete", true);

            Log.ToClient("[Info]", $"You refused to pay the ticket", ConstantColours.Info, cmd.Session.Source);
            JobHandler.SendJobAlert(JobType.Police, "[Info]", $"{cmd.Session.GetCharacterName()} refused to pay the ticket", ConstantColours.Info);
        }

        private void startTicketTimer(Session.Session targetSession)
        {
            Task.Factory.StartNew(async () =>
            {
                var ticketStartTime = targetSession.GetServerData<DateTime>("Character.Ticket.StartTime");
                var hasPaidTicket = targetSession.GetServerData("Character.Ticket.HasPaid", false);

                while ((DateTime.Now - ticketStartTime).TotalSeconds < 60 && !hasPaidTicket)
                {
                    await BaseScript.Delay(500);
                    hasPaidTicket = targetSession.GetServerData("Character.Ticket.HasPaid", false);
                }

                if (!hasPaidTicket && !targetSession.GetServerData("Character.Ticket.Complete", false))
                {
                    Log.ToClient("[Info]", $"You refused to pay the ticket", ConstantColours.Info, targetSession.Source);
                    JobHandler.SendJobAlert(JobType.Police, "[Info]", $"{targetSession.GetCharacterName()} refused to pay the ticket", ConstantColours.Info);
                }

                targetSession.SetServerData("Character.Ticket.HasPaid", false);
                targetSession.SetServerData("Character.Ticket.Complete", false);
            });
        }
    }
}
