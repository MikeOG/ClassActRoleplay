using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Player
{
    public class MiscEvents : ClientAccessor
    {
        private Animation textAnim = new Animation("amb@code_human_wander_texting@male@base", "", "static", "", "ye", new AnimationOptions{LoopEnableMovement = true, LoopDuration = 1250, OverrideLoopAnimDict = false});
        private Animation callAnim = new Animation("amb@code_human_wander_mobile@male@base", "", "static", "", "ye", new AnimationOptions{LoopEnableMovement = true, LoopDuration = 1250, OverrideLoopAnimDict = false});

        public MiscEvents(Client client) : base(client)
        {
            client.RegisterEventHandler("Player.UpdateLocation", new Action(OnUpdateLocation));
            client.RegisterEventHandler("Player.UpdatePosition", new Action(OnUpdatePosition));
            client.RegisterEventHandler("Player.OnLoginComplete", new Action(() =>
            {
                //BaseScript.TriggerEvent("chat:addTemplate", "tweet", "<img src='data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+Cjxz%0D%0AdmcKICB2aWV3Ym94PSIwIDAgMjAwMCAxNjI1LjM2IgogIHdpZHRoPSIyMDAwIgogIGhlaWdodD0i%0D%0AMTYyNS4zNiIKICB2ZXJzaW9uPSIxLjEiCiAgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAv%0D%0Ac3ZnIj4KICA8cGF0aAogICAgZD0ibSAxOTk5Ljk5OTksMTkyLjQgYyAtNzMuNTgsMzIuNjQgLTE1%0D%0AMi42Nyw1NC42OSAtMjM1LjY2LDY0LjYxIDg0LjcsLTUwLjc4IDE0OS43NywtMTMxLjE5IDE4MC40%0D%0AMSwtMjI3LjAxIC03OS4yOSw0Ny4wMyAtMTY3LjEsODEuMTcgLTI2MC41Nyw5OS41NyBDIDE2MDku%0D%0AMzM5OSw0OS44MiAxNTAyLjY5OTksMCAxMzg0LjY3OTksMCBjIC0yMjYuNiwwIC00MTAuMzI4LDE4%0D%0AMy43MSAtNDEwLjMyOCw0MTAuMzEgMCwzMi4xNiAzLjYyOCw2My40OCAxMC42MjUsOTMuNTEgLTM0%0D%0AMS4wMTYsLTE3LjExIC02NDMuMzY4LC0xODAuNDcgLTg0NS43MzksLTQyOC43MiAtMzUuMzI0LDYw%0D%0ALjYgLTU1LjU1ODMsMTMxLjA5IC01NS41NTgzLDIwNi4yOSAwLDE0Mi4zNiA3Mi40MzczLDI2Ny45%0D%0ANSAxODIuNTQzMywzNDEuNTMgLTY3LjI2MiwtMi4xMyAtMTMwLjUzNSwtMjAuNTkgLTE4NS44NTE5%0D%0ALC01MS4zMiAtMC4wMzksMS43MSAtMC4wMzksMy40MiAtMC4wMzksNS4xNiAwLDE5OC44MDMgMTQx%0D%0ALjQ0MSwzNjQuNjM1IDMyOS4xNDUsNDAyLjM0MiAtMzQuNDI2LDkuMzc1IC03MC42NzYsMTQuMzk1%0D%0AIC0xMDguMDk4LDE0LjM5NSAtMjYuNDQxLDAgLTUyLjE0NSwtMi41NzggLTc3LjIwMywtNy4zNjQg%0D%0ANTIuMjE1LDE2My4wMDggMjAzLjc1LDI4MS42NDkgMzgzLjMwNCwyODQuOTQ2IC0xNDAuNDI5LDEx%0D%0AMC4wNjIgLTMxNy4zNTEsMTc1LjY2IC01MDkuNTk3MiwxNzUuNjYgLTMzLjEyMTEsMCAtNjUuNzg1%0D%0AMSwtMS45NDkgLTk3Ljg4MjgsLTUuNzM4IDE4MS41ODYsMTE2LjQxNzYgMzk3LjI3LDE4NC4zNTkg%0D%0ANjI4Ljk4OCwxODQuMzU5IDc1NC43MzIsMCAxMTY3LjQ2MiwtNjI1LjIzOCAxMTY3LjQ2MiwtMTE2%0D%0ANy40NyAwLC0xNy43OSAtMC40MSwtMzUuNDggLTEuMiwtNTMuMDggODAuMTc5OSwtNTcuODYgMTQ5%0D%0ALjczOTksLTEzMC4xMiAyMDQuNzQ5OSwtMjEyLjQxIgogICAgc3R5bGU9ImZpbGw6IzAwYWNlZCIv%0D%0APgo8L3N2Zz4K' height='16'> <b style='color:#00BFFF'>{0}</b>: {1}");
                BaseScript.TriggerEvent("chat:addTemplate", "chop", "<div style='background-color: rgba(66, 244, 179, 0.7); border-radius: 10px; width: 100%; text-align: center; vertical-align: middle;'><p style='opacity: 1.0; color: rgb(255, 255, 255);'>{0}</p><p style='text-size: 12px; text-align: right; padding-right: 5px;'>Time remaining: {1}</p></div>");
            }));
            client.RegisterEventHandler("Player.ExecuteCommand", new Action<string>(ExecuteCommand));
            client.RegisterEventHandler("Player.SetPosition", new Action<Vector3>(SetPosition));
            client.RegisterEventHandler("Player.PlayTextAnim", new Action(() => textAnim.PlayFullAnim()));
            client.RegisterEventHandler("Player.PlayRadioAnim", new Action(() => callAnim.PlayFullAnim()));
            client.RegisterEventHandler("Player.SetHeading", new Action<float>(heading => Game.PlayerPed.Heading = heading));
            client.RegisterEventHandler("Player.DoSmoke", new Action(() => EmoteManager.playerAnimations["smoke"].PlayFullAnim()));
            client.RegisterEventHandler("Player.DoDrink", new Action(() => EmoteManager.playerAnimations["drink"].PlayFullAnim()));
            client.RegisterEventHandler("Player.DoDrunkEffect", new Action<string>(OnDoDrunk));
            client.RegisterEventHandler("Vehicle.WashVehicle", new Action(() => Cache.PlayerPed.CurrentVehicle.DirtLevel = 0.0f));
            //lient.Instance.RegisterEventHandler("Chat.CommandEntered", (Delegate)new Action<string>(((CommandProcessor)this).Handle));
            CommandRegister.RegisterCommand("givecash", OnGiveCash);
            CommandRegister.RegisterCommand("killme", OnForcePlayerDown);
            CommandRegister.RegisterCommand("dirt", cmd =>
            {
                Cache.PlayerPed.CurrentVehicle.DirtLevel = 15.0f;
            });
        }

        private void OnForcePlayerDown(Command cmd)
        {
            try
            {
                SetEntityHealth(Game.PlayerPed.Handle, 0);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("ClientCommands ForcePlayerDown exception: {0}", (object)ex));
            }
        }

        public async void LowDrunkEffect()
        {
            EmoteManager.playerAnimations["drink"].PlayFullAnim();
            // hello jazz, you can put drunk effects here in the future / fix all this code, just and outline for items.
        }

        public async void HighDrunkEffect()
        {
            EmoteManager.playerAnimations["drink"].PlayFullAnim();
            // hello jazz, you can put drunk effects here in the future / fix all this code, just and outline for items.
        }

        public async void GeneralDrunkEffect()
        {
            EmoteManager.playerAnimations["drink"].PlayFullAnim();
            // hello jazz, you can put drunk effects here in the future / fix all this code, just and outline for items.
        }

        private void OnUpdateLocation()
        {
            Client.Instance.TriggerServerEvent("Session.UpdateLocation", GTAHelpers.GetLocationString());
        }

        private void OnGiveCash(Command cmd)
        {
            var cashAmount = cmd.GetArgAs(0, 0);

            if (cashAmount < 0) return;

            var closePlayer = GTAHelpers.GetClosestPlayer(4.0f);

            if (closePlayer != null && closePlayer != Game.Player)
            {
                Client.TriggerServerEvent("Payment.GivePlayerCash", closePlayer.ServerId, cashAmount);
            }
        }

        private void OnUpdatePosition()
        {
            Client.Instance.TriggerServerEvent("Session.UpdatePositon", Game.PlayerPed.Position/*.ToArray().ToList()*/);
        }

        private void SetPosition(Vector3 posList)
        {
            Game.PlayerPed.TeleportToLocation(posList);
        }

        private void OnDoDrunk(string drinkType)
        {
            if (drinkType == "low")
                LowDrunkEffect();
            else if (drinkType == "high")
                HighDrunkEffect();
            else
                GeneralDrunkEffect();
        }
    }
}
