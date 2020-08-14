using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using System.IO;
using CsvHelper;
using System.Linq;
using Rhino.DocObjects;
using MetrixGroupPlugins.Commands;
using System.Runtime.InteropServices;
using Bullzip.PdfWriter;
using System.Windows.Forms;
using MetrixGroupPlugins.MessageBoxes;
using CsvHelper.TypeConversion;
using MetrixGroupPlugins.Utilities;
using Rhino.Display;
using System.Threading.Tasks;
using System.Threading;

namespace MetrixGroupPlugins.Commands
{
   [
     System.Runtime.InteropServices.Guid("D63B3D99-F3E2-41AB-999E-36D221757E5C"),
     CommandStyle(Style.ScriptRunner)
  ]
   public class CreateDimensionsCommand : Command
   {
      [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern bool SetDefaultPrinter(string Name);

      public CreateDimensionsCommand()
      {
         // Rhino only creates one instance of each command class defined in a
         // plug-in, so it is safe to store a refence in a static property.
         Instance = this;
      }

      ///<summary>The only instance of this command.</summary>
      public static CreateDimensionsCommand Instance
      {
         get;
         private set;
      }

      ///<returns>The command name as it appears on the Rhino command line.</returns>
      public override string EnglishName
      {
         get { return "CreateDimensions"; }
      }

      /// <summary>
      /// Runs the command.
      /// </summary>
      /// <param name="doc">The document.</param>
      /// <param name="mode">The mode.</param>
      /// <returns></returns>
      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
      {
         List<Guid> guidList = new List<Guid>(); //Create a new list to hold the guids
         AddDimensions addDimensions = new AddDimensions();
         if (addDimensions.createDimensions(guidList))//start creating dimensios
         {  
            RhinoApp.WriteLine("Dimensions added Successfully");
         }
         else
         {
            RhinoApp.WriteLine("Error : Some Dimensions were not created!");
         }
         return Result.Success;
      }
      


   }
}
