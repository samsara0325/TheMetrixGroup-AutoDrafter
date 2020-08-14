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
   public partial class Form1 : Form
   {
      public static Double holeSize;
      public static Double periOffset;
      public static Double foldOffset;
      public static Double foldSetback;
      public Form1()
      {
         InitializeComponent();
      }

      private void label1_Click(object sender, EventArgs e)
      {

      }

      private void label2_Click(object sender, EventArgs e)
      {

      }

      private void textBox3_TextChanged(object sender, EventArgs e)
      {

      }

      private void textBox4_TextChanged(object sender, EventArgs e)
      {

      }

      private void btnStart_Click(object sender, EventArgs e)
      {
         holeSize = Double.Parse(txtHoleSize.Text);
         periOffset = Double.Parse(txtPeriOffset.Text);
         foldOffset = Double.Parse(txtFoldOffset.Text);
         foldSetback = Double.Parse(txtFoldSetback.Text);
         this.Close();
      }
   
      public Double getholeSize()
      {
         return holeSize;
      }

      public Double getperiOffset()
      {
         return periOffset;
      }

      public Double getfoldOffset()
      {
         return foldOffset;
      }

      public Double getfoldSetback()
      {
         return foldSetback;
      }

      private void Form1_Load(object sender, EventArgs e)
      {

      }
   }

}
