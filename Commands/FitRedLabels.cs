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
        System.Runtime.InteropServices.Guid("D70C4BB0-66C3-42EE-9FF9-38FCCFFBCD15"),
        CommandStyle(Style.ScriptRunner)
    ]
    public class FitRedLabels : Command
    {

        String selectedOption = null;
        Boolean onlyMLabels = true;
        List<String> unClosedPanels = new List<string>();
        RhinoObject[] labelObjects;
        Boolean curveOpen = false;
        RhinoDoc doc;
        public FitRedLabels()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FitRedLabels Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "FitRedLabels"; }
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

                //Retrieve all label objects
                getLabels();
                resizeLabels();
               return Result.Success;
              
            }
            catch (Exception e)
            {
                Messages.showMassdxfError(e);
                return Result.Failure;
            }
        }
       
        //Method retrieves all label objects
        private void getLabels()
        {
            labelObjects = doc.Objects.FindByLayer("LABELS"); //get the label objects 
            //labelObjects = removeFalseLabels(labelObjects);
        }

        private void resizeLabels()
        {
            RhinoObject[] perimeterObjects = null;
            perimeterObjects = doc.Objects.FindByLayer("PANEL PERIMETER");
            BoundingBox bbox;
           // foreach (RhinoObject obj in perimeterObjects)
          //  {
                //bbox = obj.Geometry.GetBoundingBox(Plane.WorldXY);

               
                TextObject tempText;
                foreach (RhinoObject rj in labelObjects)
                {
                    tempText = ((TextObject)rj); //cast textobject
                    tempText.TextGeometry.TextHeight = 3;
                    tempText.CommitChanges();
                    
                }
           // }
        }
    
    }

}
