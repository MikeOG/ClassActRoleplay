using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Enums;
using Roleplay.Client.Player.Controls;
using Roleplay.Shared;

namespace Roleplay.Client.Vehicles
{
    class ClosestDoorEnter : ClientAccessor
    {
        /*private Dictionary<VehicleDoorIndex, string> doorIndexBones = new Dictionary<VehicleDoorIndex, string>
        {
            {VehicleDoorIndex.FrontLeftDoor, "door_dside_f"},
            {VehicleDoorIndex.FrontRightDoor, "door_pside_f"},
            {VehicleDoorIndex.BackLeftDoor, "door_dside_r"},
            {VehicleDoorIndex.BackRightDoor, "door_pside_r"},
        };*/

        private Dictionary<VehicleDoorIndex, string> doorIndexBones = new Dictionary<VehicleDoorIndex, string>
        {
            {VehicleDoorIndex.FrontLeftDoor, "handle_dside_f"},
            {VehicleDoorIndex.FrontRightDoor, "handle_pside_f"},
            {VehicleDoorIndex.BackLeftDoor, "handle_dside_r"},
            {VehicleDoorIndex.BackRightDoor, "handle_pside_r"},
        };

        private Dictionary<VehicleDoorIndex, VehicleSeat> doorToSeat = new Dictionary<VehicleDoorIndex, VehicleSeat>
        {
            {VehicleDoorIndex.FrontLeftDoor, VehicleSeat.Driver},
            {VehicleDoorIndex.FrontRightDoor, VehicleSeat.Passenger},
            {VehicleDoorIndex.BackLeftDoor, VehicleSeat.LeftRear},
            {VehicleDoorIndex.BackRightDoor, VehicleSeat.RightRear},
        };

        public ClosestDoorEnter(Client client) : base(client)
        {
        
        }

        [EventHandler("baseevents:enteringVehicle")]
        private async void OnEnterVehicle(int vehHandle, int seat)
        {
            var veh = new Vehicle(vehHandle);

            if (await Input.WaitForKeyRelease(Control.VehicleExit, ControlModifier.None, 500))
            {
                Log.Debug($"Enter vehicle key was long pressed. Aborting closest door enter");
                return;
            }

            if (veh.Handle != 0 && doorToSeat.Values.Contains((VehicleSeat)seat))
            {
                Log.Verbose($"Entering vehicle with handle {vehHandle}");

                var playerPos = Cache.PlayerPed.Position;
                var vehDoors = veh.Doors.GetAll();

                var closestDoorDistance = 10000.0f;
                VehicleDoor closestDoor = null;

                foreach (var door in vehDoors)
                {
                    if (doorIndexBones.ContainsKey(door.Index))
                    {
                        var doorBone = doorIndexBones[door.Index];
                        var bonePos = veh.Bones[doorBone].Position;
                        var distToBone = playerPos.DistanceToSquared(bonePos);

                        if (distToBone < closestDoorDistance)
                        {
                            closestDoorDistance = distToBone;
                            closestDoor = door;
                        }
                    }
                }

                if (closestDoor != null)
                {
                    Log.Verbose($"Found a close door ({closestDoor.Index})");

                    if (!veh.IsSeatFree(doorToSeat[closestDoor.Index])) return;

                    Log.Verbose($"Close door ({closestDoor.Index}) was free. Entering");

                    Cache.PlayerPed.Task.ClearAll();

                    Cache.PlayerPed.Task.EnterVehicle(veh, doorToSeat[closestDoor.Index]);
                }
            }
        }
    }
}
