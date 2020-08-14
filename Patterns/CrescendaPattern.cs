using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class CrescendaPattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrescendaPattern"/> class.
        /// </summary>
        public CrescendaPattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrescendaPattern"/> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public CrescendaPattern(bool addTools)
        {
            Name = "Crescenda";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();

                PunchingTools.Round tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 1";
                punchingToolList.Add(tool);
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
                return punchingToolList[0].X + punchingToolList[0].Y;
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
                return punchingToolList[0].Y * 2;
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

            int currentLayer = doc.Layers.CurrentLayerIndex;

            // Create the grid
            Point3d?[,] grid = new Point3d?[punchQtyX, punchQtyY];

            // Active points
            List<Point3d> activePointsList = new List<Point3d>();

            // Point list
            List<Point3d> pointsList = new List<Point3d>();

            //doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Add first point
            random = new Random();

            for (int y = 0; y < punchQtyY; y++)
            {
                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                    {
                        if (!grid[x, y].HasValue)
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
