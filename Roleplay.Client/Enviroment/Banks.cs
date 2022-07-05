using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enums;
using Roleplay.Client.Helpers;
using Roleplay.Client.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Locations;
using Roleplay.Shared.Models;

namespace Roleplay.Client.Enviroment
{
    public class Banks : ClientAccessor
    {
        private ObjectList objectList = new ObjectList();
        private Dictionary<string, Marker> vaultMarkers = new Dictionary<string, Marker>();

        private List<ObjectHash> atmHashs = new List<ObjectHash>
        {
            ObjectHash.prop_atm_01,
            ObjectHash.prop_atm_02,
            ObjectHash.prop_atm_03,
            ObjectHash.prop_fleeca_atm
        };

        public Banks(Client client) : base(client)
        {
            CommandRegister.RegisterCommand("withdraw", OnWithdrawRequest);
            CommandRegister.RegisterCommand("deposit", OnDepositRequest);
            client.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraciton));
            client.RegisterEventHandler("Robbery.UpdateRobberyPosition", new Action(OnCoordRequest));
            client.RegisterEventHandler("Bank.ShowVaultMarker", new Action<string>(OnShowVaultMarker));
            LoadBlips();
        }

        public KeyValuePair<string, Vector3> GetCloestBankInRange(float distance = 2.0f)
        {
            return BankLocations.Positions.FirstOrDefault(o => o.Value.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(distance, 2));
        }

        private void OnWithdrawRequest(Command cmd)
        {
            if (IsCloseToBank() || IsCloseToATM())
            {
                Roleplay.Client.Client.Instance.TriggerServerEvent("Bank.AttemptWithdraw", cmd.GetArgAs(0, 0));
            }
        }

        private void OnDepositRequest(Command cmd)
        {
            if (IsCloseToBank())
            {
                Roleplay.Client.Client.Instance.TriggerServerEvent("Bank.AttemptDeposit", cmd.GetArgAs(0, 0));
            }
        }

        private bool IsCloseToBank()
        {
            return BankLocations.Positions.Any(o => o.Value.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(2, 2));
        }

        private bool IsCloseToATM()
        {
            return objectList.Select(o => new Prop(o)).Where(o => atmHashs.Contains((ObjectHash)(uint)o.Model.Hash)).Any(o => o.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(3, 2));
        }

        private void OnCoordRequest()
        {
            Client.TriggerServerEvent("Robbery.OnCoordUpdate", Game.PlayerPed.Position.ToArray().ToList());
        }

        private void OnInteraciton()
        {
            if (Client.Instances.Weapons.IsAttackWeapon(Game.PlayerPed.Weapons.Current))
            {
                var cloestBank = GetCloestBankInRange();
                if (cloestBank.Key != null)
                {
                    Client.TriggerServerEvent("Robbery.AttemptStartRob", cloestBank.Key);
                }
            }
            else
            {
                var nearBank = IsCloseToBank();
                var nearATM = IsCloseToATM();

                if (nearBank)
                {
                    Log.ToChat("[Bank]", "To withdraw money from the bank do /withdraw [amount]. To deposit money into the bank do /deposit [amount]", ConstantColours.Bank);
                }
                else if (nearATM)
                {
                    Log.ToChat("[Bank]", "To withdraw money from the ATM do /withdraw [amount]", ConstantColours.Bank);
                }
            }
        }

        private void OnShowVaultMarker(string bank)
        {
            if (vaultMarkers.ContainsKey(bank))
                MarkerHandler.AddMarker(vaultMarkers[bank]);
        }

        private void LoadBlips()
        {
            foreach (var bank in BankLocations.Positions)
            {
                var bankMarker = new Marker(bank.Value, MarkerType.HorizontalCircleFat, bank.Key.Contains("Main") ? Color.FromArgb(150, ConstantColours.Yellow) : Color.FromArgb(150, ConstantColours.Red), 2.0f);
                if (bank.Key.Contains("Main"))
                {
                    MarkerHandler.AddMarker(bankMarker);
                    BlipHandler.AddBlip("Bank", bank.Value, new BlipOptions
                    {
                        Sprite = BlipSprite.DollarSign,
                        Colour = BlipColor.Green
                    });
                }
                else
                    vaultMarkers[bank.Key] = bankMarker;         
            }
        }
    }
}
