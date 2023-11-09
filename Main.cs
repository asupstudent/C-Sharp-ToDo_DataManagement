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
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue400, Primary.Blue500, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
            this.Text = "Управление задачами пользователя " + ConfigurationManager.AppSettings["Login"];
            refreshTable(DateTime.Today);
        }

        private void refreshTable(DateTime date_list)
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                toDoTransaction = fbCon.BeginTransaction();
                command = "SELECT TODO.ID AS \"Номер\", TODO.NAME_TASK AS \"Название задачи\", CAST(TODO.DATE_TASK AS TIME) AS \"Время\", IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", STATUS.NAME_STATUS AS \"Статус\" " +
                    "FROM TODO, IMPORTANCE, STATUS " +
                    "WHERE TODO.ID_STATUS = STATUS.ID AND TODO.ID_IMPORTANCE = IMPORTANCE.ID AND CAST(TODO.DATE_TASK AS DATE) = @date_list;";
                toDoCommand = new FbCommand(command, fbCon, toDoTransaction);
                toDoCommand.Parameters.AddWithValue("@date_list", date_list.ToString("dd.MM.yyyy"));
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

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.ColumnIndex == 3 & e.Value != null)
            {
                string importance = e.Value.ToString();
                switch (importance)
                {
                    case "Обязательно":
                        e.CellStyle.BackColor = Color.Red;
                        break;
                    case "Желательно":
                        e.CellStyle.BackColor = Color.Orange;
                        break;
                    case "Можно отложить":
                        e.CellStyle.BackColor = Color.Yellow;
                        break;
                }
            }
            if(e.ColumnIndex == 4 & e.Value != null)
            {
                string status = e.Value.ToString();
                switch (status)
                {
                    case "Выполнено":
                        e.CellStyle.BackColor = Color.DeepPink;
                            break;
                    case "Не выполнено":
                        e.CellStyle.BackColor = Color.Aqua;
                        break;
                    case "Просрочено":
                        e.CellStyle.BackColor = Color.DarkMagenta;
                        break;
                }
            }
        }
    }
}
