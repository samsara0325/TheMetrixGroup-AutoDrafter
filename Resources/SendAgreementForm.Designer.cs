namespace MetrixGroupPlugins.Resources
{
   partial class SendAgreementForm
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
         this.txtClientMail = new System.Windows.Forms.TextBox();
         this.txtConfirmMail = new System.Windows.Forms.TextBox();
         this.txtDefaultMessage = new System.Windows.Forms.TextBox();
         this.btnSendAgreement = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(28, 92);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(81, 17);
         this.label1.TabIndex = 0;
         this.label1.Text = "Client Email";
         this.label1.Click += new System.EventHandler(this.label1_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(28, 149);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(94, 17);
         this.label2.TabIndex = 1;
         this.label2.Text = "Confirm Email";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(28, 204);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(114, 17);
         this.label3.TabIndex = 2;
         this.label3.Text = "Default Message";
         // 
         // txtClientMail
         // 
         this.txtClientMail.Location = new System.Drawing.Point(154, 92);
         this.txtClientMail.Name = "txtClientMail";
         this.txtClientMail.Size = new System.Drawing.Size(243, 22);
         this.txtClientMail.TabIndex = 3;
         // 
         // txtConfirmMail
         // 
         this.txtConfirmMail.CausesValidation = false;
         this.txtConfirmMail.Location = new System.Drawing.Point(154, 149);
         this.txtConfirmMail.Name = "txtConfirmMail";
         this.txtConfirmMail.Size = new System.Drawing.Size(243, 22);
         this.txtConfirmMail.TabIndex = 4;
         // 
         // txtDefaultMessage
         // 
         this.txtDefaultMessage.Location = new System.Drawing.Point(154, 201);
         this.txtDefaultMessage.Name = "txtDefaultMessage";
         this.txtDefaultMessage.Size = new System.Drawing.Size(243, 22);
         this.txtDefaultMessage.TabIndex = 5;
         // 
         // btnSendAgreement
         // 
         this.btnSendAgreement.Location = new System.Drawing.Point(154, 279);
         this.btnSendAgreement.Name = "btnSendAgreement";
         this.btnSendAgreement.Size = new System.Drawing.Size(131, 61);
         this.btnSendAgreement.TabIndex = 6;
         this.btnSendAgreement.Text = "Send Agreement";
         this.btnSendAgreement.UseVisualStyleBackColor = true;
         this.btnSendAgreement.Click += new System.EventHandler(this.btnSendAgreement_Click);
         // 
         // SendAgreementForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(431, 425);
         this.Controls.Add(this.btnSendAgreement);
         this.Controls.Add(this.txtDefaultMessage);
         this.Controls.Add(this.txtConfirmMail);
         this.Controls.Add(this.txtClientMail);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Name = "SendAgreementForm";
         this.Text = "SendAgreementForm";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox txtClientMail;
      private System.Windows.Forms.TextBox txtConfirmMail;
      private System.Windows.Forms.TextBox txtDefaultMessage;
      private System.Windows.Forms.Button btnSendAgreement;
   }
}