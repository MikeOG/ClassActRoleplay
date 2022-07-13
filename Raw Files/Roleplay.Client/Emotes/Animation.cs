using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Enums;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Emotes
{
    public class Animation : PlayableAnimation
    {
        public bool IsPlayingAnim => getAnimState() != AnimState.Ended;
        protected string AnimDict { get; set; }
        protected string EnterAnim { get; set; }
        protected string LoopingAnim { get; set; }
        protected string EndingAnim { get; set; }

        private Prop currentEmoteProp = null;
        private string v1;
        private string v2;
        private string v3;
        private string v4;
        private AnimationOptions animationOptions;

        public Animation(string animDict, string enterAnim, string loopingAnim, string endingAnim, string displayName, AnimationOptions options = null)
            : base(options)
        {
            AnimDict = animDict;
            EnterAnim = enterAnim;
            LoopingAnim = loopingAnim;
            EndingAnim = endingAnim;
            DisplayName = displayName;
            //if (options != null)
            //Options = options;
        }

        public Animation(string v1, string v2, string v3, string v4, AnimationOptions animationOptions)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
            this.animationOptions = animationOptions;
        }

        public override async Task PlayFullAnim()
        {
            base.PlayFullAnim();
            await LoadAnimDicts();
            Client.Instance.RegisterTickHandler(CheckAnimTick);

            var playerPed = Game.PlayerPed;
            //if (currentAnimState == AnimState.Ended)
            {
                if (EnterAnim != "")
                {

                    playStart(playerPed);
                    await BaseScript.Delay(100);
                    while (isPlayingAnim(playerPed, AnimDict, EnterAnim))
                        await BaseScript.Delay(0);
                }

                playLoop(playerPed);
                await BaseScript.Delay(100);
                while (isPlayingAnim(playerPed, AnimDict, LoopingAnim))
                    await BaseScript.Delay(0);

                //playEnd(playerPed);
            }
        }

        private async void playStart(Ped ped)
        {
            if (!Options.PlayStart) return;

            if (Options.AttachPropStart) await AttachEmoteProp();

            await ped.Task.PlayAnimation(
                getDictString("base"),
                EnterAnim,
                4f,
                -8f,
                Options.StartDuration,
                Options.StartAnimOverride == (AnimationFlags)(-1) ? GetAnimationFlag(Options.StartEnableMovement) : Options.StartAnimOverride,
                Options.StartPlaybackRate
            );

            setAnimState(AnimState.Starting);
        }

        private async void playLoop(Ped ped)
        {
            if (Options.AttachPropLoop) await AttachEmoteProp();

            var animDict = Options.OverrideLoopAnimDict ? getDictString("idle_a") : AnimDict;
            await ped.Task.PlayAnimation(
                animDict,
                LoopingAnim,
                50f,
                -8f,
                Options.LoopDuration,
                Options.LoopAnimOverride == (AnimationFlags)(-1) ? GetAnimationFlag(Options.LoopEnableMovement, Options.LoopDoLoop) : Options.LoopAnimOverride,
                Options.LoopPlaybackRate
            );

            await BaseScript.Delay(100);
            setAnimState(AnimState.Looping);
            while (isPlayingAnim(ped, animDict, LoopingAnim))
                await BaseScript.Delay(0);

            if (!Options.LoopDoLoop)
                base.End(ped);
        }

        public override async void End(Ped ped)
        {
            if (!Options.PlayEnd)
            {
                base.End(ped);
                if (currentEmoteProp != null)
                {
                    currentEmoteProp.Detach();
                    currentEmoteProp.Delete();
                    currentEmoteProp = null;
                }
            }

            if (Options.AttachPropEnd) await AttachEmoteProp();

            /*ped.Task.PlayAnimation(
                getDictString("exit"),
                EndingAnim,
                4f,
                -1,
                AnimationFlags.None
            );*/

            await ped.Task.PlayAnimation(
                getDictString("exit"),
                EndingAnim,
                4f,
                -8f,
                Options.EndDuration,
                Options.EndAnimOverride == (AnimationFlags)(-1) ? GetAnimationFlag(Options.EndEnableMovement) : Options.EndAnimOverride,
                Options.EndPlaybackRate
            );

            await BaseScript.Delay(100);
            setAnimState(AnimState.Ended);
            while (isPlayingAnim(ped, AnimDict, EndingAnim))
                await BaseScript.Delay(0);

            if (currentEmoteProp != null)
            {
                currentEmoteProp.Detach();
                currentEmoteProp.Delete();
                currentEmoteProp = null;
            }

            base.End(ped);
        }

        private async Task AttachEmoteProp()
        {
            if (Options.Prop != (ObjectHash)(-1) && currentEmoteProp == null)
            {
                var model = new Model(Options.Prop.ToString());
                while (!model.IsLoaded)
                    await model.Request(0);

                currentEmoteProp = await World.CreateProp(model, Game.PlayerPed.Bones[Options.PropBone].Position, Options.PropRotation, false, true);
                currentEmoteProp.AttachTo(Game.PlayerPed.Bones[Options.PropBone], Options.PropOffset, Options.PropRotation);
            }
        }

        private AnimationFlags GetAnimationFlag(bool enableMovement, bool loopAnim = false)
        {
            var animFlag = AnimationFlags.None;

            if (enableMovement && loopAnim)
            {
                animFlag = (AnimationFlags)51;
            }
            else if (enableMovement && !loopAnim)
            {
                animFlag = (AnimationFlags)50;
            }
            else if (loopAnim && !enableMovement)
            {
                animFlag = (AnimationFlags)1;
            }

            return animFlag;
        }

        private string getDictString(string dictEnd)
        {
            if (AnimDict.Contains("@base") || AnimDict.Contains("@exit") || AnimDict.Contains("@idle_a") || AnimDict.Contains("@idle_b"))
            {
                var stringSplit = AnimDict.Split('@').ToList();
                stringSplit.Remove(stringSplit.Last());
                var animRemoved = string.Join("@", stringSplit);
                return animRemoved + $"@{dictEnd}";
            }
            else
            {
                return AnimDict; /*+ $"@{dictEnd}";*/
            }
        }

        private async Task LoadAnimDicts()
        {
            var dictNames = new List<string> { "base", "exit", "idle_a", "idle_b" };
            await dictNames.ForEachAsync(async o =>
            {
                if (!API.DoesAnimDictExist(o)) return;
                API.RequestAnimDict(getDictString(o));
                while (!API.HasAnimDictLoaded(getDictString(o)))
                {
                    await BaseScript.Delay(0);
                    API.RequestAnimDict(getDictString(o));
                }
            });
        }

        private async Task CheckAnimTick()
        {
            if (!Options.RegisterCheckTick)
            {
                Client.Instance.DeregisterTickHandler(CheckAnimTick);
                return;
            }

            if (!IsPlayingAnim)
            {
                await BaseScript.Delay(2000);
                return;
            }

            var ped = Cache.PlayerPed;
            if (!isPlayingAnim(ped, this.AnimDict, this.EnterAnim) && !isPlayingAnim(ped, this.AnimDict, this.LoopingAnim) && !isPlayingAnim(ped, this.AnimDict, this.EndingAnim))
            {
                Client.Instance.DeregisterTickHandler(CheckAnimTick);
                EmoteManager.IsPlayingAnim = false;
                this.End(ped);
            }
            else
            {
                EmoteManager.IsPlayingAnim = true;
            }
        }

        private bool isPlayingAnim(Ped ped, string dict, string anim) => Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, ped.NativeValue, dict, anim, 3);

        private AnimState getAnimState() => Game.PlayerPed.HasDecor("Player.AnimState") ? (AnimState)Game.PlayerPed.GetDecor<int>("Player.AnimState") : AnimState.Ended;

        private void setAnimState(AnimState state) => Game.PlayerPed.SetDecor("Player.AnimState", (int)state);
    }

}