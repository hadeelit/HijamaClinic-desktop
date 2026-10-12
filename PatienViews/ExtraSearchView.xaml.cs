using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for ExtraSearchView.xaml
    /// </summary>
    public partial class ExtraSearchView : Window
    {

        DBHandler.PatientAdapter patient_adapter = new DBHandler.PatientAdapter();

        public ExtraSearchView()
        {
            InitializeComponent();
        }

        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CleareSearchPName();
            ClearSearchPNationalNum();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try 
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch
            { }   
        }

        //Search for patients records based on name 
        private void SearchPatientName_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPatientName.Text != string.Empty)
            {
                SqlDataReader sdr = patient_adapter.Get_Patients_Name(SearchPatientName.Text);
                if (sdr.HasRows)
                {
                    PatientFileView.Load_Search(sdr);
                    SearchPatientName.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    Close();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة اسم المريض", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchPatientName.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل اسم المريض  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // Only number input field function
        private void NumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }


        //Search for patients records based on national num
        private void SearchPNationalNum_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPNationalNum.Text != string.Empty)
            {
                SqlDataReader sdr = patient_adapter.Get_Patients_NationalNum(SearchPNationalNum.Text);
                if (sdr.HasRows)
                {
                    PatientFileView.Load_Search(sdr);
                    SearchPNationalNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    Close();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة الرقم الوطني", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchPNationalNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل الرقم الوطني  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

      
        private void ClearSearchPNationalNum_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchPNationalNum();
        }

        private void ClearSearchPatientName_Button_Click(object sender, RoutedEventArgs e)
        {
            CleareSearchPName();
        }

        //Clear Patiet name
        private void CleareSearchPName()
        {
            SearchPatientName.Clear();
        }
        // Clear National Num
        private void ClearSearchPNationalNum()
        {
            SearchPNationalNum.Clear();
        }

        
    }
}
