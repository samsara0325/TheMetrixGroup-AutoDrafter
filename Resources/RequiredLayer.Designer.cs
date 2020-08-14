namespace MetrixGroupPlugins.Resources
{
   partial class RequiredLayer
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
         this.label1 = new System.Windows.Forms.Label();
         this.txtLLayerName = new System.Windows.Forms.TextBox();
         this.btnColourPicker = new System.Windows.Forms.Button();
         this.txtColourChoosen = new System.Windows.Forms.TextBox();
         this.btnCreateLayer = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(31, 115);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(85, 17);
         this.label1.TabIndex = 0;
         this.label1.Text = "Layer Name";
         this.label1.Click += new System.EventHandler(this.label1_Click);
         // 
         // txtLLayerName
         // 
         this.txtLLayerName.Location = new System.Drawing.Point(158, 110);
         this.txtLLayerName.Name = "txtLLayerName";
         this.txtLLayerName.Size = new System.Drawing.Size(325, 22);
         this.txtLLayerName.TabIndex = 1;
         // 
         // btnColourPicker
         // 
         this.btnColourPicker.Location = new System.Drawing.Point(158, 180);
         this.btnColourPicker.Name = "btnColourPicker";
         this.btnColourPicker.Size = new System.Drawing.Size(201, 33);
         this.btnColourPicker.TabIndex = 2;
         this.btnColourPicker.Text = "Pick Layer Colour ";
         this.btnColourPicker.UseVisualStyleBackColor = true;
         this.btnColourPicker.Click += new System.EventHandler(this.btnColourPicker_Click);
         // 
         // txtColourChoosen
         // 
         this.txtColourChoosen.Location = new System.Drawing.Point(374, 185);
         this.txtColourChoosen.Name = "txtColourChoosen";
         this.txtColourChoosen.ReadOnly = true;
         this.txtColourChoosen.Size = new System.Drawing.Size(100, 22);
         this.txtColourChoosen.TabIndex = 3;
         // 
         // btnCreateLayer
         // 
         this.btnCreateLayer.BackColor = System.Drawing.SystemColors.MenuHighlight;
         this.btnCreateLayer.Location = new System.Drawing.Point(173, 254);
         this.btnCreateLayer.Name = "btnCreateLayer";
         this.btnCreateLayer.Size = new System.Drawing.Size(272, 81);
         this.btnCreateLayer.TabIndex = 4;
         this.btnCreateLayer.Text = "Create Layer";
         this.btnCreateLayer.UseVisualStyleBackColor = false;
         this.btnCreateLayer.Click += new System.EventHandler(this.btnCreateLayer_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label2.Location = new System.Drawing.Point(151, 18);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(359, 38);
         this.label2.TabIndex = 5;
         this.label2.Text = "Custom Layer Creator";
         // 
         // RequiredLayer
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(558, 348);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.btnCreateLayer);
         this.Controls.Add(this.txtColourChoosen);
         this.Controls.Add(this.btnColourPicker);
         this.Controls.Add(this.txtLLayerName);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "RequiredLayer";
         this.Text = "Custom Layer Creator";
         this.Load += new System.EventHandler(this.RequiredLayer_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtLLayerName;
      private System.Windows.Forms.Button btnColourPicker;
      private System.Windows.Forms.TextBox txtColourChoosen;
      private System.Windows.Forms.Button btnCreateLayer;
      private System.Windows.Forms.Label label2;
   }
}