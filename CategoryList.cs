using FirebirdSql.Data.FirebirdClient;
using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class CategoryList : Form
    {
        FbConnection fbCon;
        FbCommand toDoCommand;
        FbDataReader dr;
        string command;
        DataTable dt;
        public CategoryList()
        {
            InitializeComponent();
        }

        private void refreshTable()
        {
            try
            {
                fbCon = new FbConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                fbCon.Open();
                command = "SELECT ID AS \"Номер\", NAME AS \"Название категории\" FROM CATEGORY;";
                toDoCommand = new FbCommand(command, fbCon);
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
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Focus();
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[1];
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public DataGridViewRow getCurrentRecord()
        {
            return dataGridView1.CurrentRow;
        }

        private void CategoryList_Load(object sender, EventArgs e)
        {
            refreshTable();
        }
    }
}
