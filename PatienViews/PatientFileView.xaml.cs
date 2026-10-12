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
using ClosedXML.Excel;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for PatientFileView.xaml
    /// </summary>
    public partial class PatientFileView : UserControl
    {
        DBHandler.PatientAdapter patient_adapter = new DBHandler.PatientAdapter();
        DBHandler.BillAdapter bill_adapter = new DBHandler.BillAdapter();
        DBHandler.SessionPhysicalAdaptercs session_adapter = new DBHandler.SessionPhysicalAdaptercs();
        DBHandler.DeletedBillsAdapter deletedb_adapter = new DBHandler.DeletedBillsAdapter();
        DBHandler.MedicalHistoryAdapter mh_adapter = new DBHandler.MedicalHistoryAdapter();
        DBHandler.PastMedicalHistoryAdapter pmh_adapter = new DBHandler.PastMedicalHistoryAdapter();
        DBHandler.DisclaimerFormAdapter disclaimerf_adapter = new DBHandler.DisclaimerFormAdapter();
        DBHandler.PainAreasAdapter painarea_adapter = new DBHandler.PainAreasAdapter();
        DBHandler.TreatmentPlanAdapter treatmentplan_adapter = new DBHandler.TreatmentPlanAdapter();
        DBHandler.ContraindicationsOfCuppingAdapter contofcupping_adapter = new DBHandler.ContraindicationsOfCuppingAdapter();

        string filenum, session_id_bill;
        //Intialize deletebill variable for notify Admin
        public static int delete_bill_Admin = 0;
        // Intialize variables for print patient file
        public static string file_num = "", date = "", name = "", age = "", mobile = "", national_num = "", gender = "", martial_status = "", address = "", job = "", job_address = "", e_name = "", e_contact = "";
        // static DataGrid for load function
        public static DataGrid DataViewPatient = new DataGrid();

        public PatientFileView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_Patient();
            DefaultData();
            LoadDataGridViw();
            //DataViewPatient.ItemsSource = DataViewFromPatientTable.ItemsSource;
             DataViewPatient =  DataViewFromPatientTable ;
        }

        //load all data to my account when loaded 
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            PatientFileDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        //Load Data from Table to DATA Grid View
        private void LoadDataGridViw()
        {
            SqlDataReader sdr = patient_adapter.Get_Patients();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            DataViewFromPatientTable.ItemsSource = dt.DefaultView;
            DBHandler.ConnectionDB.CloseConnection();
        }

        private void DataViewFromPatientTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell0 = DataViewFromPatientTable.SelectedCells[0];
            var cell1 = DataViewFromPatientTable.SelectedCells[1];
            var cell2 = DataViewFromPatientTable.SelectedCells[2];
            var cell3 = DataViewFromPatientTable.SelectedCells[3];
            var cell4 = DataViewFromPatientTable.SelectedCells[4];
            var cell5 = DataViewFromPatientTable.SelectedCells[5];
            var cell6 = DataViewFromPatientTable.SelectedCells[6];
            var cell7 = DataViewFromPatientTable.SelectedCells[7];
            var cell8 = DataViewFromPatientTable.SelectedCells[8];
            var cell9 = DataViewFromPatientTable.SelectedCells[9];
            var cell10 = DataViewFromPatientTable.SelectedCells[10];
            var cell11 = DataViewFromPatientTable.SelectedCells[11];
            var cell12 = DataViewFromPatientTable.SelectedCells[12];
            var cell13 = DataViewFromPatientTable.SelectedCells[13];

            string filenum = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
            string pdate = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string pname = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string nationalnum = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string mobilenum = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
            string age = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
            string gender = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
            string martial_s = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
            string address = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
            string job = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
            string jobaddress = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
            string email = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
            string emergency_name = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
            string emergency_contact = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;

            Clear_Patient();
            PatientFileNumField.Text = filenum;
            PatientFileDateField.Text = DateTime.Parse(pdate).ToString();
            PatientNameField.Text = pname;
            MobileNumField.Text = mobilenum;
            if (nationalnum != string.Empty)
                NationalityNumField.Text = nationalnum;
            if (age != string.Empty)
                AgeField.Text = age;
            if (gender != string.Empty && gender.Equals("ذكر"))
                Male.IsChecked = true;
            if (gender != string.Empty && gender.Equals("أنثى"))
                Femal.IsChecked = true;
            if (martial_s != string.Empty && martial_s.Equals("أعزب"))
                Single.IsChecked = true;
            if (martial_s != string.Empty && martial_s.Equals("متزوج"))
                Married.IsChecked = true;
            if (address != string.Empty)
                AddressField.Text = address;
            if (job != string.Empty)
                JobField.Text = job;
            if (jobaddress != string.Empty)
                JobAddressField.Text = jobaddress;
            if (email != string.Empty)
                EmailFeld.Text = email;
            if (emergency_name != string.Empty)
                EmergencyContactNameField.Text = emergency_name;
            if (emergency_contact != string.Empty)
                EmergencyContactMobileField.Text = emergency_contact;

        }

        //Functions For Patient Management View
        private void AddButton_PatientFileMg_Click(object sender, RoutedEventArgs e)
        {
            if (Patient_IsValid())
            {
                // Check if this record already exist 
                bool check = patient_adapter.Check_ExistFile(PatientNameField.Text);
                if (check)
                {
                    MessageBox.Show("فشلت عملية الاضافة ! اسم المريض موجود مسبقا ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    PatientNameField.Focus();
                    PatientNameField.Clear();
                    return;
                }
                else
                {
                    string gender = "";
                    string martial_s = "";

                    if (Male.IsChecked == true)
                        gender = "ذكر";
                    if (Femal.IsChecked == true)
                        gender = "أنثى";

                    if (Single.IsChecked == true)
                        martial_s = "أعزب";
                    if (Married.IsChecked == true)
                        martial_s = "متزوج";

                    bool result = patient_adapter.Insert_Patient(PatientNameField.Text, PatientFileDateField.Text, NationalityNumField.Text, MobileNumField.Text, AgeField.Text, gender, martial_s, AddressField.Text, JobField.Text, JobAddressField.Text, EmailFeld.Text, EmergencyContactNameField.Text, EmergencyContactMobileField.Text);
                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Patient();
                        DefaultData();
                        LoadDataGridViw();
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_Patient();
                        DefaultData();
                    }

                }
            }
        }

        private void DeleteButton_PatientFileMg_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromPatientTable.SelectedItem != null)
            {
                if (PatientFileDateField.Text == string.Empty || PatientFileNumField.Text == string.Empty || PatientNameField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد ! سيتم حذف جميع البيانات المرتبطة بهذا الملف ", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        var cell0 = DataViewFromPatientTable.SelectedCells[0];
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                        // Get Data from bill for insert deleted record if found it 
                        SqlDataReader file_bills = bill_adapter.Get_BillsPatientFile(PatientFileNumField.Text);
                        //if no found any records 
                        if (file_bills.HasRows == false)
                        {
                            DBHandler.ConnectionDB.CloseConnection();
                        }
                        else
                        {
                            bool r = Get_DataBIlls(PatientFileNumField.Text, file_bills);
                            // for notify Admin with message box
                            if (r == true)
                            {
                                delete_bill_Admin = 1;
                            }
                            else
                            {
                                delete_bill_Admin = 0;
                            }
                        }
                        bool result = patient_adapter.Delete_Patient(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadDataGridViw();
                            Clear_Patient();
                            DefaultData();
                            patient_adapter.Edit_Identity();
                            session_adapter.Edit_Identity_Sessions();
                            session_adapter.Edit_Identity_PhysicalExamination();
                            mh_adapter.Edit_Identity_MedicalHistory();
                            pmh_adapter.Edit_Identity_PastMedicalHistory();
                            contofcupping_adapter.Edit_Identity_ContraindicationsOfCupping();
                            disclaimerf_adapter.Edit_Identity_DisclaimerForm();
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
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // Function for get data from all bills and then inserted to Deleted Records Table
        private bool Get_DataBIlls(string filenum, SqlDataReader sdr_bills)
        {
            // Intialize Varaiables 
            float price = 0, tax = 0, discount = 0, total_p = 0, remaining = 0;
            DataTable dt = new DataTable();
            dt.Load(sdr_bills);
            // Close connection for bill_adapter.Get_BillsExtract(filenum) Query
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

            bool result_deletb = deletedb_adapter.Insert_DeletedBillsRecords(filenum, id, bill_date, chip_type, add_service, r_price, nationality_type, r_tax, r_discount, payment_type, r_totalp, r_remaining, entry_name, deletedb_date);
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

        private void ExtraSearch_PatientFile_Button_Click(object sender, RoutedEventArgs e)
        {
            ExtraSearchView searchview = new ExtraSearchView();
            searchview.Show();
        }

        //Reload DataGrid after search
        private void ReloadTable_PatientFile_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGridViw();
        }

        private void SearchPatientFileNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void EdieButton_PatientFileMg_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromPatientTable.SelectedItem != null)
            {
                if (PatientFileDateField.Text == string.Empty || PatientFileNumField.Text == string.Empty || PatientNameField.Text == string.Empty || MobileNumField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("حقل اسم المريض و رقم الجوال وتاريخ الملف لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (Patient_IsValid())
                    {
                        string gender = "";
                        string martial_s = "";

                        if (Male.IsChecked == true)
                            gender = "ذكر";
                        if (Femal.IsChecked == true)
                            gender = "أنثى";

                        if (Single.IsChecked == true)
                            martial_s = "أعزب";
                        if (Married.IsChecked == true)
                            martial_s = "متزوج";

                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد التحديث بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            var cell0 = DataViewFromPatientTable.SelectedCells[0];
                            string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                            bool result = patient_adapter.Update_Patient(id, PatientNameField.Text, PatientFileDateField.Text, NationalityNumField.Text, MobileNumField.Text, AgeField.Text, gender, martial_s, AddressField.Text, JobField.Text, JobAddressField.Text, EmailFeld.Text, EmergencyContactNameField.Text, EmergencyContactMobileField.Text);
                            if (result)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                LoadDataGridViw();
                                Clear_Patient();
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
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void ClearButton_PatientFileMg_Click(object sender, RoutedEventArgs e)
        {
            Clear_Patient();
            DefaultData();
        }

        private void ExcelExtractButton_PatientFileMg_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser;
            string filename = "ادارة المرضى";

            if (DataViewFromPatientTable.ItemsSource == null)
            {
                MessageBox.Show("لايوجد أي بيانات لاكمال الاستخراج إلى  Excel", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }
            else
            {
                // Reference From Excel 
                var wb = new XLWorkbook();
                DataTable dt = Get_PatientGV();
                // DBHandler.ConnectionDB.CloseConnection();
                var style_sheet = wb.Worksheets.Add(dt, filename);

                style_sheet.Columns().AdjustToContents();
                style_sheet.Rows().AdjustToContents();

                //Rename Columns Names in Excel File
                style_sheet.Cell(1, 1).SetValue("رقم الملف");
                style_sheet.Cell(1, 2).SetValue("اسم المريض");
                style_sheet.Cell(1, 3).SetValue("تاريخ الملف");
                style_sheet.Cell(1, 4).SetValue("رقم الهوية");
                style_sheet.Cell(1, 5).SetValue("رقم الجوال");
                style_sheet.Cell(1, 6).SetValue("العمر");
                style_sheet.Cell(1, 7).SetValue("الجنس");
                style_sheet.Cell(1, 8).SetValue("الوضع الإجتماعي");
                style_sheet.Cell(1, 9).SetValue("العنوان");
                style_sheet.Cell(1, 10).SetValue("الوظيفة");
                style_sheet.Cell(1, 11).SetValue("عنوان العمل");
                style_sheet.Cell(1, 12).SetValue("الإيميل");
                style_sheet.Cell(1, 13).SetValue("اسم للطوارئ");
                // Restyle Excel File
                var style_cell = style_sheet.Style;
                style_cell.Font.Bold = true;
                style_cell.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                style_sheet.Cell(1, 6).Style.Font.FontSize = 10;
                style_sheet.Cell(1, 8).Style.Font.FontSize = 10;
                style_sheet.Cell(1, 10).Style.Font.FontSize = 10;
                style_sheet.Columns(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                style_sheet.Columns(1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                style_sheet.Columns(1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                // Detect Lacation OF  Excel File and Save it
                folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string location = folderBrowser.SelectedPath;
                    wb.SaveAs(location + @"\" + filename + ".xlsx");
                    MessageBox.Show("تم الاستخراج بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    MessageBox.Show("يجب تحديد مكان لحفظ الملف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                }
            }
        }

        //Function for print Patient File ...
        private void PrintPage_PatientFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if (PatientFileNumField.Text == string.Empty || PatientFileDateField.Text == string.Empty || PatientNameField.Text == string.Empty || MobileNumField.Text == string.Empty)
            {
                MessageBox.Show("لايمكن الطباعة ! فضلا حدد الملف المراد طباعته من الجدول بالضغط مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                //Send Information To Print Window ..
                file_num = PatientFileNumField.Text;
                date = PatientFileDateField.Text;
                name = PatientNameField.Text;
                if (AgeField.Text != string.Empty)
                    age = AgeField.Text;
                mobile = MobileNumField.Text;
                if (NationalityNumField.Text != string.Empty)
                    national_num = NationalityNumField.Text;
                if (Femal.IsChecked == true)
                    gender = "أنثى";
                if (Male.IsChecked == true)
                    gender = "ذكر";
                if (Single.IsChecked == true)
                    martial_status = "أعزب";
                if (Married.IsChecked == true)
                    martial_status = "متزوج";
                if (AddressField.Text != string.Empty)
                    address = AddressField.Text;
                if (JobField.Text != string.Empty)
                    job = JobField.Text;
                if (JobAddressField.Text != string.Empty)
                    job_address = JobAddressField.Text;
                if (EmergencyContactNameField.Text != string.Empty)
                    e_name = EmergencyContactNameField.Text;
                if (EmergencyContactMobileField.Text != string.Empty)
                    e_contact = EmergencyContactMobileField.Text;

                PrintPatientFileView printfile = new PrintPatientFileView();
                printfile.Show();

            }
        }

        // Get Data From Gv To dt
        private DataTable Get_PatientGV()
        {
            DataTable dt = ((DataView)DataViewFromPatientTable.ItemsSource).ToTable();
            return dt;
        }

        private void NumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        // Functions for Searching
        private void PatientSearchFilNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPatientFileNum.Text = "أدخل رقم الملف";
        }

        private void PatientSearchMobileNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchMobileNum.Text = "أدخل رقم الجوال";
        }

        private void SearchMobileNum_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SearchMobileNum.Text != string.Empty && SearchMobileNum.Text != "أدخل رقم الجوال")
            {
                SqlDataReader sdr = patient_adapter.Get_Patient_Contactnum(SearchMobileNum.Text);
               // MessageBox.Show(SearchMobileNum.Text, "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                if (sdr.HasRows)
                {
                    Load_Search(sdr);
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
            if (SearchPatientFileNum.Text != string.Empty && SearchPatientFileNum.Text != "أدخل رقم الملف")
            {
                SqlDataReader sdr = patient_adapter.Get_Patient_Filenum(SearchPatientFileNum.Text);
                if (sdr.HasRows)
                {
                    Load_Search(sdr);
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

        // LoadData To Grid View 
        public static void Load_Search(SqlDataReader sdr)
        {
            DataTable dt = new DataTable();
            dt.Load(sdr);
            //DataViewFromPatientTable.ItemsSource = dt.DefaultView;
            DataViewPatient.ItemsSource = dt.DefaultView;
            
        }
        // Functions For Validation Fields and Clear Value From Fields
        private bool Patient_IsValid()
        {
            if (PatientFileDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PatientFileDateField.Focus();
                return false;
            }
            if (PatientNameField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PatientNameField.Focus();
                return false;
            }
            if (MobileNumField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MobileNumField.Focus();
                return false;
            }

            if (EmailFeld.Text != string.Empty)
            {
                if (!EmailFeld.Text.Contains("@") || !EmailFeld.Text.Contains(".com"))
                {
                    MessageBox.Show("فضلا أدخل الإيميل بالشكل الصحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    return false;
                }
            }
            if (EmailFeld.Text != string.Empty)
            {
                if (!EmailFeld.Text.Contains("gmail"))
                {
                    MessageBox.Show("فضلا أدخل ايميل Gmail فقط", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    return false;
                }
            }
            return true;
        }

        private void Clear_Patient()
        {
            PatientFileNumField.Clear();
            PatientFileDateField.SelectedDate = null;
            PatientNameField.Clear();
            NationalityNumField.Clear();
            MobileNumField.Clear();
            AgeField.Clear();
            Single.IsChecked = false;
            Married.IsChecked = false;
            AddressField.Clear();
            JobField.Clear();
            JobAddressField.Clear();
            EmailFeld.Clear();
            EmergencyContactNameField.Clear();
            EmergencyContactMobileField.Clear();
            Male.IsChecked = false;
            Femal.IsChecked = false;
        }

    }
}
