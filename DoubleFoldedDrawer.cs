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
    public class DoubleFoldedDrawer
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
        public static void drawPanel(double xLowerBound, double xUpperBound, double yLowerBound, double yUpperBound, FoldedPerforationPanel panel, bool fixingHolesManipulated)
        {
            string leftText = "AA";
            string rightText = "BB";
            string topText = "CC";
            string bottomText = "DD";

            string doubleFoldTypeFlat = "Fold Flat";
            string doubleFoldTypeNotch = "Notch Fold";
            string doubleFoldTypeMiter = "Miter Join Fold";

            Point3d pointTopLeft = new Point3d(0, 0, 0);
            Point3d pointTopRight = new Point3d(0, 0, 0);
            Point3d pointBottomLeft = new Point3d(0, 0, 0);
            Point3d pointBottomRight = new Point3d(0, 0, 0);
            Point3d pointLeftTop = new Point3d(0, 0, 0);
            Point3d pointLeftBottom = new Point3d(0, 0, 0);
            Point3d pointRightTop = new Point3d(0, 0, 0);
            Point3d pointRightBottom = new Point3d(0, 0, 0);

            double topScndFoldSetbackLeftConnect = 0;
            double topScndFoldSetbackRightConnect = 0;
            double bottomScndFoldSetbackLeftConnect = 0;
            double bottomScndFoldSetbackRightConnect = 0;
            double leftScndFoldSetbackTopConnect = 0;
            double leftScndFoldSetbackBottomConnect = 0;
            double rightScndFoldSetbackTopConnect = 0;
            double rightScndFoldSetbackBottomConnect = 0;

            RhinoDoc doc = RhinoDoc.ActiveDoc;
            string text = "";
            double height = panel.labelHeight / 3;
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
            double rightFixingHolePosition = 0;
            double positionLeftBurr = 0; //Will hold the left handside location
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
                if (panel.TopFold == 1 && panel.TopFixingHoles == "1")
                {
                    panel.TopFixingHoles = "1";
                }
                else
                {
                    panel.TopFixingHoles = "0";
                }
                if (panel.BottomFold == 1 && panel.BottomFixingHoles == "1")
                {
                    panel.BottomFixingHoles = "1";
                }
                else
                {
                    panel.BottomFixingHoles = "0";
                }
                if (panel.LeftFold == 1 && panel.LeftFixingHoles == "1")
                {
                    panel.LeftFixingHoles = "1";
                }
                else
                {
                    panel.LeftFixingHoles = "0";
                }
                if (panel.RightFold == 1 && panel.RightFixingHoles == "1")
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
            if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold != 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.KFactor, 0), new Point3d(panelX1 -panel.KFactor, panelY1 - panel.KFactor, 0));
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

            //For Top only
            if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold != 1 && panel.LeftFold != 1)
            {
                //Draw top horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
            }

            //For Bottom Only
            if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold != 1 && panel.LeftFold != 1)
            {
                //Draw Bottom horizontal (left - right)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
            }

            //For Right Only
            if (panel.TopFold != 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold != 1)
            {
                //Draw Right Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
            }

            //For Left Only
            if (panel.TopFold != 1 && panel.BottomFold != 1 && panel.RightFold != 1 && panel.LeftFold == 1)
            {
                //Draw Left Vertical (bottom to top)
                panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
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
            if (panel.DrawPerf != 3) //If panel is not a sold panel
            {
                panel.Border = doc.Objects.AddPolyline(rectangle_corners);
                guidList.Add(panel.Border);
            }


            // Create a new layer called Perimeter 
            layerName = "PANEL PERIMETER";
            MetrixUtilities.createMetrixRealDimension(); //creates the metrix real dimension and set as the default 

            layerIndex = createSubLayers.createSubLayer(layerName,
              System.Drawing.Color.Black, parent_layer_Nesting); //make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //Below code will check fold Types in the corners for double folds and set the setbacks accordingly 
            if (panel.DoubleFoldsRequired == "1")
            {
                //Check Top Right
                if (panel.TopFoldRightType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of right top to the fold width +1 as per requirement reduced by the kfactor
                    panel.RightSecondFoldSetbackTop = (panel.TopSecondFoldWidth + 1) - panel.KFactor;
                }

                //Check Top Left
                if (panel.TopFoldLeftType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of left top to the fold width +1 as per requirement reduced by the kfactor
                    panel.LeftSecondFoldSetbackTop = (panel.TopSecondFoldWidth + 1) - panel.KFactor;
                }

                //Check Bottom Right
                if (panel.BottomFoldRightType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of right bottom to the fold width +1 as per requirement reduced by the kfactor
                    panel.RightSecondFoldSetbackBottom = (panel.BottomSecondFoldWidth + 1) - panel.KFactor;
                }

                //Check Bottom Left
                if (panel.BottomFoldLeftType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of left bottom to the fold width +1 as per requirement reduced by the kfactor
                    panel.LeftSecondFoldSetbackBottom = (panel.BottomSecondFoldWidth + 1) - panel.KFactor;
                }
                //Check Left Top
                if (panel.LeftFoldTopType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of top left to the fold width +1 as per requirement reduced by the kfactor
                    panel.TopSecondFoldSetbackLeft = (panel.LeftSecondFoldWidth + 1) - panel.KFactor;
                }
                //Check Left Bottom
                if (panel.LeftFoldBottomType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of Bottom left to the fold width +1 as per requirement reduced by the kfactor
                    panel.BottomSecondFoldSetbackLeft = (panel.LeftSecondFoldWidth + 1) - panel.KFactor;
                }

                //Check Right Top
                if (panel.RightFoldTopType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of top Right to the fold width +1 as per requirement reduced by the kfactor
                    panel.TopSecondFoldSetbackRight = (panel.RightSecondFoldWidth + 1) - panel.KFactor;
                }
                //Check Right Bottom
                if (panel.RightFoldBottomType.Equals(doubleFoldTypeNotch))
                {
                    //Set the setback of Bottom Right to the fold width +1 as per requirement reduced by the kfactor
                    panel.BottomSecondFoldSetbackRight = (panel.RightSecondFoldWidth + 1) - panel.KFactor;
                }

                //Check for miter joints 
                //Checking top right joint or Right top joint 
                if (panel.TopFoldRightType.Equals(doubleFoldTypeMiter) || panel.RightFoldTopType.Equals(doubleFoldTypeMiter))
                {
                    panel.TopSecondFoldSetbackRight = 45;
                    panel.RightSecondFoldSetbackTop = 45;
                    panel.TopFoldRightType = doubleFoldTypeMiter; //set as miter joint
                    panel.RightFoldTopType = doubleFoldTypeMiter;
                }
                //Checking top left joint or Left top joint
                if (panel.TopFoldLeftType.Equals(doubleFoldTypeMiter) || panel.LeftFoldTopType.Equals(doubleFoldTypeMiter))
                {
                    panel.TopSecondFoldSetbackLeft = 45;
                    panel.LeftSecondFoldSetbackTop = 45;

                    panel.TopFoldLeftType = doubleFoldTypeMiter;
                    panel.LeftFoldTopType = doubleFoldTypeMiter;
                }
                //Checking Bottom Right joint or Right bottom joint
                if (panel.BottomFoldRightType.Equals(doubleFoldTypeMiter) || panel.RightFoldBottomType.Equals(doubleFoldTypeMiter))
                {
                    panel.BottomSecondFoldSetbackRight = 45;
                    panel.RightSecondFoldSetbackBottom = 45;

                    panel.BottomFoldRightType = doubleFoldTypeMiter;
                    panel.RightFoldBottomType = doubleFoldTypeMiter;
                }

                //Checking Bottom left joint or Left bottom joint 
                if (panel.BottomFoldLeftType.Equals(doubleFoldTypeMiter) || panel.LeftFoldBottomType.Equals(doubleFoldTypeMiter))
                {
                    panel.BottomSecondFoldSetbackLeft = 45;
                    panel.LeftSecondFoldSetbackBottom = 45;

                    panel.BottomFoldLeftType = doubleFoldTypeMiter;
                    panel.LeftFoldBottomType = doubleFoldTypeMiter;
                }
            }

            //Below code will set the setbacks to 0 based on the adjacent folds
            //Checking Top
            if (panel.RightFold == 1) //check if the top right has an adjacent fold
            {
                if (panel.RightFirstFoldSetbackTop > 3 || panel.RightFirstFoldSetbackTop == 0) //check if the adjacent fold top set back is more than 3 mil
                {
                    panel.TopFirstFoldSetbackRight = 0;
                    //panel.TopSecondFoldSetbackRight = 0;
                    cornerRFTR = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackRight = 0;  //if there is no adjacent fold set the setback to 0
                                                     // panel.TopSecondFoldSetbackRight = 0;                                    // panel.RightFirstFoldSetbackTop = 0;
                cornerRFTR = false;
            }

            if (panel.LeftFold == 1) //check if the top left has an adjacent fold
            {
                if (panel.LeftFirstFoldSetbackTop > 3 || panel.LeftFirstFoldSetbackTop == 0) //check if the adjacent fold top set back is more than 3 mi               
                {
                    panel.TopFirstFoldSetbackLeft = 0;
                    //panel.TopSecondFoldSetbackLeft = 0;
                    cornerRFTL = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackLeft = 0;
                //panel.TopSecondFoldSetbackLeft = 0;
                // panel.LeftFirstFoldSetbackTop = 0;
                cornerRFTL = false;
            }


            //Checking Bottom
            if (panel.RightFold == 1)  //check if the bottom has a right adjacent fold
            {
                if (panel.RightFirstFoldSetbackBottom > 3 || panel.RightFirstFoldSetbackBottom == 0) //check if right setback is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackRight = 0; //set the bottom set back right to 0
                                                           //  panel.BottomSecondFoldSetbackRight = 0; //set the bottom set back right to 0
                    cornerRFBR = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackRight = 0; //set the bottom set back right to 0
                                                       // panel.RightFirstFoldSetbackBottom = 0;
                                                       // panel.BottomSecondFoldSetbackRight = 0;
                cornerRFBR = false;
            }

            if (panel.LeftFold == 1)  //check if the bottom has a left adjacent fold
            {
                if (panel.LeftFirstFoldSetbackBottom > 3 || panel.LeftFirstFoldSetbackBottom == 0) //check if left setback is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackLeft = 0; //set the bottom set back left to 0
                                                          // panel.BottomSecondFoldSetbackLeft = 0;
                    cornerRFBL = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackLeft = 0; //set the bottom set back left to 0
                                                      // panel.LeftFirstFoldSetbackBottom = 0;
                                                      // panel.BottomSecondFoldSetbackLeft = 0;
                cornerRFBL = false;
            }

            //checking Left 
            if (panel.TopFold == 1) //check if the left fold has a top adjacent fold
            {
                if (panel.TopFirstFoldSetbackLeft > 3 || panel.TopFirstFoldSetbackLeft == 0) //check if the top set back is more than 3 mil
                {
                    panel.TopFirstFoldSetbackLeft = 0; //if yes set the set back to 0
                                                       // panel.TopSecondFoldSetbackLeft = 0;
                    cornerRFTL = false;
                }
            }
            else
            {
                panel.LeftFirstFoldSetbackTop = 0;
                panel.TopFirstFoldSetbackLeft = 0;
                //  panel.TopSecondFoldSetbackLeft = 0;
                cornerRFTL = false;
            }

            if (panel.BottomFold == 1) //check if the left fold has a bottom adjacent fold
            {
                if (panel.BottomFirstFoldSetbackLeft > 3 || panel.BottomFirstFoldSetbackLeft == 0) //check if the bottom set back is more than 3 mil
                {
                    panel.BottomFirstFoldSetbackLeft = 0; //if yes set the set back to 0
                                                          // panel.BottomSecondFoldSetbackLeft = 0;
                    cornerRFBL = false;
                }
            }
            else
            {
                panel.LeftFirstFoldSetbackBottom = 0;
                panel.BottomFirstFoldSetbackLeft = 0;
                // panel.BottomSecondFoldSetbackLeft = 0;
                cornerRFBL = false;
            }

            //Checking Right 
            //right top
            if (panel.TopFold == 1)     //check if  there is top a fold
            {
                if (panel.TopFirstFoldSetbackRight > 3 || panel.TopFirstFoldSetbackRight == 0) //check if the top set back is more than 3 
                {
                    panel.TopFirstFoldSetbackRight = 0; //if yes set the setback to 0
                                                        // panel.TopSecondFoldSetbackRight = 0;
                    cornerRFTR = false;
                }
            }
            else
            {
                panel.TopFirstFoldSetbackRight = 0;
                // panel.TopSecondFoldSetbackRight = 0;
                panel.RightFirstFoldSetbackTop = 0;
                cornerRFTR = false;
            }
            //right bottom
            if (panel.BottomFold == 1) //check if there is a bottom adjacent fold
            {
                if (panel.BottomFirstFoldSetbackRight > 3 || panel.BottomFirstFoldSetbackRight == 0) //check if the bottom setback is more than 3
                {
                    panel.BottomFirstFoldSetbackRight = 0; //if yes set the setback to 0
                                                           //  panel.BottomSecondFoldSetbackRight = 0;
                    cornerRFBR = false;
                }
            }
            else
            {
                panel.BottomFirstFoldSetbackRight = 0;
                //  panel.BottomSecondFoldSetbackRight = 0;
                panel.RightFirstFoldSetbackBottom = 0;
                cornerRFBR = false;
            }

            if (panel.DoubleFoldsRequired == "1") //Calculate only if double folds are required
            {
                pointTopLeft = new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0);
                pointTopRight = new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0);

                pointBottomLeft = new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0);
                pointBottomRight = new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0);

                pointLeftTop = new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0);
                pointLeftBottom = new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0);

                pointRightTop = new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0);
                pointRightBottom = new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0);
            }
            string foldText;
            var parentlayerIndex = doc.Layers.Find("LAYERS FOR APPROVAL DRAWINGS", true); //get parent layer index
                                                                                          // Draw the sample block
            int foldLabelLayer = createSubLayers.createSubLayer("FOLD LABELS",
                System.Drawing.Color.Black, doc.Layers[parentlayerIndex]);
            // Top First Fold (use setbacks to position the folds within the corner relief)
            if (panel.TopFold == 1)
            {
                //Adds the first fold
                if (panel.PanelType.Equals("Single Folded"))
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0));  // horizontal line
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0)); //left
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                    guidList.Add(panel.Perimeter);


                    if(panel.TopFirstFoldDirection == 1)
                    {
                        foldText = "UP 90°";
                    }
                    else
                    {
                        foldText = "DOWN 90°";
                    }

                    RhinoApp.RunScript("_SelNone", true);
                    height = (-2 * panel.KFactor + panel.TopFirstFoldWidth) / 2;
                    Point3d drawPoint = new Point3d((panelX0 + panelX1) / 2 - 2 * panel.TopFirstFoldWidth, (-2 * panel.KFactor + panel.TopFirstFoldWidth) / 4 + panelY1 + height, 0);
                    plane.Origin = drawPoint;
                    Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                    RhinoObject textObj = doc.Objects.Find(id);
                    textObj.Select(true);
                    BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                    RhinoApp.RunScript("_Move " + bbxt.Center + " " + (panelX0 + panelX1) / 2 + "," + bbxt.Center.Y + " ", true);

                    textObj.Attributes.LayerIndex = foldLabelLayer;
                    textObj.CommitChanges();
                    textObj.Select(false);
                }

                //Adds the second fold
                if (panel.PanelType.Equals("Double Folded"))
                {
                    //Single fold Vertical lines
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelY1 - panel.TopFirstFoldSetbackLeft, 0)); //left
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelY1 - panel.TopFirstFoldSetbackRight, 0)); //right (draw top to bottom)
                    guidList.Add(panel.Perimeter);
                    //Equation contains the calculation for single fold because double fold should be drawn off the single fold
                    double singleFold = ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor;

                    //Draw the horizontal top line
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopSecondFoldSetbackLeft, ((singleFold - panel.KFactor) + panel.TopSecondFoldWidth), 0), new Point3d(panelX1 - panel.TopSecondFoldSetbackRight, (singleFold - panel.KFactor) + panel.TopSecondFoldWidth, 0));  // horizontal line
                    guidList.Add(panel.Perimeter);
                    //Execute only if top right and top left both are notch folds
                    //If Top right and top left are both fold flats
                    //IF top right is a notch fold or  top right is flat fold
                    //If top left is a flat fold or top left is notch fold
                    if (panel.TopFoldRightType.Equals(doubleFoldTypeNotch) && panel.TopFoldLeftType.Equals(doubleFoldTypeNotch) ||
                       panel.TopFoldRightType.Equals(doubleFoldTypeFlat) && panel.TopFoldLeftType.Equals(doubleFoldTypeFlat) ||
                       panel.TopFoldRightType.Equals(doubleFoldTypeNotch) || panel.TopFoldRightType.Equals(doubleFoldTypeFlat) ||
                       panel.TopFoldLeftType.Equals(doubleFoldTypeFlat) || panel.TopFoldLeftType.Equals(doubleFoldTypeNotch))
                    {
                        //panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopSecondFoldSetbackLeft, ((singleFold - panel.KFactor) + panel.TopSecondFoldWidth), 0), new Point3d(panelX1 - panel.TopSecondFoldSetbackRight, (singleFold - panel.KFactor) + panel.TopSecondFoldWidth, 0));  // horizontal line
                        //guidList.Add(panel.Perimeter);
                        //Draw if left type is flat or notch
                        if (panel.TopFoldLeftType.Equals(doubleFoldTypeNotch) || panel.TopFoldLeftType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopSecondFoldSetbackLeft, (singleFold + panel.TopSecondFoldWidth - panel.KFactor), 0), new Point3d(panelX0 + panel.TopSecondFoldSetbackLeft, singleFold, 0)); //left
                            guidList.Add(panel.Perimeter);
                        }
                        //Draw if right type is flat or notch
                        if (panel.TopFoldRightType.Equals(doubleFoldTypeNotch) || panel.TopFoldRightType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopSecondFoldSetbackRight, (singleFold + panel.TopSecondFoldWidth - panel.KFactor), 0), new Point3d(panelX1 - panel.TopSecondFoldSetbackRight, singleFold, 0)); //right (draw top to bottom)
                            guidList.Add(panel.Perimeter);
                        }
                    }

                    //Execute only if top right and top left both are miter joints
                    //If top right is a miter joint or top left is a miter joint
                    if (panel.TopFoldRightType.Equals(doubleFoldTypeMiter) && panel.TopFoldLeftType.Equals(doubleFoldTypeMiter) ||
                       panel.TopFoldRightType.Equals(doubleFoldTypeMiter) || panel.TopFoldLeftType.Equals(doubleFoldTypeMiter))
                    {
                        if (panel.TopFoldLeftType.Equals(doubleFoldTypeMiter)) //if top left is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopSecondFoldSetbackLeft, (singleFold + panel.TopSecondFoldWidth - panel.KFactor), 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, singleFold, 0)); //left
                            guidList.Add(panel.Perimeter);
                        }
                        if (panel.TopFoldRightType.Equals(doubleFoldTypeMiter)) //if top right is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopSecondFoldSetbackRight, (singleFold + panel.TopSecondFoldWidth - panel.KFactor), 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackLeft, singleFold, 0)); //right (draw top to bottom)
                            guidList.Add(panel.Perimeter);
                        }

                    }

                }
            }

            // Bottom First Fold
            if (panel.BottomFold == 1)
            {
                //Adds the first fold
                if (panel.PanelType.Equals("Single Folded"))
                {
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0)); //horizontal line
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY0 + panel.BottomFirstFoldSetbackLeft, 0));  //Left (draws from bottom to top)
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY0 + panel.BottomFirstFoldSetbackRight, 0)); //Right (draws from bottom to top)
                    guidList.Add(panel.Perimeter);

                    if (panel.BottomFirstFoldDirection == 1)
                    {
                        foldText = "UP 90°";
                    }
                    else
                    {
                        foldText = "DOWN 90°";
                    }
                    RhinoApp.RunScript("_SelNone", true);
                    height = (-2 * panel.KFactor + panel.BottomFirstFoldWidth) / 2;
                    Point3d drawPoint = new Point3d((panelX0 + panelX1) / 2 - 2 * panel.BottomFirstFoldWidth, panelY0 + 3 * (2 * panel.KFactor - panel.BottomFirstFoldWidth) / 4 + height, 0);
                    plane.Origin = drawPoint;
                    Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                    RhinoObject textObj = doc.Objects.Find(id);
                    textObj.Select(true);
                    BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                    RhinoApp.RunScript("_Move " + bbxt.Center + " " + (panelX0 + panelX1) / 2 + "," + bbxt.Center.Y + " ", true);

                    textObj.Attributes.LayerIndex = foldLabelLayer;
                    textObj.CommitChanges();
                    textObj.Select(false);
                }

                //Adds the second fold
                if (panel.PanelType.Equals("Double Folded"))
                {
                    //Single fold Vertical lines
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelY0 + panel.BottomFirstFoldSetbackLeft, 0));  //Left (draws from bottom to top)
                    guidList.Add(panel.Perimeter);
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelY0 + panel.BottomFirstFoldSetbackRight, 0)); //Right (draws from bottom to top)
                    guidList.Add(panel.Perimeter);
                    //Equation contains the calculation for single fold because double fold should be drawn off the single fold
                    double singleFold = ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor;

                    //Draws the horizontal line
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomSecondFoldSetbackLeft, ((singleFold + panel.KFactor) - panel.BottomSecondFoldWidth), 0), new Point3d(panelX1 - panel.BottomSecondFoldSetbackRight, (singleFold + panel.KFactor) - panel.BottomSecondFoldWidth, 0));  // horizontal line
                    guidList.Add(panel.Perimeter);

                    //Execute this if Bottom left and bottom right are both notch
                    //If bottom left and bottom right are both fold flats
                    //If bottom left is a notch or bottom left is a flat fold 
                    //If bottom right is a notch or bottom right is a flat fold
                    if (panel.BottomFoldRightType.Equals(doubleFoldTypeNotch) && panel.BottomFoldLeftType.Equals(doubleFoldTypeNotch) ||
                      panel.BottomFoldRightType.Equals(doubleFoldTypeFlat) && panel.BottomFoldLeftType.Equals(doubleFoldTypeFlat) ||
                       panel.BottomFoldRightType.Equals(doubleFoldTypeNotch) || panel.BottomFoldRightType.Equals(doubleFoldTypeFlat) ||
                       panel.BottomFoldLeftType.Equals(doubleFoldTypeFlat) || panel.BottomFoldLeftType.Equals(doubleFoldTypeNotch))
                    {
                        //Draw if left type is flat or notch
                        if (panel.BottomFoldLeftType.Equals(doubleFoldTypeNotch) || panel.BottomFoldLeftType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomSecondFoldSetbackLeft, (singleFold + panel.KFactor) - panel.BottomSecondFoldWidth, 0), new Point3d(panelX0 + panel.BottomSecondFoldSetbackLeft, singleFold, 0)); //left
                            guidList.Add(panel.Perimeter);
                        }
                        //Draw if right  type is flat or notch
                        if (panel.BottomFoldRightType.Equals(doubleFoldTypeNotch) || panel.BottomFoldRightType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomSecondFoldSetbackRight, (singleFold + panel.KFactor) - panel.BottomSecondFoldWidth, 0), new Point3d(panelX1 - panel.BottomSecondFoldSetbackRight, singleFold, 0)); //right (draw top to bottom)
                            guidList.Add(panel.Perimeter);
                        }
                    }

                    //Execute only if Bottom right and Bottom left both are miter joints
                    //If Bottom right is a miter joint or Bottom left is a miter joint
                    if (panel.BottomFoldRightType.Equals(doubleFoldTypeMiter) && panel.BottomFoldLeftType.Equals(doubleFoldTypeMiter) ||
                       panel.BottomFoldRightType.Equals(doubleFoldTypeMiter) || panel.BottomFoldLeftType.Equals(doubleFoldTypeMiter))
                    {
                        if (panel.BottomFoldLeftType.Equals(doubleFoldTypeMiter)) //if Bottom left is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomSecondFoldSetbackLeft, (singleFold + panel.KFactor) - panel.BottomSecondFoldWidth, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, singleFold, 0)); //left
                            guidList.Add(panel.Perimeter);
                        }
                        if (panel.BottomFoldRightType.Equals(doubleFoldTypeMiter)) //if Bottom right is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomSecondFoldSetbackRight, (singleFold + panel.KFactor) - panel.BottomSecondFoldWidth, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, singleFold, 0)); //right (draw top to bottom)
                            guidList.Add(panel.Perimeter);
                        }
                    }
                }
            }
            RhinoApp.RunScript("_SelNone", true);
            // Left First Fold
            if (panel.LeftFold == 1)
            {
                //If only single fold
                if (panel.PanelType.Equals("Single Folded"))
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
                            panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0, 0)); // draws the vertical bottom line to connect left hand side bottom to bottom left hand side (drawn from top to bottom)
                            guidList.Add(panel.Perimeter);
                        }
                        else //if the bottom fold setback (LH) is not 0, draw the setback
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                            guidList.Add(panel.Perimeter);
                        }

                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            foldText = "UP 90°";
                        }
                        else
                        {
                            foldText = "DOWN 90°";
                        }
                        RhinoApp.RunScript("_SelNone", true);
                        height = panel.LeftFirstFoldWidth / 2 - panel.KFactor;
                        Point3d drawPoint = new Point3d(panelX0 - panel.LeftFirstFoldWidth + 2 * panel.KFactor, (panelY0 + panelY1) / 2 - 1.75 * panel.LeftFirstFoldWidth + height, 0);
                        plane.Origin = drawPoint;
                        Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                        RhinoObject textObj = doc.Objects.Find(id);
                        textObj.Select(true);
                        //             RhinoApp.RunScript("_-rotate " + (panelX0 - panel.LeftFirstFoldWidth / 2 - panel.KFactor) + "," + ((panelY0 + panelY1) / 2) + " " + "90", true);
                        textObj.Geometry.Transform(Rhino.Geometry.Transform.Rotation(Math.PI / 2, drawPoint));
                        textObj.CommitChanges();
                        BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                        RhinoApp.RunScript("_Move " + bbxt.Center + " " + ((panelX0 + 2*panel.KFactor - panel.LeftFirstFoldWidth + panelX0) / 2) + "," + bbxt.Center.Y + " ", true);
                        textObj.Attributes.LayerIndex = foldLabelLayer;                 
                        textObj.CommitChanges();
                        textObj.Select(false);
                    }


                    if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold != 1 & panel.BottomFold != 1)
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 2)), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 2)), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0 - panel.LeftFirstFoldWidth) + panel.KFactor * 2, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
                        guidList.Add(panel.Perimeter);

                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            foldText = "UP 90°";
                        }
                        else
                        {
                            foldText = "DOWN 90°";
                        }
                        RhinoApp.RunScript("_SelNone", true);
                        height = panel.LeftFirstFoldWidth / 2 - panel.KFactor;
                        Point3d drawPoint = new Point3d(panelX0 - panel.LeftFirstFoldWidth + 2 * panel.KFactor, (panelY0 + panelY1) / 2 - 1.75 * panel.LeftFirstFoldWidth + height, 0);
                        plane.Origin = drawPoint;
                        Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                        RhinoObject textObj = doc.Objects.Find(id);
                        textObj.Select(true);
                        //             RhinoApp.RunScript("_-rotate " + (panelX0 - panel.LeftFirstFoldWidth / 2 - panel.KFactor) + "," + ((panelY0 + panelY1) / 2) + " " + "90", true);
                        textObj.Geometry.Transform(Rhino.Geometry.Transform.Rotation(Math.PI / 2, drawPoint));
                        textObj.CommitChanges();
                        BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                        RhinoApp.RunScript("_Move " + bbxt.Center + " " + ((panelX0 + 2 * panel.KFactor - panel.LeftFirstFoldWidth + panelX0) / 2) + "," + bbxt.Center.Y + " ", true);
                        textObj.Attributes.LayerIndex = foldLabelLayer;
                        textObj.CommitChanges();
                        textObj.Select(false);
                    }
                } //end single fold left

                //If Double folded
                if (panel.PanelType.Equals("Double Folded"))
                {
                    //Drawing the single fold prior to drawing the double fold 
                    //get the end point of the single fold
                    //Multiple the Kfactor by twice to get the correct single fold end point
                    double singleFoldEndPoint = (panelX0 + (panel.KFactor * 3)) - panel.LeftFirstFoldWidth;


                    if (panel.TopFirstFoldSetbackLeft != 0 && panel.BottomFirstFoldSetbackLeft != 0 || panel.TopFirstFoldSetbackLeft != 0 || panel.BottomFirstFoldSetbackLeft != 0)
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0 + panel.LeftFirstFoldSetbackTop, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0 + panel.LeftFirstFoldSetbackBottom, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                    }
                    else
                    {
                        //Top
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelX0, panelY1, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                        guidList.Add(panel.Perimeter);
                        //Bottom 
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws the horizontal bottom line of the left hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelX0), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelX0, panelY0, 0)); // draws the vertical bottom line to connect left hand side bottom to bottom left hand side (drawn from top to bottom)
                        guidList.Add(panel.Perimeter);
                    }
                    //End of drawing single fold 
                    //Draw the vertical line for the left top and left bottom
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor), panelY1 - panel.LeftSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor), panelY0 + panel.LeftSecondFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                    guidList.Add(panel.Perimeter);

                    //If  Left hand side top and bottom corners are both notch execute this 
                    //If Left Top and Left Bottom are flat folds
                    //If Left top is a notch fold or Left top is flat fold     
                    //If Left Bottom is a notch fold or Left Bottom is flat fold  
                    if (panel.LeftFoldTopType.Equals("Notch Fold") && panel.LeftFoldBottomType.Equals("Notch Fold") ||
                       panel.LeftFoldTopType.Equals(doubleFoldTypeFlat) && panel.LeftFoldBottomType.Equals(doubleFoldTypeFlat) ||
                       panel.LeftFoldTopType.Equals(doubleFoldTypeNotch) || panel.LeftFoldTopType.Equals(doubleFoldTypeFlat) ||
                       panel.LeftFoldBottomType.Equals(doubleFoldTypeFlat) || panel.LeftFoldBottomType.Equals(doubleFoldTypeNotch))
                    {
                        //Draw the Left top horizontal line from single fold end point (drawn from left to right)
                        if (panel.LeftFoldTopType.Equals(doubleFoldTypeNotch) || panel.LeftFoldTopType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d((singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor)), panelY1 - panel.LeftSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint, panelY1 - panel.LeftSecondFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                            guidList.Add(panel.Perimeter);
                        }

                        //Draws the left bottom horizontal line from single fold end point (drawn from left to right)
                        if (panel.LeftFoldBottomType.Equals(doubleFoldTypeNotch) || panel.LeftFoldBottomType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor), panelY0 + panel.LeftSecondFoldSetbackBottom, 0), new Point3d(singleFoldEndPoint, panelY0 + panel.LeftSecondFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                            guidList.Add(panel.Perimeter);
                        }
                    }

                    //Execute only if Left Top and left bottom are miter joints
                    //If Left Top is a miter joint or left bottom is a miter joint
                    if (panel.LeftFoldTopType.Equals(doubleFoldTypeMiter) && panel.LeftFoldBottomType.Equals(doubleFoldTypeMiter) ||
                       panel.LeftFoldTopType.Equals(doubleFoldTypeMiter) || panel.LeftFoldBottomType.Equals(doubleFoldTypeMiter))
                    {
                        if (panel.LeftFoldTopType.Equals(doubleFoldTypeMiter)) //if Bottom left is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d((singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor)), panelY1 - panel.LeftSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                            guidList.Add(panel.Perimeter);
                        }
                        if (panel.LeftFoldBottomType.Equals(doubleFoldTypeMiter)) //if Bottom right is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint - (panel.LeftSecondFoldWidth - panel.KFactor), panelY0 + panel.LeftSecondFoldSetbackBottom, 0), new Point3d(singleFoldEndPoint, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                            guidList.Add(panel.Perimeter);
                        }
                    }

                }//end of double left fold
            } //end of left fold



            // Right First Fold
            if (panel.RightFold == 1)
            {
                if (panel.PanelType.Equals("Single Folded"))
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
                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            foldText = "UP 90°";
                        }
                        else
                        {
                            foldText = "DOWN 90°";
                        }
                        RhinoApp.RunScript("_SelNone", true);

                        height = panel.RightFirstFoldWidth / 2 - panel.KFactor;
                        Point3d drawPoint = new Point3d(panelX1 + panel.RightFirstFoldWidth / 2 + panel.RightFirstFoldWidth / 4 - panel.KFactor, (panelY0 + panelY1) / 2 - 1.75 * panel.RightFirstFoldWidth + height, 0);
                        plane.Origin = drawPoint;
                        Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                        RhinoObject textObj = doc.Objects.Find(id);
                        textObj.Select(true);
                        //                        RhinoApp.RunScript("_-rotate " + (panelX1 + panel.RightFirstFoldWidth / 2 + panel.KFactor) + "," + ((panelY0 + panelY1) / 2)+ " " + "90", true);
                        textObj.Geometry.Transform(Rhino.Geometry.Transform.Rotation(Math.PI / 2, drawPoint));
                        textObj.CommitChanges();
                        BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                        RhinoApp.RunScript("_Move " + bbxt.Center + " " + ((panelX1 - 2 * panel.KFactor + panel.RightFirstFoldWidth + panelX1) / 2) + "," + (panelY0+panelY1)/2 + " ", true);
                        textObj.Select(false);
                        textObj.Attributes.LayerIndex = foldLabelLayer;
                        textObj.CommitChanges();
                    }
                    if (panel.LeftFold == 1 && panel.RightFold == 1 && panel.TopFold != 1 & panel.BottomFold != 1)
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.RightFirstFoldSetbackTop, 0)); //draws the straight line of the right hand side fold
                        guidList.Add(panel.Perimeter);

                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the right hand side fold (left to right)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.RightFirstFoldSetbackTop, 0)); //draws the top line of the right hand side fold 
                        guidList.Add(panel.Perimeter);

                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            foldText = "UP 90°";
                        }
                        else
                        {
                            foldText = "DOWN 90°";
                        }
                        RhinoApp.RunScript("_SelNone", true);
                        height = panel.RightFirstFoldWidth  / 2 - panel.KFactor;
                        Point3d drawPoint = new Point3d(panelX1 + panel.RightFirstFoldWidth / 2 + panel.RightFirstFoldWidth / 4 - panel.KFactor, (panelY0 + panelY1) / 2 - panel.RightFirstFoldWidth + height, 0);
                        plane.Origin = drawPoint;
                        Guid id = doc.Objects.AddText(foldText, plane, height, font, false, false);
                        RhinoObject textObj = doc.Objects.Find(id);
                        textObj.Select(true);
                        textObj.Geometry.Transform(Rhino.Geometry.Transform.Rotation(Math.PI/2, drawPoint));
                        textObj.CommitChanges();
                        BoundingBox bbxt = textObj.Geometry.GetBoundingBox(true);
                        RhinoApp.RunScript("_Move " + bbxt.Center + " " + ((panelX1 - 2 * panel.KFactor + panel.RightFirstFoldWidth + panelX1) / 2) + "," + (panelY0 + panelY1) / 2 + " ", true);
                        textObj.Select(false);
                        textObj.Attributes.LayerIndex = foldLabelLayer;
                        textObj.CommitChanges();
                    }
                }//end of single fold 

                if (panel.PanelType.Equals("Double Folded"))
                {
                    //Drawing the single fold prior to drawing the double fold 
                    //get the end point of the single fold
                    //Multiple the Kfactor by twice to get the correct single fold end point
                    double singleFoldEndPoint = (panelX1 - (panel.KFactor * 3)) + panel.RightFirstFoldWidth;

                    if (panel.TopFirstFoldSetbackRight != 0 && panel.BottomFirstFoldSetbackRight != 0 || panel.TopFirstFoldSetbackRight != 0 || panel.BottomFirstFoldSetbackRight != 0)
                    {
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackTop, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //Top (draw from right to left)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1 - panel.RightFirstFoldSetbackBottom, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //Bottom (draw from right to left)
                        guidList.Add(panel.Perimeter);
                    }
                    else
                    {
                        //Top
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //Top (draw from right to left)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelX1, panelY1, 0)); //Draws a vertical line to connect the right hand side fold and the top right hand side
                        guidList.Add(panel.Perimeter);
                        //Bottom 
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //Bottom (draw from right to left)
                        guidList.Add(panel.Perimeter);
                        panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelX1, panelY0, 0)); //Draws a vertical line to connect right hand side bottom and bottom hand side right
                        guidList.Add(panel.Perimeter);
                    }


                    //Draw the vertical line for the Right top and Right bottom
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor), panelY1 - panel.RightSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor), panelY0 + panel.RightSecondFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                    guidList.Add(panel.Perimeter);
                    //If  Right hand side top and bottom corners are both notch execute this 
                    //If Right Top and Right Bottom are fold flats
                    //If Right Top is a notch or Right Top is a fold Flat
                    //If Right Bottom is a fold flat or Right Bottom is a notch fold
                    if (panel.RightFoldTopType.Equals(doubleFoldTypeNotch) && panel.RightFoldBottomType.Equals(doubleFoldTypeNotch) ||
                       panel.RightFoldTopType.Equals(doubleFoldTypeNotch) && panel.RightFoldBottomType.Equals(doubleFoldTypeFlat) ||
                       panel.RightFoldTopType.Equals(doubleFoldTypeFlat) || panel.RightFoldTopType.Equals(doubleFoldTypeNotch) ||
                       panel.RightFoldBottomType.Equals(doubleFoldTypeFlat) || panel.RightFoldBottomType.Equals(doubleFoldTypeNotch))
                    {
                        //Draw the Right top horizontal line from single fold end point (drawn from Right to Left)
                        //If right top is notch or fold flat
                        if (panel.RightFoldTopType.Equals(doubleFoldTypeNotch) || panel.RightFoldTopType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d((singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor)), panelY1 - panel.RightSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint, panelY1 - panel.RightSecondFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                            guidList.Add(panel.Perimeter);
                        }
                        //Draws the Right bottom horizontal line from single fold end point (drawn from left to right)
                        //If right Bottom is notch or fold flat
                        if (panel.RightFoldBottomType.Equals(doubleFoldTypeNotch) || panel.RightFoldBottomType.Equals(doubleFoldTypeFlat))
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor), panelY0 + panel.RightSecondFoldSetbackBottom, 0), new Point3d(singleFoldEndPoint, panelY0 + panel.RightSecondFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                            guidList.Add(panel.Perimeter);
                        }
                    }

                    //Execute only if Left Top and left bottom are miter joints
                    //If Left Top is a miter joint or left bottom is a miter joint
                    if (panel.RightFoldTopType.Equals(doubleFoldTypeMiter) && panel.RightFoldBottomType.Equals(doubleFoldTypeMiter) ||
                       panel.RightFoldTopType.Equals(doubleFoldTypeMiter) || panel.RightFoldBottomType.Equals(doubleFoldTypeMiter))
                    {
                        if (panel.RightFoldTopType.Equals(doubleFoldTypeMiter)) //if Bottom left is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d((singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor)), panelY1 - panel.RightSecondFoldSetbackTop, 0), new Point3d(singleFoldEndPoint, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the horizontal top line of the left hand side fold (left to right)
                            guidList.Add(panel.Perimeter);
                        }
                        if (panel.RightFoldBottomType.Equals(doubleFoldTypeMiter)) //if Bottom right is a miter joint
                        {
                            panel.Perimeter = doc.Objects.AddLine(new Point3d(singleFoldEndPoint + (panel.RightSecondFoldWidth - panel.KFactor), panelY0 + panel.RightSecondFoldSetbackBottom, 0), new Point3d(singleFoldEndPoint, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); // draws a vertical to connect the left top and top right lines (drawn bottom to top)
                            guidList.Add(panel.Perimeter);
                        }
                    }
                }//end of double fold

            }
            //layerIndex = createSubLayers.createSubLayer("FIXING HOLES",
            //System.Drawing.Color.Black, doc.Layers[doc.Layers.Find("LAYERS FOR NESTING", true)]);
            layerIndex = createSubLayers.createSubLayer("RELIEF HOLES",
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


            //Drawing connecting line for the double folds
            //Recalculate setbacks based on corners
            //Calculate top left second fold setback
            if (panel.TopSecondFoldRequired == 1 && panel.TopFoldLeftType.Equals(doubleFoldTypeMiter))
            {
                topScndFoldSetbackLeftConnect = panel.TopFirstFoldSetbackLeft; //Set the variable second fold setback to first fold setback
            }
            else
            {
                topScndFoldSetbackLeftConnect = panel.TopSecondFoldSetbackLeft; //Set the varaible to second fold setback
            }
            //Calculate top right second fold setback
            if (panel.TopSecondFoldRequired == 1 && panel.TopFoldRightType.Equals(doubleFoldTypeMiter))
            {
                topScndFoldSetbackRightConnect = panel.TopFirstFoldSetbackRight; //Set the variable second fold setback to first fold setback
            }
            else
            {
                topScndFoldSetbackRightConnect = panel.TopSecondFoldSetbackRight;
            }
            //Calculate bottom left second fold setback
            if (panel.BottomSecondFoldRequired == 1 && panel.BottomFoldLeftType.Equals(doubleFoldTypeMiter))
            {
                bottomScndFoldSetbackLeftConnect = panel.BottomFirstFoldSetbackLeft; //Set the variable second fold setback to first fold setback
            }
            else
            {
                bottomScndFoldSetbackLeftConnect = panel.BottomSecondFoldSetbackLeft;
            }
            //Calculate bottom right second fold setback
            if (panel.BottomSecondFoldRequired == 1 && panel.BottomFoldRightType.Equals(doubleFoldTypeMiter))
            {
                bottomScndFoldSetbackRightConnect = panel.BottomFirstFoldSetbackRight; //Set the variable second fold setback to first fold setback
            }
            else
            {
                bottomScndFoldSetbackRightConnect = panel.BottomSecondFoldSetbackRight;
            }
            //Calculate Left Top second fold setback
            if (panel.LeftSecondFoldRequired == 1 && panel.LeftFoldTopType.Equals(doubleFoldTypeMiter))
            {
                leftScndFoldSetbackTopConnect = panel.LeftFirstFoldSetbackTop; //Set the variable second fold setback to first fold setback
            }
            else
            {
                leftScndFoldSetbackTopConnect = panel.LeftSecondFoldSetbackTop;
            }
            //Calculate Left Bottom second fold setback
            if (panel.LeftSecondFoldRequired == 1 && panel.LeftFoldBottomType.Equals(doubleFoldTypeMiter))
            {
                leftScndFoldSetbackBottomConnect = panel.LeftFirstFoldSetbackBottom; //Set the variable second fold setback to first fold setback
            }
            else
            {
                leftScndFoldSetbackBottomConnect = panel.LeftSecondFoldSetbackBottom;
            }
            //Calculate Right Top second fold setback
            if (panel.RightSecondFoldRequired == 1 && panel.RightFoldTopType.Equals(doubleFoldTypeMiter))
            {
                rightScndFoldSetbackTopConnect = panel.RightFirstFoldSetbackTop; //Set the variable second fold setback to first fold setback
            }
            else
            {
                rightScndFoldSetbackTopConnect = panel.RightSecondFoldSetbackTop;
            }
            //Calculate Right Bottom second fold setback
            if (panel.RightSecondFoldRequired == 1 && panel.RightFoldBottomType.Equals(doubleFoldTypeMiter))
            {
                rightScndFoldSetbackBottomConnect = panel.RightFirstFoldSetbackBottom; //Set the variable second fold setback to first fold setback
            }
            else
            {
                rightScndFoldSetbackBottomConnect = panel.RightSecondFoldSetbackBottom;
            }


            //Connecting Top Second Fold
            if (panel.TopSecondFoldRequired == 1 && panel.TopSecondFoldSetbackLeft != 0 && panel.TopSecondFoldSetbackRight != 0)
            {
                //  if(panel.TopFoldLeftType.Equals())
                //Top Left                                                        
                panel.Perimeter = doc.Objects.AddLine(pointTopLeft, new Point3d(panelX0 + topScndFoldSetbackLeftConnect, panelY1 + (panel.TopFirstFoldWidth - panel.KFactor * 2), 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
                //Top Right
                panel.Perimeter = doc.Objects.AddLine(pointTopRight, new Point3d(panelX1 - topScndFoldSetbackRightConnect, panelY1 + (panel.TopFirstFoldWidth - panel.KFactor * 2), 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
            }
            //Connecting bottom Second Fold
            if (panel.BottomSecondFoldRequired == 1 && panel.BottomSecondFoldSetbackLeft != 0 && panel.BottomSecondFoldSetbackRight != 0)
            {
                //Bottom Left                                                        
                panel.Perimeter = doc.Objects.AddLine(pointBottomLeft, new Point3d(panelX0 + bottomScndFoldSetbackLeftConnect, panelY0 - (panel.BottomFirstFoldWidth - panel.KFactor * 2), 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
                //Bottom Right
                panel.Perimeter = doc.Objects.AddLine(pointBottomRight, new Point3d(panelX1 - bottomScndFoldSetbackRightConnect, panelY0 - (panel.BottomFirstFoldWidth - panel.KFactor * 2), 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
            }

            //Connecting Left Second Fold
            if (panel.LeftSecondFoldRequired == 1 && panel.LeftSecondFoldSetbackTop != 0 && panel.LeftSecondFoldSetbackBottom != 0)
            {
                //Left Top                                                        
                panel.Perimeter = doc.Objects.AddLine(pointLeftTop, new Point3d(panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 3), panelY1 - leftScndFoldSetbackTopConnect, 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
                //Left Bottom
                panel.Perimeter = doc.Objects.AddLine(pointLeftBottom, new Point3d(panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 3), panelY0 + leftScndFoldSetbackBottomConnect, 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
            }

            //Connecting Right Second Fold
            if (panel.RightSecondFoldRequired == 1 && panel.RightSecondFoldSetbackTop != 0 && panel.RightSecondFoldSetbackBottom != 0)
            {
                //Right Top                                                        
                panel.Perimeter = doc.Objects.AddLine(pointRightTop, new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 3), panelY1 - rightScndFoldSetbackTopConnect, 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
                //Right Bottom
                panel.Perimeter = doc.Objects.AddLine(pointRightBottom, new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 3), panelY0 + rightScndFoldSetbackBottomConnect, 0)); //Multiply kfactor by twice to get the correct first fold length
                guidList.Add(panel.Perimeter);
            }

            // Create a new layer called LABELS
            layerName = "LABELS";
            layerIndex = createSubLayers.createSubLayer(layerName,
               System.Drawing.Color.Red, parent_layer_Nesting); //pass to the method, make Nesting layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            text = panel.PartName;
            height = panel.labelHeight;
            pt = new Rhino.Geometry.Point3d(borderX0, borderY0 + 4 + height, 0);
            plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            plane.Origin = pt;
            panel.Label = doc.Objects.AddText(text, plane, height, font, false, false);
            guidList.Add(panel.Label);

            RhinoApp.RunScript("SelNone", true);
            RhinoObject labelText = doc.Objects.Find(panel.Label);
            labelText.Select(true);
            BoundingBox bbox = labelText.Geometry.GetBoundingBox(true);
            double minX = bbox.Corner(true, true, true).X;
            double maxX = bbox.Corner(false, true, true).X;
            double minY = bbox.Corner(true, true, true).Y;
            double maxY = bbox.Corner(true, false, true).Y;

            if (maxX - minX >= panel.X - panel.LeftBorder - panel.RightBorder)
            {
                double ratio = 1;
                labelText.Select(true);
                if (panel.Y > panel.X)
                {
                    RhinoApp.RunScript("_-rotate " + bbox.Center.X + "," + bbox.Center.Y + " " + "90", true);
                }
                if (maxY - minY + 4 >= panel.X - panel.LeftBorder - panel.RightBorder)
                {
                    ratio = (panel.X - panel.LeftBorder - panel.RightBorder) / (2 * (maxY - minY));
                    if (ratio * (maxX - minX) >= (panel.Y - panel.TopBorder - panel.BottomBorder))
                    {
                        ratio = ratio * (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * ratio * (maxX - minX));
                    }
                }
                else if (maxX - minX >= panel.Y - panel.TopBorder - panel.BottomBorder)
                {
                    ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxX - minX));
                }
                RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                BoundingBox bbox3 = labelText.Geometry.GetBoundingBox(true);
                double distance1 = borderX0 + ratio * (bbox3.Max.X - bbox3.Min.X) / 2;
                double distance2 = borderY0 + ratio * (bbox3.Max.Y - bbox3.Min.Y) / 2;
                if (panel.Y > panel.X)
                {
                    distance1 = borderX0 + ratio * (bbox3.Max.Y - bbox3.Min.Y) / 2;
                    distance2 = borderY0 + ratio * (bbox3.Max.X - bbox3.Min.X) / 2;
                }

                RhinoApp.WriteLine(bbox3.Center.ToString());
                RhinoApp.RunScript("_-Move " + bbox3.Center.X + "," + bbox3.Center.Y + ",0 " + distance1 + "," + distance2 + ",0", true);


            }
            else if (maxY - minY >= panel.Y - panel.TopBorder - panel.BottomBorder)
            {
                double ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxY - minY));
                labelText.Select(true);
                RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                double distanceX = borderX0 + ratio * (bbox2.Center.X - bbox2.Min.X) / 2;
                double distanceY = panelBox.Min.Y + ratio * (bbox2.Center.Y - bbox.Min.Y) / 2;

                RhinoApp.WriteLine(bbox2.Center.ToString());
                RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
            }
            labelText.Select(false);

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
                double panelOffset = 0;
                if (panel.BottomFirstFoldWidth - 2 * panel.KFactor - 9 <= 6)
                {
                    panelOffset = (panel.BottomFirstFoldWidth - 2 * panel.KFactor - 9) / 2;
                }
                else
                {
                    panelOffset = 3;
                }
                if (panel.BottomFold == 1)
                {
                    if (panel.X < 160)
                    {
                        pt = new Point3d(1 * (panelX0 + panelX1) / 2, panelFirstBottomY1 + 2 * panel.KFactor + panelOffset + 0.17 + 8.7, 0);
                    }
                    else
                    {
                        pt = new Point3d(panelX1 - 100, panelFirstBottomY1 + 2 * panel.KFactor + panelOffset + 0.17+ 8.7, 0);
                    }
                }
                else
                {
                    if (panel.BottomBorder - 8.7 <= 6)
                    {
                        panelOffset = (panel.BottomBorder - 8.7) / 2;
                    }
                    else
                    {
                        panelOffset = 3.1;
                    }
                    if (panel.X < 160)
                    {
                        pt = new Point3d(1 * (panelX0 + panelX1) / 2, panelY0 + panelOffset + 8.7, 0);
                    }
                    else
                    {
                        pt = new Point3d(panelX1 - 100, panelY0 + panelOffset + 8.7, 0);
                    }
                }
               
                //  DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, 8.7);
                if (panel.DotFontLabellerSide.Equals("Rear"))
                {
                    DotMatrixLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, 8.7, panel.X); //set the size of dotfont 
                }
                else  //If not revered use front labeller
                {
                    DotMatrixFrontLabellerCommand.Instance.drawDotMatrix(pt, panel.PartName, 8.7);
                }
            }

            // Unselect all objects
            doc.Objects.UnselectAll();
            if (panel.DrawPerf == 1)
            {
                RhinoUtilities.SetActiveLayer("TemporaryPerfLayer", System.Drawing.Color.Green);
                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 -" + panel.patternDirection, true);
                PerforationForm perforationForm = new PerforationForm(new Rhino.DocObjects.ObjRef(panel.Border).Curve());
                perforationForm.drawPerforationDesign(panel.PatternName, true, true);
                RhinoApp.RunScript("SelAll", true);
                RhinoApp.RunScript("-_Rotate 0,0,0 " + panel.patternDirection, true);
                RhinoApp.RunScript("SelNone", true);

                var rhinoObjects = doc.Objects.FindByLayer("TemporaryPerfLayer");
                var toolHitObjects = doc.Objects.FindByLayer("TemporaryToolHit");
                var temporaryTool2Layer = doc.Objects.FindByLayer("TemporaryTool2Layer");

                //Perf objects 
                int index;
                if (rhinoObjects != null && rhinoObjects.Length > 1)
                {
                    foreach (var rhinObj in rhinoObjects)
                    {
                        rhinObj.Select(true);
                    }
                    if (rhinoObjects != null && panel.patternDirection == 1)
                    {
                        RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                    }
                    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.PerforationLayerName, System.Drawing.Color.Green);
                    RhinoApp.RunScript("-_ChangeLayer PERFORATION", true);
                    index = doc.Layers.Find("TemporaryPerfLayer", true);
                    doc.Layers.Delete(index, true);
                }

                //tool hit objects
                if (toolHitObjects != null && toolHitObjects.Length > 1)
                {
                    foreach (var toolhitObj in toolHitObjects)
                    {
                        toolhitObj.Select(true);
                    }
                    if (panel.patternDirection == 1)
                    {
                        RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                    }
                    RhinoUtilities.SetActiveLayer(Properties.Settings.Default.ToolHitLayerName, System.Drawing.Color.Black);
                    RhinoApp.RunScript("-_ChangeLayer TOOL HIT", true);
                    index = doc.Layers.Find("TemporaryToolHit", true);
                    doc.Layers.Delete(index, true);
                }

                //Tool 2 objects
                if (temporaryTool2Layer != null && temporaryTool2Layer.Length > 1)
                {
                    foreach (var tool2Objs in temporaryTool2Layer)
                    {
                        tool2Objs.Select(true);
                    }
                    if (panel.patternDirection == 1)
                    {
                        RhinoApp.RunScript("-_Rotate 0,0,0 -90", true);
                        RhinoApp.RunScript("-_Rotate 0,0,0 90", true);
                    }

                    RhinoUtilities.SetActiveLayer("Tool 2 Layer", System.Drawing.Color.Yellow);
                    RhinoApp.RunScript("-_ChangeLayer Tool 2 Layer", true);
                    index = doc.Layers.Find("TemporaryTool2Layer", true);
                    doc.Layers.Delete(index, true);
                }
            }

            // Draw Folded layer on LHS
            layerName = "DIMENSIONS BLUE";  //This layer is responsible for blue colour dimensions on the panel
            layerIndex = createSubLayers.createSubLayer(layerName,
                   System.Drawing.Color.Blue, parent_layer_Approval);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            double panelRight = 0; //This will be the right hand side of the blue dimension line (left)
            double panelLeft = 0; //this will be the left hand side of the blue diemension line (left)
            double panelRightHS = 0;//This will be the right hand side of the blue dimension line (right)
            double panelLeftHS = 0;//this will be the left hand side of the blue diemension line (right)
            List<Line> listpoint = new List<Line>();
            List<Curve> listcurve = new List<Curve>();

            //panelRight = panelFirstLeftX1 - 4 * Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth);
            panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
            panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;
            //panelRightHS = panelFirstRightX1 + 4 * Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth);
            panelRightHS = panelFirstRightX1 +300 - 2 * panel.KFactor;
            panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

            //Draws the Left and Right

            //Go if top and bottom fold is single or double and top / bottom fold direction is Up
            if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1 && panel.BottomFold == 1) //top and bottom fold up (left)
            {
                {
                    //Left hand side
                    
                    //panelRight = panelFirstLeftX1 -  4 * Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth);
                    panelLeft = panelFirstLeftX1 -  300 + 2 * panel.KFactor - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth);
                    panelRight = panelLeft - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                    listpoint.Add(new Line(new Point3d(panelLeft, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                    listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                    listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                    listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                    listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0)));
                    listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                    //Execute this block only if both folds are required top and bottom
                    if (panel.TopFold == 1 && panel.BottomFold == 1)
                    {
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
                    }
                    //Execute this block only if top fold is required and no bottom fold
                    if (panel.TopFold == 1 && panel.BottomFold != 1)
                    {
                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeft, panelY0, 0), listcurve[2].PointAtStart));
                        listcurve.Clear();

                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), listcurve[2].PointAtStart));

                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft, panelY0, 0), new Point3d(panelLeft + panel.SheetThickness, panelY0, 0))));
                    }
                    //Execute this block only if bottom fold is required and no top fold
                    if (panel.TopFold != 1 && panel.BottomFold == 1)
                    {
                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeft, panelY1, 0), listcurve[2].PointAtStart));
                        listcurve.Clear();

                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                        listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), listcurve[2].PointAtStart));
                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft, panelY1, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1, 0))));
                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));

                    }
                    //draws the right Section
                    listpoint.Clear();
                    listcurve.Clear();
                    //Mark

                    panelLeftHS = panelFirstRightX1 + 300 - 2 * panel.KFactor + Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth);
                    panelRightHS = panelLeftHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                    listpoint.Add(new Line(new Point3d(panelLeftHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                    listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                    listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                    listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0)));
                    listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0, 0)));
                    listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0)));

                    if (panel.TopFold == 1 && panel.BottomFold == 1) //Draw if both folds are required
                    {
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
                    if (panel.TopFold == 1 && panel.BottomFold != 1) //Draw if only top fold is required
                    {

                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeftHS, panelY0, 0), listcurve[2].PointAtStart));
                        listcurve.Clear();

                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0), listcurve[2].PointAtStart));

                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS, panelY0, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0))));

                    }
                    if (panel.TopFold != 1 && panel.BottomFold == 1) //Draw if both folds are required
                    {
                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeftHS, panelY1, 0), listcurve[2].PointAtStart));
                        listcurve.Clear();

                        listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                        guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                        guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                        guidList.Add(doc.Objects.AddLine(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), listcurve[2].PointAtStart));

                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS, panelY1, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0))));
                        guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));
                    }


                }
            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1) //top and bottom burr
            {
                //Draws the left hand side
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;
                positionLeftBurr = panelRight;

                listpoint.Add(new Line(new Point3d(panelRight, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))); //upper bottom horizontal line

                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0))); //Bottom Upper horizontal line 
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))); //bottom bottom horizontal line

                //Execute this if top and bottom folds are required
                if (panel.TopFold == 1 && panel.BottomFold == 1)
                {

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
                }
                //Execute this block if only top fold is required
                if (panel.TopFold == 1 && panel.BottomFold != 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRight, panelY0, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight, panelY0, 0), new Point3d(panelRight - panel.SheetThickness, panelY0, 0))));
                }
                //Execute this block if only bottom fold is required
                if (panel.TopFold != 1 && panel.BottomFold == 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the top horizontal line
                    guidList.Add(doc.Objects.AddCurve(listcurve[2])); //draws the bottom horizontal line
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRight, panelY1, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight, panelY1, 0), new Point3d(panelRight - panel.SheetThickness, panelY1, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));
                }
                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelRightHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))); //upper bottom hor line

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0))); //Upper bottom vertical line 
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))); //bottom bottom vertical line

                //Execute if top and bottom folds both are required
                if (panel.TopFold == 1 && panel.BottomFold == 1)
                {
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
                //Execute this block if only top fold is required
                if (panel.TopFold == 1 && panel.BottomFold != 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRightHS, panelY0, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS, panelY0, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0))));

                }
                //Execute this block only if bottom is required
                if (panel.TopFold != 1 && panel.BottomFold == 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the second vertical line
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelRightHS, panelY1, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS, panelY1, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0))));

                }


            }
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1) //bottom burr
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);
                positionLeftBurr = panelRight;
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
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
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
            else if (panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 2 && panel.TopFold == 1 || panel.TopFoldType == 1 && panel.BottomFoldType == 1 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1) //bottom burr
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);
                positionLeftBurr = panelRight;
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
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
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
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth + 3 * panel.SheetThickness);
                positionLeftBurr = panelRight;

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
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
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
            panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
            panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;
            panelBottomBH = panelFirstBottomY0 - 300 + 2 * panel.KFactor;
            panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

            if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1 && panel.RightFold == 1) //left and right fold up
            {
                panelTop = panelFirstTopY1 + 300 - 2 * panel.KFactor + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth);
                panelBottom = panelTop + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                //draw the top              
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop, 0)));  //This and the below line of code draws the horizontal lines in the top blue dimension
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0)));

                if (panel.LeftFold == 1 && panel.RightFold == 1)
                {
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
                }
                if (panel.LeftFold == 1 && panel.RightFold != 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelTop, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop, 0))));

                }
                if (panel.LeftFold != 1 && panel.RightFold == 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelTop, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop, 0))));
                }

                //draw the bottom 
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor - Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth);
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                if (panel.LeftFold == 1 && panel.RightFold == 1)
                {
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
                if(panel.LeftFold == 1 && panel.RightFold != 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottomBH, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH, 0), new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0))));
                }
                if (panel.LeftFold != 1 && panel.RightFold == 1)
                {
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottomBH, 0), listcurve[2].PointAtStart));

                    listcurve.Clear();

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), listcurve[2].PointAtStart));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth, 0))));
                }
            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.RightFold == 1) //left and right burr
            { //place where top is drawn

                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                if (panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0)));
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));

                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));

                    listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0)));

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
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0))));

                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0))));
                }
                else if (panel.RightFold == 1)
                {
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));
                    listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0)));

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottom, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    listcurve.Clear();
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom, 0))));
                }
                else
                {
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0)));
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottom, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    listcurve.Clear();
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom, 0))));
                }






                //draw bottom
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                if (panel.LeftFold == 1 && panel.RightFold == 1)
                {
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
                else if (panel.RightFold == 1)
                {
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0)));

                    listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth, 0)));

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottomBH, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH, 0))));
                }
                else
                {
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0)));

                    listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0)));
                    listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth, 0)));

                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottomBH, 0), listcurve[2].PointAtStart));
                    listcurve.Clear();
                    listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                    guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                    guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                    guidList.Add(doc.Objects.AddLine(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), listcurve[2].PointAtStart));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH, 0))));
                    guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth, 0))));
                }

            }
            else if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 2 && panel.LeftFold == 1 || panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 2 && panel.RightFold == 1) //right burr
            {
                //Top
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
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
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
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
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
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
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
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

            //Drawing the double folds

            /**
             * Drawing the left and right burrs for double folds
             * */

            //Execute this block only if fold type is double folded and direction is downward - 2 represents downward
            if (panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopSecondFoldDirection == 2 && panel.BottomSecondFoldDirection == 2 && panel.TopSecondFoldRequired == 1
                  || panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopSecondFoldDirection == 2 && panel.BottomSecondFoldDirection == 2 && panel.BottomSecondFoldRequired == 1)
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelRight, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //top                                                                                                                                  //add the kfactor to trim
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d((panelRight - panel.TopFirstFoldWidth) + panel.KFactor, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY1 - panel.SheetThickness, 0))); //upper bottom horizontal line

                //bottom                                                                                                                                  //add the kfactor to trim
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.KFactor, panelY0, 0))); //Bottom Upper horizontal line 
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY0 + panel.SheetThickness, 0))); //bottom bottom horizontal line

                //Adds the Left Top (double fold)
                listpoint.Add(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, (panelY1 - panel.TopSecondFoldWidth), 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY1 - panel.TopSecondFoldWidth), 0))); //Upper outer horizontal line

                //Adds the Left Bottom (double)
                listpoint.Add(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY0 + panel.BottomSecondFoldWidth), 0))); //Upper outer horizontal line


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded (left hand side - bottom)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                ////Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, (panelY1 - panel.TopSecondFoldWidth), 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY1 - panel.TopSecondFoldWidth), 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness, (panelY0 + panel.BottomSecondFoldWidth), 0))));



                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelRightHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.KFactor, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness - panel.KFactor, panelY1 - panel.SheetThickness, 0))); //upper bottom hor line

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.KFactor, panelY0, 0))); //Upper bottom vertical line 
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness - panel.KFactor, panelY0 + panel.SheetThickness, 0))); //bottom bottom vertical line


                //Adds the Left Top (double fold)
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, (panelY1 - panel.TopSecondFoldWidth), 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY1 - panel.TopSecondFoldWidth), 0))); //Upper outer horizontal line

                //Adds the Left Bottom (double)
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY0 + panel.BottomSecondFoldWidth), 0))); //Upper outer horizontal line

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));
                //Double folded (left hand side - bottom)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                ////Adds the double bottom (fold up) top           
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, (panelY1 - panel.TopSecondFoldWidth), 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY1 - panel.TopSecondFoldWidth), 0))));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness, (panelY0 + panel.BottomSecondFoldWidth), 0))));


            }
            //Execute this block only if fold type is double folded and direction is upward - 1 represents Upward
            if (panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.TopSecondFoldRequired == 1 ||
              panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 1 && panel.BottomFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.BottomSecondFoldRequired == 1)
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelLeft, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //Top Left   
                //trimming
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.KFactor, panelY1 - panel.SheetThickness, 0)));

                //Bottom Left

                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.KFactor - panel.KFactor, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0 + panel.SheetThickness, 0)));

                //Double Fold Top Left                                                                                                                                      Reduced by panel sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 

                //Double Fold Bottom Left
                listpoint.Add(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0 - panel.BottomSecondFoldWidth + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth + panel.SheetThickness), 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));


                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                listcurve.Clear();
                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));


                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));


                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top - horizontal
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))));


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelLeftHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //Adds the kfactor and panel sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.KFactor, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0 + panel.SheetThickness, 0)));

                // Right Top double fold
                //In the upward direction we need to reduce by panel sheet thickness in order to get the correct length
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0)));

                // Right Bottom double fold
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0 - panel.TopSecondFoldWidth + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 - panel.BottomSecondFoldWidth + panel.SheetThickness, 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top - horizontal
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 + panel.TopSecondFoldWidth - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0 - panel.TopSecondFoldWidth + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 - panel.TopSecondFoldWidth + panel.SheetThickness, 0))));

            }

            //Execute this block if 
            //Execute this if it is a double fold and first fold direction is Up and second fold direction down
            if (panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.TopSecondFoldRequired == 1 ||
              panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.BottomSecondFoldRequired == 1)
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelLeft, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //Double Fold Left Top   
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                // Double Fold Left Bottom
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness - panel.KFactor, panelY0 + panel.SheetThickness, 0)));

                //Double Fold Left Top        //Reduced by panel sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.TopSecondFoldWidth, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 - panel.TopSecondFoldWidth, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 

                //Double Fold  Left Bottom
                listpoint.Add(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0)));
                listpoint.Add(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0, 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0 + panel.BottomSecondFoldWidth, 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));


                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                listcurve.Clear();
                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));


                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));


                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top - horizontal
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 - panel.TopSecondFoldWidth, 0), new Point3d(panelLeft + panel.TopFirstFoldWidth, panelY1 - panel.TopSecondFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeft + panel.BottomFirstFoldWidth - panel.SheetThickness, (panelY0 + panel.BottomSecondFoldWidth), 0), new Point3d(panelLeft + panel.BottomFirstFoldWidth, (panelY0 + panel.BottomSecondFoldWidth), 0))));


                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelLeftHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS, panelY1 - panel.SheetThickness, 0)));  //This and the below line draws the vertical lines in dimensions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //Adds the kfactor and panel sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY1, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY0, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY0 + panel.SheetThickness, 0)));

                // Right Top double fold
                //In the upward direction we need to reduce by panel sheet thickness in order to get the correct length
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.TopSecondFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1 - panel.TopSecondFoldWidth, 0)));  //these 4 lines are responsible for drawing the horizontal small lines and the curves in the left dimesions 

                // Right Bottom double fold
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.SheetThickness, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.BottomSecondFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0 + panel.TopSecondFoldWidth, 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double bottom (fold up) top - horizontal
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1 - panel.TopSecondFoldWidth, 0), new Point3d(panelLeftHS - panel.TopFirstFoldWidth, panelY1 - panel.TopSecondFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelLeftHS - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0 + panel.TopSecondFoldWidth, 0), new Point3d(panelLeftHS - panel.BottomFirstFoldWidth, panelY0 + panel.TopSecondFoldWidth, 0))));

            }

            //Execute this block of code only if it is a double fold, the single fold direction is downward and double fold
            //direction is upward
            if (panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopSecondFoldDirection == 1 && panel.BottomSecondFoldDirection == 1 && panel.TopSecondFoldRequired == 1
                  || panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == 2 && panel.BottomFirstFoldDirection == 2 && panel.TopSecondFoldDirection == 1 && panel.BottomSecondFoldDirection == 1 && panel.BottomSecondFoldRequired == 1)
            {
                //Mark
                panelRight = panelFirstLeftX1 - 300 + 2 * panel.KFactor;
                panelLeft = panelRight - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelRight, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                //top                                                                                                                                  //add the kfactor and sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1, 0), new Point3d((panelRight - panel.TopFirstFoldWidth) + panel.KFactor + panel.SheetThickness, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY1 - panel.SheetThickness, 0))); //upper bottom horizontal line

                //bottom                                                                                                                                  //add the kfactor to trim
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.KFactor + panel.SheetThickness, panelY0, 0))); //Bottom Upper horizontal line 
                listpoint.Add(new Line(new Point3d(panelRight - panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness + panel.KFactor, panelY0 + panel.SheetThickness, 0))); //bottom bottom horizontal line

                //Adds the Left Top (double fold)
                listpoint.Add(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, panelY1, 0), new Point3d(panelRight - panel.TopFirstFoldWidth, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))); //Upper outer horizontal line

                //Adds the Left Bottom (double)
                listpoint.Add(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness, panelY0, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))); //Upper outer horizontal line


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded (left hand side - bottom)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                ////Adds the double bottom (fold up) top 
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.TopFirstFoldWidth, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0), new Point3d(panelRight - panel.TopFirstFoldWidth + panel.SheetThickness, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRight - panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0), new Point3d(panelRight - panel.BottomFirstFoldWidth + panel.SheetThickness, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))));



                //draws the right 
                listpoint.Clear();
                listcurve.Clear();
                //Mark
                panelRightHS = panelFirstRightX1 + 300 - 2 * panel.KFactor;
                panelLeftHS = panelRightHS - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - panel.SheetThickness;

                listpoint.Add(new Line(new Point3d(panelRightHS, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS, panelY1 - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY1, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness - panel.KFactor, panelY1 - panel.SheetThickness, 0))); //upper bottom hor line

                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.KFactor - panel.SheetThickness, panelY0, 0))); //Upper bottom vertical line 
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.SheetThickness, panelY0 + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness - panel.KFactor, panelY0 + panel.SheetThickness, 0))); //bottom bottom vertical line


                //Adds the Left Top (double fold)
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, panelY1 - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))); //Upper outer horizontal line

                //Adds the Left Bottom (double)
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness, panelY0, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))); //Upper outer horizontal line
                listpoint.Add(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, panelY0, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))); //Upper outer horizontal line

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.TopFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws horizontal outer line (top)
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.BottomFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4])); //draws the top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[5])); //draws the bottom horizontal line

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));
                //Double folded (left hand side - bottom)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1])); //draws the inner top horizontal line
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //draws the second vertical line
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                ////Adds the double bottom (fold up) top           
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.TopFirstFoldWidth, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0), new Point3d(panelRightHS + panel.TopFirstFoldWidth - panel.SheetThickness, (panelY1 + panel.TopSecondFoldWidth) - panel.SheetThickness, 0))));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelRightHS + panel.BottomFirstFoldWidth, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0), new Point3d(panelRightHS + panel.BottomFirstFoldWidth - panel.SheetThickness, (panelY0 - panel.BottomSecondFoldWidth) + panel.SheetThickness, 0))));


            }

            /**
             * Drawing the top and botttom burrs for double folds
             * */

            listpoint.Clear(); //clear the list 
            listcurve.Clear(); //clear the list curve

            //Go to this if fold type is double fold and direction is downwards 
            if (panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.LeftSecondFoldRequired == 1
               || panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.RightSecondFoldRequired == 1) //left and right burr

            {
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));
                //reduce by kfactor to trim
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth - panel.KFactor, 0)));                            //reduce by sheet thicknes to trim
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth - panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));


                //Adds Top Left
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottom + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0)));
                //Adds Top right
                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d((panelX1 - panel.TopSecondFoldWidth), panelBottom + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d((panelX1 - panel.TopSecondFoldWidth), panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //here here
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0))));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX1 - panel.TopSecondFoldWidth), panelBottom + panel.RightFirstFoldWidth, 0), new Point3d((panelX1 - panel.TopSecondFoldWidth), panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0))));


                //draw bottom
                //Mark
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth + panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth + panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));

                //Adds Bottom Left
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 + panel.LeftSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0), new Point3d((panelX0 + panel.LeftSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0)));
                //Adds Bottom right
                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d((panelX1 - panel.RightSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0), new Point3d((panelX1 - panel.RightSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 + panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX1 - panel.BottomSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d((panelX1 - panel.BottomSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0))));
            }

            //Execute this block if fold is double folded and direction is Upwards
            if (panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
               && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.LeftSecondFoldRequired == 1 || panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
               && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.RightSecondFoldRequired == 1)
            {
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                //draw the top              
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop, 0)));  //This and the below line of code draws the horizontal lines in the top blue dimension
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0)));

                //Added kfactor and sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0)));

                //Double fold Top Left 
                //added sheet thickness to correct the fold length
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0)));

                //Double fold Top Right
                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));



                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0 - panel.LeftSecondFoldWidth + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1 + panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth, 0))));

                //draw the bottom 
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                //Bottom Left
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth, 0)));

                //Bottom Right
                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth, 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0 - panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX0 - panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth, 0))));

            }

            //Execute this if it is a double fold and first fold direction is Up and second fold direction down
            if (panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.LeftSecondFoldRequired == 1 ||
              panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 1 && panel.RightFirstFoldDirection == 1
              && panel.LeftSecondFoldDirection == 2 && panel.RightSecondFoldDirection == 2 && panel.RightSecondFoldRequired == 1)
            {
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                //draw the top              
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop, 0)));  //This and the below line of code draws the horizontal lines in the top blue dimension
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0)));

                //Added kfactor and sheet thickness to trim
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.SheetThickness, 0), new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.SheetThickness, 0), new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness + panel.KFactor, 0)));

                //Double fold Top Left 
                //added sheet thickness to correct the fold length
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelTop - panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0))); //This and the below line of code draws the left vertical line (outer and inner) with curves

                //Double fold Top Right
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelTop - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0))); //This and the below line of code draws the right vertical line (outer and inner) with curves

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelTop - panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelTop - panel.LeftFirstFoldWidth + panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1 - panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth + panel.SheetThickness, 0))));

                ////draw the bottom 
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                //Bottom 
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH + panel.LeftFirstFoldWidth, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0)));

                //Bottom Right
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH + panel.RightFirstFoldWidth, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0)));


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));

                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d(panelX1 - panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d(panelX1 - panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth, 0))));

            }

            //Execute this block of code if the fold is double folded, single fold is downward and double fold is upward 
            if (panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.LeftSecondFoldRequired == 1
               || panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == 2 && panel.RightFirstFoldDirection == 2 && panel.LeftSecondFoldDirection == 1 && panel.RightSecondFoldDirection == 1 && panel.RightSecondFoldRequired == 1) //left and right burr

            {
                panelBottom = panelFirstTopY1 + 300 - 2 * panel.KFactor;
                panelTop = panelBottom + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;


                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0)));
                //reduce by kfactor to trim
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth - panel.KFactor - panel.SheetThickness, 0)));                            //reduce by sheet thicknes to trim
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth - panel.KFactor - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness - panel.KFactor, 0)));


                //Adds Top Left
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth) + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth) + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0)));
                //Adds Top right
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0), new Point3d((panelX1 + panel.TopSecondFoldWidth) - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d((panelX1 + panel.TopSecondFoldWidth) - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));

                guidList.Add(doc.Objects.AddCurve(listcurve[3])); //here here
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();


                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX0 - panel.TopSecondFoldWidth) + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth) + panel.SheetThickness, panelBottom + panel.LeftFirstFoldWidth - panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX1 + panel.TopSecondFoldWidth) - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth, 0), new Point3d((panelX1 + panel.TopSecondFoldWidth) - panel.SheetThickness, panelBottom + panel.RightFirstFoldWidth - panel.SheetThickness, 0))));


                //draw bottom
                panelBottomBH = panelFirstBottomY1 - 300 + 2 * panel.KFactor;
                panelTopTH = panelBottomBH + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + panel.SheetThickness;

                listpoint.Clear();
                listcurve.Clear();

                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX0 + panel.SheetThickness, panelBottomBH - panel.LeftFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));

                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.SheetThickness, 0), new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth + panel.KFactor + panel.SheetThickness, 0)));

                //Adds Bottom Left
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth, 0)));
                //Adds Bottom right
                listpoint.Add(new Line(new Point3d(panelX1 - panel.SheetThickness, panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0), new Point3d((panelX1 + panel.BottomSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0)));
                listpoint.Add(new Line(new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d((panelX1 + panel.TopSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth, 0)));

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[0]), new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[2]), new LineCurve(listpoint[2]).PointAtStart, panel.LeftFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[0]).PointAtStart, new LineCurve(listpoint[4]), new LineCurve(listpoint[4]).PointAtEnd, panel.RightFoldRadius + panel.SheetThickness, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));
                //Double folded (Left hand side - Top)
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[2]).PointAtStart, new LineCurve(listpoint[6]), new LineCurve(listpoint[6]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Double folded Right Handside - Top
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[8]), new LineCurve(listpoint[8]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));
                listcurve.Clear();

                listcurve.AddRange(Curve.CreateFilletCurves(new LineCurve(listpoint[1]), new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[3]), new LineCurve(listpoint[3]).PointAtEnd, panel.LeftFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[1]));
                guidList.Add(doc.Objects.AddCurve(listcurve[2]));
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[0], new LineCurve(listpoint[1]).PointAtEnd, new LineCurve(listpoint[5]), new LineCurve(listpoint[5]).PointAtEnd, panel.RightFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[3]));
                guidList.Add(doc.Objects.AddCurve(listcurve[4]));
                guidList.Add(doc.Objects.AddCurve(listcurve[5]));

                //Adds the double fold  top (fold up) Left hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[1], new LineCurve(listpoint[1]).PointAtStart, new LineCurve(listpoint[7]), new LineCurve(listpoint[7]).PointAtEnd, panel.TopFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[6]));
                guidList.Add(doc.Objects.AddCurve(listcurve[7]));
                guidList.Add(doc.Objects.AddCurve(listcurve[8]));

                //Adds the double fold top (fold up) right hand side
                listcurve.AddRange(Curve.CreateFilletCurves(listcurve[4], new LineCurve(listpoint[4]).PointAtStart, new LineCurve(listpoint[9]), new LineCurve(listpoint[9]).PointAtEnd, panel.BottomFoldRadius, false, true, true, 0, 0));
                guidList.Add(doc.Objects.AddCurve(listcurve[9]));
                guidList.Add(doc.Objects.AddCurve(listcurve[10]));
                guidList.Add(doc.Objects.AddCurve(listcurve[11]));


                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX0 - panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth, 0), new Point3d((panelX0 - panel.TopSecondFoldWidth), panelBottomBH - panel.LeftFirstFoldWidth + panel.SheetThickness, 0))));
                guidList.Add(doc.Objects.AddLine(new Line(new Point3d((panelX1 + panel.BottomSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth, 0), new Point3d((panelX1 + panel.BottomSecondFoldWidth), panelBottomBH - panel.RightFirstFoldWidth + panel.SheetThickness, 0))));
            }

            //Adds the single line for fold profile
            //Adds the Top Face Dimension blue  (fold direction down)
            //0.6 is added trim the left and right (vertical lines) 

            ////Add only if Top fold is required and direction is 2
            //if (panel.TopFirstFoldDirection == 2 && panel.TopFold == 1 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 2 && panel.TopFold == 1 && panel.RightFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelBottom + panel.TopFirstFoldWidth), 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelBottom + panel.TopFirstFoldWidth), 0));  // horizontal line
            //    guidList.Add(panel.Perimeter);
            //    if (panel.TopFold == 1)
            //    {
            //        if(panel.LeftFold == 1 && panel.RightFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelBottom + 0.6, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelBottom + panel.TopFirstFoldWidth), 0)); //right
            //            guidList.Add(panel.Perimeter);

            //        }
            //        if(panel.RightFold == 1 && panel.LeftFold !=1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelBottom + 0.6, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelBottom + panel.TopFirstFoldWidth), 0)); //left 
            //            guidList.Add(panel.Perimeter);
            //        }

            //    }
            //}

            ////Adds the Top Face Dimension blue (fold direction up) 
            ////0.6 is subtracted trim the left and right (vertical lines) 

            ////Add only if Top fold is required and direction is 1
            //if (panel.TopFirstFoldDirection == 1 && panel.TopFold == 1 && panel.LeftFold == 1 || panel.TopFirstFoldDirection == 1 && panel.TopFold == 1 && panel.RightFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelTop - panel.TopFirstFoldWidth), 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelTop - panel.TopFirstFoldWidth), 0));  // horizontal line
            //    guidList.Add(panel.Perimeter);
            //    if (panel.TopFold == 1)
            //    {
            //        if (panel.LeftFold != 1 && panel.RightFold == 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, panelTop - 0.6, 0), new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, (panelTop - panel.TopFirstFoldWidth), 0)); //left 
            //            guidList.Add(panel.Perimeter);
            //        }
            //        if (panel.LeftFold == 1 && panel.RightFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, panelTop - 0.6, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, (panelTop - panel.TopFirstFoldWidth), 0)); //right
            //            guidList.Add(panel.Perimeter);
            //        }
            //    }
            //}


            ////Adds the bottom Face dimension blue (down)
            ////0.6 is added trim the left and right (vertical lines) 

            ////Add only if Bottom fold is required and direction is 2 
            //if (panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 2 && panel.BottomFold == 1 && panel.RightFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH - panel.BottomFirstFoldWidth), 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH - panel.BottomFirstFoldWidth), 0));  // horizontal line
            //    guidList.Add(panel.Perimeter);
            //    if (panel.BottomFold == 1)
            //    {
            //        if(panel.RightFold == 1 && panel.LeftFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelBottomBH - 0.6, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH - panel.BottomFirstFoldWidth), 0)); //left 
            //            guidList.Add(panel.Perimeter);
            //        }

            //        if (panel.LeftFold == 1 && panel.RightFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelBottomBH - 0.6, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH - panel.BottomFirstFoldWidth), 0)); //right
            //            guidList.Add(panel.Perimeter);
            //        }
            //    }
            //}

            ////Adds the bottom Face dimension blue (up)

            ////Add only if Bottom fold is required and direction is 1 
            //if (panel.BottomFirstFoldDirection == 1 && panel.BottomFold == 1 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 1 && panel.BottomFold == 1 && panel.RightFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH + panel.BottomFirstFoldWidth), 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH + panel.BottomFirstFoldWidth), 0));  // horizontal line
            //    guidList.Add(panel.Perimeter);
            //    if (panel.BottomFold == 1)
            //    {
            //        if (panel.RightFold == 1 && panel.LeftFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, panelBottomBH + 0.6, 0), new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, (panelBottomBH + panel.BottomFirstFoldWidth), 0)); //left 
            //            guidList.Add(panel.Perimeter);
            //        }

            //        if (panel.LeftFold == 1 && panel.RightFold != 1)
            //        {
            //            panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, panelBottomBH + 0.6, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, (panelBottomBH + panel.BottomFirstFoldWidth), 0)); //right
            //            guidList.Add(panel.Perimeter);
            //        }
            //    }
            //}

            ////Adds the Left Face Dimension blue  (fold direction down)

            ////Add only Left fold is required and direction 2
            //if (panel.LeftFirstFoldDirection == 2 && panel.LeftFold == 1 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 2 && panel.LeftFold == 1 && panel.BottomFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
            //    guidList.Add(panel.Perimeter);
            //    if(panel.TopFold == 1 && panel.BottomFold != 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelRight - 0.6, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
            //        guidList.Add(panel.Perimeter);
            //    }
            //    if(panel.TopFold != 1 && panel.BottomFold == 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRight - panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelRight - 0.6, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
            //        guidList.Add(panel.Perimeter);
            //    }

            //}

            ////Fold direction is Up

            ////Add only Left fold is required and direction 1
            //if (panel.LeftFirstFoldDirection == 1 && panel.LeftFold == 1 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 1 && panel.LeftFold == 1 && panel.BottomFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
            //    guidList.Add(panel.Perimeter);
            //    if (panel.TopFold == 1 && panel.BottomFold != 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(panelLeft + 0.6, panelY0 + panel.LeftFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
            //        guidList.Add(panel.Perimeter);
            //    }
            //    if (panel.TopFold != 1 && panel.BottomFold == 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeft + panel.LeftFirstFoldWidth), panelY1 - panel.LeftFirstFoldSetbackTop, 0), new Point3d(panelLeft + 0.6, panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
            //        guidList.Add(panel.Perimeter);
            //    }
            //}

            ////Adds the Right Face Dimension blue  (fold direction down)
            ////Add only Left fold is required and direction 2

            //if (panel.RightFirstFoldDirection == 2 && panel.RightFold == 1 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 2 && panel.RightFold == 1 && panel.BottomFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the straight lines of the right hand side fold (bottom to top)
            //    guidList.Add(panel.Perimeter);
            //    if (panel.TopFold == 1 && panel.BottomFold != 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelRightHS + 0.6, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the right hand side fold (left to right)
            //        guidList.Add(panel.Perimeter);
            //    }
            //    if (panel.TopFold != 1 && panel.BottomFold == 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelRightHS + panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelRightHS + 0.6, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the upper line of the right hand side fold  (left to right)                                                        
            //        guidList.Add(panel.Perimeter);
            //    }

            //    rightFixingHolePosition = panelRightHS;
            //}
            ////Adds the Right Face Dimension blue  (fold direction up)
            ////Add only Left fold is required and direction 1

            //if (panel.RightFirstFoldDirection == 1 && panel.RightFold == 1 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 1 && panel.RightFold == 1 && panel.BottomFold == 1)
            //{
            //    panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the straight lines of the left hand side fold 
            //    guidList.Add(panel.Perimeter);
            //    if (panel.TopFold == 1 && panel.BottomFold != 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(panelLeftHS - 0.6, panelY0 + panel.RightFirstFoldSetbackBottom, 0)); //draws the bottom line of the left hand side fold 
            //        guidList.Add(panel.Perimeter);
            //    }
            //    if (panel.TopFold != 1 && panel.BottomFold == 1)
            //    {
            //        panel.Perimeter = doc.Objects.AddLine(new Point3d((panelLeftHS - panel.RightFirstFoldWidth), panelY1 - panel.RightFirstFoldSetbackTop, 0), new Point3d(panelLeftHS - 0.6, panelY1 - panel.RightFirstFoldSetbackTop, 0)); // draws the upper line of the left hand side fold                                                          
            //        guidList.Add(panel.Perimeter);
            //    }
            //}


            // Add the word perforated area to the panel
            if (panel.DrawPerf != 3) //Add perforated text only if draw perf is not equal to solid panel
            {
                //pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 33, 0);
                text = System.Text.RegularExpressions.Regex.Unescape(panel.PerfText);
                height = panel.labelHeight / 2;
                pt = new Rhino.Geometry.Point3d(((borderX1 + borderX0) / 2) - 117.5, ((borderY1 + borderY0) / 2) + 10 + height, 0);
                plane.Origin = pt;
                Guid perforatedAreaLabel = doc.Objects.AddText(text, plane, height, font, false, false);
                guidList.Add(perforatedAreaLabel);

                double ratio = 1;
                if (panel.X - panel.LeftBorder - panel.RightBorder < 230)
                {
                    RhinoApp.RunScript("SelNone", true);
                    labelText = doc.Objects.Find(perforatedAreaLabel);
                    labelText.Select(true);
                    bbox = labelText.Geometry.GetBoundingBox(true);

                    if (panel.Y > panel.X)
                    {
                        RhinoApp.RunScript("_-rotate " + bbox.Center.X + "," + bbox.Center.Y + " " + "90", true);
                    }
                    minX = bbox.Corner(true, true, true).X;
                    maxX = bbox.Corner(false, true, true).X;
                    minY = bbox.Corner(true, true, true).Y;
                    maxY = bbox.Corner(true, false, true).Y;

                    if (maxY - minY > panel.X - panel.LeftBorder - panel.RightBorder)
                    {
                        ratio = (panel.X - panel.LeftBorder - panel.RightBorder) / (2 * (maxY - minY));
                        if (ratio * (maxX - minX) > (panel.Y - panel.TopBorder - panel.BottomBorder))
                        {
                            ratio = ratio * (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * ratio * (maxX - minX));
                        }
                    }
                    else if (maxX - minX >= panel.Y - panel.TopBorder - panel.BottomBorder)
                    {
                        ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxX - minX));
                    }
                    labelText.Select(true);
                    RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                    BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                    double distanceX = (borderX0 + borderX1) / 2;
                    double distanceY = (borderY0 + borderY1) / 2;

                    RhinoApp.WriteLine(bbox2.Center.ToString());
                    RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
                }
                else
                {
                    RhinoApp.RunScript("SelNone", true);
                    labelText = doc.Objects.Find(perforatedAreaLabel);
                    labelText.Select(true);
                    bbox = labelText.Geometry.GetBoundingBox(true);
                    minX = bbox.Corner(true, true, true).X;
                    maxX = bbox.Corner(false, true, true).X;
                    minY = bbox.Corner(true, true, true).Y;
                    maxY = bbox.Corner(true, false, true).Y;

                    if (maxX - minX > panel.Y - panel.TopBorder - panel.BottomBorder)
                    {
                        ratio = (panel.Y - panel.TopBorder - panel.BottomBorder) / (2 * (maxY - minY));
                        labelText.Select(true);
                        RhinoApp.RunScript("_-Scale " + bbox.Center.X + "," + bbox.Center.Y + " " + ratio, true);
                    }
                    BoundingBox bbox2 = labelText.Geometry.GetBoundingBox(true);
                    double distanceX = (borderX0 + borderX1) / 2;
                    double distanceY = (borderY0 + borderY1) / 2;

                    RhinoApp.WriteLine(bbox2.Center.ToString());
                    RhinoApp.RunScript("_-Move " + bbox2.Center.X + "," + bbox2.Center.Y + ",0 " + distanceX + "," + distanceY + ",0", true);
                }
            }
            MetrixUtilities.createMetrixRealDimension(); //creates the metrix real dimension and set as the default 

            Point3d origin = new Point3d(0, 0, 0);
            Point3d offset = new Point3d(0, 0, 0);
            Point2d ext1;
            Point2d ext2;
            Point2d linePt;
            LinearDimension dimension;
            Guid dimGuid = new Guid();
            double u, v;
            //If the left first fold width is larger than the right, assign it instead of right
            double largestWidth = panel.TopFirstFoldWidth >= panel.BottomFirstFoldWidth ? panel.TopFirstFoldWidth : panel.BottomFirstFoldWidth;
            // Add horizontal dimension (top)
            if (panel.LeftFold == 1 || panel.RightFold == 1)
            {
                double startPoint = panelFirstTopY1 + 300 - 2 * panel.KFactor + Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) + 50;
                origin = new Point3d(panelX0, startPoint, 0);
                offset = new Point3d(panelX1, startPoint, 0);
                pt = new Point3d((offset.X - origin.X) / 2, startPoint, 0); //change to make the dimension blue line higher and lower

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
                startPoint = panelFirstBottomY1 - 300 + 2 * panel.KFactor - Math.Max(panel.LeftFirstFoldWidth, panel.RightFirstFoldWidth) - 50;
                origin = new Point3d(panelX0, startPoint, 0);
                offset = new Point3d(panelX1, startPoint, 0);
                pt = new Point3d((offset.X - origin.X) / 2, startPoint, 0); //change to make the dimension blue line higher and lower
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
            if (panel.LeftFoldType == 1 && panel.RightFoldType == 1 && panel.LeftFirstFoldDirection == panel.RightFirstFoldDirection ||
               panel.LeftFoldType == 2 && panel.RightFoldType == 2 && panel.LeftFirstFoldDirection == panel.RightFirstFoldDirection) //LR panels fold the same direction
            {
                if (panel.LeftFold == 1 || panel.RightFold == 1)
                {
                    //Adds the top left hand dimension
                    //Draw the dimension only if the left fold is present
                    if (panel.LeftFold == 1)
                    {

                        if (panel.LeftFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelX0 - 0.25 * (5), panelBottom, 0);
                            offset = new Point3d(panelX0 - 0.25 * (5), panel.LeftFirstFoldWidth, 0);
                            pt = new Point3d(panelX0 - (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }
                        else
                        {
                            origin = new Point3d(panelX0 - 0.25 * (8), panelTop, 0);
                            offset = new Point3d(panelX0 - 0.25 * (8), -panel.LeftFirstFoldWidth, 0);
                            pt = new Point3d(panelX0 - (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }

                        plane = Plane.WorldXY;
                        plane.XAxis = new Vector3d(0, -1, 0);
                        plane.YAxis = new Vector3d(-1, 0, 0);
                        plane.ZAxis = new Vector3d(0, 0, -1);
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.LeftFirstFoldWidth, v); //set the left fold dimension number to fold width
                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            ext2 = new Point2d(panel.LeftFirstFoldWidth, v); //set the left fold dimension number to fold width
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);
                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Add Double Fold Dimension (Second Fold)
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.LeftFirstFoldDirection == 2)
                            {

                                origin = new Point3d(panelX0, panelBottom + panel.LeftFirstFoldWidth + 50, 0);
                                offset = new Point3d(-panel.LeftSecondFoldWidth, panelBottom + panel.LeftFirstFoldWidth + 50, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottom + panel.LeftFirstFoldWidth + 50, 0); //left hand side (dimension number)
                            }
                            else
                            {
                                origin = new Point3d(panelX0, panelTop - panel.LeftFirstFoldWidth - 50, 0);
                                offset = new Point3d(-panel.LeftSecondFoldWidth, panelTop - panel.LeftFirstFoldWidth - 50, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelTop - panel.LeftFirstFoldWidth - 50, 0); //left hand side (dimension number)
                            }

                            plane = Plane.WorldXY;
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(panel.LeftSecondFoldWidth, v); //set the left fold dimension number to fold width

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);
                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);
                        }
                    }

                    //Adds the top right hand side dimension
                    if (panel.RightFold == 1) //Add only if the right fold is present
                    {
                        if (panel.RightFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelX1 + 0.25 * (5), (panelBottom), 0);
                            offset = new Point3d(panelX1 + 0.25 * (5), panel.RightFirstFoldWidth, 0);
                            pt = new Point3d(panelX1 + (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }
                        if (panel.RightFirstFoldDirection == 1)

                        {
                            origin = new Point3d(panelX1 + 0.25 * (5), panelTop, 0);
                            offset = new Point3d(panelX1 + 0.25 * (5), -panel.RightFirstFoldWidth, 0);
                            pt = new Point3d(panelX1 + (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }
                        plane = Plane.WorldXY;
                        plane.XAxis = new Vector3d(0, -1, 0);
                        plane.YAxis = new Vector3d(-1, 0, 0);
                        plane.ZAxis = new Vector3d(0, 0, -1);
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.RightFirstFoldWidth, v);  //set the Right fold dimension number to the fold width

                        if (panel.RightFirstFoldDirection == 1)
                        {
                            ext2 = new Point2d(panel.RightFirstFoldWidth, v); //set the left fold dimension number to fold width
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);
                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Adding Second Fold Dimension 
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.RightFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelX1, panelBottom + panel.RightFirstFoldWidth + 50, 0);
                                offset = new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottom + panel.RightFirstFoldWidth + 50, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottom + panel.RightFirstFoldWidth + 50, 0); //left hand side (dimension number)
                            }
                            if (panel.RightFirstFoldDirection == 1)

                            {
                                origin = new Point3d(panelX1, panelTop - panel.RightFirstFoldWidth - 50, 0);
                                offset = new Point3d(panelX1 - panel.RightSecondFoldWidth, panelTop - panel.RightFirstFoldWidth - 50, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelTop - panel.RightFirstFoldWidth - 50, 0); //left hand side (dimension number)
                            }
                            plane = Plane.WorldXY;
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(-panel.RightSecondFoldWidth, v); //set the left fold dimension number to fold width

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);
                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);
                        }
                    }
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
                    //Adds the bottom left dimension
                    if (panel.LeftFold == 1) //Add only if the left fold is present
                    {
                        if (panel.LeftFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelX0 + 0.25 * (5), panelBottomBH, 0);
                            offset = new Point3d(panelX0 + 0.25 * (5), panel.LeftFirstFoldWidth, 0);
                            pt = new Point3d(panelX0 - (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }
                        if (panel.LeftFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelX0 + 0.25 * (5), panelBottomBH - panel.LeftFirstFoldWidth, 0);
                            offset = new Point3d(panelX0 + 0.25 * (5), panel.LeftFirstFoldWidth, 0);
                            pt = new Point3d(panelX0 - (10 * 6), (offset.X - origin.X) / 2, 0); //left hand side (dimension number)
                        }

                        plane = Plane.WorldXY;
                        plane.XAxis = new Vector3d(0, -1, 0);
                        plane.YAxis = new Vector3d(-1, 0, 0);
                        plane.ZAxis = new Vector3d(0, 0, -1);
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);

                        ext2 = new Point2d(-panel.LeftFirstFoldWidth, v); //set the left fold dimension number to fold width

                        if (panel.LeftFirstFoldDirection == 2)
                        {
                            ext2 = new Point2d(-panel.LeftFirstFoldWidth, v); //set the left fold dimension number to fold width
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);
                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Adding the second dimension
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.LeftFirstFoldDirection == 1)
                            {
                                origin = new Point3d(panelX0, panelBottomBH + panel.LeftFirstFoldWidth + 30, 0);
                                offset = new Point3d(panelX0 + panel.LeftSecondFoldWidth, panelBottomBH + panel.LeftFirstFoldWidth + 30, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottomBH + panel.LeftFirstFoldWidth + 30, 0); //left hand side (dimension number)
                            }
                            if (panel.LeftFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelX0, panelBottomBH - panel.LeftFirstFoldWidth - 30, 0);
                                offset = new Point3d(panelX0 - panel.LeftSecondFoldWidth, panelBottomBH - panel.LeftFirstFoldWidth - 30, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottomBH - panel.LeftFirstFoldWidth - 30, 0); //left hand side (dimension number)
                            }

                            plane = Plane.WorldXY;
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);

                            ext2 = new Point2d(panel.LeftSecondFoldWidth, v); //set the left fold dimension number to fold width

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);
                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);
                        }
                    }
                    if (panel.RightFold == 1) //Add only if the right fold is present
                    {
                        //Adds the bottom right dimension
                        if (panel.RightFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelX1 - 0.25 * (5), panelBottomBH, 0);
                            offset = new Point3d(panelX1 - 0.25 * (5), -panel.RightFirstFoldWidth, 0);
                            pt = new Point3d(panelX1 + (10 * 6), (offset.X - origin.X) / 2, 0); //right hand side (dimension number)
                        }
                        if (panel.RightFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelX1 - 0.25 * (5), panelBottomBH, 0);
                            offset = new Point3d(panelX1 - 0.25 * (5), panel.RightFirstFoldWidth, 0);
                            pt = new Point3d(panelX1 + (10 * 6), (offset.X - origin.X) / 2, 0); //right hand side (dimension number)
                        }

                        plane = Plane.WorldXY;
                        plane.XAxis = new Vector3d(0, -1, 0);
                        plane.YAxis = new Vector3d(-1, 0, 0);
                        plane.ZAxis = new Vector3d(0, 0, -1);
                        plane.Origin = origin;
                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);

                        ext2 = new Point2d(-panel.RightFirstFoldWidth, v);  //set the Right fold dimension number to the fold width (negative)

                        if (panel.RightFirstFoldDirection == 2) //If direction is downward, assign as positive
                        {
                            ext2 = new Point2d(panel.RightFirstFoldWidth, v);  //set the Right fold dimension number to the fold width
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);

                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);

                        guidList.Add(dimGuid);

                        //Adding the Second Fold Dimension 
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.RightFirstFoldDirection == 1)
                            {
                                origin = new Point3d(panelX1, panelBottomBH + panel.RightFirstFoldWidth + 30, 0);
                                offset = new Point3d(panelX1 - panel.RightSecondFoldWidth, panelBottomBH + panel.RightFirstFoldWidth + 30, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottomBH + panel.RightFirstFoldWidth + 30, 0); //right hand side (dimension number)
                            }
                            if (panel.RightFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelX1, panelBottomBH - panel.RightFirstFoldWidth - 30, 0);
                                offset = new Point3d(panelX1 + panel.RightSecondFoldWidth, panelBottomBH - panel.RightFirstFoldWidth - 30, 0);
                                pt = new Point3d((offset.X - origin.X) / 2, panelBottomBH - panel.RightFirstFoldWidth - 30, 0); //right hand side (dimension number)
                            }

                            plane = Plane.WorldXY;
                            plane.Origin = origin;
                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);

                            ext2 = new Point2d(-panel.RightSecondFoldWidth, v);  //set the Right fold dimension number to the fold width (negative)

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);

                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);

                            guidList.Add(dimGuid);

                        }
                    }
                    //Adds the thickness dimension (bottom)

                    if (panel.LeftFirstFoldDirection == 2 || panel.RightFirstFoldDirection == 2) // Fold down
                    {
                        origin = new Point3d(panelX1 - 50, (panelBottomBH - panel.SheetThickness), 0);
                        offset = new Point3d(panelX1 - 50, panelBottomBH, 0);
                        pt = new Point3d(panelX1 - 50, panelBottomBH, 0);
                    }

                    if (panel.LeftFirstFoldDirection == 1 || panel.RightFirstFoldDirection == 1) //Fold up
                    {
                        origin = new Point3d(panelX1 - 50, (panelBottomBH + panel.SheetThickness), 0);
                        offset = new Point3d(panelX1 - 50, panelBottomBH, 0);
                        pt = new Point3d(panelX1 - 50, panelBottomBH, 0);
                    }

                    plane = Plane.WorldXY;
                    plane.XAxis = new Vector3d(0, -1, 0);
                    plane.YAxis = new Vector3d(-1, 0, 0);
                    plane.ZAxis = new Vector3d(0, 0, -1);

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = panel.SheetThickness.ToString("0.0");
                    dimGuid = doc.Objects.AddLinearDimension(dimension);

                    guidList.Add(dimGuid);


                    //Adds the thickness dimension top
                    if (panel.LeftFirstFoldDirection == 2 || panel.RightFirstFoldDirection == 2) //fold down  
                    {
                        origin = new Point3d(panelX1 - 50, panelBottom, 0);
                        offset = new Point3d(panelX1 - 50, panelBottom + panel.SheetThickness, 0);
                        pt = new Point3d(panelX1 - 50, panelTop, 0);
                    }

                    if (panel.LeftFirstFoldDirection == 1 || panel.RightFirstFoldDirection == 1) //fold Up
                    {
                        //positioning the sheet thickness
                        origin = new Point3d(panelX1 - 50, panelTop, 0);
                        offset = new Point3d(panelX1 - 50, panelTop - panel.SheetThickness, 0);
                        pt = new Point3d(panelX1 - 50, panelTop, 0);
                    }

                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = panel.SheetThickness.ToString("0.0");
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
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH + 2 * (50), 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, panelBottomBH + 2 * (50), 0)); //starting point of the arrow (with text)
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
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelTop - panel.SheetThickness, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelTop - 45, 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, panelTop - 45, 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }
                if (panel.BottomFirstFoldDirection == 1 && panel.LeftFold == 1 || panel.BottomFirstFoldDirection == 1 && panel.RightFold == 1)
                {
                    //Add Face SIDE label (Horizontally)   Bottom     
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH + panel.SheetThickness, 0));  //reduce by sheet thickness to point the arrow head correctly
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2, panelBottomBH + 45, 0));   //end point of arrow
                    leaderPoints.Add(new Point3d((panelX1 + panelX0) / 2 + (panelX0 - panelX1) / 8, panelBottomBH + 45, 0)); //starting point of the arrow (with text)
                    text = "FACE SIDE";

                    burrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(burrLeader);
                    leaderPoints.Clear();
                }

            }

            //Adds the top text (CC)
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelX0 - 15, panelY1 + 80, 0));  //reduce by sheet thickness to point the arrow head correctly
            leaderPoints.Add(new Point3d(panelX0 - 15, panelY1 + 80, 0));   //end point of arrow
            leaderPoints.Add(new Point3d(panelX0 - 15.1, panelY1 + 100, 0)); //starting point of the arrow (with text)
            text = topText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            ////Adds the Bottom text (DD)
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelX0 - 55, panelY0 - 130, 0));  //reduce by sheet thickness to point the arrow head correctly
            leaderPoints.Add(new Point3d(panelX0 - 55, panelY0 - 130, 0));   //end point of arrow
            leaderPoints.Add(new Point3d(panelX0 - 55.1, panelY0 - 160, 0)); //starting point of the arrow (with text)
            text = bottomText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            /***
             * This section adds the Vertical dimensions on a panel
             * */

            //If the left first fold width is larger than the right, assign it instead of right
            largestWidth = panel.TopFirstFoldWidth >= panel.BottomFirstFoldWidth ? panel.TopFirstFoldWidth : panel.BottomFirstFoldWidth;


            // Add vertical dimension for panel
            //left vertical dimension and right vertical dimension (blue dimension - long blue vertical dimension)
            if (panel.TopFold == 1 || panel.BottomFold == 1)
            {
                double startPoint = panelFirstLeftX1 - 300 + 2 * panel.KFactor - Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) - 50;
                origin = new Point3d(startPoint, panelY0, 0); //0.25
                offset = new Point3d(startPoint, panelY1, 0); //0.25
                pt = new Point3d(startPoint, (offset.Y - origin.Y) / 2, 0);

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
                //Adding the Right Verical Dimension
                startPoint = panelFirstRightX1 + 300 - 2 * panel.KFactor + Math.Max(panel.TopFirstFoldWidth, panel.BottomFirstFoldWidth) + 50;
                origin = new Point3d(startPoint, panelY0, 0); //0.25
                offset = new Point3d(startPoint, panelY1, 0); //0.25
                pt = new Point3d(startPoint, (offset.Y - origin.Y) / 2, 0);

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
               && panel.BottomFold == 1 || panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == panel.BottomFirstFoldDirection
               && panel.TopFold == 1 || panel.TopFoldType == 2 && panel.BottomFoldType == 2 && panel.TopFirstFoldDirection == panel.BottomFirstFoldDirection
               && panel.BottomFold == 1) //TB panels fold the same direction
            {

                if (panel.TopFold == 1 || panel.BottomFold == 1)
                {
                    //Left Hand side Bottom Fold Dimension
                    if (panel.BottomFold == 1) //Add only if bottom fold is present
                    {
                        
                        if (panel.BottomFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelRight, panelY0 + 0.25 * (5), 0);
                            offset = new Point3d(-panel.BottomFirstFoldWidth, panelY0 + 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - (10 * 6), 0);
                        }
                        if (panel.BottomFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelFirstLeftX1 - 300 + 2 * panel.KFactor , panelY0 + 0.25 * (5), 0);
                            offset = new Point3d(panel.BottomFirstFoldWidth, panelY0 + 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - (10 * 6), 0);
                        }

                        plane = Plane.WorldXY;
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.BottomFirstFoldWidth, v);
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);
                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);
                        //Adding the Second Fold Dimension
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.BottomFirstFoldDirection == 2) //Direction down
                            {
                                origin = new Point3d(panelRight - panel.BottomFirstFoldWidth - 20, panelY0, 0);
                                offset = new Point3d(panelRight - panel.BottomFirstFoldWidth - 20, panelY0 + panel.BottomSecondFoldWidth, 0);
                                pt = new Point3d(panelRight - panel.BottomFirstFoldWidth - 20, (offset.Y - origin.Y) / 2, 0);
                            }
                            if (panel.BottomFirstFoldDirection == 1) // Direction up
                            {
                                origin = new Point3d(panelRight - panel.SheetThickness + panel.BottomFirstFoldWidth + 5, panelY0, 0);
                                offset = new Point3d(panelRight - panel.SheetThickness + panel.BottomFirstFoldWidth + 5, panelY0 + panel.BottomSecondFoldWidth, 0);
                                pt = new Point3d(panelRight - panel.SheetThickness + panel.BottomFirstFoldWidth + 5, (offset.Y - origin.Y) / 2, 0);
                            }
                            plane = Plane.WorldXY; //Rotate the text
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(-panel.BottomSecondFoldWidth, v);

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);
                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);
                        }
                    }
                    //Left hand side Top  fold dimension
                    if (panel.TopFold == 1)
                    {
                        if (panel.TopFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelRight, panelY1 - 0.25 * (5), 0);
                            offset = new Point3d(-panel.TopFirstFoldWidth, panelY1 - 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + (10 * 6), 0);
                        }
                        if (panel.TopFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelFirstLeftX1 - 300 + 2 * panel.KFactor, panelY1 - 0.25 * (5), 0);
                            offset = new Point3d(panel.TopFirstFoldWidth, panelY1 - 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + (10 * 6), 0);
                        }
                        plane = Plane.WorldXY;
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.TopFirstFoldWidth, v);

                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);

                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Adding Second Fold Dimension
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.TopFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelRight + panel.SheetThickness - panel.TopFirstFoldWidth - 20, panelY1, 0);
                                offset = new Point3d(panelRight + panel.SheetThickness - panel.TopFirstFoldWidth - 20, panelY1 + panel.TopSecondFoldWidth, 0);
                                pt = new Point3d(panelRight + panel.SheetThickness - panel.TopFirstFoldWidth - 20, (offset.Y - origin.Y) / 2, 0);
                            }
                            if (panel.TopFirstFoldDirection == 1)
                            {
                                origin = new Point3d(panelRight - panel.SheetThickness + panel.TopFirstFoldWidth + 5, panelY1, 0);
                                offset = new Point3d(panelRight - panel.SheetThickness + panel.TopFirstFoldWidth + 5, panelY1 - panel.TopSecondFoldWidth, 0);
                                pt = new Point3d(panelRight - panel.SheetThickness + panel.TopFirstFoldWidth + 5, (offset.Y - origin.Y) / 2, 0);
                            }
                            plane = Plane.WorldXY; //Rotate the text
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(panel.TopSecondFoldWidth, v);

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);

                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);

                        }
                    }
                    //Add panel thickness dimension -left hand side (between the sheet) 
                    if (panel.TopFirstFoldDirection == 1) //fold up
                    {
                        origin = new Point3d(panelLeft, panelY0 + 50, 0);
                        offset = new Point3d(panelLeft + panel.SheetThickness, panelY0 + 50, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 + 50, 0);
                    }

                    if (panel.TopFirstFoldDirection == 2) //Fold down
                    {
                        origin = new Point3d(panelRight - panel.SheetThickness, panelY0 + 50, 0);
                        offset = new Point3d(panelRight, panelY0 + 50, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 + 50, 0);
                    }
                    plane = Plane.WorldXY;
                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = panel.SheetThickness.ToString("0.0");
                    dimGuid = doc.Objects.AddLinearDimension(dimension);
                    guidList.Add(dimGuid);
                }

                //add Right hand Fold Dimension
                if (panel.TopFold == 1 || panel.BottomFold == 1)
                {
                    //Right hand side bottom
                    if (panel.BottomFold == 1) //Add only if bottom fold is present
                    {
                        if (panel.BottomFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelLeftHS, panelY0 + 0.25 * (5), 0);
                            offset = new Point3d(panel.BottomFirstFoldWidth, panelY0 + 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - (10 * 6), 0);
                        }

                        if (panel.BottomFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelRightHS, panelY0 + 0.25 * (5), 0);
                            offset = new Point3d(panel.BottomFirstFoldWidth, panelY0 + 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 - (10 * 6), 0);
                        }
                        plane = Plane.WorldXY;
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.BottomFirstFoldWidth, v);
                        if (panel.RightFirstFoldDirection == 2)
                        {
                            ext2 = new Point2d(panel.BottomFirstFoldWidth, v);
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);
                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Adding the second fold dimension 
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.BottomFirstFoldDirection == 1)
                            {
                                origin = new Point3d(panelLeftHS - panel.BottomFirstFoldWidth - 60, panelY0, 0);
                                offset = new Point3d(panelLeftHS - panel.BottomFirstFoldWidth - 60, panelY0 + panel.BottomSecondFoldWidth, 0);
                                pt = new Point3d(panelLeftHS - panel.BottomFirstFoldWidth - 60, (offset.Y - origin.Y) / 2, 0);
                            }

                            if (panel.BottomFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelRightHS + panel.BottomFirstFoldWidth + 60, panelY0, 0);
                                offset = new Point3d(panelRightHS + panel.BottomFirstFoldWidth + 60, panelY0 - panel.BottomSecondFoldWidth, 0);
                                pt = new Point3d(panelRightHS + panel.BottomFirstFoldWidth + 60, (offset.Y - origin.Y) / 2, 0);
                            }

                            plane = Plane.WorldXY; //Rotate the text
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(-panel.BottomSecondFoldWidth, v);

                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);
                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);

                        }
                    }
                    //Adds the Right hand side fold dimension (blue face)
                    //Right hand side Top
                    if (panel.TopFold == 1) //Add only if top fold is present
                    {
                        if (panel.TopFirstFoldDirection == 1)
                        {
                            origin = new Point3d(panelLeftHS, panelY1 - 0.25 * (5), 0);
                            offset = new Point3d(panel.TopFirstFoldWidth, panelY1 - 0.25 * (5), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + (10 * 6), 0);
                        }
                        if (panel.TopFirstFoldDirection == 2)
                        {
                            origin = new Point3d(panelRightHS, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                            offset = new Point3d(panel.TopFirstFoldWidth, panelY1 - 0.25 * (panel.TopFirstFoldWidth), 0);
                            pt = new Point3d((offset.Y - origin.Y) / 2, panelY1 + 1 * (panel.TopFirstFoldWidth), 0);
                        }

                        plane = Plane.WorldXY;
                        plane.Origin = origin;

                        plane.ClosestParameter(origin, out u, out v);
                        ext1 = new Point2d(u, v);

                        plane.ClosestParameter(offset, out u, out v);
                        ext2 = new Point2d(-panel.TopFirstFoldWidth, v);

                        if (panel.RightFirstFoldDirection == 2)
                        {
                            ext2 = new Point2d(panel.TopFirstFoldWidth, v);
                        }
                        plane.ClosestParameter(pt, out u, out v);
                        linePt = new Point2d(u, v);

                        dimension = new LinearDimension(plane, ext1, ext2, linePt);
                        dimGuid = doc.Objects.AddLinearDimension(dimension);
                        guidList.Add(dimGuid);

                        //Adding the Second Fold Dimension
                        if (panel.PanelType.Equals("Double Folded"))
                        {
                            if (panel.TopFirstFoldDirection == 1)
                            {
                                origin = new Point3d(panelLeftHS - panel.TopFirstFoldWidth - 60, panelY1, 0);
                                offset = new Point3d(panelLeftHS - panel.TopFirstFoldWidth - 60, panelY1 - panel.TopSecondFoldWidth, 0);
                                pt = new Point3d(panelLeftHS - panel.TopFirstFoldWidth - 60, (offset.Y - origin.Y) / 2, 0);
                            }
                            if (panel.TopFirstFoldDirection == 2)
                            {
                                origin = new Point3d(panelRightHS + panel.TopFirstFoldWidth + 60, panelY1, 0);
                                offset = new Point3d(panelRightHS + panel.TopFirstFoldWidth + 60, panelY1 + panel.TopSecondFoldWidth, 0);
                                pt = new Point3d(panelRightHS + panel.TopFirstFoldWidth + 60, (offset.Y - origin.Y) / 2, 0);
                            }

                            plane = Plane.WorldXY; //Rotate the text
                            plane.XAxis = new Vector3d(0, -1, 0);
                            plane.YAxis = new Vector3d(-1, 0, 0);
                            plane.ZAxis = new Vector3d(0, 0, -1);
                            plane.Origin = origin;

                            plane.ClosestParameter(origin, out u, out v);
                            ext1 = new Point2d(u, v);

                            plane.ClosestParameter(offset, out u, out v);
                            ext2 = new Point2d(panel.TopSecondFoldWidth, v);
                            plane.ClosestParameter(pt, out u, out v);
                            linePt = new Point2d(u, v);

                            dimension = new LinearDimension(plane, ext1, ext2, linePt);
                            dimGuid = doc.Objects.AddLinearDimension(dimension);
                            guidList.Add(dimGuid);

                        }
                    }
                    //add right dimension thickness
                    //Add panel thickness dimension - right hand side
                    if (panel.TopFirstFoldDirection == 1 || panel.BottomFirstFoldDirection == 1) //fold up
                    {
                        origin = new Point3d(panelLeftHS, panelY0 + 50, 0);
                        offset = new Point3d(panelLeftHS - panel.SheetThickness, panelY0 + 50, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 + 50, 0);
                    }

                    if (panel.TopFirstFoldDirection == 2 || panel.BottomFirstFoldDirection == 2) //fold down
                    {
                        origin = new Point3d(panelRightHS + panel.SheetThickness, panelY0 + 50, 0);
                        offset = new Point3d(panelRightHS, panelY0 + 50, 0);
                        pt = new Point3d((offset.Y - origin.Y) / 2, panelY0 + 50, 0);
                    }

                    plane = Plane.WorldXY;
                    plane.ClosestParameter(origin, out u, out v);
                    ext1 = new Point2d(u, v);

                    plane.ClosestParameter(offset, out u, out v);
                    ext2 = new Point2d(u, v);

                    plane.ClosestParameter(pt, out u, out v);
                    linePt = new Point2d(u, v);

                    dimension = new LinearDimension(plane, ext1, ext2, linePt);
                    dimension.Text = panel.SheetThickness.ToString("0.0");
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
                dimension.Text = panel.SheetThickness.ToString("0.0");
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
                dimension.Text = panel.SheetThickness.ToString("0.0");
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
            //For direction down
            if (panel.SideRequired.Equals("Face Side") || panel.SideRequired.Equals("Both Sides")) //add the burr side labels only if the user wants to
            {
                text = "FACE SIDE";
                if (panel.LeftFirstFoldDirection == 2 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 2 && panel.BottomFold == 1)
                {
                    //Add Face SIDE label (Verically) - Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelRight, (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d(panelRight + 30, (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d(panelRight + 30, (panelY1 + panelY0) / 2.1, 0));
                    verticalFaceBurrLeader = new Guid();
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);


                }
                if (panel.RightFirstFoldDirection == 2 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 2 && panel.BottomFold == 1)
                {
                    //Add Face side label - Right
                    leaderPoints.Clear();
                    verticalFaceBurrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelRightHS, (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d((panelRightHS - 50), (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d((panelRightHS - 50), (panelY1 + panelY0) / 2.1, 0));
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);
                }
                //For direction Up
                if (panel.LeftFirstFoldDirection == 1 && panel.TopFold == 1 || panel.LeftFirstFoldDirection == 1 && panel.BottomFold == 1)
                {
                    double tempVal = (panelY1 + panelY0) / 2;
                    //Add Face SIDE label (Verically) - Left
                    leaderPoints.Clear();
                    leaderPoints.Add(new Point3d(panelLeft + panel.SheetThickness, (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d(panelLeft + 60, tempVal, 0));
                    leaderPoints.Add(new Point3d(panelLeft + 60, tempVal + 3, 0));
                    verticalFaceBurrLeader = new Guid();
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);

                }

                if (panel.RightFirstFoldDirection == 1 && panel.TopFold == 1 || panel.RightFirstFoldDirection == 1 && panel.BottomFold == 1)
                {
                    double tempVal = (panelY1 + panelY0) / 2;
                    //Add Face side label - Right
                    leaderPoints.Clear();
                    verticalFaceBurrLeader = new Guid();
                    leaderPoints.Add(new Point3d(panelLeftHS - panel.SheetThickness, (panelY1 + panelY0) / 2, 0));
                    leaderPoints.Add(new Point3d(panelLeftHS - 50, tempVal, 0));
                    leaderPoints.Add(new Point3d(panelLeftHS - 50, tempVal + 3, 0));
                    verticalFaceBurrLeader = doc.Objects.AddLeader(text, leaderPoints);
                    guidList.Add(verticalFaceBurrLeader);
                }
            }



            //Adding the AA text
            leaderPoints.Clear();
            leaderPoints.Add(new Point3d(panelX0 - panel.LeftFirstFoldWidth - 80, panelY0 - 50, 0));  //place of the arrow head
            leaderPoints.Add(new Point3d(panelX0 - panel.LeftFirstFoldWidth - 80, panelY0 - 50, 0));  //the horizontal line draw to the arrow head
            leaderPoints.Add(new Point3d(panelX0 - panel.LeftFirstFoldWidth - 140, panelY0 - 50, 0)); //place of the text 
            text = leftText;
            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            guidList.Add(panel.Perimeter);

            //Adding the BB text
            leaderPoints.Clear();
            panelRightHS = panelFirstRightX1 + (panelFirstRightX1 - panelFirstRightX0);
            leaderPoints.Add(new Point3d(panelRightHS , panelY0 - 15, 0));  //place of the arrow head
            leaderPoints.Add(new Point3d(panelRightHS , panelY0 - 15, 0));  //the horizontal line draw to the arrow head
            leaderPoints.Add(new Point3d(panelRightHS + 50, panelY0 - 15, 0)); //place of the text 
            text = rightText;

            burrLeader = doc.Objects.AddLeader(text, leaderPoints);
            guidList.Add(burrLeader);
            leaderPoints.Clear();

            //End of Blue dimensions


            // Create a new layer called DIMENSIONS BLACK
            layerName = "DIMENSIONS BLACK";
            layerIndex = createSubLayers.createSubLayer(layerName,
                    System.Drawing.Color.Black, parent_layer_Approval); //pass to the method, make Approval layer the parent layer

            doc.Layers.SetCurrentLayerIndex(layerIndex, true);

            //multiply kfactor by 2 to get the correct dimension
            if (panel.PanelType.Equals("Single Folded"))
            {
                panelFirstLeftX1 = (panelX0 + panel.KFactor * 2) - (panel.LeftFirstFoldWidth);
                panelFirstRightX1 = (panelX1 - panel.KFactor * 2) + (panel.RightFirstFoldWidth);    
            }
            if (panel.PanelType.Equals("Double Folded"))
            {
                panelFirstLeftX1 = (panelX0 + panel.KFactor * 3) - (panel.LeftSecondFoldWidth + panel.LeftFirstFoldWidth);
                panelFirstRightX1 = (panelX1 - panel.KFactor * 3) + (panel.RightSecondFoldWidth + panel.RightFirstFoldWidth);
            }
            // Add horizontal dimension including fold (from left fold to right fold)
            if (panel.LeftFirstFoldWidth + panel.RightFirstFoldWidth > 0)
            {
                if (panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    origin = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                    offset = new Point3d(panelFirstRightX1, panelFirstBottomY1, 0);
                    pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 100 + 2 * panel.KFactor, 0);

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
                else if(panel.RightFold == 1) // only right
                {
                    origin = new Point3d(panelX0, panelFirstBottomY1, 0);
                    offset = new Point3d(panelFirstRightX1, panelFirstBottomY1, 0);
                    pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 100 + 2 * panel.KFactor, 0);

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
                else if(panel.LeftFold == 1 && panel.RightFold != 1)// left
                {
                    origin = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                    offset = new Point3d(panelX1, panelFirstBottomY1, 0);
                    pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 100 + 2 * panel.KFactor, 0);

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

            //Comment this out
            ////adds a black dimension to show the width of the panel (from right perimeter to left)
            if (panel.LeftFold == 0 && panel.RightFold == 0) // bottom width 
            {
                origin = new Point3d((panelX0), panelFirstBottomY1 - 200, 0);
                offset = new Point3d(panelX1, panelFirstBottomY1 - 200, 0);
                pt = new Point3d((offset.X - origin.X) / 2, panelFirstBottomY1 - 100 + 2 * panel.KFactor, 0);

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
            /**
             * This section adds vertical dimension for the top and bottom folds
             * */
            //Find the largest width
            largestWidth = panel.TopFirstFoldWidth >= panel.BottomFirstFoldWidth ? panel.TopFirstFoldWidth : panel.BottomFirstFoldWidth;

            // Add vertical dimension including fold (top fold to bottom fold)
       //     if (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth > 0 && panel.TopFold == 1 && panel.BottomFold == 1 ||
     //         panel.TopSecondFoldWidth + panel.BottomSecondFoldWidth > 0 && panel.TopFold == 1 && panel.BottomFold == 1)
            if (panel.TopFirstFoldWidth + panel.BottomFirstFoldWidth > 0 )
            {
                if (panel.PanelType.Equals("Single Folded"))
                {
                    panelFirstTopY1 = ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor;
                    panelFirstBottomY1 = ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor;
                }
                if (panel.PanelType.Equals("Double Folded")) //if double folded show dimension from the second fold 
                {
                    panelFirstTopY1 = ((panelY1 - (panel.KFactor * 3)) + panel.TopSecondFoldWidth + panel.TopFirstFoldWidth);
                    panelFirstBottomY1 = ((panelY0 + (panel.KFactor * 3)) - (panel.BottomSecondFoldWidth + panel.BottomFirstFoldWidth));
                }

                if(panel.TopFold == 1 && panel.BottomFold == 1)
                {
                    origin = new Point3d(panelFirstLeftX1, panelFirstTopY1, 0);
                    offset = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                    // pt = new Point3d(panelFirstLeftX1 - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                    pt = new Point3d(panelX0 - panel.LeftFirstFoldWidth - 100 + 2 * panel.KFactor, (offset.Y - origin.Y) / 2, 0);
                    //Original formula with second fold
                    //pt = new Point3d(panelX0 - panel.TopSecondFoldWidth - panel.TopFirstFoldWidth - 100 + 2 * panel.KFactor, (offset.Y - origin.Y) / 2, 0);
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
                else if(panel.TopFold == 1)
                {
                    origin = new Point3d(panelFirstLeftX1, panelFirstTopY1, 0);
                    offset = new Point3d(panelFirstLeftX1, panelY0, 0);
                    // pt = new Point3d(panelFirstLeftX1 - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                    pt = new Point3d(panelX0 - panel.LeftFirstFoldWidth - 100 + 2 * panel.KFactor, (offset.Y - origin.Y) / 2, 0);
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
                else if(panel.BottomFold == 1)
                {
                    origin = new Point3d(panelFirstLeftX1, panelY1, 0);
                    offset = new Point3d(panelFirstLeftX1, panelFirstBottomY1, 0);
                    // pt = new Point3d(panelFirstLeftX1 - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                    pt = new Point3d(panelX0 - panel.LeftFirstFoldWidth - 100 + 2 * panel.KFactor, (offset.Y - origin.Y) / 2, 0);
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
      
            }

            if(panel.TopFold != 1 && panel.BottomFold != 1)
            {
                origin = new Point3d(panelFirstLeftX1, panelY1, 0);
                offset = new Point3d(panelFirstLeftX1, panelY0, 0);
                // pt = new Point3d(panelFirstLeftX1 - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                pt = new Point3d(panelX0 - panel.LeftFirstFoldWidth - 100 + 2 * panel.KFactor, (offset.Y - origin.Y) / 2, 0);
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

            /*if (panel.TopFirstFoldWidth > 0 && panel.TopFold == 1 && panel.BottomFold != 1 && panel.LeftFold != 1 && panel.RightFold != 1)
            {
                if (panel.PanelType.Equals("Single Folded"))
                {
                    panelFirstTopY1 = ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor;
                }

                origin = new Point3d(panelFirstLeftX1, panelFirstTopY1, 0);
                offset = new Point3d(panelFirstLeftX1, panelY0, 0);
                // pt = new Point3d(panelFirstLeftX1 - 7 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);
                pt = new Point3d(panelX0 - panel.TopSecondFoldWidth - panel.TopFirstFoldWidth - 60, (offset.Y - origin.Y) / 2, 0);
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
            }*/
            // < -Comment this out as per requirement-- >
            ////Add left panel dimension (black panel dimension  the fold)
            //if (panel.LeftFold == 1 && panel.LeftFirstFoldSetbackTop > 0 || panel.LeftFold == 1 && panel.LeftFirstFoldSetbackBottom > 0) //add only if left fold is present
            //{
            //   origin = new Point3d(panelFirstLeftX1 - 3 * (panelFirstLeftX0 - panelFirstLeftX1), panelFirstLeftY0, 0);
            //   offset = new Point3d(panelFirstLeftX1 - 3 * (panelFirstLeftX0 - panelFirstLeftX1), panelFirstLeftY1, 0);
            //   pt = new Point3d(panelFirstLeftX1 - 5 * (panelFirstLeftX0 - panelFirstLeftX1), (offset.Y - origin.Y) / 2, 0);

            //   plane = Plane.WorldXY;
            //   plane.XAxis = new Vector3d(0, -1, 0);
            //   plane.YAxis = new Vector3d(-1, 0, 0);
            //   plane.ZAxis = new Vector3d(0, 0, -1);
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);
            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);
            //   dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   dimGuid = doc.Objects.AddLinearDimension(dimension);

            //   guidList.Add(dimGuid);
            //}
            //
            ////adds the left fold dimension (not the fold) Black dimension
            //{
            //   origin = new Point3d(panelFirstLeftX1 - 70, panelY0, 0);
            //   offset = new Point3d(panelFirstLeftX1 - 70, panelY1, 0);
            //   pt = new Point3d(panelFirstLeftX1 - 70, (offset.Y - origin.Y) / 2, 0);

            //   plane = Plane.WorldXY;
            //   plane.XAxis = new Vector3d(0, -1, 0);
            //   plane.YAxis = new Vector3d(-1, 0, 0);
            //   plane.ZAxis = new Vector3d(0, 0, -1);
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);

            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);
            //   dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   dimGuid = doc.Objects.AddLinearDimension(dimension);
            //   guidList.Add(dimGuid);
            //}


            // Add right dimension (Top / Bottom Fold dimension in the right)
            //if (panel.RightFold == 1) //Add only if fold is present
            //{
            //   origin = new Point3d(panelFirstRightX1 + 2 * (panelFirstRightX1 - panelFirstRightX0), panelFirstRightY0, 0);
            //   offset = new Point3d(panelFirstRightX1 + 2 * (panelFirstRightX1 - panelFirstRightX0), panelFirstRightY1, 0);
            //   pt = new Point3d(panelFirstRightX1 + 4 * (panelFirstRightX1 - panelFirstRightX0), (offset.Y - origin.Y) / 2, 0);

            //   plane = Plane.WorldXY;
            //   plane.XAxis = new Vector3d(0, -1, 0);
            //   plane.YAxis = new Vector3d(-1, 0, 0);
            //   plane.ZAxis = new Vector3d(0, 0, -1);
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);
            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);
            //   dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   dimGuid = doc.Objects.AddLinearDimension(dimension);
            //   guidList.Add(dimGuid);
            //}
            //adds the panel height black dimension (right hand side)
            //{
            //   origin = new Point3d(panelFirstRightX1 + 3 * (panelFirstRightX1 - panelFirstRightX0), panelY0, 0);
            //   offset = new Point3d(panelFirstRightX1 + 3 * (panelFirstRightX1 - panelFirstRightX0), panelY1, 0);
            //   pt = new Point3d(panelFirstRightX1 + 5 * (panelFirstRightX1 - panelFirstRightX0), (offset.Y - origin.Y) / 2, 0);

            //   plane = Plane.WorldXY;
            //   plane.XAxis = new Vector3d(0, -1, 0);
            //   plane.YAxis = new Vector3d(-1, 0, 0);
            //   plane.ZAxis = new Vector3d(0, 0, -1);
            //   plane.Origin = origin;

            //   plane.ClosestParameter(origin, out u, out v);
            //   ext1 = new Point2d(u, v);

            //   plane.ClosestParameter(offset, out u, out v);
            //   ext2 = new Point2d(u, v);
            //   plane.ClosestParameter(pt, out u, out v);
            //   linePt = new Point2d(u, v);
            //   dimension = new LinearDimension(plane, ext1, ext2, linePt);
            //   dimGuid = doc.Objects.AddLinearDimension(dimension);
            //   guidList.Add(dimGuid);
            //}

            MetrixUtilities.createMetrixBordersDimension();

            // Draw Border dimension on BORDERS layer
            layerName = "BORDERS";
            layerIndex = doc.Layers.Find(layerName, true);
            doc.Layers.SetCurrentLayerIndex(layerIndex, true);
            if (panel.DrawPerf != 3) //Add border dimensions if draw perf is not solid panel
            {
                // Add horizontal borders dimension
                origin = new Point3d(panelX1, (panelY0 + panelY1) / 2, 0);
                offset = new Point3d(borderX1, (panelY0 + panelY1) / 2, 0);
                pt = new Point3d((offset.X - origin.X) / 2, (borderY0 + borderY1) / 2, 0);

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
                origin = new Point3d(panelX0, (panelY0 + panelY1) / 2, 0);
                offset = new Point3d(borderX0, (panelY0 + panelY1) / 2 , 0);
                pt = new Point3d((offset.X - origin.X) / 2, (borderY0 + borderY1) / 2, 0);


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

                origin = new Point3d((panelX0 + panelX1) / 2, panelY0, 0);
                offset = new Point3d((panelX0 + panelX1) / 2 , borderY0, 0);
                pt = new Point3d((borderX0 + borderX1) / 2, (offset.Y - origin.Y) / 2, 0);

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

                origin = new Point3d((panelX0 + panelX1) / 2, panelY1, 0);
                offset = new Point3d((panelX0 + panelX1) / 2, borderY1, 0);
                pt = new Point3d((borderX0 + borderX1) / 2, (offset.Y - origin.Y) / 2, 0);

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
            // 50mm Dimension offset
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


            //RhinoObject panelPerimeterObj = doc.Objects.Find(panel.Perimeter);

            //// Select all objects on Perforation Layer
            //Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);

            //double tolerance = Properties.Settings.Default.Tolerance;

            //Rhino.Geometry.Curve panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

            //// If  perforation layer is missing 
            //if (rhinoObjs == null)
            //{
            //   // Select all objects on tool hit Layer
            //   rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);
            //}

            //if (Convert.ToBoolean(panel.DrawPerf) == true && rhinoObjs != null)
            //{
            //   foreach (RhinoObject rhinoObj in rhinoObjs)
            //   {
            //      Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

            //      if (testCurve != null)
            //      {
            //         if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
            //         {
            //            guidList.Add(rhinoObj.Id);
            //         }
            //      }
            //   }
            //}

            // Export the panel

            //doc.Objects.UnselectAll();

            //doc.Objects.Select(panel.Perimeter);

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

            //// Select all objects on DOT SCRIBE LABEL Layer
            //rhinoObjs = doc.Objects.FindByLayer("DOT SCRIBE LABEL");

            //tolerance = Properties.Settings.Default.Tolerance;
            //panelPerimeterCurve = panelPerimeterObj.Geometry as Rhino.Geometry.Curve;

            //if (rhinoObjs != null)
            //{
            //   foreach (RhinoObject rhinoObj in rhinoObjs)
            //   {
            //      Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

            //      if (testCurve != null)
            //      {
            //         if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
            //         {
            //            rhinoObj.Select(true);
            //         }
            //      }
            //   }
            //}

            ////Select all objects on Tool Hit
            //rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.ToolHitLayerName);

            //if (rhinoObjs == null)
            //{
            //   rhinoObjs = doc.Objects.FindByLayer(Properties.Settings.Default.PerforationLayerName);
            //}

            //if (rhinoObjs != null)
            //{
            //   foreach (RhinoObject rhinoObj in rhinoObjs)
            //   {
            //      Rhino.Geometry.Curve testCurve = rhinoObj.Geometry as Rhino.Geometry.Curve;

            //      if (testCurve != null)
            //      {
            //         if (Curve.PlanarClosedCurveRelationship(panelPerimeterCurve, testCurve, Plane.WorldXY, tolerance) == RegionContainment.BInsideA)
            //         {
            //            rhinoObj.Select(true);
            //         }
            //      }
            //   }
            //}
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
            int defaultLayerIndex = doc.Layers.Find("FOLDS", true);

            doc.Layers.SetCurrentLayerIndex(defaultLayerIndex, true);

            //Start coding double folds from here

            //Draw the dashed line to separate single and double fold (dashed line represents the single fold) 

            //For all sides have folds 
            if (panel.TopFoldType == 2 || panel.BottomFoldType == 2 || panel.LeftFoldType == 2 || panel.RightFoldType == 2)
            {
                if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    //Draw top horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Bottom horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor, 0)); //horizontal line
                    guidList.Add(panel.Perimeter);
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY0 + panel.RightFirstFoldSetbackBottom, 0), new Point3d(((panelX1 - panel.KFactor * 2) + panel.RightFirstFoldWidth) - panel.KFactor, panelY1 - panel.RightFirstFoldSetbackTop, 0)); //vertical
                    guidList.Add(panel.Perimeter);
                    //Draw Left Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY0 + panel.LeftFirstFoldSetbackBottom, 0), new Point3d(((panelX0 + panel.KFactor * 2) - panel.LeftFirstFoldWidth) + (panel.KFactor), panelY1 - panel.LeftFirstFoldSetbackTop, 0)); // draws the straight lines (vertical) of the left hand side fold (bottom to top)
                    guidList.Add(panel.Perimeter);
                }

                // For Top, right and Left
                if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    //Draw top horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor * 2, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor * 2, 0));
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor * 2, panelY0, 0), new Point3d(panelX1 - panel.KFactor * 2, panelY1 - panel.KFactor, 0));
                    //Draw Left Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor * 2, panelY0, 0), new Point3d(panelX0 + panel.KFactor * 2, panelY1 - panel.KFactor, 0));
                }
                //For Top, Right and Bottom
                if (panel.TopFold == 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold != 1)
                {
                    //Draw top horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY1 - panel.KFactor * 2, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor * 2, 0));
                    //Draw Bottom horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.KFactor * 2, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor * 2, 0));
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor * 2, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor * 2, panelY1 - panel.KFactor, 0));
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
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.TopFirstFoldSetbackLeft, ((panelY1 - panel.KFactor * 2) + panel.TopFirstFoldWidth) - panel.KFactor, 0), new Point3d(panelX1 - panel.TopFirstFoldSetbackRight, ((panelY1 - panel.KFactor) + panel.TopFirstFoldWidth) - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Bottom horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.BottomFirstFoldSetbackLeft, ((panelY0 + panel.KFactor * 2) - panel.BottomFirstFoldWidth) + panel.KFactor, 0), new Point3d(panelX1 - panel.BottomFirstFoldSetbackRight, ((panelY0 + panel.KFactor) - panel.BottomFirstFoldWidth) + panel.KFactor, 0)); //horizontal line
                    guidList.Add(panel.Perimeter);
                }

                //For Left and Right
                if (panel.TopFold != 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold == 1)
                {
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.KFactor, 0), new Point3d(panelX1 + (panel.RightFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Left Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 2), panelY0 + panel.KFactor, 0), new Point3d(panelX0 - (panel.LeftFirstFoldWidth - panel.KFactor * 2), panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                }
                //For Top and Left
                //if all sides have folds
                if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold != 1 && panel.LeftFold == 1)
                {
                    //Draw top horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1, panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Left Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0, 0), new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                }

                //For Top and Right
                //if all sides have folds
                if (panel.TopFold == 1 && panel.BottomFold != 1 && panel.RightFold == 1 && panel.LeftFold != 1)
                {
                    //Draw top horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY1 - panel.KFactor, 0), new Point3d(panelX1, panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0, 0), new Point3d(panelX1 - panel.KFactor, panelY1 - panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                }
                //For Bottom and Left
                if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold != 1 && panel.LeftFold == 1)
                {
                    //Draw Bottom horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1, panelY0 + panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Left Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0 + panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX0 + panel.KFactor, panelY1, 0));
                    guidList.Add(panel.Perimeter);
                }
                //For Bottom and Right
                if (panel.TopFold != 1 && panel.BottomFold == 1 && panel.RightFold == 1 && panel.LeftFold != 1)
                {
                    //Draw Bottom horizontal (left - right)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX0, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0));
                    guidList.Add(panel.Perimeter);
                    //Draw Right Vertical (bottom to top)
                    panel.Perimeter = doc.Objects.AddLine(new Point3d(panelX1 - panel.KFactor, panelY0 + panel.KFactor, 0), new Point3d(panelX1 - panel.KFactor, panelY1, 0));
                    guidList.Add(panel.Perimeter);
                }

            }

            //End double folds here 
            MetrixUtilities.joinCurves(doc.Layers.Find("PANEL PERIMETER", true));

            // Do not recalculate if fixing holes are not required and fixing holes are not manipulated
            if (panel.FixingHoles != "0")
            {
                if (!fixingHolesManipulated)
                {
                    panel = reCalculateDistances(panel); //recalculate fixing hole quantity and distances
                    guidList = FixingHoles.drawFixingFoles(panel, panel, true, panelBottom, panelLeft, panelY0, panelY1, dimStyle, guidList, panelX0, panelX1, panelRight, panelTop, panelBottomBH, rightFixingHolePosition, panelLeftHS); //add fixing holes
                }
                else
                {
                    guidList = CustomFixingHoles.drawFixingFoles(panel, panel, true, panelBottom, panelLeft, panelY0, panelY1, dimStyle, guidList, panelX0, panelX1, panelRight, panelTop, panelBottomBH, rightFixingHolePosition, panelLeftHS); //add fixing holes
                }
            }

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
            //If the panel is a double fold  and the user wants to draw on the second fold execute this
            if (panel.DoubleFoldsRequired == "1" && panel.FoldWithFixingHoles == 2)
            {
                //Top fixing holes required 
                if (panel.TopFixingHoles == "1")
                {
                    //recalculate top fixing hole quantity and distance
                    panel.TopFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight - panel.KFactor
                                                   - panel.TopSecondFoldSetbackLeft - panel.TopSecondFoldSetbackRight) / panel.DistanceProvidedTop) + 1);
                    panel.TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight
                                                   - panel.TopSecondFoldSetbackLeft - panel.TopSecondFoldSetbackRight) / (panel.TopFixingHoleQuantity)));

                    //if the top fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.TopFixingHoleDistance, panel.DistanceProvidedTop))
                    {
                        panel.TopFixingHoleQuantity = panel.TopFixingHoleQuantity + 1;
                        panel.TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight
                                                    - panel.TopSecondFoldSetbackLeft - panel.TopSecondFoldSetbackRight) / (panel.TopFixingHoleQuantity)));
                    }
                }

                //Bottom fixing holes required
                if (panel.BottomFixingHoles == "1")
                {
                    //recalculate bottom fixing hole quantity and distance
                    panel.BottomFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight - panel.KFactor
                                                   - panel.BottomSecondFoldSetbackLeft - panel.BottomSecondFoldSetbackRight) / panel.DistanceProvidedBottom) + 1);

                    panel.BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight
                                                   - panel.BottomSecondFoldSetbackLeft - panel.BottomSecondFoldSetbackRight) / (panel.BottomFixingHoleQuantity)));

                    //if the bottom fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.BottomFixingHoleDistance, panel.DistanceProvidedBottom))
                    {
                        panel.BottomFixingHoleQuantity = panel.BottomFixingHoleQuantity + 1;
                        panel.BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight
                                                   - panel.BottomSecondFoldSetbackLeft - panel.BottomSecondFoldSetbackRight) / panel.BottomFixingHoleQuantity));
                    }
                }

                //recalculate Left fixing hole quantity and distance
                if (panel.LeftFixingHoles == "1")
                {
                    panel.LeftFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom - panel.KFactor
                                               - panel.LeftSecondFoldSetbackTop - panel.LeftSecondFoldSetbackBottom) / panel.DistanceProvidedLeft) + 1);
                    panel.LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom
                                                   - panel.LeftSecondFoldSetbackTop - panel.LeftSecondFoldSetbackBottom) / (panel.LeftFixingHoleQuantity)));

                    //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.LeftFixingHoleDistance, panel.DistanceProvidedLeft))
                    {
                        panel.LeftFixingHoleQuantity = panel.LeftFixingHoleQuantity + 1;
                        panel.LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom
                                                     - panel.LeftSecondFoldSetbackTop - panel.LeftSecondFoldSetbackBottom) / (panel.LeftFixingHoleQuantity)));
                    }
                }


                if (panel.RightFixingHoles == "1")
                {
                    //recalculate Right fixing hole quantity and distance
                    panel.RightFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom - panel.KFactor
                                                   - panel.RightSecondFoldSetbackTop - panel.RightSecondFoldSetbackBottom) / panel.DistanceProvidedRight) + 1);
                    panel.RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom
                                                   - panel.RightSecondFoldSetbackTop - panel.RightSecondFoldSetbackBottom) / (panel.RightFixingHoleQuantity)));


                    //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.RightFixingHoleDistance, panel.DistanceProvidedRight))
                    {
                        panel.RightFixingHoleQuantity = panel.RightFixingHoleQuantity + 1;
                        panel.RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom
                                                     - panel.RightSecondFoldSetbackTop - panel.RightSecondFoldSetbackBottom) / (panel.RightFixingHoleQuantity)));
                    }
                }
            }
            else
            {
                if (panel.TopFixingHoles == "1")
                {
                    //recalculate top fixing hole quantity and distance
                    panel.TopFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight - panel.KFactor
                                                   - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / panel.DistanceProvidedTop) + 1);
                    panel.TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight
                                                   - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / (panel.TopFixingHoleQuantity)));

                    //if the top fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.TopFixingHoleDistance, panel.DistanceProvidedTop))
                    {
                        panel.TopFixingHoleQuantity = panel.TopFixingHoleQuantity + 1;
                        panel.TopFixingHoleDistance = (((panel.X - panel.TopHoleSetbackLeft - panel.TopHoleSetbackRight
                                                    - panel.TopFirstFoldSetbackLeft - panel.TopFirstFoldSetbackRight) / (panel.TopFixingHoleQuantity)));
                    }
                }

                if (panel.BottomFixingHoles == "1")
                {
                    //recalculate bottom fixing hole quantity and distance
                    panel.BottomFixingHoleQuantity = Convert.ToInt32(((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight - panel.KFactor
                                                   - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / panel.DistanceProvidedBottom) + 1);

                    panel.BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight
                                                   - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / (panel.BottomFixingHoleQuantity)));

                    //if the bottom fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.BottomFixingHoleDistance, panel.DistanceProvidedBottom))
                    {
                        panel.BottomFixingHoleQuantity = panel.BottomFixingHoleQuantity + 1;
                        panel.BottomFixingHoleDistance = (((panel.X - panel.BottomHoleSetbackLeft - panel.BottomHoleSetbackRight
                                                   - panel.BottomFirstFoldSetbackLeft - panel.BottomFirstFoldSetbackRight) / panel.BottomFixingHoleQuantity));
                    }
                }

                if (panel.LeftFixingHoles == "1")
                {
                    //recalculate Left fixing hole quantity and distance
                    panel.LeftFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom - panel.KFactor
                                                   - panel.LeftFirstFoldSetbackTop - panel.LeftFirstFoldSetbackBottom) / panel.DistanceProvidedLeft) + 1);
                    panel.LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom
                                                   - panel.LeftFirstFoldSetbackTop - panel.LeftFirstFoldSetbackBottom) / (panel.LeftFixingHoleQuantity)));

                    //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.LeftFixingHoleDistance, panel.DistanceProvidedLeft))
                    {
                        panel.LeftFixingHoleQuantity = panel.LeftFixingHoleQuantity + 1;
                        panel.LeftFixingHoleDistance = (((panel.Y - panel.LeftHoleSetbackTop - panel.LeftHoleSetbackBottom
                                                     - panel.LeftFirstFoldSetbackTop - panel.LeftFirstFoldSetbackBottom) / (panel.LeftFixingHoleQuantity)));
                    }
                }

                if (panel.RightFixingHoles == "1")
                {
                    //recalculate Left fixing hole quantity and distance
                    panel.RightFixingHoleQuantity = Convert.ToInt32(((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom - panel.KFactor
                                                   - panel.RightFirstFoldSetbackTop - panel.RightFirstFoldSetbackBottom) / panel.DistanceProvidedLeft) + 1);
                    panel.RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom
                                                   - panel.RightFirstFoldSetbackTop - panel.RightFirstFoldSetbackBottom) / (panel.RightFixingHoleQuantity)));

                    //if the Left fixing hole distance is more than the maximum provided distance recalculate 
                    while (checkDistanceWIthTolerance(panel.RightFixingHoleDistance, panel.DistanceProvidedRight))
                    {
                        panel.RightFixingHoleQuantity = panel.RightFixingHoleQuantity + 1;
                        panel.RightFixingHoleDistance = (((panel.Y - panel.RightHoleSetbackTop - panel.RightHoleSetbackBottom
                                                     - panel.RightFirstFoldSetbackTop - panel.RightFirstFoldSetbackBottom) / (panel.RightFixingHoleQuantity)));
                    }
                }
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