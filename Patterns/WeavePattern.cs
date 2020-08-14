﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using MetrixGroupPlugins.Utilities;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class WeavePattern : PerforationPattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public WeavePattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public WeavePattern(bool addTools)
        {
            Name = "Weave";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();
                PunchingTools.Weave tool1 = new PunchingTools.Weave();
                tool1.DisplayName = "Tool 1";
                punchingToolList.Add(tool1);
            }
        }

        /// <summary>
        /// Gets or sets the spacing y.
        /// </summary>
        /// <value>
        /// The spacing y.
        /// </value>
        public override double YSpacing
        {
            get
            {
                return XSpacing;
            }
        }

        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            PointMap pointMap1 = new PointMap();
            PointMap pointMap2 = new PointMap();
            Random random = new Random();

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / XSpacing)) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;
            double YSpacing = XSpacing;

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

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
                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                    {
                        if (y % 2 == 0)
                        {
                            if (x % 2 == 0)
                            {
                                punchingToolList[0].drawTool(point, 0);
                                pointMap1.AddPoint(new PunchingPoint(point));
                            }
                            else
                            {
                                punchingToolList[0].drawTool(point, Math.PI / 2);
                                pointMap2.AddPoint(new PunchingPoint(point));
                            }
                        }
                        else
                        {
                            if (x % 2 == 0)
                            {
                                punchingToolList[0].drawTool(point, Math.PI / 2);
                                pointMap2.AddPoint(new PunchingPoint(point));
                            }
                            else
                            {
                                punchingToolList[0].drawTool(point, 0);
                                pointMap1.AddPoint(new PunchingPoint(point));
                            }
                        }
                        //pointMap.AddPoint(new PunchingPoint(point));
                        //punchingToolList[0].drawTool(point);
                        //}
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = punchingToolList[0].getArea() * (pointMap1.Count + pointMap2.Count);

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
                    punchingToolList[i].ClusterTool.Shape = ClusterTool.ClusterToolShape.RectangleSmall;
                    punchingToolList[i].Angle = Math.PI / 2;

                    drawCluster(pointMap2, punchingToolList[i]);

                    punchingToolList[i].Angle = 0;
                    punchingToolList[i].ClusterTool.Shape = ClusterTool.ClusterToolShape.Rectangle90Small;
                    drawCluster(pointMap1, punchingToolList[i]);

                    punchingToolList[i].Angle = 0;
                }
            }

            doc.Views.Redraw();

            // Set the active layer back to where it was
            doc.Layers.SetCurrentLayerIndex(currentLayer, true);

            return openArea;
        }


    }
}
