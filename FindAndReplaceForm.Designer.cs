namespace MetrixGroupPlugins
{
   partial class FindAndReplaceForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindAndReplaceForm));
         this.buttonOK = new System.Windows.Forms.Button();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.textBoxFind = new System.Windows.Forms.TextBox();
         this.textBoxReplace = new System.Windows.Forms.TextBox();
         this.errorProviderFindString = new System.Windows.Forms.ErrorProvider(this.components);
         this.errorProviderReplaceString = new System.Windows.Forms.ErrorProvider(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderFindString)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderReplaceString)).BeginInit();
         this.SuspendLayout();
         // 
         // buttonOK
         // 
         this.buttonOK.Location = new System.Drawing.Point(121, 56);
         this.buttonOK.Name = "buttonOK";
         this.buttonOK.Size = new System.Drawing.Size(75, 23);
         this.buttonOK.TabIndex = 4;
         this.buttonOK.Text = "Replace";
         this.buttonOK.UseVisualStyleBackColor = true;
         this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
         // 
         // buttonCancel
         // 
         this.buttonCancel.Location = new System.Drawing.Point(202, 56);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(75, 23);
         this.buttonCancel.TabIndex = 5;
         this.buttonCancel.Text = "Cancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 7);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(30, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Find:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(12, 33);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(50, 13);
         this.label2.TabIndex = 2;
         this.label2.Text = "Replace:";
         // 
         // textBoxFind
         // 
         this.textBoxFind.Location = new System.Drawing.Point(81, 4);
         this.textBoxFind.Name = "textBoxFind";
         this.textBoxFind.Size = new System.Drawing.Size(173, 20);
         this.textBoxFind.TabIndex = 1;
         // 
         // textBoxReplace
         // 
         this.textBoxReplace.Location = new System.Drawing.Point(81, 30);
         this.textBoxReplace.Name = "textBoxReplace";
         this.textBoxReplace.Size = new System.Drawing.Size(173, 20);
         this.textBoxReplace.TabIndex = 3;
         // 
         // errorProviderFindString
         // 
         this.errorProviderFindString.ContainerControl = this;
         // 
         // errorProviderReplaceString
         // 
         this.errorProviderReplaceString.ContainerControl = this;
         // 
         // FindAndReplaceForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(289, 87);
         this.Controls.Add(this.textBoxReplace);
         this.Controls.Add(this.textBoxFind);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.buttonOK);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "FindAndReplaceForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Find and Replace";
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderFindString)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProviderReplaceString)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button buttonOK;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox textBoxFind;
      private System.Windows.Forms.TextBox textBoxReplace;
      private System.Windows.Forms.ErrorProvider errorProviderFindString;
      private System.Windows.Forms.ErrorProvider errorProviderReplaceString;
   }
}