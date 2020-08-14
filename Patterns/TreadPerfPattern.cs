using System;
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
    public class TreadPerfPattern : PerforationPattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FourtyFiveDegreePattern"/> class.
        /// </summary>
        public TreadPerfPattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public TreadPerfPattern(bool addTools)
        {
            Name = "TreadPerfPattern";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();

                PunchingTools.Round tool1 = new PunchingTools.Round();
                tool1.DisplayName = "Tool 1";
                punchingToolList.Add(tool1);

                PunchingTools.Round tool2 = new PunchingTools.Round();
                tool2.DisplayName = "Tool 2";
                punchingToolList.Add(tool2);

                PunchingTools.Round tool3 = new PunchingTools.Round();
                tool3.DisplayName = "Tool 3";
                punchingToolList.Add(tool3);
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
                return XSpacing / 2;
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
            PointMap pointMapTool2 = new PointMap();

            pointMapList.Add(pointMapTool1);

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            // Span are the total length of the area
            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            // Number of holes that can be punch in X.
            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / XSpacing)) + 1;

            double secondRowOffset = XSpacing / 2;

            // Calculate the margin - is the distance from the left border
            double marginX;

            if (spanX >= ((punchQtyX - 1) * XSpacing + secondRowOffset + (punchingToolList[0].X / 2) + (punchingToolList[2].X / 2)))
            {
                marginX = (spanX - ((punchQtyX - 1) * XSpacing) - secondRowOffset - ((punchingToolList[0].X / 2) + (punchingToolList[2].X / 2))) / 2 + (punchingToolList[0].X / 2);
            }
            else
            {
                marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;
            }

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / YSpacing)) + 1;
            double marginY = 0;

            if (punchQtyY % 2 == 0) // Even
            {
                // Make sure the Tool can fit if the row is even row
                punchQtyY = ((int)((spanY - (punchingToolList[0].Y / 2) - (punchingToolList[2].Y / 2)) / YSpacing)) + 1;
            }

            if (punchQtyY % 2 == 0) // Even Row
            {
                marginY = (spanY - ((punchQtyY - 1) * YSpacing) - (punchingToolList[0].Y / 2) - (punchingToolList[2].Y / 2)) / 2 + (punchingToolList[0].Y / 2);
            }
            else
            {
                marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;
            }

            Point3d point;

            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

            // Record the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create Perforation Layer
        //    if (punchingToolList[0].ClusterTool.Enable == true)
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
                        point = new Point3d(firstX + (x * XSpacing) + secondRowOffset, firstY + y * YSpacing, 0);

                        if (punchingToolList[2].isInside(boundaryCurve, point) == true)
                        {
                            if (random.NextDouble() < randomness)
                            {
                                pointMapTool2.AddPoint(new PunchingPoint(point));
                                punchingToolList[1].drawTool(point);
                                punchingToolList[2].drawTool(point);
                            }
                        }
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double tool1Area = punchingToolList[0].getArea() * pointMapTool1.Count;

            RhinoApp.WriteLine("Tool 1 area: {0} mm^2", tool1Area.ToString("#.##"));

            double tool2Area = punchingToolList[1].getArea() * pointMapTool2.Count;

            RhinoApp.WriteLine("Tool 2 area: {0} mm^2", tool2Area.ToString("#.##"));

            openArea = (tool1Area + tool2Area) * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            doc.Views.Redraw();

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

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);

            return openArea;
        }
    }
}
