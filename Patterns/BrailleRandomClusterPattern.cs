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
    public class BrailleRandomClusterPattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public BrailleRandomClusterPattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public BrailleRandomClusterPattern(bool addTools)
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
            PointMap pointMap = new PointMap();

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            int punchQtyX = ((int)((spanX - punchingToolList[1].X) / XSpacing)) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;
            double YSpacing = XSpacing;

            int punchQtyY = ((int)((spanY - punchingToolList[1].Y) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

            // Record current layer 
            int currentLayer = doc.Layers.CurrentLayerIndex;
                      
            int tool0Count = 0;

            for (int y = 0; y < punchQtyY; y++)
            {
                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                    {
                        pointMap.AddPoint(new PunchingPoint(point));
                        // punchingToolList[0].drawTool(point);
                        tool0Count++;
                    }
                    else
                    {
                        punchingToolList[0].drawTool(point);
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double tool0Area = punchingToolList[0].getArea() * tool0Count;

            RhinoApp.WriteLine("Tool 1 area: {0} mm^2", tool0Area.ToString("#.##"));

            //double tool1Area = punchingToolList[1].getArea() * tool1Count;

            //RhinoApp.WriteLine("Tool 2 area: {0} mm^2", tool1Area.ToString("#.##"));

            //double openArea = (tool0Area + tool1Area) * 100 / area.Area;

            // RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return 0;
        }
    }
}
