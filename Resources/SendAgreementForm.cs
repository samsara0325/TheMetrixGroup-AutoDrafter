using MetrixGroupPlugins.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetrixGroupPlugins.Resources
{
   public partial class SendAgreementForm : Form
   {
      public  string receiverMail;
      public  string defaultMessage;
      public SendAgreementForm()
      {
         InitializeComponent();
      }

      private void label1_Click(object sender, EventArgs e)
      {

      }

      private void btnSendAgreement_Click(object sender, EventArgs e)
      {
         receiverMail = txtClientMail.Text;
         defaultMessage = txtDefaultMessage.Text;
         this.Close();
      }

      //returns the receiver mail
      public  string getReceiverMail()
      {
         return this.receiverMail;
      }

      //returns the default message
      public  string getDefaultMessage()
      {
         return this.defaultMessage;
      }
   }
}
