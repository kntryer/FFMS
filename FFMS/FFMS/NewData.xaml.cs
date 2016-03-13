using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// NewData.xaml 的交互逻辑
    /// </summary>
    public partial class NewData : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        int tab_count;
        string name;
        string time;

        public NewData()
        {
            InitializeComponent();
        }

        public NewData(int tab_count, string name, string time)
        {
            InitializeComponent();

            this.tab_count = tab_count;
            this.name = name;
            this.time = time;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int money;
            string stream, define, pay_sql, income_sql;

            connection.Open();

            if (txtmoney.Text.Trim() == "")
            {
                MessageBox.Show("金额不能为空！");
            }
            else if (!Regex.IsMatch(txtmoney.Text.Trim(), @"^[0-9]*$"))
            {
                MessageBox.Show("金额必须是数字！");
            }
            else
            {
                money = int.Parse(txtmoney.Text.Trim());
                stream = txtstream.Text.Trim();
                define = txtdefine.Text.Trim();
                pay_sql = "insert into [data]([name],[money],[stream],[time],[define],[is_pay]) values('" + name + "','" + money + "','" + stream + "','" + time + "','" + define + "',true)";
                income_sql = "insert into [data]([name],[money],[stream],[time],[define],[is_pay]) values('" + name + "','" + money + "','" + stream + "','" + time + "','" + define + "',false)";

                switch (tab_count)
                {
                    case 0://支出明细
                        OleDbCommand sqlcmd = new OleDbCommand(pay_sql, connection);
                        sqlcmd.ExecuteNonQuery();
                        MessageBox.Show("新增支出记录成功！");
                        //点击提示信息“确定”后，关闭父窗口Close();
                        Close();
                        break;
                    case 1://收入明细
                        OleDbCommand sqlcmd2 = new OleDbCommand(income_sql, connection);
                        sqlcmd2.ExecuteNonQuery();
                        MessageBox.Show("新增收入记录成功！");
                        //点击提示信息“确定”后，关闭父窗口
                        Close();
                        break;
                }
                connection.Close();
            }
        }
    }
}
