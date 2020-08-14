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
   public class EqTriangle: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public EqTriangle()
      {
         base.Name = "Eq Triangle";
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         double yMin = point3d.Y - (Math.Sqrt(3) * X / 2)/3;
         double yMax = point3d.Y + (Math.Sqrt(3) * X )/3;
         double xMin = point3d.X - X / 2;
         double xMax = point3d.X + X / 2;

         Line bottom = new Line(xMax, yMin, 0, xMin,yMin, 0);
         Line left = new Line(xMin, yMin, 0, point3d.X, yMax, 0);
         Line right = new Line(point3d.X, yMax, 0, xMax, yMin, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(bottom);
         polyCurve.Append(left);
         polyCurve.Append(right);

         RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

         return Result.Success;
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
      /// Insides the specified closed curve.
      /// </summary>
      /// <param name="ClosedCurve">The closed curve.</param>
      /// <returns></returns>
      public override bool isInside(Curve closedCurve,  Point3d point)
      {
         double tolerance = Properties.Settings.Default.Tolerance;

         double yMin = point.Y - (Math.Sqrt(3) * X / 2) / 3 + tolerance;
         double yMax = point.Y + (Math.Sqrt(3) * X ) / 3 - tolerance;
         double xMin = point.X - X / 2 + tolerance;
         double xMax = point.X + X / 2 - tolerance;

         Line bottom = new Line(xMax, yMin, 0, xMin, yMin, 0);
         Line left = new Line(xMin, yMin, 0, point.X, yMax, 0);
         Line right = new Line(point.X, yMax, 0, xMax, yMin, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(bottom);
         polyCurve.Append(left);
         polyCurve.Append(right);


         if (Curve.PlanarClosedCurveRelationship(closedCurve, polyCurve, Plane.WorldXY, 0) == RegionContainment.BInsideA)
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      /// <summary>
      /// Determines whether the specified closed curve is inside.
      /// </summary>
      /// <param name="closedCurve">The closed curve.</param>
      /// <param name="point">The point.</param>
      /// <param name="angleRadians">The angle radians.</param>
      /// <returns></returns>
      public override bool isInside(Curve closedCurve, Point3d point, double angleRadians)
      {
         double tolerance = Properties.Settings.Default.Tolerance;

         double yMin = point.Y - (Math.Sqrt(3) * X / 2) / 3 + tolerance;
         double yMax = point.Y + (Math.Sqrt(3) * X) / 3 - tolerance;
         double xMin = point.X - X / 2 + tolerance;
         double xMax = point.X + X / 2 - tolerance;

         Line bottom = new Line(xMax, yMin, 0, xMin, yMin, 0);
         Line left = new Line(xMin, yMin, 0, point.X, yMax, 0);
         Line right = new Line(point.X, yMax, 0, xMax, yMin, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(bottom);
         polyCurve.Append(left);
         polyCurve.Append(right);

         Transform xform = Transform.Rotation(angleRadians, new Point3d(point.X, point.Y, angleRadians));

         polyCurve.Transform(xform);
         
         if (Curve.PlanarClosedCurveRelationship(closedCurve, polyCurve, Plane.WorldXY, 0) == RegionContainment.BInsideA)
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      /// <summary>
      /// Gets the area.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override double getArea()
      {
         // X is the diameter
         return X * Y;
      }

      /// <summary>
      /// Gets the curve.
      /// </summary>
      /// <param name="pt"></param>
      /// <returns></returns>
      public override Curve getCurve(Point3d pt)
      {
         double yMin = pt.Y - (Math.Sqrt(3) * X / 2) / 3;
         double yMax = pt.Y + (Math.Sqrt(3) * X /3);
         double xMin = pt.X - X / 2;
         double xMax = pt.X + X / 2;

         Line bottom = new Line(xMax, yMin, 0, xMin, yMin, 0);
         Line left = new Line(xMin, yMin, 0, pt.X, yMax, 0);
         Line right = new Line(pt.X, yMax, 0, xMax, yMin, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(bottom);
         polyCurve.Append(left);
         polyCurve.Append(right);

         return polyCurve;
      }

      /// <summary>
      /// Determines whether the specified point3d is outside.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <param name="curve">The curve.</param>
      /// <param name="distance">The distance.</param>
      /// <returns></returns>
      public override bool isOutside(Point3d point3d, Curve curve, double distance)
      {
         return true;
      }
   }
}
