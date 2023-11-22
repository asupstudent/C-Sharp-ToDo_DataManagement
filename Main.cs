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
using System.Runtime.ConstrainedExecution;

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
            refreshTable(DateTime.Today.ToString("dd.MM.yyyy"));
        }

        private void refreshTable(string date_list)
        {
            updateExpired(getExpiredIds());
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT TODO.ID AS \"Номер\", TODO.NAME_TASK AS \"Название задачи\", CAST(TODO.DATE_TASK_START AS TIME) AS \"Начало\", CAST(TODO.DATE_TASK_END AS TIME) AS \"Конец\", IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", STATUS.NAME_STATUS AS \"Статус\" " +
                    "FROM TODO, IMPORTANCE, STATUS, USER_TODO " +
                    "WHERE TODO.ID_STATUS = STATUS.ID AND " +
                    "TODO.ID_IMPORTANCE = IMPORTANCE.ID AND " +
                    "TODO.ID_USER = USER_TODO.ID AND " +
                    "USER_TODO.LOGIN = '" + ConfigurationManager.AppSettings["Login"].ToUpper() + "' AND " +
                    "CAST(TODO.DATE_TASK_START AS DATE) = @date_list;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@date_list", date_list);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Width = 60;
                dataGridView1.Columns[1].Width = 318;
                dataGridView1.Columns[2].Width = 100;
                dataGridView1.Columns[3].Width = 100;
                dataGridView1.Columns[4].Width = 100;
                dataGridView1.Columns[5].Width = 100;
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

        private int[] getExpiredIds()
        {
            int[] expiredId = Array.Empty<int>();
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT TODO.ID FROM TODO, STATUS WHERE TODO.ID_STATUS = STATUS.ID AND STATUS.NAME_STATUS = 'В работе' AND DATE_TASK_END between '01.01.1970 00:00:00' AND  '" + DateTime.Now + "';";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Array.Resize(ref expiredId, expiredId.Length + 1);
                    expiredId[expiredId.Length - 1] = (int)dt.Rows[i][0];
                }
                return expiredId;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                return null;
            }
            finally
            {
                fbCon.Close();
            }
        }

        private int getExpiredId()
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT STATUS.ID FROM STATUS WHERE STATUS.NAME_STATUS = 'Просрочено';";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                return (int)dt.Rows[0][0];
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                return 0;
            }
            finally
            {
                fbCon.Close();
            }

        }

        private void updateExpired(int[] ids)
        {
            int[] receivedId = ids;
            int expiredId = getExpiredId();
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                toDoTransaction = fbCon.BeginTransaction();
                for (int i = 0; i < receivedId.Length; i++)
                {
                    command = "UPDATE TODO SET TODO.ID_STATUS = @expiredId WHERE TODO.ID = @receivedId;";
                    toDoCommand = new FbCommand(command, fbCon, toDoTransaction);
                    toDoCommand.Parameters.AddWithValue("@expiredId", expiredId);
                    toDoCommand.Parameters.AddWithValue("@receivedId", receivedId[i]);
                    toDoCommand.CommandType = CommandType.Text;
                    toDoCommand.ExecuteNonQuery();
                }
                toDoTransaction.Commit();
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
            if(e.ColumnIndex == 4 & e.Value != null)
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
            if(e.ColumnIndex == 5 & e.Value != null)
            {
                string status = e.Value.ToString();
                switch (status)
                {
                    case "Выполнено":
                        e.CellStyle.BackColor = Color.DeepPink;
                            break;
                    case "В работе":
                        e.CellStyle.BackColor = Color.Aqua;
                        break;
                    case "Просрочено":
                        e.CellStyle.BackColor = Color.DarkMagenta;
                        break;
                }
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            this.refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            refreshTable(DateTime.Today.ToString("dd.MM.yyyy"));
        }
    }
}
