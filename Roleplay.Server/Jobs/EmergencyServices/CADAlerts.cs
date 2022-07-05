using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices
{
    public class CADAlerts : JobClass
    {
        public static List<CADAlertData> CurrentAlerts = new List<CADAlertData>();

        public CADAlerts()
        {
            Server.Instance.RegisterEventHandler("Alerts.SendCADAlert", new Action<Player, string, string, string>(OnSendCADAlert));
        }

        public void OnSendCADAlert([FromSource] Player source, string alertType, string alertMessage, string alertLocation)
        {
            var playerSession = Server.Instance.Instances.Session.GetPlayer(source);
            if(playerSession == null) return;

            //if(JobHandler.OnDutyAs(playerSession, JobType.Police | JobType.EMS)) return;

            JobHandler.SendJobAlert(JobType.Police, "[Dispatch]", $"{alertType} | {alertMessage} | {alertLocation}", ConstantColours.Dispatch);  
            JobHandler.GetPlayersOnJob(JobType.Police).ForEach(o =>
            {
                o.TriggerEvent("Sound.PlaySoundFrontend", "Event_Message_Purple", "GTAO_FM_Events_Soundset");
            });
            
            CurrentAlerts.Add(new CADAlertData
            {
                AlertType = alertType,
                AlertMessage = alertMessage,
                AlertLocation = alertLocation,
            });
        }
    }

    public class CADAlertData
    {
        public int AlertID = -1;
        public string AlertType = "";
        public string AlertCaller = "Local";
        public string AlertMessage = "";
        public string AlertLocation = "";
        public double AlertTime = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        public string AlertStatus = "Inactive";
        public string AlertUnits = "";
        public string AlertOther = "";
    }
}
