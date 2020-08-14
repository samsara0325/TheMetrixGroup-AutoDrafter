//using Rhino.Commands;
//using System;
//using System.Linq;
//using Bullzip.PdfWriter;
//using System.Windows.Forms;
//using Rhino;
//using MetrixGroupPlugins.Resources;
//using System.Runtime.InteropServices;
//using System.Drawing.Printing;
//using System.Diagnostics;

//namespace MetrixGroupPlugins.Commands
//{
//    [
//        System.Runtime.InteropServices.Guid("3a13066e-ba47-403f-be4a-d4481a97b272"),
//        CommandStyle(Style.ScriptRunner)
//    ]
//    public class CustomPrintCommand: Command
//    {
//        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
//        public static extern bool SetDefaultPrinter(string Name);
//        const string PRINTERNAME = "\\"+"\\QIEFP"+"\\"+"KONICA MINOLTA C227SeriesPCL";
//        //const string PRINTERNAME = "Bullzip PDF Printer";
//        CustomPrintForm cpf;
//        string panelType = "Flat";
        

//        public CustomPrintCommand()
//        {
//            // Rhino only creates one instance of each command class defined in a
//            // plug-in, so it is safe to store a refence in a static property.
//            Instance = this;
//        }

//        ///<summary>The only instance of this command.</summary>
//        public static CustomPrintCommand Instance
//        {
//            get;
//            private set;
//        }

//        ///<returns>The command name as it appears on the Rhino command line.</returns>
//        public override string EnglishName
//        {
//            get { return "CustomPrint"; }
//        }

//        ///<summary>The only instance of this command.</summary>
//        ///<param name="doc" RhinoDoc></param>
//        ///<param name="mode" Run mode></param>
//        ///<returns>returns sucess if doc is successfully created </returns>
//        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
//        {
//            #region fetching the filename for the output file based on the Rhino file

//            string[] checkFileName = Rhino.ApplicationSettings.FileSettings.RecentlyOpenedFiles()[0].ToString().Split('\\');
//            int checklength = checkFileName.Length;
//            string fileName = Rhino.ApplicationSettings.FileSettings.RecentlyOpenedFiles()[0].ToString().Split('\\')[--checklength];
//            fileName = fileName.Split('.')[0];
//            string pdfFilename = Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\" + fileName + ".pdf";

//            #endregion

//            // Setting the filename to display on the Checklist form
//            CustomPrintForm.fileName = fileName;

//            // Fetching all the layouts in the Rhino
//            var page_views = doc.Views.GetPageViews();

//            #region Asking for the checklist for one Layouts at the time 

//            foreach (var p in page_views)
//            {
//                CustomPrintForm.panelType = panelType;
//                CustomPrintForm.layoutName = p.PageName.ToString();
//                System.Windows.Forms.MessageBox.Show(p.PageName.ToString());

//                if (p.PageName.ToString().Contains("FLD"))
//                    CustomPrintForm.panelType = "Folded";

//                cpf = new CustomPrintForm();
//                Application.Run(cpf);

//                if (cpf.closeoption == false)
//                    return Result.Success;
//            }

//            #endregion

//            #region Saving and Printing the PDF 

//            PrintDialog dlg = new PrintDialog();

//            if (dlg.PrinterSettings.IsValid == false)
//            {
//                System.Windows.Forms.MessageBox.Show( GetDefaultPrinter() +" Printer is either not installed or turned off. PDF will not be generated.");
//            }
//            else
//            {
//                // If Page Views is 0 
//                if (doc.Views.GetPageViews().Count() != 0)
//                {
                    
//                    try
//                    {
//                        PdfSettings pdfSettings = new PdfSettings();
//                        //pdfSettings.PrinterName = PRINTERNAME;
//                        pdfSettings.LoadSettings(GetDefaultPrinter());
//                        pdfSettings.SetValue("Output",pdfFilename);
//                        pdfSettings.SetValue("ShowPDF", "no");
//                        pdfSettings.SetValue("ShowSettings", "never");
//                        pdfSettings.SetValue("ShowSaveAS", "never");
//                        pdfSettings.SetValue("ShowProgress", "yes");
//                        pdfSettings.SetValue("ShowProgressFinished", "no");
//                        pdfSettings.SetValue("ConfirmOverwrite", "no");
//                        pdfSettings.WriteSettings(PdfSettingsFileType.RunOnce);
//                        string command = string.Format("-_Print _Setup _Destination _Printer \"Bullzip PDF Printer\" _PageSize 297.000 210.00 _OutputType=Vector _Enter _View _AllLayout _Enter _Enter _Go");
                        
//                        // Export the selected curves
//                        RhinoApp.RunScript(command, true);

//                        System.Threading.Thread.Sleep(3000);

//                        // Set the default printer to konica from Bullzip and prints the PDF
//                        SetDefaultPrinter(PRINTERNAME);
//                        string defaultPrinterCheck = GetDefaultPrinter();
//                        SendToPrinter(pdfFilename);

//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("Error printing PDF document." + ex.Message);
//                    }
//                }
//            }

//            #endregion

//            return Result.Success;
//        }

//        ///<summary>This function set the compression parameters of PDF document</summary>
//        ///<returns>string --> default printer name</returns>
//        ///<author>Anubhav Passi, Date - 6/4/2018</author>
//        private string GetDefaultPrinter()
//        {
//            PrinterSettings settings = new PrinterSettings();
            
//            foreach (string printer in PrinterSettings.InstalledPrinters)
//            {
//                settings.PrinterName = printer;
//                if (settings.IsDefaultPrinter)
//                    return printer;
//            }
//            return string.Empty;
//        }


//        ///<summary>This function set the compression parameters of PDF document</summary>
//        ///<param name="fileName">PDF filename that need to be printed</param>
//        ///<author>Anubhav Passi, Date - 6/4/2018</author>
//        private void SendToPrinter( string fileName)
//        {
//            ProcessStartInfo info = new ProcessStartInfo();
//            info.Verb = "print";
//            info.FileName = fileName;
//            info.CreateNoWindow = true;
//            info.WindowStyle = ProcessWindowStyle.Hidden;

//            Process p = new Process();
//            p.StartInfo = info;
//            p.Start();
//            System.Threading.Thread.Sleep(2000);
//            p.WaitForInputIdle();
//            System.Threading.Thread.Sleep(3000);
//            if (false == p.CloseMainWindow())
//                p.Kill();
//        }

       
//    }
//}
