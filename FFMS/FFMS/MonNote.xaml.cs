using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace FFMS
{
    /// <summary>
    /// MonNote.xaml 的交互逻辑
    /// </summary>
    public partial class MonNote : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        //用来判断记事内容是否改变
        bool is_contentChange = false;
        string content = "";
        //是否退出
        bool is_exit = false;
        
        public MonNote()
        {
            InitializeComponent();

            connection.Open();
            //初始化每月记事
            init_note();
        }

         //保存
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (is_contentChange)
            {
                control();
            }
        }

        //操作数据
        private void control()
        {
            try
            {
                string temp = noteContent.Text.ToString().Trim();
                string sql = "";
                if (!temp.Equals(""))
                {
                    if (is_exit)
                    {
                        if (content.Equals(""))
                        {
                            sql = "insert into [note] ([content],[time]) values('" + noteContent.Text.ToString().Trim() + "','" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "')";
                        }
                        else
                        {
                            sql = "update [note] set [content] = '" + noteContent.Text.ToString().Trim() + "'where [time] = '" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "'";
                        }
                        OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                        sqlcmd.ExecuteNonQuery();
                        MessageBox.Show("保存成功！");
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("是否保存？", "每月记事", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            if (content.Equals(""))
                            {
                                sql = "insert into [note] ([content],[time]) values('" + noteContent.Text.ToString().Trim() + "','" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "')";
                            }
                            else
                            {
                                sql = "update [note] set [content] = '" + noteContent.Text.ToString().Trim() + "'where [time] = '" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "'";
                            }
                            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                            sqlcmd.ExecuteNonQuery();
                            MessageBox.Show("保存成功！");
                            is_contentChange = false;
                        }
                    }
                }
                else
                {
                    if (is_exit)
                    {
                        sql = "delete from [note] where [time] = '" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "'";
                        OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                        sqlcmd.ExecuteNonQuery();
                        MessageBox.Show("删除成功！");
                        //更新数据查询
                        init_note();
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("确定删除？", "每月记事", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            sql = "delete from [note] where [time] = '" + searchYear.SelectedValue.ToString() + "/" + searchMonth.SelectedValue.ToString() + "'";
                            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                            sqlcmd.ExecuteNonQuery();
                            MessageBox.Show("删除成功！");
                            //更新数据查询
                            init_note();
                            is_contentChange = false;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Connect Failed!");
            }
        }

        //初始化每月记事
        private void init_note()
        {
            try
            {
                List<object> note_year = new List<object>();
                List<object> note_month = new List<object>();
                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
                note_year.Add(year);
                note_month.Add(month);

                //查询数据库的年月，填充查询下拉框
                string search_sql = "select * from [note]";
                OleDbDataAdapter da = new OleDbDataAdapter(search_sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    note_year.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(0, 4));
                    note_month.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(5, 2));
                }

                note_year = note_year.ToArray().Distinct().ToList();
                searchYear.ItemsSource = note_year;
                searchYear.SelectedIndex = 0;

                note_month = note_month.ToArray().Distinct().ToList();
                searchMonth.ItemsSource = note_month;
                searchMonth.SelectedIndex = 0;

                //判断该月是否已经有记事，如有则显示
                getContent(year, month);
            }
            catch
            {
                MessageBox.Show("Connect Failed!");
            }
        }

        //显示记事内容
        private void getContent(string year, string month)
        {
            //判断该月是否已经有记事，如有则显示
            string content_sql = "select * from [note] where [time] = '" + year + "/" + month + "'";
            OleDbCommand sda = new OleDbCommand(content_sql, connection);
            OleDbDataReader reader = sda.ExecuteReader();
            if (reader.Read())
            {
                OleDbDataAdapter da = new OleDbDataAdapter(content_sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                content = ds.Tables[0].Rows[0]["content"].ToString();
                noteContent.Text = content;
            }
            else
            {
                noteContent.Text = "";
            }
        }

        //下拉框选项改变监听
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (searchYear.SelectedValue != null && searchMonth.SelectedValue != null)
                {
                    string year = searchYear.SelectedValue.ToString();
                    string month = searchMonth.SelectedValue.ToString();
                    getContent(year, month);
                }
            }
            catch
            {
                MessageBox.Show("Connect Failed!");
            }
        }

        //每月记事窗口关闭前的事件
        private void MonNote_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (is_contentChange)
            {
                MessageBoxResult result = MessageBox.Show("是否保存更改记事？", "每月记事", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    is_exit = true;
                    control();
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            connection.Close();
        }

        //记事输入改变监听事件
        private void noteContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noteContent.Text.ToString().Trim() != content)
            {
                is_contentChange = true;
            }
            else
            {
                is_contentChange = false;
            }
        }

        //获取焦点
        private void Image_Enter(object sender, RoutedEventArgs e)
        {
            p_Confirm.Source = new BitmapImage(new Uri("Source/confirm_p.png", UriKind.RelativeOrAbsolute));
        }

        //失去焦点
        private void Image_Leave(object sender, RoutedEventArgs e)
        {
            p_Confirm.Source = new BitmapImage(new Uri("Source/confirm.png", UriKind.RelativeOrAbsolute));
        }
    }
}
