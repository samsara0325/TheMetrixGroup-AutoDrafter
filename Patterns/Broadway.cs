using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using System.Drawing;

namespace MetrixGroupPlugins.Patterns
{
    /// <summary>
    /// Perforation Pattern
    /// </summary>
    public class BroadwayPattern : PerforationPattern
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraillePattern"/> class.
        /// </summary>
        public BroadwayPattern()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
        /// </summary>
        /// <param name="addTools">if set to <c>true</c> [add tools].</param>
        public BroadwayPattern(bool addTools)
        {
            Name = "Broadway";

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
                PunchingTools.Obround tool3 = new PunchingTools.Obround();
                tool3.DisplayName = "Tool 3";
                punchingToolList.Add(tool3);
                PunchingTools.Obround tool4 = new PunchingTools.Obround();
                tool4.DisplayName = "Tool 4";
                punchingToolList.Add(tool4);
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
                return XSpacing / 2;
            }
        }

        /// <summary>
        /// Draws the perforation.
        /// </summary>
        /// <returns></returns>
        public override double drawPerforation(Curve boundaryCurve)
        {
            List<PointMap> pointMapList = new List<PointMap>();

            // Add 4 Point Maps in the list as there are 4 tools
            pointMapList.Add(new PointMap());
            pointMapList.Add(new PointMap());
            pointMapList.Add(new PointMap());
            pointMapList.Add(new PointMap());

            // Find the boundary 
            BoundingBox boundingBox = boundaryCurve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            double spanX = max.X - min.X;
            double spanY = max.Y - min.Y;

            // Calculate how many to punch in X
            int punchQtyX = ((int)((spanX - punchingToolList[0].X) / XSpacing)) + 1;
            double marginX = (spanX - ((punchQtyX - 1) * XSpacing)) / 2;

            int punchQtyY = ((int)((spanY - punchingToolList[0].Y) / XSpacing)) * 2 + 1;
            double marginY = (spanY - ((punchQtyY - 1) * XSpacing / 2)) / 2;
            double secondRowOffset = XSpacing / 2;

            Point3d point;
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double firstX = min.X + marginX;
            double firstY = min.Y + marginY;

            // Set the layer
            int currentLayer = doc.Layers.CurrentLayerIndex;

            int tool0Count = 0;
            int tool1Count = 0;
            int tool2Count = 0;
            int tool3Count = 0;

            bool bigHoleRow = true;
            bool backSlashRow = true;
            int testCol = 0;

            for (int y = 0; y < punchQtyY; y++)
            {
                if (y % 2 == 0) // even rows
                {
                    for (int x = 0; x < punchQtyX; x++)
                    {
                        // Locate the point
                        point = new Point3d(firstX + x * XSpacing, firstY + y * YSpacing, 0);

                        if (bigHoleRow == true)
                        {
                            if (x % 2 == 0) // even column
                            {

                                if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                                {
                                    pointMapList[0].AddPoint(new PunchingPoint(point));
                                    punchingToolList[0].drawTool(point);
                                    tool0Count++;
                                }
                            }
                            else
                            {
                                // Don't punch it
                            }
                        }
                        else
                        {
                            if (x % 2 == 0) // even column
                            {
                                if (punchingToolList[1].isInside(boundaryCurve, point) == true)
                                {
                                    pointMapList[1].AddPoint(new PunchingPoint(point));
                                    punchingToolList[1].drawTool(point);
                                    tool1Count++;
                                }
                            }
                            else
                            {
                                if (punchingToolList[0].isInside(boundaryCurve, point) == true)
                                {
                                    pointMapList[0].AddPoint(new PunchingPoint(point));
                                    punchingToolList[0].drawTool(point);

                                    PunchingPoint pp = new PunchingPoint(point);
                                    pp.HasPunched = true;
                                    pointMapList[1].AddPoint(pp);
                                    tool0Count++;
                                }
                            }
                        }

                    }

                    bigHoleRow = !bigHoleRow;
                }
                else  // odd rows
                {
                    if (backSlashRow == true)
                    {
                        testCol = 0;
                    }
                    else
                    {
                        testCol = 1;
                    }

                    for (int x = 0; x < punchQtyX; x++)
                    {
                        point = new Point3d(firstX + (x * XSpacing) + secondRowOffset, firstY + y * YSpacing, 0);

                        if (x % 2 == testCol)
                        {
                            // Locate the point
                            if (punchingToolList[2].isInside(boundaryCurve, point) == true)
                            {
                                pointMapList[2].AddPoint(new PunchingPoint(point));
                                punchingToolList[2].drawTool(point, -Math.PI / 4);
                                tool2Count++;
                            }
                        }
                        else
                        {
                            // Locate the point
                            if (punchingToolList[3].isInside(boundaryCurve, point) == true)
                            {
                                pointMapList[3].AddPoint(new PunchingPoint(point));
                                punchingToolList[3].drawTool(point, Math.PI / 4);
                                tool3Count++;
                            }
                        }
                    }

                    backSlashRow = !backSlashRow;
                }
            }

            if (punchingToolList[0].ClusterTool.Enable == true)
            {
                drawCluster(pointMapList[0], punchingToolList[0]);
            }

            if (punchingToolList[1].ClusterTool.Enable == true)
            {
                punchingToolList[1].ClusterTool.AllowOverPunch = true;
                drawCluster(pointMapList[1], punchingToolList[1]);
            }

            if (punchingToolList[2].ClusterTool.Enable == true)
            {
                drawTool3Cluster(pointMapList[2], punchingToolList[2]);
            }

            if (punchingToolList[3].ClusterTool.Enable == true)
            {
                drawTool4Cluster(pointMapList[3], punchingToolList[3]);
            }

            // Draw the cluster for each tool 
            //for (int i = 0; i < punchingToolList.Count; i++)
            //{
            //   // Only draw cluster tool if it is enable
            //   if (punchingToolList[i].ClusterTool.Enable == true)
            //   {
            //      drawCluster(pointMapList[i], punchingToolList[i]);
            //   }
            //   else
            //   {
            //      // Set to Tool Hit Layer
            //      RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, Color.Black);

            //      // Places where cannot cluster punch
            //      for (int k = 0; k < pointMapList[i].YCount; k++)
            //      {
            //         Dictionary<int, PunchingPoint> xDict = pointMapList[i].getXDictionary(k);

            //         for (int m = 0; m< xDict.Count; m++)
            //         {
            //            if (xDict.ElementAt(m).Value.HasPunched == false)
            //            {
            //               // Draw the tool 
            //               punchingToolList[i].drawTool(xDict.ElementAt(m).Value.Point);
            //            }
            //         }
            //      }

            //      doc.Views.Redraw();
            //   }
            //}

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

        /// <summary>
        /// Draws the cluster.
        /// </summary>
        /// <param name="pointMap">The point map.</param>
        /// <param name="punchingTool">The punching tool.</param>
        /// <returns></returns>
        public virtual Result drawTool3Cluster(PointMap pointMap, PunchingTool punchingTool)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
            List<Point3d> clusterRelativePoints = new List<Point3d>();
            int clusterCounter = 0;
            Point2d centre;
            Transform xform;
            double rotatedXSpacing = XSpacing / Math.Sin(Math.PI / 4);
            double rotatedYSpacing = YSpacing / Math.Sin(Math.PI / 4);

            // Create a new layer 
            string layerName = Properties.Settings.Default.ToolHitLayerName;

            // Does a layer with the same name already exist?
            int layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
            }

            int currentLayer = doc.Layers.CurrentLayerIndex;

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            xform = Transform.Rotation(-Math.PI / 4, new Point3d(0, 0, 0));

            // Create the cluster tool that has the relative position
            for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
            {
                for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                {
                    Point3d pt = new Point3d(x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                    pt.Transform(xform);
                    clusterRelativePoints.Add(pt);
                }
            }

            centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);


            // Go through each point in the point Map list to find cluster tool starting from bottom left corner
            for (int i = 0; i < pointMap.YCount; i++)
            {
                SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                for (int j = 0; j < xDict.Count; j++)
                {
                    // If the point has already punched, ignore it.
                    if (xDict.ElementAt(j).Value.HasPunched == true)
                        continue;

                    // Clear the cluster points
                    clusterPoints.Clear();

                    // Add the first point to it
                    clusterPoints.Add(xDict.ElementAt(j).Value);

                    // Cluster tool X+ Y+ configuration
                    for (int k = 1; k < clusterRelativePoints.Count; k++)
                    {
                        // Look for point in the point list
                        PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                        if (resultPoint == null || resultPoint.HasPunched == true)
                        {
                            // Cannot find point in the cluster tool, then skip
                            break;
                        }
                        else
                        {
                            clusterPoints.Add(resultPoint);
                        }
                    }

                    // If all the points are added
                    if (clusterPoints.Count == clusterRelativePoints.Count)
                    {
                        double clusterX = centre.X + clusterPoints[0].Point.X;
                        double clusterY = centre.Y + clusterPoints[0].Point.Y;

                        // Draw all the points in the cluster tool
                        clusterCounter++;

                        if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                        {
                            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            doc.Views.Redraw();
                        }


                        foreach (PunchingPoint pp in clusterPoints)
                        {
                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                // Draw the tool
                                punchingTool.drawTool(pp.Point);
                            }

                            // Remove all the points in the clustertool list
                            pp.HasPunched = true;
                        }

                        // draw the rectangle
                        punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                    }
                }
            }

            // Overpunching

            // Use the same cluster again but run over punch from bottom left to top right
            // If the Cluster tool allow over punch have to do some more work
            if (punchingTool.ClusterTool.AllowOverPunch == true)
            {
                // Go through each point in the point Map list to find cluster tool starting from bottom left corner
                for (int i = 0; i < pointMap.YCount; i++)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = 0; j < xDict.Count; j++)
                    {
                        // If the point has already punched, ignore it.
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;

                        // Clear the cluster points
                        clusterPoints.Clear();

                        // Add the first point to it
                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X+ Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = centre.X + clusterPoints[0].Point.X;
                            double clusterY = centre.Y + clusterPoints[0].Point.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                                doc.Views.Redraw();
                            }


                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                        }
                    }
                }

                // Clear the clusterRelativePoints
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(-Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(-x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = 0; i < pointMap.YCount; i++)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = 0; j < xDict.Count; j++)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                        }
                    }
                }

                // Search from top right to bottom left
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(-Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, -y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = pointMap.YCount - 1; i >= 0; i--)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = xDict.Count - 1; j >= 0; j--)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                        }
                    }
                }

                // Search from top right to bottom left
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(-Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(-x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, -y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = pointMap.YCount - 1; i >= 0; i--)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = xDict.Count - 1; j >= 0; j--)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                        }
                    }
                }
            }

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Places where cannot cluster punch
            for (int i = 0; i < pointMap.YCount; i++)
            {
                SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                for (int j = 0; j < xDict.Count; j++)
                {
                    if (xDict.ElementAt(j).Value.HasPunched == false)
                    {
                        // Draw the tool 
                        punchingTool.drawTool(xDict.ElementAt(j).Value.Point, -Math.PI / 4);
                    }
                }
            }

            doc.Views.Redraw();
            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return Result.Success;
        }

        /// <summary>
        /// Draws the cluster.
        /// </summary>
        /// <param name="pointMap">The point map.</param>
        /// <param name="punchingTool">The punching tool.</param>
        /// <returns></returns>
        public virtual Result drawTool4Cluster(PointMap pointMap, PunchingTool punchingTool)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
            List<Point3d> clusterRelativePoints = new List<Point3d>();
            int clusterCounter = 0;
            Point2d centre;
            Transform xform;
            double rotatedXSpacing = XSpacing / Math.Sin(Math.PI / 4);
            double rotatedYSpacing = YSpacing / Math.Sin(Math.PI / 4);

            // Create a new layer 
            string layerName = Properties.Settings.Default.ToolHitLayerName;

            // Does a layer with the same name already exist?
            int layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
            }

            int currentLayer = doc.Layers.CurrentLayerIndex;

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            xform = Transform.Rotation(Math.PI / 4, new Point3d(0, 0, 0));

            // Create the cluster tool that has the relative position
            for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
            {
                for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                {
                    Point3d pt = new Point3d(x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                    pt.Transform(xform);
                    clusterRelativePoints.Add(pt);
                }
            }

            centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);


            // Go through each point in the point Map list to find cluster tool starting from bottom left corner
            for (int i = 0; i < pointMap.YCount; i++)
            {
                SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                for (int j = 0; j < xDict.Count; j++)
                {
                    // If the point has already punched, ignore it.
                    if (xDict.ElementAt(j).Value.HasPunched == true)
                        continue;

                    // Clear the cluster points
                    clusterPoints.Clear();

                    // Add the first point to it
                    clusterPoints.Add(xDict.ElementAt(j).Value);

                    // Cluster tool X+ Y+ configuration
                    for (int k = 1; k < clusterRelativePoints.Count; k++)
                    {
                        // Look for point in the point list
                        PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                        if (resultPoint == null || resultPoint.HasPunched == true)
                        {
                            // Cannot find point in the cluster tool, then skip
                            break;
                        }
                        else
                        {
                            clusterPoints.Add(resultPoint);
                        }
                    }

                    // If all the points are added
                    if (clusterPoints.Count == clusterRelativePoints.Count)
                    {
                        double clusterX = centre.X + clusterPoints[0].Point.X;
                        double clusterY = centre.Y + clusterPoints[0].Point.Y;

                        // Draw all the points in the cluster tool
                        clusterCounter++;

                        if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                        {
                            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            doc.Views.Redraw();
                        }


                        foreach (PunchingPoint pp in clusterPoints)
                        {
                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                // Draw the tool
                                punchingTool.drawTool(pp.Point);
                            }

                            // Remove all the points in the clustertool list
                            pp.HasPunched = true;
                        }

                        // draw the rectangle
                        punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                    }
                }
            }

            // Overpunching

            // Use the same cluster again but run over punch from bottom left to top right
            // If the Cluster tool allow over punch have to do some more work
            if (punchingTool.ClusterTool.AllowOverPunch == true)
            {
                // Go through each point in the point Map list to find cluster tool starting from bottom left corner
                for (int i = 0; i < pointMap.YCount; i++)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = 0; j < xDict.Count; j++)
                    {
                        // If the point has already punched, ignore it.
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;

                        // Clear the cluster points
                        clusterPoints.Clear();

                        // Add the first point to it
                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X+ Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = centre.X + clusterPoints[0].Point.X;
                            double clusterY = centre.Y + clusterPoints[0].Point.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                                doc.Views.Redraw();
                            }


                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                        }
                    }
                }

                // Clear the clusterRelativePoints
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(-x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = 0; i < pointMap.YCount; i++)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = 0; j < xDict.Count; j++)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                        }
                    }
                }

                // Clear the clusterRelativePoints
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, -y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = pointMap.YCount - 1; i >= 0; i--)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = xDict.Count - 1; j >= 0; j--)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                        }
                    }
                }

                // Search from top right to bottom left
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, -y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = pointMap.YCount - 1; i >= 0; i--)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = xDict.Count - 1; j >= 0; j--)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                        }
                    }
                }

                // Search from top right to bottom left but the cluster tool is - - 
                clusterRelativePoints.Clear();

                xform = Transform.Rotation(Math.PI / 4, new Point3d(0, 0, 0));

                // Create the cluster tool that has the relative position
                // Cluster tool is in negative x position and search from bottom left to top right again
                for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
                {
                    for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
                    {
                        Point3d pt = new Point3d(-x * rotatedXSpacing * punchingTool.ClusterTool.MultiplierX, -y * rotatedYSpacing * punchingTool.ClusterTool.MultiplierY, 0);
                        pt.Transform(xform);
                        clusterRelativePoints.Add(pt);
                    }
                }

                centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

                // Go through each point in the point Map list to find cluster tool starting from bottom left
                for (int i = pointMap.YCount - 1; i >= 0; i--)
                {
                    SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                    for (int j = xDict.Count - 1; j >= 0; j--)
                    {
                        // If the point is punched, skip to the next point
                        if (xDict.ElementAt(j).Value.HasPunched == true)
                            continue;
                        clusterPoints.Clear();

                        clusterPoints.Add(xDict.ElementAt(j).Value);

                        // Cluster tool X- Y+ configuration
                        for (int k = 1; k < clusterRelativePoints.Count; k++)
                        {
                            // Look for point in the point list
                            PunchingPoint resultPoint = pointMap.getPoint(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

                            if (resultPoint == null)
                            {
                                // Cannot find point in the cluster tool, then skip
                                break;
                            }
                            else
                            {
                                clusterPoints.Add(resultPoint);
                            }
                        }

                        // If all the points are added
                        if (clusterPoints.Count == clusterRelativePoints.Count)
                        {
                            double clusterX = clusterPoints[0].Point.X + centre.X;
                            double clusterY = clusterPoints[0].Point.Y + centre.Y;

                            // Draw all the points in the cluster tool
                            clusterCounter++;

                            if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                            {
                                doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
                            }

                            foreach (PunchingPoint pp in clusterPoints)
                            {
                                if (Properties.Settings.Default.DrawClusterOnSeparateLayer == true)
                                {
                                    // Draw the tool
                                    punchingTool.drawTool(pp.Point);

                                    //Debug only
                                    doc.Views.Redraw();
                                }

                                // Remove all the points in the clustertool list
                                pp.HasPunched = true;
                            }

                            // draw the rectangle
                            punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI / 4);
                        }
                    }
                }
            }

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Places where cannot cluster punch
            for (int i = 0; i < pointMap.YCount; i++)
            {
                SortedDictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

                for (int j = 0; j < xDict.Count; j++)
                {
                    if (xDict.ElementAt(j).Value.HasPunched == false)
                    {
                        // Draw the tool 
                        punchingTool.drawTool(xDict.ElementAt(j).Value.Point, Math.PI / 4);
                    }
                }
            }

            doc.Views.Redraw();
            doc.Layers.SetCurrentLayerIndex(currentLayer, true);
            return Result.Success;
        }
    }
}
