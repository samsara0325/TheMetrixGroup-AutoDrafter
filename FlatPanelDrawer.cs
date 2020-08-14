using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.IO;
using MetrixGroupPlugins.Utilities;
using MetrixGroupPlugins.Commands;
using MetrixGroupPlugins.CustomFixingHole;
using System.Windows.Forms;
namespace MetrixGroupPlugins
{
    public static class FlatPanelDrawer
    {

        /// <summary>
        /// Draws the panel.
        /// </summary>
        /// <param name="xLowerBound">The x lower bound.</param>
        /// <param name="xUpperBound">The x upper bound.</param>
        /// <param name="yLowerBound">The y lower bound.</param>
        /// <param name="yUpperBound">The y upper bound.</param>
        /// <param name="panel">The panel.</param>
        /// <param name="para">The para.</param>
        public static void drawPanel(double xLowerBound, double xUpperBound, double yLowerBound, double yUpperBound, PerforationPanel panel, bool fixingHolesManipulated, bool enablePerf)
        {

            RhinoDoc doc = RhinoDoc.ActiveDoc;
            List<Guid> guidList = new List<Guid>();
            string layerName = null; //name of layers
            int layerIndex = 0; //index of layers
            Rhino.DocObjects.Layer parent_layer_Approval = null; //create variable to hold approval layer 
            Rhino.DocObjects.Layer parent_layer_Nesting = null; //create variable to hold nesting layer
                                                                //Rhino.DocObjects.Layer childlayer = null; //Create a variable to hold child layers
            string text = "";
            double height = panel.labelHeight / 3;
            const string font = "Arial";
            Guid burrLeader;
            RhinoObject labelText;
            Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(0, 0, 0);
            Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();

            //start
            //Creating layer called "Layers for Approval Drawings" to make it a parent layer 
            layerName = "LAYERS FOR APPROVAL DRAWINGS";
            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
                parent_layer_Approval = doc.Layers[layerIndex]; //set the layer as parent layer
            }
            else
            {
                parent_layer_Approval = doc.Layers[layerIndex];
            }

            layerName = "LAYERS FOR NESTING";

            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
                parent_layer_Nesting = doc.Layers[layerIndex];
            }
            else
            {
                parent_layer_Nesting = doc.Layers[layerIndex];
            }


            // Create a new layer called Perimeter 
            layerName = "PANEL PERIMETER";

            layerIndex = createSubLayers.createSubLayer(layerName,
              System.Drawing.Color.Black, parent_layer_Nesting); //make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //Bottom and left justified the panels in the grid (panel x0,x1,y0,y1 - refers to the folds edg (folds layer)
            double panelX0 = xLowerBound;
            double panelX1 = panelX0 + panel.X;
            double panelY0 = yUpperBound;
            double panelY1 = panelY0 + panel.Y;

