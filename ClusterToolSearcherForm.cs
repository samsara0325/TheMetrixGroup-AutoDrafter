using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino.Geometry;
using MetrixGroupPlugins.Patterns;

namespace MetrixGroupPlugins
{
   public partial class ClusterToolSearcherForm : Form
   {
      private List<Circle> circleList;
      private ClusterToolSearcherPattern pattern;

      /// <summary>
      /// Initializes a new instance of the <see cref="ClusterToolSearcherForm" /> class.
      /// </summary>
      /// <param name="circleList">The circle list.</param>
      public ClusterToolSearcherForm(List<Circle> curveList)
      {
         InitializeComponent();

         pattern = new ClusterToolSearcherPattern();

         pattern.calculatePointMap(curveList);

         circleList = curveList;

         // Cluster shape
         comboBoxClusterShape.DataSource = Enum.GetValues(typeof(ClusterTool.ClusterToolShape));

         comboBoxClusterShape.SelectedIndex = (int) pattern.PunchingToolList[0].ClusterTool.Shape;

         textBoxXSpacing.Text = pattern.XSpacing.ToString();
         textBoxYSpacing.Text = pattern.YSpacing.ToString();

         comboBoxPinsInX.SelectedIndex = Properties.Settings.Default.CTSPinX - 1;
         comboBoxPinsInY.SelectedIndex = Properties.Settings.Default.CTSPinY - 1;
         comboBoxXMultiplier.SelectedIndex = Properties.Settings.Default.CTSXMulti - 1;
         comboBoxYMultiplier.SelectedIndex = Properties.Settings.Default.CTSYMulti - 1;
         checkBoxOverPunch.Checked = Properties.Settings.Default.CTSAllowOP;

         this.AcceptButton = buttonStart;
         this.CancelButton = buttonCancel;
      }



      /// <summary>
      /// Handles the Click event of the buttonCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void buttonCancel_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      /// <summary>
      /// Handles the Click event of the buttonStart control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
      private void buttonStart_Click(object sender, EventArgs e)
      {
         pattern.drawPerforation(circleList);

         if (comboBoxPinsInX.SelectedIndex < 0)
         {
            comboBoxPinsInX.SelectedIndex = 0;
         }

         if (comboBoxPinsInY.SelectedIndex < 0)
         {
            comboBoxPinsInY.SelectedIndex = 0;
         }

         if (comboBoxXMultiplier.SelectedIndex < 0)
         {
            comboBoxXMultiplier.SelectedIndex = 0;
         }

         if (comboBoxYMultiplier.SelectedIndex < 0)
         {
            comboBoxYMultiplier.SelectedIndex = 0;
         }

         Properties.Settings.Default.CTSPinX = comboBoxPinsInX.SelectedIndex + 1;
         Properties.Settings.Default.CTSPinY = comboBoxPinsInY.SelectedIndex + 1;
         Properties.Settings.Default.CTSXMulti = comboBoxXMultiplier.SelectedIndex + 1;
         Properties.Settings.Default.CTSYMulti = comboBoxYMultiplier.SelectedIndex + 1;
         Properties.Settings.Default.CTSAllowOP = checkBoxOverPunch.Checked;
         Properties.Settings.Default.Save();

         this.Close();
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxXSpacing control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
      private void textBoxXSpacing_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double xSpacing;
         bool result = Double.TryParse(textBoxXSpacing.Text, out xSpacing);

         if (result == true)
         {
            pattern.XSpacing = xSpacing;
            errorProviderXSpacing.Clear();
         }
         else
         {
            errorProviderXSpacing.SetError(textBoxXSpacing, "X Spacing error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxYSpacing control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
      private void textBoxYSpacing_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double ySpacing;
         bool result = Double.TryParse(textBoxYSpacing.Text, out ySpacing);

         if (result == true)
         {
            pattern.YSpacing = ySpacing;
            errorProviderYSpacing.Clear();
         }
         else
         {
            errorProviderYSpacing.SetError(textBoxYSpacing, "Y Spacing error");
         }
      }

      private void comboBoxClusterShape_SelectedIndexChanged(object sender, EventArgs e)
      {
         // Change the value in currentPattern

         ClusterTool.ClusterToolShape shape = (ClusterTool.ClusterToolShape)comboBoxClusterShape.SelectedItem;
         pattern.PunchingToolList[0].ClusterTool.Shape = shape;

      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPinsInX control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPinsInX_SelectedIndexChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.PinsX = comboBoxPinsInX.SelectedIndex + 1;
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPinsInY control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPinsInY_SelectedIndexChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.PinsY = comboBoxPinsInY.SelectedIndex + 1;
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxXMultiplier control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxXMultiplier_SelectedIndexChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.MultiplierX = comboBoxXMultiplier.SelectedIndex + 1;
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxYMultiplier control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxYMultiplier_SelectedIndexChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.MultiplierY = comboBoxYMultiplier.SelectedIndex + 1;
      }

      /// <summary>
      /// Handles the CheckedChanged event of the checkBoxRotated control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void checkBoxRotated_CheckedChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.Rotatable = checkBoxRotated.Checked;
      }

      /// <summary>
      /// Handles the CheckedChanged event of the checkBoxOverPunch control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void checkBoxOverPunch_CheckedChanged(object sender, EventArgs e)
      {
         pattern.PunchingToolList[0].ClusterTool.AllowOverPunch = checkBoxOverPunch.Checked;
      }

   }
}
