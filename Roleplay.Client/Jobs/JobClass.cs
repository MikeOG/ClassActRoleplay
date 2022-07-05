using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Client.Interfaces;
using Roleplay.Client.Session;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs
{
    public abstract class JobClass : IJob
    {
        protected Vehicle JobVehicle;
        protected Client Client => Client.Instance;

        private SessionManager sessionManager;
        public SessionManager Sessions => sessionManager ?? (sessionManager = Client.Get<SessionManager>());

        private JobHandler jobHandler;
        public JobHandler JobHandler => jobHandler ?? (jobHandler = Client.Get<JobHandler>());

        public virtual void StartJob()
        {
            Client.Instance.RegisterTickHandler(CheckVehicleTick);
        }

        public virtual void EndJob()
        {
            Client.Instance.DeregisterTickHandler(CheckVehicleTick);
        }

        public virtual void GiveJobPayment(){ }

        public virtual async Task JobTick(){ }

        protected virtual async Task<Vehicle> CreateJobVehicle(VehicleHash hash, Vector3 location, float heading)
        {
            var vehModel = new Model(hash);
            while (!vehModel.IsLoaded)
                await vehModel.Request(0);

            JobVehicle = await World.CreateVehicle(vehModel, location, heading);
            JobVehicle.PlaceOnGround();
            JobVehicle.LockStatus = VehicleLockStatus.Unlocked;
            JobVehicle.IsPersistent = true;
            JobVehicle.Mods.Livery = 1;
            Game.PlayerPed.Task.WarpIntoVehicle(JobVehicle, VehicleSeat.Driver);
            Client.Instance.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(JobVehicle), true);
            vehModel.MarkAsNoLongerNeeded();

            return JobVehicle;
        }

        protected virtual async void RemoveJobVehicle()
        {
            if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle == JobVehicle)
            {
                Game.PlayerPed.Task.LeaveVehicle(JobVehicle, true);
                await BaseScript.Delay(3000);
            }
            JobVehicle.Delete();
            JobVehicle = null;
        }

        protected virtual async Task CheckVehicleTick()
        {
            if (JobVehicle == null) return;

            if (JobVehicle.IsDead)
            {
                Log.ToChat("[Job]", "Your job vehicle was destroyed", ConstantColours.Job);
                JobVehicle = null;
                EndJob();
                Client.Instance.DeregisterTickHandler(CheckVehicleTick);
            }
        }
    }
}
