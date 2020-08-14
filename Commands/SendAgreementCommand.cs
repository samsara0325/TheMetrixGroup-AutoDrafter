using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using System.IO;
using CsvHelper;
using System.Linq;
using Rhino.DocObjects;
using MetrixGroupPlugins.Commands;
using System.Runtime.InteropServices;
using Bullzip.PdfWriter;
using System.Windows.Forms;
using MetrixGroupPlugins.MessageBoxes;
using CsvHelper.TypeConversion;
using MetrixGroupPlugins.Utilities;
using Rhino.Display;
using System.Threading.Tasks;
using System.Threading;
using MetrixGroupPlugins.Resources;

namespace MetrixGroupPlugins.Commands
{
   [CommandStyle(Style.ScriptRunner)]
   [System.Runtime.InteropServices.Guid("BCFEF4CA-FEC0-4653-B870-613019E1E81E")]

   /**
    * This class contains the code for the command which sends the final PDF drawing with the agreement 
    * to the client (author - Wilfred)
    * */
   public class SendAgreementCommand : Command
   {


      [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern bool SetDefaultPrinter(string Name);

      const string PRINTERNAME = "Bullzip PDF Printer";

      public SendAgreementCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static SendAgreementCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "SendAgreement"; }
      }

      OpenFileDialog getPDF = new OpenFileDialog();

      /// <summary>
      /// Runs the command.
      /// </summary>
      /// <param name="doc">The document.</param>
      /// <param name="mode">The mode.</param>
      /// <returns></returns>
      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         string pdfLocation;
         string receiverMail;
         string confirmMail;
         string defaultMessage;
         string fileName;

         SendAgreementForm form = new SendAgreementForm();

         // Put together the file name
         fileName = Path.GetFileNameWithoutExtension(doc.Name);

         pdfLocation = getPdfLocation(); //prompt the user and retrieve the location of the final pdf Document
         form.ShowDialog(); // display the form to get the relevant details
         
         receiverMail = form.getReceiverMail(); //retrieve the entered values (mail address)
         defaultMessage = form.getDefaultMessage(); //retrieve the entered values (default message address)


         //Grab the location of the saved PDF file
         String fileLocation = Path.GetDirectoryName(doc.Path) + @"\" + fileName + ".pdf";
         int lastPage = RhinoUtilities.calculatePageNumbers(fileLocation); //get the total page count of the PDF (to find the locatin of agreement)

         Rhino.RhinoApp.WriteLine("Please wait while the system sends the final PDF with the Agreement to the Client");
         sendAgreement(fileLocation, fileName, lastPage.ToString(), receiverMail, defaultMessage);
         Rhino.RhinoApp.WriteLine("Successfully sent");

  
         return Result.Success;
      }


      //This method prompts the user to retrieve the location of the PDF
      //Returns the location of the PDF as a string format
      public static String getPdfLocation()
      {
         //Open the File dialog to get the file location of cave image
         OpenFileDialog choofdlog = new OpenFileDialog();
         choofdlog.Filter = "PDF Files (*.pdf)|*.pdf";
         choofdlog.FilterIndex = 1;
         choofdlog.Multiselect = true;

         if (choofdlog.ShowDialog() == DialogResult.OK)
         {
            return choofdlog.FileName;
         }
         return null;
      }
      //Method is responsible for instantiating the adobe connector class to send the document
      public static Boolean sendAgreement(string fileLocation, string fileName, string lastPage, string clientMail,
         string defaultMessage)
      {
         Esign_Automation.adobeSignConnector.Main(fileLocation, fileName, clientMail, defaultMessage, lastPage.ToString());
         return true;
      }
   }
}