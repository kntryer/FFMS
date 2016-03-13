using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FFMS
{
    public class Data : INotifyPropertyChanged
    {
        string name;
        int income;
        int pay;
        int netincome;

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    INotifyPropertyChanged("Name");
                }
            }
        }

        public int Income
        {
            get { return income; }
            set
            {
                if (value != income)
                {
                    income = value;
                    INotifyPropertyChanged("Income");
                }
            }
        }

        public int Pay
        {
            get { return pay; }
            set
            {
                if (value != pay)
                {
                    pay = value;
                    INotifyPropertyChanged("Pay");
                }
            }
        }

        public int NetIncome
        {
            get { return netincome; }
            set
            {
                if (value != netincome)
                {
                    netincome = value;
                    INotifyPropertyChanged("NetIncome");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void INotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
