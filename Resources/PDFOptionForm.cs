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
    public partial class PDFOptionForm : Form
    {
        public static char optionsSelected;

        public PDFOptionForm()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxPdfOption.SelectedIndex > -1)
            {
                Object selectedItem = comboBoxPdfOption.SelectedItem;

                if (selectedItem.ToString().Equals("Normal"))
                    optionsSelected = 'N';
                if (selectedItem.ToString().Equals("Compressed"))
                    optionsSelected = 'C';
                if (selectedItem.ToString().Equals("Encrypted"))
                    optionsSelected = 'E';
                if (selectedItem.ToString().Equals("Password Protected"))
                    optionsSelected = 'P';
            }

            this.Close();
        }
    }
}
