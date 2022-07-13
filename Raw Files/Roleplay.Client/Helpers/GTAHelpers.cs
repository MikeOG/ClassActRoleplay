using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core.UI;
using Roleplay.Client.Enums;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Shared;
using Roleplay.Shared.Enums;

namespace Roleplay.Client.Helpers
{
    public static class GTAHelpers
    {
        public static async Task<bool> IsInFrontOfWater(this Ped PlayerPed)
        {
            Vector3 ProbeLocation = PlayerPed.GetOffsetPosition(new Vector3(0, 8, 0));

            OutputArgument z = new OutputArgument();
            bool groundFound = Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, ProbeLocation.X, ProbeLocation.Y, ProbeLocation.Z, z, false);
            float groundZ = z.GetResult<float>();
            ProbeLocation.Z = (float)groundZ - 0.1f;
            Ped ProbePed = await CitizenFX.Core.World.CreatePed(new Model(PedHash.Rat), ProbeLocation);
            ProbePed.PositionNoOffset = ProbeLocation;
            ProbePed.Opacity = 0;
            await BaseScript.Delay(50);
            bool isProbeInWater = Function.Call<bool>(Hash.IS_ENTITY_IN_WATER, ProbePed.Handle);
            ProbePed.Delete();
            bool isPlayerInWater = Function.Call<bool>(Hash.IS_ENTITY_IN_WATER, PlayerPed.Handle);
            return isPlayerInWater || isProbeInWater;
        }

