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
   public class Lens: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Lens()
      {
         base.Name = "Lens";
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
         Point3d pt1 = new Point3d(point3d.X + X / 2- tolerance, point3d.Y, point3d.Z);
         Point3d pt2 = new Point3d(point3d.X, point3d.Y + Y / 2 - tolerance, point3d.Z);
         Point3d pt3 = new Point3d(point3d.X - X / 2 + tolerance, point3d.Y, point3d.Z);
         Point3d pt4 = new Point3d(point3d.X, point3d.Y - Y / 2 + tolerance, point3d.Z);

         Arc top = new Arc(pt1, pt2, pt3);
         Arc bottom = new Arc(pt3, pt4, pt1);

         PolyCurve currentToolCurve = new PolyCurve();
         currentToolCurve.Append(top);
         currentToolCurve.Append(bottom);

         if (Curve.PlanarClosedCurveRelationship(closedCurve, currentToolCurve, Plane.WorldXY, 0) == RegionContainment.BInsideA)
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
         Point3d pt1 = new Point3d(point3d.X + X/2, point3d.Y, point3d.Z);
         Point3d pt2 = new Point3d(point3d.X, point3d.Y + Y/2, point3d.Z);
         Point3d pt3 = new Point3d(point3d.X - X/2, point3d.Y, point3d.Z);
         Point3d pt4 = new Point3d(point3d.X, point3d.Y - Y/2, point3d.Z);
        
         Arc top = new Arc(pt1, pt2, pt3);
         Arc bottom = new Arc(pt3, pt4, pt1);

         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(top);
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
         Curve weave = getCurve(new Point3d(0, 0, 0));
         AreaMassProperties areaMassProps = Rhino.Geometry.AreaMassProperties.Compute(weave);
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
