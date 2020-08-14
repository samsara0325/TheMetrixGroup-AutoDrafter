using MetrixGroupPlugins.PunchingTools;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System;
using System.Collections;
using System.Collections.Generic;


namespace MetrixGroupPlugins.FixingHoles
{

   public class CustomFixingHoles
   {
      /**
       * Method contains the logic to draw fixing holes evening on the panel based on the user's criteria
       * */
      public static List<Guid> drawFixingFoles(PerforationPanel panel, FoldedPerforationPanel foldPanel, Boolean Folded, double panelBottom, double panelLeft, int folds, double panelY0, double panelY1, DimensionStyle dimStyle, List<Guid> guidList, double panelX0, double panelX1, double panelRight, double panelTop)
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
            int tempLayerIndex = 0;
            tempLayerIndex = createSubLayers.createSubLayer("FIXING HOLES DIMENSIONS",
        System.Drawing.Color.DarkGreen, doc.Layers[doc.Layers.Find("LAYERS FOR APPROVAL DRAWINGS", true)]);

            Rhino.DocObjects.RhinoObject[] go = null;
            int layerIndex = createSubLayers.createSubLayer("FIXING HOLES",
        System.Drawing.Color.Black, doc.Layers[doc.Layers.Find("LAYERS FOR NESTING", true)]);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);


            
               go = doc.Objects.FindByLayer("Fixing");
           
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
                  runningX = panelX0 + foldPanel.LeftFirstFoldSetbackTop + foldPanel.KFactor + panel.TopHoleSetbackLeft;
               }
               else
               {
                  runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
               }

               int points = 1;
               Point3d point;



               //Draw the top holes
               if (panel.TopFixingHoles.Equals("1"))
               {
                  if (Folded)
                  {
                     if (folds == 4)
                     {


                        if (foldPanel != null)
                        {
                           point = new Point3d(panelX1 - panel.TopHoleSetbackRight - foldPanel.TopFirstFoldSetbackRight, runningY, 0); //adds the top right fixing hole                          
                        }
                        else
                        {
                           point = new Point3d(panelX1 - foldPanel.RightFirstFoldSetbackBottom - foldPanel.KFactor - panel.TopHoleSetbackRight, runningY, 0); //adds the top right fixing hole

                        }
                        pointsList.Add(point);

                        point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                        pointsList.Add(point);

                        while (points <= (panel.TopFixingHoleQuantity - 2))
                        {

                           runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                           point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                           pointsList.Add(point);
                           points++;
                        }
                     }
                  }
                  else
                  {
                     point = new Point3d(max.X - panel.TopHoleSetbackRight, runningY, 0); //adds the top right fixing hole
                     pointsList.Add(point);

                     point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                     pointsList.Add(point);

                     while (points <= (panel.TopFixingHoleQuantity - 2))
                     {

                        runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                        point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                        pointsList.Add(point);
                        points++;
                     }
                  }
               }


               //Draw the top horizontal fixing holes dimension (between the fixing holes)
               doc.Layers.SetCurrentLayerIndex(tempLayerIndex, true);
               if (foldPanel.TopFixingHoles == "1")
               {
                  runningY = max.Y - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)

                  if (Folded)
                  {
                     runningX = panelX0 + foldPanel.LeftFirstFoldSetbackTop + foldPanel.KFactor + panel.TopHoleSetbackLeft;
                  }
                  else
                  {
                     runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                  }

                  points = 0;
                  while (points <= (panel.TopFixingHoleQuantity) - 2)
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

                  //Draw the setback dimension of top fixing hole (left)
                  runningY = max.Y - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)

                  if (Folded)
                  {
                     runningX = panelX0 + foldPanel.LeftFirstFoldSetbackTop + foldPanel.KFactor + panel.TopHoleSetbackLeft;
                  }
                  else
                  {
                     runningX = min.X + panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                  }
                  origin = new Point3d(runningX, runningY + 15, 0);
                  offset = new Point3d(runningX - panel.TopHoleSetbackLeft, runningY + 15, 0);
                  pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 2), 0);

                  plane = Plane.WorldXY;
                  guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                  runningY = max.Y - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)

                  //Draw the setback dimension of top fixing hole (right)
                  if (Folded)
                  {
                     runningX = panelX1 - foldPanel.LeftFirstFoldSetbackTop - foldPanel.KFactor - panel.TopHoleSetbackLeft;
                  }
                  else
                  {
                     runningX = max.X - panel.TopHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                  }
                  origin = new Point3d(runningX, runningY + 15, 0);
                  offset = new Point3d(runningX + panel.TopHoleSetbackLeft, runningY + 15, 0);
                  pt = new Point3d((offset.X - origin.X) / 2, (runningY + 15) + (dimStyle.TextHeight * 2), 0);

                  plane = Plane.WorldXY;
                  guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                  //Draw the dimension between the top folded line and the center of the fixing hole       
                  origin = new Point3d(min.X - 30, runningY, 0);  //upper horizontal line of dimension
                  offset = new Point3d(min.X - 30, max.Y, 0); //bottom horizontal line of the dimension
                  pt = new Point3d(min.X - 30, (offset.Y - origin.Y) / 2, 0); //addjust the text position

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
                     runningX = panelX0 + foldPanel.LeftFirstFoldSetbackBottom + foldPanel.KFactor + panel.BottomHoleSetbackLeft;
                  }
                  else
                  {
                     runningX = min.X + panel.BottomHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                  }

                  if (Folded)
                  {

                     point = new Point3d(runningX, runningY, 0); //adds the bottom  Left fixing hole
                     pointsList.Add(point);


                     if (foldPanel != null)
                     {
                        point = new Point3d(panelX1 - panel.BottomHoleSetbackRight - foldPanel.BottomFirstFoldSetbackRight, runningY, 0); //adds the bottom right fixing hole                 
                     }
                     else
                     {
                        point = new Point3d(panelX1 - foldPanel.LeftFirstFoldSetbackBottom - foldPanel.KFactor - panel.BottomHoleSetbackRight, runningY, 0); //adds the bottom right fixing hole                 

                     }
                     pointsList.Add(point);
                     points = 1;
                     while (points <= panel.BottomFixingHoleQuantity - 2)
                     {
                        runningX = runningX + panel.BottomFixingHoleDistance;
                        point = new Point3d(runningX, runningY, 0); //adds the bottom fixing hole
                        pointsList.Add(point);
                        points++;
                     }

                  }
                  else
                  {
                     point = new Point3d(runningX, runningY, 0); //adds the bottom  Left fixing hole
                     pointsList.Add(point);

                     point = new Point3d(max.X - panel.BottomHoleSetbackRight, runningY, 0); //adds the bottom right fixing hole                  
                     pointsList.Add(point);

                     points = 1;
                     while (points <= panel.BottomFixingHoleQuantity - 2)
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
                     runningX = panelX0 + foldPanel.LeftFirstFoldSetbackBottom + foldPanel.KFactor + panel.BottomHoleSetbackLeft;                    
                  }
                  else
                  {
                     runningX = min.X + panel.BottomHoleSetbackLeft;  //calculate the starting point for the first hole from the left(with set back)
                  }
                  points = 0;
                  while (points <= (panel.BottomFixingHoleQuantity) - 2)
                  {
                     origin = new Point3d(runningX + panel.BottomFixingHoleDistance, runningY + panel.BottomHoleSetbackBottom, 0);  //right
                     offset = new Point3d(runningX, runningY + panel.BottomHoleSetbackBottom, 0); //left 
                     pt = new Point3d((offset.X - origin.X) / 2, (runningY + panel.BottomHoleSetbackBottom) - (dimStyle.TextHeight * 4), 0);

                     plane = Plane.WorldXY;
                     plane.Origin = origin;

                     guidList = drawDimension(plane, pt, offset, origin, guidList, doc); //draw the dimension

                     runningX = runningX + panel.BottomFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                     points++;
                  }
               }
               //Draw the Left holes
               doc.Layers.SetCurrentLayerIndex(layerIndex, true);
               if (panel.LeftFixingHoles.Equals("1"))
               {
                  runningX = min.X + panel.LeftHoleSetbackLeft;
                  if (Folded) //if the panel is folded x value should be within the fold
                  {
                     runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - foldPanel.KFactor - panel.LeftHoleSetbackTop;
                  }
                  else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                  {
                     runningY = max.Y - panel.LeftHoleSetbackTop;                    
                  }

                  if (!panel.TopFixingHoles.Equals("1") || folds == 4) //check if there is top fixing holes
                  {
                     point = new Point3d(runningX, runningY, 0); //adds the Left top fixing hole                     
                     pointsList.Add(point);
                  }

                  if (!panel.BottomFixingHoles.Equals("1") || folds == 4) //check if there is bottom fixing holes
                  {
                     if (foldPanel != null)
                     {
                        point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom + foldPanel.LeftFirstFoldSetbackBottom, 0); //adds the  left bottom fixing hole                     
                     }
                     else
                     {
                        point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom, 0); //adds the  left bottom fixing hole                     

                     }

                     pointsList.Add(point);
                  }
                  points = 1;
                  if (Folded)
                  {
                     while (points <= panel.LeftFixingHoleQuantity - 2)
                     {
                        runningY = runningY - panel.LeftFixingHoleDistance;
                        point = new Point3d(runningX, runningY, 0); //draw the left fixing holes
                        pointsList.Add(point);
                        points++;
                     }
                  }
                  else
                  {
                     while (points <= panel.LeftFixingHoleQuantity - 2)
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
                  if (Folded) //if the panel is folded x value should be within the fold
                  {
                   runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - foldPanel.KFactor - panel.LeftHoleSetbackTop;
                  }
                  else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                  {
                     runningY = max.Y - panel.LeftHoleSetbackTop;
                  }
                  points = 0;
                  while (points < (panel.LeftFixingHoleQuantity) - 1)
                  {
                     if (Folded)
                     {
                        origin = new Point3d(min.X - foldPanel.LeftFirstFoldWidth, runningY - panel.LeftFixingHoleDistance, 0);  //right
                        offset = new Point3d(min.X - foldPanel.LeftFirstFoldWidth, runningY, 0); //left 
                        pt = new Point3d(min.X - foldPanel.LeftFirstFoldWidth, runningY + (dimStyle.TextHeight * 4), 0);
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
                    runningY = panelY1 - foldPanel.RightFirstFoldSetbackTop - foldPanel.KFactor - panel.RightHoleSetbackTop;
                  }
                  else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                  {
                     runningY = max.Y - panel.RightHoleSetbackTop;                   
                  }

                  if (!panel.TopFixingHoles.Equals("1") || folds == 4)
                  {
                     point = new Point3d(runningX, runningY, 0); //adds the Right top fixing hole
                     pointsList.Add(point);
                  }
                  if (!panel.BottomFixingHoles.Equals("1") || folds == 4)
                  {
                     if (foldPanel != null)
                     {
                        point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom + foldPanel.RightFirstFoldSetbackBottom, 0); //adds the  RIght bottom fixing hole
                     }
                     else
                     {
                        point = new Point3d(runningX, panelY0 + panel.RightHoleSetbackBottom, 0); //adds the  RIght bottom fixing hole

                     }
                     pointsList.Add(point);
                  }
                  points = 1;
                  while (points <= panel.RightFixingHoleQuantity - 2)
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
                     runningY = panelY1 - foldPanel.RightFirstFoldSetbackTop - foldPanel.KFactor - panel.RightHoleSetbackTop;
                  }
                  else  //if the panel is flat panel the x value should be between the folded panel finished and the borders
                  {
                     runningY = max.Y - panel.RightHoleSetbackTop;
                  }
                  points = 0;
                  while (points < (panel.LeftFixingHoleQuantity) - 1)
                  {
                     //multiply by 2 to increase width (for equaliity)
                     origin = new Point3d(max.X + 50, runningY - panel.RightFixingHoleDistance, 0);  //right
                     offset = new Point3d(max.X + 50, runningY, 0); //left 
                     pt = new Point3d(max.X + 50, runningY + (dimStyle.TextHeight * 4), 0);

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
               //Draw the top hole
               if (panel.TopFixingHoles.Equals("1") && Folded)
               {
                  runningX = panelX0 + foldPanel.LeftFirstFoldSetbackTop + foldPanel.KFactor + panel.TopHoleSetbackLeft;
                  runningY = (panelBottom + foldPanel.TopFirstFoldWidth) - panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)

                  points = 1;
                  if (foldPanel != null)
                  {
                     if (foldPanel.TopFirstFoldDirection == 1)
                     {
                        runningY = (panelTop - foldPanel.TopFirstFoldWidth) + panel.TopHoleSetbackTop;  //calculate the starting point for the first from the right (with setback)
                     }


                  }
                  else
                  {
                     point = new Point3d(panelX1 - foldPanel.LeftFirstFoldSetbackTop - foldPanel.KFactor - panel.TopHoleSetbackRight, (panelBottom + foldPanel.TopFirstFoldWidth) - panel.TopHoleSetbackTop, 0); //adds the top right fixing hole in the dimensions blue (use panel bottom)
                     pointsList.Add(point);
                  }
                  point = new Point3d(panelX1 - panel.TopHoleSetbackRight - foldPanel.TopFirstFoldSetbackRight, runningY, 0); //adds the top right fixing hole in the dimensions blue (use panel bottom)
                  pointsList.Add(point);
                  point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                  pointsList.Add(point);

                  while (points <= (panel.TopFixingHoleQuantity - 2))
                  {

                     runningX = runningX + panel.TopFixingHoleDistance; //multiply by 2 to increase width (for equaliity)
                     point = new Point3d(runningX, runningY, 0); //adds the top left fixing hole
                     pointsList.Add(point);
                     points++;
                  }

               }

               //Draw the Left holes
               if (panel.LeftFixingHoles.Equals("1") && Folded && folds == 4) //draw the fixing holes on the left dimension only on folded panels because flat panels do not have the left dimension
               {
                  runningY = panelY1 - foldPanel.LeftFirstFoldSetbackTop - foldPanel.KFactor - panel.LeftHoleSetbackTop;
                  runningX = panelRight - foldPanel.LeftFirstFoldWidth + panel.LeftHoleSetbackLeft;



                  if (foldPanel != null)
                  {
                     if (foldPanel.TopFirstFoldDirection == 1)
                     {
                        runningX = (panelLeft + foldPanel.LeftFirstFoldWidth) - panel.LeftHoleSetbackLeft;
                     }

                  }
                  else
                  {
                     point = new Point3d(runningX, panelY0 + panel.LeftHoleSetbackBottom, 0); //adds the  left bottom fixing hole                     
                     pointsList.Add(point);
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

               // Process the curve              


               // Draw all the holes
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
