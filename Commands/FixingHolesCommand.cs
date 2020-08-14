using MetrixGroupPlugins.PunchingTools;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System.Collections;
using System.Collections.Generic;

namespace MetrixGroupPlugins
{
   [
      System.Runtime.InteropServices.Guid("5968f550-0299-451b-833b-86c0c5ddbd52"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
   ]
   public class FixingHolesCommand : Command
   {
      public FixingHolesCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static FixingHolesCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "FixingHoles"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // For this example we will use a GetPoint class, but all of the custom
         // "Get" classes support command line options.
         Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
         go.SetCommandPrompt("Select panels to create fixing holes");
          //get the default values for the properties
         double holeSize = Properties.Settings.Default.FixingHoleSize;             
         double offsetX = Properties.Settings.Default.FixingHoleOffsetX; 
         double offsetY = Properties.Settings.Default.FixingHoleOffsetY; 
         double spacing = Properties.Settings.Default.FixingHoleSpacing; 
         double minSpacing = Properties.Settings.Default.FixingHoleMinimum; 

         // set up the options
         Rhino.Input.Custom.OptionDouble holeSizeOption = new Rhino.Input.Custom.OptionDouble(holeSize);
         Rhino.Input.Custom.OptionDouble offsetXOption = new Rhino.Input.Custom.OptionDouble(offsetX);
         Rhino.Input.Custom.OptionDouble offsetYOption = new Rhino.Input.Custom.OptionDouble(offsetY);
         Rhino.Input.Custom.OptionDouble spacingOption = new Rhino.Input.Custom.OptionDouble(spacing);
         Rhino.Input.Custom.OptionDouble minSpacingOption = new Rhino.Input.Custom.OptionDouble(minSpacing);


         //using option to get values and save automatically(only if user enters)
         go.AddOptionDouble("HoleSize", ref holeSizeOption);
         go.AddOptionDouble("OffsetX", ref offsetXOption);
         go.AddOptionDouble("OffsetY", ref offsetYOption);
         go.AddOptionDouble("Spacing", ref spacingOption);
         go.AddOptionDouble("MinSpacing", ref minSpacingOption);

         go.AcceptNumber(true, true);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

         while (true)
         {
            go.ClearCommandOptions();
            holeSizeOption = new Rhino.Input.Custom.OptionDouble(holeSize);
            offsetXOption = new Rhino.Input.Custom.OptionDouble(offsetX);
            offsetYOption = new Rhino.Input.Custom.OptionDouble(offsetY);
            spacingOption = new Rhino.Input.Custom.OptionDouble(spacing);
            minSpacingOption = new Rhino.Input.Custom.OptionDouble(minSpacing);

            go.AddOptionDouble("HoleSize", ref holeSizeOption);
            go.AddOptionDouble("OffsetX", ref offsetXOption);
            go.AddOptionDouble("OffsetY", ref offsetYOption);
            go.AddOptionDouble("Spacing", ref spacingOption);
            go.AddOptionDouble("MinSpacing", ref minSpacingOption);
            // perform the get operation. This will prompt the user to select the list of curves, but also
            // allow for command line options defined above
            GetResult result = go.GetMultiple(1, 0);

            if (result == GetResult.Option)
            {
               holeSize = holeSizeOption.CurrentValue;
               offsetX = offsetXOption.CurrentValue;
               offsetY = offsetYOption.CurrentValue;
               spacing = spacingOption.CurrentValue;
               minSpacing = minSpacingOption.CurrentValue;
               go.EnablePreSelect(false, true);
               continue;
            }
            //else if (result == GetResult.Number)
            //{
            //   border = go.Number();
            //   continue;
            //}
            else if (result != GetResult.Object)
            {
               return Result.Cancel;
            }

            if (go.ObjectsWerePreselected)
            {
               go.EnablePreSelect(false, true);
               continue;
            }

            break;
         }

         int objecTCount = go.ObjectCount;

         foreach (ObjRef objRef in go.Objects())
         {
            Curve curve = objRef.Curve();

            // If curve is null, means objRef is not a curve
            if (curve == null)
            {
               continue;
            }

            // If curve is not Closed Curve
            if (curve.IsClosed == false)
            {
               RhinoApp.WriteLine(objRef.ToString() + " curve is open");
               continue;
            }

            if (curve.IsPlanar() == false)
            {
               RhinoApp.WriteLine(objRef.ToString() + " curve is not planar");
               continue;
            }


            // Find the boundary 
            BoundingBox boundingBox = curve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            List <Point3d> pointsList = new List<Point3d> ();

            // Calculate top and bottom fixing holes
            double runningX = min.X + offsetX; //25
            double runningY = max.Y - offsetY; //-626.5

            Point3d point;

            while (runningX < (max.X - offsetX) - minSpacing)
            {
               point = new Point3d(runningX, runningY, 0);
               pointsList.Add(point);

               point = new Point3d(runningX, min.Y + offsetY, 0);
               pointsList.Add(point);

               runningX = runningX + spacing;
            }

            point = new Point3d(max.X - offsetX, runningY, 0); //adds the top right fixing hole
            pointsList.Add(point);

            point = new Point3d(max.X - offsetX, min.Y + offsetY, 0); //adds the bottom right fixing hole
            pointsList.Add(point);

            runningY = runningY - spacing;

            // Calculate the sides
            while (runningY > (min.Y - offsetY) + minSpacing)
            {
               point = new Point3d(min.X + offsetX, runningY, 0); //adds the left fixing holes
               pointsList.Add(point);

               point = new Point3d(max.X - offsetX, runningY, 0); //adds the right fixing hole
               pointsList.Add(point);

               runningY = runningY - spacing;
            }

            // Process the curve
            Plane plane = Rhino.Geometry.Plane.WorldXY;

            int layerIndex = doc.Layers.CurrentLayerIndex;

            RhinoUtilities.SetActiveLayer("Fixing Holes", System.Drawing.Color.Black);

            // Draw all the holes
            Round round = new Round();
            round.X = holeSize;
            

            foreach (Point3d p in pointsList)
            {
               round.drawTool(p);
            }
            
            //Round round = new Round();
            //round.X = holeSize;
            //round.drawTool(point);

            
            //offsetCurves = curve.Offset(plane, -border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp);

            //if (curve.Contains(offsetCurves[0].PointAtStart, Plane.WorldXY, 0) == PointContainment.Outside)
            //{
            //   offsetCurves = curve.Offset(plane, border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp);
            //}


            //foreach (Curve c in offsetCurves)
            //{
            //   doc.Objects.AddCurve(c);
            //}
            //// }

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
         }

         doc.Views.Redraw();

         Properties.Settings.Default.FixingHoleSize = holeSize;
         Properties.Settings.Default.FixingHoleOffsetX = offsetX;
         Properties.Settings.Default.FixingHoleOffsetY = offsetY;
         Properties.Settings.Default.FixingHoleSpacing = spacing;
         Properties.Settings.Default.FixingHoleMinimum = minSpacing;
         Properties.Settings.Default.Save();

         return Result.Success;
      }
   }
}
