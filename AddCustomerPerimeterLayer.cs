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
    public partial class AddCustomerPerimeterLayer : Form
    {
        public AddCustomerPerimeterLayer()
        {
            InitializeComponent();
        }

        public string CustomerPerimeterName { get; set; }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Grab input
            var userInput = txtCustomerLayerName.Text;

            if(string.IsNullOrEmpty(userInput) || userInput.Equals("Enter customer name here"))
            {
                //Handle 
            }
            else
            {
                CustomerPerimeterName = userInput;
                this.Visible = false;
            }
        }
    }
}