            List<Point3d> list = new List<Point3d>();

            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1, 0), new Point3d(panelX1, panelY1, 0));
            guidList.Add(panel.Perimeter);

            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX0, panelY1, 0));
            guidList.Add(panel.Perimeter);

            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX1, panelY0, 0));
            guidList.Add(panel.Perimeter);

            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0, 0), new Point3d(panelX1, panelY1, 0));
            guidList.Add(panel.Perimeter);

            //MetrixUtilities.joinCurves(doc.Layers.Find("PANEL PERIMETER", true)); //join the closed curves using the method


            //Calculating the borders
            double borderX0 = panelX0 + panel.LeftBorder;  //refers to the borders corners
            double borderY0 = panelY0 + panel.BottomBorder;
            double borderX1 = panelX1 - panel.RightBorder;
            double borderY1 = panelY1 - panel.TopBorder;

            BoundingBox panelBox = new BoundingBox(borderX0, borderY0, 0, borderX1, borderY1, 0);
            List<Point3d> rectangle_corners = panelBox.GetCorners().Distinct().ToList();
            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            // Create a new layer called Border

            layerName = "BORDERS";
            layerIndex = createSubLayers.createSubLayer(layerName,
                   System.Drawing.Color.Purple, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            //Add the borders only if the panel is not solid
            if (panel.DrawPerf != 3)
            {
                //Create a bounding box for the borders       
                panel.Border = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Border);
            }

            // Create a new layer called LABELS
            layerName = "LABELS";
            layerIndex = createSubLayers.createSubLayer(layerName,
               System.Drawing.Color.Red, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            text = panel.PartName;
            height = panel.labelHeight;
            pt = new Rhino.Geometry.Point3d(borderX0, borderY0 + 4 + height, 0);
            plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            plane.Origin = pt;
            panel.Label = doc.Objects.AddText(text, plane, height, font, false, false);
            guidList.Add(panel.Label);

            RhinoApp.RunScript("SelNone", true);
            labelText = doc.Objects.Find(panel.Label);
            labelText.Select(true);
            BoundingBox bbox = labelText.Geometry.GetBoundingBox(true);
            double minX = bbox.Corner(true, true, true).X;
            double maxX = bbox.Corner(false, true, true).X;
            double minY = bbox.Corner(true, true, true).Y;
            double maxY = bbox.Corner(true, false, true).Y;

            if (maxX - minX >= panel.X - panel.LeftBorder - panel.RightBorder)
            {
                double ratio = 1;
                labelText.Select(true);
                if (panel.Y > panel.X)
                {
                    RhinoApp.RunScript("_-rotate " + bbox.Center.X + "," + bbox.Center.Y + " " + "90", true);
                }

                if (maxY - minY + 4 >= panel.X - panel.LeftBorder - panel.RightBorder)
                {
                    ratio = (panel.X - panel.LeftBorder - panel.RightBorder) / (2 * (maxY - minY));
                    if (ratio * (maxX - minX) >= (panel.Y - panel.TopBorder - panel.BottomBorder))
                    {
                        ratio = ratio * (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * ratio * (maxX - minX));
                    }
                }
                else if(maxX - minX >= panel.Y - panel.TopBorder - panel.BottomBorder)
                {
                    ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxX - minX));
                }
                RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                BoundingBox bbox3 = labelText.Geometry.GetBoundingBox(true);
                double distance1 = borderX0 + ratio * (bbox3.Max.X - bbox3.Min.X) / 2;
                double distance2 = borderY0 + ratio * (bbox3.Max.Y - bbox3.Min.Y) / 2;
                if (panel.Y > panel.X)
                {
                     distance1 = borderX0 + ratio * (bbox3.Max.Y - bbox3.Min.Y) / 2;
                     distance2 = borderY0 + ratio * (bbox3.Max.X - bbox3.Min.X) / 2;
                }

                RhinoApp.WriteLine(bbox3.Center.ToString());
                RhinoApp.RunScript("_-Move " + bbox3.Center.X + "," + bbox3.Center.Y + ",0 " + distance1 + "," + distance2 + ",0", true);


            }
            else if (maxY - minY >= panel.Y - panel.TopBorder - panel.BottomBorder)
            {
                double ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxY - minY));
                labelText.Select(true);
                RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                double distanceX = borderX0 + ratio * (bbox2.Center.X - bbox2.Min.X) / 2;
                double distanceY = panelBox.Min.Y + ratio * (bbox2.Center.Y - bbox.Min.Y) / 2;

                RhinoApp.WriteLine(bbox2.Center.ToString());
                RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
            }
            labelText.Select(false);

            // If dotFontLabel is more than 0 draw the dot font text on the panel, else skip
            // if (para.DotFont == 1)
            if (panel.DotFontLabel > 0)
            {
                // Create a new layer called DOT SCRIBE LABEL
                layerName = "DOT SCRIBE LABEL";

                layerIndex = createSubLayers.createSubLayer(layerName,
                 System.Drawing.Color.Black, parent_layer_Nesting); //make Nesting layer the parent layer


                doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                // Put in the Dot Matrix Label
                // Draw at the right side of the border aand 10mm from the bottom and 100mm from the left edge
                double panelOffset = 0;
                if (panel.BottomBorder - 8.7 <= 6)
                {
                    panelOffset = (panel.BottomBorder - 8.7) / 2;
                }
                else
                {
                    panelOffset = 3.1;
                }
                if (panel.X < 160)
                {
                    pt = new Point3d(1 * (panelX0 + panelX1) / 2, panelY0 + panelOffset + 8.7, 0);
                }
                else
                {
                    pt = new Point3d(panelX1 - 100, panelY0 + panelOffset + 8.7, 0);
                }
                if (panel.DotFontLabellerSide.Equals("Rear"))
                {
                    DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, 8.7, panel.X); //set the size of dotfont 
                }
                else //If not revered use front labeller
                {
                    DotMatrixFrontLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, 8.7);
                }
            }
            //checks whether the perforation layers exists, if not create layer and make Approval layer the parent layer
            //If exists, make Approval layer the parent layer
            layerName = "PERFORATION";
            layerIndex = createSubLayers.createSubLayer(layerName,
               System.Drawing.Color.Green, parent_layer_Approval);

            // doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //Create Temporary Layer

            if (panel.DrawPerf == 1)
            {
                RhinoUtilities.SetActiveLayer("TemporaryPerfLayer", System.Drawing.Color.Green);
                doc.Views.Redraw();

                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 -" + panel.patternDirection, true);
                PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(panel.Border).Curve());
                perforationForm.enablePerforation = enablePerf;
                perforationForm.drawPerforationDesign(panel.PatternName, true, enablePerf);
                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 " + panel.patternDirection, true);
                RhinoApp.RunScript("SelNone", true);

                var rhinoObjects = doc.Objects.FindByLayer("TemporaryPerfLayer");
                var toolHitObjects = doc.Objects.FindByLayer("TemporaryToolHit");
                var temporaryTool2Layer = doc.Objects.FindByLayer("TemporaryTool2Layer");

                //Perf objects 
                if (rhinoObjects != null && rhinoObjects.Length > 1)
                {
                    foreach (var rhinObj in rhinoObjects)
                    {
                        rhinObj.Select(true);
                    }
                    if (panel.patternDirection == 1)
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
                if (toolHitObjects != null && toolHitObjects.Length>1) {                 
                    foreach (var toolhitObj in toolHitObjects)
                {
                    toolhitObj.Select(true);
                }
               
                    if (panel.patternDirection == 1)
                    {
                        RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                    }
                    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
                    RhinoApp.RunScript("-_ChangeLayer TOOL HIT", true);
                    int index = doc.Layers.Find("TemporaryToolHit", true);
                    doc.Layers.Delete(index, true);
                }

                //Tool 2 objects
                if (temporaryTool2Layer != null && temporaryTool2Layer.Length>1)
                {
                    foreach (var tool2Objs in temporaryTool2Layer)
                {
                    tool2Objs.Select(true);
                }
                    if (panel.patternDirection == 1)
                    {
                        RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                    }

                    RhinoUtilities.SetActiveLayer("Tool 2 Layer", System.Drawing.Color.Yellow);
                    RhinoApp.RunScript("-_ChangeLayer Tool 2 Layer", true);
                    int index = doc.Layers.Find("TemporaryTool2Layer", true);
                    doc.Layers.Delete(index, true);
                }
                doc.Views.Redraw();
            }


            DimensionStyle dimStyle = MetrixUtilities.createMetrixRealDimension(); //sets the metrix real dimension

            Point3d origin = new Point3d(0, 0, 0);
            Point3d offset = new Point3d(0, 0, 0);
            Point2d ext1;
            Point2d ext2;
            Point2d linePt;
            LinearDimension dimension;
            Guid dimGuid = new Guid();
            double u, v;


            // Create a new layer called DIMENSIONS BLACK
            layerName = "DIMENSIONS BLACK";
            layerIndex = createSubLayers.createSubLayer(layerName,
                    System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Add the word perforated area to the panel
            if (panel.DrawPerf != 3) //Add the text only if the panel is not a solid panel
            {
                //pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 33, 0);
                text = System.Text.RegularExpressions.Regex.Unescape(panel.PerfText);
                height = panel.labelHeight / 2;
                pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 10 + height, 0);
                plane.Origin = pt;
                Guid perforatedAreaLabel = doc.Objects.AddText(text, plane, height, font, false, false);
                guidList.Add(perforatedAreaLabel);

                double ratio = 1;
                
                if (panel.X - panel.LeftBorder - panel.RightBorder < 230)
                {
                    RhinoApp.RunScript("SelNone", true);
                    labelText = doc.Objects.Find(perforatedAreaLabel);
                    labelText.Select(true);
                    bbox = labelText.Geometry.GetBoundingBox(true);
                    if (panel.Y > panel.X)
                    {
                        RhinoApp.RunScript("_-rotate " + bbox.Center.X + "," + bbox.Center.Y + " " + "90", true);
                    }

                    minX = bbox.Corner(true, true, true).X;
                    maxX = bbox.Corner(false, true, true).X;
                    minY = bbox.Corner(true, true, true).Y;
                    maxY = bbox.Corner(true, false, true).Y;

                    if (maxY - minY > panel.X - panel.LeftBorder - panel.RightBorder)
                    {
                        ratio = (panel.X - panel.LeftBorder - panel.RightBorder) / (2 * (maxY - minY));
                        if (ratio * (maxX - minX) > (panel.Y - panel.TopBorder - panel.BottomBorder))
                        {
                            ratio = ratio * (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * ratio * (maxX - minX));
                        }
                    }
                    else if (maxX - minX >= panel.Y - panel.TopBorder - panel.BottomBorder)
                    {
                        ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxX - minX));
                    }
                    labelText.Select(true);
                    RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                    BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                    double distanceX = (borderX0 + borderX1) / 2;
                    double distanceY = (borderY0 + borderY1) / 2;

                    RhinoApp.WriteLine(bbox2.Center.ToString());
                    RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
                }
                else
                {
                    RhinoApp.RunScript("SelNone", true);
                    labelText = doc.Objects.Find(perforatedAreaLabel);
                    labelText.Select(true);
                    bbox = labelText.Geometry.GetBoundingBox(true);
                    minX = bbox.Corner(true, true, true).X;
                    maxX = bbox.Corner(false, true, true).X;
                    minY = bbox.Corner(true, true, true).Y;
                    maxY = bbox.Corner(true, false, true).Y;

                    if (maxX - minX > panel.Y - panel.TopBorder - panel.BottomBorder)
                    {
                        ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxY - minY));
                        labelText.Select(true);
                        RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                    }
                    BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                    double distanceX = (borderX0 + borderX1) / 2;
                    double distanceY = (borderY0 + borderY1) / 2;

                    RhinoApp.WriteLine(bbox2.Center.ToString());
                    RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
                }
            }

            // Add horizontal dimension
            origin = new Point3d(panelX1, panelY0 - 50, 0);
            offset = new Point3d(panelX0, panelY0 - 50, 0);
            pt = new Point3d((offset.X - origin.X) / 2, panelY0 - 100, 0);

            plane = Plane.WorldXY;
            plane.Origin = origin;

            //double u, v;
            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);


            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);         //add the bottom dimension(vertical line)
            guidList.Add(dimGuid);

            // Add vertical dimension for panel

            origin = new Point3d(panelX0 - 20, panelY0, 0);
            offset = new Point3d(panelX0 - 20, panelY1, 0);
            pt = new Point3d(panelX0 - 100, (offset.Y - origin.Y) / 2, 0);

            plane = Plane.WorldXY;
            plane.XAxis = new Vector3d(0, -1, 0);
            plane.YAxis = new Vector3d(-1, 0, 0);
            plane.ZAxis = new Vector3d(0, 0, -1);
            plane.Origin = origin;

            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);

            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);  //adds the left dimension

            guidList.Add(dimGuid);


            MetrixUtilities.createMetrixBordersDimension(); //sets the dimension style for borders

            // Draw Border dimension on BORDERS layer
            if (panel.DrawPerf != 3) ///Add only if the panel is not a solid panel
            {
                layerName = "BORDERS";
                layerIndex = doc.Layers.Find(layerName, true);
                doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                // Add horizontal borders dimension
                origin = new Point3d(panelX1, (panelY0 + panelY1) / 2, 0);
                offset = new Point3d(borderX1, (panelY0 + panelY1) / 2, 0);
                pt = new Point3d((offset.X - origin.X) / 2, (borderY0 + borderY1) / 2, 0);

                plane = Plane.WorldXY;
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                // Add horizontal borders dimension
                origin = new Point3d(panelX0, (panelY0 + panelY1) / 2 , 0);
                offset = new Point3d(borderX0, (panelY0 + panelY1) / 2 , 0);
                pt = new Point3d((offset.X - origin.X) / 2, (borderY0 + borderY1) / 2, 0);


                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                // Add vertical border dimension for panel

                origin = new Point3d((panelX0 + panelX1) / 2 , panelY0, 0);
                offset = new Point3d((panelX0 + panelX1) / 2 , borderY0, 0);
                pt = new Point3d((borderX0 + borderX1) / 2, (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                origin = new Point3d((panelX0 + panelX1) / 2 , panelY1, 0);
                offset = new Point3d((panelX0 + panelX1) / 2 , borderY1, 0);
                pt = new Point3d((borderX0 + borderX1) / 2, (offset.Y - origin.Y) / 2, 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);
            }

            MetrixUtilities.createMetrixRealDimension(); //sets the default dimension style
            layerName = "VIEWPORT";
            layerIndex = createSubLayers.createSubLayer(layerName,
                               System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);



            Rhino.DocObjects.RhinoObject label = doc.Objects.Find(panel.Label);
            string exportFileName = "1";

            if (label != null)
            {
                label.Select(true);
                Rhino.Geometry.TextEntity textentity = label.Geometry as Rhino.Geometry.TextEntity;
                exportFileName = textentity.Text + ".dxf";
            }


            /**
             * Checks if the dxf files are required by the user, if yes check whether the panel is perforated
             * using the drawPerf property in the panel. If it is a perforated panel then check if the directory
             * for perforated panels dxf files already exists, if does not exist create directory and run command.
             * If panel is not perforated, create directory to save not perforated panels dxf files if the directory
             * does not exist. Then run the dxf file create command. 
             * */
            if (panel.DXFFilesRequired.Equals("Yes"))
            {
                String path;
                String immediateFolderName = Path.GetFileName(Path.GetDirectoryName(doc.Path)); //get the immediate foldername which the file is located in 
                                                                                                //split the path to get the parent folder. 
                String[] newPath = MetrixUtilities.splitString(Path.GetDirectoryName(doc.Path), immediateFolderName);
                if (panel.DrawPerf == 1) //checks if panel is perforated 
                {
                    path = newPath[0] + ("5TRUMPF") + ("\\WITH PERF"); //merge path for perforated files
                    if (!Directory.Exists(path)) //check if directory already exists
                    {
                        System.IO.Directory.CreateDirectory(path); //create directory if not exist
                    }
                }
                else
                {
                    path = newPath[0] + ("5TRUMPF") + ("\\NO PERF"); //merge path for not perforated files
                    if (!Directory.Exists(path)) //check if directory already exists 
                    {
                        System.IO.Directory.CreateDirectory(path); //create directory if not exist
                    }
                }
                string command = string.Format("-_Export \"" + path + @"\" + exportFileName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
                // Export the selected curves
                RhinoApp.RunScript(command, true);
            }
            // Unselect all objects
            doc.Objects.UnselectAll();

            // Default layer index
            int defaultLayerIndex = doc.Layers.Find("Default", true);

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            ////if draw perf is false, turnoff the toolhit layer
            //if (Convert.ToBoolean(panel.DrawPerf) != true)
            //{
            //   layerName = "Tool Hit";
            //   layerIndex = doc.Layers.Find(layerName, true);
            //   doc.Layers[layerIndex].IsVisible = false;
            //}



            MetrixUtilities.joinCurves(doc.Layers.Find("PANEL PERIMETER", true));
            if (panel.FixingHoles == "1")
            {
                //if fixing holes are not manipulated, recalculate distances
                if (!fixingHolesManipulated)
                {
                    //panel = reCalculateDistances(panel);

                    //Below method is if fixing holes are automated
                    guidList = FixingHoles.drawFixingFoles(panel, null, false, 0, 0, panelY0, panelY1, dimStyle, guidList, panelX0, panelX1, 0, 0, 0, 0, 0); //add fixing holes
                }
                else
                {
                    //Below method is if fixing holes  have been manipulated
                    guidList = CustomFixingHoles.drawFixingFoles(panel, null, false, 0, 0, panelY0, panelY1, dimStyle, guidList, panelX0, panelX1, 0, 0, 0, 0, 0); //add fixing holes
                }
            }
            layerName = "VIEWPORT";
            layerIndex = createSubLayers.createSubLayer(layerName,
                               System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            foreach (Guid g in guidList)
            {
                int idx = RhinoDoc.ActiveDoc.Groups.Find(panel.PartName, false);

                if (idx < 0)
                {
                    idx = RhinoDoc.ActiveDoc.Groups.Add(panel.PartName);
                }

                RhinoDoc.ActiveDoc.Groups.AddToGroup(idx, g);
            }

            //end
        }

        //Method will recalculate the distances for the top and bottom folds 
        public static PerforationPanel reCalculateDistances(PerforationPanel panel)
        {
            int TopFixingHoleQuantity = 0;
            double TopFixingHoleDistance = 0;
            int BottomFixingHoleQuantity = 0;
            double BottomFixingHoleDistance = 0;
            int LeftFixingHoleQuantity = 0;
            double LeftFixingHoleDistance = 0;
            int RightFixingHoleQuantity = 0;
            double RightFixingHoleDistance = 0;


            if (panel.TopFixingHoles == "1")
            {
                //recalculate top fixing hole quantity and distance
                TopFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight) / panel.DistanceProvidedTop) + 1);
                TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight) / (TopFixingHoleQuantity)));

                //if the top fixing hole distance is more than the maximum provided distance recalculate 
                while (checkDistanceWIthTolerance(TopFixingHoleDistance, panel.DistanceProvidedTop))
                {
                    TopFixingHoleQuantity = TopFixingHoleQuantity + 1;
                    TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight) / (TopFixingHoleQuantity)));
                }
            }

            if (panel.BottomFixingHoles == "1")
            {
                //recalculate bottom fixing hole quantity and distance
                BottomFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight) / panel.DistanceProvidedBottom) + 1);
                BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight) / (BottomFixingHoleQuantity)));

                //if the bottom fixing hole distance is more than the maximum provided distance recalculate 
                while (checkDistanceWIthTolerance(BottomFixingHoleDistance, panel.DistanceProvidedBottom))
                {
                    BottomFixingHoleQuantity = BottomFixingHoleQuantity + 1;
                    BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight) / BottomFixingHoleQuantity));
                }
            }



            if (panel.LeftFixingHoles == "1")
            {
                //new
                //recalculate Left fixing hole quantity and distance
                LeftFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom) / panel.DistanceProvidedLeft) + 1);
                LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom) / (LeftFixingHoleQuantity)));

                //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                while (checkDistanceWIthTolerance(LeftFixingHoleDistance, panel.DistanceProvidedLeft))
                {
                    LeftFixingHoleQuantity = LeftFixingHoleQuantity + 1;
                    LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom) / (LeftFixingHoleQuantity)));
                }
            }


            if (panel.RightFixingHoles == "1")
            {
                //recalculate Right fixing hole quantity and distance
                RightFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom) / panel.DistanceProvidedLeft) + 1);
                RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom) / (RightFixingHoleQuantity)));


                //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                while (checkDistanceWIthTolerance(RightFixingHoleDistance, panel.DistanceProvidedRight))
                {
                    RightFixingHoleQuantity = RightFixingHoleQuantity + 1;
                    RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom) / (RightFixingHoleQuantity)));
                }
            }

            //Check for excel sheet calcualted distances
            if (panel.TopFixingHoleDistance > panel.DistanceProvidedTop)
            {
                panel.TopFixingHoleDistance = TopFixingHoleDistance;
                panel.TopFixingHoleQuantity = TopFixingHoleQuantity;
            }
            if (panel.BottomFixingHoleDistance > panel.DistanceProvidedBottom)
            {
                panel.BottomFixingHoleDistance = BottomFixingHoleDistance;
                panel.BottomFixingHoleQuantity = BottomFixingHoleQuantity;
            }
            if (panel.LeftFixingHoleDistance > panel.DistanceProvidedLeft)
            {
                panel.LeftFixingHoleDistance = LeftFixingHoleDistance;
                panel.LeftFixingHoleQuantity = LeftFixingHoleQuantity;
            }
            if (panel.RightFixingHoleDistance > panel.DistanceProvidedRight)
            {
                panel.RightFixingHoleDistance = RightFixingHoleDistance;
                panel.RightFixingHoleQuantity = RightFixingHoleQuantity;
            }

            return panel;
        }
        //method used to check whether the calculated distance between fixing holes is greater than the provided maximum distance
        private static Boolean checkDistanceWIthTolerance(double calculatedDistance, double providedDistance)
        {
            if (calculatedDistance > providedDistance)  //if the yes return true
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