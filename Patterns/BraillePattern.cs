using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using System.Drawing;
using MetrixGroupPlugins.Utilities;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class BraillePattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public BraillePattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public BraillePattern(bool addTools)
        {
            Name = "Braille";

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
            List<PointMap> pointMapList = new List<PointMap>();

            // Add two Point Map in the list
            pointMapList.Add(new PointMap());
            pointMapList.Add(new PointMap());

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

            // Set the layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            Random random = new Random();
            int tool0Count = 0;
            int tool1Count = 0;

            for (int y = 0; y < punchQtyY; y++)
            {
                for (int x = 0; x < punchQtyX; x++)
                {
                    // Locate the point
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    // If the randomness is bigger than random, put small tool
                    if (random.NextDouble() < randomness)
                    {
                        if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                        {
                            pointMapList[1].AddPoint(new PunchingPoint(point));
                            punchingToolList[1].drawTool(point);
                            tool1Count++;
                            if (MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
                            {
                                RhinoUtilities.SetActiveLayer("TemporaryTool2Layer", System.Drawing.Color.Yellow);
                                punchingToolList[1].drawTool(point);
                                doc.Layers.SetCurrentLayerIndex(currentLayer, true);
                            }
                            else
                            {
                                RhinoUtilities.SetActiveLayer("Tool 2 Layer", System.Drawing.Color.Yellow);
                                punchingToolList[1].drawTool(point);
                                RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
                            }
                        }
                    }
                    else
                    {
                        if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                        {
                            if (punchingToolList[1].ClusterTool.Enable == true)
                            {
                                pointMapList[1].AddPoint(new PunchingPoint(point));
                            }

                            if (MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
                            {
                                RhinoUtilities.SetActiveLayer("TemporaryTool2Layer", System.Drawing.Color.Yellow);
                                punchingToolList[1].drawTool(point);
                                doc.Layers.SetCurrentLayerIndex(currentLayer, true);
                            }
                            else
                            {
                                RhinoUtilities.SetActiveLayer("Tool 2 Layer", System.Drawing.Color.Yellow);
                                punchingToolList[1].drawTool(point);
                                RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
                             }
                            pointMapList[0].AddPoint(new PunchingPoint(point));
                            punchingToolList[0].drawTool(point);
                            tool0Count++;
                        }
                    }
                }
            }

            // Draw the cluster for each tool 
            for (int i = 0; i < punchingToolList.Count; i++)
            {
                // Only draw cluster tool if it is enable
                if (punchingToolList[i].ClusterTool.Enable == true)
                {
                    drawCluster(pointMapList[i], punchingToolList[i]);
                }
                else
                {
                    // Set to Tool Hit Layer
                    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, Color.Black);

                    // Places where cannot cluster punch
                    for (int k = 0; k < pointMapList[i].YCount; k++)
                    {
                        SortedDictionary<int, PunchingPoint> xDict = pointMapList[i].getXDictionary(k);

                        for (int m = 0; m < xDict.Count; m++)
                        {
                            if (xDict.ElementAt(m).Value.HasPunched == false)
                            {
                                // Draw the tool 
                                punchingToolList[i].drawTool(xDict.ElementAt(m).Value.Point);
                            }
                        }
                    }

                    doc.Views.Redraw();
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double tool0Area = punchingToolList[0].getArea() * tool0Count;

            RhinoApp.WriteLine("Tool 1 area: {0} mm^2", tool0Area.ToString("#.##"));

            double tool1Area = punchingToolList[1].getArea() * tool1Count;

            RhinoApp.WriteLine("Tool 2 area: {0} mm^2", tool1Area.ToString("#.##"));

            openArea = (tool0Area + tool1Area) * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
