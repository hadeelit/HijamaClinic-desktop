using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for PrintBill.xaml
    /// </summary>
    public partial class PrintBill : Window
    {
        public PrintBill()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PatientName.Text = BillExtractWindow.p_name;
            FileNo.Text = BillExtractWindow.file_no;
            DateBill.Text = BillExtractWindow.bill_date;
            BillNo.Text = BillExtractWindow.bill_no;
            string SessionNum = BillExtractWindow.sessionNum;
            string CompanyName = "مركز لمسة شفاء للحجامة";
            string TaxNumber = "314340128600003";
            DateTime dateTime = BillExtractWindow.dateTime;
            DateTime time = DateTime.Now;
            string QrDateTime = dateTime.ToString("yyyy-MM-dd") + "T" + time.ToString("HH:mm:ss") + "Z";
            string Filename_QrImage = FileNo.Text + SessionNum + BillNo.Text ;
           
            //percentage tax value
            if (BillExtractWindow.bill_tax != string.Empty)
                Vat_Value.Text = BillExtractWindow.bill_tax;
            else
                Vat_Value.Text = "0%";
            //SR tax value
            if (BillExtractWindow.bill_tax_SR != string.Empty)
                SRTaxBill.Text = BillExtractWindow.bill_tax_SR;
            else
                Vat_Value.Text = "0";

            if (BillExtractWindow.bill_discount != string.Empty)
                DiscountBill.Text = BillExtractWindow.bill_discount;
            else
                DiscountBill.Text = "0";

            if (BillExtractWindow.bill_paid_type.Equals("نقدي"))
            {
                Paid_Type.Text = "نقدي";
            }
            if (BillExtractWindow.bill_paid_type.Equals("شبكة"))
            {
                Paid_Type.Text = "شبكة";
            }
            if (BillExtractWindow.bill_paid_type.Equals("نقدي وشبكة"))
            {
                Paid_Type.Text = "نقدي وشبكة";
                //cash paid
                if (BillExtractWindow.cash_paid != string.Empty)
                {
                    cash_value_lable.Visibility = Visibility.Visible;
                    cash_value.Visibility = Visibility.Visible;
                    cash_value.Text = BillExtractWindow.cash_paid;
                }

                //network paid
                if (BillExtractWindow.network_paid != string.Empty)
                {
                    network_value_lable.Visibility = Visibility.Visible;
                    network_value.Visibility = Visibility.Visible;
                    network_value.Text = BillExtractWindow.network_paid;
                }
            }

            session_price.Text = BillExtractWindow.session_price;
            TotalTaxBill.Text = BillExtractWindow.totalpaid;
            PaidAmount.Text = BillExtractWindow.paid_amount;
            paid.Text = BillExtractWindow.paid_value_patient;
            Residu.Text = BillExtractWindow.remaining;
            // Residu.Text = BillExtractWindow.residu;

            // Generate Qr image compatible with Zatca ksa (Phase 1)
            //string textCode = Zatca_Bill.SaudiZatca.getBase64(CompanyName, TaxNumber, QrDateTime, TotalTaxBill.Text, SRTaxBill.Text);
            //Zatca_Bill.SaudiZatca.GenerateQrCode(textCode, QR_CodeVat, Filename_QrImage);

            // Generate Qr image compatible with Zatca ksa (Phase 2)
            QR_CodeVat.Source = BitmapToBitmapImage(BillExtractWindow.imageBlock);
        }

        //Function for trnaslate from  Bitmap To BitmapImage
        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null) return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void PrintBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog print = new PrintDialog();
                if (print.ShowDialog() == true)
                {
                    print.PrintVisual(Print, "فاتورة");
                }
                else
                {
                    MessageBox.Show("تم الغاء عملية الطباعة  ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            finally
            {
                this.IsEnabled = true;
            }          
        }

        private void CloseIcon_Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
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
    }
}
