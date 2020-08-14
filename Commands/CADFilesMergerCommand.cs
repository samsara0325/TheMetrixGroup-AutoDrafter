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
using Excel = Microsoft.Office.Interop.Excel;
using MetrixGroupPlugins.Utilities;
using MetrixGroupPlugins.MessageBoxes;


namespace MetrixGroupPlugins
{
    [
       System.Runtime.InteropServices.Guid("9aef8a8a-1cf0-457f-8e15-226bb924a9bb"),
       Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
    ]
    public class CADFilesMergerCommand : Command
    {
        public CADFilesMergerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CADFilesMergerCommand Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "CADFilesMerger"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Open file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = doc.Path;
            openFileDialog.Filter = "3dm files (*.3dm)|*.3dm| dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "CAD files to merge";

            DialogResult dr = openFileDialog.ShowDialog();
            uint firstSN, lastSN;
            string sScript;
            BoundingBox bbox;
            string panelDetails = "";

            double importMinX, importMinY;
            double importMaxX, importMaxY;
            double globalMinX, globalMinY;
            double globalMaxX, globalMaxY;
            double gap = 500;
            Boolean executedOnce = false;
            double commonminY = 0;
            double commonminX = 0;
            double commonmaxY = 0;
            List<RhinoObject> imported = new List<RhinoObject>(); //holds the imported rhino obects (all objects)
            List<RhinoObject> oldObjs = new List<RhinoObject>(); //holds the old objects in the document 
            List<RhinoObject> newObjs = new List<RhinoObject>(); //holds the new objects in the document
            Boolean newObjFound = false;
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                //Select direction and angle
                double rotationAngle = 0;
                bool anticlockwise = true;
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                DialogResult result = MessageBox.Show("Do you want to rotate each dxf imported by 90°?", "DXF rotation", buttons);
                if (result == DialogResult.Yes)
                {
                    rotationAngle = 90;
                    result = MessageBox.Show("Select the rotation direction: 'Yes' for 'Anti-clockwise' and 'No' for 'Clockwise'.", "Rotation Direction", buttons);
                    if (result == DialogResult.No)
                    {
                        rotationAngle = - 90;
                        anticlockwise = false;
                    }
                }



                


