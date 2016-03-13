using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace FFMS
{
    /// <summary>
    /// AlterInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AlterInfo : Window
    {
        //定义用户名
        private string name;
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        public AlterInfo()
        {
            InitializeComponent();
        }

        public AlterInfo(string name)
        {
            InitializeComponent();
            this.name = name;
            txtName.Text = name;
            txtName.IsEnabled = false;

            string sql = "select * from [user] where [name] = '" + name + "'";
            connection.Open();

            //更新user表中字段
            OleDbCommand sqlcmd3 = new OleDbCommand(sql, connection);
            OleDbDataReader reader = sqlcmd3.ExecuteReader();
            
            if (reader.Read())
            {
                txtAppellation.Text = reader["appellation"].ToString();
                txtPassWord.Password = reader["password"].ToString();
                txtIncome.Text = reader["income"].ToString();
                txtPay.Text = reader["pay"].ToString();
            }

            reader.Close();
            connection.Close();
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            string appellation,password;
            int income,pay;

            if (txtAppellation.Text.Trim() == "" || txtPassWord.Password.Trim() == "" || txtIncome.Text.Trim() == "" || txtPay.Text.Trim() == "")
            {
                MessageBox.Show("请完善修改信息");
            }
            else if (!Regex.IsMatch(txtIncome.Text.Trim(), @"^[0-9]*$") || !Regex.IsMatch(txtPay.Text.Trim(), @"^[0-9]*$"))
            {
                MessageBox.Show("月预收入和月预支出只能为数字");
            }
            else
            {
                appellation = txtAppellation.Text.Trim();
                password = txtPassWord.Password.Trim();
                income = int.Parse(txtIncome.Text.Trim());
                pay = int.Parse(txtPay.Text.Trim());
                //根据用户名修改更新的sql语句
                string sql = "update [user] set [appellation]='" + appellation + "',[password]='" + password + "',[income]='" + income + "',[pay]='" + pay + "' where [name]='" + name + "'";

                connection.Open();

                //更新user表中字段
                OleDbCommand sqlcmd3 = new OleDbCommand(sql, connection);
                sqlcmd3.ExecuteNonQuery();
                MessageBox.Show("修改成功");
                //点击提示信息“确定”后，关闭父窗口
                Close();

                connection.Close();
            }
        }
    }
}
