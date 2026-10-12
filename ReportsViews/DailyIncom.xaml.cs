using System;
using System.Collections.Generic;
using System.Data;
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

namespace HijamaClinicManagement.ReportsViews
{
    /// <summary>
    /// Interaction logic for DailyIncom.xaml
    /// </summary>
    public partial class DailyIncom : UserControl
    {
        // Intialize instance from BillAdapter Class 
        DBHandler.BillAdapter billAdapter = new DBHandler.BillAdapter();
        public DailyIncom()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_DBill();
            getDailyIncomeValue();
        }

        private void getDailyIncomeValue()
        {
            double cash = 0, network = 0;
            DateTime date = DateTime.Now;
            //Assign date to textblock
            datenow.Text = date.ToString("dd/MM/yyy");
            //Query for payment_type ='نقدي وشبكة'
            double cash_paid = billAdapter.DailyBill_PaidCash(date.ToString());
            double network_paid = billAdapter.DailyBill_PaidNetwork(date.ToString());

            //Query for payment_type !='نقدي وشبكة'
            SqlDataReader sdr = billAdapter.DailyBill_Content(date.ToString());
            DataTable dt = new DataTable();
            dt.Clear();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //نقدي
                    if (dt.Rows[i].Field<string>(0).Equals("نقدي"))
                    {
                        cash = cash + Convert.ToDouble(dt.Rows[i].Field<double>(1));
                      //  MessageBox.Show("" + Convert.ToDouble(dt.Rows[i].Field<double>(1)), "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        tbc.Text = cash.ToString();
                    }

                    //شبكة
                    if (dt.Rows[i].Field<string>(0).Equals("شبكة "))
                    {
                        network = network + Convert.ToDouble(dt.Rows[i].Field<double>(1));
                       // MessageBox.Show("" + network, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        tbn.Text = network.ToString();
                       // MessageBox.Show("" + tbn.Text, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }

                    //نقدي وشبكة
                    if (dt.Rows[i].Field<string>(0).Equals("نقدي وشبكة "))
                    {
                        if(cash_paid > 0)
                        {
                            cash = cash + cash_paid;
                            tbc.Text = cash.ToString();
                        }
                        if (network_paid > 0)
                        {
                            network = network + network_paid;
                            tbn.Text = network.ToString();
                        }
                    }
                    
                }
                sdr.Close();
                DBHandler.ConnectionDB.CloseConnection();
                //tbt.Text = (dt.Rows[0].Field<double>(1) + dt.Rows[1].Field<double>(1)).ToString();
                tbt.Text = (cash + network).ToString();
                //MessageBox.Show("" + tbt.Text, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                MessageBox.Show("لايوجد بيانات وفق تاريخ اليوم", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                sdr.Close();
                DBHandler.ConnectionDB.CloseConnection();
            }
        }

        private void Clear_Billd_Button_Click(object sender, RoutedEventArgs e)
        {
            Clear_DBill();
        }

        private void Clear_DBill()
        {
            //Billd_DateFromField.SelectedDate = null;
            //Billd_DateToField.SelectedDate = null;
            tbc.Text = "";
            tbn.Text = "";
            tbt.Text = "";
        }

    }
}
