using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.IO;
using MetrixGroupPlugins.Commands;
using MetrixGroupPlugins.Utilities;
using MetrixGroupPlugins.CustomFixingHole;
namespace MetrixGroupPlugins
{
    /// <summary>
    /// 
    /// </summary>
    public class FoldedPanelDrawer
    {
        //public FoldedPanelDrawer() { }
        /// <summary>
        /// Contains the code to draw folded panels. The code contains comments to aid new developers to understand the 
        /// code logic and structure
        /// </summary>
        /// 
        /// </summary>
        /// <param name="xLowerBound">The x lower bound.</param>
        /// <param name="xUpperBound">The x upper bound.</param>
        /// <param name="yLowerBound">The y lower bound.</param>
        /// <param name="yUpperBound">The y upper bound.</param>
        /// <param name="panel">The panel.</param>
        /// <param name="para">The para.</param>
        public static void drawPanel(double xLowerBound, double xUpperBound, double yLowerBound, double yUpperBound, FoldedPerforationPanel panel, PanelParameters para)
        {
            string leftText = "AA";
            string rightText = "BB";
            string topText = "CC";
            string bottomText = "DD";
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            string text = "";
            double height = para.LabelHeight / 3;
            const string font = "Arial";
            Guid burrLeader;
            Rhino.Geometry.Point3d pt = new Rhino.Geometry.Point3d(0, 0, 0);
            Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();

            string layerName = null; //name of layers
            int layerIndex = 0; //index of layers
            Rhino.DocObjects.Layer parent_layer_Approval = null; //create variable to hold approval layer 
            Rhino.DocObjects.Layer parent_layer_Nesting = null; //create variable to hold nesting layer

            double foldsY;
            double foldsX;

            Boolean cornerRFTR = true;  //corner relief top right
            Boolean cornerRFTL = true;  //corner relief top left
            Boolean cornerRFBR = true;  // corner relief bottom right
            Boolean cornerRFBL = true;  // corner relief bottom left 

            List<Guid> guidList = new List<Guid>();

            //Initializing variables (bound - where the next panel will be drawn - measurement taken from the folded panel finished layer)
            double panelFirstTopX0 = xLowerBound + panel.TopFirstFoldSetbackLeft;   // Change xLowerBound to panelx0
            double panelFirstTopX1 = xLowerBound + panel.X - panel.TopFirstFoldSetbackRight;
            double panelFirstTopY0 = yUpperBound + panel.Y;
            double panelFirstTopY1 = panelFirstTopY0 + panel.TopFirstFoldWidth;

            double panelFirstBottomX0 = xLowerBound + panel.BottomFirstFoldSetbackLeft;
            double panelFirstBottomX1 = xLowerBound + panel.X - panel.BottomFirstFoldSetbackRight;
            double panelFirstBottomY0 = yUpperBound; //Actually the top
            double panelFirstBottomY1 = panelFirstBottomY0 - panel.BottomFirstFoldWidth; //Actually the bottom

            double panelFirstLeftX0 = xLowerBound; //Actually the right
            double panelFirstLeftX1 = panelFirstLeftX0 - panel.LeftFirstFoldWidth; //Actually the left
            double panelFirstLeftY0 = yUpperBound + panel.LeftFirstFoldSetbackBottom;
            double panelFirstLeftY1 = yUpperBound + panel.Y - panel.LeftFirstFoldSetbackTop;

            double panelFirstRightX0 = xLowerBound + panel.X;
            double panelFirstRightX1 = panelFirstRightX0 + panel.RightFirstFoldWidth;
            double panelFirstRightY0 = yUpperBound + panel.RightFirstFoldSetbackBottom;
            double panelFirstRightY1 = yUpperBound + panel.Y - panel.RightFirstFoldSetbackTop;

            //Turn on/off the fixing holes on sides based on the folds
            if (panel.FixingHoles == "1")
            {
                if (panel.TopFold == 1)
                {
                    panel.TopFixingHoles = "1";
                }
                else
                {
                    panel.TopFixingHoles = "0";
                }
                if (panel.BottomFold == 1)
                {
                    panel.BottomFixingHoles = "1";
                }
                else
                {
                    panel.BottomFixingHoles = "0";
                }
                if (panel.LeftFold == 1)
                {
                    panel.LeftFixingHoles = "1";
                }
                else
                {
                    panel.LeftFixingHoles = "0";
                }
                if (panel.RightFold == 1)
                {
                    panel.RightFixingHoles = "1";
                }
                else
                {
                    panel.RightFixingHoles = "0";
                }
            }
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

            layerName = "LAYERS FOR NESTING";

            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);

            // If layer does not exist
            if (layerIndex == -1)
            {
                // Add a new layer to the document
                layerIndex = doc.Layers.Add(layerName, System.Drawing.Color.Black);
                parent_layer_Nesting = doc.Layers[layerIndex];
            }
            else
            {
                parent_layer_Nesting = doc.Layers[layerIndex];
            }


            // Create a new layer called Panel Finished
            layerName = "FOLDED PANEL FINISHED";

