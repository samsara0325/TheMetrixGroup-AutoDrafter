namespace MetrixGroupPlugins.Resources
{
   partial class Form1
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
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.txtFoldOffset = new System.Windows.Forms.TextBox();
         this.txtPeriOffset = new System.Windows.Forms.TextBox();
         this.txtFoldSetback = new System.Windows.Forms.TextBox();
         this.txtHoleSize = new System.Windows.Forms.TextBox();
         this.btnStart = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(28, 84);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(251, 17);
         this.label1.TabIndex = 0;
         this.label1.Text = "Fold Line Offset (From Panel Finished)";
         this.label1.Click += new System.EventHandler(this.label1_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(28, 128);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(226, 17);
         this.label2.TabIndex = 1;
         this.label2.Text = "Perimeter Offset (From Folds Line)";
         this.label2.Click += new System.EventHandler(this.label2_Click);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(28, 214);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(108, 17);
         this.label3.TabIndex = 2;
         this.label3.Text = "Relief Hole Size";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(28, 170);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(221, 17);
         this.label4.TabIndex = 3;
         this.label4.Text = "Fold Line Setback (From Corners)";
         // 
         // txtFoldOffset
         // 
         this.txtFoldOffset.Location = new System.Drawing.Point(285, 84);
         this.txtFoldOffset.Name = "txtFoldOffset";
         this.txtFoldOffset.Size = new System.Drawing.Size(151, 22);
         this.txtFoldOffset.TabIndex = 4;
         // 
         // txtPeriOffset
         // 
         this.txtPeriOffset.Location = new System.Drawing.Point(285, 128);
         this.txtPeriOffset.Name = "txtPeriOffset";
         this.txtPeriOffset.Size = new System.Drawing.Size(151, 22);
         this.txtPeriOffset.TabIndex = 5;
         // 
         // txtFoldSetback
         // 
         this.txtFoldSetback.Location = new System.Drawing.Point(285, 170);
         this.txtFoldSetback.Name = "txtFoldSetback";
         this.txtFoldSetback.Size = new System.Drawing.Size(151, 22);
         this.txtFoldSetback.TabIndex = 6;
         this.txtFoldSetback.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
         // 
         // txtHoleSize
         // 
         this.txtHoleSize.Location = new System.Drawing.Point(285, 214);
         this.txtHoleSize.Name = "txtHoleSize";
         this.txtHoleSize.Size = new System.Drawing.Size(151, 22);
         this.txtHoleSize.TabIndex = 7;
         this.txtHoleSize.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
         // 
         // btnStart
         // 
         this.btnStart.Location = new System.Drawing.Point(128, 293);
         this.btnStart.Name = "btnStart";
         this.btnStart.Size = new System.Drawing.Size(187, 46);
         this.btnStart.TabIndex = 8;
         this.btnStart.Text = "Add Notches";
         this.btnStart.UseVisualStyleBackColor = true;
         this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(479, 381);
         this.Controls.Add(this.btnStart);
         this.Controls.Add(this.txtHoleSize);
         this.Controls.Add(this.txtFoldSetback);
         this.Controls.Add(this.txtPeriOffset);
         this.Controls.Add(this.txtFoldOffset);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Name = "Form1";
         this.Text = "Custom Corner Notches";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox txtFoldOffset;
      private System.Windows.Forms.TextBox txtPeriOffset;
      private System.Windows.Forms.TextBox txtFoldSetback;
      private System.Windows.Forms.TextBox txtHoleSize;
      private System.Windows.Forms.Button btnStart;
   }
}