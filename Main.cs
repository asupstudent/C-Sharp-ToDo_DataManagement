using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using System.Runtime.ConstrainedExecution;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            this.Text = "Управление задачами пользователя: " + ConfigurationManager.AppSettings["Login"] + "            База данных: " + ConfigurationManager.AppSettings["Database"];
            DataTable categories = getCategories();
            comboBox1.DataSource = categories;
            comboBox1.DisplayMember = "NAME";
            comboBox1.ValueMember = "ID";
            comboBox1.SelectedIndex = 0;
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void refreshTable(string date_list)
        {
            updateExpired(getExpiredIds());
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                if (comboBox1.SelectedIndex != 0)
                {
                    command = "SELECT TODO.ID AS \"Номер\", " +
                                     "TODO.NAME_TASK AS \"Название задачи\", " +
                                     "CAST(TODO.DATE_TASK_START AS TIME) AS \"Начало\", " +
                                     "CAST(TODO.DATE_TASK_END AS TIME) AS \"Конец\", " +
                                     "IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", " +
                                     "STATUS.NAME_STATUS AS \"Статус\", " +
                                     "CATEGORY.NAME AS \"Категория\" " +
                              "FROM TODO, IMPORTANCE, STATUS, CATEGORY, USER_TODO " +
                              "WHERE TODO.ID_STATUS = STATUS.ID AND " +
                                    "TODO.ID_IMPORTANCE = IMPORTANCE.ID AND " +
                                    "TODO.ID_CATEGORY = CATEGORY.ID AND TODO.ID_CATEGORY = @id_category AND " +
                                    "TODO.ID_USER = USER_TODO.ID AND USER_TODO.LOGIN = '" + ConfigurationManager.AppSettings["Login"].ToUpper() + "' AND " +
                                    "CAST(TODO.DATE_TASK_START AS DATE) = @date_list;";
                    toDoCommand = new FbCommand(command, fbCon);
                    toDoCommand.Parameters.AddWithValue("@id_category", comboBox1.SelectedValue.ToString());
                    toDoCommand.Parameters.AddWithValue("@date_list", monthCalendar1.SelectionRange.Start.ToShortDateString());
                }
                else
                {
                    fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                    fbCon.Open();
                    command = "SELECT TODO.ID AS \"Номер\", " +
                                     "TODO.NAME_TASK AS \"Название задачи\", " +
                                     "CAST(TODO.DATE_TASK_START AS TIME) AS \"Начало\", " +
                                     "CAST(TODO.DATE_TASK_END AS TIME) AS \"Конец\", " +
                                     "IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", " +
                                     "STATUS.NAME_STATUS AS \"Статус\", " +
                                     "CATEGORY.NAME AS \"Категория\" " +
                              "FROM TODO, IMPORTANCE, STATUS, CATEGORY, USER_TODO " +
                              "WHERE TODO.ID_STATUS = STATUS.ID AND " +
                                    "TODO.ID_IMPORTANCE = IMPORTANCE.ID AND " +
                                    "TODO.ID_CATEGORY = CATEGORY.ID AND " +
                                    "TODO.ID_USER = USER_TODO.ID AND " +
                                    "USER_TODO.LOGIN = '" + ConfigurationManager.AppSettings["Login"].ToUpper() + "' AND " +
                                    "CAST(TODO.DATE_TASK_START AS DATE) = @date_list;";
                    toDoCommand = new FbCommand(command, fbCon);
                    toDoCommand.Parameters.AddWithValue("@date_list", date_list);
                }
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
            finally
            {
                fbCon.Close();
            }
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 218;
            dataGridView1.Columns[2].Width = 100;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100;
            setStatusButton();
        }

        private int[] getExpiredIds()
        {
            int[] expiredId = Array.Empty<int>();
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT TODO.ID FROM TODO, STATUS, IMPORTANCE " +
                                "WHERE TODO.ID_STATUS = STATUS.ID AND STATUS.NAME_STATUS = 'В работе' " +
                                "AND TODO.ID_IMPORTANCE = IMPORTANCE.ID AND IMPORTANCE.NAME_IMPORTANCE = 'Обязательно' " +
                                "AND DATE_TASK_END between '01.01.1970 00:00:00' AND  '" + DateTime.Now + "';";
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

        private int getLoginId(string login)
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT USER_TODO.ID FROM USER_TODO WHERE USER_TODO.LOGIN = @login;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@login", login);
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

        private int getStatusId(string status)
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT STATUS.ID FROM STATUS WHERE STATUS.NAME_STATUS = @status;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@status", status);
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

        private int getImportanceId(string nameImportance)
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT IMPORTANCE.ID FROM IMPORTANCE WHERE IMPORTANCE.NAME_IMPORTANCE = @importance;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@importance", nameImportance);
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

        private int getCategoryId(string nameCategory)
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT CATEGORY.ID FROM CATEGORY WHERE CATEGORY.NAME = @category;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@category", nameCategory);
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
            int expiredId = getStatusId("Просрочено");
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

        private void setStatusButton()
        {
            if (dataGridView1.Rows.Count != 0)
            {
                materialRaisedButton4.Enabled = true;
                materialRaisedButton5.Enabled = true;
                materialRaisedButton8.Enabled = true;
            }
            else
            {
                materialRaisedButton4.Enabled = false;
                materialRaisedButton5.Enabled = false;
                materialRaisedButton8.Enabled = false;
            }
        }

        private DataTable getCategories()
        {
            dt = new DataTable();

            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT * FROM CATEGORY ORDER BY ID;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt.Load(dr);
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

            return dt;
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
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            monthCalendar1.SetDate(DateTime.Now);
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы действиетльно хотите удалить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                    fbCon.Open();
                    toDoTransaction = fbCon.BeginTransaction();
                    command = "DELETE FROM TODO WHERE TODO.ID = @current_record;";
                    toDoCommand = new FbCommand(command, fbCon, toDoTransaction);
                    toDoCommand.Parameters.AddWithValue("@current_record", Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value));
                    toDoCommand.CommandType = CommandType.Text;
                    toDoCommand.ExecuteNonQuery();
                    toDoTransaction.Commit();
                }
                catch (Exception)
                {
                    MessageBox.Show("Произошла ошибка при удалении");
                }
                finally
                {
                    fbCon.Close();
                }
            }
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private bool checkCrossingTime(DateTime start, DateTime finish)
        {
            dt = new DataTable();

            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT COUNT(*) FROM TODO WHERE @start_task between TODO.date_task_start and TODO.date_task_end or @end_task between TODO.date_task_start and TODO.date_task_end;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@start_task", start);
                toDoCommand.Parameters.AddWithValue("@end_task", finish);
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt.Load(dr);
                int result = Convert.ToInt32(dt.Rows[0][0]);
                if (result == 0)
                {
                    return false;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                return true;
            }
            finally
            {
                fbCon.Close();
            }
            return true;
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            using (AddEditTask addTask = new AddEditTask())
            {
                addTask.Text = "Добавление задачи";
                addTask.FormClosing += delegate (object fSender, FormClosingEventArgs fe)
                {
                    if(addTask.DialogResult == DialogResult.OK)
                    {
                        addTask.setNameColor();
                        addTask.setCategoryColor();
                        //true если пересекается, false если не пересекается
                        bool crossing = checkCrossingTime(addTask.getStartTime(), addTask.getEndTime());
                        if (crossing)
                        {
                            MessageBox.Show("Пересекается время задачи с другой, укажите другое время");
                            fe.Cancel = true;

                        }
                        else if (addTask.getName().Length == 0 || addTask.getCategory() == 0)
                        {
                            fe.Cancel = true;
                        }
                        else
                        {
                            string name = addTask.getName();
                            DateTime startTask = addTask.getStartTime();
                            DateTime endTask = addTask.getEndTime();
                            int status = getStatusId("В работе");
                            int importance = getImportanceId(addTask.isCheckedImportanceTask() ? "Обязательно" : "Можно отложить");
                            int categoryAdd = addTask.getCategory();
                            int login = getLoginId(ConfigurationManager.AppSettings["Login"]);

                            try
                            {
                                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                                fbCon.Open();
                                toDoTransaction = fbCon.BeginTransaction();
                                command = "INSERT INTO TODO (NAME_TASK, DATE_TASK_START, DATE_TASK_END, ID_STATUS, ID_IMPORTANCE, ID_CATEGORY, ID_USER) " +
                                                    "VALUES (@name_task, @start_date, @end_date, @status, @importance, @category, @user);";
                                toDoCommand = new FbCommand(command, fbCon, toDoTransaction);
                                toDoCommand.Parameters.AddWithValue("@name_task", name);
                                toDoCommand.Parameters.AddWithValue("@start_date", startTask);
                                toDoCommand.Parameters.AddWithValue("@end_date", endTask);
                                toDoCommand.Parameters.AddWithValue("@status", status);
                                toDoCommand.Parameters.AddWithValue("@importance", importance);
                                toDoCommand.Parameters.AddWithValue("@category", categoryAdd);
                                toDoCommand.Parameters.AddWithValue("@user", login);
                                toDoCommand.CommandType = CommandType.Text;
                                toDoCommand.ExecuteNonQuery();
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
                    }
                };
                addTask.ShowDialog();
            }
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            using (AddEditTask editTask = new AddEditTask())
            {
                editTask.Text = "Изменение задачи";
                editTask.setId(Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value));
                editTask.setName(Convert.ToString(dataGridView1[1, dataGridView1.CurrentRow.Index].Value));
                editTask.setStartDate(DateTime.Now);
                editTask.setStartTime(DateTime.Now);
                editTask.setEndTime();
                if (Convert.ToString(dataGridView1[4, dataGridView1.CurrentRow.Index].Value) == "Обязательно")
                {
                    editTask.setCheckedImportanceTask();
                }
                string category = Convert.ToString(dataGridView1[6, dataGridView1.CurrentRow.Index].Value);
                editTask.setCategory(getCategoryId(category), category);
                editTask.FormClosing += delegate (object fSender, FormClosingEventArgs fe)
                {
                    if (editTask.DialogResult == DialogResult.OK)
                    {
                        editTask.setNameColor();
                        editTask.setCategoryColor();
                        //true если пересекается, false если не пересекается
                        bool crossing = checkCrossingTime(editTask.getStartTime(), editTask.getEndTime());
                        if (crossing)
                        {
                            MessageBox.Show("Пересекается время задачи с другой, укажите другое время");
                            fe.Cancel = true;

                        }
                        else if (editTask.getName().Length == 0 || editTask.getCategory() == 0)
                        {
                            fe.Cancel = true;
                        }
                        else
                        {
                            string name = editTask.getName();
                            DateTime startTask = editTask.getStartTime();
                            DateTime endTask = editTask.getEndTime();
                            int status = getStatusId("В работе");
                            int importance = getImportanceId(editTask.isCheckedImportanceTask() ? "Обязательно" : "Можно отложить");
                            int categoryEdit = editTask.getCategory();
                            int login = getLoginId(ConfigurationManager.AppSettings["Login"]);
                            int current_id = editTask.getId();

                            try
                            {
                                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                                fbCon.Open();
                                toDoTransaction = fbCon.BeginTransaction();
                                command = "UPDATE TODO SET TODO.NAME_TASK = @name_task, " +
                                                          "DATE_TASK_START = @start_date, " +
                                                          "DATE_TASK_END = @end_date, " +
                                                          "ID_STATUS = @status, " +
                                                          "ID_IMPORTANCE = @importance, " +
                                                          "ID_CATEGORY = @category, " +
                                                          "ID_USER = @user " +
                                                          "WHERE TODO.ID = @current_id;";
                                toDoCommand = new FbCommand(command, fbCon, toDoTransaction);
                                toDoCommand.Parameters.AddWithValue("@name_task", name);
                                toDoCommand.Parameters.AddWithValue("@start_date", startTask);
                                toDoCommand.Parameters.AddWithValue("@end_date", endTask);
                                toDoCommand.Parameters.AddWithValue("@status", status);
                                toDoCommand.Parameters.AddWithValue("@importance", importance);
                                toDoCommand.Parameters.AddWithValue("@category", categoryEdit);
                                toDoCommand.Parameters.AddWithValue("@user", login);
                                toDoCommand.Parameters.AddWithValue("@current_id", current_id);
                                toDoCommand.CommandType = CommandType.Text;
                                toDoCommand.ExecuteNonQuery();
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
                    }
                };
                editTask.ShowDialog();
            }
            refreshTable(monthCalendar1.SelectionRange.Start.ToShortDateString());
        }

        private void materialSingleLineTextField1_TextChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT TODO.ID AS \"Номер\", " +
                                 "TODO.NAME_TASK AS \"Название задачи\", " +
                                 "CAST(TODO.DATE_TASK_START AS TIME) AS \"Начало\", " +
                                 "CAST(TODO.DATE_TASK_END AS TIME) AS \"Конец\", " +
                                 "IMPORTANCE.NAME_IMPORTANCE AS \"Важность\", " +
                                 "STATUS.NAME_STATUS AS \"Статус\", " +
                                 "CATEGORY.NAME AS \"Категория\" " +
                          "FROM TODO, IMPORTANCE, STATUS, CATEGORY, USER_TODO " +
                          "WHERE TODO.ID_STATUS = STATUS.ID AND " +
                                "TODO.ID_IMPORTANCE = IMPORTANCE.ID AND " +
                                "TODO.ID_CATEGORY = CATEGORY.ID AND " +
                                "TODO.ID_USER = USER_TODO.ID AND " +
                                "USER_TODO.LOGIN = '" + ConfigurationManager.AppSettings["Login"].ToUpper() + "' AND " +
                                "CAST(TODO.DATE_TASK_START AS DATE) = @date_list AND " +
                                "TODO.NAME_TASK LIKE @name;";
                toDoCommand = new FbCommand(command, fbCon);
                toDoCommand.Parameters.AddWithValue("@date_list", monthCalendar1.SelectionRange.Start.ToShortDateString());
                toDoCommand.Parameters.AddWithValue("@name", "%" + materialSingleLineTextField1.Text + "%");
                toDoCommand.CommandType = CommandType.Text;
                dr = toDoCommand.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
            finally
            {
                fbCon.Close();
            }
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 218;
            dataGridView1.Columns[2].Width = 100;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100;
        }

        private void materialSingleLineTextField1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }
    }
}
