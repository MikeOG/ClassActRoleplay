using CitizenFX.Core;
using Roleplay.Client.Enums;
using Roleplay.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roleplay.Client.Emotes
{
    internal static class HoldRadio
    {
        private static readonly List<JobType> WhitelistedJobs = new List<JobType>()
        {
          JobType.Police,
          JobType.EMS
        };

        public static void Init() => Client.Instance.RegisterTickHandler(new Func<Task>(HoldRadio.OnTick));

        private static async Task OnTick()
        {
                if (!ControlHelper.IsControlPressed((Control)243, true, ControlModifier.None))
                    return;
                Game.PlayerPed.Task.ClearAll();
                Game.PlayerPed.Task.PlayAnimation("random@arrests", "generic_radio_chatter", -1f, -1, (AnimationFlags)49);
                while (ControlHelper.IsControlPressed((Control)243, true, ControlModifier.None))
                    await BaseScript.Delay(0);
                Game.PlayerPed.Task.ClearAnimation("random@arrests", "generic_radio_chatter");
        }
    }
}
