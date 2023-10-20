using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Animations;
using MaterialSkin.Controls;
using MaterialSkin;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class Login : MaterialForm
    {
        MaterialSkinManager materialSkinManager;
        public Login()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue400, Primary.Blue500, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                this.TopMost= true;
            }
            else
            {
                this.TopMost= false;
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            this.materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            this.materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
        }
    }
}
