using SimpleInvoice;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

namespace HijamaClinicManagement
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {

        DBHandler.PatientAdapter patient_adapter = new DBHandler.PatientAdapter();
        DBHandler.AppointmentsAdapter appointment_adapter = new DBHandler.AppointmentsAdapter();
        DBHandler.UsersAdapter user_adapter = new DBHandler.UsersAdapter();

        // PatienViews.PatientsSessionView physical;  .......not used


        public static Grid mainbodycontent;
       

        public HomeWindow()
        {
            InitializeComponent();
        }

        // Exit From App
        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        // Maximize Window
        private void MinimizeIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximize Window
        private void MaximizeIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    var sizingParams = GetVirtualWindowSize();
                    WindowState = WindowState.Maximized;
                    MaxHeight = sizingParams.height;
                    MaxWidth = sizingParams.width;
                    navigationDrawer.DisplayMode = Syncfusion.UI.Xaml.NavigationDrawer.DisplayMode.Expanded;
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    navigationDrawer.DisplayMode = Syncfusion.UI.Xaml.NavigationDrawer.DisplayMode.Compact;
                    statistics.Margin = new Thickness(10, 20, 0, 0);
                    quickbuttons.Margin = new Thickness(10, 20, 0, 0);
                    DGBorder.Margin = new Thickness(10, 20, 20, 30);
                    datagridappointment.Margin = new Thickness(10, 10, 20, 30);
                    break;
                default:
                    break;
            }
        }

        // Method for Maximize Window
        private (double height, double width) GetVirtualWindowSize()
        {
            Window virtualWindow = new Window();
            virtualWindow.Show();
            virtualWindow.Opacity = 0;
            virtualWindow.WindowState = WindowState.Maximized;
            double returnHeight = virtualWindow.Height;
            double returnWidth = virtualWindow.Width;
            virtualWindow.Close();
            return (returnHeight, returnWidth);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            mainbodycontent = BodyContent;
            GetCurrentDateTime();
            SessionUser();
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

        //Get The Session User
        private void SessionUser()
        {
            UserNameLable.Content = Sign_in.User;
        }

        //Dragable Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }

            catch { }

        }

        //Get Date And Time For Main Window
        private void GetCurrentDateTime()
        {
            DateTime now = DateTime.Now;
            datenow.Content = now.Day + "/" + now.Month + "/" + now.Year;
            timenow.Content = now.TimeOfDay.Hours + ":" + now.TimeOfDay.Minutes + ":" + now.TimeOfDay.Seconds;

        }

        //Clinic-Settings Views
        //Move to ClinicInfo Window
        private void ClinicInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for Discount settings
            ClinicSettingsViews.ClinicInfo clinicinfo = new ClinicSettingsViews.ClinicInfo();
            BodyContent.Children.Add(clinicinfo);
        }

        //Move to Discounts-settings Window
        private void DiscountSetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for Discount settings
            ClinicSettingsViews.DiscountsSettings discount = new ClinicSettingsViews.DiscountsSettings();
            BodyContent.Children.Add(discount);
           
        }

        //Move to Tax-settings Window
        private void TaxSetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for Tax settings
            ClinicSettingsViews.TaxSettings tax = new ClinicSettingsViews.TaxSettings();
            BodyContent.Children.Add(tax);
        }

        //Users-Views
        //Set Permission For Admin and Others For Accounts Mg Window
        private void Permisiion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Sign_in.User.Equals("Admin"))
            {
                //Show accounts Mg Item For Amin Only
                AccountsMg.Visibility = Visibility.Visible;
                // For prevent Admin From Change His Own Username
                //UserNameField.IsReadOnly = true;
            }
            else
            {
                AccountsMg.Visibility = Visibility.Hidden;
               // UserNameField.IsReadOnly = false;
            }
        }

        //Move to Myaccount Window
        private void MyAccount_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for Myaccount
            UsersView.MyAccount account = new UsersView.MyAccount();          
            BodyContent.Children.Add(account);
        }

        //Move to MyNotes Window
        private void MyNotes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for MyNotes
            UsersView.MyNotes note = new UsersView.MyNotes();
            BodyContent.Children.Add(note);
        }

        //Move to AccountsMg Window
        private void AccountsMg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
            //Create instance for AccountsMg
            UsersView.AccountsManagement accounts = new UsersView.AccountsManagement();
            BodyContent.Children.Add(accounts);
        }

        // Move to SMS Widow
        private void Sms_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            //Create instance for SMS
            SMSView.SMS sms = new SMSView.SMS();
            BodyContent.Children.Add(sms);
        }

        private void Clear_ContentHomeBody()
        {
            BodyContent.Children.Clear();
            BodyContent.RowDefinitions.Remove(row1);
            BodyContent.RowDefinitions.Remove(row2);
            BodyContent.RowDefinitions.Remove(row3);
        }

        //Move to DashBoard Window
        private void DashBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            DashBoardView.DashBoardView dashboard = new DashBoardView.DashBoardView();
            BodyContent.Children.Add(dashboard);
        }

        //Move to Appointments View
        //Move to Appointments Management View Window
        private void AppointmentsMg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            AppointmentsView.AppointmentsMg appointment = new AppointmentsView.AppointmentsMg();
            BodyContent.Children.Add(appointment);
        }
        //Move to Appointments Management View Window
        private void AppointmentsView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            AppointmentsView.AppointmentsView view = new AppointmentsView.AppointmentsView();
            BodyContent.Children.Add(view);
        }

        //Move to ClinicManagement View
        //Move to Expenses Management View 
        private void ExpensesView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ClinicManagementViews.ExpensesView  expenses = new ClinicManagementViews.ExpensesView();
            BodyContent.Children.Add(expenses);
        }

        //Move to Send Email View 
        private void SendEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ClinicManagementViews.SendEmailView email = new ClinicManagementViews.SendEmailView();
            BodyContent.Children.Add(email);
        }

        //Move to BackUp View 
        private void BackUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ClinicManagementViews.BackupView backup = new ClinicManagementViews.BackupView();
            BodyContent.Children.Add(backup);
        }

        //Move to types Reports
        //Move to PatiensReports Window
        private void PatientsRe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ReportsViews.PatientsReport patient = new ReportsViews.PatientsReport();
            BodyContent.Children.Add(patient);
        }

        //Move to BillsReports Window
        private void BillsRe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ReportsViews.BillsReport bill = new ReportsViews.BillsReport();
            BodyContent.Children.Add(bill);
        }

        //Move to ExpensesReports Window
        private void ExpensesRe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ReportsViews.ExpensesReport expense = new ReportsViews.ExpensesReport();
            BodyContent.Children.Add(expense);
        }

        //Move to ZakatReport Window
        private void ZakatRe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ReportsViews.ZakatReport zakat = new ReportsViews.ZakatReport();
            BodyContent.Children.Add(zakat);
        }

        //Move to DailyIncome Window
        private void DailyIncom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            ReportsViews.DailyIncom incom = new ReportsViews.DailyIncom();
            BodyContent.Children.Add(incom);
        }

        //Move to PatientsViews Folder
        //Move to PatientsFiles Managements Window
        private void PatientsFiles_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientFileView patient = new PatienViews.PatientFileView();
            BodyContent.Children.Add(patient);
        }

        // Move To Bill Management window 
        private void Bills_Mg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.BillsMgView bill = new PatienViews.BillsMgView();
            BodyContent.Children.Add(bill);
        }

        //Set Permission For Admin and Others For Deleted Bills Mg Window 
        private void DeletedBillsPermisiion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Sign_in.User.Equals("Admin"))
            {
                //Show deleted bills Mg Item For Admin Only
                DBills_Mg.Visibility = Visibility.Visible;
            }
            else
            {
                DBills_Mg.Visibility = Visibility.Hidden;
            }
        }

        // Move To Delete Bill Management window 
        private void DBills_Mg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.DeletedBillsMgView dbill = new PatienViews.DeletedBillsMgView();
            BodyContent.Children.Add(dbill);
        }

        //Move to PatientsSessions Window
        // Move to Physical Examination View --- الفحص السريري
        private void PhysicalExamination_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            //physical = new PatienViews.PatientsSessionView();  .......not used
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Visible;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            BodyContent.Children.Add(physical);
        }

        // Move to Contraindications Of Cupping View ---  محاذير االحجامة 
        private void ContraindicationsOfCupping_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Hidden;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Visible;
            physical.Check_Contraindications_CuppingInfo();
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            BodyContent.Children.Add(physical);
        }

        // Move to Medical_History -- التاريخ المرضي
        private void Medical_History_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Hidden;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Visible;
            physical.Check_MedicalHistoryInfo();
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            BodyContent.Children.Add(physical);
        }

        // Move To PastMedical_History -- التاريخ المرضي السابق
        private void PastMedical_History_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Hidden;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Visible;
            physical.Check_PastMedicalHistoryInfo();
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            BodyContent.Children.Add(physical);
        }

        // Move To Disclaimer_Form -- نموذج الموافقة 
        private void Disclaimer_Form_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Hidden;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Visible;
            physical.Check_DisclaimerFormInfo();
            BodyContent.Children.Add(physical);
        }

        // Back To Hoem Window 
        private void BackHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            HomContentView contentview = new HomContentView();
            BodyContent.Children.Add(contentview);
            BackHome.IsSelected = true;
        }

        //Open PatientFile when clicked on New Patient 
        private void NewPatient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientFileView patient = new PatienViews.PatientFileView();
            BodyContent.Children.Add(patient);
            PatientsMg.IsSelected = true;
        }

        //Open PatientSeesion when clicked on New Session 
        private void NewSession_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            PatienViews.PatientsSessionView physical = new PatienViews.PatientsSessionView();
            physical.Physical_ExContent.Visibility = Visibility.Visible;
            physical.Contraindications_CuppingContent.Visibility = Visibility.Hidden;
            physical.Midical_HistoryContent.Visibility = Visibility.Hidden;
            physical.PastMidical_HistoryContent.Visibility = Visibility.Hidden;
            physical.Accept_FormContent.Visibility = Visibility.Hidden;
            BodyContent.Children.Add(physical);
            PatientsMg.IsSelected = true;
        }

        //Open Appointment when clicked on New Appointment 
        private void NewAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clear_ContentHomeBody();
            AppointmentsView.AppointmentsMg appointment = new AppointmentsView.AppointmentsMg();
            BodyContent.Children.Add(appointment);
            Appointmets.IsSelected = true;
        }


        //Exit from ScreensApp to Sign_in 
        private void ExitApp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            Sign_in sign_in = new Sign_in();
            sign_in.Show();
            Close();
        }

        //Functions For Help Item
        //View And Downloaded Documentation file from FastMedia Website (not used yet)
        private void Documentation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(CheckInternetConnection())
            {
                Process.Start("https://www.fastmedia.one/ar/application-of-cupping-clinic/");
            }
            else
            {
                MessageBox.Show("شبكة الإنترنت غير متوفرة!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //View Video  from FastMedia channel on youtube
        private void WatchingVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
           if(CheckInternetConnection())
           {
                Process.Start("https://www.youtube.com/channel/UCoDJRPwahDaV2ts1CiAQZqA");
           }
           else
           {
                MessageBox.Show("شبكة الإنترنت غير متوفرة!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
           }
        }

        //View contactus page from FastMedia Website
        private void Tech_Support_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckInternetConnection())
            {
                Process.Start("https://www.fastmedia.one/ar/contact/");
            }
            else
            {
                MessageBox.Show("شبكة الإنترنت غير متوفرة!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //View MessageBox for showing about app content 
        private void AboutApp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxRestyle.MessageBoxInformation2 message = new MessageBoxRestyle.MessageBoxInformation2();
            message.Show();
            // MessageBox.Show("يوفر تطبيق سطح المكتب الموجه لنظام Windows إدارة كاملة لعيادتك. ستجد كل ماتحتاجة لإدارة المرضى وزياراتهم و حسابات موظفينك و إدارة الفواتير وإدارة عيادتك الطبية بالكامل . أصدر ماتحتاجة من التقارير لمتابعة عملك واطلع على لوحة التحكم الخاصة بعملك", "تطبيق إدارة الحجامة", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        //View MessageBox for showing about Company FastMedia content 
        private void AboutCompany_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxRestyle.MessageBoxInformation message = new MessageBoxRestyle.MessageBoxInformation();
            message.Show();
            // MessageBox.Show("نحن مجموعة من المهووسين الرقميين و لدينا سنوات عديدة من الخبرة في الوسائط الرقمية. بخبرة طويلة و دافع كبير، نحن دائما قادرون على إتخاذ القرارات الصحيحة لعملائنا. نحن نضمن أن استثمارات عملائنا تحصل على عائد إيجابي يتماشى مع توقعاتنا المشتركة. نحن نتابع باستمرار مشاريع عملائنا للتأكد من وصولنا إلى الأهداف الموضوعة مسبقاً.", "فاست ميديا", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        //TestConnection with Internet
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
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
            int count_appointments = appointment_adapter.Count_Appointments(date.ToString());
            CountAppointments.Content = count_appointments.ToString();
        }

        //Load Data For Users Panel
        private void GetAllUsers()
        {
            int count_users = user_adapter.Get_Count_Users();
            Count_Users.Content = count_users.ToString();
        }

        private void Zatca_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Introfrm childForm = new Introfrm();
            childForm.Show();
        }
    }
}
