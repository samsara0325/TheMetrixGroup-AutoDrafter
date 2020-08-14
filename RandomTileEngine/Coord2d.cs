using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins.RandomTileEngine
{
   public class Coord2d
   {
      private int mX;
      private int mY;

      public Coord2d(int x, int y)
      {
         mX = x;
         mY = y;
      }

      /// <summary>
      /// </summary>
      /// <value>
      /// The x.
      /// </value>
      public int X
      {
         get
         {
            return mX;
         }
         set
         {
            mX = value;
         }
      }

      /// <summary>
      /// Gets or sets the y.
      /// </summary>
      /// <value>
      /// The y.
      /// </value>
      public int Y
      {
         get
         {
            return mY;
         }

         set
         {
            mY = value;
         }
      }
   }
}
