using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace MetrixGroupPlugins.RandomTileEngine
{
   public class RandomTiler
   {

      private int[,] weightData = new int[5, 5] 
       { { 1, 1, 2, 1, 1 }, 
         { 1, 2, 9, 2, 1 }, 
         { 2, 9, 0, 9, 2 }, 
         { 1, 2, 9, 2, 1 }, 
         { 1, 1, 2, 1, 1 } };


      private int scopeSize = 2;

      /// <summary>
      /// Initializes a new instance of the <see cref="RandomTileEngine"/> class.
      /// </summary>
      public RandomTiler()
      {

      }

      /// <summary>
      /// Gets the random map.
      /// </summary>
      /// <param name="tools">The tools.</param>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <returns></returns>
      public int[,] GetTileMap(List<int> tileCounts, int x, int y)
      {
         int noOfcolours = tileCounts.Count;
         int posX;
         int posY;
         int coordIndex;
         int[,] tileMap = new int[x, y];
         int[,] tilePattern;
         Coord2d[] coordToTile = new Coord2d[x * y];
         TileSupply[] neighbourColours = new TileSupply[noOfcolours + 1];
         coordIndex = 0;

         for(posX = 0; posX < x; posX ++)
         {
            for(posY = 0; posY < y; posY ++)
            {
               coordToTile[coordIndex] = new Coord2d(posX + scopeSize, posY + scopeSize);
               coordIndex++;
            }
         }

         coordIndex--;
         int i = coordIndex;
         int randomIndex = 0;
         Random rnd = new Random();

         while(i > 0)
         {
            randomIndex = rnd.Next(i+1);
            int tempX = coordToTile[i].X;
            int tempY = coordToTile[i].Y;

            coordToTile[i].X = coordToTile[randomIndex].X;
            coordToTile[i].Y = coordToTile[randomIndex].Y;

            coordToTile[randomIndex].X = tempX;
            coordToTile[randomIndex].Y = tempY;

            i--;
         }

         // Create the tile Pattern
         tilePattern = new int[x + (2*scopeSize) + 1, y + (2*scopeSize) + 1];

         for (int j = 0; j <= noOfcolours; j++)
         {
            neighbourColours[j] = new TileSupply();
         }

         while (coordIndex >= 0)
         {
            posX = coordToTile[coordIndex].X;
            posY = coordToTile[coordIndex].Y;

            for(int j=0; j <= noOfcolours; j ++)
            {
               neighbourColours[j].ColourIndex = j;
               neighbourColours[j].QuantityUsed = 0;
               neighbourColours[j].RandomIndex = rnd.Next(100);
            }

            int posRelX, posRelY;

            for(posRelX = -scopeSize; posRelX <= scopeSize; posRelX++)
            {
               for (posRelY = -scopeSize; posRelY <= scopeSize; posRelY++)
               {
                  int neighbourColour = tilePattern[posX + posRelX, posY + posRelY];
                  neighbourColours[neighbourColour].QuantityUsed = neighbourColours[neighbourColour].QuantityUsed + weightData[scopeSize + posRelX, scopeSize + posRelY];
               }
            }

            neighbourColours[0].ColourIndex = 0;
            neighbourColours[0].QuantityUsed = 0;

            bool didSwap;

            do
            {
               didSwap = false;

               for(int k = 1; k < noOfcolours; k++)
               {
                  if (neighbourColours[k].QuantityUsed > neighbourColours[k + 1].QuantityUsed)
                  {
                     didSwap = true;
                  }
                  else if(neighbourColours[k].QuantityUsed == neighbourColours[k+1].QuantityUsed)
                  {
                     if(neighbourColours[k].RandomIndex > neighbourColours[k+1].RandomIndex)
                     {
                        didSwap = true;
                     }
                  }

                  if (didSwap == true)
                  {
                     int tempCI = neighbourColours[k].ColourIndex;
                     int tempQU = neighbourColours[k].QuantityUsed;
                     int tempRdn = neighbourColours[k].RandomIndex;

                     neighbourColours[k].ColourIndex = neighbourColours[k+1].ColourIndex;
                     neighbourColours[k].QuantityUsed = neighbourColours[k+1].QuantityUsed;
                     neighbourColours[k].RandomIndex = neighbourColours[k+1].RandomIndex;

                     neighbourColours[k+1].ColourIndex = tempCI;
                     neighbourColours[k+1].QuantityUsed = tempQU;
                     neighbourColours[k+1].RandomIndex = tempRdn;
                  }
               }

            } while (didSwap == true);

            int chosenColour = 1;

            for(int l=1; l <= noOfcolours; l++)
            {
               chosenColour = neighbourColours[l].ColourIndex;

               // Check the availability
               if (tileCounts[chosenColour-1] > 0)
               {
                  tileCounts[chosenColour-1] = tileCounts[chosenColour-1] - 1;
                  break;
               }
            }

            tilePattern[posX, posY] = chosenColour;

            coordIndex--;
         }

         for (int a = 0; a < x; a++)
         {
            for (int b = 0; b < y; b++)
            {
               tileMap[a, b] = tilePattern[a + scopeSize, b + scopeSize];
            }
         }

         return tileMap;
      }

      /// <summary>
      /// Swaps the specified value a.
      /// </summary>
      /// <param name="valueA">The value a.</param>
      /// <param name="valueB">The value b.</param>
      public void Swap(ref int valueA,  ref int valueB)
      {
         int temp;
         temp = valueA;

         valueA = valueB;
         valueB = temp;
      }

      /// <summary>
      /// Gets or sets the weight.
      /// </summary>
      /// <value>
      /// The weight.
      /// </value>
      public int[,] Weight
      {
         get
         {
            return weightData;
         }

         set
         {
            weightData = value;
         }
      }
   }
}
