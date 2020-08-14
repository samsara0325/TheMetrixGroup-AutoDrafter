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
   public class Obround: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Obround()
      {
         base.Name = "Obround";
      }


      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         Curve polyCurve = getCurve(point3d);

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
      public override bool isInside(Curve closedCurve,  Point3d point3d)
      {
         double tolerance = Properties.Settings.Default.Tolerance;

         BoundingBox clusterBox = new BoundingBox(new Point3d(point3d.X - X / 2 + tolerance, point3d.Y - Y / 2 + tolerance, 0), new Point3d(point3d.X + X / 2 - tolerance, point3d.Y + Y / 2 - tolerance, 0));
         List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();

         // add 1st point at last to close the loop
         rectangle_corners.Add(rectangle_corners[0]);
         Curve rectangle = new PolylineCurve(rectangle_corners);

         if (Curve.PlanarClosedCurveRelationship(closedCurve, rectangle, Plane.WorldXY, 0) == RegionContainment.BInsideA)
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      /// <summary>
      /// Gets the curve.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override Curve getCurve(Point3d point3d)
      {
         Point3d leftPoint = new Point3d(point3d.X - (this.X / 2) + (Y / 2), point3d.Y, 0);
         Point3d rightPoint = new Point3d(point3d.X + (this.X / 2) - (Y / 2), point3d.Y, 0);

         Arc leftArc = new Arc(new Circle(leftPoint, Y / 2), Math.PI);
         leftArc.StartAngle = Math.PI / 2;
         leftArc.EndAngle = 3 * Math.PI / 2;

         Arc rightArc = new Arc(new Circle(rightPoint, Y / 2), Math.PI);
         rightArc.StartAngle = -Math.PI / 2;
         rightArc.EndAngle = Math.PI / 2;

         Line top = new Line(point3d.X + (this.X / 2) - (Y / 2), point3d.Y + Y / 2, 0, point3d.X - (this.X / 2) + (Y / 2), point3d.Y + Y / 2, 0);
         Line bottom = new Line(point3d.X - (this.X / 2) + (Y / 2), point3d.Y - Y / 2, 0, point3d.X + (this.X / 2) - (Y / 2), point3d.Y - Y / 2, 0);

         PolyCurve polyCurve = new PolyCurve();

         polyCurve.Append(leftArc);
         polyCurve.Append(bottom);
         polyCurve.Append(rightArc);
         polyCurve.Append(top);

         return polyCurve;
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
      /// Gets the area.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override double getArea()
      {
         // X is the diameter
         return (X-Y)*Y + Math.PI * (Y/2) * (Y/2);
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
         
         return true;
      }
   }
}
