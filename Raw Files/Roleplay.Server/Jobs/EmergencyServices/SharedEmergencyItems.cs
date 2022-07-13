using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.Enviroment;
using Roleplay.Server.Helpers;
using Roleplay.Server.Jobs.EmergencyServices.Police;
using Roleplay.Server.Jobs.Models;
using Roleplay.Server.Realtor;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Jobs.EmergencyServices
{
    public class SharedEmergencyItems : JobClass
    {
        private List<JobType> allowedDutyJobs = new List<JobType>
        {
            JobType.EMS,
            JobType.Police,
            JobType.Mechanic
        };

        public Dictionary<string, DutyLoadout> DutyLoadouts = new Dictionary<string, DutyLoadout>
        {
            ["PoliceRegular"] = new DutyLoadout
            {
                LoadoutWeapons = new List<string>
                {
                    "CombatPistol",
                    "StunGun",
                    "Nightstick",
                    "Flashlight",
                    "SmokeGrenade",
                    "Flare",
                    "FlareGun",
                    "FireExtinguisher",
                    "PetrolCan"
                }
            },
            ["EMSRegular"] = new DutyLoadout
            {
                LoadoutWeapons = new List<string>
                {
                    "StunGun",
                    "Flashlight",
                    "FireExtinguisher",
                    "PetrolCan"
                }
            }
        };

        public Dictionary<string, Vector3> spawnLocations = new Dictionary<string, Vector3>
        {
            {"lspd", new Vector3(455.201f, -990.897f, 30.6896f) },
            {"bcso", new Vector3(1853.36f, 3686.58f, 34.267f) },
            {"ems", new Vector3(301.07f, -596.41f, 43.28f) },
            {"bcso2", new Vector3(-445.03f, 6013.52f, 31.72f) },
        };

        public SharedEmergencyItems()
        {
            CommandRegister.RegisterJobCommand("duty", OnDutyRequest, JobType.Police | JobType.EMS | JobType.Mechanic, true);
            CommandRegister.RegisterCommand("911", on911Command);
            CommandRegister.RegisterJobCommand("911r", on911Reply, JobType.Police | JobType.EMS);
            CommandRegister.RegisterCommand("311", on311Command);
            CommandRegister.RegisterJobCommand("311r", on311Reply, JobType.Police | JobType.EMS);
            CommandRegister.RegisterJobCommand("cv", cmd => cmd.Player.TriggerEvent("Job.SpawnServiceVehicle", cmd.GetArgAs(0, 1) - 1), JobType.Police | JobType.EMS);
            CommandRegister.RegisterJobCommand("impound", cmd => cmd.Player.TriggerEvent("Job.DeleteVehicle", true), JobType.Police | JobType.EMS | JobType.Mechanic | JobType.Tow);
            CommandRegister.RegisterJobCommand("fix", cmd => cmd.Player.TriggerEvent("Job.FixVehicle"), JobType.Police | JobType.EMS | JobType.Mechanic);
            CommandRegister.RegisterJobCommand("repair", cmd => cmd.Player.TriggerEvent("Job.RepairVehicle"), JobType.Police | JobType.EMS | JobType.Mechanic);
            CommandRegister.RegisterJobCommand("dv", cmd => cmd.Player.TriggerEvent("Job.DeleteVehicle"), JobType.Police | JobType.EMS);
            CommandRegister.RegisterAdminCommand("dv", cmd => cmd.Player.TriggerEvent("Job.DeleteVehicle"), AdminLevel.Moderator);
            CommandRegister.RegisterJobCommand("13", OnPanicButton, JobType.Police | JobType.EMS);
            CommandRegister.RegisterJobCommand("slimjim", cmd => cmd.Player.TriggerEvent("Lockpick.StartVehicleLockpick", 2), JobType.EMS | JobType.Police | JobType.Mechanic);
            CommandRegister.RegisterJobCommand("extra", cmd => cmd.Player.TriggerEvent("Job.SetVehicleExtra", cmd.GetArgAs(0, "1"), cmd.GetArgAs(1, "false")), JobType.Police | JobType.EMS);
            CommandRegister.RegisterJobCommand("setspawn", OnSetSpawn, JobType.EMS | JobType.Police, true);
            //CommandRegister.RegisterJobCommand("resetspawn", OnResetSpawn, JobType.EMS | JobType.Police, true);
            CommandRegister.RegisterJobCommand("discount", cmd => cmd.Player.TriggerEvent("LSC:magicmechanic"), JobType.Mechanic);
            CommandRegister.RegisterJobCommand("setcallsign", OnSetCallsign, JobType.Police | JobType.EMS);
            CommandRegister.RegisterJobCommand("MDT", OnMDT, JobType.Police);
        }

        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var characterSettings = playerSession.CharacterSettings;

            if (characterSettings.ContainsKey("SpawnLocation"))
            {
                string location = characterSettings["SpawnLocation"];
                if(!location.Contains("home")) // TODO make this less hard-codey
                {
                    playerSession.SetPlayerPosition(spawnLocations[location]);
                    return;
                }

                Server.Get<PropertyManager>().SpawnPlayerAtProperty(playerSession, location.Split('-')[1]);
            }
        }

        private void OnMDT(Command cmd)
        {
            BaseScript.TriggerEvent("mdt:toggleVisibilty");
        }

        public void OnPlayerDisconnected(Session.Session playerSession)
        {
            var job = JobHandler.GetPlayerJob(playerSession);

            if (job == JobType.Mechanic)
            {
                CitizenFX.Core.Native.API.ExecuteCommand($"remove_ace identifier.{Server.CurrentIdentifier}:{playerSession.SteamIdentifier} customs.mechanicprice allow");
            }
        }

        public void OnDutyChangeState(Session.Session playerSession)
        {
            RefreshLoadout(playerSession);
        }

        public void RefreshLoadout(Session.Session playerSession)
        {
            if (JobHandler.OnDutyAs(playerSession, JobType.EMS | JobType.Police))
            {
                var dutyLoadout = DutyLoadouts[$"{JobHandler.GetPlayerJob(playerSession)}Regular"];
                playerSession.TriggerEvent("Weapons.RemoveAllWeapons");
                foreach (var weapon in dutyLoadout.LoadoutWeapons)
                {
                    playerSession.TriggerEvent("Weapons.GiveWeapon", weapon, weapon == "PetrolCan" ? 4500 : 500);
                }
                playerSession.TriggerEvent("Player.OnDutyStatusChange", true);
            }
            else
            {
                playerSession.TriggerEvent("Skin.RefreshSkin");
                playerSession.TriggerEvent("Weapons.LoadWeapons");
            }
        }

        private async void OnDutyRequest(Command cmd)
        {
            var playerSession = Sessions.GetPlayer(cmd.Player);

            var currentDutyStatus = JobHandler.OnDuty(playerSession);
            var nextDutyStatus = !currentDutyStatus;

            var job = JobHandler.GetPlayerJob(playerSession);
            if (allowedDutyJobs.Contains(job))
            {
                playerSession.SetGlobalData("Character.OnDuty", nextDutyStatus);
                playerSession.TriggerEvent("Player.OnDutyStatusChange", nextDutyStatus);
                Sessions.TriggerSessionEvent("OnDutyChangeState", playerSession);

                if (job == JobType.Mechanic)
                {
                    await BaseScript.Delay(0);
                    if (nextDutyStatus)
                    {
                        CitizenFX.Core.Native.API.ExecuteCommand($"add_ace identifier.{Server.CurrentIdentifier}:{playerSession.SteamIdentifier} customs.mechanicprice allow");
                    }
                    else
                    {
                        CitizenFX.Core.Native.API.ExecuteCommand($"remove_ace identifier.{Server.CurrentIdentifier}:{playerSession.SteamIdentifier} customs.mechanicprice allow");
                    }
                }

                //Log.Info($"{!currentDutyStatus}");
                Log.ToClient("[Duty]", $"{(nextDutyStatus ? "On" : "Off" )} duty", ConstantColours.Job, cmd.Player);
            }
        }

        private void on911Command(Command cmd)
        {
            doEmergencyCall(cmd, "911");
        }

        private void on911Reply(Command cmd)
        {
            doEmergencyResponse(cmd, "911");
        }

        private void on311Command(Command cmd)
        {
            doEmergencyCall(cmd, "311");
        }

        private void on311Reply(Command cmd)
        {
            doEmergencyResponse(cmd, "311");
        }

        private async void doEmergencyCall(Command cmd, string emergencyNumber)
        {
            var message = string.Join(" ", cmd.Args);

            Log.ToClient($"To {emergencyNumber}", message, ConstantColours.Phone, cmd.Player);
            JobHandler.SendJobAlert(JobType.EMS | JobType.Police, $"{emergencyNumber} | {await cmd.Session.GetLocation()} | #{cmd.Source}", message, ConstantColours.Phone);
            Messages.SendProximityMessage(cmd.Session, "[On phone]", message, ConstantColours.Yellow, 25.0f, false);

            if (emergencyNumber == "911")
            {
                JobHandler.GetPlayersOnJob(JobType.EMS | JobType.Police).ForEach(o =>
                {
                    o.TriggerEvent("Blip.CreateEmergencyBlip", cmd.Source);
                    o.TriggerEvent("Sound.PlaySoundFrontend", "Event_Start_Text", "GTAO_FM_Events_Soundset");
                });
            }

            CADAlerts.CurrentAlerts.Add(new CADAlertData
            {
                AlertType = emergencyNumber,
                AlertCaller = $"{cmd.Session.GetGlobalData("Character.FirstName", "")} {cmd.Session.GetGlobalData("Character.LastName", "")}",
                AlertLocation = await cmd.Session.GetLocation(),
                AlertMessage = message,
            });
        }

        private void doEmergencyResponse(Command cmd, string emergencyNumber)
        {
            var target = cmd.GetArgAs(0, 0);
            if (target == 0) return;

            cmd.Args.RemoveAt(0);
            var message = string.Join(" ", cmd.Args);

            Log.ToClient($"To #{target}", message, ConstantColours.Phone, cmd.Player);
            Log.ToClient($"From {emergencyNumber}", message, ConstantColours.Phone, /*Server.PlayerList*/Players[target]);
            Players[target].TriggerEvent("Sound.PlaySoundFrontend", "Event_Start_Text", "GTAO_FM_Events_Soundset");
            JobHandler.SendJobAlert(JobType.EMS | JobType.Police, $"{emergencyNumber} to #{target}", message, ConstantColours.Phone);
        }

        private async void OnPanicButton(Command cmd)
        {
            var playerLocation = await cmd.Session.GetLocation();
            JobHandler.SendJobAlert(JobType.EMS | JobType.Police, "[Dispatch]", $"10-13 | EMERGENCY PANIC BUTTON | Issuer: {cmd.Session.GetCharacterName()} | {playerLocation}", ConstantColours.Dispatch);
            JobHandler.GetPlayersOnJob(JobType.EMS | JobType.Police).ForEach(async o =>
            {
                o.TriggerEvent("Blip.CreateEmergencyBlip", cmd.Source);
                for (var i = 0; i < 3; i++)
                {
                    o.TriggerEvent("Sound.PlaySoundFrontend", "TIMER_STOP", "HUD_MINI_GAME_SOUNDSET");
                    await BaseScript.Delay(650);
                }
            });
        }

        private void OnSetSpawn(Command cmd)
        {
            var spawnLocation = cmd.GetArgAs(0, "").ToLower();

            if (spawnLocations.ContainsKey(spawnLocation))
            {
                var characterSettings = cmd.Session.CharacterSettings;
                characterSettings["SpawnLocation"] = spawnLocation;
                cmd.Session.CharacterSettings = characterSettings;

                Log.ToClient("[Duty]", $"Set spawn location to {spawnLocation}", ConstantColours.Job, cmd.Player);
            }
        }
        
        /*private void OnResetSpawn(Command cmd)
        {
            var characterSettings = cmd.Session.CharacterSettings;
            if(characterSettings.ContainsKey("SpawnLocation"))
                characterSettings.Remove("SpawnLocation");

            cmd.Session.CharacterSettings = characterSettings;

            Log.ToClient("[Duty]", $"Reset spawn location", ConstantColours.Job, cmd.Player);
        }*/

        private void OnSetCallsign(Command cmd)
        {
            var characterSettings = cmd.Session.CharacterSettings;
            var newCallsign = string.Join(" ", cmd.Args);

            characterSettings["Callsign"] = newCallsign;
            cmd.Session.CharacterSettings = characterSettings;

            cmd.Session.Message("[Duty]", $"Set your callsign to {newCallsign}", ConstantColours.Job);
        }
    }
}
