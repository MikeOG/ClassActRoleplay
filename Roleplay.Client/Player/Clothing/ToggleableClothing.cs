using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Shared;

namespace Roleplay.Client.Player.Clothing
{
    internal class ToggleableClothing : ClientAccessor
    {
        private int glassesPropIndex = 0;
        private int glassesTextureIndex = 0;

        private int hatPropIndex = 0;
        private int hatTextureIndex = 0;

        private int maskPropIndex = 0;
        private int maskTextureIndex = 0;

        public ToggleableClothing(Client client) : base(client)
        {

            client.RegisterEventHandler("Player.OnSkinLoaded", new Action(() =>
            {
                glassesPropIndex = Game.PlayerPed.Style[PedProps.Glasses].Index;
                glassesTextureIndex = Game.PlayerPed.Style[PedProps.Glasses].Index;
                hatPropIndex = Game.PlayerPed.Style[PedProps.Hats].Index;
                hatTextureIndex = Game.PlayerPed.Style[PedProps.Hats].TextureIndex;
                maskPropIndex = Game.PlayerPed.Style[PedComponents.Head].Index;
                maskTextureIndex = Game.PlayerPed.Style[PedComponents.Head].TextureIndex;
            }));
            CommandRegister.RegisterCommand("glasses", handleGlassesCommand);
            CommandRegister.RegisterCommand("hat", handleHatCommand);
            CommandRegister.RegisterCommand("mask", handleMaskCommand);
        }

        private async void handleGlassesCommand(Command cmd)
        {
            if(Cache.PlayerPed.IsInVehicle()) return;

            var currentGlasses = Game.PlayerPed.Style[PedProps.Glasses];
            switch (cmd.Args.Count)
            {
                case 0 when currentGlasses.Index == 0:
                    await Game.PlayerPed.Task.PlayAnimation("veh@bicycle@roadfront@base", "put_on_helmet", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f); // TODO find actual animation
                    await BaseScript.Delay(1050);
                    currentGlasses.Index = glassesPropIndex;
                    currentGlasses.TextureIndex = glassesTextureIndex;
                    break;
                case 0:
                    await Game.PlayerPed.Task.PlayAnimation("veh@bike@common@front@base", "take_off_helmet_walk", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f); // TODO find actual animation
                    await BaseScript.Delay(800);
                    glassesPropIndex = currentGlasses.Index;
                    currentGlasses.Index = 0;
                    break;
                default:
                    if (cmd.Args.ElementAt(0).ToString() == "on")
                    {
                        if (currentGlasses.Index == 0)
                        {
                            await Game.PlayerPed.Task.PlayAnimation("veh@bicycle@roadfront@base", "put_on_helmet", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f); // TODO find actual animation
                            await BaseScript.Delay(1050);
                            currentGlasses.Index = glassesPropIndex;
                            currentGlasses.TextureIndex = glassesTextureIndex;
                        }
                    }
                    else if (cmd.Args.ElementAt(0).ToString() == "off")
                    {
                        await Game.PlayerPed.Task.PlayAnimation("veh@bike@common@front@base", "take_off_helmet_walk", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f); // TODO find actual animation
                        await BaseScript.Delay(800);
                        glassesPropIndex = currentGlasses.Index;
                        currentGlasses.Index = 0;
                    }

                    break;
            }
        }

        private async void handleHatCommand(Command cmd)
        {
            if (Cache.PlayerPed.IsInVehicle()) return;

            var currentHat = Game.PlayerPed.Style[PedProps.Hats];
            switch (cmd.Args.Count)
            {
                case 0 when currentHat.Index == 0:
                    await Game.PlayerPed.Task.PlayAnimation("veh@bicycle@roadfront@base", "put_on_helmet", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                    await BaseScript.Delay(1050);
                    currentHat.Index = hatPropIndex;
                    currentHat.TextureIndex = hatTextureIndex;
                    break;
                case 0:
                    await Game.PlayerPed.Task.PlayAnimation("veh@bike@common@front@base", "take_off_helmet_walk", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                    await BaseScript.Delay(800);
                    hatPropIndex = currentHat.Index;
                    currentHat.Index = 0;
                    break;
                default:
                    if (cmd.Args.ElementAt(0).ToString() == "on")
                    {
                        if (currentHat.Index == 0)
                        {
                            await Game.PlayerPed.Task.PlayAnimation("veh@bicycle@roadfront@base", "put_on_helmet", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                            await BaseScript.Delay(1050);
                            currentHat.Index = hatPropIndex;
                            currentHat.TextureIndex = hatTextureIndex;
                        }
                    }
                    else if (cmd.Args.ElementAt(0).ToString() == "off")
                    {
                        await Game.PlayerPed.Task.PlayAnimation("veh@bike@common@front@base", "take_off_helmet_walk", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                        await BaseScript.Delay(800);
                        hatPropIndex = currentHat.Index;
                        currentHat.Index = 0;
                    }

                    break;
            }
        }

        private async void handleMaskCommand(Command cmd)
        {
            if (Cache.PlayerPed.IsInVehicle()) return;

            var currentMask = Game.PlayerPed.Style[PedComponents.Head];
            switch (cmd.Args.Count)
            {
                case 0 when currentMask.Index == 0:
                    await Game.PlayerPed.Task.PlayAnimation("misscommon@van_put_on_masks", "put_on_mask_ps", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                    await BaseScript.Delay(1050);
                    currentMask.Index = maskPropIndex;
                    currentMask.TextureIndex = maskTextureIndex;
                    break;
                case 0:
                    await Game.PlayerPed.Task.PlayAnimation("missfbi4", "takeoff_mask", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                    await BaseScript.Delay(750);
                    maskPropIndex = currentMask.Index;
                    currentMask.Index = 0;
                    break;
                default:
                    if (cmd.Args.ElementAt(0).ToString() == "on")
                    {
                        if (currentMask.Index == 0)
                        {
                            await Game.PlayerPed.Task.PlayAnimation("misscommon@van_put_on_masks", "put_on_mask_ps", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                            await BaseScript.Delay(1050);
                            currentMask.Index = maskPropIndex;
                            currentMask.TextureIndex = maskTextureIndex;
                        }
                    }
                    else if (cmd.Args.ElementAt(0).ToString() == "off")
                    {
                        await Game.PlayerPed.Task.PlayAnimation("missfbi4", "takeoff_mask", 2.0f, 2.0f, -1, (AnimationFlags)52, 0.0f);
                        await BaseScript.Delay(750);
                        maskPropIndex = currentMask.Index;
                        currentMask.Index = 0;
                    }

                    break;
            }
        }
    }
}
