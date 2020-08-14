namespace MetrixGroupPlugins
{
   partial class ClusterToolSearcherForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusterToolSearcherForm));
         this.checkBoxRotated = new System.Windows.Forms.CheckBox();
         this.checkBoxOverPunch = new System.Windows.Forms.CheckBox();
         this.label2 = new System.Windows.Forms.Label();
         this.comboBoxYMultiplier = new System.Windows.Forms.ComboBox();
         this.comboBoxPinsInY = new System.Windows.Forms.ComboBox();
         this.comboBoxXMultiplier = new System.Windows.Forms.ComboBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.comboBoxPinsInX = new System.Windows.Forms.ComboBox();
         this.comboBoxClusterShape = new System.Windows.Forms.ComboBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.labelXSpacing = new System.Windows.Forms.Label();
         this.textBoxXSpacing = new System.Windows.Forms.TextBox();
         this.labelYSpacing = new System.Windows.Forms.Label();
         this.textBoxYSpacing = new System.Windows.Forms.TextBox();
         this.errorProviderXSpacing = new System.Windows.Forms.ErrorProvider(this.components);
         this.buttonStart = new System.Windows.Forms.Button();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.errorProviderYSpacing = new System.Windows.Forms.ErrorProvider(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderXSpacing)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderYSpacing)).BeginInit();
         this.SuspendLayout();
         // 
         // checkBoxRotated
         // 
         this.checkBoxRotated.AutoSize = true;
         this.checkBoxRotated.Location = new System.Drawing.Point(171, 123);
         this.checkBoxRotated.Name = "checkBoxRotated";
         this.checkBoxRotated.Size = new System.Drawing.Size(79, 17);
         this.checkBoxRotated.TabIndex = 33;
         this.checkBoxRotated.Text = "Rotated 45";
         this.checkBoxRotated.UseVisualStyleBackColor = true;
         this.checkBoxRotated.CheckedChanged += new System.EventHandler(this.checkBoxRotated_CheckedChanged);
         // 
         // checkBoxOverPunch
         // 
         this.checkBoxOverPunch.AutoSize = true;
         this.checkBoxOverPunch.Location = new System.Drawing.Point(12, 123);
         this.checkBoxOverPunch.Name = "checkBoxOverPunch";
         this.checkBoxOverPunch.Size = new System.Drawing.Size(108, 17);
         this.checkBoxOverPunch.TabIndex = 32;
         this.checkBoxOverPunch.Text = "Allow over punch";
         this.checkBoxOverPunch.UseVisualStyleBackColor = true;
         this.checkBoxOverPunch.CheckedChanged += new System.EventHandler(this.checkBoxOverPunch_CheckedChanged);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(168, 102);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(55, 13);
         this.label2.TabIndex = 30;
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
         this.comboBoxYMultiplier.Location = new System.Drawing.Point(248, 99);
         this.comboBoxYMultiplier.Name = "comboBoxYMultiplier";
         this.comboBoxYMultiplier.Size = new System.Drawing.Size(50, 21);
         this.comboBoxYMultiplier.TabIndex = 31;
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
         this.comboBoxPinsInY.Location = new System.Drawing.Point(248, 69);
         this.comboBoxPinsInY.Name = "comboBoxPinsInY";
         this.comboBoxPinsInY.Size = new System.Drawing.Size(50, 21);
         this.comboBoxPinsInY.TabIndex = 27;
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
         this.comboBoxXMultiplier.Location = new System.Drawing.Point(84, 96);
         this.comboBoxXMultiplier.Name = "comboBoxXMultiplier";
         this.comboBoxXMultiplier.Size = new System.Drawing.Size(50, 21);
         this.comboBoxXMultiplier.TabIndex = 29;
         this.comboBoxXMultiplier.SelectedIndexChanged += new System.EventHandler(this.comboBoxXMultiplier_SelectedIndexChanged);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(12, 46);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(38, 13);
         this.label4.TabIndex = 22;
         this.label4.Text = "Shape";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 102);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(55, 13);
         this.label1.TabIndex = 28;
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
         this.comboBoxPinsInX.Location = new System.Drawing.Point(84, 69);
         this.comboBoxPinsInX.Name = "comboBoxPinsInX";
         this.comboBoxPinsInX.Size = new System.Drawing.Size(50, 21);
         this.comboBoxPinsInX.TabIndex = 25;
         this.comboBoxPinsInX.SelectedIndexChanged += new System.EventHandler(this.comboBoxPinsInX_SelectedIndexChanged);
         // 
         // comboBoxClusterShape
         // 
         this.comboBoxClusterShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboBoxClusterShape.FormattingEnabled = true;
         this.comboBoxClusterShape.Location = new System.Drawing.Point(84, 42);
         this.comboBoxClusterShape.Name = "comboBoxClusterShape";
         this.comboBoxClusterShape.Size = new System.Drawing.Size(100, 21);
         this.comboBoxClusterShape.TabIndex = 23;
         this.comboBoxClusterShape.SelectedIndexChanged += new System.EventHandler(this.comboBoxClusterShape_SelectedIndexChanged);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(168, 77);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(46, 13);
         this.label3.TabIndex = 26;
         this.label3.Text = "Pins in y";
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(12, 72);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(46, 13);
         this.label8.TabIndex = 24;
         this.label8.Text = "Pins in x";
         // 
         // labelXSpacing
         // 
         this.labelXSpacing.AutoSize = true;
         this.labelXSpacing.Location = new System.Drawing.Point(12, 15);
         this.labelXSpacing.Name = "labelXSpacing";
         this.labelXSpacing.Size = new System.Drawing.Size(52, 13);
         this.labelXSpacing.TabIndex = 36;
         this.labelXSpacing.Text = "x spacing";
         // 
         // textBoxXSpacing
         // 
         this.textBoxXSpacing.Location = new System.Drawing.Point(84, 12);
         this.textBoxXSpacing.Name = "textBoxXSpacing";
         this.textBoxXSpacing.Size = new System.Drawing.Size(50, 20);
         this.textBoxXSpacing.TabIndex = 37;
         this.textBoxXSpacing.TextChanged += new System.EventHandler(this.textBoxXSpacing_TextChanged);
         // 
         // labelYSpacing
         // 
         this.labelYSpacing.AutoSize = true;
         this.labelYSpacing.Location = new System.Drawing.Point(179, 15);
         this.labelYSpacing.Name = "labelYSpacing";
         this.labelYSpacing.Size = new System.Drawing.Size(52, 13);
         this.labelYSpacing.TabIndex = 38;
         this.labelYSpacing.Text = "y spacing";
         // 
         // textBoxYSpacing
         // 
         this.textBoxYSpacing.Location = new System.Drawing.Point(259, 12);
         this.textBoxYSpacing.Name = "textBoxYSpacing";
         this.textBoxYSpacing.Size = new System.Drawing.Size(50, 20);
         this.textBoxYSpacing.TabIndex = 39;
         this.textBoxYSpacing.TextChanged += new System.EventHandler(this.textBoxYSpacing_TextChanged);
         // 
         // errorProviderXSpacing
         // 
         this.errorProviderXSpacing.ContainerControl = this;
         // 
         // buttonStart
         // 
         this.buttonStart.Location = new System.Drawing.Point(156, 162);
         this.buttonStart.Name = "buttonStart";
         this.buttonStart.Size = new System.Drawing.Size(75, 23);
         this.buttonStart.TabIndex = 40;
         this.buttonStart.Text = "&Start";
         this.buttonStart.UseVisualStyleBackColor = true;
         this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
         // 
         // buttonCancel
         // 
         this.buttonCancel.Location = new System.Drawing.Point(237, 162);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(75, 23);
         this.buttonCancel.TabIndex = 41;
         this.buttonCancel.Text = "Ca&ncel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
         // 
         // errorProviderYSpacing
         // 
         this.errorProviderYSpacing.ContainerControl = this;
         // 
         // ClusterToolSearcherForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(331, 197);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.buttonStart);
         this.Controls.Add(this.labelXSpacing);
         this.Controls.Add(this.textBoxXSpacing);
         this.Controls.Add(this.labelYSpacing);
         this.Controls.Add(this.textBoxYSpacing);
         this.Controls.Add(this.checkBoxRotated);
         this.Controls.Add(this.checkBoxOverPunch);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.comboBoxYMultiplier);
         this.Controls.Add(this.comboBoxPinsInY);
         this.Controls.Add(this.comboBoxXMultiplier);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.comboBoxPinsInX);
         this.Controls.Add(this.comboBoxClusterShape);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label8);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "ClusterToolSearcherForm";
         this.Text = "Cluster Tool Searcher";
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderXSpacing)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderYSpacing)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckBox checkBoxRotated;
      private System.Windows.Forms.CheckBox checkBoxOverPunch;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.ComboBox comboBoxYMultiplier;
      private System.Windows.Forms.ComboBox comboBoxPinsInY;
      private System.Windows.Forms.ComboBox comboBoxXMultiplier;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox comboBoxPinsInX;
      private System.Windows.Forms.ComboBox comboBoxClusterShape;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label labelXSpacing;
      private System.Windows.Forms.TextBox textBoxXSpacing;
      private System.Windows.Forms.Label labelYSpacing;
      private System.Windows.Forms.TextBox textBoxYSpacing;
      private System.Windows.Forms.ErrorProvider errorProviderXSpacing;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonStart;
      private System.Windows.Forms.ErrorProvider errorProviderYSpacing;
   }
}