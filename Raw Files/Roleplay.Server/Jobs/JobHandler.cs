using CitizenFX.Core;
using Roleplay.Server.Enums;
using Roleplay.Server.Helpers;
using Roleplay.Server.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

using Roleplay.Server.Permissions;

namespace Roleplay.Server.Jobs
{
    public sealed class JobHandler : ServerAccessor
    {
        public List<JobType> SaveableJobTypes = new List<JobType>
        {
            JobType.EMS,
            JobType.Mechanic,
            JobType.Police,
            JobType.Realtor,
        };

        public JobHandler(Server server) : base(server)
        {
            server.RegisterEventHandler("Job.SetPlayerJob", new Action<Player, string>(OnRemoteJobSet));
            server.RegisterEventHandler("Job.SetDutyState", new Action<Player, bool>(OnSetDutyState));
            CommandRegister.RegisterAdminCommand("copadd", cmd =>
            {
                var targetPlayer = cmd.GetArgAs<int>(0);
                var targetObject = Sessions.GetPlayer(targetPlayer);
                if (targetObject != null)
                {
                    SetPlayerJob(targetObject, JobType.Police);
                    Log.ToClient("[Job]", "You are now a police officer", ConstantColours.Job, targetObject.Source);
                    Log.ToClient("[Admin]", $"You made {targetObject.PlayerName} a police officer", ConstantColours.Admin, cmd.Player);
                    Sessions.TriggerSessionEvent("OnPermissionRefresh", cmd.Session);
                }
                else
                {
                    cmd.Session.Message($"Invalid ID");
                }
            }, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("emsadd", cmd =>
            {
                var targetPlayer = cmd.GetArgAs<int>(0);
                var targetObject = Sessions.GetPlayer(targetPlayer);
                if (targetObject != null)
                {
                    SetPlayerJob(targetObject, JobType.EMS);
                    Log.ToClient("[Job]", "You are now an EMS", ConstantColours.Job, targetObject.Source);
                    Log.ToClient("[Admin]", $"You made {targetObject.PlayerName} an EMS", ConstantColours.Admin, cmd.Player);
                    Sessions.TriggerSessionEvent("OnPermissionRefresh", cmd.Session);
                }
                else
                {
                    cmd.Session.Message($"Invalid ID");
                }
            }, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("mechadd", cmd =>
            {
                var targetPlayer = cmd.GetArgAs<int>(0);
                var targetObject = Sessions.GetPlayer(targetPlayer);
                if (targetObject != null)
                {
                    SetPlayerJob(targetObject, JobType.Mechanic);
                    Log.ToClient("[Job]", "You are now a mechanic", ConstantColours.Job, targetObject.Source);
                    Log.ToClient("[Admin]", $"You made {targetObject.PlayerName} a mechanic", ConstantColours.Admin, cmd.Player);
                    Sessions.TriggerSessionEvent("OnPermissionRefresh", cmd.Session);
                }
                else
                {
                    cmd.Session.Message($"Invalid ID");
                }
            }, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("realtoradd", cmd =>
            {
                var targetPlayer = cmd.GetArgAs<int>(0);
                var targetObject = Sessions.GetPlayer(targetPlayer);
                if (targetObject != null)
                {
                    SetPlayerJob(targetObject, JobType.Realtor);
                    Log.ToClient("[Job]", "You are now a realtor", ConstantColours.Job, targetObject.Source);
                    Log.ToClient("[Admin]", $"You made {targetObject.PlayerName} a realtor", ConstantColours.Admin, cmd.Player);
                    Sessions.TriggerSessionEvent("OnPermissionRefresh", cmd.Session);
                }
                else
                {
                    cmd.Session.Message($"Invalid ID");
                }
            }, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("coprem|emsrem|mechrem|realtorrem", resetPlayerJob, AdminLevel.Moderator);
            CommandRegister.RegisterAdminCommand("getplayersonjob", OnJobCheck, AdminLevel.SuperAdmin);
            CommandRegister.RegisterAdminCommand("addperm", cmd =>
            {
                var player = cmd.GetArgAs(0, 0);

                cmd.Args.RemoveAt(0);
  
                var targetSession = Sessions.GetPlayer(player);

                if (targetSession == null) return;

                var curPerms = cmd.Session.Permissions;
                curPerms.Add(string.Join(";", cmd.Args));

                targetSession.Permissions = curPerms;

                Sessions.TriggerSessionEvent("OnJobPermissionRefresh", targetSession);

            }, AdminLevel.Admin);
            CommandRegister.RegisterAdminCommand("showperm", cmd =>
            {
                var player = cmd.GetArgAs(0, 0);

                cmd.Args.RemoveAt(0);

                var targetSession = Sessions.GetPlayer(player);

                if (targetSession == null) return;

                targetSession.ListPermissions();
            }, AdminLevel.SuperAdmin);
            loadJobClasses();
        }

        /// <summary>
        /// Global method that is triggered for any <see cref="ServerAccessor"/> inherited classes once a clients character loads
        /// </summary>
        /// <param name="playerSession"></param>
        public void OnCharacterLoaded(Session.Session playerSession)
        {
            var playerJob = stringToJob(playerSession.GetGlobalData("Character.Job", "Civillian"));
            //if (playerJob == JobType.EMS || playerJob == JobType.Police || playerJob == JobType.Mechanic)
            {
                ACEWrappers.AddPrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.GetLocalData<string>("User.SteamID")}", playerJob);
            }
        }

        /// <summary>
        /// Global method that is triggered for any <see cref="ServerAccessor"/> inherited classes once a client leaves the server
        /// </summary>
        /// <param name="playerSession"></param>
        /// <param name="reason"></param>
        public void OnPlayerDisconnect(Session.Session playerSession, string reason)
        {
            var playerJob = stringToJob(playerSession.GetGlobalData("Character.Job", "Civillian"));
            //if (playerJob == JobType.EMS || playerJob == JobType.Police || playerJob == JobType.Mechanic)
            {
                ACEWrappers.RemovePrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.GetLocalData<string>("User.SteamID")}", playerJob);
            }
        }

        public JobType GetPlayerJob(Session.Session playerSession)
        {
            if (playerSession == null) return JobType.Civillian;

            var tempJob = playerSession.GetGlobalData("Character.Temp.Job", "-1");
            if (tempJob != "-1")
            {
                return stringToJob(tempJob);
            }

            return stringToJob(playerSession.GetGlobalData("Character.Job", "Civillian"));
        }

        public bool OnDutyAs(Session.Session playerSession, JobType job)
        {
            return OnDuty(playerSession) && /*GetPlayerJob(playerSession) == job;*/ job.HasFlag(GetPlayerJob(playerSession));
        }

        public bool OnDuty(Session.Session playerSession)
        {
            return playerSession.GetGlobalData("Character.OnDuty", false);
        }

        public List<Session.Session> GetPlayersOnJob(JobType job)
        {
            var sessionList = new List<Session.Session>();

            foreach (var i in Sessions.PlayerList)
            {
                //if(GetPlayerJob(i) == job && OnDutyAs(i, job))
                if(/*job.HasFlag(GetPlayerJob(i)) &&*/ OnDutyAs(i, job))
                    sessionList.Add(i);
            }

            return sessionList;
        }

        public void SendJobAlert(JobType job, string prefix, string message, Color prefixColour)
        {
            var playersOnJob = GetPlayersOnJob(job);
            playersOnJob.ForEach(player =>
            {
                Log.ToClient(prefix, message, prefixColour, player.Source);
            });
        }

        private void OnSetDutyState([FromSource] Player source, bool dutyState)
        {
            Session.Session playerSession = Server.Instances.Session.GetPlayer(source);
            if (playerSession == null) return;

            playerSession.SetGlobalData("Character.OnDuty", dutyState);
        }

        // Used for remote job setting from client (not really for emergency services however appropiate permission levels can use this way)
        private void OnRemoteJobSet([FromSource] Player source, string jobType)
        {
            var playerSession = Sessions.GetPlayer(source);
            if (playerSession == null) return;

            var newJobType = stringToJob(jobType);
            var previousJobType = stringToJob(playerSession.GetGlobalData("Character.Job", "Civillian"));

            if (newJobType == JobType.EMS || newJobType == JobType.Police)
            {
                if (playerSession.GetLocalData("User.PermissionLevel", AdminLevel.User) >= AdminLevel.Moderator)
                {
                    playerSession.SetGlobalData("Character.Job", newJobType.ToString());
                    ACEWrappers.RemovePrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", previousJobType);
                    ACEWrappers.AddPrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", newJobType);
                    Log.Verbose($"Set the job type of {source.Name} to {newJobType.ToString()}");
                }
                else
                {
                    Log.Verbose($"{source.Name} tried to set their job to {newJobType.ToString()} but didn't have the correct permission level to do so");
                }
                return;
            }

            if (previousJobType == JobType.EMS || previousJobType == JobType.Police)
            {
                playerSession.SetGlobalData("Character.Temp.Job", newJobType.ToString());
                if (newJobType == JobType.Civillian)
                {
                    playerSession.SetGlobalData("Character.Temp.Job", previousJobType.ToString());
                    newJobType = previousJobType;
                }
            }
            else
            {
                playerSession.SetGlobalData("Character.Job", newJobType.ToString());
            }

            ACEWrappers.RemovePrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", previousJobType);
            ACEWrappers.AddPrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", newJobType);
            Log.Verbose($"Set the job type of {source.Name} to {newJobType.ToString()}");
        }

        /// <summary>
        /// Converts a string name of a job to the equivilant <see cref="JobType"/> if it exists
        /// </summary>
        /// <param name="job">Name of the job</param>
        /// <returns></returns>
        public JobType stringToJob(string job)
        {
            var success = Enum.TryParse<JobType>(job, out var result);

            if(!success)
                success = Enum.TryParse(job.ToUpper(), out result);

            if (!success)
                Enum.TryParse(job.ToLower().FirstLetterToUpper(), out result);

            return result;
        }

        /// <summary>
        /// Sets a players job
        /// </summary>
        /// <param name="playerSession">Session of the target player</param>
        /// <param name="jobType">New job of the specified player</param>
        public void SetPlayerJob(Session.Session playerSession, JobType jobType)
        {
            var previousJob = GetPlayerJob(playerSession);

            //if (previousJob == JobType.Police || previousJob == JobType.EMS)
            {
                ACEWrappers.RemovePrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", previousJob);
            }

            //if (jobType == JobType.Police || jobType == JobType.EMS)
            {
                ACEWrappers.AddPrivellagedUser($"{Server.CurrentIdentifier}:{playerSession.SteamIdentifier}", jobType);
            }

            playerSession.SetGlobalData("Character.Job", jobType.ToString());
            Log.Verbose($"Set the job type of {playerSession.PlayerName} to {jobType.ToString()}");
        }

        private void resetPlayerJob(Command cmd)
        {
            var targetPlayer = cmd.GetArgAs<int>(0);
            var playerSession = Sessions.GetPlayer(targetPlayer);

            if (playerSession != null)
            {
                SetPlayerJob(playerSession, JobType.Civillian);
                Log.ToClient("[Job]", "Job permissions revoked", ConstantColours.Job, playerSession.Source);
                Log.ToClient("[Admin]", $"You reset the job permissions of {playerSession.PlayerName}", ConstantColours.Admin, cmd.Player);
                Sessions.TriggerSessionEvent("OnPermissionRefresh", cmd.Session);
            }
            else
            {
                Log.ToClient("[Admin]", "Invalid ID", ConstantColours.Admin, cmd.Player);
            }
        }

        private void OnJobCheck(Command cmd)
        {
            var job = stringToJob(cmd.GetArgAs(0, ""));

            if (job != JobType.Civillian)
            {
                var playersWithJob = GetPlayersOnJob(job);

                Log.Info($"Players on duty as job {job}: {playersWithJob.Count}");
                playersWithJob.ForEach(o =>
                {
                    Log.Info($"{o.PlayerName} - {o.GetGlobalData<string>("Character.FirstName")} {o.GetGlobalData<string>("Character.LastName")}");
                });
            }
        }

        private async void loadJobClasses()
        {
            await BaseScript.Delay(5000);
            foreach (var instance in Assembly.GetExecutingAssembly().GetTypes().Where(o => !o.IsAbstract && o.IsSubclassOf(typeof(JobClass))))
            {
                Server.AddClassInstance(instance, false, true);
            }
        }
    }
}
