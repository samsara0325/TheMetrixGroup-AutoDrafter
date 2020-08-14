using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;

namespace MetrixGroupPlugins.Commands
{
   [System.Runtime.InteropServices.Guid("41ce7e0a-3e97-4c53-b184-8b4b6e152e5f")]
   public class Q1LabellerCommand : Command
   {
      static Q1LabellerCommand _instance;
      public Q1LabellerCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static Q1LabellerCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "Q1Labeller"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // Check the selected dot
         GetObject go = new GetObject();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.GeometryFilter = Rhino.DocObjects.ObjectType.Annotation;

         go.SetCommandPrompt("Select Text to append Metrix M01:");
         GetResult result = go.GetMultiple(1, -1);

         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         RhinoApp.WriteLine("Object selection counter = {0}", go.ObjectCount);

         List<TextEntity> textEntityList = new List<TextEntity>();
         List<RhinoObject> rhinoObjectList = new List<RhinoObject>();

         // Loop through all the objects to find Text
         for (int i = 0; i < go.ObjectCount; i++)
         {
            RhinoObject rhinoObject = go.Object(i).Object();

            if (rhinoObject.ObjectType == ObjectType.Annotation)
            {
               TextEntity textEntity = rhinoObject.Geometry as TextEntity;

               if (textEntity != null)
               {
                  rhinoObjectList.Add(rhinoObject);
                  textEntityList.Add(textEntity);
               }
            }
         }

         // Sort the text Entity list
         IEnumerable<TextEntity> query = textEntityList.OrderByDescending(t => t.Plane.OriginY).ThenBy(t => t.Plane.OriginX);

         int maxDigit = (int)Math.Floor(Math.Log10(textEntityList.Count) + 1);

         int j = 1;

         foreach (TextEntity textE in query)
         {
            textE.Text = textE.Text + "-Q1";
            j++;
         }


         // Commit the changes for all updated labels
         for (int k = 0; k < rhinoObjectList.Count; k++)
         {
            rhinoObjectList[k].CommitChanges();
         }

         doc.Views.Redraw();

         return Result.Success;
      }
   }
}
