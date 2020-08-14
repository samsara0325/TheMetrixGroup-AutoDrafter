//using System;
//using System.Collections.Generic;
//using Rhino;
//using Rhino.Commands;
//using Rhino.Geometry;
//using System.IO;
//using CsvHelper;
//using System.Linq;
//using Rhino.DocObjects;
//using MetrixGroupPlugins.Commands;
//using System.Runtime.InteropServices;
//using Bullzip.PdfWriter;
//using System.Windows.Forms;

//namespace MetrixGroupPlugins
//{
//   [
//      System.Runtime.InteropServices.Guid("9e91f9f5-2c78-4d53-a51d-b13272f316ae"),
//      CommandStyle(Style.ScriptRunner)
//   ]
//   public class CreatePanelFromCSVCommand : Command
//   {
//      [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
//      public static extern bool SetDefaultPrinter(string Name);

//      const string PRINTERNAME = "Bullzip PDF Printer";
//      public CreatePanelFromCSVCommand()
//      {
//         // Rhino only creates one instance of each command class defined in a
//         // plug-in, so it is safe to store a refence in a static property.
//         Instance = this;
//      }

//      ///<summary>The only instance of this command.</summary>
//      public static CreatePanelFromCSVCommand Instance
//      {
//         get;
//         private set;
//      }

//      ///<returns>The command name as it appears on the Rhino command line.</returns>
//      public override string EnglishName
//      {
//         get { return "CreatePanelFromCSV"; }
//      }

//      /// <summary>
//      /// Runs the command.
//      /// </summary>
//      /// <param name="doc">The document.</param>
//      /// <param name="mode">The mode.</param>
//      /// <returns></returns>
//      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
//      {
//         PanelParameters parameters = null;
//         List<PerforationPanel> panelList = null;
//         double[] rowHeight;
//         double[] colWidth;
//         double[] xGrid;
//         double[] yGrid;
//         double maxRowHeight;
//         double maxColWidth;


//         // Read in the CSV file for parameters
//         try
//         {
//            TextReader paraReader;

//            using (paraReader = new StreamReader(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\RhinoPara.csv"))
//            {
//               var csv = new CsvReader(paraReader);
//               var map = new PanelParametersCSVClassMap();
//               csv.Configuration.RegisterClassMap(map);
//               csv.Configuration.HasHeaderRecord = true;

//               csv.Read();
//               parameters = csv.GetRecord<PanelParameters>();
//            }
//         }
//         catch(Exception e)
//         {
//            Console.WriteLine("Error: " + e.Message);
//         }

//         // Read in the CSV file for panels
//         try
//         {
//            TextReader panelReader;

//            using (panelReader = File.OpenText(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\Panels.csv"))
//            {
//               var csv = new CsvReader(panelReader);
//               var map = new PanelCSVClassMap();
//               csv.Configuration.RegisterClassMap(map);
//               csv.Configuration.HasHeaderRecord = true;
//               csv.Configuration.SkipEmptyRecords = true;

//               panelList = csv.GetRecords<PerforationPanel>().ToList();
//            }
//         }
//         catch (Exception e)
//         {
//            Console.WriteLine("Error: " + e.Message);
//         }

//         // Put together the file name

//         string fileName = Path.GetFileNameWithoutExtension(doc.Name);

//         // Save the file
//         RhinoApp.RunScript("-_Save _Enter", true);

//         if (panelList != null)
//         {
//            int maxRow = 0;
//            int maxCol = 0;
//            int panelNumber = 1;

//            // Final the max Row and Col
//            foreach (PerforationPanel panel in panelList)
//            {
//               panel.PanelNumber = panelNumber;

//               panelNumber++;

//               if (panel.Row > maxRow)
//                  maxRow = panel.Row;
//               if (panel.Column > maxCol)
//                  maxCol = panel.Column;
//            }

//            rowHeight = Enumerable.Repeat(0.0, maxRow).ToArray();
//            colWidth = Enumerable.Repeat(0.0, maxCol).ToArray();
//            maxColWidth = 0;
//            maxRowHeight = 0;

//            // Final the max Row and Col
//            foreach (PerforationPanel panel in panelList)
//            {
//               if( maxColWidth < panel.X)
//               {
//                  maxColWidth = panel.X;
//               }

