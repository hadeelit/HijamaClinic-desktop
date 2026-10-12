using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HijamaClinicManagement.AppointmentsView
{
    /// <summary>
    /// Interaction logic for AppointmentsView.xaml
    /// </summary>
    public partial class AppointmentsView : UserControl
    {
        DBHandler.AppointmentsAdapter appointment_adapter = new DBHandler.AppointmentsAdapter();
        public AppointmentsView()
        {
            InitializeComponent();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MNumberField.Clear();
            NumberOfAppointments.Clear();
            QueryDateField.SelectedDate = null;
            DataViewinAppointmentsDisplay.ItemsSource = null;
            DefaultData();
        }

        //Load Default Data in fields when Load Window
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            QueryDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        private void QueryAppointmentForMNumber_Click(object sender, RoutedEventArgs e)
        {
            // if empty field
            if (MNumberField.Text == string.Empty)
                MessageBox.Show("فضلاً أدخل رقم الجوال", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            else
            {
                SqlDataReader sdr = appointment_adapter.Get_AllAppointments_ContactNum(MNumberField.Text);
                DataTable dt = new DataTable();
                if (sdr.HasRows == true)
                {
                    dt.Load(sdr);
                    DataViewinAppointmentsDisplay.ItemsSource = dt.DefaultView;
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لا يوجد حجز مطابق للرقم المدخل", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DBHandler.ConnectionDB.CloseConnection();
                }
            }
        }

        private void QueryAppointmentForDate_Click(object sender, RoutedEventArgs e)
        {
            // if date  field empety
            if (QueryDateField.Text == string.Empty)
                MessageBox.Show("فضلاً حدد التاريخ ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            else
            {
                SqlDataReader sdr = appointment_adapter.Get_AllAppointments(QueryDateField.Text);
                DataTable dt = new DataTable();
                if (sdr.HasRows)
                {
                    dt.Load(sdr);
                    DataViewinAppointmentsDisplay.ItemsSource = dt.DefaultView;
                    NumberOfAppointments.Text = dt.Rows.Count.ToString();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لا يوجد حجوزات موافقة للتاريخ ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DBHandler.ConnectionDB.CloseConnection();
                }
            }

        }

        // MobileNumberField Only Numeric Input
        private void MobileNumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

    }
}
