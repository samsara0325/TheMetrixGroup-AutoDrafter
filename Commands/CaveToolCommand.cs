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
   [System.Runtime.InteropServices.Guid("ea399879-09aa-4cbc-9f75-2f1711d03b3a")]
   public class CaveToolCommand : Command
   {
      static CaveToolCommand _instance;
      public CaveToolCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static CaveToolCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "CaveTool"; }
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
         string layerName = "CaveTool";

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

            drawCaveTool(ac.Arc.Center.X, ac.Arc.Center.Y, angle*Math.PI/180);
         }

        

         return Result.Success;
      }

      /// <summary>
      /// Draws the cave tool.
      /// </summary>
      /// <param name="cx">The cx.</param>
      /// <param name="cy">The cy.</param>
      /// <param name="angleRad">The angle RAD.</param>
      public void drawCaveTool(double cx, double cy, double angleRad)
      {
         Guid toolGuid = new Guid();
         Transform xform = Transform.Rotation(angleRad, new Point3d(cx, cy, 0));

         // draw the top half circle
         Point3d pt = new Point3d(cx, cy, 0);
         //Arc topArc = new Arc(new Circle(pt, 10), Math.PI);
         //topArc.StartAngle = Math.PI/2;
         //topArc.EndAngle =  3*Math.PI/2;

         // Replace it with Triangle instead
         Line topLeft = new Line(cx, cy - 10, 0, cx - 10, cy, 0);
         Line topRight = new Line(cx - 10, cy, 0, cx, cy + 10, 0);
         Line bottom = new Line(cx, cy + 10, 0, cx, cy - 10, 0);
         PolyCurve polyCurve = new PolyCurve();
         polyCurve.Append(topLeft);
         polyCurve.Append(topRight);
         polyCurve.Append(bottom);
         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

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
         topLeft = new Line(cx + 4.5, cy + 8, 0, cx + 9.5, cy, 0);
         topRight = new Line(cx + 9.5, cy, 0, cx + 4.5, cy - 8, 0);
         bottom = new Line(cx + 4.5, cy - 8, 0, cx + 4.5, cy + 8, 0);
         polyCurve = new PolyCurve();
         polyCurve.Append(topLeft);
         polyCurve.Append(topRight);
         polyCurve.Append(bottom);
         toolGuid = RhinoDoc.ActiveDoc.Objects.Add(polyCurve);

         RhinoDoc.ActiveDoc.Objects.Transform(toolGuid, xform, true);
      }
   }
}
