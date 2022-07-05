using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Roleplay.Client.Enums;
using Roleplay.Client.Enviroment;
using Roleplay.Client.Jobs.EmergencyServices.Police;
using Roleplay.Shared.Enums;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Locations;

namespace Roleplay.Client.Jobs.Criminal.Robberies
{
    public class SafeCracking : ClientAccessor
    {
        public enum RotationDirections
        {
            Idle,
            Anticlockwise,
            Clockwise
        }

        public enum SafeCrackingResults
        {
            Failed = -1,
            Running,
            Success
        }

        private SafeCrackingStates _state;

        private Random _rand;

        private int _initPlayerHealth;

        private float _spriteX = 0.48f;
        private float _spriteY = 0.30f;
        private float _aspectRatio;

        private Vector3 _animPosition = new Vector3(0, 0, 0);
        private float _animHeading;

        private int _lastKeyPress;
        private int _debounceTime = 100;

        private bool _initRelockCountdown;

        private float _safeDoorClosedHeading;

        private RotationDirections _initDialRotationDirection;
        private RotationDirections _currentDialRotationDirection;
        private RotationDirections _lastDialRotationDirection;
        private RotationDirections _requiredDialRotationDirection;

        private Entity _safeDoor;
        private List<int> _safeCombination;
        private List<bool> _safeLockStatus;
        private int _currentLockNum;
        private float _safeDialRotation;

        private float SafeDialRotation
        {
            get => _safeDialRotation;
            set
            {
                _safeDialRotation = value;

                if (_safeDialRotation < 0) _safeDialRotation += 360f;
                else if (_safeDialRotation > 356.4f) _safeDialRotation -= 360f;
            }
        }

        public SafeCracking(Client client) : base(client)
        {
            client.RegisterTickHandler(LoadResources);
        }

