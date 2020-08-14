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
   public class Rectangle: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Rectangle()
      {
         base.Name = "Rectangle";
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         BoundingBox clusterBox = new BoundingBox(new Point3d(point3d.X - X / 2, point3d.Y - Y / 2, 0), new Point3d(point3d.X + X / 2, point3d.Y + Y / 2, 0));
         List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();
         // add 1st point at last to close the loop
         rectangle_corners.Add(rectangle_corners[0]);
         RhinoDoc.ActiveDoc.Objects.AddPolyline(rectangle_corners);

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
         throw new NotImplementedException();
      }


      /// <summary>
      /// Insides the specified closed curve.
      /// </summary>
      /// <param name="ClosedCurve">The closed curve.</param>
      /// <returns></returns>
      public override bool isInside(Curve closedCurve,  Point3d point)
      {
         double tolerance = Properties.Settings.Default.Tolerance;

         BoundingBox clusterBox = new BoundingBox(new Point3d(point.X - X / 2 + tolerance, point.Y - Y / 2 + tolerance, 0), new Point3d(point.X + X / 2 - tolerance, point.Y + Y / 2 - tolerance, 0));
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
         BoundingBox clusterBox = new BoundingBox(new Point3d(pt.X - X / 2, pt.Y - Y / 2, 0), new Point3d(pt.X + X / 2, pt.Y + Y / 2, 0));
         List<Point3d> rectangle_corners = clusterBox.GetCorners().Distinct().ToList();
         // add 1st point at last to close the loop
         rectangle_corners.Add(rectangle_corners[0]);

         return new PolylineCurve(rectangle_corners);
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
