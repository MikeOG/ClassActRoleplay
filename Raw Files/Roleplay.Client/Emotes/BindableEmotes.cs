using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Helpers;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.Emotes
{
    public class BindableEmotes : ClientAccessor
    {
        private Dictionary<Control, string> controlToString = new Dictionary<Control, string>
        {
            {Control.ReplayStartStopRecording, "F1" },
            {Control.ReplayStartStopRecordingSecondary, "F2" },
            {Control.SaveReplayClip, "F3" },
            {Control.SelectCharacterMichael, "F5" },
            {Control.SelectCharacterFranklin, "F6" },
        };

        public BindableEmotes(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
            CommandRegister.RegisterCommand("rebind", cmd =>
            {
                var key = cmd.GetArgAs(0, "");
                var emote = cmd.GetArgAs(1, "");
                if (controlToString.FirstOrDefault(o => o.Value.ToLower() == key.ToLower()).Value != null)
                {
                    if (EmoteManager.playerAnimations.ContainsKey(emote))
                    {
                        client.TriggerServerEvent("Emotes.BindEmote", key.ToUpper(), emote);
                        Log.ToChat("[Info]", $"Bound {key.ToUpper()} to emote {emote}", ConstantColours.Info);
                    }
                    else
                    {
                        Log.ToChat("[Info]", $"The emote {emote} doesn't exist", ConstantColours.Info);
                    }
                }
                else
                {
                    Log.ToChat("[Info]", $"The key {key.ToUpper()} cannot be bound", ConstantColours.Info);
                }
            });
        }

        private async Task OnTick()
        {
            if (LocalSession == null) return;

            foreach (var kvp in controlToString)
            {
                if (Input.IsControlJustPressed(kvp.Key))
                {
                    var GetCharacterSettings = LocalSession.GetCharacterSettings();
                    var keybinds = GetCharacterSettings["EmoteBinds"].GetType() == typeof(JObject) ? ((JObject)GetCharacterSettings["EmoteBinds"]).ToObject<Dictionary<string, string>>() : GetCharacterSettings["EmoteBinds"];
                    if (keybinds.ContainsKey(kvp.Value) && !Cache.PlayerPed.IsInVehicle())
                    {
                        EmoteManager.PlayAnimation(keybinds[kvp.Value]);
                    }
                }
            }
        }
    }
}