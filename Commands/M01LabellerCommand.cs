using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;

namespace MetrixGroupPlugins
{
   [System.Runtime.InteropServices.Guid("962aeccb-af38-4ba7-b4d3-e22ab25318fc")]
   public class M01LabellerCommand : Command
   {
      static M01LabellerCommand _instance;
      public M01LabellerCommand()
      {
         _instance = this;
      }

      ///<summary>The only instance of the MyCommand1 command.</summary>
      public static M01LabellerCommand Instance
      {
         get { return _instance; }
      }

      public override string EnglishName
      {
         get { return "M01Labeller"; }
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
         IEnumerable<TextEntity> query = textEntityList.OrderByDescending(t => t.Plane.OriginY);//.ThenBy(t => t.Plane.OriginX);
         IEnumerable<TextEntity> xSortedList = textEntityList.OrderBy(t => t.Plane.OriginX);//.ThenBy(t => t.Plane.OriginY);
         IEnumerable<TextEntity> newList = null;
         List<TextEntity> toSort = new List<TextEntity>();
         List<TextEntity> toRemove = new List<TextEntity>();
         double y;
         Point3d previous = new Point3d(0, 0, 0);
         int maxDigit = (int) Math.Floor(Math.Log10(textEntityList.Count) + 1);

         int j = 1;
         Boolean found = false;
         foreach (TextEntity yText in query)
         {
            found = false;
            if (toRemove.Count > 0)
            {
               foreach (TextEntity deletedElement in toRemove){
                  if (deletedElement.Equals(yText)){
                     found = true;
                     break;
                  }
               }
            }
            if(found == false)
            {
               y = yText.Plane.OriginY;
               toSort = new List<TextEntity>();

               newList = null;
               // p = textE;
               foreach (TextEntity t in xSortedList)
               {
                  if (t.Plane.OriginY == y || t.Plane.OriginY >= y && t.Plane.OriginY <= (y+200) || t.Plane.OriginY <= y && t.Plane.OriginY >= (y - 1000)) //Check if y is same
                  {
                     toSort.Add(t);
                  }

               }

               if (toSort.Count > 0) //If the list is more than 0, sort it by X 
               {
                  //toSort.Add(yText);
                  newList = toSort.OrderBy(t => t.Plane.OriginX);
                  toRemove.AddRange(newList);
               }


               foreach (TextEntity textE in newList)
               {
                  // Test if there is a M_- in front of the text, if yes, remove it
                  if (textE.Text.Count() > 2)
                  {
                     if (textE.Text[0] == 'M')
                     {
                        if (Char.IsNumber(textE.Text[1]))
                        {
                           if (textE.Text.IndexOf('-') != -1)
                           {
                              // This means M0 exist
                              textE.Text = textE.Text.Substring(textE.Text.IndexOf('-') + 1);
                           }
                        }
                     }
                  }

                  textE.Text = "M" + j.ToString().PadLeft(maxDigit, '0') + "-" + textE.Text;
                  j++;
               }
             //  toRemove.AddRange(newList);
            }
            else
            {
               continue;
            }
         }


         // Commit the changes for all updated labels
         for (int k = 0; k < rhinoObjectList.Count; k++)
         {
            rhinoObjectList[k].CommitChanges();
         }

         doc.Views.Redraw();

         return Result.Success;
      }

      //public IEnumerable<TextEntity> removeElements(IEnumerable<TextEntity> tempList, double yValue)
      //{
      //   foreach(TextEntity text in tempList)
      //   {
      //      if()
      //   }
      //}
   }
}
