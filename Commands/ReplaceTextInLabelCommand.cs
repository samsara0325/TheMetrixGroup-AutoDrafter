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
   [System.Runtime.InteropServices.Guid("ee63a878-baa9-4a5a-89c3-0132e4cee0e2")]
   public class ReplaceTextInLabelCommand : Command
   {
      static ReplaceTextInLabelCommand _instance;
      public ReplaceTextInLabelCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the ReplaceTextInLabel command.</summary>
      public static ReplaceTextInLabelCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "ReplaceTextInLabel"; }
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

         go.SetCommandPrompt("Select Text to replace:");
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
               }
            }
         }

         FindAndReplaceForm findForm = new FindAndReplaceForm(rhinoObjectList);
         findForm.ShowDialog(RhinoApp.MainWindow());

         doc.Views.Redraw();

         return Result.Success;
      }
   }
}
