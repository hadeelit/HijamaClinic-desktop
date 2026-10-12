using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;

namespace HijamaClinicManagement.AppointmentsView
{
    /// <summary>
    /// Interaction logic for AppointmentsMg.xaml
    /// </summary>
    public partial class AppointmentsMg : UserControl
    {
        DBHandler.AppointmentsAdapter appointment_adapter = new DBHandler.AppointmentsAdapter();

        public AppointmentsMg()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataViewFromAppointmentsTable.ItemsSource = null;
            Clear_Appointment();
            DefaultData();
        }

        //Load Default Data in fields when Load Window
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            AppointmentDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        //Load Data from Table to DATA Grid View
        private void LoadDataGridViw()
        {

            //SqlDataReader sdr = appointment_adapter.Get_AllAppointments(AppointmentDateField.Text);
            SqlDataReader sdr = appointment_adapter.Get_NewAppointment();
            DataTable dt = new DataTable();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
                DataViewFromAppointmentsTable.ItemsSource = dt.DefaultView;
                DBHandler.ConnectionDB.CloseConnection();
            }
            DBHandler.ConnectionDB.CloseConnection();
        }

        // When Clicked On Row Fetch Data
        private void DataViewFromAppointmentsTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell1 = DataViewFromAppointmentsTable.SelectedCells[1];
            var cell2 = DataViewFromAppointmentsTable.SelectedCells[2];
            var cell3 = DataViewFromAppointmentsTable.SelectedCells[3];
            var cell4 = DataViewFromAppointmentsTable.SelectedCells[4];
            var cell5 = DataViewFromAppointmentsTable.SelectedCells[5];

            string contact_n = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string patient_n = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string appoint_date = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string appoint_time = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
            string room_s = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;

            MobileNumberField.Text = contact_n;
            PatientNameField.Text = patient_n;
            AppointmentDateField.SelectedDate = DateTime.Parse(appoint_date);
            AppointmentTimeField.SelectedTime = DateTime.Parse(appoint_time);
            if (room_s != string.Empty)
                SpecialServiceField.IsChecked = true;

            DataTable dt = new DataTable();
            DataGridViewForBusyTimes.ItemsSource = dt.DefaultView;
            // DataViewFromAppointmentsTable.ItemsSource = null;
        }

        //Handler Functions For Add + Delete + Edit + Query
        private void AddButton_AppointmentsManagement_Click(object sender, RoutedEventArgs e)
        {
            if (Appointments_Isvalid())
            {
                string room_service = "";
                if (SpecialServiceField.IsChecked == true)
                    room_service = "Vip";
                DateTime time = DateTime.Parse(AppointmentTimeField.SelectedTime.ToString());
                bool r = appointment_adapter.Check_Time(AppointmentDateField.Text, AppointmentTimeField.SelectedTime.ToString());
                if (r == true)
                {
                    MessageBox.Show("فضلاً حدد وقت اخر الوقت مشغول في هذا التاريخ ! ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    AppointmentTimeField.SelectedTime = null;
                }
                else
                {
                    bool result = appointment_adapter.Insert_Appointment(MobileNumberField.Text, PatientNameField.Text, AppointmentDateField.Text, AppointmentTimeField.SelectedTime.ToString(), room_service);
                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Appointment();
                        AppointmentTimeField.Visibility = Visibility.Hidden;
                        LoadDataGridViw();
                        // Send Sms
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Appointment();
                        DefaultData();
                    }
                }

            }
        }

        private void DeleteButton_AppointmentsManagement_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromAppointmentsTable.SelectedItem != null)
            {
                if (MobileNumberField.Text == string.Empty || PatientNameField.Text == string.Empty || AppointmentDateField.SelectedDate == null)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        var cell0 = DataViewFromAppointmentsTable.SelectedCells[0];
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                        bool result = appointment_adapter.Delete_Appointment(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            DataViewFromAppointmentsTable.ItemsSource = null;
                            Clear_Appointment();
                            appointment_adapter.Edit_Identity();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }

                    }
                    else
                    {
                        MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_AppointmentsManagement_Click(object sender, RoutedEventArgs e)
        {
            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromAppointmentsTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (MobileNumberField.Text == string.Empty || PatientNameField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("يجب ادخال  حقلي اسم المريض و رقم الجوال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {

                    if (Appointments_Isvalid())
                    {
                        // Step 1 : Fetch Info For Specefic cell
                        var cell0 = DataViewFromAppointmentsTable.SelectedCells[0];
                        // Step 2: get the content for this info Cell (User_id Cell)
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;


                        string room_service = "";
                        if (SpecialServiceField.IsChecked == true)
                            room_service = "Vip";

                        // MessageBox For Yes No Updated
                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            DateTime time = DateTime.Parse(AppointmentTimeField.SelectedTime.ToString());
                            bool r = appointment_adapter.Check_Time(AppointmentDateField.Text, AppointmentTimeField.SelectedTime.ToString());
                            if (r == true)
                            {
                                MessageBox.Show("فضلاً حدد وقت اخر الوقت مشغول في هذا التاريخ ! ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                AppointmentTimeField.SelectedTime = null;
                            }
                            else
                            {
                                // Call Update_Appointment()
                                bool result = appointment_adapter.Update_Appointment(id, MobileNumberField.Text, PatientNameField.Text, AppointmentDateField.Text, AppointmentTimeField.SelectedTime.ToString(), room_service);
                                if (result)
                                {
                                    MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                    //LoadDataGridViw();
                                    Clear_Appointment();
                                    DataViewFromAppointmentsTable.ItemsSource = null;
                                    AppointmentTimeField.Visibility = Visibility.Hidden;


                                }
                                else
                                {
                                    MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                }
                            }

                        }
                        else
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void ClearButton_AppointmentsManagement_Click(object sender, RoutedEventArgs e)
        {
            Clear_Appointment();
        }

        //Load AppointmentsTimes that is Busy In Specefic Date
        private void DisplayTimesButton_AppointmentsManagement_Click(object sender, RoutedEventArgs e)
        {
            // if Appointmentdate is empety
            if (AppointmentDateField.Text == string.Empty)
                MessageBox.Show("فضلاً حدد التاريخ ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            else
            {
                SqlDataReader sdr = appointment_adapter.Get_AllAppointments_Times(AppointmentDateField.Text);
                DataTable dt = new DataTable();
                if (sdr.HasRows)
                {
                    dt.Load(sdr);
                    DataGridViewForBusyTimes.ItemsSource = dt.DefaultView;
                    AppointmentTimeField.Visibility = Visibility.Visible;
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد أوقات ممتلئة ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    AppointmentTimeField.Visibility = Visibility.Visible;
                    DBHandler.ConnectionDB.CloseConnection();
                }
            }

        }

        // Clear Previous Time And Busy times When Selected New Date
        private void AppointmentDateField_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AppointmentTimeField.SelectedTime = null;
            AppointmentTimeField.Visibility = Visibility.Hidden;
            DataGridViewForBusyTimes.ItemsSource = null;
        }

        //Validition For Search Input
        private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchInput.Text = "أدخل رقم الجوال فقط";

        }
        //SearchMAppointment Function
        private void SearchMyAppointment_Click(object sender, RoutedEventArgs e)
        {
            string S_Input = SearchInput.Text;
            SqlDataReader sdr = appointment_adapter.Get_AllAppointments_ContactNum(S_Input);
            if (sdr.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                DataViewFromAppointmentsTable.ItemsSource = null;
                DataViewFromAppointmentsTable.ItemsSource = dt.DefaultView;
                SearchInput.Clear();
                DBHandler.ConnectionDB.CloseConnection();
            }
            else
            {
                DBHandler.ConnectionDB.CloseConnection();
                MessageBox.Show("لايوجد سجل مطابق لرقم الجوال المدخل  !", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // MobileNumberField Only Numeric Input
        private void MobileNumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        //Function For Verify From Appointments View  Fields
        private bool Appointments_Isvalid()
        {
            if (MobileNumberField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MobileNumberField.Focus();
                return false;
            }
            if (PatientNameField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PatientNameField.Focus();
                return false;
            }
            if (AppointmentDateField.SelectedDate == null)
            {
                MessageBox.Show("حقل التاريخ اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                AppointmentDateField.Focus();
                return false;
            }
            if (AppointmentTimeField.SelectedTime == null)
            {
                MessageBox.Show("حقل الوقت اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                AppointmentTimeField.Focus();
                return false;
            }
            return true;
        }

        // Clear Values in Appointments View  Fields
        private void Clear_Appointment()
        {
            MobileNumberField.Clear();
            PatientNameField.Clear();
            AppointmentDateField.SelectedDate = null;
            AppointmentTimeField.SelectedTime = null;
            SpecialServiceField.IsChecked = false;
            //Clear DataViewBusyTimes
            DataGridViewForBusyTimes.ItemsSource = null;
        }

    }
}
