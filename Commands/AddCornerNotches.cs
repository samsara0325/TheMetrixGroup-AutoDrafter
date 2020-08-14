using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Bullzip.PdfWriter;
using System.IO;
using System.Windows.Forms;
using MetrixGroupPlugins.MessageBoxes;
using Rhino.Input;
using Rhino.DocObjects;
using Rhino.Geometry;
using MetrixGroupPlugins.Utilities;
using MetrixGroupPlugins.Resources;

namespace MetrixGroupPlugins.Commands
{
    //Class implements the functionality to create a pdf with watermark

    [CommandStyle(Style.ScriptRunner)]
    [System.Runtime.InteropServices.Guid("201E743C-BEF1-4DD5-BF98-653810D7B194")]
    public class AddCornerNotches : Command
    {

        public AddCornerNotches()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static AddCornerNotches Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "AddCornerNotches"; }
        }


        ///<summary>The only instance of this command.</summary>
        ///<param name="doc" RhinoDoc></param>
        ///<param name="mode" Run mode></param>
        ///<returns>returns sucess if doc is successfully created </returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Double reliefHoleSize = 5;
            double offsetFromeEdge = 0.5;
            double foldLineOffset = 5;
            double perimeterOffset = 60;
            int layerIndex = 0;
            double diameter = 5;
            double generalSetback = offsetFromeEdge;
            Rhino.DocObjects.Layer parent_layer_Approval;
            Rhino.DocObjects.Layer parent_layer_Nesting;
            String layerName;
            Curve curve;
            List<Guid> guidList = new List<Guid>();



            Rhino.Input.Custom.GetObject go = new Rhino.Input.Custom.GetObject();


            //double foldsLine = Properties.Settings.Default.PanelBorderDefault; // Default borderSize is 50

            Form1 notch = new Form1();
            notch.ShowDialog();
            reliefHoleSize = notch.getholeSize();
            offsetFromeEdge = notch.getfoldSetback();
            foldLineOffset = notch.getfoldOffset();
            perimeterOffset = notch.getperiOffset();
            go.SetCommandPrompt("Select panels to create Relief Holes and perpendicular Lines");

            // set up the options
            Rhino.Input.Custom.OptionDouble reliefHoleSizes = new Rhino.Input.Custom.OptionDouble(foldLineOffset);

            go.AddOptionDouble("Relief Hole Size", ref reliefHoleSizes);
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

            //Creating layer called "Layers for Approval Drawings" to make it a parent layer 
            layerName = "LAYERS FOR APPROVAL DRAWINGS";
            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
                parent_layer_Approval = doc.Layers[layerIndex]; //set the layer as parent layer
            }
            else
            {
                parent_layer_Approval = doc.Layers[layerIndex];
            }

            //Creating layer called "Layers for Nesting" to make it a parent layer 
            layerName = "LAYERS FOR NESTING";
            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
                parent_layer_Nesting = doc.Layers[layerIndex]; //set the layer as parent layer
            }
            else
            {
                parent_layer_Nesting = doc.Layers[layerIndex];
            }
            // Create a new layer called FOLDS
            layerName = "FOLDS";
            layerIndex = createSubLayers.createSubLayer(layerName,
                  System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Nesting layer the parent layer
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            while (true)
            {
                go.ClearCommandOptions();
                //reliefHoleSize = new Rhino.Input.Custom.OptionDouble(t);
                go.AddOptionDouble("Relief Hole Size", ref reliefHoleSizes);
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
                    foldLineOffset = go.Number();
                    continue;
                }
                else if (result != GetResult.Object)
                {
                    return Result.Cancel;
                }

                if (go.ObjectsWerePreselected)
                {
                    go.EnablePreSelect(true, true);
                }

                break;
            }

            int objecTCount = go.ObjectCount;


            foreach (ObjRef objRef in go.Objects())
            {
                try
                {
                    curve = objRef.Curve();

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

                    // Create a temp FOLDS
                    layerName = "tempFOLDS";
                    layerIndex = createSubLayers.createSubLayer(layerName,
                          System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Nesting layer the parent layer
                    doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                    // Add Folds Line in to the diagram
                    Plane plane = Rhino.Geometry.Plane.WorldXY;
                    Curve[] offsetCurves;
                    offsetCurves = curve.Offset(plane, -foldLineOffset, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp); //Set the folds line to be drawn with the panel perimeter
                                                                                                                           //Check if the curve is outside border and border is a positive 
                    if (curve.Contains(offsetCurves[0].PointAtStart, Plane.WorldXY, 0) == PointContainment.Outside && foldLineOffset > 0)
                    {
                        offsetCurves = curve.Offset(plane, foldLineOffset, 0.1, Rhino.Geometry.CurveOffsetCornerStyle.Sharp); //if true, then try to set the curve to be within the border
                    }

                    foreach (Curve c in offsetCurves)
                    {
                        doc.Objects.AddCurve(c); //add the objects to the doc to be displayed
                    }

                    //Check if the select panel is rectangular or raked
                    BoundingBox checkPerimeter = curve.GetBoundingBox(Plane.WorldXY);
                    Point3d perimeterMin = checkPerimeter.Min;
                    Point3d perimeterMax = checkPerimeter.Max;

                    doc.Objects.UnselectAll();
                    Boolean pointCheck = true;
                    //Check if the bounding box coordinates are present in the selected panel
                    if (curve.Contains(new Point3d(perimeterMin.X, perimeterMax.Y, 0)) == PointContainment.Outside
                       || curve.Contains(new Point3d(perimeterMax.X, perimeterMax.Y, 0)) == PointContainment.Outside
                       || curve.Contains(new Point3d(perimeterMin.X, perimeterMin.Y, 0)) == PointContainment.Outside
                       || curve.Contains(new Point3d(perimeterMax.X, perimeterMin.Y, 0)) == PointContainment.Outside)
                    {
                        pointCheck = false;
                    }

                    //If point check is true, it mean it is a rectangular panel
                    if (pointCheck)
                    {
                        //Select all objects on Folds Layer
                        Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer("tempFOLDS");


                        //Copy tempFolds to Folds
                        doc.Layers.SetCurrentLayerIndex(doc.Layers.Find("FOLDS", true), true);
                        //  Copy objects back to Folds layer
                        foreach (var selected_object in doc.Objects.FindByLayer("tempFOLDS"))
                        {
                            selected_object.Attributes.LinetypeIndex = doc.Linetypes.Find("Dashed", true);
                            selected_object.Attributes.LayerIndex = doc.Layers.Find("FOLDS", false);
                            selected_object.CommitChanges();
                        }

                        //Loop through all the selected folds layers
                        foreach (RhinoObject rhinoObject in rhinoObjs)
                        {
                            ObjRef rjObj = new ObjRef(rhinoObject);
                            curve = rjObj.Curve();


                            BoundingBox boundingBox = curve.GetBoundingBox(true);
                            Point3d min = boundingBox.Min;
                            Point3d max = boundingBox.Max;
                            diameter = reliefHoleSize / 2;

                            layerName = "FIXING HOLES";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                            //add the corner reliefs
                            doc.Objects.AddCircle(new Circle(new Point3d(max.X, max.Y, 0), diameter)); //TR
                            doc.Objects.AddCircle(new Circle(new Point3d(min.X, max.Y, 0), diameter)); //TL
                            doc.Objects.AddCircle(new Circle(new Point3d(max.X, min.Y, 0), diameter)); //BR
                            doc.Objects.AddCircle(new Circle(new Point3d(min.X, min.Y, 0), diameter)); //BL



                            layerName = "PANEL PERIMETER";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                            //off set from folds line edge and draw the perpendicular line
                            //Top Left 
                            //Left
                            doc.Objects.AddLine(new Point3d(min.X + generalSetback, max.Y - offsetFromeEdge, 0), new Point3d((min.X + generalSetback) - perimeterOffset, max.Y - offsetFromeEdge, 0));  // horizontal line
                                                                                                                                                                                                        //Top
                            doc.Objects.AddLine(new Point3d(min.X + offsetFromeEdge, max.Y - generalSetback, 0), new Point3d(min.X + offsetFromeEdge, (max.Y - generalSetback) + perimeterOffset, 0));  // Vertical Line

                            //Top Right
                            //Right
                            doc.Objects.AddLine(new Point3d(max.X - generalSetback, max.Y - offsetFromeEdge, 0), new Point3d((max.X - generalSetback) + perimeterOffset, max.Y - offsetFromeEdge, 0));  // horizontal line
                                                                                                                                                                                                        //Top
                            doc.Objects.AddLine(new Point3d(max.X - offsetFromeEdge, max.Y - generalSetback, 0), new Point3d(max.X - offsetFromeEdge, (max.Y - generalSetback) + perimeterOffset, 0));  // Vertical line


                            //Bottom Left
                            //Left
                            doc.Objects.AddLine(new Point3d(min.X + generalSetback, min.Y + offsetFromeEdge, 0), new Point3d((min.X + generalSetback) - perimeterOffset, min.Y + offsetFromeEdge, 0));  // horizontal line
                                                                                                                                                                                                        //Bottom
                            doc.Objects.AddLine(new Point3d(min.X + offsetFromeEdge, min.Y + generalSetback, 0), new Point3d(min.X + offsetFromeEdge, (min.Y + generalSetback) - perimeterOffset, 0));  // Vertical line


                            //Bottom Right
                            //Right
                            doc.Objects.AddLine(new Point3d(max.X - generalSetback, min.Y + offsetFromeEdge, 0), new Point3d((max.X - generalSetback) + perimeterOffset, min.Y + offsetFromeEdge, 0));  // horizontal line
                            doc.Objects.AddLine(new Point3d(max.X - offsetFromeEdge, (min.Y + generalSetback), 0), new Point3d(max.X - offsetFromeEdge, (min.Y + generalSetback) - perimeterOffset, 0));  // Vertical line

                            //Adding the Folds Lines
                            //Top Left and Top Right                 
                            doc.Objects.AddLine(new Point3d(min.X + offsetFromeEdge, (max.Y - generalSetback) + perimeterOffset, 0), new Point3d(max.X - offsetFromeEdge, (max.Y - generalSetback) + perimeterOffset, 0));  // Vertical Line

                            //Bottom Left and Bottom Right
                            doc.Objects.AddLine(new Point3d(min.X + offsetFromeEdge, (min.Y + generalSetback) - perimeterOffset, 0), new Point3d(max.X - offsetFromeEdge, (min.Y + generalSetback) - perimeterOffset, 0));  // Vertical line

                            //Left Top and Left Bottom
                            doc.Objects.AddLine(new Point3d((min.X + generalSetback) - perimeterOffset, max.Y - offsetFromeEdge, 0), new Point3d((min.X + generalSetback) - perimeterOffset, min.Y + offsetFromeEdge, 0));  // horizontal line

                            //Right Top and Right Bottom
                            doc.Objects.AddLine(new Point3d((max.X - generalSetback) + perimeterOffset, max.Y - offsetFromeEdge, 0), new Point3d((max.X - generalSetback) + perimeterOffset, min.Y + offsetFromeEdge, 0));  // horizontal line

                        }
                    } //If false means the panel is not rectangular
                    else
                    {
                        RhinoObject top;
                        RhinoObject bottom;
                        RhinoObject left;
                        RhinoObject right;

                        Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer("tempFOLDS");
                        Point3d min = new Point3d(0, 0, 0);
                        Point3d max = new Point3d(0, 0, 0);
                        //Loop through all the selected folds layers
                        foreach (RhinoObject rhinoObject in rhinoObjs)
                        {
                            ObjRef rjObj = new ObjRef(rhinoObject);
                            curve = rjObj.Curve();


                            BoundingBox boundingBox = curve.GetBoundingBox(true); //get the bounding box for the select folds layer
                            min = boundingBox.Min;
                            max = boundingBox.Max;
                            diameter = reliefHoleSize / 2;

                        }

                        int layerNumber = doc.Layers.Find("tempFOLDS", true);
                       

                        RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
                        //Unjoin all
                        RhinoApp.RunScript("Explode", true);
                        rhinoObjs = doc.Objects.FindByLayer("tempFOLDS"); //get the folds objects (individual sides)


                        //Declare variables to calculate sides and identify lines 
                        double distanceToTL = 0;
                        double distanceToTR = 0;
                        Point3d TL = new Point3d(min.X, max.Y, 0);
                        Point3d TR = new Point3d(max.X, max.Y, 0);
                        Curve topLine = null;


                        double distanceToBL = 0;
                        double distanceToBR = 0;
                        Point3d BL = new Point3d(min.X, min.Y, 0);
                        Point3d BR = new Point3d(max.X, min.Y, 0);
                        Curve bottomLine = null;

                        double distanceToLT = 0;
                        double distanceToLB = 0;
                        Point3d LT = new Point3d(min.X, max.Y, 0);
                        Point3d LB = new Point3d(min.X, min.Y, 0);
                        Curve leftLine = null;


                        double distanceToRT = 0;
                        double distanceToRB = 0;
                        Point3d RT = new Point3d(max.X, max.Y, 0);
                        Point3d RB = new Point3d(max.X, min.Y, 0);
                        Curve rightLine = null;

                        //end of declaring variables 

                        //Check if more than 4, if join the panel back, simplify the panel to remove duplicates.
                        if (rhinoObjs.Length > 4)
                        {
                            //throw new Exception();
                            //Explode the folds layer
                            RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
                            RhinoApp.RunScript("_Join ", true);
                            RhinoApp.RunScript("_SimplifyCrv", true);
                            RhinoApp.RunScript("Explode", true);
                            rhinoObjs = doc.Objects.FindByLayer("tempFOLDS"); //get the folds objects (individual sides)
                        }
                        //Check again if the  number of objects are more than 4 
                        if (rhinoObjs.Length > 4)
                        {
                            throw new Exception();  //throw exception if more than 4
                        }
                        //For 3 sided shapes execute this 
                        if (rhinoObjs.Length < 4)
                        {

                            //Left Hand side
                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToLT == 0 && distanceToLB == 0)
                                {
                                    distanceToLT = curve.PointAtStart.DistanceTo(LT);
                                    distanceToLB = curve.PointAtEnd.DistanceTo(LB);
                                }
                                if (distanceToLT >= curve.PointAtEnd.DistanceTo(LT))
                                {
                                    distanceToLT = curve.PointAtEnd.DistanceTo(LT);
                                }
                                if (distanceToLT >= curve.PointAtStart.DistanceTo(LT))
                                {
                                    distanceToLT = curve.PointAtStart.DistanceTo(LT);
                                }
                                if (distanceToLB >= curve.PointAtEnd.DistanceTo(LB))
                                {
                                    distanceToLB = curve.PointAtEnd.DistanceTo(LB);
                                }
                                if (distanceToLB >= curve.PointAtStart.DistanceTo(LB))
                                {
                                    distanceToLB = curve.PointAtStart.DistanceTo(LB);
                                }

                                if (distanceToLT >= curve.PointAtEnd.DistanceTo(LT) && distanceToLB >= curve.PointAtStart.DistanceTo(LB) ||
                                    distanceToLT >= curve.PointAtStart.DistanceTo(LT) && distanceToLB >= curve.PointAtEnd.DistanceTo(LB))
                                {
                                    leftLine = curve;
                                }
                            }


                            //Right Hand side
                            foreach (RhinoObject rjObs in rhinoObjs)
                            {

                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToRT == 0 && distanceToRB == 0)
                                {
                                    distanceToRT = curve.PointAtStart.DistanceTo(RT);
                                    distanceToRB = curve.PointAtEnd.DistanceTo(RB);
                                }
                                if (distanceToRT >= curve.PointAtEnd.DistanceTo(RT))
                                {
                                    distanceToRT = curve.PointAtEnd.DistanceTo(RT);
                                }
                                if (distanceToRT >= curve.PointAtStart.DistanceTo(RT))
                                {
                                    distanceToRT = curve.PointAtStart.DistanceTo(RT);
                                }
                                if (distanceToRB >= curve.PointAtEnd.DistanceTo(RB))
                                {
                                    distanceToRB = curve.PointAtEnd.DistanceTo(RB);
                                }
                                if (distanceToRB >= curve.PointAtStart.DistanceTo(RB))
                                {
                                    distanceToRB = curve.PointAtStart.DistanceTo(RB);
                                }

                                if (distanceToRT >= curve.PointAtEnd.DistanceTo(RT) && distanceToRB >= curve.PointAtStart.DistanceTo(RB) ||
                                    distanceToRT >= curve.PointAtStart.DistanceTo(RT) && distanceToRB >= curve.PointAtEnd.DistanceTo(RB))
                                {
                                    rightLine = curve;
                                }
                            }

                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToBL == 0 && distanceToBR == 0)
                                {
                                    distanceToBL = curve.PointAtStart.DistanceTo(BL);
                                    distanceToBR = curve.PointAtEnd.DistanceTo(BR);
                                }
                                if (distanceToBR >= curve.PointAtEnd.DistanceTo(BR))
                                {
                                    distanceToBR = curve.PointAtEnd.DistanceTo(BR);
                                }
                                if (distanceToBR >= curve.PointAtStart.DistanceTo(BR))
                                {
                                    distanceToBR = curve.PointAtStart.DistanceTo(BR);
                                }
                                if (distanceToBL >= curve.PointAtEnd.DistanceTo(BL))
                                {
                                    distanceToBL = curve.PointAtEnd.DistanceTo(BL);
                                }
                                if (distanceToBL >= curve.PointAtStart.DistanceTo(BL))
                                {
                                    distanceToBL = curve.PointAtStart.DistanceTo(BL);
                                }

                                if (distanceToBR >= curve.PointAtEnd.DistanceTo(BR) && distanceToBL >= curve.PointAtStart.DistanceTo(BL) ||
                                    distanceToBR >= curve.PointAtStart.DistanceTo(BR) && distanceToBL >= curve.PointAtEnd.DistanceTo(BL))
                                {
                                    bottomLine = curve;
                                }
                            }

                            if (leftLine == null)
                            {
                                foreach (RhinoObject rjObs in rhinoObjs)
                                {
                                    ObjRef rjObj = new ObjRef(rjObs);
                                    curve = rjObj.Curve();
                                    if (!bottomLine.PointAtStart.Equals(curve.PointAtStart) && !bottomLine.PointAtEnd.Equals(curve.PointAtEnd) &&
                                        !rightLine.PointAtStart.Equals(curve.PointAtStart) && !rightLine.PointAtEnd.Equals(curve.PointAtEnd))
                                    {
                                        leftLine = curve;
                                    }
                                }
                            }

                            if (rightLine == null)
                            {
                                foreach (RhinoObject rjObs in rhinoObjs)
                                {
                                    ObjRef rjObj = new ObjRef(rjObs);
                                    curve = rjObj.Curve();
                                    if (!bottomLine.PointAtStart.Equals(curve.PointAtStart) && !bottomLine.PointAtEnd.Equals(curve.PointAtEnd) &&
                                        !leftLine.PointAtStart.Equals(curve.PointAtStart) && !leftLine.PointAtEnd.Equals(curve.PointAtEnd))
                                    {
                                        rightLine = curve;
                                    }
                                }
                            }

                            if (bottomLine == null)
                            {
                                foreach (RhinoObject rjObs in rhinoObjs)
                                {
                                    ObjRef rjObj = new ObjRef(rjObs);
                                    curve = rjObj.Curve();
                                    if (!rightLine.PointAtStart.Equals(curve.PointAtStart) && !rightLine.PointAtEnd.Equals(curve.PointAtEnd) &&
                                        !leftLine.PointAtStart.Equals(curve.PointAtStart) && !leftLine.PointAtEnd.Equals(curve.PointAtEnd))
                                    {
                                        rightLine = curve;
                                    }
                                }
                            }

                            //Copy tempFolds to Folds
                            doc.Layers.SetCurrentLayerIndex(doc.Layers.Find("FOLDS", true), true);
                            //  Copy objects back to Folds layer
                            foreach (var selected_object in doc.Objects.FindByLayer("tempFOLDS"))
                            {
                                selected_object.Attributes.LinetypeIndex = doc.Linetypes.Find("Dashed", true);
                                selected_object.Attributes.LayerIndex = doc.Layers.Find("FOLDS", false);
                                selected_object.CommitChanges();
                            }

                            //Set the layer index as fixing holes and Add the corner reliefs
                            layerName = "FIXING HOLES";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);


                            diameter = reliefHoleSize / 2;
                            //Adds the corner reliefs 
                            if (leftLine.PointAtStart == rightLine.PointAtStart || leftLine.PointAtStart == rightLine.PointAtEnd)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(leftLine.PointAtStart.X, leftLine.PointAtStart.Y, 0), diameter));
                            }
                            if (leftLine.PointAtEnd == rightLine.PointAtEnd || leftLine.PointAtEnd == rightLine.PointAtStart)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(leftLine.PointAtEnd.X, leftLine.PointAtEnd.Y, 0), diameter));
                            }

                            if (leftLine.PointAtStart == bottomLine.PointAtStart || leftLine.PointAtStart == bottomLine.PointAtEnd)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(leftLine.PointAtStart.X, leftLine.PointAtStart.Y, 0), diameter));
                            }

                            if (leftLine.PointAtEnd == bottomLine.PointAtEnd || leftLine.PointAtEnd == bottomLine.PointAtStart)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(leftLine.PointAtEnd.X, leftLine.PointAtEnd.Y, 0), diameter));
                            }

                            if (rightLine.PointAtStart == bottomLine.PointAtStart || rightLine.PointAtStart == bottomLine.PointAtEnd)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(rightLine.PointAtStart.X, rightLine.PointAtStart.Y, 0), diameter));
                            }

                            if (rightLine.PointAtEnd == bottomLine.PointAtEnd || rightLine.PointAtEnd == bottomLine.PointAtStart)
                            {
                                doc.Objects.AddCircle(new Circle(new Point3d(rightLine.PointAtEnd.X, rightLine.PointAtEnd.Y, 0), diameter));
                            }


                            //Set the layer as panel perimeter
                            layerName = "PANEL PERIMETER";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                            //Calculate the angles for the fold lines
                            Vector3d bottomV = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction;
                            Vector3d leftV = new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction;
                            Vector3d rightV = new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction;

                            //double topAngle = Rhino.Geometry.Vector3d.VectorAngle(topV, rightV);
                            double bottomAngle = Rhino.Geometry.Vector3d.VectorAngle(bottomV, leftV);
                            double leftAngle = Rhino.Geometry.Vector3d.VectorAngle(leftV, bottomV);
                            double rightAngle = Rhino.Geometry.Vector3d.VectorAngle(rightV, bottomV);

                            //Calculate bottom angle
                            bottomAngle = bottomAngle * (180 / Math.PI);
                            bottomAngle = 180 - bottomAngle;
                            //Calculate left angle
                            leftAngle = leftAngle * (180 / Math.PI);
                            leftAngle = 180 - leftAngle; //Reduce by 180 to get the actual angle
                                                         //Calculate right angle
                            rightAngle = rightAngle * (180 / Math.PI);
                            rightAngle = 180 - rightAngle;


                            Line tempBL = new Line();
                            Line tempBR = new Line();
                            Line tempLT = new Line();
                            Line tempLB = new Line();
                            Line tempRT = new Line();
                            Line tempRB = new Line();
                            Point3d topInter = new Point3d();
                            Point3d BLInter = new Point3d();
                            Point3d BRInter = new Point3d();

                            //Get the intersect point of the line
                            tempBL = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd);
                            tempBL.Length = offsetFromeEdge;
                            tempBR = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd);
                            tempBR.Length = tempBR.Length - offsetFromeEdge;

                            tempLT = new Line(leftLine.PointAtStart, leftLine.PointAtEnd);
                            tempLT.Length = offsetFromeEdge;
                            tempLB = new Line(leftLine.PointAtStart, leftLine.PointAtEnd);
                            tempLB.Length = tempLB.Length - offsetFromeEdge;

                            tempRT = new Line(rightLine.PointAtStart, rightLine.PointAtEnd);
                            tempRT.Length = offsetFromeEdge;
                            tempRB = new Line(rightLine.PointAtStart, rightLine.PointAtEnd);
                            tempRB.Length = tempRB.Length - offsetFromeEdge;

                            //Find the direction of the perpendicular lines
                            Plane helperPlane = new Plane(Plane.WorldXY);
                            Vector3d direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempBL = new Line(tempBL.To, direction, perimeterOffset);


                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempBR = new Line(tempBR.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempLT = new Line(tempLT.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempLB = new Line(tempLB.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempRT = new Line(tempRT.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempRB = new Line(tempRB.To, direction, perimeterOffset);


                            // Add the perpendicular lines
                            doc.Objects.AddLine(tempBL);

                            doc.Objects.AddLine(tempBR);

                            doc.Objects.AddLine(tempLT);

                            doc.Objects.AddLine(tempLB);

                            doc.Objects.AddLine(tempRT);

                            doc.Objects.AddLine(tempRB);

                            // Add the folds
                            doc.Objects.AddLine(tempLT.To, tempLB.To);
                            doc.Objects.AddLine(tempBL.To, tempBR.To);
                            doc.Objects.AddLine(tempRT.To, tempRB.To);

                            tempBL = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd);
                            tempBL.Length = offsetFromeEdge;
                            tempBR = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd);
                            tempBR.Length = tempBR.Length - offsetFromeEdge;

                            tempLT = new Line(leftLine.PointAtStart, leftLine.PointAtEnd);
                            tempLT.Length = offsetFromeEdge;
                            tempLB = new Line(leftLine.PointAtStart, leftLine.PointAtEnd);
                            tempLB.Length = tempLB.Length - offsetFromeEdge;

                            tempRT = new Line(rightLine.PointAtStart, rightLine.PointAtEnd);
                            tempRT.Length = tempRT.Length - offsetFromeEdge;
                            tempRB = new Line(rightLine.PointAtStart, rightLine.PointAtEnd);
                            tempRB.Length = offsetFromeEdge;

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempBL = new Line(tempBL.To, direction, 5);


                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempBR = new Line(tempBR.To, direction, 5);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempLT = new Line(tempLT.To, direction, 5);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempLB = new Line(tempLB.To, direction, 5);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempRT = new Line(tempRT.To, direction, 5);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempRB = new Line(tempRB.To, direction, 5);


                            //Find intersecting points in the setback
                            double a, b;
                            ////Top    
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempLT, tempRT, out a, out b); //get intersection point for top line left
                            topInter = tempLT.PointAt(a);

                            ////Bottom Left
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempLB, tempBL, out a, out b); //get intersection point for top line left
                            BLInter = tempBL.PointAt(a);

                            //Bottom Right
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempRB, tempBR, out a, out b); //get intersection point for top line left
                            BRInter = tempBR.PointAt(a);

                            ////Add the setbacks 
                            //Bottom Left
                            doc.Objects.AddLine(new Line(BLInter, tempLB.From));
                            doc.Objects.AddLine(new Line(BLInter, tempBL.From));
                            //Bottom Right
                            doc.Objects.AddLine(new Line(BRInter, tempRB.From));
                            doc.Objects.AddLine(new Line(BRInter, tempBR.From));

                            //Top
                            doc.Objects.AddLine(new Line(topInter, tempLT.From));
                            doc.Objects.AddLine(new Line(topInter, tempRT.From));

                        }

                        //For 4 sided 
                        if (rhinoObjs.Length == 4)
                        {
                            //Checking top left and top right

                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToTL == 0 && distanceToTR == 0)
                                {
                                    distanceToTL = curve.PointAtStart.DistanceTo(TL);
                                    distanceToTR = curve.PointAtEnd.DistanceTo(TR);
                                }
                                if (distanceToTR >= curve.PointAtEnd.DistanceTo(TR))
                                {
                                    distanceToTR = curve.PointAtEnd.DistanceTo(TR);
                                }
                                if (distanceToTR >= curve.PointAtStart.DistanceTo(TR))
                                {
                                    distanceToTR = curve.PointAtStart.DistanceTo(TR);
                                }
                                if (distanceToTL >= curve.PointAtEnd.DistanceTo(TL))
                                {
                                    distanceToTL = curve.PointAtEnd.DistanceTo(TL);
                                }
                                if (distanceToTL >= curve.PointAtStart.DistanceTo(TL))
                                {
                                    distanceToTL = curve.PointAtStart.DistanceTo(TL);
                                }

                                if (distanceToTR >= curve.PointAtEnd.DistanceTo(TR) && distanceToTL >= curve.PointAtStart.DistanceTo(TL) ||
                                    distanceToTR >= curve.PointAtStart.DistanceTo(TR) && distanceToTL >= curve.PointAtEnd.DistanceTo(TL))
                                {
                                    topLine = curve;
                                }


                            }

                            //Check Bottom Left and Bottom Right

                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToBL == 0 && distanceToBR == 0)
                                {
                                    distanceToBL = curve.PointAtStart.DistanceTo(BL);
                                    distanceToBR = curve.PointAtEnd.DistanceTo(BR);
                                }
                                if (distanceToBR >= curve.PointAtEnd.DistanceTo(BR))
                                {
                                    distanceToBR = curve.PointAtEnd.DistanceTo(BR);
                                }
                                if (distanceToBR >= curve.PointAtStart.DistanceTo(BR))
                                {
                                    distanceToBR = curve.PointAtStart.DistanceTo(BR);
                                }
                                if (distanceToBL >= curve.PointAtEnd.DistanceTo(BL))
                                {
                                    distanceToBL = curve.PointAtEnd.DistanceTo(BL);
                                }
                                if (distanceToBL >= curve.PointAtStart.DistanceTo(BL))
                                {
                                    distanceToBL = curve.PointAtStart.DistanceTo(BL);
                                }

                                if (distanceToBR >= curve.PointAtEnd.DistanceTo(BR) && distanceToBL >= curve.PointAtStart.DistanceTo(BL) ||
                                    distanceToBR >= curve.PointAtStart.DistanceTo(BR) && distanceToBL >= curve.PointAtEnd.DistanceTo(BL))
                                {
                                    bottomLine = curve;
                                }
                            }


                            //Check Left Top and Left  Bottom

                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToLT == 0 && distanceToLB == 0)
                                {
                                    distanceToLT = curve.PointAtStart.DistanceTo(LT);
                                    distanceToLB = curve.PointAtEnd.DistanceTo(LB);
                                }
                                if (distanceToLT >= curve.PointAtEnd.DistanceTo(LT))
                                {
                                    distanceToLT = curve.PointAtEnd.DistanceTo(LT);
                                }
                                if (distanceToLT >= curve.PointAtStart.DistanceTo(LT))
                                {
                                    distanceToLT = curve.PointAtStart.DistanceTo(LT);
                                }
                                if (distanceToLB >= curve.PointAtEnd.DistanceTo(LB))
                                {
                                    distanceToLB = curve.PointAtEnd.DistanceTo(LB);
                                }
                                if (distanceToLB >= curve.PointAtStart.DistanceTo(LB))
                                {
                                    distanceToLB = curve.PointAtStart.DistanceTo(LB);
                                }

                                if (distanceToLT >= curve.PointAtEnd.DistanceTo(LT) && distanceToLB >= curve.PointAtStart.DistanceTo(LB) ||
                                    distanceToLT >= curve.PointAtStart.DistanceTo(LT) && distanceToLB >= curve.PointAtEnd.DistanceTo(LB))
                                {
                                    leftLine = curve;
                                }
                            }



                            //Check Right Top and Right Bottom
                            foreach (RhinoObject rjObs in rhinoObjs)
                            {
                                ObjRef rjObj = new ObjRef(rjObs);
                                curve = rjObj.Curve();


                                if (distanceToRT == 0 && distanceToRB == 0)
                                {
                                    distanceToRT = curve.PointAtStart.DistanceTo(RT);
                                    distanceToRB = curve.PointAtEnd.DistanceTo(RB);
                                }
                                if (distanceToRT >= curve.PointAtEnd.DistanceTo(RT))
                                {
                                    distanceToRT = curve.PointAtEnd.DistanceTo(RT);
                                }
                                if (distanceToRT >= curve.PointAtStart.DistanceTo(RT))
                                {
                                    distanceToRT = curve.PointAtStart.DistanceTo(RT);
                                }
                                if (distanceToRB >= curve.PointAtEnd.DistanceTo(RB))
                                {
                                    distanceToRB = curve.PointAtEnd.DistanceTo(RB);
                                }
                                if (distanceToRB >= curve.PointAtStart.DistanceTo(RB))
                                {
                                    distanceToRB = curve.PointAtStart.DistanceTo(RB);
                                }

                                if (distanceToRT >= curve.PointAtEnd.DistanceTo(RT) && distanceToRB >= curve.PointAtStart.DistanceTo(RB) ||
                                    distanceToRT >= curve.PointAtStart.DistanceTo(RT) && distanceToRB >= curve.PointAtEnd.DistanceTo(RB))
                                {
                                    rightLine = curve;
                                }
                            }


                            //Identify the start and end points of all the lines
                            Line tLine = new Line(); //Represent top line
                            Line bLine = new Line(); //Represent bottom line
                            Line lLine = new Line(); //Represent left line
                            Line rLine = new Line(); //Represent right line




                            //Top Line
                            if (topLine.PointAtStart == leftLine.PointAtStart && topLine.PointAtEnd == rightLine.PointAtStart ||
                               topLine.PointAtStart == leftLine.PointAtStart && topLine.PointAtEnd == rightLine.PointAtEnd ||
                               topLine.PointAtStart == leftLine.PointAtEnd && topLine.PointAtEnd == rightLine.PointAtStart ||
                               topLine.PointAtStart == leftLine.PointAtEnd && topLine.PointAtEnd == rightLine.PointAtEnd)
                            {
                                tLine = new Line(topLine.PointAtStart, topLine.PointAtEnd);
                            }

                            if (topLine.PointAtStart == rightLine.PointAtStart && topLine.PointAtEnd == leftLine.PointAtStart ||
                               topLine.PointAtStart == rightLine.PointAtStart && topLine.PointAtEnd == leftLine.PointAtEnd ||
                               topLine.PointAtStart == rightLine.PointAtEnd && topLine.PointAtEnd == leftLine.PointAtEnd ||
                               topLine.PointAtStart == rightLine.PointAtEnd && topLine.PointAtEnd == leftLine.PointAtStart)
                            {
                                tLine = new Line(topLine.PointAtEnd, topLine.PointAtStart);
                            }
                            //top line length 0 means there was a problem with that line
                            if (tLine.Length == 0)
                            {
                                throw new Exception();
                            }
                            //Left Line
                            if (tLine.From == leftLine.PointAtStart)
                            {
                                lLine = new Line(leftLine.PointAtStart, leftLine.PointAtEnd);
                            }
                            else
                            {
                                lLine = new Line(leftLine.PointAtEnd, leftLine.PointAtStart);
                            }

                            //Right Line
                            if (tLine.To == rightLine.PointAtStart)
                            {
                                rLine = new Line(rightLine.PointAtStart, rightLine.PointAtEnd);
                            }
                            else
                            {
                                rLine = new Line(rightLine.PointAtEnd, rightLine.PointAtStart);
                            }

                            //Bottom Line
                            if (lLine.To == bottomLine.PointAtStart)
                            {
                                bLine = new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd);
                            }
                            else
                            {
                                bLine = new Line(bottomLine.PointAtEnd, bottomLine.PointAtStart);
                            }

                            //Calculate the angles for the fold lines

                            Vector3d topV = tLine.Direction;
                            Vector3d bottomV = bLine.Direction;
                            Vector3d leftV = lLine.Direction;
                            Vector3d rightV = rLine.Direction;

                            double topAngle = Rhino.Geometry.Vector3d.VectorAngle(topV, rightV);
                            double bottomAngle = Rhino.Geometry.Vector3d.VectorAngle(bottomV, leftV);
                            double leftAngle = Rhino.Geometry.Vector3d.VectorAngle(leftV, bottomV);
                            double rightAngle = Rhino.Geometry.Vector3d.VectorAngle(rightV, bottomV);
                            //Calculate top angle
                            topAngle = topAngle * (180 / Math.PI);
                            topAngle = 180 - topAngle; //Reduce by 180 to get the actual angle
                                                       //Calculate bottom angle
                            bottomAngle = bottomAngle * (180 / Math.PI);
                            bottomAngle = 180 - bottomAngle;
                            //Calculate left angle
                            leftAngle = leftAngle * (180 / Math.PI);
                            leftAngle = 180 - leftAngle; //Reduce by 180 to get the actual angle
                                                         //Calculate right angle
                            rightAngle = rightAngle * (180 / Math.PI);
                            rightAngle = 180 - rightAngle;


                            diameter = reliefHoleSize / 2;


                            //Copy tempFolds to Folds
                            doc.Layers.SetCurrentLayerIndex(doc.Layers.Find("FOLDS", true), true);
                            //  Copy objects back to Folds layer
                            foreach (var selected_object in doc.Objects.FindByLayer("tempFOLDS"))
                            {
                                selected_object.Attributes.LinetypeIndex = doc.Linetypes.Find("Dashed", true);
                                selected_object.Attributes.LayerIndex = doc.Layers.Find("FOLDS", false);
                                selected_object.CommitChanges();
                            }

                            //Set the layer index as fixing holes and Add the corner reliefs
                            layerName = "FIXING HOLES";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
                            //add the corner reliefs
                            doc.Objects.AddCircle(new Circle(new Point3d(tLine.From.X, tLine.From.Y, 0), diameter)); //TR
                            doc.Objects.AddCircle(new Circle(new Point3d(tLine.To.X, tLine.To.Y, 0), diameter)); //TL
                            doc.Objects.AddCircle(new Circle(new Point3d(bLine.From.X, bLine.From.Y, 0), diameter)); //BR
                            doc.Objects.AddCircle(new Circle(new Point3d(bLine.To.X, bLine.To.Y, 0), diameter)); //BL

                            //Set the layer as panel perimeter
                            layerName = "PANEL PERIMETER";
                            layerIndex = createSubLayers.createSubLayer(layerName,
                                  System.Drawing.Color.Black, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer
                            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                            Line tempTop = new Line();
                            Line tempBottom = new Line();
                            Line tempTopR = new Line();
                            Line tempBottomR = new Line();
                            Line tempLeftT = new Line();
                            Line tempLeftB = new Line();
                            Line tempRightT = new Line();
                            Line tempRightB = new Line();

                            Point3d topInterL = new Point3d();
                            Point3d topInterR = new Point3d();
                            Point3d bottomInterL = new Point3d();
                            Point3d bottomInterR = new Point3d();
                            Point3d leftInterT = new Point3d();
                            Point3d leftInterB = new Point3d();
                            Point3d RightInterT = new Point3d();
                            Point3d RightInterB = new Point3d();


                            //Calculating the point in the folds line with setback 

                            //left hand side of top line and bottom line
                            tempTop = tLine;
                            tempTop.Length = offsetFromeEdge;
                            topInterL = tempTop.To;

                            tempBottom = bLine;
                            tempBottom.Length = offsetFromeEdge;
                            bottomInterL = tempBottom.To;

                            //right handside of top line and bottom line 
                            tempTopR = tLine;
                            tempTopR.Length = tempTopR.Length - offsetFromeEdge;
                            topInterR = tempTopR.To;

                            tempBottomR = bLine;
                            tempBottomR.Length = tempBottomR.Length - offsetFromeEdge;
                            bottomInterR = tempBottomR.To;

                            //Left Line top and bottom
                            tempLeftT = lLine;
                            tempLeftT.Length = offsetFromeEdge;
                            leftInterT = tempLeftT.To;

                            tempLeftB = lLine;
                            tempLeftB.Length = tempLeftB.Length - offsetFromeEdge;
                            leftInterB = tempLeftB.To;

                            //right line top and bottom
                            tempRightT = rLine;
                            tempRightT.Length = offsetFromeEdge;
                            RightInterT = tempRightT.To;

                            tempRightB = rLine;
                            tempRightB.Length = tempRightB.Length - offsetFromeEdge;
                            RightInterB = tempRightB.To;

                            //Calculating the 90 degree angle for the points
                            Plane helperPlane = new Plane(Plane.WorldXY);
                            Vector3d direction = new Vector3d(tempTop.Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempTop = new Line(tempTop.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempBottom.Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempBottom = new Line(tempBottom.To, direction, perimeterOffset);

                            //Right hand side
                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempTopR.Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempTopR = new Line(tempTopR.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempBottomR.Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempBottomR = new Line(tempBottomR.To, direction, perimeterOffset);

                            //Test
                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempLeftT.Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempLeftT = new Line(tempLeftT.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempLeftB.Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempLeftB = new Line(tempLeftB.To, direction, perimeterOffset);

                            //Right Line Top and Bottom
                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempRightT.Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempRightT = new Line(tempRightT.To, direction, perimeterOffset);

                            helperPlane = new Plane(Plane.WorldXY);
                            direction = new Vector3d(tempRightB.Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempRightB = new Line(tempRightB.To, direction, perimeterOffset);



                            //Add Perpendicular lines
                            //Top Left
                            doc.Objects.AddLine(tempTop);
                            doc.Objects.AddLine(tempLeftT);
                            //Top Right
                            doc.Objects.AddLine(tempTopR);
                            doc.Objects.AddLine(tempRightT);

                            //Bottom Left
                            doc.Objects.AddLine(tempBottom);
                            doc.Objects.AddLine(tempLeftB);
                            //Bottom Right
                            doc.Objects.AddLine(tempBottomR);
                            doc.Objects.AddLine(tempRightB);

                            //Adding Folds
                            //Top Left and Top Right
                            doc.Objects.AddLine(new Line(tempTop.To, tempTopR.To));
                            //Bottom left and Bottom Right
                            doc.Objects.AddLine(new Line(tempBottom.To, tempBottomR.To));
                            //Left Top and Left Bottom
                            doc.Objects.AddLine(new Line(tempLeftT.To, tempLeftB.To));
                            //Right Top and Right Bottom
                            doc.Objects.AddLine(new Line(tempRightT.To, tempRightB.To));

                            //Find intersecting points in the setback
                            double a, b;

                            //Top Line Left
                            direction = new Vector3d(new Line(topLine.PointAtStart, topLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempTop = new Line(tempTop.From, direction, perimeterOffset);

                            //Left Line Top
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempLeftT = new Line(tempLeftT.From, direction, perimeterOffset);

                            //Top Line Right
                            direction = new Vector3d(new Line(topLine.PointAtStart, topLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempTopR = new Line(tempTopR.From, direction, perimeterOffset);

                            //Right Line Top
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempRightT = new Line(tempRightT.From, direction, perimeterOffset);


                            //BottomLine Left
                            direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempBottom = new Line(tempBottom.From, direction, perimeterOffset);
                            //Left Line Bottom
                            direction = new Vector3d(new Line(leftLine.PointAtStart, leftLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(270), helperPlane.ZAxis);
                            tempLeftB = new Line(tempLeftB.From, direction, perimeterOffset);

                            //BottomLine Right
                            direction = new Vector3d(new Line(bottomLine.PointAtStart, bottomLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempBottomR = new Line(tempBottomR.From, direction, perimeterOffset);
                            //Right Line Bottom
                            direction = new Vector3d(new Line(rightLine.PointAtStart, rightLine.PointAtEnd).Direction);
                            direction.Rotate(RhinoMath.ToRadians(90), helperPlane.ZAxis);
                            tempRightB = new Line(tempRightB.From, direction, perimeterOffset);


                            //Top  Left Intersecting point  
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempTop, tempLeftT, out a, out b); //get intersection point for top line left
                            topInterL = tempTop.PointAt(a);

                            //Top Right Intersecting Point 
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempTopR, tempRightT, out a, out b); //get intersection point for top line left
                            topInterR = tempTopR.PointAt(a);

                            //Bottom Left Intersecting point  
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempBottom, tempLeftB, out a, out b); //get intersection point for top line left
                            bottomInterL = tempBottom.PointAt(a);

                            //Bottom Right Intersecting point  
                            Rhino.Geometry.Intersect.Intersection.LineLine(tempBottomR, tempRightB, out a, out b); //get intersection point for top line left
                            bottomInterR = tempBottomR.PointAt(a);

                            //Add the setbacks
                            //Top Left And Left Top
                            doc.Objects.AddLine(new Line(topInterL, tempTop.From));
                            doc.Objects.AddLine(new Line(topInterL, tempLeftT.From));

                            //Top Right And Right Top
                            doc.Objects.AddLine(new Line(topInterR, tempTopR.From));
                            doc.Objects.AddLine(new Line(topInterR, tempRightT.From));

                            //Bottom Left And Left Bottom
                            doc.Objects.AddLine(new Line(bottomInterL, tempBottom.From));
                            doc.Objects.AddLine(new Line(bottomInterL, tempLeftB.From));

                            //Bottom Right And Right Bottom
                            doc.Objects.AddLine(new Line(bottomInterR, tempBottomR.From));
                            doc.Objects.AddLine(new Line(bottomInterR, tempRightB.From));

                        }
                        MetrixUtilities.joinCurves(doc.Layers.Find("PANEL PERIMETER", true));
                    }
                }
                catch (Exception e) //if exception occurs, continue adding notches to rest of the panels
                {
                    doc.Layers.SetCurrentLayerIndex(doc.Layers.Find("PANEL PERIMETER", true), true);
                    deletetempFolds();
                    continue;
                }

            }
            deletetempFolds();
            doc.Views.Redraw();
            return Result.Success;
        }

        //Method deletes the temp folds layer and all of its objects
        public void deletetempFolds()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer("tempFOLDS");

            if (rhobjs != null)
            {
                if (rhobjs.Length > 0)
                {
                    for (int i = 0; i < rhobjs.Length; i++)
                        doc.Objects.Delete(rhobjs[i], true);
                }
            }

            doc.Layers.Delete(doc.Layers.Find("tempFOLDS", false), true);
        }

    }


}

