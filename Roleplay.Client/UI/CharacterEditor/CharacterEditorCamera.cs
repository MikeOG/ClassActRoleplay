using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared;

namespace Roleplay.Client.UI
{
    internal class CharacterEditorCamera
    {
        private Camera bodyCam;
        private Camera headCam;
        private Camera LegCam;

        private float maxHeight = 0.0f;
        private float minHeight = 0.0f;
        private float currentHeight = 0.0f;
        private bool useCamera = true;

        public CharacterEditorCamera(bool useCameras = true)
        {
            this.useCamera = useCameras;

            var pos = Game.PlayerPed.Bones[Bone.SKEL_Head].Position;
            headCam = World.CreateCamera(new Vector3(pos.X += 0.3f, pos.Y += 1.0f, pos.Z + 0.40f), new Vector3(180, 180, -20), 45);

            pos = Game.PlayerPed.Bones[Bone.SKEL_Spine3].Position;
            bodyCam = World.CreateCamera(new Vector3(pos.X += 0.3f, pos.Y += 1.0f, pos.Z + 0.40f), new Vector3(180, 180, -20), 75);
            calculateCameraHeights(Game.PlayerPed.Bones[Bone.SKEL_Spine3]);

            pos = Game.PlayerPed.Bones[Bone.SKEL_R_Calf].Position;
            LegCam = World.CreateCamera(new Vector3(pos.X += 0.3f, pos.Y += 1.0f, pos.Z + 0.40f), new Vector3(180, 180, -20), 75);

            if(this.useCamera)
            {
                World.RenderingCamera = bodyCam;
                Client.Instance.RegisterTickHandler(OnTick);
            }
        }

        public void lookAtHead()
        {
            if (World.RenderingCamera != headCam && this.useCamera)
            {
                World.RenderingCamera.InterpTo(headCam, 500, true, true);
                calculateCameraHeights(Game.PlayerPed.Bones[Bone.SKEL_Head], 0.2f);
            }
        }

        public void lookAtBody()
        {
            if (World.RenderingCamera != bodyCam && this.useCamera)
            {
                World.RenderingCamera.InterpTo(bodyCam, 500, true, true);
                calculateCameraHeights(Game.PlayerPed.Bones[Bone.SKEL_Spine3]);
            }
        }

        public void lookAtLegs()
        {
            if (World.RenderingCamera != LegCam && this.useCamera)
            {
                World.RenderingCamera.InterpTo(LegCam, 500, true, true);
                calculateCameraHeights(Game.PlayerPed.Bones[Bone.SKEL_R_Calf], 0.2f);
            }
        }

        private void calculateCameraHeights(PedBone bone, float heightChange = 1.0f)
        {
            var pos = bone.Position;

            currentHeight = pos.Z;
            maxHeight = pos.Z + heightChange / 2;
            minHeight = pos.Z - heightChange;
        }

        private async Task OnTick()
        {
            float zChange = Game.GetControlNormal(1, Control.ScriptRightAxisY);
            if (zChange != 0)
            {
                var pos = World.RenderingCamera.Position;
                if (pos.Z - zChange / 4 < maxHeight && pos.Z - zChange / 4 > minHeight)
                {
                    World.RenderingCamera.Position = new Vector3(pos.X, pos.Y, pos.Z -= zChange / 4);
                }
            }

            // TODO setup behind view for character creation camera
            /*var leftArrowPressed = Game.IsControlJustPressed(1, Control.PhoneLeft);
            var rightArrowPressed = Game.IsControlJustPressed(1, Control.PhoneRight);

            if (leftArrowPressed || rightArrowPressed)
            {
                var bonePos = Game.PlayerPed.Bones[Bone.SKEL_Spine3].Position;
                var camPos = World.RenderingCamera.Position;
                var upVec = World.RenderingCamera.UpVector;
                var forwardVec = World.RenderingCamera.ForwardVector;
                Matrix.BillboardRH(ref bonePos, ref camPos, ref upVec, ref forwardVec, out var matrix);
                var vec = matrix.TranslationVector;
                vec.Y -= 1.0f;
                World.RenderingCamera.Rotation = new Vector3(180, 180, 180);
                var tries = 0;
                while (World.RenderingCamera.Position != vec && tries < 500)
                {
                    var thing = Vector3.SmoothStep(World.RenderingCamera.Position, vec, 0.15f);
                    World.RenderingCamera.Position = thing;
                    await BaseScript.Delay(0);
                    tries += 1;
                }
                World.RenderingCamera.Position = vec;
                
            }*/
        }

        ~CharacterEditorCamera()
        {
            Client.Instance.DeregisterTickHandler(OnTick);
        }
    }
}