//               if (maxRowHeight  < panel.Y)
//               {
//                  maxRowHeight = panel.Y;
//               }
               
//               //if (rowHeight[panel.Row - 1] < panel.Y)
//               //   rowHeight[panel.Row - 1] = panel.Y;
//               //if (colWidth[panel.Column - 1] < panel.X)
//               //   colWidth[panel.Column - 1] = panel.X;

//            }

//            yGrid = Enumerable.Repeat(0.0, maxRow + 1).ToArray();
//            xGrid = Enumerable.Repeat(0.0, maxCol + 1).ToArray();

//            for (int i = 0; i < maxCol; i++)
//            {
//               xGrid[i + 1] = xGrid[i] + maxColWidth + parameters.ColSpacing;
//            }

//            for (int i = 0; i < maxRow; i++)
//            {
//               yGrid[i + 1] = yGrid[i] - maxRowHeight - parameters.RowSpacing;
//            }

//            // Draw the sample block
//            RhinoUtilities.SetActiveLayer("PERF SAMPLE", System.Drawing.Color.Black);

//            BoundingBox panelBox = new BoundingBox(-400, 100, 0, -100, 400, 0);
//            List<Point3d> rectangle_corners = panelBox.GetCorners().Distinct().ToList();
//            // add 1st point at last to close the loop
//            rectangle_corners.Add(rectangle_corners[0]);
//            Guid sampleBox = doc.Objects.AddPolyline(rectangle_corners);

//            if (parameters.PatternDirection == 1)
//            {
//               // Export the selected curves
//               RhinoApp.RunScript("SelAll", true);
//               RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
//            }

//            if (panelList.Count > 0)
//            {

//               PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(sampleBox).Curve());
//               double openArea = perforationForm.drawPerforationDesign(panelList[0].PatternName, false);

//               if (parameters.PatternDirection == 1)
//               {
//                  // Export the selected curves
//                  RhinoApp.RunScript("SelAll", true);
//                  RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
//               }

//               RhinoUtilities.SetActiveLayer("PERF SAMPLE", System.Drawing.Color.Black);
//               // all non-light objects that are selected
//               var object_enumerator_settings = new ObjectEnumeratorSettings();
//               object_enumerator_settings.IncludeLights = false;
//               object_enumerator_settings.IncludeGrips = true;
//               object_enumerator_settings.NormalObjects = true;
//               object_enumerator_settings.LockedObjects = true;
//               object_enumerator_settings.HiddenObjects = true;
//               object_enumerator_settings.ReferenceObjects = true;
//               object_enumerator_settings.SelectedObjectsFilter = true;
//               var selected_objects = doc.Objects.GetObjectList(object_enumerator_settings);

//               var current_layer_index = doc.Layers.CurrentLayerIndex;

//               foreach (var rhobj in doc.Objects)
//               {
//                  if (rhobj.ObjectType == ObjectType.Curve)
//                  {
//                     Curve curve = rhobj.Geometry as Curve;

//                     if (Curve.PlanarClosedCurveRelationship(new Rhino.DocObjects.ObjRef(sampleBox).Curve(), curve, Plane.WorldXY, 0) == RegionContainment.BInsideA)
//                     {
//                        rhobj.Attributes.LayerIndex = current_layer_index;
//                        rhobj.CommitChanges();
//                     }
//                  }
//               }

//               Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(-85, 200, 0);
//               string text = "Pattern: " + panelList[0].PatternName;
//               double height = 15;
//               string font = "Arial";
//               Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
//               plane.Origin = pt;
//               Guid id = doc.Objects.AddText(text, plane, height, font, false, false);

//               pt = new Rhino.Geometry.Point3d(-85, 150, 0);
//               text = "Open Area: " + openArea.ToString("#.") + "%";
//               height = 15;
//               font = "Arial";
//               plane.Origin = pt;
//               id = doc.Objects.AddText(text, plane, height, font, false, false);

//            }

//            // Draw the block
//            drawBlock(doc);

//            parameters.TotalPanel = panelList.Count;

