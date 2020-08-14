using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using MetrixGroupPlugins.RandomTileEngine;
using System.Drawing;
using MetrixGroupPlugins.Utilities;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class JazzPattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public JazzPattern()
        {

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
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public JazzPattern(bool addTools)
        {
            Name = "Jazz";

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

                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 3";
                punchingToolList.Add(tool);
            }
        }

        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            PointMap pointMap = new PointMap();
            int[] toolHitArray = Enumerable.Repeat(0, 3).ToArray();

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

            // Set the layer to perforation
            //if (!MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
            //{
            //    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //}
            //else
            //{
            //    RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //}
            // Random Engine
            RandomTiler randomTileEngine = new RandomTiler();

            int totalQty = punchQtyX * punchQtyY;
            // except the last tool percentage will be total - all tool hit
            List<double> toolHitPercentage = new List<double>(3);

            toolHitPercentage.Add(0.368);
            toolHitPercentage.Add(0.177);

            // Create randomness for tool count
            random = new Random();

            int toolHitCount = 0;

            // Create tile counts 
            List<int> tileCounts = new List<int>();

            foreach (double pc in toolHitPercentage)
            {
                int toolHitQty = (int)(totalQty * pc);
                tileCounts.Add(toolHitQty);
                toolHitCount = toolHitCount + toolHitQty;
            }

            tileCounts.Add(totalQty - toolHitCount);

            randomTileEngine.Weight = new int[5, 5]
            { { 1, 1, 2, 1, 1 },
         { 1, 2, 3, 2, 1 },
         { 2, 3, 0, 3, 2 },
         { 1, 2, 3, 2, 1 },
         { 1, 1, 2, 1, 1 } };



            int[,] tileMap = randomTileEngine.GetTileMap(tileCounts, punchQtyX, punchQtyY);
            int[] toolHitCounter = new int[3];

            for (int y = 0; y < punchQtyY; y++)
            {
                if (y % 2 == 0) // even rows
                {
                    for (int x = 0; x < punchQtyX; x++)
                    {
                        point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                        if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                        {
                            pointMap.AddPoint(new PunchingPoint(point));
                            punchingToolList[tileMap[x, y] - 1].drawTool(point);
                            toolHitArray[tileMap[x, y] - 1]++;
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
                            pointMap.AddPoint(new PunchingPoint(point));
                            punchingToolList[tileMap[x, y] - 1].drawTool(point);
                            toolHitArray[tileMap[x, y] - 1]++;
                        }
                    }
                }
            }

            //for (int y = 0; y < punchQtyY; y++)
            //{
            //   for (int x = 0; x < punchQtyX; x++)
            //   {
            //      point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

            //      if (punchingToolList[0].isInside(boundaryCurve, point) == true)
            //      {
            //         pointMap.AddPoint(new PunchingPoint(point));
            //         punchingToolList[tileMap[x, y]-1].drawTool(point);
            //         toolHitArray[tileMap[x, y]-1]++;
            //      }
            //   }
            //}

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);
            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = 0;
            double totalToolArea = 0;

            for (int i = 0; i < 3; i++)
            {
                toolArea = punchingToolList[i].getArea() * toolHitArray[i];
                totalToolArea = totalToolArea + toolArea;
                RhinoApp.WriteLine("Tool area{0}: {1} mm^2", i, toolArea.ToString("#.##"));
            }

            openArea = totalToolArea * 100 / area.Area;
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
