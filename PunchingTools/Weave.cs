using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino;

namespace MetrixGroupPlugins.PunchingTools
{
   public class Weave: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Weave()
      {
         base.Name = "Weave";
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         return drawTool(point3d, this.Angle);
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <param name="angleRadians">The angle radians.</param>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
      public override Result drawTool(Point3d point3d, double angleRadians)
      {
         Guid toolGuid = new Guid();
         Transform xform = Transform.Rotation(angleRadians, new Point3d(point3d.X, point3d.Y, 0));

         Curve polyCurve = getCurve(point3d);
         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         return Result.Success;
      }

      /// <summary>
      /// Gets or sets the y.
      /// </summary>
      /// <value>
      /// The y.
      /// </value>
      public override double Y
      {
         get
         {
            return 17.0424;
         }
      }

      /// <summary>
      /// Gets or sets the y.
      /// </summary>
      /// <value>
      /// The y.
      /// </value>
      public override double X
      {
         get
         {
            return 17.0424;
         }
      }

      /// <summary>
      /// Insides the specified closed curve.
      /// </summary>
      /// <param name="ClosedCurve">The closed curve.</param>
      /// <returns></returns>
      public override bool isInside(Curve closedCurve,  Point3d point)
      {
         double tolerance = Properties.Settings.Default.Tolerance;

         return closedCurve.Contains(point, Plane.WorldXY,  8.5212 - tolerance) == PointContainment.Inside;
      }

      /// <summary>
      /// Determines whether the specified closed curve is inside.
      /// </summary>
      /// <param name="closedCurve">The closed curve.</param>
      /// <param name="point">The point.</param>
      /// <param name="radians">The radians.</param>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
      public override bool isInside(Curve closedCurve, Point3d point, double radians)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Gets the curve.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override Curve getCurve(Point3d point3d)
      {
         Point3d pt1 = new Point3d(point3d.X + 5.4161, point3d.Y - 7.0828, point3d.Z);
         Point3d pt2 = new Point3d(point3d.X + 3.4788, point3d.Y - 0, point3d.Z);
         Point3d pt3 = new Point3d(point3d.X + 5.4161, point3d.Y + 7.0828, point3d.Z);
         Point3d pt4 = new Point3d(point3d.X + 0.0000, point3d.Y + 8.5212, point3d.Z);
         Point3d pt5 = new Point3d(point3d.X - 5.4161, point3d.Y + 7.0828, point3d.Z);
         Point3d pt6 = new Point3d(point3d.X - 3.4788, point3d.Y - 0, point3d.Z);
         Point3d pt7 = new Point3d(point3d.X - 5.4161, point3d.Y - 7.0828, point3d.Z);
         Point3d pt8 = new Point3d(point3d.X + 0.0000, point3d.Y - 8.5212, point3d.Z);

         Arc right = new Arc(pt1, pt2, pt3);
         Arc top = new Arc(pt3, pt4, pt5);
         Arc left = new Arc(pt5, pt6, pt7);
         Arc bottom = new Arc(pt7, pt8, pt1);

         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(right);
         polyCurve.Append(top);
         polyCurve.Append(left);
         polyCurve.Append(bottom);

         return polyCurve;
      }

      /// <summary>
      /// Gets the area.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override double getArea()
      {
         // X is the diameter
         Curve weave = getCurve(new Point3d(0, 0, 0));
         AreaMassProperties areaMassProps = Rhino.Geometry.AreaMassProperties.Compute(weave);
         double curveArea = areaMassProps.Area;

         return curveArea;
      }

      /// <summary>
      /// Determines whether the specified curve is outside.
      /// </summary>
      /// <param name="curve">The curve.</param>
      /// <param name="distance">The distance.</param>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override bool isOutside(Point3d point3d, Curve curve, double distance)
      {
         Curve currentToolCurve = getCurve(point3d);
         RegionContainment result = Curve.PlanarClosedCurveRelationship(curve, currentToolCurve, Plane.WorldXY, Properties.Settings.Default.Tolerance);

         if (result == RegionContainment.Disjoint)
         {
            return true;
         }
         else
         {
            return false;
         }
      }
   }
}
