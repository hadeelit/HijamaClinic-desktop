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
using System.Text.RegularExpressions;
using System.Net.Http;
using Taqnyat;



namespace HijamaClinicManagement.SMSView
{
    /// <summary>
    /// Interaction logic for SMS.xaml
    /// </summary>
    public partial class SMS : UserControl
    {
        DBHandler.SmsAdapter Sms_Adapter = new DBHandler.SmsAdapter();

        public SMS()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_SMS();
        }

        //InputText Only With Numbers
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        //Function for send SMS to Specific Number
        private void Send_SMS_Click(object sender, RoutedEventArgs e)
        {
            if (SMS_Isvalid())
            {
                //Sms_Adapter.SendSms_Process(Number.Text, Message.Text);
                Sms_Adapter.Boadcast_Sms("test");
            }
        }

        // Ckeck Balance
        private void Check_Balance_Click(object sender, RoutedEventArgs e)
        {
            Sms_Adapter.Get_Balance();
        }

        // Clear SMS Fields
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_SMS();
        }
        // Check Validation SMS Fields
        private bool SMS_Isvalid()
        {

            if (Number.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Number.Focus();
                return false;
            }
            if (Number.Text.Length != 10)
            {
                MessageBox.Show("أدخل رقم جوال صحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Number.Focus();
                return false;
            }
            if (Message.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Message.Focus();
                return false;
            }
            return true;
        }

        // Clear SMS Fields
        private void Clear_SMS()
        {
            Number.Clear();
            Message.Clear();
        }
    }
}
