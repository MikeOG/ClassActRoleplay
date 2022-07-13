using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Shared.Enums;
using Roleplay.Shared;

namespace Roleplay.Client.Player
{
    internal class Stance : ClientAccessor
    {
        private const int MsPerS = 1000;
        private const int ProneToRagDollInvincibleTime = 250;
        private const Control StanceControl = Control.Duck; // ctrl
        private const Control ProneFrontBackControl = Control.Sprint; // shift
        private const Control CancleToIdleControl = Control.Jump; // space

        private int _lastKeyPress;
        private int _lastLeftRightPress;
        private int _lastBodyFlip;
        private int _debounceTime;

        private bool _diveActive;
        private bool _crawlActive;
        private bool _proneAimActive;
        private bool _holdStateToggleActive;

        private bool _isCrouchBlocked;
        private bool _isProneBlocked;

        private ProneStates _proneState;
        private ProneStates _prevProneState;

        private WeaponHash _prevWeapon;

        public StanceStates State { get; private set; }

        /// <summary>
        ///     Initialize class
        /// </summary>
        public Stance(Client client) : base(client)
        {
            try
            {
                API.RequestAnimDict("move_Prone");

                State = StanceStates.Idle;

                client.RegisterTickHandler(OnTick);
                client.RegisterTickHandler(ModifyStance);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Stance OnTick
        /// </summary>
        private async Task OnTick()
        {
            try
            {
                switch (State)
                {
                    case StanceStates.Idle:
                        {
                            ResetAnimations();
                        }
                        break;
                    case StanceStates.Stealth:
                        {
                        }
                        break;
                    case StanceStates.Crouch:
                    {
                            var playerPed = Game.PlayerPed;
                            //if (playerPed.IsInStealthMode)
                                //State = StanceStates.Stealth;

                            const int firstPersonView = 4;
                            if (API.GetFollowPedCamViewMode() == firstPersonView)
                            {
                                API.SetFollowPedCamViewMode(0);
                            }

                            API.DisableFirstPersonCamThisFrame();
                            DisableStealthControl();

                            if (IsCrouchStateCancelled() || _isCrouchBlocked)
                            {
                                
                                State = StanceStates.Idle;
                                break;
                            }

                            API.SetPedMovementClipset(playerPed.Handle, "move_ped_crouched", 1);
                            API.SetPedStrafeClipset(playerPed.Handle, "move_ped_crouched_strafing");
                        }
                        break;
                    case StanceStates.Prone:
                        {
                            DisableStealthControl();

                            if (_diveActive) break;

                            if (IsProneStateCancelled() || _isProneBlocked)
                            {
                                State = StanceStates.Idle;
                                break;
                            }

                            HandleProneStateToggle();
                            HandleProneAim();
                            await HandleProneWeaponChange();

                            if (_proneAimActive) break;

                            await ProneMovement();
                        }
                        break;

                    default:
                        await BaseScript.Delay(MsPerS);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            await BaseScript.Delay(0);
        }

        /// <summary>
        ///     Handle stance changes
        /// </summary>
        private async Task ModifyStance()
        {
            try
            {
                if (LocalSession == null) return;

                var playerPed = Game.PlayerPed;
                var arrestInstance = Client.Get<Arrest>();
                if (arrestInstance == null) return;
                
                if (arrestInstance.CurrentCuffState != CuffState.None || API.IsPedUsingAnyScenario(playerPed.Handle) ||
                    API.IsEntityPlayingAnim(playerPed.Handle, "mp_arresting", "idle", 3) ||
                    API.IsEntityPlayingAnim(playerPed.Handle, "random@mugging3", "handsup_standing_base", 3) ||
                    API.IsEntityPlayingAnim(playerPed.Handle, "random@arrests@busted", "idle_a", 3) ||
                    Game.IsControlJustPressed(2, CancleToIdleControl) ||
                    playerPed.IsInVehicle() ||
                    playerPed.IsInWater || playerPed.IsSwimming || playerPed.IsSwimmingUnderWater ||
                    playerPed.VehicleTryingToEnter != null)
                {
                    if (State == StanceStates.Crouch)
                    {
                        CancelCrouch();
                        State = StanceStates.Idle;
                    }
                    else if (State == StanceStates.Prone)
                    {
                        await AdvanceState();
                    }
                    return;
                }

                if (Game.IsControlJustPressed(2, StanceControl))
                {
                    _holdStateToggleActive = false;
                    _lastKeyPress = Game.GameTime;
                }
                else if (Game.IsControlPressed(2, StanceControl))
                {
                    if (!_isProneBlocked)
                    {
                        if (_lastKeyPress < Game.GameTime - 200)
                        {
                            _holdStateToggleActive = true;
                            if (State == StanceStates.Idle || State == StanceStates.Stealth ||
                                State == StanceStates.Crouch)
                                await TransitionToProneState();
                        }
                    }
                }
                else if (Game.IsControlJustReleased(0, StanceControl))
                {
                    if (_lastKeyPress >= Game.GameTime - 10) return;
                    _lastKeyPress = Game.GameTime;
                    if (!_holdStateToggleActive)
                        await AdvanceState();
                }
                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void CancelCrouch()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                API.ResetPedMovementClipset(playerPed.Handle, 1);
                API.ResetPedStrafeClipset(playerPed.Handle);
                //InteractionListMenu.SetWalkingStyle();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Handle state changes
        /// </summary>
        private async Task AdvanceState()
        {
            try
            {
                Game.PlayerPed.Task.ClearAll();
                CancelCrouch();
                switch (State)
                {
                    case StanceStates.Idle:
                        State = StanceStates.Stealth;
                        break;
                    case StanceStates.Stealth:
                        API.RequestClipSet("move_ped_crouched");
                        while (!API.HasClipSetLoaded("move_ped_crouched"))
                        {
                            API.RequestClipSet("move_ped_crouched");
                            await BaseScript.Delay(0);
                        }
                            
                        if (_isCrouchBlocked)
                        {
                            State = StanceStates.Idle;
                            return;
                        }
                        State = StanceStates.Crouch;
                        break;
                    case StanceStates.Crouch:
                        if (_isProneBlocked)
                        {
                            State = StanceStates.Idle;
                            return;
                        }
                        await TransitionToProneState();
                        break;
                    case StanceStates.Prone:
                        TransitionProneToIdle();
                        State = StanceStates.Idle;
                        break;
                    default:
                        Log.Error("Entered unused default stance state.");
                        break;
                }
                await Task.FromResult(0);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Handle player movement inputs in prone state
        /// </summary>
        private async Task ProneMovement()
        {
            try
            {
                if (Game.IsControlJustPressed(2, Control.MoveDownOnly) ||
                    Game.IsControlJustPressed(2, Control.MoveUpOnly))
                {
                    _debounceTime = 100;
                    _lastKeyPress = Game.GameTime;

                    if (!_crawlActive)
                        HandleCrawlMovement(Game.IsControlJustPressed(2, Control.MoveUpOnly)
                            ? Movements.Forward
                            : Movements.Backward);
                }
                else if (Game.IsControlPressed(2, Control.MoveDownOnly) || Game.IsControlPressed(2, Control.MoveUpOnly))
                {
                    if (_lastKeyPress >= Game.GameTime - _debounceTime) return;
                    {
                        _lastKeyPress = Game.GameTime;
                        _debounceTime = 10;

                        if (!_crawlActive)
                            HandleCrawlMovement(Game.IsControlPressed(2, Control.MoveUpOnly)
                                ? Movements.Forward
                                : Movements.Backward);
                    }
                }
                if (Game.IsControlJustPressed(2, Control.MoveLeftOnly) ||
                    Game.IsControlJustPressed(2, Control.MoveRightOnly))
                {
                    _debounceTime = 100;
                    _lastLeftRightPress = Game.GameTime;

                    Game.PlayerPed.Heading = Game.IsControlJustPressed(2, Control.MoveLeftOnly)
                        ? Game.PlayerPed.Heading + 2f
                        : Game.PlayerPed.Heading - 2f;
                }
                else if (Game.IsControlPressed(2, Control.MoveLeftOnly) ||
                         Game.IsControlPressed(2, Control.MoveRightOnly))
                {
                    if (_lastLeftRightPress >= Game.GameTime - _debounceTime) return;
                    {
                        _lastLeftRightPress = Game.GameTime;
                        _debounceTime = 10;

                        Game.PlayerPed.Heading = Game.IsControlPressed(2, Control.MoveLeftOnly)
                            ? Game.PlayerPed.Heading + 0.75f
                            : Game.PlayerPed.Heading - 0.75f;
                    }
                }
                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Trigger player prone animation
        /// </summary>
        /// <param name="proneState">on front or on back prone state</param>
        private void GoProne(ProneStates proneState)
        {
            try
            {
                var playerPed = Game.PlayerPed;
                if (_proneState != _prevProneState)
                {
                    playerPed.Heading = playerPed.Heading + 180f;
                    _prevProneState = _proneState;
                }

                var animName = proneState == ProneStates.OnFront ? "onfront_fwd" : "onback_fwd";
                var position = Game.PlayerPed.Position;
                var rotation = Game.PlayerPed.Rotation;
                const float animStartTime = 1000f;
                const int animFlags = (int)AnimationFlags.StayInEndFrame;
                API.TaskPlayAnimAdvanced(playerPed.Handle, "move_crawl", animName,
                    position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, 8f, -8f, -1, animFlags,
                    animStartTime, 2,
                    0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Transition to prone state
        /// </summary>
        private async Task TransitionToProneState()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                State = StanceStates.Prone;
                _proneAimActive = false;
                _proneState = ProneStates.OnFront;
                _prevProneState = ProneStates.OnFront;
                _prevWeapon = Game.PlayerPed.Weapons.Current.Hash;
                if (playerPed.IsRunning || playerPed.IsSprinting)
                {
                    playerPed.Task.ClearAll();
                    _diveActive = true;
                    await playerPed.Task.PlayAnimation("move_jump", "dive_start_run", 8f, -8f, -1,
                        AnimationFlags.RagdollOnCollision, 0);
                    //This is the ideal delay to make animation look good.  Do not change.
                    await BaseScript.Delay(1100);
                    _diveActive = false;
                }
                if (playerPed.IsRagdoll)
                {
                    State = StanceStates.Idle;
                }
                else
                {
                    if (API.IsPedArmed(playerPed.Handle, 4))
                        API.TaskAimGunScripted(playerPed.Handle,
                            (uint)API.GetHashKey("SCRIPTED_GUN_TASK_PLANE_WING"), true, true);
                    else
                        await playerPed.Task.PlayAnimation("move_crawl", "onfront_fwd", 8f, -8f, -1,
                            (AnimationFlags)2, 0);
                }
                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Transition from prone to idle
        /// </summary>
        private static async void TransitionProneToIdle()
        {
            try
            {
                //Going ragdoll while prone has a small chance of inflicting dmg to ped, this should prevent that
                Game.PlayerPed.IsInvincible = true;
                Game.PlayerPed.Ragdoll(1);
                await BaseScript.Delay(ProneToRagDollInvincibleTime);
                Game.PlayerPed.IsInvincible = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Handle player crawling movement
        /// </summary>
        /// <param name="movementDirection">fwd or bwd movement</param>
        private async void HandleCrawlMovement(Movements movementDirection)
        {
            try
            {
                if (_crawlActive) return;

                _crawlActive = true;
                Crawl(movementDirection, _proneState);
                await BaseScript.Delay(850);
                _crawlActive = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _crawlActive = false;
            }
        }

        /// <summary>
        ///     Trigger player crawling animation
        /// </summary>
        /// <param name="movementDirection">fwd or bwd movement</param>
        /// <param name="proneState">on front or on back prone state</param>
        private async void Crawl(Movements movementDirection, ProneStates proneState)
        {
            try
            {
                var proneStateStr = proneState == ProneStates.OnFront ? "onfront" : "onback";
                string movementStr;
                if (proneState == ProneStates.OnFront)
                    movementStr = movementDirection == Movements.Forward ? "fwd" : "bwd";
                else
                    movementStr = movementDirection == Movements.Forward ? "bwd" : "fwd";
                var animStr = string.Concat(proneStateStr, "_", movementStr);
                Game.PlayerPed.Task.ClearAnimation("move_crawl", animStr);
                await Game.PlayerPed.Task.PlayAnimation("move_crawl", animStr, 8f, -8f, -1,
                    (AnimationFlags)2, 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            await Task.FromResult(0);
        }

        /// <summary>
        ///     Handle prone front/back toggle
        /// </summary>
        private void HandleProneStateToggle()
        {
            try
            {
                if (!Game.IsControlJustPressed(0, ProneFrontBackControl)) return;

                if (_lastBodyFlip >= Game.GameTime - 1000) return;

                _lastBodyFlip = Game.GameTime;

                _proneState = _proneState == ProneStates.OnFront ? ProneStates.OnBack : ProneStates.OnFront;
                GoProne(_proneState);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Handle prone weapon changes
        /// </summary>
        private async Task HandleProneWeaponChange()
        {
            try
            {
                var currentWeapon = Game.PlayerPed.Weapons.Current.Hash;
                if (_prevWeapon != currentWeapon)
                {
                    _prevWeapon = currentWeapon;
                    //Prevents player from standing up when they change weapons
                    var proneState = _proneState == ProneStates.OnBack ? ProneStates.OnBack : ProneStates.OnFront;
                    GoProne(proneState);
                    //Simulate time pulling a weapon out while lying down
                    await BaseScript.Delay(1000);
                }
                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Handle prone aiming
        /// </summary>
        private void HandleProneAim()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                var playerIsArmed = API.IsPedArmed(playerPed.Handle, 4);
                if (playerIsArmed && !_crawlActive && !_proneAimActive && Game.IsControlPressed(2, Control.Aim))
                {
                    API.TaskAimGunScripted(playerPed.Handle,
                        (uint)API.GetHashKey("SCRIPTED_GUN_TASK_PLANE_WING"), true, true);

                    if (!_proneAimActive && _proneState == ProneStates.OnBack)
                        Game.PlayerPed.Rotation = new Vector3(Game.PlayerPed.Rotation.X,
                            Game.PlayerPed.Rotation.Y, Game.PlayerPed.Heading - 180f);

                    _proneAimActive = true;

                    //if (IsUsingWeaponWithScope())
                    //{
                    //TODO: Sniper overlay will not occur w/ "SCRIPTED_GUN_TASK_PLANE_WING" no matter what.
                    //        API.DisplaySniperScopeThisFrame();
                    //        Function.Call<bool>(Hash.DISPLAY_SNIPER_SCOPE_THIS_FRAME);
                    //}
                }
                else if (playerIsArmed && Game.IsControlJustReleased(2, Control.Aim))
                {
                    if (_proneState == ProneStates.OnBack)
                    {
                        Game.PlayerPed.Rotation = new Vector3(Game.PlayerPed.Rotation.X,
                            Game.PlayerPed.Rotation.Y, Game.PlayerPed.Heading - 180f);
                        GoProne(ProneStates.OnBack);
                    }
                    _proneAimActive = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Returns if a player is currenty in the prone position
        /// </summary>
        public bool IsPlayerProne()
        {
            try
            {
                return State == StanceStates.Prone;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Return (and handle) whether or not player has cancelled Crouch state
        /// </summary>
        private static bool IsCrouchStateCancelled()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                if (!playerPed.IsRagdoll && !playerPed.IsInMeleeCombat) return false;

                Game.PlayerPed.Task.ClearAll();
                API.ResetPedMovementClipset(playerPed.Handle, 1);
                API.ResetPedStrafeClipset(playerPed.Handle);
                //InteractionListMenu.SetWalkingStyle();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Return (and handle) whether or not player has cancelled prone state
        /// </summary>
        private bool IsProneStateCancelled()
        {
            try
            {
                if (Game.PlayerPed.IsInMeleeCombat)
                    Game.PlayerPed.Ragdoll(1);

                if (!Game.PlayerPed.IsRagdoll) return false;

                Game.PlayerPed.Task.ClearAll();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Return whether or not player is using sniper rifle
        /// </summary>
        private bool IsUsingWeaponWithScope()
        {
            try
            {
                var currentWeapon = Game.PlayerPed.Weapons.Current.Hash;
                return currentWeapon == WeaponHash.SniperRifle || currentWeapon == WeaponHash.HeavySniper ||
                       currentWeapon == WeaponHash.HeavySniperMk2 || currentWeapon == WeaponHash.MarksmanRifle;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Disable stealh control (ctrl key)
        /// </summary>
        private void DisableStealthControl()
        {
            try
            {
                Game.DisableControlThisFrame(0, Control.Duck);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Reset all stance animations
        /// </summary>
        private void ResetAnimations()
        {
            try
            {
                var playerPed = Game.PlayerPed;
                if (API.IsEntityPlayingAnim(playerPed.Handle, "move_jump", "dive_start_run", 3))
                    Game.PlayerPed.Task.ClearAnimation("move_jump", "dive_start_run");

                var animationList = new[] { "onfront_fwd", "onfront_bwd", "onback_fwd", "onback_bwd" };

                foreach (var animation in animationList)
                    if (API.IsEntityPlayingAnim(playerPed.Handle, "move_crawl", animation, 3))
                        Game.PlayerPed.Task.ClearAnimation("move_crawl", animation);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void HandleBlockingEventWrapper(bool blockCrouch, bool blockProne)
        {
            try
            {
                _isCrouchBlocked = blockCrouch;
                _isProneBlocked = blockProne;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal enum StanceStates
        {
            Idle,
            Stealth,
            Crouch,
            Prone
        }

        private enum Movements
        {
            Forward,
            Backward
        }

        private enum ProneStates
        {
            OnFront,
            OnBack
        }
    }
}
