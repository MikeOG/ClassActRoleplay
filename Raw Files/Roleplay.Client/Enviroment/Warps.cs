using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Roleplay.Client.Jobs;
using Roleplay.Shared.Enums;
using Roleplay.Shared.Helpers;

namespace Roleplay.Client.Enviroment
{
    public class Warps : ClientAccessor
    {
        public Warps(Client client) : base(client)
        {
            var dirtyMoneyWarp = new WarpPoint(new Vector3(2818.56f, 1463.43f, 24.74f), new Vector3(1118.71f, -3193.71f, -41.35f), ConstantColours.Red);
            var beachroofWarp = new WarpPoint(new Vector3(-1384.91f, -976.2f, 8.94f), new Vector3(-1399.09f, -986.07f, 19.38f), ConstantColours.Red);
            var lifeinvaderroofWarp = new WarpPoint(new Vector3(-1096.2f, -324.88f, 37.82f), new Vector3(-1112.97f, -335.53f, 50.02f), ConstantColours.Red);
            var bahamamamasWarp = new WarpPoint(new Vector3(-1388.65f, -586.64f, 30.22f), new Vector3(-1387.35f, -588.66f, 30.32f), ConstantColours.Red);
            var VuWarp = new WarpPoint(new Vector3(129.71f, -1287.73f, 29.24f), new Vector3(132.28f, -1287.17f, 29.24f), ConstantColours.Red);
            var bahamamamas2Warp = new WarpPoint(new Vector3(-1388.73f, -610.73f, 30.30f), new Vector3(-1387.4f, -609.12f, 30.30f), ConstantColours.Red);
            var blackoutWarp = new WarpPoint(new Vector3(1208.71f, -454.83f, 66.69f), new Vector3(-1569.4f, -3017f, -74.41f), ConstantColours.Red);
            var skiliftWarp = new WarpPoint(new Vector3(-741.05f, 5593.21f, 41.65f), new Vector3(445.93f, 5569.6f, 781.19f), ConstantColours.Red);
            var PsychologistHouse = new WarpPoint(new Vector3(-314.98f, -3.31f, 48.18f), new Vector3(-1910.834f, -575.006f, 19.097f), ConstantColours.Red);

            //General store back doors.
            var strawberrygeneralstoreWarp = new WarpPoint(new Vector3(31.38f, -1340.55f, 29.5f), new Vector3(51.52f, -1318.1f, 29.29f), ConstantColours.Red);
            var grovegeneralstoreWarp = new WarpPoint(new Vector3(-41.79f, -1748.81f, 29.42f), new Vector3(-41.21f, -1748.16f, 29.57f), ConstantColours.Red);
            var senorageneralstoreWarp = new WarpPoint(new Vector3(2675.71f, 3288.73f, 55.24f), new Vector3(2670.89f, 3286.4f, 55.24f), ConstantColours.Red);
            var mirrorparkgeneralstoreWarp = new WarpPoint(new Vector3(1161.19f, -313.09f, 69.21f), new Vector3(1160.61f, -312.16f, 69.35f), ConstantColours.Red);
            var northrockforddrgeneralstoreWarp = new WarpPoint(new Vector3(-1828.34f, 800.48f, 138.16f), new Vector3(-1829.13f, 801.23f, 138.41f), ConstantColours.Red);
            var route68generalstoreWarp = new WarpPoint(new Vector3(543.07f, 2663.93f, 42.16f), new Vector3(542.18f, 2663.88f, 42.36f), ConstantColours.Red);
            var tataviamgeneralstoreWarp = new WarpPoint(new Vector3(2550.85f, 387.99f, 108.62f), new Vector3(2546.52f, 385.67f, 108.62f), ConstantColours.Red);
            var downtowngeneralstoreWarp = new WarpPoint(new Vector3(380.94f, 331.4f, 103.57f), new Vector3(396.38f, 352.27f, 102.56f), ConstantColours.Red);
            var mtchiliadgeneralstoreWarp = new WarpPoint(new Vector3(1737.01f, 6418.11f, 35.04f), new Vector3(1741.65f, 6419.49f, 35.04f), ConstantColours.Red);
            var banhamgeneralstoreWarp = new WarpPoint(new Vector3(-3047.3f, 589.06f, 7.91f), new Vector3(-3047.76f, 590.64f, 7.76f), ConstantColours.Red);
            var grapeseedgeneralstoreWarp = new WarpPoint(new Vector3(1707.42f, 4918.67f, 42.06f), new Vector3(1702.64f, 4917.05f, 42.08f), ConstantColours.Red);
        }
    }
}
