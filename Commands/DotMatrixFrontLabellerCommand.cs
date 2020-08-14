using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;

namespace MetrixGroupPlugins
{
   [System.Runtime.InteropServices.Guid("cf7dae92-8111-4157-8898-ace11a82ca1b")]
   public class DotMatrixFrontLabellerCommand : Command
   {
      public DotMatrixFrontLabellerCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static DotMatrixFrontLabellerCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "DotMatrixFrontLabeller"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // Check the selected dot
         GetObject go = new GetObject();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Point | Rhino.DocObjects.ObjectType.Annotation;


            go.SetCommandPrompt("Select Label and a point:");
         GetResult result = go.GetMultiple(2, -1);



         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         RhinoApp.WriteLine("Object selection counter = {0}", go.ObjectCount);

         Point pt = null;
         TextEntity textEntity = null;
         int pointCounter = 0;
         int textCounter = 0;


         // Loop through all the objects to find Text
         for (int i = 0; i < go.ObjectCount; i++)
         {
            RhinoObject rhinoObject = go.Object(i).Object();

            if (rhinoObject.ObjectType == ObjectType.Annotation)
            {
               textEntity = rhinoObject.Geometry as TextEntity;

               if (textEntity != null && textCounter == 0)
               {
                  textCounter++;
               }
            }
            else if (rhinoObject.ObjectType == ObjectType.Point)
            {
               pt = rhinoObject.Geometry as Point;

               if(pt != null && pointCounter == 0)
               {
                  pointCounter++;
               }
            }
         }



         //if (go.Object(0).Point() != null && go.Object(1).TextEntity() != null)
         //{
         //   pt = go.Object(0).Point();
         //   textEntity = go.Object(1).TextEntity();
         //}
         //else if (go.Object(1).Point() != null && go.Object(0).TextEntity() != null)
         //{
         //   pt = go.Object(1).Point();
         //   textEntity = go.Object(0).TextEntity();
         //}
         //else
         //{
         //   RhinoApp.WriteLine("Two of the same objects are selected.");
         //   return Result.Failure;
         //}

         if(textCounter > 1)
         {
            RhinoApp.WriteLine("More than one text has been selected.");
         }

         if(pointCounter > 1)
         {
            RhinoApp.WriteLine("More than one point has been selected.");
         }


         if (pt != null && textEntity != null)
         {
            drawDotMatrix(pt.Location, textEntity.Text, Properties.Settings.Default.DotMatrixHeight);
         }

         doc.Views.Redraw();

         return Result.Success;
      }

      public Result drawDotMatrix(Point3d pt, string text, double textHeight)
      {

         TextEntity dotMatrixText = new TextEntity();
         Vector3d normalVector = new Vector3d(0, 0, 1);
         Plane textPlane = new Plane(pt, normalVector);

         dotMatrixText.Plane = textPlane;
         dotMatrixText.Text = text;
         dotMatrixText.Justification = TextJustification.BottomRight;
         dotMatrixText.TextHeight = textHeight;
         dotMatrixText.FontIndex = RhinoDoc.ActiveDoc.Fonts.FindOrCreate("Dot-Matrix", false, false);
            dotMatrixText.DrawForward = false;
         Rhino.Geometry.Curve[] curves = dotMatrixText.Explode();
         Rhino.Geometry.Curve[] newCurves = new Curve[curves.Length];

         // Get each centre of the curves
         for(int i = 0; i < curves.Length; i++)
         {
            // Find the centre 
            BoundingBox bbox = curves[i].GetBoundingBox(true);

            newCurves[i] = new ArcCurve(new Circle(bbox.Center, 0.5));
         }


         if (newCurves != null)
         {
            foreach (Curve curve in newCurves)
            {
               Guid guid = RhinoDoc.ActiveDoc.Objects.AddCurve(curve);

               int idx = RhinoDoc.ActiveDoc.Groups.Find(text, false);

               if (idx < 0)
               {
                  idx = RhinoDoc.ActiveDoc.Groups.Add(text);
               }

               RhinoDoc.ActiveDoc.Groups.AddToGroup(idx, guid);
            }
         }



         return Result.Success;
      }
   }
}
