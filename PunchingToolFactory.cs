using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// PerforationFactory generate a list of available Perforation to be used
   /// </summary>
   public static class PunchingToolFactory
   {
      /// <summary>
      /// Initializes the <see cref="PerforationFactory"/> class.
      /// </summary>
      static PunchingToolFactory()
      {         

      }

      /// <summary>
      /// Gets the perforation list.
      /// </summary>
      /// <returns></returns>
      public static List<PunchingTool> GetToolList()
      {
         List<PunchingTool> toolList = new List<PunchingTool>();
         
         // Create the list of pattern object
         toolList.Add(new PunchingTools.Round());
         toolList.Add(new PunchingTools.Rectangle());
         toolList.Add(new PunchingTools.Obround());
         toolList.Add(new PunchingTools.Lens());
         toolList.Add(new PunchingTools.Weave());
         toolList.Add(new PunchingTools.EqTriangle());
         toolList.Add(new PunchingTools.Hexagon());
         return toolList;
      }
   }
}
