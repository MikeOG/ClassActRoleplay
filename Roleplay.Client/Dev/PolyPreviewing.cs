using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Roleplay.Client.Dev
{
    public class PolyPreviewing : ClientAccessor
    {
        private float[] previousPoint = null;
        private float[] currentPoint = null;
        public static List<PolyLine> lines = new List<PolyLine>();

        public PolyPreviewing(Client client) : base(client)
        {
            client.RegisterEventHandler("Dev.Poly.StartPolyDrawing", new Action(() => client.RegisterTickHandler(DrawLineTick)));
            client.RegisterEventHandler("Dev.Poly.EndPolyDrawing", new Action(OnEndDraw));
            client.RegisterEventHandler("Dev.Poly.AddDrawingPoint", new Action<List<object>>(OnRecieveDrawingPoint));
            client.RegisterEventHandler("Dev.Poly.AddFinalPoint", new Action<List<object>, List<object>>(OnReceiveFinalPoint));
            
        }

        private void OnRecieveDrawingPoint(List<object> pointList)
        {
            var point = pointList.Select(Convert.ToSingle).ToArray();
            if (previousPoint == null)
                previousPoint = point;
            else
                currentPoint = point;

            if(previousPoint != null && currentPoint != null)
            {
                lines.Add(new PolyLine(previousPoint, currentPoint));
                previousPoint = currentPoint;
                currentPoint = null;
            }
        }

        private void OnReceiveFinalPoint(List<object> startList, List<object> endList)
        {
            var start = startList.Select(Convert.ToSingle).ToArray();
            var end = endList.Select(Convert.ToSingle).ToArray();

            lines.Add(new PolyLine(start, end));
        }

        private void OnEndDraw()
        {
            Client.DeregisterTickHandler(DrawLineTick);
            lines = new List<PolyLine>();
            previousPoint = null;
            currentPoint = null;
        }

        private async Task DrawLineTick()
        {
            foreach (var line in lines)
            {
                line.Draw();
            }
        }
    }

    public class PolyLine
    {
        private float[] startPoint;
        private float[] endPoint;
        public float zAxisEnd;
        private float zAxisStart;

        public PolyLine(float[] startPoint, float[] endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.zAxisEnd = Cache.PlayerPed.Position.Z - 0.95f;
            this.zAxisStart = PolyPreviewing.lines.Count == 0 ? zAxisEnd : PolyPreviewing.lines.Last().zAxisEnd;
        }

        public void Draw()
        {
            DrawLine(startPoint[0], startPoint[1], zAxisStart, endPoint[0], endPoint[1], zAxisEnd, 255, 0, 0, 255);
        }
    }
}
