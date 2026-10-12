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

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for PrintBillsMgView.xaml
    /// </summary>
    public partial class PrintBillsMgView : Window
    {
        public PrintBillsMgView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PatientName.Text = BillsMgView.p_name;
            FileNo.Text = BillsMgView.file_no;
            DateBill.Text = BillsMgView.bill_date;
            BillNo.Text = BillsMgView.bill_no;


            //percentage tax value
            if (BillsMgView.bill_tax != string.Empty)
                Vat_Value.Text = BillsMgView.bill_tax;
            else
                Vat_Value.Text = "0%";
            //SR tax value
            if (BillsMgView.bill_tax_SR != string.Empty)
                SRTaxBill.Text = BillsMgView.bill_tax_SR;
            else
                Vat_Value.Text = "0";

            //Discount
            if (BillsMgView.bill_discount != string.Empty)
                DiscountBill.Text = BillsMgView.bill_discount;
            else
                DiscountBill.Text = "0";


            if (BillsMgView.bill_paid_type.Equals("نقدي"))
            {
                Paid_Type.Text = "نقدي";
            }
            if (BillsMgView.bill_paid_type.Equals("شبكة"))
            {
                Paid_Type.Text = "شبكة";
            }
            if (BillsMgView.bill_paid_type.Equals("نقدي وشبكة"))
            {
                Paid_Type.Text = "نقدي وشبكة";
                //cash paid
                if (BillsMgView.cash_paid != string.Empty)
                {
                    cash_value_lable.Visibility = Visibility.Visible;
                    cash_value.Visibility = Visibility.Visible;
                    cash_value.Text = BillsMgView.cash_paid;
                }

                //network paid
                if (BillsMgView.network_paid != string.Empty)
                {
                    network_value_lable.Visibility = Visibility.Visible;
                    network_value.Visibility = Visibility.Visible;
                    network_value.Text = BillsMgView.network_paid;
                }
            }

            session_price.Text = BillsMgView.session_price;
            TotalTaxBill.Text = BillsMgView.totalpaid;
            PaidAmount.Text = BillsMgView.paid_amount;
            paid.Text = BillsMgView.paid_value_patient;
            Residu.Text = BillsMgView.remaining;
        }

        private void PrintBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog print = new PrintDialog();
                if (print.ShowDialog() == true)
                {
                    print.PrintVisual(Print, "فاتورة");
                }
                else
                {
                    MessageBox.Show("تم الغاء عملية الطباعة  ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void CloseIcon_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
                    DragMove();
            }
            catch (Exception)
            { }
        }
    }
}
