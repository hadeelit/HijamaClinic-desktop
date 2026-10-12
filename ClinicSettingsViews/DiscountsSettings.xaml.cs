using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace HijamaClinicManagement.ClinicSettingsViews
{
    /// <summary>
    /// Interaction logic for DiscountsSettings.xaml
    /// </summary>
    public partial class DiscountsSettings : UserControl
    {
        // Intialize General Variables
        DBHandler.DiscountsAdapter dis_adapter = new DBHandler.DiscountsAdapter();
        private bool result;
        private int sp;

        public DiscountsSettings()
        {
            InitializeComponent();
        }

        //Loaded UserControl 
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ClearDiscounts();
            DefaultData();
            LoadDiscountGridViw();
        }

        //Finctions For Add + Delete + Edit (DiscountsSettings)
        private void AddButton_DiscountsSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DiscountsIsValid())
            {
                // Check if date already exist in Discount table or not
                SqlDataReader sdr = dis_adapter.Get_Dis_Date(DiscountDateField.Text);
                // Not Found Any Records
                if (sdr.HasRows == false)
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    // CheckBox SpeceficPerson 
                    if (SpeceficPerson.IsChecked == true)
                        sp = 1;
                    else
                        sp = 0;
                    result = dis_adapter.Discount_Insert(DiscountDateField.Text, DiscountRateField.Text, (stime.SelectedTime).ToString(), (etime.SelectedTime).ToString(), TotalPaidP.Text, NumOfSessions.Text, sp);

                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        ClearDiscounts();
                        DefaultData();
                        LoadDiscountGridViw();
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        ClearDiscounts();
                    }
                }
                else
                {
                    DBHandler.ConnectionDB.CloseConnection();
                    MessageBox.Show("ادخل تاريخ اخر حتى تكتمل عملية الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                }

            }
        }

        //Load Default Data in fields when Load Window
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            DiscountDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        private void DeleteButton_DiscountsSettings_Click(object sender, RoutedEventArgs e)
        {
            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromDiscountTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (DiscountRateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }

                else
                {
                    // Step 1 : Fetch Info For  Specefic cell
                    var cell0 = DataViewFromDiscountTable.SelectedCells[0];
                    // Step 2: get the content for this info Cell (Discount_id Cell)
                    string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                    // MessageBox For Yes No Deleted
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        // Call Delete_Discount Function For delete this record
                        bool result = dis_adapter.Delete_Discount(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadDiscountGridViw();
                            ClearDiscounts();
                            DefaultData();
                            dis_adapter.Edit_Identity();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                        MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }

            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        private void EdieButton_DiscountsSettings_Click(object sender, RoutedEventArgs e)
        {
            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromDiscountTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (DiscountRateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("حقل نسبة التخفيض اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DiscountRateField.Focus();
                }
                else
                {
                    if (DiscountsIsValid())
                    {
                        // Step 1 : Fetch Info For  Specefic cell
                        var cell0 = DataViewFromDiscountTable.SelectedCells[0];
                        // Step 2: get the content for this info Cell (Discount_id Cell)
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                        // MessageBox For Yes No Updated
                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            // CheckBox SpeceficPerson 
                            if (SpeceficPerson.IsChecked == true)
                                sp = 1;
                            else
                                sp = 0;
                            // Call Update_Discounts()
                            bool result = dis_adapter.Update_Discount(id, DiscountDateField.Text, DiscountRateField.Text, (stime.SelectedTime).ToString(), (etime.SelectedTime).ToString(), TotalPaidP.Text, NumOfSessions.Text, sp);
                            if (result)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                LoadDiscountGridViw();
                                ClearDiscounts();
                                DefaultData();
                            }
                            else
                            {
                                MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
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

        private void ClearButton_DiscountsSettings_Click(object sender, RoutedEventArgs e)
        {
            ClearDiscounts();
            DefaultData();
        }

        //Load DataGrid from Discounts Table In DB when Load View
        private void LoadDiscountGridViw()
        {

            //Fetch Data from Discounts Table
            SqlDataReader sdr = dis_adapter.Discounts_Get();
            DataTable dt = new DataTable("Discounts");
            dt.Load(sdr);
            DataViewFromDiscountTable.ItemsSource = dt.DefaultView;
            DBHandler.ConnectionDB.CloseConnection();

        }
        // Get Data From GridView To Fields When Click on Row              
        private void DataViewDiscount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // All Fields in Discounts Table
            var cell1 = DataViewFromDiscountTable.SelectedCells[1];
            var cell2 = DataViewFromDiscountTable.SelectedCells[2];
            var cell3 = DataViewFromDiscountTable.SelectedCells[3];
            var cell4 = DataViewFromDiscountTable.SelectedCells[4];
            var cell5 = DataViewFromDiscountTable.SelectedCells[5];
            var cell6 = DataViewFromDiscountTable.SelectedCells[6];
            var cell7 = DataViewFromDiscountTable.SelectedCells[7];

            string date = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string rate = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string start_t = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string end_t = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
            string total_p = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
            string num_s = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;
            string specefic_p = (cell7.Column.GetCellContent(cell7.Item) as TextBlock).Text;

            // For Clear previous Data in Fields When Clicked
            ClearDiscounts();
            DiscountDateField.SelectedDate = DateTime.Parse(date);
            DiscountRateField.Text = rate;
            if (start_t != "" && end_t != "")
            {
                stime.SelectedTime = DateTime.Parse(start_t);
                etime.SelectedTime = DateTime.Parse(end_t);
            }
            if (total_p != "")
                TotalPaidP.Text = total_p;
            if (num_s != "")
                NumOfSessions.Text = num_s;
            if (int.Parse(specefic_p) != 0)
                SpeceficPerson.IsChecked = true;
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

        // Verify Discount Fields
        private bool DiscountsIsValid()
        {

            if (DiscountDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DiscountDateField.Focus();
                return false;
            }
            if (DiscountRateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                DiscountRateField.Focus();
                return false;
            }

            if ((stime.SelectedTime == null && etime.SelectedTime != null) || (stime.SelectedTime != null && etime.SelectedTime == null))
            {
                MessageBox.Show("يجب ادخال حقلي الوقت معاً", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;

            }

            return true;
        }

        //Finctions For Clear Data from Fields ...
        //Clear Data from Discounts View
        private void ClearDiscounts()
        {
            DiscountDateField.SelectedDate = null;
            DiscountRateField.Clear();
            stime.SelectedTime = null;
            etime.SelectedTime = null;
            TotalPaidP.Clear();
            NumOfSessions.Clear();
            SpeceficPerson.IsChecked = false;

        }

       
    }
}
