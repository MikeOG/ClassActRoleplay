using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Shared;
using Newtonsoft.Json.Linq;

namespace Roleplay.Client.Vehicles
{
    public class VehicleLocks : ClientAccessor
    {
        private Animation lockAnim = new Animation("anim@mp_player_intmenu@key_fob@", "", "fob_click", "", "lol", new AnimationOptions { LoopEnableMovement = true, LoopDuration = 500, EndClearTasks = false});

        public VehicleLocks(Client client) : base(client)
        {
            client.RegisterTickHandler(OnTick);
        }

        [EventHandler("Animation.PlayLockAnim")]
        public async void OnPlayLockAnim()
        {
            await lockAnim.PlayFullAnim();
            TriggerServerEvent("InteractSound_SV: PlayWithinDistance", 5, "carlock", 0.1);
        }

        private async Task OnTick()
        {
            if (Game.IsControlJustReleased(1, Control.PhoneCameraFocusLock))
            {
                var ownedVeh = Client.Get<VehicleHandler>().GetClosestVehicleWithKeys();

                if (ownedVeh != null)
                {
                    await lockAnim.PlayFullAnim();

                    var currentLockStatus = ownedVeh.LockStatus;

                    ownedVeh.LockStatus = currentLockStatus == VehicleLockStatus.Unlocked ? VehicleLockStatus.Locked : VehicleLockStatus.Unlocked;

                    Log.ToChat("", $"Vehicle {ownedVeh.LockStatus.ToString().ToLower()}");

                    TriggerServerEvent("InteractSound_SV:PlayWithinDistance", 6, "carlock", 0.05);
                }
                else
                {
                    //if (LocalSession.GetLocalData("Property.AccessableStorages", new JArray()).ToObject<List<Vector3>>().Count > 0)
                    {
                        Client.TriggerServerEvent("Property.AttemptToggleLocks");
                    }
                }
            }
        }
    }
}
