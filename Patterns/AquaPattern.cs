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
    public class AquaPattern : PerforationPattern
    {
        Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="NintyDegreePattern"/> class.
        /// </summary>
        public AquaPattern()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        public AquaPattern(bool addTools)
        {
            Name = "Aqua";

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

                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 4";
                punchingToolList.Add(tool);

                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 5";
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
            PointMap[] pointMap = new PointMap[AtomicNumber];

            for (int x = 0; x < AtomicNumber; ++x)
            {
                pointMap[x] = new PointMap();
            }

            int[] toolHitArray = Enumerable.Repeat(0, atomicNumber).ToArray();

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

            // Random Engine

            RandomTiler randomTileEngine = new RandomTiler();

            int totalQty = punchQtyX * punchQtyY;
            // except the last tool percentage will be total - all tool hit
            List<double> toolHitPercentage = new List<double>(atomicNumber - 1);

            double average = 1 / (double)atomicNumber;
            double minPercentage = (average - 0.125 * average);

            //for (int i = 0; i < atomicNumber-1; i++)
            //{
            //   toolHitPercentage.Add(minPercentage + random.NextDouble() * (2*0.125* average));
            //}

            double totalPercentage = 0;

            for (int i = 0; i < atomicNumber - 1; i++)
            {
                if (PunchingToolList[i].Gap > 1)
                {
                    PunchingToolList[i].Gap = 1;
                }

                if (totalPercentage + PunchingToolList[i].Gap <= 1)
                {
                    totalPercentage += PunchingToolList[i].Gap;
                    toolHitPercentage.Add(PunchingToolList[i].Gap);
                }
                else
                {
                    totalPercentage = 1;
                    PunchingToolList[i].Gap = 1 - totalPercentage;
                    toolHitPercentage.Add(PunchingToolList[i].Gap);
                }


            }

            int toolHitCount = 0;

            List<int> tileCounts = new List<int>();


            foreach (double pc in toolHitPercentage)
            {
                int toolHitQty = (int)(totalQty * pc);
                tileCounts.Add(toolHitQty);
                toolHitCount = toolHitCount + toolHitQty;
            }

            randomTileEngine.Weight = new int[5, 5]
          { { 1, 1, 2, 1, 1 },
         { 1, 2, 2, 2, 1 },
         { 2, 2, 0, 2, 2 },
         { 1, 2, 2, 2, 1 },
         { 1, 1, 2, 1, 1 } };
            tileCounts.Add(totalQty - toolHitCount);

            int[,] tileMap = randomTileEngine.GetTileMap(tileCounts, punchQtyX, punchQtyY);
            int[] toolHitCounter = new int[atomicNumber];
            int type;
            bool drawClusterToolHit = false;
            for (int i = 0; i < punchingToolList.Count; i++)
            {

                // Only draw cluster tool if it is enable
                if (punchingToolList[i].ClusterTool.Enable == true)
                {
                    // Draw the cluster tool
                    drawClusterToolHit = true;
                    break;
                }
            }

                int perforationLayer = currentLayer;
                int toolHitlayer = doc.Layers.Find("Tool Hit", true);
                if (toolHitlayer < 0)
                {
                    toolHitlayer = doc.Layers.Add("Tool Hit", System.Drawing.Color.Black);
                }                
                doc.Layers.SetCurrentLayerIndex(toolHitlayer, true);
                for (int y = 0; y < punchQtyY; y++)
                {
                    for (int x = 0; x < punchQtyX; x++)
                    {
                        point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                        type = tileMap[x, y] - 1;

                        if(punchingToolList[type].ClusterTool.Enable == true)
                        {
                            doc.Layers.SetCurrentLayerIndex(perforationLayer, true);
                        }
                        else
                        {
                            doc.Layers.SetCurrentLayerIndex(toolHitlayer, true);
                        }

                        if (punchingToolList[type].isInside(boundaryCurve, point) == true)
                        {
                            pointMap[type].AddPoint(new PunchingPoint(point));

                            for (int t = type; t < AtomicNumber; t++)
                            {
                                pointMap[t].AddPoint(new PunchingPoint(point));
                            }

                            if (PunchingToolList[type].ClusterTool.Enable == false)
                            {
                                punchingToolList[type].drawTool(point);
                                if(punchingToolList[type].Perforation == true && drawClusterToolHit == true)
                                {
                                    doc.Layers.SetCurrentLayerIndex(perforationLayer, true);
                                    punchingToolList[type].drawTool(point);
                                }
                            }
                            else if (punchingToolList[type].Perforation == true)
                            {
                                punchingToolList[type].drawTool(point);
                            }
                            
                            toolHitArray[type]++;
                        }
                    }
                }

              Rhino.DocObjects.RhinoObject [] perforations = doc.Objects.FindByLayer("Perforation");
             foreach(var perforation in perforations)
            {
                perforation.Attributes.DisplayOrder = 1;
                perforation.CommitChanges();
            }
            doc.Views.Redraw();
  

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);
            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double toolArea = 0;
            double totalToolArea = 0;

            for (int i = 0; i < this.atomicNumber; i++)
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
                    drawCluster(pointMap[i], punchingToolList[i]);
                }
            }

            doc.Views.Redraw();

            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return openArea;
        }
    }
}
