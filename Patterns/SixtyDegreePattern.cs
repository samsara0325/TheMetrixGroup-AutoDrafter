﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using MetrixGroupPlugins.Utilities;
using System.Windows.Forms;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class SixtyDegreePattern : PerforationPattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SixtyDegreePattern"/> class.
        /// </summary>
        public SixtyDegreePattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public SixtyDegreePattern(bool addTools)
        {
            Name = "60 degrees";

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
        /// Gets or sets the spacing y.
        /// </summary>
        /// <value>
        /// The spacing y.
        /// </value>
        public override double YSpacing
        {
            get
            {
                return XSpacing * Math.Sin(Math.PI * 60 / 180);
            }
        }

        /// <summary>
        /// Gets or sets the spacing x.
        /// </summary>
        /// <value>
        /// The spacing x.
        /// </value>
        public override double XSpacing
        {
            get
            {
                return base.XSpacing;
            }
            set
            {
                base.XSpacing = value;
                base.YSpacing = base.XSpacing * Math.Sin(Math.PI * 60 / 180);
                base.Pitch = value;
            }
        }

        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            List<PointMap> pointMapList = new List<PointMap>();
            Random random = new Random();
            PointMap pointMapTool1 = new PointMap();

            pointMapList.Add(pointMapTool1);

            double marginX;

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / XSpacing)) + 1;

            double secondRowOffset = pitch * Math.Cos(Math.PI * 60 / 180);

            if (spanX >= ((punchQtyX - 1) * XSpacing + secondRowOffset + punchingToolList[0].X))
            {
                marginX = (spanX - ((punchQtyX - 1) * XSpacing) - secondRowOffset) / 2;
            }
            else
            {
                marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;
            }

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / YSpacing)) + 1;

            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;

            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

            // Record the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create Perforation Layer
          //  if (punchingToolList[0].ClusterTool.Enable == true)
            {
                // RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
                doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            }
            //else
            //{
            //    if (MetrixUtilities.IsLayerFound("PerfTemporaryLayer"))
            //    {
            //        RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //    }
            //    else
            //    {
            //        RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //    }
            //}
            bool drawSimpleToolHit = true;
            for (int i = 0; i < punchingToolList.Count; i++)
            {

                // Only draw cluster tool if it is enable
                if (punchingToolList[i].ClusterTool.Enable == true)
                {
                    // Draw the cluster tool
                    drawSimpleToolHit = false;
                    break;
                }
            }
            if (drawSimpleToolHit)
            {
                int currentlayer = doc.Layers.Find("Tool Hit", true);
                if(currentlayer < 0)
                {
                    currentlayer = doc.Layers.Add("Tool Hit", System.Drawing.Color.Black);
                }
                doc.Layers.SetCurrentLayerIndex(currentlayer, true);
                for (int y = 0; y < punchQtyY; y++)
                {
                    if (y % 2 == 0) // even rows
                    {
                        for (int x = 0; x < punchQtyX; x++)
                        {
                            point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                            {
                                if (random.NextDouble() < randomness)
                                {
                                    pointMapTool1.AddPoint(new PunchingPoint(point));
                                 
                                        punchingToolList[0].drawTool(point);
                                    
                                }
                            }
                        }
                    }
                    else // odd rows
                    {
                        for (int x = 0; x < punchQtyX; x++)
                        {
                            point = new Point3d(firstX + secondRowOffset + (x * XSpacing), firstY + y * YSpacing, 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                            {
                                if (random.NextDouble() < randomness)
                                {
                                    pointMapTool1.AddPoint(new PunchingPoint(point));
                                        punchingToolList[0].drawTool(point);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < punchQtyY; y++)
                {
                    if (y % 2 == 0) // even rows
                    {
                        for (int x = 0; x < punchQtyX; x++)
                        {
                            point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                            {
                                if (random.NextDouble() < randomness)
                                {
                                    pointMapTool1.AddPoint(new PunchingPoint(point));
                                    if (PunchingToolList[0].Perforation)
                                    {
                                        punchingToolList[0].drawTool(point);
                                    }
                                }
                            }
                        }
                    }
                    else // odd rows
                    {
                        for (int x = 0; x < punchQtyX; x++)
                        {
                            point = new Point3d(firstX + secondRowOffset + (x * XSpacing), firstY + y * YSpacing, 0);

                            if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                            {
                                if (random.NextDouble() < randomness)
                                {
                                    pointMapTool1.AddPoint(new PunchingPoint(point));
                                    if (PunchingToolList[0].Perforation)
                                    {
                                        punchingToolList[0].drawTool(point);
                                    }
                                }
                            }
                        }
                    }
                }
            }
    
            

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = punchingToolList[0].getArea() * pointMapTool1.Count;

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
                    drawCluster(pointMapList[i], punchingToolList[i]);
                }
            }

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
