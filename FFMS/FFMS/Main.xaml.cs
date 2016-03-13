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
using System.Windows.Threading;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections;

using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace FFMS
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        private DispatcherTimer dispatcherTimer;

        //当前用户名
        private string name;
        //时间
        private string time;

        private static int P_STATUS = 0;//支出明细，0显示删除按钮，1显示确定删除按钮
        private static int I_STATUS = 0;//收入明细，0显示删除按钮，1显示确定删除按钮

        private ArrayList P_id_list = new ArrayList();//支出明细删除id
        private ArrayList I_id_list = new ArrayList();//收入明细删除id

        public Main()
        {
            InitializeComponent();
            //显示时间
            displayTime();
            //初始化DataGrid
            init_dataGrid();
            //初始化下拉列表框的列表项
            init_search();
        }

        //带参数的构造函数
        public Main(string name)
        {
            InitializeComponent();

            this.name = name;
            lblName.Content = "当前用户：" + name;

            //限制权限
            setAccess(name);
            //显示时间
            displayTime();
            //初始化DataGrid
            init_dataGrid();
            //初始化下拉列表框的列表项
            init_search();
        }

        //填充DataGrid的数据
        private void init_dataGrid()
        {
            try
            {
                connection.Open();
                string pay_sql = "select * from [data] where [is_pay] = true";
                OleDbDataAdapter pay_sda = new OleDbDataAdapter(pay_sql, connection);
                DataSet pay_ds = new DataSet();
                pay_sda.Fill(pay_ds);
                pay_data.ItemsSource = pay_ds.Tables[0].DefaultView;

                string income_sql = "select * from [data] where [is_pay] = false";
                OleDbDataAdapter income_sda = new OleDbDataAdapter(income_sql, connection);
                DataSet income_ds = new DataSet();
                income_sda.Fill(income_ds);
                income_data.ItemsSource = income_ds.Tables[0].DefaultView;
                
                //月度统计
                fill_count_dataGrid();
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

        //初始化月度统计
        private void fill_count_dataGrid()
        {
            string month = DateTime.Now.Month.ToString();
            string pay_sql = "";
            string income_sql = "";
            //获取所有收支的成员名字列表
            List<object> name = new List<object>();
            string sql = "select distinct [name] from [data]";
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                name.Add(ds.Tables[0].Rows[i]["name"].ToString());
            }

            string Mname = "";//最大收入成员
            string Nname = "";//最大净收入成员

            int Max_Mmoney = 0;//最大收入
            int Max_Nmoney = 0;//最大净收入

            ObservableCollection<Data> dataList = new ObservableCollection<Data>();

            //初始化Data类列表
            for (int i = 0; i < name.Count; i++)
            {
                pay_sql = "select sum([money]) from [data] where [name]='" + name[i].ToString() + "' and [time] like '%/" + month + "/%' and [is_pay] = true";
                income_sql = "select sum([money]) from [data] where [name]='" + name[i].ToString() + "' and [time] like '%/" + month + "/%' and [is_pay] = false";

                if (getMoney(income_sql) > Max_Mmoney)
                {
                    Max_Mmoney = getMoney(income_sql);
                    Mname = name[i].ToString();
                }
                if ((getMoney(income_sql) - getMoney(pay_sql)) > Max_Nmoney)
                {
                    Max_Nmoney = (getMoney(income_sql) - getMoney(pay_sql));
                    Nname = name[i].ToString();
                }
                dataList.Add(new Data()
                {
                    Name = name[i].ToString(),
                    Income = getMoney(income_sql),
                    Pay = getMoney(pay_sql),
                    NetIncome = (getMoney(income_sql) - getMoney(pay_sql)),
                });
            }

            initText(Mname, Nname);
            if (!Mname.Equals(""))
            {
                //合计
                pay_sql = "select sum([money]) from [data] where [time] like '%/" + month + "/%' and [is_pay] = true";
                income_sql = "select sum([money]) from [data] where [time] like '%/" + month + "/%' and [is_pay] = false";
                dataList.Add(new Data()
                {
                    Name = "合计",
                    Income = getMoney(income_sql),
                    Pay = getMoney(pay_sql),
                    NetIncome = (getMoney(income_sql) - getMoney(pay_sql)),
                });
            }
            //绑定数据到datagrid
            count_data.ItemsSource = dataList;
            this.DataContext = this;
        }

        //初始化走马灯文字
        private void initText(string Mname,string Nname)
        {
            Random ran = new Random();
            List<string> lists = new List<string>();
            lists.Add("，家里的顶梁柱，为他鼓掌！！！");
            lists.Add("，家里的摇钱树，欢呼吧！！！");
            lists.Add("，家里的大牛，我们膜拜吧！！！");
            lists.Add("，家里的比尔，让我们抱大腿吧！！！");
            lists.Add("，家里的聚宝盆，让我们抱抱！！！");
            lists.Add("，家里的财神爷，我们仰望吧！！！");
            lists.Add("，家里走在经济的最前沿的先驱，鼓掌！！！");
            if (Mname.Equals(""))
            {
                M_income.Content = "";
                N_income.Content = "";
            }
            else
            {
                M_income.Content = "本月收入冠军 " + Mname + lists[ran.Next(0, 6)];
                N_income.Content = "    本月净收入冠军 " + Nname + lists[ran.Next(0, 6)];
            }
        }

        //获取字段和
        private int getMoney(string sql)
        {
            object money;
            OleDbCommand sda = new OleDbCommand(sql, connection);
            money = sda.ExecuteScalar();
            if (money.ToString().Equals(""))
                money = 0;
            return int.Parse(money.ToString());
        }

        //初始化下拉列表框的列表项
        private void init_search()
        {
            try
            {
                connection.Open();
                List<object> pay_year = new List<object>();
                List<object> pay_month = new List<object>();
                List<object> pay_name = new List<object>();
                pay_year.Add("全部");
                pay_month.Add("全部");
                pay_name.Add("全部");
                List<object> income_year = new List<object>();
                List<object> income_month = new List<object>();
                List<object> income_name = new List<object>();
                income_year.Add("全部");
                income_month.Add("全部");
                income_name.Add("全部");

                string year_sql = "select * from [data]";
                OleDbDataAdapter da = new OleDbDataAdapter(year_sql, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if ((bool)(ds.Tables[0].Rows[i]["is_pay"]))
                    {
                        pay_year.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(0, 4));
                        pay_month.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(5, 2));
                        pay_name.Add(ds.Tables[0].Rows[i]["name"].ToString());
                    }
                    else
                    {
                        income_year.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(0, 4));
                        income_month.Add(ds.Tables[0].Rows[i]["time"].ToString().Substring(5, 2));
                        income_name.Add(ds.Tables[0].Rows[i]["name"].ToString());
                    }
                }
                //支出
                pay_year = pay_year.ToArray().Distinct().ToList();
                p_searchYear.ItemsSource = pay_year;
                p_searchYear.SelectedIndex = 0;

                pay_month = pay_month.ToArray().Distinct().ToList();
                p_searchMonth.ItemsSource = pay_month;
                p_searchMonth.SelectedIndex = 0;

                pay_name = pay_name.ToArray().Distinct().ToList();
                p_searchName.ItemsSource = pay_name;
                p_searchName.SelectedIndex = 0;

                //收入
                income_year = income_year.ToArray().Distinct().ToList();
                i_searchYear.ItemsSource = income_year;
                i_searchYear.SelectedIndex = 0;

                income_month = income_month.ToArray().Distinct().ToList();
                i_searchMonth.ItemsSource = income_month;
                i_searchMonth.SelectedIndex = 0;

                income_name = income_name.ToArray().Distinct().ToList();
                i_searchName.ItemsSource = income_name;
                i_searchName.SelectedIndex = 0;
            }
            catch
            {
                //MessageBox.Show("Connect Failed!");
            }
            finally
            {
                connection.Close();
            }
        }

        //自定义方法，限制非管理员的权限
        private void setAccess(String name)
        {
            string sql = "select is_manager from [user] where [name] = '" + name + "'";

            connection.Open();
            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
            OleDbDataReader reader = sqlcmd.ExecuteReader();
            if (reader.Read())
            {
                //判断是否为管理员，限制非管理员的设置
                OleDbCommand sqlcmd2 = new OleDbCommand(sql, connection);
                OleDbDataReader dr = sqlcmd2.ExecuteReader();
                if (dr.Read() && dr.GetBoolean(0))
                {
                    //管理员设置，无限制条件
                }
                else
                {
                    //非管理员设置
                    delete.IsEnabled = false;
                    p_Delete.IsEnabled = false;
                    p_Delete.Source = new BitmapImage(new Uri("Source/delete_p.png", UriKind.RelativeOrAbsolute));
                    i_Delete.IsEnabled = false;
                    i_Delete.Source = new BitmapImage(new Uri("Source/delete_p.png", UriKind.RelativeOrAbsolute));
                    deleteUser.IsEnabled = false;
                    familyFinance.IsEnabled = false;
                }
            }
            reader.Close();
            connection.Close();
        }

        //自定义事件，显示时间
        private void displayTime()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            lblTime.Content = "时间：" + DateTime.Now.ToString();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = "时间：" + DateTime.Now.ToString();
        }

        //菜单点击事件
        private void menu_click(object sender, RoutedEventArgs e)
        {
            if (sender == add)//新增
            {
                New_Click(sender, e);
            }
            else if (sender == delete)//删除
            {
                Delete_Click(sender, e);
            }
            else if (sender == search)//查询
            {
                Search_Click(sender, e);
            }
            else if (sender == changeUser)//切换用户
            {
                MessageBoxResult msg = MessageBox.Show("你确定要切换用户？", "提示", MessageBoxButton.YesNo);
                if (msg == MessageBoxResult.Yes)
                {
                    Login mLogin = new Login();
                    mLogin.Show();
                    Close();
                }
            }
            else if (sender == exit)//退出系统
            {
                MessageBoxResult msg = MessageBox.Show("你确定要退出系统？", "提示", MessageBoxButton.YesNo);
                if (msg == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            else if (sender == dataImport)//数据库导入
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".accdb";
                ofd.Filter = "Access|*.accdb";
                ofd.Title = "导入Access数据库";
                if (ofd.ShowDialog() == true)
                {
                    string path_import = ofd.FileName;
                    string path_data = "APP_Data/FFMS.accdb";

                    if (DataImport(path_import, path_data))
                    {
                        MessageBox.Show("数据库导入成功");
                    }
                    else
                    {
                        MessageBox.Show("数据库导入失败");
                    }
                }
            }
            else if (sender == dataBackup)//数据库备份
            {
                string path_data = "APP_Data/FFMS.accdb";
                string path_back = "APP_Data/FFMS_bat.accdb";
                if (Backup(path_data, path_back))
                {
                    MessageBox.Show("数据库备份成功");
                }
                else
                {
                    MessageBox.Show("数据库备份失败");
                }
            }
            else if (sender == dataRecover)//数据库还原
            {
                string path_back = "APP_Data/FFMS_bat.accdb";
                string path_data = "APP_Data/FFMS.accdb";
                if (RecoverAccess(path_back, path_data))
                {
                    MessageBox.Show("数据库还原成功");
                }
                else
                {
                    MessageBox.Show("数据库还原失败");
                }
            }
            else if (sender == payExport)//导出支出明细Excel
            {
                ExportExcel.ExportToExcel(pay_data);
            }
            else if (sender == incomeExport)//导出收入明细Excel
            {
                ExportExcel.ExportToExcel(income_data);
            }
            else if (sender == accessExport)//导出月度统计Excel
            {
                //ExportExcel.ExportToExcel(count_data);
            }
            else if (sender == pay)//支出明细
            {
                tab.SelectedIndex = 0;
            }
            else if (sender == income)//收入明细
            {
                tab.SelectedIndex = 1;
            }
            else if (sender == count)//月度统计
            {
                tab.SelectedIndex = 2;
            }
            else if (sender == assets)//家庭财产
            {
                Information info = new Information(getAssets());
                info.Title = "家庭财产";
                info.Show();
            }
            else if (sender == budget)//每月预算
            {
                new MonNote().Show();
            }
            else if (sender == alterInfo)//修改资料
            {
                AlterInfo alter = new AlterInfo(name);
                alter.Title = "修改资料";
                alter.Show();
            }
            else if (sender == deleteUser)//删除成员
            {
                new DeleteMember().ShowDialog();
                //重新刷新datagrid
                init_dataGrid();
            }
            else if (sender == message)//提醒信息
            {
                Information info = new Information(getMessage());
                info.Title = "提醒信息";
                info.Show();
            }
            else if (sender == function)//功能介绍
            {
                String infomation = "1.收支明细，记录了所有的收支信息\n"
                    +"2.月度统计，统计了当前月的收支统计\n"
                    +"3.数据部分，可以导入数据库，导出数据库及数据库备份，并且还可以导出支出明细，收入明细，月度统计的Excel\n"
                    +"4.家庭财产统计了所有月的净收入，上个月的总支出及当前月的目前支出\n"
                    +"5.每月记事，功能强大，可以查询某年某月的记事，可以增加当前月的记事，还有当记事内容被删掉";
                Information info = new Information(infomation);
                info.Title = "功能介绍";
                info.Show();
            }
            else if (sender == calculator)//计算器
            {
                Calculator mCalculator = new Calculator();
                mCalculator.Show();
            }
            else if (sender == about)//关于
            {
                String infomation = "  该软件由学生小组完成，并且是免费的。感谢你的使用！如果有好的建议,可以把建议发到邮箱1162676656@qq.com。";
                Information info = new Information(infomation);
                info.Title = "关于";
                info.Show();
            }
        }

        //家庭财产
        private string getAssets()
        {
            string temp = "";
            string last_month = DateTime.Now.AddMonths(-1).ToString("MM");
            string current_month = DateTime.Now.Month.ToString();
            if (last_month.Length == 1)
            {
                last_month = "0" + last_month;
            }
            if (current_month.Length == 1)
            {
                current_month = "0" + current_month;
            }
            try
            {
                connection.Open();
                string pay_sql = "select sum([money]) from [data] where [is_pay] = true";
                string income_sql = "select sum([money]) from [data] where [is_pay] = false";
                temp = "家庭总财产：" + (getMoney(income_sql) - getMoney(pay_sql)) + "\n";
                string last_mon_sql = "select sum([money]) from [data] where [time] like '%/" + last_month + "/%' and [is_pay] = true";
                temp += "上个月总支出：" + getMoney(last_mon_sql) + "\n";
                string current_mon_sql = "select sum([money]) from [data] where [time] like '%/" + current_month + "/%' and [is_pay] = true";
                temp += "这月目前支出：" + getMoney(current_mon_sql) + "\n";
            }
            catch
            {
                MessageBox.Show("Connect Failed!");
            }
            finally
            {
                connection.Close();
            }
            return temp;
        }

        //警告信息
        private string getMessage()
        {
            string temp = "";
            string last_month = DateTime.Now.AddMonths(-1).ToString("MM");
            string current_month = DateTime.Now.Month.ToString();
            if (last_month.Length == 1)
            {
                last_month = "0" + last_month;
            }
            if (current_month.Length == 1)
            {
                current_month = "0" + current_month;
            }
            try
            {
                connection.Open();
                string last_mon_sql = "select sum([money]) from [data] where [time] like '%/" + last_month + "/%' and [is_pay] = true";
                string current_mon_sql = "select sum([money]) from [data] where [time] like '%/" + current_month + "/%' and [is_pay] = true";
                if ((getMoney(current_mon_sql) - getMoney(last_mon_sql)) > 0)
                {
                    temp = "这月目前总支出已超出了上个月的" + (getMoney(current_mon_sql) - getMoney(last_mon_sql)) + "，请合理使用家庭财产，预防财政赤字！！！";
                }
                else
                {
                    temp = "暂时没有警告信息！";
                }
                
            }
            catch
            {
                MessageBox.Show("Connect Failed!");
            }
            finally
            {
                connection.Close();
            }
            return temp;
        }

        /**
        * 导入Access数据库
        * path_data 要导入的数据库绝对路径
        * path_back 导入到的数据库绝对路径
        * */
        public bool DataImport(string path_import, string path_data)
        {
            try
            {
                if (!File.Exists(path_data))
                {
                    //throw new Exception("源数据库不存在,无法导入");
                }
                File.Copy(path_import, path_data, true);
            }
            catch
            //(IOException ixp)
            {
                return false;
                //throw new Exception(ixp.ToString());
            }
            return true;
        }

        /**
         * 备份Access数据库
         * path_data 要备份的数据库绝对路径
         * path_back 备份到的数据库绝对路径
         * */
        public bool Backup(string path_data, string path_back)
        {
            try
            {
                if (!File.Exists(path_data))
                {
                    //throw new Exception("源数据库不存在,无法备份");
                }
                File.Copy(path_data, path_back, true);
            }
            catch
            //(IOException ixp)
            {
                return false;
                //throw new Exception(ixp.ToString());
            }
            return true;
        }

        /**
         * 还原Access数据库
         * path_back 备份的数据库绝对路径
         * path_data 要还原的数据库绝对路径
         * */
        public bool RecoverAccess(string path_back, string path_data)
        {

            try
            {
                if (!File.Exists(path_back))
                {
                    //throw new Exception("备份数据库不存在,无法还原");
                }
                File.Copy(path_back, path_data, true);
            }
            catch
            //(IOException ixp)
            {
                return false;
                //throw new Exception(ixp.ToString());
            }
            return true;
        }

        //新增
        private void New_Click(object sender, RoutedEventArgs e)
        {
            //将当前时间传入NewData窗口中
            time = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString();
            NewData newData = new NewData(tab.SelectedIndex, name, time);
            newData.ShowDialog();
            //刷新数据
            init_dataGrid();
            init_search();
        }

        //删除
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            long id;

            switch (tab.SelectedIndex)
            {
                case 0://支出明细
                    if (P_STATUS == 0)
                    {
                        pay_data.Columns[5].Visibility = Visibility.Visible;
                        P_STATUS = 1;
                    }
                    else
                    {
                        for (int i = 0; i < P_id_list.Count; i++)
                        {
                            connection.Open();
                            //执行删除操作
                            id = long.Parse(Convert.ToString(P_id_list[i]));
                            //id不能用单引号括起来，因为是数字类型，而不是字符串
                            string sql = "delete from [data] where [id]=" + id + "and is_pay = true";
                            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                            sqlcmd.ExecuteNonQuery();
                        }
                        //关闭连接
                        connection.Close();
                        //清空删除id列表
                        P_id_list.RemoveRange(0, P_id_list.Count);
                        //重新刷新daagrid
                        init_dataGrid();
                        //将checkbox列置于隐藏状态
                        pay_data.Columns[5].Visibility = Visibility.Hidden;
                        //恢复删除按钮图标
                        P_STATUS = 0;
                    }
                    break;
                case 1://收入明细
                    if (I_STATUS == 0)
                    {
                        income_data.Columns[5].Visibility = Visibility.Visible;
                        I_STATUS = 1;
                    }
                    else
                    {
                        for (int i = 0; i < I_id_list.Count; i++)
                        {
                            connection.Open();
                            //执行删除操作
                            id = long.Parse(Convert.ToString(I_id_list[i]));
                            string sql = "delete from [data] where [id]=" + id + "and is_pay = false";
                            OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                            sqlcmd.ExecuteNonQuery();
                        }
                        //关闭连接
                        connection.Close();
                        //清空删除id列表
                        P_id_list.RemoveRange(0, P_id_list.Count);
                        //重新刷新daagrid
                        init_dataGrid();
                        //将checkbox列置于隐藏状态
                        income_data.Columns[5].Visibility = Visibility.Hidden;
                        //恢复删除按钮图标
                        I_STATUS = 0;

                    }
                    break;
            }

        }

        //删除checkbox点击事件
        private void cbDelete_Click(object sender, RoutedEventArgs e)
        {

            if (tab.SelectedIndex == 0)
            {
                //获取选中的id
                DataRowView mySelectedElement = (DataRowView)pay_data.SelectedItem;
                String id = mySelectedElement.Row[0].ToString();
                if (P_id_list.Count != 0)
                {
                    bool flag = true;
                    String tid;
                    for (int i = 0; i < P_id_list.Count; i++)
                    {
                        tid = P_id_list[i].ToString();
                        if (tid == id)
                        {
                            P_id_list.RemoveAt(i);
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        P_id_list.Add(id);
                    }
                }
                else
                {
                    P_id_list.Add(id);
                }
            }
            if(tab.SelectedIndex == 1)
            {
                //获取选中的id
                DataRowView mySelectedElement = (DataRowView)income_data.SelectedItem;
                String id = mySelectedElement.Row[0].ToString();
                if (I_id_list.Count != 0)
                {
                    bool flag = true;
                    String tid;
                    for (int i = 0; i < I_id_list.Count; i++)
                    {
                        tid = I_id_list[i].ToString();
                        if (tid == id)
                        {
                            I_id_list.RemoveAt(i);
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        I_id_list.Add(id);
                    }
                }
                else
                {
                    I_id_list.Add(id);
                }
            }
                    
        }

        //查询
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (tab.SelectedIndex == 0)
            {
                string p_year = p_searchYear.Text.Trim();
                string p_month = p_searchMonth.Text.Trim();
                string p_time = "%";
                string p_name = p_searchName.Text.ToString().Trim();
                if (p_name.Equals("全部"))
                {
                    p_name = "%";
                }
                if (p_year.Equals("全部"))
                {
                    p_year = "%";
                }
                if(p_month.Equals("全部"))
                {
                    p_month = "%";
                }
                p_time = p_year + "/" + p_month + "/%";
                string sql = "select * from [data] where [name] like '" + p_name + "' and [time] like '" + p_time + "' and [is_pay] = true";
                fill_dataGrid(sql, pay_data);
            }
            else if (tab.SelectedIndex == 1)
            {
                string i_year = i_searchYear.Text.Trim();
                string i_month = i_searchMonth.Text.Trim();
                string i_time = "%";
                string i_name = i_searchName.Text.ToString().Trim();
                if (i_name.Equals("全部"))
                {
                    i_name = "%";
                }
                if (i_year.Equals("全部"))
                {
                    i_year = "%";
                }
                if (i_month.Equals("全部"))
                {
                    i_month = "%";
                }
                i_time = i_year + "/" + i_month + "%";
                string sql = "select * from [data] where [name] like '" + i_name + "' and [time] like '" + i_time + "' and [is_pay] = false";
                fill_dataGrid(sql, income_data);
            }
            
        }

        //查询填充数据
        private void fill_dataGrid(string sql, DataGrid datagrid)
        {
            try
            {
                connection.Open();
                OleDbDataAdapter sda = new OleDbDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                datagrid.ItemsSource = ds.Tables[0].DefaultView;
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

        //获取焦点
        private void Image_Enter(object sender, RoutedEventArgs e)
        {
            if (sender == p_New || sender == i_New)
            {
                p_New.Source = new BitmapImage(new Uri("Source/new_p.png", UriKind.RelativeOrAbsolute));
                i_New.Source = new BitmapImage(new Uri("Source/new_p.png", UriKind.RelativeOrAbsolute));
            }
            else if (sender == p_Delete || sender == i_Delete)
            {
                switch (tab.SelectedIndex)
                {
                    case 0:
                        if (P_STATUS == 0)
                        {
                            p_Delete.Source = new BitmapImage(new Uri("Source/delete_p.png", UriKind.RelativeOrAbsolute));
                            p_Delete.ToolTip = "删除";

                        }
                        else
                        {
                            p_Delete.Source = new BitmapImage(new Uri("Source/confirm_delete_p.png", UriKind.RelativeOrAbsolute));
                            p_Delete.ToolTip = "确定删除";
                        }
                        break;
                    case 1:
                        if (I_STATUS == 0)
                        {
                            i_Delete.Source = new BitmapImage(new Uri("Source/delete_p.png", UriKind.RelativeOrAbsolute));
                            i_Delete.ToolTip = "删除";
                        }
                        else
                        {
                            i_Delete.Source = new BitmapImage(new Uri("Source/confirm_delete_p.png", UriKind.RelativeOrAbsolute));
                            i_Delete.ToolTip = "确定删除";
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (sender == p_Search || sender == i_Search)
            {
                p_Search.Source = new BitmapImage(new Uri("Source/search_p.png", UriKind.RelativeOrAbsolute));
                i_Search.Source = new BitmapImage(new Uri("Source/search_p.png", UriKind.RelativeOrAbsolute));
            }
        }

        //失去焦点
        private void Image_Leave(object sender, RoutedEventArgs e)
        {
            if (sender == p_New || sender == i_New)
            {
                p_New.Source = new BitmapImage(new Uri("Source/new.png", UriKind.RelativeOrAbsolute));
                i_New.Source = new BitmapImage(new Uri("Source/new.png", UriKind.RelativeOrAbsolute));
            }
            else if (sender == p_Delete || sender == i_Delete)
            {
                switch (tab.SelectedIndex)
                {
                    case 0:
                        if (P_STATUS == 0)
                        {
                            p_Delete.Source = new BitmapImage(new Uri("Source/delete.png", UriKind.RelativeOrAbsolute));
                            p_Delete.ToolTip = "删除";
                        }
                        else
                        {
                            p_Delete.Source = new BitmapImage(new Uri("Source/confirm_delete.png", UriKind.RelativeOrAbsolute));
                            p_Delete.ToolTip = "确定删除";
                        }
                        break;
                    case 1:
                        if (I_STATUS == 0)
                        {
                            i_Delete.Source = new BitmapImage(new Uri("Source/delete.png", UriKind.RelativeOrAbsolute));
                            i_Delete.ToolTip = "删除";
                        }
                        else
                        {
                            i_Delete.Source = new BitmapImage(new Uri("Source/confirm_delete.png", UriKind.RelativeOrAbsolute));
                            i_Delete.ToolTip = "确定删除";
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (sender == p_Search || sender == i_Search)
            {
                p_Search.Source = new BitmapImage(new Uri("Source/search.png", UriKind.RelativeOrAbsolute));
                i_Search.Source = new BitmapImage(new Uri("Source/search.png", UriKind.RelativeOrAbsolute));
            }
        }

        //datagrid 行自动生成编号
        private void pay_data_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;

        }

        //datagrid 行自动生成编号
        private void income_data_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;

        }

        //datagrid 行自动生成编号
        private void count_data_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
