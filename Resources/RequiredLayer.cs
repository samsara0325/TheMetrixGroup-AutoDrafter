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
   public partial class RequiredLayer : Form
   {
      private Color selectedColour = Color.Purple;
      private String layerName = null;
      public RequiredLayer()
      {
         InitializeComponent();
      }

      private void RequiredLayer_Load(object sender, EventArgs e)
      {
         txtColourChoosen.BackColor = selectedColour;
      }

      private void label2_Click(object sender, EventArgs e)
      {

      }

      private void btnColourPicker_Click(object sender, EventArgs e)
      {
         ColorDialog clrDialog = new ColorDialog();

         //show the colour dialog and check that user clicked ok
         if (clrDialog.ShowDialog() == DialogResult.OK)
         {
            //save the colour that the user chose
            selectedColour = clrDialog.Color;
            txtColourChoosen.BackColor = selectedColour;
         }
      }

      private void btnCreateLayer_Click(object sender, EventArgs e)
      {
         layerName = txtLLayerName.Text;
         if (layerName == null)
         {
            MessageBox.Show("Layer Name Required");
            return;
         }
         else
         {
            this.Close();
         }
      }

      public String getLayerName()
      {
         return this.layerName;
      }
      public Color getSelectedColor()
      {
         return this.selectedColour;
      }

      private void label1_Click(object sender, EventArgs e)
      {

      }
   }
}
