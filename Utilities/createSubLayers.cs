using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace MetrixGroupPlugins
{
    /**
     * Class contains methods to create sublayers from parent layers 
     * */
    public static class createSubLayers
    {
        //Method returns the layer index after the sublayer is created   
        public static int createSubLayer(String layerName, System.Drawing.Color color, Rhino.DocObjects.Layer parentLayer)
        {
            int layerIndex = 0;
            Rhino.DocObjects.Layer parent_layer_Approval = null; //create variable to hold approval layer 
            Rhino.DocObjects.Layer childlayer = null; //Create a variable to hold child layers
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            // Does a layer with the same name already exist?
            layerIndex = doc.Layers.Find(layerName, true);
            parent_layer_Approval = parentLayer;

            if (layerIndex < 0) //If the layer does not exist, create a new layer
            {
                childlayer = new Rhino.DocObjects.Layer();
                childlayer.Color = color;
                childlayer.Name = layerName;

            if (layerName.Equals("FOLDS")) //Checks if the layer name getting created is called "FOLDS", if yes the folds line type should be dashed
            {
               const string linetype_name = "Dashed"; 
               var linetype_index = doc.Linetypes.Find(linetype_name, true); //get the index of the line
               if (linetype_index < 0)
               {
                  RhinoApp.WriteLine("\"{0}\" linetype not found.", linetype_name);
                  //  return Result.Nothing;
               }

               if (childlayer.LinetypeIndex != linetype_index)
               {
                  childlayer.LinetypeIndex = linetype_index; //set the line type to the layer
               }
            }
                childlayer.ParentLayerId = parent_layer_Approval.Id; //set parent layer id of child layer(Parent layer - Apporval Layer)
                doc.Layers.Add(childlayer); //add child layer to the layer table
                return layerIndex = doc.Layers.Find(layerName, true);
            }
            else //if layer exists, set parent id of the layer to parent layer id
            {
                doc.Layers[layerIndex].ParentLayerId = parent_layer_Approval.Id;
                return layerIndex = doc.Layers[layerIndex].LayerIndex;
            }
        }
    }
}
