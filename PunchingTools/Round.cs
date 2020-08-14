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
   public class Round: PunchingTool
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Round"/> class.
      /// </summary>
      public Round()
      {
         base.Name = "Round";
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public override Result drawTool(Point3d point3d)
      {
         RhinoDoc.ActiveDoc.Objects.AddCircle(new Circle(point3d, X / 2));

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
      /// Gets or sets the y.
      /// </summary>
      /// <value>
      /// The y.
      /// </value>
      public override double Y
      {
         get
         {
            return base.X;
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

         return closedCurve.Contains(point, Plane.WorldXY, X / 2 - tolerance) == PointContainment.Inside;
      }

      /// <summary>
      /// Gets the curve.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override Curve getCurve(Point3d point3d)
      {
         return new ArcCurve(new Circle(point3d, X / 2));
      }

      /// <summary>
      /// Gets the area.
      /// </summary>
      /// <returns></returns>
      /// <exception cref="System.NotImplementedException"></exception>
      public override double getArea()
      {
         // X is the diameter
         return Math.PI * (X/2) * (X/2); 
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
         Curve currentToolCurve = new ArcCurve(new Circle(point3d, (X / 2) + distance));
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
