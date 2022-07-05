using CitizenFX.Core;
using Roleplay.Client.Enums;

namespace Roleplay.Client.Emotes
{
    public class AnimationOptions
    {
        // All

        // Props can be found here https://github.com/GroovyGiantPanda/FiveMRpServerResources/blob/master/src/FiveM/RPClient/Enums/ObjectHash.cs
        public ObjectHash Prop = (ObjectHash)(-1);
        // Bones can be found here https://github.com/citizenfx/fivem/blob/master/code/client/clrcore/External/BoneID.cs
        public Bone PropBone;
        public Vector3 PropOffset = new Vector3(0, 0, 0);
        public Vector3 PropRotation = new Vector3(0, 0, 0);

        // Start
        public bool PlayStart = true;
        public bool StartEnableMovement = false;
        public bool AttachPropStart = false;
        public float StartPlaybackRate = 0.0f;
        public int StartDuration = -1;
        public AnimationFlags StartAnimOverride = (AnimationFlags)(-1);

        // Loop
        public bool LoopDoLoop = false;
        public bool LoopEnableMovement = false;
        public bool OverrideLoopAnimDict = true;
        public bool AttachPropLoop = false;
        public float LoopPlaybackRate = 0.0f;
        public int LoopDuration = -1;
        public AnimationFlags LoopAnimOverride = (AnimationFlags)(-1);

        // End
        public bool PlayEnd = false;
        public bool EndEnableMovement = false;
        public bool AttachPropEnd = false;
        public bool EndClearTasks = true;
        public float EndPlaybackRate = 0.0f;
        public int EndDuration = -1;
        public AnimationFlags EndAnimOverride = (AnimationFlags)(-1);

        public bool RegisterCheckTick => StartDuration == -1 && LoopDuration == -1 && EndDuration == -1;
    }
}