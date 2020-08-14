using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino.Geometry;
using System.Xml.Serialization;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// 
   /// </summary>
   [XmlInclude(typeof(PunchingTools.Round))]
   [XmlInclude(typeof(PunchingTools.Rectangle))]
   [XmlInclude(typeof(PunchingTools.Obround))]
   [XmlInclude(typeof(PunchingTools.Lens))]
   [XmlInclude(typeof(PunchingTools.Weave))]
   [XmlInclude(typeof(PunchingTools.EqTriangle))]
   [XmlInclude(typeof(PunchingTools.Hexagon))]
   [Serializable]
   public abstract class PunchingTool
   {
      private string name;
      private string displayName;
      private double x;
      private double y;
      private double areaPercentage;
      private double gap;
      private ClusterTool clusterTool;
      private double angle;
        private bool enablePerforation;
        private bool enableToolHit;

      /// <summary>
      /// Initializes a new instance of the <see cref="PunchingTool"/> class.
      /// </summary>
        public PunchingTool()
      {
         angle = 0;
         clusterTool = new ClusterTool();
      }

      /// <summary>
      /// Gets or sets the name.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      public string Name 
      {
         get
         {
            return name;
         }
         set
         {
            name = value;
         }
      }

      /// <summary>
      /// Gets or sets the display name.
      /// </summary>
      /// <value>
      /// The display name.
      /// </value>
      public string DisplayName
      {
         get
         {
            return displayName;
         }

         set
         {
            displayName = value;
         }
      }

      /// <summary>
      /// Gets or sets the x.
      /// </summary>
      /// <value>
      /// The x.
      /// </value>
      public virtual double X
      {
         get
         {
            return x;
         }

         set
         {
            x = value;
         }
      }


      /// <summary>
      /// Gets or sets the y.
      /// </summary>
      /// <value>
      /// The y.
      /// </value>
      public virtual double Y
      {
         get
         {
            return y;
         }

         set
         {
            y = value;
         }
      }

      /// <summary>
      /// Gets or sets the area percentage.
      /// </summary>
      /// <value>
      /// The area percentage.
      /// </value>
      public double AreaPercentage 
      {
         get
         {
            return areaPercentage;
         }

         set
         {
            areaPercentage = value;
         }
      }

      /// <summary>
      /// Gets or sets the cluster tool.
      /// </summary>
      /// <value>
      /// The cluster tool.
      /// </value>
      public ClusterTool ClusterTool
      {
         get
         {
            return clusterTool;
         }

         set
         {
            clusterTool = value;
         }
      } 
      /// <summary>
      /// Gets or sets the cluster tool.
      /// </summary>
      /// <value>
      /// The cluster tool.
      /// </value>
      public bool EnableToolHit
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
      /// Gets or sets whether to enable perforation.
      /// </summary>
      /// <value>
      /// The cluster tool.
      /// </value>
      public bool Perforation
      {
         get
         {
            return enablePerforation;
         }

         set
         {
                enablePerforation = value;
         }
      }

      /// <summary>
      /// Gets or sets the gap.
      /// </summary>
      /// <value>
      /// The gap.
      /// </value>
      public double Gap 
      {
         get
         {
            return gap;
         }

         set
         {
            gap = value; ;
         }
      }


      /// <summary>
      /// Gets or sets the angle.
      /// </summary>
      /// <value>
      /// The angle.
      /// </value>
      public double Angle
      {
         get
         {
            return angle;
         }

         set
         {
            angle = value;
         }
      }

      public abstract double getArea();

      /// <summary>
      /// Shadows the copy.
      /// </summary>
      /// <returns></returns>
      public PunchingTool ShallowCopy()
      {
         return (PunchingTool)this.MemberwiseClone();
      }

      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <returns></returns>
      public abstract Result drawTool(Point3d point3d);


      /// <summary>
      /// Draws the tool.
      /// </summary>
      /// <param name="point3d">The point3d.</param>
      /// <param name="angleRadians">The angle radians.</param>
      /// <returns></returns>
      public abstract Result drawTool(Point3d point3d, double angleRadians);

      /// <summary>
      /// Insides the specified closed curve.
      /// </summary>
      /// <param name="ClosedCurve">The closed curve.</param>
      /// <returns></returns>
      public abstract bool isInside(Curve closedCurve, Point3d point);

      /// <summary>
      /// Determines whether the specified closed curve is inside.
      /// </summary>
      /// <param name="closedCurve">The closed curve.</param>
      /// <param name="point">The point.</param>
      /// <param name="radians">The radians.</param>
      /// <returns></returns>
      public abstract bool isInside(Curve closedCurve, Point3d point, double radians);

      /// <summary>
      /// Determines whether the specified point is outside the curve supplied by the distance amount
      /// </summary>
      /// <param name="point">The point.</param>
      /// <param name="curve">The curve.</param>
      /// <param name="distance">The distance.</param>
      /// <returns></returns>
      public abstract bool isOutside(Point3d point, Curve curve, double distance);

      /// <summary>
      /// Gets the curve.
      /// </summary>
      /// <returns></returns>
      public abstract Curve getCurve(Point3d pt);

      /// <summary>
      /// Copies this instance.
      /// </summary>
      public void Copy(PunchingTool source)
      {
         this.X = source.X;
         this.Y = source.Y;
         this.gap = source.Gap;
         // this.name = source.Name;
         this.areaPercentage = source.AreaPercentage;
         this.DisplayName = source.DisplayName;
         this.ClusterTool = source.ClusterTool;
      }

      /// <summary>
      /// Copies this instance.
      /// </summary>
      public PunchingTool DeepCopy()
      {
         PunchingTool tool = (PunchingTool) this.MemberwiseClone();
         tool.x = this.x;
         tool.y = this.Y;
         tool.gap = this.gap;
         tool.name = this.name;
         tool.areaPercentage = this.areaPercentage;
         tool.displayName = this.DisplayName;
         tool.ClusterTool = this.ClusterTool.DeepCopy();

         return tool;
      }
   }
}
