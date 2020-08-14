using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.IO;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// 
   /// </summary>
   public static class PanelDrawer  
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
      public static void drawPanel(double xLowerBound, double xUpperBound, double yLowerBound, double yUpperBound, PerforationPanel panel, PanelParameters para)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<Guid> guidList = new List<Guid>();         

         // Create a new layer called Panel Perimeter
         string layerName = "PANEL PERIMETER";

         // Does a layer with the same name already exist?
         int layerIndex = doc.Layers.Find(layerName, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
         }

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         // Bottom and left justified the panels in the grid
         double panelX0 = xLowerBound;
         double panelX1 = panelX0 + panel.X;
         double panelY0 = yUpperBound;
         double panelY1 = panelY0 + panel.Y;

         // Position the panel to be in the middle of the grid
         //double panelX0 = xLowerBound + ((xUpperBound - xLowerBound) - panel.X) /2 ;
         //double panelX1 = panelX0 + panel.X;
         //double panelY1 = yLowerBound  - ((yLowerBound - yUpperBound) - panel.Y) / 2;
         //double panelY0 = panelY1 - panel.Y;

         BoundingBox panelBox = new BoundingBox(panelX0, panelY0, 0,panelX1, panelY1, 0);
         List<Point3d>  rectangle_corners = panelBox.GetCorners().Distinct().ToList();
         // add 1st point at last to close the loop
         rectangle_corners.Add(rectangle_corners[0]);
         panel.Perimeter = doc.Objects.AddPolyline(rectangle_corners);

         guidList.Add(panel.Perimeter);

         // Create a new layer called Border
         layerName = "BORDERS";

         // Does a layer with the same name already exist?
         layerIndex = doc.Layers.Find(layerName, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Purple);
         }

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         double borderX0 = panelX0 + panel.LeftBorder;
         double borderY0 = panelY0 + panel.BottomBorder;
         double borderX1 = panelX1 - panel.RightBorder;
         double borderY1 = panelY1 - panel.TopBorder;

         panelBox = new BoundingBox(borderX0, borderY0, 0, borderX1, borderY1, 0);
         rectangle_corners = panelBox.GetCorners().Distinct().ToList();
         // add 1st point at last to close the loop
         rectangle_corners.Add(rectangle_corners[0]);
         panel.Border = doc.Objects.AddPolyline(rectangle_corners);

         guidList.Add(panel.Border);

         // Create a new layer called LABELS
         layerName = "LABELS";

         // Does a layer with the same name already exist?
         layerIndex = doc.Layers.Find(layerName, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Red);
         }

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(borderX0, borderY0, 0);
         string text = panel.PartName;
         double height = para.LabelHeight;
         const string font = "Arial";
         Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
         plane.Origin = pt;
         panel.Label = doc.Objects.AddText(text, plane, height, font, false, false);
         guidList.Add(panel.Label);

         // If Dot font needs to be drawn
         if (para.DotFont == 1)
         {
            // Create a new layer called DOTS
            layerName = "DOTS";

            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
               // Add a new layer to the document
               layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
            }

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Put in the Dot Matrix Label
            // Draw at the right side of the border aand 10mm from the bottom and 100mm from the left edge
            pt = new Point3d(panelX1 - 100, panelY0 + 5, 0);
            DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, Properties.Settings.Default.DotMatrixHeight, panel.X);
         }

         if(para.PatternDirection == 1)
         {
            // Export the selected curves
            RhinoApp.RunScript("SelAll", true);
            RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
         }

         PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(panel.Border).Curve());
         perforationForm.drawPerforationDesign(panel.PatternName, true);

         if (para.PatternDirection == 1)
         {
            // Export the selected curves
            RhinoApp.RunScript("SelAll", true);
            RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
         }
         
         // Draw Dimension
         // Find the Dimension Style

         // Create a new layer called DIMENSION
         layerName = "DIMENSION";

         // Does a layer with the same name already exist?
         layerIndex = doc.Layers.Find(layerName, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
         }

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         // Add the word perforated area to the panel
         pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 33, 0);
         text = "PERFORATED \nAREA";
         height = para.LabelHeight / 2;
         plane.Origin = pt;
         Guid perforatedAreaLabel = doc.Objects.AddText(text, plane, height, font, false, false);

         guidList.Add(perforatedAreaLabel);

         DimensionStyle dimStyle = doc.DimStyles.Find("Metrix Real", true);
         int dimStyleIndex = 0;

         if (dimStyle == null)
         {
            dimStyleIndex = doc.DimStyles.Add("Metrix Real");
            dimStyle = doc.DimStyles.Find("Metrix Real", true);
         }
         else
         {
            dimStyleIndex = dimStyle.Index;
         }

         dimStyle.TextHeight = 40;
         dimStyle.TextGap = 25;
         dimStyle.ExtensionLineExtension = 25;
         dimStyle.ExtensionLineOffset = 25;
         dimStyle.LengthResolution = 0;
         dimStyle.AngleResolution = 0;

         dimStyle.ArrowLength = 25;
         dimStyle.LeaderArrowLength = 25;
         dimStyle.FitText = DimensionStyle.TextFit.TextInside;
            dimStyle.FixedExtensionLength = 25;
            dimStyle.FixedExtensionOn = true;
            dimStyle.DimRadialTextLocation = DimensionStyle.TextLocation.AboveDimLine;
            doc.DimStyles.Modify(dimStyle, dimStyleIndex, false);
            dimStyle.LeaderTextVerticalAlignment = TextVerticalAlignment.Middle;
        doc.DimStyles.SetCurrent(dimStyleIndex, false);

         // Add horizontal dimension
         Point3d origin = new Point3d(panelX1, panelY0, 0);
         Point3d offset = new Point3d(panelX0, panelY0, 0);
         pt = new Point3d((offset.X - origin.X) / 2, panelY0 - (dimStyle.TextHeight * 4), 0);

         plane = Plane.WorldXY;
         plane.Origin = origin;

         double u, v;
         plane.ClosestParameter(origin, out u, out v);
         Point2d ext1 = new Point2d(u, v);

         plane.ClosestParameter(offset, out u, out v);
         Point2d ext2 = new Point2d(u, v);

         plane.ClosestParameter(pt, out u, out v);
         Point2d linePt = new Point2d(u, v);

         LinearDimension dimension = new LinearDimension(plane, ext1, ext2, linePt);
         Guid dimGuid = doc.Objects.AddLinearDimension(dimension);

         guidList.Add(dimGuid);

         // Add horizontal borders dimension
         origin = new Point3d(panelX0, panelY0, 0);
         offset = new Point3d(borderX0, panelY0, 0);
         pt = new Point3d((offset.X - origin.X) / 2, panelY0 - (dimStyle.TextHeight * 2), 0);


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
         origin = new Point3d(panelX1, panelY0, 0);
         offset = new Point3d(borderX1, panelY0, 0);
         pt = new Point3d((offset.X - origin.X) / 2, panelY0 - (dimStyle.TextHeight * 2), 0);


         plane.ClosestParameter(origin, out u, out v);
         ext1 = new Point2d(u, v);

         plane.ClosestParameter(offset, out u, out v);
         ext2 = new Point2d(u, v);

         plane.ClosestParameter(pt, out u, out v);
         linePt = new Point2d(u, v);

         dimension = new LinearDimension(plane, ext1, ext2, linePt);
         dimGuid = doc.Objects.AddLinearDimension(dimension);

         guidList.Add(dimGuid);

         // Add vertical dimension for panel

         origin = new Point3d(panelX0, panelY0, 0);
         offset = new Point3d(panelX0, panelY1, 0);
         pt = new Point3d(panelX0 - (dimStyle.TextHeight * 4), (offset.Y - origin.Y) / 2, 0);

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

         // Add vertical dimension for panel

         origin = new Point3d(panelX0, panelY0, 0);
         offset = new Point3d(panelX0, borderY0, 0);
         pt = new Point3d(panelX0 - (dimStyle.TextHeight * 2), (offset.Y - origin.Y) / 2, 0);

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

         origin = new Point3d(panelX0, panelY1, 0);
         offset = new Point3d(panelX0, borderY1, 0);
         pt = new Point3d(panelX0 - (dimStyle.TextHeight * 2), (offset.Y - origin.Y) / 2, 0);

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

         doc.Views.Redraw();

         RhinoObject panelPerimeterObj = doc.Objects.Find(panel.Perimeter);

         // Select all objects on Perforation Layer
         Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);

         double tolerance = Properties.Settings.Default.Tolerance;
         Rhino.Geometry.Curve panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

         // If tool perforation layer is missing 
         if(rhinoObjs == null)
         {
            // Select all objects on Perforation Layer
            rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);
         }

         if (Convert.ToBoolean(panel.DrawPerf) == true && rhinoObjs != null)
         {
            foreach (RhinoObject rhinoObj in rhinoObjs)
            {
               Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

               if (testCurve != null)
               {
                  if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                  {
                     guidList.Add(rhinoObj.Id);
                  }
               }
            }
         }

         // Export the panel

         doc.Objects.UnselectAll();


         doc.Objects.Select(panel.Perimeter);

         // Get all of the objects on the layer. If layername is bogus, you will
         // just get an empty list back

         Rhino.DocObjects.RhinoObject label = doc.Objects.Find(panel.Label);
         string exportFileName = "1";

         if (label != null)
         {
            label.Select(true);
            Rhino.Geometry.TextEntity textentity = label.Geometry as Rhino.Geometry.TextEntity;
            exportFileName = textentity.Text + ".dxf";
         }

         // Select all objects on DOTS Layer
         rhinoObjs = doc.Objects.FindByLayer("DOTS");
         
         tolerance = Properties.Settings.Default.Tolerance;
         panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

         if (rhinoObjs != null)
         {
            foreach (RhinoObject rhinoObj in rhinoObjs)
            {
               Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

               if (testCurve != null)
               {
                  if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                  {
                     rhinoObj.Select(true);
                  }
               }
            }
         }

         // Select all objects on Tool Hit
         rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);

         if(rhinoObjs == null)
         {
            rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);
         }

         if (rhinoObjs != null)
         {
            foreach (RhinoObject rhinoObj in rhinoObjs)
            {
               Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

               if (testCurve != null)
               {
                  if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                  {
                     rhinoObj.Select(true);
                  }
               }
            }
         }

         string command = string.Format("-_Export \"" + Path.GetDirectoryName(doc.Path) + @"\" + exportFileName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
         // Export the selected curves
         RhinoApp.RunScript(command, true);

         // Unselect all objects
         doc.Objects.UnselectAll();

         // Default layer index
         int defaultLayerIndex = doc.Layers.Find("Default", true);

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         if (Convert.ToBoolean(panel.DrawPerf) != true)
         {
            // Delete the Perforation layer as it consumes too much memory
            // Create a new layer called DIMENSION
            layerName = "PERFORATION";

            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // Get all of the objects on the layer. If layername is bogus, you will
            // just get an empty list back
            Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer(layerName);

            if (rhobjs != null)
            {
               if (rhobjs.Length > 0)
               {
                  for (int i = 0; i < rhobjs.Length; i++)
                     doc.Objects.Delete(rhobjs[i], true);
               }
            }

            doc.Layers.Delete(layerIndex, true);
         }

         foreach (Guid g in guidList)
         {
            int idx = RhinoDoc.ActiveDoc.Groups.Find(panel.PartName, false);

            if (idx < 0)
            {
               idx = RhinoDoc.ActiveDoc.Groups.Add(panel.PartName);
            }

            RhinoDoc.ActiveDoc.Groups.AddToGroup(idx, g);
         }
      }

   }
}
