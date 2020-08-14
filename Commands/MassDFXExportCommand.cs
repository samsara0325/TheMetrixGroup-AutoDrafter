using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using System.Text.RegularExpressions;
using MetrixGroupPlugins.Resources;
using MetrixGroupPlugins.MessageBoxes;
using Rhino.Input;
using MetrixGroupPlugins.Utilities;
using System.IO;

/**
 * This class implements the Mass DXF exporter command (author Wilfred)
 * 
 * */
namespace MetrixGroupPlugins.Commands
{
    [
        System.Runtime.InteropServices.Guid("3a13027e-ba47-403f-be4a-d4481a97b272"),
        CommandStyle(Style.ScriptRunner)
    ]
    public class MassDFXExportCommand : Command
    {

        String selectedOption = null;
        Boolean onlyMLabels = true;
        List<String> unClosedPanels = new List<string>();
        RhinoObject[] labelObjects;
        Boolean curveOpen = false;
        RhinoDoc doc;
        public MassDFXExportCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static MassDFXExportCommand Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "MassDFXExport"; }
        }

        ///<summary>The only instance of this command.</summary>
        ///<param name="doc" RhinoDoc></param>
        ///<param name="mode" Run mode></param>
        ///<returns>returns sucess if doc is successfully created </returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            try
            {
                var myLayers = doc.Layers;
                this.doc = doc;

                Rhino.Display.RhinoView view = null;
                RhinoApp.WriteLine("Switching to Top View"); //switch to top view for the command to work
                view = doc.Views.Find("Top", false); // en
                doc.Views.ActiveView = view;
                //display the option list to the user using the method
                displayMenuOption();
                if (!curveOpen)
                {
                    setLayerVisibility(doc, false);
                    Perimeter(doc);
                    return Result.Success;
                }
                else
                {
                    Messages.showUnclosedCurves(unClosedPanels);
                    return Result.Failure;

                }
            }
            catch (Exception e)
            {
                Messages.showMassdxfError(e);
                return Result.Failure;
            }
        }
        //Method displays the option menu to the user
        private void displayMenuOption()
        {
            MassDXFOptionSelection displayOption = new MassDXFOptionSelection();//Display the option list to the user
            displayOption.ShowDialog();
            selectedOption = displayOption.getOption();  //get the selected option
            onlyMLabels = displayOption.getCheckBoxOption();
            checkClosedContours(); //check if the panels have closed contours before proceeding 
        }

        //Method checks whether there are any unclosed curves in the panels.
        private void checkClosedContours()
        {

            RhinoObject[] perimeterObjects = null;
            List<String> sortingList = new List<String>();
            int objectFound = 5;
            int indexCount = 0;
            labelObjects = doc.Objects.FindByLayer("LABELS"); //get the label objects 
            labelObjects = removeFalseLabels(labelObjects);
            perimeterObjects = doc.Objects.FindByLayer("PANEL PERIMETER"); //get the perimeter objects 
            objectFound = (perimeterObjects.Length) - 1;
            indexCount = (perimeterObjects.Length) - 1;
            foreach (RhinoObject obj in labelObjects)
            {
                sortingList.Add(((AnnotationObjectBase)obj).DisplayText);
            }

            foreach (RhinoObject panel in labelObjects)
            {
                RhinoObject rh = perimeterObjects[objectFound];

                ObjRef objRef = new ObjRef(rh);
                Curve curve = objRef.Curve();
                if (curve.IsClosed == false)
                {
                    RhinoApp.WriteLine(objRef.ToString() + "curve is open");
                    curveOpen = true;
                    unClosedPanels.Add(sortingList[indexCount]); //adds the panel label name to the list              
                }
                if (objectFound > 0 && indexCount > 0)
                {
                    objectFound--;
                    indexCount--;
                }
            }

        }
        //this method sets the visibility status of different layers based on the selected option
        private void setLayerVisibility(RhinoDoc doc, Boolean completed)
        {

            if (selectedOption.Equals("PandL") && !completed)
            {
                if(doc.Layers.Find("DOT SCRIBE LABEL", true) >= 0)
                {
                    RhinoUtilities.setLayerVisibility("DOT SCRIBE LABEL", false);
                }
                RhinoUtilities.setLayerVisibility("TOOL HIT", false);
                RhinoUtilities.setLayerVisibility("FIXING HOLES", false);
                RhinoUtilities.setLayerVisibility("FOLDED PANEL FINISHED", false);
                RhinoUtilities.setLayerVisibility("FOLDS", false);
                RhinoUtilities.setLayerVisibility("BORDERS", false);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLUE", false);
                RhinoUtilities.setLayerVisibility("DETAILS", false);
                RhinoUtilities.setLayerVisibility("PERF ORIENTATION", false);
                RhinoUtilities.setLayerVisibility("BORDERS", false);
                RhinoUtilities.setLayerVisibility("FIXING HOLE DIMENSIONS", false);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLACK", false);
            }
            if (selectedOption.Equals("FullLayer") && !completed)
            {
                RhinoUtilities.setLayerVisibility("DOT SCRIBE LABEL", true);
                RhinoUtilities.setLayerVisibility("TOOL HIT", true);
                RhinoUtilities.setLayerVisibility("FIXING HOLES", true);
                RhinoUtilities.setLayerVisibility("FOLDED PANEL FINISHED", false);
                RhinoUtilities.setLayerVisibility("FOLDS", false);
                RhinoUtilities.setLayerVisibility("BORDERS", false);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLUE", false);
                RhinoUtilities.setLayerVisibility("LAYOUT DETAILS", false);
                RhinoUtilities.setLayerVisibility("PERFORATION", false);
                RhinoUtilities.setLayerVisibility("BORDERS", false);
                RhinoUtilities.setLayerVisibility("CLUSTER SAMPLE", false);
                RhinoUtilities.setLayerVisibility("FIXING HOLES DIMENSIONS", false);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLACK", false);
            }

            if (completed)
            {
                RhinoUtilities.setLayerVisibility("DOT SCRIBE LABEL", true);
                RhinoUtilities.setLayerVisibility("TOOL HIT", true);
                RhinoUtilities.setLayerVisibility("FOLDED PANEL FINISHED", true);
                RhinoUtilities.setLayerVisibility("FOLDS", true);
                RhinoUtilities.setLayerVisibility("BORDERS", true);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLUE", true);
                RhinoUtilities.setLayerVisibility("LAYOUT DETAILS", true);
                RhinoUtilities.setLayerVisibility("PERFORATION", true);
                RhinoUtilities.setLayerVisibility("BORDERS", true);
                RhinoUtilities.setLayerVisibility("CLUSTER SAMPLE", false);
                RhinoUtilities.setLayerVisibility("FIXING HOLES DIMENSIONS", true);
                RhinoUtilities.setLayerVisibility("DIMENSIONS BLACK", true);
            }


        }

        private void Perimeter(RhinoDoc doc)
        {
            String labelName = "test";
            // RhinoObject[] labelObjects = null;
            RhinoObject[] perimeterObjects = null;
            RhinoObject rh;
            // labelObjects = doc.Objects.FindByLayer("LABELS");
            perimeterObjects = doc.Objects.FindByLayer("PANEL PERIMETER");

            int layerNumber = doc.Layers.Find("PANEL PERIMETER", true);

            int perimeterCounter = 0;
            int labelCounter = 0;

            string fileName = null;
            string pathName = Rhino.ApplicationSettings.FileSettings.WorkingFolder;

            //  objectFound = perimeterObjects.Length-1;
            int nameCount = 0;
            //if (!System.IO.Directory.Exists(pathName + "\\" + "TRUMPH"))
            //{
            //   System.IO.Directory.CreateDirectory(pathName + "\\" + "TRUMPH");
            //}
            // pathName = pathName + "\\" + "TRUMPH";
            string newPathName = pathName;


            List<String> panelLabelList = new List<String>();
            foreach (RhinoObject obj in labelObjects)
            {
                panelLabelList.Add(((AnnotationObjectBase)obj).DisplayText);
            }

            foreach (RhinoObject panel in labelObjects)
            {
                rh = perimeterObjects[perimeterCounter];

                if (selectedOption.Equals("PandL"))
                {
                    MassDFXExportCommand.drawObjects(newPathName, fileName, rh, layerNumber);
                    setLayerVisibility(doc, false);
                    labelName = findLabelName(labelObjects);
                    fileName = labelName;
                    exportDXFFile(fileName, doc);
                    //newPathName = pathName + "\\" + fileName + ".dxf";
                    //RhinoApp.RunScript("-_Export " + newPathName + "  _Enter", true);
                }

                if (selectedOption.Equals("FullLayer"))
                {
                    //Drawing panel perimeter

                    //  RhinoUtilities.setLayerVisibility("FIXING HOLES", true);
                    MassDFXExportCommand.drawObjects(newPathName, fileName, rh, layerNumber);
                    setLayerVisibility(doc, false);
                    labelName = findLabelName(labelObjects);
                    fileName = labelName;
                    exportDXFFile(fileName, doc);
                    //newPathName = pathName + "\\" + fileName + ".dxf";
                    //RhinoApp.RunScript("-_Export " + newPathName + "  _Enter", true);
                }
                perimeterCounter++;
                labelCounter++;
                nameCount++;
            }
            setLayerVisibility(doc, true);
            Messages.showMassdxfComplete();
        }

        //Draw the objects
        public static void drawObjects(String pathName, String fileName, RhinoObject rh, int layerNumber)
        {
            GetObject go = new GetObject();
            RhinoApp.RunScript("_SelNone", true);
            RhinoApp.RunScript("_SelBoundary _SelectionMode=_Crossing _SelID " + rh.Id.ToString(), true);
            RhinoApp.RunScript("_SelID " + rh.Id.ToString(), true);

            RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
            go.GeometryFilter = ObjectType.Curve;
            go.GeometryAttributeFilter = GeometryAttributeFilter.ClosedCurve;
            go.EnableTransparentCommands(true);
            RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
            //go.GetMultiple(1, 1);     
            RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
            //  go.Get();
            RhinoApp.RunScript("SelLayerNumber " + layerNumber, true);
            RhinoApp.RunScript("_SelNone", true);
            RhinoApp.RunScript("_SelBoundary _SelectionMode=_Crossing _SelID " + rh.Id.ToString(), true);
            RhinoApp.RunScript("_SelID " + rh.Id.ToString(), true);
        }
        // Copy from Website on how to Clean a string
        public static string CleanString(string dirtyString)
        {
            // Remove all the hidden characters first
            string replacement = Regex.Replace(dirtyString, @"\n|\n|\n", "");

            string removeChars = "[(?*\",\\<>&#~%{}+_.@:!;] ";
            string result = replacement;

            foreach (char c in removeChars)
            {
                result = result.Replace(c, '-');
            }

            return result;
        }

        //This method helps to find the label name for each individual panel
        private String findLabelName(RhinoObject[] labelObjects)
        {
            //get the selected objects
            IEnumerable<RhinoObject> selectedObjects = doc.Objects.GetSelectedObjects(false, false);
            foreach (RhinoObject obj in selectedObjects) //loop through the selected objects
            {
                foreach (RhinoObject label in labelObjects) //start another loop to loop through the available labels
                {
                    if (obj.Id.Equals(label.Id)) //check if the selected object id is equal to the label id
                    {
                        return ((AnnotationObjectBase)label).DisplayText; //if it is equal it is the the object is the label of the individual panel
                    }
                }
            }
            return "test";
        }


        //This method removes all objects from the labelsobjects array which are not valid labels 
        private RhinoObject[] removeFalseLabels(RhinoObject[] labelObjects)
        {
            String tempText = null;
            List<RhinoObject> tempList = new List<RhinoObject>();
            foreach (RhinoObject rjo in labelObjects)
            {
                tempText = (((AnnotationObjectBase)rjo).DisplayText);
                if (onlyMLabels)
                {
                    if (tempText.ElementAt(0) == 'M') //check if the object text starts with a M
                    {
                        tempList.Add(rjo); //if yes, it means its a valid label, add it to the temporary list
                    }
                }
                else
                {
                    tempList.Add(rjo); //if yes, it means its a valid label, add it to the temporary list
                }
            }

            labelObjects = tempList.ToArray();
            return labelObjects;
        }

        //Method export the final file containing the exported DXF
        private static void exportDXFFile(String exportFileName, RhinoDoc doc)
        {
            String path;
            String immediateFolderName = Path.GetFullPath(Path.GetDirectoryName(doc.Path));
            path = immediateFolderName + ("\\nRUMPF");
            if (!Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //create directory if not exist
            }

            RhinoApp.RunScript("-_Export \"" + path + "\\" + exportFileName + ".dxf" + "\"  Scheme \"R12 Lines & Arcs\" Enter", true);
        }
    }

}
