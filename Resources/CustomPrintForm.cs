using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetrixGroupPlugins.Resources
{
    public partial class CustomPrintForm : Form
    {
        List<CheckBox> panelCheck = new List<CheckBox>();
        public static string panelType, fileName, layoutName;
        public Boolean closeoption = true; 

        public CustomPrintForm()
        {
            InitializeComponent();
            labelName.Text = fileName;
            labelType.Text = panelType;
            labelLayout.Text = layoutName;
        }

        private void CustomPrintForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeoption = false;
        }

        
        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            Bitmap bmp = new Bitmap(this.groupBox1.Width, this.groupBox1.Height);
            this.groupBox1.DrawToBitmap(bmp, new Rectangle(0, 0, this.groupBox1.Width, this.groupBox1.Height));
            e.Graphics.DrawImage((Image)bmp, x, y);
        }

        private void buttonPrintCheckList_Click(object sender, EventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += this.Doc_PrintPage;
            PrintDialog dlgSettings = new PrintDialog();
            dlgSettings.Document = doc;
            try
            {
                if (dlgSettings.ShowDialog() == DialogResult.OK)
                {
                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Boolean boxCheck = true;

            #region adding check box to list for flat panels 

            panelCheck.Add(checkBoxPattern);
            panelCheck.Add(checkBoxPatternDirection);
            panelCheck.Add(checkBoxBorders);
            panelCheck.Add(checkBoxFixingHoles);
            panelCheck.Add(checkBoxPartSizes);
            panelCheck.Add(checkBoxPartLabels);
            panelCheck.Add(checkBoxDotFontLabels);
            panelCheck.Add(checkBoxPartQuantities);
            panelCheck.Add(checkBoxProject);
            panelCheck.Add(checkBoxCustomer);
            panelCheck.Add(checkBoxJobNumber);
            panelCheck.Add(checkBoxMaterial);
            panelCheck.Add(checkBoxCoating);
            panelCheck.Add(checkBoxTotalM2);

            #endregion



            if (panelType.Equals("Folded"))
            {
                panelCheck.Add(checkBoxFoldUp);
                panelCheck.Add(checkBoxFaceSide);
                panelCheck.Add(checkBoxGoodFace);
                panelCheck.Add(checkBoxFoldAngles);
                panelCheck.Add(checkBoxPanelFinished);
                panelCheck.Add(checkBoxTap);
                panelCheck.Add(checkBox3D);
                panelCheck.Add(checkBoxFoldLines);
                panelCheck.Add(checkBoxFLD);
                panelCheck.Add(checkBoxDotFont);
                checkBoxFoldUp.Visible = true;
                checkBoxFaceSide.Visible = true;
                checkBoxGoodFace.Visible = true;
                checkBoxFoldAngles.Visible = true;
                checkBoxPanelFinished.Visible = true;
                checkBoxTap.Visible = true;
                checkBox3D.Visible = true;
                checkBoxFoldLines.Visible = true;
                checkBoxFLD.Visible = true;
                checkBoxDotFont.Visible = true;

            }

            foreach (CheckBox cb in panelCheck)
            {
                if (cb.Checked == false)
                {
                    boxCheck = false;
                    break;
                }
            }

            if (boxCheck == true)
            {
                Close();
                closeoption = true;
            }
            else
                System.Windows.Forms.MessageBox.Show("Please check all the checkboxes");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
