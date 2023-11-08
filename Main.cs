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
                command = "SELECT TODO.ID AS \"Номер\", TODO.NAME_TASK AS \"Название задачи\", TODO.DATE_TASK AS \"Дата\", IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", STATUS.NAME_STATUS AS \"Статус\" " +
                    "FROM TODO, IMPORTANCE, STATUS " +
                    "WHERE TODO.ID_STATUS = STATUS.ID AND TODO.ID_IMPORTANCE = IMPORTANCE.ID;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Width = 60;
                dataGridView1.Columns[1].Width = 418;
                dataGridView1.Columns[2].Width = 100;
                dataGridView1.Columns[3].Width = 100;
                dataGridView1.Columns[4].Width = 100;
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
                    switch (row.Cells["Важность"].Value.ToString())
                    {
                        case "Обязательно":
                            row.DefaultCellStyle.BackColor = Color.Red;
                            break;
                        case "Желательно":
                            row.DefaultCellStyle.BackColor = Color.Orange;
                            break;
                        case "Можно отложить":
                            row.DefaultCellStyle.BackColor = Color.Yellow;
                            break;
                    }
                }
                catch
                {

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
