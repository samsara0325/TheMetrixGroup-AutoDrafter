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

namespace MetrixGroupPlugins
{
   [
      System.Runtime.InteropServices.Guid("689a2ed0-4ab0-4c85-9517-8b40e6c4d8e7"),
      Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
   ]
   public class Selection2DXFExportCommand : Command
   {
      public Selection2DXFExportCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static Selection2DXFExportCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "Selection2DXFExport"; }
      }

      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         // Do some error checking first
         // If Path == nothing, ask user to save first.
         if(doc.Path == "")
         {
            System.Windows.Forms.MessageBox.Show("Please save Rhino file before you can export.", "Export Error");
            return Result.Failure;
         }

         // Get all selected Objects
         GetObject go = new GetObject();

         go.GroupSelect = true;
         go.SubObjectSelect = false;
         go.EnableClearObjectsOnEntry(false);
         go.EnableUnselectObjectsOnExit(false);
         go.DeselectAllBeforePostSelect = false;
         go.EnableSelPrevious(true);
         go.EnablePreSelect(true, false);
         go.EnablePressEnterWhenDonePrompt(false);

         go.SetCommandPrompt("Select objects and a label to export as .DXF:");

         GetResult result = go.GetMultiple(1,-1);

         if (go.CommandResult() != Rhino.Commands.Result.Success)
         {
            return go.CommandResult();
         }

         RhinoApp.WriteLine("Total Objects Selected: {0}", go.ObjectCount);

         string labelName = null;

         if (doc.Path == null)
            {
                System.Windows.Forms.MessageBox.Show("Please save the file as .3dm file first");
                return Result.Failure;
            }

         if (Path.GetFileName(doc.Path).Split('.').Length == 1)
            {
                System.Windows.Forms.MessageBox.Show("Please save the file as .3dm file first");
                return Result.Failure;
            }

         if (Path.GetFileName(doc.Path).Split('.').Length == 2 && !(Path.GetFileName(doc.Path).Split('.')[1].Equals("3dm")))
            {
                System.Windows.Forms.MessageBox.Show("Please save the file as .3dm file first");
                return Result.Failure;
            }
                // Loop through all the objects to find Text
                for (int i = 0; i < go.ObjectCount; i++)
         {
            RhinoObject rhinoObject = go.Object(i).Object();
            
            if(rhinoObject.ObjectType == ObjectType.Annotation)
            {
               TextEntity textEntity = rhinoObject.Geometry as TextEntity;

               if (textEntity != null)
               {
                  labelName = CleanString(textEntity.Text.Trim()) +".dxf";
               }
            }
         }

         if(labelName == null)
         {
            return Rhino.Commands.Result.Failure;
         }

        
         String path = Path.GetDirectoryName(doc.Path); 

         // Export the whole lot
         //string command = string.Format("-_Export \"" + Path.GetDirectoryName(doc.Path) + @"\" + labelName + "\"  Scheme \"R12 Lines & Arcs\" Enter");
         string command = string.Format("-_Export \"" + Path.GetDirectoryName(doc.Path) + @"\" + labelName + "\" Enter");
         // Export the selected curves
         RhinoApp.RunScript(command, true);

         return Result.Success;
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

   }
}
