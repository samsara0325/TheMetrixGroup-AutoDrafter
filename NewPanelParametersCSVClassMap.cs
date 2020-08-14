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
    public sealed class NewPanelParametersCSVClassMap : CsvClassMap<PanelParameters>
    {
        public NewPanelParametersCSVClassMap()
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
            Map(m => m.PatternDirection).Name("PatternDirection");
            Map(m => m.Colour).Name("Colour");
            Map(m => m.DrafterName).Name("Drafter");
            Map(m => m.FirstRevisionDate).Name("FirstRevisionDate");
            Map(m => m.RevisionReason).Name("RevisionReason");
      }
    }
}
