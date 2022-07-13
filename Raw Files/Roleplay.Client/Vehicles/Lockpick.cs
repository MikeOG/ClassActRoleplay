using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enums;
using Roleplay.Client.Helpers;
using Roleplay.Client.Vehicles;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Vehicles
{
    internal class Lockpick : ClientAccessor
    {
        private bool isLockpicking;
        private Random rand = new Random((int)DateTime.Now.Ticks);

        public Lockpick(Client client) : base(client)
        {
            client.RegisterEventHandler("Lockpick.StartVehicleLockpick", new Action<int>(StartVehicleLockpick));
        }

        private async void StartVehicleLockpick(int lockpickType)
        {
            try
            {
                if (!isLockpicking)
                {
                    isLockpicking = true;
                    var targetVeh = GTAHelpers.GetVehicleInFrontOfPlayer(3.0f);
                    var closeRegister = GTAHelpers.GetObjectInRange(ObjectHash.prop_till_01);
                    if (targetVeh != null)
                    {
                        Log.ToChat("", "You begin lockpicking the vehicle");
                        var chosenLockpick = (LockpickTypes)lockpickType;
                        await Game.PlayerPed.Task.PlayAnimation("missheistfbisetup1", "unlock_loop_janitor", 2.0f, 2.0f, -1, (AnimationFlags)49, 0);
                        await BaseScript.Delay(chosenLockpick == LockpickTypes.BobbyPin ? 6250 : 5000);
                        var checkVeh = GTAHelpers.GetVehicleInFrontOfPlayer(3.0f);
                        if (targetVeh == checkVeh)
                        {
                            var shouldUnlock = chosenLockpick == LockpickTypes.BobbyPin ? rand.NextBool(30) : rand.NextBool(40) || chosenLockpick == LockpickTypes.SlimJim;
                            if (shouldUnlock)
                            {
                                Log.ToChat("[Robbery]", "You successfully lockpicked the vehicle", ConstantColours.Blue);
                                targetVeh.SetDecor("Vehicle.ID", -1);
                                targetVeh.LockStatus = VehicleLockStatus.Unlocked;
                                Client.TriggerServerEvent("Vehicle.CreateExternalVehicle", VehicleDataPacker.PackVehicleData(targetVeh));
                            }
                            else
                            {
                                Log.ToChat("[Robbery]", "You failed to lockpick the vehicle", ConstantColours.Blue);
                                setVehicleAlarm(targetVeh);
                                var breakPick = chosenLockpick == LockpickTypes.BobbyPin ? rand.NextBool(40) : rand.NextBool(20);
                                if(breakPick)
                                {
                                    string lockpickName = chosenLockpick == LockpickTypes.BobbyPin ? "bobby pin" : "lockpick";
                                    Log.ToChat("[Robbery]", $"Your {lockpickName} broke while trying to get into the vehicle", ConstantColours.Blue);
                                    //BaseScript.TriggerEvent("addInvItem", lockpickName.RemoveSpaces(), -1);
                                    Client.TriggerServerEvent("Inventory.AddInvItem", JsonConvert.SerializeObject(InventoryItems.GetInvItemData(lockpickName.RemoveSpaces())), -1);
                                }
                            }
                        }
                        else
                        {
                            Log.ToChat("", "You moved too far away from the vehicle");
                        }
                    }
                    else if (closeRegister != 0)
                    {
                        Client.Instances.RobberyHandler.AttemptRegisterRobbery();
                    }
                    else
                    {
                        Log.ToChat("", "There is nothing near you that can be lockpicked");
                    }
                    isLockpicking = false;
                    Game.PlayerPed.Task.ClearAll();
                }
            } catch(Exception e) { Log.Error(e); }
        }

        private async void setVehicleAlarm(CitizenFX.Core.Vehicle veh)
        {
            var i = 30;
            while (i > 0)
            {
                veh.SoundHorn(125);
                veh.AreLightsOn = true;
                await BaseScript.Delay(125);
                veh.AreLightsOn = false;
                await BaseScript.Delay(125);
                i--;
            }
            Function.Call(Hash.SET_VEHICLE_LIGHTS, veh.Handle, 0);
        }
    }
}
