using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetrixGroupPlugins
{
   public partial class ClusterToolRotationForm : Form
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="AddDesignForm"/> class.
      /// </summary>
      public ClusterToolRotationForm()
      {
         InitializeComponent();

         textBoxClusterToolRotation.Text = Properties.Settings.Default.ClusterToolRotation.ToString();
      }

      /// <summary>
      /// Handles the Click event of the buttonOK control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void buttonOK_Click(object sender, EventArgs e)
      {
         if(Double.TryParse(textBoxClusterToolRotation.Text, out double result))
         {
            Properties.Settings.Default.ClusterToolRotation = result;
            Properties.Settings.Default.Save();
         }
      }
   }
}