        public static Vector3 CalculateClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
        {
            try
            {
                float dotProduct = Vector3.Dot(start - end, point - start);
                float percent = dotProduct / (start - end).LengthSquared();
                if (percent < 0.0f) return start;
                else if (percent > 1.0f) return end;
                else return (start + (percent * (end - start)));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(Vector3);
        }

        public static CitizenFX.Core.Vehicle GetVehicleInFrontOfPlayer(float distance = 5f)
        {
            try
            {
                var source = Game.PlayerPed.IsInVehicle() ? (Entity)Game.PlayerPed.CurrentVehicle : Game.PlayerPed;
                return GetVehicleInFrontOfPlayer(source, source, distance);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(CitizenFX.Core.Vehicle);
        }

        public static CitizenFX.Core.Vehicle GetVehicleInFrontOfPlayer(Entity source, Entity ignore, float distance = 5f)
        {
            try
            {
                RaycastResult raycast = CitizenFX.Core.World.Raycast(source.Position + new Vector3(0f, 0f, -0.4f), source.GetOffsetPosition(new Vector3(0f, distance, 0f)) + new Vector3(0f, 0f, -0.4f), (IntersectOptions)71, ignore);
                if (raycast.DitHitEntity && raycast.HitEntity.Model.IsVehicle)
                {
                    return (CitizenFX.Core.Vehicle)raycast.HitEntity;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(CitizenFX.Core.Vehicle);
        }

        public static Vector3 GameplayCamForwardVector()
        {
            try
            {
                Vector3 rotation = (float)(Math.PI / 180.0) * Function.Call<Vector3>(Hash._GET_GAMEPLAY_CAM_ROT_2, 2);
                return Vector3.Normalize(new Vector3((float)-Math.Sin(rotation.Z) * (float)Math.Abs(Math.Cos(rotation.X)), (float)Math.Cos(rotation.Z) * (float)Math.Abs(Math.Cos(rotation.X)), (float)Math.Sin(rotation.X)));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(Vector3);
        }

        public static CitizenFX.Core.Player GetClosestPlayer(float distance = 3.0f)
        {
            var playerPed = Game.PlayerPed;
            return Client.Instance.PlayerList.FirstOrDefault(o => o != Game.Player && o.Character.Position.DistanceToSquared(playerPed.Position) < distance);
        }


        public static RaycastResult _CrosshairRaycast()
        {
            try
            {
                return World.Raycast(CitizenFX.Core.Game.PlayerPed.Position, CitizenFX.Core.Game.PlayerPed.Position + 1000 * GameplayCamForwardVector(), IntersectOptions.Everything, CitizenFX.Core.Game.PlayerPed);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(RaycastResult);
        }

        public struct _RaycastResult
        {
            public Entity HitEntity { get; }
            public Vector3 HitPosition { get; }
            public Vector3 SurfaceNormal { get; }
            public bool DitHit { get; }
            public bool DitHitEntity { get; }
            public int Result { get; }

            public _RaycastResult(bool DitHit, Vector3 HitPosition, Vector3 SurfaceNormal, int entityHandle, int Result)
            {
                this.DitHit = DitHit;
                this.HitPosition = HitPosition;
                this.SurfaceNormal = SurfaceNormal;
                if (entityHandle == 0)
                {
                    this.HitEntity = null;
                    this.DitHitEntity = false;
                }
                else
                {
                    this.HitEntity = Entity.FromHandle(entityHandle);
                    this.DitHitEntity = true;
                }
                this.Result = Result;
            }
        }


        public static _RaycastResult CrosshairRaycast(float distance = 1000f)
        {
            return CrosshairRaycast(CitizenFX.Core.Game.PlayerPed);
        }

        /// <summary>
        /// Because HitPosition and SurfaceNormal are currently broken in platform function
        /// </summary>
        /// <returns></returns>
        public static _RaycastResult CrosshairRaycast(Entity ignore, float distance = 1000f)
        {
            try
            {
                // Uncomment these to potentially save on raycasts (don't think they're ridiculously costly, but there's a # limit per tick)
                //if(CrosshairRaycastThisTick != null && distance == 1000f) return (_RaycastResult) CrosshairRaycastThisTick;

                Vector3 start = CitizenFX.Core.GameplayCamera.Position;
                Vector3 end = CitizenFX.Core.GameplayCamera.Position + distance * GameplayCamForwardVector();
                int raycastHandle = Function.Call<int>(Hash._START_SHAPE_TEST_RAY, start.X, start.Y, start.Z, end.X, end.Y, end.Z, IntersectOptions.Everything, ignore.Handle, 0);
                OutputArgument DitHit = new OutputArgument();
                OutputArgument HitPosition = new OutputArgument();
                OutputArgument SurfaceNormal = new OutputArgument();
                OutputArgument HitEntity = new OutputArgument();
                Function.Call<int>(Hash.GET_SHAPE_TEST_RESULT, raycastHandle, DitHit, HitPosition, SurfaceNormal, HitEntity);

                var result = new _RaycastResult(DitHit.GetResult<bool>(), HitPosition.GetResult<Vector3>(), SurfaceNormal.GetResult<Vector3>(), HitEntity.GetResult<int>(), raycastHandle);

                //if(distance == 1000f) CrosshairRaycastThisTick = result;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default(_RaycastResult);
        }

        public static string GetEntityType(int entityHandle)
        {
            try
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_A_PED, entityHandle))
                    return "PED";
                else if (Function.Call<bool>(Hash.IS_ENTITY_A_VEHICLE, entityHandle))
                    return "VEH";
                else if (Function.Call<bool>(Hash.IS_ENTITY_AN_OBJECT, entityHandle))
                    return "OBJ";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return "UNK";
        }

        public static async Task<int> PlaySound(this Entity entity, string sound)
        {
            var soundId = Function.Call<int>(Hash.GET_SOUND_ID);
            while (soundId == -1)
            {
                soundId = Function.Call<int>(Hash.GET_SOUND_ID);
                await BaseScript.Delay(0);
            }

            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, soundId, sound, entity.Handle, 0, 0, 0);

            return soundId;
        }

        public static CitizenFX.Core.Vehicle GetClosestVehicle(float distance = 3.0f, Func<Vehicle, bool> customFindFunc = null)
        {
            if (Cache.PlayerPed.IsInVehicle())
                return Cache.PlayerPed.CurrentVehicle;

            var closeVeh = GetVehicleInFrontOfPlayer(distance);
            var playerPed = Cache.PlayerPed;

            bool distanceCheckFunc(Vehicle veh) => veh.Position.DistanceToSquared(playerPed.Position) < distance;

            Func<Vehicle, bool> findFunc = veh => playerPed.Exists() && distanceCheckFunc(veh);

            if (customFindFunc != null)
                findFunc = customFindFunc;

            if (closeVeh == null)
                closeVeh = new VehicleList().Select(o => new CitizenFX.Core.Vehicle(o)).FirstOrDefault(o => findFunc(o) && distanceCheckFunc(o));

            return closeVeh;
        }

        public static int GetObjectInRange(ObjectHash objectHash, float distance = 1.0f)
        {
            var playerPos = Game.PlayerPed.Position;
            var obj = API.GetClosestObjectOfType(playerPos.X, playerPos.Y, playerPos.Z, distance, unchecked((uint)objectHash), false, false, false);
            return obj;
        }

        public static async Task TeleportToLocation(this Entity entity, Vector3 location, bool transition = false)
        {
            if(transition)
            {
                Screen.Fading.FadeOut(1000);

                while (Screen.Fading.IsFadingOut)
                    await BaseScript.Delay(0);
            }

            API.RequestCollisionAtCoord(location.X, location.Y, location.Z);
            entity.PositionNoOffset = location;
            entity.IsPositionFrozen = true;

            while (!API.HasCollisionLoadedAroundEntity(entity.Handle))
            {
                await BaseScript.Delay(0);
                entity.IsPositionFrozen = true;
                entity.PositionNoOffset = location;
            }

            /*do
            {
                entity.PositionNoOffset = location;
                entity.IsPositionFrozen = true;
                await BaseScript.Delay(0);
            } while (entity.Position != location);*/

            entity.IsPositionFrozen = false;

            if (transition)
            {
                Screen.Fading.FadeIn(1000);
                while (Screen.Fading.IsFadingIn)
                    await BaseScript.Delay(0);
            }
        }

        public static bool IsDoingAction(this Ped playerPed)
        {
            return API.IsPedUsingAnyScenario(playerPed.Handle) || API.IsEntityPlayingAnim(playerPed.Handle, "mp_arresting", "idle", 3) ||
                   API.IsEntityPlayingAnim(playerPed.Handle, "random@mugging3", "handsup_standing_base", 3) ||
                   API.IsEntityPlayingAnim(playerPed.Handle, "random@arrests@busted", "idle_a", 3) ||
                   playerPed.IsInVehicle() ||
                   playerPed.IsInWater || playerPed.IsSwimming || playerPed.IsSwimmingUnderWater ||
                   playerPed.VehicleTryingToEnter != null;
        }

        public static Ped GetCloestPed(float distance = 3.0f, Func<Ped, bool> customFindFunc = null)
        {
            var pedList = new PedList();

            if (customFindFunc == null)
            {
                return pedList.Select(o => new Ped(o)).FirstOrDefault(o => o.Position.DistanceToSquared(Game.PlayerPed.Position) < distance);
            }
            else
            {
                return pedList.Select(o => new Ped(o)).FirstOrDefault(customFindFunc);
            }
        }

        public static async Task<bool> PlayReportAnim(Ped ped)
        {
            ped.Task.ClearAll();
            TaskStartScenarioInPlace(ped.Handle, "WORLD_HUMAN_STAND_MOBILE", 0, true);
            await BaseScript.Delay(3000);
            return IsPedUsingScenario(ped.Handle, "WORLD_HUMAN_STAND_MOBILE");
        }

        public static string GetLocationString()
        {
            var playerPos = Game.PlayerPed.Position;
            uint streetName = 0;
            uint crossingRoad = 0;
            GetStreetNameAtCoord(playerPos.X, playerPos.Y, playerPos.Z, ref streetName, ref crossingRoad);

            return $"{GetStreetNameFromHashKey(streetName)} / {World.GetZoneLocalizedName(playerPos)}";
        }
    }
}
