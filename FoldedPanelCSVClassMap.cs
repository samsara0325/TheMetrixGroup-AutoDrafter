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
   public sealed class FoldedPanelCSVClassMap : CsvClassMap<FoldedPerforationPanel>
   {
      public FoldedPanelCSVClassMap() 
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
            Map(m => m.TopLeftSetback).Name("Top-Left Setback Location");
            Map(m => m.BottomleftSetback).Name("Bottom-Left Setback Location");
            Map(m => m.TopRightSetback).Name("Top-Right Setback Location");
            Map(m => m.BottomRightSetback).Name("Bottom-Right Setback Location");
            Map(m => m.FixingHoles).Name("Fixing Holes");
            Map(m => m.HoleDiameter).Name("Hole Diameter");
            Map(m => m.DotFontLabel).Name("Dot Font Labels");

            Map(m => m.LeftBorder).Name("LeftBorder");
            Map(m => m.RightBorder).Name("RightBorder");
            Map(m => m.BottomBorder).Name("BottomBorder");
            Map(m => m.TopBorder).Name("TopBorder");

           // Map(m => m.TopBorderDistance).Name("Top Fixing Hole Distance");
            Map(m => m.TopFirstFoldDirection).Name("Top First Fold Direction");
            Map(m => m.TopFirstFoldWidth).Name("Top First Fold Width");
            Map(m => m.TopFirstFoldSetbackLeft).Name("Top First Fold Setback LHS");
            Map(m => m.TopFirstFoldSetbackRight).Name("Top First Fold Setback RHS");
            Map(m => m.TopSecondFoldWidth).Name("Top Second Fold Width");
            Map(m => m.TopSecondFoldSetbackLeft).Name("Top Second Fold Setback LHS");
            Map(m => m.TopSecondFoldSetbackRight).Name("Top Second Fold Setback RHS");
            Map(m => m.TopFixingHoles).Name("Top Fixing Holes");
            Map(m => m.TopFixingHoleQuantity).Name("Top Fixing Hole Quantity");
            Map(m => m.DistanceTopCenter).Name("Distance from Top Edge to Center");
            Map(m => m.TopHoleSetbackLeft).Name("Top Holes LHS Setback");
            Map(m => m.TopHoleSetbackRight).Name("Top Holes RHS Setback");
            Map(m => m.TopFoldRadius).Name("Top Fold Radius");

            Map(m => m.BottomFixingHoleDistance).Name("Bottom Fixing Hole Distance");
            Map(m => m.BottomFirstFoldDirection).Name("Bottom First Fold Direction");
            Map(m => m.BottomFirstFoldWidth).Name("Bottom First Fold Width");
            Map(m => m.BottomFirstFoldSetbackLeft).Name("Bottom First Fold Setback LHS");
            Map(m => m.BottomFirstFoldSetbackRight).Name("Bottom First Fold Setback RHS");
            Map(m => m.BottomSecondFoldWidth).Name("Bottom Second Fold Width");
            Map(m => m.BottomSecondFoldSetbackLeft).Name("Bottom Second Fold Setback LHS");
            Map(m => m.BottomSecondFoldSetbackRight).Name("Bottom Second Fold Setback RHS");
            Map(m => m.BottomFixingHoles).Name("Bottom Fixing Holes");
            Map(m => m.BottomFixingHoleQuantity).Name("Bottom Fixing Hole Quantity");
            Map(m => m.DistanceBottomCenter).Name("Distance from Bottom Edge to Center");
            Map(m => m.BottomHoleSetbackLeft).Name("Bottom Holes LHS Setback");
            Map(m => m.BottomHoleSetbackRight).Name("Bottom Holes RHS Setback");
            Map(m => m.BottomFoldRadius).Name("Bottom Fold Radius");

            Map(m => m.LeftFixingHoleDistance).Name("Left Fixing Hole Distance");
            Map(m => m.LeftFirstFoldDirection).Name("Left First Fold Direction");
            Map(m => m.LeftFirstFoldWidth).Name("Left First Fold Width");
            Map(m => m.LeftFirstFoldSetbackTop).Name("Left First Fold Setback THS");
            Map(m => m.LeftFirstFoldSetbackBottom).Name("Left First Fold Setback BHS");
            Map(m => m.LeftSecondFoldWidth).Name("Left Second Fold Width");
            Map(m => m.LeftSecondFoldSetbackTop).Name("Left Second Fold Setback THS");
            Map(m => m.LeftSecondFoldSetbackBottom).Name("Left Second Fold Setback BHS");
            Map(m => m.LeftFixingHoles).Name("Left Fixing Holes");
            Map(m => m.LeftFixingHoleQuantity).Name("Left Fixing Hole Quantity");
            Map(m => m.DistanceLeftCenter).Name("Distance from Left Edge to Center");
            Map(m => m.LeftHoleSetbackTop).Name("Left Holes THS Setback");
            Map(m => m.LeftHoleSetbackBottom).Name("Left Holes BHS Setback");
            Map(m => m.LeftFoldRadius).Name("Left Fold Radius");

            Map(m => m.RightFixingHoleDistance).Name("Right Fixing Hole Distance");
            Map(m => m.RightFirstFoldDirection).Name("Right First Fold Direction");
            Map(m => m.RightFirstFoldWidth).Name("Right First Fold Width");
            Map(m => m.RightFirstFoldSetbackTop).Name("Right First Fold Setback THS");
            Map(m => m.RightFirstFoldSetbackBottom).Name("Right First Fold Setback BHS");
            Map(m => m.RightSecondFoldWidth).Name("Right Second Fold Width");
            Map(m => m.RightSecondFoldSetbackTop).Name("Right Second Fold Setback THS");
            Map(m => m.RightSecondFoldSetbackBottom).Name("Right Second Fold Setback BHS");
            Map(m => m.RightFixingHoles).Name("Right Fixing Holes");
            Map(m => m.RightFixingHoleQuantity).Name("Right Fixing Hole Quantity");
            Map(m => m.DistanceRightCenter).Name("Distance from Right Edge to Center");
            Map(m => m.RightHoleSetbackTop).Name("Right Holes THS Setback");
            Map(m => m.RightHoleSetbackBottom).Name("Right Holes BHS Setback");
            Map(m => m.RightFoldRadius).Name("Right Fold Radius");

            Map(m => m.PatternName).Name("Pattern");
            Map(m => m.SheetThickness).Name("Sheet Thickness");
         
      }
   }
}
