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

namespace FFMS
{
    /// <summary>
    /// Calculator.xaml 的交互逻辑
    /// </summary>
    public partial class Calculator : Window
    {
        private string number1 = "";//按下的数字1
        private string number2 = "";//按下的数字2
        private string oper = ""; //按下的运算符
        private double result = 0.0;//结果
        private bool status = false;//是否已经填入两个数

        public Calculator()
        {
            InitializeComponent();
        }

        //数字按钮的事件
        private void number_click(object sender, RoutedEventArgs e){
            if (oper == "")
            {
                if (number1 != null)
                    count.Text = "";
                number1 += ((Button)sender).Content.ToString();
                count.Text += number1;
            }
            else
            {
                number2 += ((Button)sender).Content.ToString();
                count.Text += ((Button)sender).Content.ToString();

                status = true;
            }
           
        }

        //运算符按钮事件
        private void operator_click(object sender, RoutedEventArgs e)
        {
            if (oper == "")
            {
                oper = ((Button)sender).Content.ToString();
                count.Text = count.Text + oper;
            }
        }

        //等于按钮事件
        private void result_click(object sender, RoutedEventArgs e)
        {
            if (status)
            {
                switch (oper)
                {
                    case "+":
                        result = double.Parse(number1) + double.Parse(number2);
                        break;
                    case "-":
                        result = double.Parse(number1) - double.Parse(number2);
                        break;
                    case "*":
                        result = double.Parse(number1) * double.Parse(number2);
                        break;
                    case "/":
                        if (double.Parse(number1) == 0)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = double.Parse(number1) / double.Parse(number2);
                        }
                        break;
                    default:
                        result = double.Parse(number1);
                        break;
                }
                oper = "";
                count.Text = number1 = result.ToString();
                number2 = "";
                result = 0.0;
                status = false;
            }
            else
            {
                oper = "";
                count.Text = number1;
            }
        }

        //退格按钮事件
        private void backspace_click(object sender, RoutedEventArgs e)
        {
            if (number2 != "")
            {
                number2 = number2.Substring(0, number2.Length - 1);
                count.Text = number1 + oper + number2;
            }
            else if (oper != "")
            {
                oper = "";
                count.Text = number1;
            }
            else if(number1 != "")
            {
                number1 = number1.Substring(0, number1.Length - 1);
                count.Text = number1;
            }
        }

        //清空按钮事件
        private void clear_click(object sender, RoutedEventArgs e)
        {
            count.Text = "";
            number1 = "";
            number2 = "";
            oper = ""; 
            result = 0.0;
            status = false;
        }
    }
}
