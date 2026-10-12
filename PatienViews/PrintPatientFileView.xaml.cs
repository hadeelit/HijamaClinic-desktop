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
    /// Interaction logic for PrintPatientFileView.xaml
    /// </summary>
    public partial class PrintPatientFileView : Window
    {
        public PrintPatientFileView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set all information to this View ...
            PatientFileNumField.Text = PatientFileView.file_num;
            PatientFileDateField.Text = PatientFileView.date;
            PatientNameField.Text = PatientFileView.name;
            PatientName2.Text = PatientFileView.name;
            MobileNumField.Text = PatientFileView.mobile;
            if (PatientFileView.national_num != string.Empty)
                NationalityNumField.Text = PatientFileView.national_num;
            if (PatientFileView.age != string.Empty)
                AgeField.Text = PatientFileView.age;
            if (PatientFileView.gender.Equals("أنثى"))
                Gender.Text = "أنثى";
            if (PatientFileView.gender.Equals("ذكر"))
                Gender.Text = "ذكر";
            if (PatientFileView.martial_status.Equals("أعزب"))
                MaritalStatus.Text = "أعزب";
            if (PatientFileView.martial_status.Equals("متزوج"))
                MaritalStatus.Text = "متزوج";
            if (PatientFileView.address != string.Empty)
                Address.Text = PatientFileView.address;
            Job.Text = PatientFileView.job;
            JobAddress.Text = PatientFileView.job_address;
            EmergencyContactNameField.Text = PatientFileView.e_name;
            EmergencyContactMobileField.Text = PatientFileView.e_contact;

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

        private void Print_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog dialog = new PrintDialog();
                if (dialog.ShowDialog() == true)
                {
                    dialog.PrintVisual(Print_File, "طباعة ملف المريض");
                }
                else
                {
                    MessageBox.Show("تم الغاء عملية الطباعة !", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            finally
            {
                this.IsEnabled = true;
                Close();
            }

        }

        private void CloseIcon_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Middle)
                    DragMove();
            }
            catch (Exception)
            { }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
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
