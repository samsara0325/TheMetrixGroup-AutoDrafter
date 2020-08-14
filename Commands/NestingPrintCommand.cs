using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Bullzip.PdfWriter;
using System.IO;
using System.Windows.Forms;
using MetrixGroupPlugins.MessageBoxes;
using Rhino.DocObjects;
using iTextSharp.text;

namespace MetrixGroupPlugins.Commands
{
   //Class implements the functionality to create a pdf with watermark

   [CommandStyle(Style.ScriptRunner)]    
   [System.Runtime.InteropServices.Guid("12425CF7-08D0-457F-820B-817751C68F69")]

   public class NestingPrintCommand : Command
   {
      
      //Class variables 
      String tempPdfPath = null; //location where the temporary pdf will be saved 
      String oriPdfPath = null; //location where the original pdf will be saved 
      String toolHitLocation = "W:\\Orders Current\\nOOL PDF's\\ROUND HOLE CLUSTERS\\RH60 CLUSTERS";
      String toolHitPdfLocation = null;
      public NestingPrintCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static NestingPrintCommand Instance
      {
         get; private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "NestingPrintCommand"; }
      }


      ///<summary>The only instance of this command.</summary>
      ///<param name="doc" RhinoDoc></param>
      ///<param name="mode" Run mode></param>
      ///<returns>returns sucess if doc is successfully created </returns>
      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         RhinoApp.RunScript("Save", true); //save file before printing
         String patternFound;
         patternFound = extractToolHit(doc); //get the pattern 

         if (patternFound.Contains("Round Hole 60")) //check if pattern is a round hole 60
         {
            //set the location to round hole 60 pattern
            toolHitLocation = "W:\\Orders Current\\nOOL PDF's\\ROUND HOLE CLUSTERS\\RH60 CLUSTERS";
            //Trim the pattern, so that it only reads the tool hit size
            patternFound = patternFound.Split(new string[] { "Round Hole " }, StringSplitOptions.None)[1];
            patternFound = "RH" + patternFound;
         }

         if (patternFound.Contains("Round Hole 90")) //check if pattern is a round hole 90
         {
            //set the location to round hole 60 pattern
            toolHitLocation = "W:\\Orders Current\\nOOL PDF's\\ROUND HOLE CLUSTERS\\RH90 CLUSTERS";
            //Trim the pattern, so that it only reads the tool hit size
            patternFound = patternFound.Split(new string[] { "Round Hole " }, StringSplitOptions.None)[1];
            patternFound = "RH" + patternFound;
         }

         if (patternFound.Contains("Round Hole 45")) //check if pattern is a round hole 45
         {
            //set the location to round hole 60 pattern
            toolHitLocation = "W:\\Orders Current\\nOOL PDF's\\ROUND HOLE CLUSTERS\\RH45 CLUSTERS";
            //Trim the pattern, so that it only reads the tool hit size
            patternFound = patternFound.Split(new string[] { "Round Hole " }, StringSplitOptions.None)[1];
            patternFound = "RH" + patternFound;
         }

         toolHitPdfLocation = findToolHitPdf(patternFound, toolHitLocation);
         if (toolHitPdfLocation.Equals("Pattern not found"))
         {
            MessageBoxes.Messages.showFileNotFoundForPattern(patternFound);
            return Result.Failure;
         }


         string fileName = Path.GetFileNameWithoutExtension(doc.Name);
         String[] nameSplit = fileName.Split(new string[] { "_" }, StringSplitOptions.None);

         //Add Nesting part to the file Name
         fileName = nameSplit[0]+"_"+ nameSplit[1]+"_Nesting_" + nameSplit[2] + nameSplit[3] +"_"+ nameSplit[4];

         System.Windows.Forms.PrintDialog dlg = new PrintDialog();

         if (dlg.PrinterSettings.IsValid == false)
         {
            Messages.showBullzipNotInstalled();
         }
         else
         {
            // If Page Views is 0 
            if (doc.Views.GetPageViews().Count() != 0)
            {
               try
               {
                  tempPdfPath = Path.GetDirectoryName(doc.Path) + @"\" + "temp" + ".pdf";  //create a temporary pdf with panels
                  oriPdfPath = Path.GetDirectoryName(doc.Path) + @"\" + fileName + ".pdf";
                  //set the tool hit layer and cluster sample layer to invisible
                  RhinoUtilities.setLayerVisibility("Tool Hit", false);
                  RhinoUtilities.setLayerVisibility("CLUSTER SAMPLE", false);
                  PdfSettings pdfSettings = new PdfSettings();
                  pdfSettings.SetValue("Output", tempPdfPath);
                  pdfSettings.SetValue("ShowPDF", "no");
                  pdfSettings.SetValue("ShowSettings", "never");
                  pdfSettings.SetValue("ShowSaveAS", "never");
                  pdfSettings.SetValue("ShowProgress", "yes");
                  pdfSettings.SetValue("ShowProgressFinished", "no");
                  pdfSettings.SetValue("ConfirmOverwrite", "no");
                  pdfSettings.WriteSettings(PdfSettingsFileType.RunOnce);

                  string command = string.Format("-_Print _Setup _Destination _Printer \"Bullzip PDF Printer\" _PageSize 297.000 210.00 _OutputType=Vector _Enter _View _AllLayout _Enter _Enter _Go");
                  RhinoApp.RunScript(command, true);

                  string[] pdfs = new String[2]; //create a string array to hold the locations of the pdf with panel and agreement form pdf.
                  pdfs[0] = tempPdfPath;
                  pdfs[1] = toolHitPdfLocation;

                  RhinoUtilities.combinePDF(oriPdfPath, pdfs,0,1, "Drawings Second"); //pass the array and the target location to save the final pdf

                  
               }
               catch (Exception ex)
               {
                  System.Windows.Forms.MessageBox.Show("Error printing PDF document." + ex.Message);
               }
            }
         }
         return Result.Success;
      }
      //Method extracts the tool hit (pattern name) drawn in the panels
      public static String extractToolHit(RhinoDoc doc)
      {
         RhinoObject[] perfObjects =  doc.Objects.FindByLayer("PERF ORIENTATION");
         String foundString = null ;
         foreach (RhinoObject obj in perfObjects) //loop through the selected objects
         {
            try //try to convert object to Annotation
            {
               foundString = ((AnnotationObjectBase)obj).DisplayText;
               if (foundString.Contains("Pattern")) //if the text contains Pattern break from loop
               {
                  break;
               }
            }catch(Exception e)
            { //if failed, catch exception and continue
               continue;
            }
          }
         if (foundString != null)
         {
            return trimString(foundString);
         }
         return null;
      }

      //Method trims the text found in the object and returns a much clearer string
      public static String trimString(String stringFound)
      {
         String splitString;
         return splitString = stringFound.Split(new string[] { "Pattern:" }, StringSplitOptions.None)[1]; //gets the split string
         
      }

      //Finds the pdf with the matching tool hit (pattern name)
      private static String findToolHitPdf(string patternName, string location)
      {
         //Read all file Names
         string[] files = Directory.GetFiles(location); //read all the files in the directory

         foreach (string file in files) //loop through all the files found
         {
            if(Path.GetFileName(file).Equals(patternName+(".pdf"))) //check if the tool hit name equals the pattern name
            {
               return Path.GetFullPath(file); //return the full path of the pdf file
            }
         }
         return "Pattern not found";
      }



   }
}
