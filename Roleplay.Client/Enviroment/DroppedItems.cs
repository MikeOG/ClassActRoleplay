using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Emotes;
using Roleplay.Client.Player.Controls;
using Roleplay.Server.Shared.Models;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Newtonsoft.Json;

namespace Roleplay.Client.Enviroment
{
    public class DroppedItems : ClientAccessor
    {
        private List<DroppedItemModel> droppedItems = new List<DroppedItemModel>();
        private Animation pickupAnimation = new Animation("pickup_object", "", "pickup_low", "", "", new AnimationOptions
        {
            LoopEnableMovement = true,
            LoopDuration = 1250
        });
        /*private Animation dropAnimation = new Animation("mp_weapon_drop", "", "drop_bh", "", "", new AnimationOptions
        {
            LoopEnableMovement = true,
            LoopDuration = 1250
        });*/
        //Items.UpdateDroppedItems
        public DroppedItems(Client client) : base(client)
        {
            client.RegisterEventHandler("Items.UpdateDroppedItems", new Action<string>(UpdateDroppedItems));

            client.RegisterEventHandler("Items.OnItemDropped", new Action(() =>
            {
                pickupAnimation.PlayFullAnim();
            }));

            client.TriggerServerEvent("Items.RequestDroppedItems");

            client.RegisterTickHandler(OnTick);
        }

        private void UpdateDroppedItems(string items)
        {
            Log.Verbose($"Just recieved a new dropped items list");
            var itemList = JsonConvert.DeserializeObject<List<DroppedItemModel>>(items);

            droppedItems = itemList;
            droppedItems.ForEach(o =>
            {
                Log.Info(o.ItemName);
                Log.Info(o.ItemPos.ToObjectString());
            });
        }

        // TODO figure weird behaviour when inside buildings
        private async Task OnTick()
        {
            var playerPos = Game.PlayerPed.Position;
            droppedItems.ForEach(async o =>
            {
                var distanceToPlayer = o.ItemPos.DistanceToSquared(playerPos);

                if (distanceToPlayer < 200)
                {
                    if(!o.PosEdited)
                    {
                        var itemHeight = o.ItemPos.Z;
                        if (CitizenFX.Core.Native.API.IsValidInterior(CitizenFX.Core.Native.API.GetInteriorAtCoords(playerPos.X, playerPos.Y, playerPos.Z)))
                        {
                            itemHeight -= 1.0f;
                        }
                        else
                        {
                            itemHeight = World.GetGroundHeight(o.ItemPos);
                        }

                        if (itemHeight > o.ItemPos.Z)
                        {
                            itemHeight = o.ItemPos.Z - 1.0f;
                        }
                        o.ItemPos.Z = itemHeight + 0.04f;
                        o.PosEdited = true;
                    }

                    World.DrawMarker(MarkerType.HorizontalCircleFat, o.ItemPos, Vector3.Down, Vector3.Down, new Vector3(0.5f, 0.5f, 0.5f), Color.FromArgb(120, ConstantColours.Inventory));
                }

                if (distanceToPlayer < 6)
                {
                    CitizenFX.Core.UI.Screen.ShowSubtitle($"Press ~g~E~s~ to pickup {o.ItemName} ({o.ItemAmount}x)", 100);
                    if (Input.IsControlJustPressed(Control.Pickup))
                    {
                        Log.Info($"About to pickup ItemId {o.ItemId}");
                        Client.TriggerServerEvent("Items.RequestItemPickup", o.ItemId);
                        Client.DeregisterTickHandler(OnTick);
                        await pickupAnimation.PlayFullAnim();
                        await BaseScript.Delay(250);
                        Client.RegisterTickHandler(OnTick);
                    }
                }
            });
        }
    }
}
