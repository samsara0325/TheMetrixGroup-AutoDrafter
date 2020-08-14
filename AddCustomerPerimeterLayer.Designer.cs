namespace MetrixGroupPlugins
{
    partial class AddCustomerPerimeterLayer
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
            this.txtCustomerLayerName = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(580, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = "Custom Panel Perimeter Layer ";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtCustomerLayerName
            // 
            this.txtCustomerLayerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerLayerName.Location = new System.Drawing.Point(83, 121);
            this.txtCustomerLayerName.Name = "txtCustomerLayerName";
            this.txtCustomerLayerName.Size = new System.Drawing.Size(374, 40);
            this.txtCustomerLayerName.TabIndex = 2;
            this.txtCustomerLayerName.Text = "Enter customer name here";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(20, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(541, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Note : Please provide the accurate layer name which contains the panel perimeter";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(35, 192);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(483, 87);
            this.button1.TabIndex = 4;
            this.button1.Text = "Add custom layer name";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddCustomerPerimeterLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 319);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCustomerLayerName);
            this.Controls.Add(this.label1);
            this.Name = "AddCustomerPerimeterLayer";
            this.Text = "Cad File Merger Customer Perimeter Layer Adder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtCustomerLayerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}