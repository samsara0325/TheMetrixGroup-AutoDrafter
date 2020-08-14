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
   public class ClusterToolSearcherPattern : PerforationPattern
   {
      private PointMap pointMap;

      /// <summary>
      /// Initializes a new instance of the <see cref="SixtyDegreePattern"/> class.
      /// </summary>
      public ClusterToolSearcherPattern()
      {
         Name = "Cluster Tool Searcher";

         // Add tool list 
         punchingToolList = new List<PunchingTool>();
         PunchingTools.Round tool1 = new PunchingTools.Round();
         tool1.DisplayName = "Tool 1";
         tool1.ClusterTool.Enable = true;
         punchingToolList.Add(tool1);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="PerforationPattern" /> class.
      /// </summary>
      public ClusterToolSearcherPattern(bool addTools)
      {
         Name = "Cluster Tool Searcher";
         // Add tool list 
         punchingToolList = new List<PunchingTool>();
         PunchingTools.Round tool1 = new PunchingTools.Round();
         tool1.DisplayName = "Tool 1";
         tool1.ClusterTool.Enable = true;
         punchingToolList.Add(tool1);
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
            return base.YSpacing;
         }
         set
         {
            base.YSpacing = value;
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
         }
      }

      /// <summary>
      /// Draws the perforation.
      /// </summary>
      /// <param name="boundaryCurve">The boundary curve.</param>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
      public override double drawPerforation(Curve boundaryCurve)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Draws the perforation.
      /// </summary>
      /// <returns></returns>
      public Result calculatePointMap(List<Circle> circleList)
      {
         pointMap = new PointMap();

         this.PunchingToolList[0].X = circleList[0].Diameter;

         // Create the pointMap
         foreach (Circle circle in circleList)
         {
            pointMap.AddPoint(new PunchingPoint(circle.Center));
         }

         SortedDictionary<int, PunchingPoint> xDict0 = null;
         SortedDictionary<int, PunchingPoint> xDict1 = null; 

         if (pointMap.Count > 0)
         {
            xDict0 = pointMap.getXDictionary(0);
         }

         if (pointMap.Count > 1)
         {
            xDict1 = pointMap.getXDictionary(1);
         }

         if (xDict0.Count > 2)
         {
            XSpacing = Math.Abs(xDict0.ElementAt(1).Value.Point.X - xDict0.ElementAt(0).Value.Point.X );
         }

         if (xDict0.Count > 1 && xDict1.Count > 1)
         {
            YSpacing = Math.Abs(xDict1.ElementAt(0).Value.Point.Y - xDict0.ElementAt(0).Value.Point.Y);
         }

         return Result.Success;
      }

      /// <summary>
      /// Draws the perforation.
      /// </summary>
      /// <returns></returns>
      public Result drawPerforation(List<Circle> circleList)
      {
         // pointMap = new PointMap();

         this.PunchingToolList[0].X = circleList[0].Diameter;

         //// Create the pointMap
         //foreach(Circle circle in circleList)
         //{
         //   pointMap.AddPoint(new PunchingPoint(circle.Center));
         //}

         if(pointMap == null)
         {
            calculatePointMap(circleList);
         }

         // Draw the cluster tool
         drawCluster(pointMap, punchingToolList[0]);
         
         //doc.Views.Redraw();

         //doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         return Result.Success;
      }

      /// <summary>
      /// Draws the cluster.
      /// </summary>
      /// <param name="pointMap">The point map.</param>
      /// <param name="punchingTool">The punching tool.</param>
      /// <returns></returns>
      public override Result drawCluster(PointMap pointMap, PunchingTool punchingTool)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
         List<Point3d> clusterRelativePoints = new List<Point3d>();
         int clusterCounter = 0;

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

         // Create the cluster tool that has the relative position
         for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
         {
            for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
            {
               clusterRelativePoints.Add(new Point3d(x * XSpacing * punchingTool.ClusterTool.MultiplierX, y * YSpacing * punchingTool.ClusterTool.MultiplierY, 0));
            }
         }

         Point2d centre = new Point2d(clusterRelativePoints.Last().X / 2, clusterRelativePoints.Last().Y / 2);

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
                  PunchingPoint resultPoint = pointMap.getPoint2(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

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

                  // Draw the rectangle
                  punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, 0);
               }
            }
         }

         // If the Cluster tool allow over punch have to do some more work
         if (punchingTool.ClusterTool.AllowOverPunch == true)
         {
            // Bottom right search
            // Clear the cluster tool points
            clusterRelativePoints.Clear();

            // Create the cluster tool that has the relative position
            for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
            {
               for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
               {
                  clusterRelativePoints.Add(new Point3d(-x * XSpacing * punchingTool.ClusterTool.MultiplierX, y * YSpacing * punchingTool.ClusterTool.MultiplierY, 0));
               }
            }

            // Go through each point in the point Map list to find cluster tool starting from bottom right corner
            for (int i = 0; i < pointMap.YCount; i++)
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
                     PunchingPoint resultPoint = pointMap.getPoint2(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

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
                     double clusterX = clusterPoints[0].Point.X - centre.X;
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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the rectangle
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, 0);
                  }
               }
            }

            // Top Left corner search
            // Clear the cluster tool points
            clusterRelativePoints.Clear();

            // Create the cluster tool that has the relative position
            for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
            {
               for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
               {
                  clusterRelativePoints.Add(new Point3d(x * XSpacing * punchingTool.ClusterTool.MultiplierX, -y * YSpacing * punchingTool.ClusterTool.MultiplierY, 0));
               }
            }

            // Go through each point in the point Map list to find cluster tool starting from top left
            for (int i = pointMap.YCount - 1; i >= 0; i--)
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
                     PunchingPoint resultPoint = pointMap.getPoint2(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

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
                     double clusterY = clusterPoints[0].Point.Y - centre.Y;

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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the rectangle
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, 0);
                  }
               }
            }

            // Top right corner search
            // Clear the cluster tool points
            clusterRelativePoints.Clear();

            // Create the cluster tool that has the relative position
            for (int y = 0; y < punchingTool.ClusterTool.PinsY; y++)
            {
               for (int x = 0; x < punchingTool.ClusterTool.PinsX; x++)
               {
                  clusterRelativePoints.Add(new Point3d(-x * XSpacing * punchingTool.ClusterTool.MultiplierX, -y * YSpacing * punchingTool.ClusterTool.MultiplierY, 0));
               }
            }

            centre = new Point2d(-clusterRelativePoints.Last().X / 2, -clusterRelativePoints.Last().Y / 2);

            // Go through each point in the point Map list to find cluster tool starting from top right
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
                     PunchingPoint resultPoint = pointMap.getPoint2(clusterPoints[0].Point.X + clusterRelativePoints[k].X, clusterPoints[0].Point.Y + clusterRelativePoints[k].Y);

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
                     double clusterX = clusterPoints[0].Point.X - centre.X;
                     double clusterY = clusterPoints[0].Point.Y - centre.Y;

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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the cluster tool
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, 0);
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
                  punchingTool.drawTool(xDict.ElementAt(j).Value.Point);
               }
            }
         }

         doc.Views.Redraw();
         doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         return Result.Success;
      }
   }
}
