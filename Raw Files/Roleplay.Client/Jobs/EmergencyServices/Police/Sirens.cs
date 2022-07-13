using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Helpers;
using Roleplay.Shared;

namespace Roleplay.Client.Jobs.EmergencyServices.Police
{
/*    internal class Sirens : ClientAccessor
    {
        private List<string> useAmbulanceWarning = new List<string>()
        {
            "AMBULANCE",
            "FIRETRUK",
            "LGUARD",
        };

        private List<string> SirenModes = new List<string>()
        {
            "", // No siren - lights only
            "VEHICLES_HORNS_SIREN_1",
            "VEHICLES_HORNS_SIREN_2",
            "VEHICLES_HORNS_POLICE_WARNING"
        };

        // Initial siren preset
        private string CurrentSirenPreset = "";

        // One per player
        // More is not needed
        private Dictionary<int, int> SirenSoundIds = new Dictionary<int, int>();

        private bool SirenActive = false;
        private bool disableControls = false;

        public Sirens(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
            client.RegisterTickHandler(disableKeysTick);
            client.RegisterEventHandler("Sirens.UpdateSirenState", new Action<string, int>(ReceiveSoundEvent));
            client.RegisterEventHandler("Sirens.RecieveSirenOutOfVehState", new Action<int, bool>(handleSirensOutOfVeh));
            EntityDecoration.RegisterProperty("Vehicle.SirenState", DecorationType.Int);
        }

        private async Task OnTick()
        {
            if (!SirenActive && Game.PlayerPed.IsInVehicle()
                && Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed
                && Game.PlayerPed.CurrentVehicle.ClassType == VehicleClass.Emergency)
            {
                disableControls = true;
                Function.Call(Hash.SET_VEH_RADIO_STATION, Game.PlayerPed.CurrentVehicle.Handle, "OFF");
                getVehicleSirenSounds();
                if (Game.IsDisabledControlPressed(1, Control.VehicleHorn))
                {
                    SirenActive = true;
                    SendSoundEvent("SIRENS_AIRHORN");
                    while (Game.IsDisabledControlPressed(1, Control.VehicleHorn) && Game.PlayerPed.IsInVehicle())
                    {
                        await BaseScript.Delay(0);
                    }
                    StopSound();
                    SirenActive = false;
                }
                else if (Game.IsDisabledControlPressed(1, Control.VehicleCinCam))
                {
                    SirenActive = true;
                    SendSoundEvent(SirenModes[1]);
                    while (Game.IsDisabledControlPressed(1, Control.VehicleCinCam) && Game.PlayerPed.IsInVehicle())
                    {
                        await BaseScript.Delay(0);
                    }
                    StopSound();
                    SirenActive = false;
                }
                else if (Game.IsDisabledControlJustPressed(1, Control.SpecialAbilitySecondary))
                {
                    SirenActive = true;
                    Function.Call(Hash.BLIP_SIREN, Game.PlayerPed.CurrentVehicle.Handle);
                    await BaseScript.Delay(700);
                    SirenActive = false;
                }
                else if (Game.IsDisabledControlJustPressed(1, Control.VehicleRadioWheel)) // Preset on/off
                {
                Repeat:
                    CitizenFX.Core.Vehicle playerVeh = Game.PlayerPed.CurrentVehicle;
                    BaseScript.TriggerServerEvent("Sirens.SetSirenOutOfVehState", true);
                    Function.Call(Hash.SET_VEHICLE_SIREN, playerVeh.Handle, true);
                    Function.Call(Hash.SET_SIREN_WITH_NO_DRIVER, playerVeh.Handle, true);

                    SirenActive = true;
                    PlayCurrentPresetSound();

                    while (Game.PlayerPed.IsInVehicle())
                    {
                        await BaseScript.Delay(0);
                        if (Game.IsDisabledControlJustPressed(1, Control.VehicleRadioWheel))
                        {
                            Client.Get<SoundController>().PlayFrontentSound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                            break;
                        }
                        else if (Game.IsDisabledControlJustPressed(1, Control.VehicleCinCam)) // Cycle presets
                        {
                            if (CurrentSirenPreset != "")
                            {
                                StopSound();
                                CurrentSirenPreset = SirenModes[(SirenModes.IndexOf(CurrentSirenPreset) + 1) % SirenModes.Count];
                                if (CurrentSirenPreset == "")
                                    CurrentSirenPreset = SirenModes[1]; // Don't turn off sirens while they are already on
                                PlayCurrentPresetSound();
                            }
                            else
                            {
                                SendSoundEvent(SirenModes[1]);
                                while (Game.IsDisabledControlPressed(1, Control.VehicleCinCam) && Game.PlayerPed.IsInVehicle())
                                {
                                    await BaseScript.Delay(0);
                                }
                                StopSound();
                            }
                        }
                        else if(Game.IsDisabledControlJustPressed(1, Control.ThrowGrenade) || Game.IsDisabledControlJustPressed(1, Control.CharacterWheel))
                        {
                            if (CurrentSirenPreset == "")
                            {
                                CurrentSirenPreset = SirenModes[1];
                                PlayCurrentPresetSound();
                            }
                            else
                            {
                                StopSound();
                                CurrentSirenPreset = "";
                                PlayCurrentPresetSound();
                            }
                        }
                        else if (Game.IsDisabledControlPressed(1, Control.VehicleHorn))
                        {
                            StopSound();
                            SendSoundEvent("SIRENS_AIRHORN");
                            while (Game.IsDisabledControlPressed(1, Control.VehicleHorn) && Game.PlayerPed.IsInVehicle())
                            {
                                await BaseScript.Delay(0);
                            }
                            StopSound();
                            PlayCurrentPresetSound(false);
                        }
                        else if (Game.IsDisabledControlPressed(1, Control.SpecialAbilitySecondary))
                        {
                            StopSound();
                            Function.Call(Hash.BLIP_SIREN, playerVeh.Handle);
                            await BaseScript.Delay(700);
                            PlayCurrentPresetSound();
                        }
                    }
                    if(!Game.PlayerPed.IsInVehicle())
                    {
                        Client.TriggerServerEvent("Sirens.SetSirenOutOfVehState", false);
                        while (!Game.PlayerPed.IsInVehicle() && playerVeh.Exists())
                            await BaseScript.Delay(0);

                        if (playerVeh.Exists())
                            goto Repeat;
                    }
                    StopSound();
                    if(playerVeh.Exists()) Function.Call(Hash.SET_VEHICLE_SIREN, playerVeh.Handle, false);
                    SirenActive = false;
                    CurrentSirenPreset = "";
                }
            }
            else if (!Game.PlayerPed.IsInVehicle())
            {
                SirenActive = false;
                disableControls = false;
            }
        }

        private Task disableKeysTick()
        {
            if(disableControls)
            {
                Game.DisableControlThisFrame(1, Control.VehicleHorn);
                Game.DisableControlThisFrame(1, Control.SpecialAbilitySecondary);
                Game.DisableControlThisFrame(1, Control.Duck);
                Game.DisableControlThisFrame(1, Control.VehicleCinCam);
                Game.DisableControlThisFrame(1, Control.VehicleRadioWheel);
                Game.DisableControlThisFrame(1, Control.ThrowGrenade);
                Game.DisableControlThisFrame(1, Control.CharacterWheel);
            }

            return Task.FromResult(0);
        }

        private void getVehicleSirenSounds()
        {
            SirenModes = new List<string>()
            {
                "", // No siren - lights only
                "VEHICLES_HORNS_SIREN_1",
                "VEHICLES_HORNS_SIREN_2",
                "VEHICLES_HORNS_POLICE_WARNING"
            };
        }

        private void PlaySound(int sourceServerId, string sound)
        {
            SirenSoundIds[sourceServerId] = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, SirenSoundIds[Game.Player.ServerId], sound, Game.PlayerPed.CurrentVehicle.Handle, 0, 0, 0);
        }

        private class SoundEventModel
        {
            public string SoundName { get; set; } = ""; // Empty sound name will be our stop event
            public int SourceServerId { get; set; }
            public SoundEventModel() { }
        }

        private void SendSoundEvent(string sound)
        {
            BaseScript.TriggerServerEvent("Sirens.SendStateUpdate", sound, Game.Player.ServerId);
        }

        private async void ReceiveSoundEvent(string sound, int source)
        {
            try
            {
                Log.Debug($"Recieved a sound of ({sound}) from source {source}");
                SoundEventModel SoundEvent = new SoundEventModel { SoundName = sound, SourceServerId = source };
                if ((SoundEvent.SoundName == "STOP" || SoundEvent.SoundName == "") && SirenSoundIds.ContainsKey(SoundEvent.SourceServerId))
                {
                    if (SirenSoundIds[SoundEvent.SourceServerId] != -1)
                    {
                        Function.Call(Hash.STOP_SOUND, SirenSoundIds[SoundEvent.SourceServerId]);
                        Function.Call(Hash.RELEASE_SOUND_ID, SirenSoundIds[SoundEvent.SourceServerId]);
                        SirenSoundIds[SoundEvent.SourceServerId] = -1;
                    }

                    CitizenFX.Core.Vehicle playerVeh = Client.PlayerList[SoundEvent.SourceServerId].Character.CurrentVehicle;
                    if(playerVeh != null) Function.Call(Hash.DISABLE_VEHICLE_IMPACT_EXPLOSION_ACTIVATION, playerVeh.Handle, true);
                }
                else
                {
                    if (SirenSoundIds.ContainsKey(SoundEvent.SourceServerId))
                    {
                        if (SirenSoundIds[SoundEvent.SourceServerId] != -1)
                        {
                            Function.Call(Hash.STOP_SOUND, SirenSoundIds[SoundEvent.SourceServerId]);
                            Function.Call(Hash.RELEASE_SOUND_ID, SirenSoundIds[SoundEvent.SourceServerId]);
                        }
                    }

                    SirenSoundIds[SoundEvent.SourceServerId] = Function.Call<int>(Hash.GET_SOUND_ID);
                    int retrieveAmounts = 0;
                    while (SirenSoundIds[SoundEvent.SourceServerId] == -1 && retrieveAmounts <= 300)
                    {
                        SirenSoundIds[SoundEvent.SourceServerId] = Function.Call<int>(Hash.GET_SOUND_ID);
                        retrieveAmounts++;
                        await BaseScript.Delay(0);
                    }
                    Log.Debug($"Sound ID for source {SoundEvent.SourceServerId} is {SirenSoundIds[SoundEvent.SourceServerId]}");
                    Ped targetPed = Client.PlayerList[SoundEvent.SourceServerId].Character;
                    CitizenFX.Core.Vehicle targetVeh = targetPed.IsInVehicle() ? targetPed.CurrentVehicle : targetPed.LastVehicle;
                    if(targetVeh != null)
                        Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, SirenSoundIds[SoundEvent.SourceServerId], SoundEvent.SoundName, targetVeh.Handle, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void PlayCurrentPresetSound(bool playSound = true)
        {
            SendSoundEvent(CurrentSirenPreset);
            if(playSound) Client.Get<SoundController>().PlayFrontentSound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        }

        private void StopSound()
        {
            SendSoundEvent("STOP");
        }

        private void handleSirensOutOfVeh(int player, bool state)
        {
            var playerList = Client.PlayerList;
            var targetVeh = playerList[player].Character.IsInVehicle() ? Client.PlayerList[player].Character.CurrentVehicle : playerList[player].Character.LastVehicle;
            if (targetVeh == null) return;
            Log.Debug($"Got a vehicle of handle {targetVeh.Handle} and a state of {state}");
            Function.Call(Hash.DISABLE_VEHICLE_IMPACT_EXPLOSION_ACTIVATION, targetVeh.Handle, state);
        }
    }*/
}

