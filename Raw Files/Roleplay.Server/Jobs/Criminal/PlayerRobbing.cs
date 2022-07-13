using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Bank;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.EMS;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Server.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.Criminal
{
    class PlayerRobbing : ServerAccessor
    {
        public PlayerRobbing(Server server) : base(server)
        {
            CommandRegister.RegisterCommand("rob", OnRobCommand);
        }

        private void OnRobCommand(Command cmd)
        {
            var closestPlayer = cmd.Session.GetClosestPlayer(9.0f);

            if (closestPlayer != null && (closestPlayer.GetGlobalData("Character.HasHandsUp", false) || closestPlayer.GetGlobalData("Character.HasHandsOverHead", false) || /*Server.Get<ArrestHandler>().GetCuffState(closestPlayer)*/closestPlayer.CuffState != CuffState.None || closestPlayer.DeathState == DeathState.Dead) && !closestPlayer.GetLocalData("Character.IsBeingRobbed", false))
            {
                Log.Verbose($"{cmd.Session.PlayerName} is robbing {closestPlayer.PlayerName}");

                Log.ToClient("[Robbery]", "You are being robbed!", ConstantColours.Blue, closestPlayer.Source);
                Log.ToClient("[Robbery]", "You start robbing someone", ConstantColours.Blue, cmd.Session.Source);
                closestPlayer.SetLocalData("Character.IsBeingRobbed", true);
                StartRobberyThread(cmd.Session, closestPlayer);
            }
        }

        private async void StartRobberyThread(Session.Session robberSession, Session.Session victimSession)
        {
            var robberyCash = victimSession.GetGlobalData("Character.Cash", 0);

            if (robberyCash == 0)
            {
                Log.ToClient("[Robbery]", "You found no cash", ConstantColours.Blue, robberSession.Source);
                Log.ToClient("[Robbery]", "The robber found no cash on you", ConstantColours.Blue, victimSession.Source);
                victimSession.SetLocalData("Character.IsBeingRobbed", false);
                return;
            }

            var robberyWaitTimer = CitizenFX.Core.Native.API.GetConvarInt("mg_playerRobbingTime", 6);

            while (robberyWaitTimer > 0)
            {
                var robberPos = robberSession.GetPlayerPosition();
                var victimPos = victimSession.GetPlayerPosition();

                Log.Debug($"Robbery between {robberSession.PlayerName} and {victimSession.PlayerName} distance = {robberPos.DistanceToSquared(victimPos)}");
                if (robberPos.DistanceToSquared(victimPos) > 4.0f)
                {
                    Log.ToClient("[Robbery]", "You moved to far away to finish the robbery", ConstantColours.Blue, robberSession.Source);
                    Log.ToClient("[Robbery]", "You moved away and are no longer being robbed", ConstantColours.Blue, victimSession.Source);
                    victimSession.SetLocalData("Character.IsBeingRobbed", false);

                    return;
                }

                if (!victimSession.GetGlobalData("Character.HasHandsUp", false) && !victimSession.GetGlobalData("Character.HasHandsOverHead", false) && /*Server.Get<ArrestHandler>().GetCuffState(victimSession)*/victimSession.CuffState == CuffState.None && /*Server.Get<DeathHandler>().GetDeathState(victimSession)*/victimSession.DeathState == DeathState.Alive)
                {
                    Log.ToClient("[Robbery]", "The person you are robbing put their hands down", ConstantColours.Blue, robberSession.Source);
                    Log.ToClient("[Robbery]", "You put your hands down", ConstantColours.Blue, victimSession.Source);
                    victimSession.SetLocalData("Character.IsBeingRobbed", false);

                    return;
                }

                await BaseScript.Delay(1000);
                robberyWaitTimer--;
            }

            var payHandler = Server.Get<PaymentHandler>();

            payHandler.UpdatePlayerCash(robberSession, robberyCash, $"robbing {victimSession.PlayerName}");
            payHandler.UpdatePlayerCash(victimSession, -robberyCash, $"being robbed by {robberSession.PlayerName}");

            Log.ToClient("[Robbery]", $"You just robbed this person for ${robberyCash}", ConstantColours.Blue, robberSession.Source);
            Log.ToClient("[Robbery]", $"You just got robbed for ${robberyCash}", ConstantColours.Blue, victimSession.Source);
        }
    }
}
