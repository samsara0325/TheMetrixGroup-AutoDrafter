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
   public class Hexagon: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Hexagon()
      {
         base.Name = "Hexagon";
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         return drawTool(point3d,0);
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
            return 2 * X * Math.Tan(30 *( Math.PI / 180));
         }
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
         Point3d pt1 = new Point3d(point3d.X, point3d.Y - Y / 2 + tolerance, point3d.Z);
         Point3d pt2 = new Point3d(point3d.X + X / 2 - tolerance, point3d.Y - Y / 4 +tolerance, point3d.Z);
         Point3d pt3 = new Point3d(point3d.X + X / 2 - tolerance, point3d.Y + Y / 4 - tolerance, point3d.Z);
         Point3d pt4 = new Point3d(point3d.X, point3d.Y + Y / 2 - tolerance, point3d.Z);
         Point3d pt5 = new Point3d(point3d.X - X / 2 + tolerance, point3d.Y + Y / 4 - tolerance, point3d.Z);
         Point3d pt6 = new Point3d(point3d.X - X / 2 + tolerance, point3d.Y - Y / 4 + tolerance, point3d.Z);

         Line one = new Line(pt1, pt2);
         Line two = new Line(pt2, pt3);
         Line three = new Line(pt3, pt4);
         Line four = new Line(pt4, pt5);
         Line five = new Line(pt5, pt6);
         Line six = new Line(pt6, pt1);

         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(one);
         polyCurve.Append(two);
         polyCurve.Append(three);
         polyCurve.Append(four);
         polyCurve.Append(five);
         polyCurve.Append(six);

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
      /// Gets the curve.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override Curve getCurve(Point3d point3d)
      {
         Point3d pt1 = new Point3d(point3d.X, point3d.Y - Y / 2, point3d.Z);
         Point3d pt2 = new Point3d(point3d.X + X / 2, point3d.Y - Y / 4, point3d.Z);
         Point3d pt3 = new Point3d(point3d.X + X / 2, point3d.Y + Y / 4, point3d.Z);
         Point3d pt4 = new Point3d(point3d.X , point3d.Y + Y / 2, point3d.Z);
         Point3d pt5 = new Point3d(point3d.X - X / 2, point3d.Y + Y / 4, point3d.Z);
         Point3d pt6 = new Point3d(point3d.X - X / 2, point3d.Y - Y / 4, point3d.Z);

         Line one = new Line(pt1,pt2);
         Line two = new Line(pt2, pt3);
         Line three = new Line(pt3, pt4);
         Line four = new Line(pt4, pt5);
         Line five = new Line(pt5, pt6);
         Line six = new Line(pt6, pt1);

         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(one);
         polyCurve.Append(two);
         polyCurve.Append(three);
         polyCurve.Append(four);
         polyCurve.Append(five);
         polyCurve.Append(six);
         return polyCurve;
      }

      /// <summary>
      /// Gets the area.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override double getArea()
      {
         Curve hex = getCurve(new Point3d(0, 0, 0));
         AreaMassProperties areaMassProps = Rhino.Geometry.AreaMassProperties.Compute(hex);
         double curveArea = areaMassProps.Area;

         return curveArea;
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
      /// Determines whether the specified curve is outside.
      /// </summary>
      /// <param name="curve">The curve.</param>
      /// <param name="distance">The distance.</param>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override bool isOutside(Point3d point3d, Curve curve, double distance)
      {
         Curve currentToolCurve = getCurve(point3d);
         RegionContainment result = Curve.PlanarClosedCurveRelationship(curve, currentToolCurve, Plane.WorldXY, 0);

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
