using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Enviroment;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Players
{
    public class Radios : ServerAccessor
    {
        public Radios(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("r|radio", OnRadioCommand);
            CommandRegister.RegisterCommand("ri|radioinitial", OnRadioInitial);
            CommandRegister.RegisterCommand("setfreq|setradiofrequency|setradiofreq", SetRadioFrequency);
        }

        public int GetPlayerRadioChannel(Session.Session playerSession)
        {
            var radioItem = playerSession./*GetInventory()*/Inventory.GetItem("Radio");
            if(radioItem != null)
            {
                if (radioItem.metaData != "")
                {
                    return Convert.ToInt32(radioItem.metaData);
                }

                return 1;
            }

            return -1;
        }

        public void SendRadioMessage(Session.Session sourceSession, int radioChannel, string radioMessage)
        {
            Sessions.ForAllClients(o =>
            {
                var otherRadioChannel = GetPlayerRadioChannel(o);

                if (radioChannel == otherRadioChannel)
                {
                    Log.ToClient($"[Radio Channel {radioChannel}]", radioMessage, ConstantColours.Radio, o.Source);
                }
            });
            Messages.SendProximityMessage(sourceSession, "[On Radio]", radioMessage, ConstantColours.Radio, 25.0f, false);
            sourceSession.TriggerEvent("Player.PlayRadioAnim");
        }

        private void OnRadioCommand(Command cmd)
        {
            var radioMessage = string.Join(" ", cmd.Args);
            var playerRadioChannel = GetPlayerRadioChannel(cmd.Session);

            if (/*Server.Get<ArrestHandler>().GetCuffState(cmd.Session)*/cmd.Session.CuffState != CuffState.None || /*Server.Get<ArrestHandler>().GetDragState(cmd.Session)*/cmd.Session.DragState == DragState.Dragged) return;

            if (playerRadioChannel != -1)
                SendRadioMessage(cmd.Session, playerRadioChannel, radioMessage);
        }

        private void OnRadioInitial(Command cmd)
        {
            var radioMessage = string.Join(" ", cmd.Args);
            var playerRadioChannel = GetPlayerRadioChannel(cmd.Session);
            var nameInitials = $"{cmd.Session.GetGlobalData<string>("Character.FirstName")[0]}{cmd.Session.GetGlobalData<string>("Character.LastName")[0]}";

            if (playerRadioChannel != -1)
                SendRadioMessage(cmd.Session, playerRadioChannel, $"{nameInitials.ToUpper()}: {radioMessage}");
        }

        private void SetRadioFrequency(Command cmd)
        {
            var targetFrequency = cmd.GetArgAs(0, 1);
            var playerInv = cmd.Session./*GetInventory()*/Inventory;
            var playerRadio = playerInv.GetItem("Radio");

            if (playerRadio != null)
            {
                if (targetFrequency >= 1 && targetFrequency <= 10000)
                {
                    Log.ToClient($"[Radio]", $"Set radio frequency to {targetFrequency}", ConstantColours.Radio, cmd.Player);
                    playerRadio.metaData = targetFrequency.ToString();
                    cmd.Session.SetGlobalData("Character.Inventory", playerInv.GetInvString());
                }
                else
                {
                    Log.ToClient($"[Radio]", $"{targetFrequency} is an invalid radio frequency", ConstantColours.Radio, cmd.Player);
                }
            }
        }
    }
}
