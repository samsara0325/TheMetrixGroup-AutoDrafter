using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Display;
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

namespace MetrixGroupPlugins.Commands
{
    public class AddSummaryLayout : Command
    {
        static AddSummaryLayout _instance;
        public AddSummaryLayout()
        {
            _instance = this;
        }

        ///<summary>The only instance of the AddSummaryLayout command.</summary>
        public static AddSummaryLayout Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "AddSummaryLayout"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            return createSummary(doc, null, 0);
        }

        /// <summary>
        /// Creates the layout.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="panel">The panel.</param>
        /// <param name="panelParas">The panel paras.</param>
        /// <returns></returns>
        public Result createSummary(RhinoDoc doc, List<FoldedPerforationPanel> panel, double openAreaDifference)
        {
            // Add Summary layout

            if (panel == null)
            {
                panel = new List<FoldedPerforationPanel>();
            }


            RhinoView currentView = doc.Views.ActiveView;
            var pageview = doc.Views.AddPageView("Summary", 210, 297);
            Point2d bottomLeft = new Point2d(10, 70);
            Point2d topRight = new Point2d(200, 287);

            if (pageview != null)
            {
                pageview.SetPageAsActive();
                doc.Views.ActiveView = pageview;

                drawBlock(doc, panel, openAreaDifference);
            }

            // Show all objects
            RhinoApp.RunScript("_-Show _Enter", true);

            doc.Views.DefaultViewLayout();

            doc.Views.ActiveView = currentView;

            return Result.Success;
        }

        public void drawBlock(RhinoDoc doc, List<FoldedPerforationPanel> panel, double openAreaDifference)
        {
            RhinoUtilities.SetActiveLayer("LAYOUT DETAILS", System.Drawing.Color.Black);


            string[] textArray = new string[15];

            textArray[0] = "Customer : ";
            textArray[1] = "Project : ";
            textArray[2] = "Customer Purchase Order Number : ";
            textArray[3] = "Metrix Part Number : ";
            textArray[4] = "Metrix Sales Order Number : ";
            textArray[5] = "Metrix Job Number : ";
            textArray[6] = "Description : ";
            textArray[7] = "Pattern : ";
            textArray[8] = "Open Area : ";
            textArray[9] = "Dot Fonts : ";
            textArray[10] = "Panel Types : ";
            textArray[11] = "Fixing Holes : ";
            textArray[12] = "Coating : ";
            textArray[13] = "Total Quantity of Panels : ";
            textArray[14] = "Total SQM of Panels : ";

            double height = 5.5;
            string font = "Arial";
            Rhino.Geometry.Plane plane = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            Guid id;
            int i = 0;

            //Add headings to Summary Layout
            foreach (string text in textArray)
            {
                plane.Origin = new Point3d(7, 260 - 10 * i + height, 0);
                id = doc.Objects.AddText(text, plane, height, font, false, false);
                i++;
            }

            //Add Customer Name
            string valueText = panel[0].customerName;
            if(valueText.Length > 50)
            {
                plane.Origin = new Point3d(50, 260 + 4, 0);
                id = doc.Objects.AddText(valueText, plane, 4, font, false, false);
            }
            else
            {
                plane.Origin = new Point3d(50, 260 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }

            //Add Project Name
            valueText = panel[0].project;
            plane.Origin = new Point3d(50, 250 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Customer Number
            valueText = panel[0].CustomerOrderNo;
            plane.Origin = new Point3d(135, 240 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Metrix Part Number
            valueText = panel[0].MetrixPartNo;
            plane.Origin = new Point3d(82, 230 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Customer Number
            valueText = panel[0].MetrixSalesNo;
            plane.Origin = new Point3d(107, 220 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Job Number
            valueText = panel[0].jobNo;
            plane.Origin = new Point3d(82, 210 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Description
            valueText = panel[0].SheetThickness + "mm / " + panel[0].material;
            plane.Origin = new Point3d(50, 200 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Pattern
            valueText = panel[0].PatternName;
            plane.Origin = new Point3d(40, 190 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Pattern Open Area
            if (openAreaDifference <= 2)
            {
                valueText = panel[0].PatternOpenArea + "%";
            }
            else
            {
                valueText = "%";
            }

            //valueText = panel[0].PatternOpenArea + "%";
            plane.Origin = new Point3d(50, 180 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Dot Fonts
            if (panel[0].DotFontLabel == 1)
            {
                valueText = panel[0].DotFontLabellerSide;
                plane.Origin = new Point3d(50, 170 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }
            else
            {
                valueText = "No";
                plane.Origin = new Point3d(50, 170 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }

            //Add Panel Types
            List<string> pnlList = new List<string>();
            pnlList.Add(panel[0].PanelType);
            foreach (FoldedPerforationPanel pnl in panel)
            {
                if (!pnlList.Contains(pnl.PanelType))
                {
                    pnlList.Add(pnl.PanelType);
                }
            }
            valueText = "";
            int textCounter = 0;
            foreach (string type in pnlList)
            {
                if (textCounter == 0)
                {
                    valueText += type;
                }
                else
                {
                    valueText += " & " + type;
                }
                textCounter++;
            }
            plane.Origin = new Point3d(55, 160 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add Fixing Holes
            if (panel[0].FixingHoles.Equals("0"))
            {
                valueText = "No";
                plane.Origin = new Point3d(55, 150 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }
            else
            {
                valueText = "Yes";
                plane.Origin = new Point3d(55, 150 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }

            //Add Coating
            if (panel[0].coating.Equals("Mill Finish") || panel[0].coating.Equals("Mill finish"))
            {
                valueText = panel[0].coating;
                plane.Origin = new Point3d(40, 140 + height, 0);
                id = doc.Objects.AddText(valueText, plane, height, font, false, false);
            }
            else
            {
                valueText = panel[0].colour;
                plane.Origin = new Point3d(40, 140 + 4.5, 0);
                id = doc.Objects.AddText(valueText, plane, 4.5, font, false, false);
            }


            //Add Panel Quantity
            valueText = panel[0].TotalPanelQuantity; 
            plane.Origin = new Point3d(95, 130 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);

            //Add SQM of Panels
            valueText = Math.Round(panel[0].TotalPanelSQM, 2).ToString();
            plane.Origin = new Point3d(85, 120 + height, 0);
            id = doc.Objects.AddText(valueText, plane, height, font, false, false);
        }
    }
}