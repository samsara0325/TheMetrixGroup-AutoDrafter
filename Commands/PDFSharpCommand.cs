using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Rhino.DocObjects;
using System.Windows.Forms;
using System.IO;
using MetrixGroupPlugins.Resources;


namespace MetrixGroupPlugins.Commands
{
    [CommandStyle(Style.ScriptRunner)]
    [System.Runtime.InteropServices.Guid("eef05125-c5b9-4869-aa02-c14607c871aa")]
    public class PDFSharpCommand : Command
    {
        //Class variables 
        string myUniqueFileName = "";
        static string logFilePath = Rhino.ApplicationSettings.FileSettings.WorkingFolder + "\\Log-" + DateTime.Now.GetHashCode().ToString() + ".txt";
        string ext = ".pdf";
        string rhinoFile;
        char getSelectedOption;
        PDFOptionForm pdfOptionForm;


        public PDFSharpCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static PDFSharpCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "PDFSharpCommand"; }
        }


        ///<summary>The only instance of this command.</summary>
        ///<param name="doc" RhinoDoc></param>
        ///<param name="mode" Run mode></param>
        ///<returns>returns sucess if doc is successfully created </returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            #region set all the file parameters

            pdfOptionForm = new PDFOptionForm();
            Application.Run(pdfOptionForm);
            getSelectedOption = PDFOptionForm.optionsSelected;

            myUniqueFileName = string.Format(@"{0}" + ext, Guid.NewGuid());
            string logFilePath = Rhino.ApplicationSettings.FileSettings.WorkingFolder + "\\Log-" + DateTime.Now.GetHashCode().ToString() + ".txt";
            string curveType = "C";
            try
            {
                string[] checkFileName = Rhino.ApplicationSettings.FileSettings.RecentlyOpenedFiles()[0].ToString().Split('\\');
                int checklength = checkFileName.Length;
                rhinoFile = Rhino.ApplicationSettings.FileSettings.RecentlyOpenedFiles()[0].ToString().Split('\\')[--checklength];
            }
            catch (FileNotFoundException fnf) { logFile(fnf.Message); }
            catch (IndexOutOfRangeException ioor) { logFile(ioor.Message); }

            string rhinoFileName = rhinoFile.Split('.')[0];
            string filename = Rhino.ApplicationSettings.FileSettings.WorkingFolder + '\\' + rhinoFileName + "-" + myUniqueFileName;

            #endregion

            Rhino.RhinoApp.RunScript("-_Save Enter", false);

            logFile("Process started for ---> " + rhinoFileName + "-" + myUniqueFileName);

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

            #region Check the selected curve
            GetObject go = new GetObject();

            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.EnableSelPrevious(true);
            go.EnablePreSelect(true, false);
            go.GeometryFilter = ObjectType.AnyObject;

            GetResult result = go.GetMultiple(1, -1);

            if (go.CommandResult() != Rhino.Commands.Result.Success)
            {
                return go.CommandResult();
            }

            RhinoApp.WriteLine("{0} objects are selected.", go.ObjectCount);

            #endregion

            // Process the list of objects
            logFile("Processing List of Object");

            #region set the paramenters for Xgraph to process shapes

            List<GeometryBase> geoList = new List<GeometryBase>();

            double minX, minY, maxX, maxY;
            minX = double.MaxValue;
            minY = double.MaxValue;

            maxX = double.MinValue;
            maxY = double.MinValue;

            List<Curve> curveList = new List<Curve>();
            List<TextEntity> textEntityList = new List<TextEntity>();

            List<Circle> circleList = new List<Circle>();
            List<Polyline> polyList = new List<Polyline>();
            Circle circleCurve;
            Polyline polygon;

            #endregion

            for (int i = 0; i < go.ObjectCount; i++)
            {
                // Check the type of the Object and process differently
                Curve curve = go.Object(i).Curve();

                if (go.Object(i).Curve().TryGetCircle(out circleCurve))
                    circleList.Add(circleCurve);
                if (go.Object(i).Curve().TryGetPolyline(out polygon))
                    polyList.Add(polygon);
                if (curve != null)
                    curveList.Add(curve);

                TextEntity te = go.Object(i).TextEntity();

                if (te != null)
                {
                    textEntityList.Add(te);
                }

                GeometryBase geo = go.Object(i).Geometry();
                BoundingBox bbox = geo.GetBoundingBox(Rhino.Geometry.Plane.WorldXY);

                if (bbox.Min.X < minX)
                {
                    minX = bbox.Min.X;
                }
                if (bbox.Min.Y < minY)
                {
                    minY = bbox.Min.Y;
                }
                if (bbox.Max.X > maxX)
                {
                    maxX = bbox.Max.X;
                }
                if (bbox.Max.Y > maxY)
                {
                    maxY = bbox.Max.Y;
                }

                geoList.Add(geo);

            }

            page.Height = maxY - minY;
            page.Width = maxX - minX;


            foreach (GeometryBase g in geoList)
            {
                if (g.GetType().Equals(typeof(PolyCurve)))
                {
                    //System.Windows.Forms.MessageBox.Show("PolyCurve changed");
                    PolyCurve polyCurve = (PolyCurve)g;
                    curveType = "NC";
                    break;
                }
                else if (g.GetType().Equals(typeof(Curve)))
                {
                    System.Windows.Forms.MessageBox.Show("Curve");
                    Curve curve = (Curve)g;
                }
                else if (g.GetType().Equals(typeof(TextEntity)))
                {
                    System.Windows.Forms.MessageBox.Show("TextEntity");
                    TextEntity textEntity = (TextEntity)g;
                    curveType = "T";
                }


            }

            logFile("Checking the pattern");
            if (curveType.Equals("C") || curveType.Equals("T"))
            {
                logFile("Objects processed sucessfully");

                double x1, y1, width, height;

                logFile("Creating Circles on the PDF");
                //Loop to draw the circles 
                foreach (Circle c in circleList)
                {
                    XPen pen = new XPen(XColors.Black, 0.5);
                    x1 = c.BoundingBox.Min.X - minX;
                    y1 = maxY - c.BoundingBox.Max.Y;
                    width = c.BoundingBox.Max.X - c.BoundingBox.Min.X;
                    height = c.BoundingBox.Max.Y - c.BoundingBox.Min.Y;
                    gfx.DrawEllipse(XBrushes.Black, x1, y1, width, height);

                }

                //Loop used to draw rectangles 
                foreach (Polyline p in polyList)
                {
                    XPen pen = new XPen(XColors.Black, 0.5);
                    x1 = p.BoundingBox.Min.X - minX;
                    y1 = maxY - p.BoundingBox.Max.Y;
                    width = p.BoundingBox.Max.X - p.BoundingBox.Min.X;
                    height = p.BoundingBox.Max.Y - p.BoundingBox.Min.Y;
                    XPoint p1 = new XPoint(x1, y1);
                    XPoint p2 = new XPoint(x1 + width, y1);
                    XPoint p3 = new XPoint(x1, y1 + height);
                    XPoint p4 = new XPoint(x1 + width, y1 + height);
                    XRect rect = new XRect(x1, y1, width, height);
                    XPoint[] xPoint = new XPoint[] { p1, p2, p4, p3 };
                    //XPoint mid = new XPoint( (x1 + x1)/2);
                    XGraphicsState gs = gfx.Save();
                    gfx.RotateAtTransform(-45, p1);
                    gfx.DrawPolygon(pen, XBrushes.Black, xPoint, XFillMode.Alternate);
                    gfx.Restore(gs);
                }



                #region Print the PDF as per the option selected 
                if (getSelectedOption.Equals('N'))
                {
                    logFile("Normal PDF feature was selected");
                    document.Save(filename);
                    logFile("Document saved successfully - " + Rhino.ApplicationSettings.FileSettings.WorkingFolder);
                }
                if (getSelectedOption.Equals('C'))
                {
                    logFile("Compressed PDF feature was selected");
                    CompressMyPdf(document);
                    string compressedFileName = Rhino.ApplicationSettings.FileSettings.WorkingFolder + '\\' + "C-" + rhinoFileName + "-" + myUniqueFileName;
                    document.Save(compressedFileName);
                }
                if (getSelectedOption.Equals('E'))
                {
                    logFile("Encrypted PDF feature was selected");
                    EncryptMyPdf(document);
                    string encryptPdf = Rhino.ApplicationSettings.FileSettings.WorkingFolder + '\\' + "E-" + rhinoFileName + "-" + myUniqueFileName;
                    document.Save(encryptPdf);
                }
                if (getSelectedOption.Equals('P'))
                {
                    logFile("Password Protection PDF feature was selected");
                    PasswordProtectMyPdf(document);
                    string passwordProtectPdf = Rhino.ApplicationSettings.FileSettings.WorkingFolder + '\\' + "PP-" + rhinoFileName + "-" + myUniqueFileName;
                    document.Save(passwordProtectPdf);
                }

                #endregion

                logFile("Document saved successfully - " + Rhino.ApplicationSettings.FileSettings.WorkingFolder);

                logFile("Panel perforated successfully. Check File --> " + rhinoFileName + "-" + myUniqueFileName);
                System.Windows.Forms.MessageBox.Show("         <----SUCCESS---->       " + Environment.NewLine + Environment.NewLine + " Pannels perforated Successfully. ");

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("                           ERROR!     " + Environment.NewLine + Environment.NewLine + "The curve you have selected contains some invalid shape." + Environment.NewLine + " Please select the appropriate patterns. ");
                logFile("Please select the appropriate pattern");
            }

            logFile("----------------- WAITING FOR THE NEXT PANEL PERFORATION ---------------------");

            return Result.Success;
        }

        void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, x, y, width, height);
        }


        ///<summary>This function logs the message onto a text file</summary>
        ///<param name="mssg"></param>
        ///<author>Anubhav Passi, Date - 26/3/2018</author>
        void logFile(string mssg)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Message :" + mssg + Environment.NewLine + "Date :" + DateTime.Now.ToString() + Environment.NewLine);
                    //writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                logFile(e.Message);
            }
        }

        ///<summary>This function set the compression parameters of PDF document</summary>
        ///<param name="pdfDoc"></param>
        ///<author>Anubhav Passi, Date - 2/3/2018</author>
        void CompressMyPdf(PdfDocument pdfDoc)
        {

            try
            {
                // Code for PDF Compression
                pdfDoc.Options.FlateEncodeMode = PdfFlateEncodeMode.BestCompression;
                pdfDoc.Options.UseFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Automatic;
                pdfDoc.Options.EnableCcittCompressionForBilevelImages = true;
                pdfDoc.Options.CompressContentStreams = true;
                pdfDoc.Options.NoCompression = false;
            }
            catch (Exception e)
            {
                logFile(e.Message);
            }

        }

        ///<summary>This function encrypt the PDF document</summary>
        ///<param name="pdfDoc"></param>
        ///<author>Anubhav Passi, Date - 2/3/2018</author>
        void EncryptMyPdf(PdfDocument pdfDoc)
        {
            // Code for PDF encryption
            pdfDoc.SecuritySettings.DocumentSecurityLevel = PdfSharp.Pdf.Security.PdfDocumentSecurityLevel.Encrypted128Bit;
            pdfDoc.SecuritySettings.OwnerPassword = "toby";
            PdfObject owner = pdfDoc.SecurityHandler.Owner;
            //pdfDoc.SecurityHandler.UserPassword = "passi";
            pdfDoc.SecuritySettings.PermitModifyDocument = false;
            pdfDoc.SecuritySettings.PermitPrint = false;
            pdfDoc.SecuritySettings.PermitFormsFill = false;


        }

        ///<summary>This function paswword protect the PDF document</summary>
        ///<param name="pdfDoc"></param>
        ///<author>Anubhav Passi, Date - 2/3/2018</author>
        void PasswordProtectMyPdf(PdfDocument pdfDoc)
        {
            // Code for PDF paswword protection
            pdfDoc.SecuritySettings.DocumentSecurityLevel = PdfSharp.Pdf.Security.PdfDocumentSecurityLevel.Encrypted128Bit;
            PdfObject owner = pdfDoc.SecurityHandler.Owner;
            pdfDoc.SecurityHandler.UserPassword = "passi";
            pdfDoc.SecuritySettings.OwnerPassword = "toby";
        }

        void DrawCurve(XGraphics gfx)
        {

            XPen pen = new XPen(XColors.Navy, 4);
            gfx.DrawLine(pen, 0, 20, 250, 20);

            pen = new XPen(XColors.Firebrick, 6);
            pen.DashStyle = XDashStyle.Dash;
            gfx.DrawLine(pen, 0, 40, 250, 40);
            pen.Width = 7.3;
            pen.DashStyle = XDashStyle.DashDotDot;
            gfx.DrawLine(pen, 0, 60, 250, 60);

            pen = new XPen(XColors.Goldenrod, 10);
            pen.LineCap = XLineCap.Flat;
            gfx.DrawLine(pen, 10, 90, 240, 90);
            gfx.DrawLine(XPens.Black, 10, 90, 240, 90);

            pen = new XPen(XColors.Goldenrod, 10);
            pen.LineCap = XLineCap.Square;
            gfx.DrawLine(pen, 10, 110, 240, 110);
            gfx.DrawLine(XPens.Black, 10, 110, 240, 110);

            pen = new XPen(XColors.Goldenrod, 10);
            pen.LineCap = XLineCap.Round;
            gfx.DrawLine(pen, 10, 130, 240, 130);
            gfx.DrawLine(XPens.Black, 10, 130, 240, 130);
        }
    }

}
