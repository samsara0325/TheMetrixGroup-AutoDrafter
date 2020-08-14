using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins
{
    public class PerforationPanel
    {
        private String partName;
        private double x;
        private double y;
        private int row;
        private int column;
        private double leftBorder;
        private double rightBorder;
        private double topBorder;
        private double bottomBorder;
        private String patternName;
        private int panelNumber;
        private int drawPerf;
        private int dotFontLabel;
        private String dXFFilesRequired;
        private Guid perimeter;
        private Guid border;
        private Guid label;
        private Guid xDim;
        private Guid yDim;
        private int panelQuantity;
        private Guid fold;
        private String panelType;

        private String fixingHoles;
        private double holeDiameter;

        private double leftFixingHoleDistance;
        private double rightFixingHoleDistance;
        private double topFixingHoleDistance;
        private double bottomFixingHoleDistance;

        private String topFixingHoles;
        private int topFixingHoleQuantity;
        private double distanceTopCenter;
        private double topHoleSetbackLeft;
        private double topHoleSetbackRight;
        private double topHoleSetbackTop;

        private String bottomFixingHoles;
        private int bottomFixingHoleQuantity;
        private double distanceBottomCenter;
        private double bottomHoleSetbackLeft;
        private double bottomHoleSetbackRight;
        private double bottomHoleSetbackBottom;

        private String leftFixingHoles;
        private int leftFixingHoleQuantity;
        private double distanceLeftCenter;
        private double leftHoleSetbackTop;
        private double leftHoleSetbackLeft;
        private double leftHoleSetbackBottom;

        private String rightFixingHoles;
        private int rightFixingHoleQuantity;
        private double distanceRightCenter;
        private double rightHoleSetbackTop;
        private double rightHoleSetbackBottom;
        private double rightHoleSetbackRight;

        private String perfText;
        private double distanceProvided;
        private double toleranceProvided;

        private double distanceProvidedTop;
        private double distanceProvidedBottom;
        private double distanceProvidedLeft;
        private double distanceProvidedRight;
        private String patternOpenArea;
        private String dotFontLabellerSide;

        private int fixingHoleCentres;
        private int foldWithFixingHoles;
        string firstRevisionDate;
        string revisionReason;
        string totalPanelQuantity;
        string fixingHoleText;
        private string perfOption;
        private int printPDF;
        private string centerType;

        public bool drawPerfOnFirstPanel { get; set; }

        //Rhino Para Variables
        public double rowSpacing { get; set; }
        public double colSpacing { get; set; }
        public double labelHeight { get; set; }
        public string project { get; set; }
        public string customerName { get; set; }
        public string jobNo { get; set; }
        public string material { get; set; }
        public string coating { get; set; }
        public int totalPanel { get; set; }
        public int dotFont { get; set; }
        public int patternDirection { get; set; }
        public int revision { get; set; }
        public String colour { get; set; }
        public String drafterName { get; set; }
   

        //For Page Summary
        public string CustomerOrderNo { get; set; }
        public string MetrixPartNo { get; set; }
        public string MetrixSalesNo { get; set; }
        public double TotalPanelSQM { get; set; }
        public string TotalPrice { get; set; }
        /// <summary>
        /// Gets or sets the name of the part.
        /// </summary>
        /// <value>
        /// The name of the part.
        /// </value>
        public String PartName
        {
            get
            {
                return partName;
            }
            set
            {
                partName = value;
            }
        }

        /// <summary></summary>
        /// <value>
        /// The x.
        /// </value>
        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }

        }

        /// <summary></summary>
        /// <value>
        /// The x.
        /// </value>
        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        public double Area
        {
            get
            {
                return x * y / 1000000;
            }
        }

        /// <summary></summary>
        /// <value>
        /// The x.
        /// </value>
        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
            }
        }

        /// <summary>
        /// Gets or sets the left border.
        /// </summary>
        /// <value>
        /// The left border.
        /// </value>
        public double LeftBorder
        {
            get
            {
                return leftBorder;
            }
            set
            {
                leftBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the right border.
        /// </summary>
        /// <value>
        /// The right border.
        /// </value>
        public double RightBorder
        {
            get
            {
                return rightBorder;
            }
            set
            {
                rightBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the top border.
        /// </summary>
        /// <value>
        /// The top border.
        /// </value>
        public double TopBorder
        {
            get
            {
                return topBorder;
            }
            set
            {
                topBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom border.
        /// </summary>
        /// <value>
        /// The bottom border.
        /// </value>
        public double BottomBorder
        {
            get
            {
                return bottomBorder;
            }
            set
            {
                bottomBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the pattern.
        /// </summary>
        /// <value>
        /// The name of the pattern.
        /// </value>
        public String PatternName
        {
            get
            {
                return patternName;
            }
            set
            {
                patternName = value;
            }
        }


        /// <summary>
        /// Gets or sets the panel number.
        /// </summary>
        /// <value>
        /// The panel number.
        /// </value>
        public int PanelNumber
        {
            get
            {
                return panelNumber;
            }
            set
            {
                panelNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the perimeter.
        /// </summary>
        /// <value>
        /// The perimeter.
        /// </value>
        public Guid Perimeter
        {
            get { return perimeter; }
            set { perimeter = value; }
        }

        /// <summary>
        /// Gets or sets the perimeter.
        /// </summary>
        /// <value>
        /// The perimeter.
        /// </value>
        public Guid Border
        {
            get { return border; }
            set { border = value; }
        }

        /// <summary>
        /// Gets or sets the x dim.
        /// </summary>
        /// <value>
        /// The x dim.
        /// </value>
        public Guid XDim
        {
            get { return xDim; }
            set { xDim = value; }
        }

        /// <summary>
        /// Gets or sets the x dim.
        /// </summary>
        /// <value>
        /// The x dim.
        /// </value>
        public Guid YDim
        {
            get { return yDim; }
            set { yDim = value; }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public Guid Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// Gets or sets the draw perf.
        /// </summary>
        /// <value>
        /// The draw perf.
        /// </value>
        public int DrawPerf
        {
            get
            {
                return drawPerf;
            }

            set
            {
                drawPerf = value;
            }
        }

        /// <summary>
        /// Gets or sets the Dot Font .
        /// </summary>
        /// <value>
        /// Adds the dot font label
        /// </value>
        public int DotFontLabel
        {
            get
            {
                return dotFontLabel;
            }

            set
            {
                dotFontLabel = value;
            }
        }

        /// <summary>
        /// Gets or sets the dxf files required  .
        /// </summary>
        /// <value>
        /// Creates dxf files based on the input.
        /// </value>
        public String DXFFilesRequired
        {
            get
            {
                return dXFFilesRequired;
            }
            set
            {
                dXFFilesRequired = value;
            }
        }



        /// <summary>
        /// Gets or sets the dxf files required  .
        /// </summary>
        /// <value>
        /// Creates dxf files based on the input.
        /// </value>
        public int PanelQuantity
        {
            get
            {
                return panelQuantity;
            }
            set
            {
                panelQuantity = value;
            }
        }

        public Guid Fold
        {
            get
            {
                return fold;
            }
            set
            {
                fold = value;
            }
        }

        /// <summary>
        /// Gets or sets the leftFixingHoleDistance
        /// </summary>
        /// <value>
        /// The left leftFixingHoleDistance.
        /// </value>
        public double LeftFixingHoleDistance
        {
            get
            {
                return leftFixingHoleDistance;
            }
            set
            {
                leftFixingHoleDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the right border.
        /// </summary>
        /// <value>
        /// The right border.
        /// </value>
        public double RightFixingHoleDistance
        {
            get
            {
                return rightFixingHoleDistance;
            }
            set
            {
                rightFixingHoleDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the top border.
        /// </summary>
        /// <value>
        /// The top border.
        /// </value>
        public double TopFixingHoleDistance
        {
            get
            {
                return topFixingHoleDistance;
            }
            set
            {
                topFixingHoleDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom border.
        /// </summary>
        /// <value>
        /// The bottom border.
        /// </value>
        public double BottomFixingHoleDistance
        {
            get
            {
                return bottomFixingHoleDistance;
            }
            set
            {
                bottomFixingHoleDistance = value;
            }
        }

        public string FixingHoles { get => fixingHoles; set => fixingHoles = value; }
        public double HoleDiameter { get => holeDiameter; set => holeDiameter = value; }


        public string TopFixingHoles { get => topFixingHoles; set => topFixingHoles = value; }
        public int TopFixingHoleQuantity { get => topFixingHoleQuantity; set => topFixingHoleQuantity = value; }
        public double DistanceTopCenter { get => distanceTopCenter; set => distanceTopCenter = value; }
        public double TopHoleSetbackLeft { get => topHoleSetbackLeft; set => topHoleSetbackLeft = value; }
        public double TopHoleSetbackRight { get => topHoleSetbackRight; set => topHoleSetbackRight = value; }
        public double TopHoleSetbackTop { get => topHoleSetbackTop; set => topHoleSetbackTop = value; }

        public string BottomFixingHoles { get => bottomFixingHoles; set => bottomFixingHoles = value; }
        public int BottomFixingHoleQuantity { get => bottomFixingHoleQuantity; set => bottomFixingHoleQuantity = value; }
        public double DistanceBottomCenter { get => distanceBottomCenter; set => distanceBottomCenter = value; }
        public double BottomHoleSetbackLeft { get => bottomHoleSetbackLeft; set => bottomHoleSetbackLeft = value; }
        public double BottomHoleSetbackRight { get => bottomHoleSetbackRight; set => bottomHoleSetbackRight = value; }
        public double BottomHoleSetbackBottom { get => bottomHoleSetbackBottom; set => bottomHoleSetbackBottom = value; }

        public string LeftFixingHoles { get => leftFixingHoles; set => leftFixingHoles = value; }
        public int LeftFixingHoleQuantity { get => leftFixingHoleQuantity; set => leftFixingHoleQuantity = value; }
        public double DistanceLeftCenter { get => distanceLeftCenter; set => distanceLeftCenter = value; }
        public double LeftHoleSetbackTop { get => leftHoleSetbackTop; set => leftHoleSetbackTop = value; }
        public double LeftHoleSetbackBottom { get => leftHoleSetbackBottom; set => leftHoleSetbackBottom = value; }
        public double LeftHoleSetbackLeft { get => leftHoleSetbackLeft; set => leftHoleSetbackLeft = value; }

        public string RightFixingHoles { get => rightFixingHoles; set => rightFixingHoles = value; }
        public int RightFixingHoleQuantity { get => rightFixingHoleQuantity; set => rightFixingHoleQuantity = value; }
        public double DistanceRightCenter { get => distanceRightCenter; set => distanceRightCenter = value; }
        public double RightHoleSetbackTop { get => rightHoleSetbackTop; set => rightHoleSetbackTop = value; }
        public double RightHoleSetbackBottom { get => rightHoleSetbackBottom; set => rightHoleSetbackBottom = value; }
        public double RightHoleSetbackRight { get => rightHoleSetbackRight; set => rightHoleSetbackRight = value; }

        public string PerfText { get => perfText; set => perfText = value; }
        public double DistanceProvided { get => distanceProvided; set => distanceProvided = value; }
        public double ToleranceProvided { get => toleranceProvided; set => toleranceProvided = value; }
        public string PatternOpenArea { get => patternOpenArea; set => patternOpenArea = value; }
        public string DotFontLabellerSide { get => dotFontLabellerSide; set => dotFontLabellerSide = value; }

        public string PanelType { get => panelType; set => panelType = value; }

        public double DistanceProvidedTop { get => distanceProvidedTop; set => distanceProvidedTop = value; }
        public double DistanceProvidedBottom { get => distanceProvidedBottom; set => distanceProvidedBottom = value; }
        public double DistanceProvidedLeft { get => distanceProvidedLeft; set => distanceProvidedLeft = value; }
        public double DistanceProvidedRight { get => distanceProvidedRight; set => distanceProvidedRight = value; }

        public int FixingHoleCentres { get => fixingHoleCentres; set => fixingHoleCentres = value; }
        public int FoldWithFixingHoles { get => foldWithFixingHoles; set => foldWithFixingHoles = value; }
        public string FirstRevisionDate { get => firstRevisionDate; set => firstRevisionDate = value; }
        public string RevisionReason { get => revisionReason; set => revisionReason = value; }
        public string TotalPanelQuantity { get => totalPanelQuantity; set => totalPanelQuantity = value; }
        public string FixingHoleText { get => fixingHoleText; set => fixingHoleText = value; }
        public string PerfOption { get => perfOption; set => perfOption = value; }
        public int PrintPDF { get => printPDF; set => printPDF = value; }
        public string CenterType { get => centerType; set => centerType = value; }
    }

}





