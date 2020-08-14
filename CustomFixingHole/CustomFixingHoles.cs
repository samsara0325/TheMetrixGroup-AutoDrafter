using MetrixGroupPlugins.PunchingTools;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetrixGroupPlugins.CustomFixingHole
{

    public class CustomFixingHoles
    {
        /**
         * Method contains the logic to draw fixing holes evenly on the panel based on the user's criteria
         * */
        public static List<Guid> drawFixingFoles(PerforationPanel panel, FoldedPerforationPanel foldPanel, Boolean Folded, double panelBottom, double panelLeft, double panelY0, double panelY1, DimensionStyle dimStyle, List<Guid> guidList, double panelX0, double panelX1, double panelRight, double panelTop, double panelBottomBH, double panelRightHS, double panelLeftHS)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            Rhino.Geometry.Point3d pt;
            Point3d offset;
            Plane plane = Rhino.Geometry.Plane.WorldXY;
            Point3d origin;
            double pointOne = 0;
            //Execute the method only if atleast one of the side contain fixing holes
            if (panel.TopFixingHoles.Equals("1") || panel.BottomFixingHoles.Equals("1") || panel.LeftFixingHoles.Equals("1") || panel.RightFixingHoles.Equals("1"))
            {
                int foldToDraw = 1;
                Boolean methodExc = false;
                int tempLayerIndex = 0;

                double singleFoldMaxY = 0; //variable holds the maximum y of a single fold in a double fold
                double singleFoldMaxX = 0; //variable holds the maximum X of a single fold in a double fold
                double singleFoldMinY = 0; //variable holds the minimun y of a single fold in a double fold
                double singleFoldMinX = 0; //variable holds the minimun X of a single fold in a double fold

                Point3d topLeft = new Point3d(0, 0, 0);
                Point3d topRight = new Point3d(0, 0, 0);
                Point3d bottomLeft = new Point3d(0, 0, 0);
                Point3d bottomRight = new Point3d(0, 0, 0);

                Point3d pointTopLeft = new Point3d(0, 0, 0);
                Point3d pointTopRight = new Point3d(0, 0, 0);
                Point3d pointBottomLeft = new Point3d(0, 0, 0);
                Point3d pointBottomRight = new Point3d(0, 0, 0);

                tempLayerIndex = createSubLayers.createSubLayer("FIXING HOLES DIMENSIONS",
               System.Drawing.Color.DarkGreen, doc.Layers[doc.Layers.Find("LAYERS FOR APPROVAL DRAWINGS", true)]);

                Rhino.DocObjects.RhinoObject[] go = null;
                int layerIndex = createSubLayers.createSubLayer("FIXING HOLES",
               System.Drawing.Color.Black, doc.Layers[doc.Layers.Find("LAYERS FOR NESTING", true)]);
                doc.Layers.SetCurrentLayerIndex(layerIndex, true);



                go = doc.Objects.FindByLayer("PANEL PERIMETER");

                if (Folded)
                {
                    //if the panel is folded and the fixing holes are not going to be drawn on folds, then
                    //select folded panel finished layer as the bounding box to draw the fixing holes
                    if (!foldPanel.OnFolds)
                    {
                        go = doc.Objects.FindByLayer("FOLDED PANEL FINISHED");
                    }
                    foldToDraw = foldPanel.FoldWithFixingHoles;
                }
                double holeSize = panel.HoleDiameter;
                double totalTopFixingHoles = panel.TopFixingHoleQuantity;
                double totalBottomFixingHoles = panel.BottomFixingHoleQuantity;
                double totalLeftFixingHoles = panel.LeftFixingHoleQuantity;
                double totalRightFixingHoles = panel.RightFixingHoleQuantity;




                int objecTCount = go.Length;

                foreach (RhinoObject ob in go)
                {
                    ObjRef objRef = new ObjRef(ob);
                    Curve curve = objRef.Curve();

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
                    //this will ensure that the fixing holes and dimensions are only drawn once on a panel
                    if (methodExc)
                    {
                        continue;
                    }
                    methodExc = true;

                    // Find the boundary 
                    BoundingBox boundingBox = curve.GetBoundingBox(Plane.WorldXY);
                    Point3d min = boundingBox.Min;
                    Point3d max = boundingBox.Max;

                    List<Point3d> pointsList = new List<Point3d>();
                    double runningX = 0;
                    double runningY = 0;

                    runningY = max.Y - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)


                    if (Folded)
                    {
                        //If the user wants to draw on the first fold (Applicable only for double folds)
                        if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 1)
                        {
                            singleFoldMaxY = foldPanel.TopFirstFoldWidth - (foldPanel.KFactor * 2);
                            runningY = panelY1 + singleFoldMaxY - panel.TopHoleSetbackTop;
                        }
                        runningX = panelX0 + foldPanel.TopFirstFoldSetbackLeft + panel.TopHoleSetbackLeft;
                    }
                    else
                    {
                        runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                    }


                    if (Folded) //Implemented to cater 2 side foldings
                    {
                        //Catering Top and left folds  or top and right folds
                        if (foldPanel.TopFold == 1 && foldPanel.LeftFold == 1 && foldPanel.RightFold != 1 && foldPanel.BottomFold != 1 || foldPanel.TopFold == 1 && foldPanel.LeftFold != 1 && foldPanel.RightFold == 1 && foldPanel.BottomFold != 1)
                        {
                            runningY = ((panelY1 + foldPanel.TopFirstFoldWidth) - foldPanel.KFactor) - panel.TopHoleSetbackTop;
                            runningX = (panelX0 + foldPanel.TopFirstFoldSetbackLeft) + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                        }
                    }



                    int points = 1;
                    Point3d point;



                    //Draw the top holes
                    if (panel.TopFixingHoles.Equals("1"))
                    {
                        if (Folded)
                        {

                            points = 0;
                            // if (foldPanel != null) //If folded
                            {
                                point = new Point3d(panelX1 - panel.TopHoleSetbackRight - foldPanel.TopFirstFoldSetbackRight, runningY, 0); //adds the top right fixing hole                               
                            }
                            //else //If not flat
                            //{
                            //   point = new Point3d(panelX1 - foldPanel.RightFirstFoldSetbackBottom - foldPanel.KFactor - panel.TopHoleSetbackRight, runningY, 0); //adds the top right fixing hole
                            //}
                            pointTopRight = point; // get the point of top right fixing hole
                            pointsList.Add(point);
                            topRight = point;
                            point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                            pointsList.Add(point);
                            pointTopLeft = point; //get the point of top left fixing hole
                            topLeft = point;
                            while (points < (panel.TopFixingHoleQuantity - 1))
                            {
                                runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                                pointsList.Add(point);
                                points++;
                            }

                        }
                        else
                        {
                            point = new Point3d(max.X - panel.TopHoleSetbackRight, runningY, 0); //adds the top right fixing hole
                            pointsList.Add(point);
                            pointTopRight = point; //get the top right fixing hole position

                            point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                            pointsList.Add(point);
                            pointTopLeft = point; //get the top left fixing hole position

                            while (points <= (panel.TopFixingHoleQuantity - 1))
                            {

                                runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                                pointsList.Add(point);
                                points++;
                            }
                        }
                    }

                    //End of drawing top holes
                    //Draw the top horizontal fixing holes dimension (between the fixing holes)
                    doc.Layers.SetCurrentLayerIndex(tempLayerIndex, true);
                    if (panel.TopFixingHoles == "1")
                    {
                        if (Folded) //if folded
                        {
                            runningX = panelX0 + foldPanel.TopFirstFoldSetbackLeft + panel.TopHoleSetbackLeft;
                            if (!foldPanel.OnFolds)
                            {
                                runningY = ((panelY1 + foldPanel.TopFirstFoldWidth + (foldPanel.KFactor * 2)));
                            }
                        }
                        else //if flat
                        {
                            runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                        }

                        points = 0;
                        if (Folded) //If panel is folded 
                        {
                            while (points < (panel.TopFixingHoleQuantity) - 1)
                            {
                                origin = new Point3d(runningX + panel.TopFixingHoleDistance, runningY + 15, 0);  //right
                                offset = new Point3d(runningX, runningY + 15, 0); //left 
                                pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 4), 0);

                                plane = Plane.WorldXY;
                                plane.Origin = origin;

                                guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                                runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                points++;
                            }
                        }
                        else //if panel is flat
                        {
                            while (points < (panel.TopFixingHoleQuantity) - 1)
                            {
                                origin = new Point3d(runningX + panel.TopFixingHoleDistance, runningY + 15, 0);  //right
                                offset = new Point3d(runningX, runningY + 15, 0); //left 
                                pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 4), 0);

                                plane = Plane.WorldXY;
                                plane.Origin = origin;

                                guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                                runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                points++;
                            }
                        }

                        //Draw the setback dimension of top fixing hole (left)                 
                        if (Folded) //if folded 
                        {
                            runningX = panelX0 + foldPanel.TopFirstFoldSetbackLeft + panel.TopHoleSetbackLeft;
                        }
                        else //if flat
                        {
                            runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                        }

                        origin = new Point3d(runningX, runningY + 15, 0);
                        offset = new Point3d(runningX - panel.TopHoleSetbackLeft, runningY + 15, 0);
                        pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 2), 0);

                        plane = Plane.WorldXY;
                        guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                        //Draw the setback dimension of top fixing hole (right)
                        if (Folded)
                        {
                            runningX = panelX1 - panel.TopHoleSetbackRight - foldPanel.TopFirstFoldSetbackRight;
                        }
                        else
                        {
                            runningX = max.X - panel.TopHoleSetbackRight;  //calculate the starting point for the first hole from the left(with set back)
                        }
                        origin = new Point3d(runningX, runningY + 15, 0);
                        offset = new Point3d(runningX + panel.TopHoleSetbackRight, runningY + 15, 0);
                        pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 2), 0);

                        plane = Plane.WorldXY;
                        guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                        //Draw the dimension between the top folded line and the center of the fixing hole       
                        origin = new Point3d(panelX0, runningY, 0);  //upper horizontal line of dimension
                                                                     // offset = new Point3d(min.X - 30, max.Y, 0); //bottom horizontal line of the dimension
                        offset = new Point3d(panelX0, runningY + panel.TopHoleSetbackTop, 0); //bottom horizontal line of the dimension
                        pt = new Point3d(panelX0, (offset.Y - origin.Y) / 2, 0); //addjust the text position

                        plane = Plane.WorldXY;
                        plane.XAxis = new Vector3d(0, -1, 0);
                        plane.YAxis = new Vector3d(-1, 0, 0);
                        plane.ZAxis = new Vector3d(0, 0, -1);
                        plane.Origin = origin;

                        guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension
                    }

                    //Draw the bottom holes
                    doc.Layers.SetCurrentLayerIndex(layerIndex, true);
                    if (panel.BottomFixingHoles.Equals("1"))
                    {
                        runningY = min.Y + panel.BottomHoleSetbackBottom; //calculate the starting point for the first from the right (with setback)

                        if (Folded)
                        {
                            //If the user wants to draw on the first fold (Applicable only for double folds)
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 1)
                            {
                                singleFoldMinY = foldPanel.BottomFirstFoldWidth - (foldPanel.KFactor * 2);
                                runningY = panelY0 - singleFoldMinY + foldPanel.BottomHoleSetbackBottom;
                            }

                            if (foldPanel.PanelType.Equals("Single Folded")) // For single Folds
                            {
                                runningY = (((panelY0 - foldPanel.BottomFirstFoldWidth) + foldPanel.KFactor * 2)) + panel.BottomHoleSetbackBottom;
                            }
                            runningX = panelX0 + foldPanel.BottomFirstFoldSetbackLeft + panel.BottomHoleSetbackLeft;
                        }
                        else
                        {
                            runningX = min.X + panel.BottomHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                        }

                        if (Folded)
                        {

                            point = new Point3d(runningX, runningY, 0); //adds the bottom  Left fixing hole
                            pointsList.Add(point);
                            bottomLeft = point; //sets the bottom left point for future calculations

                            //if (foldPanel != null)
                            //{
                            point = new Point3d(panelX1 - panel.BottomHoleSetbackRight - foldPanel.BottomFirstFoldSetbackRight, runningY, 0); //adds the bottom right fixing hole                 
                                                                                                                                              //}
                                                                                                                                              //else
                                                                                                                                              //{
                                                                                                                                              //   point = new Point3d(panelX1 - foldPanel.LeftFirstFoldSetbackBottom - foldPanel.KFactor - panel.BottomHoleSetbackRight, runningY, 0); //adds the bottom right fixing hole                 
                                                                                                                                              //}
                            bottomRight = point;
                            pointBottomRight = point; //get the point of the bottom right fixing hole
                            pointsList.Add(point);
                            pointBottomLeft = new Point3d(runningX, runningY, 0); // get the point of the bottom left fixing hole
                            points = 0;
                            while (points < panel.BottomFixingHoleQuantity - 1)
                            {
                                runningX = runningX + panel.BottomFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //adds the bottom fixing holes
                                pointsList.Add(point);
                                points++;
                            }

                        }
                        else
                        {
                            point = new Point3d(runningX, runningY, 0); //adds the bottom  Left fixing hole
                            pointsList.Add(point);
                            pointBottomLeft = point; //Get the point of the bottom left fixing hole
                            point = new Point3d(max.X - panel.BottomHoleSetbackRight, runningY, 0); //adds the bottom right fixing hole                  
                            pointsList.Add(point);
                            pointBottomRight = point; //get the point of the bottom right fixing hole

                            points = 1;
                            while (points <= panel.BottomFixingHoleQuantity - 1)
                            {
                                runningX = runningX + panel.BottomFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //adds the bottom fixing hole
                                pointsList.Add(point);
                                points++;
                            }
                        }
                    }

                    //Draw the Bottom horizontal fixing holes dimension (between the fixing holes)
                    doc.Layers.SetCurrentLayerIndex(tempLayerIndex, true);
                    if (panel.BottomFixingHoles == "1")
                    {
                        runningY = min.Y + panel.BottomHoleSetbackBottom;  //calculate the starting point for the first from the right (with setback)

                        if (Folded)
                        {
                            runningX = panelX0 + foldPanel.BottomFirstFoldSetbackLeft + panel.BottomHoleSetbackLeft;

                            if (foldPanel.TopFold != 1 && foldPanel.LeftFold == 1 && foldPanel.RightFold != 1 && foldPanel.BottomFold == 1 || foldPanel.TopFold != 1 && foldPanel.LeftFold != 1 && foldPanel.RightFold == 1 && foldPanel.BottomFold == 1)
                            {
                                runningY = ((panelY0 - foldPanel.BottomFirstFoldWidth) + foldPanel.KFactor) + panel.BottomHoleSetbackBottom;
                            }
                            if (!foldPanel.OnFolds)
                            {
                                runningY = ((panelY0 - foldPanel.BottomFirstFoldWidth - 2));
                            }
                        }
                        else
                        {
                            runningX = min.X + panel.BottomHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                        }
                        points = 0;
                        if (Folded)
                        {
                            while (points < (panel.BottomFixingHoleQuantity) - 1)
                            {
                                origin = new Point3d(runningX + panel.BottomFixingHoleDistance, runningY, 0);  //right
                                offset = new Point3d(runningX, runningY, 0); //left 
                                pt = new Point3d((offset.X - origin.X) / 2, (runningY) - (dimStyle.TextHeight * 4), 0);

                                plane = Plane.WorldXY;
                                plane.Origin = origin;

                                guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                                runningX = runningX + panel.BottomFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                points++;
                            }
                        }
                        else
                        {
                            while (points < (panel.BottomFixingHoleQuantity) - 1)
                            {
                                origin = new Point3d(runningX + panel.BottomFixingHoleDistance, runningY, 0);  //right
                                offset = new Point3d(runningX, runningY, 0); //left 
                                pt = new Point3d((offset.X - origin.X) / 2, (runningY) - (dimStyle.TextHeight * 4), 0);

                                plane = Plane.WorldXY;
                                plane.Origin = origin;

                                guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                                runningX = runningX + panel.BottomFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                points++;
                            }
                        }
                    }
                    //Draw the Left holes
                    doc.Layers.SetCurrentLayerIndex(layerIndex, true);


                    if (panel.LeftFixingHoles.Equals("1"))
                    {
                        runningX = min.X + panel.LeftHoleSetbackLeft;
                        if (Folded) //if the panel is folded x value should be within the fold
                        {
                            runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - panel.LeftHoleSetbackTop;
                            if (foldPanel.PanelType.Equals("Single Folded"))
                            {
                                runningX = (panelX0 - foldPanel.LeftFirstFoldWidth + foldPanel.KFactor * 2) + foldPanel.LeftHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                            }
                            //If the user wants to draw on the first fold (Applicable only for double folds)
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 1)
                            {
                                singleFoldMinX = foldPanel.LeftFirstFoldWidth - (foldPanel.KFactor * 2);
                                runningX = panelX0 - singleFoldMinX + panel.LeftHoleSetbackLeft;
                            }
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2)
                            {
                                runningY = panelY1 - foldPanel.LeftSecondFoldSetbackTop - panel.LeftHoleSetbackTop;
                            }
                        }
                        else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                        {
                            runningY = max.Y - panel.LeftHoleSetbackTop;
                        }

                        if (!Folded || Folded) //check if there is top fixing holes
                        {
                            if (foldPanel != null)
                            {
                                point = new Point3d(runningX, runningY, 0); //Calculate the left top fixing hole 

                                //check if the fixing hole is going to be drawn on the folds or not'
                                //If the not, check the distance between the top left and left top fixing holes
                                //draw the fixing hole on lef top only if the distance is more or equal or if draw on folds is true
                                double test = point.DistanceTo(topLeft);
                                if (!foldPanel.OnFolds && point.DistanceTo(topLeft) >= foldPanel.LeftFixingHoleDistance || foldPanel.OnFolds)
                                {
                                    pointsList.Add(point);
                                }
                            }
                            else
                            {
                                point = new Point3d(runningX, runningY, 0); //Create point for the left top fixing hole 

                                // Add the fixing hole if it more than the diameter + 5 more mil   or if there is no top fixing holes   
                                if (pointTopLeft.DistanceTo(point) > panel.HoleDiameter + 5 || !panel.TopFixingHoles.Equals("1"))
                                {
                                    pointsList.Add(point); //add the left top fixing hole
                                }
                            }
                        }

                        if (!Folded || Folded)
                        {
                            if (foldPanel != null)
                            {
                                //calculate the left bottom fixing hole
                                point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom + foldPanel.LeftFirstFoldSetbackBottom, 0);
                                if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2)
                                {
                                    point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom + foldPanel.LeftSecondFoldSetbackBottom, 0);
                                }
                                //check if the fixing hole is going to be drawn on the folds or not'
                                //If the not, check the distance between the bottom left and left bottom fixing holes
                                //draw the fixing hole on left bottom only if the calculate distance is more or equal or if draw on folds is true
                                if (!foldPanel.OnFolds && point.DistanceTo(bottomLeft) > foldPanel.LeftFixingHoleDistance || foldPanel.OnFolds)
                                {
                                    pointsList.Add(point);
                                }
                            }
                            else
                            {
                                point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom, 0);
                                // Add the fixing hole if it more than the diameter + 5 more mil   or if there is no Bottom fixing holes   
                                if (pointBottomLeft.DistanceTo(point) > panel.HoleDiameter + 5 || !panel.BottomFixingHoles.Equals("1"))
                                {
                                    pointsList.Add(point);
                                }
                            }
                        }
                        points = 1;
                        if (Folded) //If panel is folded
                        {
                            while (points < panel.LeftFixingHoleQuantity - 1)
                            {
                                runningY = runningY - panel.LeftFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //draw the left fixing holes
                                pointsList.Add(point);
                                points++;
                            }
                        }
                        else //If panel is flat
                        {
                            while (points < panel.LeftFixingHoleQuantity - 1)
                            {
                                runningY = runningY - panel.LeftFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //draw the left fixing holes
                                pointsList.Add(point);
                                points++;
                            }
                        }
                    }


                    //Draw the left vertical fixing holes dimension (between the left fixing holes)
                    doc.Layers.SetCurrentLayerIndex(tempLayerIndex, true);
                    if (panel.LeftFixingHoles == "1")
                    {

                        runningX = min.X + panel.LeftHoleSetbackLeft;

                        if (Folded)
                        {
                            runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - panel.LeftHoleSetbackTop;

                            //Catering Top and left folds  or bottom and left folds
                            if (foldPanel.TopFold == 1 && foldPanel.LeftFold == 1 && foldPanel.RightFold != 1 && foldPanel.BottomFold != 1 || foldPanel.TopFold != 1 && foldPanel.LeftFold == 1 && foldPanel.RightFold != 1 && foldPanel.BottomFold == 1)
                            {
                                runningX = ((panelX0 - (foldPanel.LeftFirstFoldWidth / 3)) + (foldPanel.KFactor)) + panel.LeftHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                            }

                            if (!foldPanel.OnFolds) //if the user does not want to draw the fixing holes on the folds
                            {
                                runningX = ((panelX0 - (foldPanel.LeftFirstFoldWidth / 2) - (foldPanel.KFactor)));
                            }
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2)
                            {
                                runningY = panelY1 - foldPanel.LeftSecondFoldSetbackTop - panel.LeftHoleSetbackTop;
                            }
                        }
                        else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                        {
                            runningY = max.Y - panel.LeftHoleSetbackTop;
                        }
                        points = 0;
                        while (points < (panel.LeftFixingHoleQuantity) -1)
                        {
                            if (Folded)
                            {
                                origin = new Point3d(runningX - foldPanel.LeftFirstFoldWidth, runningY - panel.LeftFixingHoleDistance, 0);  //right
                                offset = new Point3d(runningX - foldPanel.LeftFirstFoldWidth, runningY, 0); //left 
                                pt = new Point3d(runningX - foldPanel.LeftFirstFoldWidth, runningY + (dimStyle.TextHeight * 4), 0);
                            }
                            else
                            {
                                origin = new Point3d(min.X + 20, runningY - panel.LeftFixingHoleDistance, 0);  //right
                                offset = new Point3d(min.X + 20, runningY, 0); //left 
                                pt = new Point3d(min.X - 20, runningY + (dimStyle.TextHeight * 4), 0);
                            }

                            plane = Plane.WorldXY;
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension
                            runningY = runningY - panel.LeftFixingHoleDistance; //multiply by 2 to increase width (for equaliity)

                            pointOne = runningY;

                            points++;
                        }
                    }


                    //Draw the Right holes
                    doc.Layers.SetCurrentLayerIndex(layerIndex, true);
                    if (panel.RightFixingHoles.Equals("1"))
                    {
                        runningX = max.X - panel.RightHoleSetbackRight;
                        if (Folded) //if the panel is folded x value should be within the fold
                        {
                            runningY = panelY1 - foldPanel.RightFirstFoldSetbackTop - panel.RightHoleSetbackTop;


                            if (foldPanel.PanelType.Equals("Single Folded"))
                            {
                                runningX = ((panelX1 + foldPanel.RightFirstFoldWidth) - (foldPanel.KFactor * 2)) - panel.RightHoleSetbackRight;  //calculate the starting point for the first hole from the left(with set back)
                            }

                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 1)
                            {
                                singleFoldMaxX = foldPanel.RightFirstFoldWidth - (foldPanel.KFactor * 2);
                                runningX = panelX1 + singleFoldMaxX - panel.RightHoleSetbackRight;
                            }
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2)
                            {
                                runningY = panelY1 - foldPanel.RightSecondFoldSetbackTop - panel.RightHoleSetbackTop;
                            }
                        }
                        else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                        {
                            runningY = max.Y - panel.RightHoleSetbackTop;
                        }

                        //if (!panel.TopFixingHoles.Equals("1") || Folded)
                        if (!Folded || Folded)
                        {
                            point = new Point3d(runningX, runningY, 0); //Calculate the Right top fixing hole
                            if (foldPanel != null)
                            {
                                //check if the fixing hole is going to be drawn on the folds or not'
                                //If the not, check the distance between the Right top and Top Right fixing holes
                                //draw the fixing hole on Right top only if the calculated distance is more or equal or if draw on folds is true
                                if (!foldPanel.OnFolds && point.DistanceTo(topRight) > foldPanel.RightFixingHoleDistance || foldPanel.OnFolds)
                                {
                                    pointsList.Add(point);
                                }

                            }
                            else
                            {
                                // Add the fixing hole if it more than the diameter + 5 more mil   or if there is no Bottom fixing holes   
                                if (pointTopRight.DistanceTo(point) > panel.HoleDiameter + 5 || !panel.TopFixingHoles.Equals("1"))
                                {
                                    pointsList.Add(point);
                                }
                            }
                        }

                        if (!Folded || Folded)
                        {
                            if (foldPanel != null)
                            {
                                //Calculate the  Right bottom fixing hole
                                if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2) //For Double folds and to draw on fold 2
                                {
                                    point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom + foldPanel.RightSecondFoldSetbackBottom, 0);
                                }
                                else //For Single folds and double folds (to draw on fold 1)
                                {
                                    point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom + foldPanel.RightFirstFoldSetbackBottom, 0);
                                }
                                //check if the fixing hole is going to be drawn on the folds or not'
                                //If the not, check the distance between the Right bottom and Top bottom fixing holes
                                //draw the fixing hole on Right bottom only if the calculated distance is more or equal or if draw on folds is true                   
                                if (!foldPanel.OnFolds && point.DistanceTo(bottomRight) > foldPanel.RightFixingHoleDistance || foldPanel.OnFolds)
                                {
                                    pointsList.Add(point);
                                }
                            }
                            else
                            {
                                point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom, 0); //adds the  Right bottom fixing hole
                                if (pointBottomRight.DistanceTo(point) > panel.HoleDiameter + 5 || !panel.BottomFixingHoles.Equals("1"))
                                {
                                    pointsList.Add(point);
                                }
                            }

                        }
                        points = 1;
                        while (points < panel.RightFixingHoleQuantity - 1)
                        {
                            runningY = runningY - panel.RightFixingHoleDistance;
                            point = new Point3d(runningX, runningY, 0); //draw the right fixing holes
                            pointsList.Add(point);
                            points++;
                        }
                    }


                    //Draw the right vertical fixing holes dimension (between the right fixing holes)
                    doc.Layers.SetCurrentLayerIndex(tempLayerIndex, true);
                    if (panel.RightFixingHoles == "1")
                    {
                        runningX = max.X - panel.RightHoleSetbackRight;

                        if (Folded) //if the panel is folded x value should be within the fold
                        {
                            runningY = panelY1 - foldPanel.RightFirstFoldSetbackTop - panel.RightHoleSetbackTop;
                            runningX = ((panelX1 + foldPanel.RightFirstFoldWidth) + (foldPanel.KFactor) + 30);  //calculate the starting point for the first hole from the left(with set back)

                            //Catering top and right folds
                            if (foldPanel.TopFold == 1 && foldPanel.LeftFold != 1 && foldPanel.RightFold == 1 && foldPanel.BottomFold != 1 || foldPanel.TopFold != 1 && foldPanel.LeftFold != 1 && foldPanel.RightFold == 1 && foldPanel.BottomFold == 1)
                            {
                                runningX = ((panelX1 + foldPanel.RightFirstFoldWidth) + (foldPanel.KFactor) + 30);  //calculate the starting point for the first hole from the left(with set back)
                            }

                            if (!foldPanel.OnFolds) //if the user does not want to draw the fixing holes on the folds
                            {
                                runningX = ((panelX1 + foldPanel.RightFirstFoldWidth + (foldPanel.KFactor) + 30));
                            }
                            if (foldPanel.PanelType.Equals("Double Folded") && foldToDraw == 2)
                            {
                                runningX = ((panelX1 + foldPanel.RightSecondFoldWidth) + (foldPanel.KFactor) + 30);  //calculate the starting point for the first hole from the left(with set back)

                                runningY = panelY1 - foldPanel.RightSecondFoldSetbackTop - panel.RightHoleSetbackTop;
                            }
                        }
                        else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                        {
                            runningY = max.Y - panel.RightHoleSetbackTop;
                        }
                        points = 0;
                        while (points < (panel.RightFixingHoleQuantity) -1)
                        {
                            //multiply by 2 to increase width (for equaliity)
                            if (Folded)
                            {
                                origin = new Point3d(runningX + foldPanel.RightFirstFoldWidth, runningY - panel.RightFixingHoleDistance, 0);  //right
                                offset = new Point3d(runningX + foldPanel.RightFirstFoldWidth, runningY, 0); //left 
                                pt = new Point3d(runningX + foldPanel.RightFirstFoldWidth, runningY + (dimStyle.TextHeight * 4), 0);
                            }
                            else
                            {
                                origin = new Point3d(max.X + 50, runningY - panel.RightFixingHoleDistance, 0);  //right
                                offset = new Point3d(max.X + 50, runningY, 0); //left 
                                pt = new Point3d(max.X + 50, runningY + (dimStyle.TextHeight * 4), 0);
                            }
                            plane = Plane.WorldXY;
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension
                            runningY = runningY - panel.RightFixingHoleDistance;

                            points++;
                        }

                    }
                    //Drawing fixings holes on the dimensions
                    doc.Layers.SetCurrentLayerIndex(layerIndex, true);
                    //Draw the Top holes (on blue dimension)
                    if (Folded)
                    {
                        //draw the fixing holes on the blue dimension only if there is top fold
                        if (foldPanel.LeftFold == 1 && panel.TopFixingHoles == "1" || foldPanel.RightFold == 1 && panel.TopFixingHoles == "1")
                        {

                            runningX = panelX0 + foldPanel.TopFirstFoldSetbackRight + panel.TopHoleSetbackLeft;
                            runningY = (panelBottom + foldPanel.TopFirstFoldWidth) - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)

                            points = 0;
                            if (foldPanel != null)
                            {
                                if (foldPanel.TopFirstFoldDirection == 1)
                                {
                                    runningY = (panelTop - foldPanel.TopFirstFoldWidth) + panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)
                                }


                            }
                            point = new Point3d(panelX1 - panel.TopHoleSetbackRight - foldPanel.TopFirstFoldSetbackRight, runningY, 0); //adds the top right fixing hole in the dimensions blue (use panel bottom)
                            pointsList.Add(point);
                            point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                            pointsList.Add(point);

                            while (points < (panel.TopFixingHoleQuantity - 1))
                            {

                                runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                                pointsList.Add(point);
                                points++;
                            }
                        }

                    }

                    //Draw the Bottom holes (on blue dimension)
                    if (Folded)
                    {
                        //draw the fixing holes on the blue dimension only if there is bottom fold
                        if (foldPanel.LeftFold == 1 && panel.BottomFixingHoles == "1" || foldPanel.RightFold == 1 && panel.BottomFixingHoles == "1")
                        {

                            runningX = panelX0 + foldPanel.BottomFirstFoldSetbackLeft + panel.BottomHoleSetbackLeft;
                            runningY = (panelBottomBH - foldPanel.BottomFirstFoldWidth) + panel.BottomHoleSetbackBottom;  //calculate the starting point for the first from the right (with setback)

                            points = 0;
                            if (foldPanel != null)
                            {
                                if (foldPanel.TopFirstFoldDirection == 1)
                                {
                                    runningY = (panelBottomBH + foldPanel.BottomFirstFoldWidth) - panel.BottomHoleSetbackBottom;  //calculate the starting point for the first from the right (with setback)
                                }


                            }
                            point = new Point3d(panelX1 - panel.BottomHoleSetbackRight - foldPanel.BottomFirstFoldSetbackRight, runningY, 0); //adds the top right fixing hole in the dimensions blue (use panel bottom)
                            pointsList.Add(point);
                            point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                            pointsList.Add(point);

                            while (points < (panel.BottomFixingHoleQuantity - 1))
                            {

                                runningX = runningX + panel.BottomFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                                point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                                pointsList.Add(point);
                                points++;
                            }
                        }

                    }

                    ////Draw the Left holes (on blue dimension)
                    if (Folded) //draw the fixing holes on the left dimension only on folded panels because flat panels do not have the left dimension
                    {
                        if (foldPanel.TopFold == 1 && panel.LeftFixingHoles == "1" || foldPanel.BottomFold == 1 && panel.LeftFixingHoles == "1") //draw only if there is Left fold
                        {
                            runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - panel.LeftHoleSetbackTop;
                            runningX = panelRight - foldPanel.LeftFirstFoldWidth + panel.LeftHoleSetbackLeft;



                            if (foldPanel != null)
                            {
                                if (foldPanel.LeftFirstFoldDirection == 1)
                                {
                                    runningX = (panelLeft + foldPanel.LeftFirstFoldWidth) - panel.LeftHoleSetbackLeft;
                                }

                            }
                            point = new Point3d(runningX, runningY, 0); //adds the Left top fixing hole                     
                            pointsList.Add(point);
                            point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom + foldPanel.LeftFirstFoldSetbackBottom, 0); //adds the  left bottom fixing hole                     
                            pointsList.Add(point);

                            points = 1;
                            while (points <= panel.LeftFixingHoleQuantity - 2)
                            {
                                runningY = runningY - panel.LeftFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //draw the left fixing holes
                                pointsList.Add(point);
                                points++;
                            }
                        }
                    }

                    //Draw the Right holes (on blue dimension)
                    if (Folded) //draw the fixing holes on the Right dimension only on folded panels because flat panels do not have the left dimension
                    {
                        if (foldPanel.TopFold == 1 && panel.RightFixingHoles == "1" || foldPanel.BottomFold == 1 && panel.RightFixingHoles == "1") //draw only if there is right
                        {
                            runningY = panelY1 - foldPanel.RightFirstFoldSetbackTop - panel.RightHoleSetbackTop;
                            runningX = panelRightHS + foldPanel.RightFirstFoldWidth - panel.RightHoleSetbackRight;



                            if (foldPanel != null)
                            {
                                if (foldPanel.RightFirstFoldDirection == 1)
                                {
                                    runningX = (panelLeftHS - foldPanel.RightFirstFoldWidth) + panel.RightHoleSetbackRight;
                                }

                            }

                            point = new Point3d(runningX, runningY, 0); //adds the Left top fixing hole                     
                            pointsList.Add(point);
                            point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom + foldPanel.RightFirstFoldSetbackBottom, 0); //adds the  left bottom fixing hole                     
                            pointsList.Add(point);

                            points = 1;
                            while (points <= panel.RightFixingHoleQuantity - 2)
                            {
                                runningY = runningY - panel.RightFixingHoleDistance;
                                point = new Point3d(runningX, runningY, 0); //draw the left fixing holes
                                pointsList.Add(point);
                                points++;
                            }
                        }
                    }

                    //Draw all the holes
                    Round round = new Round();
                    round.X = holeSize;


                    foreach (Point3d p in pointsList)
                    {
                        round.drawTool(p);
                    }


                }
            }
            return guidList;

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
