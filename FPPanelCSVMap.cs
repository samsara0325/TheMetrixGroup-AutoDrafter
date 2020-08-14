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

    public sealed class FPPanelCSVClassMap : CsvClassMap<FoldedPerforationPanel>
    {
        public FPPanelCSVClassMap(bool isOld, bool hasSummaryPage)
        {
            Map(m => m.PartName).Name("Part");
            Map(m => m.KFactor).Name("k - factor");
            Map(m => m.X).Name("x");
            Map(m => m.Y).Name("y");
            Map(m => m.Row).Name("Row");
            Map(m => m.Column).Name("Column");
            Map(m => m.DrawPerf).Name("DrawPerf");
            Map(m => m.CornerRelief).Name("Corner Relief Type");
            Map(m => m.CornerReliefSize).Name("Corner Relief Size");
            Map(m => m.TopFoldType).Name("Top Fold Type");
            Map(m => m.BottomFoldType).Name("Bottom Fold Type");
            Map(m => m.LeftFoldType).Name("Left Fold Type");
            Map(m => m.RightFoldType).Name("Right Fold Type");
            //Map(m => m.TopLeftSetback).Name("Top-Left Setback Location");
            //Map(m => m.BottomleftSetback).Name("Bottom-Left Setback Location");
            //Map(m => m.TopRightSetback).Name("Top-Right Setback Location");
            //Map(m => m.BottomRightSetback).Name("Bottom-Right Setback Location");
            Map(m => m.FixingHoles).Name("Fixing Holes Required");
            Map(m => m.HoleDiameter).Name("Fixing Hole Diameter");
            Map(m => m.DotFontLabel).Name("DotFont");

            Map(m => m.LeftBorder).Name("Left Border");
            Map(m => m.RightBorder).Name("Right Border");
            Map(m => m.BottomBorder).Name("Bottom Border");
            Map(m => m.TopBorder).Name("Top Border");


            Map(m => m.TopFixingHoles).Name("Top Fixing Holes Required");
            Map(m => m.TopFixingHoleDistance).Name("Top Fixing Hole Distance");
            Map(m => m.TopFirstFoldDirection).Name("Top First Fold Direction");
            Map(m => m.TopFirstFoldWidth).Name("Top First Fold Width");
            Map(m => m.TopFirstFoldSetbackLeft).Name("Top First Fold Setback LHS");
            Map(m => m.TopFirstFoldSetbackRight).Name("Top First Fold Setback RHS");
            Map(m => m.TopSecondFoldWidth).Name("Top Second Fold Width");
            Map(m => m.TopSecondFoldSetbackLeft).Name("Top Second Fold Setback LHS");
            Map(m => m.TopSecondFoldSetbackRight).Name("Top Second Fold Setback RHS");
            Map(m => m.TopFixingHoleQuantity).Name("Top Fixing Hole Quantity");
            //Map(m => m.DistanceTopCenter).Name("Distance from Top Edge to Center");
            Map(m => m.TopHoleSetbackLeft).Name("Top Holes LHS Setback");
            Map(m => m.TopHoleSetbackRight).Name("Top Holes RHS Setback");
            Map(m => m.TopHoleSetbackTop).Name("Top Holes THS Setback");
            Map(m => m.TopFoldRadius).Name("Top Fold Radius");

            Map(m => m.BottomFixingHoleDistance).Name("Bottom Fixing Hole Distance");
            Map(m => m.BottomFirstFoldDirection).Name("Bottom First Fold Direction");
            Map(m => m.BottomFirstFoldWidth).Name("Bottom First Fold Width");
            Map(m => m.BottomFirstFoldSetbackLeft).Name("Bottom First Fold Setback LHS");
            Map(m => m.BottomFirstFoldSetbackRight).Name("Bottom First Fold Setback RHS");
            Map(m => m.BottomSecondFoldWidth).Name("Bottom Second Fold Width");
            Map(m => m.BottomSecondFoldSetbackLeft).Name("Bottom Second Fold Setback LHS");
            Map(m => m.BottomSecondFoldSetbackRight).Name("Bottom Second Fold Setback RHS");
            Map(m => m.BottomFixingHoles).Name("Bottom Fixing Holes Required");
            Map(m => m.BottomFixingHoleQuantity).Name("Bottom Fixing Hole Quantity");
            // Map(m => m.DistanceBottomCenter).Name("Distance from Bottom Edge to Center");
            Map(m => m.BottomHoleSetbackLeft).Name("Bottom Holes LHS Setback");
            Map(m => m.BottomHoleSetbackRight).Name("Bottom Holes RHS Setback");
            Map(m => m.BottomHoleSetbackBottom).Name("Bottom Holes BHS Setback");
            Map(m => m.BottomFoldRadius).Name("Bottom Fold Radius");

            Map(m => m.LeftFixingHoleDistance).Name("Left Fixing Holes Distance");
            Map(m => m.LeftFirstFoldDirection).Name("Left First Fold Direction");
            Map(m => m.LeftFirstFoldWidth).Name("Left First Fold Width");
            Map(m => m.LeftFirstFoldSetbackTop).Name("Left First Fold Setback THS");
            Map(m => m.LeftFirstFoldSetbackBottom).Name("Left First Fold Setback BHS");
            Map(m => m.LeftSecondFoldWidth).Name("Left Second Fold Width");
            Map(m => m.LeftSecondFoldSetbackTop).Name("Left Second Fold Setback THS");
            Map(m => m.LeftSecondFoldSetbackBottom).Name("Left Second Fold Setback BHS");
            Map(m => m.LeftFixingHoles).Name("Left Fixing Holes Required");
            Map(m => m.LeftFixingHoleQuantity).Name("Left Fixing Hole Quantity");
            Map(m => m.LeftHoleSetbackTop).Name("Left Holes THS Setback");
            Map(m => m.LeftHoleSetbackBottom).Name("Left Holes BHS Setback");
            Map(m => m.LeftHoleSetbackLeft).Name("Left Holes LHS Setback");
            Map(m => m.LeftFoldRadius).Name("Left Fold Radius");


            Map(m => m.RightFixingHoleDistance).Name("Right Fixing Holes Distance");
            Map(m => m.RightFirstFoldDirection).Name("Right First Fold Direction");
            Map(m => m.RightFirstFoldWidth).Name("Right First Fold Width");
            Map(m => m.RightFirstFoldSetbackTop).Name("Right First Fold Setback THS");
            Map(m => m.RightFirstFoldSetbackBottom).Name("Right First Fold Setback BHS");
            Map(m => m.RightSecondFoldWidth).Name("Right Second Fold Width");
            Map(m => m.RightSecondFoldSetbackTop).Name("Right Second Fold Setback THS");
            Map(m => m.RightSecondFoldSetbackBottom).Name("Right Second Fold Setback BHS");
            Map(m => m.RightFixingHoles).Name("Right Fixing Holes Required");
            Map(m => m.RightFixingHoleQuantity).Name("Right Fixing Hole Quantity");
            //     Map(m => m.DistanceRightCenter).Name("Distance from Right Edge to Center");
            Map(m => m.RightHoleSetbackTop).Name("Right Holes THS Setback");
            Map(m => m.RightHoleSetbackBottom).Name("Right Holes BHS Setback");
            Map(m => m.RightHoleSetbackRight).Name("Right Holes RHS Setback");
            Map(m => m.RightFoldRadius).Name("Right Fold Radius");

            Map(m => m.PatternName).Name("Pattern");
            Map(m => m.SheetThickness).Name("Sheet Thickness");
            Map(m => m.DXFFilesRequired).Name("DXF Files Required");
            Map(m => m.PanelQuantity).Name("PanelQuantity");
            Map(m => m.NumberOfFolds).Name("Folds");
            Map(m => m.SideRequired).Name("Required Side");
            Map(m => m.PerfText).Name("PerforateText");

            Map(m => m.DistanceProvided).Name("MaximumDistance");

            Map(m => m.TopFold).Name("Top First Fold Required");
            Map(m => m.BottomFold).Name("Bottom First Fold Required");
            Map(m => m.LeftFold).Name("Left First Fold Required");
            Map(m => m.RightFold).Name("Right First Fold Required");

            Map(m => m.OnFolds).Name("Draw Fixing Holes on Folds ");
            Map(m => m.PatternOpenArea).Name("Pattern OpenArea");
            Map(m => m.DotFontLabellerSide).Name("DotFont Labeller");

            Map(m => m.DistanceProvidedTop).Name("DistanceProvidedTop");
            Map(m => m.DistanceProvidedBottom).Name("DistanceProvidedBottom");
            Map(m => m.DistanceProvidedLeft).Name("DistanceProvidedLeft");
            Map(m => m.DistanceProvidedRight).Name("DistanceProvidedRight");

            Map(m => m.PanelType).Name("Panel Type");

            Map(m => m.TopSecondFoldDirection).Name("Top Second Fold Direction");
            Map(m => m.BottomSecondFoldDirection).Name("Bottom Second Fold Direction");
            Map(m => m.LeftSecondFoldDirection).Name("Left Second Fold Direction");
            Map(m => m.RightSecondFoldDirection).Name("Right Second Fold Direction");

            Map(m => m.TopSecondFoldRequired).Name("Top Second Fold Required");
            Map(m => m.BottomSecondFoldRequired).Name("Bottom Second Fold Required");
            Map(m => m.LeftSecondFoldRequired).Name("Left Second Fold Required");
            Map(m => m.RightSecondFoldRequired).Name("Right Second Fold Required");

            Map(m => m.TopFoldRightType).Name("Top Second Fold Right Type");
            Map(m => m.TopFoldLeftType).Name("Top Second Fold Left Type");
            Map(m => m.BottomFoldRightType).Name("Bottom Second Fold Right Type");
            Map(m => m.BottomFoldLeftType).Name("Bottom Second Fold Left Type");
            Map(m => m.LeftFoldTopType).Name("Left Second Fold Top Type");
            Map(m => m.LeftFoldBottomType).Name("Left Second Fold Bottom Type");
            Map(m => m.RightFoldTopType).Name("Right Second Fold Top Type");
            Map(m => m.RightFoldBottomType).Name("Right Second Fold Bottom Type");

            Map(m => m.DoubleFoldsRequired).Name("Double Folds Required");
            Map(m => m.FixingHoleCentres).Name("Fixing Hole Centres");
            Map(m => m.FoldWithFixingHoles).Name("FoldWithFixingHoles");
            



            //If it is not the old draft sheet, try to map the columns
            if (!isOld)
            {
                    Map(m => m.rowSpacing).Name("RowSpacing");
                    Map(m => m.colSpacing).Name("ColSpacing");
                    Map(m => m.labelHeight).Name("LabelHeight");
                    Map(m => m.project).Name("Project");
                    Map(m => m.customerName).Name("Customer");
                    Map(m => m.jobNo).Name("JobNo");
                    Map(m => m.material).Name("Material");
                    Map(m => m.coating).Name("Coating");
                    Map(m => m.revision).Name("Revision");
                    Map(m => m.patternDirection).Name("PatternDirection");
                    Map(m => m.colour).Name("Colour");
                    Map(m => m.drafterName).Name("Drafter");
                    Map(m => m.FirstRevisionDate).Name("FirstRevisionDate");
                    Map(m => m.RevisionReason).Name("RevisionReason");
                    Map(m => m.TotalPanelQuantity).Name("Total Quantity");
                    Map(m => m.PerfOption).Name("PerfOption");
                    Map(m => m.PrintPDF).Name("PrintPDF");
                    Map(m => m.CenterType).Name("CenterType");
                    

                //Other
                Map(m => m.drawPerfOnFirstPanel).Name("DrawPerfonFirstPanel");               
            }

            if (hasSummaryPage)
            {
                //For Summary Page
                Map(m => m.CustomerOrderNo).Name("CustomerOrderNo");
                Map(m => m.MetrixPartNo).Name("MetrixPartNo");
                Map(m => m.MetrixSalesNo).Name("MetrixSalesNo");
                Map(m => m.TotalPanelSQM).Name("TotalPanelSQM");    
               // Map(m => m.TotalPanelSQM).Name("TotalPanelSQM");
                //Map(m => m.TotalPrice).Name("TotalPrice");
            }

        }
    }
}
