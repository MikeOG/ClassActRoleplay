using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Interfaces;
using Roleplay.Client.Session;
using Roleplay.Shared;
using Roleplay.Shared.Enums;

namespace Roleplay.Client.Jobs
{
    public sealed class JobHandler : ClientAccessor
    {
        public bool IsOnDuty => LocalSession.GetGlobalData("Character.OnDuty", false);

        public JobHandler(Client client) : base(client)
        {
            loadJobClasses();
        }

        public JobType GetPlayerJob(Session.Session targetSession = null)
        {
            var playerSession = targetSession ?? LocalSession;

            if (playerSession == null) return JobType.Civillian;

            var tempJob = playerSession.GetGlobalData("Character.Temp.Job", "-1");
            if (tempJob != "-1")
            {
                return stringToJob(tempJob);
            }

            return stringToJob(playerSession.GetGlobalData("Character.Job", "Civillian"));
        }

        public void SetPlayerJob(JobType job)
        {
            Roleplay.Client.Client.Instance.TriggerServerEvent("Job.SetPlayerJob", job.ToString());
        }

        /// <summary>
        /// Converts a string name of a job to the equivilant <see cref="JobType"/> if it exists
        /// </summary>
        /// <param name="job">Name of the job</param>
        /// <returns></returns>
        public JobType stringToJob(string job)
        {
            Enum.TryParse<JobType>(job, out var result);

            return result;
        }

        public void SetDutyState(bool state) => Roleplay.Client.Client.Instance.TriggerServerEvent("Job.SetDutyState", state);

        public bool OnDutyAsJob(JobType job, Session.Session targetSession = null) => IsOnDuty && job.ToString().Contains(GetPlayerJob(targetSession ?? LocalSession).ToString());

        private async void loadJobClasses()
        {
            await BaseScript.Delay(5000);
            foreach (var instance in Assembly.GetExecutingAssembly().GetTypes().Where(o => !o.IsAbstract && o.IsSubclassOf(typeof(JobClass))))
            {
                /*var obj = Activator.CreateInstance(instance);
                ((IDictionary<string, dynamic>)JobClasses)[instance.Name] = Convert.ChangeType(obj, instance);
                Log.Debug($"Created instance of job class {instance.Name}");*/
                Client.AddClassInstance(instance, false, true);
                await BaseScript.Delay(0);
            }
        }
    }
}
