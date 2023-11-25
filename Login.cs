using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;
using FirebirdSql.Data.FirebirdClient;
using System.Configuration;
using System.IO;

namespace C_Sharp_ToDo_DataManagement
{
    public partial class Login : MaterialForm
    {
        MaterialSkinManager materialSkinManager;
        FbConnectionStringBuilder fb_cons;
        FbConnection fbCon;
        public Login()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue400, Primary.Blue500, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            if (this.materialSingleLineTextField1.Text.Length != 0 && this.materialSingleLineTextField2.Text.Length != 0)
            {
                fb_cons = new FbConnectionStringBuilder();
                fb_cons.Charset = "UTF8";
                fb_cons.UserID = this.materialSingleLineTextField1.Text;
                fb_cons.Password = this.materialSingleLineTextField2.Text;
                fb_cons.Dialect = 3;
                fb_cons.Database = Path.Combine(Directory.GetCurrentDirectory(), "db") + materialLabel3.Text;
                fb_cons.ServerType = 0;
                fb_cons.DataSource = "localhost";
                fb_cons.Port = 3050;
                try
                {
                    fbCon = new FbConnection(fb_cons.ToString());
                    fbCon.Open();
                    fbCon.Close();
                    Program.AddUpdateAppSettings("ConnectionString", fb_cons.ToString());
                    Program.AddUpdateAppSettings("Login", fb_cons.UserID.ToString());
                    Program.AddUpdateAppSettings("Database", materialLabel3.Text.Replace("\\", ""));
                    Main toDoList = new Main();
                    toDoList.Show();
                    this.Hide();
                }
                catch (Exception)
                {
                    MessageBox.Show("Введен неверный логин или пароль или указан неверный путь к БД!");
                }
                finally
                {
                    fbCon.Close();
                }
            }
            else
            {
                materialSingleLineTextField1.BackColor = System.Drawing.Color.Tomato;
                materialSingleLineTextField2.BackColor = System.Drawing.Color.Tomato;
            }
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "db");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetDirectoryName(openFileDialog1.FileName) != Path.Combine(Directory.GetCurrentDirectory(), "db"))
                {
                    MessageBox.Show("Пожалуйста выберите БД в директории db в корне программы", "Неправильная директория с БД", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    materialLabel3.Text = openFileDialog1.FileName.Replace(Path.Combine(Directory.GetCurrentDirectory(), "db"), "");
                }
            }
        }
    }
}
