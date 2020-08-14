using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Collections;
using CsvHelper.Configuration;

namespace MetrixGroupPlugins
{
   public sealed class PanelCSVClassMap : CsvClassMap<PerforationPanel>
   {
      public PanelCSVClassMap()
      {
         Map(m => m.PartName).Name("Part");
         Map(m => m.X).Name("x");
         Map(m => m.Y).Name("y");
         Map(m => m.Row).Name("Row");
         Map(m => m.Column).Name("Column");
         Map(m => m.LeftBorder).Name("LeftBorder");
         Map(m => m.RightBorder).Name("RightBorder");
         Map(m => m.BottomBorder).Name("BottomBorder");
         Map(m => m.TopBorder).Name("TopBorder");
         Map(m => m.PatternName).Name("Pattern");
         Map(m => m.DrawPerf).Name("DrawPerf");
      }
   }
}
