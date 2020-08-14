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
    public class MorsePattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public MorsePattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public MorsePattern(bool addTools)
        {
            Name = "Morse";

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
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            List<PointMap> pointMapList = new List<PointMap>();
            PointMap pointMapTool1 = new PointMap();

            pointMapList.Add(pointMapTool1);

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

            // Set the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            doc.Layers.SetCurrentLayerIndex(currentLayer, false);
            // We don't use cluster tool so just perf it onto Tool Hit layer
            //if (!MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
            //{
            //    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //}
            //else
            //{
            //    RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //}
            // Generate the point map first
            for (int y = 0; y < punchQtyY; y++)
            {
                for (int x = 0; x < punchQtyX; x++)
                {
                    point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                    if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                    {
                        pointMapTool1.AddPoint(new PunchingPoint(point));
                    }
                }
            }

            Random random = new Random();
            int testResult;
            int slotNumber = 1;

            PunchingTools.Obround threeHoleSlot = new PunchingTools.Obround();
            threeHoleSlot.X = 2 * this.XSpacing + punchingToolList[0].X;
            threeHoleSlot.Y = punchingToolList[0].X;

            PunchingTools.Obround fiveHoleSlot = new PunchingTools.Obround();
            fiveHoleSlot.X = 4 * this.XSpacing + punchingToolList[0].X;
            fiveHoleSlot.Y = punchingToolList[0].X;

            int chanceForHoles = 60;
            int chanceForMidSlot = 86;
            int chanceForLargeSlot = 100;

            int holeCounter = 0;
            int midSlotCounter = 0;
            int largeSlotCounter = 0;

            double toolArea = 0;

            // Go through each point in the point Map list to determine whether it is a hole or a slot.
            for (int i = 0; i < pointMapTool1.YCount; i++)
            {
                SortedDictionary<int, PunchingPoint> xDict = pointMapTool1.getXDictionary(i);

                chanceForHoles = 60;
                chanceForMidSlot = 86;
                chanceForLargeSlot = 100;

                for (int j = 0; j < xDict.Count; j = j + slotNumber)
                {
                    if (xDict.Count - j > 4)
                    {
                        // Generate whether the current hole 
                        testResult = random.Next(1, chanceForLargeSlot);
                    }
                    else if (xDict.Count - j > 2)
                    {
                        chanceForHoles = 30;
                        chanceForMidSlot = 66;
                        chanceForLargeSlot = 100;
                        // Generate whether the current slot
                        testResult = random.Next(1, chanceForMidSlot);
                    }
                    else
                    {
                        chanceForHoles = 30;
                        chanceForMidSlot = 66;
                        chanceForLargeSlot = 100;
                        // Generate whether the current large slot 
                        testResult = random.Next(1, chanceForHoles);
                    }

                    if (testResult >= 1 && testResult <= chanceForHoles)
                    {
                        slotNumber = 1;

                        holeCounter++;
                        midSlotCounter = 0;
                        largeSlotCounter = 0;

                        if (holeCounter == 1)
                        {
                            chanceForHoles = 60;
                            chanceForMidSlot = 86;
                            chanceForLargeSlot = 100;
                        }
                        else if (holeCounter == 2)
                        {
                            chanceForHoles = 30;
                            chanceForMidSlot = 66;
                            chanceForLargeSlot = 100;
                        }
                        else if (holeCounter > 2)
                        {
                            chanceForHoles = 10;
                            chanceForMidSlot = 53;
                            chanceForLargeSlot = 100;
                        }

                        // Draw circle
                        punchingToolList[0].drawTool(xDict.ElementAt(j).Value.Point);

                        toolArea += punchingToolList[0].getArea();

                    }
                    else if (testResult > chanceForHoles && testResult <= chanceForMidSlot)
                    {
                        slotNumber = 3;

                        holeCounter = 0;
                        midSlotCounter++;
                        largeSlotCounter = 0;

                        if (midSlotCounter == 1)
                        {
                            chanceForHoles = 80;
                            chanceForMidSlot = 90;
                            chanceForLargeSlot = 100;
                        }
                        else if (midSlotCounter > 1)
                        {
                            chanceForHoles = 85;
                            chanceForMidSlot = 93;
                            chanceForLargeSlot = 100;
                        }

                        // Draw slot that span over 3 holes
                        threeHoleSlot.drawTool(xDict.ElementAt(j + 1).Value.Point);

                        toolArea += threeHoleSlot.getArea();
                    }
                    else if (testResult > chanceForMidSlot && testResult <= chanceForLargeSlot)
                    {
                        slotNumber = 5;

                        holeCounter = 0;
                        midSlotCounter = 0;
                        largeSlotCounter++;

                        if (largeSlotCounter >= 1)
                        {
                            chanceForHoles = 80;
                            chanceForMidSlot = 100;
                            chanceForLargeSlot = 100;
                        }

                        // Draw slot that span over 5 holes
                        fiveHoleSlot.drawTool(xDict.ElementAt(j + 2).Value.Point);

                        toolArea += fiveHoleSlot.getArea();
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            RhinoApp.WriteLine("Tool area: {0} mm^2", toolArea.ToString("#.##"));

            openArea = toolArea * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            //for (int y = 0; y < punchQtyY; y++)
            //{
            //   for (int x = 0; x < punchQtyX; x++)
            //   {
            //      point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

            //      if (punchingToolList[0].isInside(boundaryCurve, point) == true)
            //      {
            //         pointMap.AddPoint(new PunchingPoint(point));
            //         punchingToolList[0].drawTool(point);
            //      }

            //      if(random.NextDouble() < randomness)
            //      {
            //         if (punchingToolList[1].isInside(boundaryCurve, point) == true)
            //         {
            //            doc.Layers.SetCurrentLayerIndex(brailleLayerIndex, true);
            //            punchingToolList[1].drawTool(point);
            //            doc.Layers.SetCurrentLayerIndex(perforationlayerIndex, true);
            //         }
            //      }
            //   }
            //}

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
