using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;

namespace MetrixGroupPlugins.Commands
{
   [System.Runtime.InteropServices.Guid("ea399879-09aa-4cbc-9f75-2f1711d03b4c")]
   public class FormingToolCommand : Command
   {
      static FormingToolCommand _instance;
      public FormingToolCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static FormingToolCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "FormingTool"; }
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

         List<RhinoObject> rhinoObjectList = new List<RhinoObject>();
         List<ArcCurve> arcCurveList = new List<ArcCurve>();

         // Loop through all the objects to find Text
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

                     if(!holeSizeList.Exists(element => element == curve.Radius) )
                     {
                        holeSizeList.Add(curve.Radius);
                     }

                     arcCurveList.Add(curve);
                     // rhinoObjectList.Add(rhinoObject);
                  }
               }
            }
         }

         holeSizeList.Sort();

         if (holeSizeList.Count < 1)
         {
            return Result.Failure;
         }

         double maxHole  = holeSizeList.Max();
         double minHole = holeSizeList.Min();
      
         foreach(double size in holeSizeList)
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

         // Create a new layer 
         string layerName = "FormTool";

         // Does a layer with the same name already exist?
         int layerIndex = doc.Layers.Find(layerName, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
         }

         doc.Layers.SetCurrentLayerIndex(layerIndex, true);

         foreach(ArcCurve ac in arcCurveList)
         {
            double angle = 0;

            sizeAngle.TryGetValue(ac.Radius, out angle);

            drawFormTool(ac.Arc.Center.X, ac.Arc.Center.Y, angle*Math.PI/180);
         }

         doc.Views.Redraw();

         return Result.Success;
      }

      /// <summary>
      /// Draws the cave tool.
      /// </summary>
      /// <param name="cx">The cx.</param>
      /// <param name="cy">The cy.</param>
      /// <param name="angleRad">The angle RAD.</param>
      public void drawFormTool(double cx, double cy, double angleRad)
      {
         Guid toolGuid = new Guid();
         Transform xform = Transform.Rotation(angleRad, new Point3d(cx, cy, 0));
         Transform sixtyDeg = Transform.Rotation(60*Math.PI / 180, new Point3d(cx, cy, 0));
         Transform oneTwentyDeg = Transform.Rotation(120 * Math.PI / 180, new Point3d(cx, cy, 0));
         Transform mSixtyDeg = Transform.Rotation(-60 * Math.PI / 180, new Point3d(cx, cy, 0));
         Transform mOneTwentyDeg = Transform.Rotation(-120 * Math.PI / 180, new Point3d(cx, cy, 0));

         // draw the rectangle
         Point3d pt = new Point3d(cx, cy, 0);
         
         Line top  = new Line(cx-1.5, cy + 24.2487 + 1, 0, cx +1.5, cy + 24.2487 + 1, 0);
         Line right = new Line(cx + 1.5, cy + 24.2487 + 1, 0, cx + 1.5, cy + 24.2487 - 1, 0);
         Line bottom = new Line(cx + 1.5, cy + 24.2487 - 1, 0, cx - 1.5, cy + 24.2487 - 1, 0);
         Line left = new Line(cx - 1.5, cy + 24.2487 - 1, 0, cx - 1.5, cy + 24.2487 + 1, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(top);
         polyCurve.Append(right);
         polyCurve.Append(bottom);
         polyCurve.Append(left);
         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, sixtyDeg, true);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, oneTwentyDeg, true);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, mSixtyDeg, true);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, mOneTwentyDeg, true);
         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);

         //Point3d topPoint = new Point3d(cx + 4.5,  cy+8, 0);
         //Point3d bottomPoint = new Point3d(cx + 4.5, cy-8, 0);

         //Arc topArc = new Arc(new Circle(topPoint, 1.5), Math.PI);
         //topArc.StartAngle = 0;
         //topArc.EndAngle = Math.PI;

         //Arc bottomArc = new Arc(new Circle(bottomPoint, 1.5), Math.PI);
         //bottomArc.StartAngle = Math.PI;
         //bottomArc.EndAngle = 2* Math.PI;

         //Line left = new Line(cx+3, cy+8,0, cx+3,cy-8,0);
         //Line right = new Line(cx+6, cy-8,0,cx+6,cy+8,0);

         //polyCurve = new PolyCurve();

         //polyCurve.Append(topArc);
         //polyCurve.Append(left);
         //polyCurve.Append(bottomArc);
         //polyCurve.Append(right);

         // Replace it with Triangle instead
         Line topLeft = new Line(cx+15, cy-23.7487, 0, cx, cy-23.7487-11, 0);
         Line topRight = new Line(cx,cy - 23.7487 - 11, 0, cx -15, cy - 23.7487,0);
         bottom = new Line(cx - 15, cy - 23.7487, 0, cx + 15, cy - 23.7487, 0);
         polyCurve = new PolyCurve();
         polyCurve.Append(topLeft);
         polyCurve.Append(topRight);
         polyCurve.Append(bottom);
         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);
      }
   }
}