        private async Task LoadResources()
        {
            try
            {
                API.RequestStreamedTextureDict("MPSafeCracking", false);
                API.RequestAnimDict("mini@safe_cracking");

                //Verify everything is loaded correctly
                var loadAttempt = 0;
                while (!API.HasStreamedTextureDictLoaded("MPSafeCracking") ||
                       !API.RequestAmbientAudioBank("SAFE_CRACK", false) ||
                       !API.HasAnimDictLoaded("mini@safe_cracking"))
                {
                    await BaseScript.Delay(5);
                    if (++loadAttempt < 50000) continue;

                    if (!API.HasStreamedTextureDictLoaded("MPSafeCracking"))
                        Log.Error("Failed to load MPSafeCracking.");
                    if (!API.RequestAmbientAudioBank("SAFE_CRACK", false))
                        Log.Error("Failed to load ambientAudioBank.");
                    if (!API.HasAnimDictLoaded("mini@safe_cracking"))
                        Log.Error("Failed to load mini@safe_cracking.");
                    break;
                }
                _rand = new Random();
                _aspectRatio = API.GetAspectRatio(true);
                _safeDoor = Entity.FromHandle(0);
                await Task.FromResult(0);
                Client.DeregisterTickHandler(LoadResources);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Initialize current safe
        /// </summary>
        internal bool InitializeSafe(int safeHandle, int safeDoorHandle, List<int> safeCombination,
            RotationDirections initRotationalDirection)
        {
            try
            {
                if (safeHandle == 0 || safeDoorHandle == 0)
                {
                    Log.Error($"SafeCracking: Invalid handle: safe={safeHandle}, safeDoor={safeDoorHandle}");
                    return false;
                }

                var safe = Entity.FromHandle(safeHandle);
                _safeDoor = Entity.FromHandle(safeDoorHandle);

                if (safe == null || _safeDoor == null)
                {
                    Log.Error($"Null entity");
                    return false;
                }

                var isNewLocation = World.GetDistance(_animPosition, _safeDoor.Position) > 5f;
                if (isNewLocation)
                {
                    _safeDoorClosedHeading = safe.Heading;

                    _safeCombination = safeCombination;
                    if (_safeCombination.Count == 0)
                    {
                        Log.Error("SafeCracking: Safe is missing combination.");
                        return false;
                    }
                    _initDialRotationDirection = initRotationalDirection;

                    RelockSafe();

                    SetSafeDialStartNumber();
                }
                _initRelockCountdown = false;

                if (IsSafeDoorOpen())
                {
                    Log.Error("Safe door is not closed");
                    return false;
                }
                _state = SafeCrackingStates.Setup;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Security relock countdown when safe is inactive
        /// </summary>
        private async Task UnloadSafeCountdown()
        {
            try
            {
                for (var i = 0; i < 12; i++)
                {
                    if (!_initRelockCountdown) break;
                    await BaseScript.Delay(5000);
                }
                if (_initRelockCountdown) RelockSafe();
                Client.DeregisterTickHandler(UnloadSafeCountdown);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Initialize list of safe locks to LOCKED status
        /// </summary>
        private List<bool> InitSafeLocks()
        {
            try
            {
                var locks = new List<bool>();
                for (var i = 0; i < _safeCombination.Count; i++) locks.Add(true);
                return locks;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new List<bool>();
            }
        }

        /// <summary>
        ///     Relock the safe
        /// </summary>
        internal void RelockSafe()
        {
            try
            {
                if (_safeCombination == null) return;
                _safeLockStatus = InitSafeLocks();
                _currentLockNum = 0;
                _requiredDialRotationDirection = _initDialRotationDirection;

                for (var i = 0; i < _safeLockStatus.Count; i++)
                    _safeLockStatus[i] = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Initialize the safe dial to start on random number
        /// </summary>
        private void SetSafeDialStartNumber()
        {
            try
            {
                var dialStartNumber = _rand.Next(0, 100);

                SafeDialRotation = 3.6f * dialStartNumber;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Run the safecracking mini game
        /// </summary>
        internal async Task<int> RunMiniGame()
        {
            try
            {
                switch (_state)
                {
                    case SafeCrackingStates.Setup:
                        {
                            _animHeading = _safeDoor.Heading;
                            _animPosition = GetSafeDoorAnimOffsetPosition(_safeDoor.Position, _safeDoor.Heading);

                            if (_animPosition == Vector3.Zero)
                            {
                                Log.Error("Safe door position not initialized. Safe crack aborted.");
                                EndMiniGame(false, true);
                                return (int)SafeCrackingResults.Failed;
                            }
                            _initPlayerHealth = Game.PlayerPed.Health;

                            API.FreezeEntityPosition(Game.PlayerPed.Handle, true);
                            //CurrentPlayer.EnableWeaponWheel(false);

                            await PlaySafeCrackIntroAnim();

                            _state = SafeCrackingStates.Cracking;
                            break;
                        }
                    case SafeCrackingStates.Cracking:
                        {
                            var currentHealth = Game.PlayerPed.Health;
                            if (currentHealth > _initPlayerHealth) _initPlayerHealth = currentHealth;

                            var isDead = Game.PlayerPed.IsDead || Function.Call<bool>(Hash.DECOR_GET_BOOL, Game.PlayerPed.Handle, "Ped.IsIncapacitated");
                            var endImmediately = Client.Get<Arrest>().CurrentCuffState != CuffState.None || Game.PlayerPed.IsRagdoll ||
                                                 currentHealth < _initPlayerHealth || isDead;

                            if (Game.IsControlPressed(0, Control.MoveDownOnly) || endImmediately)
                            {
                                EndMiniGame(false, endImmediately);
                                return (int)SafeCrackingResults.Failed;
                            }

                            HandleSafeDialMovement();

                            var incorrectMovement = _currentLockNum != 0 &&
                                                    _requiredDialRotationDirection != RotationDirections.Idle &&
                                                    _currentDialRotationDirection != RotationDirections.Idle &&
                                                    _currentDialRotationDirection != _requiredDialRotationDirection;

                            if (incorrectMovement)
                            {
                                HandleIncorrectMovement();
                            }
                            else
                            {
                                var currentDialNumber = GetCurrentSafeDialNumber(SafeDialRotation);

                                var correctMovement = _requiredDialRotationDirection != RotationDirections.Idle &&
                                                      (_currentDialRotationDirection == _requiredDialRotationDirection ||
                                                       _lastDialRotationDirection == _requiredDialRotationDirection);
                                if (correctMovement)
                                {
                                    const int tumbleSettleTime = 275;
                                    if (_lastKeyPress < Game.GameTime - tumbleSettleTime)
                                    {
                                        var pinUnlocked = _safeLockStatus[_currentLockNum] && currentDialNumber == _safeCombination[_currentLockNum];
                                        if (pinUnlocked)
                                        {
                                            ReleaseCurrentPin();

                                            if (IsSafeUnlocked())
                                            {
                                                EndMiniGame(true, false);
                                                return (int)SafeCrackingResults.Success;
                                            }
                                        }
                                    }
                                    var dialProximityToTarget = GetDialProximityToTargetPin(currentDialNumber);
                                    SetDialSpriteShake(dialProximityToTarget);
                                }
                            }
                            DrawSprites(false);
                            break;
                        }
                    default:
                        {
                            Log.Error("Entered 'default' unused state.");
                            EndMiniGame(false, true);
                            return (int)SafeCrackingResults.Failed;
                        }
                }
                return (int)SafeCrackingResults.Running;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return (int)SafeCrackingResults.Failed;
            }
        }

        /// <summary>
        ///     End the safe cracking minigame
        /// </summary>
        private void EndMiniGame(bool safeUnlocked, bool endImmediately)
        {
            try
            {
                if (endImmediately)
                {
                    Game.PlayerPed.Task.ClearAllImmediately();
                }
                else
                {
                    Game.PlayerPed.Task.ClearAll();

                    const string safeCrackAnimDict = "mini@safe_cracking";
                    var pos = Game.PlayerPed.Position;
                    API.TaskPlayAnimAdvanced(Game.PlayerPed.Handle, safeCrackAnimDict, "step_out", pos.X, pos.Y, pos.Z, 0, 0, Game.PlayerPed.Heading, 1.0f, 1.0f, -1, 0, 0, 0, 0);
                    //EmotesManager.PlayAnimation(Game.PlayerPed.Handle, safeCrackAnimDict, "step_out",
                    // Game.PlayerPed.Position,
                    //new Vector3(0, 0, Game.PlayerPed.Heading));
                }
                ClearAnimations();

                API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
                //CurrentPlayer.EnableWeaponWheel(true);

                _state = SafeCrackingStates.Setup;
                if (safeUnlocked) return;
                //Run 60s relock countdown
                _initRelockCountdown = true;
                Client.RegisterTickHandler(UnloadSafeCountdown);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Clear minigame animations
        /// </summary>
        private void ClearAnimations()
        {
            try
            {
                IEnumerable<string> animList = new List<string> {
                    "dial_turn_fail_3",
                    "dial_turn_fail_4",
                    "dial_turn_anti_normal",
                    "dial_turn_clock_normal",
                    "dial_turn_anti_fast",
                    "dial_turn_clock_fast",
                    "idle_heavy_breathe",
                    "idle_look_around",
                    "idle_base"
                };

                foreach (var anim in animList) Game.PlayerPed.Task.ClearAnimation("mini@safe_cracking", anim);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Return the current dial number promixity to the target number
        /// </summary>
        /// <param name="currentDialNumber"></param>
        private int GetDialProximityToTargetPin(int currentDialNumber)
        {
            try
            {
                var target = _safeCombination[_currentLockNum];

                int dialPromixityToTumble;
                switch (_currentDialRotationDirection)
                {
                    case RotationDirections.Clockwise:
                    case RotationDirections.Idle when _lastDialRotationDirection == RotationDirections.Clockwise:
                        dialPromixityToTumble = target - currentDialNumber;
                        break;
                    case RotationDirections.Anticlockwise:
                    case RotationDirections.Idle when _lastDialRotationDirection == RotationDirections.Anticlockwise:
                        dialPromixityToTumble = currentDialNumber - target;
                        break;
                    default:
                        dialPromixityToTumble = 100;
                        break;
                }

                if (dialPromixityToTumble < 0)
                    dialPromixityToTumble = dialPromixityToTumble + 100;

                return dialPromixityToTumble;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1;
            }
        }

        /// <summary>
        ///     Release the current lock pin
        /// </summary>
        private void ReleaseCurrentPin()
        {
            try
            {
                _safeLockStatus[_currentLockNum] = false;
                _currentLockNum = _currentLockNum + 1;

                _requiredDialRotationDirection =
                    _requiredDialRotationDirection == RotationDirections.Anticlockwise
                        ? RotationDirections.Clockwise
                        : RotationDirections.Anticlockwise;

                Client.Get<SoundController>().PlayFrontentSound(IsSafeUnlocked() ? "TUMBLER_PIN_FALL_FINAL" : "TUMBLER_PIN_FALL",
                    "SAFE_CRACK_SOUNDSET");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Manipulate safe dial according to player input
        /// </summary>
        private void HandleSafeDialMovement()
        {
            try
            {
                var playerHandle = Game.PlayerPed.Handle;
                const string safeCrackAnimDict = "mini@safe_cracking";
                var currAnim = "";
                //Forced delay on failures
                if (API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_fail_3", 3) || API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_fail_4", 3))
                    return;
                //Button presses are slow rotations
                if (Game.IsControlJustPressed(0, Control.MoveLeftOnly))
                {
                    _debounceTime = 100;
                    _lastKeyPress = Game.GameTime;

                    if (!API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_anti_normal", 3))
                        currAnim = "dial_turn_anti_normal";

                    RotateSafeDial(RotationDirections.Anticlockwise);
                }
                else if (Game.IsControlJustPressed(0, Control.MoveRightOnly))
                {
                    _debounceTime = 100;
                    _lastKeyPress = Game.GameTime;

                    if (!API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_clock_normal", 3))
                        currAnim = "dial_turn_clock_normal";

                    RotateSafeDial(RotationDirections.Clockwise);
                }
                //Button holds are fast/continuous rotations
                else if (Game.IsControlPressed(0, Control.MoveLeftOnly))
                {
                    if (_lastKeyPress >= Game.GameTime - _debounceTime) return;

                    _debounceTime = 10;
                    _lastKeyPress = Game.GameTime;

                    if (!API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_anti_fast", 3))
                        currAnim = "dial_turn_anti_fast";

                    RotateSafeDial(RotationDirections.Anticlockwise);
                }
                else if (Game.IsControlPressed(0, Control.MoveRightOnly))
                {
                    if (_lastKeyPress >= Game.GameTime - _debounceTime) return;

                    _debounceTime = 10;
                    _lastKeyPress = Game.GameTime;

                    if (!API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                        "dial_turn_clock_fast", 3))
                        currAnim = "dial_turn_clock_fast";

                    RotateSafeDial(RotationDirections.Clockwise);
                }
                else
                {
                    _currentDialRotationDirection = RotationDirections.Idle;
                    if (API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict,
                            "dial_turn_anti_normal", 3) || API.IsEntityPlayingAnim(playerHandle,
                            safeCrackAnimDict,
                            "dial_turn_clock_normal", 3) || API.IsEntityPlayingAnim(playerHandle,
                            safeCrackAnimDict,
                            "dial_turn_anti_fast", 3) || API.IsEntityPlayingAnim(playerHandle,
                            safeCrackAnimDict,
                            "dial_turn_clock_fast", 3) || API.IsEntityPlayingAnim(playerHandle,
                            safeCrackAnimDict, "idle_base", 3) || API.IsEntityPlayingAnim(playerHandle,
                            safeCrackAnimDict, "idle_heavy_breathe", 3) || API.IsEntityPlayingAnim(
                            playerHandle,
                            safeCrackAnimDict, "idle_look_around", 3)) return;

                    var idleAnimNumm = Game.GameTime % 3;
                    string idleAnim;
                    switch (idleAnimNumm)
                    {
                        case 2:
                            {
                                idleAnim = "idle_heavy_breathe";
                                break;
                            }
                        case 1:
                            {
                                idleAnim = "idle_look_around";
                                break;
                            }
                        default:
                            {
                                idleAnim = "idle_base";
                                break;
                            }
                    }

                    currAnim = idleAnim;
                }

                if (string.IsNullOrEmpty(currAnim)) return;

                API.TaskPlayAnimAdvanced(Game.PlayerPed.Handle, safeCrackAnimDict, currAnim, _animPosition.X, _animPosition.Y, _animPosition.Z, 0, 0, _animHeading, 1.0f, 1.0f, -1, 0, 0, 0, 0);
                //EmotesManager.PlayAnimation(playerHandle, safeCrackAnimDict, currAnim,
                //   _animPosition,
                //   new Vector3(0, 0, _animHeading));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Rotate the safe dial
        /// </summary>
        /// <param name="rotationDirection"></param>
        private void RotateSafeDial(RotationDirections rotationDirection)
        {
            try
            {
                const float rotationPerNumber = 3.6f;
                var multiplier = rotationDirection == RotationDirections.Anticlockwise ? 1f : -1f;
                var rotationChange = multiplier * rotationPerNumber;

                SafeDialRotation += rotationChange;
                Client.Get<SoundController>().PlayFrontentSound("TUMBLER_TURN", "SAFE_CRACK_SOUNDSET");

                _currentDialRotationDirection = rotationDirection;
                _lastDialRotationDirection = rotationDirection;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Return the number the safe dial is currently at
        /// </summary>
        /// <param name="currentDialAngle">Current angle of the safe dial</param>
        private int GetCurrentSafeDialNumber(float currentDialAngle)
        {
            try
            {
                var number = (int)Math.Round(100 * (currentDialAngle / 360f));
                if (number > 0)
                    number = 100 - number;
                return Math.Abs(number);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return -1;
            }
        }

        /// <summary>
        ///     Handle safe dial failures when player rotates dial opposite the
        ///     required direction
        /// </summary>
        private void HandleIncorrectMovement()
        {
            try
            {
                ResetSafeLocks();

                const string safeCrackAnimDict = "mini@safe_cracking";

                var animation = Game.GameTime % 2 == 0 ? "dial_turn_fail_3" : "dial_turn_fail_4";

                API.TaskPlayAnimAdvanced(Game.PlayerPed.Handle, safeCrackAnimDict, animation, _animPosition.X, _animPosition.Y, _animPosition.Z, 0, 0, _animHeading, 1.0f, 1.0f, -1, 0, 0, 0, 0);
                //EmotesManager.PlayAnimation(Cache.PlayerHandle, safeCrackAnimDict, animation, _animPosition,
                  //  new Vector3(0, 0, _animHeading));

                _currentDialRotationDirection = RotationDirections.Idle;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Reset the locks on the safe
        /// </summary>
        private void ResetSafeLocks()
        {
            try
            {
                RelockSafe();

                Client.Get<SoundController>().PlayFrontentSound("TUMBLER_RESET", "SAFE_CRACK_SOUNDSET");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Open the safe door with animation and sound
        /// </summary>
        internal async Task OpenSafeDoor()
        {
            try
            {
                if (_safeDoor == null || _safeDoor.Handle == 0) return;

                var animationPosition = GetSafeDoorAnimOffsetPosition(_safeDoor.Position, _safeDoor.Heading);
                var animationHeading = _safeDoor.Heading;
                const string safeCrackAnimDict = "mini@safe_cracking";
                const string openSafeAnim = "door_open_succeed_stand";

                API.TaskPlayAnimAdvanced(Game.PlayerPed.Handle, safeCrackAnimDict, openSafeAnim, animationPosition.X, animationPosition.Y, animationPosition.Z, 0, 0, animationHeading, 1.0f, 1.0f, -1, 0, 0, 0, 0);
                //Game.PlayerPed.Task.PlayAnimation(Cache.PlayerHandle, safeCrackAnimDict, openSafeAnim, animationPosition,
                    //new Vector3(0, 0, animationHeading));

                const int timeTillAnimOpens = 2050;
                await BaseScript.Delay(timeTillAnimOpens);

                Client.Get<SoundController>().PlayFrontentSound("SAFE_DOOR_OPEN", "SAFE_CRACK_SOUNDSET");

                for (var i = 0; i < 90; i++)
                {
                    _safeDoor.Heading += 1f;
                    await BaseScript.Delay(16);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Close the safe door with animation and sound
        /// </summary>
        internal async Task CloseSafeDoor()
        {
            try
            {
                if (!IsSafeDoorOpen()) return;

                if (_safeDoor == null || _safeDoor.Handle == 0) return;

                for (var i = 0; i < 90; i++)
                {
                    _safeDoor.Heading -= 1f;
                    await BaseScript.Delay(16);
                }
                _safeDoor.Heading = _safeDoorClosedHeading;

                Client.Get<SoundController>().PlayFrontentSound("SAFE_DOOR_CLOSE", "SAFE_CRACK_SOUNDSET");
                await BaseScript.Delay(750);
                ResetSafeLocks();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal bool IsSafeDoorOpen()
        {
            try
            {
                if (_safeDoor == null || _safeDoor.Handle == 0) return false;

                return Math.Abs(_safeDoor.Heading - _safeDoorClosedHeading) > 0.05;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        internal bool IsSafeUnlocked()
        {
            try
            {
                if (_safeDoor == null || _safeDoor.Handle == 0) return false;

                return _safeLockStatus.All(locked => locked == false);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        /// <summary>
        ///     Get the position offset for safe cracking animations
        /// </summary>
        /// <param name="safePosition"></param>
        /// <param name="safeHeading"></param>
        /// <param name="animation">type of animation, intro anim requires unique offset</param>
        internal Vector3 GetSafeDoorAnimOffsetPosition(Vector3 safePosition, float safeHeading,
            Animations animation = Animations.Other)
        {
            try
            {
                float sinx;
                float cosx;
                float siny;
                float cosy;

                if (animation == Animations.Intro)
                {
                    sinx = 0.8f;
                    cosx = -0.35f;
                    siny = -0.35f;
                    cosy = -0.8f;
                }
                else
                {
                    sinx = 0.53f;
                    cosx = -0.6f;
                    siny = -0.6f;
                    cosy = -0.53f;
                }

                var offsetX = sinx * (float)Math.Sin(safeHeading * Math.PI / 180) +
                              cosx * (float)Math.Cos(safeHeading * Math.PI / 180);

                var offsetY = siny * (float)Math.Sin(safeHeading * Math.PI / 180) +
                              cosy * (float)Math.Cos(safeHeading * Math.PI / 180);

                return new Vector3(safePosition.X + offsetX,
                    safePosition.Y + offsetY,
                    Game.PlayerPed.Position.Z);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Game.PlayerPed.Position;
            }
        }

        /// <summary>
        ///     Play the starting position animation for the minigame
        /// </summary>
        private async Task PlaySafeCrackIntroAnim()
        {
            try
            {
                var animationPosition = GetSafeDoorAnimOffsetPosition(_safeDoor.Position, _safeDoor.Heading, Animations.Intro);
                var animationHeading = _safeDoor.Heading;
                var playerHandle = Game.PlayerPed.Handle;

                const string safeCrackAnimDict = "mini@safe_cracking";
                const string safeCrackIntroAnim = "step_into";
                API.TaskPlayAnimAdvanced(Game.PlayerPed.Handle, safeCrackAnimDict, safeCrackIntroAnim, animationPosition.X, animationPosition.Y, animationPosition.Z, 0, 0, animationHeading, 1.0f, 1.0f, -1, 0, 0, 0, 0);
                //EmotesManager.PlayAnimation(playerHandle, safeCrackAnimDict, safeCrackIntroAnim, animationPosition,
                // new Vector3(0, 0, animationHeading));

                while (API.IsEntityPlayingAnim(playerHandle, safeCrackAnimDict, safeCrackIntroAnim,
                    3)) await BaseScript.Delay(10);

                await BaseScript.Delay(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Set shaking effect on the safe dial sprite based on proximity
        ///		Change these values to increase/decrease difficulty.
        /// </summary>
        /// <param name="dialPromixityToTarget"></param>
        private void SetDialSpriteShake(int dialPromixityToTarget)
        {
            try
            {
                switch (dialPromixityToTarget)
                {
                    case 2:
                        _spriteX = (float)_rand.NextDouble(.48 - 0.00025, .48 + 0.0005);
                        _spriteY = (float)_rand.NextDouble(.3 - 0.00025, .3 + 0.0005);
                        break;
                    case 1:
                        _spriteX = (float)_rand.NextDouble(.48 - 0.0005, .48 + 0.0005);
                        _spriteY = (float)_rand.NextDouble(.3 - 0.0005, .3 + 0.0005);
                        break;
                    default:
                        _spriteX = 0.48f;
                        _spriteY = 0.3f;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Draw safe dial and locks sprites
        /// </summary>
        /// <param name="drawLocks">Enable to draw lock sprites</param>
        private void DrawSprites(bool drawLocks)
        {
            try
            {
                const string textureDict = "MPSafeCracking";

                API.DrawSprite(textureDict, "Dial_BG", _spriteX, _spriteY, 0.3f, _aspectRatio * 0.3f, 0, 255,
                    255, 255, 255);
                API.DrawSprite(textureDict, "Dial", _spriteX, _spriteY, 0.3f * 0.5f,
                    _aspectRatio * 0.3f * 0.5f, SafeDialRotation,
                    255, 255, 255, 255);

                if (!drawLocks) return;

                //TODO: Center position based on number of locks
                var xPos = 0.38f;
                foreach (var lockActive in _safeLockStatus)
                {
                    var lockString = lockActive ? "lock_closed" : "lock_open";
                    API.DrawSprite(textureDict, lockString, xPos, 0.8f, 0.06f,
                        _aspectRatio * 0.03f, 0,
                        255, 255, 255, 255);

                    xPos = xPos + 0.10f;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public enum Animations
        {
            Intro,
            Other
        }

        public enum SafeCrackingStates
        {
            Setup,
            Cracking
        }
    }
}
