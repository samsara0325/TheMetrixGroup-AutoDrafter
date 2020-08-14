using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins
{
    public class FoldedPerforationPanel : PerforationPanel
    {

        private double kFactor;
        private int cornerRelief;
        private double cornerReliefSize;
        private int topFoldType;
        private int bottomFoldType;
        private int leftFoldType;
        private int rightFoldType;
        private String topLeftSetback;
        private String bottomleftSetback;
        private String topRightSetback;
        private String bottomRightSetback;
        private double sheetThickness;

        private int topFirstFoldDirection;
        private double topFirstFoldWidth;
        private double topFirstFoldSetbackLeft;
        private double topFirstFoldSetbackRight;
        private double topSecondFoldWidth;
        private double topSecondFoldSetbackLeft;
        private double topSecondFoldSetbackRight;
        private double topFoldRadius;
        private int topSecondFoldDirection;
        private int topSecondFoldRequired;

        private int bottomFirstFoldDirection;
        private double bottomFirstFoldWidth;
        private double bottomFirstFoldSetbackLeft;
        private double bottomFirstFoldSetbackRight;
        private double bottomSecondFoldWidth;
        private double bottomSecondFoldSetbackLeft;
        private double bottomSecondFoldSetbackRight;
        private double bottomFoldRadius;
        private int bottomSecondFoldDirection;
        private int bottomSecondFoldRequired;

        private int leftFirstFoldDirection;
        private double leftFirstFoldWidth;
        private double leftFirstFoldSetbackTop;
        private double leftFirstFoldSetbackBottom;
        private double leftSecondFoldWidth;
        private double leftSecondFoldSetbackTop;
        private double leftSecondFoldSetbackBottom;
        private int leftSecondFoldDirection;
        private double leftFoldRadius;
        private int leftSecondFoldRequired;

        private int rightFirstFoldDirection;
        private double rightFirstFoldWidth;
        private double rightFirstFoldSetbackTop;
        private double rightFirstFoldSetbackBottom;
        private double rightSecondFoldWidth;
        private double rightSecondFoldSetbackTop;
        private double rightSecondFoldSetbackBottom;
        private int rightSecondFoldDirection;
        private double rightFoldRadius;
        private int rightSecondFoldRequired;

        private int topFold;
        private int bottomFold;
        private int rightFold;
        private int leftFold;

        private Guid folds;

        private int numberOfFolds;
        private String sideRequired;
        private Boolean onFolds; //to distinguish whether the user wants the fixing holes on folds or foled panel finished


        private String doubleFoldsRequired;
        private String topFoldLeftType;
        private String topFoldRightType;
        private String bottomFoldLeftType;
        private String bottomFoldRightType;
        private String leftFoldTopType;
        private String leftFoldBottomType;
        private String rightFoldTopType;
        private String rightFoldBottomType;
        private string totalPanelQuantity;

        public FoldedPerforationPanel() { }









        /// <summary>
        /// Gets or sets the perimeter.
        /// </summary>
        /// <value>
        /// The perimeter.
        /// </value>
        public Guid Folds
        {
            get { return folds; }
            set { folds = value; }
        }

        /// <summary>
        /// Gets or sets the numberOfFolds.
        /// </summary>
        /// <value>
        /// THe numberOfFolds
        /// </value>
        public int NumberOfFolds
        {
            get { return numberOfFolds; }
            set { numberOfFolds = value; }
        }



        public double KFactor { get => kFactor; set => kFactor = value; }
        public int CornerRelief { get => cornerRelief; set => cornerRelief = value; }
        public double CornerReliefSize { get => cornerReliefSize; set => cornerReliefSize = value; }
        public int TopFoldType { get => topFoldType; set => topFoldType = value; }
        public int BottomFoldType { get => bottomFoldType; set => bottomFoldType = value; }
        public int LeftFoldType { get => leftFoldType; set => leftFoldType = value; }
        public int RightFoldType { get => rightFoldType; set => rightFoldType = value; }
        public string TopLeftSetback { get => topLeftSetback; set => topLeftSetback = value; }
        public string BottomleftSetback { get => bottomleftSetback; set => bottomleftSetback = value; }
        public string TopRightSetback { get => topRightSetback; set => topRightSetback = value; }
        public string BottomRightSetback { get => bottomRightSetback; set => bottomRightSetback = value; }
        public string SideRequired { get => sideRequired; set => sideRequired = value; }

        public int TopFirstFoldDirection { get => topFirstFoldDirection; set => topFirstFoldDirection = value; }
        public double TopFirstFoldWidth { get => topFirstFoldWidth; set => topFirstFoldWidth = value; }
        public double TopFirstFoldSetbackLeft { get => topFirstFoldSetbackLeft; set => topFirstFoldSetbackLeft = value; }
        public double TopFirstFoldSetbackRight { get => topFirstFoldSetbackRight; set => topFirstFoldSetbackRight = value; }
        public double TopSecondFoldWidth { get => topSecondFoldWidth; set => topSecondFoldWidth = value; }
        public double TopSecondFoldSetbackLeft { get => topSecondFoldSetbackLeft; set => topSecondFoldSetbackLeft = value; }
        public double TopSecondFoldSetbackRight { get => topSecondFoldSetbackRight; set => topSecondFoldSetbackRight = value; }

        public int BottomFirstFoldDirection { get => bottomFirstFoldDirection; set => bottomFirstFoldDirection = value; }
        public double BottomFirstFoldWidth { get => bottomFirstFoldWidth; set => bottomFirstFoldWidth = value; }
        public double BottomFirstFoldSetbackLeft { get => bottomFirstFoldSetbackLeft; set => bottomFirstFoldSetbackLeft = value; }
        public double BottomFirstFoldSetbackRight { get => bottomFirstFoldSetbackRight; set => bottomFirstFoldSetbackRight = value; }
        public double BottomSecondFoldWidth { get => bottomSecondFoldWidth; set => bottomSecondFoldWidth = value; }
        public double BottomSecondFoldSetbackLeft { get => bottomSecondFoldSetbackLeft; set => bottomSecondFoldSetbackLeft = value; }
        public double BottomSecondFoldSetbackRight { get => bottomSecondFoldSetbackRight; set => bottomSecondFoldSetbackRight = value; }


        public int LeftFirstFoldDirection { get => leftFirstFoldDirection; set => leftFirstFoldDirection = value; }
        public double LeftFirstFoldWidth { get => leftFirstFoldWidth; set => leftFirstFoldWidth = value; }
        public double LeftFirstFoldSetbackTop { get => leftFirstFoldSetbackTop; set => leftFirstFoldSetbackTop = value; }
        public double LeftFirstFoldSetbackBottom { get => leftFirstFoldSetbackBottom; set => leftFirstFoldSetbackBottom = value; }
        public double LeftSecondFoldWidth { get => leftSecondFoldWidth; set => leftSecondFoldWidth = value; }
        public double LeftSecondFoldSetbackTop { get => leftSecondFoldSetbackTop; set => leftSecondFoldSetbackTop = value; }
        public double LeftSecondFoldSetbackBottom { get => leftSecondFoldSetbackBottom; set => leftSecondFoldSetbackBottom = value; }


        public int RightFirstFoldDirection { get => rightFirstFoldDirection; set => rightFirstFoldDirection = value; }
        public double RightFirstFoldWidth { get => rightFirstFoldWidth; set => rightFirstFoldWidth = value; }
        public double RightFirstFoldSetbackTop { get => rightFirstFoldSetbackTop; set => rightFirstFoldSetbackTop = value; }
        public double RightFirstFoldSetbackBottom { get => rightFirstFoldSetbackBottom; set => rightFirstFoldSetbackBottom = value; }
        public double RightSecondFoldWidth { get => rightSecondFoldWidth; set => rightSecondFoldWidth = value; }
        public double RightSecondFoldSetbackTop { get => rightSecondFoldSetbackTop; set => rightSecondFoldSetbackTop = value; }
        public double RightSecondFoldSetbackBottom { get => rightSecondFoldSetbackBottom; set => rightSecondFoldSetbackBottom = value; }
        public double SheetThickness { get => sheetThickness; set => sheetThickness = value; }
        public double TopFoldRadius { get => topFoldRadius; set => topFoldRadius = value; }
        public double BottomFoldRadius { get => bottomFoldRadius; set => bottomFoldRadius = value; }
        public double LeftFoldRadius { get => leftFoldRadius; set => leftFoldRadius = value; }
        public double RightFoldRadius { get => rightFoldRadius; set => rightFoldRadius = value; }

        public int TopFold { get => topFold; set => topFold = value; }
        public int BottomFold { get => bottomFold; set => bottomFold = value; }
        public int LeftFold { get => leftFold; set => leftFold = value; }
        public int RightFold { get => rightFold; set => rightFold = value; }

        public int TopSecondFoldRequired { get => topSecondFoldRequired; set => topSecondFoldRequired = value; }
        public int BottomSecondFoldRequired { get => bottomSecondFoldRequired; set => bottomSecondFoldRequired = value; }
        public int LeftSecondFoldRequired { get => leftSecondFoldRequired; set => leftSecondFoldRequired = value; }
        public int RightSecondFoldRequired { get => rightSecondFoldRequired; set => rightSecondFoldRequired = value; }

        public Boolean OnFolds { get => onFolds; set => onFolds = value; }

        //Double fold directions
        public int TopSecondFoldDirection { get => topSecondFoldDirection; set => topSecondFoldDirection = value; }
        public int BottomSecondFoldDirection { get => bottomSecondFoldDirection; set => bottomSecondFoldDirection = value; }
        public int LeftSecondFoldDirection { get => leftSecondFoldDirection; set => leftSecondFoldDirection = value; }
        public int RightSecondFoldDirection { get => rightSecondFoldDirection; set => rightSecondFoldDirection = value; }

        public string TopFoldLeftType { get => topFoldLeftType; set => topFoldLeftType = value; }
        public string TopFoldRightType { get => topFoldRightType; set => topFoldRightType = value; }
        public string BottomFoldLeftType { get => bottomFoldLeftType; set => bottomFoldLeftType = value; }
        public string BottomFoldRightType { get => bottomFoldRightType; set => bottomFoldRightType = value; }
        public string LeftFoldTopType { get => leftFoldTopType; set => leftFoldTopType = value; }
        public string LeftFoldBottomType { get => leftFoldBottomType; set => leftFoldBottomType = value; }
        public string RightFoldTopType { get => rightFoldTopType; set => rightFoldTopType = value; }
        public string RightFoldBottomType { get => rightFoldBottomType; set => rightFoldBottomType = value; }

        public string DoubleFoldsRequired { get => doubleFoldsRequired; set => doubleFoldsRequired = value; }

    }
}
