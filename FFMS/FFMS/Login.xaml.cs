using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FFMS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        public Login()
        {
            InitializeComponent();
            //初始化下拉列表框的列表项
            fill_List();
        }

        //自定义事件，初始化下拉列表框的列表项
        private void fill_List()
        {
            string sql = "select [name] from [user] where [is_first] = false";
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            da.Fill(ds, "Name");

            txtMemberName.DisplayMemberPath = ds.Tables["Name"].Columns["name"].ToString();
            txtMemberName.ItemsSource = ds.Tables["Name"].DefaultView;
            txtMemberName.SelectedIndex = 0;
        }

        //登录按钮事件
        private void login_Click(object sender, RoutedEventArgs e)
        {
            string name = txtMemberName.Text.Trim();
            string password = txtPassWord.Password.Trim();
            string sql = "select * from [user] where [name] = '" + name + "' and [password] = '" + password + "'";
            //判断是否为管理员的sql语句
            string sql2 = "select is_manager from [user] where [name] = '" + name + "'";
            //更新user表中字段isFirstLogin的sql语句,如果用户第一次登陆后将user表中字段isFirstLogin的值更新为false
            string sql3 = "update [user] set [is_first] = false where [is_first] = true and [name]='" + name + "'";

            if (name != "" && password != "")
            {
                try
                {
                    connection.Open();

                    //更新user表中字段is_first
                    OleDbCommand sqlcmd3 = new OleDbCommand(sql3, connection);
                    sqlcmd3.ExecuteNonQuery();


                    OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                    OleDbDataReader reader = sqlcmd.ExecuteReader();
                    if (reader.Read())
                    {
                        //判断是否为管理员
                        OleDbCommand sqlcmd2 = new OleDbCommand(sql2, connection);
                        OleDbDataReader dr = sqlcmd2.ExecuteReader();
                        //将当前用户名传入主窗体中
                        Main main = new Main(name);
                        main.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名与密码不匹配，请确认密码是否正确！");
                    }
                    reader.Close();
                    
                }
                catch
                {
                    MessageBox.Show("Connect Failed!");
                }
                finally
                {
                    connection.Close();
                }
            }
            else if (name == "")
            {
                MessageBox.Show("请输入成员名字！");
            }
            else
            {
                MessageBox.Show("请输入密码！");
            }
        }
        //鼠标进入控件事件
        private void Mouse_Enter(object sender, RoutedEventArgs e)
        {
            if (sender == register)
            {
                register.Foreground = Brushes.Blue;
            }
            else if (sender == findpwd)
            {
                findpwd.Foreground = Brushes.Blue;
            }
            else if (sender == login)
            {
                login.Foreground = Brushes.Blue;
            }
        }
        //鼠标离开控件事件
        private void Mouse_Leave(object sender, RoutedEventArgs e)
        {
            register.Foreground = Brushes.Black;
            findpwd.Foreground = Brushes.Black;
            login.Foreground = Brushes.Black;
        }

        private void Label_Click(object sender, RoutedEventArgs e)
        {
            if (sender == register)
            {
                Register reg = new Register();
                reg.ShowDialog();
            }
            else if (sender == findpwd)
            {
                FindPassword fp = new FindPassword();
                fp.ShowDialog();
            }
        }
    }
}