//            // draw the panel
//            foreach (PerforationPanel panel in panelList)
//            {
//               PanelDrawer.drawPanel(xGrid[panel.Column - 1], xGrid[panel.Column], yGrid[panel.Row - 1], yGrid[panel.Row], panel, parameters);
//            }

//            // Select each group and add layout
//            for (int i = 0; i < doc.Groups.Count; i++)
//            {
//               doc.Objects.UnselectAll();
//               RhinoObject[] rhObjList = doc.Groups.GroupMembers(i);

//               foreach (RhinoObject ro in rhObjList)
//               {
//                  ro.Select(true);
//               }

//               AddLayoutCommand.Instance.createLayout(doc, panelList[i], parameters);
//            }


//            // Set the default printer to bull zip 
//            // Locals
//            // bool result;

//            // Set default printer
//            // result = SetDefaultPrinter(PRINTERNAME);

//            System.Windows.Forms.PrintDialog dlg = new PrintDialog();

//            if (dlg.PrinterSettings.IsValid == false)
//            {
//               System.Windows.Forms.MessageBox.Show("Bullzip PDF printer is not installed. PDF will not be generated.");
//            }
//            else
//            {
//               // If Page Views is 0 
//               if (doc.Views.GetPageViews().Count() != 0)
//               {
//                  try
//                  {
//                     PdfSettings pdfSettings = new PdfSettings();
//                     pdfSettings.PrinterName = PRINTERNAME;
//                     pdfSettings.SetValue("Output", Path.GetDirectoryName(doc.Path) + @"\" + fileName + ".pdf");
//                     pdfSettings.SetValue("ShowPDF", "no");
//                     pdfSettings.SetValue("ShowSettings", "never");
//                     pdfSettings.SetValue("ShowSaveAS", "never");
//                     pdfSettings.SetValue("ShowProgress", "yes");
//                     pdfSettings.SetValue("ShowProgressFinished", "no");
//                     pdfSettings.SetValue("ConfirmOverwrite", "no");
//                     pdfSettings.WriteSettings(PdfSettingsFileType.RunOnce);

//                     string command = string.Format("-_Print _Setup _Destination _Printer \"Bullzip PDF Printer\" _PageSize 297.000 210.00 _OutputType=Vector _Enter _View _AllLayout _Enter _Enter _Go");

//                     // Export the selected curves
//                     RhinoApp.RunScript(command, true);
//                  }
//                  catch (Exception ex)
//                  {
//                     System.Windows.Forms.MessageBox.Show("Error printing PDF document." + ex.Message);
//                  }
//               }
//            }
//         }

//         return Result.Success;
//      }

//      /// <summary>
//      /// Draws the block.
//      /// </summary>
//      private void drawBlock(RhinoDoc doc)
//      {
//         // Set the layer
//         RhinoUtilities.SetActiveLayer("DETAILS", System.Drawing.Color.Black);

//         // Get the location of current API 
//         String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Logo\\MetrixLogo.jpg";

//         Plane picture = new Plane(new Point3d(5, 510, 0), new Vector3d(0, 0, 1));

//         // Add Company logo.
//         doc.Objects.AddPictureFrame(picture, path, false, 62.5, 25, false, false);

//         // Draw bottom line
//         doc.Objects.AddLine(new Point3d(5, 505, 0), new Point3d(205, 505, 0));

//         // Top line
//         doc.Objects.AddLine(new Point3d(5, 792, 0), new Point3d(205, 792, 0));

//         // Left line
//         doc.Objects.AddLine(new Point3d(5, 505, 0), new Point3d(5, 792, 0));

//         // Right line
//         doc.Objects.AddLine(new Point3d(205, 505, 0), new Point3d(205, 792, 0));

//         // Divider line
//         doc.Objects.AddLine(new Point3d(5, 510, 0), new Point3d(205, 510, 0));

//         // Row 1 line
//         doc.Objects.AddLine(new Point3d(67.5, 515, 0), new Point3d(205, 515, 0));

//         // Row 2 line
//         doc.Objects.AddLine(new Point3d(67.5, 520, 0), new Point3d(205, 520, 0));

//         // Row 3 line
//         doc.Objects.AddLine(new Point3d(67.5, 525, 0), new Point3d(205, 525, 0));

