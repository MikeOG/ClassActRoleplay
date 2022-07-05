using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Roleplay.Client.Enums;
using Roleplay.Client.Player.Controls;
using Roleplay.Client.UI.Classes;
using Roleplay.Shared;
using Font = CitizenFX.Core.UI.Font;

namespace Roleplay.Client.Player
{
    /*internal class VoipRanges : ClientAccessor
    {
        private Dictionary<string, float> voipTypes = new Dictionary<string, float>()
        {
            ["Whisper"] = 3.0f,
            ["Nearby"] = 12.0f,
            ["Shout"] = 43.0f
        };
        private List<float> voipRanges = new List<float>();
        private float currentVoipRange = 12.0f;
        private string currentVoipName = "Nearby";
        private ScreenText voiceText;
        private ScreenText rangeText;
        private bool isTalking;
        private bool isInVehicle;

        public VoipRanges(Client client) : base(client)
        {
            CommandRegister.RegisterCommand("voip", changeVoipRange);
            voipTypes.ToList().ForEach(o => voipRanges.Add(o.Value));
            client.RegisterTickHandler(OnTick);
            voiceText = new ScreenText("Voice: ", 31, 1026, 0.25f, TextThread, System.Drawing.Color.FromArgb(255, 200, 200, 200), Font.ChaletLondon, Alignment.Left);
            rangeText = new ScreenText(currentVoipName, 78, 1026, 0.25f, VoipRangeTick, System.Drawing.Color.FromArgb(255, 200, 200, 200), Font.ChaletLondon, Alignment.Left);

            CitizenFX.Core.Native.API.NetworkSetTalkerProximity(voipTypes["Nearby"]);
            client.RegisterEventHandler("Session.Loaded", new Action(() =>
            {
                CitizenFX.Core.Native.API.NetworkSetTalkerProximity(voipTypes["Nearby"]);
                CitizenFX.Core.Native.API.NetworkClearVoiceChannel();
            })); 
        }

        private async Task OnTick()
        {
            if(CinematicMode.InCinematicMode) return;

            if (Input.IsControlPressed(Control.ThrowGrenade, true, ControlModifier.Shift))
            {
                changeVoipRange();
            }

            isInVehicle = Cache.PlayerPed.IsInVehicle() && Cache.PlayerPed.CurrentVehicle.ClassType != VehicleClass.Cycles;
            isTalking = Game.IsControlPressed(1, Control.PushToTalk);
            voiceText.DrawTick();
            rangeText.DrawTick();
        }

        private async Task TextThread()
        {
            System.Drawing.Color textColour = System.Drawing.Color.FromArgb(255, 200, 200, 200);
            var voiceX = 31 * 0.6666666667f;
            var voiceY = 1026 * 0.6666666667f;
            
            if(isInVehicle)
            {
                voiceX = 31 * 0.6666666667f;
                voiceY = 1026 * 0.6666666667f;
            }

            if (isTalking)
                textColour = System.Drawing.Color.FromArgb(200, 0, 191, 255);

            voiceText.Position = new PointF(voiceX, voiceText.Position.Y);
            voiceText.Position = new PointF(voiceText.Position.X, voiceY);
            voiceText.Color = textColour;
        }

        private async Task VoipRangeTick()
        {
            System.Drawing.Color textColour = System.Drawing.Color.FromArgb(255, 200, 200, 200);
            var voipX = 78 * 0.6666666667f;
            var voipY = 1026 * 0.6666666667f;
            if (isInVehicle)
            {
                voipX = 78 * 0.6666666667f;
                voipY = 1026 * 0.6666666667f;
            }

            if (isTalking)
                textColour = System.Drawing.Color.FromArgb(200, 0, 191, 255);

            rangeText.Position = new PointF(voipX, rangeText.Position.Y);
            rangeText.Position = new PointF(rangeText.Position.X, voipY);
            rangeText.Caption = currentVoipName;
            rangeText.Color = textColour;
        }

        private bool changingVoip = false;

        private async void changeVoipRange()
        {
            if (changingVoip) return;
            changingVoip = true;
            // Checks if we should reset the list search
            float nextVoipRange = voipRanges[voipRanges.FindIndex(o => o == currentVoipRange) == voipRanges.Count - 1 ? 0 : voipRanges.FindIndex(o => o == currentVoipRange) + 1];
            currentVoipRange = nextVoipRange;
            currentVoipName = voipTypes.First(o => o.Value == nextVoipRange).Key;
            CitizenFX.Core.Native.API.NetworkSetTalkerProximity(currentVoipRange);
            await BaseScript.Delay(100);
            changingVoip = false;
        }

        private void changeVoipRange(Command cmd)
        {
            try // Because it errors out for some reason if no args are present
            {
                string newVoip = cmd.GetArgAs(0, "Nearby");
                // Does voip name even exist?
                float newVoipRange = voipTypes.FirstOrDefault(o => o.Key.ToLower() == newVoip.ToLower()).Value;
                if (newVoipRange == default(float))
                {
                    currentVoipRange = voipTypes["Nearby"];
                    currentVoipName = "Nearby";
                }
                else
                {
                    currentVoipRange = newVoipRange;
                    currentVoipName = voipTypes.First(o => o.Key.ToLower() == newVoip.ToLower()).Key;
                }
            }
            catch
            {
                currentVoipRange = voipTypes["Nearby"];
                currentVoipName = "Nearby";
            }
            finally
            {
                CitizenFX.Core.Native.API.NetworkSetTalkerProximity(currentVoipRange);
            }
        }
    }*/
}
