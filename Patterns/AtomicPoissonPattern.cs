using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using System.Collections;
using System.Drawing;
using MetrixGroupPlugins.Utilities;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class AtomicPoissonPattern : PerforationPattern
    {
        Random random = new Random();
        bool debugAtomic = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public AtomicPoissonPattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public AtomicPoissonPattern(bool addTools)
        {
            Name = "Atomic Poisson";

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

                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 6";
                punchingToolList.Add(tool);

                tool = new PunchingTools.Round();
                tool.DisplayName = "Tool 7";
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
            double[] cellSize = new double[atomicNumber];
            int[] gridWidth = new int[atomicNumber];
            int[] gridHeight = new int[atomicNumber];

            RhinoDoc doc = RhinoDoc.ActiveDoc;

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            //int gridWidth = (int) (spanX /  cellSizeTool1) + 1;
            //int gridHeight = (int) (spanY /  cellSizeTool1) + 1;

            // Create the grid
            List<Point3d?[,]> gridList = new List<Point3d?[,]>();

            // Active points
            List<List<Point3d>> activePointsList = new List<List<Point3d>>();

            // Point list
            List<List<Point3d>> pointsList = new List<List<Point3d>>();

            // Record the current layer so that lat
            int currentLayer = doc.Layers.CurrentLayerIndex;

            //if (!MetrixUtilities.IsLayerFound("TemporaryPerfLayer"))
            {
                //RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, Color.Black);
                doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            }
            //else
            //{
            //    RhinoUtilities.SetActiveLayer("TemporaryToolHit", System.Drawing.Color.Black);
            //}
            // Calculate cell size and create grid
            for (int a = 0; a < atomicNumber; a++)
            {
                cellSize[a] = (punchingToolList[a].Gap + punchingToolList[a].X) / Math.Sqrt(2);
                gridWidth[a] = (int)(spanX / cellSize[a]) + 1;
                gridHeight[a] = (int)(spanY / cellSize[a]) + 1;
                gridList.Add(new Point3d?[gridWidth[a], gridHeight[a]]);
                activePointsList.Add(new List<Point3d>());
                pointsList.Add(new List<Point3d>());
            }

            // Add first point
            random = new Random();

            bool added = false;
            int breakCounter = 0;

            while (!added && breakCounter < 250)
            {
                double d = random.NextDouble();
                double xr = min.X + spanX * d;

                d = random.NextDouble();
                double yr = min.Y + spanY * d;

                Point3d p = new Point3d(xr, yr, 0);

                bool test = punchingToolList[0].isInside(boundaryCurve, p);

                if (test)
                {
                    added = true;
                    Point3d index = Denormalize(p, min, cellSize[0]);

                    gridList[0][(int)index.X, (int)index.Y] = p;
                    activePointsList[0].Add(p);
                    pointsList[0].Add(p);

                    punchingToolList[0].drawTool(p);


                    if (debugAtomic)
                    {
                        doc.Views.Redraw();
                    }
                }

                breakCounter++;
            }

            // Loop through each of the tool
            for (int b = 0; b < atomicNumber; b++)
            {
                // Loop through each of the active point list
                for (int c = 0; c < atomicNumber; c++)
                {
                    while (activePointsList[c].Count != 0)
                    {
                        // Pick any point in the list
                        int listIndex = random.Next(activePointsList[c].Count);
                        Point3d point = activePointsList[c][listIndex];

                        bool found = false;
                        Point3d q;
                        double minDistance;

                        if (c == b)
                        {
                            minDistance = punchingToolList[b].X + punchingToolList[b].Gap;
                        }
                        else
                        {
                            minDistance = punchingToolList[b].X / 2 + pitch + punchingToolList[c].X / 2;
                        }

                        for (int d = 0; d < Properties.Settings.Default.PoissonIterations; d++)
                        {
                            q = GenerateRandomAround(point, minDistance);

                            bool test = punchingToolList[b].isInside(boundaryCurve, q);

                            //punchingToolList[4].drawTool(q);
                            //doc.Views.Redraw();

                            if (test)
                            {
                                bool tooClose = false;

                                Point3d qIndex = Denormalize(q, min, cellSize[b]);

                                // Look through the current grid and all the previous grid 
                                for (int e = b; e >= 0; e--)
                                {
                                    double minCurrentDistance;

                                    if (e == b)
                                    {
                                        minCurrentDistance = punchingToolList[b].X + punchingToolList[b].Gap;
                                    }
                                    else
                                    {
                                        minCurrentDistance = punchingToolList[b].X / 2 + pitch + punchingToolList[e].X / 2;
                                    }

                                    Point3d qCurrentGridIndex = Denormalize(q, min, cellSize[e]);
                                    // Check the grid to see if the test point is close to 
                                    for (int i = (int)Math.Max(0, qCurrentGridIndex.X - 2); i < Math.Min(gridWidth[e], qCurrentGridIndex.X + 3) && !tooClose; i++)
                                    {
                                        for (var j = (int)Math.Max(0, qCurrentGridIndex.Y - 2); j < Math.Min(gridHeight[e], qCurrentGridIndex.Y + 3) && !tooClose; j++)
                                        {
                                            if (gridList[e][i, j].HasValue == true)
                                            {
                                                double distance = Distance(gridList[e][i, j].Value, q);

                                                if (distance < minCurrentDistance)
                                                {
                                                    tooClose = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!tooClose)
                                {
                                    found = true;
                                    activePointsList[c].Add(q);
                                    pointsList[b].Add(q);
                                    gridList[b][(int)qIndex.X, (int)qIndex.Y] = q;

                                    punchingToolList[b].drawTool(q);

                                    if (debugAtomic)
                                    {
                                        doc.Views.Redraw();
                                    }
                                }
                            }
                        }

                        // If cannot find point after max iterations remove point from active list 
                        if (!found)
                        {
                            activePointsList[c].RemoveAt(listIndex);
                        }
                    }
                }

                // Next acive point list will be the current list 
                for (int f = b; f >= 0; f--)
                {
                    foreach (Point3d p in pointsList[f])
                    {
                        activePointsList[f].Add(p);
                    }
                }
            }

            // Display the open area calculation
            AreaMassProperties area = AreaMassProperties.Compute(boundaryCurve);

            RhinoApp.WriteLine("Total area: {0} mm^2", area.Area.ToString("#.##"));

            double[] toolArea = new double[atomicNumber];

            double totalToolArea = 0;

            for (int g = 0; g < atomicNumber; g++)
            {
                toolArea[g] = punchingToolList[g].getArea() * pointsList[g].Count;
                totalToolArea = totalToolArea + toolArea[g];
                RhinoApp.WriteLine("Tool {0} area: {1} mm^2", g + 1, toolArea[g].ToString("#.##"));
            }

            openArea = totalToolArea * 100 / area.Area;

            RhinoApp.WriteLine("Open area: {0}%", openArea.ToString("#."));

            doc.Views.Redraw();
            doc.Layers.SetCurrentLayerIndex(currentLayer, true);

            return openArea;
        }

        /// <summary>
        /// Distances the specified p0.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <returns></returns>
        double Distance(Point3d p0, Point3d p1)
        {
            return (double)Math.Sqrt(DistanceSquared(p0, p1));
        }

        /// <summary>
        /// Distances the squared.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <returns></returns>
        double DistanceSquared(Point3d p0, Point3d p1)
        {
            return (p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y) + (p1.Z - p0.Z) * (p1.Z - p0.Z);
        }

        /// <summary>
        /// Generates the random around.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="minDistance">The minimum distance.</param>
        /// <returns></returns>
        public Point3d GenerateRandomAround(Point3d center, double minDistance)
        {
            double d = random.NextDouble();
            double radius = minDistance + minDistance * d;

            d = random.NextDouble();
            double angle = 2 * Math.PI * d;

            double newX = radius * Math.Sin(angle);
            double newY = radius * Math.Cos(angle);

            return new Point3d((double)(center.X + newX), (double)(center.Y + newY), 0);
        }

        /// <summary>
        /// Denormalizes the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="cellSize">Size of the cell.</param>
        /// <returns></returns>
        public Point3d Denormalize(Point3d point, Point3d origin, double cellSize)
        {
            return new Point3d((int)((point.X - origin.X) / cellSize), (int)((point.Y - origin.Y) / cellSize), (int)((point.Z - origin.Z) / cellSize));
        }
    }
}
