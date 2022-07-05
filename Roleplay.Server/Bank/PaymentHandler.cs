using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Server.Models;
using Roleplay.Server.Enums;
using Roleplay.Server.Jobs;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Bank
{
    class PaymentHandler : ServerAccessor
    {
        private JobHandler job => Server.Get<JobHandler>();
        private Dictionary<string, int> jobPayments = new Dictionary<string, int>
        {
            ["CityBus"] = Settings.JobPayCityBus,
            ["CoachBus"] = Settings.JobPayCoachBus,
            ["Garbage"] = Settings.JobPayGarbage
        };
        private Dictionary<string, Func<int>> paymentFuncs = new Dictionary<string, Func<int>>
        {
            ["Tow"] = () => new Random((int)DateTime.Now.Ticks).Next(Settings.JobPayTowMinimum, Settings.JobPayTowMaximum)
        };
        private Dictionary<JobType, int> jobPaychecks = new Dictionary<JobType, int>
        {
            [JobType.EMS] = Settings.PaycheckEmergencyServices,
            [JobType.Police] = Settings.PaycheckEmergencyServices,
            [JobType.Tow] = Settings.PaycheckTow,
            [JobType.Civillian] = Settings.PaycheckCivillian,
            [JobType.Mechanic] = Settings.PaycheckMechanic,
            [JobType.Taxi] = Settings.PaycheckTaxi,
            [JobType.Delivery] = Settings.PaycheckDelivery,
        };

        public PaymentHandler(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("setpaymenttype|setpaymentmethod|paywith", OnPaymentMethodSet);
            server.RegisterEventHandler("Job.RequestDeliveryPayment", new Action<Player, List<object>, List<object>, string>(OnDeliveryPayReq));
            server.RegisterEventHandler("Job.RequestPayForJob", new Action<Player, string, string>(OnJobPayReq));
            server.RegisterEventHandler("Job.RequestRemoteJobPayment", new Action<Player, int, int, int, string>(OnRemoteJobPay));
            server.RegisterEventHandler("Payment.PayForItem", new Action<Player, int, string>(OnRemotePay));
            server.RegisterEventHandler("Payment.CanPayForItem", new Action<Player, int>(OnCheckCanPay));
            server.RegisterEventHandler("Payment.GivePlayerCash", new Action<Player, int, int>(OnGiveCash));
            server.RegisterTickHandler(PaycheckTick);
        }

        public PaymentType GetPaymentType(Session.Session playerSession)
        {
            return playerSession.GetGlobalData("Session.PaymentType", PaymentType.Cash);
        }

        /// <summary>
        /// Checks if a player is able to purchase a certain amount of an item
        /// </summary>
        /// <param name="playerSession">Session object of the player</param>
        /// <param name="itemPrice">Base price of the item</param>
        /// <param name="itemAmount">Amount of the item being bought</param>
        /// <param name="paymentTypeOverride">Option param if you want to specifically check a certain payment type</param>
        /// <returns></returns>
        public bool CanPayForItem(Session.Session playerSession, int itemPrice, int itemAmount = 1, int paymentTypeOverride = -1)
        {
            var playerPayMethod = paymentTypeOverride == -1 ?  playerSession.GetGlobalData("Session.PaymentType", PaymentType.Cash) : (PaymentType)paymentTypeOverride;
            var totalPrice = itemPrice * itemAmount;

            if (playerPayMethod == PaymentType.Cash)
            {
                var currentCash = playerSession.GetGlobalData<int>("Character.Cash");
                if (currentCash - totalPrice >= 0)
                    return true;
            }
            else
            {
                var playerBank = Server.Get<BankHandler>().GetBank(playerSession.GetGlobalData<int>("Character.CharID"));
                if ((playerBank?.Balance ?? 0) - totalPrice >= 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Updates a player cash and logs it to the database
        /// </summary>
        /// <param name="playerSession">Session object of the player</param>
        /// <param name="cashAmount">The amount of cash that will be given or taken away (can be positive or negative)</param>
        /// <param name="updateReason">Reason the money is being used or taken away</param>
        /// <param name="skipLog">Should logging be done or not</param>
        public void UpdatePlayerCash(Session.Session playerSession, int cashAmount, string updateReason, bool skipLog = false)
        {
            var logText = "";
            if (cashAmount < 0 && !skipLog) // adding to bank or spending on something 
            {
                Log.Verbose($"{playerSession.PlayerName} spent ${cashAmount * -1} on {updateReason}");
                logText = $"spent ${cashAmount * -1} on {updateReason}";
            }
            else if(cashAmount >= 0 && !skipLog)
            {
                Log.Verbose($"{playerSession.PlayerName} received ${cashAmount} from {updateReason}");
                logText = $"received ${cashAmount} from {updateReason}";
            }
            if (playerSession.GetGlobalData("Character.Cash", 0) < 0) playerSession.SetGlobalData("Character.Cash", 0);

            playerSession.SetGlobalData("Character.Cash", playerSession.GlobalData["Character.Cash"] += cashAmount);
            Server.TriggerLocalEvent("Log.ToDatabase", playerSession.PlayerName, playerSession.SteamIdentifier, "money", logText);
        }

        /// <summary>
        /// Updates and banks balance and logs it to the database
        /// </summary>
        /// <param name="bank">Bank object wanting to be updated</param>
        /// <param name="cashAmount">The amount of money that will be given or taken away (can be positive or negative)</param>
        /// <param name="playerSession">Session object of the player interacting with the bank</param>
        /// <param name="updateReason">Reason the money is being used or taken away</param>
        /// <param name="skipLog">Should logging be done or not</param>
        public void UpdateBankBalance(BankAccountModel bank, int cashAmount, Session.Session playerSession, string updateReason = "bank transaction", bool skipLog = false)
        {
            var logText = "";
            if (cashAmount < 0 && !skipLog) // adding to bank or spending on something 
            {
                Log.Verbose($"{playerSession.PlayerName} withdrew ${cashAmount * -1} for {updateReason} from account {bank.AccountID}");
                logText = $"withdrew ${cashAmount * -1} for {updateReason} from account {bank.AccountID}";
            }
            else if (cashAmount >= 0 && !skipLog)
            {
                Log.Verbose($"{playerSession.PlayerName} deposited ${cashAmount} for {updateReason} into account {bank.AccountID}");
                logText = $"deposited ${cashAmount} for {updateReason} into account {bank.AccountID}";
            }

            bank.Balance += cashAmount;
            Server.TriggerLocalEvent("Log.ToDatabase", playerSession.PlayerName, playerSession.SteamIdentifier, "bank", logText);
        }

        public int GetPaymentTypeBalance(Session.Session playerSession)
        {
            var paymentType = GetPaymentType(playerSession);

            if (paymentType == PaymentType.Cash)
            {
                return playerSession.GetGlobalData("Character.Cash", 0);
            }

            if (paymentType == PaymentType.Debit)
            {
                return Server.Get<BankHandler>().GetBank(playerSession.GetGlobalData<int>("Character.CharID")).Balance;
            }

            return 0;
        }

        public void PayForItem(Session.Session playerSession, int itemCost, string updateReason, int paymentTypeOverride = -1)
        {
            if (itemCost < 0) itemCost *= -1;
            var paymentType = paymentTypeOverride == -1 ? playerSession.GetGlobalData("Session.PaymentType", PaymentType.Cash) : (PaymentType)paymentTypeOverride;
            if (paymentType == PaymentType.Cash)
            {
                UpdatePlayerCash(playerSession, -itemCost, updateReason);
            }
            else
            {
                UpdateBankBalance(Server.Get<BankHandler>().GetBank(playerSession.GetGlobalData<int>("Character.CharID")), -itemCost, playerSession, updateReason);
            }

            MySQL.execute("UPDATE server_statistics SET StatisticValue = StatisticValue + @val WHERE StatisticName = 'money_spent'", new Dictionary<string, dynamic> { {"@val", itemCost} });
        }

        private void OnRemotePay([FromSource] Player source, int itemCost, string updateReason)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            PayForItem(playerSession, itemCost, updateReason);
        }

        private void OnPaymentMethodSet(Command cmd)
        {
            var newPaymentMethod = cmd.GetArgAs(0, "cash").ToLower();
            var playerSession = Sessions.GetPlayer(cmd.Player);
            if (playerSession == null) return;

            if (newPaymentMethod == "bank" || newPaymentMethod == "debit")
            {
                playerSession.SetGlobalData("Session.PaymentType", (int)PaymentType.Debit);
            }
            else
            {
                playerSession.SetGlobalData("Session.PaymentType", (int)PaymentType.Cash);
            }
            Log.ToClient("[Bank]", $"Payment type set to {playerSession.GetGlobalData<PaymentType>("Session.PaymentType")}", ConstantColours.Bank, cmd.Player);
        }

        private void OnDeliveryPayReq([FromSource] Player source, List<object> previousLocation, List<object> currentLocation, string jobType)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var dutyAsDelivery = job.OnDutyAs(playerSession, JobType.Delivery);
            var dutyAsTaxi = job.OnDutyAs(playerSession, JobType.Taxi);
            if (dutyAsDelivery || dutyAsTaxi)
            {
                var previousDeliveryLocation = new Vector3(previousLocation.Select(Convert.ToSingle).ToArray());
                var currentDeliveryLocation = new Vector3(currentLocation.Select(Convert.ToSingle).ToArray());
                var payAmount = Convert.ToInt32(Math.Floor(Math.Sqrt(previousDeliveryLocation.DistanceToSquared(currentDeliveryLocation)) / (dutyAsDelivery ? (jobType == "trucking job" ? 2.0f : 6.0f) : 19.0)));
                UpdatePlayerCash(playerSession, payAmount, jobType);
                Log.ToClient("[Job]", $"You got ${payAmount} for this job", ConstantColours.Job, source);
                MySQL.execute("UPDATE server_statistics SET StatisticValue = StatisticValue + @val WHERE StatisticName = 'money_earned'", new Dictionary<string, dynamic> { { "@val", payAmount } });
            }
        }

        private void OnJobPayReq([FromSource] Player source, string jobType, string jobName)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            if (job.OnDutyAs(playerSession, job.stringToJob(jobType)))
            {
                var jobPayOut = paymentFuncs.ContainsKey(jobName) ? paymentFuncs[jobName]() : jobPayments[jobName];
                UpdatePlayerCash(playerSession, jobPayOut, $"{jobName.AddSpacesToCamelCase()} job");
                Log.ToClient("[Job]", $"You got ${jobPayOut} from this job", ConstantColours.Job, source);
                MySQL.execute("UPDATE server_statistics SET StatisticValue = StatisticValue + @val WHERE StatisticName = 'money_earned'", new Dictionary<string, dynamic> { { "@val", jobPayOut } });
            }
        }

        private void OnRemoteJobPay([FromSource] Player source, int pay, int additionalCost, int taxCost, string payReason)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            if ((additionalCost != pay * 4) && (taxCost != pay * additionalCost) && string.IsNullOrEmpty(payReason)) return;

            UpdatePlayerCash(playerSession, pay, payReason);
        }

        private void OnGiveCash([FromSource] Player source, int targetPlayer, int cashAmount)
        {
            var playerSession = Sessions.GetPlayer(source);
            var targetSession = Sessions.GetPlayer(targetPlayer);

            if (playerSession == null || targetSession == null || playerSession.GetLocalData("Character.IsBeingRobbed", false)) return;

            if (CanPayForItem(playerSession, cashAmount, 1, (int)PaymentType.Cash))
            {
                UpdatePlayerCash(playerSession, -cashAmount, $"giving money to {targetSession.PlayerName}");
                UpdatePlayerCash(targetSession, cashAmount, $"recieving money from {playerSession.PlayerName}");

                Log.ToClient("", $"You just got ${cashAmount} from {playerSession.GetGlobalData<string>("Character.FirstName")} {playerSession.GetGlobalData<string>("Character.LastName")}", default, targetSession.Source);
                Log.ToClient("", $"You just gave ${cashAmount} to {targetSession.GetGlobalData<string>("Character.FirstName")} {targetSession.GetGlobalData<string>("Character.LastName")}", default, source);
            }
            else
            {
                Log.ToClient("[Payment]", "You are not have enough cash to do this", ConstantColours.Green, source);
            }
        }

        private async Task PaycheckTick()
        {
            await BaseScript.Delay(Settings.PaycheckPayInterval);
            var jobHandler = Server.Get<JobHandler>();
            foreach (var session in Sessions.PlayerList)
            {
                if (session.GetGlobalData("Character.CharID", -1) == -1) continue;

                var playerJob = jobHandler.GetPlayerJob(session);
                var onDutyAsJob = jobHandler.OnDutyAs(session, playerJob);
                //var jobPay = jobPaychecks[playerJob];

                if(!jobPaychecks.TryGetValue(playerJob, out var jobPay)) continue;

                var playerBank = Server.Get<BankHandler>().GetBank(session.GetGlobalData<int>("Character.CharID"));

                if(playerBank == null) continue;

                if (playerJob == JobType.Civillian || !onDutyAsJob)
                {
                    jobPay = jobPaychecks[JobType.Civillian];

                    //Log.ToClient("[Bank]", $"You received your ${jobPay} benefit cheque", ConstantColours.Bank, session.Source);
                    UpdateBankBalance(playerBank, jobPay, session, "paycheck");
                    continue;
                }

                if (jobPay == 0) continue;

                if (onDutyAsJob)
                {
                    //Log.ToClient("[Bank]", $"You received your ${jobPay} paycheck", ConstantColours.Bank, session.Source);
                    UpdateBankBalance(playerBank, jobPay, session, "paycheck");
                }      
            }
        }

        private void OnCheckCanPay([FromSource] Player source, int itemCost)
        {
            source.TriggerEvent("Payment.SendPaymentStatus", CanPayForItem(Server.Instances.Session.GetPlayer(source), itemCost));
        }
    }
}
