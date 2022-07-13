using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Client.Emotes;
using Roleplay.Client.Enums;
using Roleplay.Shared;

namespace Roleplay.Client.Jobs.Civillian.Fishing
{
    class Fishing : ClientAccessor
    {
        private Animation fishingAnim = new Animation("amb@world_human_stand_fishing@base", "", "base", "", "Fish", new AnimationOptions
        {
            LoopDoLoop = true,
            LoopEnableMovement = true,
            OverrideLoopAnimDict = false,
            AttachPropLoop = true,
            Prop = ObjectHash.prop_fishing_rod_01,
            PropBone = Bone.PH_L_Hand,
        });

        public Fishing(Client client) : base(client)
        {
            client.RegisterEventHandler("Fishing.StartFishing", new Action(StartFishing));
        }

        private async void StartFishing()
        {
            fishingAnim.PlayFullAnim();
            await BaseScript.Delay(1500);
            Client.RegisterTickHandler(FishingTick);
        }

        private async Task FishingTick()
        {
            if (!IsEntityPlayingAnim(Cache.PlayerPed.Handle, "amb@world_human_stand_fishing@base", "base", 3))
            {
                Client.TriggerServerEvent("Fishing.CancelFishing");
                fishingAnim.End(Cache.PlayerPed);
                Client.DeregisterTickHandler(FishingTick);
            }

            if (!LocalSession.GetLocalData("Character.IsFishing", false))
            {
                fishingAnim.End(Cache.PlayerPed);
                Client.DeregisterTickHandler(FishingTick);
            }
        }
    }
}
