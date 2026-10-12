using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using ZatcaIntegrationSDK;
using ZatcaIntegrationSDK.APIHelper;
using ZatcaIntegrationSDK.BLL;
using ZatcaIntegrationSDK.HelperContracts;
using ZXing;
using ZXing.Common;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for BillExtractWindow.xaml
    /// </summary>
    public partial class BillExtractWindow : Window
    {
        //Intialize & Assign 0 to print_iterface1 for printBill xml 
        //public static int print_interface1 = 0;
        public static bool open = false;
        DBHandler.BillAdapter bill_adapter = new DBHandler.BillAdapter();
        DBHandler.TaxAdapter tax_adapter = new DBHandler.TaxAdapter();
        DBHandler.DiscountsAdapter discount_adapter = new DBHandler.DiscountsAdapter();
        // Get Current Date 
        DateTime now;
        string cureent_date;
        string session_id;
        // intialize variable for discount whith TotalPricePaid 
        double price_paid = 0;
        //Intialize all elements in Bill View for printing
        public static string p_name = "", file_no = "", bill_no = "", bill_date = "", bill_tax = "", bill_discount = "", totalpaid = "", remaining = "", residu = "", bill_paid_type = "", 
            username = "", cash_paid = "", network_paid = "", bill_tax_SR = "", session_price = "", paid_amount = "" , paid_value_patient = "";
        public static Bitmap imageBlock = null;

        //for datebill according to Qr code
        public static DateTime dateTime;
        public static string sessionNum;
        // intialize variable for zatca
        private Mode mode = Mode.Simulation;
        public string zatca_discount;
        public string zatca_tax_percentage;
        public string zatca_sr_tax;
        public BillExtractWindow()
        {
            InitializeComponent();
            session_id = PatientsSessionView.s_id;
            now = DateTime.Now;
            cureent_date = now.Day + "/" + now.Month + "/" + now.Year;
            PaymentTogether.Visibility = Visibility.Hidden;
        }


        //Dragable Window
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

        //Functions for Clearing values (paidvalue - tax SR - Price with tax - finally paid amount - Remaining ) when change Sprice + Discount + Nationality 
        private void Na_Type1_Checked(object sender, RoutedEventArgs e)
        {
            //(تعديل جديد) SessionPrice == السعر قبل الضريبة
            SessionPrice.Clear();
            Tax_SR.Clear();
            TotalPaidPrice.Clear();
            PaidAmount.Clear();
            PaidValue.Clear();
            RemainingUnPaid.Clear();
        }

        private void Na_Type2_Checked(object sender, RoutedEventArgs e)
        {
            //(تعديل جديد) SessionPrice == السعر قبل الضريبة
            SessionPrice.Clear();
            Tax_SR.Clear();
            TotalPaidPrice.Clear();
            PaidAmount.Clear();
            PaidValue.Clear();
            RemainingUnPaid.Clear();
        }

        private void bat_SessionPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Tax_SR.Clear();
            //(تعديل جديد) SessionPrice == السعر قبل الضريبة
            SessionPrice.Clear();
            TotalPaidPrice.Clear();
            PaidAmount.Clear();
            PaidValue.Clear();
            RemainingUnPaid.Clear();
            CashTotalPaidPrice.Clear();
            NetworkTotalPaidPrice.Clear();
            if (bat_SessionPrice.Text == string.Empty)
                Tax_SR.Clear();
        }

        private void Discount_TextChanged(object sender, TextChangedEventArgs e)
        {
            PaidAmount.Clear();
            PaidValue.Clear();
            RemainingUnPaid.Clear();
        }


        //Get Current Date and Time  
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Check_BillExtract_Info();
            DefaultData();
            Check_Discount(cureent_date);
        }

        // Exit From this window
        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            open = true;
        }

        private void DefaultData()
        {
            BillDateField.SelectedDate = DateTime.Parse(PatientsSessionView.session_date);
            BillTimeField.SelectedTime = DateTime.Now;
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            SendBillToZatca(zatca_discount,TotalPaidPrice.Text);
        }

        // Functions For Handler in View 5 functions

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
                   MessageBox.Show("ادخال غير صحيح","تحذير",MessageBoxButton.OK,MessageBoxImage.Warning,MessageBoxResult.OK,MessageBoxOptions.RtlReading);
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
                
                MessageBox.Show("يسمح بادخال الأرقام","تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                e.Handled = true;
            }
        }

        private void Check_BillExtract_Info()
        {
            PatientFileNumField.Text = PatientsSessionView.FileNum;
            SessionNumField.Text = PatientsSessionView.s_num;
            // Get Data To this window
            Get_BillExtractValues();
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

        //Function for calculiting Total paid amount (النموذج الأول شامل الضريبة)
        private void TotalBillPriceButton_BillExtract_Click(object sender, RoutedEventArgs e) 
        {
            double price_add_service = 0, discount = 0, session_price = 0, tax_percentage_int = 0, total_before_tax = 0;

            //price add service
            if (Price.Text != string.Empty)
            {
                price_add_service = double.Parse(Price.Text);
            }

            //check Addtional servicess & price 
            if (AddionalServices.Text != string.Empty && Price.Text == string.Empty)
            {
                MessageBox.Show("حقل السعر لايمكن أن يكون فارغ عند ملئ الخدمات الإضافية ! أدخل السعر وأعد حساب السعر الإجمالي", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Price.Focus();
            }

            // (تعديل جديد) Session price ==  bat_SessionPrice
            if (bat_SessionPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة سعر الجلسة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                bat_SessionPrice.Focus();
            }
            else
            {
                session_price = double.Parse(bat_SessionPrice.Text);
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
                    zatca_tax_percentage = tax_percentage_int.ToString();
                }
                else
                {
                    MessageBox.Show("أدخل حقل الضريبة من اعدادات الضريبة قد تكون القيمة صفر بالمئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Tax.Focus();
                }

                //step 1 : Combine the price of services with the session
                double service_price_with_session = price_add_service + session_price;
                // Saudi => There is no (/ 1.15) , otherwise there is (/ 1.15)  , (1.15 = Tax)
                if (Na_Type1.IsChecked == true)
                {
                    {   //step 2 : Total ( services with the session) before tax
                        total_before_tax = Math.Round(service_price_with_session,2);
                    }
                }
                else
                {  //step 2 : Total ( services with the session) before tax
                    total_before_tax = Math.Round((service_price_with_session / 1.15),2);
                }
            }
            //step 3 : Calculate S.R Tax from Tax percentage
            double SR_Tax = Math.Truncate((tax_percentage_int / 100 * total_before_tax) * 100)/100;
            // step 4 : assign to  Tax_SR & Other fields
            Tax_SR.Text = SR_Tax.ToString();
            SessionPrice.Text = total_before_tax.ToString();
            zatca_sr_tax = SR_Tax.ToString();
            // step 5 : Calculate price before discount == (Total price with s.r Tax value)
            double price_without_dis = total_before_tax + SR_Tax;
            //  step 6 :Check discount value for totalpricepaid 
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
            //step 7 :  Assignment values 
            //Calculate Finally Paid  Amount 
            double total_price = Math.Round(price_without_dis - discount, 2);
            zatca_discount = discount.ToString();
            // Total price with s.r Tax value + without discount
            TotalPaidPrice.Text = price_without_dis.ToString();
            //Finally Paid  Amount with (s.r Tax value + discount + price of sesssion + price of added services)
            PaidAmount.Text = total_price.ToString();
        }

        //Function insert new Bill record to Bill Table
        private void AddButton_BillExtract_Click(object sender, RoutedEventArgs e)
        {
            if(Bill_IsValid())
            {

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
                bool result = bill_adapter.Insert_Bill(BillDateField.Text, chip_type, AddionalServices.Text, Price.Text, nationality_type, Tax_SR.Text, Tax.Text, Discount.Text, paid_type, 
                    SessionPrice.Text,TotalPaidPrice.Text,CashTotalPaidPrice.Text,NetworkTotalPaidPrice.Text,PaidAmount.Text,PaidValue.Text,RemainingUnPaid.Text, Sign_in.User, 
                    PatientFileNumField.Text, session_id);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    AddButton_BillExtract.IsEnabled = false;
                    TotalBillPriceButton_BillExtract.IsEnabled = false;
                    bill_no = bill_adapter.Get_LastRecord().ToString();
                    BillNumField.Text = bill_no;
                    // call function SendBillToZatca 
                    SendBillToZatca(zatca_discount, TotalPaidPrice.Text);
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }

            }
        }

        //Function for Integration with Zatca
        private void SendBillToZatca(string discount, string TotalPaidPrice)
        {
            UBLXML ubl = new UBLXML();
            Invoice inv = new Invoice();
            ZatcaIntegrationSDK.Result res = new ZatcaIntegrationSDK.Result();
            Zatca_Bill.InvoiceItems item = new Zatca_Bill.InvoiceItems();

            inv.ID = BillNumField.Text; // invoice number in your system example= SME00010
            //inv.UUID = Guid.NewGuid().ToString();
            DateTime? selectedDate = BillDateField.SelectedDate;
            inv.IssueDate = selectedDate.Value.ToString("yyyy-MM-dd");
            //inv.IssueDate = DateTimePicker1.Value.ToString("yyyy-MM-dd"); //invoice issue date with format yyyy-MM-dd example "2023-02-07"
            DateTime? selectedTime = BillTimeField.SelectedTime;
            inv.IssueTime = selectedTime.Value.ToString("HH:mm:ss"); 

            //Fill InvoiceItems Fields with Data 
            item.ProductQuantity = 1;
            item.ProductName = "جلسة حجامة";
            //item.VatPercentage = Convert.ToDecimal(Tax.Text);
            decimal.TryParse(zatca_tax_percentage, out decimal vatValue);
            if (vatValue >= 0)
            {
                item.VatPercentage = vatValue;
            }
            else
            {
                MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة للضريبة");
                item.VatPercentage = 0; // قيمة افتراضية
            }
            //item.ProductPrice = Convert.ToDecimal(SessionPrice.Text); 
            decimal.TryParse(SessionPrice.Text, out decimal sessionValue);
            if (sessionValue > 0) //Total ( services with the session) before tax ????
            {
                item.ProductPrice = sessionValue;
            }
            else
            {
                MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة للجلسة");
                item.ProductPrice = 0; // قيمة افتراضية
            }
            //item.DiscountValue = Convert.ToDecimal(discount);
            decimal.TryParse(discount, out decimal discountValue);
            if (discountValue >= 0)
            {
                item.DiscountValue = discountValue;
            }
            else
            {
                MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة للتخفيض");
                item.DiscountValue = 0; // قيمة افتراضية
            }

            //all needed codes for invoiceTypeCode id ????????????????? هل هذه الأسطر لازمة
            inv.invoiceTypeCode.id = GetInvoiceType();
            inv.invoiceTypeCode.Name = GetInvoiceTypeName();
            inv.DocumentCurrencyCode = "SAR"; // Document Currency Code (invoice currency example SAR or USD) 
            inv.TaxCurrencyCode = "SAR"; // Tax Currency Code it must be with SAR
            inv.CurrencyRate = 1m; // incase of DocumentCurrencyCode equal any currency code not SAR we must mention CurrencyRate value
            if (inv.invoiceTypeCode.id == 383 || inv.invoiceTypeCode.id == 381) // ممكن حذفه
            {
                // فى حالة ان اشعار دائن او مدين فقط هانكتب رقم الفاتورة اللى اصدرنا الاشعار ليها
                // in case of return sales invoice or debit notes we must mention the original sales invoice number
                InvoiceDocumentReference invoiceDocumentReference = new InvoiceDocumentReference();
                invoiceDocumentReference.ID = "from invoice number 500 to invoice number 505"; // mandatory in case of return sales invoice or debit notes
                inv.billingReference.invoiceDocumentReferences.Add(invoiceDocumentReference);
            }

            //select from ZatcaCSID table
            //LastInvoiceHash
            //invoicecounter=(invoicecounter)from table +1
            string LastInvoiceHash = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";
            long invoicecounter = 1;
            inv.AdditionalDocumentReferencePIH.EmbeddedDocumentBinaryObject = LastInvoiceHash;
            // قيمة عداد الفاتورة
            // Invoice counter (1,2,3,4) this counter must start from 1 for each CSID

            inv.AdditionalDocumentReferenceICV.UUID = invoicecounter; // لابد ان يكون ارقام فقط must be numbers only

            //هل هذه الأسطر لازمة ؟؟؟؟؟؟؟؟ // ممكن حذفه 
            if (inv.invoiceTypeCode.Name.Substring(0, 2) == "01")
            {
                //supply date mandatory only for standard invoices
                // فى حالة فاتورة مبسطة وفاتورة ملخصة هانكتب تاريخ التسليم واخر تاريخ التسليم
                inv.delivery.ActualDeliveryDate = "2022-10-22";
                inv.delivery.LatestDeliveryDate = "2022-10-23";
            }

            // payment methods mandatory for return invoice and debit notes and optional for invoices ؟؟؟؟؟؟
            // بيانات البائع 
            //other identifier for seller like commercial registration number
            inv.SupplierParty.partyIdentification.ID = "7023297117"; // رقم السجل التجارى الخاض بالبائع
            //other identifier scheme id example CRN for commercial registration number 
            inv.SupplierParty.partyIdentification.schemeID = "CRN"; // رقم السجل التجارى
            inv.SupplierParty.partyTaxScheme.CompanyID = "314340128600003";  // رقم التسجيل الضريبي
            //seller street name mandatory
            inv.SupplierParty.postalAddress.StreetName = "Airport"; // اجبارى
            inv.SupplierParty.postalAddress.BuildingNumber = "8833"; // اجبارى رقم المبنى
            inv.SupplierParty.postalAddress.CityName = "AT TAIF"; // اسم المدينة
            //seller postal zone must be 5 digits 
            inv.SupplierParty.postalAddress.PostalZone = "26523"; // الرقم البريدي
                                                                  //seller City Subdivision Name
            inv.SupplierParty.postalAddress.CitySubdivisionName = "Jabrah Dist"; // اسم المنطقة او الحى 
            //SA for Saudi it must be SA with seller data
            inv.SupplierParty.postalAddress.country.IdentificationCode = "SA";
            // seller company name
            inv.SupplierParty.partyLegalEntity.RegistrationName = "مركز لمسة شفاء للحجامة"; // اسم الشركة المسجل فى الهيئة

            if (Na_Type1.IsChecked.GetValueOrDefault())
            {
                // بيانات المشترى
                inv.CustomerParty.partyIdentification.ID = "123123"; // رقم الهوية للمواطن السعودى
                inv.CustomerParty.partyIdentification.schemeID = "NAT"; //كود رقم الهوية

                inv.CustomerParty.postalAddress.country.IdentificationCode = "SA";
                inv.CustomerParty.partyLegalEntity.RegistrationName = "اسم المواطن"; // اسم المواطن السعودى
            }
            else
            {
                // بيانات المشترى
                inv.CustomerParty.partyIdentification.ID = "123123"; // رقم الاقامة للمواطن غير السعودى
                inv.CustomerParty.partyIdentification.schemeID = "IQA"; //كود رقم الهوية

                inv.CustomerParty.postalAddress.country.IdentificationCode = "SA";
                inv.CustomerParty.partyLegalEntity.RegistrationName = "اسم المقيم"; // اسم المقيم
            }


            // التعامل مع التخفيض على مستوى الفاتورة وليس الصنف
            decimal.TryParse(discount, out decimal invoicediscount);
            if (invoicediscount > 0)
            {
                //this code incase of there is a discount in invoice level 
                AllowanceCharge allowance = new AllowanceCharge();
                //ChargeIndicator = false means that this is discount
                //ChargeIndicator = true means that this is charges(like cleaning service - transportation)
                allowance.ChargeIndicator = false;
                //write this lines in case you will make discount as percentage  ???????
                //allowance.MultiplierFactorNumeric = 0; //dscount percentage like 10
                //allowance.BaseAmount = 0; // the amount we will apply percentage on example (MultiplierFactorNumeric=10 ,BaseAmount=1000 then AllowanceAmount will be 100 SAR)

                // in case we will make discount as Amount 
                allowance.Amount = invoicediscount; // 
                                                    // allowance.AllowanceChargeReasonCode = "95"; //discount or charge reason code
                                                    // allowance.AllowanceChargeReason = "discount"; //discount or charge reson
                allowance.taxCategory.ID = "O";// كود الضريبة tax code (S Z O E ) ????????????????
                //allowance.taxCategory.Percent = 15;// نسبة الضريبة tax percentage (0 - 15 - 5 ) ????
                allowance.taxCategory.Percent = 0;// نسبة الضريبة tax percentage (0 - 15 - 5 ) 
                allowance.taxCategory.TaxExemptionReasonCode = "VATEX-SA-OOS";
                allowance.taxCategory.TaxExemptionReason = "Service outside scope of tax";
                inv.allowanceCharges.Add(allowance);
            }

            Decimal.TryParse(TotalPaidPrice, out decimal payableamount); // (TotalPaidPrice = invoice total with vat without Discount ) المبلغ الصافي 
            //this is the invoice total amount (invoice total with vat) and you can set its value with Zero and i will calculate it from sdk
            inv.legalMonetaryTotal.PayableAmount = payableamount;

            // One invoice line data
            InvoiceLine invline = new InvoiceLine();
            //Product Quantity  ????
            invline.InvoiceQuantity = item.ProductQuantity;
            //Product Name  ?????
            invline.item.Name = item.ProductName;

            if (item.VatPercentage == 0)
            {
                //item Tax code
                invline.item.classifiedTaxCategory.ID = "Z"; // كود الضريبة
                                                             //item Tax code
                invline.taxTotal.TaxSubtotal.taxCategory.ID = "Z"; // كود الضريبة
                                                                   //item Tax Exemption Reason Code mentioned in zatca pdf page(32-33)
                                                                   //"VATEX-SA-35", "Medicines and medical equipment"
                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReasonCode = "VATEX-SA-HEA"; // كود الضريبة
                                                                                                  
                invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReason = "Private healthcare to citizen"; // كود الضريبة

            }
            else
            {
                //item Tax code
                invline.item.classifiedTaxCategory.ID = "S"; // كود الضريبة
                                                             //item Tax code
                invline.taxTotal.TaxSubtotal.taxCategory.ID = "S"; // كود الضريبة
            }
            //// ???? من كود دالة  GetAdditionalInvoiceLine
            //invline.taxTotal.TaxSubtotal.TaxableAmount = 10;
            //invline.taxTotal.TaxSubtotal.TaxAmount = 1.5m;

            //item Tax percentage
            invline.item.classifiedTaxCategory.Percent = item.VatPercentage; // نسبة الضريبة
            invline.taxTotal.TaxSubtotal.taxCategory.Percent = item.VatPercentage; // نسبة الضريبة
            //EncludingVat = false this flag will be false in case you will give me Product Price not including vat
            //EncludingVat = true this flag will be true in case you will give me Product Price including vat
            invline.price.EncludingVat = false;
            //Product Price
            invline.price.PriceAmount = item.ProductPrice;
            //item.VatValue
            decimal.TryParse(zatca_sr_tax, out decimal srTaxValue);
            //if (srTaxValue >= 0)
            //{
            //    item.VatValue = srTaxValue;
            //    invline.taxTotal.TaxAmount = item.VatValue;
            //}
            //else
            //{
            //    MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة لضريبة القيمة المضافة ريال");
            //    item.VatValue = 0; // قيمة افتراضية
            //    invline.taxTotal.TaxAmount = item.VatValue;
            //}

            //item.TotalPrice
            //decimal.TryParse(item.ProductPrice.ToString(), out decimal TotalPriceVat);
            //item.TotalPrice = TotalPriceVat;

            ////item.TotalWithVat
            //decimal.TryParse(TotalPaidPrice, out decimal TotalWithVat);
            //item.TotalWithVat = TotalWithVat;
            //invline.taxTotal.RoundingAmount = item.TotalWithVat;

            ////item.TotalPriceAfterDiscount , المبلغ الصافي = المجموع شامل الضريبة - التخفيض ؟؟؟؟؟
            //decimal.TryParse(PaidAmount.Text, out decimal TotalPriceAfterDiscount);
            //item.TotalPriceAfterDiscount = TotalPriceAfterDiscount;

            //Invoice ID
            //invline.ID = inv.ID;

            // تم التعامل مع التخفيض ضمن الدالة مرتين ؟؟؟؟؟؟
            //if (item.DiscountValue > 0)
            //{
            //    // incase there is discount in invoice line level
            //    AllowanceCharge allowanceCharge = new AllowanceCharge();
            //    // فى حالة الرسوم incase of charges
            //    // allowanceCharge.ChargeIndicator = true;
            //    // فى حالة الخصم incase of discount
            //    allowanceCharge.ChargeIndicator = false;

            //    allowanceCharge.AllowanceChargeReason = "discount"; // سبب الخصم على مستوى المنتج ???????
            //                                                        // allowanceCharge.AllowanceChargeReasonCode = "90"; // سبب الخصم على مستوى المنتج
            //    allowanceCharge.Amount = item.DiscountValue; // قيم الخصم discount amount or charge amount

            //    allowanceCharge.MultiplierFactorNumeric = 0;
            //    allowanceCharge.BaseAmount = 0;
            //    invline.allowanceCharges.Add(allowanceCharge);
            //}
            inv.InvoiceLines.Add(invline);


            // here you can pass csid data
            string publickey = @"MIIFOTCCBN6gAwIBAgITeAAAf0d/NxPfANthhAABAAB/RzAKBggqhkjOPQQDAjBiMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxEzARBgoJkiaJk/IsZAEZFgNnb3YxFzAVBgoJkiaJk/IsZAEZFgdleHRnYXp0MRswGQYDVQQDExJQRVpFSU5WT0lDRVNDQTQtQ0EwHhcNMjUxMTI2MTMwNTExWhcNMjcxMTI2MTMxNTExWjCBmzELMAkGA1UEBhMCU0ExMjAwBgNVBAoMKdmF2LHZg9iyINmE2YXYs9ipINi02YHYp9ihINmE2YTYrdis2KfZhdipMSIwIAYDVQQLDBnYp9mE2YHYsdi5INin2YTYsdim2YrYs9mKMTQwMgYDVQQDEytUU1QtMzE0MzQwMTI4NjAwMDAzLURFVklDRS0xQ0I0RDhBQjc3Mjk0MUMwMFYwEAYHKoZIzj0CAQYFK4EEAAoDQgAEkVgPjgcrQvzkDhgXV3c7y8NO3sFYwZVFHIYScResd8EbIi9ql5o/gC6wBA7CIt+k7LWAaaYRX7txYUGM2y7hraOCAzowggM2MIGkBgNVHREEgZwwgZmkgZYwgZMxOTA3BgNVBAQMMDEtSGlqYW1hQ2xpbmljfDItMS4wLjB8My1ERVZJQ0UtMUNCNEQ4QUI3NzI5NDFDMDEfMB0GCgmSJomT8ixkAQEMDzMxNDM0MDEyODYwMDAwMzENMAsGA1UEDAwEMDEwMDERMA8GA1UEGgwIVEFEQTg4MzMxEzARBgNVBA8MCkhlYWx0aGNhcmUwHQYDVR0OBBYEFBIztpPIBgPCdvuvEjbrmCY6gPA2MB8GA1UdIwQYMBaAFMfA5rep3RLKTz1taIKWIUXzAXWkMIHlBgNVHR8Egd0wgdowgdeggdSggdGGgc5sZGFwOi8vL0NOPVBFWkVJTlZPSUNFU0NBNC1DQSgxKSxDTj1QUlpFSU5WT0lDRVBLSTQsQ049Q0RQLENOPVB1YmxpYyUyMEtleSUyMFNlcnZpY2VzLENOPVNlcnZpY2VzLENOPUNvbmZpZ3VyYXRpb24sREM9ZXh0emF0Y2EsREM9Z292LERDPWxvY2FsP2NlcnRpZmljYXRlUmV2b2NhdGlvbkxpc3Q/YmFzZT9vYmplY3RDbGFzcz1jUkxEaXN0cmlidXRpb25Qb2ludDCBzgYIKwYBBQUHAQEEgcEwgb4wgbsGCCsGAQUFBzAChoGubGRhcDovLy9DTj1QRVpFSU5WT0lDRVNDQTQtQ0EsQ049QUlBLENOPVB1YmxpYyUyMEtleSUyMFNlcnZpY2VzLENOPVNlcnZpY2VzLENOPUNvbmZpZ3VyYXRpb24sREM9ZXh0emF0Y2EsREM9Z292LERDPWxvY2FsP2NBQ2VydGlmaWNhdGU/YmFzZT9vYmplY3RDbGFzcz1jZXJ0aWZpY2F0aW9uQXV0aG9yaXR5MA4GA1UdDwEB/wQEAwIHgDA8BgkrBgEEAYI3FQcELzAtBiUrBgEEAYI3FQiBhqgdhND7EobtnSSHzvsZ08BVZoGc2C2D5cVdAgFkAgEQMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcDAzAnBgkrBgEEAYI3FQoEGjAYMAoGCCsGAQUFBwMCMAoGCCsGAQUFBwMDMAoGCCqGSM49BAMCA0kAMEYCIQDpTVulcTMYqAHGTCTlpJ06Y6r8ZIxvvP4jbhki4An5QAIhAIQoZs2nUOZg9wvrxp7kSHstUYtthNN5493Jp/cu8X/X";
            string privatekey = @"MHQCAQEEIHRrIH2fjza2hX05zwEgfSKMBEmsBRqnSI1BpJ7unJrvoAcGBSuBBAAKoUQDQgAEkVgPjgcrQvzkDhgXV3c7y8NO3sFYwZVFHIYScResd8EbIi9ql5o/gC6wBA7CIt+k7LWAaaYRX7txYUGM2y7hrQ==";

            inv.cSIDInfo.CertPem = publickey;
            inv.cSIDInfo.PrivateKey = privatekey;
            string secretkey = "lTLnyTbVyA5nabipEt3Nw7wND5r5N5RnVj/UKfBdzME=";
            //استعلام من ZatcaCSID
            try
            {
                //string g=Guid.NewGuid().ToString();
                //if you need to save xml file true if not false;
                bool savexmlfile = false;
                if (rdb_savexml.IsChecked == true)  // اجباري التفعيل من اجل الحفظ - هل من الممكن اخفاءها في الواجهة ؟؟؟
                {
                    savexmlfile = true;
                }
                // this method is used to generate xml file with invoice data 
                //Directory.GetCurrentDirectory() or Directory that you need to save xml file 
                //after call this method xml is created if res.IsValid=true
                res = ubl.GenerateInvoiceXML(inv, Directory.GetCurrentDirectory(), savexmlfile);
                // res.IsValid must equal true to be ready to send to zatca api
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n\n" + ex.InnerException.ToString());
            }
            if (res.IsValid)
            {
                //لو فى اى تحذير عندى قبل ما ارسلة للزكاة والدخل هايظهر فى res.WarningMessage
                string warningmessage = res.WarningMessage;
                //res.SingedXML palintext
                //res.EncodedInvoice base64
                //res.InvoiceHash
                //res.UUID
                //res.QRCode
                //res.TaxAmount
                // here you can save all xml data into database
                //and show QRCode
                //all data you must save is
                //---------------------------------------------------------
                //this is totals from generated xml (you can save into database if you need)  هل بحاجة للحفظ في القاعدة ؟؟؟؟؟؟؟؟؟
                decimal allowanceTotalAmount = 0;
                decimal chargeTotalAmount = 0;
                decimal lineExtensionAmount = 0;
                decimal payableAmount = 0;
                decimal prepaidAmount = 0;
                decimal taxExclusiveAmount = 0;
                decimal taxInclusiveAmount = 0;
                decimal taxAmount = 0;
                decimal.TryParse(res.AllowanceTotalAmount, out allowanceTotalAmount);
                decimal.TryParse(res.ChargeTotalAmount, out chargeTotalAmount);
                decimal.TryParse(res.LineExtensionAmount, out lineExtensionAmount);
                decimal.TryParse(res.PayableAmount, out payableAmount);
                decimal.TryParse(res.PrepaidAmount, out prepaidAmount);
                decimal.TryParse(res.TaxExclusiveAmount, out taxExclusiveAmount);
                decimal.TryParse(res.TaxInclusiveAmount, out taxInclusiveAmount);
                decimal.TryParse(res.TaxAmount, out taxAmount);
                imageBlock = QrCodeImage(res.QRCode);
               // GenerateQrCode(res.QRCode, imageBlock,"")
                //save into db xml table    tbZatcaBill
                //update ZatcaCSID set

                //LastInvoiceHash= res.InvoiceHash
                //InvoiceCounter=inv.AdditionalDocumentReferenceICV.UUID
            }
            else
            {
                //
                MessageBox.Show(res.ErrorMessage);
                return;
            }
         

            //-------------------------------------------------------------------------
            //second step
            //send to ZATCA and save normal and signed xml file and get response from ZATCA
            //Sending modes
            // developer mode (for developers only)
            //foor loop
            if (rdb_simulation.IsChecked == true)
                mode = Mode.Simulation; //simulation mode (for test) تم تثبيت هذه القيمة
            else if (rdb_production.IsChecked == true)
                mode = Mode.Production; //production mode for live
            else
                mode = Mode.developer;

            // zatca call api
            ApiRequestLogic apireqlogic = new ApiRequestLogic(mode, Directory.GetCurrentDirectory(), true);

            InvoiceReportingRequest invrequestbody = new InvoiceReportingRequest();
            invrequestbody.invoice = res.EncodedInvoice; //this is xml file with base 64 format
            invrequestbody.invoiceHash = res.InvoiceHash; // invoicehash of xml
            invrequestbody.uuid = res.UUID;//uuid for xml

            //يرجى حفظ هذة البيانات فى قاعدة البيانات لديكم
            // you must save all this fields in your database
            int status_code = 0;
            string sending_status = "";
            bool is_sent = false;
            string warningmessage1 = "";
            string errormessage = "";
            string clearedinvoice = "";
            string ClearedXMLFileNameFullPath = "";
            string qrcode = "";
            //all this data is required for sending to zatca api
            if (mode==Mode.developer)
            {
                
                    InvoiceReportingResponse responsemodel = apireqlogic.CallComplianceInvoiceAPI(Utility.ToBase64Encode(publickey), secretkey, invrequestbody);
                    if (responsemodel.IsSuccess)
                    {
                        if (responsemodel.StatusCode == 202)
                        {
                            //save warning message in database to solve for next invoices
                            //responsemodel.WarningMessage
                        }
                        MessageBox.Show(responsemodel.ReportingStatus + responsemodel.ClearanceStatus); //REPORTED
                        //string qr = "ATHYudio2K/Yp9mE2YXYrdiz2YYg2LnZhNmKINmG2KfYrNmKINin2YTYtNix2KfYsdmKAg8zMDA0NTQ4MDI1MDAwMDMDEzIwMjQtMTEtMTVUMDA6MDA6MDAEBDQuMDAFBDAuNTIGLFExbnNsSFJSV3F5clZmQ3NjZ1lYb2tQMWw1U3hXUnMwSGVnaG5icmk3ZEE9B2BNRVVDSVFDdUJ2SVgyMDcvanFES01yNG0wcURxRmZ6SEI4WVc2VmlSVXJFRUppTExod0lnYmRBMisvREZRNjd5bHo1VS9YR3VBTUZXdlVFUWhsWHQ0ajkyVncrczd2TT0IWDBWMBAGByqGSM49AgEGBSuBBAAKA0IABCheNDlJ8BXkbLAoa2+qR6Y9AkcjmODyUmwObmslXiKLDpZ9Pm9VMLc+AfB+dfPo6RulPygL593FmG+R7zvKR74JRzBFAiEAlGE9DNv5cORJCCqv4wEcMjX8ieglkTD8/4lft9MBm2sCICy0C3UCcI7/aRimkTJBRrwHf3n7fXVCB2khmNtlf/+g";
                        imageBlock = QrCodeImage(res.QRCode, 200, 200);

                    }
                    else
                    {
                        MessageBox.Show(responsemodel.ErrorMessage);
                    }
               
            }
            else
            {
                //this code is for simulation and production mode

                if (inv.invoiceTypeCode.Name.Substring(0, 2) == "01")
                {
                    // to send standard invoices for clearing
                    //this this the calling of api 
                    //CLearedInvoice is a new xml after clearing it from zatca api with base64 format
                    InvoiceClearanceResponse responsemodel = apireqlogic.CallClearanceAPI(Utility.ToBase64Encode(publickey), secretkey, invrequestbody);
                    //if responsemodel.IsSuccess = true this means that your xml is successfully sent to zatca 
                    status_code = responsemodel.StatusCode;
                    sending_status = responsemodel.ClearanceStatus;
                    is_sent = responsemodel.IsSuccess;
                    if (responsemodel.IsSuccess)
                    {
                        ///////////

                        //if status code =202 it means that xml accepted but with warning 
                        //no need to sent xml again but you must solve that warning messages for the next invoices
                        if (responsemodel.StatusCode == 202)
                        {
                            //save warning message in database to solve for next invoices
                            warningmessage1 = responsemodel.WarningMessage;
                        }

                        // Cleared
                        //MessageBox.Show(responsemodel.QRCode);
                        clearedinvoice = responsemodel.ClearedInvoice;
                        ClearedXMLFileNameFullPath = responsemodel.ClearedXMLFileNameFullPath;
                        qrcode = responsemodel.QRCode;
                        imageBlock = QrCodeImage(responsemodel.QRCode);
                        MessageBox.Show(responsemodel.ClearanceStatus);
                    }
                    else
                    {

                        // if status_code=400 must take new invoice counter and take pih of last created xml 
                        errormessage = responsemodel.ErrorMessage;
                        MessageBox.Show(responsemodel.ErrorMessage);
                    }
                }
                else
                {
                    //to send simplified invoices for reporting
                    //this this the calling of api 
                    InvoiceReportingResponse responsemodel = apireqlogic.CallReportingAPI(Utility.ToBase64Encode(publickey), secretkey, invrequestbody);
                    status_code = responsemodel.StatusCode;
                    sending_status = responsemodel.ReportingStatus; //REPORTED    NOT_REPORTED
                    is_sent = responsemodel.IsSuccess;
                    if (responsemodel.IsSuccess)
                    {
                        //if status code =202 it means that xml accespted but with warning 
                        //no need to sent xml again but you must solve that warning messages for the next invoices
                        if (responsemodel.StatusCode == 202)
                        {
                            //save warning message in database to solve for next invoices
                            warningmessage1 = responsemodel.WarningMessage;
                        }

                       
                        MessageBox.Show(responsemodel.ReportingStatus);// Reported
                    }
                    else
                    {
                        // if status_code=400 must take new invoice counter and take pih of last created xml 
                        errormessage = responsemodel.ErrorMessage;
                        MessageBox.Show(responsemodel.ErrorMessage);
                    }

                }


            }

            //update xml table    tbZatcaBill
            /*
             ZatcaStatus=sending_status
StatusCode=status_code
SendingIsSuccess=is_sent
ClearedXMLFileNameFullPath=ClearedXMLFileNameFullPath
ClearedInvoice=clearedinvoice
ZatcaErrorMessage=errormessage
WarningMessage=warningmessage1
SubmitXMLDate=datetime.now 
             */

        }

        // function 1 for  QrCode phase 2
        public Bitmap QrCodeImage(string Qrcode, int width = 250, int height = 250)
        {

            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height
                }
            };
            Bitmap QrCode = barcodeWriter.Write(Qrcode);

            return QrCode;
        }
        // function 2 for  QrCode phase 2
        public byte[] QrCodeToByteArray(string Qrcode, int width = 250, int height = 250)
        {
            byte[] data;
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height
                }
            };
            Bitmap QrCode = barcodeWriter.Write(Qrcode);
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                QrCode.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                data = stream.ToArray();
            }
            return data;

        }

        //helper function1 for  SendBillToZatc
        private int GetInvoiceType()
        {
            // RadioButton3 Checked == Simplified Invoice
            int InvoiceType = 388;
            return InvoiceType;
        }

        ///helper function2 for  SendBillToZatc
        public string GetInvoiceTypeName()
        {
            // RadioButton3 Checked == Simplified Invoice
            string InvoiceTypeName = "0200000";   
            return InvoiceTypeName;
        }

        //Check if tax value for Saudis in tax Table is found or not
        private void Na_Type1_Click(object sender, RoutedEventArgs e)
        {
            SqlDataReader sdr = tax_adapter.Get_TaxForSaudis();
            if(sdr.HasRows == false)
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

        //Check if discount value for Discount Table is found or not for specefic date
        private void Check_Discount(string date)
        {
            SqlDataReader sdr = discount_adapter.Get_Dis_Date(date);
            if(sdr.HasRows == false)
            {
                //Not found Discount Value for current Date
                DBHandler.ConnectionDB.CloseConnection();
            }
            else
            {
                float dis = 0;
                DataTable dt = new DataTable();
                dt.Load(sdr);
                // Start time & End time are Empty and TotalPricePaid is null => discount for all bills during current date
                if (dt.Rows[0].IsNull("StartTime") && dt.Rows[0].IsNull("TotalPricePaid"))
                {
                    dis = Convert.ToSingle(dt.Rows[0].Field<double>(2));
                    Discount.Text = dis.ToString();
                    MessageBox.Show("يوجد في تاريخ اليوم خصم محدد", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                // Start time & End time are Not Empty and TotalPricePaid is null => discount for all bills during current date && specefic time
                if (!dt.Rows[0].IsNull("StartTime") && dt.Rows[0].IsNull("TotalPricePaid"))
                {
                    DataTable dt_times = new DataTable();
                    dt_times.Columns.Add("Discount_Id", typeof(int));
                    dt_times.Columns.Add("DiscountDate", typeof(DateTime));
                    dt_times.Columns.Add("DiscountRate", typeof(float));
                    dt_times.Columns.Add("StartTime", typeof(TimeSpan));
                    dt_times.Columns.Add("EndTime", typeof(TimeSpan));
                    dt_times.Load(sdr);

                    //Get start and end Times
                    TimeSpan start_time = dt.Rows[0].Field<TimeSpan>(3);
                    TimeSpan end_time  = dt.Rows[0].Field<TimeSpan>(4);
                    //Get Current Time
                    DateTime now = DateTime.Now;
                    TimeSpan current_time = TimeSpan.Parse(now.TimeOfDay.Hours + ":" + now.TimeOfDay.Minutes + ":" + now.TimeOfDay.Seconds);
                    // Compare Between cureent time and others times
                    if(current_time.Equals(start_time) || current_time.Equals(end_time) || (current_time > start_time && current_time < end_time))
                    {
                        dis = Convert.ToSingle(dt.Rows[0].Field<double>(2));
                        Discount.Text = dis.ToString();
                        MessageBox.Show("يوجد في تاريخ اليوم وضمن وقت محدد خصم محدد", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                DBHandler.ConnectionDB.CloseConnection();
            }
        }

        // Function For Calculate Total Price Filed (Not Used Currently .....)
        /*
        private void TotalPriceCalButton_BillExtract_Click(object sender, RoutedEventArgs e)
        {
            float price_add_service = 0, tax = 0, discount = 0;
            //Get PaidChipType RadioButtons
            int chip_type = 0;
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

            if (Price.Text != string.Empty)
            {
                price_add_service = float.Parse(Price.Text);
            }

            if (Tax.Text != string.Empty)
            {
                tax = float.Parse(Tax.Text);
            }

            // Calculate price before discount
            float price_without_dis = chip_type + price_add_service + tax;
            // Check discount value for totalpricepaid 
            float result =  Check_Dis_Totalprice(cureent_date, price_without_dis);
            if (result == 0 && (Discount.Text == string.Empty))
            {
                discount = 0;
            }
            if (result == 0 && (Discount.Text != string.Empty))
            {
                discount = float.Parse(Discount.Text);
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
                    discount = float.Parse(Discount.Text);
                }
                else
                {
                    discount = result;
                }
            }

            // Calculate 
            float total_price = chip_type + price_add_service + tax - discount;
            TotalPaidPrice.Text = total_price.ToString();

        }
        */

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
                    if(dt.Rows[0].IsNull("TotalPricePaid"))
                    {
                        //MessageBox.Show("2");
                        DBHandler.ConnectionDB.CloseConnection();
                        return 0;
                    }
                    else
                    {
                        totalpricepaid = Convert.ToSingle(dt.Rows[0].Field<double>(5));
                        if ( totalprice == totalpricepaid)
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

        private void Get_BillExtractValues()
        {
            SqlDataReader sdr = bill_adapter.Get_BillExtract(PatientsSessionView.s_id);
            if(sdr.HasRows == false)
            {
                DBHandler.ConnectionDB.CloseConnection();
                AddButton_BillExtract.IsEnabled = true;
                TotalBillPriceButton_BillExtract.IsEnabled = true;
                MessageBox.Show("يمكنك ملئ الحقول واصدار فاتورة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            
            }
            else
            {
                //Assignment ll Values from specefic record to this View
                double price = 0, sr_tax = 0,discount = 0, total_p = 0,paid_amount = 0 ,remaining = 0 , paid_cash = 0, paid_network = 0, session_price = 0, paid_value_patient = 0;
                string tax_percentage = "";
                AddButton_BillExtract.IsEnabled = false;
                DataTable dt = new DataTable();
                dt.Load(sdr);
                // Column 1
                string bill_id = dt.Rows[0].Field<int>(0).ToString();
                BillNumField.Text = bill_id;
                // Column 2
                DateTime bill_date = dt.Rows[0].Field<DateTime>(1);
                BillDateField.SelectedDate = bill_date;
                // Column 3
                string chip_type = dt.Rows[0].Field<int>(2).ToString();
                // Currently not used ....
                /*
                if (chip_type.Equals("50"))
                    PaidChip_Type1.IsChecked = true;
                if (chip_type.Equals("100"))
                    PaidChip_Type2.IsChecked = true;
                if (chip_type.Equals("150"))
                    PaidChip_Type3.IsChecked = true;
                if (chip_type.Equals("200"))
                    PaidChip_Type4.IsChecked = true;
                if (chip_type.Equals("250"))
                    PaidChip_Type5.IsChecked = true;
                if (chip_type.Equals("300"))
                    PaidChip_Type6.IsChecked = true;
                if (chip_type.Equals("350"))
                    PaidChip_Type7.IsChecked = true;
                */
                // Column 4
                string add_service = dt.Rows[0].Field<string>(3);
                if (add_service != string.Empty)
                    AddionalServices.Text = add_service;
                // Column 5
                if (!dt.Rows[0].IsNull("PriceOfAdditionalServices"))
                {
                    price= Convert.ToSingle(dt.Rows[0].Field<double>(4));
                    Price.Text = price.ToString();
                   // Price.Text = "30";
                }
                // Column 6
                string nationality_type = dt.Rows[0].Field<string>(5);
                if (nationality_type.Equals("سعودي"))
                    Na_Type1.IsChecked = true;
                if (nationality_type.Equals("غير سعودي"))
                    Na_Type2.IsChecked = true;

                // Column 7 S.R Tax Value
                if (!dt.Rows[0].IsNull("Tax"))
                {
                    sr_tax = Convert.ToSingle(dt.Rows[0].Field<double>(6));
                    Tax_SR.Text = sr_tax.ToString();
                }
                else
                {
                    sr_tax = 0;
                    Tax_SR.Text = sr_tax.ToString();
                }

                // Column 8 % Tax Value
                if (!dt.Rows[0].IsNull("TaxPercentage"))
                {
                    tax_percentage = dt.Rows[0].Field<string>(7);
                    Tax.Text = tax_percentage;
                }
                else
                {
                    tax_percentage = "0%";
                    Tax.Text = tax_percentage;
                }

                // Column 9
                if (!dt.Rows[0].IsNull("Discount"))
                {
                    discount = Convert.ToSingle(dt.Rows[0].Field<double>(8));
                    Discount.Text = discount.ToString();
                }
                else
                {
                    bill_discount = "";
                }
                // Column 10
                string payment_type = dt.Rows[0].Field<string>(9);
                if (payment_type.Equals("نقدي"))
                    Paid_Type1.IsChecked = true;
                if (payment_type.Equals("شبكة "))
                    Paid_Type2.IsChecked = true;
                if (payment_type.Equals("نقدي وشبكة "))
                {
                    Paid_Type3.IsChecked = true;
                    PaymentTogether.Visibility = Visibility.Visible;
                    //Get paid cash Column 13
                    if (!dt.Rows[0].IsNull("PaidCash"))
                    {
                        paid_cash = Convert.ToSingle(dt.Rows[0].Field<double>(12));
                        CashTotalPaidPrice.Text = paid_cash.ToString();
                    }
                    else
                    {
                        paid_cash = 0;
                        CashTotalPaidPrice.Text = paid_cash.ToString();
                    }
                    //Get Paid Network Column 14
                    if (!dt.Rows[0].IsNull("PaidNetwork"))
                    {
                        paid_network = Convert.ToSingle(dt.Rows[0].Field<double>(13));
                        NetworkTotalPaidPrice.Text = paid_network.ToString();
                    }
                    else
                    {
                        paid_network = 0;
                        NetworkTotalPaidPrice.Text = paid_network.ToString();
                    }
                }
                // Column 11
                if (!dt.Rows[0].IsNull("SessionPrice"))
                {
                    //(تعديل جديد)  -  SessionPrice == السعر قبل الضريبة
                    session_price = Convert.ToSingle(dt.Rows[0].Field<double>(10));
                    //(تعديل جديد)  -  bat_SessionPrice == سعر الجلسة
                    bat_SessionPrice.Text = Math.Round((session_price * 1.15) - price,2).ToString();
                    SessionPrice.Text = session_price.ToString();
                }
                // Column 12
                if (!dt.Rows[0].IsNull("TotalPricePaid"))
                {
                    total_p = Convert.ToSingle(dt.Rows[0].Field<double>(11));
                    TotalPaidPrice.Text = total_p.ToString();
                }
                // Column 15
                if (!dt.Rows[0].IsNull("FinallyPaidAmount"))
                {
                    paid_amount = Convert.ToSingle(dt.Rows[0].Field<double>(14));
                    PaidAmount.Text = paid_amount.ToString();
                }
                // Column 16 المبلغ المدفوع من المريض
                if (!dt.Rows[0].IsNull("PaidValue"))
                {
                    paid_value_patient = Convert.ToSingle(dt.Rows[0].Field<double>(15));
                    PaidValue.Text = paid_value_patient.ToString();
                }
                // Column 17  المتبقي لم يسدد من المبلغ الصافي
                if (!dt.Rows[0].IsNull("AmountRemainingUnpaid"))
                {
                    remaining = Convert.ToSingle(dt.Rows[0].Field<double>(16));
                    RemainingUnPaid.Text = remaining.ToString();
                }
                DBHandler.ConnectionDB.CloseConnection();

                //for avoiding any changes on the values of this fields
                SessionPrice.IsReadOnly = true;
                bat_SessionPrice.IsReadOnly = true;
                Tax_SR.IsReadOnly = true;
                TotalPaidPrice.IsReadOnly = true;
                Discount.IsReadOnly = true;
                PaidAmount.IsReadOnly = true;
                PaidValue.IsReadOnly = true;
                RemainingUnPaid.IsReadOnly = true;
            }
        }

        private void PrintButton_BillExtract_Click(object sender, RoutedEventArgs e)
        {
            //Assign 1 to print_iterface for printBill xml 
            //print_interface1 = 1;
            // add is_enabled = false && bill_no not empity 
            if(AddButton_BillExtract.IsEnabled == false && BillNumField.Text != string.Empty)
            {
                p_name = PatientsSessionView.patient_name;
                file_no = PatientFileNumField.Text;
                bill_date = BillDateField.Text;
                dateTime = (DateTime)BillDateField.SelectedDate;
                bill_no = BillNumField.Text;
                sessionNum = SessionNumField.Text;

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

                //(تعديل جديد)  -  SessionPrice == السعر قبل الضريبة
                session_price = SessionPrice.Text;
                totalpaid = TotalPaidPrice.Text;
                paid_amount = PaidAmount.Text;
                paid_value_patient = PaidValue.Text;
                remaining = RemainingUnPaid.Text;
                if(paid_value_patient != string.Empty && remaining != string.Empty)
                {
                    double total = Convert.ToSingle(paid_value_patient);
                    //حقل المتبقي
                    double re = double.Parse(remaining);
                    if (re > total)
                    {
                        MessageBox.Show("فضلاً أدخل قيمة صحيحة في حقل المتبقي لايمكن أن تكون أكبر من المبلغ المدفوع ! ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        RemainingUnPaid.Focus();
                    }
                    else
                    {
                        PrintBill print = new PrintBill();
                        print.Show();
                    }
                }
                else
                {
                    PrintBill print = new PrintBill();
                    print.Show();
                }
            }
            else
            {
                MessageBox.Show("فضلاً أدخل سجل فاتورة قبل معاينة الطابعة ! ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private bool Bill_IsValid()
        {
            if (BillDateField.Text == string.Empty)
            {
                MessageBox.Show("  حقل التاريخ اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                BillDateField.Focus();
                return false;
            }


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

            //(تعديل جديد)  -  bat_SessionPrice == سعر الجلسة 
            if (bat_SessionPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقل قيمة سعر الجلسة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                bat_SessionPrice.Focus();
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

            // if Method of payment is togather => Tow boxes visibility 
            if (Paid_Type3.IsChecked == true && CashTotalPaidPrice.Text == string.Empty && NetworkTotalPaidPrice.Text == string.Empty)
            {
                MessageBox.Show("فضلاً يجب ملئ حقلي قيمة النقدي والشبكة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                CashTotalPaidPrice.Focus();
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

    }
}
