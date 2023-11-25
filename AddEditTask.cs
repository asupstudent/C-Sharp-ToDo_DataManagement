using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class AddEditTask : Form
    {
        public AddEditTask()
        {
            InitializeComponent();
        }

        public void setName(string name)
        {
            materialSingleLineTextField1.Text = name;
        }

        public string getName()
        {
            return materialSingleLineTextField1.Text;
        }

        //public DateTime getStart()
        //{

        //}

        //public DateTime getEnd()
        //{

        //}

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
