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
    public class ThirdStackPattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdStackPattern"/> class.
        /// </summary>
        public ThirdStackPattern()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdStackPattern"/> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public ThirdStackPattern(bool addTools)
        {
            Name = "Third Stack";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();
                PunchingTools.Rectangle tool = new PunchingTools.Rectangle();
                tool.X = 40.0 / 3;
                tool.Y = 8;
                tool.DisplayName = "Short";
                punchingToolList.Add(tool);

                // Add tool list 
                tool = new PunchingTools.Rectangle();
                tool.X = 95.0 / 3;
                tool.Y = 8;
                tool.DisplayName = "Medium";
                punchingToolList.Add(tool);

                // Add tool list 
                tool = new PunchingTools.Rectangle();
                tool.X = 50;
                tool.Y = 8;
                tool.DisplayName = "Long";
                punchingToolList.Add(tool);
            }
        }



        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <param name="boundaryCurve">The boundary curve.</param>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            List<PointMap> pointMapList = new List<PointMap>();
            Random random = new Random();
            PointMap pointMapTool1 = new PointMap();
            int tool0count = 0;
            int tool1count = 0;
            int tool2count = 0;
            double marginX;

            pointMapList.Add(pointMapTool1);

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            double gap = xSpacing - punchingToolList[2].X;

            // Maximum punching qty 
            int punchQtyX = ((int)((spanX - punchingToolList[2].X) / XSpacing)) + 1;

            // PunchQty need to add 1 if the last bit can fit the small rectangle
            if (spanX - (punchingToolList[2].X * punchQtyX) > (punchingToolList[0].X + gap))
            {
                punchQtyX = punchQtyX + 1;
            }

            // Find the location of the first punch
            if (spanX >= ((punchQtyX - 1) * XSpacing + punchingToolList[2].X))
            {
                marginX = (spanX - ((punchQtyX - 2) * XSpacing) - (punchingToolList[2].X) - gap) / 2;
            }
            else if (spanX >= ((punchQtyX - 1) * XSpacing + punchingToolList[1].X))
            {
                marginX = (spanX - ((punchQtyX - 2) * XSpacing) - (punchingToolList[1].X) - gap) / 2;
            }
            else
            {
                marginX = (spanX - ((punchQtyX - 2) * XSpacing) - (punchingToolList[0].X) - gap) / 2;
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
            //if (punchingToolList[0].ClusterTool.Enable == true)
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

            int totalQty = (punchQtyX + 1) * punchQtyY;
            int toolHitQty = (int)(totalQty * randomness);
            int blankQty = totalQty - toolHitQty;

            List<int> tileCounts = new List<int>();
            tileCounts.Add(toolHitQty);
            tileCounts.Add(blankQty);
            int[,] tileMap = randomTileEngine.GetTileMap(tileCounts, punchQtyX + 1, punchQtyY);

            Point3d pointMedL;
            Point3d pointMedR;
            Point3d pointShortL;
            Point3d pointShortR;
            Point3d pointLeft, pointRight, pointTop, pointBottom;
            double toolOffset = XSpacing / 6;
            double rowOffset = XSpacing / 3;


            for (int y = 0; y < punchQtyY; y++)
            {
                if (y % 3 == 0) // Long tool row
                {
                    // first X
                    firstX = min.X + marginX;
                }
                else if (y % 3 == 1) // Med tool row
                {
                    firstX = min.X + marginX - rowOffset;
                }
                else // short tool rows
                {
                    firstX = min.X + marginX - (2 * rowOffset);
                }

                bool isBorderPunch = false;

                for (int x = 0; x < punchQtyX + 1; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);
                    pointMedL = new Point3d(firstX + x * XSpacing - toolOffset, firstY + y * YSpacing, 0);
                    pointMedR = new Point3d(firstX + x * XSpacing + toolOffset, firstY + y * YSpacing, 0);
                    pointShortL = new Point3d(firstX + x * XSpacing - 2 * toolOffset, firstY + y * YSpacing, 0);
                    pointShortR = new Point3d(firstX + x * XSpacing + 2 * toolOffset, firstY + y * YSpacing, 0);

                    pointLeft = new Point3d(firstX + (x - 1) * XSpacing, firstY + y * YSpacing, 0);
                    pointRight = new Point3d(firstX + (x + 1) * XSpacing, firstY + y * YSpacing, 0);
                    pointTop = new Point3d(firstX + x * XSpacing, firstY + (y + 1) * YSpacing, 0);
                    pointBottom = new Point3d(firstX + (x + 1) * XSpacing, firstY + (y - 1) * YSpacing, 0);

                    // Test if the current punch is close to border
                    isBorderPunch = false;

                    if (punchingToolList[0].isInside(boundaryCurve, pointLeft) == false)
                    {
                        isBorderPunch = true;
                    }

                    if (punchingToolList[0].isInside(boundaryCurve, pointRight) == false)
                    {
                        isBorderPunch = true;
                    }

                    if (punchingToolList[0].isInside(boundaryCurve, pointTop) == false)
                    {
                        isBorderPunch = true;
                    }

                    if (punchingToolList[0].isInside(boundaryCurve, pointBottom) == false)
                    {
                        isBorderPunch = true;
                    }

                    if (isBorderPunch == true || tileMap[x, y] == 1)
                    {
                        if (punchingToolList[2].isInside(boundaryCurve, point) == true)
                        {
                            pointMapTool1.AddPoint(new PunchingPoint(point));
                            punchingToolList[2].drawTool(point);
                            tool2count++;
                        }
                        else if (punchingToolList[1].isInside(boundaryCurve, pointMedL) == true)
                        {
                            pointMapTool1.AddPoint(new PunchingPoint(pointMedL));
                            punchingToolList[1].drawTool(pointMedL);
                            tool1count++;
                        }
                        else if (punchingToolList[1].isInside(boundaryCurve, pointMedR) == true)
                        {
                            pointMapTool1.AddPoint(new PunchingPoint(pointMedR));
                            punchingToolList[1].drawTool(pointMedR);
                            tool1count++;
                        }
                        else if (punchingToolList[0].isInside(boundaryCurve, pointShortL) == true)
                        {
                            pointMapTool1.AddPoint(new PunchingPoint(pointShortL));
                            punchingToolList[0].drawTool(pointShortL);
                            tool0count++;
                        }
                        else if (punchingToolList[0].isInside(boundaryCurve, pointShortR) == true)
                        {
                            pointMapTool1.AddPoint(new PunchingPoint(pointShortR));
                            punchingToolList[0].drawTool(pointShortR);
                            tool0count++;
                        }
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double tool0Area = punchingToolList[0].getArea() * tool0count;
            double tool1Area = punchingToolList[1].getArea() * tool1count;
            double tool2Area = punchingToolList[2].getArea() * tool2count;

            RhinoApp.WriteLine("Tool 1 area: {0} mm^2", tool0Area.ToString("#.##"));



            RhinoApp.WriteLine("Tool 2 area: {0} mm^2", tool1Area.ToString("#.##"));


            RhinoApp.WriteLine("Tool 3 area: {0} mm^2", tool2Area.ToString("#.##"));

            openArea = (tool0Area + tool1Area + tool2Area) * 100 / area.Area;

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
