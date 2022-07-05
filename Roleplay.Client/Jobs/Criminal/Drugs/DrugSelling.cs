using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Roleplay.Client.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Jobs.Criminal.Drugs
{
    public class DrugSelling
    {
        private PedList pedList = new PedList();
        private List<Ped> interactedPeds = new List<Ped>();
        private Drugs drugInstance;
        private bool isInteracting = false;
        private DateTime lastInteraction = DateTime.Now;

        public DrugSelling(Drugs drugInstance)
        {
            Client.Instance.RegisterEventHandler("Player.CheckForInteraction", new Action(OnInteraciton));
            this.drugInstance = drugInstance;
        }

        private async void OnInteraciton()
        {
            if (Cache.PlayerPed.IsInVehicle()) return;

            if(!isInteracting || (DateTime.Now - lastInteraction).TotalSeconds > 15)
            {
                isInteracting = true;
                var closePed = GTAHelpers.GetCloestPed(customFindFunc: o => !o.IsPlayer && !o.IsDead && o.IsHuman && !o.IsInVehicle() && o.Position.DistanceToSquared(Game.PlayerPed.Position) < 3.0f);

                if (closePed != null && !interactedPeds.Contains(closePed))
                {
                    interactedPeds.Add(closePed);
                    var hasDrug = false;
                    var drugName = "";
                    var playerInv = await Client.LocalSession.GetInventory(false);
                    await drugInstance.currentDrugs.ForEachAsync(o =>
                    {
                        if (hasDrug) return Task.FromResult(0);

                        //Log.ToChat(o.ProcessDrugName);
                        if (playerInv.HasItem(o.ProcessDrugName))
                        {
                            hasDrug = true;
                            drugName = o.ProcessDrugName;
                        }

                        return Task.FromResult(0);
                    });

                    if (hasDrug)
                    {
                        closePed.Task.ClearAllImmediately();
                        closePed.Task.TurnTo(Cache.PlayerPed);
                        await BaseScript.Delay(1000);
                        closePed.Task.ClearAllImmediately();
                        closePed.Task.TurnTo(Cache.PlayerPed);
                        StartDrugSell(closePed, drugName);
                        return;
                    }
                }

                isInteracting = false;
            }
        }

        private async void StartDrugSell(Ped buyingPed, string drug)
        {
            lastInteraction = DateTime.Now;
            var playerPed = Game.PlayerPed;
            var waitTime = new Random().Next(8, 12);
            buyingPed.IsPositionFrozen = true;

            Screen.ShowSubtitle($"~r~{waitTime}~s~ seconds until sale is complete", 1001);
            while (waitTime > 1)
            {
                buyingPed.IsPositionFrozen = true;
                //buyingPed.Task.TurnTo(Cache.PlayerPed);
                await BaseScript.Delay(1000);
                waitTime--;
                Screen.ShowSubtitle($"~r~{waitTime}~s~ seconds until sale is complete", 1001);

                if (buyingPed.IsDead)
                {
                    Log.ToChat("[Drugs]", "Your client died", ConstantColours.Criminal);
                    isInteracting = false;
                    buyingPed.IsPositionFrozen = false;
                    return;
                }
                
                if (buyingPed.Position.DistanceToSquared(playerPed.Position) > 15.0f || Cache.PlayerPed.IsInVehicle())
                {
                    Log.ToChat("[Drugs]", "You moved to far away from your client", ConstantColours.Criminal);
                    isInteracting = false;
                    buyingPed.IsPositionFrozen = false;
                    return;
                }
            }

            buyingPed.Task.ClearAllImmediately();
            buyingPed.IsPositionFrozen = false;
            buyingPed.Task.TurnTo(Cache.PlayerPed);

            await BaseScript.Delay(2000);

            playerPed.Task.LookAt(buyingPed);

            var sellDrug = new Random().NextBool();

            if (buyingPed.IsDead)
            {
                Log.ToChat("[Drugs]", "Your client died", ConstantColours.Criminal);
                isInteracting = false;
                buyingPed.IsPositionFrozen = false;
                return;
            }
            
            if (buyingPed.Position.DistanceToSquared(playerPed.Position) > 5.0f)
            {
                Log.ToChat("[Drugs]", "You moved to far away from your client", ConstantColours.Criminal);
                isInteracting = false;
                buyingPed.IsPositionFrozen = false;
                return;
            }

            if (!sellDrug)
            {
                Log.ToChat("[Drugs]", "Your client doesn't want to buy your product", ConstantColours.Criminal);
                isInteracting = false;
                buyingPed.IsPositionFrozen = false;
                return;
            }

            await playerPed.Task.PlayAnimation("mp_common", "givetake1_a", 1.0f, 1.0f, -1, AnimationFlags.None, 0.0f);
            await buyingPed.Task.PlayAnimation("mp_common", "givetake1_a", 1.0f, 1.0f, -1, AnimationFlags.None, 0.0f);

            Client.Instance.TriggerServerEvent("Drugs.AttemptSellDrug", drug);

            isInteracting = false;
            buyingPed.IsPositionFrozen = false;
            buyingPed.Task.ClearAll();

            //if (new Random().NextBool(90)) return;

            var closePed = GTAHelpers.GetCloestPed(customFindFunc: o => !o.IsPlayer && !o.IsDead && o.IsHuman && o != buyingPed);

            if(closePed != null)
            {
                var sendAlert = await GTAHelpers.PlayReportAnim(closePed);

                if (sendAlert)
                {
                    var gender = CitizenFX.Core.Native.API.IsPedMale(Cache.PlayerPed.Handle) ? "Male" : "Female";
                    //Client.Instance.TriggerServerEvent("Blip.CreateEmergencyBlip");
                    Client.Instance.TriggerServerEvent("Alerts.SendCADAlert", $"10-66", $"{gender} selling {drug}", GTAHelpers.GetLocationString());
                }
            }
        }
    }
}
