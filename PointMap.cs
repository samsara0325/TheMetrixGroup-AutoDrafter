using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MetrixGroupPlugins
{
   public class PointMap
   {
      private SortedDictionary<int, SortedDictionary<int, PunchingPoint>> pointDictionary;

      /// <summary>
      /// Initializes a new instance of the <see cref="PointMap"/> class.
      /// </summary>
      public PointMap()
      {
         pointDictionary = new SortedDictionary<int, SortedDictionary<int, PunchingPoint>>();
      }

      /// <summary>
      /// Adds the point.
      /// </summary>
      /// <param name="punchingPoint">The punching point.</param>
      public void AddPoint(PunchingPoint punchingPoint)
      {
         SortedDictionary<int, PunchingPoint> xDictionary;

         int x = getIndexValue(punchingPoint.Point.X);
         int y = getIndexValue(punchingPoint.Point.Y);

         // If the dictionary exist
         if(pointDictionary.ContainsKey(y))
         {
            xDictionary = pointDictionary[y];

            if(xDictionary.ContainsKey(x) == false)
            {
               xDictionary.Add(x, punchingPoint);
            }
         }
         else
         {
            xDictionary = new SortedDictionary<int, PunchingPoint>();
            xDictionary.Add(x, punchingPoint);

            pointDictionary.Add(y, xDictionary);
         }        
      }

      /// <summary>
      /// Gets the point.
      /// </summary>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <returns></returns>
      public PunchingPoint getPoint(double x, double y)
      {
         int indexX = getIndexValue(x);
         int indexY = getIndexValue(y);

         if (pointDictionary.ContainsKey(indexY))
         {
            if (pointDictionary[indexY].ContainsKey(indexX))
            {
               return pointDictionary[indexY][indexX];
            }
         }

         return null;
      }

      /// <summary>
      /// Gets the point.
      /// </summary>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <returns></returns>
      public PunchingPoint getPoint2(double x, double y)
      {
         int indexX = getIndexValue(x);
         int indexY = getIndexValue(y);

         int actualIndexY = int.MinValue;
         int actualIndexX = int.MinValue;

         if(pointDictionary.ContainsKey(indexY))
         {
            actualIndexY = indexY;
         }
         else if(pointDictionary.ContainsKey(indexY - 1))
         {
            actualIndexY = indexY - 1;
         }
         else if (pointDictionary.ContainsKey(indexY + 1))
         {
            actualIndexY = indexY + 1;
         }
         else
         {
            return null;
         }

         if (pointDictionary[actualIndexY].ContainsKey(indexX))
         {
            actualIndexX = indexX;
         }
         else if (pointDictionary[actualIndexY].ContainsKey(indexX - 1))
         {
            actualIndexX = indexX - 1;
         }
         else if (pointDictionary[actualIndexY].ContainsKey(indexX + 1))
         {
            actualIndexX = indexX + 1;
         }
         else
         {
            return null;
         }

         return pointDictionary[actualIndexY][actualIndexX];
      }


      /// <summary>
      /// Gets the point.
      /// </summary>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <param name="idx">The index.</param>
      /// <param name="idy">The idy.</param>
      /// <returns></returns>
      public PunchingPoint getPoint(double x, double y, int idx, int idy)
      {
         return null;
      }


      /// <summary>
      /// Gets the index value.
      /// </summary>
      /// <returns></returns>
      public int getIndexValue(double x)
      {
         return (int) (Math.Round(x, 7) * 10);
      }

      /// <summary>
      /// Gets the y count.
      /// </summary>
      /// <value>
      /// The y count.
      /// </value>
      public int YCount 
      {
         get
         {
            return pointDictionary.Count;
         }
      }

      /// <summary>
      /// Gets or sets the count.
      /// </summary>
      /// <value>
      /// The count.
      /// </value>
      public int Count 
      {
         get
         {
            int count = 0;

            for (int i = 0; i < pointDictionary.Count; i++)
            {
               count = count + pointDictionary.ElementAt(i).Value.Count;
            }

            return count;
         }
      }

      /// <summary>
      /// Gets the x array.
      /// </summary>
      /// <returns></returns>
      public SortedDictionary<int, PunchingPoint> getXDictionary(int y)
      {
         return pointDictionary.ElementAt(y).Value;
      }
   }
}
