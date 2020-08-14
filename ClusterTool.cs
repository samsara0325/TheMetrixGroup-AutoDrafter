using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino;
using Rhino.Commands;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// Cluster Tool
   /// </summary>
   [Serializable]
   public class ClusterTool
   {
      private ClusterToolShape shape;
      private bool enable;
      private int pinsX;
      private int pinsY;
      private int multiplierX;
      private int multiplierY;
      private bool allowOverPunch;
      private bool rotatable;

      public enum ClusterToolShape
      {
         RectangleSmall,  // 2 x 1 Rect
         RectangleLarge,  // 2.5 x 1 Rect
         Rectangle90Small, // 1 x 2 Rect
         Rectangle90Large,  // 1 x 2.5 Rect
         HalfCircle, // Half circle
         Triangle,    // Triangle
         HalfCircleRotate // Half circle
      };

      /// <summary>
      /// Initializes a new instance of the <see cref="ClusterTool"/> class.
      /// </summary>
      public ClusterTool()
      {

      }

      /// <summary>
      /// Gets or sets the shape.
      /// </summary>
      /// <value>
      /// The shape.
      /// </value>
      public ClusterToolShape Shape
      {
         get
         {
            return shape;
         }

         set
         {
            shape = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="ClusterTool"/> is enable.
      /// </summary>
      /// <value>
      ///   <c>true</c> if enable; otherwise, <c>false</c>.
      /// </value>
      public bool Enable
      {
         get
         {
            return enable;
         }
         set
         {
            enable = value;
         }
      }

      /// <summary>
      /// Gets or sets the pins x.
      /// </summary>
      /// <value>
      /// The pins x.
      /// </value>
      public int PinsX
      {
         get
         {
            return pinsX;
         }
         set
         {
            pinsX = value;
         }
      }

      /// <summary>
      /// Gets or sets the pins y.
      /// </summary>
      /// <value>
      /// The pins y.
      /// </value>
      public int PinsY
      {
         get
         {
            return pinsY;
         }
         set
         {
            pinsY = value;
         }
      }


      /// <summary>
      /// Gets or sets the multiplier x.
      /// </summary>
      /// <value>
      /// The multiplier x.
      /// </value>
      public int MultiplierX
      {
         get
         {
            return multiplierX;
         }
         set
         {
            multiplierX = value;
         }
      }

      /// <summary>
      /// Gets or sets the multiplier y.
      /// </summary>
      /// <value>
      /// The multiplier y.
      /// </value>
      public int MultiplierY
      {
         get
         {
            return multiplierY;
         }
         set
         {
            multiplierY = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [allow over punch].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [allow over punch]; otherwise, <c>false</c>.
      /// </value>
      public bool AllowOverPunch
      {
         get
         {
            return allowOverPunch;
         }

         set
         { 
            allowOverPunch = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="ClusterTool"/> is rotatable.
      /// </summary>
      /// <value>
      ///   <c>true</c> if rotatable; otherwise, <c>false</c>.
      /// </value>
      public bool Rotatable
      {
         get
         {
            return rotatable;
         }

         set
         {
            rotatable = value;
         }
      }


      /// <summary>
      /// Draws the cluster tool.
      /// </summary>
      /// <param name="X">The x.</param>
      /// <param name="Y">The y.</param>
      public void drawClusterTool(double x, double y, double angleRadians)
      {
         Guid toolGuid = new Guid();
         Transform xform = Transform.Rotation(angleRadians, new Point3d(x,y,0));

         if (shape == ClusterToolShape.RectangleSmall)
         {
            BoundingBox clusterBox = new BoundingBox(new Point3d(x - 1, y - 0.5, 0), new Point3d(x + 1, y + 0.5, 0));
            List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();

            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            toolGuid = RhinoDoc.ActiveDoc.Objects.AddPolyline(rectangle_corners);
         }
         else if(shape == ClusterToolShape.RectangleLarge)
         {
            BoundingBox clusterBox = new BoundingBox(new Point3d(x - 1.25, y - 0.5, 0), new Point3d(x + 1.25, y + 0.5, 0));
            List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();


            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            toolGuid = RhinoDoc.ActiveDoc.Objects.AddPolyline(rectangle_corners);
         }
         else if (shape == ClusterToolShape.Rectangle90Small)
         {
            BoundingBox clusterBox = new BoundingBox(new Point3d(x - 0.5, y - 1, 0), new Point3d(x + 0.5, y + 1, 0));
            List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();


            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            toolGuid = RhinoDoc.ActiveDoc.Objects.AddPolyline(rectangle_corners);
         }
         else if (shape == ClusterToolShape.Rectangle90Large)
         {
            BoundingBox clusterBox = new BoundingBox(new Point3d(x - 0.5, y - 1.25, 0), new Point3d(x + 0.5, y + 1.25, 0));
            List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();

            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            toolGuid = RhinoDoc.ActiveDoc.Objects.AddPolyline(rectangle_corners);
         }
         else if(shape == ClusterToolShape.HalfCircle)
         {
            Point3d pt = new Point3d(x, y - 0.5, 0);
            Arc topArc = new Arc(new Circle(pt, 1), Math.PI);
            topArc.StartAngle = 0;
            topArc.EndAngle = Math.PI ;

            Line bottom = new Line(x - 1, y - 0.5, 0, x + 1, y - 0.5, 0);
            PolyCurve polyCurve = new PolyCurve();
            polyCurve.Append(topArc);
            polyCurve.Append(bottom);
            toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

            double rotationRadians = Properties.Settings.Default.ClusterToolRotation * Math.PI / 180;

            xform = Transform.Rotation(rotationRadians, new Point3d(x, y, 0));
         }
         else if(shape == ClusterToolShape.Triangle)
         {
            Line bottom = new Line(x + 1, y - 0.5, 0, x - 1, y - 0.5, 0);
            Line left = new Line(x - 1, y - 0.5, 0, x, y + 0.5, 0);
            Line right = new Line(x, y + 0.5, 0, x + 1, y - 0.5, 0);
            PolyCurve polyCurve = new PolyCurve();
            polyCurve.Append(bottom);
            polyCurve.Append(left);
            polyCurve.Append(right);

            toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         }
         else if (shape == ClusterToolShape.HalfCircleRotate)
         {
            Point3d pt = new Point3d(x, y - 0.5, 0);
            Arc topArc = new Arc(new Circle(pt, 1), Math.PI);
            topArc.StartAngle = 0;
            topArc.EndAngle = Math.PI;

            Line bottom = new Line(x - 1, y - 0.5, 0, x + 1, y - 0.5, 0);
            PolyCurve polyCurve = new PolyCurve();
            polyCurve.Append(topArc);
            polyCurve.Append(bottom);

            toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
            double rotationRadians = Properties.Settings.Default.ClusterToolRotation * Math.PI / 180;

            xform = Transform.Rotation(rotationRadians, new Point3d(x, y, 0));
         }

         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);
      }

      /// <summary>
      /// Deeps the copy.
      /// </summary>
      /// <returns></returns>
      public ClusterTool DeepCopy()
      {
         ClusterTool clusterTool = (ClusterTool) this.MemberwiseClone();
         clusterTool.shape = this.shape;
         clusterTool.enable = this.enable;
         clusterTool.pinsX = this.pinsX;
         clusterTool.pinsY = this.pinsY;
         clusterTool.multiplierX = this.multiplierX;
         clusterTool.multiplierY = this.multiplierY;
         clusterTool.allowOverPunch = this.allowOverPunch;
         clusterTool.rotatable = this.rotatable;

         return clusterTool;
      }
    }
}
