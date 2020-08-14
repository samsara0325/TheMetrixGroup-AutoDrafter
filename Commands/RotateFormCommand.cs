using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace MetrixGroupPlugins.Commands
{
   [System.Runtime.InteropServices.Guid("9e91f9f5-2c78-4d53-a51d-b13272f316ae"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]


   /**
    * This class contains the code for adding custom cave tools (author - Wilfred)
    * */
   public class RotateFormCommand : Command
   {
      static RotateFormCommand _instance;
      public RotateFormCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static RotateFormCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "RotateForm"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // Check the selected dot
         GetObject go = new GetObject();

         // Create a new dictionary of strings, with string keys.
         //
         Dictionary<int, int> sizeAngle = new Dictionary<int, int>();
         List<int> holeSizeList = new List<int>();

         //Setting up for hole selection
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

                     if (!holeSizeList.Exists(element => element == curve.Radius))
                     {
                        holeSizeList.Add(Convert.ToInt32(curve.Radius)); //add unique hole sizes
                     }

                     arcCurveList.Add(curve);
                  }
               }
            }
         }

         holeSizeList.Sort();
         
         int maxHole = Convert.ToInt32 (holeSizeList.Max()); //get the maximum hole size in the list
         int minHole = Convert.ToInt32(holeSizeList.Min()); //get the minimum hole size in the list

         double maximumRotation = (360 - (360 / holeSizeList.Count)); //equation to calculate the maximum rotation
         int indexCOunt = 0;
         foreach (int size in holeSizeList) //for each hole size in the list, calculate the angle of rotation
         {
            int angle;
            if ((maxHole - minHole) != 0)
            {               
               angle = Convert.ToInt32(indexCOunt * (360 / holeSizeList.Count));
               indexCOunt++;
            }
            else
            {
               angle = 0;
            }

            sizeAngle.Add(size, angle); //assign the angle for each hole size
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

         String location = getCaveToolLocation(); //get the location of the cave tool (request from user)
         if (location != null)
         {
            //for each hole found (selected by user)
            //start drawing the cave tool
            foreach (ArcCurve ac in arcCurveList)
            {
               int angle = 0;
               //pass the hole size and get the angle  specific to the hole size
               sizeAngle.TryGetValue(Convert.ToInt32(ac.Radius), out angle);
               //draw the cave tool
               drawCaveImageTool(ac.Arc.Center.X, ac.Arc.Center.Y, angle, layerIndex, location);
            }
         }
         return Result.Success;
      }

      public void drawCaveImageTool(double cx, double cy, double angleRad, int currentLayerIndex, string location)
      {
         String layerOne = "SURFACES TRIM"; //Layer names found in the imported drawing rhino
         String layerTwo = "SURFACES 2"; 
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         List<RhinoObject> imported = new List<RhinoObject>();
         List<RhinoObject> transformed = new List<RhinoObject>();
         String loggedUser = Environment.UserName;
      
        // String fileLocation = "C:\\Users\\"+ loggedUser+"\\Documents\\RotateForm_Rhino_Document\\ROTATEFORM.3dm";

       
         String fileImportScript = String.Format("_-Import \"{0}\" _Enter", location); //import the cave image
         RhinoApp.RunScript(fileImportScript, true); //run the script
         
         //Import the objects in the layers
         foreach (RhinoObject obj in doc.Objects.FindByLayer(layerOne)){
            imported.Add(obj); //import all objects found in the layer1 of the imported cave tool
         }
         foreach (RhinoObject obj in doc.Objects.FindByLayer(layerTwo)){
            imported.Add(obj); //import all objects found in layer2 of the imported cave tool
         }

     
         //Find the location of the hole
         var locationOfCircle = new Vector3d(cx,cy,0);

     

            foreach (RhinoObject objRef in imported) //Move the objects to the location of the hole
         {
            RhinoObject testObj = objRef;
            var xform = Transform.Translation(locationOfCircle);          
            doc.Objects.Transform(objRef, xform, true);
            transformed.Add(doc.Objects.Find(objRef.Id));
         }
         
            //Select objects from the 2 layers 
         Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer(layerOne); //selecting objects in layer 1
         
         for (int i = 0; i < rhobjs.Length; i++)
         {
            rhobjs[i].Select(true);
         }
         
         rhobjs = doc.Objects.FindByLayer(layerTwo); //selecting objects in layer 2 

         for (int y = 0; y < rhobjs.Length; y++)
         {
            rhobjs[y].Select(true);
         }
         
         //Rotate all the selected objects (using the script)
         RhinoApp.RunScript("_-rotate " + cx + "," + cy + " " + angleRad, true);
         
         foreach (RhinoObject objRef in transformed) //Move the objects to the location of the hole
         {
            objRef.Attributes.LayerIndex = currentLayerIndex; //change the layerindex of the holes
            objRef.CommitChanges();
         }
         //delete the original layers of the hole and their objects
         RhinoUtilities.deleteLayer(layerOne);
         RhinoUtilities.deleteLayer(layerTwo);

      }

      //This method prompts the user to retrieve the location of the cave tool
      //Returns the location of the cave tool as a string format
      public static String getCaveToolLocation()
      {
         //Open the File dialog to get the file location of cave image
         OpenFileDialog choofdlog = new OpenFileDialog();
         choofdlog.Filter = "3dm Files (*.3dm)|*.3dm";
         choofdlog.FilterIndex = 1;
         choofdlog.Multiselect = true;

         if (choofdlog.ShowDialog() == DialogResult.OK)
         {
            return choofdlog.FileName;                   
         }
         return null;
      }

   }
}
