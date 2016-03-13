using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|/APP_Data/FFMS.accdb;Persist Security Info=True";
        OleDbConnection connection = new OleDbConnection(connectionString);

        public Register()
        {
            InitializeComponent();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            string name = txtname.Text.Trim();
            string digitalsecurity = txtDigitalSecurity.Password.Trim();
            string password = txtPassWord.Password.Trim();
            string CPassWord = txtCPassWord.Password.Trim();
            //注册用户的sql语句
            string sql = "insert into [user]([name],[digitalSecurity],[password]) values('" 
                + txtname.Text.Trim() + "','" + txtDigitalSecurity.Password.Trim() + "','" + txtPassWord.Password.Trim() + "')";
            //判断注册用户是否已经存在的sql语句
            string sql2 = "select * from [user] where [name] = '" + name + "'";
            //是否为该系统第一个登陆，第一个登陆为管理员
            string is_manager_sql = "select * from [user]";
            //sql += txtname.Text.Trim() + "','";
            //sql += txtDigitalSecurity.Password.Trim() + "','";
            //sql += txtPassWord.Password.Trim() + "')";

            if (name != "")
            {
                //try
                //{
                    connection.Open();

                    //判断注册用户是否存在
                    OleDbCommand sqlcmd2 = new OleDbCommand(sql2, connection);
                    OleDbDataReader reader = sqlcmd2.ExecuteReader();
                    if (reader.Read())
                    {
                        MessageBox.Show("用户已注册过，请登录！");
                    }
                    else
                    {
                        //密保的位数
                        int digitalSecurityLength = digitalsecurity.Length;
                        //用户未注册，判断密码是否一致
                        if (password == CPassWord && digitalSecurityLength >= 1 && digitalSecurityLength <= 6)
                        {
                            //判断密码六位以上，十六位以下
                            int length = password.Length;

                            if (length >= 6 && length <= 16)
                            {
                                OleDbCommand sda = new OleDbCommand(is_manager_sql, connection);
                                OleDbDataReader reader1 = sda.ExecuteReader();
                                if (!reader1.Read())
                                {
                                    sql = "insert into [user]([name],[digitalSecurity],[password],[is_manager]) values('"
                                           + txtname.Text.Trim() + "','" + txtDigitalSecurity.Password.Trim() + "','" + txtPassWord.Password.Trim() + "',true)";
                                }
                                OleDbCommand sqlcmd = new OleDbCommand(sql, connection);
                                sqlcmd.ExecuteNonQuery();
                                MessageBox.Show("注册成功！");
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("密码要求六位以上，十六位以下！");
                            }
                        }
                        else
                        {
                            if (!(digitalSecurityLength >= 1 && digitalSecurityLength <= 6))
                                MessageBox.Show("密保要求一位以上，六位以下！");
                            else
                                MessageBox.Show("密码不一致！");
                        }
                    }
                //}
                //catch
                //{
                //    MessageBox.Show("Connect Failed!");
                //}
                //finally
                //{
                //    connection.Close();
                //}
            }
            else
            {
                MessageBox.Show("请输入成员名字！");
            }
            
        }

        private void txtname_LostFocus(object sender, RoutedEventArgs e)
        {
            string name = txtname.Text.Trim();
            if (name != "")
            {

                string sql2 = "select * from [user] where [name] = '" + name + "'";

                connection.Open();

                OleDbCommand sqlcmd2 = new OleDbCommand(sql2, connection);
                OleDbDataReader reader = sqlcmd2.ExecuteReader();
                if (reader.Read())
                {
                    checkName.Foreground = new SolidColorBrush(Colors.Red);
                    checkName.FontSize = 20;
                    checkName.Content = "×";
                    checkName.ToolTip = "该成员名字已注册！";
                }
                else
                {
                    checkName.Foreground = new SolidColorBrush(Colors.Green);
                    checkName.FontSize = 20;
                    checkName.Content = "√";
                    checkName.ToolTip = "该成员名字可以使用！";
                }

                connection.Close();
            }
            else
            {
                checkName.Foreground = new SolidColorBrush(Colors.Red);
                checkName.FontSize = 20;
                checkName.Content = "×";
                checkName.ToolTip = "成员名字不能为空！";
            }
        }

        private void txtCPassWord_PasswordChanged(object sender, RoutedEventArgs e)
        {
            checkPassword.Foreground = new SolidColorBrush(Colors.Red);
            String p1 = txtPassWord.Password;
            String p2 = txtCPassWord.Password;
            if (p1 != p2)
            {
                checkPassword.Content = "密码不一致";
            }
            else
            {
                checkPassword.Content = "";
            }
        }

        private void txtPassWord_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = txtPassWord.Password.Trim();
            string strRegular = @"^\d{6,16}$";
            Boolean regularTrue = Regex.IsMatch(password, strRegular);
            if (!regularTrue)
            {
                checkPassordLength.Foreground = new SolidColorBrush(Colors.Red);
                checkPassordLength.Content = "输入6-16位";
            }
            else
            {
                checkPassordLength.Content = "";
            }

        }

        private void txtDigitalSecurity_LostFocus(object sender, RoutedEventArgs e)
        {
            string digitalSecurity = txtDigitalSecurity.Password.Trim();
            string strRegular = @"^\d{1,6}$";
            Boolean regularTrue = Regex.IsMatch(digitalSecurity, strRegular);
            if (!regularTrue)
            {
                checkPassordLength.Foreground = new SolidColorBrush(Colors.Red);
                checkPassordLength.Content = "输入1-6位";
            }
            else
            {
                checkPassordLength.Content = "";
            }
        }
    }
}
