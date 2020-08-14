using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.IO;
using Rhino.Display;

namespace MetrixGroupPlugins.Commands
{
   [
      System.Runtime.InteropServices.Guid("f5ff68e4-66f7-4af2-92c8-1eb6010429f4"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
   ]
   public class AddFoldedLayoutCommand : Command
   {
      static AddFoldedLayoutCommand _instance;
      public AddFoldedLayoutCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static AddFoldedLayoutCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "AddFoldedLayout"; }
      }

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="doc">The current document.</param>
      /// <param name="mode">The command running mode.</param>
      /// <returns>
      /// The command result code.
      /// </returns>
      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         return createLayout(doc, null, null);

      }

      /// <summary>
      /// Creates the layout.
      /// </summary>
      /// <param name="doc">The document.</param>
      /// <param name="panel">The panel.</param>
      /// <param name="panelParas">The panel paras.</param>
      /// <returns></returns>
      public Result createLayout(RhinoDoc doc, FoldedPerforationPanel panel, PanelParameters panelParas)
      {
         if(panelParas == null)
         {
            panelParas = new PanelParameters();
         }

         if (panel == null)
         {
            panel = new FoldedPerforationPanel();
         }

         // Get all selected Objects
         GetObject go = new GetObject();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.EnablePressEnterWhenDonePrompt(false);

         go.SetCommandPrompt("Select items for the new layout:");

         // Disable the scaling
         RhinoApp.RunScript("_-DocumentProperties AnnotationStyles ModelSpaceScaling=Disabled LayoutSpaceScaling=Disabled _Enter _Enter", true);

         GetResult result = go.GetMultiple(1, -1);

         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         RhinoApp.WriteLine("Total Objects Selected: {0}", go.ObjectCount);

         string labelName = panel.PartName;
         string area = string.Format("{0:0.00}", panel.Area);
         Point2d minPoint = new Point2d(0, 0);
         Point2d maxPoint = new Point2d(0, 0);

         // Loop through all the objects to find Text
         for (int i = 0; i < go.ObjectCount; i++)
         {

            BoundingBox bBox = go.Object(i).Object().Geometry.GetBoundingBox(true);

            if (bBox.Min.X < minPoint.X)
            {
               minPoint.X = bBox.Min.X;
            }

            if (bBox.Min.Y < minPoint.Y)
            {
               minPoint.Y = bBox.Min.Y;
            }

            if (bBox.Max.X > maxPoint.X)
            {
               maxPoint.X = bBox.Max.X;
            }

            if (bBox.Max.Y > maxPoint.Y)
            {
               maxPoint.Y = bBox.Max.Y;
            }
         }

         // If the selected items has no label, return failure
         if (labelName == null)
         {
            return Rhino.Commands.Result.Failure;
         }

         // Hide all the non selected objects
         foreach (var obj in doc.Objects)
         {
            if (obj.IsSelected(true) == 0)
            {
               doc.Objects.Hide(obj, false);
            }
         }

         // Add layout
         doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;


         RhinoView currentView = doc.Views.ActiveView;
         var pageview = doc.Views.AddPageView(string.Format("{0}", labelName), 210, 297);
         Point2d bottomLeft = new Point2d(10, 70);
         Point2d topRight = new Point2d(200, 287);

         if (pageview != null)
         {
            pageview.SetPageAsActive();

            var detail = pageview.AddDetailView("Panel", bottomLeft, topRight, Rhino.Display.DefinedViewportProjection.Top);

            // Show all objects
            RhinoApp.RunScript("_-Show _Enter", true);

            if (detail != null)
            {
               pageview.SetActiveDetail(detail.Id);

               doc.Views.ActiveView = pageview;

               //doc.Views.Redraw();
               // Select all the objects
               for (int i = 0; i < go.ObjectCount; i++)
               {
                  RhinoObject rhinoObject = go.Object(i).Object();

                  rhinoObject.Select(true);
               }

               // Hide all the non selected objects
               var filter = new ObjectEnumeratorSettings
               {
                  NormalObjects = true,
                  LockedObjects = false,
                  HiddenObjects = false,
                  ActiveObjects = true,
                  ReferenceObjects = true
               };

               var rh_objects = doc.Objects.FindByFilter(filter);

                pageview.SetPageAsActive();

               //doc.Views.Redraw();

               foreach (var rh_obj in rh_objects)
               {
                  var select = 0 == rh_obj.IsSelected(false) && rh_obj.IsSelectable();
                  rh_obj.Select(select);
               }

               RhinoApp.RunScript("_-HideInDetail Enter", true);

               detail.IsActive = false;
            }

            bottomLeft = new Point2d(10, 40);
            topRight = new Point2d(135, 70);

            detail = pageview.AddDetailView("Sample", bottomLeft, topRight, Rhino.Display.DefinedViewportProjection.Top);

          //  doc.Views.Redraw();
            pageview.SetActiveDetail(detail.Id);

            detail.Viewport.SetCameraLocation(new Point3d(50, 160, 0), true);
            detail.CommitViewportChanges();
          //  doc.Views.Redraw();

            detail.DetailGeometry.IsProjectionLocked = true;
            detail.DetailGeometry.SetScale(4.5, doc.ModelUnitSystem, 1, doc.PageUnitSystem);
            detail.CommitChanges();

          //  doc.Views.Redraw();


            detail.IsActive = true;
            pageview.SetActiveDetail(detail.Id);

            RhinoApp.WriteLine("Name = {0}: Width = {1}, Height = {2}",
               detail.Viewport.Name, detail.Viewport.Size.Width, detail.Viewport.Size.Height);
            detail.CommitViewportChanges();
          //  doc.Views.Redraw();

            detail.IsActive = false;

            bottomLeft = new Point2d(5, 5);
            topRight = new Point2d(205, 35);
            detail = pageview.AddDetailView("Block", bottomLeft, topRight, Rhino.Display.DefinedViewportProjection.Top);

          //  doc.Views.Redraw();

            detail.IsActive = true;
            pageview.SetActiveDetail(detail.Id);

            detail.Viewport.SetCameraLocation(new Point3d(105, 520, 0), true);
            detail.CommitViewportChanges();

          //  doc.Views.Redraw();

            detail.DetailGeometry.IsProjectionLocked = true;
            detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 1, doc.PageUnitSystem);
            detail.CommitChanges();

            detail.IsActive = false;
          //  doc.Views.Redraw();

            drawBlock(doc, labelName, area, panel.PanelNumber, panelParas);

          //  doc.Views.Redraw();
            
         }

         // Show all objects
         RhinoApp.RunScript("_-Show _Enter", true);

         doc.Views.DefaultViewLayout();

         doc.Views.ActiveView = currentView;

         return Result.Success;
      }

      public void drawBlock(RhinoDoc doc, string labelName, string area, int panelNum, PanelParameters panelParas)
      {
         RhinoUtilities.SetActiveLayer("LAYOUT DETAILS", System.Drawing.Color.Black);

         //// Get the location of current API 
         //String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Logo\\MetrixLogo.jpg";

         //Plane picture = new Plane(new Point3d(5, 10, 0), new Vector3d(0, 0, 1));

         //// Add Company logo.
         //doc.Objects.AddPictureFrame(picture, path, false, 62.5, 25, false, false);

         // Draw bottom line
         doc.Objects.AddLine(new Point3d(5, 5, 0), new Point3d(205, 5, 0));

         // Top line
         doc.Objects.AddLine(new Point3d(5, 292, 0), new Point3d(205, 292, 0));

         // Left line
         doc.Objects.AddLine(new Point3d(5, 5, 0), new Point3d(5, 292, 0));

         // Right line
         doc.Objects.AddLine(new Point3d(205, 5, 0), new Point3d(205, 292, 0));

         //// Divider line
         //doc.Objects.AddLine(new Point3d(5, 10, 0), new Point3d(205, 10, 0));

         //// Row 1 line
         //doc.Objects.AddLine(new Point3d(67.5, 15, 0), new Point3d(205, 15, 0));

         //// Row 2 line
         //doc.Objects.AddLine(new Point3d(67.5, 20, 0), new Point3d(205, 20, 0));

         //// Row 3 line
         //doc.Objects.AddLine(new Point3d(67.5, 25, 0), new Point3d(205, 25, 0));

         //// Row 4 line
         //doc.Objects.AddLine(new Point3d(67.5, 30, 0), new Point3d(205, 30, 0));

         //// Row 5 line
         //doc.Objects.AddLine(new Point3d(67.5, 35, 0), new Point3d(205, 35, 0));

         //// Vertical divider line
         //doc.Objects.AddLine(new Point3d(136.5, 5, 0), new Point3d(136.5, 35, 0));

         //// Vertical divider line
         //doc.Objects.AddLine(new Point3d(87.5, 10, 0), new Point3d(87.5, 35, 0));

         //// Vertical divider line
         //doc.Objects.AddLine(new Point3d(156.5, 10, 0), new Point3d(156.5, 35, 0));

         // Draw Approval Box
         // Horizontal lines
         //doc.Objects.AddLine(new Point3d(136.5, 40, 0), new Point3d(200, 40, 0));
         //doc.Objects.AddLine(new Point3d(136, 39.5, 0), new Point3d(200.5, 39.5, 0));
         //doc.Objects.AddLine(new Point3d(136.5, 45, 0), new Point3d(200, 45, 0));
         //doc.Objects.AddLine(new Point3d(136.5, 55, 0), new Point3d(200, 55, 0));
         //doc.Objects.AddLine(new Point3d(136.5, 60, 0), new Point3d(200, 60, 0));
         //doc.Objects.AddLine(new Point3d(136.5, 65, 0), new Point3d(200, 65, 0));
         //doc.Objects.AddLine(new Point3d(136, 65.5, 0), new Point3d(200.5, 65.5, 0));
        
         //// Vertical lines
         //doc.Objects.AddLine(new Point3d(136, 39.5, 0), new Point3d(136, 65.5, 0));
         //doc.Objects.AddLine(new Point3d(136.5, 40, 0), new Point3d(136.5, 65, 0));
         //doc.Objects.AddLine(new Point3d(156.5, 40, 0), new Point3d(156.5, 60, 0));
         //doc.Objects.AddLine(new Point3d(200, 40, 0), new Point3d(200, 65, 0));
         //doc.Objects.AddLine(new Point3d(200.5, 39.5, 0), new Point3d(200.5, 65.5, 0));

         string text = "51 Holloway Drive            metrixgroup.com.au\nBayswater VIC 3153                  1300 792 493";
         double height = 3.5;
        Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(11, 15 + height, 0);
        string font = "Arial";
         Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
         plane.Origin = pt;
         Guid id;
         
         // id = doc.Objects.AddText(text, plane, height, font, false, false);

         //height = 1.5;
         //text = "Supply of this order will be solely and exlusively according to the terms and conditions of Metrix Group Pty Ltd.";
         //plane.Origin = new Point3d(7, 6, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //height = 1.5;
         //text = "PANEL";
         //plane.Origin = new Point3d(69, 31, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = labelName;
         plane.Origin = new Point3d(89, 31 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "DRAWN";
         //plane.Origin = new Point3d(69, 26, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "CHECKED";
         //plane.Origin = new Point3d(69, 21, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "m²";
         //plane.Origin = new Point3d(69, 16, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         text = area;
         plane.Origin = new Point3d(89, 16 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "Page ";
         //plane.Origin = new Point3d(69, 11, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         text = panelNum + " of " + panelParas.TotalPanel;
         plane.Origin = new Point3d(89, 11 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "PROJECT";
         //plane.Origin = new Point3d(138, 31, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = panelParas.Project;
         plane.Origin = new Point3d(158, 31 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "CUSTOMER ";
         //plane.Origin = new Point3d(138, 26, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = panelParas.CustomerName;
         plane.Origin = new Point3d(158, 26 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "JOB NO.";
         //plane.Origin = new Point3d(138, 21, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = panelParas.JobNo;
         plane.Origin = new Point3d(158, 21 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "MATERIAL";
         //plane.Origin = new Point3d(138, 16, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = panelParas.Material;
         plane.Origin = new Point3d(158, 16 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "COATING";
         //plane.Origin = new Point3d(138, 11, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         height = 1.5;
         text = panelParas.Coating;
         plane.Origin = new Point3d(158, 11 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         text = "Copyright © Metrix Group " + DateTime.Today.Year;
         plane.Origin = new Point3d(138, 6 + height, 0);
         id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "APPROVED BY";
         //plane.Origin = new Point3d(138, 61, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "NAME";
         //plane.Origin = new Point3d(138, 56, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "SIGNATURE";
         //plane.Origin = new Point3d(138, 51, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         //text = "DATE";
         //plane.Origin = new Point3d(138, 41, 0);
         //id = doc.Objects.AddText(text, plane, height, font, false, false);

         if (panelParas.DotFont == 1)
         {
            text = "* 10 mm Dot font labelling to the rear of all panels";
            plane.Origin = new Point3d(136.5, 70 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);

            text = "The purple line represents the unperforated distance"+"\nbetween the edge of the panel and the perforation";
            plane.Origin = new Point3d(136.5, 65 + height, 0);
            id = doc.Objects.AddText(text, plane, height, font, false, false);
         }
      }
   }
}
