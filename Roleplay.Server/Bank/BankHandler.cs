using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Models;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Bank
{
    public class BankHandler : ServerAccessor
    {
        private List<BankAccountModel> currentAccounts = new List<BankAccountModel>();
        private List<string> currentAccountIds = new List<string>();

        public IReadOnlyList<BankAccountModel> BankAccounts => new List<BankAccountModel>(currentAccounts);

        public BankHandler(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("cash|balance", HandleBalanceRequest);
            server.RegisterEventHandler("Bank.AttemptWithdraw", new Action<Player, int>(OnWithdrawRequest));
            server.RegisterEventHandler("Bank.AttemptDeposit", new Action<Player, int>(OnDepositRequest));
            server.RegisterTickHandler(SaveTick);
            loadAccountIDs();
        }

        /// <summary>
        /// Gets a specific bank based on the owner ID
        /// </summary>
        /// <param name="ownerId">The owner ID for the bank you want to get</param>
        /// <returns></returns>
        public BankAccountModel GetBank(int ownerId) => currentAccounts.FirstOrDefault(o => o.OwnerID == ownerId);

        /// <summary>
        /// Loads all bank accounts associated with a specifc character id
        /// </summary>
        /// <param name="charId">ID of the character you want to load</param>
        public void LoadCharacterBankAccounts(int charId)
        {
            MySQL.execute("SELECT * FROM bank_accounts WHERE OwnerID = @id", new Dictionary<string, dynamic>
            {
                ["@id"] = charId
            }, new Action<List<object>>(data =>
            {
                if(data.ElementAtOrDefault(0) == null)
                    CreateBankAccount(charId);
                else
                {
                    dynamic accountData = data[0];
                    currentAccounts.Add(new BankAccountModel
                    {
                        AccountID = accountData.AccountID,
                        AccountName = accountData.AccountName,
                        Balance = Convert.ToInt32(accountData.Balance),
                        OwnerID = accountData.OwnerID
                    });
                }
            }));
        }

        /// <summary>
        /// Creates a bank account under a specific ownerID
        /// </summary>
        /// <param name="ownerId">ID of the character you want to bank to link to</param>
        public async void CreateBankAccount(int ownerId)
        {
            var bankId = await getUniqueBankId();

            MySQL.execute("INSERT INTO bank_accounts (`AccountID`, `Balance`, `OwnerID`, `AccountEditors`) VALUES (@id, @bal, @owner, @editors)", new Dictionary<string, dynamic>
            {
                ["@id"] = bankId,
                ["@bal"] = Settings.BankStartingBalance,
                ["@owner"] = ownerId,
                ["@editors"] = $"{ownerId},"
            }, new Action<dynamic>(rows => LoadCharacterBankAccounts(ownerId)));
        }

        public void OnPlayerDisconnect(Session.Session playerSession, string reason)
        {
            var playerBank = GetBank(playerSession.GetGlobalData<int>("Character.CharID"));
            if (playerBank == null) return;

            saveBankAccount(playerBank);
            currentAccounts.Remove(playerBank);
            Log.Verbose($"Removed {playerSession.PlayerName}'s bank account ({playerBank.AccountID}) due to them disconnecting");
        }

        private void HandleBalanceRequest(Command cmd)
        {
            var playerSession = Sessions.GetPlayer(cmd.Player);
            if (playerSession == null) return;

            var playerBank = GetBank(playerSession.GetGlobalData<int>("Character.CharID"));
            var cashBalance = playerSession.GetGlobalData<int>("Character.Cash");
            var bankBalance = playerBank?.Balance ?? 0;

            Log.ToClient("[Bank]", $"Cash: ${cashBalance} | Bank: ${bankBalance} | Total: ${cashBalance + bankBalance}" , ConstantColours.Bank, cmd.Player);
        }

        private string RandomString(int maxSize)
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private async Task<string> getUniqueBankId()
        {
            var bankString = RandomString(5);
            while (currentAccountIds.Contains(bankString))
            {
                bankString = RandomString(5);
                await BaseScript.Delay(0);
            }
            currentAccountIds.Add(bankString);

            return bankString;
        }

        private void OnWithdrawRequest([FromSource] Player source, int withdrawAmount)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var playerBank = GetBank(playerSession.GetGlobalData<int>("Character.CharID"));
            var bankBalance = playerBank?.Balance ?? 0;

            if (bankBalance - withdrawAmount < 0)
            {
                Log.ToClient("[Bank]", "You do not have enough money in this bank account to do this", ConstantColours.Bank, source);
            }
            else
            {
                if (withdrawAmount <= 0 || playerBank == null) return;
                var payHandler = Server.Get<PaymentHandler>();
                payHandler.UpdatePlayerCash(playerSession, withdrawAmount, "bank withdraw", true);
                payHandler.UpdateBankBalance(playerBank, withdrawAmount * -1, playerSession);

                Log.ToClient("[Bank]", $"You just withdrew ${withdrawAmount} from your bank account", ConstantColours.Bank, source);
            }
        }

        private void OnDepositRequest([FromSource] Player source, int withdrawAmount)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var playerBank = GetBank(playerSession.GetGlobalData<int>("Character.CharID"));
            var playerBalance = playerSession.GetGlobalData<int>("Character.Cash");

            if (playerBalance - withdrawAmount < 0)
            {
                Log.ToClient("[Bank]", "You do not have enough cash to do this", ConstantColours.Bank, source);
            }
            else
            {
                if (withdrawAmount <= 0 || playerBank == null) return;
                var payHandler = Server.Get<PaymentHandler>();
                payHandler.UpdatePlayerCash(playerSession, withdrawAmount * -1, "bank deposit", true);
                payHandler.UpdateBankBalance(playerBank, withdrawAmount, playerSession);

                Log.ToClient("[Bank]", $"You just deposited ${withdrawAmount} into your bank account", ConstantColours.Bank, source);
            }
        }

        private async void loadAccountIDs()
        {
            await BaseScript.Delay(10000);

            MySQL.execute("SELECT AccountID FROM bank_accounts", new Dictionary<string, dynamic>(), 
                new Action<List<object>>(data =>
            {
                foreach (dynamic i in data)
                {
                    currentAccountIds.Add(i.AccountID.ToString());
                }
            }));
        }

        private async Task SaveTick()
        {
            await BaseScript.Delay(Settings.SaveInterval);

            foreach (var i in currentAccounts)
            {
                saveBankAccount(i);
            }
        }

        private void saveBankAccount(BankAccountModel account)
        {
            MySQL.execute("UPDATE bank_accounts SET Balance = @bal WHERE AccountID = @id", new Dictionary<string, dynamic>
            {
                ["@bal"] = account.Balance,
                ["@id"] = account.AccountID
            });
        }
    }
}
