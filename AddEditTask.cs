using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class AddEditTask : Form
    {
        int id = 0;
        int id_category = 0;
        public AddEditTask()
        {
            InitializeComponent();
            dateTimePicker1.MinDate = DateTime.Now;
            dateTimePicker2.MinDate = DateTime.Now;
            dateTimePicker3.MinDate = dateTimePicker3.Value.AddMinutes(10);
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public int getId()
        {
            return this.id;
        }

        public void setName(string name)
        {
            materialSingleLineTextField1.Text = name;
        }

        public string getName()
        {
            return materialSingleLineTextField1.Text;
        }

        public void setStartTime(DateTime dt)
        {
            dateTimePicker1.Value = dt;
            dateTimePicker2.Value = dt;
        }

        public DateTime getStartTime()
        {
            return new DateTime(dateTimePicker1.Value.Year,
                                dateTimePicker1.Value.Month,
                                dateTimePicker1.Value.Day,
                                dateTimePicker2.Value.Hour,
                                dateTimePicker2.Value.Minute,
                                dateTimePicker2.Value.Second);
        }

        public void setEndTime(DateTime dt)
        {
            dateTimePicker3.Value = dt;
        }

        public DateTime getEndTime()
        {
            return new DateTime(dateTimePicker1.Value.Year,
                                dateTimePicker1.Value.Month,
                                dateTimePicker1.Value.Day,
                                dateTimePicker3.Value.Hour,
                                dateTimePicker3.Value.Minute,
                                dateTimePicker3.Value.Second);
        }

        public bool isCheckedImportanceTask()
        {
            if(materialCheckBox1.Checked)
            {
                return true;    
            }
            else
            {
                return false;
            }
        }

        public void setCheckedImportanceTask()
        {
            materialCheckBox1.Checked = true;
        }

        public void setCategory(int id_category, string category)
        {
            this.id_category = id_category;
            materialLabel3.Text = category;
        }

        public int getCategory()
        {
            return this.id_category;
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialSingleLineTextField1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }
    }
}
