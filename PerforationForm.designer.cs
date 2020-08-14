namespace MetrixGroupPlugins
{
    partial class PerforationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerforationForm));
            this.buttonPerforate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxPattern = new System.Windows.Forms.ComboBox();
            this.comboBoxPunchingTool = new System.Windows.Forms.ComboBox();
            this.labelTool1DimensionX = new System.Windows.Forms.Label();
            this.labelTool1DimensionY = new System.Windows.Forms.Label();
            this.textBoxToolDimensionY = new System.Windows.Forms.TextBox();
            this.textBoxToolDimensionX = new System.Windows.Forms.TextBox();
            this.textBoxXSpacing = new System.Windows.Forms.TextBox();
            this.textBoxYSpacing = new System.Windows.Forms.TextBox();
            this.labelYSpacing = new System.Windows.Forms.Label();
            this.labelXSpacing = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxTool = new System.Windows.Forms.GroupBox();
            this.chckBxEnableToolHit = new System.Windows.Forms.CheckBox();
            this.checkBoxEnablePerforation = new System.Windows.Forms.CheckBox();
            this.checkBoxRotated = new System.Windows.Forms.CheckBox();
            this.labelToolGap = new System.Windows.Forms.Label();
            this.textBoxToolGap = new System.Windows.Forms.TextBox();
            this.checkBoxOverPunch = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxYMultiplier = new System.Windows.Forms.ComboBox();
            this.comboBoxPinsInY = new System.Windows.Forms.ComboBox();
            this.comboBoxXMultiplier = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPinsInX = new System.Windows.Forms.ComboBox();
            this.comboBoxClusterShape = new System.Windows.Forms.ComboBox();
            this.checkBoxClusterTool = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelAtomic = new System.Windows.Forms.Label();
            this.textBoxAtomicNumber = new System.Windows.Forms.TextBox();
            this.pictureBoxPattern = new System.Windows.Forms.PictureBox();
            this.listBoxPunchingTool = new System.Windows.Forms.ListBox();
            this.labelRandomness = new System.Windows.Forms.Label();
            this.textBoxRandomness = new System.Windows.Forms.TextBox();
            this.labelPitch = new System.Windows.Forms.Label();
            this.textBoxPitch = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonLoadDesign = new System.Windows.Forms.Button();
            this.comboBoxDesign = new System.Windows.Forms.ComboBox();
            this.errorProviderPitch = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderXSpacing = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderYSpacing = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderRandomness = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderToolX = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderToolY = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderAtomicNumber = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderOpenArea = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.errorProviderAreaPercentage = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderToolGap = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderAtomic = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTipDesignFile = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxTool.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPattern)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderXSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderYSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderRandomness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAtomicNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderOpenArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAreaPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAtomic)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPerforate
            // 
            this.buttonPerforate.Location = new System.Drawing.Point(341, 522);
            this.buttonPerforate.Name = "buttonPerforate";
            this.buttonPerforate.Size = new System.Drawing.Size(75, 23);
            this.buttonPerforate.TabIndex = 3;
            this.buttonPerforate.Text = "&Perforate";
            this.buttonPerforate.UseVisualStyleBackColor = true;
            this.buttonPerforate.Click += new System.EventHandler(this.buttonPerforate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(422, 522);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboBoxPattern
            // 
            this.comboBoxPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPattern.FormattingEnabled = true;
            this.comboBoxPattern.Location = new System.Drawing.Point(6, 19);
            this.comboBoxPattern.Name = "comboBoxPattern";
            this.comboBoxPattern.Size = new System.Drawing.Size(473, 21);
            this.comboBoxPattern.TabIndex = 0;
            this.comboBoxPattern.SelectedIndexChanged += new System.EventHandler(this.comboBoxPattern_SelectedIndexChanged);
            // 
            // comboBoxPunchingTool
            // 
            this.comboBoxPunchingTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPunchingTool.FormattingEnabled = true;
            this.comboBoxPunchingTool.Location = new System.Drawing.Point(6, 19);
            this.comboBoxPunchingTool.Name = "comboBoxPunchingTool";
            this.comboBoxPunchingTool.Size = new System.Drawing.Size(299, 21);
            this.comboBoxPunchingTool.TabIndex = 0;
            this.comboBoxPunchingTool.SelectedIndexChanged += new System.EventHandler(this.comboBoxPunchingTool_SelectedIndexChanged);
            // 
            // labelTool1DimensionX
            // 
            this.labelTool1DimensionX.AutoSize = true;
            this.labelTool1DimensionX.Location = new System.Drawing.Point(6, 49);
            this.labelTool1DimensionX.Name = "labelTool1DimensionX";
            this.labelTool1DimensionX.Size = new System.Drawing.Size(12, 13);
            this.labelTool1DimensionX.TabIndex = 1;
            this.labelTool1DimensionX.Text = "x";
            this.labelTool1DimensionX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTool1DimensionY
            // 
            this.labelTool1DimensionY.AutoSize = true;
            this.labelTool1DimensionY.Location = new System.Drawing.Point(162, 49);
            this.labelTool1DimensionY.Name = "labelTool1DimensionY";
            this.labelTool1DimensionY.Size = new System.Drawing.Size(12, 13);
            this.labelTool1DimensionY.TabIndex = 3;
            this.labelTool1DimensionY.Text = "y";
            // 
            // textBoxToolDimensionY
            // 
            this.textBoxToolDimensionY.Location = new System.Drawing.Point(242, 46);
            this.textBoxToolDimensionY.Name = "textBoxToolDimensionY";
            this.textBoxToolDimensionY.Size = new System.Drawing.Size(50, 20);
            this.textBoxToolDimensionY.TabIndex = 4;
            this.textBoxToolDimensionY.TextChanged += new System.EventHandler(this.textBoxToolDimensionY_TextChanged);
            // 
            // textBoxToolDimensionX
            // 
            this.textBoxToolDimensionX.Location = new System.Drawing.Point(78, 46);
            this.textBoxToolDimensionX.Name = "textBoxToolDimensionX";
            this.textBoxToolDimensionX.Size = new System.Drawing.Size(50, 20);
            this.textBoxToolDimensionX.TabIndex = 2;
            this.textBoxToolDimensionX.TextChanged += new System.EventHandler(this.textBoxToolDimensionX_TextChanged);
            // 
            // textBoxXSpacing
            // 
            this.textBoxXSpacing.Location = new System.Drawing.Point(246, 72);
            this.textBoxXSpacing.Name = "textBoxXSpacing";
            this.textBoxXSpacing.Size = new System.Drawing.Size(50, 20);
            this.textBoxXSpacing.TabIndex = 4;
            this.textBoxXSpacing.TextChanged += new System.EventHandler(this.textBoxXSpacing_TextChanged);
            // 
            // textBoxYSpacing
            // 
            this.textBoxYSpacing.Location = new System.Drawing.Point(410, 72);
            this.textBoxYSpacing.Name = "textBoxYSpacing";
            this.textBoxYSpacing.Size = new System.Drawing.Size(50, 20);
            this.textBoxYSpacing.TabIndex = 6;
            this.textBoxYSpacing.TextChanged += new System.EventHandler(this.textBoxYSpacing_TextChanged);
            // 
            // labelYSpacing
            // 
            this.labelYSpacing.AutoSize = true;
            this.labelYSpacing.Location = new System.Drawing.Point(330, 75);
            this.labelYSpacing.Name = "labelYSpacing";
            this.labelYSpacing.Size = new System.Drawing.Size(52, 13);
            this.labelYSpacing.TabIndex = 5;
            this.labelYSpacing.Text = "y spacing";
            // 
            // labelXSpacing
            // 
            this.labelXSpacing.AutoSize = true;
            this.labelXSpacing.Location = new System.Drawing.Point(163, 75);
            this.labelXSpacing.Name = "labelXSpacing";
            this.labelXSpacing.Size = new System.Drawing.Size(52, 13);
            this.labelXSpacing.TabIndex = 3;
            this.labelXSpacing.Text = "x spacing";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 156);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Pins in x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Pins in y";
            // 
            // groupBoxTool
            // 
            this.groupBoxTool.Controls.Add(this.chckBxEnableToolHit);
            this.groupBoxTool.Controls.Add(this.checkBoxEnablePerforation);
            this.groupBoxTool.Controls.Add(this.checkBoxRotated);
            this.groupBoxTool.Controls.Add(this.labelToolGap);
            this.groupBoxTool.Controls.Add(this.textBoxToolGap);
            this.groupBoxTool.Controls.Add(this.checkBoxOverPunch);
            this.groupBoxTool.Controls.Add(this.label2);
            this.groupBoxTool.Controls.Add(this.comboBoxYMultiplier);
            this.groupBoxTool.Controls.Add(this.comboBoxPinsInY);
            this.groupBoxTool.Controls.Add(this.comboBoxXMultiplier);
            this.groupBoxTool.Controls.Add(this.label4);
            this.groupBoxTool.Controls.Add(this.label1);
            this.groupBoxTool.Controls.Add(this.comboBoxPinsInX);
            this.groupBoxTool.Controls.Add(this.comboBoxClusterShape);
            this.groupBoxTool.Controls.Add(this.comboBoxPunchingTool);
            this.groupBoxTool.Controls.Add(this.textBoxToolDimensionX);
            this.groupBoxTool.Controls.Add(this.label3);
            this.groupBoxTool.Controls.Add(this.labelTool1DimensionX);
            this.groupBoxTool.Controls.Add(this.labelTool1DimensionY);
            this.groupBoxTool.Controls.Add(this.label8);
            this.groupBoxTool.Controls.Add(this.textBoxToolDimensionY);
            this.groupBoxTool.Controls.Add(this.checkBoxClusterTool);
            this.groupBoxTool.Location = new System.Drawing.Point(168, 203);
            this.groupBoxTool.Name = "groupBoxTool";
            this.groupBoxTool.Size = new System.Drawing.Size(311, 238);
            this.groupBoxTool.TabIndex = 9;
            this.groupBoxTool.TabStop = false;
            this.groupBoxTool.Text = "Tool 1";
            // 
            // chckBxEnableToolHit
            // 
            this.chckBxEnableToolHit.AutoSize = true;
            this.chckBxEnableToolHit.Location = new System.Drawing.Point(205, 100);
            this.chckBxEnableToolHit.Name = "chckBxEnableToolHit";
            this.chckBxEnableToolHit.Size = new System.Drawing.Size(100, 17);
            this.chckBxEnableToolHit.TabIndex = 22;
            this.chckBxEnableToolHit.Text = "Enable Tool hit ";
            this.chckBxEnableToolHit.UseVisualStyleBackColor = true;
            this.chckBxEnableToolHit.CheckedChanged += new System.EventHandler(this.chckBxEnableToolHit_CheckedChanged);
            // 
            // checkBoxEnablePerforation
            // 
            this.checkBoxEnablePerforation.AutoSize = true;
            this.checkBoxEnablePerforation.Checked = true;
            this.checkBoxEnablePerforation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnablePerforation.Location = new System.Drawing.Point(95, 100);
            this.checkBoxEnablePerforation.Name = "checkBoxEnablePerforation";
            this.checkBoxEnablePerforation.Size = new System.Drawing.Size(113, 17);
            this.checkBoxEnablePerforation.TabIndex = 23;
            this.checkBoxEnablePerforation.Text = "Enable Perforation";
            this.checkBoxEnablePerforation.UseVisualStyleBackColor = true;
            this.checkBoxEnablePerforation.CheckedChanged += new System.EventHandler(this.checkBoxEnablePerforation_CheckedChanged);
            // 
            // checkBoxRotated
            // 
            this.checkBoxRotated.AutoSize = true;
            this.checkBoxRotated.Location = new System.Drawing.Point(165, 207);
            this.checkBoxRotated.Name = "checkBoxRotated";
            this.checkBoxRotated.Size = new System.Drawing.Size(79, 17);
            this.checkBoxRotated.TabIndex = 21;
            this.checkBoxRotated.Text = "Rotated 45";
            this.checkBoxRotated.UseVisualStyleBackColor = true;
            this.checkBoxRotated.CheckedChanged += new System.EventHandler(this.checkBoxRotatable_CheckedChanged);
            // 
            // labelToolGap
            // 
            this.labelToolGap.AutoSize = true;
            this.labelToolGap.Location = new System.Drawing.Point(6, 75);
            this.labelToolGap.Name = "labelToolGap";
            this.labelToolGap.Size = new System.Drawing.Size(27, 13);
            this.labelToolGap.TabIndex = 20;
            this.labelToolGap.Text = "Gap";
            this.labelToolGap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxToolGap
            // 
            this.textBoxToolGap.Location = new System.Drawing.Point(78, 72);
            this.textBoxToolGap.Name = "textBoxToolGap";
            this.textBoxToolGap.Size = new System.Drawing.Size(50, 20);
            this.textBoxToolGap.TabIndex = 19;
            this.textBoxToolGap.TextChanged += new System.EventHandler(this.textBoxToolGap_TextChanged);
            // 
            // checkBoxOverPunch
            // 
            this.checkBoxOverPunch.AutoSize = true;
            this.checkBoxOverPunch.Location = new System.Drawing.Point(6, 207);
            this.checkBoxOverPunch.Name = "checkBoxOverPunch";
            this.checkBoxOverPunch.Size = new System.Drawing.Size(108, 17);
            this.checkBoxOverPunch.TabIndex = 16;
            this.checkBoxOverPunch.Text = "Allow over punch";
            this.checkBoxOverPunch.UseVisualStyleBackColor = true;
            this.checkBoxOverPunch.CheckedChanged += new System.EventHandler(this.checkBoxOverPunch_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "y multiplier";
            // 
            // comboBoxYMultiplier
            // 
            this.comboBoxYMultiplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxYMultiplier.FormattingEnabled = true;
            this.comboBoxYMultiplier.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxYMultiplier.Location = new System.Drawing.Point(242, 183);
            this.comboBoxYMultiplier.Name = "comboBoxYMultiplier";
            this.comboBoxYMultiplier.Size = new System.Drawing.Size(50, 21);
            this.comboBoxYMultiplier.TabIndex = 15;
            this.comboBoxYMultiplier.SelectedIndexChanged += new System.EventHandler(this.comboBoxYMultiplier_SelectedIndexChanged);
            // 
            // comboBoxPinsInY
            // 
            this.comboBoxPinsInY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPinsInY.FormattingEnabled = true;
            this.comboBoxPinsInY.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.comboBoxPinsInY.Location = new System.Drawing.Point(242, 153);
            this.comboBoxPinsInY.Name = "comboBoxPinsInY";
            this.comboBoxPinsInY.Size = new System.Drawing.Size(50, 21);
            this.comboBoxPinsInY.TabIndex = 11;
            this.comboBoxPinsInY.SelectedIndexChanged += new System.EventHandler(this.comboBoxPinsInY_SelectedIndexChanged);
            // 
            // comboBoxXMultiplier
            // 
            this.comboBoxXMultiplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxXMultiplier.FormattingEnabled = true;
            this.comboBoxXMultiplier.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxXMultiplier.Location = new System.Drawing.Point(78, 180);
            this.comboBoxXMultiplier.Name = "comboBoxXMultiplier";
            this.comboBoxXMultiplier.Size = new System.Drawing.Size(50, 21);
            this.comboBoxXMultiplier.TabIndex = 13;
            this.comboBoxXMultiplier.SelectedIndexChanged += new System.EventHandler(this.comboBoxXMultiplier_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Shape";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "x multiplier";
            // 
            // comboBoxPinsInX
            // 
            this.comboBoxPinsInX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPinsInX.FormattingEnabled = true;
            this.comboBoxPinsInX.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.comboBoxPinsInX.Location = new System.Drawing.Point(78, 153);
            this.comboBoxPinsInX.Name = "comboBoxPinsInX";
            this.comboBoxPinsInX.Size = new System.Drawing.Size(50, 21);
            this.comboBoxPinsInX.TabIndex = 9;
            this.comboBoxPinsInX.SelectedIndexChanged += new System.EventHandler(this.comboBoxPinsInX_SelectedIndexChanged);
            // 
            // comboBoxClusterShape
            // 
            this.comboBoxClusterShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClusterShape.FormattingEnabled = true;
            this.comboBoxClusterShape.Location = new System.Drawing.Point(78, 124);
            this.comboBoxClusterShape.Name = "comboBoxClusterShape";
            this.comboBoxClusterShape.Size = new System.Drawing.Size(100, 21);
            this.comboBoxClusterShape.TabIndex = 7;
            this.comboBoxClusterShape.SelectedIndexChanged += new System.EventHandler(this.comboBoxClusterShape_SelectedIndexChanged);
            // 
            // checkBoxClusterTool
            // 
            this.checkBoxClusterTool.AutoSize = true;
            this.checkBoxClusterTool.Location = new System.Drawing.Point(6, 100);
            this.checkBoxClusterTool.Name = "checkBoxClusterTool";
            this.checkBoxClusterTool.Size = new System.Drawing.Size(93, 17);
            this.checkBoxClusterTool.TabIndex = 5;
            this.checkBoxClusterTool.Text = "Enable cluster";
            this.checkBoxClusterTool.UseVisualStyleBackColor = true;
            this.checkBoxClusterTool.CheckedChanged += new System.EventHandler(this.checkBoxClusterTool_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelAtomic);
            this.groupBox2.Controls.Add(this.textBoxAtomicNumber);
            this.groupBox2.Controls.Add(this.pictureBoxPattern);
            this.groupBox2.Controls.Add(this.listBoxPunchingTool);
            this.groupBox2.Controls.Add(this.labelRandomness);
            this.groupBox2.Controls.Add(this.textBoxRandomness);
            this.groupBox2.Controls.Add(this.labelPitch);
            this.groupBox2.Controls.Add(this.textBoxPitch);
            this.groupBox2.Controls.Add(this.comboBoxPattern);
            this.groupBox2.Controls.Add(this.groupBoxTool);
            this.groupBox2.Controls.Add(this.labelXSpacing);
            this.groupBox2.Controls.Add(this.textBoxXSpacing);
            this.groupBox2.Controls.Add(this.labelYSpacing);
            this.groupBox2.Controls.Add(this.textBoxYSpacing);
            this.groupBox2.Location = new System.Drawing.Point(12, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(485, 447);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pattern";
            // 
            // labelAtomic
            // 
            this.labelAtomic.AutoSize = true;
            this.labelAtomic.Location = new System.Drawing.Point(330, 101);
            this.labelAtomic.Name = "labelAtomic";
            this.labelAtomic.Size = new System.Drawing.Size(65, 13);
            this.labelAtomic.TabIndex = 14;
            this.labelAtomic.Text = "No. of Tools";
            // 
            // textBoxAtomicNumber
            // 
            this.textBoxAtomicNumber.Location = new System.Drawing.Point(410, 98);
            this.textBoxAtomicNumber.Name = "textBoxAtomicNumber";
            this.textBoxAtomicNumber.Size = new System.Drawing.Size(50, 20);
            this.textBoxAtomicNumber.TabIndex = 15;
            this.toolTip.SetToolTip(this.textBoxAtomicNumber, "Number of tools ");
            this.textBoxAtomicNumber.TextChanged += new System.EventHandler(this.textBoxAtomicNumber_TextChanged);
            // 
            // pictureBoxPattern
            // 
            this.pictureBoxPattern.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPattern.Location = new System.Drawing.Point(6, 46);
            this.pictureBoxPattern.Name = "pictureBoxPattern";
            this.pictureBoxPattern.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxPattern.TabIndex = 11;
            this.pictureBoxPattern.TabStop = false;
            // 
            // listBoxPunchingTool
            // 
            this.listBoxPunchingTool.FormattingEnabled = true;
            this.listBoxPunchingTool.Location = new System.Drawing.Point(6, 203);
            this.listBoxPunchingTool.Name = "listBoxPunchingTool";
            this.listBoxPunchingTool.Size = new System.Drawing.Size(150, 238);
            this.listBoxPunchingTool.TabIndex = 10;
            this.listBoxPunchingTool.SelectedIndexChanged += new System.EventHandler(this.listBoxPunchingTool_SelectedIndexChanged);
            // 
            // labelRandomness
            // 
            this.labelRandomness.AutoSize = true;
            this.labelRandomness.Location = new System.Drawing.Point(160, 101);
            this.labelRandomness.Name = "labelRandomness";
            this.labelRandomness.Size = new System.Drawing.Size(69, 13);
            this.labelRandomness.TabIndex = 7;
            this.labelRandomness.Text = "Randomness";
            // 
            // textBoxRandomness
            // 
            this.textBoxRandomness.Location = new System.Drawing.Point(246, 98);
            this.textBoxRandomness.Name = "textBoxRandomness";
            this.textBoxRandomness.Size = new System.Drawing.Size(50, 20);
            this.textBoxRandomness.TabIndex = 8;
            this.textBoxRandomness.TextChanged += new System.EventHandler(this.textBoxRandomness_TextChanged);
            // 
            // labelPitch
            // 
            this.labelPitch.AutoSize = true;
            this.labelPitch.Location = new System.Drawing.Point(162, 49);
            this.labelPitch.Name = "labelPitch";
            this.labelPitch.Size = new System.Drawing.Size(31, 13);
            this.labelPitch.TabIndex = 1;
            this.labelPitch.Text = "Pitch";
            // 
            // textBoxPitch
            // 
            this.textBoxPitch.Location = new System.Drawing.Point(246, 46);
            this.textBoxPitch.Name = "textBoxPitch";
            this.textBoxPitch.Size = new System.Drawing.Size(50, 20);
            this.textBoxPitch.TabIndex = 2;
            this.textBoxPitch.TextChanged += new System.EventHandler(this.textBoxPitch_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonLoadDesign);
            this.groupBox4.Controls.Add(this.comboBoxDesign);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(485, 51);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Design";
            // 
            // buttonLoadDesign
            // 
            this.buttonLoadDesign.Location = new System.Drawing.Point(385, 19);
            this.buttonLoadDesign.Name = "buttonLoadDesign";
            this.buttonLoadDesign.Size = new System.Drawing.Size(94, 23);
            this.buttonLoadDesign.TabIndex = 1;
            this.buttonLoadDesign.Text = "Load Design";
            this.buttonLoadDesign.UseVisualStyleBackColor = true;
            this.buttonLoadDesign.Click += new System.EventHandler(this.buttonLoadDesign_Click);
            // 
            // comboBoxDesign
            // 
            this.comboBoxDesign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDesign.FormattingEnabled = true;
            this.comboBoxDesign.Items.AddRange(new object[] {
            "Custom"});
            this.comboBoxDesign.Location = new System.Drawing.Point(6, 19);
            this.comboBoxDesign.Name = "comboBoxDesign";
            this.comboBoxDesign.Size = new System.Drawing.Size(370, 21);
            this.comboBoxDesign.TabIndex = 0;
            this.comboBoxDesign.SelectedIndexChanged += new System.EventHandler(this.comboBoxDesign_SelectedIndexChanged);
            // 
            // errorProviderPitch
            // 
            this.errorProviderPitch.ContainerControl = this;
            // 
            // errorProviderXSpacing
            // 
            this.errorProviderXSpacing.ContainerControl = this;
            // 
            // errorProviderYSpacing
            // 
            this.errorProviderYSpacing.ContainerControl = this;
            // 
            // errorProviderRandomness
            // 
            this.errorProviderRandomness.ContainerControl = this;
            // 
            // errorProviderToolX
            // 
            this.errorProviderToolX.ContainerControl = this;
            // 
            // errorProviderToolY
            // 
            this.errorProviderToolY.ContainerControl = this;
            // 
            // errorProviderAtomicNumber
            // 
            this.errorProviderAtomicNumber.ContainerControl = this;
            // 
            // errorProviderOpenArea
            // 
            this.errorProviderOpenArea.ContainerControl = this;
            // 
            // errorProviderAreaPercentage
            // 
            this.errorProviderAreaPercentage.ContainerControl = this;
            // 
            // errorProviderToolGap
            // 
            this.errorProviderToolGap.ContainerControl = this;
            // 
            // errorProviderAtomic
            // 
            this.errorProviderAtomic.ContainerControl = this;
            // 
            // toolTipDesignFile
            // 
            this.toolTipDesignFile.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTipDesignFile_Popup);
            // 
            // PerforationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 549);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonPerforate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "PerforationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Metrix Group Perforation Creator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PerforationForm_FormClosed);
            this.Load += new System.EventHandler(this.PerforationForm_Load);
            this.groupBoxTool.ResumeLayout(false);
            this.groupBoxTool.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPattern)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderXSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderYSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderRandomness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAtomicNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderOpenArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAreaPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderToolGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderAtomic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonPerforate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxPattern;
        private System.Windows.Forms.ComboBox comboBoxPunchingTool;
        private System.Windows.Forms.Label labelTool1DimensionX;
        private System.Windows.Forms.Label labelTool1DimensionY;
        private System.Windows.Forms.TextBox textBoxToolDimensionY;
        private System.Windows.Forms.TextBox textBoxToolDimensionX;
        private System.Windows.Forms.TextBox textBoxXSpacing;
        private System.Windows.Forms.TextBox textBoxYSpacing;
        private System.Windows.Forms.Label labelYSpacing;
        private System.Windows.Forms.Label labelXSpacing;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxTool;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxClusterTool;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxYMultiplier;
        private System.Windows.Forms.ComboBox comboBoxXMultiplier;
        private System.Windows.Forms.ComboBox comboBoxPinsInY;
        private System.Windows.Forms.ComboBox comboBoxPinsInX;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxDesign;
        private System.Windows.Forms.Label labelPitch;
        private System.Windows.Forms.TextBox textBoxPitch;
        private System.Windows.Forms.ComboBox comboBoxClusterShape;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelRandomness;
        private System.Windows.Forms.TextBox textBoxRandomness;
        private System.Windows.Forms.ListBox listBoxPunchingTool;
        private System.Windows.Forms.PictureBox pictureBoxPattern;
        private System.Windows.Forms.ErrorProvider errorProviderPitch;
        private System.Windows.Forms.ErrorProvider errorProviderXSpacing;
        private System.Windows.Forms.ErrorProvider errorProviderYSpacing;
        private System.Windows.Forms.ErrorProvider errorProviderRandomness;
        private System.Windows.Forms.ErrorProvider errorProviderToolX;
        private System.Windows.Forms.ErrorProvider errorProviderToolY;
        private System.Windows.Forms.CheckBox checkBoxOverPunch;
        private System.Windows.Forms.Label labelAtomic;
        private System.Windows.Forms.TextBox textBoxAtomicNumber;
        private System.Windows.Forms.ErrorProvider errorProviderAtomicNumber;
        private System.Windows.Forms.ErrorProvider errorProviderOpenArea;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ErrorProvider errorProviderAreaPercentage;
        private System.Windows.Forms.Label labelToolGap;
        private System.Windows.Forms.TextBox textBoxToolGap;
        private System.Windows.Forms.ErrorProvider errorProviderToolGap;
      private System.Windows.Forms.ErrorProvider errorProviderAtomic;
      private System.Windows.Forms.CheckBox checkBoxRotated;
      private System.Windows.Forms.Button buttonLoadDesign;
      private System.Windows.Forms.ToolTip toolTipDesignFile;
      private System.Windows.Forms.CheckBox chckBxEnableToolHit;
        private System.Windows.Forms.CheckBox checkBoxEnablePerforation;
    }
}