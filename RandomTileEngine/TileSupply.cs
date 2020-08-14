using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins.RandomTileEngine
{
   public class TileSupply
   {
      private int colourIndex;         // Color index 
      private int quantityAvailable;   // Available quantity of this colour (ignored when struct is used for counting neighbour colours)
      private int quantityUsed;        // Quantity used of this colour
      private int randomIndex;              // 

      public TileSupply()
      {

      }

      /// <summary>
      /// Gets or sets the index of the colour.
      /// </summary>
      /// <value>
      /// The index of the colour.
      /// </value>
      public int ColourIndex
      {
         get { return colourIndex; }
         set { colourIndex = value; }
      }

      /// <summary>
      /// Gets or sets the quantity available.
      /// </summary>
      /// <value>
      /// The quantity available.
      /// </value>
      public int QuantityAvailable
      {
         get { return quantityAvailable; }
         set { quantityAvailable = value; }
      }


      /// <summary>
      /// Gets or sets the quantity used.
      /// </summary>
      /// <value>
      /// The quantity used.
      /// </value>
      public int QuantityUsed
      {
         get { return quantityUsed; }
         set { quantityUsed = value; }
      }

      /// <summary>
      /// Gets or sets the random index.
      /// </summary>
      /// <value>
      /// The random index.
      /// </value>
      public int RandomIndex
      {
         get { return randomIndex; }
         set { randomIndex = value; }
      }

   }
}
