using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Fade = CitizenFX.Core.UI.Screen.Fading;

namespace Roleplay.Client.UI.Utils
{
    public class UIUtils : ClientAccessor
    {
        public UIUtils(Client client) : base(client)
        {
            client.RegisterEventHandler("UI.StartScreenTransition", new Action<int, int, int>(StartScreenTransition));
        }

        /// <summary>
        /// Fades a players screen in and out
        /// </summary>
        /// <param name="initialTransitionTime">How long it should take for the screen to transition into black</param>
        /// <param name="transitionWaitTime">How long the screen should stay black for</param>
        /// <param name="exitTransitionTime">How long it should take for the screen to transition out of black</param>
        /// <returns>Awaitable task</returns>
        public static async Task DoScreenTransition(int initialTransitionTime, int transitionWaitTime, int exitTransitionTime)
        {
            Fade.FadeOut(initialTransitionTime);
            while (Fade.IsFadingOut)
                await BaseScript.Delay(0);

            await BaseScript.Delay(transitionWaitTime);

            Fade.FadeIn(exitTransitionTime);
            while (Fade.IsFadingIn)
                await BaseScript.Delay(0);
        }

#region Remote event methods
        private static void StartScreenTransition(int initialTransitionTime, int transitionWaitTime, int exitTransitionTime)
        {
            DoScreenTransition(initialTransitionTime, transitionWaitTime, exitTransitionTime);
        }
#endregion
    }
}
