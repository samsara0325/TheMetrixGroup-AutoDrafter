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
   public sealed class PanelParametersCSVClassMap : CsvClassMap<PanelParameters>
   {
      public PanelParametersCSVClassMap()
      {
         Map(m => m.RowSpacing).Name("RowSpacing");
         Map(m => m.ColSpacing).Name("ColSpacing");
         Map(m => m.LabelHeight).Name("LabelHeight");
         Map(m => m.Project).Name("Project");
         Map(m => m.CustomerName).Name("Customer");
         Map(m => m.JobNo).Name("JobNo");
         Map(m => m.Material).Name("Material");
         Map(m => m.Coating).Name("Coating");
         Map(m => m.Revision).Name("Revision");
         Map(m => m.DotFont).Name("DotFont");
         Map(m => m.PatternDirection).Name("PatternDirection");
      }
   }
}
