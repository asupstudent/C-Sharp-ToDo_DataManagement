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
            dateTimePicker3.MinDate = dateTimePicker2.Value.AddMinutes(10);
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
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        public void setNameColor()
        {
            if (this.getName().Length == 0)
            {
                materialSingleLineTextField1.BackColor = System.Drawing.Color.Salmon;
            }
            else
            {
                materialSingleLineTextField1.BackColor = System.Drawing.Color.PaleGreen;
            }
        }

        public void setCategoryColor()
        {
            if (this.getCategory() == 0)
            {
                materialLabel3.BackColor = System.Drawing.Color.Salmon;
            }
            else
            {
                materialLabel3.BackColor = System.Drawing.Color.PaleGreen;
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            using (CategoryList categoryList = new CategoryList())
            {
                categoryList.FormClosing += delegate (object fSender, FormClosingEventArgs fe)
                {
                    if (categoryList.DialogResult == DialogResult.OK)
                    {
                        materialLabel3.Text = categoryList.getCurrentRecord().Cells[1].Value.ToString();
                        id_category = Convert.ToInt32(categoryList.getCurrentRecord().Cells[0].Value);
                    }
                };
                categoryList.ShowDialog(this);
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker3.MinDate = dateTimePicker2.Value.AddMinutes(10);
        }
    }
}
