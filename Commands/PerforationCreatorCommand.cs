using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace MetrixGroupPlugins
{
    [System.Runtime.InteropServices.Guid("3f2e1395-a145-4dcd-9b1f-3388a75215c3")]
    public class PerforationCreatorCommand : Command
    {

        public PerforationCreatorCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static PerforationCreatorCommand Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "PerforationCreator"; }
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
            RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
            PerforationForm perforationForm = new PerforationForm(curve);

            perforationForm.ShowDialog(RhinoApp.MainWindow());
            RhinoDoc.ActiveDoc.Views.Redraw();
            return Result.Success;
        }
    }
}
