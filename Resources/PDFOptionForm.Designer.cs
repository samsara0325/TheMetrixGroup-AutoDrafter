namespace MetrixGroupPlugins.Resources
{
    partial class PDFOptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFOptionForm));
            this.labelDisplay = new System.Windows.Forms.Label();
            this.comboBoxPdfOption = new System.Windows.Forms.ComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelDisplay
            // 
            this.labelDisplay.AutoSize = true;
            this.labelDisplay.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDisplay.Location = new System.Drawing.Point(134, 47);
            this.labelDisplay.Name = "labelDisplay";
            this.labelDisplay.Size = new System.Drawing.Size(263, 19);
            this.labelDisplay.TabIndex = 1;
            this.labelDisplay.Text = "PLEASE SELECT THE PDF TYPE";
            this.labelDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxPdfOption
            // 
            this.comboBoxPdfOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxPdfOption.ForeColor = System.Drawing.SystemColors.MenuText;
            this.comboBoxPdfOption.FormattingEnabled = true;
            this.comboBoxPdfOption.Items.AddRange(new object[] {
            "Normal",
            "Compressed",
            "Encrypted",
            "Password Protected"});
            this.comboBoxPdfOption.Location = new System.Drawing.Point(138, 105);
            this.comboBoxPdfOption.Name = "comboBoxPdfOption";
            this.comboBoxPdfOption.Size = new System.Drawing.Size(237, 28);
            this.comboBoxPdfOption.TabIndex = 7;
            this.comboBoxPdfOption.Text = "Please Select an Option";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(137, 177);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(107, 40);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // PDFOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 295);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.comboBoxPdfOption);
            this.Controls.Add(this.labelDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PDFOptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PDF Option";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDisplay;
        private System.Windows.Forms.ComboBox comboBoxPdfOption;
        private System.Windows.Forms.Button buttonOk;
    }
}