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
    public class TrianglePattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public TrianglePattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public TrianglePattern(bool addTools)
        {
            Name = "Triangle";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();

                PunchingTools.EqTriangle tool1 = new PunchingTools.EqTriangle();
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

            double d = Math.Tan(Math.PI / 6) * (XSpacing - punchingToolList[0].X) / 2;
            double rowHeight = (Math.Sqrt(3) * punchingToolList[0].X / 2) + d;
            double h = (Math.Sqrt(3) * punchingToolList[0].X / 2);
            double g = d / Math.Sin(Math.PI / 6);

            YSpacing = rowHeight + g;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / (XSpacing / 2))) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * (XSpacing / 2))) / 2;

            int punchQtyY = ((int)((spanY - rowHeight) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;
            Point3d origin = new Point3d(firstX, firstY, 0);

            // Record the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create Perforation Layer
         //   if (punchingToolList[0].ClusterTool.Enable == true)
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

            for (int y = 0; y < punchQtyY; y++)
            {
                if (y % 2 == 0) // even rows
                {
                    for (int x = 0; x < punchQtyX; x++)
                    {
                        if (x % 2 == 0) // even location
                        {
                            point = new Point3d(firstX + x * (XSpacing / 2), firstY + y * YSpacing - (rowHeight / 2) + (h / 3), 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point, 0) == true)
                            {
                                pointMap.AddPoint(new PunchingPoint(point));
                                punchingToolList[0].drawTool(point, 0);
                            }
                        }
                        else
                        {
                            point = new Point3d(firstX + x * (XSpacing / 2), firstY + y * YSpacing + (rowHeight / 2) - (h / 3), 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point, Math.PI) == true)
                            {
                                pointMap.AddPoint(new PunchingPoint(point));
                                punchingToolList[0].drawTool(point, Math.PI);
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < punchQtyX; x++)
                    {
                        if (x % 2 == 0) // even location
                        {
                            point = new Point3d(firstX + x * (XSpacing / 2), firstY + y * YSpacing + (rowHeight / 2) - (h / 3), 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point, Math.PI) == true)
                            {
                                pointMap.AddPoint(new PunchingPoint(point));
                                punchingToolList[0].drawTool(point, Math.PI);
                            }
                        }
                        else
                        {
                            point = new Point3d(firstX + x * (XSpacing / 2), firstY + y * YSpacing - (rowHeight / 2) + (h / 3), 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point, 0) == true)
                            {
                                pointMap.AddPoint(new PunchingPoint(point));
                                punchingToolList[0].drawTool(point, 0);
                            }
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
