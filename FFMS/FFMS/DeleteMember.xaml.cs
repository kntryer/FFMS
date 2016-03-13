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

namespace FFMS
{
    /// <summary>
    /// DeleteMember.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteMember : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        public DeleteMember()
        {
            InitializeComponent();
            fill_ListView();
        }

        private void fill_ListView()
        {
            connection.Open();

            String sql = "select [name] from [user] where [is_manager] = false";
            OleDbDataAdapter sda = new OleDbDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            MemberListView.ItemsSource = ds.Tables[0].DefaultView;

            connection.Close();
        }

        private void deleteMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberListView.SelectedItem == null)
            {
                MessageBox.Show("请选择要删除的成员。", "提示信息");
            }
            else
            {
                string name = "";

                connection.Open();

                //按Shift键可以多选要删除的成员
                foreach (DataRowView var in MemberListView.SelectedItems)
                {
                    name = (string)var["name"];
                    string sql = "delete from [user] where [name]='" + name + "'";
                    OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                    sqlcmd.ExecuteNonQuery();
                }
                MessageBox.Show("删除成功", "提示信息");
                //刷新listview
                String sql4 = "select [name] from [user] where [is_manager] = true";
                OleDbDataAdapter sda = new OleDbDataAdapter(sql4, connection);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                MemberListView.ItemsSource = ds.Tables[0].DefaultView;

                connection.Close();
            }
        }
    }
}
