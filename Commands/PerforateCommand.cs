using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace MetrixGroupPlugins
{
   [System.Runtime.InteropServices.Guid("b53693cd-73bf-4ab3-b7e2-7a8b5e10f09a")]
   public class PerforationCommand : Command
   {
      public PerforationCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static PerforationCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "Perforation"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // Check the selected curve
         GetObject go = new GetObject();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

         GetResult result = go.Get();

         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         if (go.ObjectCount != 1)
         {
            RhinoApp.WriteLine("Error: {0} curve is selected.", go.ObjectCount);
            return Rhino.Commands.Result.Failure;
         }

         RhinoApp.WriteLine("{0} curve is selected.", go.ObjectCount);

         Curve curve = go.Object(0).Curve();

         // If curve is null
         if (curve == null)
         {
            return Rhino.Commands.Result.Failure;
         }

         // If curve is Closed Curve Orientation 
         if (curve.IsClosed == false)
         {
            RhinoApp.WriteLine("The curve is open");
            return Rhino.Commands.Result.Failure;
         }

         PerforationForm perforationForm = new PerforationForm(curve);

         //  perforationForm.ShowDialog(RhinoApp.MainWindow());
         // Prompt the user to enter a layer name
         Rhino.Input.Custom.GetString gs = new Rhino.Input.Custom.GetString();
         gs.SetCommandPrompt("Name of the Design: e.g. 60 degrees - Round 2.4 @ 3.6");
         gs.AcceptNothing(false);
         gs.Get();
         if (gs.CommandResult() != Rhino.Commands.Result.Success)
            return gs.CommandResult();

         perforationForm.drawPerforationDesign(gs.StringResult().Trim(), true);

         return Result.Success;
      }
   }
}
