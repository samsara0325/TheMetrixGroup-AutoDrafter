using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace MetrixGroupPlugins
{
   public class PunchingPoint
   {
      private bool hasPunched;
      private Point3d point; 
      
      /// <summary>
      /// Initializes a new instance of the <see cref="PunchingPoint"/> class.
      /// </summary>
      /// <param name="pt">The pt.</param>
      public PunchingPoint(Point3d pt)
      {
         hasPunched = false;
         point = pt;
      }

      /// <summary>
      /// Gets or sets a value indicating whether this instance has punched.
      /// </summary>
      /// <value>
      /// <c>true</c> if this instance has punched; otherwise, <c>false</c>.
      /// </value>
      public bool HasPunched
      {
         get
         {
            return hasPunched;
         }

         set
         {
            hasPunched = value;
         }
      }

      /// <summary>
      /// Gets or sets the point.
      /// </summary>
      /// <value>
      /// The point.
      /// </value>
      public Point3d Point 
      {
         get
         {
            return point;
         } 
         set
         {
            point = value;
         } 
      }
   }
}
