using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APP
{
    public partial class AddGroup : Form
    {
        public AddGroup()
        {
            InitializeComponent();
           
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddGroup_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < 5; i++)
            {
                ChatlistUser user = new ChatlistUser();
                flowLayoutPanel1.Controls.Add(user);
            }
        }
    }
}
