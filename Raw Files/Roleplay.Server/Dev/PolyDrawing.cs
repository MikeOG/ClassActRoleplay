using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roleplay.Server.Helpers;
using Roleplay.Shared;
using Roleplay.Shared.Helpers;

namespace Roleplay.Server.Dev
{
    public class PolyDrawing : ServerAccessor
    {
        private DevEnviroment Dev => Server.Get<DevEnviroment>();

        public PolyDrawing(Server server) : base(server)
        {
            Dev.RegisterDevCommand("drawpoly", OnDrawPoly);
            Dev.RegisterDevCommand("endpoly", OnEndPoly);
            Dev.RegisterDevCommand("endpolypreview", cmd => cmd.Session.TriggerEvent("Dev.Poly.EndPolyDrawing"));
            Dev.RegisterDevCommand("polyp", cmd => AddPolyPoint(cmd.Session));
        }

        private void OnDrawPoly(Command cmd)
        {
            var playerSession = cmd.Session;
            if(!playerSession.GetServerData("Dev.IsDrawingPoly", false))
            {
                Log.Debug($"Starting polygon drawing for {playerSession.PlayerName}");
                Log.ToClient("[Dev]", "Starting polygon drawing", ConstantColours.Admin, cmd.Player);
                playerSession.SetServerData("Dev.IsDrawingPoly", true);
                playerSession.SetServerData("Dev.PolyPoints", new List<float[]>());
                playerSession.TriggerEvent("Dev.Poly.StartPolyDrawing");
                AddPolyPoint(playerSession);
            }
            else
            {
                Log.ToClient("[Dev]", "You are already drawing a polygon! Do /endpoly to end your current creation", ConstantColours.Admin, cmd.Player);
            }
        }

        private void OnEndPoly(Command cmd)
        {
            if (cmd.Session.GetServerData("Dev.IsDrawingPoly", false))
            {
                var polyPoints = cmd.Session.GetServerData("Dev.PolyPoints", new List<float[]>());

                if(polyPoints.Count >= 3)
                {
                    var polyString = "new float[][]\n{";
                    polyPoints.ForEach(point =>
                    {
                        polyString += $"\n\tnew[]{{ {point[0]}f, {point[1]}f }},";
                    });
                    polyString += "\n},";

                    Log.Info(polyString);

                    cmd.Session.TriggerEvent("Dev.Poly.AddFinalPoint", polyPoints.First(), polyPoints.Last());
                    cmd.Session.SetServerData("Dev.IsDrawingPoly", false);
                }
                else
                {
                    Log.ToClient("[Dev]", $"There must be at least 3 points to the shape you are drawing", ConstantColours.Admin, cmd.Session.Source);
                }
            }
        }

        private void AddPolyPoint(Session.Session playerSession)
        {
            if (playerSession.GetServerData("Dev.IsDrawingPoly", false))
            {
                var playerPos = playerSession.GetPlayerPosition();
                var polyPos = new[] { playerPos.X, playerPos.Y };

                Log.Debug($"Adding a position of X: {polyPos[0]} Y: {polyPos[1]} for {playerSession.PlayerName} current poly");
                Log.ToClient("[Dev]", $"Adding position X: {polyPos[0]} Y: {polyPos[1]}", ConstantColours.Admin, playerSession.Source);
                var currentPolys = playerSession.GetServerData("Dev.PolyPoints", new List<float[]>());
                currentPolys.Add(polyPos);

                playerSession.SetServerData("Dev.PolyPoints", currentPolys);
                playerSession.TriggerEvent("Dev.Poly.AddDrawingPoint", polyPos);
            }
        }
    }
}
