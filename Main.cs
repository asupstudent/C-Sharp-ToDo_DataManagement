using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class Main : MaterialForm
    {
        MaterialSkinManager materialSkinManager;
        FbConnection fbCon;
        FbCommand toDoCommand;
        FbTransaction toDoTransaction;
        FbDataReader dr;
        string command;
        DataTable dt;
        public Main()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue400, Primary.Blue500, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
            this.Text = "Управление задачами пользователя " + ConfigurationManager.AppSettings["Login"];
            refreshTable();
        }

        private void refreshTable()
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT ID AS \"Номер\", NAME_TASK AS \"Название задачи\", DATE_TASK AS \"Дата\", ID_STATUS, ID_IMPORTANCE FROM TODO;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[3].Visible = false;
                dataGridView1.Columns[4].Visible = false;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
            finally
            {
                fbCon.Close();
            }
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void paintImportance()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    //MessageBox.Show(row.Cells["ID_IMPORTANCE"].Value.ToString());
                    //for(int i = 0; i < dataGridView1.Rows.Count; i++)
                    //{
                    //MessageBox.Show(dataGridView1[4, i].Value.ToString());
                    switch (row.Cells["ID_IMPORTANCE"].Value.ToString())
                    {
                        //case "Обязательно":
                        case "1":
                            row.DefaultCellStyle.BackColor = Color.Red;
                            break;
                        //case "Желательно":
                        case "2":
                            row.DefaultCellStyle.BackColor = Color.Orange;
                            break;
                        //case "Можно отложить":
                        case "3":
                            row.DefaultCellStyle.BackColor = Color.Yellow;
                            break;
                    }
                    //}
                }
                catch
                {
                    // здесь можно отреагировать на неправильные данные, а можно ничего не делать
                }
            }
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            paintImportance();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
