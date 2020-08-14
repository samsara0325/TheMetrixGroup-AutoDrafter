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
    public class MatrixPattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public MatrixPattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public MatrixPattern(bool addTools)
        {
            Name = "Matrix";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();
                PunchingTools.Round tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 1";
                punchingToolList.Add(tool);
                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 2";
                punchingToolList.Add(tool);
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

            double maxToolSizeX;
            double maxToolSizeY;

            // Find the max tool size first
            if (punchingToolList[0].X > punchingToolList[1].X)
            {
                maxToolSizeX = punchingToolList[0].X;
            }
            else
            {
                maxToolSizeX = punchingToolList[1].X;
            }

            if (punchingToolList[0].Y > punchingToolList[1].Y)
            {
                maxToolSizeY = punchingToolList[0].Y;
            }
            else
            {
                maxToolSizeY = punchingToolList[1].Y;
            }

            int punchQtyX = ((int)((spanX - maxToolSizeX) / XSpacing)) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;

            int punchQtyY = ((int)((spanY - maxToolSizeY) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

            // Record the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create Perforation Layer
           // if (punchingToolList[0].ClusterTool.Enable == true)
            {
                doc.Layers.SetCurrentLayerIndex(currentLayer, false);
            }
            //else
            //{
            //    if (MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
            //    {
            //        RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //    }
            //    else
            //    {
            //        RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //    }
            //}

            int toolHitCounter = 0;
            bool isTool1 = true;

            for (int y = 0; y < punchQtyY; y++)
            {
                toolHitCounter = 0;
                isTool1 = true;

                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (isTool1 == true)
                    {
                        // Tool 1
                        if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                        {
                            pointMapList[0].AddPoint(new PunchingPoint(point));
                            punchingToolList[0].drawTool(point);
                        }
                    }
                    else
                    {
                        // Tool 2
                        if (punchingToolList[1].isInside(boundaryCurve, point) == true)
                        {
                            pointMapList[1].AddPoint(new PunchingPoint(point));
                            punchingToolList[1].drawTool(point);
                        }
                    }

                    toolHitCounter++;

                    if (toolHitCounter >= 4)
                    {
                        isTool1 = !isTool1;
                        toolHitCounter = 0;
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = punchingToolList[0].getArea() * pointMapList[0].Count + punchingToolList[1].getArea() * pointMapList[1].Count;

            RhinoApp.WriteLine("Tool area: {0} mm^2", toolArea.ToString("#.##"));

            openArea = toolArea * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            // Draw the cluster for each tool 
            for (int i = 0; i < punchingToolList.Count; i++)
            {
                // Only draw cluster tool if it is enable
                if (punchingToolList[i].ClusterTool.Enable == true)
                {
                    if (punchingToolList[i].ClusterTool.Rotatable == true)
                    {  // Draw the cluster tool
                        drawRotatedCluster(pointMapList[i], punchingToolList[i]);
                    }
                    else
                    {
                        drawCluster(pointMapList[i], punchingToolList[i]);
                    }
                }
            }

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
