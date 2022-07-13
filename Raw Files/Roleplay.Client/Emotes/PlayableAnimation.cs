using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Client.Emotes
{
    public abstract class PlayableAnimation
    {
        public AnimationOptions Options { get; set; } = new AnimationOptions();
        public string DisplayName { get; set; }
        public Action OnAnimEnd;

        protected PlayableAnimation(AnimationOptions options = null)
        {
            if (options != null)
                this.Options = options;
        }

        public virtual async Task PlayFullAnim()
        {
            Client.Instance.RegisterTickHandler(CancelAnim);
            EmoteManager.IsPlayingAnim = true;
        }

        public virtual void End(Ped ped)
        {
            EmoteManager.IsPlayingAnim = false;
            Client.Instance.DeregisterTickHandler(CancelAnim);
            OnAnimEnd?.Invoke();

            if (Options.EndClearTasks && Options.RegisterCheckTick)
            {
                ped.Task.ClearAll();
            }
        }

        public async Task CancelAnim()
        {
            if (!Options.RegisterCheckTick)
            {
                Client.Instance.DeregisterTickHandler(CancelAnim);
                return;
            }

            Game.Player.DisableFiringThisFrame();
            Game.DisableControlThisFrame(1, Control.SelectWeapon);
            Game.DisableControlThisFrame(1, Control.Aim);
            if (Game.IsControlJustPressed(1, Control.Jump) || Game.IsDisabledControlJustPressed(1, Control.Aim))
            {
                End(Game.PlayerPed);
                Client.Instance.DeregisterTickHandler(CancelAnim);
                await BaseScript.Delay(250);
            }
        }
    }
}