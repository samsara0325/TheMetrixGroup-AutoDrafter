using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;

namespace MetrixGroupPlugins.Utilities
{
   public class MetrixUtilities
   {
      /**
       * Method used to split a string using the provide delimeter
       * */
      public static String[] splitString(String originalString, String delimeter)
      {
         return originalString.Split(new[] { delimeter }, StringSplitOptions.None);
      }

      /*
       * Methor joins curves on a layer provided by the layer number
       **/
      public static void joinCurves(int layerNumber)
      {
         RhinoApp.RunScript("SelLayerNumber "+ layerNumber, true);
         RhinoApp.RunScript("_Join ", true);
      }
      /*
       * Delete dupilicate objects in layers
       * */
      public static void deleteDuplicates()
      {
         RhinoApp.RunScript("SelDup ", true);
         RhinoApp.RunScript("Delete ", true);
      }

      /*
       * delete the entire layer matching the provided layer number
       * */
      public static void deleteLayer(int layerNumber)
      {
         RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
         RhinoApp.RunScript("Delete ", true);
      }

      //Creates the metrix borders dimension style
      public static void createMetrixBordersDimension()
      {
         int dimStyleIndex = 0;
         RhinoDoc doc = RhinoDoc.ActiveDoc;

         DimensionStyle dimStyle = doc.DimStyles.Find("Metrix Real Borders", true);

         if (dimStyle == null)
         {
            dimStyleIndex = doc.DimStyles.Add("Metrix Real Borders");
            dimStyle = doc.DimStyles.Find("Metrix Real Borders", true);
         }
         else
         {
            dimStyleIndex = dimStyle.Index;
         }
         dimStyle.TextHeight = 20;
         dimStyle.TextGap = 5;
         dimStyle.ExtensionLineExtension = 25;
         dimStyle.ExtensionLineOffset = 25;
         dimStyle.LengthResolution = 0;
         dimStyle.AngleResolution = 0;

         dimStyle.ArrowLength = 20;
         dimStyle.LeaderArrowLength = 20;
         dimStyle.FitText = DimensionStyle.TextFit.TextInside;
         dimStyle.FixedExtensionLength = 25;
         dimStyle.FixedExtensionOn = true;
         dimStyle.LeaderTextVerticalAlignment = TextVerticalAlignment.Middle;
         dimStyle.LeaderContentAngleType = DimensionStyle.LeaderContentAngleStyle.Aligned;
         dimStyle.DimRadialTextLocation = DimensionStyle.TextLocation.AboveDimLine;
            doc.DimStyles.Modify(dimStyle, dimStyleIndex, false);

         doc.DimStyles.SetCurrent(dimStyleIndex, false);
      }

      //Creates the Metrix real dimension style
      public static DimensionStyle createMetrixRealDimension()
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         DimensionStyle dimStyle = doc.DimStyles.FindName("Metrix Real");
         int dimStyleIndex = 0;

         if (dimStyle == null)
         {
            dimStyleIndex = doc.DimStyles.Add("Metrix Real");
            dimStyle = doc.DimStyles.FindName("Metrix Real");
         }
         else
         {
            dimStyleIndex = dimStyle.Index;
         }

         dimStyle.TextHeight = 20;
         dimStyle.TextGap = 5;
         dimStyle.ExtensionLineExtension = 25;
         dimStyle.ExtensionLineOffset = 25;
         dimStyle.LengthResolution = 1;
         dimStyle.AngleResolution = 1;
         dimStyle.ArrowLength = 20;
         dimStyle.LeaderArrowLength = 20;
         dimStyle.FitText = DimensionStyle.TextFit.TextInside;
        dimStyle.LeaderContentAngleType = DimensionStyle.LeaderContentAngleStyle.Aligned;
        dimStyle.FixedExtensionLength = 25;
        dimStyle.FixedExtensionOn = true;
        dimStyle.DimRadialTextLocation = DimensionStyle.TextLocation.AboveDimLine;
        dimStyle.LeaderTextVerticalAlignment = TextVerticalAlignment.Middle;
            doc.DimStyles.Modify(dimStyle, dimStyleIndex, false);

         doc.DimStyles.SetCurrent(dimStyleIndex, false);
         return dimStyle;
      }


        public static bool IsLayerFound(string LayerName)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            if (doc.Layers.Find(LayerName, true)!=-1){
                return true;
            }
            else
            {
                return false;
            }
        }
   }
}
