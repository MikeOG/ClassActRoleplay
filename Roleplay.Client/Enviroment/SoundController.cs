using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Enviroment
{
    public class SoundController : ClientAccessor
    {
        public SoundController(Client client) : base(client)
        {
            client.RegisterEventHandler("Sound.PlaySoundFrontend", new Action<string, string>(PlayFrontentSound));
        }

        public async void PlayFrontentSound(string audioName, string audioRef)
        {
            var soundId = GetSoundId();
            Log.Debug($"soundId for played sound is {soundId}");
            PlaySoundFrontend(soundId, audioName, audioRef, true);
            await BaseScript.Delay(10000);
            Audio.StopSound(soundId);
            Audio.ReleaseSound(soundId);
        }
    }
}