                // Read the files 
                foreach (String file in openFileDialog.FileNames)
                {

                    importMinX = Double.MaxValue;
                    importMinY = Double.MaxValue;
                    importMaxX = Double.MinValue;
                    importMaxY = Double.MinValue;
                    globalMinX = Double.MaxValue;
                    globalMinY = Double.MaxValue;
                    globalMaxX = Double.MinValue;
                    globalMaxY = Double.MinValue;

                    // Import each file
                    try
                    {
                        sScript = String.Format("! _-Import \"{0}\" _Enter", file);

                        firstSN = RhinoObject.NextRuntimeSerialNumber;
                        RhinoApp.RunScript(sScript, false);
                        lastSN = RhinoObject.NextRuntimeSerialNumber; //Investigate later
                        List<Guid> ids = new List<Guid>();
                        imported = new List<RhinoObject>();
                        newObjs = new List<RhinoObject>();
                        foreach (RhinoObject obj in doc.Objects)
                        {
                            if (obj.RuntimeSerialNumber >= firstSN && obj.RuntimeSerialNumber < lastSN)
                            {
                                imported.Add(obj);
                                ids.Add(obj.Id);

                                bbox = obj.Geometry.GetBoundingBox(Plane.WorldXY);

                                if (importMaxX < bbox.Max.X)
                                {
                                    importMaxX = bbox.Max.X;
                                }

                                if (importMaxY < bbox.Max.Y)
                                {
                                    importMaxY = bbox.Max.Y;
                                }

                                if (importMinX > bbox.Min.X)
                                {
                                    importMinX = bbox.Min.X;
                                }

                                if (importMinY > bbox.Min.Y)
                                {
                                    importMinY = bbox.Min.Y;
                                }




                            }
                            else
                            {
                                //Global
                                bbox = obj.Geometry.GetBoundingBox(Plane.WorldXY);

                                if (globalMaxX < bbox.Max.X)
                                {
                                    globalMaxX = bbox.Max.X;
                                }

                                if (globalMaxY < bbox.Max.Y)
                                {
                                    globalMaxY = bbox.Max.Y;
                                }

                                if (globalMinX > bbox.Min.X)
                                {
                                    globalMinX = bbox.Min.X;
                                }

                                if (globalMinY > bbox.Min.Y)
                                {
                                    globalMinY = bbox.Min.Y;
                                }
                            }
                        }

                        if (!executedOnce) //execute this only once 
                        {
                            commonminY = importMinY;
                            commonminX = importMinX;
                            commonmaxY = importMaxY;
                        }

                        var startPoint = new Point3d(importMinX, importMinY, 0);

                        if (globalMaxX == Double.MinValue)
                        {
                            globalMaxX = 0;
                        }
                        if (globalMaxY == Double.MinValue)
                        {
                            globalMaxY = 0;
                        }
                        if (globalMinX == Double.MaxValue)
                        {
                            globalMinX = 0;
                        }
                        if (globalMinY == Double.MaxValue)
                        {
                            globalMinY = 0;
                        }


                        var endPoint = new Point3d(globalMaxX + gap, commonminY, 0);

                        List<string> names = new List<string>();
                        //find which type of perimeter
                        for(int i = 0; i < 100; i++)
                        {
                            if (doc.Layers.FindIndex(i) != null)
                            {
                                names.Add(doc.Layers.FindIndex(i).Name);
                            }
                            else
                            {
                                break;
                            }
                        }

                        /*
                        String perimeterName = "Default";
                        //Get the correct perimeter name 
                        if (doc.Layers.Find("LAYERS_FOR_NESTING$PANEL_P", true) >= 0)
                        {
                            perimeterName = "LAYERS_FOR_NESTING$PANEL_P";
                        }

                        if (doc.Layers.Find("Panel_Perimeter", true) >= 0)
                        {
                            perimeterName = "Panel_Perimeter";
                        }  */

                        int layer_index = doc.Layers.Find("PANEL PERIMETER", true);
                        if (layer_index < 0)
                        {
                            // Add a new layer to the document
                            layer_index = doc.Layers.Add("PANEL PERIMETER", System.Drawing.Color.Black);
                        }

                        
                        foreach (string perimeterName in names)
                        {
                            foreach (RhinoObject obj in doc.Objects.FindByLayer(perimeterName))
                            {
                                ObjRef objR = new ObjRef(obj);
                                if (objR.Curve() == null)
                                {
                                    continue;
                                }
                                //loop through the old objects array to find if the object is found in the array
                                foreach (RhinoObject rhinObj in oldObjs)
                                {
                                    if (rhinObj.Id.Equals(obj.Id)) //if the ids are equals it means the object belongs to a panel which has already been rotated
                                    {
                                        newObjFound = true; //set to true
                                        break; //exit from this second loop
                                    }
                                }
                                if (!newObjFound) //go in only if the newObjectfound is false 
                                {
                                    newObjs.Add(obj); //add the new object to the array
                                }
                                newObjFound = false; //set the variable back to false
                            }
                        }
                        


                        // Move all the objects 
                        var xform = Transform.Translation(endPoint - startPoint);
                        foreach (var objRef in imported)
                        {
                            doc.Objects.Transform(objRef, xform, true);
                        }

                        executedOnce = true;

                        Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(globalMaxX + gap, (commonminY + commonmaxY) / 2, 0);
                        Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                        plane.Origin = pt;
                        String dimension = findLeastDimension(newObjs, plane, "PANEL PERIMETER", rotationAngle); //call the method and pass the parameters to find the min X and min Y for the panel


                        // Add the file as the label

                        //The codes below are to create red labels for each panel

                        double height = (importMaxY - importMinY) * 0.1;
                        //double height = 3;
                        string label = Path.GetFileNameWithoutExtension(file);
                        int layerIndex = doc.Layers.CurrentLayerIndex;

                        RhinoUtilities.SetActiveLayer("LABELS", System.Drawing.Color.Red);

                        const string font = "Arial";
                        plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                        plane.Origin = pt;
                         Guid labelGuid = doc.Objects.AddText(label, plane, height, font, false, false);

                        ids.Add(labelGuid);
                        imported.Add(doc.Objects.Find(labelGuid));

                        doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                        //panelDetails = panelDetails + label + "\t" + (importMaxX - importMinX) + "\t" + (importMaxY - importMinY) + "\n";
                        panelDetails = panelDetails + label + "#" + dimension + "\n"; //set the dimension with the panel name                                                                                                                                                                                                                                                                             vv



                        //int index = doc.Groups.Add(ids);

                        oldObjs.AddRange(newObjs);
                    }
                    catch (Exception ex)
                    {
                        // Could not load the image - probably related to Windows file system permissions.
                        MessageBox.Show(ex.Message);
                    }


                }


                double labelHeight = 100;
                Rhino.Geometry.Point3d panelDetailsPt = new Rhino.Geometry.Point3d(0, 0 + labelHeight, 0);
                const string labelFont = "Arial";
                Rhino.Geometry.Plane panelDetailsPlane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
                panelDetailsPlane.Origin = panelDetailsPt;
                doc.Objects.AddText(panelDetails, panelDetailsPlane, labelHeight, labelFont, false, false);

