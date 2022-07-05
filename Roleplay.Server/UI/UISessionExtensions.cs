using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Roleplay.Server.UI
{
    public static class UISessionExtensions
    {
        /// <summary>
        /// Fades a players screen in and out
        /// </summary>
        /// <param name="playerSession">Session of target player</param>
        /// <param name="initialTransitionTime">How long it should take for the screen to transition into black</param>
        /// <param name="transitionWaitTime">How long the screen should stay black for</param>
        /// <param name="exitTransitionTime">How long it should take for the screen to transition out of black</param>
        /// <returns>Awaitable task that will finish around the time the players screen will be black</returns>
        public static async Task Transition(this Session.Session playerSession, int initialTransitionTime, int transitionWaitTime, int exitTransitionTime)
        {
            playerSession.TriggerEvent("UI.StartScreenTransition", initialTransitionTime, transitionWaitTime, exitTransitionTime);
            await BaseScript.Delay(initialTransitionTime);
        }
    }
}