//         // Row 4 line
//         doc.Objects.AddLine(new Point3d(67.5, 530, 0), new Point3d(205, 530, 0));

//         // Row 5 line
//         doc.Objects.AddLine(new Point3d(67.5, 535, 0), new Point3d(205, 535, 0));

//         // Vertical divider line
//         doc.Objects.AddLine(new Point3d(136.5, 505, 0), new Point3d(136.5, 535, 0));

//         // Vertical divider line
//         doc.Objects.AddLine(new Point3d(87.5, 510, 0), new Point3d(87.5, 535, 0));

//         // Vertical divider line
//         doc.Objects.AddLine(new Point3d(156.5, 510, 0), new Point3d(156.5, 535, 0));

//         // Draw Approval Box
//         // Horizontal lines
//         doc.Objects.AddLine(new Point3d(136.5, 540, 0), new Point3d(200, 540, 0));
//         doc.Objects.AddLine(new Point3d(136, 539.5, 0), new Point3d(200.5, 539.5, 0));
//         doc.Objects.AddLine(new Point3d(136.5, 545, 0), new Point3d(200, 545, 0));
//         doc.Objects.AddLine(new Point3d(136.5, 555, 0), new Point3d(200, 555, 0));
//         doc.Objects.AddLine(new Point3d(136.5, 560, 0), new Point3d(200, 560, 0));
//         doc.Objects.AddLine(new Point3d(136.5, 565, 0), new Point3d(200, 565, 0));
//         doc.Objects.AddLine(new Point3d(136, 565.5, 0), new Point3d(200.5, 565.5, 0));

//         // Vertical lines
//         doc.Objects.AddLine(new Point3d(136, 539.5, 0), new Point3d(136, 565.5, 0));
//         doc.Objects.AddLine(new Point3d(136.5, 540, 0), new Point3d(136.5, 565, 0));
//         doc.Objects.AddLine(new Point3d(156.5, 540, 0), new Point3d(156.5, 560, 0));
//         doc.Objects.AddLine(new Point3d(200, 540, 0), new Point3d(200, 565, 0));
//         doc.Objects.AddLine(new Point3d(200.5, 539.5, 0), new Point3d(200.5, 565.5, 0));

//         Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(11, 515, 0);
//         string text = "51 Holloway Drive            metrixgroup.com.au\nBayswater VIC 3153                  1300 792 493";
//         double height = 1.8;
//         string font = "Arial";
//         Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
//         plane.Origin = pt;
//         Guid id = doc.Objects.AddText(text, plane, height, font, false, false);

//         height = 1.5;
//         text = "Supply of this order will be solely and exlusively according to the terms and conditions of Metrix Group Pty Ltd.";
//         plane.Origin = new Point3d(7, 506, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         height = 1.5;
//         text = "PANEL";
//         plane.Origin = new Point3d(69, 531, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);
         
//         text = "DRAWN";
//         plane.Origin = new Point3d(69, 526, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = Environment.UserName;
//         plane.Origin = new Point3d(89, 526, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "CHECKED";
//         plane.Origin = new Point3d(69, 521, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "m²";
//         plane.Origin = new Point3d(69, 516, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "Page ";
//         plane.Origin = new Point3d(69, 511, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "PROJECT";
//         plane.Origin = new Point3d(138, 531, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "CUSTOMER ";
//         plane.Origin = new Point3d(138, 526, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "JOB NO.";
//         plane.Origin = new Point3d(138, 521, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "MATERIAL";
//         plane.Origin = new Point3d(138, 516, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "COATING";
//         plane.Origin = new Point3d(138, 511, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);
         
//         text = "Copyright © Metrix Group " + DateTime.Today.Year;
//         plane.Origin = new Point3d(138, 506, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "APPROVED BY";
//         plane.Origin = new Point3d(138, 561, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "NAME";
//         plane.Origin = new Point3d(138, 556, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "SIGNATURE";
//         plane.Origin = new Point3d(138, 551, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);

//         text = "DATE";
//         plane.Origin = new Point3d(138, 541, 0);
//         id = doc.Objects.AddText(text, plane, height, font, false, false);
         
//      }
//   }
//}
