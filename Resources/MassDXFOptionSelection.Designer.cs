namespace MetrixGroupPlugins.Resources
{
   partial class MassDXFOptionSelection
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
         this.rbPandL = new System.Windows.Forms.RadioButton();
         this.rbFullLayer = new System.Windows.Forms.RadioButton();
         this.btnSubmit = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.chckLabels = new System.Windows.Forms.CheckBox();
         this.SuspendLayout();
         // 
         // rbPandL
         // 
         this.rbPandL.AutoSize = true;
         this.rbPandL.Location = new System.Drawing.Point(217, 165);
         this.rbPandL.Name = "rbPandL";
         this.rbPandL.Size = new System.Drawing.Size(164, 21);
         this.rbPandL.TabIndex = 0;
         this.rbPandL.TabStop = true;
         this.rbPandL.Text = "Perimeter and Labels";
         this.rbPandL.UseVisualStyleBackColor = true;
         this.rbPandL.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
         // 
         // rbFullLayer
         // 
         this.rbFullLayer.AutoSize = true;
         this.rbFullLayer.Location = new System.Drawing.Point(217, 249);
         this.rbFullLayer.Name = "rbFullLayer";
         this.rbFullLayer.Size = new System.Drawing.Size(145, 21);
         this.rbFullLayer.TabIndex = 1;
         this.rbFullLayer.TabStop = true;
         this.rbFullLayer.Text = "Layers for Nesting";
         this.rbFullLayer.UseVisualStyleBackColor = true;
         // 
         // btnSubmit
         // 
         this.btnSubmit.Location = new System.Drawing.Point(217, 391);
         this.btnSubmit.Name = "btnSubmit";
         this.btnSubmit.Size = new System.Drawing.Size(164, 50);
         this.btnSubmit.TabIndex = 2;
         this.btnSubmit.Text = "Export";
         this.btnSubmit.UseVisualStyleBackColor = true;
         this.btnSubmit.Click += new System.EventHandler(this.button1_Click);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(180, 64);
         this.label1.Name = "label1";
         this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
         this.label1.Size = new System.Drawing.Size(316, 25);
         this.label1.TabIndex = 3;
         this.label1.Text = "Select an Option to Start Exporting ";
         this.label1.Click += new System.EventHandler(this.label1_Click);
         // 
         // chckLabels
         // 
         this.chckLabels.AutoSize = true;
         this.chckLabels.Checked = true;
         this.chckLabels.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chckLabels.Cursor = System.Windows.Forms.Cursors.No;
         this.chckLabels.Location = new System.Drawing.Point(204, 321);
         this.chckLabels.Name = "chckLabels";
         this.chckLabels.Size = new System.Drawing.Size(196, 21);
         this.chckLabels.TabIndex = 4;
         this.chckLabels.Text = "Export With M Labels Only";
         this.chckLabels.UseVisualStyleBackColor = true;
         this.chckLabels.CheckedChanged += new System.EventHandler(this.chckLabels_CheckedChanged);
         // 
         // MassDXFOptionSelection
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(639, 574);
         this.Controls.Add(this.chckLabels);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.btnSubmit);
         this.Controls.Add(this.rbFullLayer);
         this.Controls.Add(this.rbPandL);
         this.Name = "MassDXFOptionSelection";
         this.Text = "MassDXFOptionSelection";
         this.Load += new System.EventHandler(this.MassDXFOptionSelection_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.RadioButton rbPandL;
      private System.Windows.Forms.RadioButton rbFullLayer;
      private System.Windows.Forms.Button btnSubmit;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.CheckBox chckLabels;
   }
}