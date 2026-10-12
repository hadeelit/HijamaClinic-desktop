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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HijamaClinicManagement
{
    /// <summary>
    /// Interaction logic for HomContentView.xaml
    /// </summary>
    public partial class HomContentView : UserControl
    {

        DBHandler.PatientAdapter patient_adapter = new DBHandler.PatientAdapter();
        DBHandler.AppointmentsAdapter appointment_adapter = new DBHandler.AppointmentsAdapter();
        DBHandler.UsersAdapter user_adapter = new DBHandler.UsersAdapter();

        public HomContentView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //set margin foe content body in Home Page
            statistics.Margin = new Thickness(10, 20, 0, 0);
            quickbuttons.Margin = new Thickness(10, 20, 0, 0);
            DGBorder.Margin = new Thickness(10, 20, 20, 40);
            datagridappointment.Margin = new Thickness(10, 10, 20, 40);

            //Load Data For Patients Panel
            GetAllpatients();
            //Load Data For Appointments Panel
            GetAllAppointments();
            //Load Data For Users Panel
            GetAllUsers();
        }

        private void NewPatient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            Clear_ContentHomeBody();
            PatienViews.PatientFileView patient = new PatienViews.PatientFileView();
            HomeWindow.mainbodycontent.Children.Add(patient);
            //HomeWindow.homeview.PatientsMg.IsSelected = true;
        }

        private void NewSession_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Visible;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            HomeWindow.mainbodycontent.Children.Add(physical);
           // HomeWindow.mainbodycontent.PatientsMg.IsSelected = true;
        }

        private void NewAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            AppointmentsView.AppointmentsMg appointment = new AppointmentsView.AppointmentsMg();
            HomeWindow.mainbodycontent.Children.Add(appointment);
            //HomeWindow.mainbodycontent.Appointmets.IsSelected = true;
        }

        private void Clear_ContentHomeBody()
        {
            //Clear UC 
            HomeWindow.mainbodycontent.Children.Clear();
            HomeWindow.mainbodycontent.RowDefinitions.Remove(row1);
            HomeWindow.mainbodycontent.RowDefinitions.Remove(row2);
            HomeWindow.mainbodycontent.RowDefinitions.Remove(row3);
        }


        //Load Data For Patients Panel
        private void GetAllpatients()
        {
            int count = patient_adapter.Count_Of_patients();
            CountPatients.Content = count.ToString();
        }

        //Load Data For Appointments Panel
        private void GetAllAppointments()
        {
            DateTime date = DateTime.Now.Date;
            int count_appointments = appointment_adapter.Count_Appointments(date.ToString()); ;
            CountAppointments.Content = count_appointments.ToString();
        }

        //Load Data For Users Panel
        private void GetAllUsers()
        {
            int count_users = user_adapter.Get_Count_Users();
            Count_Users.Content = count_users.ToString();
        }
    }
}
