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
   public partial class MassDXFOptionSelection : Form
   {
      public static String optionSelected;
      public static Boolean onlyMLabels;

      public MassDXFOptionSelection()
      {
         InitializeComponent();
      }

      private void radioButton1_CheckedChanged(object sender, EventArgs e)
      {

      }

      private void button1_Click(object sender, EventArgs e)
      {
         if (rbPandL.Checked) //get the selected option from the radio button
         {
            optionSelected = "PandL";
         }
         else
         {
            optionSelected = "FullLayer";
         }

         if (chckLabels.Checked) //Check if the check box is checked or not
         {
            onlyMLabels = true;
         }
         else
         {
            onlyMLabels = false;
         }
         this.Close();
      }
      
      //Method returns the selected option
      public String getOption() 
      {
         return optionSelected;
      }

      //Method returns the check option
      public Boolean getCheckBoxOption()
      {
         return onlyMLabels;
      }
      private void MassDXFOptionSelection_Load(object sender, EventArgs e)
      {

      }

      private void label1_Click(object sender, EventArgs e)
      {

      }

      private void chckLabels_CheckedChanged(object sender, EventArgs e)
      {
         
      }
   }
}
