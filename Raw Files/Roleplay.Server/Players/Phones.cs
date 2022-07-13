using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enviroment;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Players
{
    public class Phones : ServerAccessor
    {
        private Contacts contact;

        public Phones(Server server) : base(server)
        {
            contact = Server.Get<Contacts>();
            CommandRegister.RegisterCommand("text", OnTextCommand);
            CommandRegister.RegisterCommand("phonenumber|phone", cmd =>
            {
                Log.ToClient("[Phone]", IntToPhoneNumber(cmd.Session.GetGlobalData<int>("Character.CharID")), ConstantColours.Phone, cmd.Player);
            });
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        
        {
            BaseScript.TriggerEvent("Phones.AddCharacterNumber", playerSession.ServerID, IntToPhoneNumber(playerSession.CharId), /*$"steam:{playerSession.SteamIdentifier}"*/playerSession.CharId);
        }

        public int PhoneNumberToInt(string numString)
        {
            int number = -1;
            try
            {
                number = Convert.ToInt32(numString.Replace("555-", ""));
            }
            catch (Exception e)
            {

            }

            return number;
        }

        public string IntToPhoneNumber(int number)
        {
            return string.Format("555-{0:D4}", number);
        }

        public Session.Session GetPlayerWithNumber(int phoneNumber)
        {
            foreach (var session in Server.Get<Session.SessionManager>().PlayerList)
            {
                var charId = session.GetGlobalData("Character.CharID", 0);

                if (charId == phoneNumber)
                    return session;
            }

            return null;
        }

        private void OnTextCommand(Command cmd)
        {
            Session.Session targetSession = null;
            var targetPlayer = cmd.GetArgAs(0, "");
            var isDirectNumber = int.TryParse(targetPlayer, out var targetNum);

            if(/*Server.Get<ArrestHandler>().GetCuffState(cmd.Session)*/cmd.Session.CuffState != CuffState.None || /*Server.Get<ArrestHandler>().GetDragState(cmd.Session)*/cmd.Session.DragState == DragState.Dragged) return;

            if (isDirectNumber)
            {
                targetSession = GetPlayerWithNumber(targetNum);
            }
            else
            {
                var contacts = contact.GetContacts(cmd.Session);

                if (contacts.ContainsKey(targetPlayer))
                {
                    targetNum = PhoneNumberToInt(contacts[targetPlayer]);
                    targetSession = GetPlayerWithNumber(targetNum);
                }
            }

            if (cmd.Session.GetGlobalData("Character.JailTime", 0) > 0) return;

            if (targetSession != null)
            {
                cmd.Args.RemoveAt(0);
                var messageText = string.Join(" ", cmd.Args);

                Log.ToClient($"To - {IntToPhoneNumber(targetNum)}", messageText, ConstantColours.Yellow, cmd.Player);
                Messages.SendProximityMessage(cmd.Session, "[Text]", "Someone sends a text message", ConstantColours.Phone, 20.0f);
                Log.ToClient($"From - {IntToPhoneNumber(cmd.Session.GetGlobalData("Character.CharID", 0))}", messageText, ConstantColours.Yellow, targetSession.Source);
                Messages.SendProximityMessage(targetSession, "[Text]", "Someone receives a text message", ConstantColours.Phone, 20.0f);

                cmd.Player.TriggerEvent("Player.PlayTextAnim");

                //Messages.SendProximityMessage(cmd.Session, "[On phone]", messageText, ConstantColours.Yellow, 25.0f, false);
            }
        }
    }
}
