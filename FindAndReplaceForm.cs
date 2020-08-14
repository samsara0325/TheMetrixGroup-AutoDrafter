using Rhino.DocObjects;
using Rhino.Geometry;
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
   public partial class FindAndReplaceForm : Form
   {
      private List<RhinoObject> rhinoObjList;

      public FindAndReplaceForm(List<RhinoObject> list)
      {
         rhinoObjList = list;
         InitializeComponent();
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
      /// Handles the Click event of the buttonOK control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void buttonOK_Click(object sender, EventArgs e)
      {
         String searchString = textBoxFind.Text ;
         String replaceString = textBoxReplace.Text;

         // Check the data first
         if (searchString  == "")
         {
            errorProviderFindString.SetError(textBoxFind, "Cannot be empty");
            return;
         }

         // Go through each object and search the text entity and replace string
         foreach(RhinoObject rhinoObj in rhinoObjList)
         {
            TextEntity textEntity = rhinoObj.Geometry as TextEntity;

            if(textEntity != null)
            {
               textEntity.Text = textEntity.Text.Replace(searchString, replaceString);
            }

            rhinoObj.CommitChanges();
         }

         this.Close();
      } 
   }
}
