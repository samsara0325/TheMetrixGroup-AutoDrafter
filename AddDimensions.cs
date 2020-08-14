using MetrixGroupPlugins.Utilities;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins
{

   /**
    * @author-Wilfred
    * This class contains the methods to draw horizontal and vertica dimensions in panels 
    * */
   class AddDimensions
   {
      Rhino.Input.Custom.GetObject go = null;
      Rhino.Geometry.Point3d pt;
      Point3d offset;
      Point3d origin;
      Plane plane = Rhino.Geometry.Plane.WorldXY;
      RhinoDoc doc;
      Boolean dimensionsCreated = false;
      public Boolean createDimensions(List<Guid> guidList)
      {
         doc = RhinoDoc.ActiveDoc;
         go = new Rhino.Input.Custom.GetObject();
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
         go.SetCommandPrompt("Select panels to create dimensions");
         while (true)
         {

            go.ClearCommandOptions();
          
            // perform the get operation. This will prompt the user to select the list of curves, but also
            // allow for command line options defined above
            GetResult result = go.GetMultiple(1, 0);

            if (result == GetResult.Option)
            {
               go.EnablePreSelect(false, true);
               continue;
            }
            else if (result == GetResult.Number)
            {
              
               continue;
            }
            else if (result != GetResult.Object)
            {
             
            }

            if (go.ObjectsWerePreselected)
            {
               go.EnablePreSelect(true, true);
               break;
            }

            break;
         }

         int objecTCount = go.ObjectCount;

         foreach (ObjRef objRef in go.Objects()) //Go through each curve  in the objects list
         {
            Curve curve = objRef.Curve();

            // If curve is null, means objRef is not a curve
            if (curve == null)
            {
               dimensionsCreated = false;
               continue;
            }

            // If curve is not Closed Curve
            if (curve.IsClosed == false)
            {
               RhinoApp.WriteLine(objRef.ToString() + " curve is open");
               dimensionsCreated = false;
               continue;
            }

            if (curve.IsPlanar() == false)
            {
               RhinoApp.WriteLine(objRef.ToString() + " curve is not planar");
               dimensionsCreated = false;
               continue;
            }
            MetrixUtilities.createMetrixRealDimension();
            BoundingBox boundingBox = curve.GetBoundingBox(Plane.WorldXY);
            Point3d min = boundingBox.Min;
            Point3d max = boundingBox.Max;

            //Add Horizontal dimension
            origin = new Point3d(min.X, max.Y ,0);  
            offset = new Point3d(max.X, max.Y, 0); 
            pt = new Point3d((offset.X - origin.X) / 2, max.Y + 180, 0);
            plane = Plane.WorldXY;
            plane.Origin = origin;
            guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension


            //Add vertical Dimensions
            origin = new Point3d(min.X, min.Y, 0);
            offset = new Point3d(min.X, max.Y, 0); //left 
            pt = new Point3d(min.X - 180, (offset.Y - origin.Y) / 2, 0);
            plane = Plane.WorldXY;
            plane.XAxis = new Vector3d(0, -1, 0); //-1 to rotate the dimension vertically
            plane.YAxis = new Vector3d(-1, 0, 0);
            plane.ZAxis = new Vector3d(0, 0, -1);
            plane.Origin = origin;
            guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

            dimensionsCreated = true;
         }
         doc.Views.Redraw(); //finally redraw to refresh the screen and display dimensions to user
         return dimensionsCreated;
      }
      //Method used to draw the dimension based on the provided parameters and it then adds to the guid provided.
      //The guid is returned back to the main method with the added dimension
      public static List<Guid> drawDimension(Plane plane, Point3d pt, Point3d offset, Point3d origin, List<Guid> guidList, RhinoDoc doc)
      {
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
         return guidList;
      }
   }
}
