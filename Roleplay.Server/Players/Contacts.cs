using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Server.Players
{
    public class Contacts : ServerAccessor
    {
        public Contacts(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("addcontact", OnAddContact);
            CommandRegister.RegisterCommand("removecontact", OnRemoveContact);
            CommandRegister.RegisterCommand("showcontacts|contacts", OnShowContacts);
        }

        public Dictionary<string, string> GetContacts(Session.Session playerSession)
        {
            var characterSettings = playerSession.CharacterSettings;

            return characterSettings.ContainsKey("Contacts") ? JsonConvert.DeserializeObject<Dictionary<string, string>>(characterSettings["Contacts"]) : new Dictionary<string, string>();
        }

        public void SetContacts(Session.Session playerSession, Dictionary<string, string> contacts)
        {
            var characterSettings = playerSession./*GetCharacterSettings()*/CharacterSettings;

            characterSettings["Contacts"] = JsonConvert.SerializeObject(contacts);

            playerSession./*SetCharacterSettings()*/CharacterSettings = characterSettings;
        }

        private void OnAddContact(Command cmd)
        {
            var contacts = GetContacts(cmd.Session);
            var contactName = cmd.GetArgAs(0, "");
            var contactNumber = cmd.GetArgAs(1, "");

            contacts[contactName] = contactNumber;
            SetContacts(cmd.Session, contacts);
            Log.ToClient("[Phone]", $"Added contact {contactName} with number {contactNumber} to your contact list", ConstantColours.Phone, cmd.Player);
        }
        
        private void OnRemoveContact(Command cmd)
        {
            var contacts = GetContacts(cmd.Session);
            var contactName = cmd.GetArgAs(0, "").ToLower();

            if (contacts.ContainsKey(contactName))
            {
                contacts.Remove(contactName);
                SetContacts(cmd.Session, contacts);
                Log.ToClient("[Phone]", $"Removed contact {contactName} from your contact list", ConstantColours.Phone, cmd.Player);
            }
            else
            {
                Log.ToClient("[Phone]", $"Unable to find {contactName} in your contact list", ConstantColours.Phone, cmd.Player);
            }
        }
        
        private void OnShowContacts(Command cmd)
        {
            var contacts = GetContacts(cmd.Session);
            var contactsString = "\n";

            foreach (var contact in contacts)
            {
                var phoneNum = contact.Value;
                if (!phoneNum.Contains("555-"))
                {
                    phoneNum = "555-" + phoneNum;
                }

                contactsString += $"{contact.Key} - {phoneNum}\n";
            }

            Log.ToClient("[Contacts]", contactsString, ConstantColours.Phone, cmd.Player);
        }
    }
}
