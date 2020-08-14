using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using MetrixGroupPlugins.RandomTileEngine;
using MetrixGroupPlugins.Utilities;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class StraightPattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public StraightPattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public StraightPattern(bool addTools)
        {
            Name = "Straight";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();

                PunchingTools.Round tool1 = new PunchingTools.Round();
                tool1.DisplayName = "Tool 1";
                punchingToolList.Add(tool1);
            }
        }

        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            PointMap pointMap = new PointMap();

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / XSpacing)) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;
            Point3d origin = new Point3d(firstX, firstY, 0);

            // Record the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create Perforation Layer
       //     if (punchingToolList[0].ClusterTool.Enable == true)
            {
                doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            }
            //else
            //{
            //    if (!MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
            //    {
            //        RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //    }
            //    else
            //    {
            //        RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //    }
            //}

            // Random Engine

            RandomTiler randomTileEngine = new RandomTiler();

            int totalQty = punchQtyX * punchQtyY;
            int toolHitQty = (int)(totalQty * randomness);
            int blankQty = totalQty - toolHitQty;

            List<int> tileCounts = new List<int>();
            tileCounts.Add(toolHitQty);
            tileCounts.Add(blankQty);
            int[,] tileMap = randomTileEngine.GetTileMap(tileCounts, punchQtyX, punchQtyY);

            // Add first point
            random = new Random();

            for (int y = 0; y < punchQtyY; y++)
            {
                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                    {
                        if (tileMap[x, y] == 1)
                        {
                            pointMap.AddPoint(new PunchingPoint(point));
                            punchingToolList[0].drawTool(point);
                        }
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = punchingToolList[0].getArea() * pointMap.Count;

            RhinoApp.WriteLine("Tool area: {0} mm^2", toolArea.ToString("#.##"));

            openArea = toolArea * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            // Draw the cluster for each tool 
            for (int i = 0; i < punchingToolList.Count; i++)
            {
                // Only draw cluster tool if it is enable
                if (punchingToolList[i].ClusterTool.Enable == true)
                {
                    // Draw the cluster tool
                    drawCluster(pointMap, punchingToolList[i]);
                }
            }

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
