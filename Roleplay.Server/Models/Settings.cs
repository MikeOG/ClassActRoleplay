using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Shared.Models;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Server.Models
{
    public static partial class Settings
    {
#region Queue
        public static bool QueueDebugEnabled => GetConvar("mg_queueDebugEnabled", "false") == "true";
        public static bool QueueGraceEnabled => GetConvar("mg_queueGraceEnabled", "true") == "true";
        public static bool QueueJoiningEnabled => GetConvar("mg_queueJoiningEnabled", "true") == "true";
        public static bool QueueDynamicPermissionRefreshEnabled => GetConvar("mg_queueDynamicRefreshEnabled", "true") == "true";
        public static bool QueueOptionalOptInRequired => GetConvar("mg_queueOptionalOptInRequired", "true") == "true";
        public static int QueueTimeoutPeriod => GetConvarInt("mg_queueTimeoutPeriod", 240);
        public static int QueuePassiveTimeoutPeriod => GetConvarInt("mg_queuePassiveTimeoutPeriod", 420);
        public static int QueueConnectingTimeoutPeriod => GetConvarInt("mg_queueConnectingTimeoutPeriod", 30);
        public static int QueueGraceTimeoutPeriod => GetConvarInt("mg_queueGraceDuration", 300);
        public static int QueueReservedStaffSlots => GetConvarInt("mg_queueReservedStaffSlots", 6);
        public static int QueueReservedEmergencyServiceSlots => GetConvarInt("mg_queueReservedEmergencyServiceSlots", 6);
        public static int QueueTotalReserveSlots => QueueReservedStaffSlots + QueueReservedEmergencyServiceSlots;
        public static int QueueTotalNonPrioritySlots => MaxServerSlots - QueueTotalReserveSlots;
        public static int QueueMaxConnectingPlayers => GetConvarInt("mg_queueMaxConnectingPlayers", 6);
#endregion

#region Bank
        private static CachedConvar<int> bankStartingBalanceConvar = new CachedConvar<int>("bank_starting_bank_balance", 30000, 60000);
        public static int BankStartingBalance => bankStartingBalanceConvar.Value;
#endregion

#region Paycheck
        private static CachedConvar<int> paycheckEmergencyServicesConvar = new CachedConvar<int>("paycheck_emergency_services", 350);
        public static int PaycheckEmergencyServices => paycheckEmergencyServicesConvar.Value;

        private static CachedConvar<int> paycheckTowConvar = new CachedConvar<int>("paycheck_tow", 250);
        public static int PaycheckTow => paycheckTowConvar.Value;

        private static CachedConvar<int> paycheckCivillianConvar = new CachedConvar<int>("paycheck_civillian", 150);
        public static int PaycheckCivillian => paycheckCivillianConvar.Value;

        private static CachedConvar<int> paycheckMechanicConvar = new CachedConvar<int>("paycheck_mechanic", 0);
        public static int PaycheckMechanic => paycheckMechanicConvar.Value;

        private static CachedConvar<int> paycheckTaxiConvar = new CachedConvar<int>("paycheck_taxi", 400);
        public static int PaycheckTaxi => paycheckTaxiConvar.Value;

        private static CachedConvar<int> paycheckDeliveryConvar = new CachedConvar<int>("paycheck_delivery", 300);
        public static int PaycheckDelivery => paycheckDeliveryConvar.Value;

        private static CachedConvar<int> paycheckPayIntervalConvar = new CachedConvar<int>("paycheck_pay_interval", 300000);
        public static int PaycheckPayInterval => paycheckPayIntervalConvar.Value;
#endregion

#region Job Pay
        private static CachedConvar<int> jobPayCityBusConvar = new CachedConvar<int>("job_pay_city_bus", 100);
        public static int JobPayCityBus => jobPayCityBusConvar.Value;

        private static CachedConvar<int> jobPayCoachBusConvar = new CachedConvar<int>("job_pay_coach_bus", 250);
        public static int JobPayCoachBus => jobPayCoachBusConvar.Value;

        private static CachedConvar<int> jobPayGarbageConvar = new CachedConvar<int>("job_pay_coach_bus", 250);
        public static int JobPayGarbage => jobPayGarbageConvar.Value;

        private static CachedConvar<int> jobPayTowMinimumConvar = new CachedConvar<int>("job_pay_tow_minimum", 250);
        public static int JobPayTowMinimum => jobPayTowMinimumConvar.Value;

        private static CachedConvar<int> jobPayTowMaximumConvar = new CachedConvar<int>("job_pay_tow_maximum", 650);
        public static int JobPayTowMaximum => jobPayTowMaximumConvar.Value;
#endregion

#region Server
        public static int MaxServerSlots => GetConvarInt("sv_maxClients", 32);

        private static CachedConvar<bool> whitelistConvar = new CachedConvar<bool>("sv_whitelisted", false, 1000);
        public static bool WhitelistEnabled => whitelistConvar.Value;

        private static CachedConvar<string> serverPasswordConvar = new CachedConvar<string>("sv_password", "", 1000);
        public static string ServerPassword => serverPasswordConvar.Value;
        public static bool PasswordEnabled => !string.IsNullOrEmpty(ServerPassword);

        private static CachedConvar<int> saveIntervalConvar = new CachedConvar<int>("sv_save_interval", 300000);
        public static int SaveInterval => saveIntervalConvar.Value;
#endregion

#region Realtor
        private static CachedConvar<int> propertyTimeBetweenPayemntConvar = new CachedConvar<int>("property_payment_time_between_payment", 14);
        public static int PropertyTimeBetweenPayment => propertyTimeBetweenPayemntConvar.Value;

        private static CachedConvar<int> propertyMaxAllowedMissedPaymentsConvar = new CachedConvar<int>("property_payment_max_allowed_missed_payments", 4);
        public static int PropertyMaxAllowedMissedPayments => propertyMaxAllowedMissedPaymentsConvar.Value;

        private static CachedConvar<int> propertyMaxAllowedConsecutiveMissedPaymentsConvar = new CachedConvar<int>("property_payment_max_allowed_consecutive_missed_payments", 3);
        public static int PropertyMaxAllowedConsecutiveMissedPayments => propertyMaxAllowedConsecutiveMissedPaymentsConvar.Value;

        private static CachedConvar<int> propertyMortgageTotalPaymentsConvar = new CachedConvar<int>("property_payment_mortgage_total_payments", 39);
        public static int PropertyMortgageTotalPayments => propertyMortgageTotalPaymentsConvar.Value;

        private static CachedConvar<float> propertyDownPaymentPercentConvar = new CachedConvar<float>("property_payment_required_down_payment_percent", 0.25f);
        public static float PropertyRequiredDownPaymentPercent => propertyDownPaymentPercentConvar.Value;

        private static CachedConvar<float> propertyDownPaymentPercentForRealtorConvar = new CachedConvar<float>("property_payment_down_payment_percent_for_realtor_on_sale", 0.07f);
        public static float PropertyDownPaymentPercentForRealtor => propertyDownPaymentPercentForRealtorConvar.Value;

        private static CachedConvar<float> propertyMaintenancePayPercentConvar = new CachedConvar<float>("property_payment_maintenance_pay_percent", 0.005f);
        public static float PropertyMaintenancePayPercent => propertyMaintenancePayPercentConvar.Value;

        private static CachedConvar<bool> propertyCreationEnabledConvar = new CachedConvar<bool>("property_creation_enabled", true);
        public static bool PropetyCreationEnabled => propertyCreationEnabledConvar.Value;

        private static CachedConvar<bool> propertyCreationValidationEnabled = new CachedConvar<bool>("property_creation_validation_enabled", true);
        public static bool PropertyCreationValidationEnabled => propertyCreationValidationEnabled.Value;

#endregion
    }
}