            layerIndex = createSubLayers.createSubLayer(layerName,
                     System.Drawing.Color.Blue, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //Bottom and left justified the panels in the grid (panel x0,x1,y0,y1 - refers to the folds edg (folds layer)
            double panelX0 = xLowerBound;
            double panelX1 = panelX0 + panel.X;
            double panelY0 = yUpperBound;
            double panelY1 = panelY0 + panel.Y;

            List<Point3d> list = new List<Point3d>();
            //The rest of the code adds the folded panel finished bounding box to the panel
            if (panel.TopFirstFoldWidth != 0)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1, 0), new Point3d(panelX1, panelY1, 0));
                guidList.Add(panel.Perimeter);
            }

            if (panel.LeftFirstFoldWidth != 0)
            {

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX0, panelY1, 0));
                guidList.Add(panel.Perimeter);
            }

            if (panel.BottomFirstFoldWidth != 0)
            {

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX1, panelY0, 0));
                guidList.Add(panel.Perimeter);

            }

            if (panel.RightFirstFoldWidth != 0)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0, 0), new Point3d(panelX1, panelY1, 0));
                guidList.Add(panel.Perimeter);

            }
            MetrixUtilities.joinCurves(doc.Layers.Find("FOLDED PANEL FINISHED", true)); //join the closed curves using the method


            // Create a new layer called FOLDS
            layerName = "FOLDS";
            layerIndex = createSubLayers.createSubLayer(layerName,
                   System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //For all sides have folds
            if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold == 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
            }

            // For Top, right and Left
            if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold == 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
            }
            //For Top, Right and Bottom
            if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold != 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
            }

            //Top, Left and Bottom
            if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold != 1 && panel.LeftFold == 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1, panelY1 - panel.KFactor, 0));
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1, panelY0 + panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
            }
            // For Bottom, right and Left
            if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold == 1)
            {
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1, 0));
            }

            //For top and bottom
            if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold != 1 && panel.LeftFold != 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
            }

            //For Left and Right
            if (panel.TopFold != 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold == 1)
            {
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));

            }
            //For Top and Left
            //if all sides have folds
            if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold != 1 && panel.LeftFold == 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1, panelY1 - panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
            }

            //For Top and Right
            //if all sides have folds
            if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold != 1 && panel.LeftFold == 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1, panelY1 - panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
            }
            //For Bottom and Left
            if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold != 1 && panel.LeftFold == 1)
            {
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1, panelY0 + panel.KFactor, 0));
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1, 0));
            }
            //For Bottom and Right
            if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold != 1)
            {
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1, 0));
            }

            //Below codes adds the fold up or down text to the panel (sides, top and bottom)
            //if (folds == 4 || folds == 3)
            //{
            //   //adds the Bottom Text (Fold down or up)
            //   pt = new Point3d(panelFirstBottomX0 + panel.KFactor * 2, panelFirstBottomY1 + panel.KFactor * 2, 0);
            //   plane.Origin = pt;
            //   guidList = addFoldsText(panel.BottomFirstFoldDirection, plane, height, font, guidList);
            //}



            //if (folds == 4 || folds == 3)
            //{
            //   //adds the Top Text (Fold down or up)
            //   pt = new Point3d(panelFirstTopX0 + panel.KFactor, panelFirstTopY0 + panel.KFactor, 0);
            //   plane.Origin = pt;
            //   guidList = addFoldsText(panel.TopFirstFoldDirection, plane, height, font, guidList);
            //}

            //plane = Plane.WorldXY;  //rotate the text
            //plane.XAxis = new Vector3d(0, 1, 0);
            //plane.YAxis = new Vector3d(-1, 0, 0);
            //plane.ZAxis = new Vector3d(0, 0, 1);
            ////Adds the Right hand side Bottom Text (Fold down or up)
            //if (folds == 4 || folds == 2)
            //{
            //   pt = new Point3d(panelFirstRightX1 - panel.KFactor - panel.KFactor, panelFirstRightY0 + panel.KFactor, 0); //-10 to move the fold down text back between
            //   plane.Origin = pt;
            //   guidList = addFoldsText(panel.RightFirstFoldDirection, plane, height, font, guidList);
            //}



            ////adds the Left hand side Bottom Text (Fold down or up)
            //if (folds == 4 || folds == 2)
            //{
            //   pt = new Point3d(panelFirstLeftX0 - panel.KFactor, panelFirstLeftY0 + panel.KFactor, 0);
            //   plane.Origin = pt;
            //   guidList = addFoldsText(panel.LeftFirstFoldDirection, plane, height, font, guidList);

            //}



            // Create a new layer called Border
            layerName = "BORDERS";
            layerIndex = createSubLayers.createSubLayer(layerName,
                   System.Drawing.Color.Purple, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            //Create a bounding box for the borders

            double borderX0 = panelX0 + panel.LeftBorder;  //refers to the borders corners
            double borderY0 = panelY0 + panel.BottomBorder;
            double borderX1 = panelX1 - panel.RightBorder;
            double borderY1 = panelY1 - panel.TopBorder;

            BoundingBox panelBox = new BoundingBox(borderX0, borderY0, 0, borderX1, borderY1, 0);
            List<Point3d> rectangle_corners = panelBox.GetCorners().Distinct().ToList();
            // add 1st point at last to close the loop
            rectangle_corners.Add(rectangle_corners[0]);
            panel.Border = doc.Objects.AddPolyline(rectangle_corners);

            guidList.Add(panel.Border);



            // Create a new layer called Perimeter 
            layerName = "PANEL PERIMETER";

            layerIndex = createSubLayers.createSubLayer(layerName,
              System.Drawing.Color.Black, parent_layer_Nesting); //make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //Below code will set the setbacks to 0 based on the adjacent folds
            //Checking Top
            if (panel.RightFold == 1) //check if the top right has an adjacent fold
            {
                if (panel.RightFirstFoldSetbackTop > 3 || panel.RightFirstFoldSetbackTop == 0) //check if the adjacent fold top set back is more than 3 mil
                {
                    panel.TopFirstFoldSetbackRight = 0;
                    cornerRFTR = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackRight = 0;  //if there is no adjacent fold set the setback to 0
                                                     // panel.RightFirstFoldSetbackTop = 0;
                cornerRFTR = false;
            }

            if (panel.LeftFold == 1) //check if the top left has an adjacent fold
            {
                if (panel.LeftFirstFoldSetbackTop > 3 || panel.LeftFirstFoldSetbackTop == 0) //check if the adjacent fold top set back is more than 3 mi               
                {
                    panel.TopFirstFoldSetbackLeft = 0;
                    cornerRFTL = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackLeft = 0;
                // panel.LeftFirstFoldSetbackTop = 0;
                cornerRFTL = false;
            }


            //Checking Bottom
            if (panel.RightFold == 1)  //check if the bottom has a right adjacent fold
            {
                if (panel.RightFirstFoldSetbackBottom > 3 || panel.RightFirstFoldSetbackBottom == 0) //check if right setback is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackRight = 0; //set the bottom set back right to 0
                    cornerRFBR = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackRight = 0; //set the bottom set back right to 0
                                                       // panel.RightFirstFoldSetbackBottom = 0;
                cornerRFBR = false;
            }

            if (panel.LeftFold == 1)  //check if the bottom has a left adjacent fold
            {
                if (panel.LeftFirstFoldSetbackBottom > 3 || panel.LeftFirstFoldSetbackBottom == 0) //check if left setback is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackLeft = 0; //set the bottom set back left to 0
                    cornerRFBL = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackLeft = 0; //set the bottom set back left to 0
                                                      // panel.LeftFirstFoldSetbackBottom = 0;
                cornerRFBL = false;
            }

            //checking Left 
            if (panel.TopFold == 1) //check if the left fold has a top adjacent fold
            {
                if (panel.TopFirstFoldSetbackLeft > 3 || panel.TopFirstFoldSetbackLeft == 0) //check if the top set back is more than 3 mil
                {
                    panel.TopFirstFoldSetbackLeft = 0; //if yes set the set back to 0
                    cornerRFTL = false;
                }
            }
            else
            {
                //  panel.LeftFirstFoldSetbackTop = 0;
                panel.TopFirstFoldSetbackLeft = 0;
                cornerRFTL = false;
            }

            if (panel.BottomFold == 1) //check if the left fold has a bottom adjacent fold
            {
                if (panel.BottomFirstFoldSetbackLeft > 3 || panel.BottomFirstFoldSetbackLeft == 0) //check if the bottom set back is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackLeft = 0; //if yes set the set back to 0
                    cornerRFBL = false;
                }
            }
            else
            {
                //   panel.LeftFirstFoldSetbackBottom = 0;
                panel.BottomFirstFoldSetbackLeft = 0;
                cornerRFBL = false;
            }

            //Checking Right 
            //right top
            if (panel.TopFold == 1)     //check if  there is top a fold
            {
                if (panel.TopFirstFoldSetbackRight > 3 || panel.TopFirstFoldSetbackRight == 0) //check if the top set back is more than 3 
                {
                    panel.TopFirstFoldSetbackRight = 0; //if yes set the setback to 0
                    cornerRFTR = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackRight = 0;
                //  panel.RightFirstFoldSetbackTop = 0;
                cornerRFTR = false;
            }
            //right bottom
            if (panel.BottomFold == 1) //check if there is a bottom adjacent fold
            {
                if (panel.BottomFirstFoldSetbackRight > 3 || panel.BottomFirstFoldSetbackRight == 0) //check if the bottom setback is more than 3
                {
                    panel.BottomFirstFoldSetbackRight = 0; //if yes set the setback to 0
                    cornerRFBR = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackRight = 0;
                //   panel.RightFirstFoldSetbackBottom = 0;
                cornerRFBR = false;
            }
            if (panel.FixingHoles != "0")
            {
                panel = reCalculateDistances(panel); //recalculate fixing hole quantity and distances
            }
            // Top First Fold (use setbacks to position the folds within the corner relief)
            if (panel.TopFold == 1)
            {
                //go here if all 4 sides have folds
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0));  // horizontal line
                guidList.Add(panel.Perimeter);

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0)); //left
                guidList.Add(panel.Perimeter);

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                guidList.Add(panel.Perimeter);
            }

            // Bottom First Fold
            if (panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0)); //horizontal line
                guidList.Add(panel.Perimeter);

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY0 + panel.BottomFirstFoldSetbackLeft, 0));  //Left (draws from bottom to top)
                guidList.Add(panel.Perimeter);

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY0 + panel.BottomFirstFoldSetbackRight, 0)); //Right (draws from bottom to top)
                guidList.Add(panel.Perimeter);

            }


            // Left First Fold
            if (panel.LeftFold == 1)
            {
                if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold == 1 && panel.BottomFold == 1 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1 ||
                   panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold != 1 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold == 1 || panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold == 1
                   || panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold == 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines (vertical) of the left hand side fold (bottom to top)
                    guidList.Add(panel.Perimeter);
                    if (panel.TopFirstFoldSetbackLeft == 0 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1) //if the top fold set back(LH) is 0 dont setback in to the panel in the left top fold
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                        guidList.Add(panel.Perimeter);

                    }
                    else
                    { //if the top fold set back (LH) is not 0, draw the setback
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0 + panel.LeftFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                    }
                    if (panel.BottomFirstFoldSetbackLeft == 0 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1)//if the bottom fold setback (LH) is 0 dont setback in to the panel in the left bottom fold
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 + panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0, 0)); // draws the vertical bottom line to connect left hand side bottom to bottom left hand side (drawn from top to bottom)
                        guidList.Add(panel.Perimeter);
                    }
                    else //if the bottom fold setback (LH) is not 0, draw the setback
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                    }

                }

                if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold != 1 & panel.BottomFold != 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
                    guidList.Add(panel.Perimeter);
                }


            }

            // Right First Fold
            if (panel.RightFold == 1)
            {
                if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold == 1 && panel.BottomFold == 1 || panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1 || panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1
                   || panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold != 1 || panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold == 1 || panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold == 1
                   || panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1 || panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold == 1)
                {

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //vertical
                    guidList.Add(panel.Perimeter);

                    if (panel.TopFirstFoldSetbackRight == 0 || panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1) //if the top fold (RH) is 0 dont draw the setback in right fold (TH)
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //Top (draw from right to left)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1, 0)); //Draws a vertical line to connect the right hand side fold and the top right hand side
                        guidList.Add(panel.Perimeter);
                    }
                    else //if the the top fold (RH) is not 0, draw the setback 
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackTop, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //Top (draw from right to left)
                        guidList.Add(panel.Perimeter);

                    }
                    if (panel.BottomFirstFoldSetbackRight == 0 || panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1) //if the bottom fold (RH) is 0 dont draw a setback in the right hand side bottom
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //Bottom (draw from right to left)
                        guidList.Add(panel.Perimeter);

                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0, 0)); //Draws a vertical line to connect right hand side bottom and bottom hand side right
                        guidList.Add(panel.Perimeter);
                    }
                    else //if the bottom (RH) is not 0, draw the setback 
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackBottom, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //Bottom (draw from right to left)
                        guidList.Add(panel.Perimeter);
                    }

                }
                if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold != 1 & panel.BottomFold != 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.RightFirstFoldSetbackTop, 0)); //draws the straight line of the right hand side fold
                    guidList.Add(panel.Perimeter);

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the right hand side fold (left to right)
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.RightFirstFoldSetbackTop, 0)); //draws the top line of the right hand side fold 
                    guidList.Add(panel.Perimeter);
                }
            }
            layerIndex = createSubLayers.createSubLayer("FIXING HOLES",
            System.Drawing.Color.Black, doc.Layers[doc.Layers.Find("LAYERS FOR NESTING", true)]);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            double cornerX0 = panelX0 + panel.KFactor;
            double cornerX1 = panelX1 - panel.KFactor;
            double cornerY0 = panelY0 + panel.KFactor;
            double cornerY1 = panelY1 - panel.KFactor;

            //Below code adds the corner relief based on the users selection (Circle =1 and Square =2)
            //Circle corner relief
            if (panel.CornerRelief == 1)   // Circle Corner relief
            {
                double diameter = panel.CornerReliefSize / 2;
                if (cornerRFTR)
                {
                    panel.Perimeter = doc.Objects.AddCircle(new Circle(new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0), diameter)); //TR
                    guidList.Add(panel.Perimeter);
                }
                if (cornerRFTL)
                {
                    panel.Perimeter = doc.Objects.AddCircle(new Circle(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), diameter)); //TL
                    guidList.Add(panel.Perimeter);
                }
                if (cornerRFBR)
                {
                    panel.Perimeter = doc.Objects.AddCircle(new Circle(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), diameter)); //BR
                    guidList.Add(panel.Perimeter);
                }
                if (cornerRFBL)
                {
                    panel.Perimeter = doc.Objects.AddCircle(new Circle(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), diameter)); //BL
                    guidList.Add(panel.Perimeter);
                }
            }
            else if (panel.CornerRelief == 2)  //Square Corner relief
            {
                BoundingBox reliefBox = new BoundingBox(cornerX1 - 0.5 * panel.CornerReliefSize, cornerY1 - 0.5 * panel.CornerReliefSize, 0, cornerX1 + 0.5 * panel.CornerReliefSize, cornerY1 + 0.5 * panel.CornerReliefSize, 0);
                rectangle_corners = reliefBox.GetCorners().Distinct().ToList();
                rectangle_corners.Add(rectangle_corners[0]);
                panel.Perimeter = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Perimeter);
                reliefBox = new BoundingBox(cornerX0 - 0.5 * panel.CornerReliefSize, cornerY1 - 0.5 * panel.CornerReliefSize, 0, cornerX0 + 0.5 * panel.CornerReliefSize, cornerY1 + 0.5 * panel.CornerReliefSize, 0);
                rectangle_corners = reliefBox.GetCorners().Distinct().ToList();
                rectangle_corners.Add(rectangle_corners[0]);
                panel.Perimeter = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Perimeter);
                reliefBox = new BoundingBox(cornerX0 - 0.5 * panel.CornerReliefSize, cornerY0 - 0.5 * panel.CornerReliefSize, 0, cornerX0 + 0.5 * panel.CornerReliefSize, cornerY0 + 0.5 * panel.CornerReliefSize, 0);
                rectangle_corners = reliefBox.GetCorners().Distinct().ToList();
                rectangle_corners.Add(rectangle_corners[0]);
                panel.Perimeter = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Perimeter);
                reliefBox = new BoundingBox(cornerX1 - 0.5 * panel.CornerReliefSize, cornerY0 - 0.5 * panel.CornerReliefSize, 0, cornerX1 + 0.5 * panel.CornerReliefSize, cornerY0 + 0.5 * panel.CornerReliefSize, 0);
                rectangle_corners = reliefBox.GetCorners().Distinct().ToList();
                rectangle_corners.Add(rectangle_corners[0]);
                panel.Perimeter = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Perimeter);
            }


            layerName = "PANEL PERIMETER";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            //drawing connecting lines

            //Drawing for individual folds
            //top
            if (panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold != 1 && panel.BottomFold != 1)
            {
                //Left
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);
                //vertical line right
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY0, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);

                //Horizontal line
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY0, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);
            }
            //Bottom
            if (panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold != 1 && panel.BottomFold == 1)
            {
                //Left
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY0, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY1, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);
                //vertical line right
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY0, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY1, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);

                //Horizontal line
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY1, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY1, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);

            }

            //Left
            if (panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1)
            {
                //Bottom horizontal
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);
                //Top horizontal
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);

                //vertical line right
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);

            }

            //right
            if (panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1)
            {
                //Bottom horizontal
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);
                //Top horizontal
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //draw a horzontal line to connect 2 joints
                guidList.Add(panel.Perimeter);

                //vertical line Left
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                guidList.Add(panel.Perimeter);

            }

            //Connecting Top, Left and Right
            if (panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold != 1)
            {
                //draw a horizontal line connecting left and right folds (from bottom)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackBottom, panelY0 + panel.RightFirstFoldSetbackBottom, 0));
                guidList.Add(panel.Perimeter);
            }

            //Connecting Bottom, Left and Right
            if (panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold == 1)
            {
                //draw a horizontal line connecting left and right folds  (from top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackTop, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackTop, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //Top (draw from right to left)
                guidList.Add(panel.Perimeter);
            }

            //Connect Top right and bottom right
            if (panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold == 1)
            {
                //draw a vertical line connecting Top right and Bottom right folds  (from top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY0 + panel.TopFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                guidList.Add(panel.Perimeter);
            }

            //Connect Top Left and bottom Left
            if (panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold == 1)
            {
                //draw a vertical line connecting Top right and Bottom right folds  (from top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0 + panel.TopFirstFoldSetbackLeft, 0)); //left
                guidList.Add(panel.Perimeter);
            }
            //Connecting Top and Bottom
            if (panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold != 1 && panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0 + panel.TopFirstFoldSetbackLeft, 0)); //left
                guidList.Add(panel.Perimeter);

                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY0 + panel.TopFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                guidList.Add(panel.Perimeter);
            }

            //Connecting top and left
            if (panel.TopFold == 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold != 1)
            {
                //right
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY0 + panel.BottomFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                guidList.Add(panel.Perimeter);
                //bottom            
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackBottom, panelY0 + panel.BottomFirstFoldSetbackRight, 0));
                guidList.Add(panel.Perimeter);
            }

            //Connecting top and Right
            if (panel.TopFold == 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold != 1)
            {
                //left
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0 + panel.TopFirstFoldSetbackLeft, 0)); //left
                guidList.Add(panel.Perimeter);
                //bottom
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackBottom, panelY0 + panel.RightFirstFoldSetbackBottom, 0));
                guidList.Add(panel.Perimeter);
            }

            //Connecting Bottom and Left
            if (panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold != 1 && panel.BottomFold == 1)
            {
                //right (drawn from top to bottom)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY1 - panel.BottomFirstFoldSetbackRight, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY0 + panel.BottomFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                guidList.Add(panel.Perimeter);
                //top  (drawn from left to right)          
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0));
                guidList.Add(panel.Perimeter);
            }
            //Connecting Bottom and Right
            if (panel.TopFold != 1 && panel.LeftFold != 1 && panel.RightFold == 1 && panel.BottomFold == 1)
            {
                //left (drawn from top to bottom)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY1 - panel.BottomFirstFoldSetbackLeft, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY0 + panel.BottomFirstFoldSetbackLeft, 0)); //left
                guidList.Add(panel.Perimeter);
                //Top (drawn left to right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.LeftFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0));
                guidList.Add(panel.Perimeter);
            }
            //Connecting Left and Right
            if (panel.TopFold != 1 && panel.LeftFold == 1 && panel.RightFold == 1 && panel.BottomFold != 1)
            {
                //bottom
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1, 0), new Point3d(panelX1, panelY1, 0));
                guidList.Add(panel.Perimeter);
                //bottom
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX1, panelY0, 0));
                guidList.Add(panel.Perimeter);

                //left top
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1, 0), new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0));
                guidList.Add(panel.Perimeter);
                //Left bottom            
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0, 0), new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0));
                guidList.Add(panel.Perimeter);

                //right top
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY1, 0), new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0));
                guidList.Add(panel.Perimeter);
                //right bottom            
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0, 0), new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0));
                guidList.Add(panel.Perimeter);
            }
            // Create a new layer called LABELS
            layerName = "LABELS";
            layerIndex = createSubLayers.createSubLayer(layerName,
               System.Drawing.Color.Red, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            text = panel.PartName;
            height = para.LabelHeight;
            pt = new Rhino.Geometry.Point3d(borderX0, borderY0 + height, 0);
            plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            plane.Origin = pt;
            panel.Label = doc.Objects.AddText(text, plane, height, font, false, false);
            guidList.Add(panel.Label);

            // If dotFontLabel is more than 0 draw the dot font text on the panel, else skip
            // if (para.DotFont == 1)
            if (panel.DotFontLabel > 0)
            {
                // Create a new layer called DOT SCRIBE LABEL
                layerName = "DOT SCRIBE LABEL";

                layerIndex = createSubLayers.createSubLayer(layerName,
                 System.Drawing.Color.Black, parent_layer_Nesting); //make Nesting layer the parent layer


                doc.Layers.SetCurrentLayerIndex(layerIndex, true);

                // Put in the Dot Matrix Label
                // Draw at the right side of the border aand 10mm from the bottom and 100mm from the left edge
                pt = new Point3d(panelX1 - 100, panelY0 + 5, 0);
                //  DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, Properties.Settings.Default.DotMatrixHeight);
                if (panel.DotFontLabellerSide.ToUpper().Equals("REVERSED"))
                {
                    DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, Properties.Settings.Default.DotMatrixHeight, panel.X); //set the size of dotfont 
                }
                else  //If not revered use front labeller
                {
                    DotMatrixFrontLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, Properties.Settings.Default.DotMatrixHeight);
                }
            }

            if (para.PatternDirection == 1)
            {
                // Export the selected curves
                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
            }
            if (panel.DrawPerf == 1)
            {
                PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(panel.Border).Curve());
                perforationForm.drawPerforationDesign(panel.PatternName, true);
            }
            if (para.PatternDirection == 1)
            {
                // Export the selected curves
                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
            }

            // Draw Folded layer on LHS
            layerName = "DIMENSIONS BLUE";  //This layer is responsible for blue colour dimensions on the panel
            layerIndex = createSubLayers.createSubLayer(layerName,
                   System.Drawing.Color.Blue, parent_layer_Approval);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            double panelRight = 0;
            double panelLeft = 0;
            double panelRightHS = 0;
            double panelLeftHS = 0;
            List<Line> listpoint = new List<Line>();
            List<Curve> listcurve = new List<Curve>();

            panelRight = panelFirstLeftX1 - 9 * (panelFirstLeftX0 - panelFirstLeftX1);
            panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;
            panelRightHS = panelFirstRightX1 + 9 * (panelFirstRightX1 - panelFirstRightX0);
            panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

            //Draws the Left and Right
            if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1 && panel.BottomFold == 1) //top and bottom fold up (left)
            {
                panelRight = panelFirstLeftX1 - 9 * (panelFirstLeftX0 - panelFirstLeftX1);
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelLeft, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));
                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                panelRightHS = panelFirstRightX1 + 9 * (panelFirstRightX1 - panelFirstRightX0);
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelLeftHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));

            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1) //top and bottom burr
            {
                panelRight = panelFirstLeftX1 - 9 * (panelFirstLeftX0 - panelFirstLeftX1);
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelRight, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))); //upper bottom horizontal line

                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0))); //Bottom Upper horizontal line 
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))); //bottom bottom horizontal line


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))); //bottom bottom horizontal line


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                panelRightHS = panelFirstRightX1 + 9 * (panelFirstRightX1 - panelFirstRightX0);
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelRightHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))); //upper bottom hor line

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0))); //Upper bottom vertical line 
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))); //bottom bottom vertical line


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));


            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1) //bottom burr
            {
                panelRight = panelFirstLeftX1 - 9 * (panelFirstLeftX0 - panelFirstLeftX1);
                panelLeft = panelRight - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);

                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness - panel.TopFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0))));


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                panelRightHS = panelFirstRightX1 + 9 * (panelFirstRightX1 - panelFirstRightX0);
                panelLeftHS = panelRightHS - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);

                listpoint.Add(new Line(new Point3d(panelRightHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS - panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS + panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS - panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS + panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0))));

            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 1 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 1 && panel.BottomFold == 1) //top burr
            {

                panelRight = panelFirstLeftX1 - 9 * (panelFirstLeftX0 - panelFirstLeftX1);
                panelLeft = panelRight - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - 2 * panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - 2 * panel.SheetThickness - panel.BottomFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.SheetThickness + panel.TopFirstFoldWidth, panelY1, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0))));


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                panelRightHS = panelFirstRightX1 + 9 * (panelFirstRightX1 - panelFirstRightX0);
                panelLeftHS = panelRightHS - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);


                listpoint.Add(new Line(new Point3d(panelRightHS - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS - panel.SheetThickness - panel.BottomFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS - 2 * panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS - panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS + panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0))));

            }


            //This section of the code will draw the top Blue dimensions. The blue dimension will be drawn based on the user's
            //input values
            double panelTop = 0;
            double panelBottom = 0;
            double panelBottomBH = 0;
            double panelTopTH = 0;
            listpoint.Clear();
            listcurve.Clear();
            panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
            panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;
            panelBottomBH = panelFirstBottomY0 - 9 * ((panelFirstTopY1 - panelFirstTopY0));
            panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

            if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1 && panel.RightFold == 1) //left and right fold up
            {
                panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                //draw the top              
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop, 0)));  //This and the below line of code draws the horizontal lines in the top blue dimension
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0))));

                //draw the bottom 
                panelBottomBH = panelFirstBottomY0 - 9 * ((panelFirstTopY1 - panelFirstTopY0));
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0))));



            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.RightFold == 1) //left and right burr
            { //place where top is drawn
                panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));

                guidList.Add(doc.Objects.AddCurve(listcurve[2]));

                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //here here

                guidList.Add(doc.Objects.AddCurve(listcurve[4]));

                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0))));


                //draw bottom
                panelBottomBH = panelFirstBottomY0 - 9 * ((panelFirstTopY1 - panelFirstTopY0));
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth, 0))));


            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 2 && panel.RightFold == 1) //right burr
            {
                //Top
                panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
                panelTop = panelBottom + (panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth + 3 * panel.SheetThickness);


                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness + panel.LeftFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + 2 * panel.SheetThickness + panel.LeftFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + 2 * panel.SheetThickness + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop - panel.SheetThickness - panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness - panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.SheetThickness + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0))));


                //draw bottom
                panelBottomBH = panelFirstBottomY0 - 9 * ((panelFirstTopY1 - panelFirstTopY0));
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;


                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0))));


            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 1 && panel.RightFold == 1) //left 

            {
                //Top
                panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
                panelTop = panelBottom + (panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth + 3 * panel.SheetThickness);


                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + 2 * panel.SheetThickness + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + 2 * panel.SheetThickness + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.SheetThickness + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop - panel.SheetThickness - panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness - panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtEnd, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0))));


                //draw bottom
                panelBottomBH = panelFirstBottomY0 - 9 * ((panelFirstTopY1 - panelFirstTopY0));
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;


                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0))));

            }

            //Adds the Top Face Dimension blue  (fold direction down)
            //0.6 is added trim the left and right (vertical lines) 
            if (panel.TopFold == 1 && panel.TopFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.TopFold == 1 && panel.TopFirstFoldDirection == 2 && panel.RightFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelBottom + panel.TopFirstFoldWidth), 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelBottom + panel.TopFirstFoldWidth), 0));  // horizontal line
                guidList.Add(panel.Perimeter);
                if (panel.TopFold == 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelBottom + 0.6, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelBottom + panel.TopFirstFoldWidth), 0)); //left 
                    guidList.Add(panel.Perimeter);

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelBottom + 0.6, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelBottom + panel.TopFirstFoldWidth), 0)); //right
                    guidList.Add(panel.Perimeter);
                }
            }

            //Adds the Top Face Dimension blue (fold direction up) 
            //0.6 is subtracted trim the left and right (vertical lines) 
            if (panel.TopFold == 1 && panel.TopFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.TopFold == 1 && panel.TopFirstFoldDirection == 1 && panel.RightFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelTop - panel.TopFirstFoldWidth), 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelTop - panel.TopFirstFoldWidth), 0));  // horizontal line
                guidList.Add(panel.Perimeter);
                if (panel.TopFold == 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelTop - 0.6, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelTop - panel.TopFirstFoldWidth), 0)); //left 
                    guidList.Add(panel.Perimeter);

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelTop - 0.6, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelTop - panel.TopFirstFoldWidth), 0)); //right
                    guidList.Add(panel.Perimeter);
                }
            }


            //Adds the bottom Face dimension blue (down)
            //0.6 is added trim the left and right (vertical lines) 
            if (panel.BottomFold == 1 && panel.BottomFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.BottomFold == 1 && panel.BottomFirstFoldDirection == 2 && panel.RightFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH - panel.BottomFirstFoldWidth), 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH - panel.BottomFirstFoldWidth), 0));  // horizontal line
                guidList.Add(panel.Perimeter);
                if (panel.BottomFold == 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelBottomBH - 0.6, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH - panel.BottomFirstFoldWidth), 0)); //left 
                    guidList.Add(panel.Perimeter);

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelBottomBH - 0.6, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH - panel.BottomFirstFoldWidth), 0)); //right
                    guidList.Add(panel.Perimeter);
                }
            }

            //Adds the bottom Face dimension blue (up)
            if (panel.BottomFold == 1 && panel.BottomFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.BottomFold == 1 && panel.BottomFirstFoldDirection == 1 && panel.RightFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH + panel.BottomFirstFoldWidth), 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH + panel.BottomFirstFoldWidth), 0));  // horizontal line
                guidList.Add(panel.Perimeter);
                if (panel.BottomFold == 1)
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelBottomBH + 0.6, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH + panel.BottomFirstFoldWidth), 0)); //left 
                    guidList.Add(panel.Perimeter);

                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelBottomBH + 0.6, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH + panel.BottomFirstFoldWidth), 0)); //right
                    guidList.Add(panel.Perimeter);
                }
            }

            //Adds the Left Face Dimension blue  (fold direction down)
            if (panel.LeftFold == 1 && panel.LeftFirstFoldDirection == 2 && panel.TopFold == 1 || panel.LeftFold == 1 && panel.LeftFirstFoldDirection == 2 && panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelRight - 0.6, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelRight - 0.6, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
                guidList.Add(panel.Perimeter);
            }
            //Fold direction is Up
            if (panel.LeftFold == 1 && panel.LeftFirstFoldDirection == 1 && panel.TopFold == 1 || panel.LeftFold == 1 && panel.LeftFirstFoldDirection == 1 && panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelLeft + 0.6, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelLeft + 0.6, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
                guidList.Add(panel.Perimeter);
            }

            //Adds the Right Face Dimension blue  (fold direction up)
            if (panel.RightFold == 1 && panel.RightFirstFoldDirection == 2 && panel.TopFold == 1 || panel.RightFold == 1 && panel.RightFirstFoldDirection == 2 && panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the straight lines of the right hand side fold (bottom to top)
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelRightHS + 0.6, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the right hand side fold (left to right)
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelRightHS + 0.6, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the upper line of the right hand side fold  (left to right)                                                        
                guidList.Add(panel.Perimeter);
            }
            if (panel.RightFold == 1 && panel.RightFirstFoldDirection == 1 && panel.TopFold == 1 || panel.RightFold == 1 && panel.RightFirstFoldDirection == 1 && panel.BottomFold == 1)
            {
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelLeftHS - 0.6, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
                guidList.Add(panel.Perimeter);
                panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelLeftHS - 0.6, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
                guidList.Add(panel.Perimeter);
            }


            // Add the word perforated area to the panel
            pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 33, 0);

            text = System.Text.RegularExpressions.Regex.Unescape(panel.PerfText); //will make the /n unescape

            height = para.LabelHeight / 2;
            plane.Origin = pt;
            Guid perforatedAreaLabel = doc.Objects.AddText(text, plane, height, font, false, false);

            guidList.Add(perforatedAreaLabel);

            MetrixUtilities.createMetrixRealDimension(); //creates the metrix real dimension and set as the default 

            Point3d origin = new Point3d(0, 0, 0);
            Point3d offset = new Point3d(0, 0, 0);
            Point2d ext1;
            Point2d ext2;
            Point2d linePt;
            LinearDimension dimension;
            Guid dimGuid = new Guid();
            double u, v;
            // Add horizontal dimension (top)
            if (panel.LeftFold == 1 || panel.RightFold == 1)
            {
                origin = new Point3d(panelX0, panelBottom + panel.TopFirstFoldWidth + 0.25 * (panelFirstTopY1 - panelFirstTopY0), 0);
                offset = new Point3d(panelX1, panelBottom + panel.TopFirstFoldWidth + 0.25 * (panelFirstTopY1 - panelFirstTopY0), 0);
                pt = new Point3d((offset.X - origin.X) / 2, panelBottom + 3 * (panelFirstTopY1 - panelFirstTopY0), 0); //change to make the dimension blue line higher and lower

                plane = Plane.WorldXY;
                plane.Origin = origin;


                Point2d extDimensionsBlue;
                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);
                extDimensionsBlue = ext2;
                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid); //horizontal blue dimension added

                //Add Bottom Dimension
                origin = new Point3d(panelX0, (panelBottomBH - panel.BottomFirstFoldWidth) + 0.25 * (panelFirstBottomY1 - panelFirstBottomY0), 0);
                offset = new Point3d(panelX1, (panelBottomBH - panel.BottomFirstFoldWidth) + 0.25 * (panelFirstBottomY1 - panelFirstBottomY0), 0);
                pt = new Point3d((offset.X - origin.X) / 2, panelBottomBH + 3 * (panelFirstBottomY1 - panelFirstBottomY0), 0); //change to make the dimension blue line higher and lower

                plane = Plane.WorldXY;
                plane.Origin = origin;


                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);
                extDimensionsBlue = ext2;
                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid); //horizontal blue dimension added

            }
            // Add LR fold dimension
            List<Point3d> leaderPoints = new List<Point3d>();
            if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == panel.RightFirstFoldDirection) //LR panels fold the same direction
            {
                if (panel.LeftFold == 1 || panel.RightFold == 1)
                {
                    origin = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelBottom, 0);
                    offset = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelTop, 0);
                    pt = new Point3d(panelX0 - 1 * (panel.LeftFirstFoldWidth), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)

                    plane = Plane.WorldXY;
                    plane.XAxis = new Vector3d(0, -1, 0);
                    plane.YAxis = new Vector3d(-1, 0, 0);
                    plane.ZAxis = new Vector3d(0, 0, -1);
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);

                    ext2 = new Point2d(-panel.LeftFirstFoldWidth, v); //set the left fold dimension number to fold width

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    origin = new Point3d(panelX1 - 0.10 * (panel.RightFirstFoldWidth), panelBottom, 0);
                    offset = new Point3d(panelX1 - 0.10 * (panel.RightFirstFoldWidth), panelTop, 0);
                    pt = new Point3d(panelX1 + 2 * (panel.RightFirstFoldWidth), (offset.X - origin.X) / 2, 0); //right hand side (dimension number)

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);

                    ext2 = new Point2d(-panel.RightFirstFoldWidth, v);  //set the Right fold dimension number to the fold width

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }
                //set up for burr side print
                if (panel.LeftFirstFoldDirection == 1) //fold up
                {
                    origin = new Point3d((panelX1 + panelX0) / 1.15, panelTop - panel.SheetThickness, 0);
                    offset = new Point3d((panelX1 + panelX0) / 1.15, panelTop, 0);
                }
                else
                {
                    origin = new Point3d((panelX1 + panelX0) / 1.15, panelBottom, 0);
                    offset = new Point3d((panelX1 + panelX0) / 1.15, panelBottom + panel.SheetThickness, 0);
                }

                if (panel.LeftFold == 1 || panel.RightFold == 1)
                {
                    //adds the bottom dimensions 
                    if (panel.BottomFirstFoldDirection == 1)
                    {
                        origin = new Point3d(panelX0 + 0.25 * (panel.BottomFirstFoldWidth), panelBottomBH, 0);
                        offset = new Point3d(panelX0 + 0.25 * (panel.BottomFirstFoldWidth), panelTopTH, 0);
                        pt = new Point3d(panelX0 - 1 * (panel.BottomFirstFoldWidth), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                    }
                    if (panel.BottomFirstFoldDirection == 2)
                    {
                        origin = new Point3d(panelX0 + 0.25 * (panel.BottomFirstFoldWidth), panelBottomBH - panel.BottomFirstFoldWidth, 0);
                        offset = new Point3d(panelX0 + 0.25 * (panel.BottomFirstFoldWidth), panelTopTH - panel.BottomFirstFoldWidth, 0);
                        pt = new Point3d(panelX0 - 1 * (panel.BottomFirstFoldWidth), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                    }

                    plane = Plane.WorldXY;
                    plane.XAxis = new Vector3d(0, -1, 0);
                    plane.YAxis = new Vector3d(-1, 0, 0);
                    plane.ZAxis = new Vector3d(0, 0, -1);
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);

                    ext2 = new Point2d(-panel.BottomFirstFoldWidth, v); //set the left fold dimension number to fold width

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);
                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);
                    guidList.Add(dimGuid);


                    if (panel.BottomFirstFoldDirection == 1)
                    {
                        origin = new Point3d(panelX1 - 0.10 * (panel.BottomFirstFoldWidth), panelBottomBH, 0);
                        offset = new Point3d(panelX1 - 0.10 * (panel.BottomFirstFoldWidth), panelTopTH, 0);
                        pt = new Point3d(panelX1 + 2 * (panel.BottomFirstFoldWidth), (offset.X - origin.X) / 2, 0); //right hand side (dimension number)
                    }
                    if (panel.BottomFirstFoldDirection == 2)
                    {
                        origin = new Point3d(panelX1 - 0.10 * (panel.BottomFirstFoldWidth), panelBottomBH - panel.BottomFirstFoldWidth, 0);
                        offset = new Point3d(panelX1 - 0.10 * (panel.BottomFirstFoldWidth), panelTopTH - panel.BottomFirstFoldWidth, 0);
                        pt = new Point3d(panelX1 + 2 * (panel.BottomFirstFoldWidth), (offset.X - origin.X) / 2, 0); //right hand side (dimension number)
                    }
                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);

                    ext2 = new Point2d(-panel.BottomFirstFoldWidth, v);  //set the Right fold dimension number to the fold width

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    //Adds the thickness dimension (bottom)

                    if (panel.BottomFirstFoldDirection == 2) //fold down
                    {
                        origin = new Point3d((panelX0 + panelX1) / 2, (panelBottomBH - panel.SheetThickness), 0);
                        offset = new Point3d((panelX0 + panelX1) / 2, panelBottomBH, 0);
                        pt = new Point3d((panelX0 + panelX1) / 2, panelBottomBH, 0);
                    }

                    if (panel.BottomFirstFoldDirection == 1) //fold Up
                    {
                        origin = new Point3d((panelX0 + panelX1) / 2, (panelBottomBH + panel.SheetThickness), 0);
                        offset = new Point3d((panelX0 + panelX1) / 2, panelBottomBH, 0);
                        pt = new Point3d((panelX0 + panelX1) / 2, panelBottomBH, 0);
                    }

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = " " + dimension.Text;
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);


                    //Adds the thickness dimension top

                    if (panel.TopFirstFoldDirection == 2) //fold down
                    {
                        origin = new Point3d((panelX0 + panelX1) / 2, panelBottom, 0);
                        offset = new Point3d((panelX0 + panelX1) / 2, panelBottom + panel.SheetThickness, 0);
                        pt = new Point3d((panelX0 + panelX1) / 2, panelTop, 0);
                    }

                    if (panel.TopFirstFoldDirection == 1) //fold Up
                    {
                        origin = new Point3d((panelX0 + panelX1) / 2, panelBottom, 0);
                        offset = new Point3d((panelX0 + panelX1) / 2, panelBottom - panel.SheetThickness, 0);
                        pt = new Point3d((panelX0 + panelX1) / 2, panelTop, 0);
                    }

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = " " + dimension.Text;
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }
                //set up for burr side print
                if (panel.LeftFirstFoldDirection == 1) //fold up
                {
                    origin = new Point3d((panelX1 + panelX0) / 1.15, panelTop - panel.SheetThickness, 0);
                    offset = new Point3d((panelX1 + panelX0) / 1.15, panelTop, 0);
                }
                else
                {
                    origin = new Point3d((panelX1 + panelX0) / 1.15, panelBottom, 0);
                    offset = new Point3d((panelX1 + panelX0) / 1.15, panelBottom + panel.SheetThickness, 0);
                }
            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 2) //right burr
            {
                origin = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelTop - 2 * panel.SheetThickness - panel.LeftFirstFoldWidth, 0);
                offset = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelTop - panel.SheetThickness, 0);
                pt = new Point3d(panelX0 - 0.75 * (panel.LeftFirstFoldWidth), (offset.X - origin.X) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                origin = new Point3d(panelX1 - 0.25 * (panel.RightFirstFoldWidth), panelBottom + panel.SheetThickness, 0);
                offset = new Point3d(panelX1 - 0.25 * (panel.RightFirstFoldWidth), panelBottom + 2 * panel.SheetThickness + panel.RightFirstFoldWidth, 0);
                pt = new Point3d(panelX1 + 0.75 * (panel.RightFirstFoldWidth), (offset.X - origin.X) / 2, 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                //set up for burr side print
                origin = new Point3d((panelX1 + panelX0) / 1.15, panelTop - 2 * panel.SheetThickness - panel.LeftFirstFoldWidth, 0);
                offset = new Point3d((panelX1 + panelX0) / 1.15, panelTop - panel.SheetThickness - panel.LeftFirstFoldWidth, 0);
            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 1) //left burr
            {
                origin = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelBottom + 2 * panel.SheetThickness + panel.LeftFirstFoldWidth, 0);
                offset = new Point3d(panelX0 + 0.25 * (panel.LeftFirstFoldWidth), panelBottom + panel.SheetThickness, 0);
                pt = new Point3d(panelX0 - 0.75 * (panel.LeftFirstFoldWidth), (offset.X - origin.X) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                origin = new Point3d(panelX1 - 0.25 * (panel.RightFirstFoldWidth), panelTop - panel.SheetThickness, 0);
                offset = new Point3d(panelX1 - 0.25 * (panel.RightFirstFoldWidth), panelTop - 2 * panel.SheetThickness - panel.RightFirstFoldWidth, 0);
                pt = new Point3d(panelX1 + 0.75 * (panel.RightFirstFoldWidth), (offset.X - origin.X) / 2, 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                //set up for burr side print
                origin = new Point3d((panelX1 + panelX0) / 1.15, panelTop - 2 * panel.SheetThickness - panel.RightFirstFoldWidth, 0);
                offset = new Point3d((panelX1 + panelX0) / 1.15, panelTop - panel.SheetThickness - panel.RightFirstFoldWidth, 0);
            }

            if (panel.SideRequired.Equals("Burr Side") || panel.SideRequired.Equals("Both Sides")) //add the burr side labels only if the user wants to
            {
                if (panel.TopFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 2 && panel.RightFold == 1) //fold down
                {
                    //Add BURR SIDE label Top
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y, 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y + 20 * (offset.Y - origin.Y), 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX1 - panelX0) / 8, offset.Y + 20 * (offset.Y - origin.Y), 0));

                    text = "BURR SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }
                if (panel.BottomFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 2 && panel.RightFold == 1) //fold down
                {
                    //Add BURR SIDE label Bottom
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH - panel.SheetThickness, 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH + 2 * (panelBottomBH - panelTopTH), 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX1 - panelX0) / 8, panelBottomBH + 2 * (panelBottomBH - panelTopTH), 0));

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }

                //If panel direction is up
                if (panel.TopFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 1 && panel.RightFold == 1)
                {
                    //Add BURR SIDE label Top
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y, 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y - 20 * (offset.Y - origin.Y), 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX1 - panelX0) / 8, offset.Y - 20 * (offset.Y - origin.Y), 0));

                    text = "BURR SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }

                if (panel.BottomFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 1 && panel.RightFold == 1)
                {
                    //Add BURR SIDE label Bottom
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH - panel.SheetThickness, 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH - 2 * (panelBottomBH - panelTopTH), 0));
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX1 - panelX0) / 8, panelBottomBH - 2 * (panelBottomBH - panelTopTH), 0));

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }
            }


            if (panel.SideRequired.Equals("Face Side") || panel.SideRequired.Equals("Both Sides")) //add the Face side labels only if the user wants to
            {
                if (panel.TopFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 2 && panel.RightFold == 1) //fold down
                {
                    //Add Face SIDE label (Horizontally)   Top     
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y - panel.SheetThickness, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y - panel.SheetThickness + 10 * (origin.Y - offset.Y), 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, offset.Y - panel.SheetThickness + 10 * (origin.Y - offset.Y), 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }
                if (panel.BottomFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 2 && panel.RightFold == 1) //fold down
                {

                    //Add Face SIDE label (Horizontally)   Bottom     
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH - 2 * (panelBottomBH - panelTopTH), 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, panelBottomBH - 2 * (panelBottomBH - panelTopTH), 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";


                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }

                //If panel direction is up
                if (panel.TopFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 1 && panel.RightFold == 1)
                {
                    //Add Face SIDE label (Horizontally)    Top    
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, offset.Y - 20 * (origin.Y - offset.Y), 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, offset.Y - 20 * (origin.Y - offset.Y), 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }
                if (panel.BottomFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 1 && panel.RightFold == 1)
                {
                    //Add Face SIDE label (Horizontally)   Bottom     
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH + 2 * (panelBottomBH - panelTopTH), 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, panelBottomBH + 2 * (panelBottomBH - panelTopTH), 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }

            }

            //Adds the top text (CC)
            panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelX0, panelBottom - (80 * 2), 0));  //reduce by sheet thickness to point the arrow head correctly
            leaderPoints.Add(new Point3d(panelX0, panelBottom - (80 * 2), 0));   //end point of arrow
            leaderPoints.Add(new Point3d(panelX0 - 0.1, panelBottom - (50 * 2), 0)); //starting point of the arrow (with text)
            text = topText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            ////Adds the Bottom text (DD)
            panelBottom = panelFirstTopY1 + 7 * (panelFirstTopY1 - panelFirstTopY0);
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelX0, panelBottomBH + (90 * 2), 0));  //reduce by sheet thickness to point the arrow head correctly
            leaderPoints.Add(new Point3d(panelX0, panelBottomBH + (90 * 2), 0));   //end point of arrow
            leaderPoints.Add(new Point3d(panelX0 - 0.1, panelBottomBH + (60 * 2), 0)); //starting point of the arrow (with text)
            text = bottomText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            // Add vertical dimension for panel
            //left vertical dimension
            if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.LeftFold == 1 && panel.RightFold == 1 ||
               panel.TopFold == 1 && panel.BottomFold == 1)
            {

                if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.LeftFold == 1 && panel.RightFold == 1)
                {
                    origin = new Point3d(panelRight - 1 * (panelFirstLeftX0 - panelFirstLeftX1), panelY0, 0); //adjust to make top horizontal line move 
                    offset = new Point3d(panelRight - 1 * (panelFirstLeftX0 - panelFirstLeftX1), panelY1, 0); //adjust to make bottom horizontal line move 
                    pt = new Point3d(panelRight - 3 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0); //adjust here to make the line move horizontally 
                }
                else
             if (panel.TopFold == 1 || panel.BottomFold == 1)
                {
                    origin = new Point3d(panelRight - 5 * (panelFirstLeftX0 - panelFirstLeftX1), panelY0, 0); //0.25
                    offset = new Point3d(panelRight - 5 * (panelFirstLeftX0 - panelFirstLeftX1), panelY1, 0); //0.25
                    pt = new Point3d(panelRight - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                }

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);

                dimGuid = doc.Objects.AddLinearDimension(dimension);
                guidList.Add(dimGuid);
            }
            // Add TB fold dimension
            if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == panel.BottomFirstFoldDirection
               && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == panel.BottomFirstFoldDirection
               && panel.BottomFold == 1) //TB panels fold the same direction
            {
                if (panel.TopFold == 1 || panel.BottomFold == 1)
                {
                    origin = new Point3d(panelLeft, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                    offset = new Point3d(panelRight, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                    pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - 1.8 * (panel.BottomFirstFoldWidth), 0);

                    plane = Plane.WorldXY;
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(panel.TopFirstFoldWidth, v);
                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    origin = new Point3d(panelLeft, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                    offset = new Point3d(panelRight, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                    pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(panel.BottomFirstFoldWidth, v);
                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    //Add panel thickness dimension -left hand side (between the sheet) 
                    if (panel.TopFirstFoldDirection == 1) //fold up
                    {
                        origin = new Point3d(panelLeft, (panelY1 + panelY0) / 1.65, 0);
                        offset = new Point3d(panelLeft + panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 1.65, 0);
                    }

                    if (panel.TopFirstFoldDirection == 2)
                    {
                        origin = new Point3d(panelRight - panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                        offset = new Point3d(panelRight, (panelY1 + panelY0) / 1.65, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 1.65, 0);
                    }

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = " " + dimension.Text;
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }

                //add Right dimension 
                if (panel.TopFold == 1 || panel.BottomFold == 1)
                {
                    if (panel.RightFirstFoldDirection == 1)
                    {
                        origin = new Point3d(panelLeftHS - panel.RightFirstFoldWidth, panelY0 + 0.25 * (panel.RightFirstFoldWidth), 0);
                        offset = new Point3d(panelRightHS - panel.RightFirstFoldWidth, panelY0 + 0.25 * (panel.RightFirstFoldWidth), 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - 1.8 * (panel.RightFirstFoldWidth), 0);
                    }

                    if (panel.RightFirstFoldDirection == 2)
                    {
                        origin = new Point3d(panelLeftHS + panel.RightFirstFoldWidth, panelY0 + 0.25 * (panel.RightFirstFoldWidth), 0);
                        offset = new Point3d(panelRightHS + panel.RightFirstFoldWidth, panelY0 + 0.25 * (panel.RightFirstFoldWidth), 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - 1.8 * (panel.RightFirstFoldWidth), 0);
                    }
                    plane = Plane.WorldXY;
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(panel.TopFirstFoldWidth, v);
                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    if (panel.RightFirstFoldDirection == 1)
                    {
                        origin = new Point3d(panelLeftHS - panel.RightFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                        offset = new Point3d(panelRightHS - panel.RightFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);
                    }
                    if (panel.RightFirstFoldDirection == 2)
                    {
                        origin = new Point3d(panelLeftHS + panel.RightFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                        offset = new Point3d(panelRightHS + panel.RightFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);
                    }
                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(panel.BottomFirstFoldWidth, v);
                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);

                    //add right dimension thickness
                    //Add panel thickness dimension - right hand side
                    if (panel.TopFirstFoldDirection == 1) //fold up
                    {
                        origin = new Point3d(panelLeftHS, (panelY1 + panelY0) / 1.65, 0);
                        offset = new Point3d(panelLeftHS + panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 1.65, 0);
                    }

                    if (panel.TopFirstFoldDirection == 2) //fold down
                    {
                        origin = new Point3d(panelRightHS + panel.SheetThickness, (panelY1 + panelY0) / 2, 0);
                        offset = new Point3d(panelRightHS, (panelY1 + panelY0) / 2, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 2.1, 0);
                    }

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = " " + dimension.Text;
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }


            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold == 1
               || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 && panel.BottomFold == 1) //Bottom burr
            {
                origin = new Point3d(panelLeft + panel.SheetThickness, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                offset = new Point3d(panelLeft + 2 * panel.SheetThickness + panel.BottomFirstFoldWidth, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - 1.8 * (panel.BottomFirstFoldWidth), 0);

                plane = Plane.WorldXY;
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                origin = new Point3d(panelRight - 2 * panel.SheetThickness - panel.TopFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                offset = new Point3d(panelRight - panel.SheetThickness, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                //Add panel thickness dimension
                origin = new Point3d(panelLeft + panel.BottomFirstFoldWidth + panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                offset = new Point3d(panelLeft + panel.BottomFirstFoldWidth + 2 * panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 1.65, 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimension.Text = " " + dimension.Text;
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);
            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 1 && panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 1 && panel.LeftFold == 1 && panel.RightFold == 1) //Top burr
            {
                origin = new Point3d(panelLeft + panel.SheetThickness, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                offset = new Point3d(panelLeft + 2 * panel.SheetThickness + panel.TopFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);

                plane = Plane.WorldXY;
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                origin = new Point3d(panelRight - 2 * panel.SheetThickness - panel.BottomFirstFoldWidth, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                offset = new Point3d(panelRight - panel.SheetThickness, panelY0 + 0.25 * (panel.BottomFirstFoldWidth), 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - 1.8 * (panel.BottomFirstFoldWidth), 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

                //Add panel thickness dimension
                origin = new Point3d(panelLeft + panel.TopFirstFoldWidth + panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                offset = new Point3d(panelLeft + panel.TopFirstFoldWidth + 2 * panel.SheetThickness, (panelY1 + panelY0) / 1.65, 0);
                pt = new Point3d((offset.Y - origin.Y) / 2, (panelY1 + panelY0) / 1.65, 0);

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimension.Text = " " + dimension.Text;
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);
            }
            Guid verticalFaceBurrLeader;

            if (panel.SideRequired.Equals("Burr Side") || panel.SideRequired.Equals("Both Sides")) //add the burr side labels only if the user wants to
            {

                if (panel.LeftFirstFoldDirection == 2 && panel.LeftFold == 1)
                {
                    text = "BURR SIDE";
                    // Add BURR SIDE label Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelLeft + panel.LeftFirstFoldWidth, (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth) - 25 * (offset.X - origin.X), (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth) - 25 * (offset.X - origin.X), (panelY1 + panelY0) / 2.2, 0)); //2.25
                                                                                                                                                      //leaderPoints.Add(new Point3d(origin.X - 10 * (offset.X - origin.X), (panelY1 + panelY0) / 2.25, 0));

                    // Add burrLeader
                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }
                if (panel.RightFirstFoldDirection == 2 && panel.RightFold == 1)
                {
                    text = "BURR SIDE";
                    // Add BURR SIDE label Right
                    leaderPoints.Clear();
                    burrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelRightHS + panel.SheetThickness, (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d(panelRightHS + 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d(panelRightHS + 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2, 0)); //2.25


                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }

                if (panel.LeftFirstFoldDirection == 1 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 1 && panel.BottomFold == 1) // up
                {
                    text = "BURR SIDE";
                    // Add BURR SIDE label Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelLeft + panel.LeftFirstFoldWidth, (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth) + 25 * (offset.X - origin.X), (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth) + 25 * (offset.X - origin.X), (panelY1 + panelY0) / 2.2, 0)); //2.25
                                                                                                                                                      //leaderPoints.Add(new Point3d(origin.X - 10 * (offset.X - origin.X), (panelY1 + panelY0) / 2.25, 0));

                    // Add burrLeader
                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }
                if (panel.RightFirstFoldDirection == 1 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 1 && panel.BottomFold == 1)
                {
                    text = "BURR SIDE";
                    // Add BURR SIDE label Right
                    leaderPoints.Clear();
                    burrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelRightHS - panel.SheetThickness, (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d(panelRightHS - 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2, 0)); //2.35
                    leaderPoints.Add(new Point3d(panelRightHS - 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2.25, 0)); //2.25


                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                }

            }

            if (panel.SideRequired.Equals("Face Side") || panel.SideRequired.Equals("Both Sides")) //add the burr side labels only if the user wants to
            {
                text = "FACE SIDE";
                if (panel.LeftFirstFoldDirection == 2 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 2 && panel.BottomFold == 1)
                {
                    //Add Face SIDE label (Verically) - Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelLeft + panel.LeftFirstFoldWidth + panel.SheetThickness, (panelY1 + panelY0) / 2.1, 0));
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth + panel.SheetThickness) + 15 * (origin.X - offset.X), (panelY1 + panelY0) / 2.1, 0));
                    leaderPoints.Add(new Point3d((panelLeft + panel.LeftFirstFoldWidth + panel.SheetThickness) + 15 * (origin.X - offset.X), (panelY1 + panelY0) / 2.2, 0));
                    verticalFaceBurrLeader = new Guid();
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);


                }
                if (panel.RightFirstFoldDirection == 2 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 2 && panel.BottomFold == 1)
                {
                    //Add Face side label - Right
                    leaderPoints.Clear();
                    verticalFaceBurrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelRightHS, (panelY1 + panelY0) / 2.1, 0));
                    leaderPoints.Add(new Point3d((panelLeftHS + panel.SheetThickness) + 2 * (panelLeftHS - panelRightHS), (panelY1 + panelY0) / 2.1, 0));
                    leaderPoints.Add(new Point3d((panelLeftHS + panel.SheetThickness) + 2 * (panelLeftHS - panelRightHS), (panelY1 + panelY0) / 2.2, 0));
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);
                }
                //For direction Up
                if (panel.LeftFirstFoldDirection == 1 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 1 && panel.BottomFold == 1)
                {
                    //Add Face SIDE label (Verically) - Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelLeft, (panelY1 + panelY0) / 2.3, 0));
                    leaderPoints.Add(new Point3d((panelLeft - 100) - (15 * (origin.X - offset.X)), (panelY1 + panelY0) / 2.3, 0));
                    leaderPoints.Add(new Point3d((panelLeft - 100) - (15 * (origin.X - offset.X)), (panelY1 + panelY0) / 2.4, 0));
                    verticalFaceBurrLeader = new Guid();
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);
                }

                if (panel.RightFirstFoldDirection == 1 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 1 && panel.BottomFold == 1)
                {
                    //Add Face side label - Right
                    leaderPoints.Clear();
                    verticalFaceBurrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelRightHS - panel.SheetThickness - panel.BottomFirstFoldWidth, (panelY1 + panelY0) / 2.2, 0));
                    leaderPoints.Add(new Point3d(panelRightHS + 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2.2, 0));
                    leaderPoints.Add(new Point3d(panelRightHS + 2 * (panelRightHS - panelLeftHS), (panelY1 + panelY0) / 2.3, 0));
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);
                }
            }



            //Adding the AA text
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelLeft + panel.SheetThickness + (80 * 2), panelY0, 0));  //place of the arrow head
            leaderPoints.Add(new Point3d(panelLeft + panel.SheetThickness + (80 * 2), panelY0, 0));  //the horizontal line draw to the arrow head
            leaderPoints.Add(new Point3d(panelLeft + panel.SheetThickness + (50 * 2), panelY0, 0)); //place of the text 
            text = leftText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            //Adding the BB text
            leaderPoints.Clear();
            panelRightHS = panelFirstRightX1 + 6 * (panelFirstRightX1 - panelFirstRightX0);
            leaderPoints.Add(new Point3d(panelRightHS - 50, panelY0, 0));  //place of the arrow head
            leaderPoints.Add(new Point3d(panelRightHS - 50, panelY0, 0));  //the horizontal line draw to the arrow head
            leaderPoints.Add(new Point3d(panelRightHS, panelY0, 0)); //place of the text 
            text = rightText;

            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();


            // Create a new layer called DIMENSIONS BLACK
            layerName = "DIMENSIONS BLACK";
            layerIndex = createSubLayers.createSubLayer(layerName,
                    System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //multiply kfactor by 2 to get the correct dimension
            panelFirstLeftX1 = (panelX0 + panel.KFactor * 2) - (panel.LeftFirstFoldWidth);
            panelFirstRightX1 = (panelX1 - panel.KFactor * 2) + (panel.RightFirstFoldWidth);
            // Add horizontal dimension including fold (from left fold to right fold)
            if (panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth > 0)
            {
                if (panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    origin = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                    offset = new Point3d(panelFirstRightX1, panelFirstBottomY1, 0);
                    pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 3 * (panelFirstBottomY0 - panelFirstBottomY1), 0);

                    plane = Plane.WorldXY;
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }

                //adds a black dimension to show the width of the panel (from right perimeter to left)
                if (panel.LeftFold == 1 || panel.RightFold == 1)
                {
                    origin = new Point3d((panelX0), panelFirstBottomY1 + 10, 0);
                    offset = new Point3d(panelX1, panelFirstBottomY1 + 10, 0);
                    pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 2 * (panelFirstBottomY0 - panelFirstBottomY1), 0);

                    plane = Plane.WorldXY;
                    plane.Origin = origin;

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);
                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);
                }
            }

            // Add top panel dimension
            //if (panel.TopFirstFoldWidth > 0)
            //{
            //   origin = new Point3d(panelFirstTopX0, panelFirstTopY1, 0);
            //   offset = new Point3d(panelFirstTopX1, panelFirstTopY1, 0);
            //   pt = new Point3d((offset.X - origin.X) / 2, panelFirstTopY1 + (panelFirstTopY1 - panelFirstTopY0), 0);

            //   plane = Plane.WorldXY;
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);

            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);
            //   //if (folds == 4)
            //   {
            //      //dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   }
            //  // if (folds == 2) //set the dimension to exact value
            //   {
            //      //dimension = new LinearDimension(plane, ext1, extDimensionsBlue, linePt);
            //   }
            //   //dimGuid = doc.Objects.AddLinearDimension(dimension);

            //   //guidList.Add(dimGuid); //horizontal black dimension added
            //}

            // Add bottom panel dimension
            //if (panel.BottomFirstFoldWidth > 0 && panel.BottomFold == 1)
            //{
            //   origin = new Point3d(panelFirstBottomX0, panelFirstBottomY1 - 50, 0);
            //   offset = new Point3d(panelFirstBottomX1, panelFirstBottomY1 - 50, 0);
            //   pt = new Point3d((offset.X - origin.X) / 2, (panelFirstBottomY1 - 50) - (panelFirstBottomY0 - panelFirstBottomY1), 0);

            //   plane = Plane.WorldXY;
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);

            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);

            //   dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   dimGuid = doc.Objects.AddLinearDimension(dimension);
            //   doc.Views.Redraw();
            //   guidList.Add(dimGuid);
            //   doc.Views.Redraw();
            //}

            // Add vertical dimension including fold (top fold to bottom fold)
            if (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth > 0 && panel.TopFold == 1 && panel.BottomFold == 1)
            {
                panelFirstTopY1 = ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor;
                panelFirstBottomY1 = ((panelY0 + panel.KFactor) - panel.TopFirstFoldWidth) + panel.KFactor;
                origin = new Point3d(panelFirstLeftX1, panelFirstTopY1, 0);
                offset = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                pt = new Point3d(panelFirstLeftX1 - 5 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

            }

            //Add left panel dimension (black panel dimension  the fold)
            if (panel.LeftFold == 1 && panel.LeftFirstFoldSetbackTop > 0 || panel.LeftFold == 1 && panel.LeftFirstFoldSetbackBottom > 0) //add only if left fold is present
            {
                origin = new Point3d(panelFirstLeftX1 - 3 * (panelFirstLeftX0 - panelFirstLeftX1), panelFirstLeftY0, 0);
                offset = new Point3d(panelFirstLeftX1 - 3 * (panelFirstLeftX0 - panelFirstLeftX1), panelFirstLeftY1, 0);
                pt = new Point3d(panelFirstLeftX1 - 4 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);
                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

            }
            //adds the left fold dimension (not the fold)
            {
                origin = new Point3d(panelFirstLeftX1 - (panelFirstLeftX0 - panelFirstLeftX1), panelY0, 0);
                offset = new Point3d(panelFirstLeftX1 - (panelFirstLeftX0 - panelFirstLeftX1), panelY1, 0);
                pt = new Point3d(panelFirstLeftX1 - 2 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

            }


            // Add right panel dimension
            if (panel.RightFold == 1) //Add only if fold is present
            {
                origin = new Point3d(panelFirstRightX1 + 2 * (panelFirstRightX1 - panelFirstRightX0), panelFirstRightY0, 0);
                offset = new Point3d(panelFirstRightX1 + 2 * (panelFirstRightX1 - panelFirstRightX0), panelFirstRightY1, 0);
                pt = new Point3d(panelFirstRightX1 + 4 * (panelFirstRightX1 - panelFirstRightX0), (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

            }
            //adds the black dimension (right)
            {
                origin = new Point3d(panelFirstRightX1 + (panelFirstRightX1 - panelFirstRightX0), panelY0, 0);
                offset = new Point3d(panelFirstRightX1 + (panelFirstRightX1 - panelFirstRightX0), panelY1, 0);
                pt = new Point3d(panelFirstRightX1 + 2 * (panelFirstRightX1 - panelFirstRightX0), (offset.Y - origin.Y) / 2, 0);

                plane = Plane.WorldXY;
                plane.XAxis = new Vector3d(0, -1, 0);
                plane.YAxis = new Vector3d(-1, 0, 0);
                plane.ZAxis = new Vector3d(0, 0, -1);
                plane.Origin = origin;

                plane.ClosestParameter(origin, out u, out v);
                ext1 = new Point2d(u, v);

                plane.ClosestParameter(offset, out u, out v);
                ext2 = new Point2d(u, v);

                plane.ClosestParameter(pt, out u, out v);
                linePt = new Point2d(u, v);

                dimension = new LinearDimension(plane, ext1, ext2, linePt);
                dimGuid = doc.Objects.AddLinearDimension(dimension);

                guidList.Add(dimGuid);

            }

            MetrixUtilities.createMetrixBordersDimension();

            // Draw Border dimension on BORDERS layer
            layerName = "BORDERS";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            // Add horizontal borders dimension
            origin = new Point3d(panelX1, (panelY0 + panelY1) / 2 + (panelY1 - panelY0) / 5, 0);
            offset = new Point3d(borderX1, (panelY0 + panelY1) / 2 + (panelY1 - panelY0) / 5, 0);
            pt = new Point3d((offset.X - origin.X) / 2, (panelY0 + panelY1) / 2, 0);

            plane = Plane.WorldXY;
            plane.Origin = origin;

            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);

            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);

            guidList.Add(dimGuid);

            // Add horizontal borders dimension
            origin = new Point3d(panelX0, (panelY0 + panelY1) / 2 + (panelY1 - panelY0) / 5, 0);
            offset = new Point3d(borderX0, (panelY0 + panelY1) / 2 + (panelY1 - panelY0) / 5, 0);
            pt = new Point3d((offset.X - origin.X) / 2, (panelY0 + panelY1) / 2, 0);


            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);

            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);

            guidList.Add(dimGuid);

            // Add vertical border dimension for panel

            origin = new Point3d((panelX0 + panelX1) / 2 + (panelX1 - panelX0) / 5, panelY0, 0);
            offset = new Point3d((panelX0 + panelX1) / 2 + (panelX1 - panelX0) / 5, borderY0, 0);
            pt = new Point3d((panelX0 + panelX1) / 2, (offset.Y - origin.Y) / 2, 0);

            plane = Plane.WorldXY;
            plane.XAxis = new Vector3d(0, -1, 0);
            plane.YAxis = new Vector3d(-1, 0, 0);
            plane.ZAxis = new Vector3d(0, 0, -1);
            plane.Origin = origin;

            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);

            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);

            guidList.Add(dimGuid);

            origin = new Point3d((panelX0 + panelX1) / 2 + (panelX1 - panelX0) / 5, panelY1, 0);
            offset = new Point3d((panelX0 + panelX1) / 2 + (panelX1 - panelX0) / 5, borderY1, 0);
            pt = new Point3d((panelX0 + panelX1) / 2, (offset.Y - origin.Y) / 2, 0);

            plane.ClosestParameter(origin, out u, out v);
            ext1 = new Point2d(u, v);

            plane.ClosestParameter(offset, out u, out v);
            ext2 = new Point2d(u, v);

            plane.ClosestParameter(pt, out u, out v);
            linePt = new Point2d(u, v);

            dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimGuid = doc.Objects.AddLinearDimension(dimension);

            guidList.Add(dimGuid);

            DimensionStyle dimStyle = MetrixUtilities.createMetrixRealDimension(); //sets the metrix real dimension as the default.

            //checks whether the perforation layers exists, if not create layer and make Approval layer the parent layer
            //If exists, make Approval layer the parent layer
            layerName = "PERFORATION";
            layerIndex = createSubLayers.createSubLayer(layerName,
               System.Drawing.Color.Green, parent_layer_Approval);

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            layerName = "VIEWPORT";
            layerIndex = createSubLayers.createSubLayer(layerName,
                               System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //doc.Views.Redraw();

            RhinoObject panelPerimeterObj = doc.Objects.Find(panel.Perimeter);

            // Select all objects on Perforation Layer
            Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);

            double tolerance = Properties.Settings.Default.Tolerance;
            Rhino.Geometry.Curve panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

            // If  perforation layer is missing 
            if (rhinoObjs == null)
            {
                // Select all objects on tool hit Layer
                rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);
            }

            if (Convert.ToBoolean(panel.DrawPerf) == true && rhinoObjs != null)
            {
                foreach (RhinoObject rhinoObj in rhinoObjs)
                {
                    Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

                    if (testCurve != null)
                    {
                        if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                        {
                            guidList.Add(rhinoObj.Id);
                        }
                    }
                }
            }

            // Export the panel

            doc.Objects.UnselectAll();

            doc.Objects.Select(panel.Perimeter);

            // Get all of the objects on the layer. If layername is bogus, you will
            // just get an empty list back

            Rhino.DocObjects.RhinoObject label = doc.Objects.Find(panel.Label);
            string exportFileName = "1";

            if (label != null)
            {
                label.Select(true);
                Rhino.Geometry.TextEntity textentity = label.Geometry as Rhino.Geometry.TextEntity;
                exportFileName = textentity.Text + ".dxf";
            }

            // Select all objects on DOT SCRIBE LABEL Layer
            rhinoObjs = doc.Objects.FindByLayer("DOT SCRIBE LABEL");

            tolerance = Properties.Settings.Default.Tolerance;
            panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

            if (rhinoObjs != null)
            {
                foreach (RhinoObject rhinoObj in rhinoObjs)
                {
                    Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

                    if (testCurve != null)
                    {
                        if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                        {
                            rhinoObj.Select(true);
                        }
                    }
                }
            }

            // Select all objects on Tool Hit
            rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);

            if (rhinoObjs == null)
            {
                rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);
            }

            if (rhinoObjs != null)
            {
                foreach (RhinoObject rhinoObj in rhinoObjs)
                {
                    Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

                    if (testCurve != null)
                    {
                        if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
                        {
                            rhinoObj.Select(true);
                        }
                    }
                }
            }
            /**
             * Checks if the dxf files are required by the user, if yes check whether the panel is perforated
             * using the drawPerf property in the panel. If it is a perforated panel then check if the directory
             * for perforated panels dxf files already exists, if does not exist create directory and run command.
             * If panel is not perforated, create directory to save not perforated panels dxf files if the directory
             * does not exist. Then run the dxf file create command. 
             * */
            if (panel.DXFFilesRequired.Equals("Yes"))
            {
                String path;
                String immediateFolderName = Path.GetFileName(Path.GetDirectoryName(doc.Path)); //get the immediate foldername which the file is located in 
                                                                                                //split the path to get the parent folder. 
                String[] newPath = MetrixUtilities.splitString(Path.GetDirectoryName(doc.Path), immediateFolderName);
                if (panel.DrawPerf == 1) //checks if panel is perforated 
                {
                    path = newPath[0] + ("5TRUMPF") + ("\\WITH PERF"); //merge path for perforated files
                    if (!Directory.Exists(path)) //check if directory already exists
                    {
                        System.IO.Directory.CreateDirectory(path); //create directory if not exist
                    }
                }
                else
                {
                    path = newPath[0] + ("5TRUMPF") + ("\\NO PERF"); //merge path for not perforated files
                    if (!Directory.Exists(path)) //check if directory already exists 
                    {
                        System.IO.Directory.CreateDirectory(path); //create directory if not exist
                    }
                }
                string command = string.Format("-_Export \"" + path + @"\" + exportFileName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
                // Export the selected curves
                RhinoApp.RunScript(command, true);
            }
            // Unselect all objects
            doc.Objects.UnselectAll();

            // Default layer index
            int defaultLayerIndex = doc.Layers.Find("Default", true);

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            ////if draw perf is false, turnoff the toolhit layer
            //if (Convert.ToBoolean(panel.DrawPerf) != true)
            //{
            //   layerName = "Tool Hit";
            //   layerIndex = doc.Layers.Find(layerName, true);
            //   doc.Layers[layerIndex].IsVisible = false;
            //}



            MetrixUtilities.joinCurves(doc.Layers.Find("PANEL PERIMETER", true));
            //guidList = CustomFixingHoles.drawFixingFoles(panel, panel, true, panelBottom, panelLeft, panelY0, panelY1, dimStyle, guidList, panelX0, panelX1, panelRight, panelTop, panelBottomBH, panelRightHS); //add fixing holes

            layerName = "VIEWPORT";
            layerIndex = createSubLayers.createSubLayer(layerName,
                               System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            foreach (Guid g in guidList)
            {
                int idx = RhinoDoc.ActiveDoc.Groups.Find(panel.PartName, false);

                if (idx < 0)
                {
                    idx = RhinoDoc.ActiveDoc.Groups.Add(panel.PartName);
                }

                RhinoDoc.ActiveDoc.Groups.AddToGroup(idx, g);
            }
        }


        /**
         * Below method adds the fold text based on to the panel based on the passed parameters. 
         * The method adds the fold text to the guid and returns the guid back to the main method 
         * */
        public static List<Guid> addFoldsText(int direction, Plane plane, double height, string font, List<Guid> guidList)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            if (direction == 1) //if the direction is 1 it is a fold up so add the fold up text to the panel
            {
                Guid panelFoldText = doc.Objects.AddText("FOLD UP", plane, height, font, false, false);
                guidList.Add(panelFoldText);
                return guidList;
            }
            else //if the direction is 0 it is a fold down so add the fold down text to the panel
            {
                Guid panelFoldText = doc.Objects.AddText("FOLD DOWN", plane, height, font, false, false);
                guidList.Add(panelFoldText);
                return guidList;
            }
        }

        //Method will recalculate the distances for the top and bottom folds 
        public static FoldedPerforationPanel reCalculateDistances(FoldedPerforationPanel panel)
        {
            //recalculate top fixing hole quantity and distance
            panel.TopFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight - panel.KFactor
                                           - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / panel.DistanceProvidedTop) + 1);
            panel.TopFixingHoleDistance = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight - panel.KFactor
                                           - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / (panel.TopFixingHoleQuantity)));

            //recalculate bottom fixing hole quantity and distance
            panel.BottomFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight - panel.KFactor
                                           - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / panel.DistanceProvidedBottom) + 1);

            panel.BottomFixingHoleDistance = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight - panel.KFactor
                                           - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / (panel.BottomFixingHoleQuantity)));

            //if the top fixing hole distance is more than the maximum provided distance recalculate 
            while (checkDistanceWIthTolerance(panel.TopFixingHoleDistance, panel.DistanceProvidedTop))
            {
                panel.TopFixingHoleQuantity = panel.TopFixingHoleQuantity + 1;
                panel.TopFixingHoleDistance = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight - panel.KFactor
                                            - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / (panel.TopFixingHoleQuantity)));
            }
            //if the bottom fixing hole distance is more than the maximum provided distance recalculate 
            while (checkDistanceWIthTolerance(panel.BottomFixingHoleDistance, panel.DistanceProvidedBottom))
            {
                panel.BottomFixingHoleQuantity = panel.BottomFixingHoleQuantity + 1;
                panel.BottomFixingHoleDistance = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight - panel.KFactor
                                           - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / panel.BottomFixingHoleQuantity));
            }

            return panel;
        }
        //method used to check whether the calculated distance between fixing holes is greater than the provided maximum distance
        private static Boolean checkDistanceWIthTolerance(double calculatedDistance, double providedDistance)
        {
            if (calculatedDistance > providedDistance)  //if the yes return true
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}