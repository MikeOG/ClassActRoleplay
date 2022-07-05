using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.MySQL;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices.Police
{
    public class JailTimeHandler : ServerAccessor
    {
        public JailTimeHandler(Server server) : base(server)
        {
            server.RegisterTickHandler(JailTimeTick);
            server.RegisterTickHandler(OfflineServedTimeTick);
            CommandRegister.RegisterCommand("showjail|showjailtime", OnShowJailTime);
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            UpdateJailState(playerSession);
        }

        public void UpdateJailState(Session.Session session)
        {
            var jailTime = session.GetGlobalData("Character.JailTime", 0);
            if(jailTime > 0 && !session.GetLocalData("Jail.IsInJail", false))
            {
                session.SetServerData("Jail.LastJailReduction", DateTime.Now);
                session.SetLocalData("Jail.IsInJail", true);
                session.TriggerEvent("Jail.SetJailState", true);
            }
            else if (jailTime <= 0 && session.GetLocalData("Jail.IsInJail", false))
            {
                session.TriggerEvent("Jail.SetJailState", false);
                session.SetLocalData("Jail.IsInJail", false);
            }
        }

        private async Task JailTimeTick()
        {
            foreach (var session in Sessions.PlayerList)
            {
                var jailTime = session.GetGlobalData("Character.JailTime", 0);
                if (jailTime > 0)
                {
                    var lastReduction = session.GetServerData("Jail.LastJailReduction", DateTime.Now);
                    Log.Debug($"{session.PlayerName} JailTime minute count = ({(DateTime.Now - lastReduction).TotalMinutes})");
                    if ((DateTime.Now - lastReduction).TotalMinutes >= 1)
                    {
                        var newJailTime = jailTime - 1;
                        session.SetServerData("Jail.LastJailReduction", DateTime.Now);
                        session.SetGlobalData("Character.JailTime", newJailTime);
                        UpdateJailState(session);

                        if (newJailTime % 3 == 0)
                        {
                            Log.ToClient("[Jail]", $"You have {newJailTime} months left on your sentence", ConstantColours.Jail, session.Source);
                        }
                    }
                }
            }

            await BaseScript.Delay(4000);
        }

        //private Query<int> jailTimeQuery = new Query<int>("UPDATE character_data SET JailTime = JailTime - 1", new Dictionary<string, dynamic> { {"JailTime", "> 30" } });
        private async Task OfflineServedTimeTick()
        {
            await BaseScript.Delay(180000);

            //jailTimeQuery.Execute(rows => { });
            MySQL.execute("UPDATE character_data SET JailTime = JailTime - 1 WHERE JailTime > 30");
        }

        private void OnShowJailTime(Command cmd)
        {
            var session = cmd.Session;
            var jailTime = session.GetGlobalData("Character.JailTime", 0);

            if (jailTime == 0)
            {
                Log.ToClient("[Jail]", "You do not have a jail sentence", ConstantColours.Jail, cmd.Player);
            }
            else
            {
                Log.ToClient("[Jail]", $"You have {jailTime} months remaining on your sentence", ConstantColours.Jail, cmd.Player);
            }
        }
    }
}
