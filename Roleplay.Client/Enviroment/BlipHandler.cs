using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Enviroment
{
    public static class BlipHandler
    {
        private static BlipOptions defaultBlipOptions = new BlipOptions();

        static BlipHandler()
        {
            Client.Instance.RegisterEventHandler("Blips.AddBlip", new Action<string, List<object>, string>((blipName, blipPosition, blipOptions) =>
            {
                AddBlip(blipName, blipPosition.ToVector3(), blipOptions == null ? null : JsonConvert.DeserializeObject<BlipOptions>(blipOptions));
            }));
            Client.Instance.RegisterEventHandler("Blips.AddBlips", new Action<string, List<object>, string>((blipName, blipPositions, blipOptions) =>
            {
                AddBlip(blipName, blipPositions.Select(o => ((List<object>)o).ToVector3()).ToList(), blipOptions == null ? null : JsonConvert.DeserializeObject<BlipOptions>(blipOptions));
            }));
        }

        public static Blip AddBlip(string blipName, Vector3 location, BlipOptions options = null)
        {
            var blipOptions = options ?? defaultBlipOptions;

            var blip = World.CreateBlip(location);
            blip.Sprite = blipOptions.Sprite;
            blip.Scale = blipOptions.Scale;
            blip.Color = blipOptions.Colour;
            blip.IsShortRange = blipOptions.IsShortRange;
            blip.Name = blipName;

            return blip;
        }

        public static List<Blip> AddBlip(string blipName, List<Vector3> locations, BlipOptions options = null)
        {
            var blipList = new List<Blip>();

            locations.ForEach(o =>
            {
                blipList.Add(AddBlip(blipName, o, options));
            });

            return blipList;
        }

        public static async Task<List<Blip>> AddBlipAsync(string blipName, List<Vector3> locations, BlipOptions options = null)
        {
            var blipList = new List<Blip>();

            await locations.ForEachAsync(async o =>
            {
                blipList.Add(AddBlip(blipName, o, options));
                await BaseScript.Delay(0);
            });

            return blipList;
        }
    }
}
