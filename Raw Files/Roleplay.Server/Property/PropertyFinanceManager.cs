using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Bank;
using Roleplay.Server.Enums;
using Roleplay.Server.Models;
using Roleplay.Server.Property.Models;
using Roleplay.Server.Realtor;
using Roleplay.Server.Realtor.Models;
using Roleplay.Shared;
using Roleplay.Shared.Attributes;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Server.Property
{
    public class PropertyFinanceManager : ServerAccessor
    {
        public PropertyFinanceManager(Server server) : base(server)
        {
            
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var properties = Server.Get<PropertyManager>().Properties.Where(o => o.IsPropertyTenant(playerSession.CharId));

            foreach (var property in properties)
            {
                var daysUntilPay = getDaysUntilNextPayment(property);

                if (daysUntilPay == 0)
                {
                    playerSession.Message("[Property]", $"Payment for {property.Address} is due today (${GetPropertyNextPaymentPrice(property)})");
                }
            }
        }

        public void OnPropertyLoaded(PropertyModel property)
        {
            var daysUntilPay = getDaysUntilNextPayment(property);

            if (daysUntilPay < 0)
            {
                if (property.FinanceData.CurrentInstallmentPayed >= GetPropertyNextPaymentPrice(property)) // reset everything because all stuff was payed on time
                {
                    property.FinanceData.CurrentInstallmentPayed = 0;
                    property.FinanceData.ConsecutiveMissedPayments = 0;
                    property.FinanceData.LastPaymentDate = DateTime.Now;

                    MySQL.execute("UPDATE property_finance SET LastPayment = CURRENT_TIMESTAMP WHERE PropertyID = @propid", new Dictionary<string, dynamic>
                    {
                        {"@propid", property.PropertyId},
                    });
                    SaveFinanceInformation(property);

                    return;
                }

                Log.Verbose($"Property {property.Address} ({property.PropertyId}) wasn't payed on time. Increasing ConsecutiveMissedPayments from {property.FinanceData.ConsecutiveMissedPayments} to {property.FinanceData.ConsecutiveMissedPayments + 1}");
                Log.Verbose($"Property {property.Address} ({property.PropertyId}) wasn't payed on time. Increasing TotalMissedPayments from {property.FinanceData.TotalMissedPayments} to {property.FinanceData.TotalMissedPayments + 1}");
                property.FinanceData.ConsecutiveMissedPayments += 1;
                property.FinanceData.TotalMissedPayments += 1;

                if (property.FinanceData.ConsecutiveMissedPayments >= Settings.PropertyMaxAllowedConsecutiveMissedPayments || property.FinanceData.TotalMissedPayments >= Settings.PropertyMaxAllowedMissedPayments && property.OwnerCharacterId != -1)
                {
                    Log.Info($"Property {property.Address} ({property.PropertyId}) Is being foreclosed due to either the consecutive missed payments or total missed payments exceding their max values");
                    Log.Verbose($"Property {property.Address} ({property.PropertyId}) ConsecutiveMissedPayments: {property.FinanceData.ConsecutiveMissedPayments}");
                    Log.Verbose($"Property {property.Address} ({property.PropertyId}) TotalMissedPayments: {property.FinanceData.TotalMissedPayments}");

                    property.DesyncProperty();
                    property.OwnerCharacterId = -1;
                    foreach (var tenant in property.PropertyCharacterAccess)
                    {
                        Server.Get<PropertyManager>().RemovePropertyTenant(property, tenant);
                    }

                    foreach (var guest in property.TemporaryCharacterAccess)
                    {
                        Server.Get<PropertyManager>().RemovePropertyTenant(property, guest);
                    }

                    MySQL.execute("UPDATE vehicle_data SET Garage = 'Public1' WHERE Garage = ?", new List<string> { $"home-{property.PropertyId}" }, new Action<dynamic>(data =>
                    {
                        Log.Verbose($"Reset garages for vehicles in property {property.PropertyId} to Public1 due to the property being foreclosed");
                    }));
                }

                SaveFinanceInformation(property);
            }
        }

        public void SaveFinanceInformation(PropertyModel property)
        {
            MySQL.execute("UPDATE property_finance SET PropertyPrice = @price, TotalAmountPaid = @curpaid, TotalMissedPayments = @totalmissed, ConsecutiveMissedPayments = @conmissed, CurrentInstallmentPayed = @curinstall WHERE PropertyID = @propid", new Dictionary<string, dynamic>
            {
                {"@price", property.FinanceData.PropertyPrice},
                {"@curpaid", property.FinanceData.TotalAmountPaid},
                {"@totalmissed", property.FinanceData.TotalMissedPayments},
                {"@conmissed", property.FinanceData.ConsecutiveMissedPayments},
                {"@curinstall", property.FinanceData.CurrentInstallmentPayed},
                {"@propid", property.PropertyId},
            });
        }

        public int GetPropertyNextPaymentPrice(PropertyModel property)
        {
            var finance = property.FinanceData;
            var priceToPay = finance.PropertyPrice - (int)Math.Ceiling(finance.PropertyPrice * Settings.PropertyRequiredDownPaymentPercent);

            if (finance.TotalAmountPaid >= priceToPay) // do maintenance pay
            {
                return (int)Math.Ceiling(finance.PropertyPrice * Settings.PropertyMaintenancePayPercent);
            }

            var nextPaymentPrice = priceToPay / Settings.PropertyMortgageTotalPayments;

            if (finance.ConsecutiveMissedPayments > 0)
            {
                nextPaymentPrice *= finance.ConsecutiveMissedPayments;
            }

            if (finance.CurrentInstallmentPayed + nextPaymentPrice > finance.PropertyPrice) // larger than needed reduce to only what is needed
            {
                nextPaymentPrice = finance.PropertyPrice - finance.CurrentInstallmentPayed;
            }

            return nextPaymentPrice;
        }

        [ServerCommand("property_finance|mortgage_info")]
        private void OnShowFinanceCommand(Command cmd)
        {
            var property = cmd.Session.GetClosestPropertyWithTenancy();

            if (property == null)
            {
                cmd.Session.Message("[Property]", "You are not near enough to one of your properties to check finance details", ConstantColours.Housing);
                return;
            }

            var session = cmd.Session;

            session.Message("[Property]", $"Finance infomation for {property.Address}", ConstantColours.Housing);
            session.Message("[Property]", $"Total property price: ${property.FinanceData.PropertyPrice}", ConstantColours.Housing);
            if(property.FinanceData.PropertyPrice > property.FinanceData.TotalAmountPaid) // not paid off initial mortgage yet
            {
                session.Message("[Property]", $"Total amount paid: ${property.FinanceData.TotalAmountPaid}", ConstantColours.Housing);
                session.Message("[Property]", $"Remaining mortgage amount: ${property.FinanceData.PropertyPrice - property.FinanceData.TotalAmountPaid}", ConstantColours.Housing);
            }
            else // payed off mortgage maintenance payment time
            {
                //session.Message("[Property]", $"Total mortgage paid: ${property.FinanceData.PropertyPrice}", ConstantColours.Housing);
                session.Message("[Property]", $"Total maintenance paid: ${property.FinanceData.TotalAmountPaid - property.FinanceData.PropertyPrice}", ConstantColours.Housing);
            }
            session.Message("[Property]", $"Total missed payments: {property.FinanceData.TotalMissedPayments}/{Settings.PropertyMaxAllowedMissedPayments}", ConstantColours.Housing);
            session.Message("[Property]", $"Consecutive missed payments: {property.FinanceData.ConsecutiveMissedPayments}/{Settings.PropertyMaxAllowedConsecutiveMissedPayments}", ConstantColours.Housing);

            var dayUntilPayment = getDaysUntilNextPayment(property.FinanceData);
            var dayUntilPayString = "";

            var remainingPay = GetPropertyNextPaymentPrice(property);
            var overPay = -1;

            if (remainingPay - property.FinanceData.CurrentInstallmentPayed < 0)
            {
                overPay = property.FinanceData.CurrentInstallmentPayed - remainingPay;
                remainingPay = 0;
            }
            else
            {
                remainingPay -= property.FinanceData.CurrentInstallmentPayed;
            }

            if (dayUntilPayment > 0)
            {
                dayUntilPayString = $"Full payment due is due in {dayUntilPayment} days";
            }
            else if (dayUntilPayment == 0)
            {
                dayUntilPayString = $"Payment is due today";
            }
            else if (dayUntilPayment < 0)
            {
                dayUntilPayString = $"Payment was due {dayUntilPayment * -1} days ago";
            }

            if (remainingPay <= 0)
            {
                dayUntilPayString = $"You have paid all you need for the current installment. The next installment will be needed {(dayUntilPayment == 0 ? "tommorow" : $"in {dayUntilPayment} days")}";
            }

            session.Message("[Property]", dayUntilPayString, ConstantColours.Housing);

            if(remainingPay > 0)
            {
                session.Message("[Property]", $"Remaining cost for current installment ${remainingPay}", ConstantColours.Housing);
            }

            if (overPay != -1 && overPay > 0)
            {
                session.Message("[Property]", $"You have payed ${overPay} over what is needed for the current installment", ConstantColours.Housing);
            }
        }

        [ServerCommand("property_finance|mortgage_pay")]
        private void OnPayPropertyBills(Command cmd)
        {
            var property = cmd.Session.GetClosestPropertyWithTenancy();
            var optionalPayAmount = cmd.GetArgAs(0, 0);
            var session = cmd.Session;

            if (property == null || optionalPayAmount < 0)
            {
                cmd.Session.Message("[Property]", "You are not near enough to one of your properties to pay bills", ConstantColours.Housing);
                return;
            }

            var totalInstallmentPay = GetPropertyNextPaymentPrice(property);
            var totalPayNeeded = totalInstallmentPay - property.FinanceData.CurrentInstallmentPayed; // how much is remaining for this installment

            var totalRemainingPay = property.FinanceData.PropertyPrice - property.FinanceData.TotalAmountPaid; // how much is remaining on total payment

            if (totalPayNeeded < 0)
                totalPayNeeded = 0;

            if (optionalPayAmount < 0)
                optionalPayAmount = 0;

            if (totalPayNeeded == 0 && optionalPayAmount == 0)
            {
                session.Message("[Property]", $"You have already payed all that is needed for this current installment. If you would like to pay more please do /property mortgage pay [amount]", ConstantColours.Housing);
                return;
            }

            var payHandler = Server.Get<PaymentHandler>();

            if (optionalPayAmount == 0 || optionalPayAmount == totalPayNeeded) // Only pay what is needed since no optional amount was entered
            {
                if (payHandler.CanPayForItem(session, totalPayNeeded, 1, (int)PaymentType.Debit))
                {
                    payHandler.PayForItem(session, totalPayNeeded, $"paying mortgage on property {property.Address} ({property.PropertyId})", (int)PaymentType.Debit);

                    session.Message("[Property]", $"You just payed the full instalment of ${totalPayNeeded} for this property.", ConstantColours.Housing);

                    MySQL.execute("UPDATE property_finance SET LastPayment = CURRENT_TIMESTAMP WHERE PropertyID = @propid", new Dictionary<string, dynamic>
                    {
                        {"@propid", property.PropertyId},
                    });
                    property.FinanceData.ConsecutiveMissedPayments = 0;
                    property.FinanceData.CurrentInstallmentPayed = totalInstallmentPay;
                    property.FinanceData.TotalAmountPaid += totalInstallmentPay;
                    //property.FinanceData.LastPaymentDate = DateTime.Now;

                    SaveFinanceInformation(property);
                }
                else
                {
                    session.Message("[Property]", $"You cannot afford the ${totalPayNeeded} payment. Please make sure you have the appropiate funds in you bank account", ConstantColours.Housing);
                }

                return;
            }

            // if optional pay was selected this will happen

            if (optionalPayAmount > totalRemainingPay)
                optionalPayAmount = totalRemainingPay;

            if (!payHandler.CanPayForItem(session, optionalPayAmount, 1, (int)PaymentType.Debit))
            {
                session.Message("[Property]", $"You cannot afford a payment of ${optionalPayAmount}. Please make sure you have the appropiate funds in you bank account", ConstantColours.Housing);
                return;
            }

            property.FinanceData.CurrentInstallmentPayed += optionalPayAmount;
            property.FinanceData.TotalAmountPaid += optionalPayAmount;
            if (optionalPayAmount > totalPayNeeded)
            {
                session.Message("[Property]", $"You just paid ${optionalPayAmount} towards this properties bills, which is ${optionalPayAmount - totalPayNeeded} more than is needed for this current installment", ConstantColours.Housing);
            }
            else
            {
                session.Message("[Property]", $"You paid ${optionalPayAmount} out of the needed ${totalPayNeeded} payment. You still need to pay another ${totalPayNeeded - optionalPayAmount}. If you do not pay this before your next payment date it will count as a missed payment", ConstantColours.Housing);
            }
            payHandler.PayForItem(session, optionalPayAmount, $"paying mortgage on property {property.Address} ({property.PropertyId})", (int)PaymentType.Debit);
            SaveFinanceInformation(property);
        }

        [ServerCommand("listpropertyfinance", AdminLevel.Developer)]
        private void OnListPropertyFinance(Command cmd)
        {
            var property = Server.Get<PropertyManager>().GetProperty(cmd.GetArgAs(0, ""));

            if (property == null) return;

            Log.Info($"{property.FinanceData}");
        }

        private int getDaysUntilNextPayment(PropertyModel property) => getDaysUntilNextPayment(property.FinanceData);

        private int getDaysUntilNextPayment(PropertyFinanceInformation data)
        {
            var lpd = data.LastPaymentDate;

            return Convert.ToInt32(Settings.PropertyTimeBetweenPayment - (DateTime.Now - lpd).TotalDays);
        }
    }
}
