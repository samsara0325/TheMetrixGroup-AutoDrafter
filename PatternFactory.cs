using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MetrixGroupPlugins.Patterns;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// PerforationFactory generate a list of available Perforation to be used
   /// </summary>
   public static class PatternFactory
   {
      /// <summary>
      /// Gets the perforation list.
      /// </summary>
      /// <returns></returns>
      public static List<PerforationPattern> GetPatternList()
      {
         List<PerforationPattern> patternList = new List<PerforationPattern>();

         // Create the list of pattern object
         patternList.Add(new FourtyFiveDegreePattern(true));
         patternList.Add(new SixtyDegreePattern(true));
         patternList.Add(new NintyDegreePattern(true));
         patternList.Add(new StraightPattern(true));
         patternList.Add(new AquaPattern(true));
         patternList.Add(new AtomicPoissonPattern(true));
         patternList.Add(new BraillePattern(true));
         patternList.Add(new MorsePattern(true));
         //patternList.Add(new CrescendaPattern(true));
         patternList.Add(new TechnoPattern(true));
         patternList.Add(new StaggeredPattern(true));
         patternList.Add(new JazzPattern(true));
         patternList.Add(new WeavePattern(true));
         patternList.Add(new BroadwayPattern(true));
         patternList.Add(new MatrixPattern(true));
         patternList.Add(new SixtyDegreeStripePattern(true));
         patternList.Add(new TrianglePattern(true));
         patternList.Add(new TreadPerfPattern(true));
         patternList.Add(new ThirdStackPattern(true));
         patternList.Add(new PhoenixMorsePattern(true));
         patternList.Add(new MetrixPattern(true));
         return patternList;
      }
   }
}
