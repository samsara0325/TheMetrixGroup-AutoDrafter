using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetrixGroupPlugins.MessageBoxes
{
    /**
     * Holds the messages that gets displayed in the system
     * */
    public static class Messages
    {
        //Method displays the DFX required or not message box and returns a boolean based on users 
        //selection
        public static Boolean showDXFRequiredMessage()
        {
            DialogResult result = MessageBox.Show("Do you want to generate DXF files?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Displays Bullzip not installed message to user
        public static void showBullzipNotInstalled()
        {
            MessageBox.Show("Bullzip PDF printer is not installed. PDF will not be generated.");
        }

        //Method displays the Dotfont text required or not message box and returns a boolean based on users 
        //selection
        public static Boolean showDotfontRequiredMessage()
        {
            DialogResult result = MessageBox.Show("Do you want to generate the Dotfont Text?",
               "Dotfont Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

      //Method displays the #ref common error message found when loading panel csv file
        public static void showPanelCSVMapError(String error)
        {
            MessageBox.Show("This error usually occurs where there is '#REF!' values in the RhinoPanelData sheet (Excel), \n" +
               "Please makesure there are no referencing errors or links broken between RhinoPanelData sheet and Drafting Sheet", "Error!",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("Could not match Excel Columns to Code Variables!", "Error!",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("Error thrown from System :" + error);
            MessageBox.Show("Aborting Panel Drafting", "Aborting!",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

      //Message informs the user, the system is generating the final
      public static void showGeneratingPDF()
      {       
         MessageBoxEx.Show("Hang in there " + Environment.UserName+ " The system is generating the final PDF\n\n Wait for this message to automatically Close!", 3000);        
      }

    //Message informs the user, the system is generating the final Excel sheet
     public static void showGeneratingExcel()
      {
        MessageBoxEx.Show("Hang in there " + Environment.UserName + "\nThe system is generating the Excel Sheet with the dimensions \n\nWait for this message to automatically Close!", 5000);
      }

        //Message informs the user, the system is generating the final Excel sheet
        public static void excelOperationComplete()
        {
            MessageBoxEx.Show("The system has generated the Excel Sheet Successfully!", 3000);
        }

        //Method displays all the panels with unclosed curves (used in the mass dxf exporter)
        public static void showUnclosedCurves(List<String> curves)
      {
         String unclosedCurves = "";
         List<String> Result1 = new HashSet<String>(curves).ToList(); 
         foreach (String panel in Result1)
         {
            unclosedCurves = unclosedCurves + "\n Panel Id : " + panel;
         }
         
         MessageBox.Show("Unclosed curves were found in the below panels. Please close the curves and try again! \n"+ unclosedCurves, "Error!",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
         MessageBox.Show("Aborting Panel Drafting", "Aborting!",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      //Method displays a message if the mass dxf exporter function throws any error 
      public static void showMassdxfError(Exception e)
      {
         MessageBox.Show("An en error was thrown when trying to export DXF files! \nMakesure you have PANEL PERIMETER AND LABELS Layer seperated!\n" + e, "Error!",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
         //MessageBox.Show("Details from the system : " + e.GetBaseException());
         MessageBox.Show("Aborting DXF Exporting", "Aborting!",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      //Method displays a message if the mass dxf exporter function throws any error 
      public static void showMassdxfComplete()
      {
         MessageBox.Show("Mass DXF Exporter has successfully finished exporting", "Success",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      //Method display the error message if the pattern is not found when trying to Nest print
      public static void showFileNotFoundForPattern(String pattern)
      {
         MessageBox.Show("No Matching PDF Tool Hit Found in the Directory for the pattern : "+pattern, "Toolhit Not Found",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
         MessageBox.Show("Aborting Print Nesting","Aborting!",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      //Method will display a message box requesting the user whether compression is required for the final PDF
      public static Boolean showCompressionRequired()
      {
         DialogResult result = MessageBox.Show("Do you want to compress the final PDF?", "Confirmation", MessageBoxButtons.YesNo);
         if (result == DialogResult.Yes)
         {
            return true;
         }
         else
         {
            return false;
         }
      }


      //Method will display a message box requesting the user whether compression is required for the final PDF
      public static Boolean showToolHitRequired()
      {
         DialogResult result = MessageBox.Show("Do you want the Tool Hit to be Displayed on the PDF?", "Tool Hit Confirmation", MessageBoxButtons.YesNo);
         if (result == DialogResult.Yes)
         {
            return true;
         }
         else
         {
            return false;
         }
      }
      /// <summary>
      /// Display a message box asking users whether they want to print the PDF
      /// </summary>
      /// <returns></returns>
        public static Boolean showPDFRequired()
      {
         DialogResult result = MessageBox.Show("Do you want to print PDF file?", "PDF Confirmation", MessageBoxButtons.YesNo);
         if (result == DialogResult.Yes)
         {
            return true;
         }
         else
         {
            return false;
         }
      }

        //Method shows default perimeter layer names were not found (CAd file merger)
        public static void ShowDefaultPerimeterLayerNotFound()
        {
            DialogResult result = MessageBox.Show("Perimeter for the panel could not be found in the default layers \nPlease add customer layer name which has the perimeter in the next prompt", "Perimeter Not found in default layers", MessageBoxButtons.OK);
        }


        //Method will display a message box requesting the user to delete already generate excel sheet
        public static Boolean showDeleteExcelFileManually()
        {
            DialogResult result = MessageBox.Show("Error!\nCould not override existing excel file with panel dimensions\n" +
                "Please delete excel sheet with panel dimensions and click OK to retry", "Could not generate excel sheet", MessageBoxButtons.OKCancel);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Method will display to ask if Fixinghole data is manipulated manually
        public static bool showIsFixingHoleDataChanged()
        {
            DialogResult result = MessageBox.Show("Fixing holes have been detected in this project.\nHave you manually edited fixing holes quantity or fixing holes centres in the drafting sheet? ", "Fixing Holes Detected", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
