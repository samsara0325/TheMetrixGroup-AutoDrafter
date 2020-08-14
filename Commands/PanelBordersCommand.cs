using MetrixGroupPlugins.Resources;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System.Drawing;

namespace MetrixGroupPlugins
{
   [
      System.Runtime.InteropServices.Guid("5d2e20e2-87ce-4d33-ad17-ba822114ccb0"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
   ]
   public class PanelBordersCommand : Command
   {
      public PanelBordersCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static PanelBordersCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "PanelBorders"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         string selectedLayer;
         Color selectedColour;
         // For this example we will use a GetPoint class, but all of the custom
         // "Get" classes support command line options.
         Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();
         go.SetCommandPrompt("Select panels to create borders");

         double border = Properties.Settings.Default.PanelBorderDefault; // Default borderSize is 50

         // set up the options
         Rhino.Input.Custom.OptionDouble borderSizeOption = new Rhino.Input.Custom.OptionDouble(border);

         go.AddOptionDouble("Borders", ref borderSizeOption);
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


         RequiredLayer getRequiredLayer = new RequiredLayer();
         getRequiredLayer.ShowDialog();
         selectedLayer = getRequiredLayer.getLayerName();
         selectedColour = getRequiredLayer.getSelectedColor();
         while (true)
         {
            go.ClearCommandOptions();
            borderSizeOption = new Rhino.Input.Custom.OptionDouble(border);
            go.AddOptionDouble("Borders", ref borderSizeOption);
            // perform the get operation. This will prompt the user to select the list of curves, but also
            // allow for command line options defined above
            GetResult result = go.GetMultiple(1, 0);

            if (result == GetResult.Option)
            {
                border = borderSizeOption.CurrentValue;
                go.EnablePreSelect(false, true);
                continue;
            }
            else if (result == GetResult.Number)
            {
               border = go.Number();
               continue;
            }
            else if (result != GetResult.Object)
            {
               return Result.Cancel;
            }

            if(go.ObjectsWerePreselected)
            {
               go.EnablePreSelect(false, true);
               continue;
            }

            break;
         }

         int objecTCount = go.ObjectCount;
         border = borderSizeOption.CurrentValue;

         foreach(ObjRef objRef in go.Objects())
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

            // Process the curve
            Plane plane = Rhino.Geometry.Plane.WorldXY;
            Curve[] offsetCurves;

            int layerIndex = doc.Layers.CurrentLayerIndex;
            RhinoUtilities.SetActiveLayer(selectedLayer, selectedColour);

            //if (curve.TryGetPlane(out plane))
            //{
            if (border < 0) //If the border is negative, it means the border should be drawn outside the perimeter
            {
               plane.XAxis = -plane.XAxis;
               plane.YAxis = -plane.YAxis;
               plane.ZAxis = -plane.ZAxis;

               offsetCurves = curve.Offset(plane, border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp);
            }
            else
            {
               offsetCurves = curve.Offset(plane, -border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp);
            }

            //Check if the curve is outside border and border is a positive 
            if (curve.Contains(offsetCurves[0].PointAtStart, Plane.WorldXY, 0) == PointContainment.Outside && border > 0)
            { 
               offsetCurves = curve.Offset(plane, border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp); //if true, then try to set the curve to be within the border
            }

            //Check if the curve is within the border and border is a negative
            if (curve.Contains(offsetCurves[0].PointAtStart, Plane.WorldXY, 0) == PointContainment.Inside && border < 0)
            {
               offsetCurves = curve.Offset(plane, -border, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp); //if true, then try to set the curve to be outside the border
            }

            foreach ( Curve c in offsetCurves)
            {
               doc.Objects.AddCurve(c);
            }
           

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
         }

         doc.Views.Redraw();

         Properties.Settings.Default.PanelBorderDefault = border;
         Properties.Settings.Default.Save();

         return Result.Success;
      }
   }
}
