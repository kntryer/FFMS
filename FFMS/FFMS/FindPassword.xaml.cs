using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Windows.Shapes;

namespace FFMS
{
    /// <summary>
    /// FindPassword.xaml 的交互逻辑
    /// </summary>
    public partial class FindPassword : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);
        public FindPassword()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            string name = txtname.Text.Trim();
            string digitalSecurity = txtDigitalSecurity.Password.Trim();
            string sql = "select [password] from [user] where [name] = '" + name + "'and [digitalSecurity] = '" + digitalSecurity + "'";

            connection.Open();

            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
            OleDbDataReader reader = sqlcmd.ExecuteReader();
            if (reader.Read())
            {
                String password = reader.GetString(0);
                MessageBox.Show("用户:" + name + "，密码为：" + password);
            }
            else
                MessageBox.Show("请输入正确的密保！");

            connection.Close();

        }
        private void txtname_LostFocus(object sender, RoutedEventArgs e)
        {
            string name = txtname.Text.Trim();
            string sql2 = "select * from [user] where [name] = '" + name + "'";

            connection.Open();

            OleDbCommand sqlcmd2 = new OleDbCommand(sql2, connection);
            OleDbDataReader reader = sqlcmd2.ExecuteReader();
            if (!reader.Read())
            {
                checkName.Foreground = new SolidColorBrush(Colors.Red);
                checkName.FontSize = 20;
                checkName.Content = "×";
                checkName.ToolTip = "该成员不存在！";
            }
            else
            {
                checkName.Content = "";
                checkName.ToolTip = "";
            }

            connection.Close();
        }
    }
}
