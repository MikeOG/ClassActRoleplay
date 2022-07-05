using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;
using Roleplay.Shared.Models;
using Newtonsoft.Json;

namespace Roleplay.Client.Enviroment
{
    public class Marker
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Direction { get; set; }
        public MarkerType Type { get; set; }
        public Vector3 Scale { get; set; }
        public System.Drawing.Color Color { get; set; }
        public string Id { get; set; }

        public Marker(Vector3 position, MarkerType type = MarkerType.VerticleCircle)
        {
            this.Position = position;
            this.Rotation = new Vector3(0, 0, 0);
            this.Direction = new Vector3(0, 0, 0);
            this.Color = System.Drawing.Color.FromArgb(140, 255, 255, 0);
            this.Type = type;
            this.Scale = 1.0f * new Vector3(1f, 1f, 1f);
        }

        public Marker(Vector3 position, MarkerType type, System.Drawing.Color color, float scale = 0.3f)
        {
            this.Position = position;
            this.Rotation = new Vector3(0, 0, 0);
            this.Direction = new Vector3(0, 0, 0);
            this.Color = System.Drawing.Color.FromArgb(120, color.R, color.G, Color.B);
            this.Type = type;
            this.Scale = scale * new Vector3(1f, 1f, 1f);
        }

        public Marker(Vector3 position, MarkerType type, System.Drawing.Color color, Vector3 scale, Vector3 rotation, Vector3 direction)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Direction = direction;
            this.Color = System.Drawing.Color.FromArgb(120, color.R, color.G, Color.B);
            this.Type = type;
            this.Scale = scale;
        }
    }

    internal static class MarkerHandler
    {
        public static bool ShowMarkers = true;
        public const float MaxDrawDistance = 200f;
        public static Dictionary<int, Marker> AllMarkers = new Dictionary<int, Marker>();
        public static List<Marker> CloseMarkers = new List<Marker>();

        static MarkerHandler()
        {
            Client.Instance.RegisterTickHandler(OnTick);
            Client.Instance.RegisterTickHandler(MarkerUpdater);
            Client.Instance.RegisterEventHandler("Markers.AddMarker", new Action<List<object>, string>((markerLocation, markerOptions) =>
            {
                AddMarker(markerLocation.ToVector3(), markerOptions == null ? null : JsonConvert.DeserializeObject<MarkerOptions>(markerOptions));
            }));
            Client.Instance.RegisterEventHandler("Markers.AddMarkers", new Action<List<object>, string>((markerLocations, markerOptions) =>
            {
                AddMarker(markerLocations.Select(o => ((List<object>)o).ToVector3()).ToList(), markerOptions == null ? null : JsonConvert.DeserializeObject<MarkerOptions>(markerOptions));
            }));
            Client.Instance.RegisterEventHandler("Markers.RemoveMarker", new Action<string>(RemoveMarker));
        }

        /// <summary>
        /// Adds a marker the will be drawn in the world when close enough to it
        /// </summary>
        /// <param name="marker"><see cref="Marker"/> object wanting to be added</param>
        /// <returns>The key of the marker in the marker dictionary that can be used to remove it if needed</returns>
        public static int AddMarker(Marker marker)
        {
            if (AllMarkers.Count == 0)
            {
                AllMarkers.Add(0, marker);
            }
            else
            {
                //if (AllMarkers.Any(o => o.Value.Position.X == marker.Position.X && o.Value.Position.Y == marker.Position.Y)) return -1;

                var oldMarker = AllMarkers.FirstOrDefault(o => !string.IsNullOrEmpty(o.Value.Id) && o.Value.Id == marker.Id).Key;

                if (oldMarker != default(int))
                {
                    AllMarkers.Remove(oldMarker);

                    return AddMarker(marker);
                }

                AllMarkers.Add(AllMarkers.Keys.Max() + 1, marker);
            }

            return AllMarkers.Keys.Max();
        }

        public static int AddMarker(Vector3 location, MarkerOptions options = null)
        {
            var markerOptions = options ?? new MarkerOptions();
            location.Z += markerOptions.zOffset;

            var marker = new Marker(location, markerOptions.Type, markerOptions.ColorInternal, markerOptions.Scale, markerOptions.Rotation, markerOptions.Direction)
            {
                Id = markerOptions.MarkerId
            };

            return AddMarker(marker);
        }

        public static List<int> AddMarker(List<Vector3> locations, MarkerOptions options = null)
        {
            var markerList = new List<int>();

            locations.ForEach(o =>
            {
                markerList.Add(AddMarker(o, options));
            });

            return markerList;
        }

        public static async Task<List<int>> AddMarkerAsync(List<Vector3> locations, MarkerOptions options = null)
        {
            var markerList = new List<int>();

            await locations.ForEachAsync(async o =>
            {
                markerList.Add(AddMarker(o, options));
                await BaseScript.Delay(0);
            });

            return markerList;
        }

        public static void RemoveMarker(string markerId)
        {
            var marker = AllMarkers.FirstOrDefault(o => !string.IsNullOrEmpty(o.Value.Id) && o.Value.Id == markerId).Key;

            if (marker != default(int))
            {
                AllMarkers.Remove(marker);
            }
        }

        public static void RemoveMarker(int markerId)
        {
            if (AllMarkers.ContainsKey(markerId))
            {
                AllMarkers.Remove(markerId);
            }
        }

        private static async Task OnTick()
        {
            if(ShowMarkers)
                CloseMarkers.ForEach(o => World.DrawMarker(o.Type, o.Position, o.Direction, o.Rotation, o.Scale, o.Color));
        }

        private static async Task MarkerUpdater()
        {
            CloseMarkers = AllMarkers.ToList().Select(o => o.Value).Where(o => o.Position.DistanceToSquared(Game.PlayerPed.Position) < Math.Pow(MaxDrawDistance, 2)).ToList();
            await BaseScript.Delay(1000);
        }
    }
}