                doc.Layers.Delete(doc.Layers.Find("temp", false), true);

                exportToExcel(panelDetails); //call to export panel dimensions to excel      
            }

            Messages.excelOperationComplete();

            // Export the whole lot
            //string command = string.Format("-_Export \"" + Path.GetDirectoryName(doc.Path) + @"\" + labelName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
            // Export the selected curves
            //RhinoApp.RunScript(command, true);
            doc.Views.Redraw();
            return Result.Success;
        }

        public String findLeastDimension(List<RhinoObject> newObjs, Plane plane, String perimeterName,  double rotationAngle)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            double minX = 0;
            double minY = 0;
            double minArea = double.MaxValue;
            Layer layerIndex = doc.Layers.FindName("Default");
            RhinoUtilities.SetActiveLayer("temp", System.Drawing.Color.Black);
            //Copy Objects of current layer to temp layer
            foreach (var selected_object in newObjs)
            {
                selected_object.Attributes.LayerIndex = doc.Layers.Find("temp", false);
                selected_object.CommitChanges();
            }
            //select objects in the temporart layer and join
            RhinoApp.RunScript("SelNone", true);
            RhinoApp.RunScript("SelLayerNumber " + doc.Layers.Find("temp", true), false);
            //MetrixUtilities.joinCurves(doc.Layers.Find("temp", false));


            //plane.Rotate(i, plane.XAxis); //rotateshape with the value of i
            RhinoObject[] objs = doc.Objects.FindByLayer("temp");
            List<Curve> curveList = new List<Curve>();
            Curve[] curveArray = null;
            RhinoObject obj = objs[0];

                foreach (RhinoObject objt in objs)
                {
                    curveList.Add(new ObjRef(objt).Curve());
                }
                curveArray = Curve.JoinCurves(curveList);
            



                obj.Attributes.ToString();
                ObjRef objR = new ObjRef(obj);
                Curve curve = objR.Curve();
            double curvelength = -1;
            Curve baseline = null;
            double angle = 0;
            foreach(Curve cv in curveList)
            {
                if (cv.GetLength() > curvelength)
                {
                    curvelength = cv.GetLength();
                    baseline = cv;
                    angle = Math.Atan(baseline.TangentAtStart.Y / baseline.TangentAtStart.X);
                }
            }
            
            if (curveArray != null)
            {
                double maxArea = Double.MinValue;
                foreach(Curve tempCurve in curveArray)
                {
                    BoundingBox tempBBox = tempCurve.GetBoundingBox(true);
                    if(tempBBox.Area > maxArea)
                    {
                        curve = tempCurve;
                    }
                }
            }
            BoundingBox boundingBox = curve.GetBoundingBox(Plane.WorldXY);
                List<Point3d> points = new List<Point3d>();
                PolylineCurve polyline_curve = curve as PolylineCurve;
                double diagnal = 0;
            bool rectangle = false;
            Point3d centerC = boundingBox.Center;
            minArea = boundingBox.Area;
            minX = boundingBox.Max.X - boundingBox.Min.X;
            minY = boundingBox.Max.Y - boundingBox.Min.Y;
               
            /*
            if (polyline_curve != null && polyline_curve.PointCount == 5)
            {
                for (int j = 0; j < polyline_curve.PointCount; j++)
                {
                    points.Add(polyline_curve.Point(j));
                }

                if(Math.Round(points[0].DistanceTo(points[1])) == Math.Round(points[2].DistanceTo(points[3])))
                {
                    minX = points[0].DistanceTo(points[1]);
                    minY = points[1].DistanceTo(points[2]);
                    rectangle = true;
                }
                else
                {
                    rectangle = false;
                }
            }*/

            if (rectangle == false)
            {

                curve.Rotate(-angle, Plane.WorldXY.ZAxis, baseline.PointAtStart);
                boundingBox = curve.GetBoundingBox(Plane.WorldXY);
                if(boundingBox.Area < minArea)
                {
                    Point3d min = boundingBox.Min;
                    Point3d max = boundingBox.Max;
                    minX = max.X - min.X;
                    minY = max.Y - min.Y;
                }
            }


            RhinoApp.RunScript("-_Rotate " + centerC + " " + rotationAngle, false);


            //RhinoApp.RunScript("-_Rotate " + boundingBox.Center +" " + 5, false);



            //Set back the previous default layer
            RhinoUtilities.SetActiveLayer(perimeterName, System.Drawing.Color.Black);
            //Copy all object of temp layer back to the previous layer
            foreach (var selected_object in doc.Objects.FindByLayer("temp"))
            {
                selected_object.Attributes.LayerIndex = doc.Layers.FindByFullPath(perimeterName, false);
                selected_object.CommitChanges();
            }

            doc.Layers.SetCurrentLayerIndex(doc.Layers.FindByFullPath(layerIndex.Name, false), true);
            //needs to fix the tab issue
            return String.Format("{0}#{1}", Math.Max(minX, minY), Math.Min(minX, minY)); //return the minX and minY for the panel

        }



        //Method exports all panel dimensions to an excel sheet
        public void exportToExcel(string pDetails)
        {

            //Messages.showGeneratingExcel();

            //TextEntity textEntity = null;
            //String text = "";
            List<String> dimensions = new List<string>();
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            Rhino.DocObjects.RhinoObject[] go = doc.Objects.FindByLayer("Default"); //get all objects in default layer
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            //loop through the object array
            /*foreach (RhinoObject obj in go)
            {
               
                //check if the object type is annotation 
                if (obj.ObjectType == ObjectType.Annotation)
                {
                    textEntity = obj.Geometry as TextEntity; //get the text in the object
                    text = textEntity.PlainText;
                }
            }*/

            //Split the text string and put in to array
            String[] splitText = pDetails.Split(new char[] { '\t', '#', '\n' });
            //String[] splitText = text.Split(new Char[] {'#','\n'});
            //RhinoApp.WriteLine("{0}", splitText.Length);
            //foreach(string tex in splitText)
            //{
            //   RhinoApp.WriteLine(tex);
            //}

            xlexcel = new Excel.Application();
            xlexcel.Visible = true;

            //Add the new workbook
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = xlWorkBook.ActiveSheet;



            //Set sheet 1 as the active sheet
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);



            //Set the range
            xlWorkSheet.Cells[1, 1] = "Panel Name";
            xlWorkSheet.Cells[1, 2] = "Comment";
            xlWorkSheet.Cells[1, 3] = "Quantity";
            xlWorkSheet.Cells[1, 4] = "Length";
            xlWorkSheet.Cells[1, 5] = "X";
            xlWorkSheet.Cells[1, 6] = "Width";

            /*Clipboard.SetText("Comment");
            CR = (Excel.Range)xlWorkSheet.Cells[1, 2];
            xlWorkSheet.Paste(CR, false);  //paste the text

            Clipboard.SetText("Quantity");
            CR = (Excel.Range)xlWorkSheet.Cells[1, 3];
            xlWorkSheet.Paste(CR, false);  //paste the text

            Clipboard.SetText("Length");
            CR = (Excel.Range)xlWorkSheet.Cells[1, 4];
            xlWorkSheet.Paste(CR, false);  //paste the text

            Clipboard.SetText("X");
            CR = (Excel.Range)xlWorkSheet.Cells[1, 5];
            xlWorkSheet.Paste(CR, false);  //paste the text

            Clipboard.SetText("Width");
            CR = (Excel.Range)xlWorkSheet.Cells[1, 6];
            xlWorkSheet.Paste(CR, false);  //paste the text
            */

            //Loop and Add rows
            int columnCount = 1;
            int rowCount = 2; //row 1 is for headers 




            for (int i = 0; i < splitText.Length; i++)
            {

                //RhinoApp.WriteLine(i.ToString());
                if (splitText[i].Equals("")) //if tex is "" just continue the loop
                {
                    continue;
                }
                else
                {
                    if (columnCount == 2 || columnCount == 3)
                    {
                        columnCount = 4;
                    }
                    if (columnCount == 5)
                    {

                        columnCount = 6;
                    }

                    //Clipboard.SetText(splitText[i]); //copy the tex to the clipboard 

                    xlWorkSheet.Cells[rowCount, columnCount] = splitText[i];
                    //CR = (Excel.Range)xlWorkSheet.Cells[rowCount, columnCount]; //set the excel range 
                    //xlWorkSheet.Paste(CR, false); //paste to the work sheet 

                    columnCount++; //Increment column counter 


                    if (columnCount == 7) //if column counter is 4 it means the first row is now complete 
                    {
                        columnCount = 1; //change column count back to 1
                        rowCount++; //increment row count to start the next row 
                    }



                }
            }
           // String[] path = doc.Path.Split(new String[] { ".3dm" }, StringSplitOptions.None); //get the path of the rhino doc and split
            //xlWorkBook.SaveAs(path[0] + "Panel_Dimensions"); //save the workbook and close 

        }

    }
}
