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

namespace MetrixGroupPlugins.Commands
{
    [
      System.Runtime.InteropServices.Guid("9e91f9f5-2c78-4d53-a51d-b13242f316ae"),
      CommandStyle(Style.ScriptRunner)
   ]
    public class CreatePanelsCommand : Command
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

        const string PRINTERNAME = "Bullzip PDF Printer";

        public CreatePanelsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CreatePanelsCommand Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "CreatePanels"; }
        }

        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            PanelParameters parameters = null;
            bool shouldDrawSummary = true;
            //List created to add the panels
            List<FoldedPerforationPanel> panelList = new List<FoldedPerforationPanel>();
            int parentlayerIndex;
            double[] rowHeight;
            double[] colWidth;
            double[] xGrid;
            double[] yGrid;
            double maxRowHeight;
            double maxColWidth;
            Boolean foundFolded = false;
            String materialThickness = null;
            int panelQty = 0;
            int totalPanelQty = 0;

            bool fixingHolesManipulated = false;
            bool flatPanelsFound = false;
            // Read in the CSV file for panels
            // Map the CSV to FoldedPanelCSV 
            // Read all the panels to the List of panelList 

            TextReader panelReader;

            using (panelReader = File.OpenText(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\Panel.csv"))
            {
                //First try to map with new draft sheet specs and summary page specs
                try
                {
                    var csv = new CsvReader(panelReader);
                    var map = new FPPanelCSVClassMap(false, true);
                    csv.Configuration.RegisterClassMap(map);
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.SkipEmptyRecords = true;

                    panelList = csv.GetRecords<FoldedPerforationPanel>().ToList();
                    shouldDrawSummary = true;
                }
                catch (CsvTypeConverterException ex)
                {
                    Messages.showPanelCSVMapError(ex.GetBaseException().ToString());
                    return Result.Failure;
                }
                catch (Exception ex)
                {
                    //Catch rhino para not found exception
                    //Map rhino para
                    // Read in the CSV file for parameters
                    try
                    {
                        TextReader paraReader;

                        using (paraReader = new StreamReader(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\Panel.csv"))
                        {
                            var paraCsv = new CsvReader(paraReader);
                            var paraMap = new NewPanelParametersCSVClassMap();
                            paraCsv.Configuration.RegisterClassMap(paraMap);
                            paraCsv.Configuration.HasHeaderRecord = true;
                            paraCsv.Read();
                            parameters = paraCsv.GetRecord<PanelParameters>();
                        }

                        //Read the panels and map it 
                        using (panelReader = File.OpenText(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\Panel.csv"))
                        {
                            var csv = new CsvReader(panelReader);
                            var map = new FPPanelCSVClassMap(false, false); //try with new draft specs but no summary page specs
                            csv.Configuration.RegisterClassMap(map);
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.SkipEmptyRecords = true;

                            panelList = csv.GetRecords<FoldedPerforationPanel>().ToList();
                            shouldDrawSummary = false;
                        }
                    }
                    catch (Exception e)
                    {
                        //Catch rhino para not found exception
                        //Map rhino para
                        // Read in the CSV file for parameters
                        try
                        {
                            TextReader paraReader;

                            using (paraReader = new StreamReader(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\RhinoPara.csv"))
                            {
                                var paraCsv = new CsvReader(paraReader);
                                var paraMap = new NewPanelParametersCSVClassMap();
                                paraCsv.Configuration.RegisterClassMap(paraMap);
                                paraCsv.Configuration.HasHeaderRecord = true;
                                paraCsv.Read();
                                parameters = paraCsv.GetRecord<PanelParameters>();
                            }

                            //Read the panels and map it 
                            using (panelReader = File.OpenText(Rhino.ApplicationSettings.FileSettings.WorkingFolder + @"\Panel.csv"))
                            {
                                var csv = new CsvReader(panelReader);
                                var map = new FPPanelCSVClassMap(true, false); //do not add new specs or summary specs
                                csv.Configuration.RegisterClassMap(map);
                                csv.Configuration.HasHeaderRecord = true;
                                csv.Configuration.SkipEmptyRecords = true;

                                panelList = csv.GetRecords<FoldedPerforationPanel>().ToList();
                                shouldDrawSummary = false;
                            }

                            //Set rhino parameter values in the parameters in panels so that it behaves like the new draft sheet
                            foreach (FoldedPerforationPanel panel in panelList)
                            {
                                panel.rowSpacing = parameters.RowSpacing;
                                panel.colSpacing = parameters.ColSpacing;
                                panel.labelHeight = parameters.LabelHeight;
                                panel.project = parameters.Project;
                                panel.customerName = parameters.CustomerName;
                                panel.jobNo = parameters.JobNo;
                                panel.material = parameters.Material;
                                panel.coating = parameters.Coating;
                                panel.revision = parameters.Revision;
                                panel.patternDirection = parameters.PatternDirection;
                                panel.colour = parameters.Colour;
                                panel.drafterName = parameters.DrafterName;
                                panel.drawPerfOnFirstPanel = panel.DrawPerf == 1 ? true : false;
                                panel.FirstRevisionDate = parameters.FirstRevisionDate;
                                panel.RevisionReason = parameters.RevisionReason;
                            }
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine("Error: " + excep.Message);
                            MessageBox.Show(e.Message);
                        }
                    }

                }
            }

            foreach (FoldedPerforationPanel singlePanel in panelList)
            {
                if (string.IsNullOrEmpty(singlePanel.PatternOpenArea))
                {
                    singlePanel.PatternOpenArea = "0";
                }
            }
            // Put together the file name
            string fileName = Path.GetFileNameWithoutExtension(doc.Name);

            // Save the file
            RhinoApp.RunScript("-_Save _Enter", true);


            //Check whether the panel list count is not 0, if not proceed 
            if (panelList.Count > 0)
            {
                int maxRow = 0;
                int maxCol = 0;
                int panelNumber = 1;
                bool fixingHolesManipulatedMessageShown = false;
                // Final the max Row and Col
                foreach (FoldedPerforationPanel panel in panelList)
                {
                    panel.PanelNumber = panelNumber;

                    panelNumber++;

                    if (panel.Row > maxRow)
                        maxRow = panel.Row;
                    if (panel.Column > maxCol)
                        maxCol = panel.Column;

                    if (panel.PanelType.Equals("Single Folded") || panel.PanelType.Equals("Double Folded")) //finds whether there are any folded panels in the panel list, if found set the boolean to true
                    {
                        foundFolded = true;
                    }

                    //Temporary pop up to ask if fixing holes are entered manually 
                    //if (panel.FixingHoles == "1" && !fixingHolesManipulatedMessageShown)
                   // {
                        //pop up message
                        fixingHolesManipulated = false;
                       // fixingHolesManipulated = Messages.showIsFixingHoleDataChanged();
                  //      fixingHolesManipulatedMessageShown = true;
                  //  }
                }

                panelQty = panelNumber - 1; //sets the total number of panels in the list 

                rowHeight = Enumerable.Repeat(0.0, maxRow).ToArray();
                colWidth = Enumerable.Repeat(0.0, maxCol).ToArray();
                maxColWidth = 0;
                maxRowHeight = 0;

                // Final the max Row and Col
                foreach (FoldedPerforationPanel panel in panelList)
                {

                    if (foundFolded)  //Folded panels are found, use the folded panels to calculate the even distance 
                    {
                        if (maxColWidth < panel.X + panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth + panel.LeftSecondFoldWidth + panel.RightSecondFoldWidth)
                        {
                            maxColWidth = panel.X + panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth + panel.LeftSecondFoldWidth + panel.RightSecondFoldWidth;
                        }

                        if (maxRowHeight < panel.Y + panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + panel.TopSecondFoldWidth + panel.BottomSecondFoldWidth)
                        {
                            maxRowHeight = panel.Y + panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + panel.TopSecondFoldWidth + panel.BottomSecondFoldWidth;
                        }
                    }

                    if (!foundFolded) //If there arent any folded panels in the list use the flat panels to calculate the even distance
                    {
                        if (maxColWidth < panel.X)
                        {
                            maxColWidth = panel.X;
                        }

                        if (maxRowHeight < panel.Y)
                        {
                            maxRowHeight = panel.Y;
                        }
                    }
                }



                yGrid = Enumerable.Repeat(0.0, maxRow + 1).ToArray();
                xGrid = Enumerable.Repeat(0.0, maxCol + 1).ToArray();

                foreach (FoldedPerforationPanel panel in panelList)
                {
                    for (int i = 0; i < maxCol; i++)
                    {
                        xGrid[i + 1] = xGrid[i] + maxColWidth + panel.colSpacing;

                    }

                    for (int i = 0; i < maxRow; i++)
                    {
                        yGrid[i + 1] = yGrid[i] - maxRowHeight - panel.rowSpacing;
                    }

                    //Add to total Panel Quantity 
                    totalPanelQty = totalPanelQty + panel.PanelQuantity;
                }
                //Check whether the perf value of the first panel is 0 and draw perf on panel is true, if yes set it to 1 by default
                foreach (FoldedPerforationPanel panel in panelList)
                {
                    if (panel.DrawPerf <= 0 && panel.drawPerfOnFirstPanel && !panelList[0].PatternName.Equals("Empty"))
                    {
                        panel.DrawPerf = 1;
                    }
                    materialThickness = panel.SheetThickness + "mm";
                    break;
                }
                createLayers(); //create parent layers and sub layers

                parentlayerIndex = doc.Layers.Find("LAYERS FOR APPROVAL DRAWINGS", true); //get parent layer index
                                                                                          // Draw the sample block
                createSubLayers.createSubLayer("PERF ORIENTATION",
                    System.Drawing.Color.Black, doc.Layers[parentlayerIndex]); //create layer called perf orientation (sublayer)

                RhinoUtilities.SetActiveLayer("PERF ORIENTATION", System.Drawing.Color.Black); //set as current layer
                BoundingBox panelBox = new BoundingBox(-400, 100, 0, -100, 400, 0);
                List<Point3d> rectangle_corners = panelBox.GetCorners().Distinct().ToList();
                // add 1st point at last to close the loop
                rectangle_corners.Add(rectangle_corners[0]);
                Guid sampleBox = doc.Objects.AddPolyline(rectangle_corners);

                doc.Views.Redraw();

                double openAreaDifference = 3;

                if (panelList.Count > 0)
                {
                    if (panelList[0].DrawPerf == 1 && !panelList[0].PatternName.Equals("Empty"))
                    {
                        //Create Temporary Layer
                        RhinoUtilities.SetActiveLayer("TemporaryPerfLayer", System.Drawing.Color.Green);
                        RhinoApp.RunScript("SelAll", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 -" + panelList[0].patternDirection, true);
                        PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(sampleBox).Curve());
                        double openAreaComparison = perforationForm.drawPerforationDesign(panelList[0].PatternName, false);
                        openAreaDifference = openAreaComparison - Convert.ToDouble(panelList[0].PatternOpenArea);

                        if (Math.Abs(openAreaDifference) > 2)
                        {
                            MessageBox.Show("Calculated Open Area: " + openAreaComparison + "  Stored Open Area: " + panelList[0].PatternOpenArea);
                        }
                        RhinoApp.RunScript("SelAll", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 " + panelList[0].patternDirection, true);


                        var rhinoObjects = doc.Objects.FindByLayer("TemporaryPerfLayer");
                        var toolHitObjects = doc.Objects.FindByLayer("TemporaryToolHit");
                        var temporaryTool2Layer = doc.Objects.FindByLayer("TemporaryTool2Layer");

                        //Perf objects 
                        foreach (var rhinObj in rhinoObjects)
                        {
                            rhinObj.Select(true);
                        }
                        if (rhinoObjects != null && rhinoObjects.Length > 0)
                        {
                            if (panelList[0].patternDirection == 1)
                            {
                                RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                                RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                            }
                            RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
                            RhinoApp.RunScript("-_ChangeLayer PERFORATION", true);
                            int index = doc.Layers.Find("TemporaryPerfLayer", true);
                            doc.Layers.Delete(index, true);
                        }

                        //tool hit objects
                        if (toolHitObjects != null)
                        {
                            foreach (var toolhitObj in toolHitObjects)
                            {
                                toolhitObj.Select(true);
                            }

                            if (panelList[0].patternDirection == 1)
                            {
                                RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                                RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                            }
                            RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
                            string command = "-_ChangeLayer " + Properties.Settings.Default.ToolHitLayerName;
                            RhinoApp.RunScript(command, false);
                            int index = doc.Layers.Find("TemporaryToolHit", true);
                            doc.Layers.Delete(index, true);
                        }

                        //Tool 2 objects
                        if (temporaryTool2Layer != null)
                        {
                            foreach (var tool2Objs in temporaryTool2Layer)
                            {
                                tool2Objs.Select(true);
                            }                            
                                if (panelList[0].patternDirection == 1)
                                {
                                    RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                                    RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                                }

                                RhinoUtilities.SetActiveLayer("Tool 2 Layer", System.Drawing.Color.Yellow);
                                RhinoApp.RunScript("-_ChangeLayer Tool 2 Layer", true);
                                int index = doc.Layers.Find("TemporaryTool2Layer", true);
                                doc.Layers.Delete(index, true);

                                doc.Views.Redraw();                            
                        }
                    }

                    double openArea;

                    try
                    {
                        openArea = double.Parse(panelList[0].PatternOpenArea);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Open Area value cannot be empty on SpreadSheet.\nError Column:'RhinoPanelData' DF");
                    }



                    RhinoUtilities.SetActiveLayer("PERF ORIENTATION", System.Drawing.Color.Black);
                    // all non-light objects that are selected
                    var object_enumerator_settings = new ObjectEnumeratorSettings();
                    object_enumerator_settings.IncludeLights = false;
                    object_enumerator_settings.IncludeGrips = true;
                    object_enumerator_settings.NormalObjects = true;
                    object_enumerator_settings.LockedObjects = true;
                    object_enumerator_settings.HiddenObjects = true;
                    object_enumerator_settings.ReferenceObjects = true;
                    object_enumerator_settings.SelectedObjectsFilter = true;
                    var selected_objects = doc.Objects.GetObjectList(object_enumerator_settings);

                    var current_layer_index = doc.Layers.CurrentLayerIndex;

                    foreach (var rhobj in doc.Objects)
                    {
                        if (rhobj.ObjectType == ObjectType.Curve)
                        {
                            Curve curve = rhobj.Geometry as Curve;

                            if (Curve.PlanarClosedCurveRelationship(new Rhino.DocObjects.ObjRef(sampleBox).Curve(), curve, Plane.WorldXY, 0) == RegionContainment.BInsideA)
                            {
                                rhobj.Attributes.LayerIndex = current_layer_index;                                
                                rhobj.CommitChanges();
                            }
                        }
                    }
                    if (panelList[0].DrawPerf == 1 && !panelList[0].PatternName.Equals("Empty"))
                    {
                        string command = "-_ChangeLayer \"PERF ORIENTATION\"";
                        RhinoApp.RunScript(command, true);
                    }

                    // Unselect all objects
                    doc.Objects.UnselectAll();

                    MetrixUtilities.createMetrixBordersDimension();

                    //Create 3rd angle project block
                    Point3d center = new Point3d(216.293, 140.097, 0);
                    Curve circle1 = new ArcCurve(new Circle(center, 23.3 / 2));
                    Curve circle2 = new ArcCurve(new Circle(center, 35.8 / 2));

                    ObjectAttributes attributes = new Rhino.DocObjects.ObjectAttributes();
                    attributes.LinetypeSource = ObjectLinetypeSource.LinetypeFromObject;
                    attributes.LinetypeIndex = RhinoDoc.ActiveDoc.Linetypes.Find("Center", true); //Center linetype

                    RhinoDoc.ActiveDoc.Objects.AddCurve(circle1);
                    RhinoDoc.ActiveDoc.Objects.AddCurve(circle2);
                    doc.Objects.AddLine(new Point3d(189.086, 140.097, 0), new Point3d(293.488, 140.097, 0), attributes);
                    doc.Objects.AddLine(new Point3d(216.293, 165.505, 0), new Point3d(216.293, 112.611, 0), attributes);

                    doc.Objects.AddLine(new Point3d(241.717, 151.761, 0), new Point3d(281.277, 157.998, 0));
                    doc.Objects.AddLine(new Point3d(281.277, 122.196, 0), new Point3d(281.277, 157.998, 0));
                    doc.Objects.AddLine(new Point3d(241.717, 128.433, 0), new Point3d(281.277, 122.196, 0));
                    doc.Objects.AddLine(new Point3d(241.717, 151.761, 0), new Point3d(241.717, 128.433, 0));
                    var dashLineObjects = doc.Objects.FindByLayer("Default");
                    var layerIndex = doc.Layers.Find("PERF ORIENTATION", true);

                    foreach (var rhinObj in dashLineObjects)
                    {
                        rhinObj.Attributes.LayerIndex = layerIndex;
                        rhinObj.CommitChanges();
                    }


                    if (panelList[0].DrawPerf != 3)
                    {
                        openArea = double.Parse(panelList[0].PatternOpenArea);
                        double height = 8;
                        Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(-85, 200 + height, 0);
                        DateTime today = DateTime.Today.Date;
                        string text;
                        if (panelList[0].revision == 0)
                        {
                            text = "Revision: 0" + " (Initial Released) " + today.ToString("dd/MM/yyyy") + "\nPattern: " + panelList[0].PatternName;
                        }
                        else
                        {
                            pt = new Rhino.Geometry.Point3d(-85, 210 + height, 0);
                            text = "Revision: " + panelList[0].revision + " (" + panelList[0].RevisionReason + ") " + today.ToString("dd/MM/yyyy") + "\nRevision: 0" + " (Initial Released) " + panelList[0].FirstRevisionDate + "\nPattern: " + panelList[0].PatternName;
                        }


                        if (panelList[0].patternDirection != 0 && panelList[0].patternDirection != 90)
                        {
                            text += "\nOrientation: Rotated at " + panelList[0].patternDirection + "°";
                        }
                        else if (panelList[0].patternDirection == 0)
                        {
                           
                            text += "\nOrientation: Horizontal";
                        }
                        else
                        {
                            
                            text += "\nOrientation: Vertical";
                        }

                        string font = "Arial";
                        Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                        plane.Origin = pt;
                        Guid id = doc.Objects.AddText(text, plane, height, font, false, false);
                        List<double> boarder = new List<double>();
                        string boarderStr = "";

                        // find all boarder values
                        foreach (FoldedPerforationPanel panel in panelList)
                        {
                            if (!boarder.Contains(panel.LeftBorder))
                            {
                                boarder.Add(panel.LeftBorder);
                            }
                            if (!boarder.Contains(panel.TopBorder))
                            {
                                boarder.Add(panel.TopBorder);
                            }
                            if (!boarder.Contains(panel.BottomBorder))
                            {
                                boarder.Add(panel.BottomBorder);
                            }
                            if (!boarder.Contains(panel.RightBorder))
                            {
                                boarder.Add(panel.RightBorder);
                            }
                        }
                        boarder.Sort();

                        foreach (double boarderValue in boarder)
                        {
                            if (boarderStr.Equals(""))
                            {
                                boarderStr += boarderValue.ToString();
                            }
                            else
                            {
                                boarderStr = boarderStr + "/" + boarderValue.ToString();
                            }
                        }
                        height = 7;

                        string openAreaLabel = "Open Area: %\n";
                        if(openAreaDifference <= 2)
                        {
                            openAreaLabel = "Open Area: " + panelList[0].PatternOpenArea + "%\n";
                        }

                        if (panelList[0].FixingHoles.Equals("0"))
                        {
                            if (panelList[0].PanelType.Equals("Flat"))
                            {
                                text = openAreaLabel + "Borders: " + boarderStr + "mm\nNo Fixing Holes\nTolerances: Linear X.X≤1m ±0.5, X≥1m ±1.0\nAngular X ±1°, X.X ±0.5°";
                                //text = "Open Area: %\nBorders: " + panelList[0].LeftBorder + "mm\nFixing holes: Ø " + panelList[0].HoleDiameter + "mm\nTolerances: Linear X.X≤1m ±0.5, X≥1m ±1.0\nAngular X ±1°, X.X ±0.5°";
                            }
                            else if(panelList[0].PanelType.Equals("Single Folded"))
                            {
                                height -= 1;
                                text = openAreaLabel + "Borders: " + boarderStr + "mm\nNo Fixing Holes\nFolds: " + panelList[0].TopFoldRadius + "mm Internal Radius" + "\nTolerances: Linear X.X≤1m ±0.5, X≥1m ±1.0\nAngular X ±1°, X.X ±0.5°";

                            }
                            pt = new Point3d(-85, 148.241 + height, 0.000);
                            font = "Arial";
                            plane.Origin = pt;
                            id = doc.Objects.AddText(text, plane, height, font, false, false);

                        }

                        else
                        {
                            if(panelList[0].PanelType.Equals("Single Folded"))
                            {
                                height -= 1;
                                text = openAreaLabel + "Borders: " + boarderStr + "mm\nFixing holes: Ø " + panelList[0].HoleDiameter + "mm, at " + panelList[0].DistanceProvided + "mm " + panelList[0].CenterType + " Centres" + "\nFolds: " + panelList[0].TopFoldRadius + "mm Internal Radius" + "\nTolerances: Linear X.X≤1m ±0.5, X≥1m ±1.0\nAngular X ±1°, X.X ±0.5°";
                            }
                            else
                            {
                                text = openAreaLabel + "Borders: " + boarderStr + "mm\nFixing holes: Ø " + panelList[0].HoleDiameter + "mm, at " + panelList[0].DistanceProvided + "mm " + panelList[0].CenterType + " Centres" + "\nTolerances: Linear X.X≤1m ±0.5, X≥1m ±1.0\nAngular X ±1°, X.X ±0.5°";
                            }

                            pt = new Point3d(-85, 148.241 + height, 0.000);
                            font = "Arial";
                            plane.Origin = pt;
                            id = doc.Objects.AddText(text, plane, height, font, false, false);
                        }

                        text = "THIRD ANGLE PROJECTION";
                        height = 5;
                        pt = new Point3d(193.431, 102.399+height, 0.000);
                        font = "Arial";
                        plane.Origin = pt;
                        id = doc.Objects.AddText(text, plane, height, font, false, false);
                    }
                    else
                    {
                        string text = "Solid Panel";
                        double height = 15;
                        Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(-85, 200+height, 0);
                        string font = "Arial";
                        Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                        plane.Origin = pt;
                        Guid id = doc.Objects.AddText(text, plane, height, font, false, false);
                    }
                }

                // Draw the block
                drawBlock(doc, panelList[0], materialThickness);

                MetrixUtilities.createMetrixRealDimension();
                int panelcount = 0;
                bool enablePerf = true;

                try
                {
                    // draw the panel
                    //Check whether the panel is a flat panel or a folded panel,
                    //if it is a folded panel use foldedpaneldrawer class, else use PanelDrawer class
                    foreach (FoldedPerforationPanel panel in panelList)
                    {
                        panelcount++; 
                        if ((panel.PanelType.Equals("Single Folded") || panel.PanelType.Equals("Double Folded")) && !(panel.TopFold==0 && panel.BottomFold==0 && panel.LeftFold==0 && panel.RightFold==0))
                        {
                            doc.Objects.UnselectAll();
                            DoubleFoldedDrawer.drawPanel(xGrid[panel.Column - 1], xGrid[panel.Column], yGrid[panel.Row - 1], yGrid[panel.Row], panel, fixingHolesManipulated);
                        }
                        else
                        {
                            doc.Objects.UnselectAll();
                            //if(panel.PerfOption.Equals("First Panel Only"))
                            //{
                            //    enablePerf = false;
                            //}
                            FlatPanelDrawer.drawPanel(xGrid[panel.Column - 1], xGrid[panel.Column], yGrid[panel.Row - 1], yGrid[panel.Row], panel, fixingHolesManipulated, enablePerf);
                            flatPanelsFound = true; //set flat panels found true for summary creation
                        }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error :" + e.GetBaseException());
                }
                // Select each group and add layout
                for (int i = 0; i < doc.Groups.Count; i++)
                {
                    doc.Objects.UnselectAll();
                    RhinoObject[] rhObjList = doc.Groups.GroupMembers(i);

                    foreach (RhinoObject ro in rhObjList)
                    {
                        ro.Select(true);
                    }

                    AddFoldedFlatLayoutCommand.Instance.createLayout(doc, panelList[i], panelQty);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                //Call Method to create summary page
                if (shouldDrawSummary)
                {
                    AddSummaryLayout.Instance.createSummary(doc, panelList, openAreaDifference);
                }

                for (int i = 0; i < doc.Groups.Count; i++)
                {
                    doc.Groups.Delete(i); //ungroup everthing
                }

                bool askToolHit = true;
                if(panelList[0].PatternName.Equals("Empty"))
                {
                    askToolHit = false;
                }
                if(panelList[0].DrawPerf == 0)
                {
                    askToolHit = false;
                }
                if (panelList[0].PrintPDF == 1)
                {
                    CreatePDFWithWaterMark.Instance.createPDF(doc, askToolHit);
                }

                Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer("PANEL PERIMETER");
                for (int i = 0; i < rhobjs.Length; i++)
                {
                    rhobjs[i].Attributes.DisplayOrder = 1;
                    rhobjs[i].CommitChanges();
                }

                doc.Views.Redraw();
                Rhino.RhinoApp.WriteLine("All Tasks completed Successfully");
            }
            return Result.Success;
        }

        /// <summary>
        /// Draws the block.
        /// </summary>
        private void drawBlock(RhinoDoc doc, FoldedPerforationPanel panelParas, String sheetThickness)
        {
            int parentlayerIndex = doc.Layers.Find("LAYERS FOR APPROVAL DRAWINGS", true);
            createSubLayers.createSubLayer("DETAILS",
               System.Drawing.Color.Black, doc.Layers[parentlayerIndex]); //create layer called details

            // Set the layer
            RhinoUtilities.SetActiveLayer("DETAILS", System.Drawing.Color.Black);
            // Get the location of current API 
            //String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Logo\\MetrixLogo.jpg";
            String path = "Z:\\Automation\\RequiredDraftingDocuments\\MetrixLogo.jpg";
            Plane picture = new Plane(new Point3d(5, 510, 0), new Vector3d(0, 0, 1));

            // Add Company logo.
            doc.Objects.AddPictureFrame(picture, path, false, 62.5, 25, false, false);

            // Draw bottom line
            doc.Objects.AddLine(new Point3d(5, 505, 0), new Point3d(205, 505, 0));

            // Top line
            doc.Objects.AddLine(new Point3d(5, 792, 0), new Point3d(205, 792, 0));

            // Left line
            doc.Objects.AddLine(new Point3d(5, 505, 0), new Point3d(5, 792, 0));

            // Right line
            doc.Objects.AddLine(new Point3d(205, 505, 0), new Point3d(205, 792, 0));

            // Divider line
            doc.Objects.AddLine(new Point3d(5, 510, 0), new Point3d(205, 510, 0));

            // Row 1 line
            doc.Objects.AddLine(new Point3d(67.5, 515, 0), new Point3d(205, 515, 0));

            // Row 2 line
            doc.Objects.AddLine(new Point3d(67.5, 520, 0), new Point3d(205, 520, 0));

            // Row 3 line
            doc.Objects.AddLine(new Point3d(67.5, 525, 0), new Point3d(205, 525, 0));

            // Row 4 line
            doc.Objects.AddLine(new Point3d(67.5, 530, 0), new Point3d(205, 530, 0));

            // Row 5 line
            doc.Objects.AddLine(new Point3d(67.5, 535, 0), new Point3d(205, 535, 0));

            // Vertical divider line
            doc.Objects.AddLine(new Point3d(136.5, 505, 0), new Point3d(136.5, 535, 0));

            // Vertical divider line
            doc.Objects.AddLine(new Point3d(87.5, 510, 0), new Point3d(87.5, 535, 0));

            // Vertical divider line
            doc.Objects.AddLine(new Point3d(156.5, 510, 0), new Point3d(156.5, 535, 0));



            string text = "51 Holloway Drive            metrixgroup.com.au\nBayswater VIC 3153                  1300 792 493";
            double height = 1.8;
            Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(11, 515+height, 0);
            string font = "Arial";
            Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            plane.Origin = pt;
            Guid id = doc.Objects.AddText(text, plane, height, font, false, false);

            height = 1.5;
            text = "Supply of this order will be solely and exlusively according to the terms and conditions of Metrix Group Pty Ltd.";
            plane.Origin = new Point3d(7, 506 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            height = 1.5;
            text = "PANEL";
            plane.Origin = new Point3d(69, 531 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "DRAWN";
            plane.Origin = new Point3d(69, 526 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);
            //Adds the drafter's name
            // text = Environment.UserName;
            text = panelParas.drafterName;
            plane.Origin = new Point3d(89, 526 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "Quantity";
            plane.Origin = new Point3d(69, 521 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "m²";
            plane.Origin = new Point3d(69, 516 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "Page ";
            plane.Origin = new Point3d(69, 511 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "PROJECT";
            plane.Origin = new Point3d(138, 531 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "CUSTOMER ";
            plane.Origin = new Point3d(138, 526 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "JOB NO.";
            plane.Origin = new Point3d(138, 521 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "MATERIAL";
            plane.Origin = new Point3d(138, 516 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "COATING";
            plane.Origin = new Point3d(138, 511 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "Copyright © Metrix Group " + DateTime.Today.Year;
            plane.Origin = new Point3d(138, 506 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            //Add Project name
            height = 1.5;
            text = panelParas.project;
            plane.Origin = new Point3d(158, 531 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            //Add customer name 
            height = 1.5;
            text = panelParas.customerName;
            plane.Origin = new Point3d(158, 526 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            //Add Job No
            height = 1.5;
            text = panelParas.jobNo;
            plane.Origin = new Point3d(158, 521 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            //Add Material
            height = 1.5;
            text = sheetThickness + " " + panelParas.material;
            plane.Origin = new Point3d(158, 516 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);
            if (panelParas.coating.Equals("Mill finish") || panelParas.coating.Equals("Mill Finish"))
            {
                //Add Coating
                height = 1.3;
                text = panelParas.coating;
                plane.Origin = new Point3d(158, 510.81 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);
            }
            else
            {
                //Add Colour 
                height = 1.3;
                text = panelParas.colour;
                plane.Origin = new Point3d(158, 510.81 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);
            }

        }

        /**
         * This method creates the summary page in the rhino file.
         * @panelList - type list containing Foldedperforationpanel, has the information regarding panels
         * @flatPanelsFound - type bool, states if the panel list has flat panels or not
         * @foundFolded - type bool, states if the panel list has Folded panels or not
         * */
        private void CreateSummaryPage(List<FoldedPerforationPanel> panelList, bool flatPanelsFound, bool foundFolded, int panelQty)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            //Add summary page to rhino 
            // Unselect all objects
            doc.Objects.UnselectAll();

            doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;

            RhinoView currentView = doc.Views.ActiveView;
            var pageview = doc.Views.AddPageView(string.Format("{0}", "Summary"), 210, 297);
            Point2d bottomLeft = new Point2d(10, 70);
            Point2d topRight = new Point2d(200, 287);

            if (pageview != null)
            {
                bool isFixingHolesRequired = false;

                //Check if fixing holes are required for panels or not
                foreach (PerforationPanel panel in panelList)
                {
                    if (panel.FixingHoles == "1")
                    {
                        isFixingHolesRequired = true;
                        break;
                    }
                }


                pageview.SetPageAsActive();
                doc.Objects.UnselectAll();
                doc.Views.ActiveView = pageview;


                //Add the text
                string text = "";
                double height = 1.8;
                string font = "Arial";
                Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(11, 100 + height, 0);
                Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                plane.Origin = pt;
                Guid id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Customer : ";
                plane.Origin = new Point3d(7, 260 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Customer Name 
                height = 5.0;
                text = panelList[0].customerName;
                plane.Origin = new Point3d(50, 260 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Project : ";
                plane.Origin = new Point3d(7, 250 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add project name
                height = 5.0;
                text = panelList[0].project;
                plane.Origin = new Point3d(50, 250 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Customer Purchase Order Number : ";
                plane.Origin = new Point3d(7, 240 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Customer order purchase number
                height = 5.0;
                text = panelList[0].CustomerOrderNo;
                plane.Origin = new Point3d(135, 240 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Metrix Part Number : ";
                plane.Origin = new Point3d(7, 230 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Metrix Part Number 
                height = 5.0;
                text = panelList[0].MetrixPartNo;
                plane.Origin = new Point3d(82, 230 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);


                height = 5.5;
                text = "Metrix Sales Order Number : ";
                plane.Origin = new Point3d(7, 220 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Metrix Sales order number
                height = 5.0;
                text = panelList[0].MetrixSalesNo;
                plane.Origin = new Point3d(107, 220 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Metrix Job Number : ";
                plane.Origin = new Point3d(7, 210 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Metrix Job Number
                height = 5.0;
                text = panelList[0].jobNo;
                plane.Origin = new Point3d(82, 210 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Description : ";
                plane.Origin = new Point3d(7, 200 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Description 
                height = 5.0;
                text = $"{panelList[0].SheetThickness}mm / {panelList[0].material}";
                plane.Origin = new Point3d(50, 200 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Pattern : ";
                plane.Origin = new Point3d(7, 190 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Pattern
                height = 5.0;
                text = panelList[0].PatternName;
                plane.Origin = new Point3d(40, 190 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Open Area : ";
                plane.Origin = new Point3d(7, 180 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add open area
                height = 5.0;
                text = panelList[0].PatternOpenArea;
                plane.Origin = new Point3d(50, 180 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Dot Fonts : ";
                plane.Origin = new Point3d(7, 170 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add dot fonts
                height = 5.0;
                text = panelList[0].dotFont == 1 ? "Yes" : "No";
                plane.Origin = new Point3d(50, 170 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Panel Types : ";
                plane.Origin = new Point3d(7, 160 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Panel Types
                height = 5.0;
                if (flatPanelsFound && foundFolded)
                {
                    text = "Flat Panels and Folded Panels";
                }
                else
                {
                    if (flatPanelsFound && !foundFolded)
                    {
                        text = "Flat Panels only";
                    }
                    else
                    {
                        text = "Folded Panels only";
                    }
                }
                plane.Origin = new Point3d(55, 160 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Fixing Holes : ";
                plane.Origin = new Point3d(7, 150 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add fixing holes
                height = 5.0;
                text = isFixingHolesRequired ? "Yes" : "No";
                plane.Origin = new Point3d(55, 150 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Coating : ";
                plane.Origin = new Point3d(7, 140 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add Coating
                height = 5.0;
                text = panelList[0].coating;
                plane.Origin = new Point3d(50, 140 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);


                height = 5.5;
                text = "Total Quantity of Panels : ";
                plane.Origin = new Point3d(7, 130 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Total panel quantity
                height = 5.0;
                text = panelQty.ToString();
                plane.Origin = new Point3d(95, 130 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                height = 5.5;
                text = "Total SQM of Panels : ";
                plane.Origin = new Point3d(7, 120 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);

                //Add total sqm of panels
                height = 5.0;
                text = panelList[0].TotalPanelSQM.ToString();
                plane.Origin = new Point3d(85, 120 + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);
            }
        }

        /**
         * Method creates parent layers and some sub layers of the parent layers 
         * (toolhit, cluster sample and perforation layer)
         * */
        private void createLayers()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            String layerName = null;
            String apporvalParentLayer = "LAYERS FOR APPROVAL DRAWINGS"; //name of approval layer/parent layer name
            int layerIndex = 0;
            int parentLayerIndex = 0;
            Rhino.DocObjects.Layer parent_layer;

            //Create Parent Layer Nesting
            RhinoUtilities.SetActiveLayer("LAYERS FOR NESTING", System.Drawing.Color.Black);

            //create parent layer Approval
            RhinoUtilities.SetActiveLayer(apporvalParentLayer, System.Drawing.Color.Black);
            parentLayerIndex = doc.Layers.Find(apporvalParentLayer, true);
            parent_layer = doc.Layers[parentLayerIndex];

            layerName = "CLUSTER SAMPLE";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.Delete(layerIndex, true);
            layerIndex = createSubLayers.createSubLayer(layerName,
              System.Drawing.Color.DarkOrange, parent_layer);

            layerName = "PERFORATION";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.Delete(layerIndex, true);
            layerIndex = createSubLayers.createSubLayer(layerName,
            System.Drawing.Color.Green, parent_layer);

            layerName = "LAYOUT DETAILS";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.Delete(layerIndex, true);
            layerIndex = createSubLayers.createSubLayer(layerName,
            System.Drawing.Color.Black, parent_layer);


            parentLayerIndex = doc.Layers.Find("LAYERS FOR NESTING", true);
            parent_layer = doc.Layers[parentLayerIndex];


            layerName = "TOOL HIT";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.Delete(layerIndex, true);
            layerIndex = createSubLayers.createSubLayer(layerName,
              System.Drawing.Color.Black, parent_layer);
        }



    }
}
