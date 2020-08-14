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

namespace MetrixGroupPlugins.Commands
{
   //Class implements the functionality to create a pdf with watermark

   [CommandStyle(Style.ScriptRunner)]
   [System.Runtime.InteropServices.Guid("43fcf448-722f-4783-a36e-2dc73a24909b")]
   public class CreatePDFWithWaterMark : Command
   {
      //Class variables 
      String tempPdfPath = null; //location where the temporary pdf will be saved 
      String oriPdfPath = null; //location where the original pdf will be saved 
      String agreementLocation = "Z:\\Automation\\RequiredDraftingDocuments\\AgreementMetrix.pdf";
      public CreatePDFWithWaterMark()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static CreatePDFWithWaterMark Instance
      {
         get; private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "CreatePDFWithWaterMark"; }
      }


      ///<summary>The only instance of this command.</summary>
      ///<param name="doc" RhinoDoc></param>
      ///<param name="mode" Run mode></param>
      ///<returns>returns sucess if doc is successfully created </returns>
      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
            return createPDF(doc, true);
      }

      public Result createPDF(RhinoDoc doc, bool askToolHit)
        {
            RhinoApp.RunScript("Save", true); //save file before printing
            string fileName = Path.GetFileNameWithoutExtension(doc.Name);
            System.Windows.Forms.PrintDialog dlg = new PrintDialog();

            //Prompt the user whether the tool hit is required on the final PDF or not
            if (askToolHit)
            {
                if (!MessageBoxes.Messages.showToolHitRequired())
                {
                    //If the tool hit is not required, make the tool hit layer invisible
                    RhinoUtilities.SetActiveLayer("LABELS", System.Drawing.Color.Red);
                    RhinoUtilities.setLayerVisibility("Tool Hit", false);
                }
                else
                {
                    RhinoUtilities.SetActiveLayer("LABELS", System.Drawing.Color.Red);
                    RhinoUtilities.setLayerVisibility("Tool Hit", true);
                }
            }
            else
            {
                RhinoUtilities.SetActiveLayer("LABELS", System.Drawing.Color.Red);
                RhinoUtilities.setLayerVisibility("Tool Hit", false);
            }

            //set the cluster sample layer to invisible
            RhinoUtilities.setLayerVisibility("CLUSTER SAMPLE", false);
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
                        PdfSettings pdfSettings = new PdfSettings();
                        //pdfSettings.PrinterName = PRINTERNAME;
                        pdfSettings.SetValue("Output", tempPdfPath);
                        pdfSettings.SetValue("ShowPDF", "no");
                        pdfSettings.SetValue("ShowSettings", "never");
                        pdfSettings.SetValue("ShowSaveAS", "never");
                        pdfSettings.SetValue("ShowProgress", "yes");
                        pdfSettings.SetValue("ShowProgressFinished", "no");
                        pdfSettings.SetValue("ConfirmOverwrite", "no");
                        pdfSettings.SetValue("Orientation", "portrait");
                        pdfSettings.WriteSettings(PdfSettingsFileType.RunOnce);
         

                        string command = string.Format("-_Print _Setup _Destination _Printer \"Bullzip PDF Printer\" _PageSize 210.000 297.00 _OutputType=Vector _Enter _View _AllLayouts _Enter _Enter _Go _Enter");
                        RhinoApp.RunScript(command, true);

                        string[] pdfs = new String[2]; //create a string array to hold the locations of the pdf with panel and agreement form pdf.
                        pdfs[0] = tempPdfPath;
                        pdfs[1] = agreementLocation;

                        //Uncomment the below line when adobe is purchased
                        // RhinoUtilities.combinePDF(oriPdfPath, pdfs, 0, 1, "Drawings First"); //pass the array and the target location to save the final pdf

                        RhinoUtilities.combinePDF(oriPdfPath, pdfs, 0, 1, "Watermark Only");

                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error printing PDF document." + ex.Message);
                    }
                }
            }
            return Result.Success;
        }
   }
}
