using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Input;


namespace MetrixGroupPlugins.Commands
{
    public class DistanceDetection : Command
    {
        static DistanceDetection _instance;
        public DistanceDetection()
        {
            _instance = this;
        }

        ///<summary>The only instance of the DistanceDetection command.</summary>
        public static DistanceDetection Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "DistanceDetection"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            GetObject go = new GetObject();
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;
            go.EnableSelPrevious(true);
            go.EnablePreSelect(true, false);
            go.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;

            go.SetCommandPrompt("Select All circles you want to filter:");
            GetResult result = go.GetMultiple(1, 0);

            if (go.CommandResult() != Rhino.Commands.Result.Success)
            {
                return go.CommandResult();
            }

            int fixingHoleCounter = go.ObjectCount;
            RhinoApp.WriteLine("circle selection counter = {0}", fixingHoleCounter);

            int i = 0;
            Point3d[] fixingHole = new Point3d[fixingHoleCounter];
            double[] fixingHoleD = new double[fixingHoleCounter];
            RhinoObject[] references = new RhinoObject[fixingHoleCounter];

            for (i = 0; i < go.ObjectCount; i++)
            {
                RhinoObject rhinoObject = go.Object(i).Object();
                Curve curve = (new ObjRef(rhinoObject)).Curve();
                if (curve == null)
                {
                    continue;
                }

                if (curve.IsClosed == false)
                {
                    continue;
                }

                if (curve.IsPlanar() == false)
                {
                    continue;
                }

                if (curve.IsCircle())
                {
                    BoundingBox boundingBox = curve.GetBoundingBox(true);
                    fixingHoleD[i] = boundingBox.Max.X - boundingBox.Min.X;
                    fixingHole[i] = boundingBox.Center;
                    references[i] = rhinoObject;

                }
            }

            //Get the gap clearance offset
            go.SetCommandPrompt("Enter detection radius:");
            go.AcceptNumber(true, false);
            go.Get();
            double offset = go.Number();

            double perforationHoldD;
            string layerName = "";


            //for testing purpose, draw the hole with offset using red color

            /*Curve[] circles = new Curve[fixingHole.Length];
            for (i = 0; i < circles.Length; i++)
            {

                circles[i] = new ArcCurve(new Circle(fixingHole[i], offset));
            }


            if (circles != null)
            {
                layerName = offset + "mm GAP CLEARANCE";
                RhinoUtilities.SetActiveLayer(layerName, System.Drawing.Color.Red);

                foreach (Curve cv in circles)
                {
                    Guid guid = RhinoDoc.ActiveDoc.Objects.AddCurve(cv);
                }
            }*/

            string clashHoleLayerName = "HOLES CLASHED";
            Layer clashHoleLayer = new Layer();
            clashHoleLayer.Name = clashHoleLayerName;
            clashHoleLayer.Color = System.Drawing.Color.Red;

            int clashHoleLayerIndex = doc.Layers.Add(clashHoleLayer);
            int currentHoleCounter;
            int flag;

            bool[] deletedCircles = new bool[fixingHole.Length];
            for (i = 0; i < deletedCircles.Length; i++)
            {
                deletedCircles[i] = false;
            }

            Curve circles;
            RhinoUtilities.SetActiveLayer(clashHoleLayerName, System.Drawing.Color.Red);
            for (i = 0; i < fixingHole.Length - 1; i++)
            {

                currentHoleCounter = 0;
                flag = 0;
                for (int j = i + 1; j < fixingHole.Length; j++)
                {

                    if (deletedCircles[j] == true)
                    {
                        continue;
                    }
                    if (fixingHole[i].DistanceTo(fixingHole[j]) < fixingHoleD[i] + offset)
                    {
                        if (currentHoleCounter == 0)
                        {
                            flag = j;
                            currentHoleCounter++;
                        }
                        else
                        {
                            currentHoleCounter++;
                            break;
                        }
                    }
                    if (currentHoleCounter == 1)
                    {
                        circles = new ArcCurve(new Circle(fixingHole[flag], fixingHoleD[flag] / 2));
                        Guid guid = RhinoDoc.ActiveDoc.Objects.AddCurve(circles);

                        RhinoDoc.ActiveDoc.Objects.Delete(references[flag], false);
                        deletedCircles[flag] = true;
                    }
                    else if (currentHoleCounter == 2)
                    {
                        circles = new ArcCurve(new Circle(fixingHole[i], fixingHoleD[i] / 2));
                        Guid guid = RhinoDoc.ActiveDoc.Objects.AddCurve(circles);

                        RhinoDoc.ActiveDoc.Objects.Delete(references[i], false);
                        deletedCircles[i] = true;
                    }
                }


            }


            RhinoUtilities.setLayerVisibility("HOLES CLASHED", true);

            RhinoUtilities.setLayerVisibility(layerName, false);

            doc.Views.Redraw();

            return Result.Success;
        }
    }
}