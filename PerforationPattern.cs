using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;
using System.Xml.Serialization;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// Perforation Pattern
   /// </summary>
   /// 
   [XmlInclude(typeof(Patterns.FourtyFiveDegreePattern))]
   [XmlInclude(typeof(Patterns.SixtyDegreePattern))]
   [XmlInclude(typeof(Patterns.NintyDegreePattern))]
   [XmlInclude(typeof(Patterns.BraillePattern))]
   [XmlInclude(typeof(Patterns.AtomicPoissonPattern))]
   [XmlInclude(typeof(Patterns.MorsePattern))]
   [XmlInclude(typeof(Patterns.StraightPattern))]
   [XmlInclude(typeof(Patterns.CrescendaPattern))]
   [XmlInclude(typeof(Patterns.TechnoPattern))]
   [XmlInclude(typeof(Patterns.StaggeredPattern))]
   [XmlInclude(typeof(Patterns.AquaPattern))]
   [XmlInclude(typeof(Patterns.JazzPattern))]
   [XmlInclude(typeof(Patterns.WeavePattern))]
   [XmlInclude(typeof(Patterns.MatrixPattern))]
   [XmlInclude(typeof(Patterns.BroadwayPattern))]
   [XmlInclude(typeof(Patterns.SixtyDegreeStripePattern))]
   [XmlInclude(typeof(Patterns.TrianglePattern))]
   [XmlInclude(typeof(Patterns.TreadPerfPattern))]
   [XmlInclude(typeof(Patterns.ThirdStackPattern))]
   [XmlInclude(typeof(Patterns.PhoenixMorsePattern))]
   [XmlInclude(typeof(Patterns.MetrixPattern))]
   [Serializable]
   public abstract class PerforationPattern
   {
      protected string perforationName;
      protected List<PunchingTool> punchingToolList;
      protected double xSpacing;
      protected double ySpacing;
      protected double pitch;
      protected double randomness;
      protected double openArea;
      protected int atomicNumber;
      protected Boolean enableToolHit;
      /// <summary>
      /// Initializes a new instance of the <see cref="PerforationPattern"/> class.
      /// </summary>
      public PerforationPattern()
      {

      }

      /// <summary>
      /// Gets the name.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      public string Name 
      {
         get
         {
            return perforationName;
         }

         set
         {
            perforationName = value;
         }
      }

      /// <summary>
      /// Gets or sets the spacing x.
      /// </summary>
      /// <value>
      /// The spacing x.
      /// </value>
      public virtual double XSpacing
      {
         get
         {
            return xSpacing;
         }

         set
         {
            xSpacing = value;
         }
      }

      /// <summary>
      /// Gets or sets the spacing y.
      /// </summary>
      /// <value>
      /// The spacing y.
      /// </value>
      public virtual double YSpacing
      {
         get
         {
            return ySpacing;
         }

         set
         {
            ySpacing = value;
         }
      }

      /// <summary>
      /// Gets or sets the delta x.
      /// </summary>
      /// <value>
      /// The delta x.
      /// </value>
      public double Pitch  
      {
         get
         {
            return pitch;
         }

         set
         {
            pitch = value;
         }
      }

      /// <summary>
      /// Gets or sets the randomness.
      /// </summary>
      /// <value>
      /// The randomness.
      /// </value>
      public double Randomness 
      {
         get
         {
            return randomness;
         }
         set
         {
            randomness = value;
         }
      }

      /// <summary>
      /// Gets or sets the open area.
      /// </summary>
      /// <value>
      /// The open area.
      /// </value>
      public double OpenArea
      {
         get
         {
            return openArea;
         }
         set
         {
            openArea = value;
         }
      }

      /// <summary>
      /// Gets or sets the atomic number.
      /// </summary>
      /// <value>
      /// The atomic number.
      /// </value>
      public int AtomicNumber 
      {
         get
         {
            return atomicNumber;
         }
         set
         {
            atomicNumber = value;
         }
      }

      /// <summary>
      /// Gets or sets the delta x.
      /// </summary>
      /// <value>
      /// The delta x.
      /// </value>
      public Boolean EnableToolHit
      {
         get
         {
            return enableToolHit;
         }

         set
         {
            enableToolHit = value;
         }
      }

      /// <summary>
      /// Gets or sets the punching tool list.
      /// </summary>
      /// <value>
      /// The punching tool list.
      /// </value>
      public List<PunchingTool> PunchingToolList
      {
         get
         {
            return punchingToolList;
         }

         set
         {
            punchingToolList = value;
         }

      }

      /// <summary>
      /// Draws the perforation.
      /// </summary>
      /// <param name="boundaryCurve">The boundary curve.</param>
      /// <returns></returns>
      public abstract double drawPerforation(Curve boundaryCurve);

     //   public abstract float drawPerforation(Curve boundaryCurve, bool clusterEnable, bool enablePerf);

      /// <summary>
      /// Draws the cluster.
      /// </summary>
      /// <param name="pointMap">The point map.</param>
      /// <param name="punchingTool">The punching tool.</param>
      /// <returns></returns>
      public virtual Result drawRotatedCluster(PointMap pointMap, PunchingTool punchingTool)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
         List<Point3d> clusterRelativePoints = new List<Point3d>();
         int clusterCounter = 0;
         Point2d centre;
         Transform xform;
         double rotatedXSpacing = XSpacing / Math.Sin(Math.PI / 4);
         double rotatedYSpacing = YSpacing / Math.Sin(Math.PI / 4);
         bool isFirstCluster = true;
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

         xform = Transform.Rotation(Math.PI/4, new Point3d(0, 0, 0));
            
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
                  punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI/4);

                  if (isFirstCluster == true)
                  {
                     RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ClusterSampleLayerName, System.Drawing.Color.RosyBrown);

                     foreach (PunchingPoint pp in clusterPoints)
                     {
                        // Draw the tool
                        punchingTool.drawTool(pp.Point);
                     }

                     isFirstCluster = false;

                     RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
                  }
               }
            }
         }
        
         // If the Cluster tool allow over punch have to do some more work
         if (punchingTool.ClusterTool.AllowOverPunch == true)
         {
            // Clear the clusterRelativePoints
            clusterRelativePoints.Clear();

            xform = Transform.Rotation(Math.PI*3 / 4, new Point3d(0, 0, 0));

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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the rectangle
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, Math.PI * 3 / 4);
                  }
               }
            }

            // Top Left corner search
            // Clear the cluster tool points
            clusterRelativePoints.Clear();

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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the rectangle
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI / 4);
                  }
               }
            }

            // Top right corner search
            // Clear the cluster tool points
            clusterRelativePoints.Clear();

            xform = Transform.Rotation(-Math.PI * 3/ 4, new Point3d(0, 0, 0));

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
                        }

                        // Remove all the points in the clustertool list
                        pp.HasPunched = true;
                     }

                     // draw the rectangle
                     punchingTool.ClusterTool.drawClusterTool(clusterX, clusterY, -Math.PI * 3 / 4);
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

         doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         return Result.Success;
      }

      /// <summary>
      /// Draws the cluster.
      /// </summary>
      /// <param name="pointMap">The point map.</param>
      /// <param name="punchingTool">The punching tool.</param>
      /// <returns></returns>
      public virtual Result drawCluster(PointMap pointMap, PunchingTool punchingTool)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
         List<Point3d> clusterRelativePoints = new List<Point3d>();
         int clusterCounter = 0;
         bool isFirstCluster = true;

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

                  if(isFirstCluster == true)
                  {
                     RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ClusterSampleLayerName, System.Drawing.Color.RosyBrown);

                     foreach (PunchingPoint pp in clusterPoints)
                     {
                        // Draw the tool
                        punchingTool.drawTool(pp.Point);
                     }

                     isFirstCluster = false;

                     RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
                  }
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

                     // draw the rectangle
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

        
         doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         return Result.Success;
      }

      /// <summary>
      /// Draws the cluster.
      /// </summary>
      /// <returns></returns>
      public virtual Result drawCluster( )
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<PunchingPoint> clusterPoints = new List<PunchingPoint>();
         // int clusterCounter = 0;

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

         // Go through each point in the point Map list to find cluster tool starting from bottom left corner
         //for (int i = 0; i < pointMap.YCount; i++)
         //{
         //   Dictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

         //   for (int j = 0; j < xDict.Count; j++)
         //   {
         //      // Check if a cluster tool can fit
         //      if (xDict.ElementAt(j).Value.HasPunched == true)
         //         continue;
         //      clusterPoints.Clear();

         //      clusterPoints.Add(xDict.ElementAt(j).Value);

         //      // Cluster tool X- Y- configuration
         //      for (int k = 1; k < punchingToolList[0].ClusterTool.ClusterPoints.Count; k++)
         //      {
         //         int x = (int)Math.Round(clusterPoints[0].Point.X + punchingToolList[0].ClusterTool.ClusterPoints[k].X, 1);
         //         int y = (int)Math.Round(clusterPoints[0].Point.Y + punchingToolList[0].ClusterTool.ClusterPoints[k].Y, 1);

         //         // Look for point in the point list
         //         PunchingPoint resultPoint = pointMap.getPoint(x, y);

         //         if (resultPoint == null)
         //         {
         //            // Cannot find point in the cluster tool, then skip
         //            break;
         //         }
         //         else
         //         {
         //            clusterPoints.Add(resultPoint);
         //         }
         //      }

         //      // If all the points are added
         //      if (clusterPoints.Count == punchingToolList[0].ClusterTool.ClusterPoints.Count)
         //      {
         //         double clusterX = punchingToolList[0].ClusterTool.ClusterCentre.X + clusterPoints[0].Point.X;
         //         double clusterY = punchingToolList[0].ClusterTool.ClusterCentre.Y + clusterPoints[0].Point.Y;

         //         // Draw all the points in the cluster tool
         //         clusterCounter++;

         //         if (clusterLayer)
         //         { 
         //            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
         //         }

         //         foreach (PunchingPoint pp in clusterPoints)
         //         {
         //            if (clusterLayer)
         //            { 
         //               // Draw the tool
         //               punchingToolList[0].drawTool(pp.Point);
         //            }

         //            // Remove all the points in the clustertool list
         //            pp.HasPunched = true;
         //         }

         //         // draw the rectangle
         //         punchingToolList[0].ClusterTool.drawClusterTool(clusterX, clusterY);
         //      }
         //   }
         //}

         //// Go through each point in the point Map list to find cluster tool starting from bottom right corner
         //for (int i = 0; i < pointMap.YCount; i++)
         //{
         //   Dictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

         //   for (int j = xDict.Count - 1; j >= 0; j--)
         //   {
         //      // If the point is punched, skip to the next point
         //      if (xDict.ElementAt(j).Value.HasPunched == true)
         //         continue;
         //      clusterPoints.Clear();

         //      clusterPoints.Add(xDict.ElementAt(j).Value);

         //      // Cluster tool X- Y- configuration
         //      for (int k = 1; k < punchingToolList[0].ClusterTool.ClusterPoints.Count; k++)
         //      {
         //         int x = (int)Math.Round(clusterPoints[0].Point.X + punchingToolList[0].ClusterTool.ClusterPointsXNYP[k].X, 1);
         //         int y = (int)Math.Round(clusterPoints[0].Point.Y + punchingToolList[0].ClusterTool.ClusterPointsXNYP[k].Y, 1);

         //         // Look for point in the point list
         //         PunchingPoint resultPoint = pointMap.getPoint(x, y);

         //         if (resultPoint == null)
         //         {
         //            // Cannot find point in the cluster tool, then skip
         //            break;
         //         }
         //         else
         //         {
         //            clusterPoints.Add(resultPoint);
         //         }
         //      }

         //      // If all the points are added
         //      if (clusterPoints.Count == punchingToolList[0].ClusterTool.ClusterPoints.Count)
         //      {
         //         double clusterX = clusterPoints[0].Point.X - punchingToolList[0].ClusterTool.ClusterCentre.X;
         //         double clusterY = clusterPoints[0].Point.Y + punchingToolList[0].ClusterTool.ClusterCentre.Y;

         //         // Draw all the points in the cluster tool
         //         clusterCounter++;
                  
         //         if (clusterLayer)
         //         {
         //            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
         //         }

         //         foreach (PunchingPoint pp in clusterPoints)
         //         {
         //            if (clusterLayer)
         //            {
         //               // Draw the tool
         //               punchingToolList[0].drawTool(pp.Point);
         //            }

         //            // Remove all the points in the clustertool list
         //            pp.HasPunched = true;
         //         }

         //         // draw the rectangle
         //         punchingToolList[0].ClusterTool.drawClusterTool(clusterX, clusterY);
         //      }
         //   }
         //}

         //// Go through each point in the point Map list to find cluster tool starting from top left corner
         //for (int i = pointMap.YCount - 1; i >= 0; i--)
         //{
         //   Dictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

         //   for (int j = 0; j < xDict.Count; j++)
         //   {
         //      // If the point is punched, skip to the next point
         //      if (xDict.ElementAt(j).Value.HasPunched == true)
         //         continue;
         //      clusterPoints.Clear();

         //      clusterPoints.Add(xDict.ElementAt(j).Value);

         //      // Cluster tool X- Y- configuration
         //      //for (int k = 1; k < punchingToolList[0].ClusterTool.ClusterPoints.Count; k++)
         //      //{

         //      //   int x = (int)Math.Round(clusterPoints[0].Point.X + punchingToolList[0].ClusterTool.ClusterPointsXPYN[k].X, 1);
         //      //   int y = (int)Math.Round(clusterPoints[0].Point.Y + punchingToolList[0].ClusterTool.ClusterPointsXPYN[k].Y, 1);

         //      //   // Look for point in the point list
         //      //   PunchingPoint resultPoint = pointMap.getPoint(x, y);

         //      //   if (resultPoint == null)
         //      //   {
         //      //      // Cannot find point in the cluster tool, then skip
         //      //      break;
         //      //   }
         //      //   else
         //      //   {
         //      //      clusterPoints.Add(resultPoint);
         //      //   }
         //      //}

         //      // If all the points are added
         //      if (clusterPoints.Count == punchingToolList[0].ClusterTool.ClusterPoints.Count)
         //      {
         //         double clusterX = clusterPoints[0].Point.X + punchingToolList[0].ClusterTool.ClusterCentre.X;
         //         double clusterY = clusterPoints[0].Point.Y - punchingToolList[0].ClusterTool.ClusterCentre.Y;

         //         // Draw all the points in the cluster tool
         //         clusterCounter++;
                 
         //         if (clusterLayer)
         //         {
         //            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
         //         }
                  
         //         foreach (PunchingPoint pp in clusterPoints)
         //         {
         //            if (clusterLayer)
         //            {
         //               // Draw the tool
         //               punchingToolList[0].drawTool(pp.Point);
         //            }

         //            // Remove all the points in the clustertool list
         //            pp.HasPunched = true;
         //         }

         //         // Draw rectangle for cluster tool
         //         punchingToolList[0].ClusterTool.drawClusterTool(clusterX, clusterY);
         //      }
         //   }
         //}

         //// Go through each point in the point Map list to find cluster tool starting from top right corner
         //for (int i = pointMap.YCount - 1; i >= 0; i--)
         //{
         //   Dictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

         //   for (int j = xDict.Count - 1; j >= 0; j--)
         //   {
         //      // If the point is punched, skip to the next point
         //      if (xDict.ElementAt(j).Value.HasPunched == true)
         //         continue;
         //      clusterPoints.Clear();

         //      clusterPoints.Add(xDict.ElementAt(j).Value);

         //      // Cluster tool X- Y- configuration
         //      for (int k = 1; k < punchingToolList[0].ClusterTool.ClusterPoints.Count; k++)
         //      {

         //         int x = (int)Math.Round(clusterPoints[0].Point.X + punchingToolList[0].ClusterTool.ClusterPointsXNYN[k].X, 1);
         //         int y = (int)Math.Round(clusterPoints[0].Point.Y + punchingToolList[0].ClusterTool.ClusterPointsXNYN[k].Y, 1);

         //         // Look for point in the point list
         //         PunchingPoint resultPoint = pointMap.getPoint(x, y);

         //         if (resultPoint == null)
         //         {
         //            // Cannot find point in the cluster tool, then skip
         //            break;
         //         }
         //         else
         //         {
         //            clusterPoints.Add(resultPoint);
         //         }
         //      }

         //      // If all the points are added
         //      if (clusterPoints.Count == punchingToolList[0].ClusterTool.ClusterPoints.Count)
         //      {
         //         double clusterX = clusterPoints[0].Point.X - punchingToolList[0].ClusterTool.ClusterCentre.X;
         //         double clusterY = clusterPoints[0].Point.Y - punchingToolList[0].ClusterTool.ClusterCentre.Y;

         //         // Draw all the points in the cluster tool
         //         clusterCounter++;

         //         if (clusterLayer)
         //         {
         //            doc.Layers.SetCurrentLayerIndex(CreateLayer("Cluster " + clusterCounter), true);
         //         }

         //         foreach (PunchingPoint pp in clusterPoints)
         //         {
         //            if (clusterLayer)
         //            {
         //               // Draw the tool
         //               punchingToolList[0].drawTool(pp.Point);
         //            }

         //            // Remove all the points in the clustertool list
         //            pp.HasPunched = true;
         //         }

         //         // draw the rectangle
         //         punchingToolList[0].ClusterTool.drawClusterTool(clusterX, clusterY);
         //      }
         //   }
         //}

         //doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         //// Places where cannot cluster punch
         //for (int i = 0; i < pointMap.YCount; i++)
         //{
         //   Dictionary<int, PunchingPoint> xDict = pointMap.getXDictionary(i);

         //   for (int j = 0; j < xDict.Count; j++)
         //   {
         //      if (xDict.ElementAt(j).Value.HasPunched == false)
         //      {
         //         // Draw the tool 
         //         punchingToolList[0].drawTool(xDict.ElementAt(j).Value.Point);
         //      }
         //   }
         //}

      
         doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         return Result.Success;
      }

      public PerforationPattern DeepCopy()
      {
         PerforationPattern pattern = (PerforationPattern) this.MemberwiseClone();
         pattern.perforationName = this.perforationName;
         pattern.xSpacing = this.xSpacing;
         pattern.ySpacing = this.ySpacing;
         pattern.pitch = this.pitch;
         pattern.randomness = this.randomness;
         pattern.openArea = this.openArea;
         pattern.atomicNumber = this.atomicNumber;
         pattern.punchingToolList = new List<PunchingTool>();

         foreach(PunchingTool t in this.punchingToolList)
         {
            PunchingTool newTool = t.DeepCopy();
            pattern.punchingToolList.Add(newTool);
         }

         return pattern;
   }

      /// <summary>
      /// Creates the layer.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <returns></returns>
      protected int CreateLayer(string name)
      {
         // Does a layer with the same name already exist?
         int layerIndex = RhinoDoc.ActiveDoc.Layers.Find(name, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = RhinoDoc.ActiveDoc.Layers.Add(name, System.Drawing.Color.Black);
         }

         return layerIndex;
      }
   }
}
