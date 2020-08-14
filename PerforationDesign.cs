using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// 
   /// </summary>
   [Serializable]
   public class PerforationDesign : IComparable<PerforationDesign>
   {
      private string name;
      private PerforationPattern pattern;

      /// <summary>
      /// Initializes a new instance of the <see cref="PerforationDesign"/> class.
      /// </summary>
      public PerforationDesign()
      {

      }

      /// <summary>
      /// Initializes a new instance of the <see cref="PerforationDesign"/> class.
      /// </summary>
      /// <param name="designName">Name of the design.</param>
      public PerforationDesign(string designName)
      {
         name = designName;
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
      /// Gets or sets the pattern.
      /// </summary>
      /// <value>
      /// The pattern.
      /// </value>
      public PerforationPattern Pattern 
      {
         get
         {
            return pattern;
         }

         set
         {
            pattern = value;
         }
      }
      //Method compares the name of 2 perforation design to sort in asscending order
      public int CompareTo(PerforationDesign other)
      {
         //Execute this block only if the names of both the objects are not round hole
         if (!this.Name.Contains("Round Hole") || !other.Name.Contains("Round Hole")) 
         {
            String[] getNameOne = this.name.Split(' '); //split to get the first part of the name (we dont need the full)
            String[] getNameTwo = other.name.Split(' ');
            //if (getNameOne[0].Equals("Custom") || getNameTwo[0].Equals("Custom")) //if custom return 0 to put custom in the last positon
            //{
            //   return 0;
            //}
            return getNameOne[0].CompareTo(getNameTwo[0]);
         }
         else //if pattern names are round holes
         {
            return this.Name.CompareTo(other.Name);
         }
      }
   }
}
