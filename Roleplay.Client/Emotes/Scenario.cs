using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Emotes
{
    public sealed class Scenario : PlayableAnimation
    {
        private int v1;
        private string v2;
        private Bone bone;
        private AnimationFlags animationFlags;

        private string ScenarioName { get; set; }

        public Scenario(string scenarioName, string displayName)
        {
            ScenarioName = scenarioName;
            DisplayName = displayName;
        }

        public Scenario(string scenarioName, string displayName, int v1, string v2, Bone bone, AnimationFlags animationFlags) : this(scenarioName, displayName)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.bone = bone;
            this.animationFlags = animationFlags;
        }

        public override async Task PlayFullAnim()
        {
            await base.PlayFullAnim();
            if (ScenarioName == "PROP_HUMAN_SEAT_CHAIR_MP_PLAYER")
            {
                Vector3 playerCoords = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0.0f, 0 - 0.5f, -0.5f);
                TaskStartScenarioAtPosition(Game.PlayerPed.Handle, ScenarioName, playerCoords.X, playerCoords.Y, playerCoords.Z, Game.PlayerPed.Heading, -1, true, true);
            }
            else
                TaskStartScenarioInPlace(Game.PlayerPed.Handle, ScenarioName, 0, true);
        }
    }
}
