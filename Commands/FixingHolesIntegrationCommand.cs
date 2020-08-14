//using System;
//using System.Collections.Generic;
//using Rhino;
//using Rhino.Commands;
//using Rhino.Geometry;
//using Rhino.Input;
//using Rhino.Input.Custom;

//namespace MetrixGroupPlugins
//{
//   [System.Runtime.InteropServices.Guid("7086bb96-9fcb-4879-b8da-a68e6c0f4033")]
//   public class FixingHolesIntegrationCommand : Command
//   {
   
//      public FixingHolesIntegrationCommand()
//      {
//         // Rhino only creates one instance of each command class defined in a
//         // plug-in, so it is safe to store a refence in a static property.
//         Instance = this;
//      }

//      ///<summary>The only instance of this command.</summary>
//      public static FixingHolesIntegrationCommand Instance
//      {
//         get;
//         private set;
//      }

//      ///<returns>The command name as it appears on the Rhino command line.</returns>
//      public override string EnglishName
//      {
//         get { return "FixingHolesIntegation"; }
//      }

//      protected override Result RunCommand(RhinoDoc doc, RunMode mode)
//      {
//         // Check the selected curve
//         GetObject go = new GetObject();

//         go.GroupSelect = true;
//         go.SubObjectSelect = false;
//         go.EnableClearObjectsOnEntry(false);
//         go.EnableUnselectObjectsOnExit(false);
//         go.DeselectAllBeforePostSelect = false;
//         go.EnableSelPrevious(true);
//         go.EnablePreSelect(true, false);
//         go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

//         GetResult result = go.GetMultiple(1,-1);

//         if (go.CommandResult() != Rhino.Commands.Result.Success)
//         {
//            return go.CommandResult();
//         }

//         RhinoApp.WriteLine("{0} curve is selected.", go.ObjectCount);

//         // Process the curveList and put it in a data structure
//         List<Curve> curveList = new List<Curve>();

//         for (int i = 0; i < go.ObjectCount; i++)
//         {
//            //Curve currentCurve = go.Object(i).Curve();
//            //bool converted = currentCurve.TryGetCircle(out circle);

//            //if(converted == true)
//            //{
//            //   if(diameter == -1)
//            //   {
//            //      diameter = circle.Diameter;
//            //   }
//            //   else if(Math.Abs(diameter - circle.Diameter) > Properties.Settings.Default.Tolerance)
//            //   {
//            //      RhinoApp.WriteLine("Not all the curves are the same size. {0}", currentCurve.ToString());
//            //      return Rhino.Commands.Result.Failure;
//            //   }
//            //}

//            //circleList.Add(circle);
//         }
         
//         //ClusterToolSearcherForm clusterToolSearcherForm = new ClusterToolSearcherForm(circleList);
//         //clusterToolSearcherForm.ShowDialog(RhinoApp.MainWindow());

//         //Curve curve = go.Object(0).Curve();

//         //// If curve is null
//         //if (curve == null)
//         //{
//         //   return Rhino.Commands.Result.Failure;
//         //}

//         //// If curve is Closed Curve Orientation 
//         //if (curve.IsClosed == false)
//         //{
//         //   RhinoApp.WriteLine("The curve is open");
//         //   return Rhino.Commands.Result.Failure;
//         //}

//         //PerforationForm perforationForm = new PerforationForm(curve);

//         //perforationForm.ShowDialog(RhinoApp.MainWindow());

//         return Result.Success;
//      }
//   }
//}
