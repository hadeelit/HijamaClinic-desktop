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
using System.Windows.Shapes;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for PatientsSessionView.xaml
    /// </summary>
    public partial class PatientsSessionView : UserControl
    {
        private PainAreaWindow painArea;
        private TreatmentPlanWindow treatmentPlan;
        DBHandler.SessionPhysicalAdaptercs session_physical_adapter = new DBHandler.SessionPhysicalAdaptercs();
        DBHandler.PatientAdapter patient_adapter = new DBHandler.PatientAdapter();
        DBHandler.ContraindicationsOfCuppingAdapter contofcupping_adapter = new DBHandler.ContraindicationsOfCuppingAdapter();
        DBHandler.MedicalHistoryAdapter medicalh_adapter = new DBHandler.MedicalHistoryAdapter();
        DBHandler.PastMedicalHistoryAdapter pastmh_adapter = new DBHandler.PastMedicalHistoryAdapter();
        DBHandler.DisclaimerFormAdapter disclaimerf_adapter = new DBHandler.DisclaimerFormAdapter();
        DBHandler.PainAreasAdapter painarea_adapter = new DBHandler.PainAreasAdapter();
        DBHandler.TreatmentPlanAdapter treatmentplan_adapter = new DBHandler.TreatmentPlanAdapter();
        DBHandler.BillAdapter bill_adapter = new DBHandler.BillAdapter();
        DBHandler.DeletedBillsAdapter deleteb_adapter = new DBHandler.DeletedBillsAdapter();

        public static string s_id, s_num , FileNum, session_date, patient_name;
        private int bill_id;
        public static int delete_bill_admin = 0;

        public PatientsSessionView()
        {
            InitializeComponent();
            // configuration treatment plan window variable for moving between windows
            treatmentPlan = null;
            TreatmentPlanWindow.open = false;
            // configuration painarea window variable for moving between windows
            painArea = null;
            PainAreaWindow.open = false; 
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_InfoPatient();
            Clear_Session();
            DataViewFromPatientTable.ItemsSource = null;
            DataViewFromPhysicalExTable.ItemsSource = null;
            DefaultData();

        }

        //load all data to Views when loaded 
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            PatientSessionDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);

            // Default Data for PhysicalExamination
            TempField.Text = "37";
            BpField.Text = "120/80";
            HeadNeckField.Text = "Normal";
            ChestBackField.Text = "Normal";
            AbdomenField.Text = "Normal";
            PelvisField.Text = "Normal";
            UpperLowerLimbsField.Text = "Normal";
            OthersField.Text = "Normal";

            // Default Data for ContraindicationsOfCupping
            Diabetes_NotFound.IsChecked = true;
            RenalF_NotFound.IsChecked = true;
            Pregnancy_NotFound.IsChecked = true;
            Pacemaker_NotFound.IsChecked = true;
            VaricoseV_NotFound.IsChecked = true;
            BleedingD_NotFound.IsChecked = true;
            Aids_NotFound.IsChecked = true;
            LiverF_NotFound.IsChecked = true;
            ChildrenB12_NotFound.IsChecked = true;
            Cancer_NotFound.IsChecked = true;

            // Default data for MedicalHistory
            Duration.Text = "من 10 إلى 30 دقيقة";
            Character.Text = "متواصل";

            // Default Data For PastMH
            Adjust_PastMedicalH_Values();

            //Default data for DiscForm ...
        }

        //Load Data from Table Session and PhysicalEx  to DATA Grid View 
        private void LoadPatientGridViw(SqlDataReader sdr)
        {
            DataTable dt = new DataTable();
            dt.Load(sdr);
            DataViewFromPatientTable.ItemsSource = dt.DefaultView;
        }

        private void LoadSessionsGridViw(string file_num)
        {
            SqlDataReader sdr = session_physical_adapter.Get_Sessions(file_num);
            if (sdr.HasRows == false)
            {
                MessageBox.Show("لا يوجد جلسات بعد لهذا الملف !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DataViewFromPhysicalExTable.ItemsSource = null;
                DBHandler.ConnectionDB.CloseConnection();
                return;
            }
            else
            {
                DataTable dt_session = new DataTable("Session");
                dt_session.Columns.Add("SessionId");
                dt_session.Columns.Add("SessionNum");
                dt_session.Columns.Add("SessionDate", typeof(DateTime));
                dt_session.Columns.Add("CurrentCopmlaints");
                dt_session.Columns.Add("Complications");
                dt_session.Columns.Add("SessionDuration");
                dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                dt_session.Load(sdr);
                DBHandler.ConnectionDB.CloseConnection();

                SqlDataReader sdr_PhysicalExamination = session_physical_adapter.Get_PhysicalExamination(file_num);
                DataTable dt_PhysicalEx = new DataTable("PhysicalExamination");
                dt_session.Columns.Add("Temp");
                dt_session.Columns.Add("Hr");
                dt_session.Columns.Add("Rr");
                dt_session.Columns.Add("Bp");
                dt_session.Columns.Add("HeadAndNeck");
                dt_session.Columns.Add("ChestAndBack");
                dt_session.Columns.Add("Abdomen");
                dt_session.Columns.Add("Pelvis");
                dt_session.Columns.Add("UpperAndLowerLimbs");
                dt_session.Columns.Add("Others");
                dt_PhysicalEx.PrimaryKey = new DataColumn[] { dt_PhysicalEx.Columns["SessionId"] };

                dt_PhysicalEx.Load(sdr_PhysicalExamination);
                DBHandler.ConnectionDB.CloseConnection();
                // Merg Tow DataTable in one for this GridView
                dt_session.Merge(dt_PhysicalEx, false, MissingSchemaAction.Ignore);
                DataViewFromPhysicalExTable.ItemsSource = dt_session.DefaultView;
            }

        }

        private void DataViewFromPhysicalExTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell0 = DataViewFromPhysicalExTable.SelectedCells[0];
                var cell1 = DataViewFromPhysicalExTable.SelectedCells[1];
                var cell2 = DataViewFromPhysicalExTable.SelectedCells[2];
                var cell3 = DataViewFromPhysicalExTable.SelectedCells[3];
                var cell4 = DataViewFromPhysicalExTable.SelectedCells[4];
                var cell5 = DataViewFromPhysicalExTable.SelectedCells[5];
                var cell6 = DataViewFromPhysicalExTable.SelectedCells[6];
                var cell7 = DataViewFromPhysicalExTable.SelectedCells[7];
                var cell8 = DataViewFromPhysicalExTable.SelectedCells[8];
                var cell9 = DataViewFromPhysicalExTable.SelectedCells[9];
                var cell10 = DataViewFromPhysicalExTable.SelectedCells[10];
                var cell11 = DataViewFromPhysicalExTable.SelectedCells[11];
                var cell12 = DataViewFromPhysicalExTable.SelectedCells[12];
                var cell13 = DataViewFromPhysicalExTable.SelectedCells[13];
                var cell14 = DataViewFromPhysicalExTable.SelectedCells[14];
                var cell15 = DataViewFromPhysicalExTable.SelectedCells[15];

                s_id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                s_num = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
                string s_date = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
                string complaints = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
                string complications = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
                string s_duration = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
                string temp = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
                string hr = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
                string rr = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
                string bp = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
                string headneck = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
                string chestandback = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
                string abdomen = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
                string pelvis = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;
                string upperUpperandLowerlimbs = (cell14.Column.GetCellContent(cell14.Item) as TextBlock).Text;
                string others = (cell15.Column.GetCellContent(cell15.Item) as TextBlock).Text;

                Clear_Session();
                PatientSessionNumField.Text = s_num;
                PatientSessionDateField.Text = s_date;
                session_date = s_date;
                PatientComplaintsField.Text = complaints;
                ComplicationsField.Text = complications;
                PatientSessionPeriodField.Text = s_duration;
                TempField.Text = temp;
                HRField.Text = hr;
                RRField.Text = rr;
                BpField.Text = bp;
                HeadNeckField.Text = headneck;
                ChestBackField.Text = chestandback;
                AbdomenField.Text = abdomen;
                PelvisField.Text = pelvis;
                UpperLowerLimbsField.Text = upperUpperandLowerlimbs;
                OthersField.Text = others;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }


        }

        private void DataViewFromPatientTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell0 = DataViewFromPatientTable.SelectedCells[0];
                var cell2 = DataViewFromPatientTable.SelectedCells[2];

                string filenum = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                string pname = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;

                Clear_InfoPatient();
                // Info patient 
                PatientFileNumField.Text = filenum;
                PatientNameField.Text = pname;
                patient_name = pname;
                //Swich Between Tow Grid 
                DataViewFromPatientTable.Visibility = Visibility.Hidden;
                DataViewFromPatientTable.ItemsSource = null;
                DataViewFromPhysicalExTable.Visibility = Visibility.Visible;
                AddButton_PatientPhysical_Ex.IsEnabled = true;
                // Save FileNum For Other Windows
                FileNum = PatientFileNumField.Text;
                // Get All Sessions For THis File Num
                LoadSessionsGridViw(filenum);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }


        // Only number input field function
        private void NumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }


        // Function For Handler in PatientPhysical_Ex
        private void AddButton_PatientPhysical_Ex_Click(object sender, RoutedEventArgs e)
        {
            if (Session_IsValid())
            {
                // Get Session Num For Specefic filenum
                int s_num = session_physical_adapter.Get_SessionNum(PatientFileNumField.Text);
                bool result_session = session_physical_adapter.Insert_Session(s_num.ToString(), PatientSessionDateField.Text, PatientComplaintsField.Text, ComplicationsField.Text, PatientSessionPeriodField.Text, PatientFileNumField.Text);
                if (result_session)
                {
                    int s_id = session_physical_adapter.Get_LastSession();
                    bool result_PhysicalEx = session_physical_adapter.Insert_PhysicalExamination(TempField.Text, HRField.Text, RRField.Text, BpField.Text, HeadNeckField.Text, ChestBackField.Text, AbdomenField.Text, PelvisField.Text, UpperLowerLimbsField.Text, OthersField.Text, PatientFileNumField.Text, s_id.ToString());

                    if (result_PhysicalEx)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Session();
                        DefaultData();
                        LoadSessionsGridViw(PatientFileNumField.Text);
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Session();
                        DefaultData();
                    }
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Session();
                    DefaultData();
                }
            }

        }

        private void DeleteButton_PatientPhysical_Ex_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromPhysicalExTable.SelectedItem != null)
            {
                if (PatientSessionNumField.Text == string.Empty || PatientSessionDateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سجلا من جدول الجلسات بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    // Get  current file num
                    string filenum = PatientFileNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف الجلسة التابعة لملف المريض رقم  " + " " + ":" + filenum, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        // Get Session Id
                        var cell = DataViewFromPhysicalExTable.SelectedCells[0];
                        s_id = (cell.Column.GetCellContent(cell.Item) as TextBlock).Text;

                        //Get Bill id
                        bill_id = bill_adapter.Get_BillExtract_id(s_id);
                        //Get All Data Bill Before Deleted if Found it
                        SqlDataReader sdr_bill = bill_adapter.Get_Bill(bill_id.ToString());
                        if (bill_id == 0)
                        {
                            //  MessageBox.Show("bill" + bill_id);
                            DBHandler.ConnectionDB.CloseConnection();
                        }
                        else
                        {
                            // DBHandler.ConnectionDB.CloseConnection();
                            //Insert DeleteBillRecord to DeleteBillsRecord Table
                            bool r4 = Get_dataBill(bill_id.ToString(), filenum, sdr_bill);
                            // MessageBox.Show("call DEletebill");
                            if (r4)
                            {
                                delete_bill_admin = 1;
                            }
                            else
                            {
                                delete_bill_admin = 0;
                            }
                        }

                        // Delete current session
                        bool result = session_physical_adapter.Delete_Session(s_id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            //Swich Between Tow Grid 
                            DataViewFromPatientTable.Visibility = Visibility.Visible;
                            DataViewFromPhysicalExTable.Visibility = Visibility.Hidden;
                            DataViewFromPhysicalExTable.ItemsSource = null;
                            DataViewFromPatientTable.ItemsSource = null;
                            // Clear Info Patient  + Session + MH + PMH
                            Clear_InfoPatient();
                            Clear_Session();
                            Clear_Contraindications();
                            Clear_MedicalHistory();
                            Clear_PastMedicalHistory();
                            Clear_DisclaimerForm();
                            // PainArea.Clear_PainArea();
                            DefaultData();
                            //Adjust identity for all tables which are associated with session table
                            // Edit_Identity for all Tables
                            session_physical_adapter.Edit_Identity_Sessions();
                            session_physical_adapter.Edit_Identity_PhysicalExamination();
                            contofcupping_adapter.Edit_Identity_ContraindicationsOfCupping();
                            medicalh_adapter.Edit_Identity_MedicalHistory();
                            pastmh_adapter.Edit_Identity_PastMedicalHistory();
                            disclaimerf_adapter.Edit_Identity_DisclaimerForm();
                            bool r1 = Delete_ImgPainArea(s_id);
                            if (r1)
                            {
                                painarea_adapter.Edit_Identity_PainAreas();
                            }
                            bool r2 = Delete_ImgTreatmentPlan(s_id);
                            if (r2)
                            {
                                treatmentplan_adapter.Edit_Identity_TreatmentPlan();
                            }
                            // delete associated bill witth this session and then Adjust identity
                            bill_adapter.Edit_Identity_Bill();

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
                MessageBox.Show("حدد سجلا من جدول الجلسات بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // Function For Delete Image  PainAreaImg from PainAreaImages Folder when Delete  Specefic painArea Record and session
        private bool Delete_ImgPainArea(string s_id)
        {
            SqlDataReader sdr = painarea_adapter.Get_ImgPath(s_id);
            // Result of Query If Contains one record or not
            if (sdr.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                DBHandler.ConnectionDB.CloseConnection();
                // Get PainAreaId From dt
                string painarea_id = dt.Rows[0].Field<int>(0).ToString();
                // step 1 : Delete this session id from PainAreas Table
                bool result = painarea_adapter.Delete_PainArea(painarea_id);
                if (result)
                {
                    // Get PainAreaImage Path if found From dt
                    string path = dt.Rows[0].Field<string>(1);
                    if (path != string.Empty)
                    {
                        // Delete PainAreaImg from PainAreaImages Folder when Delete  Specefic painArea Record
                        PainAreaWindow.Delete_PainAreaImg(path);
                        return true;
                    }
                    return true;
                }
                else
                {
                    // ex. Disconnect with DB
                    MessageBox.Show("فشلت عملية حذف سجل أماكن الألم ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    return false;
                }
            }
            DBHandler.ConnectionDB.CloseConnection();
            return false;
        }

        // Function For Delete Image  TreatmentPlanImg from ImagesTreatmentPlan Folder when Delete  Specefic TreatmentPlan Record and session
        private bool Delete_ImgTreatmentPlan(string s_id)
        {
            SqlDataReader sdr = treatmentplan_adapter.Get_ImgPath(s_id);
            // Result of Query If Contains one record or not
            if (sdr.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                DBHandler.ConnectionDB.CloseConnection();
                // Get treatmentpId From dt
                string treatmentp_id = dt.Rows[0].Field<int>(0).ToString();
                // step 1 : Delete this session id from TreatmentPlan Table
                bool result = treatmentplan_adapter.Delete_TreatmentPlan(treatmentp_id);
                if (result)
                {
                    // Get treatmentpImage Path if found From dt
                    string path = dt.Rows[0].Field<string>(1);
                    if (path != null)
                    {

                        // Delete treatmentpImg from treatmentPlanImages Folder when Delete  Specefic treatmentPlan Record
                        TreatmentPlanWindow.Delete_TreatmentPlanImg(path);
                        return true;
                    }
                    return true;
                }
                else
                {
                    // ex. Disconnect with DB
                    MessageBox.Show("فشلت عملية حذف سجل خطة العلاج لهذه الجلسة ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    return false;
                }
            }
            DBHandler.ConnectionDB.CloseConnection();
            return false;
        }

        // Function for delete bill that is associated with specefic session (not used)
        /* private bool Delete_bill_session(string session_id)
         {
             bill_id = bill_adapter.Get_BillExtract_id(session_id);
             //No Bill for this sessionid yet
             if (bill_id == 0)
             {
                 MessageBox.Show("1");
                 return false;

             }
             else
             {
                 bool result_bill = bill_adapter.Delete_BillExtract(bill_id.ToString());
                 if(result_bill)
                 {
                     MessageBox.Show("2");
                     return true;
                 }
                 else
                 {
                     MessageBox.Show("3");
                     MessageBox.Show("فشلت عملية حذف سجل الفاتورة المرتبطه مع هذه الجلسة ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                     return false;
                 }
             }
         }
        */


        //Get all data from bill and then isert to DeleteBillsRecord Table
        private bool Get_dataBill(string bill_id, string filenum, SqlDataReader sdr_bill)
        {
            //SqlDataReader sdr_bill = bill_adapter.Get_Bill(bill_id);
            if (sdr_bill.HasRows == false)
            {
                DBHandler.ConnectionDB.CloseConnection();
                return false;
            }
            else
            {

                // Intialize Varaiables 
                float price = 0, tax = 0 ,discount = 0, total_p = 0, remaining = 0;
                DataTable dt = new DataTable();
                dt.Load(sdr_bill);
                // Close connection for bill_adapter.Get_Bill(bill_id) Query
                DBHandler.ConnectionDB.CloseConnection();

                // Column 1
                string id = dt.Rows[0].Field<int>(0).ToString();
                // Column 2
                string bill_date = dt.Rows[0].Field<DateTime>(1).ToString();
                // Column 3
                int chip_type = dt.Rows[0].Field<int>(2);
                chip_type = 0;
                if (chip_type.Equals("50"))
                    chip_type = 50;
                if (chip_type.Equals("100"))
                    chip_type = 100;
                if (chip_type.Equals("150"))
                    chip_type = 150;
                if (chip_type.Equals("200"))
                    chip_type = 200;
                if (chip_type.Equals("250"))
                    chip_type = 250;
                if (chip_type.Equals("300"))
                    chip_type = 300;
                if (chip_type.Equals("350"))
                    chip_type = 350;
                // Column 4
                string add_service = "";
                if (!dt.Rows[0].IsNull("AdditionalServices"))
                {
                    add_service = dt.Rows[0].Field<string>(3);
                }
                // Column 5
                string r_price = "";
                if (!dt.Rows[0].IsNull("PriceOfAdditionalServices"))
                {
                    price = Convert.ToSingle(dt.Rows[0].Field<double>(4));
                    r_price = price.ToString();
                }
                // Column 6
                string nationality_type = dt.Rows[0].Field<string>(5);
                if (nationality_type.Equals("سعودي"))
                    nationality_type = "سعودي";
                if (nationality_type.Equals("غير سعودي"))
                    nationality_type = "غير سعودي";
                // Column 7
                string r_tax = "";
                if (!dt.Rows[0].IsNull("Tax"))
                {
                    tax = Convert.ToSingle(dt.Rows[0].Field<double>(6));
                    r_tax = tax.ToString();
                }
                // Column 8
                string r_discount = "";
                if (!dt.Rows[0].IsNull("Discount"))
                {
                    discount = Convert.ToSingle(dt.Rows[0].Field<double>(7));
                    r_discount = discount.ToString();
                }
                // Column 9
                string payment_type = dt.Rows[0].Field<string>(8);
                if (payment_type.Equals("نقدي"))
                    payment_type = "نقدي";
                if (payment_type.Equals("شبكة "))
                    payment_type = "شبكة";
                if (payment_type.Equals("نقدي وشبكة "))
                    payment_type = "نقدي وشبكة ";
                // Column 10
                string r_totalp = "";
                if (!dt.Rows[0].IsNull("TotalPricePaid"))
                {
                    total_p = Convert.ToSingle(dt.Rows[0].Field<double>(9));
                    r_totalp = total_p.ToString();
                }
                // Column 11
                string r_remaining = "";
                if (!dt.Rows[0].IsNull("AmountRemainingUnpaid"))
                {
                    remaining = Convert.ToSingle(dt.Rows[0].Field<double>(10));
                    r_remaining = remaining.ToString();
                }
                // Column 12
                string entry_name = dt.Rows[0].Field<string>(11);
                DateTime now = DateTime.Now;
                string deletedb_date = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year).ToString();
                // Insert this data To DeleteBillsRecords Table
                bool result_deletb = deleteb_adapter.Insert_DeletedBillsRecords(filenum, id, bill_date, chip_type, add_service, r_price, nationality_type, r_tax, r_discount, payment_type, r_totalp, r_remaining, entry_name, deletedb_date);
                if (result_deletb)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    return true;
                }
                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    MessageBox.Show("فشلت عملية الاضافة الى الفواتير المحذوفة");
                    return false;
                }
            }
        }

        private void EdieButton_PatientPhysical_Ex_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromPhysicalExTable.SelectedItem != null)
            {
                if (PatientSessionNumField.Text == string.Empty || PatientSessionDateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سجلا من جدول الجلسات بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (Session_IsValid())
                    {
                        // Get  current file num
                        string filenum = PatientFileNumField.Text;

                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الجلسة التابعة لملف المريض رقم  " + " " + ":" + filenum, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            // Get Session Id
                            var cell = DataViewFromPhysicalExTable.SelectedCells[0];
                            s_id = (cell.Column.GetCellContent(cell.Item) as TextBlock).Text;
                            // update current session
                            bool result1 = session_physical_adapter.Update_Session(s_id, PatientSessionDateField.Text, PatientComplaintsField.Text, ComplicationsField.Text, PatientSessionPeriodField.Text);
                            bool result2 = session_physical_adapter.Update_PhysicalExamination(s_id, TempField.Text, HRField.Text, RRField.Text, BpField.Text, HeadNeckField.Text, ChestBackField.Text, AbdomenField.Text, PelvisField.Text, UpperLowerLimbsField.Text, OthersField.Text);
                            if (result1 && result2)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                //Swich Between Tow Grid 
                                DataViewFromPatientTable.Visibility = Visibility.Visible;
                                DataViewFromPhysicalExTable.Visibility = Visibility.Hidden;
                                DataViewFromPhysicalExTable.ItemsSource = null;
                                DataViewFromPatientTable.ItemsSource = null;
                                // Clear Info Patient  + Session
                                Clear_InfoPatient();
                                Clear_Session();
                                DefaultData();
                            }
                            else
                            {
                                MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            }
                        }
                        else
                        {
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من جدول الجلسات بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void ClearButton_PatientPhysical_Ex_Click(object sender, RoutedEventArgs e)
        {
            Clear_Session();
            DefaultData();
        }

        //Restrict Input Data in TextBoxes (Discount - Tax - TotalPaidPrice - Price - RemainingUnPaid -Hr - TEmp - RR... etc) 
        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //Recognize Digites + dot character and  Comma character from others Characters (Decimal Input)
            Char ch = e.Text[0];
            String TextBoxValue = ((System.Windows.Controls.TextBox)e.Source).Text.ToString();
            if (Char.IsDigit(ch) || ch == '.' || ch == ',')
            {
                if ((ch == '.' && TextBoxValue.Contains('.')))
                {
                    System.Windows.MessageBox.Show("ادخال غير صحيح", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
                if (ch == ',')
                {
                    System.Windows.MessageBox.Show("لايسمح بالفاصلة ", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
            }
            else
            {

                System.Windows.MessageBox.Show("يسمح بادخال الأرقام", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                e.Handled = true;
            }
        }

        private void PainArea_Button_Click(object sender, RoutedEventArgs e)
        {
            if (PatientSessionNumField.Text != string.Empty)
            {
                if (painArea == null && PainAreaWindow.open == false)
                {
                    painArea = new PainAreaWindow();
                    painArea.Show();
                }
                if (painArea != null && PainAreaWindow.open == true)
                {
                    painArea = new PainAreaWindow();
                    painArea.Show();
                    PainAreaWindow.open = false;

                }
                if (painArea != null && PainAreaWindow.open == false)
                {
                    _ = painArea.Activate();
                    _ = painArea.Focus();
                }
            }
            else
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى نافذة أماكن الألم !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void TreatmentPlan_Button_Click(object sender, RoutedEventArgs e)
        {
            if (PatientSessionNumField.Text != string.Empty)
            {
                if (treatmentPlan == null && TreatmentPlanWindow.open == false)
                {
                    treatmentPlan = new TreatmentPlanWindow();
                    treatmentPlan.Show();
                }
                if (treatmentPlan != null && TreatmentPlanWindow.open == true)
                {
                    treatmentPlan = new TreatmentPlanWindow();
                    treatmentPlan.Show();
                    TreatmentPlanWindow.open = false;
                }
                if (treatmentPlan != null && TreatmentPlanWindow.open == false)
                {
                    _ = treatmentPlan.Activate();
                    _ = treatmentPlan.Focus();
                }

            }
            else
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى نافذة خطة العلاج !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void PatientFileNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPatientFileNum.Text = "أدخل رقم الملف";
        }

        private void PatientMobileNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchMobileNum.Text = "أدخل رقم الجوال";
        }

        private void SearchMobileNum_Button_Click(object sender, RoutedEventArgs e)
        {
            //Swich Between Tow Grid 
            DataViewFromPatientTable.Visibility = Visibility.Visible;
            DataViewFromPhysicalExTable.Visibility = Visibility.Hidden;
            DataViewFromPhysicalExTable.ItemsSource = null;
            DataViewFromPatientTable.ItemsSource = null;
            // Clear Info Patient  + Session
            Clear_InfoPatient();
            Clear_Session();

            // IsEnabled Add _ Button
            AddButton_PatientPhysical_Ex.IsEnabled = false;
            // IsEnabled AddContraindication _ Button 
            AddButton_Contraindications_Cupping.IsEnabled = false;
            // IsEnabled MedicalHistory _ Button 
            AddButton_Medical_History.IsEnabled = false;
            // IsEnabled PMedicalHistory _ Button 
            AddButton_PMH.IsEnabled = false;
            // IsEnabled DiscForm _ Button 
            AddButton_DiscForm.IsEnabled = false;
            // IsEnabled PainArea _ Button 
            //painArea.AddButton_PainArea.IsEnabled = false;
            // Clear previous data for GV Contraindications
            DataGv_Contraindications_Cupping.ItemsSource = null;
            // Clear previous data for GV MedicalH
            DataGv_MedicalH.ItemsSource = null;
            // Clear previous data for GV MedicalH
            DataGv_MedicalH.ItemsSource = null;
            // Clear previous data for GV PMedicalH
            DataGv_PMedicalH.ItemsSource = null;
            // Clear previous data for GV DiscForm
            DataGv_DiscForm.ItemsSource = null;
            //Clear Previous data for Contraindications
            Clear_Contraindications();
            Clear_MedicalHistory();
            Clear_PastMedicalHistory();
            Clear_DisclaimerForm();
            //painArea.Clear_PainArea();
            DefaultData();

            if (SearchMobileNum.Text != string.Empty && SearchMobileNum.Text != "أدخل رقم الجوال")
            {
                SqlDataReader sdr = patient_adapter.Get_Patient_Contactnum(SearchMobileNum.Text);
                if (sdr.HasRows)
                {
                    LoadPatientGridViw(sdr);
                    SearchMobileNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة رقم الجوال", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchMobileNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل رقم الجوال  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void SearchPatientFileNum_Button_Click(object sender, RoutedEventArgs e)
        {

            //Swich Between Tow Grid 
            DataViewFromPatientTable.Visibility = Visibility.Visible;
            DataViewFromPhysicalExTable.Visibility = Visibility.Hidden;
            DataViewFromPhysicalExTable.ItemsSource = null;
            DataViewFromPatientTable.ItemsSource = null;
            // Clear Info Patient  + Session
            Clear_InfoPatient();
            Clear_Session();

            // IsEnabled Add _ Button
            AddButton_PatientPhysical_Ex.IsEnabled = false;
            // IsEnabled AddContraindication _ Button 
            AddButton_Contraindications_Cupping.IsEnabled = false;
            // IsEnabled MedicalHistory _ Button 
            AddButton_Medical_History.IsEnabled = false;
            // IsEnabled PMedicalHistory _ Button 
            AddButton_PMH.IsEnabled = false;
            // IsEnabled DiscForm _ Button 
            AddButton_DiscForm.IsEnabled = false;
            // IsEnabled PainArea _ Button 
            // PainArea.AddButton_PainArea.IsEnabled = false;
            // Clear previous data for GV Contraindications
            DataGv_Contraindications_Cupping.ItemsSource = null;
            // Clear previous data for GV MedicalH
            DataGv_MedicalH.ItemsSource = null;
            // Clear previous data for GV MedicalH
            DataGv_MedicalH.ItemsSource = null;
            // Clear previous data for GV PMedicalH
            DataGv_PMedicalH.ItemsSource = null;
            // Clear previous data for GV DiscForm
            DataGv_DiscForm.ItemsSource = null;
            //Clear Previous data for Contraindications
            Clear_Contraindications();
            Clear_MedicalHistory();
            Clear_PastMedicalHistory();
            Clear_DisclaimerForm();
            // PainArea.Clear_PainArea();
            DefaultData();

            if (SearchPatientFileNum.Text != string.Empty && SearchPatientFileNum.Text != "أدخل رقم الملف")
            {
                SqlDataReader sdr = patient_adapter.Get_Patient_Filenum(SearchPatientFileNum.Text);
                if (sdr.HasRows)
                {
                    LoadPatientGridViw(sdr);
                    SearchPatientFileNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة رقم الملف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchPatientFileNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل رقم الملف  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }



        // Functions For Contraindications_Cupping View ......
        public void Check_Contraindications_CuppingInfo()
        {
            // PatientSessionNumField.Text == string.Empty   ...... not used
            // Check if Session num  in seesion view detected or not for move to another window
            if (string.IsNullOrEmpty(s_num))
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                AddButton_Contraindications_Cupping.IsEnabled = true;
                //PFileNumField.Text = PatientFileNumField.Text;
                PFileNumField.Text = FileNum; ;
                PSessionNumField.Text = s_num;
                // Load Data Contraindicaions of cupping  For this session and file num
                LoadContraindicationsOfCuppingGridViw(PFileNumField.Text, s_id);

            }
        }

        private void LoadContraindicationsOfCuppingGridViw(string file_num, string session_id)
        {
            // Get  current file num & session num from current window
            string filenum = PFileNumField.Text;
            string session_num = PSessionNumField.Text;

            SqlDataReader sdr_ContraindicationsOfCupping = contofcupping_adapter.Get_ContraindicationsOfCupping(file_num, s_id);
            if (sdr_ContraindicationsOfCupping.HasRows == false)
            {
                /* not used
                // Allow only one add for current session with Contraindications
                MessageBox.Show("لا يوجد سجل محاذير حجامة بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DBHandler.ConnectionDB.CloseConnection();
                DataGv_Contraindications_Cupping.ItemsSource = null;
                // IsEnabled AddContraindication _ Button  For  Add and  delete  and edit  to this session 
                AddButton_Contraindications_Cupping.IsEnabled = true;
                // Assigning null to clear all field values when re-entering the interface.
                s_num = null;
                return;*/

                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("لا يوجد سجل محاذير حجامة بعد لهذه الجلسة التابعة للمريض" + " " + ":" + filenum +
                " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد إضافة سجل جديد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_Contraindications_Cupping.ItemsSource = null;
                    // IsEnabled AddContraindication _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_Contraindications_Cupping.IsEnabled = true;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }

                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_Contraindications_Cupping.ItemsSource = null;
                    // IsEnabled AddContraindication _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_Contraindications_Cupping.IsEnabled = false;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }
            }

            else
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show(" يوجد سجل محاذير حجامة لهذه الجلسة التابعة للمريض" + " " + ":" + filenum +
                " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد تحديث السجل السابق أو حذفه", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_ContraindicationsOfCupping = contofcupping_adapter.Get_ContraindicationsOfCupping(file_num, s_id);
                    DataTable dt_ContraindicationsOfCupping = new DataTable();
                    dt_session.Columns.Add("ContraindicationId");
                    dt_session.Columns.Add("Diabetes");
                    dt_session.Columns.Add("RenalFailure");
                    dt_session.Columns.Add("Pregnancy");
                    dt_session.Columns.Add("Pacemaker");
                    dt_session.Columns.Add("VaricoseVeins");
                    dt_session.Columns.Add("BleedingDisordersAndAnticoagulants");
                    dt_session.Columns.Add("Aids");
                    dt_session.Columns.Add("LiverFailureOrHepAtitis");
                    dt_session.Columns.Add("ChildrenBelow12Years");
                    dt_session.Columns.Add("ChemoOrCancer");
                    dt_ContraindicationsOfCupping.PrimaryKey = new DataColumn[] { dt_ContraindicationsOfCupping.Columns["SessionId"] };

                    dt_ContraindicationsOfCupping.Load(sdr_ContraindicationsOfCupping);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_ContraindicationsOfCupping, false, MissingSchemaAction.Ignore);
                    DataGv_Contraindications_Cupping.ItemsSource = dt_session.DefaultView;

                    // IsEnabled AddContraindication _ Button  For  only delete  and edit  to this session 
                    AddButton_Contraindications_Cupping.IsEnabled = false;

                    // Fill all fields with values in Contraindications of cupping table
                    Adjust_Contraindication_Values();

                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                }

                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_ContraindicationsOfCupping = contofcupping_adapter.Get_ContraindicationsOfCupping(file_num, s_id);
                    DataTable dt_ContraindicationsOfCupping = new DataTable();
                    dt_session.Columns.Add("ContraindicationId");
                    dt_session.Columns.Add("Diabetes");
                    dt_session.Columns.Add("RenalFailure");
                    dt_session.Columns.Add("Pregnancy");
                    dt_session.Columns.Add("Pacemaker");
                    dt_session.Columns.Add("VaricoseVeins");
                    dt_session.Columns.Add("BleedingDisordersAndAnticoagulants");
                    dt_session.Columns.Add("Aids");
                    dt_session.Columns.Add("LiverFailureOrHepAtitis");
                    dt_session.Columns.Add("ChildrenBelow12Years");
                    dt_session.Columns.Add("ChemoOrCancer");
                    dt_ContraindicationsOfCupping.PrimaryKey = new DataColumn[] { dt_ContraindicationsOfCupping.Columns["SessionId"] };

                    dt_ContraindicationsOfCupping.Load(sdr_ContraindicationsOfCupping);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_ContraindicationsOfCupping, false, MissingSchemaAction.Ignore);
                    DataGv_Contraindications_Cupping.ItemsSource = dt_session.DefaultView;

                    // IsEnabled false for AddContraindication _ Button  + delete  and edit  to this session 
                    AddButton_Contraindications_Cupping.IsEnabled = false;
                    DeleteButton_Contraindications_Cupping.IsEnabled = false;
                    EdieButton_Contraindications_Cupping.IsEnabled = false;
                    // Fill all fields with values in Contraindications of cupping table
                    Adjust_Contraindication_Values();
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                }
            }

        }

        // All Radio buttons not checked before clicked on record in DGV
        private void Adjust_Contraindication_Values()
        {
            // All Buttons NotFound 
            Diabetes_NotFound.IsChecked = true;
            RenalF_NotFound.IsChecked = true;
            Pregnancy_NotFound.IsChecked = true;
            Pacemaker_NotFound.IsChecked = true;
            VaricoseV_NotFound.IsChecked = true;
            BleedingD_NotFound.IsChecked = true;
            Aids_NotFound.IsChecked = true;
            LiverF_NotFound.IsChecked = true;
            ChildrenB12_NotFound.IsChecked = true;
            Cancer_NotFound.IsChecked = true;
            // All Buttons Found
            Diabetes_Found.IsChecked = false;
            RenalF_Found.IsChecked = false;
            Pregnancy_Found.IsChecked = false;
            Pacemaker_Found.IsChecked = false;
            VaricoseV_Found.IsChecked = false;
            BleedingD_Found.IsChecked = false;
            Aids_Found.IsChecked = false;
            LiverF_Found.IsChecked = false;
            ChildrenB12_Found.IsChecked = false;
            Cancer_Found.IsChecked = false;

        }

        private void DataGv_Contraindications_Cupping_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell3 = DataGv_Contraindications_Cupping.SelectedCells[3];
                var cell4 = DataGv_Contraindications_Cupping.SelectedCells[4];
                var cell5 = DataGv_Contraindications_Cupping.SelectedCells[5];
                var cell6 = DataGv_Contraindications_Cupping.SelectedCells[6];
                var cell7 = DataGv_Contraindications_Cupping.SelectedCells[7];
                var cell8 = DataGv_Contraindications_Cupping.SelectedCells[8];
                var cell9 = DataGv_Contraindications_Cupping.SelectedCells[9];
                var cell10 = DataGv_Contraindications_Cupping.SelectedCells[10];
                var cell11 = DataGv_Contraindications_Cupping.SelectedCells[11];
                var cell12 = DataGv_Contraindications_Cupping.SelectedCells[12];
                var cell13 = DataGv_Contraindications_Cupping.SelectedCells[13];

                string contraindicationid = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
                string diabetes = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
                string RenalFailure = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
                string Pregnancy = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
                string Pacemaker = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
                string VaricoseVeins = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
                string BleedingDisordersAndAnticoagulants = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
                string Aids = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
                string LiverFailureOrHepAtitis = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
                string ChildrenBelow12Years = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
                string ChemoRadiotherapyOrCancer = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;

                PContraindicationNumField.Text = contraindicationid;

                if (diabetes.Equals("يوجد"))
                    Diabetes_Found.IsChecked = true;
                else
                    Diabetes_NotFound.IsChecked = true;

                if (RenalFailure.Equals("يوجد"))
                    RenalF_Found.IsChecked = true;
                else
                    RenalF_NotFound.IsChecked = true;

                if (Pregnancy.Equals("يوجد"))
                    Pregnancy_Found.IsChecked = true;
                else
                    Pregnancy_NotFound.IsChecked = true;

                if (Pacemaker.Equals("يوجد"))
                    Pacemaker_Found.IsChecked = true;
                else
                    Pacemaker_NotFound.IsChecked = true;

                if (VaricoseVeins.Equals("يوجد"))
                    VaricoseV_Found.IsChecked = true;
                else
                    VaricoseV_NotFound.IsChecked = true;

                if (BleedingDisordersAndAnticoagulants.Equals("يوجد"))
                    BleedingD_Found.IsChecked = true;
                else
                    BleedingD_NotFound.IsChecked = true;

                if (Aids.Equals("يوجد"))
                    Aids_Found.IsChecked = true;
                else
                    Aids_NotFound.IsChecked = true;

                if (LiverFailureOrHepAtitis.Equals("يوجد"))
                    LiverF_Found.IsChecked = true;
                else
                    LiverF_NotFound.IsChecked = true;

                if (ChildrenBelow12Years.Equals("يوجد"))
                    ChildrenB12_Found.IsChecked = true;
                else
                    ChildrenB12_NotFound.IsChecked = true;

                if (ChemoRadiotherapyOrCancer.Equals("يوجد"))
                    Cancer_Found.IsChecked = true;
                else
                    Cancer_NotFound.IsChecked = true;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        private void AddButton_Contraindications_Cupping_Click(object sender, RoutedEventArgs e)
        {
            if (PFileNumField.Text != string.Empty && PSessionNumField.Text != string.Empty)
            {
                string Diabetes, RenalFailure, BleedingDisordersAndAnticoagulants, Aids, LiverFailureOrHepAtitis, Pregnancy, ChildrenBelow12Years, ChemoRadiotherapyOrCancer, Pacemaker, VaricoseVeins;

                if (Diabetes_Found.IsChecked == true)
                    Diabetes = "يوجد";
                else
                    Diabetes = "لا يوجد";
                if (RenalF_Found.IsChecked == true)
                    RenalFailure = "يوجد";
                else
                    RenalFailure = "لا يوجد";
                if (BleedingD_Found.IsChecked == true)
                    BleedingDisordersAndAnticoagulants = "يوجد";
                else
                    BleedingDisordersAndAnticoagulants = "لا يوجد";
                if (Aids_Found.IsChecked == true)
                    Aids = "يوجد";
                else
                    Aids = "لا يوجد";
                if (LiverF_Found.IsChecked == true)
                    LiverFailureOrHepAtitis = "يوجد";
                else
                    LiverFailureOrHepAtitis = "لا يوجد";
                if (Pregnancy_Found.IsChecked == true)
                    Pregnancy = "يوجد";
                else
                    Pregnancy = "لا يوجد";
                if (ChildrenB12_Found.IsChecked == true)
                    ChildrenBelow12Years = "يوجد";
                else
                    ChildrenBelow12Years = "لا يوجد";
                if (Cancer_Found.IsChecked == true)
                    ChemoRadiotherapyOrCancer = "يوجد";
                else
                    ChemoRadiotherapyOrCancer = "لا يوجد";
                if (Pacemaker_Found.IsChecked == true)
                    Pacemaker = "يوجد";
                else
                    Pacemaker = "لا يوجد";

                if (VaricoseV_Found.IsChecked == true)
                    VaricoseVeins = "يوجد";
                else
                    VaricoseVeins = "لا يوجد";


                bool result = contofcupping_adapter.Insert_ContraindicationsOfCupping(Diabetes, RenalFailure, BleedingDisordersAndAnticoagulants, Aids, LiverFailureOrHepAtitis, Pregnancy, ChildrenBelow12Years, ChemoRadiotherapyOrCancer, Pacemaker, VaricoseVeins, PFileNumField.Text, s_id);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    LoadContraindicationsOfCuppingGridViw(PFileNumField.Text, s_id);
                    AddButton_Contraindications_Cupping.IsEnabled = false;
                    //Clear_Contraindications();
                    //DefaultData();
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Contraindications();
                    DefaultData();

                }
            }
            else
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void DeleteButton_Contraindications_Cupping_Click(object sender, RoutedEventArgs e)
        {
            if (DataGv_Contraindications_Cupping.SelectedItem != null)
            {
                if (PContraindicationNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = PFileNumField.Text;
                    string session_num = PSessionNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = contofcupping_adapter.Delete_ContraindicationsOfCupping(PContraindicationNumField.Text);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadContraindicationsOfCuppingGridViw(filenum, s_id);
                            PContraindicationNumField.Clear();
                            DefaultData();
                            contofcupping_adapter.Edit_Identity_ContraindicationsOfCupping();
                           // Clear_Contraindications();
                           // DefaultData();
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
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_Contraindications_Cupping_Click(object sender, RoutedEventArgs e)
        {

            if (DataGv_Contraindications_Cupping.SelectedItem != null)
            {
                if (PContraindicationNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = PFileNumField.Text;
                    string session_num = PSessionNumField.Text;

                    string Diabetes, RenalFailure, BleedingDisordersAndAnticoagulants, Aids, LiverFailureOrHepAtitis, Pregnancy, ChildrenBelow12Years, ChemoRadiotherapyOrCancer, Pacemaker, VaricoseVeins;

                    if (Diabetes_Found.IsChecked == true)
                        Diabetes = "يوجد";
                    else
                        Diabetes = "لا يوجد";
                    if (RenalF_Found.IsChecked == true)
                        RenalFailure = "يوجد";
                    else
                        RenalFailure = "لا يوجد";
                    if (BleedingD_Found.IsChecked == true)
                        BleedingDisordersAndAnticoagulants = "يوجد";
                    else
                        BleedingDisordersAndAnticoagulants = "لا يوجد";
                    if (Aids_Found.IsChecked == true)
                        Aids = "يوجد";
                    else
                        Aids = "لا يوجد";
                    if (LiverF_Found.IsChecked == true)
                        LiverFailureOrHepAtitis = "يوجد";
                    else
                        LiverFailureOrHepAtitis = "لا يوجد";
                    if (Pregnancy_Found.IsChecked == true)
                        Pregnancy = "يوجد";
                    else
                        Pregnancy = "لا يوجد";
                    if (ChildrenB12_Found.IsChecked == true)
                        ChildrenBelow12Years = "يوجد";
                    else
                        ChildrenBelow12Years = "لا يوجد";
                    if (Cancer_Found.IsChecked == true)
                        ChemoRadiotherapyOrCancer = "يوجد";
                    else
                        ChemoRadiotherapyOrCancer = "لا يوجد";
                    if (Pacemaker_Found.IsChecked == true)
                        Pacemaker = "يوجد";
                    else
                        Pacemaker = "لا يوجد";

                    if (VaricoseV_Found.IsChecked == true)
                        VaricoseVeins = "يوجد";
                    else
                        VaricoseVeins = "لا يوجد";

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = contofcupping_adapter.Update_ContraindicationsOfCupping(PContraindicationNumField.Text, Diabetes, RenalFailure, BleedingDisordersAndAnticoagulants, Aids, LiverFailureOrHepAtitis, Pregnancy, ChildrenBelow12Years, ChemoRadiotherapyOrCancer, Pacemaker, VaricoseVeins);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadContraindicationsOfCuppingGridViw(filenum, s_id);
                            PContraindicationNumField.Clear();
                            //Clear_Contraindications();
                            //DefaultData();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }


        //Functions For Medical History View ....
        public void Check_MedicalHistoryInfo()
        {
            // if (PatientSessionNumField.Text == string.Empty) ....... not used  
            // Check if Session num detected or not for move to another window
            if (string.IsNullOrEmpty(s_num))
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                AddButton_Medical_History.IsEnabled = true;
                //MHFileNumField.Text = PatientFileNumField.Text; .... not used
                MHFileNumField.Text = FileNum;
                MHSessionNumField.Text = s_num;
                // Load Data Medical History table  For this session and file num
                LoadMedicalHistoryGridViw(MHFileNumField.Text, s_id);

            }
        }

        private void LoadMedicalHistoryGridViw(string file_num, string session_id)
        {
            // Get  current file num from current window
            string filenum = MHFileNumField.Text;
            string session_num = MHSessionNumField.Text;

            SqlDataReader sdr_MedicalHistory = medicalh_adapter.Get_MedicalHistory(file_num, s_id);
            if (sdr_MedicalHistory.HasRows == false)
            {
                /* not used
                // Allow only one add for current session with Medical History
                MessageBox.Show("لا يوجد سجل تاريخ مرضي بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DBHandler.ConnectionDB.CloseConnection();
                DataGv_MedicalH.ItemsSource = null;
                // IsEnabled AddMedical History _ Button  For  Add and  delete  and edit  to this session 
                AddButton_Medical_History.IsEnabled = true;
                // Assigning null to clear all field values when re-entering the interface.
                s_num = null;
                return;*/

                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("لا يوجد سجل تاريخ مرضي بعد لهذه الجلسة التابعة للمريض" + " " + ":" + filenum + 
                " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد إضافة سجل جديد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_MedicalH.ItemsSource = null;
                    // IsEnabled AddMedical History _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_Medical_History.IsEnabled = true;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }
                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_MedicalH.ItemsSource = null;
                    // IsEnabled AddMedical History _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_Medical_History.IsEnabled = false;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }
            }
            else
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show(" يوجد سجل تاريخ مرضي لهذه الجلسة التابعة للمريض" + " " + ":" + filenum +
                " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد تحديث السجل السابق أو حذفه", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    // Close Connection for Get_MedicalHistory Methode
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_MedicalHistory = medicalh_adapter.Get_MedicalHistory(file_num, s_id);
                    DataTable dt_MedicalHistory = new DataTable();
                    dt_session.Columns.Add("MedicalHistoryId");
                    dt_session.Columns.Add("MainSymptoms");
                    dt_session.Columns.Add("OnsetOfPain");
                    dt_session.Columns.Add("LocationOfPain");
                    dt_session.Columns.Add("DurationOfPain");
                    dt_session.Columns.Add("CharacterOfPain");
                    dt_session.Columns.Add("AggravatingAndRelievingFactorsOnPain");
                    dt_session.Columns.Add("Timing");
                    dt_session.Columns.Add("Severity");
                    dt_session.Columns.Add("Allergies");
                    dt_session.Columns.Add("Medications");
                    dt_session.Columns.Add("PastSurgicalOrCuppingHistory");
                    dt_session.Columns.Add("AnotherComplaints");
                    dt_session.Columns.Add("ConsultationWithDoctor");
                    dt_MedicalHistory.PrimaryKey = new DataColumn[] { dt_MedicalHistory.Columns["SessionId"] };

                    dt_MedicalHistory.Load(sdr_MedicalHistory);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_MedicalHistory, false, MissingSchemaAction.Ignore);
                    DataGv_MedicalH.ItemsSource = dt_session.DefaultView;

                    // IsEnabled AddMedicalH _ Button  For  only delete  and edit  to this session 
                    AddButton_Medical_History.IsEnabled = false;

                    // Fill all fields with values in MedicalH  table
                    Adjust_MedicalH_Values();

                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                }

                else
                {
                    // Close Connection for Get_MedicalHistory Methode
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_MedicalHistory = medicalh_adapter.Get_MedicalHistory(file_num, s_id);
                    DataTable dt_MedicalHistory = new DataTable();
                    dt_session.Columns.Add("MedicalHistoryId");
                    dt_session.Columns.Add("MainSymptoms");
                    dt_session.Columns.Add("OnsetOfPain");
                    dt_session.Columns.Add("LocationOfPain");
                    dt_session.Columns.Add("DurationOfPain");
                    dt_session.Columns.Add("CharacterOfPain");
                    dt_session.Columns.Add("AggravatingAndRelievingFactorsOnPain");
                    dt_session.Columns.Add("Timing");
                    dt_session.Columns.Add("Severity");
                    dt_session.Columns.Add("Allergies");
                    dt_session.Columns.Add("Medications");
                    dt_session.Columns.Add("PastSurgicalOrCuppingHistory");
                    dt_session.Columns.Add("AnotherComplaints");
                    dt_session.Columns.Add("ConsultationWithDoctor");
                    dt_MedicalHistory.PrimaryKey = new DataColumn[] { dt_MedicalHistory.Columns["SessionId"] };

                    dt_MedicalHistory.Load(sdr_MedicalHistory);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_MedicalHistory, false, MissingSchemaAction.Ignore);
                    DataGv_MedicalH.ItemsSource = dt_session.DefaultView;

                    // IsEnabled false for Add Button + delete  + edit  to this session 
                    AddButton_Medical_History.IsEnabled = false;
                    DeleteButton_Medical_History.IsEnabled = false;
                    EdieButton_Medical_History.IsEnabled = false;

                    // Fill all fields with values in MedicalH  table
                    Adjust_MedicalH_Values();

                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;

                }


            }
        }

        private void Adjust_MedicalH_Values()
        {

            CurrentSymptom.Clear();
            Onset.Clear();
            Location.Clear();
            Duration.Clear();
            Character.Clear();
            AggravatingFactors.Clear();
            Timing.Clear();
            Severity.Clear();
            Allergies.Clear();
            Medications.Clear();
            PastSurgicalOrCuppingHistory.Clear();
            OthersComplaints.Clear();
            CounsultAdoctor.Clear();
        }

        private void DataGv_MedicalH_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell3 = DataGv_MedicalH.SelectedCells[3];
                var cell4 = DataGv_MedicalH.SelectedCells[4];
                var cell5 = DataGv_MedicalH.SelectedCells[5];
                var cell6 = DataGv_MedicalH.SelectedCells[6];
                var cell7 = DataGv_MedicalH.SelectedCells[7];
                var cell8 = DataGv_MedicalH.SelectedCells[8];
                var cell9 = DataGv_MedicalH.SelectedCells[9];
                var cell10 = DataGv_MedicalH.SelectedCells[10];
                var cell11 = DataGv_MedicalH.SelectedCells[11];
                var cell12 = DataGv_MedicalH.SelectedCells[12];
                var cell13 = DataGv_MedicalH.SelectedCells[13];
                var cell14 = DataGv_MedicalH.SelectedCells[14];
                var cell15 = DataGv_MedicalH.SelectedCells[15];
                var cell16 = DataGv_MedicalH.SelectedCells[16];

                string mh_id = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
                string main_sy = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
                string onset = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
                string location = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
                string duration = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
                string character = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
                string aggravatingfactors = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
                string timing = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
                string severity = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
                string allergies = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
                string medications = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;
                string pastsergury = (cell14.Column.GetCellContent(cell14.Item) as TextBlock).Text;
                string otherscomplaints = (cell15.Column.GetCellContent(cell15.Item) as TextBlock).Text;
                string counsultdoctor = (cell16.Column.GetCellContent(cell16.Item) as TextBlock).Text;

                Adjust_MedicalH_Values();
                MHNumField.Text = mh_id;
                CurrentSymptom.Text = main_sy;
                Onset.Text = onset;
                Location.Text = location;
                Duration.Text = duration;
                Character.Text = character;
                AggravatingFactors.Text = aggravatingfactors;
                Timing.Text = timing;
                Severity.Text = severity;
                Allergies.Text = allergies;
                Medications.Text = medications;
                PastSurgicalOrCuppingHistory.Text = pastsergury;
                OthersComplaints.Text = otherscomplaints;
                CounsultAdoctor.Text = counsultdoctor;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void AddButton_Medical_History_Click(object sender, RoutedEventArgs e)
        {
            if (MHFileNumField.Text != string.Empty && MHSessionNumField.Text != string.Empty)
            {
                bool result = medicalh_adapter.Insert_MedicalHistory(CurrentSymptom.Text, Onset.Text, Location.Text, Duration.Text, Character.Text, AggravatingFactors.Text, Timing.Text, Severity.Text, Allergies.Text, Medications.Text, PastSurgicalOrCuppingHistory.Text, OthersComplaints.Text, CounsultAdoctor.Text, MHFileNumField.Text, s_id);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    LoadMedicalHistoryGridViw(MHFileNumField.Text, s_id);
                    AddButton_Medical_History.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DefaultData();
                }
            }
            else
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void DeleteButton_Medical_History_Click(object sender, RoutedEventArgs e)
        {
            if (DataGv_MedicalH.SelectedItem != null)
            {
                if (MHNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = MHFileNumField.Text;
                    string session_num = MHSessionNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = medicalh_adapter.Delete_MedicalHistory(MHNumField.Text);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadMedicalHistoryGridViw(filenum, s_id);
                            MHNumField.Clear();
                            Adjust_MedicalH_Values();
                            DefaultData();
                            medicalh_adapter.Edit_Identity_MedicalHistory();
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
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_Medical_History_Click(object sender, RoutedEventArgs e)
        {
            if (DataGv_MedicalH.SelectedItem != null)
            {
                if (MHNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = MHFileNumField.Text;
                    string session_num = MHSessionNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = medicalh_adapter.Update_MedicalHistory(MHNumField.Text, CurrentSymptom.Text, Onset.Text, Location.Text, Duration.Text, Character.Text, AggravatingFactors.Text, Timing.Text, Severity.Text, Allergies.Text, Medications.Text, PastSurgicalOrCuppingHistory.Text, OthersComplaints.Text, CounsultAdoctor.Text);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadMedicalHistoryGridViw(filenum, s_id);
                            MHNumField.Clear();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }



        // Functions For PastMedicalHistory  View ......
        public void Check_PastMedicalHistoryInfo()
        {
            // PatientSessionNumField.Text == string.Empty ..... not used
            // Check if Session num detected or not for move to another window
            if (string.IsNullOrEmpty(s_num))
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                AddButton_PMH.IsEnabled = true;
                // PMHFileNumField.Text = PatientFileNumField.Text;  .......not used 
                PMHFileNumField.Text = FileNum;
                PMHSNumField.Text = s_num;
                // Load Data PastMedicalHistory table  For this session and file num
                LoadPastMedicalHistoryGridViw(PMHFileNumField.Text, s_id);

            }
        }

        private void LoadPastMedicalHistoryGridViw(string file_num, string session_id)
        {
            // Get  current file num
            string filenum = PMHFileNumField.Text;
            string session_num = PMHSNumField.Text;

            SqlDataReader sdr_PastMedicalHistory = pastmh_adapter.Get_PastMedicalHistory(file_num, s_id);
            if (sdr_PastMedicalHistory.HasRows == false)
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("لا يوجد سجل تاريخ مرضي سابق بعد لهذه الجلسة التابعة للمريض" + " " + ":" + filenum +
               " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد إضافة سجل جديد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_PMedicalH.ItemsSource = null;
                    // IsEnabled AddPastMedical History _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_PMH.IsEnabled = true;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }

                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    DataGv_PMedicalH.ItemsSource = null;
                    // IsEnabled AddPastMedical History _ Button  For  Add and  delete  and edit  to this session 
                    AddButton_PMH.IsEnabled = false;
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                    return;
                }

                /*
                // Allow only one add for current session with PastMedicalHistory
                MessageBox.Show("لا يوجد سجل تاريخ مرضي سابق  بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DBHandler.ConnectionDB.CloseConnection();
                DataGv_PMedicalH.ItemsSource = null;
                // IsEnabled AddPastMedical History _ Button  For  Add and  delete  and edit  to this session 
                AddButton_PMH.IsEnabled = true;
                // Assigning null to clear all field values when re-entering the interface.
                s_num = null;
                return;*/
            }
            else
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show(" يوجد سجل تاريخ مرضي سابق لهذه الجلسة التابعة للمريض" + " " + ":" + filenum +
                " " + "والجلسة رقم " + ":" + session_num + " " + "هل تريد تحديث السجل السابق أو حذفه", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    // Close Connection for Get_PastMedicalHistory Methode
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_PastMedicalHistory = pastmh_adapter.Get_PastMedicalHistory(file_num, s_id);
                    DataTable dt_PastMedicalHistory = new DataTable();
                    dt_session.Columns.Add("PastMedicalHistoryId");
                    dt_session.Columns.Add("BleedingDisorders");
                    dt_session.Columns.Add("Hypertension");
                    dt_session.Columns.Add("CardiacDiseases");
                    dt_session.Columns.Add("Diabetes");
                    dt_session.Columns.Add("HepAtitis");
                    dt_session.Columns.Add("Aids");
                    dt_session.Columns.Add("Cva");
                    dt_session.Columns.Add("Tb");
                    dt_session.Columns.Add("Epilepsy");
                    dt_session.Columns.Add("KidneyDiseases");
                    dt_session.Columns.Add("PsychiatricDiseases");
                    dt_session.Columns.Add("Cancer");
                    dt_session.Columns.Add("Anemia");
                    dt_session.Columns.Add("Smoking");
                    dt_session.Columns.Add("Others");
                    dt_PastMedicalHistory.PrimaryKey = new DataColumn[] { dt_PastMedicalHistory.Columns["SessionId"] };

                    dt_PastMedicalHistory.Load(sdr_PastMedicalHistory);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_PastMedicalHistory, false, MissingSchemaAction.Ignore);
                    DataGv_PMedicalH.ItemsSource = dt_session.DefaultView;

                    // IsEnabled AddPastMedicalH _ Button  For  only delete  and edit  to this session 
                    AddButton_PMH.IsEnabled = false;
                    // Fill all fields with values in PastMedicalH  table
                    Adjust_PastMedicalH_Values();
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                }

                else
                {
                    // Close Connection for Get_PastMedicalHistory Methode
                    DBHandler.ConnectionDB.CloseConnection();
                    SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                    DataTable dt_session = new DataTable();
                    dt_session.Columns.Add("SessionId");
                    dt_session.Columns.Add("SessionDate", typeof(DateTime));
                    dt_session.Columns.Add("SessionNum");
                    dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();

                    sdr_PastMedicalHistory = pastmh_adapter.Get_PastMedicalHistory(file_num, s_id);
                    DataTable dt_PastMedicalHistory = new DataTable();
                    dt_session.Columns.Add("PastMedicalHistoryId");
                    dt_session.Columns.Add("BleedingDisorders");
                    dt_session.Columns.Add("Hypertension");
                    dt_session.Columns.Add("CardiacDiseases");
                    dt_session.Columns.Add("Diabetes");
                    dt_session.Columns.Add("HepAtitis");
                    dt_session.Columns.Add("Aids");
                    dt_session.Columns.Add("Cva");
                    dt_session.Columns.Add("Tb");
                    dt_session.Columns.Add("Epilepsy");
                    dt_session.Columns.Add("KidneyDiseases");
                    dt_session.Columns.Add("PsychiatricDiseases");
                    dt_session.Columns.Add("Cancer");
                    dt_session.Columns.Add("Anemia");
                    dt_session.Columns.Add("Smoking");
                    dt_session.Columns.Add("Others");
                    dt_PastMedicalHistory.PrimaryKey = new DataColumn[] { dt_PastMedicalHistory.Columns["SessionId"] };

                    dt_PastMedicalHistory.Load(sdr_PastMedicalHistory);
                    DBHandler.ConnectionDB.CloseConnection();
                    // Merg Tow DataTable in one for this GridView
                    dt_session.Merge(dt_PastMedicalHistory, false, MissingSchemaAction.Ignore);
                    DataGv_PMedicalH.ItemsSource = dt_session.DefaultView;

                    // IsEnabled false for AddPastMedicalH _ Button  & delete  and edit  to this session 
                    AddButton_PMH.IsEnabled = false;
                    DeleteButton_PMH.IsEnabled = false;
                    EdieButton_PMH.IsEnabled = false;
                    // Fill all fields with values in PastMedicalH  table
                    Adjust_PastMedicalH_Values();
                    // Assigning null to clear all field values when re-entering the interface.
                    s_num = null;
                }   
            }
        }

        private void Adjust_PastMedicalH_Values()
        {
            BleedingD_You.IsChecked = false;
            BleedingD_Family.IsChecked = false;
            HyperTentio_You.IsChecked = false;
            HyperTentio_Family.IsChecked = false;
            CardiacD_You.IsChecked = false;
            CardiacD_Family.IsChecked = false;
            Diabetes_You.IsChecked = false;
            Diabetes_Family.IsChecked = false;
            Hepatitis_You.IsChecked = false;
            Hepatitis_Family.IsChecked = false;
            Aids_You.IsChecked = false;
            Aids_Family.IsChecked = false;
            Cva_You.IsChecked = false;
            Cva_Family.IsChecked = false;
            Tb_You.IsChecked = false;
            Tb_Family.IsChecked = false;
            Epilepsy_You.IsChecked = false;
            Epilepsy_Family.IsChecked = false;
            Kidney_You.IsChecked = false;
            Kidney_Family.IsChecked = false;
            PsychiatricD_Family.IsChecked = false;
            PsychiatricD_You.IsChecked = false;
            Cancer_Family.IsChecked = false;
            Cancer_You.IsChecked = false;
            Anemia_Family.IsChecked = false;
            Anemia_You.IsChecked = false;
            Smoking_You.IsChecked = false;
            Others_You.IsChecked = false;

        }

        private void DataGv_PMedicalH_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell3 = DataGv_PMedicalH.SelectedCells[3];
                var cell4 = DataGv_PMedicalH.SelectedCells[4];
                var cell5 = DataGv_PMedicalH.SelectedCells[5];
                var cell6 = DataGv_PMedicalH.SelectedCells[6];
                var cell7 = DataGv_PMedicalH.SelectedCells[7];
                var cell8 = DataGv_PMedicalH.SelectedCells[8];
                var cell9 = DataGv_PMedicalH.SelectedCells[9];
                var cell10 = DataGv_PMedicalH.SelectedCells[10];
                var cell11 = DataGv_PMedicalH.SelectedCells[11];
                var cell12 = DataGv_PMedicalH.SelectedCells[12];
                var cell13 = DataGv_PMedicalH.SelectedCells[13];
                var cell14 = DataGv_PMedicalH.SelectedCells[14];
                var cell15 = DataGv_PMedicalH.SelectedCells[15];
                var cell16 = DataGv_PMedicalH.SelectedCells[16];
                var cell17 = DataGv_PMedicalH.SelectedCells[17];
                var cell18 = DataGv_PMedicalH.SelectedCells[18];

                string pmh_id = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
                string bleeding_dis = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
                string hypertention = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
                string cardiac_dis = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
                string diabetes = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
                string hepatitis = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
                string aids = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
                string cva = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
                string tb = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
                string epilepsy = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
                string kidney_dis = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;
                string psy_dis = (cell14.Column.GetCellContent(cell14.Item) as TextBlock).Text;
                string cancer = (cell15.Column.GetCellContent(cell15.Item) as TextBlock).Text;
                string anemia = (cell16.Column.GetCellContent(cell16.Item) as TextBlock).Text;
                string smoking = (cell17.Column.GetCellContent(cell17.Item) as TextBlock).Text;
                string other = (cell18.Column.GetCellContent(cell18.Item) as TextBlock).Text;

                Adjust_PastMedicalH_Values();
                PMHNumField.Text = pmh_id;

                if (bleeding_dis.Equals("أنت"))
                    BleedingD_You.IsChecked = true;
                if (bleeding_dis.Equals("العائلة"))
                    BleedingD_Family.IsChecked = true;
                if (bleeding_dis.Equals("أنت والعائلة"))
                {
                    BleedingD_You.IsChecked = true;
                    BleedingD_Family.IsChecked = true;
                }
                if (hypertention.Equals("أنت"))
                    HyperTentio_You.IsChecked = true;
                if (hypertention.Equals("العائلة"))
                    HyperTentio_Family.IsChecked = true;
                if (hypertention.Equals("أنت والعائلة"))
                {
                    HyperTentio_You.IsChecked = true;
                    HyperTentio_Family.IsChecked = true;
                }
                if (cardiac_dis.Equals("أنت"))
                    CardiacD_You.IsChecked = true;
                if (cardiac_dis.Equals("العائلة"))
                    CardiacD_Family.IsChecked = true;
                if (cardiac_dis.Equals("أنت والعائلة"))
                {
                    CardiacD_You.IsChecked = true;
                    CardiacD_Family.IsChecked = true;
                }
                if (diabetes.Equals("أنت"))
                    Diabetes_You.IsChecked = true;
                if (diabetes.Equals("العائلة"))
                    Diabetes_Family.IsChecked = true;
                if (diabetes.Equals("أنت والعائلة"))
                {
                    Diabetes_You.IsChecked = true;
                    Diabetes_Family.IsChecked = true;
                }
                if (hepatitis.Equals("أنت"))
                    Hepatitis_You.IsChecked = true;
                if (hepatitis.Equals("العائلة"))
                    Hepatitis_Family.IsChecked = true;
                if (hepatitis.Equals("أنت والعائلة"))
                {
                    Hepatitis_You.IsChecked = true;
                    Hepatitis_Family.IsChecked = true;
                }
                if (aids.Equals("أنت"))
                    Aids_You.IsChecked = true;
                if (aids.Equals("العائلة"))
                    Aids_Family.IsChecked = true;
                if (aids.Equals("أنت والعائلة"))
                {
                    Aids_You.IsChecked = true;
                    Aids_Family.IsChecked = true;
                }
                if (cva.Equals("أنت"))
                    Cva_You.IsChecked = true;
                if (cva.Equals("العائلة"))
                    Cva_Family.IsChecked = true;
                if (cva.Equals("أنت والعائلة"))
                {
                    Cva_You.IsChecked = true;
                    Cva_Family.IsChecked = true;
                }
                if (tb.Equals("أنت"))
                    Tb_You.IsChecked = true;
                if (tb.Equals("العائلة"))
                    Tb_Family.IsChecked = true;
                if (tb.Equals("أنت والعائلة"))
                {
                    Tb_You.IsChecked = true;
                    Tb_Family.IsChecked = true;
                }
                if (hypertention.Equals("أنت"))
                    HyperTentio_You.IsChecked = true;
                if (hypertention.Equals("العائلة"))
                    HyperTentio_Family.IsChecked = true;
                if (hypertention.Equals("أنت والعائلة"))
                {
                    HyperTentio_You.IsChecked = true;
                    HyperTentio_Family.IsChecked = true;
                }
                if (epilepsy.Equals("أنت"))
                    Epilepsy_You.IsChecked = true;
                if (epilepsy.Equals("العائلة"))
                    Epilepsy_Family.IsChecked = true;
                if (epilepsy.Equals("أنت والعائلة"))
                {
                    Epilepsy_You.IsChecked = true;
                    Epilepsy_Family.IsChecked = true;
                }
                if (kidney_dis.Equals("أنت"))
                    Kidney_You.IsChecked = true;
                if (kidney_dis.Equals("العائلة"))
                    Kidney_Family.IsChecked = true;
                if (kidney_dis.Equals("أنت والعائلة"))
                {
                    Kidney_You.IsChecked = true;
                    Kidney_Family.IsChecked = true;
                }
                if (psy_dis.Equals("أنت"))
                    PsychiatricD_You.IsChecked = true;
                if (psy_dis.Equals("العائلة"))
                    PsychiatricD_Family.IsChecked = true;
                if (psy_dis.Equals("أنت والعائلة"))
                {
                    PsychiatricD_You.IsChecked = true;
                    PsychiatricD_Family.IsChecked = true;
                }
                if (cancer.Equals("أنت"))
                    Cancer_You.IsChecked = true;
                if (cancer.Equals("العائلة"))
                    Cancer_Family.IsChecked = true;
                if (cancer.Equals("أنت والعائلة"))
                {
                    Cancer_You.IsChecked = true;
                    Cancer_Family.IsChecked = true;
                }
                if (anemia.Equals("أنت"))
                    Anemia_You.IsChecked = true;
                if (anemia.Equals("العائلة"))
                    Anemia_Family.IsChecked = true;
                if (anemia.Equals("أنت والعائلة"))
                {
                    Anemia_You.IsChecked = true;
                    Anemia_Family.IsChecked = true;
                }
                if (smoking.Equals("أنت"))
                    Smoking_You.IsChecked = true;

                if (other.Equals("أنت"))
                    Others_You.IsChecked = true;

            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void AddButton_PMH_Click(object sender, RoutedEventArgs e)
        {
            if (PMHFileNumField.Text != string.Empty && PMHSNumField.Text != string.Empty)
            {
                string BleedingDisorders = "", Hypertension = "", CardiacDiseases = "", Diabetes = "", HepAtitis = "", Aids = "", Cva = "", Tb = "", Epilepsy = "", KidneyDiseases = "", PsychiatricDiseases = "", Cancer = "", Anemia = "", Smoking = "", Others = "";

                if (BleedingD_You.IsChecked == true && BleedingD_Family.IsChecked != true)
                    BleedingDisorders = "أنت";
                if (BleedingD_You.IsChecked != true && BleedingD_Family.IsChecked == true)
                    BleedingDisorders = "العائلة";
                if (BleedingD_You.IsChecked == true && BleedingD_Family.IsChecked == true)
                    BleedingDisorders = "أنت والعائلة";

                if (HyperTentio_You.IsChecked == true && HyperTentio_Family.IsChecked != true)
                    Hypertension = "أنت";
                if (HyperTentio_You.IsChecked != true && HyperTentio_Family.IsChecked == true)
                    Hypertension = "العائلة";
                if (HyperTentio_You.IsChecked == true && HyperTentio_Family.IsChecked == true)
                    Hypertension = "أنت والعائلة";

                if (CardiacD_You.IsChecked == true && CardiacD_Family.IsChecked != true)
                    CardiacDiseases = "أنت";
                if (CardiacD_You.IsChecked != true && CardiacD_Family.IsChecked == true)
                    CardiacDiseases = "العائلة";
                if (CardiacD_You.IsChecked == true && CardiacD_Family.IsChecked == true)
                    CardiacDiseases = "أنت والعائلة";

                if (Diabetes_You.IsChecked == true && Diabetes_Family.IsChecked != true)
                    Diabetes = "أنت";
                if (Diabetes_You.IsChecked != true && Diabetes_Family.IsChecked == true)
                    Diabetes = "العائلة";
                if (Diabetes_You.IsChecked == true && Diabetes_Family.IsChecked == true)
                    Diabetes = "أنت والعائلة";

                if (Hepatitis_You.IsChecked == true && Hepatitis_Family.IsChecked != true)
                    HepAtitis = "أنت";
                if (Hepatitis_You.IsChecked != true && Hepatitis_Family.IsChecked == true)
                    HepAtitis = "العائلة";
                if (Hepatitis_You.IsChecked == true && Hepatitis_Family.IsChecked == true)
                    HepAtitis = "أنت والعائلة";

                if (Aids_You.IsChecked == true && Aids_Family.IsChecked != true)
                    Aids = "أنت";
                if (Aids_You.IsChecked != true && Aids_Family.IsChecked == true)
                    Aids = "العائلة";
                if (Aids_You.IsChecked == true && Aids_Family.IsChecked == true)
                    Aids = "أنت والعائلة";

                if (Cva_You.IsChecked == true && Cva_Family.IsChecked != true)
                    Cva = "أنت";
                if (Cva_You.IsChecked != true && Cva_Family.IsChecked == true)
                    Cva = "العائلة";
                if (Cva_You.IsChecked == true && Cva_Family.IsChecked == true)
                    Cva = "أنت والعائلة";

                if (Tb_You.IsChecked == true && Tb_Family.IsChecked != true)
                    Tb = "أنت";
                if (Tb_You.IsChecked != true && Tb_Family.IsChecked == true)
                    Tb = "العائلة";
                if (Tb_You.IsChecked == true && Tb_Family.IsChecked == true)
                    Tb = "أنت والعائلة";

                if (Epilepsy_You.IsChecked == true && Epilepsy_Family.IsChecked != true)
                    Epilepsy = "أنت";
                if (Epilepsy_You.IsChecked != true && Epilepsy_Family.IsChecked == true)
                    Epilepsy = "العائلة";
                if (Epilepsy_You.IsChecked == true && Epilepsy_Family.IsChecked == true)
                    Epilepsy = "أنت والعائلة";

                if (Kidney_You.IsChecked == true && Kidney_Family.IsChecked != true)
                    KidneyDiseases = "أنت";
                if (Kidney_You.IsChecked != true && Kidney_Family.IsChecked == true)
                    KidneyDiseases = "العائلة";
                if (Kidney_You.IsChecked == true && Kidney_Family.IsChecked == true)
                    KidneyDiseases = "أنت والعائلة";

                if (PsychiatricD_You.IsChecked == true && PsychiatricD_Family.IsChecked != true)
                    PsychiatricDiseases = "أنت";
                if (Kidney_You.IsChecked != true && PsychiatricD_Family.IsChecked == true)
                    PsychiatricDiseases = "العائلة";
                if (PsychiatricD_You.IsChecked == true && PsychiatricD_Family.IsChecked == true)
                    PsychiatricDiseases = "أنت والعائلة";

                if (Cancer_You.IsChecked == true && Cancer_Family.IsChecked != true)
                    Cancer = "أنت";
                if (Cancer_You.IsChecked != true && Cancer_Family.IsChecked == true)
                    Cancer = "العائلة";
                if (Cancer_You.IsChecked == true && Cancer_Family.IsChecked == true)
                    Cancer = "أنت والعائلة";

                if (Anemia_You.IsChecked == true && Anemia_Family.IsChecked != true)
                    Anemia = "أنت";
                if (Anemia_You.IsChecked != true && Anemia_Family.IsChecked == true)
                    Anemia = "العائلة";
                if (Anemia_You.IsChecked == true && Anemia_Family.IsChecked == true)
                    Anemia = "أنت والعائلة";

                if (Smoking_You.IsChecked == true)
                    Smoking = "أنت";
                if (Others_You.IsChecked == true)
                    Others = "أنت";

                bool result = pastmh_adapter.Insert_PastMedicalHistory(BleedingDisorders, Hypertension, CardiacDiseases, Diabetes, HepAtitis, Aids, Cva, Tb, Epilepsy, KidneyDiseases, PsychiatricDiseases, Cancer, Anemia, Smoking, Others, PMHFileNumField.Text, s_id);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    LoadPastMedicalHistoryGridViw(PMHFileNumField.Text, s_id);
                    AddButton_PMH.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DefaultData();
                }
            }
            else
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void DeleteButton_PMH_Click(object sender, RoutedEventArgs e)
        {
            if (DataGv_PMedicalH.SelectedItem != null)
            {
                if (PMHNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = PMHFileNumField.Text;
                    string session_num = PMHSNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = pastmh_adapter.Delete_PastMedicalHistory(PMHNumField.Text);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadPastMedicalHistoryGridViw(filenum, s_id);
                            PMHNumField.Clear();
                            Adjust_PastMedicalH_Values();
                            pastmh_adapter.Edit_Identity_PastMedicalHistory();
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
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_PMH_Click(object sender, RoutedEventArgs e)
        {
            if (DataGv_PMedicalH.SelectedItem != null)
            {
                if (PMHNumField.Text != string.Empty)
                {
                    // Get  current file num
                    string filenum = PMHFileNumField.Text;
                    string session_num = PMHSNumField.Text;

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + filenum + "" + "والجلسة رقم " + ":" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        string BleedingDisorders = "", Hypertension = "", CardiacDiseases = "", Diabetes = "", HepAtitis = "", Aids = "", Cva = "", Tb = "", Epilepsy = "", KidneyDiseases = "", PsychiatricDiseases = "", Cancer = "", Anemia = "", Smoking = "", Others = "";


                        if (BleedingD_You.IsChecked == true && BleedingD_Family.IsChecked != true)
                            BleedingDisorders = "أنت";
                        if (BleedingD_You.IsChecked != true && BleedingD_Family.IsChecked == true)
                            BleedingDisorders = "العائلة";
                        if (BleedingD_You.IsChecked == true && BleedingD_Family.IsChecked == true)
                            BleedingDisorders = "أنت والعائلة";

                        if (HyperTentio_You.IsChecked == true && HyperTentio_Family.IsChecked != true)
                            Hypertension = "أنت";
                        if (HyperTentio_You.IsChecked != true && HyperTentio_Family.IsChecked == true)
                            Hypertension = "العائلة";
                        if (HyperTentio_You.IsChecked == true && HyperTentio_Family.IsChecked == true)
                            Hypertension = "أنت والعائلة";

                        if (CardiacD_You.IsChecked == true && CardiacD_Family.IsChecked != true)
                            CardiacDiseases = "أنت";
                        if (CardiacD_You.IsChecked != true && CardiacD_Family.IsChecked == true)
                            CardiacDiseases = "العائلة";
                        if (CardiacD_You.IsChecked == true && CardiacD_Family.IsChecked == true)
                            CardiacDiseases = "أنت والعائلة";

                        if (Diabetes_You.IsChecked == true && Diabetes_Family.IsChecked != true)
                            Diabetes = "أنت";
                        if (Diabetes_You.IsChecked != true && Diabetes_Family.IsChecked == true)
                            Diabetes = "العائلة";
                        if (Diabetes_You.IsChecked == true && Diabetes_Family.IsChecked == true)
                            Diabetes = "أنت والعائلة";

                        if (Hepatitis_You.IsChecked == true && Hepatitis_Family.IsChecked != true)
                            HepAtitis = "أنت";
                        if (Hepatitis_You.IsChecked != true && Hepatitis_Family.IsChecked == true)
                            HepAtitis = "العائلة";
                        if (Hepatitis_You.IsChecked == true && Hepatitis_Family.IsChecked == true)
                            HepAtitis = "أنت والعائلة";

                        if (Aids_You.IsChecked == true && Aids_Family.IsChecked != true)
                            Aids = "أنت";
                        if (Aids_You.IsChecked != true && Aids_Family.IsChecked == true)
                            Aids = "العائلة";
                        if (Aids_You.IsChecked == true && Aids_Family.IsChecked == true)
                            Aids = "أنت والعائلة";

                        if (Cva_You.IsChecked == true && Cva_Family.IsChecked != true)
                            Cva = "أنت";
                        if (Cva_You.IsChecked != true && Cva_Family.IsChecked == true)
                            Cva = "العائلة";
                        if (Cva_You.IsChecked == true && Cva_Family.IsChecked == true)
                            Cva = "أنت والعائلة";

                        if (Tb_You.IsChecked == true && Tb_Family.IsChecked != true)
                            Tb = "أنت";
                        if (Tb_You.IsChecked != true && Tb_Family.IsChecked == true)
                            Tb = "العائلة";
                        if (Tb_You.IsChecked == true && Tb_Family.IsChecked == true)
                            Tb = "أنت والعائلة";

                        if (Epilepsy_You.IsChecked == true && Epilepsy_Family.IsChecked != true)
                            Epilepsy = "أنت";
                        if (Epilepsy_You.IsChecked != true && Epilepsy_Family.IsChecked == true)
                            Epilepsy = "العائلة";
                        if (Epilepsy_You.IsChecked == true && Epilepsy_Family.IsChecked == true)
                            Epilepsy = "أنت والعائلة";

                        if (Kidney_You.IsChecked == true && Kidney_Family.IsChecked != true)
                            KidneyDiseases = "أنت";
                        if (Kidney_You.IsChecked != true && Kidney_Family.IsChecked == true)
                            KidneyDiseases = "العائلة";
                        if (Kidney_You.IsChecked == true && Kidney_Family.IsChecked == true)
                            KidneyDiseases = "أنت والعائلة";

                        if (PsychiatricD_You.IsChecked == true && PsychiatricD_Family.IsChecked != true)
                            PsychiatricDiseases = "أنت";
                        if (Kidney_You.IsChecked != true && PsychiatricD_Family.IsChecked == true)
                            PsychiatricDiseases = "العائلة";
                        if (PsychiatricD_You.IsChecked == true && PsychiatricD_Family.IsChecked == true)
                            PsychiatricDiseases = "أنت والعائلة";

                        if (Cancer_You.IsChecked == true && Cancer_Family.IsChecked != true)
                            Cancer = "أنت";
                        if (Cancer_You.IsChecked != true && Cancer_Family.IsChecked == true)
                            Cancer = "العائلة";
                        if (Cancer_You.IsChecked == true && Cancer_Family.IsChecked == true)
                            Cancer = "أنت والعائلة";

                        if (Anemia_You.IsChecked == true && Anemia_Family.IsChecked != true)
                            Anemia = "أنت";
                        if (Anemia_You.IsChecked != true && Anemia_Family.IsChecked == true)
                            Anemia = "العائلة";
                        if (Anemia_You.IsChecked == true && Anemia_Family.IsChecked == true)
                            Anemia = "أنت والعائلة";

                        if (Smoking_You.IsChecked == true)
                            Smoking = "أنت";
                        if (Others_You.IsChecked == true)
                            Others = "أنت";

                        bool result = pastmh_adapter.Update_PastMedicalHistory(PMHNumField.Text, BleedingDisorders, Hypertension, CardiacDiseases, Diabetes, HepAtitis, Aids, Cva, Tb, Epilepsy, KidneyDiseases, PsychiatricDiseases, Cancer, Anemia, Smoking, Others);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadPastMedicalHistoryGridViw(filenum, s_id);
                            PMHNumField.Clear();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }

                    }
                    else
                    {
                        MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من الجدول  بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }




        // Functions For DisclaimerForm View .....
        public void Check_DisclaimerFormInfo()
        {
            // PatientSessionNumField.Text == string.Empty    .... not used
            // Check if Session num detected or not for move to another window
            if (string.IsNullOrEmpty(s_num))
            {
                MessageBox.Show("حدد جلسة  تابعة لرقم ملف محدد من نافذة الفحص السريري حتى تستطيع الانتقال إلى النوافذ التالية !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                if (PrintImg.Visibility == Visibility.Visible)
                {
                    AddButton_DiscForm.IsEnabled = true;
                }
                // DiscFormFileNumField.Text = PatientFileNumField.Text;   ..... not used
                DiscFormFileNumField.Text = FileNum;
                DiscFormSNumField.Text = s_num;
                // Load Data DisclaimerForm table  For this session and file num
                LoadDisclaimerFormGridView(DiscFormFileNumField.Text, s_id);

            }
        }

        private void LoadDisclaimerFormGridView(string file_num, string session_id)


        {
            SqlDataReader sdr_DisclaimerForm = disclaimerf_adapter.Get_DisclaimerForm(file_num, s_id);
            if (sdr_DisclaimerForm.HasRows == false)
            {
                // Allow only one add for current session with DisclaimerForm
                MessageBox.Show("لا يوجد سجل نموذج موافقة مستنيرة بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DBHandler.ConnectionDB.CloseConnection();
                DataGv_DiscForm.ItemsSource = null;
                // IsEnabled AddDisclaimerForm _ Button  For  Add and  delete  and edit  to this session 
                if (PrintImg.Visibility == Visibility.Visible)
                {
                    AddButton_DiscForm.IsEnabled = true;
                }
                // Assigning null to clear all field values when re-entering the interface.
                s_num = null;
                return;
            }
            else
            {
                // Close Connection for DisclaimerForm Methode
                DBHandler.ConnectionDB.CloseConnection();
                SqlDataReader sdr_session = session_physical_adapter.Get_Sessions_OtherTables(file_num, s_id);
                DataTable dt_session = new DataTable();
                dt_session.Columns.Add("SessionId");
                dt_session.Columns.Add("SessionDate", typeof(DateTime));
                dt_session.Columns.Add("SessionNum");
                dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                dt_session.Load(sdr_session);
                DBHandler.ConnectionDB.CloseConnection();

                sdr_DisclaimerForm = disclaimerf_adapter.Get_DisclaimerForm(file_num, s_id);
                DataTable dt_DisclaimerForm = new DataTable();
                dt_session.Columns.Add("DisclaimerFormId");
                dt_session.Columns.Add("PrintImg");
                dt_DisclaimerForm.PrimaryKey = new DataColumn[] { dt_DisclaimerForm.Columns["SessionId"] };

                dt_DisclaimerForm.Load(sdr_DisclaimerForm);
                DBHandler.ConnectionDB.CloseConnection();
                // Merg Tow DataTable in one for this GridView
                dt_session.Merge(dt_DisclaimerForm, false, MissingSchemaAction.Ignore);
                DataGv_MedicalH.ItemsSource = dt_session.DefaultView;

                // IsEnabled AddDisclaimerForm_ Button  For  only delete  and edit  to this session 
                if (PrintImg.Visibility == Visibility.Visible)
                {
                    AddButton_DiscForm.IsEnabled = true;
                }

                // Assigning null to clear all field values when re-entering the interface.
                s_num = null;

                // Fill all fields with values in DiscForm  table
                // Adjust_DiscForm_Values();
            }
        }

        private void DataGv_DiscForm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Not Used Yet
        }

        private void AddButton_DiscForm_Click(object sender, RoutedEventArgs e)
        {
            // Not Used Yet
        }

        private void DeleteButton_DiscForm_Click(object sender, RoutedEventArgs e)
        {
            // Not Used Yet
        }

        private void Print_DiscForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog dialog = new PrintDialog();
                if (dialog.ShowDialog() == true)
                {
                    AddButton_DiscForm.Visibility = Visibility.Hidden;
                    DeleteButton_DiscForm.Visibility = Visibility.Hidden;
                    Print_DiscForm.Visibility = Visibility.Hidden;
                    dialog.PrintVisual(DisclaimerForm_Content, "طباعة نموذج الموافقة المستنيرة");
                }
                else
                {
                    MessageBox.Show("تم الغاء عملية الطباعة !", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            finally
            {
                this.IsEnabled = true;
                AddButton_DiscForm.Visibility = Visibility.Visible;
                DeleteButton_DiscForm.Visibility = Visibility.Visible;
                Print_DiscForm.Visibility = Visibility.Visible;
            }
            //Specefic print 
            // Create a FlowDocument step 2 (Method1 .. not used)
            /* FlowDocument doc = new FlowDocument(new Paragraph(new Run("مركز لمسة شفاء للحجامة")));
             doc.Name = "DisclaimerFormDocument";

             doc.PagePadding = new Thickness(0);
             doc.ColumnGap = 0;
             doc.ColumnWidth = print.PrintableAreaWidth;

             // Create IDocumentPaginatorSource from FlowDocument  step 3
             IDocumentPaginatorSource idpsource = doc;
             // Call PrintDocument method to send document to printer step 4
             print.PrintDocument(idpsource.DocumentPaginator, "طباعة نموذج موافقة مستنيرة");
            */
        }


        // Fucnctions for Validations
        private bool Session_IsValid()
        {
            if (PatientFileNumField.Text == string.Empty)
            {
                MessageBox.Show("  حقل رقم الملف اجباري - اضغط مرتين وحدد الملف من الجدول", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DataViewFromPatientTable.Focus();
                return false;
            }
            if (PatientNameField.Text == string.Empty)
            {
                MessageBox.Show("حقل اسم المريض اجباري - اضغط مرتين وحدد الملف من الجدول", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DataViewFromPatientTable.Focus();
                return false;
            }
            if (PatientSessionDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PatientSessionDateField.Focus();
                return false;
            }

            return true;
        }
        // Fucnctions for clear
        private void Clear_InfoPatient()
        {
            PatientFileNumField.Clear();
            PatientNameField.Clear();
        }

        private void Clear_Session()
        {

            // Session Fields
            PatientSessionNumField.Clear();
            PatientSessionDateField.SelectedDate = null;
            PatientSessionPeriodField.Clear();
            PatientComplaintsField.Clear();
            ComplicationsField.Clear();

            // PhysicalExamination Fields
            TempField.Clear();
            HRField.Clear();
            RRField.Clear();
            BpField.Clear();
            HeadNeckField.Clear();
            ChestBackField.Clear();
            AbdomenField.Clear();
            PelvisField.Clear();
            UpperLowerLimbsField.Clear();
            OthersField.Clear();

        }

        private void SearchPatientFileNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public void Clear_Contraindications()
        {
            DataGv_Contraindications_Cupping.ItemsSource = null;
            // Patient info
            PFileNumField.Clear();
            PSessionNumField.Clear();
            PContraindicationNumField.Clear();

            // Default Data for ContraindicationsOfCupping
            Diabetes_NotFound.IsChecked = true;
            RenalF_NotFound.IsChecked = true;
            Pregnancy_NotFound.IsChecked = true;
            Pacemaker_NotFound.IsChecked = true;
            VaricoseV_NotFound.IsChecked = true;
            BleedingD_NotFound.IsChecked = true;
            Aids_NotFound.IsChecked = true;
            LiverF_NotFound.IsChecked = true;
            ChildrenB12_NotFound.IsChecked = true;
            Cancer_NotFound.IsChecked = true;
        }

        private void Clear_MedicalHistory()
        {
            DataGv_MedicalH.ItemsSource = null;
            MHFileNumField.Clear();
            MHSessionNumField.Clear();
            MHNumField.Clear();

            CurrentSymptom.Clear();
            Onset.Clear();
            Location.Clear();
            Duration.Clear();
            Character.Clear();
            AggravatingFactors.Clear();
            Timing.Clear();
            Severity.Clear();
            Allergies.Clear();
            Medications.Clear();
            PastSurgicalOrCuppingHistory.Clear();
            OthersComplaints.Clear();
            CounsultAdoctor.Clear();
        }

        private void Clear_PastMedicalHistory()
        {
            DataGv_PMedicalH.ItemsSource = null;
            PMHFileNumField.Clear();
            PMHSNumField.Clear();
            PMHNumField.Clear();

            Adjust_PastMedicalH_Values();
        }

        private void Clear_DisclaimerForm()
        {
            DataGv_DiscForm.ItemsSource = null;
            DiscFormFileNumField.Clear();
            DiscFormSNumField.Clear();
            DiscFormNumField.Clear();

            PrintImg.Source = null;
        }

    }
}
