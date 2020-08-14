using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace MetrixGroupPlugins
{
   [
      System.Runtime.InteropServices.Guid("e0deb26c-f20e-485f-bfbf-e6db793aa38f"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
   ]
   public class GeometryPopulate : Command
   {
      public GeometryPopulate()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static GeometryPopulate Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "GeometryPopulate"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {

         // Check the selected dot
         GetObject go = new GetObject();

         // Create a new dictionary of strings, with string keys.
         //
         Dictionary<double, double> sizeAngle = new Dictionary<double, double>();
         List<double> holeSizeList = new List<double>();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

         go.SetCommandPrompt("Select all the circles:");
         GetResult result = go.GetMultiple(1, -1);

         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         RhinoApp.WriteLine("Object selection counter = {0}", go.ObjectCount);

         List<ArcCurve> arcCurveList = new List<ArcCurve>();

         // Loop through all the objects to find Curve
         for (int i = 0; i < go.ObjectCount; i++)
         {
            RhinoObject rhinoObject = go.Object(i).Object();

            if (rhinoObject.ObjectType == ObjectType.Curve)
            {
               ArcCurve curve = rhinoObject.Geometry as ArcCurve;
               
               if (curve != null)
               {
                  if (curve.IsCircle() == true)
                  {

                     if (!holeSizeList.Exists(element => element == curve.Radius))
                     {
                        holeSizeList.Add(curve.Radius);
                     }

                     arcCurveList.Add(curve);
                  }
               }
            }
         }

         holeSizeList.Sort();

         double maxHole = holeSizeList.Max();
         double minHole = holeSizeList.Min();

         foreach (double size in holeSizeList)
         {
            double angle;
            if ((maxHole - minHole) != 0)
            {
               angle = 180 * ((size - minHole) / (maxHole - minHole));
            }
            else
            {
               angle = 0;
            }

            sizeAngle.Add(size, angle);
         }


         // Open file dialog
         OpenFileDialog openFileDialog = new OpenFileDialog();

         openFileDialog.InitialDirectory = doc.Path;
         openFileDialog.Filter = "3dm files (*.3dm)|*.3dm| dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";
         openFileDialog.FilterIndex = 1;
         openFileDialog.RestoreDirectory = true;
         openFileDialog.Multiselect = false;
         openFileDialog.Title = "CAD file containing geometry";

         DialogResult dr = openFileDialog.ShowDialog();
         uint firstSN, lastSN;
         string sScript;

         List<RhinoObject> imported = new List<RhinoObject>();

         if (dr == System.Windows.Forms.DialogResult.OK)
         {
            string file = openFileDialog.FileName;
            // Import the file
            try
            {
               sScript = String.Format("! _-Import \"{0}\" _Enter", file);
               firstSN = RhinoObject.NextRuntimeSerialNumber;
               RhinoApp.RunScript(sScript, false);
               lastSN = RhinoObject.NextRuntimeSerialNumber;
               List<Guid> ids = new List<Guid>();

               foreach (RhinoObject obj in doc.Objects)
               {
                  if (obj.RuntimeSerialNumber >= firstSN && obj.RuntimeSerialNumber < lastSN)
                  {
                     imported.Add(obj);
                     ids.Add(obj.Id);
                  }
               }               

               foreach (ArcCurve ac in arcCurveList)
               {
                  double angle = 0;

                  sizeAngle.TryGetValue(ac.Radius, out angle);

                  Transform translation = Transform.Translation(ac.Arc.Center.X, ac.Arc.Center.Y, ac.Arc.Center.Z);
                  Transform rotate = Transform.Rotation(angle * Math.PI / 180, new Point3d(0, 0, 0));

                  List<Guid> rotatedIds = new List<Guid>();
                  foreach (var objRef in imported)
                  {
                     rotatedIds.Add(doc.Objects.Transform(objRef, rotate, false));
                  }

                  foreach(var id in rotatedIds)
                  {
                     RhinoObject objectRot;

                     objectRot = doc.Objects.Find(id);
                     doc.Objects.Transform(objectRot, translation, true);                     
                  }
               }

            }
            catch (Exception ex)
            {
               // Probably related to Windows file system permissions.
               MessageBox.Show(ex.Message);
            }
         }



         // Export the whole lot
         //string command = string.Format("-_Export \"" + Path.GetDirectoryName(doc.Path) + @"\" + labelName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
         // Export the selected curves
         //RhinoApp.RunScript(command, true);

         return Result.Success;
      }
   }
}
