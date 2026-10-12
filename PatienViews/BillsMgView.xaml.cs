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
    /// Interaction logic for BillsMgView.xaml
    /// </summary>
    public partial class BillsMgView : UserControl
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
        DBHandler.DiscountsAdapter discount_adapter = new DBHandler.DiscountsAdapter();
        DBHandler.TaxAdapter tax_adapter = new DBHandler.TaxAdapter();
        // intialize Variables ....
        string filenum, session_id_bill;
        //Intialize deletebill variable for notify Admin
        public static int delete_bill_Admin = 0;
        // Intialize variables for print patient file
        public static string file_num = "", date = "", name = "", age = "", mobile = "", national_num = "", gender = "", martial_status = "", address = "", job = "", job_address = "", e_name = "", e_contact = "";
        // intialize variable for discount whith TotalPricePaid 
        double price_paid = 0;
        // Get Current Date 
        DateTime now;
        string cureent_date;
        //Intialize all elements in Bill View for printing
        public static string p_name = "", file_no = "", bill_no = "", bill_date = "", bill_tax = "", bill_discount = "", totalpaid = "", remaining = "", residu = "", bill_paid_type = "", username = "", cash_paid = "", network_paid = "", bill_tax_SR = "", session_price = "", paid_amount = "", paid_value_patient = "";
        //Intialize & Assign 0 to print_iterface2 for printBill xml 
        //public static int print_interface2 = 0;

        public BillsMgView()
        {
            InitializeComponent();
            MessageBox.Show("  فضلا ابحث عن الفاتورة المطلوبة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_BillMg();
            now = DateTime.Now;
            cureent_date = now.Day + "/" + now.Month + "/" + now.Year;
           // MessageBox.Show("  فضلا ابحث عن الفاتورة المطلوبة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            DefaultData();
            if(Sign_in.User.Equals("Admin"))
            {
                DeleteButton_BillMg.Visibility = Visibility.Visible;
                EditButton_BillMg.Visibility = Visibility.Visible;
            }
        }

        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            BillDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        // Functions For Bill Management View ....
        private void DeleteButton_BillMg_Click(object sender, RoutedEventArgs e)
        {
            if (BillMgNumField.Text != string.Empty)
            {
                if (DataGv_BillMg.SelectedItem != null)
                {
                    if (BillNumField.Text != string.Empty)
                    {

                        System.Windows.Forms.DialogResult dialog1 = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف الفاتورة التابعة لملف المريض رقم  " + " " + ":" + BillMgFileNumField.Text, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog1) == 6)
                        {
                            System.Windows.Forms.DialogResult dialog2 = (System.Windows.Forms.DialogResult)MessageBox.Show("سيتم حذف الجلسة التابعة للفاتورة رقم " + " " + ":" + BillMgNumField.Text + "" + "هل تريد المتابعة ", "تحذير هام", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                            if (((int)dialog2) == 6)
                            {
                                //Delete Bill
                                bool result1 = bill_adapter.Delete_BillExtract(BillMgNumField.Text);
                                if (result1)
                                {
                                    // Call Function for delete session for this bill 
                                    bool result2 = session_adapter.Delete_Session(session_id_bill);
                                    if (result2)
                                    {
                                        MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                        //Swich Between Tow Grid 
                                        DataViewFromPatientTable_BillMg.Visibility = Visibility.Visible;
                                        DataGv_BillMg.Visibility = Visibility.Hidden;
                                        DataGv_BillMg.ItemsSource = null;
                                        DataViewFromPatientTable_BillMg.ItemsSource = null;
                                        //EditIdintity for session and all associated tables
                                        bill_adapter.Edit_Identity_Bill();
                                        session_adapter.Edit_Identity_PhysicalExamination();
                                        session_adapter.Edit_Identity_Sessions();
                                        contofcupping_adapter.Edit_Identity_ContraindicationsOfCupping();
                                        mh_adapter.Edit_Identity_MedicalHistory();
                                        pmh_adapter.Edit_Identity_PastMedicalHistory();
                                        disclaimerf_adapter.Edit_Identity_DisclaimerForm();
                                        bool r1 = Delete_ImgPainArea(session_id_bill);
                                        if (r1)
                                        {
                                            painarea_adapter.Edit_Identity_PainAreas();

                                        }
                                        bool r2 = Delete_ImgTreatmentPlan(session_id_bill);
                                        if (r2)
                                        {
                                            treatmentplan_adapter.Edit_Identity_TreatmentPlan();
                                        }
                                        // Insert this Deleted Bill Record to DeletedBillsRecords Table ...
                                        // Get All Data From Bill View 
                                        //Get PaidChipType RadioButtons (not Used)
                                        int chip_type = 0;
                                        /*if (PaidChip_Type1.IsChecked == true)
                                        {
                                            chip_type = 50;
                                        }
                                        if (PaidChip_Type2.IsChecked == true)
                                        {
                                            chip_type = 100;
                                        }
                                        if (PaidChip_Type3.IsChecked == true)
                                        {
                                            chip_type = 150;
                                        }
                                        if (PaidChip_Type4.IsChecked == true)
                                        {
                                            chip_type = 200;
                                        }
                                        if (PaidChip_Type5.IsChecked == true)
                                        {
                                            chip_type = 250;
                                        }
                                        if (PaidChip_Type6.IsChecked == true)
                                        {
                                            chip_type = 300;
                                        }
                                        if (PaidChip_Type7.IsChecked == true)
                                        {
                                            chip_type = 350;
                                        }*/

                                        //Get NationalityType RadioButtons
                                        string nationality_type = "";
                                        if (Na_Type1.IsChecked == true)
                                        {
                                            nationality_type = "سعودي";
                                        }
                                        if (Na_Type2.IsChecked == true)
                                        {
                                            nationality_type = "غير سعودي";
                                        }
                                        //Get PaidType RadioButtons
                                        string paid_type = "";
                                        if (Paid_Type1.IsChecked == true)
                                        {
                                            paid_type = "نقدي";
                                        }
                                        if (Paid_Type2.IsChecked == true)
                                        {
                                            paid_type = "شبكة ";
                                        }
                                        if (Paid_Type3.IsChecked == true)
                                        {
                                            paid_type = "نقدي وشبكة";
                                        }
                                        // Get DeletedBillDate Field
                                        DateTime now = DateTime.Now;
                                        string deletedb_date = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year).ToString();
                                        // Get EntryName
                                        string entry_name = Sign_in.User;
                                        bool result3 = deletedb_adapter.Insert_DeletedBillsRecords(filenum, BillMgNumField.Text, BillDateField.Text, chip_type, AddionalServices.Text, Price.Text, nationality_type, Tax.Text, Discount.Text, paid_type, TotalPaidPrice.Text, RemainingUnPaid.Text, entry_name, deletedb_date);
                                        if (result3)
                                        {
                                            //Notefication  To Admin
                                            delete_bill_Admin = 1;
                                        }
                                        else
                                        {
                                            // Test ....
                                            MessageBox.Show("فشلت عملية الاضافة الى الفواتير المحذوفة");
                                            delete_bill_Admin = 0;
                                        }

                                        Clear_BillMg();
                                        DefaultData();
                                    }
                                    else
                                    {
                                        MessageBox.Show("فشلت عملية الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                    }
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
                            MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        MessageBox.Show("حدد سجلا من جدول الفواتير لتحديد الفاتورة المطلوب حذفها بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show("حدد سجلا من جدول الفواتير لتحديد الفاتورة المطلوب حذفها بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من جدول ملفات المرضى بالنقر مرتين حتى تستطيع الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
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

        private void EditButton_BillMg_Click(object sender, RoutedEventArgs e)
        {
            if (BillMgNumField.Text != string.Empty)
            {
                if (DataGv_BillMg.SelectedItem != null)
                {
                    if (BillNumField.Text != string.Empty)
                    {
                        System.Windows.Forms.DialogResult dialog1 = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الفاتورة التابعة لملف المريض رقم  " + " " + ":" + BillMgFileNumField.Text, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog1) == 6)
                        {
                            if (Bill_IsValid())
                            {
                                // Get All Data From Bill View 
                                //Get PaidChipType RadioButtons
                                int chip_type = 0;
                                /* not used currently
                                if (PaidChip_Type1.IsChecked == true)
                                {
                                    chip_type = 50;
                                }
                                if (PaidChip_Type2.IsChecked == true)
                                {
                                    chip_type = 100;
                                }
                                if (PaidChip_Type3.IsChecked == true)
                                {
                                    chip_type = 150;
                                }
                                if (PaidChip_Type4.IsChecked == true)
                                {
                                    chip_type = 200;
                                }
                                if (PaidChip_Type5.IsChecked == true)
                                {
                                    chip_type = 250;
                                }
                                if (PaidChip_Type6.IsChecked == true)
                                {
                                    chip_type = 300;
                                }
                                if (PaidChip_Type7.IsChecked == true)
                                {
                                    chip_type = 350;
                                }
                                */
                                //Get NationalityType RadioButtons
                                string nationality_type = "";
                                if (Na_Type1.IsChecked == true)
                                {
                                    nationality_type = "سعودي";
                                }
                                if (Na_Type2.IsChecked == true)
                                {
                                    nationality_type = "غير سعودي";
                                }
                                //Get PaidType RadioButtons
                                string paid_type = "";
                                if (Paid_Type1.IsChecked == true)
                                {
                                    paid_type = "نقدي";
                                }
                                if (Paid_Type2.IsChecked == true)
                                {
                                    paid_type = "شبكة ";
                                }
                                if (Paid_Type3.IsChecked == true)
                                {
                                    paid_type = "نقدي وشبكة ";
                                }

                                System.Windows.Forms.DialogResult dialog2 = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تأكدت من حساب السعر الإجمالي  قبل انهاء عملية التحديث   ", "تحذير هام", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                                if (((int)dialog2) == 6)
                                {
                                    bool result = bill_adapter.Update_BillExtract(BillMgNumField.Text, chip_type, AddionalServices.Text, Price.Text, nationality_type, Tax_SR.Text, Tax.Text , Discount.Text, paid_type,SessionPrice.Text ,TotalPaidPrice.Text,CashTotalPaidPrice.Text ,NetworkTotalPaidPrice.Text ,PaidAmount.Text ,PaidValue.Text , RemainingUnPaid.Text);
                                    if (result)
                                    {
                                        MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                        //Swich Between Tow Grid 
                                        DataViewFromPatientTable_BillMg.Visibility = Visibility.Visible;
                                        DataGv_BillMg.Visibility = Visibility.Hidden;
                                        DataGv_BillMg.ItemsSource = null;
                                        DataViewFromPatientTable_BillMg.ItemsSource = null;

                                        Clear_BillMg();
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
                        else
                        {
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        MessageBox.Show("حدد سجلا من جدول الفواتير لتحديد الفاتورة المطلوب التعديل عليها بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show("حدد سجلا من جدول الفواتير لتحديد الفاتورة المطلوب التعديل عليها بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حدد سجلا من جدول ملفات المرضى بالنقر مرتين حتى تستطيع التعديل عليه", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // If chacked Visibility cash and network TextBox
        private void Paid_Type3_Checked(object sender, RoutedEventArgs e)
        {
            PaymentTogether.Visibility = Visibility.Visible;
        }

        // If chacked Hidden cash and network TextBox
        private void Paid_Type3_Unchecked(object sender, RoutedEventArgs e)
        {
            PaymentTogether.Visibility = Visibility.Hidden;
        }

        //Check if tax value for Saudis in tax Table is found or not
        private void Na_Type1_Click(object sender, RoutedEventArgs e)
        {
            SqlDataReader sdr = tax_adapter.Get_TaxForSaudis();
            if (sdr.HasRows == false)
            {
                //Not Found Value for TaxForSaudis
                DBHandler.ConnectionDB.CloseConnection();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                Tax.Text = dt.Rows[0].Field<string>(0);
                DBHandler.ConnectionDB.CloseConnection();
            }
        }

        //Check if tax value for NonSaudis in tax Table is found or not
        private void Na_Type2_Click(object sender, RoutedEventArgs e)
        {
            SqlDataReader sdr = tax_adapter.Get_TaxForNonSaudis();
            if (sdr.HasRows == false)
            {
                //Not Found Value for TaxForSaudis
                DBHandler.ConnectionDB.CloseConnection();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                Tax.Text = dt.Rows[0].Field<string>(0);
                DBHandler.ConnectionDB.CloseConnection();

            }
        }

        //Function for calculiting Total paid amount 
        private void CalculateBillButton_BillMg_Click(object sender, RoutedEventArgs e)
        {
            double price_add_service = 0, discount = 0, session_price = 0, tax_percentage_int = 0;

            //price add service
            if (Price.Text != string.Empty)
            {
                price_add_service = double.Parse(Price.Text);
            }

            //check nationality 
            if (Na_Type1.IsChecked == false && Na_Type2.IsChecked == false)
            {
                MessageBox.Show("  حدد الجنسية لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

            else
            {
                if (Tax.Text != string.Empty)
                {
                    //split string Tax percentage to int tax
                    string[] tax_percentage = Tax.Text.Split('%');
                    tax_percentage_int = double.Parse(tax_percentage[0]);

                    /*foreach(var value in tax_percentage)
                    {
                       // MessageBox.Show("tax" + value, "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        // 15% =>   15
                        tax_percentage_int = int.Parse(value);
                    }*/
                }
                else
                {
                    MessageBox.Show("أدخل حقل الضريبة من اعدادات الضريبة قد تكون القيمة صفر بالمئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Tax.Focus();
                }
            }

            //check Addtional servicess & price 
            if (AddionalServices.Text != string.Empty && Price.Text == string.Empty)
            {
                MessageBox.Show("حقل السعر لايمكن أن يكون فارغ عند ملئ الخدمات الإضافية ! أدخل السعر وأعد حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Price.Focus();
            }

            // Session price 
            if (SessionPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة سعر الجلسة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SessionPrice.Focus();
            }
            else
            {
                session_price = double.Parse(SessionPrice.Text);
            }

            // Calculate S.R Tax from Tax percentage
            //double test = ( (double)15 / (double)4);
            double SR_Tax = (Double)((tax_percentage_int / 100) * session_price);
            Tax_SR.Text = SR_Tax.ToString();

            // Calculate price before discount == (Total price with s.r Tax value)
            double price_without_dis = session_price + price_add_service + SR_Tax;
            // Check discount value for totalpricepaid 
            double result = Check_Dis_Totalprice(cureent_date, price_without_dis);
            if (result == 0 && (Discount.Text == string.Empty))
            {
                discount = 0;
            }
            if (result == 0 && (Discount.Text != string.Empty))
            {
                discount = double.Parse(Discount.Text);
            }
            if (result != 0 && (Discount.Text == string.Empty))
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("المبلغ الإجمالي للفاتورة يحقق قيمة الخصم المتاح " + result + "" + " في تاريخ اليوم هل تريد التطبيق ؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    discount = result;
                }
                else
                {
                    discount = 0;
                }
            }
            if (result != 0 && (Discount.Text != string.Empty))
            {
                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("المبلغ الإجمالي للفاتورة يحقق قيمة الخصم المتاح " + result + "" + " في تاريخ اليوم هل تريد تطبيق الخصم الذي أدخلته والغاء المتاح ؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    discount = double.Parse(Discount.Text);
                }
                else
                {
                    discount = result;
                }
            }

            // Calculate Finally Paid  Amount 
            double total_price = (double)((session_price + price_add_service + SR_Tax) - discount);
            //Assignment values 
            // Total price with s.r Tax value + without discount
            TotalPaidPrice.Text = price_without_dis.ToString();
            //Finally Paid  Amount with (s.r Tax value + discount + price of sesssion + price of added services)
            PaidAmount.Text = total_price.ToString();
        }

        //Check if Discount value for TotalPricePaid Column in Discount Table is found or not in specefic date 
        private double Check_Dis_Totalprice(string date, double totalprice)
        {
            SqlDataReader sdr = discount_adapter.Get_Dis_Date(date);
            if (sdr.HasRows == false)
            {
                //Not found Discount Value for current Date
                // MessageBox.Show("1");
                DBHandler.ConnectionDB.CloseConnection();
                return 0;
            }
            else
            {
                double dis = 0, totalpricepaid = 0;
                DataTable dt = new DataTable();
                dt.Load(sdr);
                // Start time & End time are Empty
                if (dt.Rows[0].IsNull("StartTime"))
                {
                    if (dt.Rows[0].IsNull("TotalPricePaid"))
                    {
                        //MessageBox.Show("2");
                        DBHandler.ConnectionDB.CloseConnection();
                        return 0;
                    }
                    else
                    {
                        totalpricepaid = Convert.ToSingle(dt.Rows[0].Field<double>(5));
                        if (totalprice == totalpricepaid)
                        {
                            dis = Convert.ToSingle(dt.Rows[0].Field<double>(2));
                            price_paid = totalpricepaid;
                            // MessageBox.Show("3");
                            DBHandler.ConnectionDB.CloseConnection();
                            return dis;
                        }
                        else
                        {
                            //MessageBox.Show("4");
                            DBHandler.ConnectionDB.CloseConnection();
                            return 0;
                        }

                    }
                }
                else
                {
                    if (dt.Rows[0].IsNull("TotalPricePaid"))
                    {
                        //MessageBox.Show("5");
                        DBHandler.ConnectionDB.CloseConnection();
                        return 0;
                    }
                    else
                    {
                        DataTable dt_times = new DataTable();
                        dt_times.Columns.Add("Discount_Id", typeof(int));
                        dt_times.Columns.Add("DiscountDate", typeof(DateTime));
                        dt_times.Columns.Add("DiscountRate", typeof(float));
                        dt_times.Columns.Add("StartTime", typeof(TimeSpan));
                        dt_times.Columns.Add("EndTime", typeof(TimeSpan));
                        totalpricepaid = Convert.ToSingle(dt.Rows[0].Field<double>(5));
                        dt_times.Load(sdr);

                        //Get start and end Times
                        TimeSpan start_time = dt.Rows[0].Field<TimeSpan>(3);
                        TimeSpan end_time = dt.Rows[0].Field<TimeSpan>(4);
                        //Get Current Time
                        DateTime now = DateTime.Now;
                        TimeSpan current_time = TimeSpan.Parse(now.TimeOfDay.Hours + ":" + now.TimeOfDay.Minutes + ":" + now.TimeOfDay.Seconds);
                        // Compare Between cureent time and others times
                        if (current_time.Equals(start_time) || current_time.Equals(end_time) || (current_time > start_time && current_time < end_time))
                        {
                            if (totalprice == totalpricepaid)
                            {
                                dis = Convert.ToSingle(dt.Rows[0].Field<double>(2));
                                price_paid = totalpricepaid;
                                //MessageBox.Show("6");
                                DBHandler.ConnectionDB.CloseConnection();
                                return dis;
                            }
                            else
                            {
                                // MessageBox.Show("7");
                                DBHandler.ConnectionDB.CloseConnection();
                                return 0;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("8");
                            DBHandler.ConnectionDB.CloseConnection();
                            return 0;
                        }

                    }
                }

            }
        }

        private void PrintButton_BillExtract_Click(object sender, RoutedEventArgs e)
        {
            //Assign 2 to print_iterface for printBill xml 
            // print_interface2 = 2;
            //  bill_no not empity 
            if (BillNumField.Text != string.Empty)
            {
                //p_name = 
                file_no = BillMgFileNumField.Text;
                bill_date = BillDateField.Text;
                bill_no = BillNumField.Text;

                if (Paid_Type1.IsChecked == true)
                {
                    bill_paid_type = "نقدي";
                }
                if (Paid_Type2.IsChecked == true)
                {
                    bill_paid_type = "شبكة";
                }
                if (Paid_Type3.IsChecked == true)
                {
                    bill_paid_type = "نقدي وشبكة";
                    cash_paid = CashTotalPaidPrice.Text;
                    network_paid = NetworkTotalPaidPrice.Text;
                }

                //percentage tax value
                if (Tax.Text != string.Empty)
                {
                    bill_tax = Tax.Text;
                }
                //S.R tax value
                if (Tax_SR.Text != string.Empty)
                {
                    bill_tax_SR = Tax_SR.Text;
                }
                //Discount value
                if (Discount.Text != string.Empty)
                {
                    bill_discount = Discount.Text;
                }

                session_price = SessionPrice.Text;
                totalpaid = TotalPaidPrice.Text;
                paid_amount = PaidAmount.Text;
                paid_value_patient = PaidValue.Text;
                remaining = RemainingUnPaid.Text;
                if (paid_value_patient != string.Empty && remaining != string.Empty)
                {
                    double total = double.Parse(paid_value_patient);
                    //حقل المتبقي
                    double re = double.Parse(remaining);
                    if (re > total)
                    {
                        MessageBox.Show("فضلاً أدخل قيمة صحيحة في حقل المتبقي لايمكن أن تكون أكبر من المبلغ المدفوع ! ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        RemainingUnPaid.Focus();
                    }
                    else
                    {
                        PrintBillsMgView print = new PrintBillsMgView();
                        print.Show();
                    }
                }
                else
                {
                    PrintBillsMgView print = new PrintBillsMgView();
                    print.Show();
                }
            }
            else
            {
                MessageBox.Show("فضلاً حدد سجل فاتورة من الجدول قبل معاينة الطابعة ! ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //Load Data 
        private void LoadPatient_BillMg(SqlDataReader sdr)
        {
            DataTable dt = new DataTable();
            dt.Load(sdr);
            DataViewFromPatientTable_BillMg.ItemsSource = dt.DefaultView;
        }

        private void LoadBills_Patient(string filenum)
        {
            SqlDataReader sdr_bill = bill_adapter.Get_BillsExtract(filenum);
            if (sdr_bill.HasRows == false)
            {
                MessageBox.Show("لا يوجد فواتير مصدرة بعد لهذا الملف !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DataGv_BillMg.ItemsSource = null;
                DBHandler.ConnectionDB.CloseConnection();
                return;
            }
            else
            {
                DBHandler.ConnectionDB.CloseConnection();
                // Not used Merg between Session and Bill
                /* SqlDataReader sdr_session = session_adapter.Get_SessionNum_BillMg(filenum);
                 DataTable dt_session = new DataTable("Session");
                // dt_session.Columns.Add("SessionId");
                 dt_session.Columns.Add("SessionNum");
                 dt_session.PrimaryKey = new DataColumn[] { dt_session.Columns["SessionId"] };
                 dt_session.Load(sdr_session);
                 DBHandler.ConnectionDB.CloseConnection();
                */
                sdr_bill = bill_adapter.Get_BillsExtract(filenum);
                DataTable dt_BillMg = new DataTable("Bill");
                /*dt_session.Columns.Add("BillId");
                dt_session.Columns.Add("BillDate");
                dt_session.Columns.Add("PaymentChipType");
                dt_session.Columns.Add("AdditionalServices");
                dt_session.Columns.Add("PriceOfAdditionalServices");
                dt_session.Columns.Add("Nationality");
                dt_session.Columns.Add("Tax");
                dt_session.Columns.Add("Discount");
                dt_session.Columns.Add("PaymentType");
                dt_session.Columns.Add("TotalPricePaid");
                dt_session.Columns.Add("AmountRemainingUnpaid");
                dt_BillMg.PrimaryKey = new DataColumn[] { dt_BillMg.Columns["SessionId"] };

                dt_BillMg.Load(sdr_bill);
                DBHandler.ConnectionDB.CloseConnection();
                // Merg Tow DataTable in one for this GridView
                dt_session.Merge(dt_BillMg, false, MissingSchemaAction.Ignore);
                DataGv_BillMg.ItemsSource = dt_session.DefaultView;
                */
                dt_BillMg.Load(sdr_bill);
                DataGv_BillMg.ItemsSource = dt_BillMg.DefaultView;
                DBHandler.ConnectionDB.CloseConnection();
            }
        }

        //Restrict Input Data in TextBoxes (Discount - Tax - TotalPaidPrice - Price - RemainingUnPaid) 
        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //Recognize Digites + dot character and  Comma character from others Characters (Decimal Input)
            Char ch = e.Text[0];
            String TextBoxValue = (((TextBox)e.Source).Text).ToString();
            if (Char.IsDigit(ch) || ch == '.' || ch == ',')
            {
                if ((ch == '.' && TextBoxValue.Contains('.')))
                {
                    MessageBox.Show("ادخال غير صحيح", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
                if (ch == ',')
                {
                    MessageBox.Show("لايسمح بالفاصلة ", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
            }
            else
            {
                MessageBox.Show("يسمح بادخال الأرقام", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                e.Handled = true;
            }
        }

        //Click on Bill Record
        private void DataGv_BillMg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cell0 = DataGv_BillMg.SelectedCells[0];
                var cell1 = DataGv_BillMg.SelectedCells[1];
                var cell2 = DataGv_BillMg.SelectedCells[2];
                var cell3 = DataGv_BillMg.SelectedCells[3];
                var cell4 = DataGv_BillMg.SelectedCells[4];
                var cell5 = DataGv_BillMg.SelectedCells[5];
                var cell6 = DataGv_BillMg.SelectedCells[6];
                var cell7 = DataGv_BillMg.SelectedCells[7];
                var cell8 = DataGv_BillMg.SelectedCells[8];
                var cell9 = DataGv_BillMg.SelectedCells[9];
                var cell10 = DataGv_BillMg.SelectedCells[10];
                var cell11 = DataGv_BillMg.SelectedCells[11];
                var cell12 = DataGv_BillMg.SelectedCells[12];
                var cell13 = DataGv_BillMg.SelectedCells[13];
                var cell14 = DataGv_BillMg.SelectedCells[14];
                var cell15 = DataGv_BillMg.SelectedCells[15];
                var cell16 = DataGv_BillMg.SelectedCells[16];

                string bill_id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                string bill_date = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
                string add_serv = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
                string price_add_serv = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
                string nationality = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
                string tax_percentage = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
                string session_p = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
                string sr_tax = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;
                string total_price = (cell8.Column.GetCellContent(cell8.Item) as TextBlock).Text;
                string discount = (cell9.Column.GetCellContent(cell9.Item) as TextBlock).Text;
                string payment_type = (cell10.Column.GetCellContent(cell10.Item) as TextBlock).Text;
                string cash_paid = (cell11.Column.GetCellContent(cell11.Item) as TextBlock).Text;
                string network_paid = (cell12.Column.GetCellContent(cell12.Item) as TextBlock).Text;
                string paid_amount = (cell13.Column.GetCellContent(cell13.Item) as TextBlock).Text;
                string paid_value = (cell14.Column.GetCellContent(cell14.Item) as TextBlock).Text;
                string remaining = (cell15.Column.GetCellContent(cell15.Item) as TextBlock).Text;
                string s_id_bill = (cell16.Column.GetCellContent(cell16.Item) as TextBlock).Text;

                Clear_BillMg();

                //Get session_id according to bill_id from bill Table
                int sdr_session_id = bill_adapter.Get_BillExtract_Sid(bill_id);
                // This step Totaly Not used
                if (sdr_session_id == 0)
                {
                    MessageBox.Show("لا تستطيع اتمام أي عملية ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    // Get Session_num and Session_id for this bill and filenum
                    SqlDataReader sdr_session = session_adapter.Get_SessionNum_BillMg(filenum, sdr_session_id.ToString());
                    DataTable dt_session = new DataTable("Session");
                    dt_session.Load(sdr_session);
                    DBHandler.ConnectionDB.CloseConnection();
                    session_id_bill = sdr_session_id.ToString();
                    BillMgSNumField.Text = dt_session.Rows[0].Field<int>(0).ToString();

                    BillMgFileNumField.Text = filenum;
                    BillMgNumField.Text = bill_id;
                    BillNumField.Text = bill_id;
                    BillDateField.Text = bill_date;
                    /*
                    if (payment_chip.Equals("50"))
                        PaidChip_Type1.IsChecked = true;
                    if (payment_chip.Equals("100"))
                        PaidChip_Type2.IsChecked = true;
                    if (payment_chip.Equals("150"))
                        PaidChip_Type3.IsChecked = true;
                    if (payment_chip.Equals("200"))
                        PaidChip_Type4.IsChecked = true;
                    if (payment_chip.Equals("250"))
                        PaidChip_Type5.IsChecked = true;
                    if (payment_chip.Equals("300"))
                        PaidChip_Type6.IsChecked = true;
                    if (payment_chip.Equals("350"))
                        PaidChip_Type7.IsChecked = true;
                    */
                    if (add_serv != string.Empty)
                    {
                        AddionalServices.Text = add_serv;
                        Price.Text = price_add_serv;
                    }

                    if (nationality.Equals("سعودي"))
                        Na_Type1.IsChecked = true;
                    else
                        Na_Type2.IsChecked = true;

                    //Tax percentage
                    if (tax_percentage != string.Empty)
                        Tax.Text = tax_percentage;
                    else
                        Tax.Text = "0%";
                    //SR tax
                    if (sr_tax != string.Empty)
                        Tax_SR.Text = sr_tax;

                    if (discount != string.Empty)
                        Discount.Text = discount;

                    if (payment_type.Equals("نقدي"))
                        Paid_Type1.IsChecked = true;
                    if (payment_type.Equals("شبكة "))
                        Paid_Type2.IsChecked = true;
                    if (payment_type.Equals("نقدي وشبكة "))
                    {
                        Paid_Type3.IsChecked = true;
                        CashTotalPaidPrice.Text = cash_paid;
                        NetworkTotalPaidPrice.Text = network_paid;
                    }
                    SessionPrice.Text = session_p;
                    TotalPaidPrice.Text = total_price;
                    PaidAmount.Text = paid_amount;
                    PaidValue.Text = paid_value;
                    RemainingUnPaid.Text = remaining;
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        //Click on patient Record
        private void DataViewFromPatientTable_BillMg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Get FileNum  from Grid View To TextBox
                var cell0 = DataViewFromPatientTable_BillMg.SelectedCells[0];
                filenum = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                // Get Patient name from Grid View To TextBox
                var cell2 = DataViewFromPatientTable_BillMg.SelectedCells[2];
                p_name = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;

                BillMgFileNumField.Text = filenum;
                DataViewFromPatientTable_BillMg.Visibility = Visibility.Hidden;
                DataViewFromPatientTable_BillMg.ItemsSource = null;
                DataGv_BillMg.Visibility = Visibility.Visible;
                LoadBills_Patient(filenum);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("فضلا اضغط على أحد السجلات مرتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void BillMgFNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SPFileNum_BillMg.Text = "أدخل رقم الملف";
        }
        private void BillMgMNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SMNum_BillMg.Text = "أدخل رقم الجوال";
        }

        // Only number input field function
        private void NumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        // Search Functions 
        private void SPFileNum_BillMg_Button_Click(object sender, RoutedEventArgs e)
        {
            DataViewFromPatientTable_BillMg.Visibility = Visibility.Visible;
            DataGv_BillMg.Visibility = Visibility.Hidden;
            DataViewFromPatientTable_BillMg.ItemsSource = null;
            DataGv_BillMg.ItemsSource = null;

            Clear_BillMg();

            if (SPFileNum_BillMg.Text != string.Empty && SPFileNum_BillMg.Text != "أدخل رقم الملف")
            {

                SqlDataReader sdr = patient_adapter.Get_Patient_Filenum(SPFileNum_BillMg.Text);
                if (sdr.HasRows)
                {
                    LoadPatient_BillMg(sdr);
                    SPFileNum_BillMg.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة رقم الملف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SPFileNum_BillMg.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل رقم الملف  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void SMNum_BillMg_Button_Click(object sender, RoutedEventArgs e)
        {
            DataViewFromPatientTable_BillMg.Visibility = Visibility.Visible;
            DataGv_BillMg.Visibility = Visibility.Hidden;
            DataViewFromPatientTable_BillMg.ItemsSource = null;
            DataGv_BillMg.ItemsSource = null;

            Clear_BillMg();

            if (SMNum_BillMg.Text != string.Empty && SMNum_BillMg.Text != "أدخل رقم الجوال")
            {
                SqlDataReader sdr = patient_adapter.Get_Patient_Contactnum(SMNum_BillMg.Text);
                if (sdr.HasRows)
                {
                    LoadPatient_BillMg(sdr);
                    SMNum_BillMg.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة رقم الجوال", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SMNum_BillMg.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل رقم الجوال  فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //Function For validation 
        private bool Bill_IsValid()
        {
            if (BillDateField.Text == string.Empty)
            {
                MessageBox.Show("  حقل التاريخ اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                BillDateField.Focus();
                return false;
            }

            // Not Used Currently
            /* if (PaidChip_Type1.IsChecked == false && PaidChip_Type2.IsChecked == false && PaidChip_Type3.IsChecked == false && PaidChip_Type4.IsChecked == false && PaidChip_Type5.IsChecked == false && PaidChip_Type6.IsChecked == false && PaidChip_Type7.IsChecked == false)
             {
                 MessageBox.Show("  حدد نوع شريحة الدفع لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                 return false;
             }*/

            if (Na_Type1.IsChecked == false && Na_Type2.IsChecked == false)
            {
                MessageBox.Show("  حدد الجنسية لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }

            //OnlyCash - Only Network - togather
            if (Paid_Type1.IsChecked == false && Paid_Type2.IsChecked == false && Paid_Type3.IsChecked == false)
            {
                MessageBox.Show("  حدد طريقة الدفع لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }

            // if Method of payment is togather => Tow boxes visibility 
            if (Paid_Type3.IsChecked == true && CashTotalPaidPrice.Text == string.Empty && NetworkTotalPaidPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقلي قيمة النقدي والشبكة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                CashTotalPaidPrice.Focus();
                return false;
            }
            if (Paid_Type3.IsChecked == true && CashTotalPaidPrice.Text == string.Empty && NetworkTotalPaidPrice.Text != string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة النقدي المدفوع", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                CashTotalPaidPrice.Focus();
                return false;
            }
            if (Paid_Type3.IsChecked == true && CashTotalPaidPrice.Text != string.Empty && NetworkTotalPaidPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة الشبكة المدفوع", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                NetworkTotalPaidPrice.Focus();
                return false;
            }


            if (AddionalServices.Text != string.Empty && Price.Text == string.Empty)
            {
                MessageBox.Show("حقل السعر لايمكن أن يكون فارغ عند ملئ الخدمات الإضافية ! أدخل السعر وأعد حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Price.Focus();
                return false;
            }

            if (AddionalServices.Text == string.Empty && Price.Text != string.Empty)
            {
                MessageBox.Show("حقل الخدمات الإضافية لايمكن أن يكون فارغ عند ملئ  السعر المقابل له! أدخل الخدمات الاضافية المقدمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                AddionalServices.Focus();
                return false;
            }

            if (Na_Type2.IsChecked == true && Tax.Text == string.Empty)
            {
                MessageBox.Show("  حقل الضريبة اجباري عند الجنسية غير سعودي !أدخل الضريبة من إعدادات الضريبة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Tax.Focus();
                return false;
            }

            if (Na_Type1.IsChecked == true && Tax.Text == string.Empty)
            {
                MessageBox.Show("  حقل الضريبة اجباري عند الجنسية  سعودي !أدخل الضريبة من إعدادات الضريبة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Tax.Focus();
                return false;
            }

            // Session price 
            if (SessionPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة سعر الجلسة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SessionPrice.Focus();
                return false;
            }

            if (Tax.Text == string.Empty)
            {
                MessageBox.Show(" حقل الضريبة بالنسبة المئوية اجباري ! أدخل الضريبة من إعدادات الضريبة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Tax.Focus();
                return false;
            }

            if (Tax_SR.Text == string.Empty)
            {
                MessageBox.Show(" حقل ضريبة القيمة المضافة ( ريال) اجباري ! أكمل عملية حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Tax_SR.Focus();
                return false;
            }


            if (TotalPaidPrice.Text == string.Empty)
            {
                MessageBox.Show(" حقل السعر الإجمالي المدفوع اجباري ! أكمل عملية حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                TotalPaidPrice.Focus();
                return false;
            }

            if (PaidAmount.Text == string.Empty)
            {
                MessageBox.Show(" حقل المبلغ الصافي المدفوع اجباري ! أكمل عملية حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PaidAmount.Focus();
                return false;
            }

            if (PaidValue.Text == string.Empty)
            {
                MessageBox.Show("  حقل المدفوع اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PaidValue.Focus();
                return false;
            }

            if (RemainingUnPaid.Text == string.Empty)
            {
                MessageBox.Show("  حقل المتبقي اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                RemainingUnPaid.Focus();
                return false;
            }

            // Check PaidValue & RemainingUnPaid
            if (PaidValue.Text != string.Empty && RemainingUnPaid.Text != string.Empty)
            {
                double total = double.Parse(PaidValue.Text);
                //حقل المتبقي
                double re = double.Parse(RemainingUnPaid.Text);
                if (re > total)
                {
                    MessageBox.Show("فضلاً أدخل قيمة صحيحة في حقل المتبقي لايمكن أن تكون أكبر من المبلغ المدفوع ! ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    RemainingUnPaid.Focus();
                    return false;
                }
            }
            return true;
        }

        // Function For Clear Fields in bill window
        private void Clear_BillMg()
        {

            // Clear All Fileds
            BillMgFileNumField.Clear();
            BillMgSNumField.Clear();
            BillMgNumField.Clear();
            BillNumField.Clear();
            BillDateField.SelectedDate = null;
            AddionalServices.Clear();
            Price.Clear();
            Na_Type1.IsChecked = false;
            Na_Type2.IsChecked = false;
            Tax.Clear();
            Tax_SR.Clear();
            Discount.Clear();
            Paid_Type1.IsChecked = false;
            Paid_Type2.IsChecked = false;
            Paid_Type3.IsChecked = false;
            CashTotalPaidPrice.Clear();
            NetworkTotalPaidPrice.Clear();
            SessionPrice.Clear();
            TotalPaidPrice.Clear();
            PaidAmount.Clear();
            PaidValue.Clear();
            RemainingUnPaid.Clear();
        }
    }
}
