using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Bank;
using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class Billing : JobClass
    {
        public Billing()
        {
            CommandRegister.RegisterJobCommand("bill", OnBillCommand, JobType.Police);
            CommandRegister.RegisterJobCommand("paytow", OnPayTow, JobType.Police);
            CommandRegister.RegisterCommand("showbill|showdebt", cmd => Log.ToClient("[Bill]", $"Bill to state: ${cmd.Session.GetGlobalData("Character.Bill", 0)}", ConstantColours.Housing, cmd.Player));
            CommandRegister.RegisterCommand("paystate", OnPayBill);
        }

        private void OnBillCommand(Command cmd)
        {
            var targetUser = Sessions.GetPlayer(cmd.GetArgAs(0, 0));
            var billAmount = cmd.GetArgAs(1, 0);
            cmd.Args.RemoveAt(0); cmd.Args.RemoveAt(0);
            var billReason = string.Join(" ", cmd.Args);

            if (targetUser == null || billAmount < 0) return;

            Log.ToClient("[Bill]", $"You have been billed ${billAmount} for {billReason}", ConstantColours.Housing, targetUser.Source);
            JobHandler.SendJobAlert(JobType.Police, "[Bill]", $"{targetUser.GetCharacterName()} has been billed ${billAmount} for {billReason}", ConstantColours.Housing);

            targetUser.SetGlobalData("Character.Bill", targetUser.GetGlobalData("Character.Bill", 0) + billAmount);

            MySQL.execute("INSERT INTO fivem_server_data.player_tickets (`reason`, `amount`, `issuing_officer`, `game_character_id`) VALUES (@reason, @amount, @officer, @charid)", new Dictionary<string, dynamic>
            {
                {"@reason", billReason },
                {"@amount", billAmount },
                {"@officer",  cmd.Session.GetCharacterName()},
                {"@charid",  targetUser./*GetCharId()*/CharId}
            }, new Action<int>(rows =>
            {
                Log.Verbose($"Inserted a new bill entry for character {targetUser.GetCharacterName()}");
            }));
        }

        private void OnPayBill(Command cmd)
        {
            var payAmount = cmd.GetArgAs(0, 0);
            var playerBill = cmd.Session.GetGlobalData("Character.Bill", 0);

            if (payAmount < 0)
                payAmount = 1;
            else if (payAmount > playerBill)
                payAmount = playerBill;

            if (playerBill == 0)
            {
                Log.ToClient("[Bill]", $"You don't have a bill to pay off", ConstantColours.Info, cmd.Player);
                return;
            }

            if (Server.Get<PaymentHandler>().CanPayForItem(cmd.Session, payAmount, 1, (int)PaymentType.Debit))
            {
                cmd.Session.SetGlobalData("Character.Bill", playerBill - payAmount);
                Log.ToClient("[Bill]", $"You just paid off ${payAmount} of your bill. Your new bill amount is ${playerBill - payAmount}", ConstantColours.Housing, cmd.Player);
                Server.Get<PaymentHandler>().PayForItem(cmd.Session, payAmount, "paying bill", (int)PaymentType.Debit);
            }
            else
            {
                Log.ToClient("[Bill]", $"You cannot afford to pay off ${payAmount} of your bill", ConstantColours.Info, cmd.Player);
            }
        }

        private void OnPayTow(Command cmd)
        {
            var targetId = cmd.GetArgAs(0, 0);
            var targetSession = Sessions.GetPlayer(targetId);
            if (targetSession == null || targetId == cmd.Source) return;

            var lastTowPay = cmd.Session.GetServerData("Character.Job.LastTowPay", TimeSpan.FromTicks(0));

            if ((TimeSpan.FromTicks(DateTime.Now.Ticks) - lastTowPay).TotalMinutes >= 4)
            {
                Server.Get<PaymentHandler>().UpdateBankBalance(targetSession.GetBankAccount(), 750, targetSession, "payed for tow");
                Log.ToClient("[Tow]", $"You just got $750 to tow this vehicle", ConstantColours.Job, targetSession.Source);
                Log.ToClient("[Police]", $"You just paid {targetSession.GetCharacterName()} $750 to tow this vehicle", ConstantColours.Police, cmd.Player);
                cmd.Session.SetServerData("Character.Job.LastTowPay", TimeSpan.FromTicks(DateTime.Now.Ticks));
            }
            else
            {
                Log.ToClient("[Police]", $"You can only pay tow every 4 minutes", ConstantColours.Police, cmd.Player);
            }
        }
    }
}
