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
    public class PhoenixMorsePattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public PhoenixMorsePattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public PhoenixMorsePattern(bool addTools)
        {
            Name = "Phoenix Morse";

            if (addTools == true)
            {
                // Add tool list 
                punchingToolList = new List<PunchingTool>();
                PunchingTools.Round tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 1";
                punchingToolList.Add(tool);

                // Add obround tool
                PunchingTools.Obround tool2 = new PunchingTools.Obround();
                tool2.DisplayName = "Tool 2";
                punchingToolList.Add(tool2);


                PunchingTools.Obround tool3 = new PunchingTools.Obround();
                tool3.DisplayName = "Tool 3";
                punchingToolList.Add(tool3);

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
            PointMap pointMapTool2 = new PointMap();
            PointMap pointMapTool3 = new PointMap();

            pointMapList.Add(pointMapTool1);

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            double gap = XSpacing - punchingToolList[0].X;

            int punchQtyX = ((int)((spanX - punchingToolList[0].X - gap) / XSpacing)) + 1;

            double marginX = (spanX - (((punchQtyX - 1) * XSpacing) - gap)) / 2;

            // Debug purpose
            // marginX = 0;

            double YSpacing = XSpacing;

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / YSpacing)) + 1;
            double marginY = (spanY - ((punchQtyY - 1) * YSpacing)) / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX + gap;
            double firstY = min.Y + marginY;

            // Set the current layer
            int currentLayer = doc.Layers.CurrentLayerIndex;
            doc.Layers.SetCurrentLayerIndex(currentLayer, false);
            // We don't use cluster tool so just perf it onto Tool Hit layer

            //if (!MetrixUtilities.IsLayerFound("TemporaryPerfLayer")){
            //    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
            //}else{
            //    RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //}
            double runningX = firstX - punchingToolList[0].X / 2;

            double twoToolTest;
            double threeToolTest;
            Random random = new Random();
            int roundToolCounter = 0;
            int slotToolCounter = 0;
            int tool0Counter = 0;
            int tool1Counter = 0;
            int tool2Counter = 0;
            bool isToolHit = false;
            int toolMissingCounter = 0;

            // Generate the pattern
            for (int y = 0; y < punchQtyY; y++)
            {
                // For even rows 

                if (random.NextDouble() < randomness || toolMissingCounter > 0)
                {
                    isToolHit = true;
                    toolMissingCounter = 0;
                }
                else
                {
                    isToolHit = false;
                    toolMissingCounter++;
                }


                if (y % 2 == 0)
                {
                    // firstX = min.X + marginX + gap;
                    runningX = min.X + gap;
                    twoToolTest = random.Next(42);

                    // Small slot
                    if (twoToolTest < 14)
                    {
                        point = new Point3d(runningX + punchingToolList[1].X / 2, firstY + y * YSpacing, 0);
                        pointMapTool2.AddPoint(new PunchingPoint(point));

                        runningX = runningX + punchingToolList[1].X + gap;

                        punchingToolList[1].drawTool(point);

                        if (isToolHit)
                        {
                            slotToolCounter = 1;
                            tool1Counter++;
                        }
                    }
                    else // Big slot
                    {
                        point = new Point3d(runningX + punchingToolList[2].X / 2, firstY + y * YSpacing, 0);
                        pointMapTool3.AddPoint(new PunchingPoint(point));

                        runningX = runningX + punchingToolList[2].X + gap;

                        slotToolCounter = 1;

                        if (isToolHit)
                        {
                            tool2Counter++;
                            punchingToolList[2].drawTool(point);
                        }
                    }
                }
                else
                {
                    //firstX = min.X + marginX;
                    //runningX = firstX - punchingToolList[0].X / 2;
                    runningX = min.X;
                    point = new Point3d(runningX + punchingToolList[0].X / 2, firstY + y * YSpacing, 0);
                    pointMapTool1.AddPoint(new PunchingPoint(point));

                    runningX = runningX + punchingToolList[0].X + gap;

                    roundToolCounter = 1;

                    if (isToolHit)
                    {
                        tool0Counter++;
                        punchingToolList[0].drawTool(point);
                    }
                }

                // 
                for (int x = 0; x < punchQtyX; x++)
                {
                    // Skip if the point already taken up by the tool
                    if (runningX > firstX + x * XSpacing)
                    {
                        continue;
                    }

                    if (random.NextDouble() < randomness || toolMissingCounter > 0)
                    {
                        isToolHit = true;
                        toolMissingCounter = 0;
                    }
                    else
                    {
                        isToolHit = false;
                        toolMissingCounter++;
                    }

                    threeToolTest = random.Next(100);

                    if (threeToolTest < 16 && slotToolCounter < 2)
                    {
                        point = new Point3d(runningX + punchingToolList[1].X / 2, firstY + y * YSpacing, 0);
                        pointMapTool2.AddPoint(new PunchingPoint(point));

                        runningX = runningX + punchingToolList[1].X + gap;

                        roundToolCounter = 0;
                        slotToolCounter++;

                        if (isToolHit)
                        {
                            tool1Counter++;
                            punchingToolList[1].drawTool(point);
                        }
                    }
                    else if (threeToolTest < 47 && slotToolCounter < 2)
                    {
                        point = new Point3d(runningX + punchingToolList[2].X / 2, firstY + y * YSpacing, 0);
                        pointMapTool3.AddPoint(new PunchingPoint(point));

                        runningX = runningX + punchingToolList[2].X + gap;



                        roundToolCounter = 0;
                        slotToolCounter++;
                        if (isToolHit)
                        {
                            tool2Counter++;
                            punchingToolList[2].drawTool(point);
                        }
                    }
                    else if (threeToolTest >= 47 && roundToolCounter < 4)
                    {
                        point = new Point3d(runningX + punchingToolList[0].X / 2, firstY + y * YSpacing, 0);
                        pointMapTool1.AddPoint(new PunchingPoint(point));

                        runningX = runningX + punchingToolList[0].X + gap;

                        roundToolCounter++;
                        slotToolCounter = 0;

                        if (isToolHit)
                        {
                            tool0Counter++;
                            punchingToolList[0].drawTool(point);
                        }
                    }
                    else
                    {
                        x--;
                    }
                }

            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double tool0Area = punchingToolList[0].getArea() * tool0Counter;
            double tool1Area = punchingToolList[1].getArea() * tool1Counter;
            double tool2Area = punchingToolList[2].getArea() * tool2Counter;
            double totalToolArea = tool0Area + tool1Area + tool2Area;

            RhinoApp.WriteLine("Tool 1 area: {0} mm^2", tool0Area.ToString("#.##"));
            RhinoApp.WriteLine("Tool 2 area: {0} mm^2", tool1Area.ToString("#.##"));
            RhinoApp.WriteLine("Tool 3 area: {0} mm^2", tool2Area.ToString("#.##"));

            double openArea = (totalToolArea * 100) / area.Area;
            RhinoApp.WriteLine("Open area: {0} %", openArea.ToString("#.##"));

            //int testResult;
            //int slotNumber = 1;

            //PunchingTools.Obround threeHoleSlot = new PunchingTools.Obround();
            //threeHoleSlot.X = 2*this.XSpacing + punchingToolList[0].X;
            //threeHoleSlot.Y = punchingToolList[0].X;

            //PunchingTools.Obround fiveHoleSlot = new PunchingTools.Obround();
            //fiveHoleSlot.X = 4 * this.XSpacing + punchingToolList[0].X;
            //fiveHoleSlot.Y = punchingToolList[0].X;

            //int chanceForHoles = 60;
            //int chanceForMidSlot = 86;
            //int chanceForLargeSlot = 100;

            //int holeCounter = 0;
            //int midSlotCounter = 0;
            //int largeSlotCounter = 0;

            //double toolArea = 0;

            // // Go through each point in the point Map list to determine whether it is a hole or a slot.
            //for (int i = 0; i < pointMapTool1.YCount; i++)
            //{
            //   SortedDictionary<int, PunchingPoint> xDict = pointMapTool1.getXDictionary(i);

            //   chanceForHoles = 60;
            //   chanceForMidSlot = 86;
            //   chanceForLargeSlot = 100;

            //   for (int j = 0; j < xDict.Count; j = j + slotNumber)
            //   {
            //      if (xDict.Count - j > 4)
            //      {
            //         // Generate whether the current hole 
            //         testResult = random.Next(1, chanceForLargeSlot);
            //      }
            //      else if (xDict.Count - j > 2)
            //      {
            //         chanceForHoles = 30;
            //         chanceForMidSlot = 66;
            //         chanceForLargeSlot = 100;
            //         // Generate whether the current slot
            //         testResult = random.Next(1, chanceForMidSlot);
            //      }
            //      else
            //      {
            //         chanceForHoles = 30;
            //         chanceForMidSlot = 66;
            //         chanceForLargeSlot = 100;
            //         // Generate whether the current large slot 
            //         testResult = random.Next(1, chanceForHoles);
            //      }

            //      if (testResult >= 1 && testResult <= chanceForHoles)
            //      {
            //         slotNumber = 1;

            //         holeCounter++;
            //         midSlotCounter = 0;
            //         largeSlotCounter = 0;

            //         if (holeCounter == 1)
            //         {
            //            chanceForHoles = 60;
            //            chanceForMidSlot = 86;
            //            chanceForLargeSlot = 100;
            //         }
            //         else if(holeCounter == 2)
            //         {
            //            chanceForHoles = 30;
            //            chanceForMidSlot = 66;
            //            chanceForLargeSlot = 100;
            //         }
            //         else if(holeCounter > 2)
            //         {
            //            chanceForHoles = 10;
            //            chanceForMidSlot = 53;
            //            chanceForLargeSlot = 100;
            //         }

            //         // Draw circle
            //         punchingToolList[0].drawTool(xDict.ElementAt(j).Value.Point);

            //         toolArea += punchingToolList[0].getArea();

            //      }
            //      else if (testResult > chanceForHoles && testResult <= chanceForMidSlot)
            //      {
            //         slotNumber = 3;

            //         holeCounter = 0;
            //         midSlotCounter++;
            //         largeSlotCounter = 0;

            //         if (midSlotCounter == 1)
            //         {
            //            chanceForHoles = 80;
            //            chanceForMidSlot = 90;
            //            chanceForLargeSlot = 100;
            //         }
            //         else if(midSlotCounter > 1)
            //         {
            //            chanceForHoles = 85;
            //            chanceForMidSlot = 93;
            //            chanceForLargeSlot = 100;
            //         }

            //         // Draw slot that span over 3 holes
            //         threeHoleSlot.drawTool(xDict.ElementAt(j + 1).Value.Point);

            //         toolArea += threeHoleSlot.getArea();
            //      }
            //      else if (testResult > chanceForMidSlot && testResult <= chanceForLargeSlot)
            //      {
            //         slotNumber = 5;

            //         holeCounter = 0;
            //         midSlotCounter = 0;
            //         largeSlotCounter++;

            //         if(largeSlotCounter >= 1)
            //         {
            //            chanceForHoles = 80;
            //            chanceForMidSlot = 100;
            //            chanceForLargeSlot = 100;
            //         }

            //         // Draw slot that span over 5 holes
            //         fiveHoleSlot.drawTool(xDict.ElementAt(j + 2).Value.Point);

            //         toolArea += fiveHoleSlot.getArea();
            //      }
            //   }
            //}

            // Display the open area calculation
            //AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            //RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            //  RhinoApp.WriteLine("Tool area: {0} mm^2", toolArea.ToString("#.##"));

            // openArea = toolArea * 100 / area.Area;

            // RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

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
